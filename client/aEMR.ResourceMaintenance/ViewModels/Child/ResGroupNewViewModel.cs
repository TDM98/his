/*
 * 20182905 TTM #001: Fix bug hien thong bao ben duoi popup.
 */
using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using eHCMSLanguage;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IResGroupNew)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResGroupNewViewModel : Conductor<object>, IResGroupNew, IHandle<ResourceCategoryEnumEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ResGroupNewViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
#region property



        private string _GroupName;
        private string _Description;

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
                switch (_ResourceCategoryEnum)
                {
                    case (int)AllLookupValues.ResGroupCategory.THIET_BI_Y_TE:
                        //_VSupplierType = (int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE;
                        GroupCategoryTypeName = "Nhóm Thiết Bị Y Tế"; break;
                    case (int)AllLookupValues.ResGroupCategory.THIET_BI_VAN_PHONG:
                        //_VSupplierType = (int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_VAN_PHONG;
                        GroupCategoryTypeName = "Nhóm Thiết Bị Văn Phòng"; break;
                    case (int)AllLookupValues.ResGroupCategory.KHAC:
                        //_VSupplierType = (int)AllLookupValues.SupplierType.KHAC;
                        GroupCategoryTypeName = "Nhóm Khác"; break;
                }
            }
        }

        private string _GroupCategoryTypeName;
        public string GroupCategoryTypeName
        {
            get
            {
                return _GroupCategoryTypeName;
            }
            set
            {
                if (_GroupCategoryTypeName == value)
                    return;
                _GroupCategoryTypeName = value;
                NotifyOfPropertyChange(() => GroupCategoryTypeName);
            }
        }

        public string GroupName
        {
            get
            {
                return _GroupName;
            }
            set
            {
                if (_GroupName == value)
                    return;
                _GroupName = value;
                NotifyOfPropertyChange(() => GroupName);
            }
        }
        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                if (_Description == value)
                    return;
                _Description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }
#endregion

        public void Cancel()
        {
            TryClose();
        }
        public void Save()
        {
            if(GroupName=="")
            {
                //▼=====#001
                MessageBox.Show(eHCMSResources.Z1734_G1_ChuaNhapGroupName, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                //Globals.ShowMessage(eHCMSResources.Z1734_G1_ChuaNhapGroupName, "");
                //▲=====#001
                return;
            }else
            {
                AddNewResourceGroup(GroupName, Description, ResourceCategoryEnum);
            }
            TryClose();
        }



        public void AddNewResourceGroup(string GroupName, string Description, long V_ResGroupCategory)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddNewResourceGroup(GroupName, Description, V_ResGroupCategory, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ResVal = contract.EndAddNewResourceGroup(asyncResult);
                            if (ResVal == true)
                            {
                                //▼=====#001
                                MessageBox.Show(eHCMSResources.Z1735_G1_ThemNhomVTu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                //Globals.ShowMessage(eHCMSResources.Z1735_G1_ThemNhomVTu, "");
                                Globals.EventAggregator.Publish(new ResourceNewGroupEvent{isNewGroup = true});
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z1736_G1_ThemNhomVTuBiLoi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                //Globals.ShowMessage(eHCMSResources.Z1736_G1_ThemNhomVTuBiLoi, "");
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

        public  void Handle(ResourceCategoryEnumEvent obj)
        {
            if (obj!=null)
            {
                ResourceCategoryEnum = (long) obj.ResourceCategoryEnum;
            }
        }
    }
}
