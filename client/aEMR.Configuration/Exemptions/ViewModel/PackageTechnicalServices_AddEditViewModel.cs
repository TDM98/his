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
using DataEntities.MedicalInstruction;
using aEMR.Common.BaseModel;

namespace aEMR.Configuration.Exemptions.ViewModels
{
    [Export(typeof(IPackageTechnicalServices_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PackageTechnicalServices_AddEditViewModel : ViewModelBase, IPackageTechnicalServices_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PackageTechnicalServices_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            RefMedSerItemsSearchCriteria = new RefMedicalServiceItemsSearchCriteria();
            ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<RefMedicalServiceType>();
            GetAllMedicalServiceTypes_SubtractPCL();

            ObjMedServiceItems_Paging = new PagedSortableCollectionView<RefMedicalServiceItem>();
            ObjMedServiceItems_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjMedServiceItems_Paging_OnRefresh);

            ObjGetPackageTechnicalServiceMedServiceItems_Paging = new PagedSortableCollectionView<PackageTechnicalServiceDetail>();
            ObjGetPackageTechnicalServiceMedServiceItems_Paging.PageSize = 1000;
            ObjGetPackageTechnicalServiceMedServiceItems_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjGetPackageTechnicalServiceMedServiceItems_Paging_OnRefresh);
            ExemptionsSearchCriteria = new ExemptionsMedServiceItemsSearchCriteria();

            allMedServiceItems = new ObjectEdit<PackageTechnicalServiceDetail>("MedServiceID", "PackageTechnicalServiceID", "");

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

            ObjPCLExamTypePackageTechnicalService = new ObservableCollection<PCLExamType>();
        }

        private int _ServiceQty;
        public int ServiceQty
        {
            get { return _ServiceQty; }
            set
            {
                _ServiceQty = value;
                NotifyOfPropertyChange(() => ServiceQty);
            }
        }

        private int _PCLQty;
        public int PCLQty
        {
            get { return _PCLQty; }
            set
            {
                _PCLQty = value;
                NotifyOfPropertyChange(() => PCLQty);
            }
        }

        private PackageTechnicalService _ObjPackageTechnicalServices_Current;
        public PackageTechnicalService ObjPackageTechnicalServices_Current
        {
            get { return _ObjPackageTechnicalServices_Current; }
            set
            {
                _ObjPackageTechnicalServices_Current = value;
                NotifyOfPropertyChange(() => ObjPackageTechnicalServices_Current);
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

        private ObjectEdit<PackageTechnicalServiceDetail> _allMedServiceItems;
        public ObjectEdit<PackageTechnicalServiceDetail> allMedServiceItems
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
                            GetMedServiceItems_Paging(0, ObjGetPackageTechnicalServiceMedServiceItems_Paging.PageSize, true);
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
        private PagedSortableCollectionView<PackageTechnicalServiceDetail> _ObjGetPackageTechnicalServiceMedServiceItems_Paging;
        public PagedSortableCollectionView<PackageTechnicalServiceDetail> ObjGetPackageTechnicalServiceMedServiceItems_Paging
        {
            get { return _ObjGetPackageTechnicalServiceMedServiceItems_Paging; }
            set
            {
                _ObjGetPackageTechnicalServiceMedServiceItems_Paging = value;
                NotifyOfPropertyChange(() => ObjGetPackageTechnicalServiceMedServiceItems_Paging);
            }
        }

        public void InitializeNewItem()
        {
            ObjPackageTechnicalServices_Current = new PackageTechnicalService();
            ServiceQty = 1;
            PCLQty = 1;
        }

        public void InitializeItem()
        {
            ExemptionsSearchCriteria.PromoDiscProgID = ObjPackageTechnicalServices_Current.PackageTechnicalServiceID;
            ObjGetPackageTechnicalServiceMedServiceItems_Paging.PageIndex = 0;
            GetPackageTechnicalServiceMedServiceItems_Paging(0, ObjGetPackageTechnicalServiceMedServiceItems_Paging.PageSize, true);
            if (ObjPackageTechnicalServices_Current.PackageTechnicalServiceID > 0)
            {
                PCLExamTypeLocations_ByDeptLocationID();
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
                        ObjPCLExamTypes_List_Paging.PageIndex = 0;
                        PCLExamTypes_List_Paging(0, ObjPCLExamTypes_List_Paging.PageSize, true);
                    }
                    else
                    {
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
            PackageTechnicalService p = temp as PackageTechnicalService;
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
        private PackageTechnicalServiceDetail _curMedServiceItems;
        public PackageTechnicalServiceDetail curMedServiceItems
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
        private ObservableCollection<PCLExamType> _ObjPCLExamTypePackageTechnicalService;
        public ObservableCollection<PCLExamType> ObjPCLExamTypePackageTechnicalService
        {
            get { return _ObjPCLExamTypePackageTechnicalService; }
            set
            {
                _ObjPCLExamTypePackageTechnicalService = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypePackageTechnicalService);
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

        void ObjGetPackageTechnicalServiceMedServiceItems_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetPackageTechnicalServiceMedServiceItems_Paging(ObjGetPackageTechnicalServiceMedServiceItems_Paging.PageIndex, ObjGetPackageTechnicalServiceMedServiceItems_Paging.PageSize, false);
        }

        private PagedSortableCollectionView<PCLExamType> _ObjPCLExamTypes_List_Paging;
        public PagedSortableCollectionView<PCLExamType> ObjPCLExamTypes_List_Paging
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
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypes_List_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PCLExamType> allItems = null;
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
                            finally
                            {
                                this.DlgHideBusyIndicator();
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
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }


        public void PCLExamTypeSubCategory_ByV_PCLMainCategory()
        {
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Clear();
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
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
                            this.DlgHideBusyIndicator();
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
                this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
        void ObjMedServiceItems_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetMedServiceItems_Paging(ObjMedServiceItems_Paging.PageIndex, ObjMedServiceItems_Paging.PageSize, true);
        }

        private void PackageTechnicalServices_InsertUpdate(PackageTechnicalService Obj, bool SaveToDB)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });
            if (Obj.PackageTechnicalServiceID == 0)
            {
                Obj.CreatedStaffID = (long)Globals.LoggedUserAccount.StaffID;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPackageTechnicalService_InsertUpdate(Obj, (long)Globals.LoggedUserAccount.StaffID
                        , Globals.DispatchCallback((asyncResult) =>
                     {
                         try
                         {
                             string Result = "";
                             long NewID = 0;
                             contract.EndPackageTechnicalService_InsertUpdate(out Result, out NewID, asyncResult);
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
                                         TitleForm = string.Format("{0} ({1})", eHCMSResources.T1484_G1_HChinh, ObjPackageTechnicalServices_Current.Title.Trim());
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
                                         ObjPackageTechnicalServices_Current.PackageTechnicalServiceID = NewID;
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
                            
                         }
                     }), null);
                }
            });

            t.Start();
        }

        public void GetAllMedicalServiceTypes_SubtractPCL()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0604_G1_DangLayDSLoaiDV);
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
                                    ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<RefMedicalServiceType>(items);

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

        private void GetMedServiceItems_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z1007_G1_LoadDSDV);
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
                                this.DlgHideBusyIndicator();
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
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        public void MedServiceItems_InsertXML(ObservableCollection<PackageTechnicalServiceDetail> lstMedServiceItems)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPackageTechnicalServiceMedServiceItems_InsertXML(lstMedServiceItems, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndPackageTechnicalServiceMedServiceItems_InsertXML(asyncResult))
                                {
                                    ExemptionsSearchCriteria.PromoDiscProgID = ObjPackageTechnicalServices_Current.PackageTechnicalServiceID;
                                    ObjGetPackageTechnicalServiceMedServiceItems_Paging.PageIndex = 0;
                                    GetPackageTechnicalServiceMedServiceItems_Paging(0, ObjGetPackageTechnicalServiceMedServiceItems_Paging.PageSize, true);
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

        public void MedServiceItems_DeleteXML(ObservableCollection<PackageTechnicalServiceDetail> lstMedServiceItems)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPackageTechnicalServiceMedServiceItems_DeleteXML(lstMedServiceItems, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndPackageTechnicalServiceMedServiceItems_DeleteXML(asyncResult))
                                {
                                    ExemptionsSearchCriteria.PromoDiscProgID = ObjPackageTechnicalServices_Current.PackageTechnicalServiceID;
                                    ObjGetPackageTechnicalServiceMedServiceItems_Paging.PageIndex = 0;
                                    GetPackageTechnicalServiceMedServiceItems_Paging(0, ObjGetPackageTechnicalServiceMedServiceItems_Paging.PageSize, true);
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

        private void GetPackageTechnicalServiceMedServiceItems_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z1007_G1_LoadDSDV);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPackageTechnicalServiceMedServiceItems_Paging(ExemptionsSearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PackageTechnicalServiceDetail> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetPackageTechnicalServiceMedServiceItems_Paging(out Total, asyncResult);
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

                            ObjGetPackageTechnicalServiceMedServiceItems_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjGetPackageTechnicalServiceMedServiceItems_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjGetPackageTechnicalServiceMedServiceItems_Paging.Add(item);
                                    }
                                }
                                allMedServiceItems = new ObjectEdit<PackageTechnicalServiceDetail>(ObjGetPackageTechnicalServiceMedServiceItems_Paging
                                    , "MedServiceID", "PackageTechnicalServiceID", "");
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

        public void PCLExamTypeLocations_ByDeptLocationID()
        {
            ObjPCLExamTypePackageTechnicalService.Clear();
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypePackageTechnicalService(PCLExamTypeName, ObjPackageTechnicalServices_Current.PackageTechnicalServiceID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPCLExamTypePackageTechnicalService(asyncResult);

                            if (items != null)
                            {
                                ObjPCLExamTypePackageTechnicalService = new ObservableCollection<PCLExamType>(items);
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
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }

            });

            t.Start();
        }

        public void PCLExamTypeExamptions_XMLInsert(long PackageTechnicalServiceID, IEnumerable<PCLExamType> ObjList)
        {
            bool Result = false;
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypePackageTechnicalService_XMLInsert(PackageTechnicalServiceID, ObjPCLExamTypePackageTechnicalService, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            Result = contract.EndPCLExamTypePackageTechnicalService_XMLInsert(asyncResult);
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
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private bool b_Adding = false;
        private void AddPCLItem()
        {
            if (GetView() != null)
            {
                b_Adding = true;

                if (ObjPackageTechnicalServices_Current == null)
                    return;
                if (ObjPackageTechnicalServices_Current.PackageTechnicalServiceID <= 0)
                {
                    MessageBox.Show("Chưa lưu thông tin gói dvkt", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }

                if (ObjPCLExamType_SelectForAdd != null)
                {
                    if (b_btSearch1Click == false)
                    {
                        if (!ObjPCLExamTypePackageTechnicalService.Contains(ObjPCLExamType_SelectForAdd))
                        {
                            ObjPCLExamTypePackageTechnicalService.Add(ObjPCLExamType_SelectForAdd);
                            b_Adding = false;
                        }
                        else
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z0357_G1_DVDaChonRoi, ObjPCLExamType_SelectForAdd.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                    }
                    else/*Có bấm nút Search ai biết list hiện tại là gì nên phải đọc lại list rồi add thêm vô*/
                    {
                        PCLExamTypeName = "";
                        PCLExamTypeLocations_ByDeptLocationID();
                    }
                }
            }
        }

        public void MedServiceItemDoubleClick()
        {
            if (ObjPackageTechnicalServices_Current == null || ObjPackageTechnicalServices_Current.PackageTechnicalServiceID <= 0)
            {
                MessageBox.Show("Chưa lưu thông tin gói dvkt", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            PackageTechnicalServiceDetail MedSer = new PackageTechnicalServiceDetail();
            MedSer.ObjRefMedicalServiceItem = curRefMedicalServiceItem;
            MedSer.PackageTechnicalServiceID = ObjPackageTechnicalServices_Current.PackageTechnicalServiceID;
            MedSer.MedServiceID = curRefMedicalServiceItem.MedServiceID;
            MedSer.Qty = ServiceQty;
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
            {
                if (ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID > 0)
                {
                    ExemptionsSearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
                    ExemptionsSearchCriteria.PromoDiscProgID = ObjPackageTechnicalServices_Current.PackageTechnicalServiceID;
                    ObjGetPackageTechnicalServiceMedServiceItems_Paging.PageIndex = 0;
                    GetPackageTechnicalServiceMedServiceItems_Paging(0, ObjGetPackageTechnicalServiceMedServiceItems_Paging.PageSize, true);

                    RefMedSerItemsSearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
                    ObjMedServiceItems_Paging.PageIndex = 0;
                    GetMedServiceItems_Paging(0, ObjGetPackageTechnicalServiceMedServiceItems_Paging.PageSize, true);
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0327_G1_Msg_InfoChonLoaiDV, eHCMSResources.G1174_G1_TimKiem, MessageBoxButton.OK);
                }
            }
        }

        public void hplDeletePCL_Click(object selectedItem)
        {
            if (ObjPackageTechnicalServices_Current == null || ObjPackageTechnicalServices_Current.PackageTechnicalServiceID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0355_G1_Msg_InfoChonPg);
                return;
            }

            PCLExamType p = selectedItem as PCLExamType;
            if (p != null && p.PCLExamTypeID > 0)
            {
                ObjPCLExamTypePackageTechnicalService.Remove(p);
            }
        }

        public void hplDeleteService_Click(object datacontext)
        {
            PackageTechnicalServiceDetail p = datacontext as PackageTechnicalServiceDetail;
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.ObjRefMedicalServiceItem.MedServiceName), eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    allMedServiceItems.Remove(p);
                    if (p.PackageTechnicalServiceDetailID == 0)
                    {
                        allMedServiceItems.NewObject.Remove(p);
                    }
                }
            }
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
            if (curRefMedicalServiceItem == null)
            {
                return;
            }
            if (ObjPackageTechnicalServices_Current == null || ObjPackageTechnicalServices_Current.PackageTechnicalServiceID <= 0)
            {
                MessageBox.Show("Chưa lưu thông tin gói dvkt", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            PackageTechnicalServiceDetail MedSer = new PackageTechnicalServiceDetail();
            MedSer.ObjRefMedicalServiceItem = curRefMedicalServiceItem;
            MedSer.PackageTechnicalServiceID = ObjPackageTechnicalServices_Current.PackageTechnicalServiceID;
            MedSer.MedServiceID = curRefMedicalServiceItem.MedServiceID;
            MedSer.Qty = ServiceQty;
            if (!allMedServiceItems.Add(MedSer))
            {
                MessageBox.Show(eHCMSResources.A0453_G1_Msg_InfoDaCoDV);
            }
        }

        public void btSaveMedServiceItems()
        {
            if (ObjPackageTechnicalServices_Current == null)
                return;
            if (ObjPackageTechnicalServices_Current.PackageTechnicalServiceID <= 0)
            {
                MessageBox.Show("Chưa lưu thông tin gói dvkt");
                return;
            }
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
            if (ObjPackageTechnicalServices_Current == null)
                return;
            if (ObjPackageTechnicalServices_Current.PackageTechnicalServiceID <= 0)
            {
                MessageBox.Show("Chưa lưu thông tin gói dvkt");
                return;
            }

            if (ObjPCLExamTypePackageTechnicalService != null && ObjPCLExamTypePackageTechnicalService.Count > 0)
            {
                PCLExamTypeExamptions_XMLInsert(ObjPackageTechnicalServices_Current.PackageTechnicalServiceID, ObjPCLExamTypePackageTechnicalService);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        public void btSave()
        {
            if (CheckValid(ObjPackageTechnicalServices_Current))
            {
                PackageTechnicalServices_InsertUpdate(ObjPackageTechnicalServices_Current, true);
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
            if (ObjPackageTechnicalServices_Current.PackageTechnicalServiceID > 0)
            {
                PCLExamTypeLocations_ByDeptLocationID();
            }
            else
            {
                MessageBox.Show("Chưa lưu thông tin gói DVKT");
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
    }
}
