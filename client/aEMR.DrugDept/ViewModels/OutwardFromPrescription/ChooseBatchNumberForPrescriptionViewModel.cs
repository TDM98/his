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
using System.Linq;
using Castle.Windsor;

namespace eHCMS.DrugDept.ViewModels
{
    [Export(typeof(IChooseBatchNumberForPrescription)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ChooseBatchNumberForPrescriptionViewModel : Conductor<object>, IChooseBatchNumberForPrescription
    {
        [ImportingConstructor]
        public ChooseBatchNumberForPrescriptionViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
        }
       
        #region Properties member

        private ObservableCollection<GetGenMedProductForSell> _BatchNumberListShow;
        public ObservableCollection<GetGenMedProductForSell> BatchNumberListShow
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

        public OutwardDrugMedDept SelectedOutwardDrug
        {
            get;
            set;
        }
        public ObservableCollection<OutwardDrugMedDept> OutwardDrugListByGenMedProductID
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


        public void grdRequestDetails_DblClick(object sender, aEMR.Common.EventArgs<object> e)
        {
            SelectedBatchNumber(e.Value);
        }

        public void grdRequestDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

       

        private void SelectedBatchNumber(object value)
        {
            GetGenMedProductForSell Item = value as GetGenMedProductForSell;
            ObservableCollection<OutwardDrugMedDept> ListNotContainsRowSelected = OutwardDrugListByGenMedProductID.Where(x=>x.InID !=SelectedOutwardDrug.InID).ToObservableCollection();
            if (ListNotContainsRowSelected != null && ListNotContainsRowSelected.Count > 0)
            {
                //var results = ListNotContainsRowSelected.Where(x => x.InID == Item.InID);
                ObservableCollection<OutwardDrugMedDept> results = new ObservableCollection<OutwardDrugMedDept>();
                foreach (var detail in ListNotContainsRowSelected)
                {
                    if (detail.InID == Item.InID)
                    {
                        results.Add(detail);
                    }
                }
                if (results != null && results.Count() > 0)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0833_G1_LoDaChonBenDuoi));
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

        private void ChooseBatchAndClose(GetGenMedProductForSell Item)
        {
            //20200422 TBL: Nếu chọn đúng dòng nhập đang được chọn để đổi lô thì thông báo
            if (Item.InID == SelectedOutwardDrug.InID)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0833_G1_LoDaChonBenDuoi));
                return;
            }
            if (Item.Remaining < SelectedOutwardDrug.OutQuantity)
            {
                if (MessageBox.Show(eHCMSResources.A0763_G1_Msg_InfoLoNayChiCon + Item.Remaining + " ( " + Item.UnitName + ") , " + eHCMSResources.A0764_G1_Msg_ConfBanCoMuonChon.ToLower(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //neu khac lo dang dc chon thi phat su kien de form duoi bat
                    if (Item.InID != SelectedOutwardDrug.InID)
                    {
                        if (FormType == 1)
                        {
                            Globals.EventAggregator.Publish(new ChooseBatchNumberForPrescriptionResetQtyEvent { BatchNumberVisitorSelected = Item });
                        }
                        else
                        {
                            Globals.EventAggregator.Publish(new EditChooseBatchNumberForPrescriptionResetQtyEvent { BatchNumberVisitorSelected = Item });
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
                        Globals.EventAggregator.Publish(new ChooseBatchNumberForPrescriptionEvent { BatchNumberVisitorSelected = Item });
                    }
                    else
                    {
                        Globals.EventAggregator.Publish(new EditChooseBatchNumberForPrescriptionEvent { BatchNumberVisitorSelected = Item });
                    }
                }
                TryClose();
            }
        }
    }
}
