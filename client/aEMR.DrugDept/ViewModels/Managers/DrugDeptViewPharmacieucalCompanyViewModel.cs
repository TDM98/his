using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using aEMR.ViewContracts;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using System.Collections.ObjectModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using System.Collections.Generic;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptViewPharmacieucalCompany)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptViewPharmacieucalCompanyViewModel : Conductor<object>, IDrugDeptViewPharmacieucalCompany
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DrugDeptViewPharmacieucalCompanyViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            RefGenMedProductDetailss = new PagedSortableCollectionView<RefGenMedProductDetails>();
            RefGenMedProductDetailss.PageSize = Globals.PageSize;
            RefGenMedProductDetailss.OnRefresh += new EventHandler<RefreshEventArgs>(RefGenMedProductDetailss_OnRefresh);

            Coroutine.BeginExecute(DoGetMedProductType());
        }

        void RefGenMedProductDetailss_OnRefresh(object sender, RefreshEventArgs e)
        {
            RefGenMedProductDetails_ByPCOID(RefGenMedProductDetailss.PageIndex, RefGenMedProductDetailss.PageSize);
        }


        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        #region checking account
        #region check invisible
        private bool _mTim = true;
        private bool _mThemMoi = true;
        private bool _mChinhSua = true;

        public bool mTim
        {
            get
            {
                return _mTim;
            }
            set
            {
                if (_mTim == value)
                    return;
                _mTim = value;
                NotifyOfPropertyChange(() => mTim);
            }
        }

        public bool mThemMoi
        {
            get
            {
                return _mThemMoi;
            }
            set
            {
                if (_mThemMoi == value)
                    return;
                _mThemMoi = value;
                NotifyOfPropertyChange(() => mThemMoi);
            }
        }
        public bool mChinhSua
        {
            get
            {
                return _mChinhSua;
            }
            set
            {
                if (_mChinhSua == value)
                    return;
                _mChinhSua = value;
                NotifyOfPropertyChange(() => mChinhSua);
            }
        }
        #endregion

        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;

        public bool bEdit
        {
            get
            {
                return _bEdit;
            }
            set
            {
                if (_bEdit == value)
                    return;
                _bEdit = value;
            }
        }
        public bool bAdd
        {
            get
            {
                return _bAdd;
            }
            set
            {
                if (_bAdd == value)
                    return;
                _bAdd = value;
            }
        }
        public bool bDelete
        {
            get
            {
                return _bDelete;
            }
            set
            {
                if (_bDelete == value)
                    return;
                _bDelete = value;
            }
        }
        public bool bView
        {
            get
            {
                return _bView;
            }
            set
            {
                if (_bView == value)
                    return;
                _bView = value;
            }
        }
        public bool bPrint
        {
            get
            {
                return _bPrint;
            }
            set
            {
                if (_bPrint == value)
                    return;
                _bPrint = value;
            }
        }

        public bool bReport
        {
            get
            {
                return _bReport;
            }
            set
            {
                if (_bReport == value)
                    return;
                _bReport = value;
            }
        }
        #endregion

        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkView { get; set; }
        
        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(mChinhSua);
        }
        public void lnkView_Loaded(object sender)
        {
            lnkView = sender as Button;
            lnkView.Visibility = Globals.convertVisibility(mChinhSua);
        }
        #endregion

        #region Public Properties

        private long? _V_MedProductType;
        public long? V_MedProductType
        {
            get { return _V_MedProductType; }
            set
            {
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
            }
        }

        private ObservableCollection<Lookup> _MedProductTypes;
        public ObservableCollection<Lookup> MedProductTypes
        {
            get { return _MedProductTypes; }
            set
            {
                if (_MedProductTypes != value)
                    _MedProductTypes = value;
                NotifyOfPropertyChange(() => MedProductTypes);
            }
        }

        private DrugDeptPharmaceuticalCompany _selectedDrugDeptPharmaceuticalCompany;
        public DrugDeptPharmaceuticalCompany SelectedDrugDeptPharmaceuticalCompany
        {
            get
            {
                return _selectedDrugDeptPharmaceuticalCompany;
            }
            set
            {
                if (_selectedDrugDeptPharmaceuticalCompany != value)
                {
                    _selectedDrugDeptPharmaceuticalCompany = value;
                    NotifyOfPropertyChange(() => SelectedDrugDeptPharmaceuticalCompany);
                }
            }
        }

        private PagedSortableCollectionView<RefGenMedProductDetails> _RefGenMedProductDetailss;
        public PagedSortableCollectionView<RefGenMedProductDetails> RefGenMedProductDetailss
        {
            get { return _RefGenMedProductDetailss; }
            set
            {
                if (_RefGenMedProductDetailss != value)
                    _RefGenMedProductDetailss = value;
                NotifyOfPropertyChange(() => RefGenMedProductDetailss);
            }
        }

        #endregion

        #region Function Member

        private IEnumerator<IResult> DoGetMedProductType()
        {
            var paymentTypeTask = new LoadLookupListTask(LookupValues.V_MedProductType, false, true);
            yield return paymentTypeTask;
            MedProductTypes = paymentTypeTask.LookupList;
            yield break;
        }

        public void RefGenMedProductDetails_ByPCOID(int PageIndex, int PageSize)
        {
            long PCOID = 0;
            if (SelectedDrugDeptPharmaceuticalCompany != null)
            {
                PCOID = SelectedDrugDeptPharmaceuticalCompany.PCOID;
            }
            else
            {
                RefGenMedProductDetailss.Clear();
                return;
            }
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenMedProductDetails_ByPCOID(PCOID, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int Total = 0;
                            var results = contract.EndRefGenMedProductDetails_ByPCOID(out Total, asyncResult);
                            RefGenMedProductDetailss.Clear();
                            if (results != null && results.Count > 0)
                            {
                                RefGenMedProductDetailss.TotalItemCount = Total;
                                foreach (RefGenMedProductDetails p in results)
                                {
                                    RefGenMedProductDetailss.Add(p);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void cbx_ChooseType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefGenMedProductDetailss.PageIndex = 0;
            RefGenMedProductDetails_ByPCOID(RefGenMedProductDetailss.PageIndex, RefGenMedProductDetailss.PageSize);
        }


      

        #endregion
    }
}
