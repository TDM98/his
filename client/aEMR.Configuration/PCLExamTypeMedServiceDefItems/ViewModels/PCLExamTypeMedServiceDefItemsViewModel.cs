using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Controls;
using aEMR.Common;
using eHCMSLanguage;
using aEMR.Common.PagedCollectionView;
using Castle.Windsor;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PCLExamTypeMedServiceDefItems.ViewModels
{
    [Export(typeof(IPCLExamTypeMedServiceDefItems)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypeMedServiceDefItemsViewModel : Conductor<object>, IPCLExamTypeMedServiceDefItems
        , IHandle<DbClickSelectedObjectEvent<PCLExamType>>
        , IHandle<SelectedObjectEvent<PCLExamType>>
    {
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



        private object _leftContent;
        public object leftContent
        {
            get
            {
                return _leftContent;
            }
            set
            {
                if (_leftContent == value)
                    return;
                _leftContent = value;
                NotifyOfPropertyChange(() => leftContent);
            }
        }

        private RefMedicalServiceItem _ObjRefMedicalServiceItem_isPCL;
        public RefMedicalServiceItem ObjRefMedicalServiceItem_isPCL
        {
            get { return _ObjRefMedicalServiceItem_isPCL; }
            set
            {
                if (_ObjRefMedicalServiceItem_isPCL != value)
                {
                    _ObjRefMedicalServiceItem_isPCL = value;
                    NotifyOfPropertyChange(() => ObjRefMedicalServiceItem_isPCL);
                }
            }
        }
        [ImportingConstructor]
        public PCLExamTypeMedServiceDefItemsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);

        }

        //Main
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

        public void LoadV_PCLMainCategory()
        {
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent{IsBusy = true,Message = "Danh Sách Loại..."});

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
                                    firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
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
        //Main


        //Sub
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

        public void PCLExamTypeSubCategory_ByV_PCLMainCategory()
        {
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Nhóm..." });

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
        //Sub

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

        public void FormLoad()
        {
            authorization();

            //Load UC
            var UCPCLExamTypes = Globals.GetViewModel<IPCLExamTypes_List_Paging>();
            leftContent = UCPCLExamTypes;
            (this as Conductor<object>).ActivateItem(leftContent);
            UCPCLExamTypes.SearchCriteria = new PCLExamTypeSearchCriteria();
            UCPCLExamTypes.SearchCriteria.IsNotInPCLExamTypeLocations = true;
            UCPCLExamTypes.SearchCriteria.IsNotInPCLItems = true;
            UCPCLExamTypes.IsNotInPCLItemsVisibility = Visibility.Collapsed;
            UCPCLExamTypes.FormLoad();
            //Load UC

            ObjV_PCLMainCategory = new ObservableCollection<Lookup>();
            ObjV_PCLMainCategory_Selected = new Lookup();
            ObjV_PCLMainCategory_Selected.LookupID = -1;

            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory = new ObservableCollection<PCLExamTypeSubCategory>();
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected = new PCLExamTypeSubCategory();
            ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected.PCLExamTypeSubCategoryID = -1;

            LoadV_PCLMainCategory();

            SearchCriteria = new PCLExamTypeSearchCriteria();

            HasGroup = true;

            if (ObjRefMedicalServiceItem_isPCL != null)
            {
                PCLExamTypeMedServiceDefItems_ByMedServiceID(HasGroup);
            }

        }

        private ObservableCollection<PCLExamType> _ObjPCLExamTypeMedServiceDefItems_ByMedServiceID_TMP;
        public ObservableCollection<PCLExamType> ObjPCLExamTypeMedServiceDefItems_ByMedServiceID_TMP
        {
            get { return _ObjPCLExamTypeMedServiceDefItems_ByMedServiceID_TMP; }
            set
            {
                _ObjPCLExamTypeMedServiceDefItems_ByMedServiceID_TMP = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeMedServiceDefItems_ByMedServiceID_TMP);
            }
        }

        private PagedCollectionView _ObjPCLExamTypeMedServiceDefItems_ByMedServiceID;
        public PagedCollectionView ObjPCLExamTypeMedServiceDefItems_ByMedServiceID
        {
            get { return _ObjPCLExamTypeMedServiceDefItems_ByMedServiceID; }
            set
            {
                _ObjPCLExamTypeMedServiceDefItems_ByMedServiceID = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeMedServiceDefItems_ByMedServiceID);
            }
        }

        private bool _HasGroup;
        public bool HasGroup
        {
            get { return _HasGroup; }
            set
            {
                if (_HasGroup != value)
                {
                    _HasGroup = value;
                    NotifyOfPropertyChange(() => HasGroup);
                }
            }
        }

        private void PCLExamTypeMedServiceDefItems_ByMedServiceID(bool isGroup)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách PCLExamType..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeMedServiceDefItems_ByMedServiceID(SearchCriteria, ObjRefMedicalServiceItem_isPCL.MedServiceID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPCLExamTypeMedServiceDefItems_ByMedServiceID(asyncResult);

                            if (items != null)
                            {
                                ObjPCLExamTypeMedServiceDefItems_ByMedServiceID_TMP = new ObservableCollection<PCLExamType>(items);

                                ObjPCLExamTypeMedServiceDefItems_ByMedServiceID = new PagedCollectionView(items);
                                
                                if (b_Adding)
                                {
                                    if (b_btSearch1Click)
                                    {
                                        b_btSearch1Click = false;
                                        AddItem();
                                    }
                                }

                                if (isGroup)
                                {
                                    ObjPCLExamTypeMedServiceDefItems_ByMedServiceID.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("PCLSectionName"));
                                }
                            }
                            else
                            {
                                ObjPCLExamTypeMedServiceDefItems_ByMedServiceID = null;
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


        public void cboV_PCLMainCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AxComboBox Ctr = sender as AxComboBox;
            if (Ctr == null)
                return;

            Lookup Objtmp = Ctr.SelectedItemEx as Lookup;

            if (Objtmp != null)
            {
                SearchCriteria.V_PCLMainCategory = Objtmp.LookupID;

                if (Objtmp.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    ObjPCLExamTypeSubCategory_ByV_PCLMainCategory.Clear();
                    PCLExamTypeMedServiceDefItems_ByMedServiceID(HasGroup);
                }
                else
                {
                    ObjPCLExamTypeSubCategory_ByV_PCLMainCategory_Selected.PCLExamTypeSubCategoryID = -1;
                    PCLExamTypeSubCategory_ByV_PCLMainCategory();
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

                PCLExamTypeMedServiceDefItems_ByMedServiceID(HasGroup);
            }
        }



        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mCauHinhPCLExamTypes_Sessions,
                                               (int)oConfigurationEx.mQuanLySession_PCLExamType, (int)ePermission.mDelete);
            bBtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mCauHinhPCLExamTypes_Sessions,
                                               (int)oConfigurationEx.mQuanLySession_PCLExamType, (int)ePermission.mView);
            bBtnAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mCauHinhPCLExamTypes_Sessions,
                                               (int)oConfigurationEx.mQuanLySession_PCLExamType, (int)ePermission.mAdd);
        }

        #region checking account

        private bool _bhplDelete = true;
        private bool _bBtSearch = true;
        private bool _bBtnAdd = true;
        public bool bhplDelete
        {
            get
            {
                return _bhplDelete;
            }
            set
            {
                if (_bhplDelete == value)
                    return;
                _bhplDelete = value;
            }
        }
        public bool bBtSearch
        {
            get
            {
                return _bBtSearch;
            }
            set
            {
                if (_bBtSearch == value)
                    return;
                _bBtSearch = value;
            }
        }
        public bool bBtnAdd
        {
            get
            {
                return _bBtnAdd;
            }
            set
            {
                if (_bBtnAdd == value)
                    return;
                _bBtnAdd = value;
            }
        }
        #endregion

        private bool b_btSearch1Click = false;
        public void btFind()
        {
            b_btSearch1Click = true;
            if (ObjRefMedicalServiceItem_isPCL != null)
            {
                PCLExamTypeMedServiceDefItems_ByMedServiceID(HasGroup);
            }
        }


        public void chkGroupBy_Click(object sender, RoutedEventArgs e)
        {
            CheckBox Ctr = sender as CheckBox;
            if (Ctr == null)
                return;
            PCLExamTypeMedServiceDefItems_ByMedServiceID(HasGroup);
        }


        public void hplDelete_Click(object datacontext)
        {
            ObjPCLExamTypeMedServiceDefItems_ByMedServiceID.Remove(datacontext as PCLExamType);
            ObjPCLExamTypeMedServiceDefItems_ByMedServiceID_TMP.Remove(datacontext as PCLExamType);
        }

        private DataEntities.PCLExamType _ObjPCLExamType_SelectForAdd;
        public DataEntities.PCLExamType ObjPCLExamType_SelectForAdd
        {
            get { return _ObjPCLExamType_SelectForAdd; }
            set
            {
                _ObjPCLExamType_SelectForAdd = value;
                NotifyOfPropertyChange(() => ObjPCLExamType_SelectForAdd);
            }
        }


        public void Handle(SelectedObjectEvent<PCLExamType> message)
        {
            if (this.GetView() != null)
            {
                if (message != null)
                {
                    ObjPCLExamType_SelectForAdd = message.Result;
                }
            }
        }

        public void Handle(DbClickSelectedObjectEvent<PCLExamType> message)
        {
            if (this.GetView() != null)
            {
                if (message != null)
                {
                    ObjPCLExamType_SelectForAdd = message.Result;
                    AddItem();
                }
            }
        }


        public void btAddChoose()
        {
            if (ObjPCLExamType_SelectForAdd != null)
            {
                AddItem();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }

        private bool b_Adding = false;
        private void AddItem()
        {
            if (this.GetView() != null)
            {
                b_Adding=true;  

                if (ObjPCLExamType_SelectForAdd != null)
                {
                    if (ObjRefMedicalServiceItem_isPCL != null && ObjRefMedicalServiceItem_isPCL.MedServiceID > 0)
                    {

                        if (b_btSearch1Click == false)
                        {
                            if (!ObjPCLExamTypeMedServiceDefItems_ByMedServiceID.Contains(ObjPCLExamType_SelectForAdd))
                            {
                                ObjPCLExamTypeMedServiceDefItems_ByMedServiceID_TMP.Add(ObjPCLExamType_SelectForAdd);
                                ShowDataAfterCRUD();
                                b_Adding = false;
                            }
                            else
                            {
                                MessageBox.Show(ObjPCLExamType_SelectForAdd.PCLExamTypeName.Trim() + string.Format(" {0}", eHCMSResources.A0071_G1_Msg_InfoItemIsSelected), eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                            }
                        }
                        else/*Có bấm nút Search ai biết list hiện tại là gì nên phải đọc lại list rồi add thêm vô*/
                        {
                            SearchCriteria.PCLExamTypeName = "";
                            PCLExamTypeMedServiceDefItems_ByMedServiceID(HasGroup);
                        }
                        
                    
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.K2088_G1_ChonPCLForm2, eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                    }
                }
            }
        }

        private void ShowDataAfterCRUD()
        {
            ObjPCLExamTypeMedServiceDefItems_ByMedServiceID = new PagedCollectionView(ObjPCLExamTypeMedServiceDefItems_ByMedServiceID_TMP);

            if (HasGroup)
            {
                ObjPCLExamTypeMedServiceDefItems_ByMedServiceID.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("PCLSectionName"));
            }
        }

        public void btSaveItems()
        {
            if (ObjRefMedicalServiceItem_isPCL != null && ObjRefMedicalServiceItem_isPCL.MedServiceID > 0)
            {
                if (ObjPCLExamTypeMedServiceDefItems_ByMedServiceID != null && ObjPCLExamTypeMedServiceDefItems_ByMedServiceID.Count > 0)
                {
                    PCLExamTypeMedServiceDefItems_XMLInsert();
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("Chọn PCLForm!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        private void PCLExamTypeMedServiceDefItems_XMLInsert()
        {
            bool Result = false;

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeMedServiceDefItems_XMLInsert(ObjRefMedicalServiceItem_isPCL.MedServiceID, ObjPCLExamTypeMedServiceDefItems_ByMedServiceID_TMP, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            Result = contract.EndPCLExamTypeMedServiceDefItems_XMLInsert(asyncResult);
                            if (Result)
                            {
                                Globals.EventAggregator.Publish(new PCLItemsEvent_Save() { Result = true });

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


        public void btClose()
        {
            TryClose();
        }


    }
}






