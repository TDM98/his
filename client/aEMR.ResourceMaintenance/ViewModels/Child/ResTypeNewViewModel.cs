/*
 * 20182905 TTM #001: Fix bug hien thong bao ben duoi popup.
 */
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
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

namespace aEMR.ResourceMaintenance.ViewModels
{
    [Export(typeof(IResTypeNew)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResTypeNewViewModel : Conductor<object>, IResTypeNew
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ResTypeNewViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
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
                if (_refResourceGroup == value)
                    return;
                _refResourceGroup = value;
                NotifyOfPropertyChange(() => refResourceGroup);
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
            }
        }

        private string _ResourceTypeName;
        public string ResourceTypeName
        {
            get
            {
                return _ResourceTypeName;
            }
            set
            {
                if (_ResourceTypeName == value)
                    return;
                _ResourceTypeName = value;
                NotifyOfPropertyChange(() => ResourceTypeName);
            }
        }

        private string _ResourceTypeDecript;
        public string ResourceTypeDecript
        {
            get
            {
                return _ResourceTypeDecript;
            }
            set
            {
                if (_ResourceTypeDecript == value)
                    return;
                _ResourceTypeDecript = value;
                NotifyOfPropertyChange(() => ResourceTypeDecript);
            }
        }

        public void CancelButton()
        {
            TryClose();
        }
        public void OKButton()
        {
            if (selectedResourceGroup!=null
                || ResourceTypeName!="")
            {
                AddNewResourceType(selectedResourceGroup.RscrGroupID,ResourceTypeName,ResourceTypeDecript);
            }else
            {
                //▼=====#001
                MessageBox.Show("Chưa chọn Resource Group hoặc nhập vào Resource Type name!");
                //Globals.ShowMessage("Chua chon resource group hoac nhap vao resource type name!", "");
                return;
                //▲=====#001
            }

            TryClose();
        }

        
        public void AddNewResourceType(long GroupID,string typeName,string description)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Dang Load resource group!" });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddNewResourceType(GroupID, typeName, description, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ResVal = contract.EndAddNewResourceType(asyncResult);
                            if (ResVal == true)
                            {
                                //▼=====#001
                                //Globals.ShowMessage("Them loai vat tu thanh cong!", "");
                                MessageBox.Show(eHCMSResources.Z1740_G1_ThemMoiVTuThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            else
                            {
                                //Globals.ShowMessage("Them loai vat tu bi loi!", "");
                                MessageBox.Show(eHCMSResources.Z1740_G1_ThemMoiVTuThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
    }
}
