using System.ComponentModel.Composition;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using aEMR.ViewContracts;
/*
 * #001 20180921 TNHX: Refactor code
 */
namespace aEMR.Configuration.RefMedicalServiceItems.ViewModels
{
    [Export(typeof(IDeptRefMedicalServiceItemsContent)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeptRefMedicalServiceItemsContentViewModel : Conductor<object>, IDeptRefMedicalServiceItemsContent
    {
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

        private object _UCRefMedicalServiceItems;
        public object UCRefMedicalServiceItems
        {
            get
            {
                return _UCRefMedicalServiceItems;
            }
            set
            {
                if (_UCRefMedicalServiceItems == value)
                    return;
                _UCRefMedicalServiceItems = value;
                NotifyOfPropertyChange(() => UCRefMedicalServiceItems);
            }
        }

        private object _UCDeptRefMedicalServiceItems;
        public object UCDeptRefMedicalServiceItems
        {
            get
            {
                return _UCDeptRefMedicalServiceItems;
            }
            set
            {
                if (_UCDeptRefMedicalServiceItems == value)
                    return;
                _UCDeptRefMedicalServiceItems = value;
                NotifyOfPropertyChange(() => UCDeptRefMedicalServiceItems);
            }
        }

        [ImportingConstructor]
        public DeptRefMedicalServiceItemsContentViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            Globals.EventAggregator.Subscribe(this);
            UCDeptRefMedicalServiceItems = Globals.GetViewModel<IDeptRefMedicalServiceItems>();

            UCRefMedicalServiceItems = Globals.GetViewModel<IRefMedicalServiceItems>();

        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplEditService = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLDichVu_DonGiaCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDichVuDonGia, (int)ePermission.mEdit);
            bhplDeleteService = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLDichVu_DonGiaCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDichVuDonGia, (int)ePermission.mDelete);
            bhplListPrice = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLDichVu_DonGiaCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDichVuDonGia, (int)ePermission.mView);
            bhplAddNewRefMedicalServiceItemsView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLDichVu_DonGiaCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDichVuDonGia, (int)ePermission.mAdd);
        }

        #region checking account
        private bool _bhplEditService = true;
        private bool _bhplDeleteService = true;
        private bool _bhplListPrice = true;
        private bool _bhplAddNewRefMedicalServiceItemsView = true;
        public bool bhplEditService
        {
            get
            {
                return _bhplEditService;
            }
            set
            {
                if (_bhplEditService == value)
                    return;
                _bhplEditService = value;
            }
        }

        public bool bhplDeleteService
        {
            get
            {
                return _bhplDeleteService;
            }
            set
            {
                if (_bhplDeleteService == value)
                    return;
                _bhplDeleteService = value;
            }
        }

        public bool bhplListPrice
        {
            get
            {
                return _bhplListPrice;
            }
            set
            {
                if (_bhplListPrice == value)
                    return;
                _bhplListPrice = value;
            }
        }

        public bool bhplAddNewRefMedicalServiceItemsView
        {
            get
            {
                return _bhplAddNewRefMedicalServiceItemsView;
            }
            set
            {
                if (_bhplAddNewRefMedicalServiceItemsView == value)
                    return;
                _bhplAddNewRefMedicalServiceItemsView = value;
            }
        }
        #endregion

        #region binding visibilty
        public Button hplEditService { get; set; }
        public Button hplDeleteService { get; set; }
        public Button hplListPrice { get; set; }

        public void hplEditService_Loaded(object sender)
        {
            hplEditService = sender as Button;
            hplEditService.Visibility = Globals.convertVisibility(bhplEditService);
        }

        public void hplDeleteService_Loaded(object sender)
        {
            hplDeleteService = sender as Button;
            hplDeleteService.Visibility = Globals.convertVisibility(bhplDeleteService);
        }

        public void hplListPrice_Loaded(object sender)
        {
            hplListPrice = sender as Button;
            hplListPrice.Visibility = Globals.convertVisibility(bhplListPrice);
        }
        #endregion        
    }
}
