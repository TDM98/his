/*
 * 29052018 TTM #001: Người dùng lưu vật tư thành công thay vì thông báo lưu thành công ở popup, lại hiện thông báo ở màn hình chứa popup => sửa lại cho hiện thông báo đúng vị trí.
 * 29052018 TTM #002: Thêm parameter HIRepResourceCode.
 */
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System.Collections.ObjectModel;
using eHCMSLanguage;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IResourcesNew)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourcesNewViewModel : Conductor<object>, IResourcesNew
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ResourcesNewViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;


            _newResource =new Resources();
            _refResourceType=new ObservableCollection<ResourceType>();
            //(int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE;
        }

        public void Exit()
        {
            Globals.EventAggregator.Publish(new ResourceEvent() {Resource = null});
            TryClose();
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
            //var newGroup = Globals.GetViewModel<IResGroupNew>();
            //newGroup.ResourceCategoryEnum = ResourceCategoryEnum;
            //var instance = newGroup as Conductor<object>;


            //Globals.ShowDialog(instance,(o)=> { });

            Action<IResGroupNew> onInitDlg = (newGroup) =>
            {
                newGroup.ResourceCategoryEnum = ResourceCategoryEnum;
            };
            GlobalsNAV.ShowDialog<IResGroupNew>(onInitDlg);
        }
        public void newResType()
        {
            //var newType = Globals.GetViewModel<IResTypeNew>();
            //newType.refResourceGroup = refResourceGroup;
            //var instance = newType as Conductor<object>;

            //Globals.ShowDialog(instance,(o)=> { });

            Action<IResTypeNew> onInitDlg = (newType) =>
            {
                newType.refResourceGroup = refResourceGroup;
            };
            GlobalsNAV.ShowDialog<IResTypeNew>(onInitDlg);

        }
        public void newSuplier()
        {
            
        }
        
        #region Properties
        private Resources _newResource;
        public Resources newResource
        {
            get
            {
                return _newResource;
            }
            set
            {
                if (_newResource == value)
                    return;
                _newResource = value;
                NotifyOfPropertyChange(() => newResource);
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
            }
        }

        private ObservableCollection<ResourceType> _refResourceType;
        public ObservableCollection<ResourceType> refResourceType
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
                    GetAllResourceTypeByGroupID(selectedResourceGroup.RscrGroupID);
                    newResource.VResourceGroup = selectedResourceGroup;
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
                NotifyOfPropertyChange(() => SelectedExpenditureSource);
            }
        }
        #endregion

        #region Method

        public bool CheckValid(object temp)
        {
            Resources u = temp as Resources;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }
        public void butSave()
        {
            //check validate
            if (!CheckValid(newResource))
            {
                return;
            }
            if (newResource.VResourceGroup== null)
            {
                MessageBox.Show(eHCMSResources.A0099_G1_Msg_InfoChuaChonNhomVatTu);
                //Globals.ShowMessage("Bạn chưa chọn loại vật tư!",eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (newResource.VResourceType == null)
            {
                MessageBox.Show(eHCMSResources.A0097_G1_Msg_InfoChuaChonLoaiVatTu);
                //Globals.ShowMessage("Bạn chưa chọn loại vật tư!",eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (newResource.VSupplier == null)
            {
                //nho thay doi thanh global.showmessage
                MessageBoxResult mB = MessageBox.Show(eHCMSResources.A0397_G1_Msg_WarnChuaChonNCC, "Warning", MessageBoxButton.OKCancel);
                if (mB == MessageBoxResult.OK)
                {
                    newResource.VSupplier = new Supplier();
                }
                else
                    return;
            }
            AddNewResources(newResource);
        }

        private void AddNewResources(Resources resource)
        {
            
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách RoomType" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddNewResources(resource, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndAddNewResources(asyncResult);
                            if (items == true)
                            {
                                //▼=====#001
                                //Globals.ShowMessage(eHCMSResources.Z1740_G1_ThemMoiVTuThCong, "");
                                MessageBox.Show(eHCMSResources.Z1740_G1_ThemMoiVTuThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                newResource = new Resources();
                                selectedResourceGroup = null;
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

        public void GetAllResourceTypeByGroupID(long GroupID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllResourceTypeByGID(GroupID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ResourceType = contract.EndGetAllResourceTypeByGID(asyncResult);

                            if (ResourceType != null)
                            {
                                refResourceType.Clear();
                                foreach (ResourceType rt in ResourceType)
                                {
                                    refResourceType.Add(rt);
                                }
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
                            if (ExpenditureSourceCollection.Count > 0)
                                SelectedExpenditureSource = ExpenditureSourceCollection[0];
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
    }
}
