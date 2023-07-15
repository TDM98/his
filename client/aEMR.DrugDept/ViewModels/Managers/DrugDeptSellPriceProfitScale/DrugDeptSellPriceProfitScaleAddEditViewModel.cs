using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptSellPriceProfitScaleAddEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptSellPriceProfitScaleAddEditViewModel : ViewModelBase, IDrugDeptSellPriceProfitScaleAddEdit
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DrugDeptSellPriceProfitScaleAddEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
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
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
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

        private DrugDeptSellPriceProfitScale _ObjDrugDeptSellPriceProfitScale_Current;
        public DrugDeptSellPriceProfitScale ObjDrugDeptSellPriceProfitScale_Current
        {
            get { return _ObjDrugDeptSellPriceProfitScale_Current; }
            set
            {
                _ObjDrugDeptSellPriceProfitScale_Current = value;
                NotifyOfPropertyChange(() => ObjDrugDeptSellPriceProfitScale_Current);
            }
        }

        public void FormLoad()
        {
         
        }

        public void InitializeNewItem()
        {
            ObjDrugDeptSellPriceProfitScale_Current = new DrugDeptSellPriceProfitScale();
            ObjDrugDeptSellPriceProfitScale_Current.IsActive = true;
            ObjDrugDeptSellPriceProfitScale_Current.V_MedProductType = V_MedProductType;
        }

        public void btSave()
        {
            if (ObjDrugDeptSellPriceProfitScale_Current.BuyingCostTo<=0)
            {
                MessageBox.Show(eHCMSResources.A0560_G1_Msg_InfoGVonDenLonHon0);
                return;
            }

            if (ObjDrugDeptSellPriceProfitScale_Current.BuyingCostFrom > ObjDrugDeptSellPriceProfitScale_Current.BuyingCostTo)
            {
                MessageBox.Show(eHCMSResources.A0561_G1_Msg_InfoGiaTuNhoHonGiaDen);
                return;
            }

            DrugDeptSellPriceProfitScale_AddEdit();
        }

        public void btClose()
        {
            TryClose();
        }

        private void DrugDeptSellPriceProfitScale_AddEdit()
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0343_G1_DangLuu);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDrugDeptSellPriceProfitScale_AddEdit(ObjDrugDeptSellPriceProfitScale_Current, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string Result = "";
                            contract.EndDrugDeptSellPriceProfitScale_AddEdit(out Result, asyncResult);
                            switch (Result)
                            {
                                case "0":
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0477_G1_LuuKhongThanhCong), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        break;
                                    }
                                case "1":
                                    {
                                        Globals.EventAggregator.Publish(new SaveEvent<bool>() { Result = true });
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                        TryClose();
                                        break;
                                    }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }
                    }), null);
                }


            });
            t.Start();
        }
    }
}

