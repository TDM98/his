using System.Windows;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Collections.ObjectModel;
using System.Linq;
using eHCMSLanguage;
using System.Windows.Data;
using aEMR.Infrastructure.Events;
using System.Collections.Generic;
using aEMR.Common;
using aEMR.Common.PagedCollectionView;
using Castle.Windsor;
using System.Windows.Controls;
using System.Threading;
using aEMR.ServiceClient;
using System;
using Service.Core.Common;
using aEMR.CommonTasks;
using aEMR.Controls;
using aEMR.Common.Collections;
using System.ComponentModel;
/*
* 20190803 #001 TTM:    BM 0013059: Sửa lỗi AutoComplete y lệnh.
* 20190805 #002 TTM:    BM 0013102: Grid chỗ lưu thuốc bị chết khi click vào để chỉnh sửa.
* 20190830 #003 TTM:    BM 0013171: Kiểm tra SL-CD khác sl thành phần, chỉ kiểm tra trước khi thêm, nếu đã thêm rồi thì không kiểm tra nữa. Nếu người dùng không chọn theo liều dùng thì không cần kiểm tra.
* 20220106 #004 TNHX: 887 Cho tích lọc thuốc theo danh mục COVID ( kiếm không thấy thông tin BN nên tạm thời để false)
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IIntravenous)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class IntravenousViewModel : Conductor<object>, IIntravenous
        , IHandle<ReqOutwardDrugClinicDeptPatient_Add>
    {
        [ImportingConstructor]
        public IntravenousViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            CommonSelectDrugContent = Globals.GetViewModel<ICommonSelectDrug>();
            CommonSelectIntravenousContent = Globals.GetViewModel<ICommonSelectIntravenous>();
            CommonSelectDrugContent.AddItemCallback = AddItem;
            IntravenousList = new ObservableCollection<Intravenous>();
            CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
            refGenMedProductDetails = new ObservableCollection<RefGenMedProductDetails>();
            StoreCbx = new ObservableCollection<RefStorageWarehouseLocation>();
            IsInputDosage = true;
            Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedProductType).ToObservableCollection();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            if(!IsDialogViewObject) //20200414 TBL: BM 0030111: Nếu là xem lại y lệnh thì không cần chạy
            {
                Coroutine.BeginExecute(DoGetSubStore());
            }
        }
        //▼====: #004
        private bool _IsCOVID = false;
        public bool IsCOVID
        {
            get
            {
                return _IsCOVID;
            }
            set
            {
                if (_IsCOVID != value)
                {
                    _IsCOVID = value;
                    NotifyOfPropertyChange(() => IsCOVID);
                }
            }
        }
        //▲====: #004
        private ICommonSelectDrug _commonSelectDrugContent;
        public ICommonSelectDrug CommonSelectDrugContent
        {
            get
            {
                return _commonSelectDrugContent;
            }
            set
            {
                _commonSelectDrugContent = value;
                NotifyOfPropertyChange(() => CommonSelectDrugContent);
            }
        }

        private ICommonSelectIntravenous _commonSelectIntravenousContent;
        public ICommonSelectIntravenous CommonSelectIntravenousContent
        {
            get
            {
                return _commonSelectIntravenousContent;
            }
            set
            {
                _commonSelectIntravenousContent = value;
                NotifyOfPropertyChange(() => CommonSelectIntravenousContent);
            }
        }

        private Intravenous _DrugIntravenous;
        public Intravenous DrugIntravenous
        {
            get { return _DrugIntravenous; }
            set
            {
                if (_DrugIntravenous != value)
                {
                    _DrugIntravenous = value;
                    NotifyOfPropertyChange(() => DrugIntravenous);
                }
            }
        }

        private PagedCollectionView _ReqOutwardDrugClinicDeptPatientlst;
        public PagedCollectionView ReqOutwardDrugClinicDeptPatientlst
        {
            get { return _ReqOutwardDrugClinicDeptPatientlst; }
            set
            {
                if (_ReqOutwardDrugClinicDeptPatientlst != value)
                {
                    _ReqOutwardDrugClinicDeptPatientlst = value;
                    NotifyOfPropertyChange(() => ReqOutwardDrugClinicDeptPatientlst);
                }
            }
        }
        private ObservableCollection<Intravenous> _IntravenousList;
        public ObservableCollection<Intravenous> IntravenousList
        {
            get { return _IntravenousList; }
            set
            {
                if (_IntravenousList != value)
                {
                    _IntravenousList = value;
                    NotifyOfPropertyChange(() => IntravenousList);
                }
            }
        }
        private List<ReqOutwardDrugClinicDeptPatient> _DeletedIntravenousDetails;
        public List<ReqOutwardDrugClinicDeptPatient> DeletedIntravenousDetails
        {
            get
            {
                return _DeletedIntravenousDetails;
            }
            set
            {
                _DeletedIntravenousDetails = value;
                NotifyOfPropertyChange(() => DeletedIntravenousDetails);
            }
        }

        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> _IntravenousDetailsList;
        public ObservableCollection<ReqOutwardDrugClinicDeptPatient> IntravenousDetailsList
        {
            get { return _IntravenousDetailsList; }
            set
            {
                _IntravenousDetailsList = value;
                NotifyOfPropertyChange(() => IntravenousDetailsList);
            }
        }
        private ReqOutwardDrugClinicDeptPatient _SelectedReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient SelectedReqOutwardDrugClinicDeptPatient
        {
            get { return _SelectedReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_SelectedReqOutwardDrugClinicDeptPatient != value)
                {
                    _SelectedReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => SelectedReqOutwardDrugClinicDeptPatient);
                }
            }
        }
        public void lnkDelete_Loaded(object sender)
        {
        }
        private void FillPagedCollectionAndGroup(bool isNew = true)
        {
            if (isNew)
            {
                IntravenousDetailsList = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            }
            foreach (var IntravenousItem in IntravenousList)
            {
                foreach (var item in IntravenousItem.IntravenousDetails)
                {
                    IntravenousDetailsList.Add(item);
                    if (IntravenousItem.IntravenousID == 0
                        || IntravenousItem.V_InfusionType == null
                        || IntravenousItem.V_InfusionProcessType == null)
                    {
                        item.IDAndInfusionProcessType = item.RefGenericDrugDetail.V_MedProductType == (long)AllLookupValues.MedProductType.THUOC ? eHCMSResources.G0787_G1_Thuoc : (item.RefGenericDrugDetail.V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU ? eHCMSResources.G2907_G1_YCu : eHCMSResources.T1616_G1_HC);
                    }
                    else
                    {
                        item.IDAndInfusionProcessType = string.Format("{0} {1} [{2}]", IntravenousItem.V_InfusionType.ToString(), IntravenousItem.V_InfusionProcessType.ToString(), IntravenousItem.IntravenousID);
                    }
                }
            }
            //ReqOutwardDrugClinicDeptPatientlst = new PagedCollectionView(IntravenousDetailsList);
            //FillGroupName();

            //▼===== #002
            if (IntravenousDetailsList != null && IntravenousDetailsList != null)
            {
                CVReqDetails = CollectionViewSource.GetDefaultView(IntravenousDetailsList);
                CVReqDetails.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("IDAndInfusionProcessType"));
                CVReqDetails.Filter = (x) =>
                {
                    ReqOutwardDrugClinicDeptPatient Item = x as ReqOutwardDrugClinicDeptPatient;
                    if (Item == null)
                    {
                        return false;
                    }
                    return Item.RefGenericDrugDetail != null && Item.RefGenericDrugDetail.V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU;
                };
            }
            else
            {
                CVReqDetails = CollectionViewSource.GetDefaultView(new ObservableCollection<ReqOutwardDrugClinicDeptPatient>());
            }
            //▲===== #002
        }
        public void ReloadIntravenousList()
        {
            DrugIntravenous = null;
            DeletedIntravenousDetails = new List<ReqOutwardDrugClinicDeptPatient>();
            FillPagedCollectionAndGroup();
        }
        public void ReloadIntravenousListForCreateNewFromOld()
        {
            DrugIntravenous = IntravenousList.Any(x => x.IntravenousID == 0) ? IntravenousList.Where(x => x.IntravenousID == 0).FirstOrDefault() : null;
            DeletedIntravenousDetails = new List<ReqOutwardDrugClinicDeptPatient>();
            FillPagedCollectionAndGroup();
        }
        public void AddItem(ReqOutwardDrugClinicDeptPatient item)
        {
            //Tự động chọn đợt kháng sinh cho thuốc cần đánh số KS 
            if (item.RefGenericDrugDetail != null && item.RefGenericDrugDetail.V_InstructionOrdinalType == (long)AllLookupValues.V_InstructionOrdinalType.KhangSinh &&
                AntibioticTreatmentCollection.Any(x => x.AntibioticTreatmentID > 0))
            {
                item.CurrentAntibioticTreatment = AntibioticTreatmentCollection.FirstOrDefault(x => x.AntibioticTreatmentID > 0);
            }
            Globals.AutoSetAntibioticIndex(AntibioticTreatmentUsageHistories, item, MedicalInstructionDate);
            item.IntravenousPlan_InPtID = 0;
            item.IDAndInfusionProcessType = item.RefGenericDrugDetail.V_MedProductType == (long)AllLookupValues.MedProductType.THUOC ? eHCMSResources.G0787_G1_Thuoc : (item.RefGenericDrugDetail.V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU ? eHCMSResources.G2907_G1_YCu : eHCMSResources.T1616_G1_HC);
            //20200409 TBL: BM 0030106: Nếu thuốc đã có thì cập nhật lại số lượng
            bool bExist = false;
            foreach (Intravenous intravenous in IntravenousList)
            {
                foreach (ReqOutwardDrugClinicDeptPatient reqOutward in intravenous.IntravenousDetails)
                {
                    if (reqOutward.RefGenericDrugDetail != null && item.RefGenericDrugDetail != null && reqOutward.RefGenericDrugDetail.GenMedProductID == item.RefGenericDrugDetail.GenMedProductID)
                    {
                        reqOutward.PrescribedQty += item.PrescribedQty;
                        reqOutward.ReqQty += item.ReqQty;
                        reqOutward.RecordState = RecordState.MODIFIED;
                        bExist = true;
                        break;
                    }
                }
            }
            if (!bExist)
            {
                if (DrugIntravenous == null)
                {
                    DrugIntravenous = new Intravenous();
                    DrugIntravenous.IntravenousID = 0;
                    DrugIntravenous.IntravenousDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                    IntravenousList.Insert(0, DrugIntravenous);
                }
                DrugIntravenous.IntravenousDetails.Add(item);
            }
            FillPagedCollectionAndGroup();
        }
        public void Handle(ReqOutwardDrugClinicDeptPatient_Add message)
        {
            if (message != null && message.AddedReqOutwardDrugClinicDeptPatient != null)
            {
                IntravenousList.Add(message.AddedReqOutwardDrugClinicDeptPatient as Intravenous);
                FillPagedCollectionAndGroup();
            }
        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedReqOutwardDrugClinicDeptPatient == null)
            {
                return;
            }
            if (IntravenousList.SelectMany(x => x.IntravenousDetails).Any(x => x == SelectedReqOutwardDrugClinicDeptPatient))
            {
                if (SelectedReqOutwardDrugClinicDeptPatient.OutClinicDeptReqID > 0)
                {
                    if (SelectedReqOutwardDrugClinicDeptPatient.ReqDrugInClinicDeptID > 0)
                    {
                        MessageBox.Show(eHCMSResources.Z2216_G1_KhongTheXoaVTDaTaoPL, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        return;
                    }
                    DeletedIntravenousDetails.Add(SelectedReqOutwardDrugClinicDeptPatient.DeepCopy());
                }
                IntravenousList.Where(x => x.IntravenousDetails.Any(y => y == SelectedReqOutwardDrugClinicDeptPatient)).FirstOrDefault().IntravenousDetails.Remove(SelectedReqOutwardDrugClinicDeptPatient);
                FillPagedCollectionAndGroup(true);
            }
        }
        //▼===== #001
        #region AutoComplete
        private ReqOutwardDrugClinicDeptPatient _CurrentReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient CurrentReqOutwardDrugClinicDeptPatient
        {
            get { return _CurrentReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_CurrentReqOutwardDrugClinicDeptPatient != value)
                {
                    _CurrentReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => CurrentReqOutwardDrugClinicDeptPatient);
                }
            }
        }
        private ObservableCollection<RefGenMedProductDetails> _refGenMedProductDetails;
        public ObservableCollection<RefGenMedProductDetails> refGenMedProductDetails
        {
            get
            {
                return _refGenMedProductDetails;
            }
            set
            {
                if (_refGenMedProductDetails != value)
                {
                    _refGenMedProductDetails = value;
                    NotifyOfPropertyChange(() => refGenMedProductDetails);
                }
            }
        }
        private bool _IsInputDosage;
        public bool IsInputDosage
        {
            get { return _IsInputDosage; }
            set
            {
                if (_IsInputDosage != value)
                {
                    _IsInputDosage = value;
                    NotifyOfPropertyChange(() => IsInputDosage);
                }
            }
        }

        private string BrandName = "";

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; 
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
                    DoseVisibility = V_MedProductType == (long)AllLookupValues.MedProductType.THUOC;
                    NotifyOfPropertyChange(() => V_MedProductType);
                    NotifyOfPropertyChange(() => DoseVisibility);
                }

            }
        }
        private ObservableCollection<RefGenMedProductDetails> _RefGenMedProductDetailsSum;
        public ObservableCollection<RefGenMedProductDetails> RefGenMedProductDetailsListSum
        {
            get { return _RefGenMedProductDetailsSum; }
            set
            {
                if (_RefGenMedProductDetailsSum != value)
                    _RefGenMedProductDetailsSum = value;
                NotifyOfPropertyChange(() => RefGenMedProductDetailsListSum);
            }
        }

        string txt = "";
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
        private Action<ReqOutwardDrugClinicDeptPatient> _addItemCallback;
        public Action<ReqOutwardDrugClinicDeptPatient> AddItemCallback
        {
            get { return _addItemCallback; }
            set
            {
                if (_addItemCallback != value)
                {
                    _addItemCallback = value;
                    NotifyOfPropertyChange(() => AddItemCallback);
                }
            }
        }
        private bool? _IsFocusTextCode;
        public bool? IsFocusTextCode
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
        private bool _isSearchByGenericName = false;
        public bool IsSearchByGenericName
        {
            get { return _isSearchByGenericName; }
            set
            {
                if (_isSearchByGenericName != value)
                {
                    _isSearchByGenericName = value;
                    NotifyOfPropertyChange(() => IsSearchByGenericName);
                }
            }
        }

        private bool _visSearchByGenericName = true;
        public bool vIsSearchByGenericName
        {
            get { return _visSearchByGenericName; }
            set
            {
                if (_visSearchByGenericName != value)
                {
                    _visSearchByGenericName = value;
                    NotifyOfPropertyChange(() => vIsSearchByGenericName);
                }
            }
        }

        private bool _IsDialogViewObject;
        public bool IsDialogViewObject
        {
            get
            {
                return _IsDialogViewObject;
            }
            set
            {
                if (_IsDialogViewObject == value)
                {
                    return;
                }
                _IsDialogViewObject = value;
                NotifyOfPropertyChange(() => IsDialogViewObject);
            }
        }
        public void chkSearchByGenericName_Loaded(object sender, RoutedEventArgs e)
        {
            var chkSearchByGenericName = sender as CheckBox;

            if (Globals.ServerConfigSection.ConsultationElements.DefSearchByGenericName)
            {
                chkSearchByGenericName.IsChecked = true;
            }
            else
            {
                chkSearchByGenericName.IsChecked = false;
            }
        }
        TextBox tbxQty;
        public void tbxQty_Loaded(object sender, RoutedEventArgs e)
        {
            tbxQty = sender as TextBox;
        }
        TextBox tbxMDoseStr;
        public void tbxMDoseStr_Loaded(object sender, RoutedEventArgs e)
        {
            tbxMDoseStr = sender as TextBox;
        }
        private AutoCompleteBox acbAutoDrug_Text = null;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            acbAutoDrug_Text = sender as AutoCompleteBox;
        }
        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (refGenMedProductDetails == null || refGenMedProductDetails.Count < 1)
            {
                return;
            }
            RefGenMedProductDetails obj = acbAutoDrug_Text.SelectedItem as RefGenMedProductDetails;
            if (obj != null && CurrentReqOutwardDrugClinicDeptPatient != null)
            {
                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = obj;
            }
            AutoDrug_LostFocus(null, null);
            refGenMedProductDetails.Clear();
        }
        public void AutoDrug_LostFocus(object sender, RoutedEventArgs e)
        {
            if (acbAutoDrug_Text == null || string.IsNullOrEmpty(acbAutoDrug_Text.Text))
            {
                return;
            }
            if (IsInputDosage)
            {
                if (tbxMDoseStr == null)
                {
                    return;
                }
                tbxMDoseStr.Focus();
            }
            else
            {
                if (tbxQty == null)
                {
                    return;
                }
                tbxQty.Focus();
            }
        }
        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            e.Cancel = true;
            BrandName = e.Parameter;
            SearchRefGenMedProductDetails(BrandName, false);
        }
        private void SearchRefGenMedProductDetails(string Name, bool? IsCode)
        {
            long OutFromStoreObject = 1;
            if (IsCode == false && BrandName.Length < 1)
            {
                return;
            }
            IsFocusTextCode = IsCode;
            long? RefGenDrugCatID_1 = null;
            if (StoreCbx != null)
            {
                OutFromStoreObject = (long)cbb_Store.SelectedValue;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(IsSearchByGenericName, null, Name, OutFromStoreObject
                        , V_MedProductType, RefGenDrugCatID_1, null, IsCode, null, null, null, true
                        //▼====: #004
                        , IsCOVID
                        //▲====: #004
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                GroupRemaining(results);

                                if (IsCode == true)
                                {
                                    if (results != null && results.Count > 0)
                                    {
                                        if (CurrentReqOutwardDrugClinicDeptPatient == null)
                                        {
                                            CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
                                        }
                                        CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = results.FirstOrDefault();

                                        if (IsInputDosage)
                                        {
                                            if (tbxMDoseStr == null)
                                            {
                                                return;
                                            }
                                            tbxMDoseStr.Focus();
                                        }
                                        else
                                        {
                                            if (tbxQty == null)
                                            {
                                                return;
                                            }
                                            tbxQty.Focus();
                                        }
                                    }
                                    else
                                    {
                                        if (CurrentReqOutwardDrugClinicDeptPatient != null)
                                        {
                                            CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = null;
                                        }

                                        if (tbx != null)
                                        {
                                            txt = "";
                                            tbx.Text = "";
                                            tbx.Focus();
                                        }
                                        if (acbAutoDrug_Text != null)
                                        {
                                            acbAutoDrug_Text.Text = "";
                                        }

                                        MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                }
                                else
                                {
                                    refGenMedProductDetails.Clear();
                                    refGenMedProductDetails = RefGenMedProductDetailsListSum;
                                    acbAutoDrug_Text.PopulateComplete();
                                }
                            }
                            else
                            {
                                refGenMedProductDetails = new ObservableCollection<RefGenMedProductDetails>();
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
        private void GroupRemaining(IList<RefGenMedProductDetails> results)
        {
            var ListRefGMP = from RefGMP in results
                group RefGMP by new
                {
                    RefGMP.GenMedProductID,
                    RefGMP.BrandName,
                    RefGMP.SelectedUnit.UnitName,
                    RefGMP.RequestQty,
                    RefGMP.Code,
                    RefGMP.ProductCodeRefNum,
                    RefGMP.RefGenMedDrugDetails.Content,
                    RefGMP.GenericName,
                    RefGMP.V_MedProductType,
                    RefGMP.V_InstructionOrdinalType,
                    RefGMP.MinDayOrdinalContinueIsAllowable
                } into RefGMPGroup
                select new
                {
                    Remaining = RefGMPGroup.Sum(groupItem => groupItem.Remaining),
                    GenMedProductID = RefGMPGroup.Key.GenMedProductID,
                    UnitName = RefGMPGroup.Key.UnitName,
                    BrandName = RefGMPGroup.Key.BrandName,
                    GenericName = RefGMPGroup.Key.GenericName,
                    Content = RefGMPGroup.Key.Content,
                    Code = RefGMPGroup.Key.Code,
                    Qty = RefGMPGroup.Key.RequestQty,
                    ProductCodeRefNum = RefGMPGroup.Key.ProductCodeRefNum,
                    V_MedProductType = RefGMPGroup.Key.V_MedProductType,
                    V_InstructionOrdinalType = RefGMPGroup.Key.V_InstructionOrdinalType,
                    MinDayOrdinalContinueIsAllowable = RefGMPGroup.Key.MinDayOrdinalContinueIsAllowable
                };
            RefGenMedProductDetailsListSum = new ObservableCollection<RefGenMedProductDetails>();
            foreach (var Details in ListRefGMP)
            {
                RefGenMedProductDetails item = new RefGenMedProductDetails();
                item.GenMedProductID = Details.GenMedProductID;
                item.BrandName = Details.BrandName;
                item.SelectedUnit = new RefUnit();
                item.SelectedUnit.UnitName = Details.UnitName;
                item.Code = Details.Code;
                item.Remaining = Details.Remaining;
                item.RequestQty = Details.Qty;
                item.ProductCodeRefNum = Details.ProductCodeRefNum;
                item.V_MedProductType = Details.V_MedProductType;
                item.V_InstructionOrdinalType = Details.V_InstructionOrdinalType;
                item.MinDayOrdinalContinueIsAllowable = Details.MinDayOrdinalContinueIsAllowable;
                RefGenMedProductDetailsListSum.Add(item);
            }
        }
        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;
                if (!string.IsNullOrEmpty(txt))
                {
                    string Code = Globals.FormatCode(V_MedProductType, txt);
                    SearchRefGenMedProductDetails(Code, true);
                }
                else
                {
                    CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
                }
            }
        }
        public void tbxMDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.MDose, CurrentReqOutwardDrugClinicDeptPatient);
        }
        public void tbxADoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.ADose, CurrentReqOutwardDrugClinicDeptPatient);
        }
        public void tbxEDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.EDose, CurrentReqOutwardDrugClinicDeptPatient);
        }
        public void tbxNDoseStr_LostFocus(object sender, RoutedEventArgs e)
        {
            Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.NDose, CurrentReqOutwardDrugClinicDeptPatient);
        }
        public void btnAddItem()
        {
            System.Diagnostics.Debug.WriteLine(" ========> btnAddItem  1 .....");
            if (CurrentReqOutwardDrugClinicDeptPatient == null)
            {
                return;
            }
            if (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail == null || CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.GenMedProductID == 0
                                    || string.IsNullOrEmpty(CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.Code))
            {
                CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();

                Globals.ShowMessage(eHCMSResources.Z1147_G1_ChonHgCanYC, eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (CurrentReqOutwardDrugClinicDeptPatient.ReqQty <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1161_G1_SLgCDinhKgHopLe, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //▼===== #003
            if (!CheckPrescribedQtyBeforeAddGrid() && IsInputDosage)
            {
                MessageBox.Show(eHCMSResources.Z2806_G1_KhDuocThemThuocSLLDKhacSLCD);
                return;
            }
            //▲===== #003
            if (CalByUnitUse)
            {
                CurrentReqOutwardDrugClinicDeptPatient.ReqQty = CurrentReqOutwardDrugClinicDeptPatient.ReqQty / (CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.DispenseVolume == 0 ? 1 : (decimal)CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail.DispenseVolume);
                CurrentReqOutwardDrugClinicDeptPatient.ReqQty = Math.Round(CurrentReqOutwardDrugClinicDeptPatient.ReqQty, 2);
            }

            CurrentReqOutwardDrugClinicDeptPatient.DateTimeSelection = Globals.GetCurServerDateTime();
            {
                CurrentReqOutwardDrugClinicDeptPatient.EntityState = EntityState.NEW;
                CurrentReqOutwardDrugClinicDeptPatient.PrescribedQty = CurrentReqOutwardDrugClinicDeptPatient.ReqQty;
                var item = CurrentReqOutwardDrugClinicDeptPatient.DeepCopy();
                AddItem(item);
            }

            CurrentReqOutwardDrugClinicDeptPatient.RefGenericDrugDetail = new RefGenMedProductDetails();
            CurrentReqOutwardDrugClinicDeptPatient.MDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.ADoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.EDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.NDoseStr = "0";
            CurrentReqOutwardDrugClinicDeptPatient.MDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.ADose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.EDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.NDose = 0;
            CurrentReqOutwardDrugClinicDeptPatient.ReqQty = 0;

            if (IsFocusTextCode == false)
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
                if (acbAutoDrug_Text != null)
                {
                    acbAutoDrug_Text.Text = "";
                    acbAutoDrug_Text.Focus();
                }
            }
        }
        KeyEnabledComboBox cbb_Store = null;
        public void cbb_Store_Loaded(object sender, RoutedEventArgs e)
        {
            cbb_Store = sender as KeyEnabledComboBox;
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
        private RefStorageWarehouseLocation _SelectedStorage;
        public RefStorageWarehouseLocation SelectedStorage
        {
            get
            {
                return _SelectedStorage;
            }
            set
            {
                if (_SelectedStorage != value)
                {
                    _SelectedStorage = value;
                    NotifyOfPropertyChange(() => SelectedStorage);
                }
            }
        }
        private IEnumerator<IResult> DoGetSubStore()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, true, false);
            yield return paymentTypeTask;
            // 20191207 TNHX: Loại kho chẵn cho TV dựa trên cấu hình IsEnableMedSubStorage
            if ((V_MedProductType == (long)AllLookupValues.MedProductType.THUOC || V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU) && Globals.ServerConfigSection.MedDeptElements.IsEnableMedSubStorage)
            {
                StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && !x.IsMain && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()) && x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT)).ToObservableCollection();
            }
            else
            {
                StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()) && x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT)).ToObservableCollection();
            }
            SelectedStorage = StoreCbx.FirstOrDefault();
            yield break;
        }
        private bool _DoseVisibility = true;
        public bool DoseVisibility
        {
            get
            {
                return _DoseVisibility;
            }
            set
            {
                _DoseVisibility = value;
                NotifyOfPropertyChange(() => DoseVisibility);
            }
        }
        public void SetDafaultValueAllProperties()
        {
            ObservableCollection<ReqOutwardDrugClinicDeptPatient> tmp = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            ReqOutwardDrugClinicDeptPatientlst = new PagedCollectionView(tmp);
            CurrentReqOutwardDrugClinicDeptPatient = new ReqOutwardDrugClinicDeptPatient();
            refGenMedProductDetails = new ObservableCollection<RefGenMedProductDetails>();
            StoreCbx = new ObservableCollection<RefStorageWarehouseLocation>();
            if (!IsDialogViewObject) //20200414 TBL: BM 0030111: Nếu là xem lại y lệnh thì không cần chạy
            {
                Coroutine.BeginExecute(DoGetSubStore());
            }
            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                vIsSearchByGenericName = true;
                IsInputDosage = true;
            }
            else
            {
                vIsSearchByGenericName = false;
                IsInputDosage = false;
            }
        }
        public void cbbMedProductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDafaultValueAllProperties();
        }
        private ObservableCollection<Lookup> _gMedProductTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedProductType && (x.LookupID == (long)AllLookupValues.MedProductType.THUOC || x.LookupID == (long)AllLookupValues.MedProductType.Y_CU)).ToObservableCollection();
        public ObservableCollection<Lookup> gMedProductTypeCollection
        {
            get
            {
                return _gMedProductTypeCollection;
            }
            set
            {
                _gMedProductTypeCollection = value;
                NotifyOfPropertyChange(() => gMedProductTypeCollection);
            }
        }
        #endregion
        //▲===== #001

        //▼===== #002
        private ICollectionView _CVReqDetails;
        public ICollectionView CVReqDetails
        {
            get
            {
                return _CVReqDetails;
            }
            set
            {
                if (_CVReqDetails == value)
                {
                    return;
                }
                _CVReqDetails = value;
                NotifyOfPropertyChange(() => CVReqDetails);
            }
        }
        DataGrid grdReqOutwardDetails = null;
        public void grdReqOutwardDetails_Loaded(object sender, RoutedEventArgs e)
        {
            grdReqOutwardDetails = sender as DataGrid;

            if (grdReqOutwardDetails == null)
            {
                return;
            }

            var colMDoseStr = grdReqOutwardDetails.GetColumnByName("colMDoseStr");
            var colADoseStr = grdReqOutwardDetails.GetColumnByName("colADoseStr");
            var colEDoseStr = grdReqOutwardDetails.GetColumnByName("colEDoseStr");
            var colNDoseStr = grdReqOutwardDetails.GetColumnByName("colNDoseStr");

            if (colMDoseStr == null || colADoseStr == null || colEDoseStr == null || colNDoseStr == null)
            {
                return;
            }

            if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                colMDoseStr.Visibility = Visibility.Visible;
                colADoseStr.Visibility = Visibility.Visible;
                colEDoseStr.Visibility = Visibility.Visible;
                colNDoseStr.Visibility = Visibility.Visible;
            }
            else
            {
                colMDoseStr.Visibility = Visibility.Collapsed;
                colADoseStr.Visibility = Visibility.Collapsed;
                colEDoseStr.Visibility = Visibility.Collapsed;
                colNDoseStr.Visibility = Visibility.Collapsed;
            }

        }
        public void grdReqOutwardDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            ReqOutwardDrugClinicDeptPatient rowItem = e.Row.DataContext as ReqOutwardDrugClinicDeptPatient;

            if (rowItem == null)
            {
                return;
            }

            rowItem.DisplayGridRowNumber = e.Row.GetIndex() + 1;

            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        public void grdReqOutwardDetails_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            copySelectedReqOutwardDrugClinicDeptPatient = SelectedReqOutwardDrugClinicDeptPatient;
        }
        private ReqOutwardDrugClinicDeptPatient _copySelectedReqOutwardDrugClinicDeptPatient;
        public ReqOutwardDrugClinicDeptPatient copySelectedReqOutwardDrugClinicDeptPatient
        {
            get { return _copySelectedReqOutwardDrugClinicDeptPatient; }
            set
            {
                if (_copySelectedReqOutwardDrugClinicDeptPatient != value)
                {
                    _copySelectedReqOutwardDrugClinicDeptPatient = value;
                    NotifyOfPropertyChange(() => copySelectedReqOutwardDrugClinicDeptPatient);
                }
            }
        }
        private enum QtyColumnIndexes
        {
            M_Dose_Idx = 4,
            A_Dose_Idx = 5,
            E_Dose_Idx = 6,
            N_Dose_Idx = 7,
        };
        // TxD 13/07/2018: The previously edited Column       
        DataGridColumn prevWorkingColumn = null;
        public void grdReqOutwardDetails_CurrentCellChanged(object sender, EventArgs e)
        {
            if (sender == null)
                return;

            if (prevWorkingColumn == null)
            {
                prevWorkingColumn = ((DataGrid)sender).CurrentColumn;
                return;
            }

            int prevIdx = prevWorkingColumn.DisplayIndex;
            if (SelectedReqOutwardDrugClinicDeptPatient == null)
            {
                SelectedReqOutwardDrugClinicDeptPatient = copySelectedReqOutwardDrugClinicDeptPatient;
            }

            if ((sender as DataGrid).SelectedItem != null)
            {
                ischanged((sender as DataGrid).SelectedItem);
            }
            if (prevIdx == (int)QtyColumnIndexes.M_Dose_Idx)
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.MDose, SelectedReqOutwardDrugClinicDeptPatient);
            }
            else if (prevIdx == (int)QtyColumnIndexes.A_Dose_Idx)
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.ADose, SelectedReqOutwardDrugClinicDeptPatient);
            }
            else if (prevIdx == (int)QtyColumnIndexes.E_Dose_Idx)
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.EDose, SelectedReqOutwardDrugClinicDeptPatient);
            }
            else if (prevIdx == (int)QtyColumnIndexes.N_Dose_Idx)
            {
                Globals.ChangeAnyDoseQty(AllLookupValues.Dosage.NDose, SelectedReqOutwardDrugClinicDeptPatient);
            }
            ConvertCeiling(SelectedReqOutwardDrugClinicDeptPatient);
            prevWorkingColumn = ((DataGrid)sender).CurrentColumn;

        }
        private void ischanged(object item)
        {
            ReqOutwardDrugClinicDeptPatient p = item as ReqOutwardDrugClinicDeptPatient;
            if (p != null)
            {
                if (p.EntityState == EntityState.PERSITED)
                {
                    p.EntityState = EntityState.MODIFIED;
                }
            }
        }
        private void ConvertCeiling(ReqOutwardDrugClinicDeptPatient req)
        {
            if (req == null)
            {
                return;
            }
            if (Globals.ServerConfigSection.ClinicDeptElements.LamTronSLXuatNoiTru && V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            {
                req.ReqQty = Math.Ceiling(req.ReqQty);
            }
            req.ReqQtyStr = req.ReqQty.ToString();
        }
        //▲===== #002
        //▼===== #003
        public bool CheckPrescribedQtyBeforeAddGrid()
        {
            ReqOutwardDrugClinicDeptPatient tmpReq = CurrentReqOutwardDrugClinicDeptPatient;
            if ((tmpReq.ADose + tmpReq.EDose + tmpReq.NDose + tmpReq.MDose) != (float)tmpReq.ReqQty)
            {
                return false;
            }
            return true;
        }
        //▲===== #003
        private ObservableCollection<AntibioticTreatment> _AntibioticTreatmentCollection;
        public ObservableCollection<AntibioticTreatment> AntibioticTreatmentCollection
        {
            get
            {
                return _AntibioticTreatmentCollection;
            }
            set
            {
                if (_AntibioticTreatmentCollection == value)
                {
                    return;
                }
                var DrugIntravenousCopy = DrugIntravenous == null ? null : DrugIntravenous.DeepCopy();
                _AntibioticTreatmentCollection = value;
                NotifyOfPropertyChange(() => AntibioticTreatmentCollection);
                DrugIntravenous = DrugIntravenousCopy;
                FillPagedCollectionAndGroup();
            }
        }
        public IList<ReqOutwardDrugClinicDeptPatient> AntibioticTreatmentUsageHistories { get; set; }
        public DateTime? MedicalInstructionDate { get; set; }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            V_MedProductType = (long)AllLookupValues.MedProductType.Y_CU;
            SetDafaultValueAllProperties();
        }
    }
}