using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Linq;
/*
 *  20200704 #001 TTM: 0039312: Fix lỗi không đổi lô được nhà thuốc - xuất nội bộ
 */
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IChooseBatchNumberVisitor)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ChooseBatchNumberVisitorViewModel : Conductor<object>, IChooseBatchNumberVisitor
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ChooseBatchNumberVisitorViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        #region Properties member

        private ObservableCollection<GetDrugForSellVisitor> _BatchNumberListShow;
        public ObservableCollection<GetDrugForSellVisitor> BatchNumberListShow
        {
            get { return _BatchNumberListShow; }
            set
            {
                if (_BatchNumberListShow != value)
                {
                    _BatchNumberListShow = value;
                    NotifyOfPropertyChange(() => BatchNumberListShow);
                }
            }
        }

        public OutwardDrug SelectedOutwardDrug
        {
            get;
            set;
        }
        public ObservableCollection<OutwardDrug> OutwardDrugListByDrugID
        {
            get;
            set;
        }

        //1 : phat su kien cho form ban
        //2:phat su kien cho form chinh sua
        private int _FormType = 1;
        public int FormType
        {
            get { return _FormType; }
            set
            {
                _FormType = value;
            }
        }
        #endregion


        public void grdRequestDetails_DblClick(object sender, Common.EventArgs<object> e)
        {
            SelectedBatchNumber(e.Value);
        }

        public void grdRequestDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }



        private void SelectedBatchNumber(object value)
        {
            GetDrugForSellVisitor Item = value as GetDrugForSellVisitor;
            ObservableCollection<OutwardDrug> ListNotContainsRowSelected = OutwardDrugListByDrugID.Where(x => x.InID != SelectedOutwardDrug.InID).ToObservableCollection();
            if (ListNotContainsRowSelected != null && ListNotContainsRowSelected.Count > 0)
            {
                //var results = ListNotContainsRowSelected.Where(x => x.InID == Item.InID);
                ObservableCollection<OutwardDrug> results = new ObservableCollection<OutwardDrug>();
                foreach (var detail in ListNotContainsRowSelected)
                {
                    if (detail.InID == Item.InID)
                    {
                        results.Add(detail);
                    }
                }
                if (results != null && results.Count() > 0)
                {
                    MessageBox.Show(eHCMSResources.A0765_G1_Msg_InfoLoDaDcChon);
                }
                else
                {
                    ChooseBatchAndClose(Item);
                }
            }
            else
            {
                ChooseBatchAndClose(Item);
            }
        }

        private void ChooseBatchAndClose(GetDrugForSellVisitor Item)
        {
            //20200422 TBL: Nếu chọn đúng dòng nhập đang được chọn để đổi lô thì thông báo
            //▼===== #001
            //if (Item.InID != SelectedOutwardDrug.InID)
            if (Item.InID == SelectedOutwardDrug.InID)
            //▲===== 
            {
                MessageBox.Show(eHCMSResources.A0765_G1_Msg_InfoLoDaDcChon);
                return;
            }
            if (Item.Remaining < SelectedOutwardDrug.OutQuantity)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z1399_G1_LoNayChiCon01, Item.Remaining, Item.UnitName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //neu khac lo dang dc chon thi phat su kien de form duoi bat
                    if (Item.InID != SelectedOutwardDrug.InID)
                    {
                        if (FormType == 1)
                        {
                            Globals.EventAggregator.Publish(new ChooseBatchNumberVisitorResetQtyEvent { BatchNumberVisitorSelected = Item });
                        }
                        else
                        {
                            Globals.EventAggregator.Publish(new EditChooseBatchNumberVisitorResetQtyEvent { BatchNumberVisitorSelected = Item });
                        }
                    }
                    TryClose();
                }
            }
            else
            {
                //neu khac lo dang dc chon thi phat su kien de form duoi bat
                if (Item.InID != SelectedOutwardDrug.InID)
                {
                    if (FormType == 1)
                    {
                        Globals.EventAggregator.Publish(new ChooseBatchNumberVisitorEvent { BatchNumberVisitorSelected = Item });
                    }
                    else
                    {
                        Globals.EventAggregator.Publish(new EditChooseBatchNumberVisitorEvent { BatchNumberVisitorSelected = Item });
                    }
                }
                TryClose();
            }
        }
    }
}
