using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Controls;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
/*
 * 20190912 #001 TTM:   BM 0014330: Lỗi khi tạo mới giường bệnh trong màn hình quản lý giường bệnh.
 *                      => Lý do lỗi: Do trước giờ Viện Tim chỉ có 1 danh mục loại giường nên code đang gán cứng lấy cái đầu tiên nó tìm được mà Thanh Vũ có nhiều loại danh mục giường => Sai.
 *                      => Sửa lại lấy hết tất cả loại giường, tuỳ vào gán giường nào vào khoa mà query danh sách cho đúng.
 */
namespace aEMR.Configuration.BedAllocations.ViewModels
{
    [Export(typeof(IcwdBedAllocation)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class cwdBedAllocationViewModel : Conductor<object>, IcwdBedAllocation
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public cwdBedAllocationViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            _ObjGetDeptMedServiceItems = new ObservableCollection<DataEntities.MedServiceItemPrice>();
            _ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<RefMedicalServiceType>();
            _selectedBedAllocation = new BedAllocation();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            _SearchCriteria = new DeptMedServiceItemsSearchCriteria();
            //▼===== #001
            SearchCriteria.DeptID = SeletedRefDepartmentsTree.Parent.NodeID;
            GetBedAllocationAll_ByDeptID();
            //▲===== #001
        }

        private RefDepartmentsTree _SeletedRefDepartmentsTree;
        public RefDepartmentsTree SeletedRefDepartmentsTree
        {
            get
            {
                return _SeletedRefDepartmentsTree;
            }
            set
            {
                if (_SeletedRefDepartmentsTree == value)
                    return;
                _SeletedRefDepartmentsTree = value;
                NotifyOfPropertyChange(() => SeletedRefDepartmentsTree);
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

        private RefMedicalServiceType _selectedRefMedicalServiceTypes;
        public RefMedicalServiceType selectedRefMedicalServiceTypes
        {
            get
            {
                return _selectedRefMedicalServiceTypes;
            }
            set
            {
                if (_selectedRefMedicalServiceTypes == value)
                    return;
                _selectedRefMedicalServiceTypes = value;
                NotifyOfPropertyChange(() => selectedRefMedicalServiceTypes);
                SearchCriteria.DeptID = SeletedRefDepartmentsTree.Parent.NodeID;
                GetBedAllocationAll_ByDeptID();
            }
        }

        private object _cboMedicalServiceTypesSubTractPCL;
        public object cboMedicalServiceTypesSubTractPCL
        {
            get
            {
                return _cboMedicalServiceTypesSubTractPCL;
            }
            set
            {
                if (_cboMedicalServiceTypesSubTractPCL == value)
                    return;
                _cboMedicalServiceTypesSubTractPCL = value;
                NotifyOfPropertyChange(() => cboMedicalServiceTypesSubTractPCL);
            }
        }
        private bool _ChkRememberMe;
        public bool ChkRememberMe
        {
            get
            {
                return _ChkRememberMe;
            }
            set
            {
                if (_ChkRememberMe == value)
                    return;
                _ChkRememberMe = value;
                if (ChkRememberMe == true)
                    ((AxComboBox)cboMedicalServiceTypesSubTractPCL).IsEnabled = false;
                else
                    ((AxComboBox)cboMedicalServiceTypesSubTractPCL).IsEnabled = true;
            }
        }

        private DeptMedServiceItemsSearchCriteria _SearchCriteria;
        public DeptMedServiceItemsSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                if (_SearchCriteria == value)
                    return;
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);

            }
        }

        private ObservableCollection<DataEntities.MedServiceItemPrice> _ObjGetDeptMedServiceItems;
        public ObservableCollection<DataEntities.MedServiceItemPrice> ObjGetDeptMedServiceItems
        {
            get
            {
                return _ObjGetDeptMedServiceItems;
            }
            set
            {
                if (_ObjGetDeptMedServiceItems == value)
                    return;
                _ObjGetDeptMedServiceItems = value;
                NotifyOfPropertyChange(() => ObjGetDeptMedServiceItems);
            }
        }

        private DataEntities.MedServiceItemPrice _selectedGetDeptMedServiceItems;
        public DataEntities.MedServiceItemPrice selectedGetDeptMedServiceItems
        {
            get
            {
                return _selectedGetDeptMedServiceItems;
            }
            set
            {
                if (_selectedGetDeptMedServiceItems == value)
                    return;
                _selectedGetDeptMedServiceItems = value;
                NotifyOfPropertyChange(() => selectedGetDeptMedServiceItems);
            }
        }

        private BedAllocation _selectedBedAllocation;
        public BedAllocation selectedBedAllocation
        {
            get
            {
                return _selectedBedAllocation;
            }
            set
            {
                if (_selectedBedAllocation == value)
                    return;
                _selectedBedAllocation = value;
                NotifyOfPropertyChange(() => selectedBedAllocation);
            }
        }

        private ObservableCollection<BedAllocation> _allBedAllocation;
        public ObservableCollection<BedAllocation> allBedAllocation
        {
            get
            {
                return _allBedAllocation;
            }
            set
            {
                if (_allBedAllocation == value)
                    return;
                _allBedAllocation = value;
            }
        }
        public void butExit()
        {
            TryClose();
        }
        public void butSave()
        {
            if (selectedBedAllocation != null)
            {
                if (selectedBedAllocation.VBedLocType == null)
                {
                    selectedBedAllocation.VBedLocType = new Lookup();
                }
                selectedBedAllocation.BedNumber = "";
                selectedBedAllocation.IsActive = true;
                if (selectedGetDeptMedServiceItems == null)
                {
                    MessageBox.Show(eHCMSResources.A0096_G1_Msg_InfoChuaChonLoaiGiaGiuong);
                    return;
                }

                selectedBedAllocation.VRefMedicalServiceItem = selectedGetDeptMedServiceItems.ObjMedServiceID;
                selectedBedAllocation.VRefMedicalServiceItem.MedServiceName =
                    selectedGetDeptMedServiceItems.ObjDeptMedServiceItems.ObjRefMedicalServiceItem.MedServiceName;

                selectedBedAllocation.VRefMedicalServiceItem.NormalPrice = selectedGetDeptMedServiceItems.NormalPrice;
                selectedBedAllocation.DeptLocationID = SeletedRefDepartmentsTree.NodeID;
                selectedBedAllocation.BAGuid = Guid.NewGuid().ToString();
                //▼===== #001:  Bỏ GetViewModel ở chỗ này vì UCBedAllocGridView là 1 view NonShared nên khi GetView sẽ tạo 1 view mới không liên quan gì đến view cũ
                //              => Gán dữ liệu vào thì dữ liệu sẽ nằm ở View mới không ở View cũ. Chuyển sang sử dụng Publish Event chuyển thông tin về UCBedAllocGridView.
                ObservableCollection<BedAllocation> tmpBedAllocationsList = new ObservableCollection<BedAllocation>();
                for (int i = 0; i < selectedBedAllocation.BedQuantity; i++)
                {
                    BedAllocation bl = new DataEntities.BedAllocation();
                    selectedBedAllocation.Status = 1;

                    bl = ObjectCopier.DeepCopy(selectedBedAllocation);
                    tmpBedAllocationsList.Add(bl);
                }
                if (tmpBedAllocationsList != null && tmpBedAllocationsList.Count > 0)
                {
                    Globals.EventAggregator.Publish(new SetNewBedForUCBedAllocGridViewModel { BedAllocationsList = tmpBedAllocationsList });
                }
                //▲===== #001
            }
            TryClose();
        }
        public void cmbLoaded(object sender, RoutedEventArgs e)
        {
            cboMedicalServiceTypesSubTractPCL = sender as AxComboBox;
            if (ChkRememberMe == true)
                ((AxComboBox)cboMedicalServiceTypesSubTractPCL).IsEnabled = false;
        }


        public void GetAllMedicalServiceTypes()
        {

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllMedicalServiceTypes(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetAllMedicalServiceTypes(asyncResult);

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
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        //public void GetRefMedicalServiceTypes_ByV_RefMedicalServiceTypes()
        //{
        //    var t = new Thread(() =>
        //    {
        //        IsLoading = true;

        //        using (var serviceFactory = new ConfigurationManagerServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginRefMedicalServiceTypes_ByV_RefMedicalServiceTypes((long)AllLookupValues.V_RefMedicalServiceTypes.GIUONGBENH, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var items = contract.EndRefMedicalServiceTypes_ByV_RefMedicalServiceTypes(asyncResult);

        //                    if (items != null)
        //                    {
        //                        ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<DataEntities.RefMedicalServiceType>();
        //                        selectedRefMedicalServiceTypes = (RefMedicalServiceType)items[0];
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    IsLoading = false;
        //                }
        //            }), null);
        //        }


        //    });
        //    t.Start();
        //}

        private void GetDeptMedServiceItems_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetDeptMedServiceItems_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.MedServiceItemPrice> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetDeptMedServiceItems_Paging(out Total, asyncResult);
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

                            ObjGetDeptMedServiceItems.Clear();

                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    if (allItems.Count == 0)
                                    {
                                        MessageBox.Show(eHCMSResources.A0088_G1_Msg_InfoChuaC_HinhGiaGiuong);
                                        TryClose();
                                    }
                                    else
                                        foreach (var item in allItems)
                                        {
                                            ObjGetDeptMedServiceItems.Add(item);
                                        }
                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch
                {
                }
                finally
                {
                    IsLoading = false;
                }
            });
            t.Start();
        }


        private void GetBedAllocationAll_ByDeptID()
        {
            var t = new Thread(() =>
            {
                IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetBedAllocationAll_ByDeptID(SearchCriteria, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<DataEntities.MedServiceItemPrice> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetBedAllocationAll_ByDeptID(asyncResult);
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

                            ObjGetDeptMedServiceItems.Clear();

                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    if (allItems.Count == 0)
                                    {
                                        MessageBox.Show(eHCMSResources.A0088_G1_Msg_InfoChuaC_HinhGiaGiuong);
                                        TryClose();
                                    }
                                    else
                                    {
                                        foreach (var item in allItems)
                                        {
                                            ObjGetDeptMedServiceItems.Add(item);
                                        }
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch
                {
                }
                finally
                {
                    IsLoading = false;
                }
            });
            t.Start();
        }
    }
}
