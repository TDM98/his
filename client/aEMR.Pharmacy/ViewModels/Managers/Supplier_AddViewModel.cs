using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using DataEntities;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ISupplier_Add)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Supplier_AddViewModel : Conductor<object>, ISupplier_Add
    {
        //sau khi add xog dong popup lai va chon supplier vua moi them vao,true la dong, false la mo
        private bool _IsAddFinishClosed = false;
        public bool IsAddFinishClosed
        {
            get
            {
                return _IsAddFinishClosed;
            }
            set
            {
                if (_IsAddFinishClosed != value)
                {
                    _IsAddFinishClosed = value;
                    NotifyOfPropertyChange(() => IsAddFinishClosed);
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Supplier_AddViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            _newSupplier = new DrugDeptSupplier();
            _newSupplier.SupplierType = (long)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE;
            _newSupplier.SupplierDrugDeptPharmOthers = 2;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
        }

        public void SupplierType_Checked(object sender, byte value)
        {
            NewSupplier.SupplierDrugDeptPharmOthers = value;
        }

        private DrugDeptSupplier _newSupplier;
        public DrugDeptSupplier NewSupplier
        {
            get
            {
                if (_newSupplier == null)
                    _newSupplier = new DrugDeptSupplier();
                return _newSupplier;
            }
            set
            {
                _newSupplier = value;
                NotifyOfPropertyChange(() => NewSupplier);
            }
        }

        
        public void AddNewSupplier()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            long SupplierID = 0;
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginAddNewDrugDeptSupplier(NewSupplier, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string StrError = "";
                            contract.EndAddNewDrugDeptSupplier(out SupplierID,out StrError, asyncResult);
                            if (SupplierID > 0)
                            {
                                GlobalsNAV.ShowMessagePopup(eHCMSResources.Z1493_G1_DuLieuDaDcThem);
                                //xoa cac truong tren du lieu
                                if (IsAddFinishClosed)
                                {
                                    NewSupplier.SupplierID = SupplierID;
                                    TryClose();
                                    //add xong dog lai lien va chon thuoc moi nay de lam viec luon
                                    Globals.EventAggregator.Publish(new PharmacyCloseFinishAddSupplierEvent { SelectedSupplier = NewSupplier });
                                }
                                else
                                {
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                    TryClose();
                                }
                                //Refesh();
                            }
                            else if (SupplierID == 0)
                            {
                                MessageBox.Show(StrError);
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

        public bool CheckValid(object temp)
        {
            DrugDeptSupplier u = temp as DrugDeptSupplier;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }

        public void OKButton(object sender, RoutedEventArgs e)
        {
            if (CheckValid(NewSupplier))
            {
                AddNewSupplier();
            }
        }

        public void CancelButton(object sender, RoutedEventArgs e)
        {
            TryClose();
            if (!IsAddFinishClosed)
            {
                //phat su kien de form cha load lai du lieu
                Globals.EventAggregator.Publish(new PharmacyCloseAddSupplierEvent());
            }
        }

        public void Refesh()
        {
            NewSupplier = null;
            NewSupplier = new DrugDeptSupplier();
            _newSupplier.SupplierType = (long)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE;
        }
    }
}
