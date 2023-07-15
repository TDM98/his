using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.ViewContracts;
using System.Linq;
using aEMR.Common.Collections;
using System.ServiceModel;
using aEMR.Common;
using aEMR.DataContracts;
using System.Collections.Generic;
using System.Windows.Controls;
using aEMR.Controls;

namespace aEMR.Configuration.Exemptions.ViewModels
{
    [Export(typeof(IExemptions_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Exemptions_AddEditViewModel : Conductor<object>, IExemptions_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Exemptions_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            ValidFromDate = Globals.GetViewModel<IMinHourDateControl>();
            ValidFromDate.DateTime = null;
            ValidToDate = Globals.GetViewModel<IMinHourDateControl>();
            ValidToDate.DateTime = null;

            DiscountTypeCountCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_DiscountTypeCount).ToObservableCollection();
            RefMedSerItemsSearchCriteria = new RefMedicalServiceItemsSearchCriteria();
            ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<RefMedicalServiceType>();
            GetAllMedicalServiceTypes_SubtractPCL();

            ObjMedServiceItems_Paging = new PagedSortableCollectionView<RefMedicalServiceItem>();
            ObjMedServiceItems_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjMedServiceItems_Paging_OnRefresh);

            ObjGetExemptionsMedServiceItems_Paging = new PagedSortableCollectionView<PromoDiscountItems>();
            ObjGetExemptionsMedServiceItems_Paging.PageSize = 1000;
            ObjGetExemptionsMedServiceItems_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjGetExemptionsMedServiceItems_Paging_OnRefresh);
            ExemptionsSearchCriteria = new ExemptionsMedServiceItemsSearchCriteria();

            allMedServiceItems = new ObjectEdit<PromoDiscountItems>("MedServiceID", "PromoDiscProgID", "");

            ObjV_PCLMainCategory = new ObservableCollection<Lookup>();
            ObjV_PCLMainCategory_Selected = new Lookup();
            ObjV_PCLMainCategory_Selected.LookupID = -1;

            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>();
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = new PCLExamTypeSubCategory();
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected.PCLExamTypeSubCategoryID = -1;

            LoadV_PCLMainCategory();
            SearchCriteria = new PCLExamTypeSearchCriteria();
            SearchCriteria.V_PCLMainCategory = ObjV_PCLMainCategory_Selected.LookupID;
            SearchCriteria.PCLExamTypeName = "";

            ObjPCLExamTypes_List_Paging = new PagedSortableCollectionView<PCLExamType>();
            ObjPCLExamTypes_List_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLExamTypes_List_Paging_OnRefresh);

            ObjPCLExamTypeExemptions = new ObservableCollection<PCLExamType>();
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private ObservableCollection<Lookup> _DiscountTypeCountCollection;
        public ObservableCollection<Lookup> DiscountTypeCountCollection
        {
            get { return _DiscountTypeCountCollection; }
            set
            {
                _DiscountTypeCountCollection = value;
                NotifyOfPropertyChange(() => DiscountTypeCountCollection);
            }
        }

        private PromoDiscountProgram _ObjExemptions_Current;
        public PromoDiscountProgram ObjExemptions_Current
        {
            get { return _ObjExemptions_Current; }
            set
            {
                _ObjExemptions_Current = value;
                NotifyOfPropertyChange(() => ObjExemptions_Current);
            }
        }

        private ObservableCollection<RefMedicalServiceType> _ObjRefMedicalServiceTypes_GetAll;
        public ObservableCollection<RefMedicalServiceType> ObjRefMedicalServiceTypes_GetAll
        {
            get { return _ObjRefMedicalServiceTypes_GetAll; }
            set
            {
                _ObjRefMedicalServiceTypes_GetAll = value;
                NotifyOfPropertyChange(() => ObjRefMedicalServiceTypes_GetAll);
            }
        }

        private PagedSortableCollectionView<RefMedicalServiceItem> _ObjMedServiceItems_Paging;
        public PagedSortableCollectionView<RefMedicalServiceItem> ObjMedServiceItems_Paging
        {
            get { return _ObjMedServiceItems_Paging; }
            set
            {
                _ObjMedServiceItems_Paging = value;
                NotifyOfPropertyChange(() => ObjMedServiceItems_Paging);
            }
        }
        private Lookup _ObjV_PCLMainCategory_Selected;
        public Lookup ObjV_PCLMainCategory_Selected
        {
            get { return _ObjV_PCLMainCategory_Selected; }
            set
            {
                _ObjV_PCLMainCategory_Selected = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory_Selected);
            }
        }

        private ObservableCollection<Lookup> _ObjV_PCLMainCategory;
        public ObservableCollection<Lookup> ObjV_PCLMainCategory
        {
            get { return _ObjV_PCLMainCategory; }
            set
            {
                _ObjV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory);
            }
        }

        private IMinHourDateControl _ValidFromDate;
        public IMinHourDateControl ValidFromDate
        {
            get { return _ValidFromDate; }
            set
            {
                _ValidFromDate = value;
                NotifyOfPropertyChange(() => ValidFromDate);
            }
        }
        private IMinHourDateControl _ValidToDate;
        public IMinHourDateControl ValidToDate
        {
            get { return _ValidToDate; }
            set
            {
                _ValidToDate = value;
                NotifyOfPropertyChange(() => ValidToDate);
            }
        }

        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }

        private RefMedicalServiceItemsSearchCriteria _RefMedSerItemsSearchCriteria;
        public RefMedicalServiceItemsSearchCriteria RefMedSerItemsSearchCriteria
        {
            get
            {
                return _RefMedSerItemsSearchCriteria;
            }
            set
            {
                _RefMedSerItemsSearchCriteria = value;
                NotifyOfPropertyChange(() => RefMedSerItemsSearchCriteria);
            }
        }
        private PCLExamTypeSubCategory _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected;
        public PCLExamTypeSubCategory ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected
        {
            get { return _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected; }
            set
            {
                _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected);
            }
        }

        private ObservableCollection<PCLExamTypeSubCategory> _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory;
        public ObservableCollection<PCLExamTypeSubCategory> ObjPCLExamTypeSubCategory_ByV_PCLMainCategory
        {
            get { return _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory; }
            set
            {
                _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory);
            }
        }
        private RefMedicalServiceItem _curRefMedicalServiceItem;
        public RefMedicalServiceItem curRefMedicalServiceItem
        {
            get { return _curRefMedicalServiceItem; }
            set
            {
                _curRefMedicalServiceItem = value;
                NotifyOfPropertyChange(() => curRefMedicalServiceItem);
            }
        }

        private ObjectEdit<PromoDiscountItems> _allMedServiceItems;
        public ObjectEdit<PromoDiscountItems> allMedServiceItems
        {
            get { return _allMedServiceItems; }
            set
            {
                _allMedServiceItems = value;
                NotifyOfPropertyChange(() => allMedServiceItems);
            }
        }
        private RefMedicalServiceType _ObjRefMedicalServiceTypeSelected = new RefMedicalServiceType();
        public RefMedicalServiceType ObjRefMedicalServiceTypeSelected
        {
            get
            {
                return _ObjRefMedicalServiceTypeSelected;
            }
            set
            {
                if (_ObjRefMedicalServiceTypeSelected != value)
                {
                    _ObjRefMedicalServiceTypeSelected = value;
                    NotifyOfPropertyChange(() => ObjRefMedicalServiceTypeSelected);
                    {
                        if (ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID > 0)
                        {
                            RefMedSerItemsSearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
                            ObjMedServiceItems_Paging.PageIndex = 0;
                            GetMedServiceItems_Paging(0, ObjGetExemptionsMedServiceItems_Paging.PageSize, true); 
                        }
                        else
                        {
                            if (ObjMedServiceItems_Paging == null)
                            {
                                ObjMedServiceItems_Paging = new PagedSortableCollectionView<RefMedicalServiceItem>();
                            }
                            ObjMedServiceItems_Paging.Clear();
                        }
                    }
                }
            }
        }
        private PagedSortableCollectionView<PromoDiscountItems> _ObjGetExemptionsMedServiceItems_Paging;
        public PagedSortableCollectionView<PromoDiscountItems> ObjGetExemptionsMedServiceItems_Paging
        {
            get { return _ObjGetExemptionsMedServiceItems_Paging; }
            set
            {
                _ObjGetExemptionsMedServiceItems_Paging = value;
                NotifyOfPropertyChange(() => ObjGetExemptionsMedServiceItems_Paging);
            }
        }
        private bool _OutPatientChecked;
        public bool OutPatientChecked
        {
            get { return _OutPatientChecked; }
            set
            {
                _OutPatientChecked = value;
                NotifyOfPropertyChange(() => OutPatientChecked);
            }
        }
        private bool _InPatientChecked;
        public bool InPatientChecked
        {
            get { return _InPatientChecked; }
            set
            {
                _InPatientChecked = value;
                NotifyOfPropertyChange(() => InPatientChecked);
            }
        }
        private bool _AllPatientChecked;
        public bool AllPatientChecked
        {
            get { return _AllPatientChecked; }
            set
            {
                _AllPatientChecked = value;
                NotifyOfPropertyChange(() => AllPatientChecked);
            }
        }
        public void InitializeNewItem()
        {
            ObjExemptions_Current = new PromoDiscountProgram();
            ValidFromDate.DateTime = Globals.GetCurServerDateTime();
            ValidToDate.DateTime = Globals.GetCurServerDateTime();
            OutPatientChecked = true;
        }
        public void InitializeItem()
        {
            ValidFromDate.DateTime = ObjExemptions_Current.ValidFromDate;
            ValidToDate.DateTime = ObjExemptions_Current.ValidToDate;
            ExemptionsSearchCriteria.PromoDiscProgID = ObjExemptions_Current.PromoDiscProgID;
            ObjGetExemptionsMedServiceItems_Paging.PageIndex = 0;
            GetExemptionsMedServiceItems_Paging(0, ObjGetExemptionsMedServiceItems_Paging.PageSize, true);
            if (ObjExemptions_Current.PromoDiscProgID > 0)
            {
                PCLExamTypeLocations_ByDeptLocationID();
                if(ObjExemptions_Current.V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    OutPatientChecked = true;
                }
                else if (ObjExemptions_Current.V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU)
                {
                    InPatientChecked = true;
                }
                else
                {
                    AllPatientChecked = true;
                }
            }
        }
        public void cboV_PCLMainCategory_SelectionChanged(object selectItem)
        {
            if (selectItem != null)
            {
                Lookup Objtmp = (selectItem as Lookup);

                if (Objtmp != null)
                {
                    SearchCriteria.V_PCLMainCategory = Objtmp.LookupID;

                    if (Objtmp.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                    {
                        ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Clear();
                        PCLExamTypeSubCategory firstItem = new PCLExamTypeSubCategory();
                        firstItem.PCLExamTypeSubCategoryID = -1;
                        firstItem.PCLSubCategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                        ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Insert(0, firstItem);
                        ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = firstItem;
                        ObjPCLExamTypes_List_Paging.PageIndex = 0;
                        PCLExamTypes_List_Paging(0, ObjPCLExamTypes_List_Paging.PageSize, true);
                    }
                    else
                    {
                        ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected.PCLExamTypeSubCategoryID = -1;
                        PCLExamTypeSubCategory_ByV_PCLMainCategory();
                    }
                }
            }
        }
        public void cboPCLExamTypeSubCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AxComboBox Ctr = sender as AxComboBox;
            if (Ctr == null)
                return;

            PCLExamTypeSubCategory Objtmp = Ctr.SelectedItemEx as PCLExamTypeSubCategory;

            if (Objtmp != null)
            {
                SearchCriteria.PCLExamTypeSubCategoryID = Objtmp.PCLExamTypeSubCategoryID;
                if (SearchCriteria.V_PCLMainCategory != (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    ObjPCLExamTypes_List_Paging.PageIndex = 0;
                    PCLExamTypes_List_Paging(0, ObjPCLExamTypes_List_Paging.PageSize, true);
                }
            }
        }

        public bool CheckValid(object temp)
        {
            PromoDiscountProgram p = temp as PromoDiscountProgram;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }
        private PCLExamTypeSearchCriteria _SearchCriteria;
        public PCLExamTypeSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }
        private ExemptionsMedServiceItemsSearchCriteria _ExemptionsSearchCriteria;
        public ExemptionsMedServiceItemsSearchCriteria ExemptionsSearchCriteria
        { 
            get
            {
                return _ExemptionsSearchCriteria;
            }
            set
            {
                _ExemptionsSearchCriteria = value;
                NotifyOfPropertyChange(() => ExemptionsSearchCriteria);
            }
        }
        private PromoDiscountItems _curMedServiceItems;
        public PromoDiscountItems curMedServiceItems
        {
            get { return _curMedServiceItems; }
            set
            {
                _curMedServiceItems = value;
                NotifyOfPropertyChange(() => curMedServiceItems);
            }
        }
        private PCLExamType _ObjPCLExamType_SelectForAdd;
        public PCLExamType ObjPCLExamType_SelectForAdd
        {
            get { return _ObjPCLExamType_SelectForAdd; }
            set
            {
                _ObjPCLExamType_SelectForAdd = value;
                NotifyOfPropertyChange(() => ObjPCLExamType_SelectForAdd);
            }
        }
        private ObservableCollection<PCLExamType> _ObjPCLExamTypeExemptions;
        public ObservableCollection<PCLExamType> ObjPCLExamTypeExemptions
        {
            get { return _ObjPCLExamTypeExemptions; }
            set
            {
                _ObjPCLExamTypeExemptions = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeExemptions);
            }
        }
        private string _PCLExamTypeName;
        public string PCLExamTypeName
        {
            get { return _PCLExamTypeName; }
            set
            {
                if (_PCLExamTypeName != value)
                {
                    _PCLExamTypeName = value;
                    NotifyOfPropertyChange(() => PCLExamTypeName);
                }
            }
        }
        public Button hplDeleteService { get; set; }
        public void hplDeleteService_Loaded(object sender)
        {
            hplDeleteService = sender as Button;
            //hplDeleteService.Visibility = Globals.convertVisibility();
        }
        private bool CheckClickHeaderNotValid()
        {
            if (SearchCriteria.V_PCLMainCategory > 0)
                return true;
            return false;
        }
        void ObjPCLExamTypes_List_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLExamTypes_List_Paging(ObjPCLExamTypes_List_Paging.PageIndex, ObjPCLExamTypes_List_Paging.PageSize, false);
        }
        void ObjGetExemptionsMedServiceItems_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetExemptionsMedServiceItems_Paging(ObjGetExemptionsMedServiceItems_Paging.PageIndex, ObjGetExemptionsMedServiceItems_Paging.PageSize, false);
        }
        private PagedSortableCollectionView<DataEntities.PCLExamType> _ObjPCLExamTypes_List_Paging;
        public PagedSortableCollectionView<DataEntities.PCLExamType> ObjPCLExamTypes_List_Paging
        {
            get { return _ObjPCLExamTypes_List_Paging; }
            set
            {
                _ObjPCLExamTypes_List_Paging = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypes_List_Paging);
            }
        }

        private void PCLExamTypes_List_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            if (CheckClickHeaderNotValid() == false)
                return;

            ObjPCLExamTypes_List_Paging.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3014_G1_DSPCLExamType) });

            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypes_List_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.PCLExamType> allItems = null;
                            bool bOK = false;

                            try
                            {
                                allItems = client.EndPCLExamTypes_List_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjPCLExamTypes_List_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPCLExamTypes_List_Paging.Add(item);
                                    }

                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    //Globals.IsBusy = false;
                    IsLoading = false;
                }
            });
            t.Start();
        }


        public void PCLExamTypeSubCategory_ByV_PCLMainCategory()
        {
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0532_G1_DSNhom) });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeSubCategory_ByV_PCLMainCategory(ObjV_PCLMainCategory_Selected.LookupID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPCLExamTypeSubCategory_ByV_PCLMainCategory(asyncResult);

                            if (items != null)
                            {
                                ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>(items);
                                PCLExamTypeSubCategory firstItem = new PCLExamTypeSubCategory();
                                firstItem.PCLExamTypeSubCategoryID = -1;
                                firstItem.PCLSubCategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Insert(0, firstItem);

                                ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = firstItem;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        public void LoadV_PCLMainCategory()
        {
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent{IsBusy = true,Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0185_G1_DSLoai)});

                IsLoading = true;

                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_PCLMainCategory,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_PCLMainCategory = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2034_G1_ChonLoai2);
                                    ObjV_PCLMainCategory.Insert(0, firstItem);
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
                    //Globals.IsBusy = false;
                    IsLoading = false;
                }
            });
            t.Start();
        }
        void ObjMedServiceItems_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetMedServiceItems_Paging(ObjMedServiceItems_Paging.PageIndex, ObjMedServiceItems_Paging.PageSize, true);
        }
        private void Exemptions_InsertUpdate(PromoDiscountProgram Obj, bool SaveToDB)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });
            if (Obj.PromoDiscProgID == 0)
            {
                Obj.StaffID = (long)Globals.LoggedUserAccount.StaffID;
            }
            Obj.ValidFromDate = ValidFromDate.DateTime;
            Obj.ValidToDate = ValidToDate.DateTime;
            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginExemptions_InsertUpdate(Obj, Globals.DispatchCallback((asyncResult) =>
                     {
                         try
                         {
                             string Result = "";
                             long NewID = 0;
                             contract.EndExemptions_InsertUpdate(out Result, out NewID, asyncResult);
                             switch (Result)
                             {
                                 case "Duplex-Name":
                                     {
                                         MessageBox.Show(string.Format("{0} {1}!", eHCMSResources.A1009_G1_Msg_InfoTenPgDaTonTai, eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                         break;
                                     }
                                 case "Update-0":
                                     {
                                         MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                         break;
                                     }
                                 case "Update-1":
                                     {
                                         Globals.EventAggregator.Publish(new Location_Event_Save() { Result = true });
                                         TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjExemptions_Current.PromoDiscName.Trim());
                                         MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                         break;
                                     }
                                 case "Insert-0":
                                     {
                                         
                                         MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                         break;
                                     }
                                 case "Insert-1":
                                     {
                                         Globals.EventAggregator.Publish(new Location_Event_Save() { Result = true });
                                         ObjExemptions_Current.PromoDiscProgID = NewID;
                                         MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                         break;
                                     }
                             }
                         }
                         catch (Exception ex)
                         {
                             Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                         }
                         finally
                         {
                             //Globals.IsBusy = false;
                             IsLoading = false;
                         }
                     }), null);
                }


            });
            t.Start();
        }
      
        public void GetAllMedicalServiceTypes_SubtractPCL()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Dịch Vụ..." });
            this.ShowBusyIndicator(eHCMSResources.Z0604_G1_DangLayDSLoaiDV);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllMedicalServiceTypes_SubtractPCL(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndGetAllMedicalServiceTypes_SubtractPCL(asyncResult);
                                if (items != null)
                                {
                                    ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<DataEntities.RefMedicalServiceType>(items);

                                    //Item Default
                                    RefMedicalServiceType ItemDefault = new RefMedicalServiceType();
                                    ItemDefault.MedicalServiceTypeID = -1;
                                    ItemDefault.MedicalServiceTypeName = "--Chọn Loại Dịch Vụ--";
                                    //Item Default

                                    ObjRefMedicalServiceTypes_GetAll.Insert(0, ItemDefault);
                                }
                                else
                                {
                                    ObjRefMedicalServiceTypes_GetAll = null;
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
        private void GetMedServiceItems_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Dịch Vụ..." });
            this.ShowBusyIndicator(eHCMSResources.Z1007_G1_LoadDSDV);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetMedServiceItems_Paging(RefMedSerItemsSearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<RefMedicalServiceItem> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetMedServiceItems_Paging(out Total, asyncResult);
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
                                this.HideBusyIndicator();
                            }

                            ObjMedServiceItems_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjMedServiceItems_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjMedServiceItems_Paging.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void MedServiceItems_InsertXML(ObservableCollection<PromoDiscountItems> lstMedServiceItems)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            this.ShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExemptionsMedServiceItems_InsertXML(lstMedServiceItems, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndExemptionsMedServiceItems_InsertXML(asyncResult))
                                {
                                    ExemptionsSearchCriteria.PromoDiscProgID = ObjExemptions_Current.PromoDiscProgID;
                                    ObjGetExemptionsMedServiceItems_Paging.PageIndex = 0;
                                    GetExemptionsMedServiceItems_Paging(0, ObjGetExemptionsMedServiceItems_Paging.PageSize, true);
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, "Thêm Dịch Vụ Cho Khoa", MessageBoxButton.OK);
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
        public void MedServiceItems_DeleteXML(ObservableCollection<PromoDiscountItems> lstMedServiceItems)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            this.ShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExemptionsMedServiceItems_DeleteXML(lstMedServiceItems, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndExemptionsMedServiceItems_DeleteXML(asyncResult))
                                {
                                    ExemptionsSearchCriteria.PromoDiscProgID = ObjExemptions_Current.PromoDiscProgID;
                                    ObjGetExemptionsMedServiceItems_Paging.PageIndex = 0;
                                    GetExemptionsMedServiceItems_Paging(0, ObjGetExemptionsMedServiceItems_Paging.PageSize, true);
                                    MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, "Xóa Dịch Vụ Cho Khoa", MessageBoxButton.OK);
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
        private void GetExemptionsMedServiceItems_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Dịch Vụ..." });
            this.ShowBusyIndicator(eHCMSResources.Z1007_G1_LoadDSDV);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetExemptionsMedServiceItems_Paging(ExemptionsSearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PromoDiscountItems> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetExemptionsMedServiceItems_Paging(out Total, asyncResult);
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
                                this.HideBusyIndicator();
                            }

                            ObjGetExemptionsMedServiceItems_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjGetExemptionsMedServiceItems_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjGetExemptionsMedServiceItems_Paging.Add(item);
                                    }
                                }
                                allMedServiceItems = new ObjectEdit<PromoDiscountItems>(ObjGetExemptionsMedServiceItems_Paging
                                    , "MedServiceID", "PromoDiscProgID", "");
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void PCLExamTypeLocations_ByDeptLocationID()
        {
            ObjPCLExamTypeExemptions.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách PCLExamType..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeExemptions(PCLExamTypeName, ObjExemptions_Current.PromoDiscProgID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPCLExamTypeExemptions(asyncResult);

                            if (items != null)
                            {
                                ObjPCLExamTypeExemptions = new ObservableCollection<DataEntities.PCLExamType>(items);
                                if (b_Adding)
                                {
                                    if (b_btSearch1Click)
                                    {
                                        b_btSearch1Click = false;
                                        AddPCLItem();
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        public void PCLExamTypeExamptions_XMLInsert(Int64 PromoDiscProgID, IEnumerable<PCLExamType> ObjList)
        {
            bool Result = false;

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeExemptions_XMLInsert(PromoDiscProgID, ObjPCLExamTypeExemptions, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            Result = contract.EndPCLExamTypeExemptions_XMLInsert(asyncResult);
                            if (Result)
                            {
                                MessageBox.Show(eHCMSResources.Z0655_G1_DaGhi, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0654_G1_GhiKgThCong, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        private bool b_Adding = false;
        private void AddPCLItem()
        {
            if (this.GetView() != null)
            {
                b_Adding = true;

                if (ObjExemptions_Current == null)
                    return;
                if (ObjExemptions_Current.PromoDiscProgID <= 0)
                {
                    MessageBox.Show("Chưa lưu thông tin miễn giảm", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }

                if (ObjPCLExamType_SelectForAdd != null)
                {
                    //if (b_btSearch1Click == false)
                    //{
                        if (!ObjPCLExamTypeExemptions.Contains(ObjPCLExamType_SelectForAdd))
                        {
                            ObjPCLExamTypeExemptions.Add(ObjPCLExamType_SelectForAdd);
                            b_Adding = false;
                        }
                        else
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z0357_G1_DVDaChonRoi, ObjPCLExamType_SelectForAdd.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                    //}
                    //else/*Có bấm nút Search ai biết list hiện tại là gì nên phải đọc lại list rồi add thêm vô*/
                    //{
                    //    PCLExamTypeName = "";
                    //    PCLExamTypeLocations_ByDeptLocationID();
                    //}
                }
            }
        }
        public void MedServiceItemDoubleClick()
        {
            if (ObjExemptions_Current == null || ObjExemptions_Current.PromoDiscProgID <= 0)
            {
                MessageBox.Show("Chưa lưu thông tin miễn giảm", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            
            PromoDiscountItems MedSer = new PromoDiscountItems();
            MedSer.ObjRefMedicalServiceItem = curRefMedicalServiceItem;
            MedSer.PromoDiscProgID = ObjExemptions_Current.PromoDiscProgID;
            MedSer.MedServiceID = curRefMedicalServiceItem.MedServiceID;
            if (!allMedServiceItems.Add(MedSer))
            {
                MessageBox.Show(eHCMSResources.A0453_G1_Msg_InfoDaCoDV);
            }
        }
        public void PCLDoubleClick()
        {
            if (ObjPCLExamType_SelectForAdd != null)
            {
                AddPCLItem();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }
        public void btSearch()
        {
            //if (ObjTreeNodeRefDepartments_Current.NodeID > 0)
            {
                if (ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID > 0)
                {
                    //ExemptionsSearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
                    //ExemptionsSearchCriteria.PromoDiscProgID = ObjExemptions_Current.PromoDiscProgID;
                    //ObjGetExemptionsMedServiceItems_Paging.PageIndex = 0;
                    //GetExemptionsMedServiceItems_Paging(0, ObjGetExemptionsMedServiceItems_Paging.PageSize, true);

                    RefMedSerItemsSearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
                    ObjMedServiceItems_Paging.PageIndex = 0;
                    GetMedServiceItems_Paging(0, ObjGetExemptionsMedServiceItems_Paging.PageSize, true); 

                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0327_G1_Msg_InfoChonLoaiDV, eHCMSResources.G1174_G1_TimKiem, MessageBoxButton.OK);
                }
            }
        }
        public void hplDeletePCL_Click(object selectedItem)
        {
            if (ObjExemptions_Current == null || ObjExemptions_Current.PromoDiscProgID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0355_G1_Msg_InfoChonPg);
                return;
            }

            PCLExamType p = selectedItem as PCLExamType;
            if (p != null && p.PCLExamTypeID > 0)
            {
                ObjPCLExamTypeExemptions.Remove(p);
            }

            //ObjPCLExamTypeLocations_ByDeptLocationID.Remove(selectedItem as PCLExamType);
        }
        public void hplDeleteService_Click(object datacontext)
        {
            PromoDiscountItems p = datacontext as PromoDiscountItems;

            //if (p.PriceType == "PriceFuture-Active-1")
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.ObjRefMedicalServiceItem.MedServiceName), eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    allMedServiceItems.Remove(p);
                    if(p.PromoDiscItemID == 0)
                    {
                        allMedServiceItems.NewObject.Remove(p);
                    }
                    //DeptMedServiceItems_TrueDelete(p.DeptMedServItemID, p.ObjRefMedicalServiceItem.MedServItemPriceID, p.MedServiceID);
                }
            }
            //else if (p.PriceType == "PriceCurrent")
            //{
            //    if (MessageBox.Show(string.Format("{0} ", eHCMSResources.A0156_G1_Msg_ConfXoaDV) + p.ObjMedServiceID.MedServiceName + ", Này Không?", "Xóa Dịch Vụ", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //    {
            //        RefMedicalServiceItems_MarkDeleted(p.MedServiceID);
            //    }
            //}
        }
        public void btAddChoosePCL()
        {
            if (ObjPCLExamType_SelectForAdd != null)
            {
                AddPCLItem();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }
        public void btAddChooseMedService()
        {
            if (ObjExemptions_Current == null || ObjExemptions_Current.PromoDiscProgID <= 0)
            {
                MessageBox.Show("Chưa lưu thông tin miễn giảm", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            PromoDiscountItems MedSer = new PromoDiscountItems();
            MedSer.ObjRefMedicalServiceItem = curRefMedicalServiceItem;
            MedSer.PromoDiscProgID = ObjExemptions_Current.PromoDiscProgID;
            MedSer.MedServiceID = curRefMedicalServiceItem.MedServiceID;
            if (!allMedServiceItems.Add(MedSer))
            {
                MessageBox.Show(eHCMSResources.A0453_G1_Msg_InfoDaCoDV);
            }
        }

        public void btSaveMedServiceItems()
        {
            if (ObjExemptions_Current == null)
                return;
            if (ObjExemptions_Current.PromoDiscProgID <= 0)
            {
                MessageBox.Show("Chưa lưu thông tin miễn giảm");
                return;
            }
            //if(allMedServiceItems.DeleteObject != null
            //    && allMedServiceItems.DeleteObject.Count > 0
            //    && allMedServiceItems.NewObject != null
            //    && allMedServiceItems.NewObject.Count > 0)
            //{
            //    MessageBox.Show("Không thể thêm dịch vụ và xóa cùng lúc vui lòng tải lại thông tin và");
            //    return;
            //}
            if (allMedServiceItems.DeleteObject != null
                && allMedServiceItems.DeleteObject.Count > 0)
            {
                MedServiceItems_DeleteXML(allMedServiceItems.DeleteObject);
            }
            if (allMedServiceItems.NewObject != null
                && allMedServiceItems.NewObject.Count > 0)
            {
                MedServiceItems_InsertXML(allMedServiceItems.NewObject);
            }
        }
        public void btSavePCLItems()
        {
            if (ObjExemptions_Current == null)
                return;
            if (ObjExemptions_Current.PromoDiscProgID <= 0)
            {
                MessageBox.Show("Chưa lưu thông tin miễn giảm");
                return;
            }

            if (ObjPCLExamTypeExemptions != null && ObjPCLExamTypeExemptions.Count > 0)
            {
                PCLExamTypeExamptions_XMLInsert(ObjExemptions_Current.PromoDiscProgID, ObjPCLExamTypeExemptions);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }
        public void btSave()
        {
            if (CheckValid(ObjExemptions_Current))
            {
                Exemptions_InsertUpdate(ObjExemptions_Current, true);
            }
        }
        public void btClose()
        {
            TryClose();
        }
        private bool b_btSearch1Click = false;
        public void btSearchPCL()
        {
            b_btSearch1Click = true;
            if (ObjExemptions_Current.PromoDiscProgID > 0)
            {
                //PCLExamTypeLocations_ByDeptLocationID();
            }
            else
            {
                MessageBox.Show("Chưa lưu thông tin miễn giảm");
                return;
            }
            if (SearchCriteria.V_PCLMainCategory > 0)
            {
                ObjPCLExamTypes_List_Paging.PageIndex = 0;
                PCLExamTypes_List_Paging(0, ObjPCLExamTypes_List_Paging.PageSize, true);
            }
            else//-1 Text yêu cầu chọn
            {
                MessageBox.Show(eHCMSResources.A0335_G1_Msg_InfoChonLoai, eHCMSResources.G1174_G1_TimKiem, MessageBoxButton.OK);
            }
        }
        public void rdRegistrationType_Checked(object sender, RoutedEventArgs e)
        {
            if (OutPatientChecked)
            {
                ObjExemptions_Current.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
            }
            else if (InPatientChecked)
            {
                ObjExemptions_Current.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
            }
            else
            {
                ObjExemptions_Current.V_RegistrationType = 0;
            }
        }
    }
}
