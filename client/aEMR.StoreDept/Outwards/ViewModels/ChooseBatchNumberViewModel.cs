using eHCMSLanguage;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common;

namespace aEMR.StoreDept.Outwards.ViewModels
{
    [Export(typeof(IChooseBatchNumberClinicDept)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ChooseBatchNumberViewModel : Conductor<object>, IChooseBatchNumberClinicDept
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ChooseBatchNumberViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }

        protected override void OnDeactivate(bool close)
        {
            BatchNumberListShow = null;
            OutwardDrugListByDrugID = null;
        }
        #region Properties member

        private ObservableCollection<RefGenMedProductDetails> _BatchNumberListShow;
        public ObservableCollection<RefGenMedProductDetails> BatchNumberListShow
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

        public OutwardDrugClinicDept SelectedOutwardDrug
        {
            get;
            set;
        }
        public ObservableCollection<OutwardDrugClinicDept> OutwardDrugListByDrugID
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


        public void grdRequestDetails_DblClick(object sender, EventArgs<object> e)
        {
            SelectedBatchNumber(e.Value);
        }

        public void grdRequestDetails_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }



        private void SelectedBatchNumber(object value)
        {
            RefGenMedProductDetails Item = value as RefGenMedProductDetails;
            ObservableCollection<OutwardDrugClinicDept> ListNotContainsRowSelected = OutwardDrugListByDrugID.Where(x => x.InID != SelectedOutwardDrug.InID).ToObservableCollection();
            if (ListNotContainsRowSelected != null && ListNotContainsRowSelected.Count > 0)
            {
                //var results = ListNotContainsRowSelected.Where(x => x.InID == Item.InID);
                ObservableCollection<OutwardDrugClinicDept> results = new ObservableCollection<OutwardDrugClinicDept>();
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

        private void ChooseBatchAndClose(RefGenMedProductDetails Item)
        {
            //20200422 TBL: Nếu chọn đúng dòng nhập đang được chọn để đổi lô thì thông báo
            if (Item.InID == SelectedOutwardDrug.InID)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0833_G1_LoDaChonBenDuoi));
                return;
            }
            if (Item.Remaining < SelectedOutwardDrug.OutQuantity)
            {
                if (MessageBox.Show(eHCMSResources.A0763_G1_Msg_InfoLoNayChiCon + Item.Remaining + " ( " + Item.SelectedUnit.UnitName + ") , bạn có muốn chọn không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //neu khac lo dang dc chon thi phat su kien de form duoi bat
                    if (Item.InID != SelectedOutwardDrug.InID)
                    {
                        if (FormType == 1)
                        {
                            Globals.EventAggregator.Publish(new ClinicDeptChooseBatchNumberResetQtyEvent { BatchNumberSelected = Item });
                        }
                        else
                        {
                            Globals.EventAggregator.Publish(new ClinicDeptEditChooseBatchNumberResetQtyEvent { BatchNumberSelected = Item });
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
                        Globals.EventAggregator.Publish(new ClinicDeptChooseBatchNumberEvent { BatchNumberSelected = Item });
                    }
                    else
                    {
                        Globals.EventAggregator.Publish(new ClinicDeptEditChooseBatchNumberEvent { BatchNumberSelected = Item });
                    }
                }
                TryClose();
            }
        }
    }
}
