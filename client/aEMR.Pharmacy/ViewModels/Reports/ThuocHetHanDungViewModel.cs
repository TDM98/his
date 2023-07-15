using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using DataEntities;
using System.Windows;
using aEMR.Infrastructure.Events;
using System.Threading;
using aEMR.ServiceClient;
using System;
using aEMR.Common.Printing;
using DevExpress.Xpf.Printing;
using aEMR.ReportModel.ReportModels;
using System.Collections.Generic;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using DevExpress.ReportServer.Printing;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IThuocHetHanDung)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ThuocHetHanDungViewModel : Conductor<object>, IThuocHetHanDung
    {
        public string TitleForm { get; set; }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ThuocHetHanDungViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            authorization();

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
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

        private RefStorageWarehouseLocation _CurrentStore;
        public RefStorageWarehouseLocation CurrentStore
        {
            get
            {
                return _CurrentStore;
            }
            set
            {
                if (_CurrentStore != value)
                {
                    _CurrentStore = value;
                    NotifyOfPropertyChange(() => CurrentStore);
                }
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

        private bool IsTongKho = false;

        public long? StoreID;
        private int Type = 2;
        public string DateShow;
        public string Message;
        #endregion
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bXem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCKhac_ThuocHetHanDung,
                                               (int)oPharmacyEx.mBCKhac_ThuocHetHanDung_Xem, (int)ePermission.mEdit);
            bIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCKhac_ThuocHetHanDung,
                                               (int)oPharmacyEx.mBCKhac_ThuocHetHanDung_In, (int)ePermission.mAdd);
        }
        #region checking account

        private bool _bXem = true;
        private bool _bIn = true;
        

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
        
        #endregion

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null,false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
        //public void _reportModel_RequestDefaultParameterValues(object sender, System.EventArgs e)
        public void FillReport_ParameterValues()
        {
            rParams["StoreID"].Value = (int)StoreID.GetValueOrDefault(0);
            rParams["Type"].Value = Type;
            rParams["ShowMessage"].Value = Message;
            rParams["DateReport"].Value = DateShow;
            rParams["parLogoUrl"].Value = Globals.ServerConfigSection.CommonItems.ReportLogoUrl;
            // ReportModel.AutoShowParametersPanel = false;
        }

        public void btn_View(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                ReportModel = null;
                ReportModel = new DrugExpiryModel().PreviewModel;

                //ReportModel.RequestDefaultParameterValues += new WeakEventHandler<EventArgs>(_reportModel_RequestDefaultParameterValues).Handler;
                FillReport_ParameterValues();
                // ReportModel.AutoShowParametersPanel = false;
                ReportModel.CreateDocument(rParams);
            }
        }

        private void PrintDrugExpiryRpt(long? StoreID, int Type, string Message, string showdate)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDrugExpiryInPdfFormat(StoreID, Type, Message, showdate, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugExpiryInPdfFormat(asyncResult);
                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                            Globals.EventAggregator.Publish(results);
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
        string ShowDate = "";
        private bool CheckData()
        {
            if (!IsTongKho)
            {
                if (CurrentStore == null)
                {
                    MessageBox.Show(eHCMSResources.K0310_G1_ChonKhoCanXem);
                    return false;
                }
                else
                {
                    StoreID = CurrentStore.StoreID;
                    return true;
                }
            }
            else
            {
                StoreID = null;
                return true;
            }
        }

        public void btn_Print(object sender, RoutedEventArgs e)
        {
            if (CheckData())
            {
                PrintDrugExpiryRpt(StoreID,Type,Message, ShowDate);
            }
        }


        public void chk_TongKho_Checked(object sender, RoutedEventArgs e)
        {
            IsTongKho = true;
        }

        public void chk_TongKho_Unchecked(object sender, RoutedEventArgs e)
        {
            IsTongKho = false;
        }

        public void rdtExpiry_Checked(object sender, RoutedEventArgs e)
        {
            Type = 0;

        }
        public void rdtPreExpiry_Checked(object sender, RoutedEventArgs e)
        {
            Type = 1;

        }
        public void rdtAll_Checked(object sender, RoutedEventArgs e)
        {
            Type = 2;

        }

    }
}
