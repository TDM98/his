using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Linq;
using Castle.Windsor;
using System.Windows.Controls;
using aEMR.Common.Utilities;
using System.Collections.Generic;
using aEMR.CommonTasks;
using aEMR.Common.Collections;
/*
* 29052018 TTM #001: Người dùng sửa/xóa vật tư thành công thay vì thông báo sửa/xóa thành công ở popup, lại hiện thông báo ở màn hình chứa popup => sửa lại cho hiện thông báo đúng vị trí.
* 29052018 TTM #002: Thêm parameter HIRepResourceCode.
* 20230424 #003 DatTB: 
* + Gộp view/model thêm mới và chỉnh sửa lại
* + Thay đổi cách truyền biến một số function
* + Chỉnh sửa màn hình thêm/sửa thiết bị
* 20230501 #004 DatTB:
* + Model thiết bị đổi từ ItemBrand sang ResourceCode
* + Lưu thêm parameter V_PCLMainCategory
*/
namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IResourcesEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourcesEditViewModel : Conductor<object>, IResourcesEdit
        , IHandle<OnChangedSelectionResourceTypes>
    {
        IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public ResourcesEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventAggregator = eventArg;
            _eventAggregator.Subscribe(this);

            _selectedResourceGroup =new ResourceGroup();

            //▼==== #003
            //_refResourceType = new ObservableCollection<ResourceType>();
            _refResourceType = new ObservableCollection<RefMedicalServiceType>();
            selectedResourceTypes = new ObservableCollection<RefMedicalServiceType>();
            MServiceTypeListID = new List<long>();
            curRscrTypeLists = new ObservableCollection<ResourceTypeLists>();
            if (Auto_PharmaCountry != null)
            {
                Auto_PharmaCountry.Text = "";
            }

            if (!isEdit)
            {
                curResource = new Resources();
            }

            Coroutine.BeginExecute(LoadDepartments());
            LoadRscrStatus_Level();

            //▼==== #004
            LoadV_PCLMainCategory_Level();
            //▲==== #004

            Coroutine.BeginExecute(GetAllRefInfo());
            //▲==== #003
        }

        public void Exit()
        {
            //Globals.EventAggregator.Publish(new ResourceEvent() {Resource = null});
            TryClose();
        }        

        //▼==== #003
        private bool _isEdit;
        public bool isEdit
        {
            get
            {
                return _isEdit;
            }
            set
            {
                if (_isEdit == value)
                    return;
                _isEdit = value;
                NotifyOfPropertyChange(() => isEdit);
            }
        }

        private long _ResourceCategoryEnum;
        public long ResourceCategoryEnum
        {
            get
            {
                return _ResourceCategoryEnum;
            }
            set
            {
                if (_ResourceCategoryEnum == value)
                    return;
                _ResourceCategoryEnum = value;
                NotifyOfPropertyChange(() => ResourceCategoryEnum);
            }
        }

        public void newResGroup()
        {
            Action<IResGroupNew> onInitDlg = (newGroup) =>
            {
                newGroup.ResourceCategoryEnum = ResourceCategoryEnum;
            };
            GlobalsNAV.ShowDialog<IResGroupNew>(onInitDlg);
        }

        //public void newResType()
        //{
        //    if (isEdit)
        //    {
        //        GlobalsNAV.ShowDialog<IResTypeNew>();
        //    }
        //    else
        //    {
        //        Action<IResTypeNew> onInitDlg = (newType) =>
        //        {
        //            newType.refResourceGroup = refResourceGroup;
        //        };
        //        GlobalsNAV.ShowDialog<IResTypeNew>(onInitDlg);
        //    }
        //}
        //▲==== #003

        public void newSuplier()
        {
        }
        #region Properties
        private Resources _curResource;
        public Resources curResource
        {
            get
            {
                return _curResource;
            }
            set
            {
                if (_curResource == value)
                    return;
                _curResource = value;
                NotifyOfPropertyChange(() => curResource);
                LoadSelectedResourceTypes();
                LoadRscrStatus_Level();
                //▼==== #004
                LoadV_PCLMainCategory_Level();
                //▲==== #004
                if (curResource.VResourceGroup!=null && refResourceGroup != null)
                {
                    selectedResourceGroup = refResourceGroup.Where(x => x.RscrGroupID == curResource.VResourceGroup.RscrGroupID).FirstOrDefault();
                }
            }
        }
        private ObservableCollection<Lookup> _refResourceUnit;
        public ObservableCollection<Lookup> refResourceUnit
        {
            get
            {
                return _refResourceUnit;
            }
            set
            {
                if (_refResourceUnit == value)
                    return;
                _refResourceUnit = value;
                NotifyOfPropertyChange(() => refResourceUnit);

                if (refResourceUnit != null && curResource != null && (curResource.V_UnitLookup == null || curResource.V_UnitLookup.LookupID == 0))
                {
                    curResource.V_UnitLookup = refResourceUnit.FirstOrDefault();
                }
            }
        }

        private ObservableCollection<ResourceGroup> _refResourceGroup;
        public ObservableCollection<ResourceGroup> refResourceGroup
        {
            get
            {
                return _refResourceGroup;
            }
            set
            {
                _refResourceGroup = value;
                NotifyOfPropertyChange(() => refResourceGroup);
                if (curResource.VResourceGroup != null && refResourceGroup != null)
                {
                    selectedResourceGroup = refResourceGroup.Where(x => x.RscrGroupID == curResource.VResourceGroup.RscrGroupID).FirstOrDefault();
                }
            }
        }
        
        //▼==== #003
        //private ObservableCollection<ResourceType> _refResourceType;
        //public ObservableCollection<ResourceType> refResourceType
        //{
        //    get
        //    {
        //        return _refResourceType;
        //    }
        //    set
        //    {
        //        _refResourceType = value;
        //        NotifyOfPropertyChange(() => refResourceType);
        //    }
        //}
        //▲==== #003

        private ObservableCollection<Supplier> _refSuplier;
        public ObservableCollection<Supplier> refSuplier
        {
            get
            {
                return _refSuplier;
            }
            set
            {
                _refSuplier = value;
                NotifyOfPropertyChange(() => refSuplier);
            }
        }
        
        private bool _eResourceType;
        public bool eResourceType
        {
            get
            {
                return _eResourceType;
            }
            set
            {
                _eResourceType = value;
                NotifyOfPropertyChange(() => eResourceType);
            }
        }

        private ResourceGroup _selectedResourceGroup;
        public ResourceGroup selectedResourceGroup
        {
            get
            {
                return _selectedResourceGroup;
            }
            set
            {
                if (_selectedResourceGroup == value)
                    return;
                _selectedResourceGroup = value;
                NotifyOfPropertyChange(() => selectedResourceGroup);

                if (selectedResourceGroup != null
                    && selectedResourceGroup.RscrGroupID > 0)
                {
                    //▼==== #003
                    //GetAllResourceTypeByGroupID(selectedResourceGroup.RscrGroupID);

                    eResourceType = false;

                    if (selectedResourceGroup != null && selectedResourceGroup.GroupName != null)
                    {
                        if (selectedResourceGroup.GroupName.Contains("Máy Thủ Thuật"))
                        {
                            eResourceType = true;
                            GetMedicalServiceTypes_ByResourceGroup(1);
                        }
                        else if (selectedResourceGroup.GroupName.Contains("Máy Phẫu Thuật"))
                        {
                            eResourceType = true;
                            GetMedicalServiceTypes_ByResourceGroup(2);
                        }
                        else
                        {
                            eResourceType = false;
                            GetMedicalServiceTypes_ByResourceGroup(0);
                        }
                    }
                    //▲==== #003
                    curResource.VResourceGroup = selectedResourceGroup;
                }

            }
        }

        private ObservableCollection<Lookup> _ExpenditureSourceCollection;
        public ObservableCollection<Lookup> ExpenditureSourceCollection
        {
            get
            {
                return _ExpenditureSourceCollection;
            }
            set
            {
                if (_ExpenditureSourceCollection == value)
                    return;
                _ExpenditureSourceCollection = value;
                NotifyOfPropertyChange(() => ExpenditureSourceCollection);
            }
        }

        private Lookup _SelectedExpenditureSource;
        public Lookup SelectedExpenditureSource
        {
            get
            {
                return _SelectedExpenditureSource;
            }
            set
            {
                if (_SelectedExpenditureSource == value)
                    return;
                _SelectedExpenditureSource = value;

                if (_SelectedExpenditureSource.LookupID > 0)
                {
                    curResource.V_ExpenditureSource = _SelectedExpenditureSource.LookupID;
                }
                NotifyOfPropertyChange(() => SelectedExpenditureSource);
            }
        }
        #endregion
        #region Method
        public bool CanDelete
        {
            get
            {
                if (!Globals.isAccountCheck)
                {
                    return true;
                }
                else
                {
                    return Globals.CheckAuthorization(Globals.listRefModule, (int) eModules.mResources
                                                      , (int) eResources.mPtDashboardResource
                                                      , (int) oResourcesEx.mResourceList
                                                      , (int) ePermission.mAdd);
                }
            }
        }
        public bool CanUpdate
        {
            get
            {
                if (!Globals.isAccountCheck)
                {
                    return true;
                }
                else
                {
                    return Globals.CheckAuthorization(Globals.listRefModule, (int) eModules.mResources
                                                      , (int) eResources.mPtDashboardResource
                                                      , (int) oResourcesEx.mResourceList
                                                      , (int) ePermission.mAdd);
                }
            }
        }
        public void Delete()
        {
            DeleteResources(curResource.RscrID);
            this.TryClose();
        }
        
        public bool CheckValid(object temp)
        {
            DataEntities.Resources p = temp as DataEntities.Resources;
            if (p == null)
            {
                return false;
            }
            return p.Validate();
        }  


        //public void GetAllResourceTypeByGroupID(long GroupID)
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Load Resource Group!" });

        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new ResourcesManagementServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginGetAllResourceTypeByGID(GroupID, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var ResourceType = contract.EndGetAllResourceTypeByGID(asyncResult);

        //                    if (ResourceType != null)
        //                    {
        //                        refResourceType.Clear();
        //                        foreach (ResourceType rt in ResourceType)
        //                        {
        //                            refResourceType.Add(rt);
        //                        }
        //                    }

        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }
        //            }), null);
        //        }


        //    });
        //    t.Start();
        //}

        //▼==== #003
        public void butEdit()
        {
            butSave();
        }

        public void butSave()
        {
            //check validate
            if (!CheckValid(curResource))
            {
                return;
            }
            if (string.IsNullOrEmpty(curResource.HisItemName))
            {
                MessageBox.Show(String.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.Z3313_G1_TenThietBiBV));
                return;
            }
            if (string.IsNullOrEmpty(curResource.ItemName))
            {
                MessageBox.Show(String.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.Z3311_G1_TenThietBiBH));
                return;
            }
            if (string.IsNullOrEmpty(curResource.ResourceCode))
            {
                MessageBox.Show(String.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.Z3309_G1_Model));
                return;
            }
            if (curResource.V_PCLMainCategory == null || curResource.V_PCLMainCategory == 0)
            {
                MessageBox.Show(String.Format(eHCMSResources.Z0579_G1_VuiLongChon, "Danh mục chính"));
                return;
            }
            if (curResource.VSupplier == null)
            {
                //nho thay doi thanh global.showmessage
                MessageBoxResult mB = MessageBox.Show(eHCMSResources.A0397_G1_Msg_WarnChuaChonNCC, "Warning", MessageBoxButton.OKCancel);
                if (mB == MessageBoxResult.OK)
                {
                    curResource.VSupplier = new Supplier();
                }
                else
                    return;
            }
            if (isEdit)
            {
                curResource.LastUpdateStaffID = Globals.LoggedUserAccount.StaffID.Value;
                UpdateResources(curResource, MServiceTypeListIDStr);
            }
            else
            {
                curResource.CreatedStaffID = Globals.LoggedUserAccount.StaffID.Value;
                AddcurResources(curResource, MServiceTypeListIDStr);
            }
        }

        private void AddcurResources(Resources resource, string MSTListIDStr)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách RoomType" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddNewResources(resource, MSTListIDStr, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndAddNewResources(asyncResult);
                            if (items == true)
                            {
                                //▼=====#001
                                //Globals.ShowMessage(eHCMSResources.Z1740_G1_ThemMoiVTuThCong, "");
                                MessageBox.Show(eHCMSResources.Z1740_G1_ThemMoiVTuThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                curResource = new Resources();
                                auHangSX.Text = null;
                                Auto_PharmaCountry.Text = null;
                                selectedResourceGroup = null;
                                if (ExpenditureSourceCollection.Count > 0)
                                {
                                    SelectedExpenditureSource = ExpenditureSourceCollection[0];
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z1741_G1_ThemMoiVTuBiLoi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                //Globals.ShowMessage(eHCMSResources.Z1741_G1_ThemMoiVTuBiLoi, "");
                                //▲=====#001
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
                    }), null);
                }


            });
            t.Start();
        }
        //▲==== #003

        public void UpdateResources(Resources resource, string MSTListIDStr)
        { 
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Cập Nhật!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginUpdateResources(resource, MSTListIDStr, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ResVal = contract.EndUpdateResources(asyncResult);
                            if (ResVal == true)
                            {
                                //▼=====#001
                                Globals.EventAggregator.Publish(new ResourceEditEvent() { });
                                MessageBox.Show(eHCMSResources.Z1737_G1_ChSuaVTuThCong,eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                //Globals.ShowMessage(eHCMSResources.Z1737_G1_ChSuaVTuThCong, "");
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z1738_G1_ChSuaVTuBiLoi,eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                //Globals.ShowMessage(eHCMSResources.Z1738_G1_ChSuaVTuBiLoi, "");
                                //▲=====#001
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
                    }), null);
                }


            });
            t.Start();
        }
        public void DeleteResources(long RscrID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0343_G1_DangLuu) });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDeleteResources(RscrID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ResVal = contract.EndDeleteResources(asyncResult);
                            if (ResVal == true)
                            {
                                //▼=====#001
                                Globals.EventAggregator.Publish(new ResourceEditEvent() { });
                                MessageBox.Show(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                //Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, "");
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z1739_G1_XoaVTuBiLoi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                //Globals.ShowMessage(eHCMSResources.Z1739_G1_XoaVTuBiLoi, "");
                                //▲=====#001
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
                    }), null);
                }


            });
            t.Start();
        }
        public void LoadExpenditureSources()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.V_ExpenditureSource, Globals.DispatchCallback((asyncResult) =>
                        {
                            ExpenditureSourceCollection = new ObservableCollection<Lookup>(contract.EndGetAllLookupValuesByType(asyncResult));
                            if (isEdit)
                            {
                                if (curResource.V_ExpenditureSource > 0)
                                {
                                    SelectedExpenditureSource = ExpenditureSourceCollection.Where(x => x.LookupID == curResource.V_ExpenditureSource).FirstOrDefault();
                                }
                                else
                                {
                                    SelectedExpenditureSource = ExpenditureSourceCollection[0];
                                }
                            }
                            else
                            {
                                if (ExpenditureSourceCollection.Count > 0)
                                    SelectedExpenditureSource = ExpenditureSourceCollection[0];
                            }
                        }), null);
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
            });
            t.Start();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            LoadExpenditureSources();
        }
        #endregion

        #region "LostFocus"
        public void LostFocus_txtName(object txtName)
        {
            curResource.ItemName = txtName.ToString();
        }
        public void LostFocus_txtPrice(object txtPrice)
        {
            decimal BuyPrice=0;
            decimal.TryParse(txtPrice.ToString(),out BuyPrice);
            curResource.BuyPrice = BuyPrice;
        }
        #endregion

        //▼==== #003
        private ObservableCollection<PharmaceuticalCompany> _AllPharmaceuticalCompanies;
        public ObservableCollection<PharmaceuticalCompany> AllPharmaceuticalCompanies
        {
            get { return _AllPharmaceuticalCompanies; }
            set
            {
                if (_AllPharmaceuticalCompanies != value)
                    _AllPharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => AllPharmaceuticalCompanies);
            }
        }

        private ObservableCollection<PharmaceuticalCompany> _pharmaceuticalCompanies;
        public ObservableCollection<PharmaceuticalCompany> PharmaceuticalCompanies
        {
            get { return _pharmaceuticalCompanies; }
            set
            {
                if (_pharmaceuticalCompanies != value)
                    _pharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => PharmaceuticalCompanies);
            }
        }

        AutoCompleteBox auHangSX;
        public void HangSX_Loaded(object sender, RoutedEventArgs e)
        {
            auHangSX = sender as AutoCompleteBox;
        }
        public void HangSX_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllPharmaceuticalCompanies == null || AllPharmaceuticalCompanies.Count <= 0)
            {
                return;
            }
            //20180713 TBL: Them kiem tra e.Parameter neu khong se bao loi 
            if (e.Parameter == null)
            {
                return;
            }
            PharmaceuticalCompanies = new ObservableCollection<PharmaceuticalCompany>();
            foreach (var item in AllPharmaceuticalCompanies)
            {
                string str = VNConvertString.ConvertString(item.PCOName);
                str = str.ToLower();
                bool bres = str.Contains(VNConvertString.ConvertString(e.Parameter).ToLower());
                if (bres)
                {
                    PharmaceuticalCompanies.Add(item);
                }
            }
            auHangSX.ItemsSource = PharmaceuticalCompanies;
            auHangSX.PopulateComplete();
        }
        public void HangSX_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (curResource != null)
            {
                if (auHangSX != null && auHangSX.SelectedItem != null)
                {
                    PharmaceuticalCompany item = auHangSX.SelectedItem as PharmaceuticalCompany;
                    if (item != null && item.PCOID > 0)
                    {
                        curResource.Manufacturer = item.PCOID;
                    }
                    else
                    {
                        curResource.Manufacturer = 0;
                    }
                }
                else
                {
                    curResource.Manufacturer = 0;
                }
            }
        }

        private ObservableCollection<RefCountry> _AllCountries;
        public ObservableCollection<RefCountry> AllCountries
        {
            get
            {
                return _AllCountries;
            }
            set
            {
                if (_AllCountries != value)
                {
                    _AllCountries = value;
                    NotifyOfPropertyChange(() => AllCountries);
                }
            }
        }

        private ObservableCollection<RefCountry> _countries;
        public ObservableCollection<RefCountry> Countries
        {
            get
            {
                return _countries;
            }
            set
            {
                if (_countries != value)
                {
                    _countries = value;
                    NotifyOfPropertyChange(() => Countries);
                }
            }
        }

        AutoCompleteBox Auto_PharmaCountry;

        public void Auto_PharmaCountry_Loaded(object sender, RoutedEventArgs e)
        {
            Auto_PharmaCountry = sender as AutoCompleteBox;
        }

        public void Auto_PharmaCountry_Populating(object sender, PopulatingEventArgs e)
        {
            if (AllCountries == null || AllCountries.Count <= 0)
            {
                return;
            }
            //20180713 TBL: Them kiem tra e.Parameter neu khong se bao loi 
            if (e.Parameter == null)
            {
                return;
            }

            Countries = new ObservableCollection<RefCountry>();
            foreach (var x in AllCountries)
            {
                string str = VNConvertString.ConvertString(x.CountryName);
                str = str.ToLower();
                bool bres = str.Contains(VNConvertString.ConvertString(e.Parameter).ToLower());
                if (bres)
                {
                    Countries.Add(x);
                }
            }
            Auto_PharmaCountry.ItemsSource = Countries;
            Auto_PharmaCountry.PopulateComplete();
        }
        public void Country_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axCountry = sender as AutoCompleteBox;
            if (curResource != null)
            {
                if (axCountry != null && axCountry.SelectedItem != null)
                {
                    RefCountry item = axCountry.SelectedItem as RefCountry;
                    if (item != null && item.CountryID > 0)
                    {
                        curResource.ManufactureCountry = item.CountryID;
                    }
                    else
                    {
                        curResource.ManufactureCountry = 0;
                    }
                }
                else
                {
                    curResource.ManufactureCountry = 0;
                }
            }

        }
        
        private ObservableCollection<RefDepartment> _RefDepartmentCollection;
        public ObservableCollection<RefDepartment> RefDepartmentCollection
        {
            get => _RefDepartmentCollection; set
            {
                _RefDepartmentCollection = value;
                NotifyOfPropertyChange(() => RefDepartmentCollection);
            }
        }

        private IEnumerator<IResult> LoadDepartments()
        {
            ObservableCollection<RefDepartment> tempDepartments = new ObservableCollection<RefDepartment>();
            var departmentTask = new LoadDepartmentsByV_DeptTypeOperationTask(new List<long> { (long)V_DeptTypeOperation.KhoaNgoaiTru, (long)V_DeptTypeOperation.KhoaNoi });
            yield return departmentTask;
            RefDepartmentCollection = departmentTask.Departments.Where(x => x.V_DeptTypeOperation != (long)V_DeptTypeOperation.KhoaNgoaiTru || x.DeptID == Globals.ServerConfigSection.Hospitals.KhoaPhongKham).ToObservableCollection();
            if (RefDepartmentCollection == null) RefDepartmentCollection = new ObservableCollection<RefDepartment>();
            RefDepartmentCollection.Insert(0, new RefDepartment { DeptID = 0, DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1997_G1_ChonKhoa) });
            curResource.UseForDeptID = RefDepartmentCollection.FirstOrDefault().DeptID;
            yield break;
        }


        private ObservableCollection<Lookup> _RscrStatus;
        public ObservableCollection<Lookup> RscrStatus
        {
            get
            {
                return _RscrStatus;
            }
            set
            {
                if (_RscrStatus == value)
                    return;
                _RscrStatus = value;
                NotifyOfPropertyChange(() => RscrStatus);
            }
        }
        private void LoadRscrStatus_Level()
        {
            ObservableCollection<Lookup> tmpLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_RscrStatus).ToObservableCollection();

            if (tmpLookupList == null || tmpLookupList.Count <= 0)
            {
                MessageBox.Show("Không tìm thấy " + eHCMSResources.G1348_G1_TTrangTBi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            RscrStatus = tmpLookupList;

            if (curResource != null && (curResource.V_RscrStatus == null || curResource.V_RscrStatus.LookupID == 0))
            {
                curResource.V_RscrStatus = RscrStatus.FirstOrDefault();
            }
        }
        private IEnumerator<IResult> GetAllRefInfo()
        {
            if (AllCountries == null)
            {
                var loadTask6 = new LoadCountryListTask(false, false);
                yield return loadTask6;
                AllCountries = loadTask6.RefCountryList;
                LoadSelectedCountry();
            }

            if (AllPharmaceuticalCompanies == null)
            {
                var loadTask7 = new LoadPharmaceuticalCompanyListTask(false, false);
                yield return loadTask7;
                AllPharmaceuticalCompanies = loadTask7.PharmaceuticalCompanyList;
                LoadPharmaceuticalCompany();
            }
        }

        public void editResTypeList()
        {
            if (curResource.VResourceGroup == null)
            {
                MessageBox.Show(eHCMSResources.A0099_G1_Msg_InfoChuaChonNhomVatTu);
                //Globals.ShowMessage("Bạn chưa chọn loại vật tư!",eHCMSResources.G0442_G1_TBao);
                return;
            }
            Action<IResTypeListSelector> onInitDlg = (newType) =>
            {
                newType.refResourceTypes = refResourceType;
                newType.RscrTypeListsOld = curRscrTypeLists;
            };
            GlobalsNAV.ShowDialog<IResTypeListSelector>(onInitDlg);
        }

        public void Handle(OnChangedSelectionResourceTypes message)
        {
            if (GetView() != null)
            {
                selectedResourceTypes = message.selectedResourceTypes;
                MServiceTypeListIDStr = message.MServiceTypeListIDStr;
            }
        }
        
        private ObservableCollection<RefMedicalServiceType> _refResourceType;
        public ObservableCollection<RefMedicalServiceType> refResourceType
        {
            get
            {
                return _refResourceType;
            }
            set
            {
                _refResourceType = value;
                NotifyOfPropertyChange(() => refResourceType);
            }
        }

        private ObservableCollection<ResourceTypeLists> _curRscrTypeLists;
        public ObservableCollection<ResourceTypeLists> curRscrTypeLists
        {
            get
            {
                return _curRscrTypeLists;
            }
            set
            {
                if (_curRscrTypeLists == value)
                    return;
                _curRscrTypeLists = value;
                NotifyOfPropertyChange(() => curRscrTypeLists);
            }
        }

        private ObservableCollection<RefMedicalServiceType> _selectedResourceTypes;
        public ObservableCollection<RefMedicalServiceType> selectedResourceTypes
        {
            get
            {
                return _selectedResourceTypes;
            }
            set
            {
                if (_selectedResourceTypes == value)
                    return;
                _selectedResourceTypes = value;

                curRscrTypeLists = new ObservableCollection<ResourceTypeLists>();
                foreach (var item in _selectedResourceTypes)
                {
                    if (item != null && item.MedicalServiceTypeID > 0)
                    {
                        curRscrTypeLists.Add(new ResourceTypeLists() { MedicalServiceTypeID = item.MedicalServiceTypeID });
                    }
                }

                NotifyOfPropertyChange(() => selectedResourceTypes);
            }
        }

        private List<long> _MServiceTypeListID;
        public List<long> MServiceTypeListID
        {
            get
            {
                return _MServiceTypeListID;
            }
            set
            {
                if (_MServiceTypeListID == value)
                    return;
                _MServiceTypeListID = value;
                NotifyOfPropertyChange(() => MServiceTypeListID);
            }
        }

        private string _MServiceTypeListIDStr;
        public string MServiceTypeListIDStr
        {
            get
            {
                return _MServiceTypeListIDStr;
            }
            set
            {
                if (_MServiceTypeListIDStr == value)
                    return;
                _MServiceTypeListIDStr = value;
                NotifyOfPropertyChange(() => MServiceTypeListIDStr);
            }
        }

        public void GetMedicalServiceTypes_ByResourceGroup(long GroupID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetMedicalServiceTypes_ByResourceGroup(GroupID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ResourceType = contract.EndGetMedicalServiceTypes_ByResourceGroup(asyncResult);

                            if (ResourceType != null)
                            {
                                refResourceType.Clear();
                                foreach (RefMedicalServiceType rt in ResourceType)
                                {
                                    refResourceType.Add(rt);
                                }
                            }
                            LoadSelectedResourceTypes();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void LoadSelectedResourceTypes()
        {
            selectedResourceTypes = new ObservableCollection<RefMedicalServiceType>();
            MServiceTypeListID = new List<long>();

            if (curResource.RscrTypeLists != null && refResourceType != null && refResourceType.Count() > 0)
            {
                try
                {
                    foreach (var item in curResource.RscrTypeLists)
                    {
                        var findResourceType = refResourceType.Where(x => x.MedicalServiceTypeID == item.MedicalServiceTypeID).ToList().FirstOrDefault();
                        if (findResourceType != null)
                        {
                            selectedResourceTypes.Add(findResourceType);
                        }
                        MServiceTypeListID.Add(item.MedicalServiceTypeID);
                    }
                    MServiceTypeListIDStr = String.Join(";", MServiceTypeListID);
                    curRscrTypeLists = curResource.RscrTypeLists;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
        }

        
        private PharmaceuticalCompany _PharmaceuticalCompany;
        public PharmaceuticalCompany PharmaceuticalCompany
        {
            get
            {
                return _PharmaceuticalCompany;
            }
            set
            {
                if (_PharmaceuticalCompany == value)
                    return;
                _PharmaceuticalCompany = value;

                if (_PharmaceuticalCompany != null && _PharmaceuticalCompany.PCOID > 0)
                {
                    curResource.Manufacturer = _PharmaceuticalCompany.PCOID;
                }
                NotifyOfPropertyChange(() => PharmaceuticalCompany);
            }
        }

        private RefCountry _SelectedCountry;
        public RefCountry SelectedCountry
        {
            get
            {
                return _SelectedCountry;
            }
            set
            {
                if (_SelectedCountry == value)
                    return;
                _SelectedCountry = value;

                if (_SelectedCountry != null && _SelectedCountry.CountryID > 0)
                {
                    curResource.ManufactureCountry = _SelectedCountry.CountryID;
                }
                NotifyOfPropertyChange(() => SelectedCountry);
            }
        }

        public void LoadPharmaceuticalCompany()
        {
            this.ShowBusyIndicator();
            try
            {
                if (isEdit)
                {
                    if (curResource.Manufacturer > 0)
                        PharmaceuticalCompany = AllPharmaceuticalCompanies.Where(x => x.PCOID == curResource.Manufacturer).FirstOrDefault();
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
        }

        public void LoadSelectedCountry()
        {
            this.ShowBusyIndicator();
            try
            {
                if (isEdit)
                {
                    if (curResource.ManufactureCountry > 0)
                        SelectedCountry = AllCountries.Where(x => x.CountryID == curResource.ManufactureCountry).FirstOrDefault();
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
        }
        //▲==== #003


        //▼==== #004
        private ObservableCollection<Lookup> _V_PCLMainCategory;
        public ObservableCollection<Lookup> V_PCLMainCategory
        {
            get
            {
                return _V_PCLMainCategory;
            }
            set
            {
                if (_V_PCLMainCategory == value)
                    return;
                _V_PCLMainCategory = value;
                NotifyOfPropertyChange(() => V_PCLMainCategory);
            }
        }

        private void LoadV_PCLMainCategory_Level()
        {
            ObservableCollection<Lookup> tmpLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_PCLMainCategory).ToObservableCollection();

            if (tmpLookupList == null || tmpLookupList.Count <= 0)
            {
                MessageBox.Show("Không tìm thấy danh mục chính", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            V_PCLMainCategory = tmpLookupList;

            //if (curResource != null && (curResource.V_PCLMainCategory == null || curResource.V_PCLMainCategory == 0))
            //{
            //    curResource.V_PCLMainCategory = V_PCLMainCategory.FirstOrDefault().LookupID;
            //}
        }
        //▲==== #004
    }
}
