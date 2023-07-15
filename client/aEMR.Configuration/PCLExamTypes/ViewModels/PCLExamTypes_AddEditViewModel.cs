using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;
using aEMR.Common.Collections;
using aEMR.ViewContracts;
using Castle.Windsor;
/* 
* 18052018 #001 TTM: Add condition for HICode and PCLExamTypeName_Ax
* 31052018 #002
* 20210901 #003 BLQ: Chỉnh lại cho cận lâm sàng hình ảnh có test con
*/
namespace aEMR.Configuration.PCLExamTypes.ViewModels
{
    [Export(typeof(IPCLExamTypes_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypes_AddEditViewModel : Conductor<object>, IPCLExamTypes_AddEdit
        , IHandle<SelectedObjectEvent<DataEntities.PCLExamTestItems>>
        , IHandle<PCLItemsEvent>
    {
        //Đơn Vị Tính
        private Lookup _ObjV_PCLExamTypeUnit_Selected;
        public Lookup ObjV_PCLExamTypeUnit_Selected
        {
            get { return _ObjV_PCLExamTypeUnit_Selected; }
            set
            {
                _ObjV_PCLExamTypeUnit_Selected = value;
                NotifyOfPropertyChange(() => ObjV_PCLExamTypeUnit_Selected);
            }
        }

        private ObservableCollection<Lookup> _ObjV_PCLExamTypeUnit;
        public ObservableCollection<Lookup> ObjV_PCLExamTypeUnit
        {
            get { return _ObjV_PCLExamTypeUnit; }
            set
            {
                _ObjV_PCLExamTypeUnit = value;
                NotifyOfPropertyChange(() => ObjV_PCLExamTypeUnit);
            }
        }

        private ObservableCollection<PCLForm> _ObjPCLForms_GetList_Paging;
        public ObservableCollection<PCLForm> ObjPCLForms_GetList_Paging
        {
            get { return _ObjPCLForms_GetList_Paging; }
            set
            {
                _ObjPCLForms_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjPCLForms_GetList_Paging);
            }
        }

        public void LoadV_PCLExamTypeUnit()
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.V_PCLExamTypeUnit,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_PCLExamTypeUnit = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup
                                    {
                                        LookupID = -1,
                                        ObjectValue = "--Chọn Đơn Vị--"
                                    };
                                    ObjV_PCLExamTypeUnit.Insert(0, firstItem);

                                    if (ObjPCLExamType_Current.V_PCLExamTypeUnit <= 0)
                                    {
                                        ObjPCLExamType_Current.V_PCLExamTypeUnit = -1;
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
        //Đơn Vị Tính

        //HITransactionType
        private ObservableCollection<HITransactionType> _ObjHITransactionType_GetListNoParentID;
        public ObservableCollection<HITransactionType> ObjHITransactionType_GetListNoParentID
        {
            get { return _ObjHITransactionType_GetListNoParentID; }
            set
            {
                _ObjHITransactionType_GetListNoParentID = value;
                NotifyOfPropertyChange(() => ObjHITransactionType_GetListNoParentID);
            }
        }

        //▼=====#002
        #region Khai báo các property để thực hiện việc mapping Máy và dịch vụ CLS
        //Khai báo các property để thực hiện việc mapping.
        private Resources _SelectedEquip;
        public Resources SelectedEquip
        {
            get { return _SelectedEquip; }
            set
            {
                if (_SelectedEquip != value)
                    _SelectedEquip = value;
                NotifyOfPropertyChange(() => SelectedEquip);
            }
        }

        private ObservableCollection<Resources> _tmphiRepEquip;
        public ObservableCollection<Resources> tmphiRepEquip
        {
            get { return _tmphiRepEquip; }
            set
            {
                _tmphiRepEquip = value;
                NotifyOfPropertyChange(() => tmphiRepEquip);
            }
        }

        private ObservableCollection<Resources> _hiRepEquip;
        public ObservableCollection<Resources> hiRepEquip
        {
            get { return _hiRepEquip; }
            set
            {
                _hiRepEquip = value;
                NotifyOfPropertyChange(() => hiRepEquip);
            }
        }

        //Thêm nút Delete vào Grid các máy đã mapping với CLS để người dùng có thể Delete máy không sử dụng ra khỏi bảng mapping
        DataGrid GridResources = null;
        public void GridResources_Loaded(object sender, RoutedEventArgs e)
        {
            GridResources = sender as DataGrid;
        }

        public void btnDeleteResources_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.Z2234_G1_CoMuonXoaMayNayKhong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (GridResources != null && GridResources.SelectedItem != null)
                {
                    ObjPCLExamType_Current.Resource.Remove(GridResources.SelectedItem as Resources);
                }
            }
        }

        //Thêm trang thiết bị vào dịch vụ CLS.
        public void btAdd()
        {
            if (SelectedEquip == null)
                return;
            if (ObjPCLExamType_Current.Resource == null)
                ObjPCLExamType_Current.Resource = new ObservableCollection<Resources>();
            foreach (Resources HiEquip in ObjPCLExamType_Current.Resource)
            {
                if (HiEquip.RscrID == SelectedEquip.RscrID)
                {
                    MessageBox.Show(eHCMSResources.Z2235_G1_DaTonTaiMayNay);
                    return;
                }
            }
            ObjPCLExamType_Current.Resource.Add(SelectedEquip);
        }

        //Để load các máy đã được mapping của dịch vụ CLS.
        public void GetResourcesForMedicalServices_LoadNotPaging(long PCLExamTypeID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resources" });
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ResourcesManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetResourcesForMedicalServices_LoadNotPaging(ObjPCLExamType_Current.PCLExamTypeID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mResources = contract.EndGetResourcesForMedicalServices_LoadNotPaging(asyncResult);
                                if (mResources != null)
                                {
                                    ObjPCLExamType_Current.Resource = mResources.ToObservableCollection();
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
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        #endregion
        //▲=====#002

        public void HITransactionType_GetListNoParentID()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginHITransactionType_GetListNoParentID(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndHITransactionType_GetListNoParentID(asyncResult);

                                if (items != null)
                                {
                                    ObjHITransactionType_GetListNoParentID = new ObservableCollection<HITransactionType>(items);

                                    //ItemDefault
                                    HITransactionType ItemDefault = new HITransactionType
                                    {
                                        HITTypeID = -1,
                                        HITypeDescription = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon)
                                    };
                                    ObjHITransactionType_GetListNoParentID.Insert(0, ItemDefault);
                                    //ItemDefault

                                    if (ObjPCLExamType_Current.HITTypeID <= 0)
                                    {
                                        ObjPCLExamType_Current.HITTypeID = -1;
                                    }
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
        //HITransactionType


        private bool _cboPCLExamTypeSubCategoryIsEnabled;
        public bool cboPCLExamTypeSubCategoryIsEnabled
        {
            get { return _cboPCLExamTypeSubCategoryIsEnabled; }
            set
            {
                _cboPCLExamTypeSubCategoryIsEnabled = value;
                NotifyOfPropertyChange(() => cboPCLExamTypeSubCategoryIsEnabled);
            }
        }

        private bool _cboV_PCLMainCategoryIsEnabled;
        public bool cboV_PCLMainCategoryIsEnabled
        {
            get { return _cboV_PCLMainCategoryIsEnabled; }
            set
            {
                _cboV_PCLMainCategoryIsEnabled = value;
                NotifyOfPropertyChange(() => cboV_PCLMainCategoryIsEnabled);
            }
        }

        private Visibility _InputForLab;
        public Visibility InputForLab
        {
            get
            {
                return _InputForLab;
            }
            set
            {
                _InputForLab = value;
                NotifyOfPropertyChange(() => InputForLab);
            }
        }

        private Visibility _ResultParamVisibility;
        public Visibility ResultParamVisibility
        {
            get
            {
                return _ResultParamVisibility;
            }
            set
            {
                _ResultParamVisibility = value;
                NotifyOfPropertyChange(() => ResultParamVisibility);
            }
        }

        //Main
        //private Lookup _ObjV_PCLMainCategory_Selected;
        //public Lookup ObjV_PCLMainCategory_Selected
        //{
        //    get { return _ObjV_PCLMainCategory_Selected; }
        //    set
        //    {
        //        _ObjV_PCLMainCategory_Selected = value;
        //        NotifyOfPropertyChange(() => ObjV_PCLMainCategory_Selected);
        //    }
        //}

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
        //Main


        //Sub
        //private PCLExamTypeSubCategory _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected;
        //public PCLExamTypeSubCategory ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected
        //{
        //    get { return _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected; }
        //    set
        //    {
        //        _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = value;
        //        NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected);
        //    }
        //}

        private ObservableCollection<PCLExamTypeSubCategory> _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory;
        public ObservableCollection<PCLExamTypeSubCategory> ObjPCLExamTypeSubCategory_ByV_PCLMainCategory
        {
            get { return _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory; }
            set
            {
                if (_ObjPCLExamTypeSubCategory_ByV_PCLMainCategory == value)
                {
                    return;
                }
                _ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeSubCategory_ByV_PCLMainCategory);
            }
        }


        public void PCLExamTypeSubCategory_ByV_PCLMainCategory()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPCLExamTypeSubCategory_ByV_PCLMainCategory(ObjPCLExamType_Current.ObjV_PCLMainCategory.LookupID
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndPCLExamTypeSubCategory_ByV_PCLMainCategory(asyncResult);
                                if (items != null)
                                {
                                    ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>(items);

                                    PCLExamTypeSubCategory firstItem = new PCLExamTypeSubCategory();
                                    firstItem.PCLExamTypeSubCategoryID = -1;
                                    firstItem.PCLSubCategoryName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2074_G1_ChonNhom2);
                                    ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Insert(0, firstItem);

                                    if (ObjPCLExamType_Current.PCLExamTypeID <= 0)/*TH Thêm Mới thì Nhắc Chọn*/
                                    {
                                        ObjPCLExamType_Current.ObjPCLExamTypeSubCategoryID = firstItem;
                                    }
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
        //Sub

        //PCLSection
        private ObservableCollection<PCLSection> _ObjPCLSections_All;
        public ObservableCollection<PCLSection> ObjPCLSections_All
        {
            get { return _ObjPCLSections_All; }
            set
            {
                _ObjPCLSections_All = value;
                NotifyOfPropertyChange(() => ObjPCLSections_All);
            }
        }

        private void PCLSections_All()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLSections_All(Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PCLSection> allItems = null;
                            try
                            {
                                allItems = client.EndPCLSections_All(asyncResult);

                                if (allItems != null)
                                {
                                    ObjPCLSections_All = new ObservableCollection<PCLSection>(allItems);
                                    PCLSection firstItem = new PCLSection
                                    {
                                        PCLSectionID = -1,
                                        PCLSectionName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.A0359_G1_Msg_InfoChonSection)
                                    };
                                    ObjPCLSections_All.Insert(0, firstItem);
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        //PCLSection


        //PCLResultParamImplementations
        private ObservableCollection<PCLResultParamImplementations> _ObjPCLResultParamImplementations_GetAll;
        public ObservableCollection<PCLResultParamImplementations> ObjPCLResultParamImplementations_GetAll
        {
            get { return _ObjPCLResultParamImplementations_GetAll; }
            set
            {
                _ObjPCLResultParamImplementations_GetAll = value;
                NotifyOfPropertyChange(() => ObjPCLResultParamImplementations_GetAll);
            }
        }

        private void PCLResultParamImplementations_GetAll()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLResultParamImplementations_GetAll(Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PCLResultParamImplementations> allItems = null;
                            try
                            {
                                allItems = client.EndPCLResultParamImplementations_GetAll(asyncResult);
                                if (allItems != null)
                                {
                                    ObjPCLResultParamImplementations_GetAll = new ObservableCollection<PCLResultParamImplementations>(allItems);

                                    PCLResultParamImplementations firstItem = new PCLResultParamImplementations
                                    {
                                        PCLResultParamImpID = -1,
                                        ParamName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2092_G1_ChonPCLresultparam2)
                                    };
                                    ObjPCLResultParamImplementations_GetAll.Insert(0, firstItem);
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        //PCLResultParamImplementations

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

        [ImportingConstructor]
        public PCLExamTypes_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);

            SearchCriteria = new GeneralSearchCriteria();

            ObjPCLExamTestItems = new PagedSortableCollectionView<DataEntities.PCLExamTestItems>
            {
                PageSize = Globals.PageSize
            };
            ObjPCLExamTestItems.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPCLExamTestItems_OnRefresh);
            PCLForms_GetList_Paging(0, 1000, true);
        }

        //▼=====#002
        //Comment hàm này lại, vì không cần thiết phải load tất cả TestItems lên khi load View AddEditPCLExamtypes
        //mà nên để người dùng quyết định load khi nào.
        //protected override void OnActivate()
        //{
        //        base.OnActivate();
        //        btSearch();
        //}
        //▲=====#002
        void ObjPCLExamTestItems_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLExamTestItems_SearchPaging(ObjPCLExamTestItems.PageIndex, ObjPCLExamTestItems.PageSize);
        }

        #region Thông tin bệnh viện ngoài
        private ObservableCollection<TestingAgency> _ObjTestingAgencyList;
        public ObservableCollection<TestingAgency> ObjTestingAgencyList
        {
            get
            {
                return _ObjTestingAgencyList;
            }
            set
            {
                if (_ObjTestingAgencyList != value)
                {
                    _ObjTestingAgencyList = value;
                    NotifyOfPropertyChange(() => ObjTestingAgencyList);
                }
            }
        }

        private void GetTestingAgency_All()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsImportClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetTestingAgency_All(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndGetTestingAgency_All(asyncResult);
                                if (items != null)
                                {
                                    ObjTestingAgencyList = new ObservableCollection<TestingAgency>(items);
                                    TestingAgency ItemDefault = new TestingAgency
                                    {
                                        AgencyID = -1,
                                        HosID = -1,
                                        AgencyName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon)
                                    };
                                    ObjTestingAgencyList.Insert(0, ItemDefault);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }
        #endregion

        public void FormLoad()
        {
            PCLSections_All();
            PCLResultParamImplementations_GetAll();
            LoadV_PCLExamTypeUnit();
            LoadV_ReportForm();
            HITransactionType_GetListNoParentID();

            if (ObjPCLExamType_Current != null)
            {
                if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList == null)
                    ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList = new ObservableCollection<PCLExamTypeTestItems>();

                if (ObjPCLExamType_Current.PCLExamTypeID > 0)/*Sửa*/
                {
                    cboV_PCLMainCategoryIsEnabled = false;

                    if (ObjPCLExamType_Current.PCLExamTypeSubCategoryID > 0 && ObjPCLExamType_Current.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)/*Không Phải Lab*/
                    {
                        //PCLExamTypeSubCategory_ByV_PCLMainCategory();
                        //CheckInputPCLResultParam(ObjPCLExamType_Current.ObjV_PCLMainCategory);
                        /*▼====: #003*/
                        Lookup LK = new Lookup();
                        ObjPCLExamType_Current.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;
                        ObjPCLExamType_Current.ObjV_PCLMainCategory.LookupID = (long)AllLookupValues.V_PCLMainCategory.Imaging;
                        LK.LookupID = (long)AllLookupValues.V_PCLMainCategory.Imaging;
                        //CheckInputPCLResultParam(LK);
                        PCLExamTestItems_ByPCLExamTypeID();
                        /*▲====: #003*/
                    }
                    else
                    {
                        Lookup LK = new Lookup();
                        ObjPCLExamType_Current.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
                        ObjPCLExamType_Current.ObjV_PCLMainCategory.LookupID = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
                        LK.LookupID = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
                        CheckInputPCLResultParam(LK);
                        PCLExamTestItems_ByPCLExamTypeID();
                    }
                }
                else
                {
                    cboV_PCLMainCategoryIsEnabled = true;
                    CheckInputPCLResultParam(ObjPCLExamType_Current.ObjV_PCLMainCategory);
                }

                DataPCLExamTestItems_Insert = new ObservableCollection<PCLExamTypeTestItems>();
                DataPCLExamTestItems_Delete = new ObservableCollection<PCLExamTypeTestItems>();
                DataPCLExamTestItems_Update = new ObservableCollection<PCLExamTypeTestItems>();

                GetTestingAgency_All();

                //▼=====#002
                cboV_PCLMainCategory_SelectionChanged(null, null);
                //Hàm FormLoad đc gọi khi người sử dụng click vào Hyperlink tạo mới và nút chỉnh sửa CLS.
                //Hàm GetResourcesForMedicalServices_LoadNotPaging đc gọi ở đây để load các máy đã được mapping vào dịch vụ khi Popup AddEdit đc gọi
                //vì người dùng cần được xem các máy đã mapping vs CLS trước đó
                //
                if (ObjPCLExamType_Current.PCLExamTypeID != 0)
                {
                    GetResourcesForMedicalServices_LoadNotPaging(ObjPCLExamType_Current.PCLExamTypeID);
                }
                //▲=====#002
            }
        }

        private PCLExamType _ObjPCLExamType_Current;
        public PCLExamType ObjPCLExamType_Current
        {
            get { return _ObjPCLExamType_Current; }
            set
            {
                _ObjPCLExamType_Current = value;
                NotifyOfPropertyChange(() => ObjPCLExamType_Current);
                ObjPCLExamType_DeepCopy = ObjPCLExamType_Current.DeepCopy();
                SetRadioButton();
            }
        }

        private PCLExamType _ObjPCLExamType_DeepCopy;
        public PCLExamType ObjPCLExamType_DeepCopy
        {
            get { return _ObjPCLExamType_DeepCopy; }
            set
            {
                _ObjPCLExamType_DeepCopy = value;
                NotifyOfPropertyChange(() => ObjPCLExamType_DeepCopy);
            }
        }

        public void InitializeNewItem(Lookup pObjV_PCLMainCategory, PCLExamTypeSubCategory pObjPCLExamTypeSubCategory)
        {
            ObjPCLExamType_Current = new PCLExamType
            {
                ObjV_PCLMainCategory = pObjV_PCLMainCategory,

                ObjPCLExamTypeSubCategoryID = pObjPCLExamTypeSubCategory,

                ObjPCLSectionID = new PCLSection
                {
                    PCLSectionID = -1
                },

                ObjPCLResultParamImpID = new PCLResultParamImplementations
                {
                    PCLResultParamImpID = -1
                },

                ObjPCLExamTypeTestItemsList = new ObservableCollection<PCLExamTypeTestItems>(),

                PCLExamTypeCode = "",
                PCLExamTypeName = "",
                PCLExamTypeDescription = "",
                HIApproved = false,
                IsExternalExam = false,
                IsActive = true
            };

            InputForLab = Visibility.Collapsed;
        }

        public void PCLExamTestItems_ByPCLExamTypeID()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPCLExamTestItems_ByPCLExamTypeID(ObjPCLExamType_Current.PCLExamTypeID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndPCLExamTestItems_ByPCLExamTypeID(asyncResult);
                                if (items != null && items.Count > 0)
                                {
                                    ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList = items.ToObservableCollection();
                                }
                                else
                                {
                                    ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList = new ObservableCollection<PCLExamTypeTestItems>();
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

        public void hplAddNewTestItem_Click()
        {
            if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList != null)
            {
                if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Count > 0)
                {
                    if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList[0].V_PCLExamTestItem.TestItemIsExamType)
                    {
                        MessageBox.Show(eHCMSResources.A0711_G1_Msg_InfoKhTheThemPCLExamTest);
                        return;
                    }
                }
            }

            void onInitDlg(IPCLExamTestItemsNew typeInfo)
            {
                typeInfo.TitleForm = "Thêm Mới PCLExamTest";

                typeInfo.ObjPCLExamType_Current = ObjPCLExamType_Current;

                typeInfo.InitializeNewItem();
            }
            GlobalsNAV.ShowDialog<IPCLExamTestItemsNew>(onInitDlg);
        }

        public void hplDelete_Click(object datacontext)
        {
            PCLExamTypeTestItems p = (datacontext as PCLExamTypeTestItems);

            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.V_PCLExamTestItem.PCLExamTestItemName.Trim()), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Remove(p);

                CalcDataXML("D", p);
            }
        }

        public void cboV_PCLMainCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //AxComboBox Ctr = sender as AxComboBox;
            //if (Ctr == null)
            //    return;
            //Lookup Objtmp = Ctr.SelectedItemEx as Lookup;

            if (ObjPCLExamType_Current == null)
            {
                return;
            }

            if (ObjPCLExamType_Current.ObjV_PCLMainCategory != null)
            {
                CheckInputPCLResultParam(ObjPCLExamType_Current.ObjV_PCLMainCategory);
            }
            //▼=====#002
            if (ObjPCLExamType_Current.ObjV_PCLMainCategory == null)
            {
                return;
            }
            hiRepEquip = new ObservableCollection<Resources>();
            hiRepEquip.Clear();
            //Để phân loại xem máy thuộc loại CLS nào (Chẩn đoán hình ảnh hoặc xét nghiệm) để load cho đúng loại vào cbb Tên vật tư y tế.
            if (ObjPCLExamType_Current.ObjV_PCLMainCategory.LookupID == (long)AllLookupValues.V_PCLMainCategory.Imaging)
            {
                foreach (Resources tmp in tmphiRepEquip)
                {
                    if (tmp.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                    {
                        hiRepEquip.Add(tmp);
                    }
                }
            }
            else if (ObjPCLExamType_Current.ObjV_PCLMainCategory.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
            {
                foreach (Resources tmp in tmphiRepEquip)
                {
                    if (tmp.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                    {
                        hiRepEquip.Add(tmp);
                    }
                }
            }
            if (ObjPCLExamType_Current.Resource != null)
            {
                ObjPCLExamType_Current.Resource.Clear();
            }
            //▲=====#002
        }

        private void CheckInputPCLResultParam(Lookup ObjMain)
        {
            if (ObjMain.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
            {
                InputForLab = Visibility.Visible;
                ResultParamVisibility = Visibility.Collapsed;
                cboPCLExamTypeSubCategoryIsEnabled = false;
            }
            else
            {
                InputForLab = Visibility.Collapsed;
                ResultParamVisibility = Visibility.Visible;
                cboPCLExamTypeSubCategoryIsEnabled = true;
            }

            ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList = new ObservableCollection<PCLExamTypeTestItems>(); /*Khi có chuyển Loại Main khác */

            if (ObjMain.LookupID != (long)AllLookupValues.V_PCLMainCategory.Laboratory && (ObjPCLExamTypeSubCategory_ByV_PCLMainCategory == null || ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Count == 0))
            {
                PCLExamTypeSubCategory_ByV_PCLMainCategory();
            }
        }

        public void btSave()
        {
            bool IsInsert = false;
            if (CheckValidPCLExamType(ObjPCLExamType_Current) == false)
                return;
            if (ObjPCLExamType_Current.IsExternalExam == null
                || !ObjPCLExamType_Current.IsExternalExam.Value)
            {
                ObjPCLExamType_Current.HosIDofExternalExam = null;
            }
            /*▼====: #001*/
            if (ObjPCLExamType_Current.HIApproved == true && Checkvalid_For_HI() == false)
            {
                return;
            }
            /*▲====: #001*/
            //ObjPCLExamType_Current.PCLExamTypeName = ObjPCLExamType_Current.PCLExamTypeName.Replace(" (N.V)", "");
            //ObjPCLExamType_Current.PCLExamTypeName = ObjPCLExamType_Current.PCLExamTypeName.Replace("(N.V)", "");
            if (CheckPropertyChanged(ObjPCLExamType_Current, ObjPCLExamType_DeepCopy))
            {
                IsInsert = true;
            }
            //▼====: #003
            //if (ObjPCLExamType_Current.ObjV_PCLMainCategory.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
            //{
            //    if (CheckValid_IsLab())
            //    {
            //        SetValueForIsLab();
            //        PCLExamTypes_Save_IsLab(IsInsert);
            //    }
            //}
            //else
            //{
            //    if (CheckValid_NotIsLab())
            //    {
            //        SetValueForNotIsLab();
            //        PCLExamTypes_Save_NotIsLab(IsInsert);
            //    }
            //}
            if (CheckValidGeneral())
            {
                SetValueGeneral();
                PCLExamTypes_Save_General(IsInsert);
            }
            //▲====: #003
        }

        public bool CheckValidPCLExamType(object temp)
        {
            PCLExamType p = temp as PCLExamType;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }

        private bool CheckValid_NotIsLab()
        {
            if (ObjPCLExamType_Current.ObjV_PCLMainCategory.LookupID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0335_G1_Msg_InfoChonLoai);
                return false;
            }

            if (ObjPCLExamType_Current.ObjPCLExamTypeSubCategoryID.PCLExamTypeSubCategoryID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0344_G1_Msg_InfoChonNhom);
                return false;
            }

            if (ObjPCLExamType_Current.HITTypeID <= 0)
            {
                MessageBox.Show("Chọn HITTypeID!");
                return false;
            }

            if (ObjPCLExamType_Current.V_PCLExamTypeUnit <= 0)
            {
                MessageBox.Show(eHCMSResources.A0310_G1_Msg_InfoChonDVT);
                return false;
            }

            if (ObjPCLExamType_Current.V_ReportForm <= 0)
            {
                MessageBox.Show("Chưa chọn mẫu báo cáo");
                return false;
            }

            // DPT 23-08-2017 xét trường hợp (ObjPCLExamType_Current.ObjPCLResultParamImpID == null
            if (ObjPCLExamType_Current.ObjPCLResultParamImpID == null)
            {
                ObjPCLExamType_Current.PCLResultParamImpID = null;
            }

            else if (ObjPCLExamType_Current.ObjPCLResultParamImpID.PCLResultParamImpID <= 0)
            {
                //MessageBox.Show("Chọn PCLResultParam!");
                //return false;
                ObjPCLExamType_Current.PCLResultParamImpID = null;
            }
            //▼====: #003
            if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0876_G1_Msg_InfoNhapPCLExamTest);
                return false;
            }
            //▲====: #003
            return true;
        }

        private bool CheckValid_IsLab()
        {
            if (ObjPCLExamType_Current.ObjV_PCLMainCategory.LookupID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0335_G1_Msg_InfoChonLoai);
                return false;
            }

            if (ObjPCLExamType_Current.ObjPCLSectionID == null || ObjPCLExamType_Current.ObjPCLSectionID.PCLSectionID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0359_G1_Msg_InfoChonSection);
                return false;
            }

            if (ObjPCLExamType_Current.HITTypeID <= 0)
            {
                MessageBox.Show("Chọn HITTypeID!");
                return false;
            }

            if (ObjPCLExamType_Current.V_PCLExamTypeUnit <= 0)
            {
                MessageBox.Show(eHCMSResources.A0310_G1_Msg_InfoChonDVT);
                return false;
            }

            if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0876_G1_Msg_InfoNhapPCLExamTest);
                return false;
            }
            return true;
        }

        private void SetValueForNotIsLab()
        {
            ObjPCLExamType_Current.V_PCLMainCategory = ObjPCLExamType_Current.ObjV_PCLMainCategory.LookupID;
            ObjPCLExamType_Current.PCLExamTypeSubCategoryID = ObjPCLExamType_Current.ObjPCLExamTypeSubCategoryID.PCLExamTypeSubCategoryID;
            //DPT 23/08/2017  kiểm tra ObjPCLExamType_Current.ObjPCLResultParamImpID != null 
            if (ObjPCLExamType_Current.ObjPCLResultParamImpID != null)
            {
                if (ObjPCLExamType_Current.ObjPCLResultParamImpID.PCLResultParamImpID > 0)
                {
                    ObjPCLExamType_Current.PCLResultParamImpID =
                        ObjPCLExamType_Current.ObjPCLResultParamImpID.PCLResultParamImpID;

                }
            }
            ObjPCLExamType_Current.PCLSectionID = null;
        }

        private void PCLExamTypes_Save_NotIsLab(bool IsInsert)
        {
            string Result = "";
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPCLExamTypes_Save_NotIsLab(ObjPCLExamType_Current, IsInsert, Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long PCLExamTypeID_New = 0;
                                contract.EndPCLExamTypes_Save_NotIsLab(out Result, out PCLExamTypeID_New, asyncResult);
                                switch (Result)
                                {
                                    case "0":
                                        {
                                            MessageBox.Show(eHCMSResources.A0549_G1_Msg_InfoGhiDataFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "1":
                                        {
                                            Globals.EventAggregator.Publish(new PCLExamTypesEvent_AddEditSave() { Result_AddEditSave = true });
                                            ObjPCLExamType_Current.PCLExamTypeID = PCLExamTypeID_New;
                                            MessageBox.Show(eHCMSResources.Z0655_G1_DaGhi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Error-PCLResultParamImpID":
                                        {
                                            MessageBox.Show(eHCMSResources.A0475_G1_Msg_PCLExamTypeDaTonTai, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            break;
                                        }
                                }
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

        private void SetValueForIsLab()
        {
            ObjPCLExamType_Current.V_PCLMainCategory = ObjPCLExamType_Current.ObjV_PCLMainCategory.LookupID;
            ObjPCLExamType_Current.PCLExamTypeSubCategoryID = null;
            ObjPCLExamType_Current.PCLResultParamImpID = null;
            ObjPCLExamType_Current.PCLSectionID = ObjPCLExamType_Current.ObjPCLSectionID.PCLSectionID;

            if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Count > 1)
            {
                TestItemIsExamType = false;
                PCLExamTestItemUnitForPCLExamType = "";
                PCLExamTestItemRefScaleForPCLExamType = "";
            }
            else
            {
                TestItemIsExamType = ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList[0].V_PCLExamTestItem.TestItemIsExamType;
                PCLExamTestItemUnitForPCLExamType = ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList[0].V_PCLExamTestItem.PCLExamTestItemUnit;
                PCLExamTestItemRefScaleForPCLExamType = ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList[0].V_PCLExamTestItem.PCLExamTestItemRefScale;
            }
        }

        private void PCLExamTypes_Save_IsLab(bool IsInsert)
        {
            string Result = "";
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginPCLExamTypes_Save_IsLab(ObjPCLExamType_Current, IsInsert, TestItemIsExamType, PCLExamTestItemUnitForPCLExamType, PCLExamTestItemRefScaleForPCLExamType,
                            Globals.LoggedUserAccount.StaffID.Value,
                            DataPCLExamTestItems_Insert,
                            DataPCLExamTestItems_Update,
                            DataPCLExamTestItems_Delete,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    contract.EndPCLExamTypes_Save_IsLab(out Result, asyncResult);
                                    switch (Result)
                                    {
                                        case "0":
                                            {
                                                MessageBox.Show(eHCMSResources.A0549_G1_Msg_InfoGhiDataFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                                break;
                                            }
                                        case "1":
                                            {
                                                Globals.EventAggregator.Publish(new PCLExamTypesEvent_AddEditSave() { Result_AddEditSave = true });
                                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                                TryClose();
                                                break;
                                            }
                                    }
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

        private void PCLForms_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            PCLFormsSearchCriteria SearchCriteria = new PCLFormsSearchCriteria();
            SearchCriteria.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
            SearchCriteria.OrderBy = "";
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        ObjPCLForms_GetList_Paging = new ObservableCollection<PCLForm>();
                        client.BeginPCLForms_GetList_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            try
                            {
                                ObjPCLForms_GetList_Paging = client.EndPCLForms_GetList_Paging(out Total, asyncResult).ToObservableCollection();
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

        public void btClose()
        {
            TryClose();
        }

        #region 1 ExamTest từ Form ExamTest quăng qua
        public void Handle(SelectedObjectEvent<DataEntities.PCLExamTestItems> message)
        {
            if (GetView() != null)
            {
                if (message != null)
                {
                    if (message.Result.TestItemIsExamType)
                    {
                        if (DataPCLExamTestItems_Delete == null)
                        {
                            DataPCLExamTestItems_Delete = new ObservableCollection<PCLExamTypeTestItems>();
                        }
                        foreach (PCLExamTypeTestItems p in ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList)
                        {
                            if (p.PCLExamTypeTestItemID > 0)
                            {
                                DataPCLExamTestItems_Delete.Add(p);
                            }
                        }
                        ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Clear();
                    }

                    if (CheckTrung(message.Result) == false)
                    {
                        //neu code do chua co trong danh sach xoa,thi lay lai tu ds xoa
                        bool exists = false;
                        if (DataPCLExamTestItems_Delete != null && DataPCLExamTestItems_Delete.Count > 0)
                        {
                            foreach (PCLExamTypeTestItems p in DataPCLExamTestItems_Delete)
                            {
                                if (p.V_PCLExamTestItem.PCLExamTestItemCode.ToLower() == message.Result.PCLExamTestItemCode.ToLower())
                                {
                                    exists = true;
                                    DataPCLExamTestItems_Delete.Remove(p);
                                    ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Add(p);
                                    break;
                                }
                            }
                        }
                        //nguoc lai thi them vao 
                        if (!exists)
                        {
                            //Thêm Vào 
                            PCLExamTypeTestItems p = new PCLExamTypeTestItems { PCLExamTypeID = ObjPCLExamType_Current.PCLExamTypeID, PCLExamTestItemID = message.Result.PCLExamTestItemID, V_PCLExamTestItem = message.Result };
                            ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Add(p);

                            CalcDataXML(eHCMSResources.T1779_G1_I, p);
                        }
                    }
                    else
                    {
                        MessageBox.Show("PCLExamTest" + message.Result.PCLExamTestItemName.Trim() + " này đã tồn tại!");
                    }
                }
            }
        }

        private void UpdateObjPCLExamTestItems_ByPCLExamTypeID(DataEntities.PCLExamTestItems ObjCheck)
        {
            //foreach (DataEntities.PCLExamTestItems item in ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList)
            //{
            //    if (CheckEqualObjPCLExamTestItems(item, ObjCheck))
            //    {
            //        BeginUpdate(item, ObjCheck);
            //        break;
            //    }
            //}
        }

        private bool CheckEqualObjPCLExamTestItems(PCLExamTypeTestItems A, PCLExamTypeTestItems B)
        {
            if (A.PCLExamTestItemID == B.PCLExamTestItemID)
                return true;

            if (A.V_PCLExamTestItem.CodeTMP.ToLower() == B.V_PCLExamTestItem.CodeTMP.ToLower())
                return true;

            if (A.V_PCLExamTestItem.PCLExamTestItemCode.ToLower().Trim() == B.V_PCLExamTestItem.PCLExamTestItemCode.ToLower().Trim())
                return true;
            return false;
        }

        private void BeginUpdate(DataEntities.PCLExamTestItems A, DataEntities.PCLExamTestItems B)
        {
            A.PCLExamTestItemCode = B.PCLExamTestItemCode;
            A.PCLExamTestItemName = B.PCLExamTestItemName;
            A.PCLExamTestItemDescription = B.PCLExamTestItemDescription;
            A.PCLExamTestItemUnit = B.PCLExamTestItemUnit;
            A.PCLExamTestItemRefScale = B.PCLExamTestItemRefScale;
            A.TestItemIsExamType = B.TestItemIsExamType;
        }

        private bool CheckTrung(DataEntities.PCLExamTestItems ObjCheck)
        {
            ObservableCollection<PCLExamTypeTestItems> ObjPCLExamTypeTestItemsListTMP = new ObservableCollection<PCLExamTypeTestItems>();
            ObjPCLExamTypeTestItemsListTMP = ObjectCopier.DeepCopy(ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList);

            foreach (PCLExamTypeTestItems item in ObjPCLExamTypeTestItemsListTMP)
            {
                if (item.V_PCLExamTestItem.PCLExamTestItemCode.ToLower().Trim() == ObjCheck.PCLExamTestItemCode.ToLower().Trim())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Thuộc Tính Lưu Cho Lab
        private bool _TestItemIsExamType;
        public bool TestItemIsExamType
        {
            get { return _TestItemIsExamType; }
            set
            {
                if (_TestItemIsExamType != value)
                {
                    _TestItemIsExamType = value;
                    NotifyOfPropertyChange(() => TestItemIsExamType);
                }
            }
        }

        private string _PCLExamTestItemUnitForPCLExamType;
        public string PCLExamTestItemUnitForPCLExamType
        {
            get { return _PCLExamTestItemUnitForPCLExamType; }
            set
            {
                if (_PCLExamTestItemUnitForPCLExamType != value)
                {
                    _PCLExamTestItemUnitForPCLExamType = value;
                    NotifyOfPropertyChange(() => PCLExamTestItemUnitForPCLExamType);
                }
            }
        }

        private string _PCLExamTestItemRefScaleForPCLExamType;
        public string PCLExamTestItemRefScaleForPCLExamType
        {
            get { return _PCLExamTestItemRefScaleForPCLExamType; }
            set
            {
                if (_PCLExamTestItemRefScaleForPCLExamType != value)
                {
                    _PCLExamTestItemRefScaleForPCLExamType = value;
                    NotifyOfPropertyChange(() => PCLExamTestItemRefScaleForPCLExamType);
                }
            }
        }

        private ObservableCollection<PCLExamTypeTestItems> _DataPCLExamTestItems_Insert;
        public ObservableCollection<PCLExamTypeTestItems> DataPCLExamTestItems_Insert
        {
            get { return _DataPCLExamTestItems_Insert; }
            set
            {
                _DataPCLExamTestItems_Insert = value;
                NotifyOfPropertyChange(() => DataPCLExamTestItems_Insert);
            }
        }

        private ObservableCollection<PCLExamTypeTestItems> _DataPCLExamTestItems_Update;
        public ObservableCollection<PCLExamTypeTestItems> DataPCLExamTestItems_Update
        {
            get { return _DataPCLExamTestItems_Update; }
            set
            {
                _DataPCLExamTestItems_Update = value;
                NotifyOfPropertyChange(() => DataPCLExamTestItems_Update);
            }
        }

        private ObservableCollection<PCLExamTypeTestItems> _DataPCLExamTestItems_Delete;
        public ObservableCollection<PCLExamTypeTestItems> DataPCLExamTestItems_Delete
        {
            get { return _DataPCLExamTestItems_Delete; }
            set
            {
                _DataPCLExamTestItems_Delete = value;
                NotifyOfPropertyChange(() => DataPCLExamTestItems_Delete);
            }
        }

        #endregion

        private void CalcDataXML(string sKey, PCLExamTypeTestItems Obj)
        {
            switch (sKey)
            {
                case "I":
                    {
                        DataPCLExamTestItems_Insert.Add(Obj);
                        break;
                    }
                case "D":
                    {
                        if (ObjPCLExamType_Current.PCLExamTypeID > 0)
                        {
                            if (Obj.PCLExamTypeTestItemID > 0)
                            {
                                DataPCLExamTestItems_Delete.Add(Obj);
                            }
                        }

                        break;
                    }
                case "U":
                    {
                        //if (Obj.PCLExamTestItemID > 0)/*Có cập nhật thật*/
                        //{
                        //    foreach (DataEntities.PCLExamTestItems item in DataPCLExamTestItems_Update)
                        //    {
                        //        if (CheckEqualObjPCLExamTestItems(item, Obj))
                        //        {
                        //            DataPCLExamTestItems_Update.Remove(item);
                        //            break;
                        //        }
                        //    }
                        //    DataPCLExamTestItems_Update.Add(Obj);
                        //}
                        break;
                    }
            }
        }

        /*▼====: #001*/
        private bool Checkvalid_For_HI()
        {
            if (ObjPCLExamType_Current != null)
            {
                if (string.IsNullOrEmpty(ObjPCLExamType_Current.PCLExamTypeName_Ax))
                {
                    MessageBox.Show(eHCMSResources.Z2203_G1_LoiTenBH);
                    return false;
                }
                if (string.IsNullOrEmpty(ObjPCLExamType_Current.HICode))
                {
                    MessageBox.Show(eHCMSResources.Z2204_G1_LoiMaBH);
                    return false;
                }
            }
            return true;
        }
        /*▲====: #001*/

        #region member ItemTest

        private GeneralSearchCriteria _SearchCriteria;
        public GeneralSearchCriteria SearchCriteria
        {
            get { return _SearchCriteria; }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private PagedSortableCollectionView<DataEntities.PCLExamTestItems> _ObjPCLExamTestItems;
        public PagedSortableCollectionView<DataEntities.PCLExamTestItems> ObjPCLExamTestItems
        {
            get { return _ObjPCLExamTestItems; }
            set
            {
                _ObjPCLExamTestItems = value;
                NotifyOfPropertyChange(() => ObjPCLExamTestItems);
            }
        }

        private void PCLExamTestItems_SearchPaging(int PageIndex, int PageSize)
        {
            var t = new Thread(() =>
            {
                //IsLoading = true;
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTestItems_SearchPaging(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            try
                            {
                                var allItems = client.EndPCLExamTestItems_SearchPaging(out Total, asyncResult);
                                ObjPCLExamTestItems.Clear();
                                ObjPCLExamTestItems.TotalItemCount = Total;
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPCLExamTestItems.Add(item);
                                    }
                                }
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });

            t.Start();
        }

        public void btSearch()
        {
            ObjPCLExamTestItems.PageIndex = 0;
            PCLExamTestItems_SearchPaging(ObjPCLExamTestItems.PageIndex, ObjPCLExamTestItems.PageSize);
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            DataEntities.PCLExamTestItems items = eventArgs.Value as DataEntities.PCLExamTestItems;

            //if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList != null)
            //{
            //    if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Count > 0)
            //    {
            //        if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList[0].V_PCLExamTestItem.TestItemIsExamType)
            //        {
            //            MessageBox.Show("Không Thể Thêm PCLExamTest! Vì PCLExamType này là PCLExamTest");
            //            return;
            //        }
            //    }
            //}

            if (items != null)
            {
                //if (items.TestItemIsExamType)
                //{
                //    if (MessageBox.Show("PCLExamType '" + ObjPCLExamType_Current.PCLExamTypeName.Trim() + "' này Có Chứa ExamTest bên trong!" + Environment.NewLine + "Bạn Có Muốn Bỏ Hết Các ExamTest và Cấu Hình PCLExamType là PCLExamTest không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                //    {
                //        return;
                //    }
                //}


                //if (items.TestItemIsExamType)
                //{
                //    if (DataPCLExamTestItems_Delete == null)
                //    {
                //        DataPCLExamTestItems_Delete = new ObservableCollection<PCLExamTypeTestItems>();
                //    }
                //    foreach (PCLExamTypeTestItems p in ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList)
                //    {
                //        if (p.PCLExamTypeTestItemID > 0)
                //        {
                //            DataPCLExamTestItems_Delete.Add(p);
                //        }
                //    }
                //    ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Clear();
                //}

                if (CheckTrung(items) == false)
                {
                    //neu code do chua co trong danh sach xoa,thi lay lai tu ds xoa
                    bool exists = false;
                    if (DataPCLExamTestItems_Delete != null && DataPCLExamTestItems_Delete.Count > 0)
                    {
                        foreach (PCLExamTypeTestItems p in DataPCLExamTestItems_Delete)
                        {
                            if (p.V_PCLExamTestItem.PCLExamTestItemCode.ToLower() == items.PCLExamTestItemCode.ToLower())
                            {
                                exists = true;
                                DataPCLExamTestItems_Delete.Remove(p);
                                ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Add(p);
                                break;
                            }
                        }
                    }
                    //nguoc lai thi them vao 
                    if (!exists)
                    {
                        //Thêm Vào 
                        PCLExamTypeTestItems p = new PCLExamTypeTestItems { PCLExamTypeID = ObjPCLExamType_Current.PCLExamTypeID, PCLExamTestItemID = items.PCLExamTestItemID, V_PCLExamTestItem = items };
                        ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Add(p);

                        CalcDataXML(eHCMSResources.T1779_G1_I, p);
                    }
                }
                else
                {
                    MessageBox.Show("PCLExamTest" + items.PCLExamTestItemName.Trim() + " này đã tồn tại!");
                }
            }
            //Globals.EventAggregator.Publish(new SelectedObjectEvent<DataEntities.PCLExamTestItems>() { Result = items });
            //TryClose();
        }

        public void hplEdit_Click(object datacontext)
        {
            DataEntities.PCLExamTestItems p = (datacontext as DataEntities.PCLExamTestItems);
            //  ObjPCLExamTestItems_Current = p.DeepCopy();

            void onInitDlg(IPCLExamTestItemsNew typeInfo)
            {
                typeInfo.TitleForm = eHCMSResources.Z2228_G1_ChinhSuaPCLExamTest;

                typeInfo.ObjPCLExamType_Current = ObjPCLExamType_Current;

                typeInfo.ObjPCLExamTestItems_Current = p.DeepCopy();
            }
            GlobalsNAV.ShowDialog<IPCLExamTestItemsNew>(onInitDlg);
        }

        public void Search_Code(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.FindCode = (sender as TextBox).Text;
                }
                btSearch();
            }
        }

        public void Search_Name(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.FindName = (sender as TextBox).Text;
                }
                btSearch();
            }
        }

        #endregion

        public void Handle(PCLItemsEvent message)
        {
            btSearch();
        }

        #region NewPriceType

        // Không có giá
        private bool _IsUnknown_PriceType;
        public bool IsUnknown_PriceType
        {
            get
            {
                return _IsUnknown_PriceType;
            }
            set
            {
                _IsUnknown_PriceType = value;
                if (_IsUnknown_PriceType)
                {
                    IsFixed_PriceType = !_IsUnknown_PriceType;
                    IsUpdatable_PriceType = !_IsUnknown_PriceType;
                }
                NotifyOfPropertyChange(() => IsUnknown_PriceType);
            }
        }

        // Giá cố định
        private bool _IsFixed_PriceType = true;
        public bool IsFixed_PriceType
        {
            get
            {
                return _IsFixed_PriceType;
            }
            set
            {
                _IsFixed_PriceType = value;
                if (_IsFixed_PriceType)
                {
                    IsUnknown_PriceType = !_IsFixed_PriceType;
                    IsUpdatable_PriceType = !_IsFixed_PriceType;
                }
                NotifyOfPropertyChange(() => IsFixed_PriceType);
            }
        }

        // Giá không cố định
        private bool _IsUpdatable_PriceType;
        public bool IsUpdatable_PriceType
        {
            get
            {
                return _IsUpdatable_PriceType;
            }
            set
            {
                _IsUpdatable_PriceType = value;
                if (_IsUpdatable_PriceType)
                {
                    IsUnknown_PriceType = !_IsUpdatable_PriceType;
                    IsFixed_PriceType = !_IsUpdatable_PriceType;
                }
                NotifyOfPropertyChange(() => IsUpdatable_PriceType);
            }
        }

        // Điều khiển radioButton

        public void rdbNewPriceType_Click(object sender, RoutedEventArgs e)
        {
            if (ObjPCLExamType_Current == null)
            {
                return;
            }
            if (IsUnknown_PriceType)
            {
                ObjPCLExamType_Current.V_NewPriceType = Convert.ToInt32(AllLookupValues.V_NewPriceType.Unknown_PriceType);
            }
            else if (IsFixed_PriceType)
            {
                ObjPCLExamType_Current.V_NewPriceType = Convert.ToInt32(AllLookupValues.V_NewPriceType.Fixed_PriceType);
            }
            else
            {
                ObjPCLExamType_Current.V_NewPriceType = Convert.ToInt32(AllLookupValues.V_NewPriceType.Updatable_PriceType);
            }
        }

        public void SetRadioButton()
        {
            if (ObjPCLExamType_Current == null || ObjPCLExamType_Current.V_NewPriceType == 0 || ObjPCLExamType_Current.V_NewPriceType <= 0)
            {
                return;
            }
            if (ObjPCLExamType_Current.V_NewPriceType == Convert.ToInt32(AllLookupValues.V_NewPriceType.Unknown_PriceType))
            {
                IsUnknown_PriceType = true;
            }
            else if (ObjPCLExamType_Current.V_NewPriceType == Convert.ToInt32(AllLookupValues.V_NewPriceType.Fixed_PriceType))
            {
                IsFixed_PriceType = true;
            }
            else
            {
                IsUpdatable_PriceType = true;
            }
        }

        //20190608 TBL: Kiem tra khi chinh sua cac truong quan trong se insert them dong moi trong db
        private bool CheckPropertyChanged(PCLExamType PCLExamTypeA, PCLExamType PCLExamTypeB)
        {
            if (PCLExamTypeA.PCLExamTypeCode != PCLExamTypeB.PCLExamTypeCode 
                || PCLExamTypeA.PCLExamTypeName != PCLExamTypeB.PCLExamTypeName 
                || PCLExamTypeA.HIApproved != PCLExamTypeB.HIApproved
                || PCLExamTypeA.HICode != PCLExamTypeB.HICode 
                || PCLExamTypeA.PCLExamTypeName_Ax != PCLExamTypeB.PCLExamTypeName_Ax)
            {
                return true;
            }
            return false;
        }
        #endregion
        //▼====: #003
        private bool CheckValidGeneral()
        {
            if(ObjPCLExamType_Current != null && ObjPCLExamType_Current.ObjV_PCLMainCategory != null
                && ObjPCLExamType_Current.ObjV_PCLMainCategory.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
            {
                return CheckValid_IsLab();
            }
            else
            {
                return CheckValid_NotIsLab();
            }
        }
        private void SetValueGeneral()
        {
            ObjPCLExamType_Current.V_PCLMainCategory = ObjPCLExamType_Current.ObjV_PCLMainCategory.LookupID;
            if (ObjPCLExamType_Current.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
            {
                ObjPCLExamType_Current.PCLExamTypeSubCategoryID = null;
                ObjPCLExamType_Current.PCLResultParamImpID = null;
                ObjPCLExamType_Current.PCLSectionID = ObjPCLExamType_Current.ObjPCLSectionID.PCLSectionID;
            }
            else
            {
                ObjPCLExamType_Current.PCLExamTypeSubCategoryID = ObjPCLExamType_Current.ObjPCLExamTypeSubCategoryID.PCLExamTypeSubCategoryID;
                //DPT 23/08/2017  kiểm tra ObjPCLExamType_Current.ObjPCLResultParamImpID != null 
                if (ObjPCLExamType_Current.ObjPCLResultParamImpID != null)
                {
                    if (ObjPCLExamType_Current.ObjPCLResultParamImpID.PCLResultParamImpID > 0)
                    {
                        ObjPCLExamType_Current.PCLResultParamImpID = ObjPCLExamType_Current.ObjPCLResultParamImpID.PCLResultParamImpID;
                    }
                }
                ObjPCLExamType_Current.PCLSectionID = null;
            }
            if (ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList.Count > 1)
            {
                TestItemIsExamType = false;
                PCLExamTestItemUnitForPCLExamType = "";
                PCLExamTestItemRefScaleForPCLExamType = "";
            }
            else
            {

                TestItemIsExamType = ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList[0].V_PCLExamTestItem.TestItemIsExamType;
                PCLExamTestItemUnitForPCLExamType = ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList[0].V_PCLExamTestItem.PCLExamTestItemUnit;
                PCLExamTestItemRefScaleForPCLExamType = ObjPCLExamType_Current.ObjPCLExamTypeTestItemsList[0].V_PCLExamTestItem.PCLExamTestItemRefScale;
            }
        }
        private void PCLExamTypes_Save_General(bool IsInsert)
        {
            string Result = "";
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginPCLExamTypes_Save_General(ObjPCLExamType_Current, IsInsert, TestItemIsExamType, PCLExamTestItemUnitForPCLExamType, PCLExamTestItemRefScaleForPCLExamType,
                            Globals.LoggedUserAccount.StaffID.Value,
                            DataPCLExamTestItems_Insert,
                            DataPCLExamTestItems_Update,
                            DataPCLExamTestItems_Delete,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    contract.EndPCLExamTypes_Save_General(out Result, asyncResult);
                                    switch (Result)
                                    {
                                        case "0":
                                            {
                                                MessageBox.Show(eHCMSResources.A0549_G1_Msg_InfoGhiDataFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                                break;
                                            }
                                        case "1":
                                            {
                                                Globals.EventAggregator.Publish(new PCLExamTypesEvent_AddEditSave() { Result_AddEditSave = true });
                                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                                TryClose();
                                                break;
                                            }
                                    }
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

        private ObservableCollection<Lookup> _ObjV_ReportForm;
        public ObservableCollection<Lookup> ObjV_ReportForm
        {
            get { return _ObjV_ReportForm; }
            set
            {
                _ObjV_ReportForm = value;
                NotifyOfPropertyChange(() => ObjV_ReportForm);
            }
        }
        public void LoadV_ReportForm()
        {
            var t = new Thread(() =>
            {
                this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.V_ReportForm,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_ReportForm = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup
                                    {
                                        LookupID = -1,
                                        ObjectValue = "--Chọn Loại báo cáo--"
                                    };
                                    ObjV_ReportForm.Insert(0, firstItem);

                                    if (ObjPCLExamType_Current.V_ReportForm <= 0)
                                    {
                                        ObjPCLExamType_Current.V_ReportForm = -1;
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
        //▲====: #003
    }
}
