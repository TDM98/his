using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.CommonTasks;
using System.Collections.Generic;
using eHCMSLanguage;
using Castle.Windsor;

namespace eHCMS.DrugDept.ViewModels
{
    [Export(typeof(IUsedMedProductInternal)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UsedMedProductInternalViewModel : Conductor<object>, IUsedMedProductInternal
    {
        [ImportingConstructor]
        public UsedMedProductInternalViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            Globals.EventAggregator.Subscribe(this);
            OutwardDrugClinicDepts = new PagedSortableCollectionView<OutwardDrugClinicDept>();
            OutwardDrugClinicDepts.OnRefresh += new EventHandler<RefreshEventArgs>(OutwardDrugClinicDepts_OnRefresh);
            OutwardDrugClinicDepts.PageSize = Globals.PageSize;

            Coroutine.BeginExecute(DoGetStore_MedDept());

            SearchCriteria = new SearchOutwardInfo();
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

        void OutwardDrugClinicDepts_OnRefresh(object sender, RefreshEventArgs e)
        {
            OutwardDrugClinicDeptInvoice_Search(OutwardDrugClinicDepts.PageIndex, OutwardDrugClinicDepts.PageSize);
        }


        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;
        private bool _bXuatExcel = true;

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
        public bool bXuatExcel
        {
            get
            {
                return _bXuatExcel;
            }
            set
            {
                if (_bXuatExcel == value)
                    return;
                _bXuatExcel = value;
            }
        }
        #endregion

        #region Propeties Member

        private string _strHienThi;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
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

        private SearchOutwardInfo _SearchCriteria;
        public SearchOutwardInfo SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                if (_SearchCriteria != value)
                {
                    _SearchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

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

        private PagedSortableCollectionView<OutwardDrugClinicDept> _OutwardDrugClinicDepts;
        public PagedSortableCollectionView<OutwardDrugClinicDept> OutwardDrugClinicDepts
        {
            get
            {
                return _OutwardDrugClinicDepts;
            }
            set
            {
                if (_OutwardDrugClinicDepts != value)
                {
                    _OutwardDrugClinicDepts = value;
                    NotifyOfPropertyChange(() => OutwardDrugClinicDepts);
                }
            }
        }

        #endregion

        #region Function Member

        private IEnumerator<IResult> DoGetStore_MedDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_CLINIC, false, null,false, true);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        private void OutwardDrugClinicDeptInvoice_Search(int PageIndex, int PageSize)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugClinicDeptInvoices_SearchTKPaging(SearchCriteria,V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int TotalCount = 0;
                            var results = contract.EndOutwardDrugClinicDeptInvoices_SearchTKPaging(out TotalCount, asyncResult);
                            OutwardDrugClinicDepts.Clear();
                            OutwardDrugClinicDepts.TotalItemCount = TotalCount;
                            if (results != null)
                            {
                                foreach (OutwardDrugClinicDept p in results)
                                {
                                   OutwardDrugClinicDepts.Add(p);
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

        #endregion

        public void btnSearch()
        {
            if (SearchCriteria != null)
            {
                if (SearchCriteria.fromdate != null && SearchCriteria.todate != null)
                {
                    if (SearchCriteria.fromdate.GetValueOrDefault() > SearchCriteria.todate.GetValueOrDefault())
                    {
                        MessageBox.Show(eHCMSResources.A0862_G1_Msg_InfoNgXuatKhHopLe3);
                        return;
                    }
                }
                if (SearchCriteria.fromdatedk != null && SearchCriteria.todatedk != null)
                {
                    if (SearchCriteria.fromdatedk.GetValueOrDefault() > SearchCriteria.todatedk.GetValueOrDefault())
                    {
                        MessageBox.Show(eHCMSResources.A0825_G1_Msg_InfoNgDKKhHopLe);
                        return;
                    }
                }
            }
            OutwardDrugClinicDepts.PageIndex = 0;
            OutwardDrugClinicDeptInvoice_Search(OutwardDrugClinicDepts.PageIndex, OutwardDrugClinicDepts.PageSize);
        }
    }
}
