using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common.BaseModel;
using System.Windows.Media;


namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ICollectionMultiDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CollectionMultiDrugViewModel : ViewModelBase, ICollectionMultiDrug
    {
        #region Indicator Member

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
        #endregion

        private int count = 0;
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public CollectionMultiDrugViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }


        #region Properties member

        private ObservableCollection<OutwardDrugInvoice> _OutwardInfoList;
        public ObservableCollection<OutwardDrugInvoice> OutwardInfoList
        {
            get
            {
                return _OutwardInfoList;
            }
            set
            {
                if (_OutwardInfoList != value)
                {
                    _OutwardInfoList = value;
                    NotifyOfPropertyChange(() => OutwardInfoList);
                }
            }
        }

        private string _pageTitle;
        public string pageTitle
        {
            get { return _pageTitle; }
            set
            {
                if (_pageTitle != value)
                    _pageTitle = value;
                NotifyOfPropertyChange(() => pageTitle);
            }
        }

        private DateTime? _FromDate = Globals.ServerDate.Value.AddDays(-1);
        public DateTime? FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    NotifyOfPropertyChange(() => FromDate);
                }
            }
        }

        private DateTime? _ToDate = Globals.ServerDate.Value;
        public DateTime? ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    NotifyOfPropertyChange(() => ToDate);
                }
            }
        }

        //AllChecked

        private bool _AllChecked;
        public bool AllChecked
        {
            get
            {
                return _AllChecked;
            }
            set
            {
                if (_AllChecked != value)
                {
                    _AllChecked = value;
                    NotifyOfPropertyChange(() => AllChecked);
                    if (_AllChecked)
                    {
                        AllCheckedfc();
                    }
                    else
                    {
                        UnCheckedfc();
                    }
                }
            }
        }

        private long _StoreID;
        public long StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                if(_StoreID != value)
                {
                    _StoreID = value;
                    NotifyOfPropertyChange(() => StoreID);
                }
            }
        }
        #endregion

        public void GetlstCollectionDrug()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
            //IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginOutwardDrugInvoice_CollectMultiDrug(Top, FromDate.GetValueOrDefault(), ToDate.GetValueOrDefault(), StoreID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndOutwardDrugInvoice_CollectMultiDrug(asyncResult);
                            OutwardInfoList = results.ToObservableCollection();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            _logger.Info(ex.Message);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                            //IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void UpdateCollectionDrug()
        {
            this.DlgShowBusyIndicator(eHCMSResources.A0282_G1_Msg_InfoCNhatStatusOK);
            //IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginOutwardDrugInvoice_UpdateCollectMulti(OutwardInfoList, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndOutwardDrugInvoice_UpdateCollectMulti(asyncResult);
                            MessageBox.Show(eHCMSResources.A0282_G1_Msg_InfoCNhatStatusOK);
                            OutwardInfoList.Clear();
                            count++;
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            _logger.Info(ex.Message);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                            //IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnSearch()
        {
            if (FromDate == null || ToDate == null)
            {
                MessageBox.Show(eHCMSResources.K0438_G1_NhapNgThCanXem);
                return;
            }
            if (FromDate > ToDate)
            {
                MessageBox.Show(eHCMSResources.K0229_G1_TuNgKhongLonHonDenNg);
                return;
            }
            if (FromDate.GetValueOrDefault().Year <= (Globals.ServerDate.Value.Year - 10) || ToDate.GetValueOrDefault().Year <= (Globals.ServerDate.Value.Year - 10))
            {
                MessageBox.Show(eHCMSResources.A0859_G1_Msg_InfoNgThangKhHopLe4 + (Globals.ServerDate.Value.Year - 10).ToString());
                return;
            }
            GetlstCollectionDrug();
        }

        public void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            OutwardDrugInvoice p = e.Row.DataContext as OutwardDrugInvoice;
            if (p != null && p.IsHICount.GetValueOrDefault() && p.V_OutDrugInvStatus != (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                e.Row.Background = new SolidColorBrush(Colors.Yellow);
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                e.Row.Background = new SolidColorBrush(Colors.Transparent);
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private int Top = 100;
        public void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Top = Convert.ToInt32(((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
        }
        private void AllCheckedfc()
        {
            if (OutwardInfoList != null && OutwardInfoList.Count > 0)
            {
                for (int i = 0; i < OutwardInfoList.Count; i++)
                {
                    OutwardInfoList[i].Checked = true;
                }
            }
        }
        private void UnCheckedfc()
        {
            if (OutwardInfoList != null && OutwardInfoList.Count > 0)
            {
                for (int i = 0; i < OutwardInfoList.Count; i++)
                {
                    OutwardInfoList[i].Checked = false;
                }
            }
        }

        public void btnCollectDrug()
        {
            if (OutwardInfoList != null && OutwardInfoList.Count > 0)
            {
                if (OutwardInfoList.Where(x => x.Checked == true).Count() > 0)
                {
                    UpdateCollectionDrug();
                }
                else
                {
                    MessageBox.Show(eHCMSResources.K0378_G1_ChonPhCNhatTrThai);
                }
            }
        }

        public void btnClose()
        {
            TryClose();
            if (count > 0)
            {
                //phat su kien de from duoi bat
                Globals.EventAggregator.Publish(new PharmacyCloseCollectionMultiEvent { });
            }
        }
    }
}
