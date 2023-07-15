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
using System.Collections.Generic;
using System.ServiceModel;
using aEMR.Common;
using aEMR.DataContracts;
using aEMR.Common.Collections;
using aEMR.Controls;
using System.Windows.Controls;

namespace aEMR.Configuration.AdmissionCriterion_Mgnt.ViewModels
{
    [Export(typeof(IGroupPCLs_AddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class GroupPCLs_AddEditViewModel : Conductor<object>, IGroupPCLs_AddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public GroupPCLs_AddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            //GetGroupPCLsTypes();
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
            PCLExamType_ByGroupPCL = new ObservableCollection<PCLExamType>();
        }
        void ObjPCLExamTypes_List_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PCLExamTypes_List_Paging(ObjPCLExamTypes_List_Paging.PageIndex, ObjPCLExamTypes_List_Paging.PageSize, false);
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


        private GroupPCLs _ObjGroupPCLs_Current;
        public GroupPCLs ObjGroupPCLs_Current
        {
            get { return _ObjGroupPCLs_Current; }
            set
            {
                _ObjGroupPCLs_Current = value;
                NotifyOfPropertyChange(() => ObjGroupPCLs_Current);
            }
        }
        private ObservableCollection<Lookup> _SymptomTypes;
        public ObservableCollection<Lookup> SymptomTypes
        {
            get { return _SymptomTypes; }
            set
            {
                _SymptomTypes = value;
                NotifyOfPropertyChange(() => SymptomTypes);
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
        private ObservableCollection<PCLExamType> _PCLExamType_ByGroupPCL;
        public ObservableCollection<PCLExamType> PCLExamType_ByGroupPCL
        {
            get { return _PCLExamType_ByGroupPCL; }
            set
            {
                _PCLExamType_ByGroupPCL = value;
                NotifyOfPropertyChange(() => PCLExamType_ByGroupPCL);
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
        public void InitializeItem()
        {
            if (ObjGroupPCLs_Current.GroupPCLID > 0)
            {
                PCLExamType_ByGroupPCLID();
            }
        }

        public void btSave()
        {
            if (!string.IsNullOrEmpty(ObjGroupPCLs_Current.GroupPCLName))
            {
                if (CheckValid(ObjGroupPCLs_Current))
                {
                    GroupPCLs_InsertUpdate(ObjGroupPCLs_Current, true);
                }
            }
            else
            {
                MessageBox.Show("Tên không được để trống!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
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
        private bool b_btSearch1Click = false;
        public void btSearchPCL()
        {
            b_btSearch1Click = true;
            //if (ObjGroupPCLs_Current.GroupPCLID > 0)
            //{
            //    PCLExamType_ByGroupPCLID();
            //}
            //else
            //{
            //    MessageBox.Show("Chưa lưu thông tin nhóm");
            //    return;
            //}
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
        public bool CheckValid(object temp)
        {
            GroupPCLs p = temp as GroupPCLs;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }

        public void btClose()
        {
            TryClose();
        }
        public void PCLExamType_ByGroupPCLID()
        {
            PCLExamType_ByGroupPCL.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách PCLExamType..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamType_ByGroupPCLID(ObjGroupPCLs_Current.GroupPCLID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPCLExamType_ByGroupPCLID(asyncResult);

                            if (items != null)
                            {
                                PCLExamType_ByGroupPCL = new ObservableCollection<PCLExamType>(items);
                                //if (b_Adding)
                                //{
                                //    if (b_btSearch1Click)
                                //    {
                                //        b_btSearch1Click = false;
                                //        AddPCLItem();
                                //    }
                                //}
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
        public void PCLExamTypeExamptions_XMLInsert(Int64 GroupPCLID, IEnumerable<PCLExamType> ObjList)
        {
            bool Result = false;

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeGroupPCL_XMLInsert(GroupPCLID, PCLExamType_ByGroupPCL,(long)Globals.LoggedUserAccount.StaffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            Result = contract.EndPCLExamTypeGroupPCL_XMLInsert(asyncResult);
                            if (Result)
                            {
                                PCLExamType_ByGroupPCLID();
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

                if (ObjGroupPCLs_Current == null)
                    return;
                if (ObjGroupPCLs_Current.GroupPCLID <= 0)
                {
                    MessageBox.Show("Chưa lưu thông tin nhóm", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }

                if (ObjPCLExamType_SelectForAdd != null)
                {
                    if (!PCLExamType_ByGroupPCL.Contains(ObjPCLExamType_SelectForAdd))
                    {
                        PCLExamType_ByGroupPCL.Add(ObjPCLExamType_SelectForAdd);
                        b_Adding = false;
                    }
                    else
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z0357_G1_DVDaChonRoi, ObjPCLExamType_SelectForAdd.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    }
                    //if (b_btSearch1Click == false)
                    //{
                    //    if (!PCLExamType_ByGroupPCL.Contains(ObjPCLExamType_SelectForAdd))
                    //    {
                    //        PCLExamType_ByGroupPCL.Add(ObjPCLExamType_SelectForAdd);
                    //        b_Adding = false;
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show(string.Format(eHCMSResources.Z0357_G1_DVDaChonRoi, ObjPCLExamType_SelectForAdd.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    //    }
                    //}
                    //else/*Có bấm nút Search ai biết list hiện tại là gì nên phải đọc lại list rồi add thêm vô*/
                    //{
                    //    PCLExamTypeName = "";
                    //    PCLExamType_ByGroupPCLID();
                    //}
                }
            }
        }
        public void hplDeletePCL_Click(object selectedItem)
        {
            if (ObjGroupPCLs_Current == null || ObjGroupPCLs_Current.GroupPCLID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0355_G1_Msg_InfoChonPg);
                return;
            }

            PCLExamType p = selectedItem as PCLExamType;
            if (p != null && p.PCLExamTypeID > 0)
            {
                PCLExamType_ByGroupPCL.Remove(p);
            }

            //ObjPCLExamType_ByGroupPCLID.Remove(selectedItem as PCLExamType);
        }
        public void btSavePCLItems()
        {
            if (ObjGroupPCLs_Current == null)
                return;
            if (ObjGroupPCLs_Current.GroupPCLID <= 0)
            {
                MessageBox.Show("Chưa lưu thông tin nhóm");
                return;
            }

            if (PCLExamType_ByGroupPCL != null && PCLExamType_ByGroupPCL.Count > 0)
            {
                PCLExamTypeExamptions_XMLInsert(ObjGroupPCLs_Current.GroupPCLID, PCLExamType_ByGroupPCL);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }
        private void GroupPCLs_InsertUpdate(GroupPCLs Obj, bool SaveToDB)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;
                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    Obj.CreatedStaffID = (long)Globals.LoggedUserAccount.StaffID;
                    contract.BeginGroupPCLs_Save(Obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndGroupPCLs_Save(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Duplex-Name":
                                    {
                                        MessageBox.Show(string.Format("{0} {1}!", "Nhóm đã tồn tại!", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Update-1":
                                    {
                                        Globals.EventAggregator.Publish(new SaveEvent<GroupPCLs>() { Result = Obj });
                                        TitleForm = string.Format("{0}", eHCMSResources.T1484_G1_HChinh);
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        //TryClose();
                                        break;
                                    }
                                case "Insert-0":
                                    {
                                        MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "Insert-1":
                                    {
                                        Globals.EventAggregator.Publish(new SaveEvent<GroupPCLs>() { Result = Obj });
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        //TryClose();
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
        private bool CheckClickHeaderNotValid()
        {
            if (SearchCriteria.V_PCLMainCategory > 0)
                return true;
            return false;
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
        //private void GetGroupPCLsTypes()
        //{
        //    SymptomTypes = new ObservableCollection<Lookup>();
        //    foreach (var tmpLookup in Globals.AllLookupValueList)
        //    {
        //        if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_SymptomType))
        //        {
        //            SymptomTypes.Add(tmpLookup);
        //        }
        //    }
        //}
    }
}
