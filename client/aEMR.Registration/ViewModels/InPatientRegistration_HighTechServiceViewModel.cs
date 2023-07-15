using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using Service.Core.Common;
using aEMR.Common;
using aEMR.Common.Converters;
using eHCMSLanguage;
using Castle.Windsor;
/*
* 20170420 #001 CMN: Fixed Used date of Issue Bills out of admit date for HT Patient
*/
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IInPatientRegistration_HighTechService)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientRegistration_HighTechServiceViewModel : Conductor<object>, IInPatientRegistration_HighTechService
            , IHandle<ResultFound<Patient>>
            , IHandle<ItemSelected<Patient>>
            , IHandle<ItemSelected<PatientRegistration>>
            , IHandle<ResultNotFound<Patient>>
            , IHandle<PayForRegistrationCompleted>
            , IHandle<SaveAndPayForRegistrationCompleted>
            , IHandle<ItemSelected<RefMedicalServiceItem>>
            , IHandle<ItemSelected<PCLExamType>>
            , IHandle<ItemSelected<RefGenMedProductDetails>>
            , IHandle<RemoveItem<MedRegItemBase>>
            , IHandle<EditItem<InPatientBillingInvoice>>
            , IHandle<ItemRemoved<MedRegItemBase, IInPatientBillingInvoiceDetailsListing>>
            , IHandle<ItemChanged<MedRegItemBase, IInPatientBillingInvoiceDetailsListing>>
            , IHandle<DoubleClick>
            , IHandle<DoubleClickAddReqLAB>
            , IHandle<InPatientReturnMedProduct>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public InPatientRegistration_HighTechServiceViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();

            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            searchPatientAndRegVm.PatientFindByVisibility = false;
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            searchPatientAndRegVm.mTimBN = mDangKyNoiTru_Patient_TimBN;
            searchPatientAndRegVm.mThemBN = mDangKyNoiTru_Patient_ThemBN;
            searchPatientAndRegVm.mTimDangKy = mDangKyNoiTru_Patient_TimDangKy;

            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
            patientInfoVm.mInfo_CapNhatThongTinBN = mDangKyNoiTru_Info_CapNhatThongTinBN;
            patientInfoVm.mInfo_XacNhan = mDangKyNoiTru_Info_XacNhan;
            patientInfoVm.mInfo_XoaThe = mDangKyNoiTru_Info_XoaThe;
            
            //patientInfoVm.mInfo_XemPhongKham = mDangKyNoiTru_Info_XemPhongKham;
            patientInfoVm.mInfo_XemPhongKham = false;

            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);
            PatientSummaryInfoContent.CanConfirmHi = false;
            PatientSummaryInfoContent.DisplayButtons = false;

            var selectServiceVm = Globals.GetViewModel<IInPatientSelectService>();
            InPatientSelectServiceContent = selectServiceVm;
            ActivateItem(selectServiceVm);

            var selectPclVm = Globals.GetViewModel<IInPatientSelectPcl>();
            SelectPCLContent = selectPclVm;
            ActivateItem(selectPclVm);

            //LAB
            var selectPclVmLAB = Globals.GetViewModel<IInPatientSelectPclLAB>();
            SelectPCLContentLAB = selectPclVmLAB;
            ActivateItem(selectPclVmLAB);
            //LAB


            var newBillingVm = Globals.GetViewModel<IInPatientBillingInvoiceDetailsListing>();
            EditingInvoiceDetailsContent = newBillingVm;
            ActivateItem(newBillingVm);

            //KMx: Chuyển từ View IInPatientBillingInvoiceListing -> IInPatientBillingInvoiceListingNew (13/09/2014 16:54).
            //var oldBillingVm = Globals.GetViewModel<IInPatientBillingInvoiceListing>();
            var oldBillingVm = Globals.GetViewModel<IInPatientBillingInvoiceListingNew>();
            OldBillingInvoiceContent = oldBillingVm;
            OldBillingInvoiceContent.mDangKyNoiTru_SuaDV = mDangKyNoiTru_SuaDV;
            OldBillingInvoiceContent.mDangKyNoiTru_XemChiTiet = mDangKyNoiTru_XemChiTiet;
            OldBillingInvoiceContent.ShowRecalcHiColumn = false;
            OldBillingInvoiceContent.ShowRecalcHiWithPriceListColumn = false;
            OldBillingInvoiceContent.ShowPrintBillColumn = true;

            ActivateItem(oldBillingVm);

            var drugsVm = Globals.GetViewModel<IDrugListing>();
            SelectDrugContent = drugsVm;
            ActivateItem(drugsVm);

            var medItemVm = Globals.GetViewModel<IMedItemListing>();
            MedItemContent = medItemVm;
            ActivateItem(medItemVm);

            var chemicalVm = Globals.GetViewModel<IChemicalListing>();
            ChemicalItemContent = chemicalVm;
            ActivateItem(chemicalVm);

            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            if (Globals.isAccountCheck)
            {
                DepartmentContent.AddSelectOneItem = true;
            }
            else
            {
                DepartmentContent.AddSelectedAllItem = true;
            }
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                InPatientSelectServiceContent.GetServiceTypes();

            }
            //KMx: Cấu hình của view này sai rồi, khi nào có thời gian thì làm cấu hình lại.
            //Lưu ý: View này có 2 link (Tạo bill, tạo bill tài vụ), phải check operation ở LeftMenu rồi truyền vào, không phải check trong view này (07/01/2015).
            //authorization();

            // TxD 02/08/2014 Use Global Server Date instead
            //Coroutine.BeginExecute(GetDateTimeFromServer());
            SelectedDate = Globals.GetCurServerDateTime().Date;
            
            (DepartmentContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(InPatientRegistrationViewModel_PropertyChanged);

            EditingBillingInvoice = new InPatientBillingInvoice();
        }

        private DeptLocation _SelLocationInDept = null;
        public DeptLocation SelLocationInDept
        {
            get
            {
                return _SelLocationInDept;
            }
            set
            {
                _SelLocationInDept = value;
                NotifyOfPropertyChange(() => SelLocationInDept);                
            }
        }

        private ObservableCollection<DeptLocation> _LocationsInDept;
        public ObservableCollection<DeptLocation> LocationsInDept
        {
            get
            {
                return _LocationsInDept;
            }
            set
            {
                _LocationsInDept = value;
                NotifyOfPropertyChange(() => LocationsInDept);
            }
        }

        public void LoadLocations(long? deptId)
        {
            this.ShowBusyIndicator();

            var list = new List<refModule>();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllLocationsByDeptIDOld(deptId, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allItems = contract.EndGetAllLocationsByDeptIDOld(asyncResult);

                            if (allItems != null)
                            {
                                LocationsInDept = new ObservableCollection<DeptLocation>(allItems);
                            }
                            else
                            {
                                LocationsInDept = new ObservableCollection<DeptLocation>();
                            }

                            var itemDefault = new DeptLocation();
                            itemDefault.DeptID = -1;
                            itemDefault.Location = new Location();
                            itemDefault.Location.LID = -1;
                            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
                            LocationsInDept.Insert(0, itemDefault);

                            SelLocationInDept = itemDefault;
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        private bool _UsedByTaiVuOffice = false;
        public bool UsedByTaiVuOffice
        {
            get { return _UsedByTaiVuOffice; }
            set
            {
                _UsedByTaiVuOffice = value;
                if (_UsedByTaiVuOffice)
                {
                    SearchRegistrationContent.CanSearhRegAllDept = true;
                }
                else
                {
                    SearchRegistrationContent.CanSearhRegAllDept = false;
                }

                InitSelDeptCombo();
            }
        }

        // TxD 06/01/2015: Added the following to allow TaiVu Office to create bill for patients of all departments
        private void InitSelDeptCombo()
        {
            DepartmentContent.LstRefDepartment = new ObservableCollection<long>();

            if (UsedByTaiVuOffice)
            {                
                foreach (var itemDept in Globals.AllRefDepartmentList)
                {
                    DepartmentContent.LstRefDepartment.Add(itemDept.DeptID);
                }
                DepartmentContent.AddSelectOneItem = false;
                DepartmentContent.AddSelectedAllItem = true;
            }
            else
            {
                if (Globals.LoggedUserAccount.DeptIDResponsibilityList.Count() > 0)
                {                    
                    if (Globals.AddDrugDeptToListRespDepts && Globals.DrugDeptID > 0)
                    {                        
                        Globals.LoggedUserAccount.DeptIDResponsibilityList.Add(Globals.DrugDeptID);
                    }
                }

                DepartmentContent.LstRefDepartment = Globals.LoggedUserAccount.DeptIDResponsibilityList;

            }

            DepartmentContent.LoadData();
        }

        private long? DeptID = 0;
        void InPatientRegistrationViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                SetConditionWhenChangeSelectedItem();

                if (CurRegistration != null && CurRegistration.PtRegistrationID > 0)
                {
                    GetAllInPatientBillingInvoices();
                }
                if (DepartmentContent.SelectedItem != null && DepartmentContent.SelectedItem.DeptID > 0)
                {
                    LoadLocations(DepartmentContent.SelectedItem.DeptID);
                }
            }
        }

        private void SetConditionWhenChangeSelectedItem()
        {
            if (EditingBillingInvoice != null && DepartmentContent != null)
            {
                EditingBillingInvoice.Department = DepartmentContent.SelectedItem;
                if (DepartmentContent.SelectedItem != null && DepartmentContent.SelectedItem.DeptID > 0)
                {
                    EditingBillingInvoice.DeptID = DepartmentContent.SelectedItem.DeptID;
                    Coroutine.BeginExecute(DoGetStore_EXTERNAL(EditingBillingInvoice.DeptID));
                    DeptID = DepartmentContent.SelectedItem.DeptID;
                }
                else
                {
                    DeptID = 0;
                    if (WarehouseList != null)
                    {
                        WarehouseList.Clear();
                    }
                }
                CreateNewBillCmd();
            }

            //KMx: Dời ra ngoài hàm InPatientRegistrationViewModel_PropertyChanged(). Vì hàm ShowOldRegistration() không cần load bill cũ (đã load rồi) (23/08/2014 15:53).
            //if (CurRegistration != null && CurRegistration.PtRegistrationID > 0)
            //{
            //    GetAllInPatientBillingInvoices();
            //}
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


        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadPatientClassifications();
            }
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private ISearchPatientAndRegistration _searchRegistrationContent;

        public ISearchPatientAndRegistration SearchRegistrationContent
        {
            get { return _searchRegistrationContent; }
            set
            {
                _searchRegistrationContent = value;
                NotifyOfPropertyChange(() => SearchRegistrationContent);
            }
        }

        private IPatientSummaryInfoV2 _patientSummaryInfoContent;

        public IPatientSummaryInfoV2 PatientSummaryInfoContent
        {
            get { return _patientSummaryInfoContent; }
            set
            {
                _patientSummaryInfoContent = value;
                NotifyOfPropertyChange(() => PatientSummaryInfoContent);
            }
        }

        private IInPatientSelectService _inPatientSelectServiceContent;
        public IInPatientSelectService InPatientSelectServiceContent
        {
            get { return _inPatientSelectServiceContent; }
            set
            {
                _inPatientSelectServiceContent = value;
                NotifyOfPropertyChange(() => InPatientSelectServiceContent);
            }
        }

        private IInPatientSelectPcl _selectPCLContent;
        public IInPatientSelectPcl SelectPCLContent
        {
            get { return _selectPCLContent; }
            set
            {
                _selectPCLContent = value;
                NotifyOfPropertyChange(() => SelectPCLContent);
            }
        }

        private IInPatientSelectPclLAB _selectPCLContentLAB;
        public IInPatientSelectPclLAB SelectPCLContentLAB
        {
            get { return _selectPCLContentLAB; }
            set
            {
                _selectPCLContentLAB = value;
                NotifyOfPropertyChange(() => SelectPCLContentLAB);
            }
        }

        private string _DeptLocTitle;
        public string DeptLocTitle
        {
            get
            {
                return _DeptLocTitle;
            }
            set
            {
                _DeptLocTitle = value;
                NotifyOfPropertyChange(() => DeptLocTitle);
            }
        }

        private bool _isChangeDept = true;
        public bool isChangeDept
        {
            get { return _isChangeDept; }
            set
            {
                _isChangeDept = value;
                NotifyOfPropertyChange(() => isChangeDept);
            }
        }

        private bool _isDischarged;
        public bool isDischarged
        {
            get { return _isDischarged; }
            set
            {
                _isDischarged = value;
                NotifyOfPropertyChange(() => isDischarged);
                isChangeDept = !isDischarged;
            }
        }

        private IDrugListing _selectDrugContent;
        public IDrugListing SelectDrugContent
        {
            get { return _selectDrugContent; }
            set
            {
                _selectDrugContent = value;
                NotifyOfPropertyChange(() => SelectDrugContent);
            }
        }
        private IMedItemListing _medItemContent;
        public IMedItemListing MedItemContent
        {
            get { return _medItemContent; }
            set
            {
                _medItemContent = value;
                NotifyOfPropertyChange(() => MedItemContent);
            }
        }

        private IChemicalListing _chemicalItemContent;
        public IChemicalListing ChemicalItemContent
        {
            get { return _chemicalItemContent; }
            set
            {
                _chemicalItemContent = value;
                NotifyOfPropertyChange(() => ChemicalItemContent);
            }
        }


        private bool _isEditing;
        public bool IsEditing
        {
            get { return _isEditing; }
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    NotifyOfPropertyChange(() => IsEditing);
                    NotifyOfPropertyChange(() => CanStartEditRegistrationCmd);
                    NotifyOfPropertyChange(() => CanRegister);

                    EditingInvoiceDetailsContent.CanDelete = _isEditing;
                    EditingInvoiceDetailsContent.CanEditOnGrid = _isEditing || IsBillOfUsedItems;
                }
            }
        }

        public string EditingBillingInvoiceTitle
        {
            get
            {
                if (_editingBillingInvoice == null)
                {
                    return string.Empty;
                }
                if (_editingBillingInvoice.InPatientBillingInvID > 0)
                {
                    return string.Format("{0} ", eHCMSResources.Z0152_G1_CNhatBill) + _editingBillingInvoice.BillingInvNum;
                }
                else
                {
                    return eHCMSResources.Z0014_G1_ThemBillMoi;
                }
            }
        }

        private IInPatientBillingInvoiceDetailsListing _editingInvoiceDetailsContent;
        public IInPatientBillingInvoiceDetailsListing EditingInvoiceDetailsContent
        {
            get { return _editingInvoiceDetailsContent; }
            set
            {
                _editingInvoiceDetailsContent = value;
                NotifyOfPropertyChange(() => EditingInvoiceDetailsContent);
            }
        }

        //private IInPatientBillingInvoiceListing _oldBillingInvoiceContent;
        //public IInPatientBillingInvoiceListing OldBillingInvoiceContent
        //{
        //    get { return _oldBillingInvoiceContent; }
        //    set
        //    {
        //        _oldBillingInvoiceContent = value;
        //        NotifyOfPropertyChange(() => OldBillingInvoiceContent);
        //    }
        //}

        private IInPatientBillingInvoiceListingNew _oldBillingInvoiceContent;
        public IInPatientBillingInvoiceListingNew OldBillingInvoiceContent
        {
            get { return _oldBillingInvoiceContent; }
            set
            {
                _oldBillingInvoiceContent = value;
                NotifyOfPropertyChange(() => OldBillingInvoiceContent);
            }
        }

        public bool MedRegItemConfirmed
        {
            get
            {
                //Hiện tại là vậy.
                return true;
            }
        }
        private InPatientBillingInvoice _tempBillingInvoice;
        private InPatientBillingInvoice _editingBillingInvoice;
        public InPatientBillingInvoice EditingBillingInvoice
        {
            get
            {
                return _editingBillingInvoice;
            }
            set
            {
                if (_editingBillingInvoice != value)
                {
                    _editingBillingInvoice = value;
                    NotifyOfPropertyChange(() => EditingBillingInvoice);
                    NotifyOfPropertyChange(() => EditingBillingInvoiceTitle);
                    EditingInvoiceDetailsContent.BillingInvoice = _editingBillingInvoice;
                    EditingInvoiceDetailsContent.ResetView();

                    NotifyOfPropertyChange(() => CanStartEditRegistrationCmd);
                }
            }
        }

        private ObservableCollection<PatientClassification> _patientClassifications;

        public ObservableCollection<PatientClassification> PatientClassifications
        {
            get { return _patientClassifications; }
            set
            {
                _patientClassifications = value;
                NotifyOfPropertyChange(() => PatientClassifications);
            }
        }

        private bool _calcPaymentToEndOfDay;

        public bool CalcPaymentToEndOfDay
        {
            get { return _calcPaymentToEndOfDay; }
            set
            {
                _calcPaymentToEndOfDay = value;
                NotifyOfPropertyChange(() => CalcPaymentToEndOfDay);
            }
        }
        private bool _canCalcPaymentToEndOfDay;

        public bool CanCalcPaymentToEndOfDay
        {
            get { return _canCalcPaymentToEndOfDay; }
            set
            {
                _canCalcPaymentToEndOfDay = value;
                NotifyOfPropertyChange(() => CanCalcPaymentToEndOfDay);
                if (!_canCalcPaymentToEndOfDay)
                {
                    CalcPaymentToEndOfDay = false;
                }
            }
        }

        //KMx: Cấu hình có cho phép thêm Thuốc, Y Cụ, Hóa Chất vào bill hay không.
        //Hiện tại thì không cho add trực tiếp vào bill. Muốn chỉnh sửa thì qua kho nội trú chỉnh sửa.
        //Lý do: 1. Khi tạo bill mà thêm thuốc, y cụ, hóa chất thì sẽ tạo ra 1 phiếu xuất giống bên kho nội trú => không phân biệt được.
        //       2. Khi tạo bill mà xóa 1 phiếu xuất: Phiếu xuất chỉ xóa khỏi bill, nhưng vẫn còn tồn tại trong DB. Người dùng nghĩ rằng phiếu đó đã bị xóa khỏi DB => Báo cáo sai.
        //private bool _addMedProductToBillDirectly;

        public bool AddMedProductToBillDirectly
        {
            get { return Globals.ServerConfigSection.InRegisElements.AddMedProductToBillDirectly; }
        }

        /// <summary>
        /// Neu nguoi dung chon benh nhan, dang ky. Va dang ky hop le thi moi set bien nay true.
        /// </summary>
        public bool CanAddEditBill
        {
            // Hpt 22/10/2015: Anh tuấn nói không khóa cứng màn hình để người dùng còn xem lại bill cũ của bệnh nhân đã xuất viện hoặc các đăng ký đã đóng... (theo yêu cầu bv)
            get
            {
                return (CurRegistration != null && CurRegistration.PtRegistrationID > 0 && CurRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU);
            }          
        }

        private ObservableCollection<RefStorageWarehouseLocation> _warehouseList;
        public ObservableCollection<RefStorageWarehouseLocation> WarehouseList
        {
            get
            {
                return _warehouseList;
            }
            set
            {
                _warehouseList = value;
                NotifyOfPropertyChange(() => WarehouseList);
            }
        }

        private RefStorageWarehouseLocation _selectedWarehouse;
        public RefStorageWarehouseLocation SelectedWarehouse
        {
            get
            {
                return _selectedWarehouse;
            }
            set
            {
                _selectedWarehouse = value;
                NotifyOfPropertyChange(() => SelectedWarehouse);

                SelectDrugContent.SearchCriteria.Storage = _selectedWarehouse;
                SelectDrugContent.Clear();
                ChemicalItemContent.SearchCriteria.Storage = _selectedWarehouse;
                ChemicalItemContent.Clear();
                MedItemContent.SearchCriteria.Storage = _selectedWarehouse;
                MedItemContent.Clear();
            }
        }

        private bool _registrationLoading = false;
        /// <summary>
        /// Dang trong qua trinh lay thong tin dang ky tu server.
        /// </summary>
        public bool RegistrationLoading
        {
            get
            {
                return _registrationLoading;
            }
            set
            {
                _registrationLoading = value;
                NotifyOfPropertyChange(() => RegistrationLoading);

                NotifyWhenBusy();
            }
        }
        private bool _patientLoading = false;
        /// <summary>
        /// Dang trong qua trinh lay thong tin benh nhan tu server.
        /// </summary>
        public bool PatientLoading
        {
            get
            {
                return _patientLoading;
            }
            set
            {
                _patientLoading = value;
                NotifyOfPropertyChange(() => PatientLoading);

                NotifyWhenBusy();
            }
        }

        private bool _patientSearching = false;
        /// <summary>
        /// Dang trong qua trinh tim kiem benh nhan co hay khong.
        /// </summary>
        public bool PatientSearching
        {
            get
            {
                return _patientSearching;
            }
            set
            {
                _patientSearching = value;
                NotifyOfPropertyChange(() => PatientSearching);

                NotifyWhenBusy();
            }
        }
        private void NotifyWhenBusy()
        {
            NotifyOfPropertyChange(() => IsProcessing);
        }

        public string StatusText
        {
            get
            {
                if (_isSaving)
                {
                    return eHCMSResources.Z0172_G1_DangLuuDLieu;
                }
                if (_patientLoading)
                {
                    return eHCMSResources.Z0119_G1_DangLayTTinBN;
                }
                if (_registrationLoading)
                {
                    return eHCMSResources.Z0086_G1_DangLayTTinDK;
                }
                if (_isLoadingBill)
                {
                    return eHCMSResources.Z0151_G1_DangLayTTinBill;
                }
                return eHCMSResources.Z0153_G1_Pleasewait;
            }
        }
        public bool IsProcessing
        {
            get
            {
                return _patientLoading || _isSaving || _registrationLoading;
            }
        }

        private bool _isSaving;
        public bool IsSaving
        {
            get
            {
                return _isSaving;
            }
            set
            {
                _isSaving = value;
                NotifyOfPropertyChange(() => IsSaving);

                NotifyWhenBusy();
            }
        }

        private bool _isLoadingBill;
        public bool IsLoadingBill
        {
            get
            {
                return _isLoadingBill;
            }
            set
            {
                _isLoadingBill = value;
                NotifyOfPropertyChange(() => IsLoadingBill);

                NotifyWhenBusy();
            }
        }

        //private bool _canChangePatientType = false;
        //public bool CanChangePatientType
        //{
        //    get
        //    {
        //        return _canChangePatientType;
        //    }
        //    set
        //    {
        //        if (_canChangePatientType != value)
        //        {
        //            _canChangePatientType = value;
        //            NotifyOfPropertyChange(() => CanChangePatientType);
        //        }
        //    }
        //}

        private bool _canSaveRegistrationAndPay;
        public bool CanSaveRegistrationAndPay
        {
            get
            {
                return _canSaveRegistrationAndPay;
            }
            set
            {
                if (_canSaveRegistrationAndPay != value)
                {
                    _canSaveRegistrationAndPay = value;
                    NotifyOfPropertyChange(() => CanSaveRegistrationAndPay);
                }
            }
        }
        private bool _canSaveRegistration;
        public bool CanSaveRegistration
        {
            get
            {
                return _canSaveRegistration;
            }
            set
            {
                if (_canSaveRegistration != value)
                {
                    _canSaveRegistration = value;
                    NotifyOfPropertyChange(() => CanSaveRegistration);
                }
            }
        }

        private bool _canSearchPatient = true;
        public bool CanSearchPatient
        {
            get
            {
                return _canSearchPatient;
            }
            set
            {
                if (_canSearchPatient != value)
                {
                    _canSearchPatient = value;
                    NotifyOfPropertyChange(() => CanSearchPatient);
                }
            }
        }
        private bool _registrationInfoHasChanged;
        /// <summary>
        /// Cho biet thong tin dang ky tren form da duoc thay doi chua.
        /// </summary>
        public bool RegistrationInfoHasChanged
        {
            get
            {
                return _registrationInfoHasChanged;
            }
            set
            {
                if (_registrationInfoHasChanged != value)
                {
                    _registrationInfoHasChanged = value;
                    NotifyOfPropertyChange(() => RegistrationInfoHasChanged);

                    //ApplicationViewModel.Instance.IsProcessing = _registrationInfoHasChanged;
                    CanSearchPatient = !_registrationInfoHasChanged;

                    NotifyOfPropertyChange(() => CanCancelChangesCmd);
                    NotifyOfPropertyChange(() => CanSaveBillingInvoiceCmd);
                    NotifyOfPropertyChange(() => CanCreateNewBillCmd);
                    NotifyOfPropertyChange(() => CanStartEditRegistrationCmd);
                    //NotifyOfPropertyChange(() => CanCreateBillingInvoiceFromExistingItemsCmd);
                    NotifyOfPropertyChange(() => CanLoadBillCmd);
                }
            }
        }


        private PatientClassification _curClassification;
        public PatientClassification CurClassification
        {
            get
            {
                return _curClassification;
            }
            set
            {
                if (_curClassification != value)
                {
                    _curClassification = value;
                    NotifyOfPropertyChange(() => CurClassification);
                    NotifyOfPropertyChange(() => HiServiceBeingUsed);
                    //RegistrationSummaryVM.HIServiceBeingUsed = HIServiceBeingUsed;
                }
            }
        }
        public bool HiServiceBeingUsed
        {
            get
            {
                if (_curClassification == null)
                {
                    return false;
                }
                return _curClassification.PatientType == PatientType.INSUARED_PATIENT;
            }
        }
        private HealthInsurance _confirmedHiItem;
        /// <summary>
        /// Thông tin thẻ bảo hiểm đã được confirm
        /// </summary>
        public HealthInsurance ConfirmedHiItem
        {
            get
            {
                return _confirmedHiItem;
            }
            set
            {
                _confirmedHiItem = value;
                NotifyOfPropertyChange(() => ConfirmedHiItem);

                CurClassification = CreateDefaultClassification();
                PatientSummaryInfoContent.CurrentPatientClassification = CreateDefaultClassification();
            }
        }

        private PatientRegistration _curRegistration;
        public PatientRegistration CurRegistration
        {
            get
            {
                return _curRegistration;
            }
            set
            {
                if (_curRegistration != value)
                {
                    _curRegistration = value;
                    NotifyOfPropertyChange(() => CurRegistration);

                    NotifyOfPropertyChange(() => CanLoadBillCmd);
                    NotifyOfPropertyChange(() => CanAddEditBill);
                    NotifyOfPropertyChange(() => CanRegister);

                    if (CurRegistration != null && CurRegistration.AdmissionInfo != null
                        && CurRegistration.AdmissionInfo.DischargeDate != null)
                    {
                        isDischarged = true;
                    }
                    else
                    {
                        isDischarged = false;
                    }
                    NotifyOfPropertyChange(() => isDischarged);

                    if (EditingInvoiceDetailsContent != null && OldBillingInvoiceContent != null)
                    {
                        if (CurRegistration != null && CurRegistration.HisID.GetValueOrDefault() > 0 && CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0)
                        {
                            EditingInvoiceDetailsContent.ShowHIAppliedColumn = true;
                            OldBillingInvoiceContent.ShowHIAppliedColumn = true;
                        }
                        else
                        {
                            EditingInvoiceDetailsContent.ShowHIAppliedColumn = false;
                            OldBillingInvoiceContent.ShowHIAppliedColumn = false;
                        }
                    }
                    PatientSummaryInfoContent.CurrentPatientRegistration = CurRegistration;
                }
            }
        }
        private Patient _curPatient;
        public Patient CurPatient
        {
            get
            {
                return _curPatient;
            }
            set
            {
                _curPatient = value;
                NotifyOfPropertyChange(() => CurPatient);
            }
        }

        private PaperReferal _confirmedPaperReferal;
        /// <summary>
        /// Thông tin giấy chuyển viện đã được confirm
        /// </summary>
        public PaperReferal ConfirmedPaperReferal
        {
            get
            {
                return _confirmedPaperReferal;
            }
            set
            {
                _confirmedPaperReferal = value;
                NotifyOfPropertyChange(() => ConfirmedPaperReferal);
            }
        }
        public bool CanRegister
        {
            get
            {
                //return _isEditing && SelectedTabIndex == (int)InPatientRegistrationViewTab.EDITING_BILLING_INVOICE;
                // Hpt 22/10/2015: Thêm điều kiện để chặn người dùng thêm dịch vụ vào bill
                // Trước đây không cần vì đã điều khiển bằng biến CanAddEditBill (khóa cả màn hình) --> sửa theo yêu cầu vẫn xem được bill của bệnh nhân cũ tuy nhiên không thể tạo mới hoặc chỉnh sửa
                if (_isEditing && SelectedTabIndex == (int)InPatientRegistrationViewTab.EDITING_BILLING_INVOICE)
                {
                    // Hpt 22/10/2015: Chỉ đăng ký đã nhập viện chưa xuất viện hoặc đăng ký Vãng lai/Tiền Giải Phẫu chưa nhập viện - chưa quá hạn mới được tạo bill
                    return (CurRegistration!= null && ((CurRegistration.AdmissionInfo != null && CurRegistration.AdmissionInfo.DischargeDate == null) 
                        || (CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING_INPT && Globals.Check_CasualAndPreOpReg_StillValid(CurRegistration))));
                }
                return false;
            }
        }
        private RegistrationFormMode _currentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
        public RegistrationFormMode CurrentRegMode
        {
            get
            {
                return _currentRegMode;
            }
            set
            {
                if (_currentRegMode != value)
                {
                    _currentRegMode = value;
                    NotifyOfPropertyChange(() => CurrentRegMode);
                }
            }
        }
        public void ResetPatientClassificationToDefaultValue()
        {
            CurClassification = CreateDefaultClassification();
        }
        private PatientClassification CreateDefaultClassification()
        {
            if (ConfirmedHiItem != null)
            {
                return PatientClassification.CreatePatientClassification((long)PatientType.INSUARED_PATIENT, "");
            }
            else
            {
                return PatientClassification.CreatePatientClassification((long)PatientType.NORMAL_PATIENT, "");
            }
        }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                NotifyOfPropertyChange(() => SelectedDate);
            }
        }

        private bool CheckDeptID()
        {
            if (DeptID.GetValueOrDefault(0) > 0)
            {
                if (DepartmentContent.SelectedItem != null && DepartmentContent.SelectedItem.DeptID > 0)
                {
                    if (DepartmentContent.SelectedItem.SelectDeptReqSelectRoom)
                    {
                        if (SelLocationInDept != null && SelLocationInDept.DeptLocationID > 0)
                        {
                            return true;
                        }
                        MessageBox.Show(eHCMSResources.Z0116_G1_HayChonPg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0340_G1_ChonKhoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);               
            }

            return false;
        }

        public void LoadPatientClassifications()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0150_G1_DangLayDSLoaiBN)
                });
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllClassifications(
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                var allClassifications = contract.EndGetAllClassifications(asyncResult);

                                if (allClassifications != null)
                                {
                                    PatientClassifications = new ObservableCollection<PatientClassification>(allClassifications);
                                }
                                else
                                {
                                    PatientClassifications = null;
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
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }
        private DateTime _RegDate;
        public DateTime RegistrationDate
        {
            get
            {
                return _RegDate;
            }
            set
            {
                _RegDate = value;
                NotifyOfPropertyChange(() => RegistrationDate);
                if (CurRegistration != null && CurRegistration.PtRegistrationID <= 0)
                {
                    CurRegistration.ExamDate = _RegDate;
                    if (_tempRegistration != null && _tempRegistration.PtRegistrationID == CurRegistration.PtRegistrationID)
                    {
                        _tempRegistration.ExamDate = _RegDate;
                    }
                }
            }
        }
        private void InitFormData()
        {
            IsLoadNoBill = false;
            if (CurRegistration == null)
            {
                return;
            }

            //PatientRegItemsContent.SetRegistration(CurRegistration);
            var newBillingInvoiceList = new ObservableCollection<InPatientBillingInvoice>();
            var oldBillingInvoiceList = new ObservableCollection<InPatientBillingInvoice>();

            if (CurRegistration.InPatientBillingInvoices != null)
            {
                foreach (var inv in CurRegistration.InPatientBillingInvoices)
                {
                    //KMx: Chỉ hiển thị những bill có cùng Khoa với Khoa đang chọn trong combobox. Tránh trường hợp Khoa này nhìn thấy Khoa khác(05/12/2014 11:11).
                    //if (inv.InPatientBillingInvID > 0)
                    if (inv.InPatientBillingInvID > 0)   
                    {
                        if (UsedByTaiVuOffice || (!Globals.isAccountCheck) || (Globals.isAccountCheck && inv.DeptID == DeptID))
                        {
                            oldBillingInvoiceList.Add(inv);
                        }
                    }
                    else
                    {
                        newBillingInvoiceList.Add(inv);
                    }
                }
            }

            OldBillingInvoiceContent.BillingInvoices = oldBillingInvoiceList;
        }

        public void InitRegistrationForPatient()
        {
            if (_curPatient == null)
                return;

            /*<Code Tuyen>
            if (_curPatient.LatestRegistration == null)
            {
                MessageBox.Show(eHCMSResources.A0236_G1_Msg_InfoBNChuaDKLanNao);
                return;
            }
            if (_curPatient.LatestRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU
                && _curPatient.LatestRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            {
                MessageBox.Show(eHCMSResources.A0485_G1_Msg_InfoDKCuoiKhPhaiNoiTru);
                return;
            }
            if (_curPatient.LatestRegistration.RegistrationStatus != AllLookupValues.RegistrationStatus.OPENED
                && _curPatient.LatestRegistration.RegistrationStatus != AllLookupValues.RegistrationStatus.PROCESSING)
            {
                //Thong bao trang thai khong dung
                var converter = new EnumValueToStringConverter();
                var enumDescription = (string)converter.Convert(_curPatient.LatestRegistration.RegistrationStatus, typeof(AllLookupValues.RegistrationStatus), null, Thread.CurrentThread.CurrentCulture);
                MessageBox.Show("Không thao tác với đăng ký này. Trạng thái đăng ký: " + enumDescription);
                return;
            }
            //Mở đăng ký còn đang sử dụng
            OpenRegistration(_curPatient.LatestRegistration.PtRegistrationID);
            Code Tuyen*/


            /*************************/
            /*Code Edit vi Tach Table*/
            /*************************/
            if (_curPatient.LatestRegistration_InPt == null)
            {
                MessageBox.Show(eHCMSResources.A0234_G1_Msg_InfoBNChuaCoDKNoiTru);
                return;
            }
            // Dang ky duoc xem la hop le khi thuoc mot trong hai truong hop duoi day:
            // 1. Dang ky noi tru da nhap vien (OPENED)
            // 2. Dang ky Vang Lai hoac Tien Giai Phau chua nhap vien (PENDING_INPT)
            if ((_curPatient.LatestRegistration_InPt.RegistrationStatus != AllLookupValues.RegistrationStatus.OPENED && _curPatient.LatestRegistration_InPt.AdmissionDate != null)
                || (_curPatient.LatestRegistration_InPt.RegistrationStatus != AllLookupValues.RegistrationStatus.PENDING_INPT && Globals.IsCasuaOrPreOpPt(_curPatient.LatestRegistration_InPt.V_RegForPatientOfType))
                //&& regInfo.RegistrationStatus != AllLookupValues.RegistrationStatus.PROCESSING
                )
            {
                //Thong bao trang thai khong dung
                var converter = new EnumValueToStringConverter();
                var enumDescription = (string)converter.Convert(_curPatient.LatestRegistration_InPt.RegistrationStatus, typeof(AllLookupValues.RegistrationStatus), null, Thread.CurrentThread.CurrentCulture);
                MessageBox.Show(string.Format("{0} {1}!", eHCMSResources.A0684_G1_Msg_InfoKhThaoTacVoiDKNay, enumDescription));
                return;
            }
            //Mở đăng ký còn đang sử dụng
            OpenRegistration(_curPatient.LatestRegistration_InPt.PtRegistrationID);

        }

        // TxD 02/08/2014 : The following method is nolonger required
        //private void GetCurrentDate()
        //{
        //    var t = new Thread(() =>
        //                           {
        //                               AxErrorEventArgs error = null;
        //                               try
        //                               {
        //                                   using (var serviceFactory = new CommonServiceClient())
        //                                   {
        //                                       var contract = serviceFactory.ServiceInstance;

        //                                       contract.BeginGetDate(Globals.DispatchCallback((asyncResult) =>
        //                                       {
        //                                           try
        //                                           {
        //                                               DateTime date = contract.EndGetDate(asyncResult);
        //                                               if (CurRegistration != null && CurRegistration.PtRegistrationID <= 0 && CurRegistration.ExamDate == DateTime.MinValue)//Đăng ký mới và chưa có ngày tháng.
        //                                               {
        //                                                   RegistrationDate = date.Date;
        //                                               }
        //                                               Globals.ServerDate = date;
        //                                           }
        //                                           catch (FaultException<AxException> fault)
        //                                           {
        //                                               error = new AxErrorEventArgs(fault);
        //                                           }
        //                                           catch (Exception ex)
        //                                           {
        //                                               error = new AxErrorEventArgs(ex);
        //                                           }

        //                                       }), null);
        //                                   }
        //                               }
        //                               catch (Exception ex)
        //                               {
        //                                   error = new AxErrorEventArgs(ex);
        //                               }
        //                               if (error != null)
        //                               {
        //                                   Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
        //                               }
        //                           });
        //    t.Start();
        //}
        private void ShowOldRegistration(PatientRegistration regInfo)
        {
            IsLoadNoBill = false;
            if (//regInfo.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU
                //&& 
                regInfo.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            {
                MessageBox.Show(eHCMSResources.Z0085_G1_DayKhongPhaiDKNoiTru);
                return;
            }
            CurRegistration = regInfo;

            CurrentRegMode = RegistrationFormMode.OLD_REGISTRATION_OPENED;
            _confirmedHiItem = CurRegistration.HealthInsurance;
            _confirmedPaperReferal = CurRegistration.PaperReferal;
            NotifyOfPropertyChange(() => ConfirmedHiItem);
            NotifyOfPropertyChange(() => ConfirmedPaperReferal);
            InitRegistration();

            if (PatientSummaryInfoContent != null)
            {
                //PatientSummaryInfoContent.HiBenefit = null;
                PatientSummaryInfoContent.CurrentPatient = CurPatient;
                //PatientSummaryInfoContent.ConfirmedPaperReferal = _confirmedPaperReferal;
                //PatientSummaryInfoContent.ConfirmedHiItem = _confirmedHiItem;
                //PatientSummaryInfoContent.IsCrossRegion = regInfo.IsCrossRegion.GetValueOrDefault(false);
                //if (CurRegistration.PtInsuranceBenefit.HasValue)
                //{
                //    PatientSummaryInfoContent.HiBenefit = CurRegistration.PtInsuranceBenefit;
                //}
                PatientSummaryInfoContent.SetPatientHISumInfo(CurRegistration.PtHISumInfo);

                //EditingInvoiceDetailsContent.HIBenefit = PatientSummaryInfoContent.HiBenefit;
                EditingInvoiceDetailsContent.HIBenefit = CurRegistration.PtInsuranceBenefit;
            }
            if (CurRegistration.PatientClassification == null && CurRegistration.PatientClassID.HasValue)
            {
                CurClassification = PatientClassification.CreatePatientClassification(CurRegistration.PatientClassID.Value, "");
            }
            else
            {
                CurClassification = CurRegistration.PatientClassification;
            }
            PatientSummaryInfoContent.CurrentPatientClassification = CreateDefaultClassification();
            //CanChangePatientType = false;

            //KMx: Khi load bệnh nhân thì xóa hết những món hàng trong EditingInvoiceDetailsContent. Nếu không thì sẽ có 2 thread chạy đua về server (23/08/2014 15:35).
            //bool readOnly = false;

            //if (EditingInvoiceDetailsContent.BillingInvoice != null && CurRegistration.InPatientBillingInvoices != null
            //    && EditingInvoiceDetailsContent.BillingInvoice.PtRegistrationID == CurRegistration.PtRegistrationID)
            //{
            //    foreach (var inv in CurRegistration.InPatientBillingInvoices)
            //    {
            //        if (EditingInvoiceDetailsContent.BillingInvoice.InPatientBillingInvID == inv.InPatientBillingInvID)
            //        {
            //            EditingInvoiceDetailsContent.LoadDetails(UpdateBillingInvoiceCompletedCallback);
            //            readOnly = true;
            //        }
            //    }
            //}

            SetConditionWhenChangeSelectedItem();
            Validate_RegistrationInfo(regInfo);
        }  

        // Hpt 22/10/2015: Cần kiểm tra đăng ký nhiều lần nên đưa hàm kiểm tra ra ngoài để gọi lại cho tiện
        private bool Validate_RegistrationInfo(PatientRegistration regInfo)
        {
            if (regInfo.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND || regInfo.RegistrationStatus == AllLookupValues.RegistrationStatus.COMPLETED)
            {
                MessageBox.Show(eHCMSResources.A0483_G1_Msg_InfoKhTheTaoHayCNhatBill);
                return false;
            }
            // Dang ky noi tru chua nhap vien (AdmissionDate == null) hoac da nhap vien ma trang thai khac OPENED (= REFUND) thi khong hop le  
            if ((regInfo.AdmissionInfo != null && regInfo.RegistrationStatus != AllLookupValues.RegistrationStatus.OPENED)
                || (regInfo.AdmissionInfo == null && Globals.IsCasuaOrPreOpPt(regInfo.V_RegForPatientOfType) && regInfo.RegistrationStatus != AllLookupValues.RegistrationStatus.PENDING_INPT))
            {
                var converter = new EnumValueToStringConverter();
                var enumDescription = (string)converter.Convert(regInfo.RegistrationStatus, typeof(AllLookupValues.RegistrationStatus), null, Thread.CurrentThread.CurrentCulture);
                MessageBox.Show(string.Format("{0} {1}!", eHCMSResources.A0684_G1_Msg_InfoKhThaoTacVoiDKNay, enumDescription));
                return false;
            }
            if (Globals.IsCasuaOrPreOpPt(regInfo.V_RegForPatientOfType) && regInfo.AdmissionInfo == null
                && regInfo.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING_INPT && !Globals.Check_CasualAndPreOpReg_StillValid(regInfo))
            {
                MessageBox.Show(eHCMSResources.A0491_G1_Msg_InfoKhTheTaoHayCNhatBill2, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (regInfo.AdmissionInfo != null && regInfo.AdmissionInfo.DischargeDate != null)
            {
                MessageBox.Show(string.Format("{0}. {1}!", eHCMSResources.A0226_G1_Msg_InfoBNDaXV, eHCMSResources.A0227_G1_Msg_KhTheTaoHoacCapNhapBill), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            return true;
        }
        /// <summary>
        /// Gọi hàm này khi tạo mới một đăng ký, hoặc load xong một đăng ký đã có.
        /// Khởi tạo những giá trị cần thiết để đưa lên form
        /// </summary>
        private void InitRegistration()
        {
            _curPatient = CurRegistration.Patient;
            NotifyOfPropertyChange(() => CurPatient);

            InitFormData();
        }        

        //HPT: Thêm hàm kiểm tra đăng ký vãng lai hoặc tiền giải phẫu đã hết hạn hay chưa để cho phép hay không cho phép tạo bill

        //HPT - END - 10/09/2015
        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            LoadRegisSwitch.IsGetBillingInvoices = true;

            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, LoadRegisSwitch);
            yield return loadRegTask;
            if (loadRegTask.Registration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = "Error!(6)" });
            }
            else
            {
                //khong can hoi->chon lai la load len thoi
                //if (_curPatient != null && regInfo.PatientID != _curPatient.PatientID)
                //{
                //    string newPatientName = regInfo != null &&
                //                            regInfo.Patient != null
                //                            ? regInfo.Patient.FullName : "";
                //    string message = string.Format("Bạn đang thao tác với bệnh nhân '{0}'. Bạn có muốn chuyển sang đăng ký của bệnh nhân '{1}'?", _curPatient.FullName, newPatientName);
                //    MessageBoxResult result = MessageBox.Show(message, eHCMSResources.G0442_G1_TBao,
                //                                              MessageBoxButton.OKCancel);
                //    if (result == MessageBoxResult.OK)
                //    {
                //        ShowOldRegistration(regInfo);

                //        //goi lai ham chon khoa phong de load lai
                //        SetConditionWhenChangeSelectedItem();
                //    }
                //}


                //KMx: Ở đây set CurRegistration rồi, trong hàm ShowOldRegistration lại set nữa là sao? (16/09/2014 11:33).
                CurRegistration = loadRegTask.Registration;
                PatientSummaryInfoContent.CurrentPatient = CurRegistration.Patient;
                ShowOldRegistration(CurRegistration); 
            }
        }

        public void OpenRegistration(long regID)
        {
            RegistrationLoading = true;
            Coroutine.BeginExecute(DoOpenRegistration(regID), null, (o, e) => { RegistrationLoading = false; });
        }

        private PatientRegistration _tempRegistration;

        /// <summary>
        /// Chuan bi cong viec bat dau edit. Backup lai may cai object.
        /// </summary>
        public void BeginEdit()
        {
            RegistrationInfoHasChanged = false;
            _tempRegistration = CurRegistration.DeepCopy();
            if (EditingBillingInvoice == null)
            {
                EditingBillingInvoice = new InPatientBillingInvoice();
            }
            if (EditingBillingInvoice.InPatientBillingInvID <= 0)
            {
                if (RegistrationDate != DateTime.MinValue)
                {
                    EditingBillingInvoice.InvDate = RegistrationDate;
                }
                else
                {
                    EditingBillingInvoice.InvDate = Globals.GetCurServerDateTime();
                    //KMx: Thêm ngày 02/01/2015 09:44.
                    EditingBillingInvoice.BillFromDate = Globals.GetCurServerDateTime();
                    EditingBillingInvoice.BillToDate = Globals.GetCurServerDateTime();
                }
                EditingBillingInvoice.V_InPatientBillingInvStatus = AllLookupValues.V_InPatientBillingInvStatus.NEW;
            }
            _tempBillingInvoice = EditingBillingInvoice.DeepCopy();
            IsEditing = true;
            IsLoadNoBill = false;
        }

        public void CancelEdit()
        {
            CurRegistration = _tempRegistration;
            _tempRegistration = null;
            EditingBillingInvoice = _tempBillingInvoice;
            IsBillOfUsedItems = false;
            EditingInvoiceDetailsContent.ResetView();

            _tempBillingInvoice = null;
            InitFormData();
            RegistrationInfoHasChanged = false;
            IsEditing = false;
            IsLoadNoBill = false;
        }

        public void EndEdit()
        {
            _tempRegistration = null;
            IsBillOfUsedItems = false;
        }

        private int? _serviceQty;
        public int? ServiceQty
        {
            get { return _serviceQty; }
            set
            {
                _serviceQty = value;
                NotifyOfPropertyChange(() => ServiceQty);
            }
        }

        private int? _PclQty;
        public int? PclQty
        {
            get { return _PclQty; }
            set
            {
                _PclQty = value;
                NotifyOfPropertyChange(() => PclQty);
            }
        }

        private int? _PclQtyLAB;
        public int? PclQtyLAB
        {
            get { return _PclQtyLAB; }
            set
            {
                _PclQtyLAB = value;
                NotifyOfPropertyChange(() => PclQtyLAB);
            }
        }

        private decimal? _drugQty;
        public decimal? DrugQty
        {
            get { return _drugQty; }
            set
            {
                _drugQty = value;
                NotifyOfPropertyChange(() => DrugQty);
            }
        }
        private decimal? _medItemQty;
        public decimal? MedItemQty
        {
            get { return _medItemQty; }
            set
            {
                _medItemQty = value;
                NotifyOfPropertyChange(() => MedItemQty);
            }
        }
        private decimal? _chemicalQty;
        public decimal? ChemicalQty
        {
            get { return _chemicalQty; }
            set
            {
                _chemicalQty = value;
                NotifyOfPropertyChange(() => ChemicalQty);
            }
        }
        public void Handle(ResultFound<Patient> message)
        {
            if (this.GetView() != null && message != null)
            {
                CurPatient = message.Result;
                CurRegistration = null;

                if (CurPatient != null)
                {
                    SetCurrentPatient(CurPatient);
                }
            }
        }

        public void SetCurrentPatient(object patient)
        {
            Patient p = patient as Patient;
            if (p == null || p.PatientID <= 0)
            {
                return;
            }
            ConfirmedHiItem = null;
            ConfirmedPaperReferal = null;

            EditingBillingInvoice = null;
            OldBillingInvoiceContent.BillingInvoices = null;

            if (p.PatientID > 0)
            {
                GetPatientByID(p.PatientID);
            }
            else
            {
                CurPatient = null;
                CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
            }
        }
        private bool _patientLoaded;
        public bool PatientLoaded
        {
            get
            {
                return _patientLoaded;
            }
            set
            {
                _patientLoaded = value;
                NotifyOfPropertyChange(() => PatientLoaded);
            }
        }
        private void GetPatientByID(long patientID)
        {
            PatientLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetPatientByID(patientID, false,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var patient = contract.EndGetPatientByID(asyncResult);
                                    CurPatient = patient;

                                    PatientLoaded = true;
                                    PatientLoading = false;
                                    InitRegistrationForPatient();
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
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        private void AddMedRegItemBaseToNewInvoice(MedRegItemBase item)
        {
            EditingInvoiceDetailsContent.AddItemToView(item);
        }

        //private void CalcInvoiceItem(IInvoiceItem item)
        private void CalcInvoiceItem(MedRegItemBase item)
        {
            //double? hiBenefit = PatientSummaryInfoContent.HiBenefit;
            
            //item.HIBenefit = PatientSummaryInfoContent.HiBenefit;
            item.HIBenefit = CurRegistration.PtInsuranceBenefit;

            //if (hiBenefit > 0)
            //{
            //    //item.HiApplied = true;
            //}

            item.InvoicePrice = item.HIBenefit.HasValue ? item.ChargeableItem.HIPatientPrice : item.ChargeableItem.NormalPrice;
            item.HIAllowedPrice = item.ChargeableItem.HIAllowedPrice;
            item.PriceDifference = item.InvoicePrice - item.HIAllowedPrice.GetValueOrDefault(0);
            item.TotalInvoicePrice = item.InvoicePrice * (decimal)item.Qty;
            item.TotalHIPayment = MathExt.Round(item.HIAllowedPrice.GetValueOrDefault(0) * (decimal)item.HIBenefit.GetValueOrDefault(0.0) * item.Qty, Common.Converters.MidpointRounding.AwayFromZero);
            item.TotalPatientPayment = item.TotalInvoicePrice - item.TotalHIPayment;

            if (item.HIBenefit.GetValueOrDefault() > 0 && item.HIAllowedPrice.GetValueOrDefault() > 0)
            {
                item.IsCountHI = true;
            }
            else
            {
                item.IsCountHI = false;
            }

        }
        public void AddGenMedService(object medicalService, int qty, DateTime createdDate)
        {
            if (!CheckDeptID())
            {
                return;
            }

            RefMedicalServiceItem curItem = medicalService as RefMedicalServiceItem;

            if (CurRegistration == null || curItem == null)
            {
                return;
            }

            if (curItem.RefMedicalServiceType.V_RefMedicalServiceTypes != (long)AllLookupValues.V_RefMedicalServiceTypes.CANLAMSANG)
            {
                PatientRegistrationDetail item;
                item = new PatientRegistrationDetail();
                item.StaffID = Globals.LoggedUserAccount.StaffID;
                item.CreatedDate = createdDate;
                item.MedProductType = AllLookupValues.MedProductType.KCB;
                item.RefMedicalServiceItem = curItem;
                item.Qty = qty;

                //KMx: Không sử dụng biến HiApplied nữa, thay thế bằng IsCountHI và được set trong hàm CalcInvoiceItem (12/12/2014 09:07).
                //if (CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0 && CurRegistration.HisID.GetValueOrDefault(0) > 0)
                //{
                //    item.HiApplied = true;
                //}
                //else
                //{
                //    item.HiApplied = false;
                //}

                if (EditingBillingInvoice.RegistrationDetails == null)
                {
                    EditingBillingInvoice.RegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                }
                else
                {
                    foreach (var row in EditingBillingInvoice.RegistrationDetails)
                    {
                        if (row.MedServiceID == item.MedServiceID && item.MedServiceID > 0)
                        {
                            if (MessageBox.Show(string.Format("{0} {1}", item.RefMedicalServiceItem.MedServiceName, eHCMSResources.W0949_G1_W), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                            {
                                return;
                            }
                        }
                    }
                }
                CalcInvoiceItem(item);
                EditingBillingInvoice.RegistrationDetails.Add(item);

                AddMedRegItemBaseToNewInvoice(item);
                RegistrationInfoHasChanged = true;
            }
            else
            {
                AddDefaultPCLRequest(curItem.MedServiceID, qty);
            }

        }

        public void AddPCLItem_Goi(object pclItem, int qty, DateTime createdDate, bool used)
        {
            if (!CheckDeptID())
            {
                return;
            }
            if (CurRegistration != null)
            {
                var curItem = pclItem as PCLExamType;
                if (curItem != null)
                {
                    PatientPCLRequestDetail item;
                    item = new PatientPCLRequestDetail();
                    item.StaffID = Globals.LoggedUserAccount.StaffID;
                    item.CreatedDate = createdDate;
                    item.MedProductType = AllLookupValues.MedProductType.CAN_LAM_SANG;
                    item.PCLExamType = curItem;
                    item.Qty = qty;

                    //KMx: Không sử dụng biến HiApplied nữa, thay thế bằng IsCountHI và được set trong hàm CalcInvoiceItem (12/12/2014 09:07).
                    //if (CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0 && CurRegistration.HisID.GetValueOrDefault(0) > 0)
                    //{
                    //    item.HiApplied = true;
                    //}
                    //else
                    //{
                    //    item.HiApplied = false;
                    //}

                    if (used)
                    {
                        item.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.HOAN_TAT;
                    }
                    else
                    {
                        item.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
                    }

                    if (EditingBillingInvoice.PclRequests == null)
                    {
                        EditingBillingInvoice.PclRequests = new ObservableCollection<PatientPCLRequest>();
                    }
                    PatientPCLRequest tempRequest = EditingBillingInvoice.PclRequests.Where(p => p.V_PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU
                                                                                            && p.StaffID == item.StaffID
                                                                                            && p.CreatedDate.Date == createdDate.Date
                                                                                            && p.RecordState == RecordState.DETACHED).FirstOrDefault();
                    if (tempRequest == null)
                    {
                        tempRequest = new PatientPCLRequest();
                        tempRequest.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                        tempRequest.Diagnosis = eHCMSResources.Z0159_G1_CLSBNMoi;
                        tempRequest.StaffID = item.StaffID;
                        tempRequest.CreatedDate = createdDate;
                        tempRequest.V_PCLRequestType = AllLookupValues.V_PCLRequestType.NOI_TRU;

                        if (used)
                        {
                            tempRequest.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.CLOSE;
                        }
                        else
                        {
                            tempRequest.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
                        }
                        tempRequest.RecordState = RecordState.DETACHED;
                        tempRequest.EntityState = EntityState.DETACHED;

                        if (Globals.DeptLocation != null)
                        {
                            tempRequest.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;
                        }

                        if (DepartmentContent != null && DepartmentContent.SelectedItem != null
                            && DepartmentContent.SelectedItem.SelectDeptReqSelectRoom)
                        {
                            if (SelLocationInDept != null && SelLocationInDept.DeptLocationID > 0)
                            {
                                tempRequest.ReqFromDeptLocID = SelLocationInDept.DeptLocationID;
                            }
                        }

                        EditingBillingInvoice.PclRequests.Add(tempRequest);
                    }
                    CalcInvoiceItem(item);
                    tempRequest.PatientPCLRequestIndicators.Add(item);
                    AddMedRegItemBaseToNewInvoice(item);

                    RegistrationInfoHasChanged = true;
                }
            }
        }

        public void CheckAndAddAllPCL(ObservableCollection<PCLExamType> AllPCLExamType, int qty, DateTime createdDate, bool used)
        {
            if (AllPCLExamType == null || !CheckDeptID())
            {
                return;
            }

            ObservableCollection<PCLExamType> NewPCLList = new ObservableCollection<PCLExamType>();
            ObservableCollection<PCLExamType> ExistsPCLList = new ObservableCollection<PCLExamType>();

            if (EditingBillingInvoice != null && EditingBillingInvoice.PclRequests != null)
            {
                var lstpcldetails = EditingBillingInvoice.PclRequests.SelectMany(x => x.PatientPCLRequestIndicators);

                foreach (var item in AllPCLExamType)
                {
                    if (lstpcldetails.Any(x => x.PCLExamType != null && x.PCLExamType.PCLExamTypeID == item.PCLExamTypeID))
                    {
                        ExistsPCLList.Add(item);
                    }
                    else
                    {
                        NewPCLList.Add(item);
                    }
                }
            }

            if (ExistsPCLList != null && ExistsPCLList.Count > 0)
            {
                string strPCLName = "";
                foreach (PCLExamType existsPCL in ExistsPCLList)
                {
                    strPCLName += Environment.NewLine + existsPCL.PCLExamTypeName + ".";
                }

                if (MessageBox.Show(string.Format("{0} {1}. \n{2}", eHCMSResources.A0892_G1_Msg_InfoPCLDaTonTai, strPCLName, eHCMSResources.T1986_G1_CoMuonTiepTucThemKhong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    foreach (PCLExamType item in AllPCLExamType)
                    {
                        AddPCLItem(item, qty, createdDate, used);
                    }
                }
                else
                {
                    foreach (PCLExamType item in NewPCLList)
                    {
                        AddPCLItem(item, qty, createdDate, used);
                    }
                }
            }
            else
            {
                foreach (PCLExamType item in AllPCLExamType)
                {
                    AddPCLItem(item, qty, createdDate, used);
                }
            }
        }


        private void CheckAndAddPCL(PCLExamType pclItem, int qty, DateTime createdDate, bool used)
        {
            if (pclItem == null || !CheckDeptID())
            {
                return;
            }

            if (EditingBillingInvoice != null && EditingBillingInvoice.PclRequests != null)
            {
                var lstpcldetails = EditingBillingInvoice.PclRequests.SelectMany(x => x.PatientPCLRequestIndicators);


                if (lstpcldetails.Any(x => x.PCLExamType != null && x.PCLExamType.PCLExamTypeID == pclItem.PCLExamTypeID))
                {
                    if (MessageBox.Show(string.Format("{0} {1}", pclItem.PCLExamTypeName, eHCMSResources.W0949_G1_W), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        return;
                    }
                }
            }

            AddPCLItem(pclItem, qty, createdDate, used);
        }

        private void AddPCLItem(PCLExamType pclItem, int qty, DateTime createdDate, bool used)
        {
            if (CurRegistration == null || pclItem == null)
            {
                return;
            }

            //KMx: Trước đây nếu số lượng > 1 thì chỉ có 1 dòng. Sửa lại nếu SL bao nhiêu thì sinh ra bấy nhiêu dòng, để 1 dòng sẽ có 1 kết quả CLS khác nhau (15/12/2014 10:29).
            for (int i = 0; i < qty; i++)
            {
                PatientPCLRequestDetail item;
                item = new PatientPCLRequestDetail();
                item.StaffID = Globals.LoggedUserAccount.StaffID;
                item.CreatedDate = createdDate;
                item.MedProductType = AllLookupValues.MedProductType.CAN_LAM_SANG;
                item.PCLExamType = pclItem;
                //KMx: Nếu muốn sửa SL > 1 mà chỉ hiển thị 1 dòng thì sử dụng lại dòng code bên dưới và bỏ dòng item.Qty = 1 đi (15/12/2014 10:30).
                //item.Qty = qty;
                item.Qty = 1;

                //KMx: Không sử dụng biến HiApplied nữa, thay thế bằng IsCountHI và được set trong hàm CalcInvoiceItem (12/12/2014 09:07).
                //if (CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0 && CurRegistration.HisID.GetValueOrDefault(0) > 0)
                //{
                //    item.HiApplied = true;
                //}
                //else
                //{
                //    item.HiApplied = false;
                //}

                if (used)
                {
                    item.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.HOAN_TAT;
                }
                else
                {
                    item.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
                }

                if (EditingBillingInvoice.PclRequests == null)
                {
                    EditingBillingInvoice.PclRequests = new ObservableCollection<PatientPCLRequest>();
                }

                PatientPCLRequest tempRequest = EditingBillingInvoice.PclRequests.Where(p => p.V_PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU
                                                                                        && p.StaffID == item.StaffID
                                                                                        && p.CreatedDate.Date == createdDate.Date
                                                                                        && p.RecordState == RecordState.DETACHED).FirstOrDefault();
                if (tempRequest == null)
                {
                    tempRequest = new PatientPCLRequest();
                    tempRequest.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                    tempRequest.Diagnosis = eHCMSResources.Z0159_G1_CLSBNMoi;
                    tempRequest.StaffID = item.StaffID;
                    tempRequest.CreatedDate = createdDate;
                    tempRequest.V_PCLRequestType = AllLookupValues.V_PCLRequestType.NOI_TRU;

                    if (used)
                    {
                        tempRequest.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.CLOSE;
                    }
                    else
                    {
                        tempRequest.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
                    }
                    tempRequest.RecordState = RecordState.DETACHED;
                    tempRequest.EntityState = EntityState.DETACHED;

                    if (Globals.DeptLocation != null)
                    {
                        tempRequest.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;
                    }

                    if (DepartmentContent != null && DepartmentContent.SelectedItem != null
                        && DepartmentContent.SelectedItem.SelectDeptReqSelectRoom)
                    {
                        if (SelLocationInDept != null && SelLocationInDept.DeptLocationID > 0)
                        {
                            tempRequest.ReqFromDeptLocID = SelLocationInDept.DeptLocationID;
                        }
                    }


                    EditingBillingInvoice.PclRequests.Add(tempRequest);
                }
                CalcInvoiceItem(item);
                tempRequest.PatientPCLRequestIndicators.Add(item);
                AddMedRegItemBaseToNewInvoice(item);
            }
            RegistrationInfoHasChanged = true;
        }




        //public void AddPCLItem(object pclItem, int qty, DateTime createdDate, bool used)
        //{
        //    if (!CheckDeptID())
        //    {
        //        return;
        //    }
        //    if (CurRegistration == null)
        //    {
        //        return;
        //    }
        //    var curItem = pclItem as PCLExamType;
        //    if (curItem != null)
        //    {
        //        PatientPCLRequestDetail item;
        //        item = new PatientPCLRequestDetail();
        //        item.StaffID = Globals.LoggedUserAccount.StaffID;
        //        item.CreatedDate = createdDate;
        //        item.MedProductType = AllLookupValues.MedProductType.CAN_LAM_SANG;
        //        item.PCLExamType = curItem;
        //        item.Qty = qty;


        //        if (CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0 && CurRegistration.HisID.GetValueOrDefault(0) > 0)
        //        {
        //            item.HiApplied = true;
        //        }
        //        else
        //        {
        //            item.HiApplied = false;
        //        }

        //        if (used)
        //        {
        //            item.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.HOAN_TAT;
        //        }
        //        else
        //        {
        //            item.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
        //        }
        //        if (EditingBillingInvoice.PclRequests != null)
        //        {
        //            var lstpcldetails = EditingBillingInvoice.PclRequests.SelectMany(x => x.PatientPCLRequestIndicators);
        //            if (lstpcldetails != null)
        //            {
        //                foreach (var row in lstpcldetails)
        //                {
        //                    if (row.PCLExamType.PCLExamTypeID == item.PCLExamType.PCLExamTypeID)
        //                    {
        //                        if (MessageBox.Show(item.PCLExamType.PCLExamTypeName + " này đã tồn tại.Bạn có muốn tiếp tục thêm không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
        //                        {
        //                            return;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        if (EditingBillingInvoice.PclRequests == null)
        //        {
        //            EditingBillingInvoice.PclRequests = new ObservableCollection<PatientPCLRequest>();
        //        }
        //        PatientPCLRequest tempRequest = EditingBillingInvoice.PclRequests.Where(p => p.V_PCLRequestType == AllLookupValues.V_PCLRequestType.NOI_TRU
        //                                                                                && p.StaffID == item.StaffID
        //                                                                                && p.CreatedDate.Date == createdDate.Date
        //                                                                                && p.RecordState == RecordState.DETACHED).FirstOrDefault();
        //        if (tempRequest == null)
        //        {
        //            tempRequest = new PatientPCLRequest();
        //            tempRequest.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
        //            tempRequest.Diagnosis = eHCMSResources.Z0159_G1_CLSBNMoi;
        //            tempRequest.StaffID = item.StaffID;
        //            tempRequest.CreatedDate = createdDate;
        //            tempRequest.V_PCLRequestType = AllLookupValues.V_PCLRequestType.NOI_TRU;

        //            if (used)
        //            {
        //                tempRequest.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.CLOSE;
        //            }
        //            else
        //            {
        //                tempRequest.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
        //            }
        //            tempRequest.RecordState = RecordState.DETACHED;
        //            tempRequest.EntityState = EntityState.DETACHED;

        //            if (Globals.DeptLocation != null)
        //            {
        //                tempRequest.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;
        //            }

        //            EditingBillingInvoice.PclRequests.Add(tempRequest);
        //        }
        //        CalcInvoiceItem(item);
        //        tempRequest.PatientPCLRequestIndicators.Add(item);
        //        AddMedRegItemBaseToNewInvoice(item);

        //        RegistrationInfoHasChanged = true;
        //    }
        //}

        public void AddGenMedDrug(object drug, decimal qty, DateTime createdDate)
        {
            if (!CheckDeptID())
            {
                return;
            }
            if (CurRegistration != null)
            {
                if (_selectedWarehouse == null || _selectedWarehouse.StoreID <= 0)
                {
                    MessageBox.Show(eHCMSResources.K0338_G1_ChonKho);
                    return;
                }
                RefGenMedProductDetails curItem = drug as RefGenMedProductDetails;
                if (curItem != null)
                {
                    if (curItem.Remaining < qty)
                    {
                        if (MessageBox.Show(eHCMSResources.A0975_G1_Msg_ConfTiepTucThem, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        {
                            return;
                        }
                    }
                    OutwardDrugClinicDept item = new OutwardDrugClinicDept();
                    item.StaffID = Globals.LoggedUserAccount.StaffID;
                    item.CreatedDate = createdDate;
                    item.GenMedProductItem = curItem;
                    item.MedProductType = AllLookupValues.MedProductType.THUOC;
                    item.OutQuantity = qty;
                    item.Qty = qty;

                    //KMx: Không sử dụng biến HiApplied nữa, thay thế bằng IsCountHI và được set trong hàm CalcInvoiceItem (12/12/2014 09:07).
                    //if (CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0 && CurRegistration.HisID.GetValueOrDefault(0) > 0)
                    //{
                    //    item.HiApplied = true;
                    //}
                    //else
                    //{
                    //    item.HiApplied = false;
                    //}

                    if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices != null)
                    {
                        var lstdrugDetails = EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Where(x => x.MedProductType == AllLookupValues.MedProductType.THUOC).SelectMany(x => x.OutwardDrugClinicDepts).Where(owd => owd.GenMedProductItem.GenMedProductID == item.GenMedProductItem.GenMedProductID);

                        if (lstdrugDetails != null && lstdrugDetails.Count() > 0)
                        {
                            if (MessageBox.Show(string.Format("{0} {1}", item.GenMedProductItem.BrandName, eHCMSResources.W0949_G1_W), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                            {
                                return;
                            }
                        }
                        //if (lstdrugDetails != null)
                        //{
                        //    foreach (var row in lstdrugDetails)
                        //    {
                        //        if (row.GenMedProductItem.GenMedProductID == item.GenMedProductItem.GenMedProductID)
                        //        {
                        //            if (MessageBox.Show(row.GenMedProductItem.BrandName + " đã tồn tại. Bạn có muốn tiếp tục thêm không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        //            {
                        //                return;
                        //            }
                        //        }
                        //    }

                        //}
                    }

                    if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices == null)
                    {
                        EditingBillingInvoice.OutwardDrugClinicDeptInvoices = new ObservableCollection<OutwardDrugClinicDeptInvoice>();
                    }
                    var lastInvoice = EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Where(p => p.MedProductType == AllLookupValues.MedProductType.THUOC
                                                                    && p.outiID <= 0
                                                                    && p.CreatedDate.Date.ToShortDateString() == createdDate.Date.ToShortDateString()
                                                                    && p.StoreID == curItem.StoreID
                                                                    && p.StoreID == _selectedWarehouse.StoreID).FirstOrDefault();
                    if (lastInvoice == null)
                    {
                        lastInvoice = new OutwardDrugClinicDeptInvoice()
                        {
                            OutwardDrugClinicDepts = new ObservableCollection<OutwardDrugClinicDept>(),
                            MedProductType = AllLookupValues.MedProductType.THUOC,
                            OutDate = createdDate,
                            StaffID = Globals.LoggedUserAccount.StaffID,
                            StoreID = curItem.StoreID,
                            SelectedStorage = curItem.Storage
                        };
                        EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Add(lastInvoice);
                    }

                    item.DrugInvoice = lastInvoice;
                    CalcInvoiceItem(item);
                    lastInvoice.OutwardDrugClinicDepts.Add(item);
                    AddMedRegItemBaseToNewInvoice(item);

                    RegistrationInfoHasChanged = true;
                }
            }
        }

        public void AddGenMedItem(object medItem, decimal qty, DateTime createdDate)
        {
            if (!CheckDeptID())
            {
                return;
            }
            if (CurRegistration != null)
            {
                if (_selectedWarehouse == null || _selectedWarehouse.StoreID <= 0)
                {
                    MessageBox.Show(eHCMSResources.K0338_G1_ChonKho);
                    return;
                }
                RefGenMedProductDetails curItem = medItem as RefGenMedProductDetails;
                if (curItem != null)
                {
                    if (curItem.Remaining < qty)
                    {
                        if (MessageBox.Show(eHCMSResources.A0975_G1_Msg_ConfTiepTucThem, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        {
                            return;
                        }
                    }
                    var item = new OutwardDrugClinicDept();
                    item.StaffID = Globals.LoggedUserAccount.StaffID;
                    item.CreatedDate = createdDate;
                    item.GenMedProductItem = curItem;
                    item.MedProductType = AllLookupValues.MedProductType.Y_CU;
                    item.OutQuantity = qty;
                    item.Qty = qty;

                    //KMx: Không sử dụng biến HiApplied nữa, thay thế bằng IsCountHI và được set trong hàm CalcInvoiceItem (12/12/2014 09:07).
                    //if (CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0 && CurRegistration.HisID.GetValueOrDefault(0) > 0)
                    //{
                    //    item.HiApplied = true;
                    //}
                    //else
                    //{
                    //    item.HiApplied = false;
                    //}

                    if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices != null)
                    {
                        var lstdrugDetails = EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Where(x => x.MedProductType == AllLookupValues.MedProductType.Y_CU).SelectMany(x => x.OutwardDrugClinicDepts).Where(owd => owd.GenMedProductItem.GenMedProductID == item.GenMedProductItem.GenMedProductID);

                        if (lstdrugDetails != null && lstdrugDetails.Count() > 0)
                        {
                            if (MessageBox.Show(string.Format("{0} {1}", item.GenMedProductItem.BrandName, eHCMSResources.W0949_G1_W), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                            {
                                return;
                            }
                        }
                        
                        //if (lstdrugDetails != null)
                        //{
                        //    foreach (var row in lstdrugDetails)
                        //    {
                        //        if (row.GenMedProductItem.GenMedProductID == item.GenMedProductItem.GenMedProductID)
                        //        {
                        //            if (MessageBox.Show(row.GenMedProductItem.BrandName + " đã tồn tại. Bạn có muốn tiếp tục thêm không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        //            {
                        //                return;
                        //            }
                        //        }
                        //    }

                        //}
                    }

                    if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices == null)
                    {
                        EditingBillingInvoice.OutwardDrugClinicDeptInvoices = new ObservableCollection<OutwardDrugClinicDeptInvoice>();
                    }
                    if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Where(p => p.MedProductType == AllLookupValues.MedProductType.Y_CU
                                                                    && p.outiID <= 0
                                                                    && p.CreatedDate.Date == createdDate.Date
                                                                    && p.StoreID == curItem.StoreID
                                                                    && p.StoreID == _selectedWarehouse.StoreID).FirstOrDefault() == null)
                    {

                        EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Add(new OutwardDrugClinicDeptInvoice()
                        {
                            OutwardDrugClinicDepts = new ObservableCollection<OutwardDrugClinicDept>(),
                            MedProductType = AllLookupValues.MedProductType.Y_CU,
                            OutDate = createdDate,
                            StaffID = Globals.LoggedUserAccount.StaffID,
                            StoreID = curItem.StoreID,
                            SelectedStorage = curItem.Storage
                        }
                       );
                    }
                    var lastInvoice = EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Last();

                    item.DrugInvoice = lastInvoice;
                    CalcInvoiceItem(item);
                    lastInvoice.OutwardDrugClinicDepts.Add(item);
                    AddMedRegItemBaseToNewInvoice(item);

                    RegistrationInfoHasChanged = true;
                }
            }
        }

        public void AddGenChemical(object medItem, decimal qty, DateTime createdDate)
        {
            if (!CheckDeptID())
            {
                return;
            }
            if (CurRegistration != null)
            {
                if (_selectedWarehouse == null || _selectedWarehouse.StoreID <= 0)
                {
                    MessageBox.Show(eHCMSResources.K1973_G1_ChonKho);
                    return;
                }
                var curItem = medItem as RefGenMedProductDetails;
                if (curItem != null)
                {
                    if (curItem.Remaining < qty)
                    {
                        if (MessageBox.Show(eHCMSResources.A0975_G1_Msg_ConfTiepTucThem, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        {
                            return;
                        }
                    }
                    var item = new OutwardDrugClinicDept();
                    item.StaffID = Globals.LoggedUserAccount.StaffID;
                    item.CreatedDate = createdDate;
                    item.GenMedProductItem = curItem;
                    item.MedProductType = AllLookupValues.MedProductType.HOA_CHAT;
                    item.OutQuantity = qty;
                    item.Qty = qty;

                    //KMx: Không sử dụng biến HiApplied nữa, thay thế bằng IsCountHI và được set trong hàm CalcInvoiceItem (12/12/2014 09:07).
                    //if (CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0 && CurRegistration.HisID.GetValueOrDefault(0) > 0)
                    //{
                    //    item.HiApplied = true;
                    //}
                    //else
                    //{
                    //    item.HiApplied = false;
                    //}
                    if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices != null)
                    {
                        var lstdrugDetails = EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Where(x => x.MedProductType == AllLookupValues.MedProductType.HOA_CHAT).SelectMany(x => x.OutwardDrugClinicDepts).Where(owd => owd.GenMedProductItem.GenMedProductID == item.GenMedProductItem.GenMedProductID);
                        if (lstdrugDetails != null && lstdrugDetails.Count() > 0)
                        {
                            if (MessageBox.Show(string.Format("{0} {1}", item.GenMedProductItem.BrandName, eHCMSResources.W0949_G1_W), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                            {
                                return;
                            }
                        }
                        //if (lstdrugDetails != null)
                        //{
                        //    foreach (var row in lstdrugDetails)
                        //    {
                        //        if (row.GenMedProductItem.GenMedProductID == item.GenMedProductItem.GenMedProductID)
                        //        {
                        //            if (MessageBox.Show(row.GenMedProductItem.BrandName + " đã tồn tại. Bạn có muốn tiếp tục thêm không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        //            {
                        //                return;
                        //            }
                        //        }
                        //    }

                        //}
                    }

                    if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices == null)
                    {
                        EditingBillingInvoice.OutwardDrugClinicDeptInvoices = new ObservableCollection<OutwardDrugClinicDeptInvoice>();
                    }
                    if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Where(p => p.MedProductType == AllLookupValues.MedProductType.HOA_CHAT
                                                                    && p.outiID <= 0
                                                                    && p.CreatedDate.Date == createdDate.Date
                                                                    && p.StoreID == curItem.StoreID
                                                                    && p.SelectedStorage.StoreID == _selectedWarehouse.StoreID).FirstOrDefault() == null)
                    {

                        EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Add(new OutwardDrugClinicDeptInvoice()
                        {
                            OutwardDrugClinicDepts = new ObservableCollection<OutwardDrugClinicDept>(),
                            MedProductType = AllLookupValues.MedProductType.HOA_CHAT,
                            OutDate = createdDate,
                            StaffID = Globals.LoggedUserAccount.StaffID,
                            StoreID = curItem.StoreID,
                            SelectedStorage = curItem.Storage
                        }
                        );
                    }
                    var lastInvoice = EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Last();

                    item.DrugInvoice = lastInvoice;
                    CalcInvoiceItem(item);
                    lastInvoice.OutwardDrugClinicDepts.Add(item);
                    AddMedRegItemBaseToNewInvoice(item);

                    RegistrationInfoHasChanged = true;
                }
            }
        }


        public void AddDefaultPCLRequest(long medServiceID, int qty)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginCreateNewPCLRequest(medServiceID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                PatientPCLRequest item = null;
                                PatientPCLRequest externalRequest = null;

                                try
                                {
                                    contract.EndCreateNewPCLRequest(out item, out externalRequest, asyncResult);

                                    ObservableCollection<PCLExamType> PCLExamTypeList = new ObservableCollection<PCLExamType>();
                                    ObservableCollection<PCLExamType> PCLExamTypeList_Ext = new ObservableCollection<PCLExamType>();

                                    if (item != null && item.PatientPCLRequestIndicators != null)
                                    {
                                        foreach (var requestDetails in item.PatientPCLRequestIndicators)
                                        {
                                            PCLExamTypeList.Add(requestDetails.PCLExamType);
                                        }
                                        if (PCLExamTypeList != null && PCLExamTypeList.Count > 0)
                                        {
                                            CheckAndAddAllPCL(PCLExamTypeList, qty, SelectedDate.Value, false);
                                        }
                                    }

                                    if (externalRequest != null && externalRequest.PatientPCLRequestIndicators != null)
                                    {
                                        foreach (var requestDetails in externalRequest.PatientPCLRequestIndicators)
                                        {
                                            PCLExamTypeList_Ext.Add(requestDetails.PCLExamType);
                                        }
                                        if (PCLExamTypeList_Ext != null && PCLExamTypeList_Ext.Count > 0)
                                        {
                                            CheckAndAddAllPCL(PCLExamTypeList_Ext, qty, SelectedDate.Value, false);
                                        }
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                    this.HideBusyIndicator();
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    this.HideBusyIndicator();
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        //KMx: Lỗi 1: Khi thêm 1 món, mà món đó đã có 2 dòng rồi, thì cảnh báo món đó 2 lần. Lỗi 2: Người dùng nhập SL > 1 mà chỉ add vào 1. Không sử dụng hàm này nữa (15/12/2014 11:32).
        //public void AddDefaultPCLRequest(long medServiceID)
        //{
        //    var t = new Thread(() =>
        //    {
        //        Globals.EventAggregator.Publish(new BusyEvent
        //        {
        //            IsBusy = true,
        //            Message = "Đang thêm các dịch vụ Cận Lâm Sàng..."
        //        });
        //        try
        //        {
        //            using (var serviceFactory = new PatientRegistrationServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginCreateNewPCLRequest(medServiceID,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        PatientPCLRequest item = null;
        //                        PatientPCLRequest externalRequest = null;
        //                        try
        //                        {
        //                            contract.EndCreateNewPCLRequest(out item, out externalRequest, asyncResult);
        //                            string Mgsshow = "";

        //                            if (item != null && item.PatientPCLRequestIndicators != null)
        //                            {
        //                                if (EditingBillingInvoice.PclRequests != null)
        //                                {
        //                                    var lstpcls = EditingBillingInvoice.PclRequests.SelectMany(x => x.PatientPCLRequestIndicators);
        //                                    if (lstpcls != null)
        //                                    {
        //                                        foreach (var row1 in lstpcls)
        //                                        {
        //                                            foreach (var row2 in item.PatientPCLRequestIndicators)
        //                                            {
        //                                                if (row1.PCLExamType.PCLExamTypeID == row2.PCLExamType.PCLExamTypeID)
        //                                                {
        //                                                    Mgsshow = Mgsshow + row1.PCLExamType.PCLExamTypeName + ",";
        //                                                    break;
        //                                                }
        //                                            }
        //                                        }
        //                                        if (!string.IsNullOrEmpty(Mgsshow))
        //                                        {
        //                                            if (MessageBox.Show(Mgsshow + " đã tồn tại. Bạn có muốn tiếp tục thêm không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
        //                                            {
        //                                                return;
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                foreach (var requestDetails in item.PatientPCLRequestIndicators)
        //                                {
        //                                    AddPCLItem_Goi(requestDetails.PCLExamType, 1, SelectedDate.Value, false);
        //                                }
        //                            }

        //                            if (externalRequest != null && externalRequest.PatientPCLRequestIndicators != null)
        //                            {
        //                                if (EditingBillingInvoice.PclRequests != null)
        //                                {
        //                                    var lstpcls = EditingBillingInvoice.PclRequests.SelectMany(x => x.PatientPCLRequestIndicators);
        //                                    if (lstpcls != null)
        //                                    {
        //                                        foreach (var row1 in lstpcls)
        //                                        {
        //                                            foreach (var row2 in externalRequest.PatientPCLRequestIndicators)
        //                                            {
        //                                                if (row1.PCLExamType.PCLExamTypeID == row2.PCLExamType.PCLExamTypeID)
        //                                                {
        //                                                    Mgsshow = Mgsshow + row1.PCLExamType.PCLExamTypeName + ",";
        //                                                    break;
        //                                                }
        //                                            }
        //                                        }
        //                                        if (!string.IsNullOrEmpty(Mgsshow))
        //                                        {
        //                                            if (MessageBox.Show(Mgsshow + " đã tồn tại.Bạn có muốn tiếp tục thêm không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
        //                                            {
        //                                                return;
        //                                            }
        //                                        }
        //                                    }
        //                                }

        //                                foreach (var requestDetails in externalRequest.PatientPCLRequestIndicators)
        //                                {
        //                                    AddPCLItem_Goi(requestDetails.PCLExamType, 1, SelectedDate.Value, false);
        //                                }
        //                            }
        //                        }
        //                        catch (FaultException<AxException> fault)
        //                        {
        //                            ClientLoggerHelper.LogInfo(fault.ToString());
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            ClientLoggerHelper.LogInfo(ex.ToString());
        //                        }

        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //        }
        //        finally
        //        {
        //            Globals.IsBusy = false;
        //        }
        //    });
        //    t.Start();
        //}

        public void Handle(ItemSelected<Patient> message)
        {
            if (this.GetView() != null && message != null)
            {
                CurPatient = message.Item;
                CurRegistration = null;
                if (CurPatient != null)
                {
                    SetCurrentPatient(CurPatient);
                }
            }
        }
        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (this.GetView() != null && message != null && message.Item != null)
            {
                OpenRegistration(message.Item.PtRegistrationID);
            }
        }

        public void Handle(ResultNotFound<Patient> message)
        {
            if (this.GetView() != null && message != null)
            {
                //Thông báo không tìm thấy bệnh nhân.
                MessageBoxResult result = MessageBox.Show(eHCMSResources.A0728_G1_Msg_ConfTiepTucTimBN,eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    Action<IFindPatient> onInitDlg = delegate (IFindPatient vm)
                    {
                        vm.SearchCriteria = message.SearchCriteria as PatientSearchCriteria;
                        var criteria = message.SearchCriteria as PatientSearchCriteria;
                        vm.SearchCriteria = criteria;
                    };
                    GlobalsNAV.ShowDialog<IFindPatient>(onInitDlg);
                }
            }
        }

        public void Handle(PayForRegistrationCompleted message)
        {
            if (this.GetView() == null || message == null)
            {
                return;
            }
            //Load lai dang ky:
            var payment = message.Payment;
            if (payment != null && payment.PatientTransaction != null && payment.PatientTransaction.PtRegistrationID.HasValue)
            {
                //Show Report:
                Action<IPaymentReport> onInitDlg = delegate (IPaymentReport reportVm)
                {
                    reportVm.PaymentID = payment.PtTranPaymtID;
                };
                GlobalsNAV.ShowDialog<IPaymentReport>(onInitDlg);

                OpenRegistration(payment.PatientTransaction.PtRegistrationID.Value);
            }
        }

        public void Handle(SaveAndPayForRegistrationCompleted message)
        {
            if (this.GetView() != null && message != null)
            {
                //Load lai dang ky:
                var payment = message.Payment;
                if (payment != null && payment.PatientTransaction != null && payment.PatientTransaction.PtRegistrationID.HasValue)
                {
                    //Show Report:
                    Action<IPaymentReport> onInitDlg = delegate (IPaymentReport reportVm)
                    {
                        reportVm.PaymentID = payment.PtTranPaymtID;
                    };
                    GlobalsNAV.ShowDialog<IPaymentReport>(onInitDlg);

                    if (message.RegistrationInfo != null)
                    {
                        ShowOldRegistration(message.RegistrationInfo);
                    }
                    else
                    {
                        OpenRegistration(payment.PatientTransaction.PtRegistrationID.Value);
                    }
                }
            }
        }
        //public void ConfirmHiItem(object hiItem)
        //{
        //    ConfirmedHiItem = hiItem as HealthInsurance;
        //    if (CurRegistration != null)
        //    {
        //        CurRegistration.HealthInsurance = ConfirmedHiItem;
        //    }

        //}
        //public void ConfirmPaperReferal(object referal)
        //{
        //    ConfirmedPaperReferal = referal as PaperReferal;

        //}

        public void Handle(ItemSelected<RefMedicalServiceItem> message)
        {
            if (this.GetView() != null)
            {
                //Reset so luong dich vu dang ky = 1
                ServiceQty = 1;
            }
        }
        public void AddServiceCmd()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidateServiceItem(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }

            AddGenMedService(InPatientSelectServiceContent.MedServiceItem, ServiceQty.Value, SelectedDate.Value);
        }
        private bool ValidateServiceItem(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (!SelectedDate.HasValue)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0161_G1_HayChonNgSDDV, new string[] { "SelectedDate" });
                result.Add(item);
            }

            if (InPatientSelectServiceContent.MedServiceItem == null || InPatientSelectServiceContent.MedServiceItem.MedServiceID < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0162_G1_HayChonDV, new string[] { "MedServiceItem" });
                result.Add(item);
            }
            if (ServiceQty.GetValueOrDefault(0) < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0163_G1_NhapGTriSLgDV, new string[] { "ServiceQty" });
                result.Add(item);
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }

        public void Handle(ItemSelected<PCLExamType> message)
        {
            if (this.GetView() != null)
            {
                PclQty = 1;
                PclQtyLAB = 1;
            }
        }
        public void Handle(DoubleClick message)
        {
            if (message.Source != SelectPCLContent)
            {
                return;
            }
            if (message.EventArgs.Value != SelectPCLContent.SelectedPCLExamType)
            {
                return;
            }
            AddPclExamTypeCmd();
        }
        public void AddPclExamTypeCmd()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidatePclItem(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }
            //bool used = SelectPCLContent != null && SelectPCLContent.Used;

            bool used = true;/*A Tuan quyet dinh lam phieu CLS noi tru tai man hinh nay thi la da thuc hien roi*/

            //AddPCLItem(SelectPCLContent.SelectedPCLExamType, PclQty.Value, SelectedDate.Value, used);
            CheckAndAddPCL(SelectPCLContent.SelectedPCLExamType, PclQty.Value, SelectedDate.Value, used);
        }
        private bool ValidatePclItem(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (!SelectedDate.HasValue)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0161_G1_HayChonNgSDDV, new string[] { "SelectedDate" });
                result.Add(item);
            }

            if (SelectPCLContent.SelectedPCLExamType == null || SelectPCLContent.SelectedPCLExamType.PCLExamTypeID < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0164_G1_HayChonDVCLS, new string[] { "SelectedPclExamType" });
                result.Add(item);
            }
            if (PclQty.GetValueOrDefault(0) < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0163_G1_NhapGTriSLgDV, new string[] { "PclQty" });
                result.Add(item);
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }

        public void AddDrugItemCmd()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidateDrugItem(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }

            AddGenMedDrug(SelectDrugContent.SelectedMedProduct, DrugQty.Value, SelectedDate.Value);
        }
        private bool ValidateDrugItem(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (!SelectedDate.HasValue)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0161_G1_HayChonNgSDDV, new string[] { "SelectedDate" });
                result.Add(item);
            }

            if (SelectDrugContent.SelectedMedProduct == null || SelectDrugContent.SelectedMedProduct.GenMedProductID < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0165_G1_HayChonThuoc, new string[] { "SelectedDrug" });
                result.Add(item);
            }
            if (DrugQty.GetValueOrDefault(0) < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0163_G1_NhapGTriSLgDV, new string[] { "DrugQty" });
                result.Add(item);
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }

        public void Handle(ItemSelected<RefGenMedProductDetails> message)
        {
            if (this.GetView() != null && message.Item != null && message.Item != null)
            {
                long type = message.Item.V_MedProductType;
                if (type == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    MedItemQty = 1;
                    AddMedItemCmd();
                }
                else if (type == (long)AllLookupValues.MedProductType.HOA_CHAT)
                {
                    ChemicalQty = 1;
                    AddChemicalCmd();
                }
                else if (type == (long)AllLookupValues.MedProductType.THUOC)
                {
                    DrugQty = 1;
                    AddDrugItemCmd();
                   
                }
            }

        }
        public void AddMedItemCmd()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidateMedItem(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }

            AddGenMedItem(MedItemContent.SelectedMedProduct, MedItemQty.Value, SelectedDate.Value);
        }
        private bool ValidateMedItem(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (!SelectedDate.HasValue)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0161_G1_HayChonNgSDDV, new string[] { "SelectedDate" });
                result.Add(item);
            }

            if (MedItemContent.SelectedMedProduct == null || MedItemContent.SelectedMedProduct.GenMedProductID < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0166_G1_HayChonYCu, new string[] { "SelectedInwardDrug" });
                result.Add(item);
            }
            if (MedItemQty.GetValueOrDefault(0) < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0167_G1_NhapGTriSLgYCu, new string[] { "MedItemQty" });
                result.Add(item);
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }
        public void AddChemicalCmd()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidateChemicalItem(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }

            AddGenChemical(ChemicalItemContent.SelectedMedProduct, ChemicalQty.Value, SelectedDate.Value);
        }
        private bool ValidateChemicalItem(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (!SelectedDate.HasValue)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0161_G1_HayChonNgSDDV, new string[] { "SelectedDate" });
                result.Add(item);
            }

            if (ChemicalItemContent.SelectedMedProduct == null || ChemicalItemContent.SelectedMedProduct.GenMedProductID < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0168_G1_HayChonHC, new string[] { "SelectedInwardDrug" });
                result.Add(item);
            }
            if (ChemicalQty.GetValueOrDefault(0) < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0169_G1_NhapGTriSLgHC, new string[] { "ChemicalQty" });
                result.Add(item);
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }

        public bool ValidateRegistrationInfo(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (PatientSummaryInfoContent.CurrentPatient == null)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0148_G1_HayChon1BN, new string[] { "CurrentPatient" });
                result.Add(item);
            }

            if (CurClassification == null)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0154_G1_HayChonLoaiBN, new string[] { "CurClassification" });
                result.Add(item);
            }
            if (CurRegistration.ExamDate == DateTime.MinValue)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0154_G1_NgDKKhongHopLe, new string[] { "ExamDate" });
                result.Add(item);
            }

            if ((long)EditingBillingInvoice.V_BillingInvType < 0)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0155_G1_HayChonLoaiTToan, new string[] { "BillingType" });
                result.Add(item);
            }

            CurRegistration.PatientClassification = CurClassification;
            if (ConfirmedPaperReferal != null)
            {
                CurRegistration.PaperReferal = ConfirmedPaperReferal;
            }


            if (EditingInvoiceDetailsContent.BillingInvoice == null)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0156_G1_Chon1DV, new string[] { "AllRegistrationItems" });
                result.Add(item);
            }

            if (CurRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0085_G1_DayKhongPhaiDKNoiTru, new string[] { "AllRegistrationItems" });
                result.Add(item);
            }

            CurRegistration.StaffID = Globals.LoggedUserAccount.StaffID;

            if (HiServiceBeingUsed)
            {
                //Dang la benh nhan bao hiem.
                //Kiem tra neu chua confirm the bao hiem thi thong bao loi.
                if (ConfirmedHiItem == null)
                {
                    var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0157_G1_ChuaKTraTheBH, new string[] { "ConfirmedHiItem" });
                    result.Add(item);
                }
                CurRegistration.HealthInsurance = ConfirmedHiItem;

                long? hisID = null;
                if (ConfirmedHiItem != null
                    && ConfirmedHiItem.HealthInsuranceHistories != null
                    && ConfirmedHiItem.HealthInsuranceHistories.Count > 0)
                {
                    hisID = ConfirmedHiItem.HealthInsuranceHistories[0].HisID;
                }

                foreach (PatientRegistrationDetail d in CurRegistration.PatientRegistrationDetails)
                {
                    d.HisID = hisID;
                }
                CurRegistration.HisID = hisID;
            }

            CurRegistration.PatientID = PatientSummaryInfoContent.CurrentPatient.PatientID;
            CurRegistration.RegTypeID = (byte)PatientRegistrationType.DK_KHAM_BENH_NOI_TRU;
            CurRegistration.RefDepartment = Globals.ObjRefDepartment == null ? null : Globals.ObjRefDepartment;

            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }
        public bool CanSaveBillingInvoiceCmd
        {
            get
            {
                return RegistrationInfoHasChanged;
            }
        }
        public void SaveBillingInvoiceCmd()
        {
            if (isDischarged)
            {
                MessageBox.Show(eHCMSResources.A0240_G1_Msg_InfoBNDaXV_KTheLuuBill, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (Globals.isAccountCheck && CurRegistration != null && CurRegistration.AdmissionInfo != null)
            {
                int NumOfDayAllowSaveBillAfterDischarge = Globals.ServerConfigSection.InRegisElements.NumOfDayAllowSaveBillAfterDischarge;
                DateTime? DischargeDetailRecCreatedDate = CurRegistration.AdmissionInfo.DischargeDetailRecCreatedDate;
                if (NumOfDayAllowSaveBillAfterDischarge > 0 && DischargeDetailRecCreatedDate != null
                    && (Globals.GetCurServerDateTime().Date - DischargeDetailRecCreatedDate.GetValueOrDefault().Date).TotalDays > NumOfDayAllowSaveBillAfterDischarge)
                {
                    MessageBox.Show(string.Format("{0} {1} {2} \n{3} {4}.", eHCMSResources.A0845_G1_Msg_InfoCBaoNgLuuBill, NumOfDayAllowSaveBillAfterDischarge.ToString(), eHCMSResources.N0045_G1_Ng
                                , eHCMSResources.N0132_G1_NgXuatVien, DischargeDetailRecCreatedDate.GetValueOrDefault().ToString("dd/MM/yyyy hh:mm:ss tt")), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
            }

            if (!CheckDeptID())
            {
                return;
            }
            SaveBillingInvoice();
        }
        private void SaveBillingInvoice()
        {
            if (!CanAddEditBill)
            {
                MessageBox.Show(eHCMSResources.A0701_G1_Msg_InfoLuuFail);
                return;
            }
            if (EditingBillingInvoice == null)
            {
                return;
            }

            if (EditingBillingInvoice.BillToDate.GetValueOrDefault().Date > Globals.GetCurServerDateTime().Date)
            {
                MessageBox.Show(eHCMSResources.A0623_G1_Msg_InfoTGianTaoBillKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (EditingBillingInvoice.BillFromDate.GetValueOrDefault().Date > EditingBillingInvoice.BillToDate.GetValueOrDefault().Date)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0454_G1_KhoangTGianTaoBillKgHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //Check Phòng PCL
            //if(CheckDeptLocationPCL()==false)
            //    return; 
            //Check Phòng PCL

            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults = null;
            if (!EditingInvoiceDetailsContent.ValidateInfo(out validationResults))
            {
                Action<IValidationError> onInitDlg = delegate (IValidationError errorVm)
                {
                    errorVm.SetErrors(validationResults);
                };
                GlobalsNAV.ShowDialog<IValidationError>(onInitDlg);
                return;
            }

            CurRegistration.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;

            if (DepartmentContent != null && DepartmentContent.SelectedItem != null
                && DepartmentContent.SelectedItem.SelectDeptReqSelectRoom)
            {
                if (SelLocationInDept != null && SelLocationInDept.DeptLocationID > 0)
                {
                    CurRegistration.ReqFromDeptLocID = SelLocationInDept.DeptLocationID;
                }
            }


            EditingBillingInvoice.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();

            //if (IsEditing == false)

            //KMx: Load bill cũng phải sử dụng hàm add, nếu không sẽ không lưu những món thêm tay vô (20/08/2014 15:47).
            //if (IsLoadNoBill)
            //{
            //    CreateBillingInvoiceFromExistingItems();
            //    IsLoadNoBill = false;
            //}
            //else
            //{
                if (EditingBillingInvoice.InPatientBillingInvID <= 0)
                {
                    AddBillingInvoice();
                }
                else
                {
                    UpdateBillingInvoice();
                }
            //}
        }

        private void AdditionalFee()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginInsertAdditionalFee(EditingBillingInvoice.InPatientBillingInvID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool result = contract.EndInsertAdditionalFee(asyncResult);
                                    if (result)
                                    {
                                        MessageBox.Show(eHCMSResources.A1027_G1_Msg_InfoThemOK);
                                        RegistrationInfoHasChanged = false;
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail);
                                        RegistrationInfoHasChanged = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                finally
                                {

                                    EditingInvoiceDetailsContent.LoadDetails(UpdateBillingInvoiceCompletedCallback);

                                    IsEditing = false;
                                    RegistrationInfoHasChanged = false;

                                    this.HideBusyIndicator();
                                }

                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }

            });
            t.Start();
        }


        public void AdditionalFeeCmd()
        {
            if (EditingBillingInvoice == null || EditingBillingInvoice.InPatientBillingInvID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0628_G1_Msg_InfoKhTheTinhPhuThu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (MessageBox.Show(eHCMSResources.A0150_G1_Msg_ConfTinhPhuThuBill, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                AdditionalFee();
            }
        }

        //private bool CheckDeptLocationPCL()
        //{
        //    if(EditingBillingInvoice!=null && EditingBillingInvoice.PclRequests!=null)
        //    {
        //        foreach (var request in EditingBillingInvoice.PclRequests)
        //        {
        //            if (request.PatientPCLRequestIndicators != null)
        //            {
        //                foreach (var requestDetail in request.PatientPCLRequestIndicators)
        //                {
        //                    if(requestDetail.PCLExamType!=null)
        //                    {
        //                        if(requestDetail.DeptLocation==null || requestDetail.DeptLocation.DeptLocationID<=0)
        //                        {
        //                            MessageBox.Show("Chọn Phòng cho dịch vụ Cận Lâm Sàng!");
        //                            return false;
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //    }
        //    return true;
        //}


        //Tao Bill tu items load len
        private void CreateBillingInvoiceFromExistingItems()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidateRegistrationInfo(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }
            IsSaving = true;

            var bCanSave = false;

            if (EditingBillingInvoice.RegistrationDetails != null && EditingBillingInvoice.RegistrationDetails.Count > 0)
            {
                bCanSave = true;
            }

            if (!bCanSave && EditingBillingInvoice.PclRequests != null && EditingBillingInvoice.PclRequests.Count > 0)
            {
                bCanSave = true;
            }
            if (!bCanSave && EditingBillingInvoice.OutwardDrugClinicDeptInvoices != null)
            {
                if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Any(inv => inv.OutwardDrugClinicDepts != null && inv.OutwardDrugClinicDepts.Count > 0))
                {
                    bCanSave = true;
                }
            }
            if (!bCanSave)
            {
                MessageBox.Show(eHCMSResources.A0697_G1_Msg_InfoKhTheLuuBill);
                IsSaving = false;
                return;
            }

            IsSaving = true;

            if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices != null)
            {
                foreach (var inv in EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Where(inv => inv.RecordState == RecordState.DETACHED))
                {
                    inv.Confirmed = MedRegItemConfirmed;
                }
            }

            if (EditingBillingInvoice.RegistrationDetails != null)
            {
                foreach (var item in EditingBillingInvoice.RegistrationDetails)
                {
                    item.ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
                }
            }

            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                IsSaving = true;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginCreateBillingInvoiceFromExistingItems(CurRegistration, EditingBillingInvoice,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                long NewBillingInvoiceID = 0;

                                try
                                {
                                    contract.EndCreateBillingInvoiceFromExistingItems(out NewBillingInvoiceID, asyncResult);

                                    if (NewBillingInvoiceID > 0)
                                    {
                                        //Khoa man hinh lai
                                        EditingInvoiceDetailsContent.BillingInvoice.RegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                                        EditingInvoiceDetailsContent.BillingInvoice.PclRequests = new ObservableCollection<PatientPCLRequest>();
                                        EditingInvoiceDetailsContent.BillingInvoice.OutwardDrugClinicDeptInvoices = new ObservableCollection<OutwardDrugClinicDeptInvoice>();
                                        EditingInvoiceDetailsContent.ResetView();
                                        BeginEdit();
                                        //Khoa man hinh lai

                                        //Load lai DS Bill Cu
                                        OpenRegistration(CurRegistration.PtRegistrationID);
                                        //Load lai DS Bill Cu

                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK);

                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A0701_G1_Msg_InfoLuuFail);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                }
                                finally
                                {
                                    IsSaving = false;
                                }


                            }), EditingBillingInvoice);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsSaving = false;
                    if (error != null)
                    {
                        Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                    }
                }

            });
            t.Start();
        }
        //Tao Bill tu items load len


        private void UpdateBillingInvoice()
        {
            ////IsSaving = true;
            ////this.ShowBusyIndicator();
            //if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices != null)
            //{
            //    foreach (var inv in EditingBillingInvoice.OutwardDrugClinicDeptInvoices)
            //    {
            //        if (inv.RecordState == RecordState.DETACHED)
            //        {
            //            inv.Confirmed = MedRegItemConfirmed;
            //        }
            //    }
            //}

            //string strError = "";
            //string strItemInvalid = "";

            //DateTime BillFromDate = EditingBillingInvoice.BillFromDate.GetValueOrDefault();
            //DateTime BillToDate = EditingBillingInvoice.BillToDate.GetValueOrDefault();
            //// Hpt 23/09/2015: Sửa lại hàm này vì ViewModel này dùng chung cho cả đăng ký Vãng Lai và Tiền Giải Phẫu, mà đăng ký loại VL và TGP thì không nhập viện nên k có ngày nhập viện
            //// Vì vậy phải gán AdmissionDate = ngày đăng ký, sau đó nếu có nhập viện thì set lại cho đúng
            //DateTime AdmissionDate = CurRegistration.ExamDate;
            //if (CurRegistration.AdmissionInfo != null)
            //{
            //    AdmissionDate = CurRegistration.AdmissionInfo.AdmissionDate.GetValueOrDefault();
            //}

            //DateTime Now = Globals.GetCurServerDateTime();

            //var newRegDetails = new List<PatientRegistrationDetail>();
            //var modifiedRegDetails = new List<PatientRegistrationDetail>();
            //var deletedRegDetails = new List<PatientRegistrationDetail>();

            //if (EditingBillingInvoice.RegistrationDetails != null)
            //{
            //    List<long> existDetailID = new List<long>();
            //    List<long> existItemInvalidID = new List<long>();

            //    foreach (var registrationDetail in EditingBillingInvoice.RegistrationDetails)
            //    {
            //        switch (registrationDetail.RecordState)
            //        {
            //            case RecordState.DELETED:
            //                deletedRegDetails.Add(registrationDetail);
            //                break;
            //            case RecordState.MODIFIED:
            //                modifiedRegDetails.Add(registrationDetail);
            //                break;
            //            case RecordState.DETACHED:
            //                if (registrationDetail.RefMedicalServiceItem == null || registrationDetail.RefMedicalServiceItem.MedServiceID <= 0)
            //                {
            //                    continue;
            //                }
            //                if ((registrationDetail.CreatedDate.Date < BillFromDate.Date || registrationDetail.CreatedDate.Date > BillToDate.Date) && !existDetailID.Contains(registrationDetail.RefMedicalServiceItem.MedServiceID))
            //                {
            //                    existDetailID.Add(registrationDetail.RefMedicalServiceItem.MedServiceID);

            //                    strError += "     " + registrationDetail.ChargeableItemName + "." + Environment.NewLine;
            //                }
            //                if ((registrationDetail.CreatedDate.Date < AdmissionDate.Date || registrationDetail.CreatedDate.Date > Now.Date) && !existItemInvalidID.Contains(registrationDetail.RefMedicalServiceItem.MedServiceID))
            //                {
            //                    existItemInvalidID.Add(registrationDetail.RefMedicalServiceItem.MedServiceID);

            //                    strItemInvalid += "     " + registrationDetail.ChargeableItemName + "." + Environment.NewLine;
            //                }

            //                newRegDetails.Add(registrationDetail);
            //                break;
            //        }
            //    }
            //}

            //var newPclRequests = new List<PatientPCLRequest>();
            //var newPclRequestDetails = new List<PatientPCLRequestDetail>();
            //var deletedPclRequestDetails = new List<PatientPCLRequestDetail>();
            //var modifiedPclRequestDetails = new List<PatientPCLRequestDetail>();

            //if (EditingBillingInvoice.PclRequests != null)
            //{
            //    List<long> existPCLID = new List<long>();
            //    List<long> existPCLInvalidID = new List<long>();

            //    foreach (var request in EditingBillingInvoice.PclRequests)
            //    {
            //        if (request.RecordState == RecordState.DETACHED)
            //        {
            //            if (request.PatientPCLRequestIndicators == null || request.PatientPCLRequestIndicators.Count <= 0)
            //            {
            //                continue;
            //            }

            //            foreach (PatientPCLRequestDetail child in request.PatientPCLRequestIndicators)
            //            {
            //                if (child.PCLExamType == null || child.PCLExamType.PCLExamTypeID <= 0)
            //                {
            //                    continue;
            //                }

            //                if ((child.CreatedDate.Date < BillFromDate.Date || child.CreatedDate.Date > BillToDate.Date) && !existPCLID.Contains(child.PCLExamType.PCLExamTypeID))
            //                {
            //                    existPCLID.Add(child.PCLExamType.PCLExamTypeID);

            //                    strError += "     " + child.ChargeableItemName + "." + Environment.NewLine;
            //                }
            //                if ((child.CreatedDate.Date < AdmissionDate.Date || child.CreatedDate.Date > Now.Date) && !existPCLInvalidID.Contains(child.PCLExamType.PCLExamTypeID))
            //                {
            //                    existPCLInvalidID.Add(child.PCLExamType.PCLExamTypeID);

            //                    strItemInvalid += "     " + child.ChargeableItemName + "." + Environment.NewLine;
            //                }
            //            }

            //            newPclRequests.Add(request);
            //        }
            //        else if (request.RecordState == RecordState.MODIFIED)
            //        {
            //            if (request.PatientPCLRequestIndicators != null)
            //            {
            //                foreach (var requestDetail in request.PatientPCLRequestIndicators)
            //                {
            //                    if (requestDetail.RecordState == RecordState.DELETED)
            //                    {
            //                        deletedPclRequestDetails.Add(requestDetail);
            //                    }
            //                    else if (requestDetail.RecordState == RecordState.DETACHED)
            //                    {
            //                        newPclRequestDetails.Add(requestDetail);
            //                    }
            //                    else if (requestDetail.RecordState == RecordState.MODIFIED)
            //                    {
            //                        modifiedPclRequestDetails.Add(requestDetail);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //var newOutwardDrugClinicDeptInvoices = new List<OutwardDrugClinicDeptInvoice>();
            //var modifiedOutwardDrugClinicDeptInvoices = new List<OutwardDrugClinicDeptInvoice>();
            ////KMx: List những invoices xóa khỏi bill để kho phòng chỉnh sửa phiếu xuất (20/08/2014 11:00).
            //var deleteOutwardDrugClinicDeptInvoices = new List<OutwardDrugClinicDeptInvoice>();

            //if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices != null)
            //{
            //    foreach (var inv in EditingBillingInvoice.OutwardDrugClinicDeptInvoices)
            //    {
            //        if (inv.RecordState == RecordState.DETACHED)
            //        {
            //            newOutwardDrugClinicDeptInvoices.Add(inv);
            //        }
            //        else if (inv.RecordState == RecordState.MODIFIED)
            //        {
            //            modifiedOutwardDrugClinicDeptInvoices.Add(inv);
            //        }
            //        else if (inv.RecordState == RecordState.DELETED)
            //        {
            //            deleteOutwardDrugClinicDeptInvoices.Add(inv);
            //        }
            //    }
            //}

            //var bCanSave = false;
            //if (newRegDetails.Count > 0 || deletedRegDetails.Count > 0 || newPclRequests.Count > 0
            //|| newPclRequestDetails.Count > 0 || deletedPclRequestDetails.Count > 0 || modifiedRegDetails.Count > 0 || modifiedPclRequestDetails.Count > 0)
            //{
            //    bCanSave = true;
            //}

            //if (!bCanSave && newOutwardDrugClinicDeptInvoices.Count > 0)
            //{
            //    if (newOutwardDrugClinicDeptInvoices.Any(inv => inv.OutwardDrugClinicDepts != null && inv.OutwardDrugClinicDepts.Count > 0))
            //    {
            //        bCanSave = true;
            //    }
            //}
            //if (!bCanSave && modifiedOutwardDrugClinicDeptInvoices.Count > 0)
            //{
            //    if (modifiedOutwardDrugClinicDeptInvoices.Any(inv => inv.OutwardDrugClinicDepts != null && inv.OutwardDrugClinicDepts.Count > 0))
            //    {
            //        bCanSave = true;
            //    }
            //}
            //if (!bCanSave && deleteOutwardDrugClinicDeptInvoices.Count > 0)
            //{
            //    bCanSave = true;
            //}

            ////KMx: Nếu DV, CLS, Thuốc không có gì thay đổi thì vẫn cho cập nhật, vì có thể user muốn cập nhật Bill từ ngày, đến ngày (07/01/2015 16:32).
            ////if (!bCanSave)
            ////{
            ////    MessageBox.Show("Không thể lưu bill.");
            ////    //IsSaving = false;
            ////    this.HideBusyIndicator();
            ////    return;
            ////}

            //if (!string.IsNullOrEmpty(strItemInvalid))
            //{
            //    MessageBox.Show(string.Format("{0}:", eHCMSResources.Z0456_G1_NgSDKgHopLe) + Environment.NewLine + strItemInvalid + "Ngày sử dụng không được nhỏ hơn ngày nhập viện và không được lớn hơn ngày hiện tại. Ngày nhập viện: " + AdmissionDate.ToShortDateString(), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            //    return;
            //}

            //if (!string.IsNullOrEmpty(strError))
            //{
            //    MessageBoxResult result = MessageBox.Show("Những loại sau đây có ngày sử dụng không nằm trong khoảng thời gian tạo bill:" + Environment.NewLine + strError + "Bạn có đồng ý lưu không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
            //    if (result != MessageBoxResult.OK)
            //    {
            //        return;
            //    }
            //}


            //this.ShowBusyIndicator();
            //var t = new Thread(() =>
            //{
            //    AxErrorEventArgs error = null;
            //    //IsSaving = true;
            //    this.ShowBusyIndicator();
            //    try
            //    {
            //        using (var serviceFactory = new CommonServiceClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;

            //            var updatedInvoice = new InPatientBillingInvoice();
            //            updatedInvoice.InPatientBillingInvID = EditingBillingInvoice.InPatientBillingInvID;
            //            updatedInvoice.InvDate = EditingBillingInvoice.InvDate;
            //            updatedInvoice.PaidTime = EditingBillingInvoice.PaidTime;
            //            updatedInvoice.V_BillingInvType = EditingBillingInvoice.V_BillingInvType;
            //            updatedInvoice.V_InPatientBillingInvStatus = EditingBillingInvoice.V_InPatientBillingInvStatus;
            //            updatedInvoice.PtRegistrationID = EditingBillingInvoice.PtRegistrationID;
            //            updatedInvoice.BillFromDate = EditingBillingInvoice.BillFromDate;
            //            updatedInvoice.BillToDate = EditingBillingInvoice.BillToDate;

            //            contract.BeginUpdateInPatientBillingInvoice(Globals.LoggedUserAccount.StaffID, updatedInvoice, newRegDetails, deletedRegDetails,
            //                newPclRequests, newPclRequestDetails, deletedPclRequestDetails, newOutwardDrugClinicDeptInvoices,
            //                modifiedOutwardDrugClinicDeptInvoices, deleteOutwardDrugClinicDeptInvoices, modifiedRegDetails, modifiedPclRequestDetails,
            //                Globals.DispatchCallback((asyncResult) =>
            //                {
            //                    bool bOK = false;
            //                    //PatientRegistration registration = null;
            //                    Dictionary<long, List<long>> drugIDList_Error = null;
            //                    long regID = updatedInvoice.PtRegistrationID;
            //                    try
            //                    {
            //                        contract.EndUpdateInPatientBillingInvoice(out drugIDList_Error, asyncResult);
            //                        if (drugIDList_Error == null || drugIDList_Error.Count == 0)
            //                        {
            //                            bOK = true;
            //                            RegistrationInfoHasChanged = false;
            //                        }
            //                        else
            //                        {
            //                            bOK = false;
            //                        }
            //                    }
            //                    //catch (FaultException<AxException> fault)
            //                    //{
            //                    //    bOK = false;
            //                    //    error = new AxErrorEventArgs(fault);
            //                    //}
            //                    catch (Exception ex)
            //                    {
            //                        bOK = false;
            //                        error = new AxErrorEventArgs(ex);
            //                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            //                    }
            //                    finally
            //                    {
            //                        if (drugIDList_Error != null && drugIDList_Error.Count > 0)
            //                        {
            //                            var vm = Globals.GetViewModel<IMedProductRemaingListing>();
            //                            vm.StartLoadingByIdList(drugIDList_Error);
            //                            vm.Title = eHCMSResources.K2948_G1_DSLgoaiThuocKhongTheLuu;
            //                            Globals.ShowDialog(vm as Conductor<object>);
            //                        }
            //                        if (bOK && regID > 0)
            //                        {
            //                            //IsSaving = false;
            //                            //this.HideBusyIndicator();
            //                            EditingInvoiceDetailsContent.LoadDetails(UpdateBillingInvoiceCompletedCallback);

            //                            IsEditing = false;
            //                            RegistrationInfoHasChanged = false;
            //                        }
            //                        this.HideBusyIndicator();
            //                    }


            //                }), updatedInvoice);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        error = new AxErrorEventArgs(ex);
            //        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            //    }
            //    finally
            //    {
            //        //IsSaving = false;
            //        this.HideBusyIndicator();
            //        if (error != null)
            //        {
            //            Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
            //        }
            //    }

            //});
            //t.Start();
        }
        /// <summary>
        /// Thêm mới billing invoice vào database.
        /// </summary>
        private void AddBillingInvoice()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidateRegistrationInfo(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }
            //IsSaving = true;

            DateTime BillFromDate = EditingBillingInvoice.BillFromDate.GetValueOrDefault();
            DateTime BillToDate = EditingBillingInvoice.BillToDate.GetValueOrDefault();
            // Hpt 23/09/2015: Sửa lại hàm này vì ViewModel này dùng chung cho cả đăng ký Vãng Lai và Tiền Giải Phẫu, mà đăng ký loại VL và TGP thì không nhập viện nên k có ngày nhập viện
            // Vì vậy phải gán AdmissionDate = ngày đăng ký, sau đó nếu có nhập viện thì set lại cho đúng
            DateTime AdmissionDate = CurRegistration.ExamDate;
            if (CurRegistration.AdmissionInfo != null)
            {
                AdmissionDate = CurRegistration.AdmissionInfo.AdmissionDate.GetValueOrDefault();
            }

            DateTime Now = Globals.GetCurServerDateTime();


            var bCanSave = false;


            string strError = "";
            string strItemInvalid = "";
            string strItemInvalid2 = "";

            if (EditingBillingInvoice.RegistrationDetails != null && EditingBillingInvoice.RegistrationDetails.Count > 0)
            {
                List<long> existDetailID = new List<long>();

                List<long> existItemInvalidID = new List<long>();
                List<long> existItemInvalidID2 = new List<long>();

                foreach (PatientRegistrationDetail item in EditingBillingInvoice.RegistrationDetails)
                {
                    if (item.RefMedicalServiceItem == null || item.RefMedicalServiceItem.MedServiceID <= 0)
                    {
                        continue;
                    }
                    if ((item.CreatedDate.Date < BillFromDate.Date || item.CreatedDate.Date > BillToDate.Date) && !existDetailID.Contains(item.RefMedicalServiceItem.MedServiceID))
                    {
                        existDetailID.Add(item.RefMedicalServiceItem.MedServiceID);

                        strError += "     " + item.ChargeableItemName + "." + Environment.NewLine;
                    }
                    if (CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.Unknown && (item.CreatedDate.Date < AdmissionDate.Date || item.CreatedDate.Date > Now.Date) && !existItemInvalidID.Contains(item.RefMedicalServiceItem.MedServiceID))
                    {
                        existItemInvalidID.Add(item.RefMedicalServiceItem.MedServiceID);

                        strItemInvalid += "     " + item.ChargeableItemName + "." + Environment.NewLine;
                    }
                    if ((CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT || CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI)
                       && (item.CreatedDate.Date < CurRegistration.ExamDate.Date || item.CreatedDate.Date > Now.Date) && !existItemInvalidID.Contains(item.RefMedicalServiceItem.MedServiceID))
                    {
                        existItemInvalidID2.Add(item.RefMedicalServiceItem.MedServiceID);

                        strItemInvalid2 += "     " + item.ChargeableItemName + ".";
                    }
                }
                bCanSave = true;
            }

            if (EditingBillingInvoice.PclRequests != null && EditingBillingInvoice.PclRequests.Count > 0)
            {
                List<long> existPCLID = new List<long>();

                List<long> existPCLInvalidID = new List<long>();
                List<long> existPCLInvalidID2 = new List<long>();

                foreach (PatientPCLRequest parent in EditingBillingInvoice.PclRequests)
                {
                    if (parent.PatientPCLRequestIndicators == null || parent.PatientPCLRequestIndicators.Count <= 0)
                    {
                        continue;
                    } 

                    foreach (PatientPCLRequestDetail child in parent.PatientPCLRequestIndicators)
                    {
                        if (child.PCLExamType == null || child.PCLExamType.PCLExamTypeID <= 0)
                        {
                            continue;
                        }
                        if ((child.CreatedDate.Date < BillFromDate.Date || child.CreatedDate.Date > BillToDate.Date) && !existPCLID.Contains(child.PCLExamType.PCLExamTypeID))
                        {
                            existPCLID.Add(child.PCLExamType.PCLExamTypeID);

                            strError += "     " + child.ChargeableItemName + "." + Environment.NewLine;
                        }
                        if (CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.Unknown && (child.CreatedDate.Date < AdmissionDate.Date || child.CreatedDate.Date > Now.Date) && !existPCLInvalidID.Contains(child.PCLExamType.PCLExamTypeID))
                        {
                            existPCLInvalidID.Add(child.PCLExamType.PCLExamTypeID);

                            strItemInvalid += "     " + child.ChargeableItemName + "." + Environment.NewLine;
                        }
                        if ((CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT || CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI)
                            && (child.CreatedDate.Date < CurRegistration.ExamDate.Date || child.CreatedDate.Date > Now.Date) && !existPCLInvalidID.Contains(child.PCLExamType.PCLExamTypeID))
                        {
                            existPCLInvalidID2.Add(child.PCLExamType.PCLExamTypeID);

                            strItemInvalid2 += "     " + child.ChargeableItemName + ".";
                        }
                    }
                }

                bCanSave = true;
            }

            if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices != null)
            {
                List<long> existGenMedProdID = new List<long>();

                List<long> existProductInvalidID = new List<long>();
                List<long> existProductInvalidID2 = new List<long>();

                foreach (OutwardDrugClinicDeptInvoice parent in EditingBillingInvoice.OutwardDrugClinicDeptInvoices)
                {
                    if (parent.OutwardDrugClinicDepts == null || parent.OutwardDrugClinicDepts.Count <= 0)
                    {
                        continue;
                    }

                    foreach (OutwardDrugClinicDept child in parent.OutwardDrugClinicDepts)
                    {
                        if (child.GenMedProductItem == null || child.GenMedProductItem.GenMedProductID <= 0)
                        {
                            continue;
                        }
                        if ((child.CreatedDate.Date < BillFromDate.Date || child.CreatedDate.Date > BillToDate.Date) && !existGenMedProdID.Contains(child.GenMedProductItem.GenMedProductID))
                        {
                            existGenMedProdID.Add(child.GenMedProductItem.GenMedProductID);

                            strError += "     " + child.ChargeableItemName + "." + Environment.NewLine;
                        }
                        if (CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.Unknown && (child.CreatedDate.Date < AdmissionDate.Date || child.CreatedDate.Date > Now.Date) && !existProductInvalidID.Contains(child.GenMedProductItem.GenMedProductID))
                        {
                            existProductInvalidID.Add(child.GenMedProductItem.GenMedProductID);

                            strItemInvalid += "     " + child.ChargeableItemName + "." + Environment.NewLine;
                        }
                        if ((CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.NBNT_BN_TIEN_PHAU_KHONG_BHYT || CurRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_VANG_LAI)
                            && (child.CreatedDate.Date < CurRegistration.ExamDate.Date || child.CreatedDate.Date > Now.Date) && !existProductInvalidID.Contains(child.GenMedProductItem.GenMedProductID))
                        {
                            existProductInvalidID2.Add(child.GenMedProductItem.GenMedProductID);

                            strItemInvalid2 += "     " + child.ChargeableItemName + "." + Environment.NewLine;
                        }
                    }
                }

                if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Any(inv => inv.OutwardDrugClinicDepts != null && inv.OutwardDrugClinicDepts.Count > 0))
                {
                    bCanSave = true;
                }
            }

            if (!bCanSave)
            {
                MessageBox.Show(eHCMSResources.A0697_G1_Msg_InfoKhTheLuuBill);
                //IsSaving = false;
                return;
            }
            if (!string.IsNullOrEmpty(strItemInvalid2))
            {
                MessageBox.Show(string.Format("{0}:", eHCMSResources.Z0456_G1_NgSDKgHopLe) + Environment.NewLine + strItemInvalid
                    + string.Format("\nNgày sử dụng không nhỏ hơn ngày đăng ký. Ngày đăng ký: {0}", CurRegistration.ExamDate.Date.ToShortDateString()), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            if (!string.IsNullOrEmpty(strItemInvalid))
            {
                MessageBox.Show(string.Format("{0}: \n{1} \n {2}. {3}: {4}", eHCMSResources.Z0456_G1_NgSDKgHopLe, strItemInvalid
                    , eHCMSResources.Z0458_G1_NgSDKgNhoHonNgNV, eHCMSResources.N0096_G1_NgNhapVien, AdmissionDate.ToShortDateString()), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            if (!string.IsNullOrEmpty(strError))
            {
                MessageBoxResult result = MessageBox.Show(string.Format("{0} {1} \n{2}", eHCMSResources.A0894_G1_Msg_InfoDVNamNgoaiTGianTaoBill, strError, eHCMSResources.A0890_G1_Msg_ConfDongYLuu), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                {
                    return;
                }
            }

            IsSaving = true;

            if (EditingBillingInvoice.OutwardDrugClinicDeptInvoices != null)
            {
                foreach (var inv in EditingBillingInvoice.OutwardDrugClinicDeptInvoices.Where(inv => inv.RecordState == RecordState.DETACHED))
                {
                    inv.Confirmed = MedRegItemConfirmed;
                }
            }

            if (EditingBillingInvoice.RegistrationDetails != null)
            {
                foreach (var item in EditingBillingInvoice.RegistrationDetails)
                {
                    item.ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
                }
            }

            // TxD 12/05/2015 added DeptLocationID to be saved in InPtBillingInv
            if (DepartmentContent != null && DepartmentContent.SelectedItem != null && DepartmentContent.SelectedItem.SelectDeptReqSelectRoom)
            {
                if (SelLocationInDept != null && SelLocationInDept.DeptLocationID > 0)
                {
                    EditingBillingInvoice.DeptLocationID = SelLocationInDept.DeptLocationID;
                }
            }

            //KMx: Thêm ngày (16/11/2015 16:30).
            EditingBillingInvoice.IsHighTechServiceBill = true;

            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                IsSaving = true;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddInPatientBillingInvoice(null, CurRegistration, EditingBillingInvoice, CalcPaymentToEndOfDay, false,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                bool bOK = false;
                                //PatientRegistration registration = null;
                                Dictionary<long, List<long>> drugIDList_Error = null;
                                long regID = -1;
                                long billingInvID = -1;
                                bool isNewRegistration = CurRegistration.PtRegistrationID <= 0;
                                try
                                {
                                    contract.EndAddInPatientBillingInvoice(out regID, out drugIDList_Error, out billingInvID, asyncResult);
                                    if (drugIDList_Error == null || drugIDList_Error.Count == 0)
                                    {
                                        bOK = true;
                                        var inv = asyncResult.AsyncState as InPatientBillingInvoice;
                                        inv.InPatientBillingInvID = billingInvID;
                                        inv.PtRegistrationID = regID;
                                    }
                                    else
                                    {
                                        bOK = false;
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    bOK = false;
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    bOK = false;
                                    error = new AxErrorEventArgs(ex);
                                }
                                finally
                                {
                                    IsSaving = false;
                                    if (drugIDList_Error != null && drugIDList_Error.Count > 0)
                                    {
                                        Action<IMedProductRemaingListing> onInitDlg = delegate (IMedProductRemaingListing vm)
                                        {
                                            vm.StartLoadingByIdList(drugIDList_Error);
                                            vm.Title = eHCMSResources.K2948_G1_DSLgoaiThuocKhongTheLuu;
                                        };
                                        GlobalsNAV.ShowDialog<IMedProductRemaingListing>(onInitDlg);
                                    }
                                    if (bOK && regID > 0)
                                    {
                                        if (isNewRegistration)
                                        {
                                            OpenRegistration(regID);
                                        }
                                        else //La dang ky cu chi can load bill nay roi add vao danh sach san co. Khoi mac cong.
                                        {
                                            EditingInvoiceDetailsContent.LoadDetails(AddBillingInvoiceCompletedCallback);
                                        }
                                        IsEditing = false;
                                        RegistrationInfoHasChanged = false;
                                    }

                                }


                            }), EditingBillingInvoice);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsSaving = false;
                    if (error != null)
                    {
                        Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                    }
                }

            });
            t.Start();
        }

        public void Handle(RemoveItem<MedRegItemBase> message)
        {
            if (this.GetView() == null || !CanRegister || message == null || message.Item == null)
            {
                return;
            }
            if (message.Item is PatientRegistrationDetail)
            {
                var details = message.Item as PatientRegistrationDetail;
                if (details.RecordState == RecordState.ADDED || details.RecordState == RecordState.DETACHED)
                {
                    CurRegistration.PatientRegistrationDetails.Remove(details);
                }
                else
                {
                    details.RecordState = RecordState.DELETED;
                    details.MarkedAsDeleted = true;
                }
            }
            else if (message.Item is PatientPCLRequestDetail)
            {
                foreach (var request in CurRegistration.PCLRequests)
                {
                    if (request.PatientPCLRequestIndicators != null
                        && request.PatientPCLRequestIndicators.Remove(message.Item as PatientPCLRequestDetail))
                    {
                        break;
                    }
                }
            }
            else if (message.Item is OutwardDrugClinicDept)
            {
                foreach (var invoice in CurRegistration.InPatientInvoices)
                {
                    if (invoice.OutwardDrugClinicDepts != null
                        && invoice.OutwardDrugClinicDepts.Remove(message.Item as OutwardDrugClinicDept))
                    {
                        break;
                    }
                }
            }
            else
            {
                message.Item.RecordState = RecordState.DELETED;
            }
            OnDetailsChanged();
        }
        private void OnDetailsChanged()
        {
            RegistrationInfoHasChanged = true;
            if (_currentRegMode == RegistrationFormMode.OLD_REGISTRATION_OPENED)
            {
                CurrentRegMode = RegistrationFormMode.OLD_REGISTRATION_CHANGED;
            }
            else if (_currentRegMode == RegistrationFormMode.NEW_REGISTRATION_OPENED)
            {
                CurrentRegMode = RegistrationFormMode.NEW_REGISTRATION_CHANGED;
            }
        }

        public bool CanCancelChangesCmd
        {
            get
            {
                return RegistrationInfoHasChanged;
            }
        }
        public void CancelChangesCmd()
        {
            this.CancelEdit();
            BeginEdit();

            //KMx: Khi bấm "Bỏ qua" thì BillInvoice sẽ trở về trạng thái ban đầu (ẩn nút "Lưu"), phải enable nút "Lưu" lên để user cập nhật "Từ ngày", "Đến ngày" (08/01/2015 09:30).
            RegistrationInfoHasChanged = true;

            //KMx: Khi bấm "Bỏ qua" thì BillInvoice sẽ trở về trạng thái ban đầu (ẩn "Từ ngày", "Đến ngày"), phải enable "Từ ngày", "Đến ngày" lên để user cập nhật. (08/01/2015 09:30).
            if (EditingBillingInvoice != null)
            {
                EditingBillingInvoice.CanEditPeriodOfTime = true;
            }
        }

        public bool CanStartEditRegistrationCmd
        {
            get
            {
                return !RegistrationInfoHasChanged
                    && (EditingBillingInvoice != null && EditingBillingInvoice.InPatientBillingInvID > 0)
                    && !IsEditing;
            }
        }
        public void StartEditRegistrationCmd()
        {
            if (isDischarged)
            {
                MessageBox.Show(eHCMSResources.A0241_G1_Msg_InfoBNDaXV_KTheSuaBill, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            BeginEdit();

            //KMx: Khi bấm nút "Sửa" thì phải set RegistrationInfoHasChanged = true để nút "Lưu" hiện lên (08/01/2015 09:28).
            //Trước đây khi bấm nút "Sửa" thì nút "Lưu" vẫn ẩn, khi nào thêm, xóa, sửa DV, CLS thì nút "Lưu" mới hiện lên. Dẫn đến không thể cập nhật từ ngày, đến ngày của bill được.
            RegistrationInfoHasChanged = true;

            //KMx: Khi bấm nút "Sửa" thì phải set CanEditPeriodOfTime = true để enable "Từ ngày", "Đến ngày" tạo bill (08/01/2015 09:28).
            if (EditingBillingInvoice != null)
            {
                EditingBillingInvoice.CanEditPeriodOfTime = true;
            }
        }
        #region COROUTINES
        //public IEnumerator<IResult> DoCalcHiBenefit(HealthInsurance hiItem, PaperReferal referal)
        //{            
        //    bool isEmergency = CurRegistration.EmergRecID > 0 ? true : false;
        //    var calcHiTask = new CalcHiBenefitTask(hiItem, referal, (long)AllLookupValues.RegistrationType.NOI_TRU, isEmergency);
        //    yield return calcHiTask;
        //    if (calcHiTask.Error == null)
        //    {
        //        PatientSummaryInfoContent.HiBenefit = calcHiTask.HiBenefit;
        //    }
        //}
        #endregion

        //private IEnumerator<IResult> GetDateTimeFromServer()
        //{
        //    var loadCurrentDate = new LoadCurrentDateTask();
        //    yield return loadCurrentDate;

        //    if (loadCurrentDate.CurrentDate == DateTime.MinValue)
        //    {
        //        SelectedDate = DateTime.Now.Date;
        //        MessageBoxTask _msgTask = new MessageBoxTask("Không lấy được ngày tháng từ server.", eHCMSResources.G0442_G1_TBao);
        //        yield return _msgTask;
        //    }
        //    else
        //    {
        //        SelectedDate = loadCurrentDate.CurrentDate.Date;
        //    }
        //    yield break;
        //}

        private IEnumerator<IResult> DoGetStore_EXTERNAL(long? DeptID)
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, DeptID, false, false);
            yield return paymentTypeTask;
            WarehouseList = paymentTypeTask.LookupList;
            if (WarehouseList != null)
            {
                SelectedWarehouse = WarehouseList.FirstOrDefault();
            }
            yield break;
        }

        public void ShowBillingReport(long inPatientBillingInvId)
        {
            Action<IBillingInvoiceDetailsReport> onInitDlg = delegate (IBillingInvoiceDetailsReport reportVm)
            {
                reportVm.InPatientBillingInvID = inPatientBillingInvId;
            };
            GlobalsNAV.ShowDialog<IBillingInvoiceDetailsReport>(onInitDlg, null, false, true);
        }

        public bool CanPrintOldBillingInvoiceCmd
        {
            get
            {
                return true;
            }
        }
        public void PrintOldBillingInvoiceCmd()
        {
            List<long> ids = OldBillingInvoiceContent.GetSelectedIds();
            if (ids != null && ids.Count > 0)
            {
                ShowBillingReport(ids[0]);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0591_G1_Msg_InfoChonDVDeIn);
            }
        }

        public void PrintBillingInvoiceCmd()
        {
            if (EditingBillingInvoice == null || EditingBillingInvoice.InPatientBillingInvID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0653_G1_Msg_InfoKhCoHDDeIn, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            ShowBillingReport(EditingBillingInvoice.InPatientBillingInvID);
        }

        public void CreateSuggestCashAdvanceCmd()
        {

            if (EditingBillingInvoice == null || EditingBillingInvoice.InPatientBillingInvID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0654_G1_Msg_InfoKhCoHDDeDNTU, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0142_G1_Msg_ConfTaoPhDNTUChoBill, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            long RptPtCashAdvRemID = 0;

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginCreateSuggestCashAdvance(EditingBillingInvoice.InPatientBillingInvID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0),
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var res = contract.EndCreateSuggestCashAdvance(out RptPtCashAdvRemID, asyncResult);
                                    if (res)
                                    {
                                        EditingBillingInvoice.RptPtCashAdvRemID = RptPtCashAdvRemID;
                                        MessageBox.Show(eHCMSResources.A0759_G1_Msg_InfoLapPhDNTUOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A0760_G1_Msg_InfoLapPhDNTUFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }


                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }


        public void PrintSuggestCashAdvanceCmd()
        {
            if (EditingBillingInvoice == null || EditingBillingInvoice.RptPtCashAdvRemID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0260_G1_Msg_InfoBillKhCoPhDNTUDeIn, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.ID = EditingBillingInvoice.RptPtCashAdvRemID;
                // 20181017 TNHX: [BM0002176] Change PHIEUDENGHITAMUNG -> PHIEUDENGHITAMUNG_TV
                proAlloc.eItem = ReportName.PHIEUDENGHITAMUNG_TV;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        public void Handle(EditItem<InPatientBillingInvoice> message)
        {
            object curView = this.GetView();
            if (curView != null)
            {
                // Không cho người dùng chỉnh sửa các bill thuộc đăng ký của bệnh nhân trong các trường hợp sau:
                // 1. Đăng ký đã xuất viện
                // 2. Đăng ký Vãng Lai hoặc Tiền Giải Phẫu chưa nhập viện nhưng đã quá hạn???
                // 3. Đăng ký đã bị đóng (khi có một đăng ký mới được tạo ra theo xác nhận của người dùng)
                //if (CurRegistration != null && CurRegistration.AdmissionInfo != null && CurRegistration.AdmissionInfo.DischargeDate != null)
                //{
                //    MessageBox.Show(eHCMSResources.A0685_G1_Msg_InfoKhTheSuaBillBNDaXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                //    return;
                //}

                if (!Validate_RegistrationInfo(CurRegistration))
                {
                    return;
                }
                EditingBillingInvoice = message.Item;
                if (curView is IInPatientRegistration_HighTechServiceView)
                {
                    ((IInPatientRegistration_HighTechServiceView)curView).SetActiveTab(InPatientRegistrationViewTab.EDITING_BILLING_INVOICE);
                }
                EditingInvoiceDetailsContent.LoadDetails(EditBillingInvoice);
            }
        }

        public void AddBillingInvoiceCompletedCallback(InPatientBillingInvoice inv)
        {
            if (CurRegistration != null && CurRegistration.PtRegistrationID > 0
                && inv.PtRegistrationID == CurRegistration.PtRegistrationID)
            {
                bool bExists = false;
                if (CurRegistration.InPatientBillingInvoices != null)
                {
                    foreach (var temp in CurRegistration.InPatientBillingInvoices)
                    {
                        if (inv == temp)
                        {
                            bExists = true;
                            break;
                        }
                    }
                    if (!bExists)
                    {
                        CurRegistration.InPatientBillingInvoices.Add(inv);

                        if (OldBillingInvoiceContent.BillingInvoices != null)
                        {
                            OldBillingInvoiceContent.BillingInvoices.Add(inv);
                        }
                    }
                }
                if (EditingBillingInvoice != null && EditingBillingInvoice.InPatientBillingInvID == inv.InPatientBillingInvID)
                {
                    EditingBillingInvoice = inv;
                }
            }
        }

        //KMx: Chỉnh sửa ngày 20/10/2014 11:28.
        //Lý do: Sau khi cập nhật bill, không load lại, vì index của CurRegistration.InPatientBillingInvoices và OldBillingInvoiceContent.BillingInvoices khác nhau.
        public void UpdateBillingInvoiceCompletedCallback(InPatientBillingInvoice inv)
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0 || CurRegistration.InPatientBillingInvoices == null
                || inv == null || inv.PtRegistrationID != CurRegistration.PtRegistrationID)
            {
                return;
            }


            for (int i = 0; i < CurRegistration.InPatientBillingInvoices.Count; i++)
            {
                if (inv.InPatientBillingInvID == CurRegistration.InPatientBillingInvoices[i].InPatientBillingInvID)
                {
                    CurRegistration.InPatientBillingInvoices[i] = inv;

                    break;
                }
            }

            for (int j = 0; j < OldBillingInvoiceContent.BillingInvoices.Count; j++)
            {
                if (inv.InPatientBillingInvID == OldBillingInvoiceContent.BillingInvoices[j].InPatientBillingInvID)
                {
                    OldBillingInvoiceContent.BillingInvoices[j] = inv;

                    break;
                }
            }  

            if (EditingBillingInvoice != null && EditingBillingInvoice.InPatientBillingInvID == inv.InPatientBillingInvID)
            {
                EditingBillingInvoice = inv;
            }
        }




        //public void UpdateBillingInvoiceCompletedCallback(InPatientBillingInvoice inv)
        //{
        //    if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0 || CurRegistration.InPatientBillingInvoices == null
        //        || inv == null || inv.PtRegistrationID != CurRegistration.PtRegistrationID)
        //    {
        //        return;
        //    }

        //    int idx = -1;

        //    for (int i = 0; i < CurRegistration.InPatientBillingInvoices.Count; i++)
        //    {
        //        if (inv.InPatientBillingInvID == CurRegistration.InPatientBillingInvoices[i].InPatientBillingInvID)
        //        {
        //            idx = i;
        //            break;
        //        }
        //    }
        //    if (idx >= 0)
        //    {
        //        CurRegistration.InPatientBillingInvoices[idx] = inv;
        //        if (OldBillingInvoiceContent.BillingInvoices != null
        //            && OldBillingInvoiceContent.BillingInvoices.Count > idx
        //            && OldBillingInvoiceContent.BillingInvoices[idx].InPatientBillingInvID == inv.InPatientBillingInvID)//Cho chac an.
        //        {
        //            OldBillingInvoiceContent.BillingInvoices[idx] = inv;

        //            //KMx: Khi Update xong bill thì không cần gán lại, vì khi xem chi tiết thì cũng phải đi load lại (13/09/2014 15:20).
        //            //if (OldBillingInvoiceContent.InvoiceDetailsContent.BillingInvoice != null
        //            //    && OldBillingInvoiceContent.InvoiceDetailsContent.BillingInvoice.InPatientBillingInvID == inv.InPatientBillingInvID)
        //            //{
        //            //    OldBillingInvoiceContent.RefreshDetailsView(inv);
        //            //}
        //        }

        //        if (EditingBillingInvoice != null && EditingBillingInvoice.InPatientBillingInvID == inv.InPatientBillingInvID)
        //        {
        //            EditingBillingInvoice = inv;
        //        }
        //    }
        //}

        public void EditBillingInvoice(InPatientBillingInvoice inv)
        {
            if (inv != null)
            {
                EditingBillingInvoice = inv;
                BeginEdit();

                //KMx: Khi chọn 1 bill để sửa thì phải set RegistrationInfoHasChanged = true để nút "Lưu" hiện lên (08/01/2015 09:28).
                //Trước đây khi chọn 1 bill để sửa thì nút "Lưu" vẫn ẩn, khi nào thêm, xóa, sửa DV, CLS thì nút "Lưu" mới hiện lên. Dẫn đến không thể cập nhật từ ngày, đến ngày của bill được.
                RegistrationInfoHasChanged = true;

                //KMx: Khi chọn 1 bill để sửa thì phải set CanEditPeriodOfTime = true để enable "Từ ngày", "Đến ngày" tạo bill (08/01/2015 09:28).
                EditingBillingInvoice.CanEditPeriodOfTime = true;

                // TxD 12/05/2015: Set Room location to the DeptLocationID of the Billing Invoice if DeptLocationID > 0 ( this field was not saved previously ie. old bills)
                if (inv.DeptLocationID.HasValue && inv.DeptLocationID > 0)
                {
                    foreach (var locItem in LocationsInDept)
                    {
                        if (locItem.DeptLocationID == inv.DeptLocationID)
                        {
                            SelLocationInDept = locItem;
                        }
                    }
                }
            }
        }
        public bool CanCreateNewBillCmd
        {
            get
            {
                return !RegistrationInfoHasChanged;
            }
        }
        public void CreateNewBillCmd()
        {
            EditingBillingInvoice = new InPatientBillingInvoice();
            EditingBillingInvoice.DeptID = DeptID;
            if (EditingBillingInvoice.V_BillingInvType == 0)
            {
                EditingBillingInvoice.V_BillingInvType = AllLookupValues.V_BillingInvType.TINH_TIEN_NOI_TRU;
            }
            BeginEdit();

            if (LocationsInDept != null && LocationsInDept.Count() > 0)
            {
                foreach (var locItem in LocationsInDept)
                {
                    if (locItem.DeptLocationID == 0)
                    {
                        SelLocationInDept = locItem;
                    }
                }
            }
            
        }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get
            {
                return _selectedTabIndex;
            }
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    NotifyOfPropertyChange(() => SelectedTabIndex);
                    NotifyOfPropertyChange(() => CanRegister);
                    NotifyOfPropertyChange(() => CanLoadBillCmd);
                }
            }
        }
        public void tabBillingInvoiceInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RegistrationInfoHasChanged)
            {
                //Giu lai tab index cu.
                var tabCtrl = sender as TabControl;
                if (tabCtrl != null && tabCtrl.SelectedIndex != SelectedTabIndex)
                {
                    MessageBoxResult result = MessageBox.Show(string.Format("{0}. {1}", eHCMSResources.Z0446_G1_TTinDaThayDoi, eHCMSResources.A0138_G1_Msg_ConfBoQua), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        CancelEdit();
                        _tempRegistration = CurRegistration.DeepCopy();
                    }
                    else
                    {
                        tabCtrl.SelectedIndex = SelectedTabIndex;
                    }
                }
            }
        }

        public void Handle(ItemRemoved<MedRegItemBase, IInPatientBillingInvoiceDetailsListing> message)
        {
            if (this.GetView() != null)
            {
                if (message.Source == EditingInvoiceDetailsContent)
                {
                    RegistrationInfoHasChanged = true;
                }
            }
        }


        public void Handle(ItemChanged<MedRegItemBase, IInPatientBillingInvoiceDetailsListing> message)
        {
            if (this.GetView() != null)
            {
                if (message.Source == EditingInvoiceDetailsContent)
                {
                    RegistrationInfoHasChanged = true;
                }
            }
        }

        public void ReturnDrugCmd()
        {
            if (!CheckDeptID())
            {
                return;
            }
            if (this.GetView() != null)
            {
                Action<IInPatientReturnDrug> onInitDlg = delegate (IInPatientReturnDrug vm)
                {
                    vm.MedProductType = AllLookupValues.MedProductType.THUOC;
                    vm.Registration = CurRegistration;
                    vm.InitData(DeptID);
                };
                GlobalsNAV.ShowDialog<IInPatientReturnDrug>(onInitDlg);
            }
        }
        //public void hplkNhapVien()
        //{
        //    {
        //        var vm = Globals.GetViewModel<IfrmPatientAdmission>();
        //        vm.curPatientRegistration= CurRegistration;

        //        Globals.ShowDialog(vm as Conductor<object>);
        //    }
        //}
        public bool CanLoadBillCmd
        {
            get
            {
                return (CurRegistration != null && CurRegistration.PtRegistrationID > 0)
                    && !RegistrationInfoHasChanged && SelectedTabIndex == (int)InPatientRegistrationViewTab.EDITING_BILLING_INVOICE;
            }
        }
        /// <summary>
        /// La bill moi (chua insert vo db) cua nhung item da su dung.
        /// </summary>
        private bool _IsBillOfUsedItems = false;
        public bool IsBillOfUsedItems
        {
            get
            {
                return _IsBillOfUsedItems;
            }
            set
            {
                _IsBillOfUsedItems = value;
                NotifyOfPropertyChange(() => IsBillOfUsedItems);

                EditingInvoiceDetailsContent.CanDelete = _isEditing;
                EditingInvoiceDetailsContent.CanEditOnGrid = _isEditing || IsBillOfUsedItems;

                CanCalcPaymentToEndOfDay = _IsBillOfUsedItems;
            }
        }

        private bool IsLoadNoBill = false;

        public void LoadBillCmd()
        {
            if (!CheckDeptID())
            {
                return;
            }

            if (SelectedWarehouse == null)
            {
                MessageBox.Show(eHCMSResources.K0338_G1_ChonKho, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (EditingInvoiceDetailsContent != null && EditingInvoiceDetailsContent.BillingInvoice != null && EditingInvoiceDetailsContent.BillingInvoice.InPatientBillingInvID > 0)
            {
                MessageBox.Show(eHCMSResources.A0180_G1_Msg_InfoDangXemBillCu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            //KMx: Không cần thiết gọi hàm này, nếu gọi hàm này sẽ bị lỗi: Người dùng chọn BillFromDate, BillToDate, chương trình tự set về ngày hiện tại (02/01/2015 17:40).
            //BeginEdit();

            DateTime? FromDate = null;
            DateTime? ToDate = null;
            if (EditingInvoiceDetailsContent != null && EditingInvoiceDetailsContent.BillingInvoice != null)
            {
                FromDate = EditingInvoiceDetailsContent.BillingInvoice.BillFromDate;
                ToDate = EditingInvoiceDetailsContent.BillingInvoice.BillToDate;
            }

            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                IsLoadingBill = true;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginLoadInPatientRegItemsIntoBill(CurRegistration.PtRegistrationID, DeptID
                            , SelectedWarehouse.StoreID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault()
                            , !Globals.isAccountCheck, FromDate, ToDate, (int)AllLookupValues.PatientFindBy.NOITRU
                            , 0, null, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var inv = contract.EndLoadInPatientRegItemsIntoBill(asyncResult);
                                    if (inv != null)
                                    {
                                        if (CheckIfInvoiceIsEmpty(inv))
                                        {
                                            MessageBox.Show(eHCMSResources.A0729_G1_Msg_InfoKhTimThayBill);
                                        }
                                        else
                                        {
                                            if (EditingInvoiceDetailsContent != null && EditingInvoiceDetailsContent.BillingInvoice != null && EditingInvoiceDetailsContent.BillingInvoice.InPatientBillingInvID <= 0)
                                            {
                                                inv.BillFromDate = EditingInvoiceDetailsContent.BillingInvoice.BillFromDate;
                                                inv.BillToDate = EditingInvoiceDetailsContent.BillingInvoice.BillToDate;
                                            }
                                            else
                                            {
                                                inv.BillFromDate = Globals.GetCurServerDateTime();
                                                inv.BillToDate = Globals.GetCurServerDateTime();
                                            }

                                            //KMx: Chỉ load những DV có ngày sử dụng <= Đến ngày (03/01/2015 10:57).
                                            if (inv.RegistrationDetails != null && inv.RegistrationDetails.Count > 0)
                                            {
                                                var FilterItem = inv.RegistrationDetails.Where(x => x.CreatedDate.Date <= inv.BillToDate.GetValueOrDefault().Date);

                                                inv.RegistrationDetails = new ObservableCollection<PatientRegistrationDetail>(FilterItem);
                                            }

                                            //KMx: Chỉ load những CLS có ngày sử dụng <= Đến ngày (03/01/2015 10:57).
                                            if (inv.PclRequests != null && inv.PclRequests.Count > 0)
                                            {
                                                var FilterItem = inv.PclRequests.Where(x => x.CreatedDate.Date <= inv.BillToDate.GetValueOrDefault().Date);

                                                inv.PclRequests = new ObservableCollection<PatientPCLRequest>(FilterItem);
                                            }

                                            //KMx: Chỉ load những thuốc/ y cụ/ hóa chất có ngày sử dụng <= Đến ngày (03/01/2015 10:57).
                                            if (inv.OutwardDrugClinicDeptInvoices != null && inv.OutwardDrugClinicDeptInvoices.Count > 0)
                                            {
                                                var FilterItem = inv.OutwardDrugClinicDeptInvoices.Where(x => x.OutDate.GetValueOrDefault().Date <= inv.BillToDate.GetValueOrDefault().Date);

                                                inv.OutwardDrugClinicDeptInvoices = new ObservableCollection<OutwardDrugClinicDeptInvoice>(FilterItem);
                                            }

                                            EditingBillingInvoice = inv;
                                            EditingBillingInvoice.DeptID = DeptID;
                                            IsEditing = true;

                                            RegistrationInfoHasChanged = true;
                                            IsBillOfUsedItems = true;
                                            //tam thoi de = false de ko cho cap nhat so luong ok
                                            //IsBillOfUsedItems = false;
                                            IsLoadNoBill = true;
                                        }
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                }
                                finally
                                {
                                    Globals.IsBusy = false;
                                }


                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsLoadingBill = false;
                    if (error != null)
                    {
                        Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                    }
                }

            });
            t.Start();
        }

        private bool CheckIfInvoiceIsEmpty(InPatientBillingInvoice inv)
        {
            if (inv.RegistrationDetails != null && inv.RegistrationDetails.Count > 0)
            {
                return false;
            }
            if (inv.PclRequests != null && inv.PclRequests.Count > 0)
            {
                if (inv.PclRequests.Any(req => req.PatientPCLRequestIndicators != null && req.PatientPCLRequestIndicators.Count > 0))
                {
                    return false;
                }
            }
            if (inv.OutwardDrugClinicDeptInvoices != null && inv.OutwardDrugClinicDeptInvoices.Count > 0)
            {
                return inv.OutwardDrugClinicDeptInvoices.All(drugInv => drugInv.OutwardDrugClinicDepts == null || drugInv.OutwardDrugClinicDepts.Count <= 0);
            }
            return true;
        }


        //KMx: Cấu hình của view này sai rồi, khi nào có thời gian thì làm cấu hình lại.
        //Lưu ý: View này có 2 link (Tạo bill, tạo bill tài vụ), phải check operation ở LeftMenu rồi truyền vào, không phải check trong view này (07/01/2015).
        //public void authorization()
        //{
        //    if (!Globals.isAccountCheck)
        //    {
        //        return;
        //    }

        //    mDangKyNoiTru_LoadBill = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                     , (int)ePatient.mInPatientRegister,
        //                                     (int)oRegistrionEx.mDangKyNoiTru_LoadBill, (int)ePermission.mView);
        //    mDangKyNoiTru_BillMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                     , (int)ePatient.mInPatientRegister,
        //                                     (int)oRegistrionEx.mDangKyNoiTru_BillMoi, (int)ePermission.mView);
        //    mDangKyNoiTru_ThemDV = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                     , (int)ePatient.mInPatientRegister,
        //                                     (int)oRegistrionEx.mDangKyNoiTru_ThemDV, (int)ePermission.mView);
        //    mDangKyNoiTru_SuaDV = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                     , (int)ePatient.mInPatientRegister,
        //                                     (int)oRegistrionEx.mDangKyNoiTru_SuaDV, (int)ePermission.mView);
        //    mDangKyNoiTru_XemChiTiet = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                     , (int)ePatient.mInPatientRegister,
        //                                     (int)oRegistrionEx.mDangKyNoiTru_XemChiTiet, (int)ePermission.mView);
        //    mDangKyNoiTru_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                     , (int)ePatient.mInPatientRegister,
        //                                     (int)oRegistrionEx.mDangKyNoiTru_In, (int)ePermission.mView);
        //    mDangKyNoiTru_TraThuoc = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                     , (int)ePatient.mInPatientRegister,
        //                                     (int)oRegistrionEx.mDangKyNoiTru_XemChiTiet, (int)ePermission.mView);

        //    //phan nay nam trong module chung ne
        //    mDangKyNoiTru_Patient_TimBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                     , (int)ePatient.mRegister,
        //                                     (int)oRegistrionEx.mDangKyNoiTru_Patient_TimBN, (int)ePermission.mView);
        //    mDangKyNoiTru_Patient_ThemBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                         , (int)ePatient.mRegister,
        //                                         (int)oRegistrionEx.mDangKyNoiTru_Patient_ThemBN, (int)ePermission.mView);
        //    mDangKyNoiTru_Patient_TimDangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                         , (int)ePatient.mRegister,
        //                                         (int)oRegistrionEx.mDangKyNoiTru_Patient_TimDangKy, (int)ePermission.mView);

        //    mDangKyNoiTru_Info_CapNhatThongTinBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                         , (int)ePatient.mRegister,
        //                                         (int)oRegistrionEx.mDangKyNoiTru_Info_CapNhatThongTinBN, (int)ePermission.mView);
        //    mDangKyNoiTru_Info_XacNhan = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                         , (int)ePatient.mRegister,
        //                                         (int)oRegistrionEx.mDangKyNoiTru_Info_XacNhan, (int)ePermission.mView);
        //    mDangKyNoiTru_Info_XoaThe = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                         , (int)ePatient.mRegister,
        //                                         (int)oRegistrionEx.mDangKyNoiTru_Info_XoaThe, (int)ePermission.mView);
        //    mDangKyNoiTru_Info_XemPhongKham = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
        //                                         , (int)ePatient.mRegister,
        //                                         (int)oRegistrionEx.mDangKyNoiTru_Info_XemPhongKham, (int)ePermission.mView);

        //}

        #region bool properties

        private bool _mDangKyNoiTru_LoadBill = true;
        private bool _mDangKyNoiTru_BillMoi = true;
        private bool _mDangKyNoiTru_ThemDV = true;
        private bool _mDangKyNoiTru_SuaDV = true;
        private bool _mDangKyNoiTru_XemChiTiet = true;
        private bool _mDangKyNoiTru_In = true;
        private bool _mDangKyNoiTru_TraThuoc = true;


        public bool mDangKyNoiTru_LoadBill
        {
            get
            {
                return _mDangKyNoiTru_LoadBill;
            }
            set
            {
                if (_mDangKyNoiTru_LoadBill == value)
                    return;
                _mDangKyNoiTru_LoadBill = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_LoadBill);
            }
        }

        public bool mDangKyNoiTru_BillMoi
        {
            get
            {
                return _mDangKyNoiTru_BillMoi;
            }
            set
            {
                if (_mDangKyNoiTru_BillMoi == value)
                    return;
                _mDangKyNoiTru_BillMoi = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_BillMoi);
            }
        }

        public bool mDangKyNoiTru_ThemDV
        {
            get
            {
                return _mDangKyNoiTru_ThemDV;
            }
            set
            {
                if (_mDangKyNoiTru_ThemDV == value)
                    return;
                _mDangKyNoiTru_ThemDV = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_ThemDV);
            }
        }

        public bool mDangKyNoiTru_SuaDV
        {
            get
            {
                return _mDangKyNoiTru_SuaDV;
            }
            set
            {
                if (_mDangKyNoiTru_SuaDV == value)
                    return;
                _mDangKyNoiTru_SuaDV = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_SuaDV);
            }
        }

        public bool mDangKyNoiTru_XemChiTiet
        {
            get
            {
                return _mDangKyNoiTru_XemChiTiet;
            }
            set
            {
                if (_mDangKyNoiTru_XemChiTiet == value)
                    return;
                _mDangKyNoiTru_XemChiTiet = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_XemChiTiet);
            }
        }

        public bool mDangKyNoiTru_In
        {
            get
            {
                return _mDangKyNoiTru_In;
            }
            set
            {
                if (_mDangKyNoiTru_In == value)
                    return;
                _mDangKyNoiTru_In = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_In);
            }
        }


        public bool mDangKyNoiTru_TraThuoc
        {
            get
            {
                return _mDangKyNoiTru_TraThuoc;
            }
            set
            {
                if (_mDangKyNoiTru_TraThuoc == value)
                    return;
                _mDangKyNoiTru_TraThuoc = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_TraThuoc);
            }
        }


        //phan nay nam trong module chung
        private bool _mDangKyNoiTru_Patient_TimBN = true;
        private bool _mDangKyNoiTru_Patient_ThemBN = true;
        private bool _mDangKyNoiTru_Patient_TimDangKy = true;

        private bool _mDangKyNoiTru_Info_CapNhatThongTinBN = true;
        private bool _mDangKyNoiTru_Info_XacNhan = true;
        private bool _mDangKyNoiTru_Info_XoaThe = true;
        private bool _mDangKyNoiTru_Info_XemPhongKham = true;

        public bool mDangKyNoiTru_Patient_TimBN
        {
            get
            {
                return _mDangKyNoiTru_Patient_TimBN;
            }
            set
            {
                if (_mDangKyNoiTru_Patient_TimBN == value)
                    return;
                _mDangKyNoiTru_Patient_TimBN = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_Patient_TimBN);
            }
        }

        public bool mDangKyNoiTru_Patient_ThemBN
        {
            get
            {
                return _mDangKyNoiTru_Patient_ThemBN;
            }
            set
            {
                if (_mDangKyNoiTru_Patient_ThemBN == value)
                    return;
                _mDangKyNoiTru_Patient_ThemBN = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_Patient_ThemBN);
            }
        }

        public bool mDangKyNoiTru_Patient_TimDangKy
        {
            get
            {
                return _mDangKyNoiTru_Patient_TimDangKy;
            }
            set
            {
                if (_mDangKyNoiTru_Patient_TimDangKy == value)
                    return;
                _mDangKyNoiTru_Patient_TimDangKy = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_Patient_TimDangKy);
            }
        }

        public bool mDangKyNoiTru_Info_CapNhatThongTinBN
        {
            get
            {
                return _mDangKyNoiTru_Info_CapNhatThongTinBN;
            }
            set
            {
                if (_mDangKyNoiTru_Info_CapNhatThongTinBN == value)
                    return;
                _mDangKyNoiTru_Info_CapNhatThongTinBN = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_Info_CapNhatThongTinBN);
            }
        }

        public bool mDangKyNoiTru_Info_XacNhan
        {
            get
            {
                return _mDangKyNoiTru_Info_XacNhan;
            }
            set
            {
                if (_mDangKyNoiTru_Info_XacNhan == value)
                    return;
                _mDangKyNoiTru_Info_XacNhan = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_Info_XacNhan);
            }
        }

        public bool mDangKyNoiTru_Info_XoaThe
        {
            get
            {
                return _mDangKyNoiTru_Info_XoaThe;
            }
            set
            {
                if (_mDangKyNoiTru_Info_XoaThe == value)
                    return;
                _mDangKyNoiTru_Info_XoaThe = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_Info_XoaThe);
            }
        }

        public bool mDangKyNoiTru_Info_XemPhongKham
        {
            get
            {
                return _mDangKyNoiTru_Info_XemPhongKham;
            }
            set
            {
                if (_mDangKyNoiTru_Info_XemPhongKham == value)
                    return;
                _mDangKyNoiTru_Info_XemPhongKham = value;
                NotifyOfPropertyChange(() => mDangKyNoiTru_Info_XemPhongKham);
            }
        }

        #endregion

        public void Handle(DoubleClickAddReqLAB message)
        {
            if (message.Source != SelectPCLContentLAB)
            {
                return;
            }
            if (message.EventArgs.Value != SelectPCLContentLAB.SelectedPCLExamType)
            {
                return;
            }
            AddPclExamTypeCmdLAB();
        }

        #region Add LAB

        public void AddAllPclExamTypeCmdLAB()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidatePclItemLAB(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }
            //bool used = SelectPCLContentLAB != null && SelectPCLContent.Used;

            bool used = true;/*A Tuan quyet dinh lam phieu CLS noi tru tai man hinh nay thi la da thuc hien roi*/

            CheckAndAddAllPCL(SelectPCLContentLAB.PclExamTypes, PclQtyLAB.Value, SelectedDate.Value, used);
        }

        public void AddPclExamTypeCmdLAB()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidatePclItemLAB(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent() { ValidationResults = validationResults });
                return;
            }
            //bool used = SelectPCLContentLAB != null && SelectPCLContent.Used;

            bool used = true;/*A Tuan quyet dinh lam phieu CLS noi tru tai man hinh nay thi la da thuc hien roi*/

            //AddPCLItem(SelectPCLContentLAB.SelectedPCLExamType, PclQtyLAB.Value, SelectedDate.Value, used);
            CheckAndAddPCL(SelectPCLContentLAB.SelectedPCLExamType, PclQtyLAB.Value, SelectedDate.Value, used);
        }
        private bool ValidatePclItemLAB(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (!SelectedDate.HasValue)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0161_G1_HayChonNgSDDV, new string[] { "SelectedDate" });
                result.Add(item);
            }

            if (SelectPCLContentLAB.PclExamTypes == null || SelectPCLContentLAB.PclExamTypes.Count <= 0
                || SelectPCLContentLAB.SelectedPCLExamType == null || SelectPCLContentLAB.SelectedPCLExamType.PCLExamTypeID < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0171_G1_HayChonLoaiCLSCanThem, new string[] { "SelectedPclExamType" });
                result.Add(item);
            }
            if (PclQtyLAB.GetValueOrDefault(0) < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0163_G1_NhapGTriSLgDV, new string[] { "PclQtyLAB" });
                result.Add(item);
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }

        #endregion

        public void GetAllInPatientBillingInvoices()
        {
            if (!UsedByTaiVuOffice && (Globals.isAccountCheck && DeptID.GetValueOrDefault() <= 0))
            {
                return;
            }

            var t = new Thread(() =>
            {
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllInPatientBillingInvoices(CurRegistration.PtRegistrationID, DeptID, (long)AllLookupValues.RegistrationType.NOI_TRU,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var inv = contract.EndGetAllInPatientBillingInvoices(asyncResult);
                                    if (OldBillingInvoiceContent.BillingInvoices == null)
                                    {
                                        OldBillingInvoiceContent.BillingInvoices = new ObservableCollection<InPatientBillingInvoice>();
                                    }
                                    OldBillingInvoiceContent.BillingInvoices.Clear();
                                    if (inv != null && inv.Count > 0)
                                    {
                                        foreach (InPatientBillingInvoice item in inv)
                                        {
                                            OldBillingInvoiceContent.BillingInvoices.Add(item);
                                        }
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                }
                                finally
                                {
                                    Globals.IsBusy = false;
                                }


                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    if (error != null)
                    {
                        Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                    }
                }

            });
            t.Start();
        }

        public void Handle(InPatientReturnMedProduct message)
        {
            if (this.IsActive && message != null)
            {
                GetAllInPatientBillingInvoices();
            }
        }
    }
}
