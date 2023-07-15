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
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(ISupplierGenMedProductsPrice_ListSupplier)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SupplierGenMedProductsPrice_ListSupplierViewModel : Conductor<object>, ISupplierGenMedProductsPrice_ListSupplier
    {
        private long _V_MedProductType;
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                }

            }
        }

        private string _TitleForm;
        public string TitleForm
        {
            get
            {
                return _TitleForm;
            }
            set
            {
                if(_TitleForm!=value)
                {
                    _TitleForm = value;
                    NotifyOfPropertyChange(()=>TitleForm);
                }
            }
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

        public void Init()
        {
            Criteria = new SupplierGenMedProductsPriceSearchCriteria();
            Criteria.V_MedProductType = V_MedProductType;

            ObjSupplierGenMedProductsPrice_GetListSupplier_Paging = new PagedSortableCollectionView<Supplier>();
            ObjSupplierGenMedProductsPrice_GetListSupplier_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjSupplierGenMedProductsPrice_GetListSupplier_Paging_OnRefresh);

            //Load_V_MedProductType();

            ObjSupplierGenMedProductsPrice_GetListSupplier_Paging.PageIndex = 0;
            SupplierGenMedProductsPrice_GetListSupplier_Paging(0, ObjSupplierGenMedProductsPrice_GetListSupplier_Paging.PageSize, true);

        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SupplierGenMedProductsPrice_ListSupplierViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;


            authorization();
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _mTim = true;
        private bool _mQuanLy = true;
        private bool _mTaoGiaMoi = true;
        private bool _mSuaGia = true;
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
        public bool mQuanLy
        {
            get
            {
                return _mQuanLy;
            }
            set
            {
                if (_mQuanLy == value)
                    return;
                _mQuanLy = value;
                NotifyOfPropertyChange(() => mQuanLy);
            }
        }
        public bool mTaoGiaMoi
        {
            get
            {
                return _mTaoGiaMoi;
            }
            set
            {
                if (_mTaoGiaMoi == value)
                    return;
                _mTaoGiaMoi = value;
                NotifyOfPropertyChange(() => mTaoGiaMoi);
            }
        }
        public bool mSuaGia
        {
            get
            {
                return _mSuaGia;
            }
            set
            {
                if (_mSuaGia == value)
                    return;
                _mSuaGia = value;
                NotifyOfPropertyChange(() => mSuaGia);
            }
        }

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
        public Button hplMgntDrug { get; set; }
        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }
        public void hplMgntDrug_Loaded(object sender)
        {
            hplMgntDrug = sender as Button;
            hplMgntDrug.Visibility = Globals.convertVisibility(mQuanLy);
        }

        #endregion

        private ObservableCollection<Lookup> _ObjV_MedProductType;
        public ObservableCollection<Lookup> ObjV_MedProductType
        {
            get { return _ObjV_MedProductType; }
            set
            {
                _ObjV_MedProductType = value;
                NotifyOfPropertyChange(() => ObjV_MedProductType);
            }
        }

        //public void Load_V_MedProductType()
        //{
        //    var t = new Thread(() =>
        //    {
        //        Globals.EventAggregator.Publish(new BusyEvent
        //        {
        //            IsBusy = true,
        //            Message ="Danh Sách Loại Cung Cấp..."
        //        });
        //        try
        //        {
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetAllLookupValuesByType(LookupValues.V_MedProductType,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        IList<Lookup> allItems = new ObservableCollection<Lookup>();
        //                        try
        //                        {
        //                            allItems = contract.EndGetAllLookupValuesByType(asyncResult);

        //                            ObjV_MedProductType = new ObservableCollection<Lookup>(allItems);
        //                            Lookup firstItem = new Lookup();
        //                            firstItem.LookupID = -1;
        //                            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
        //                            ObjV_MedProductType.Insert(0, firstItem);
        //                        }
        //                        catch (FaultException<AxException> fault)
        //                        {

        //                        }
        //                        catch (Exception ex)
        //                        {
        //                        }

        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //        }
        //        finally
        //        {
        //            Globals.IsBusy = false;
        //        }
        //    });
        //    t.Start();
        //}

        void ObjSupplierGenMedProductsPrice_GetListSupplier_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            SupplierGenMedProductsPrice_GetListSupplier_Paging(ObjSupplierGenMedProductsPrice_GetListSupplier_Paging.PageIndex,ObjSupplierGenMedProductsPrice_GetListSupplier_Paging.PageSize, false);
        }


        private SupplierGenMedProductsPriceSearchCriteria _Criteria;
        public SupplierGenMedProductsPriceSearchCriteria Criteria
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

        private PagedSortableCollectionView<DataEntities.Supplier> _ObjSupplierGenMedProductsPrice_GetListSupplier_Paging;
        public PagedSortableCollectionView<DataEntities.Supplier> ObjSupplierGenMedProductsPrice_GetListSupplier_Paging
        {
            get { return _ObjSupplierGenMedProductsPrice_GetListSupplier_Paging; }
            set
            {
                _ObjSupplierGenMedProductsPrice_GetListSupplier_Paging = value;
                NotifyOfPropertyChange(() => ObjSupplierGenMedProductsPrice_GetListSupplier_Paging);
            }
        }

        private void SupplierGenMedProductsPrice_GetListSupplier_Paging(int PageIndex, int PageSize, bool CountTotal)
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
                        client.BeginSupplierGenMedProductsPrice_GetListSupplier_Paging(Criteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.Supplier> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSupplierGenMedProductsPrice_GetListSupplier_Paging(out Total, asyncResult);
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

                            ObjSupplierGenMedProductsPrice_GetListSupplier_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjSupplierGenMedProductsPrice_GetListSupplier_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjSupplierGenMedProductsPrice_GetListSupplier_Paging.Add(item);
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
            ObjSupplierGenMedProductsPrice_GetListSupplier_Paging.PageIndex = 0;
            SupplierGenMedProductsPrice_GetListSupplier_Paging(0, ObjSupplierGenMedProductsPrice_GetListSupplier_Paging.PageSize, true);
        }

        public void hplMgntDrug_Click(object selectItem)
        {
            if (selectItem != null)
            {
                //Supplier p = (selectItem as Supplier);

                //var typeInfo = Globals.GetViewModel<ISupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID>();
                //typeInfo.mSuaGia = mSuaGia;
                //typeInfo.mTaoGiaMoi = mTaoGiaMoi;
                //typeInfo.ObjSupplierCurrent = p;
                //typeInfo.ObjV_MedProductType = ObjV_MedProductType;
                //typeInfo.Criteria = new SupplierGenMedProductsPriceSearchCriteria();
                //typeInfo.Criteria.SupplierID = p.SupplierID;
                //typeInfo.Criteria.V_MedProductType = Criteria.V_MedProductType;
                //typeInfo.TitleForm = TitleForm_DanhSachHangCungCap();

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //                                 {
                //                                     //lam gi do
                //                                 });
                Action<ISupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID> onInitDlg = (typeInfo) =>
                {
                    Supplier p = (selectItem as Supplier);
                    typeInfo.mSuaGia = mSuaGia;
                    typeInfo.mTaoGiaMoi = mTaoGiaMoi;
                    typeInfo.ObjSupplierCurrent = p;
                    typeInfo.ObjV_MedProductType = ObjV_MedProductType;
                    typeInfo.Criteria = new SupplierGenMedProductsPriceSearchCriteria();
                    typeInfo.Criteria.SupplierID = p.SupplierID;
                    typeInfo.Criteria.V_MedProductType = Criteria.V_MedProductType;
                    typeInfo.TitleForm = TitleForm_DanhSachHangCungCap();
                };
                GlobalsNAV.ShowDialog<ISupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID>(onInitDlg);
            }
        }

        private string TitleForm_DanhSachHangCungCap()
        {
            string str = "";
            if(V_MedProductType==(long)AllLookupValues.MedProductType.THUOC)
            {
                str = eHCMSResources.K3080_G1_DSThuoc;
            }
            if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            {
                str = eHCMSResources.Z0657_G1_DSYCu;
            }
            if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
            {
                str= eHCMSResources.Z0658_G1_DSHChat;
            }
            return str;
        }


        public void cboV_MedProductType_SelectionChanged(object selectedItem)
        {
            Criteria.V_MedProductType = (selectedItem as DataEntities.Lookup).LookupID;
            ObjSupplierGenMedProductsPrice_GetListSupplier_Paging.PageIndex = 0;
            SupplierGenMedProductsPrice_GetListSupplier_Paging(0, ObjSupplierGenMedProductsPrice_GetListSupplier_Paging.PageSize, true);
        }
    }
}
