using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IOutPtAddServiceAndPCL)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UCOutPtAddServiceAndPCLViewModel : ViewModelBase, IOutPtAddServiceAndPCL
        , IHandle<DoubleClick>
        , IHandle<DoubleClickAddReqLAB>
    {
        #region Properties
        private IInPatientSelectPcl _SelectPCLContent;
        private IInPatientSelectPclLAB _SelectPCLContentLAB;
        private bool _CanAddService = true;
        private ObservableCollection<RefMedicalServiceType> _ServiceTypeCollection;
        private ObservableCollection<RefMedicalServiceItem> _MedServiceItemCollection;
        private ObservableCollection<RefMedicalServiceItem> _AllMedServiceItemCollection;
        private RefMedicalServiceItem _SelectedMedServiceItem;
        private byte? _ServiceQty;
        private ObservableCollection<DeptLocation> _DeptLocationCollection;
        private DeptLocation _SelectedDeptLocation;
        private RefMedicalServiceType _SelectedMedServiceType;
        private ObservableCollection<Lookup> _EkipCollection;
        private Lookup _SelectedEkip;
        public IInPatientSelectPcl SelectPCLContent
        {
            get { return _SelectPCLContent; }
            set
            {
                _SelectPCLContent = value;
                NotifyOfPropertyChange(() => SelectPCLContent);
            }
        }
        public IInPatientSelectPclLAB SelectPCLContentLAB
        {
            get { return _SelectPCLContentLAB; }
            set
            {
                _SelectPCLContentLAB = value;
                NotifyOfPropertyChange(() => SelectPCLContentLAB);
            }
        }
        public bool CanAddService
        {
            get
            {
                return _CanAddService;
            }
            set
            {
                _CanAddService = value;
                NotifyOfPropertyChange(() => CanAddService);
            }
        }
        public ObservableCollection<RefMedicalServiceType> ServiceTypeCollection
        {
            get { return _ServiceTypeCollection; }
            set
            {
                _ServiceTypeCollection = value;
                NotifyOfPropertyChange(() => ServiceTypeCollection);
            }
        }
        public ObservableCollection<RefMedicalServiceItem> MedServiceItemCollection
        {
            get { return _MedServiceItemCollection; }
            set
            {
                _MedServiceItemCollection = value;
                NotifyOfPropertyChange(() => MedServiceItemCollection);
            }
        }
        public ObservableCollection<RefMedicalServiceItem> AllMedServiceItemCollection
        {
            get
            {
                return _AllMedServiceItemCollection;
            }
            set
            {
                if (_AllMedServiceItemCollection == value)
                {
                    return;
                }
                _AllMedServiceItemCollection = value;
                NotifyOfPropertyChange(() => AllMedServiceItemCollection);
            }
        }
        public RefMedicalServiceItem SelectedMedServiceItem
        {
            get
            {
                return _SelectedMedServiceItem;
            }
            set
            {
                if (_SelectedMedServiceItem != value)
                {
                    _SelectedMedServiceItem = value;
                    NotifyOfPropertyChange(() => SelectedMedServiceItem);
                    SelectedDeptLocation = null;
                    ResetQuantityToDefaultValue();
                    //Load lai danh sach location tuong ung
                    long mMedServiceID = -1;
                    if (SelectedMedServiceItem != null)
                    {
                        mMedServiceID = SelectedMedServiceItem.MedServiceID;
                    }
                    ClearLocations();
                    if (mMedServiceID > 0 && SelectedMedServiceType != null)
                    {
                        if (SelectedMedServiceType.V_RefMedicalServiceTypes != (long)AllLookupValues.V_RefMedicalServiceTypes.CANLAMSANG)
                        {
                            GetLocationsByServiceIDFromCatche(mMedServiceID);
                        }
                    }
                }
            }
        }
        public byte? ServiceQty
        {
            get
            {
                return _ServiceQty;
            }
            set
            {
                if (_ServiceQty != value)
                {
                    _ServiceQty = value;
                    NotifyOfPropertyChange(() => ServiceQty);
                }
            }
        }
        public ObservableCollection<DeptLocation> DeptLocationCollection
        {
            get
            {
                return _DeptLocationCollection;
            }
            set
            {
                if (_DeptLocationCollection != value)
                {
                    _DeptLocationCollection = value;
                    NotifyOfPropertyChange(() => DeptLocationCollection);
                }
            }
        }
        public DeptLocation SelectedDeptLocation
        {
            get
            {
                return _SelectedDeptLocation;
            }
            set
            {
                if (_SelectedDeptLocation != value)
                {
                    _SelectedDeptLocation = value;
                    NotifyOfPropertyChange(() => SelectedDeptLocation);
                }
            }
        }
        public RefMedicalServiceType SelectedMedServiceType
        {
            get
            {
                return _SelectedMedServiceType;
            }
            set
            {
                if (_SelectedMedServiceType != value)
                {
                    _SelectedMedServiceType = value;
                    NotifyOfPropertyChange(() => SelectedMedServiceType);
                    NotifyOfPropertyChange(() => ShowLocationAndDoctor);
                    ReSelectedMedServiceType();
                }
            }
        }
        public bool ShowLocationAndDoctor
        {
            get
            {
                if (SelectedMedServiceType != null && SelectedMedServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.CANLAMSANG)
                {
                    return false;
                }
                return true;
            }
        }
        public ObservableCollection<Lookup> EkipCollection
        {
            get { return _EkipCollection; }
            set
            {
                if (_EkipCollection != value)
                {
                    _EkipCollection = value;
                    NotifyOfPropertyChange(() => EkipCollection);
                }
            }
        }
        public Lookup SelectedEkip
        {
            get { return _SelectedEkip; }
            set
            {
                if (_SelectedEkip != value)
                {
                    _SelectedEkip = value;
                    NotifyOfPropertyChange(() => SelectedEkip);
                }
            }
        }
        private List<RefMedicalServiceGroups> MedicalServiceGroupCollection { get; set; }
        public OnItemAdded OnItemAddedCallback { get; set; }
        #endregion
        #region Events
        [ImportingConstructor]
        public UCOutPtAddServiceAndPCLViewModel(IWindsorContainer aContainer, INavigationService aNavigation, ISalePosCaching aCaching)
        {
            GetAllMedicalServiceItems();
            LoadMedicalServiceGroupCollection();

            SelectPCLContent = Globals.GetViewModel<IInPatientSelectPcl>();
            SelectPCLContent.ShowUsedField = false;
            SelectPCLContent.ShowLocationSelection = true;
            ActivateItem(SelectPCLContent);

            SelectPCLContentLAB = Globals.GetViewModel<IInPatientSelectPclLAB>();
            SelectPCLContentLAB.ShowUsedField = false;
            SelectPCLContentLAB.ShowLocationSelection = true;
            ActivateItem(SelectPCLContentLAB);

            GetEkipCollection();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }
        public void AddRegItemCmd()
        {
            CallAddRegItemCmd(SelectedMedServiceItem, SelectedDeptLocation);
        }
        public void AddPclExamTypeCmd()
        {
            if (SelectPCLContent == null || SelectPCLContent.SelectedPCLExamType == null || SelectPCLContent.SelectedPCLExamType.PCLExamTypeID <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0156_G1_Chon1DV, eHCMSResources.G0442_G1_TBao);
                return;
            }
            AddPclExamType(SelectPCLContent.SelectedPCLExamType, SelectPCLContent.SelectedPclExamTypeLocation == null || SelectPCLContent.SelectedPclExamTypeLocation.DeptLocation == null ? null : SelectPCLContent.SelectedPclExamTypeLocation.DeptLocation);
        }
        public void AddPclExamTypeCmd_LAB()
        {
            if (SelectPCLContentLAB == null || SelectPCLContentLAB.SelectedPCLExamType == null || SelectPCLContentLAB.SelectedPCLExamType.PCLExamTypeID <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0156_G1_Chon1DV, eHCMSResources.G0442_G1_TBao);
                return;
            }
            AddPclExamType(SelectPCLContentLAB.SelectedPCLExamType, SelectPCLContentLAB.SelectedPclExamTypeLocation == null || SelectPCLContentLAB.SelectedPclExamTypeLocation.DeptLocation == null ? null : SelectPCLContentLAB.SelectedPclExamTypeLocation.DeptLocation);
        }
        public void AddAllPclExamTypeCmd_LAB()
        {
            if (SelectPCLContentLAB == null || SelectPCLContentLAB.PclExamTypes == null || SelectPCLContentLAB.PclExamTypes.Count == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0156_G1_Chon1DV, eHCMSResources.G0442_G1_TBao);
                return;
            }
            foreach (var item in SelectPCLContentLAB.PclExamTypes)
            {
                AddPclExamType(item, item.ObjDeptLocationList == null ? null : item.ObjDeptLocationList.FirstOrDefault());
            }
        }
        public void AddRegPackCmd()
        {
            ISearchMedicalServiceGroups SearchView = Globals.GetViewModel<ISearchMedicalServiceGroups>();
            SearchView.ApplySearchContent(MedicalServiceGroupCollection, "", null);
            GlobalsNAV.ShowDialog_V3(SearchView, null, null, false, true, Globals.GetTwoFourthWidthDefaultDialogViewSize());
            if (SearchView.SelectedRefMedicalServiceGroup != null)
            {
                ApplyRefMedicalServiceGroup(SearchView.SelectedRefMedicalServiceGroup, null, true);
            }
        }
        #endregion
        #region Methods
        private void CallAddRegItemCmd(RefMedicalServiceItem MedServiceItem, DeptLocation MedDeptLocation)
        {
            if (MedServiceItem == null || MedServiceItem.MedServiceID <= 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0156_G1_Chon1DV, eHCMSResources.G0442_G1_TBao);
                return;
            }
            var mItem = new PatientRegistrationDetail { RefMedicalServiceItem = MedServiceItem };
            mItem.RefMedicalServiceItem.RefMedicalServiceType = SelectedMedServiceType;
            mItem.MedServiceID = MedServiceItem.MedServiceID;
            mItem.DeptLocation = MedDeptLocation != null && MedDeptLocation.DeptLocationID > 0 ? MedDeptLocation : null;
            mItem.EntityState = Service.Core.Common.EntityState.DETACHED;
            mItem.ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
            mItem.CreatedDate = Globals.GetCurServerDateTime();
            mItem.Qty = 1;
            //newRegistrationDetail.HisID = CurrentRegistration.HisID;
            mItem.RecordState = RecordState.ADDED;
            mItem.CanDelete = true;
            mItem.HIAllowedPrice = MedServiceItem.HIAllowedPrice;
            mItem.InvoicePrice = null != null ? MedServiceItem.HIPatientPrice : MedServiceItem.NormalPrice;
            mItem.ReqDeptID = Globals.DeptLocation.DeptID;
            //newRegistrationDetail.GetItemPrice(CurrentRegistration, Globals.GetCurServerDateTime(), false, Globals.ServerConfigSection.HealthInsurances.FullHIBenefitForConfirm, Globals.ServerConfigSection.HealthInsurances.HiPolicyMinSalary);
            //newRegistrationDetail.GetItemTotalPrice();
            //Globals.EventAggregator.Publish(new ItemSelected<MedRegItemBase>() { Item = mItem });
            if (OnItemAddedCallback != null)
            {
                OnItemAddedCallback(mItem);
            }
        }
        private void LoadMedicalServiceGroupCollection()
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var CurrentFactory = new CommonService_V2Client())
                    {
                        var CurrentContract = CurrentFactory.ServiceInstance;
                        CurrentContract.BeginGetRefMedicalServiceGroups("", Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                MedicalServiceGroupCollection = new List<RefMedicalServiceGroups>();
                                var mItemCollection = CurrentContract.EndGetRefMedicalServiceGroups(asyncResult);
                                if (mItemCollection != null)
                                {
                                    MedicalServiceGroupCollection = mItemCollection.ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        private void GetServiceTypeCollection()
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                try
                {
                    using (var mFactory = new PatientRegistrationServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        var mOutTypes = new List<long> {
                            (long) AllLookupValues.V_RefMedicalServiceInOutOthers.NGOAITRU,
                            (long) AllLookupValues.V_RefMedicalServiceInOutOthers.NOITRU_NGOAITRU
                        };
                        mContract.BeginGetMedicalServiceTypesByInOutType(mOutTypes, Globals.DispatchCallback(asyncResult =>
                        {
                            IList<RefMedicalServiceType> mItemCollection = new ObservableCollection<RefMedicalServiceType>();
                            try
                            {
                                mItemCollection = mContract.EndGetMedicalServiceTypesByInOutType(asyncResult);
                            }
                            catch (FaultException<AxException> fault)
                            {
                                Globals.ShowMessage(fault.ToString(), eHCMSResources.T0432_G1_Error);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            ServiceTypeCollection = new ObservableCollection<RefMedicalServiceType>(mItemCollection);
                            ResetServiceTypeToDefaultValue();
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
        private void ResetServiceTypeToDefaultValue()
        {
            if (ServiceTypeCollection != null && ServiceTypeCollection.Any(x => x.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH))
            {
                SelectedMedServiceType = ServiceTypeCollection.First(x => x.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH);
            }
            else
            {
                SelectedMedServiceType = null;
            }
        }
        private void ResetServiceToDefaultValue()
        {
            if (MedServiceItemCollection != null && MedServiceItemCollection.Count > 0)
            {
                SelectedMedServiceItem = MedServiceItemCollection[0];
            }
            else
            {
                SelectedMedServiceItem = null;
            }
        }
        private void ResetQuantityToDefaultValue()
        {
            if (SelectedMedServiceItem != null && SelectedMedServiceItem.MedicalServiceTypeID > 0)
            {
                ServiceQty = 1;
            }
            else
            {
                ServiceQty = null;
            }
        }
        private void ClearLocations()
        {
            if (DeptLocationCollection != null)
            {
                DeptLocationCollection.Clear();
            }
        }
        private void GetLocationsByServiceIDFromCatche(long aMedServiceID)
        {
            if (MedServiceItemCollection.Any(o => o.MedServiceID == aMedServiceID) == false)
            {
                DeptLocationCollection = null;
                return;
            }
            var mItem = (from aItem in MedServiceItemCollection
                         where aItem.MedServiceID == aMedServiceID && aItem.allDeptLocation != null && aItem.allDeptLocation.Count > 0
                         select aItem).SingleOrDefault();
            if (mItem == null || mItem.allDeptLocation == null || mItem.allDeptLocation.Count == 0)
            {
                DeptLocationCollection = null;
                return;
            }
            DeptLocationCollection = new ObservableCollection<DeptLocation>(MedServiceItemCollection.Where(o => o.MedServiceID == aMedServiceID && o.allDeptLocation != null && o.allDeptLocation.Count > 0).ToList()[0].allDeptLocation);//.Where(c => CheckDeptLocValid(c.DeptLocationID)));
            //20191231 TBL: BM 0019707: Thêm cấu hình Tự động phân bổ phòng
            if (DeptLocationCollection.Count > 1 && Globals.ServerConfigSection.OutRegisElements.AutoLocationAllocation)
            {
                DeptLocation mDefaultDeptLoc = new DeptLocation()
                {
                    Location = new Location()
                    {
                        LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0606_G1_TuDongPhBoPg)
                    },
                    DeptLocationID = -1
                };
                DeptLocationCollection.Insert(0, mDefaultDeptLoc);
                SelectedDeptLocation = mDefaultDeptLoc;
            }
            else
            {
                SelectedDeptLocation = DeptLocationCollection.FirstOrDefault();
            }
        }
        private void GetAllMedicalServiceItemsByType(RefMedicalServiceType aServiceType)
        {
            var mItemCollection = AllMedServiceItemCollection == null ? null : AllMedServiceItemCollection.Where(x => x.MedicalServiceTypeID == aServiceType.MedicalServiceTypeID);
            if (mItemCollection != null)
            {
                foreach (var item in mItemCollection)
                {
                    item.RefMedicalServiceType = aServiceType;
                }
            }
            MedServiceItemCollection = mItemCollection.ToObservableCollection();
        }
        private void ReSelectedMedServiceType()
        {
            MedServiceItemCollection = null;
            SelectedMedServiceItem = null;
            GetAllMedicalServiceItemsByType(SelectedMedServiceType);
        }
        private void GetAllMedicalServiceItems()
        {
            var CurrentThread = new Thread(() =>
            {
                this.ShowBusyIndicator();
                try
                {
                    using (var CurrentFactory = new PatientRegistrationServiceClient())
                    {
                        var CurrentContract = CurrentFactory.ServiceInstance;
                        CurrentContract.BeginGetAllMedicalServiceItemsByType(null, null, null, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                AllMedServiceItemCollection = CurrentContract.EndGetAllMedicalServiceItemsByType(asyncResult).ToObservableCollection();
                            }
                            catch (Exception ex)
                            {
                                MedServiceItemCollection = new ObservableCollection<RefMedicalServiceItem>();
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                GetServiceTypeCollection();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });
            CurrentThread.Start();
        }
        public void GetEkipCollection()
        {
            EkipCollection = new ObservableCollection<Lookup>();
            Lookup mFirstItem = new Lookup();
            mFirstItem.LookupID = 0;
            mFirstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z2647_G1_ChonEkip);
            EkipCollection.Insert(0, mFirstItem);
            foreach (var item in Globals.AllLookupValueList)
            {
                if (item.ObjectTypeID == (long)(LookupValues.V_Ekip))
                {
                    EkipCollection.Add(item);
                }
            }
            SelectedEkip = new Lookup();
            if (EkipCollection != null && EkipCollection.Count > 0)
            {
                SelectedEkip = EkipCollection[0];
            }
        }
        private void AddPclExamType(PCLExamType aPCLExamType, DeptLocation aDeptLocation)
        {
            PatientPCLRequestDetail mItem = new PatientPCLRequestDetail { PCLExamType = aPCLExamType };
            mItem.PCLExamTypeID = aPCLExamType.PCLExamTypeID;
            mItem.StaffID = Globals.LoggedUserAccount.StaffID;
            mItem.MedProductType = AllLookupValues.MedProductType.CAN_LAM_SANG;
            mItem.Qty = 1;
            mItem.DeptLocation = aDeptLocation == null ? null : aDeptLocation;
            mItem.HIAllowedPrice = aPCLExamType.HIAllowedPrice;
            mItem.InvoicePrice = null != null ? aPCLExamType.HIPatientPrice : aPCLExamType.NormalPrice;
            mItem.CreatedDate = Globals.GetCurServerDateTime();
            //Globals.EventAggregator.Publish(new ItemSelected<MedRegItemBase>() { Item = mItem });
            if (OnItemAddedCallback != null)
            {
                OnItemAddedCallback(mItem);
            }
        }
        private ObservableCollection<DeptLocation> GetDefaultLocationsByServiceIDFromCatche(long medServiceID)
        {
            if (AllMedServiceItemCollection.Any(o => o.MedServiceID == medServiceID) == false)
            {
                return null;
            }
            var theItem = (from medItem in AllMedServiceItemCollection
                           where medItem.MedServiceID == medServiceID && medItem.allDeptLocation != null && medItem.allDeptLocation.Count > 0
                           select medItem).SingleOrDefault();
            if (theItem == null || theItem.allDeptLocation == null || theItem.allDeptLocation.Count == 0)
            {
                return null;
            }
            var DeptLocations = new ObservableCollection<DeptLocation>(AllMedServiceItemCollection.Where(o => o.MedServiceID == medServiceID && o.allDeptLocation != null && o.allDeptLocation.Count > 0).ToList()[0].allDeptLocation);
            if (DeptLocations.Count > 1)
            {
                DeptLocation defaultDeptLoc = new DeptLocation()
                {
                    Location = new Location()
                    {
                        LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0606_G1_TuDongPhBoPg)
                    },
                    DeptLocationID = -1
                };
                DeptLocations.Insert(0, defaultDeptLoc);
            }
            return DeptLocations;
        }
        private void ApplyRefMedicalServiceGroup(RefMedicalServiceGroups RefMedicalServiceGroupObj, DeptLocation aDeptLocation, bool IsAddNewOnly = false)
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var CurrentFactory = new CommonService_V2Client())
                    {
                        var CurrentContract = CurrentFactory.ServiceInstance;
                        CurrentContract.BeginGetRefMedicalServiceGroupItemsByID(RefMedicalServiceGroupObj.MedicalServiceGroupID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var ItemCollection = CurrentContract.EndGetRefMedicalServiceGroupItemsByID(asyncResult);
                                RefMedicalServiceGroupObj.RefMedicalServiceGroupItems = ItemCollection.ToList();
                                foreach (var Item in RefMedicalServiceGroupObj.RefMedicalServiceGroupItems.Where(x => x.MedServiceID.HasValue && x.MedServiceID > 0))
                                {
                                    Item.RefMedicalServiceItemObj.RefMedicalServiceType = new RefMedicalServiceType { V_RefMedicalServiceTypes = (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH };
                                    DeptLocation MedDeptLocation = null;
                                    Item.RefMedicalServiceItemObj.allDeptLocation = GetDefaultLocationsByServiceIDFromCatche(Item.RefMedicalServiceItemObj.MedServiceID);
                                    if (Item.RefMedicalServiceItemObj.allDeptLocation != null && Item.RefMedicalServiceItemObj.allDeptLocation.Count == 1)
                                    {
                                        MedDeptLocation = Item.RefMedicalServiceItemObj.allDeptLocation.FirstOrDefault();
                                    }
                                    CallAddRegItemCmd(Item.RefMedicalServiceItemObj, MedDeptLocation);
                                }
                                foreach (var Item in RefMedicalServiceGroupObj.RefMedicalServiceGroupItems.Where(x => x.PCLExamTypeID.HasValue && x.PCLExamTypeID > 0))
                                {
                                    if (Globals.ListPclExamTypesAllPCLFormImages.Any(x => x.PCLExamTypeID == Item.PCLExamTypeID))
                                    {
                                        AddPclExamType(Globals.ListPclExamTypesAllPCLFormImages.Where(x => x.PCLExamTypeID == Item.PCLExamTypeID).FirstOrDefault(), null);
                                    }
                                    if (Globals.ListPclExamTypesAllPCLForms.Any(x => x.PCLExamTypeID == Item.PCLExamTypeID))
                                    {
                                        AddPclExamType(Globals.ListPclExamTypesAllPCLForms.Where(x => x.PCLExamTypeID == Item.PCLExamTypeID).FirstOrDefault(), null);
                                    }
                                    else if (Globals.ListPclExamTypesAllCombos.Any(x => x.PCLExamTypeID == Item.PCLExamTypeID))
                                    {
                                        AddPclExamType(Globals.ListPclExamTypesAllCombos.Where(x => x.PCLExamTypeID == Item.PCLExamTypeID).FirstOrDefault().PCLExamType, null);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        #endregion
        #region Handles
        public void Handle(DoubleClick message)
        {
            if (message != null && this.GetView() != null && message.Source != null)
            {
                AddPclExamTypeCmd();
            }
        }
        public void Handle(DoubleClickAddReqLAB message)
        {
            if (message != null && this.GetView() != null && message.Source != null)
            {
                AddPclExamTypeCmd_LAB();
            }
        }
        #endregion
    }
}