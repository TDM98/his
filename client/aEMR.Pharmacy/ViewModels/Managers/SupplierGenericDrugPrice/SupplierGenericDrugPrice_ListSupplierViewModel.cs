using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ISupplierGenericDrugPrice_ListSupplier)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SupplierGenericDrugPrice_ListSupplierViewModel : Conductor<object>, ISupplierGenericDrugPrice_ListSupplier
    {
       public string TitleForm { get; set; }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SupplierGenericDrugPrice_ListSupplierViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;


            Criteria =new SupplierGenericDrugPriceSearchCriteria();
            authorization();
            
            ObjSupplierGenericDrugPrice_GetListSupplier_Paging=new PagedSortableCollectionView<Supplier>();
            ObjSupplierGenericDrugPrice_GetListSupplier_Paging.OnRefresh +=new System.EventHandler<RefreshEventArgs>(ObjSupplierGenericDrugPrice_GetListSupplier_Paging_OnRefresh);

            ObjSupplierGenericDrugPrice_GetListSupplier_Paging.PageIndex = 0;
            SupplierGenericDrugPrice_GetListSupplier_Paging(0, ObjSupplierGenericDrugPrice_GetListSupplier_Paging.PageSize, true);
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
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyGiaTuNCC,
                                               (int)oPharmacyEx.mQuanLyGiaTuNCC_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyGiaTuNCC,
                                               (int)oPharmacyEx.mQuanLyGiaTuNCC_Them, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyGiaTuNCC,
                                               (int)oPharmacyEx.mQuanLyGiaTuNCC_ChinhSua, (int)ePermission.mView);
        }
        #region checking account

        private bool _bTim = true;
        private bool _bThem = true;
        private bool _bChinhSua = true;

        public bool bTim
        {
            get
            {
                return _bTim;
            }
            set
            {
                if (_bTim == value)
                    return;
                _bTim = value;
            }
        }
        public bool bThem
        {
            get
            {
                return _bThem;
            }
            set
            {
                if (_bThem == value)
                    return;
                _bThem = value;
            }
        }
        public bool bChinhSua
        {
            get
            {
                return _bChinhSua;
            }
            set
            {
                if (_bChinhSua == value)
                    return;
                _bChinhSua = value;
            }
        }

        #endregion
        #region binding visibilty

        public Button hplMgntDrug { get; set; }

        public void hplMgntDrug_Loaded(object sender)
        {
            hplMgntDrug = sender as Button;
            hplMgntDrug.Visibility = Globals.convertVisibility(bTim);
        }

        #endregion

        #region Properties Member

        private PagedSortableCollectionView<RefGenericDrugDetail> _RefGenericDrugDetails;
        public PagedSortableCollectionView<RefGenericDrugDetail> RefGenericDrugDetails
        {
            get
            {
                return _RefGenericDrugDetails;
            }
            set
            {
                if (_RefGenericDrugDetails != value)
                {
                    _RefGenericDrugDetails = value;
                }
                NotifyOfPropertyChange(() => RefGenericDrugDetails);
            }
        }

        private SupplierGenericDrug _SupplierDrug;
        public SupplierGenericDrug SupplierDrug
        {
            get
            {
                return _SupplierDrug;
            }
            set
            {
                if (_SupplierDrug != value)
                {
                    _SupplierDrug = value;
                }
                NotifyOfPropertyChange(() => SupplierDrug);
            }
        }

        private PagedSortableCollectionView<SupplierGenericDrug> _ListSupplierDrug;
        public PagedSortableCollectionView<SupplierGenericDrug> ListSupplierDrug
        {
            get
            {
                return _ListSupplierDrug;
            }
            set
            {
                if (_ListSupplierDrug != value)
                {
                    _ListSupplierDrug = value;
                }
                NotifyOfPropertyChange(() => ListSupplierDrug);
            }
        }

        private string _supplierName;
        public string SupplierName
        {
            get
            {
                return _supplierName;
            }
            set
            {
                if (_supplierName != value)
                {
                    _supplierName = value;
                    NotifyOfPropertyChange(() => SupplierName);
                }
            }
        }

        private int _SupplierType;
        public int SupplierType
        {
            get { return _SupplierType; }
            set
            {
                if (_SupplierType != value)
                {
                    _SupplierType = value;
                    NotifyOfPropertyChange(() => SupplierType);
                }
            }
        }
        #endregion
        void ObjSupplierGenericDrugPrice_GetListSupplier_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            SupplierGenericDrugPrice_GetListSupplier_Paging(ObjSupplierGenericDrugPrice_GetListSupplier_Paging.PageIndex,ObjSupplierGenericDrugPrice_GetListSupplier_Paging.PageSize, false);
        }


        private SupplierGenericDrugPriceSearchCriteria _Criteria;
        public SupplierGenericDrugPriceSearchCriteria Criteria
        {
            get
            {
                return _Criteria;
            }
            set
            {
                _Criteria = value;
                NotifyOfPropertyChange(() => Criteria);

            }
        }

        private PagedSortableCollectionView<DataEntities.Supplier> _ObjSupplierGenericDrugPrice_GetListSupplier_Paging;
        public PagedSortableCollectionView<DataEntities.Supplier> ObjSupplierGenericDrugPrice_GetListSupplier_Paging
        {
            get { return _ObjSupplierGenericDrugPrice_GetListSupplier_Paging; }
            set
            {
                _ObjSupplierGenericDrugPrice_GetListSupplier_Paging = value;
                NotifyOfPropertyChange(() => ObjSupplierGenericDrugPrice_GetListSupplier_Paging);
            }
        }

        private void SupplierGenericDrugPrice_GetListSupplier_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3002_G1_DSNCC) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySuppliersServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSupplierGenericDrugPrice_GetListSupplier_Paging(Criteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.Supplier> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSupplierGenericDrugPrice_GetListSupplier_Paging(out Total, asyncResult);
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

                            ObjSupplierGenericDrugPrice_GetListSupplier_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjSupplierGenericDrugPrice_GetListSupplier_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjSupplierGenericDrugPrice_GetListSupplier_Paging.Add(item);
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
                    IsLoading = false;
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        public void btFind()
        {
            ObjSupplierGenericDrugPrice_GetListSupplier_Paging.PageIndex = 0;
            SupplierGenericDrugPrice_GetListSupplier_Paging(0, ObjSupplierGenericDrugPrice_GetListSupplier_Paging.PageSize, true);
        }

        public void hplMgntDrug_Click(object selectItem)
        {
            if (selectItem != null)
            {
                Supplier p = (selectItem as Supplier);

                //var typeInfo = Globals.GetViewModel<ISupplierGenericDrugPrice_ListDrugBySupplierID>();
                //typeInfo.ObjSupplierCurrent = p;

                //typeInfo.Criteria = new SupplierGenericDrugPriceSearchCriteria();
                //typeInfo.Criteria.SupplierID = p.SupplierID;

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //                                 {
                //                                     //lam gi do
                //                                 });
                Action<ISupplierGenericDrugPrice_ListDrugBySupplierID> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjSupplierCurrent = p;

                    typeInfo.Criteria = new SupplierGenericDrugPriceSearchCriteria();
                    typeInfo.Criteria.SupplierID = p.SupplierID;
                };
                GlobalsNAV.ShowDialog<ISupplierGenericDrugPrice_ListDrugBySupplierID>(onInitDlg);
            }

        }

        public void DoubleClick(object sender, Common.EventArgs<object> e)
        {
            Supplier p = (e.Value as Supplier).DeepCopy();
            if (p != null)
            {
                //var typeInfo = Globals.GetViewModel<ISupplierGenericDrugPrice_ListDrugBySupplierID>();
                //typeInfo.ObjSupplierCurrent = p;

                //typeInfo.Criteria = new SupplierGenericDrugPriceSearchCriteria();
                //typeInfo.Criteria.SupplierID = p.SupplierID;

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //lam gi do
                //});

                Action<ISupplierGenericDrugPrice_ListDrugBySupplierID> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjSupplierCurrent = p;

                    typeInfo.Criteria = new SupplierGenericDrugPriceSearchCriteria();
                    typeInfo.Criteria.SupplierID = p.SupplierID;
                };
                GlobalsNAV.ShowDialog<ISupplierGenericDrugPrice_ListDrugBySupplierID>(onInitDlg);
            }
        }
    }
}

