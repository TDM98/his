using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using DataEntities;
using System.Collections.Generic;
using aEMR.ReportModel.ReportModels;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DevExpress.ReportServer.Printing;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IReportByStorage)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportByStorageViewModel : Conductor<object>, IReportByStorage
    {
        public string TitleForm { get; set; }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ReportByStorageViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Authorization();

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
        }

        protected override void OnActivate()
        {
        }

        protected override void OnDeactivate(bool close)
        {
            //base.OnDeactivate(close);
            ReportModel = null;
            StoreCbx = null;

        }
        #region Properties Member

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }

        private RefStorageWarehouseLocation _SelectedStore;
        public RefStorageWarehouseLocation SelectedStore
        {
            get
            {
                return _SelectedStore;
            }
            set
            {
                if (_SelectedStore != value)
                {
                    _SelectedStore = value;
                    NotifyOfPropertyChange(() => SelectedStore);
                }
            }
        }

        private ReportName _eItem;
        public ReportName eItem
        {
            get
            {
                return _eItem;
            }
            set
            {
                _eItem = value;
                NotifyOfPropertyChange(() => eItem);
            }
        }

        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(() => ReportModel);
            }
        }

        #endregion
        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bXem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDuTru_ThuocCanLayThemDeBan,
                                               (int)oPharmacyEx.mDuTru_ThuocCanLayThemDeBan_Xem, (int)ePermission.mView);
            bIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDuTru_ThuocCanLayThemDeBan,
                                               (int)oPharmacyEx.mDuTru_ThuocCanLayThemDeBan_In, (int)ePermission.mView);
            bCombo = bXem || bIn;

        }
        #region checking account

        private bool _bXem = true;
        private bool _bIn = true;
        private bool _bCombo = true;

        public bool bXem
        {
            get
            {
                return _bXem;
            }
            set
            {
                if (_bXem == value)
                    return;
                _bXem = value;
            }
        }

        public bool bIn
        {
            get
            {
                return _bIn;
            }
            set
            {
                if (_bIn == value)
                    return;
                _bIn = value;
            }
        }

        public bool bCombo
        {
            get
            {
                return _bCombo;
            }
            set
            {
                if (_bCombo == value)
                    return;
                _bCombo = value;
            }
        }

        #endregion

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        private void GetReport()
        {
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            switch (_eItem)
            {
                case ReportName.PHARMACY_HESOANTOANBAN:
                    ReportModel = null;
                    ReportModel = new PharmacyHeSoAnToanBanReportModel().PreviewModel;
                    rParams["StoreID"].Value = (int)SelectedStore.StoreID;
                    rParams["StoreName"].Value = SelectedStore.swhlName;
                    rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
                    break;
            }

            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(rParams);
        }

        #region Print Member

        public void btnXemIn(object sender, EventArgs e)
        {
            if (GetParameters())
            {
                GetReport();
            }
        }

        public void btnIn(object sender, EventArgs e)
        {
        }

        #endregion

        ComboBox cbx_ChooseKho = null;
        public void cbx_ChooseKho_Loaded(object sender, RoutedEventArgs e)
        {
            cbx_ChooseKho = sender as ComboBox;
        }

        private bool GetParameters()
        {
            if (SelectedStore == null)
            {
                MessageBox.Show(eHCMSResources.K0310_G1_ChonKhoCanXem);
                return false;
            }
            return true;
        }
    }
}
