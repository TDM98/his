using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
/*
 * 20191106 #001 TNHX: [BM 0013306]: separate V_MedProductType
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IBidCollection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BidCollectionViewModel : ViewModelBase, IBidCollection
    {
        #region Properties
        private ObservableCollection<Bid> _gBidCollection;
        public ObservableCollection<Bid> gBidCollection
        {
            get
            {
                return _gBidCollection;
            }
            set
            {
                if (_gBidCollection != value)
                {
                    _gBidCollection = value;
                    NotifyOfPropertyChange(() => gBidCollection);
                }
            }
        }
        public Bid SelectedBid { get; set; }

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
        #endregion

        #region Events
        [ImportingConstructor]
        public BidCollectionViewModel(IWindsorContainer aContainer, INavigationService aNavigation, IEventAggregator aEventArg, ISalePosCaching aCaching)
        {
            LoadBids();
        }
        public void RemoveItemCmd(object source, object eventArgs)
        {
        }
        public void EditItemCmd(object source, object eventArgs)
        {
            if ((source as FrameworkElement).DataContext == null)
            {
                return;
            }
            Action<IBidEdit> onInitDlg = (Alloc) =>
            {
                Alloc.ObjBid = (source as FrameworkElement).DataContext as Bid;
            };
            GlobalsNAV.ShowDialog<IBidEdit>(onInitDlg, (o, s) => { LoadBids(); });
        }
        public void gvBidCollection_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!(sender is DataGrid) || (sender as DataGrid).SelectedItem == null
                || !((sender as DataGrid).SelectedItem is Bid))
            {
                return;
            }
            SelectedBid = (sender as DataGrid).SelectedItem as Bid;
            TryClose();
        }
        #endregion

        #region Methods
        private void LoadBids()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = mServiceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginGetAllBids(V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                gBidCollection = new ObservableCollection<Bid>(contract.EndGetAllBids(asyncResult));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                    }
                }
            });
            t.Start();
        }

        public void gvBidCollection_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }
        #endregion
    }
}
