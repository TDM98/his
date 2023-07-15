using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using System.Collections.ObjectModel;
using aEMR.Common;
using eHCMSLanguage;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(ISupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierIDViewModel : Conductor<object>, ISupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID
        ,IHandle<SaveEvent<bool>>
        ,IHandle<DeleteEvent<bool>>
    {

        private string _TitleForm;
        public string TitleForm
        {
            get
            {
                return _TitleForm;
            }
            set
            {
                if (_TitleForm != value)
                {
                    _TitleForm = value;
                    NotifyOfPropertyChange(() => TitleForm);
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

        protected override void OnActivate()
        {
            authorization();
            Globals.EventAggregator.Subscribe(this);

            base.OnActivate();
            
            ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging=new PagedSortableCollectionView<SupplierGenMedProductsPrice>();
            ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging_OnRefresh);
            ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.PageSize = Globals.PageSize;

            ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.PageIndex = 0;
            SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(0, ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.PageSize, true);
        }

        void ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.PageIndex, ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.PageSize, true);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account
        private bool _mTaoGiaMoi = true;
        private bool _mSuaGia = true;
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
        public Button lnkView { get; set; }
        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }
        public void lnkView_Loaded(object sender)
        {
            lnkView = sender as Button;
            lnkView.Visibility = Globals.convertVisibility(bView);
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

        private Supplier _ObjSupplierCurrent;
        public Supplier ObjSupplierCurrent
        {
            get { return _ObjSupplierCurrent; }
            set
            {   
                _ObjSupplierCurrent = value;
                NotifyOfPropertyChange(()=>ObjSupplierCurrent);
            }
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

        private PagedSortableCollectionView<DataEntities.SupplierGenMedProductsPrice> _ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging;
        public PagedSortableCollectionView<DataEntities.SupplierGenMedProductsPrice> ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging
        {
            get { return _ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging; }
            set
            {
                _ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging = value;
                NotifyOfPropertyChange(() => ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging);
            }
        }

        private void SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3080_G1_DSThuoc) });
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySuppliersServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(Criteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.SupplierGenMedProductsPrice> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(out Total, asyncResult);
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

                            ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.Add(item);
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
            ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.PageIndex = 0;
            SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(0, ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.PageSize, true);
        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataEntities.SupplierGenMedProductsPrice objRows = e.Row.DataContext as DataEntities.SupplierGenMedProductsPrice;
            if (objRows != null)
            {
                switch (objRows.PriceType)
                {
                    case "PriceCurrent":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Green);
                            break;
                        }
                    case "PriceFuture-Active-1":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Gray);
                            break;
                        }
                    case "PriceFuture-Active-0":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Blue);
                            break;
                        }
                    case "PriceOld":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Orange);
                            break;
                        }
                    default:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Black);
                            break;
                        }
                }
            }
        }

        public void hplListPrice_Click(object selectItem)
        {
            if(selectItem!=null)
            {
                //SupplierGenMedProductsPrice p = (selectItem as SupplierGenMedProductsPrice);

                //var typeInfo = Globals.GetViewModel<ISupplierGenMedProductsPrice_ListPrice>();
                //typeInfo.mSuaGia = mSuaGia;
                //typeInfo.mTaoGiaMoi = mTaoGiaMoi;
                //typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                //typeInfo.ObjDrugCurrent = p;

                //typeInfo.Criteria=new SupplierGenMedProductsPriceSearchCriteria();
                //typeInfo.Criteria.SupplierID = ObjSupplierCurrent.SupplierID;
                //typeInfo.Criteria.GenMedProductID = p.GenMedProductID;
                //typeInfo.Criteria.PriceType = 1;//Giá hiện hành

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //lam gi do
                //});
                
                Action<ISupplierGenMedProductsPrice_ListPrice> onInitDlg = (typeInfo) =>
                {
                    SupplierGenMedProductsPrice p = (selectItem as SupplierGenMedProductsPrice);
                    typeInfo.mSuaGia = mSuaGia;
                    typeInfo.mTaoGiaMoi = mTaoGiaMoi;
                    typeInfo.ObjSupplierCurrent = ObjSupplierCurrent;
                    typeInfo.ObjDrugCurrent = p;

                    typeInfo.Criteria = new SupplierGenMedProductsPriceSearchCriteria();
                    typeInfo.Criteria.SupplierID = ObjSupplierCurrent.SupplierID;
                    typeInfo.Criteria.GenMedProductID = p.GenMedProductID;
                    typeInfo.Criteria.PriceType = 1;//Giá hiện hành
                };
                GlobalsNAV.ShowDialog<ISupplierGenMedProductsPrice_ListPrice>(onInitDlg);
            }
        }

        public void cboV_MedProductType_SelectionChanged(object selectedItem)
        {
            Criteria.V_MedProductType = (selectedItem as DataEntities.Lookup).LookupID;
            ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.PageIndex = 0;
            SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(0, ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.PageSize, true);
        }

        public void Handle(SaveEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.PageIndex = 0;
                    SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(0,ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.PageSize, true);
                }
            }
        }

        public void Handle(DeleteEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.PageIndex = 0;
                    SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(0, ObjSupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging.PageSize, true);
                }
            }
        }
    }
}
