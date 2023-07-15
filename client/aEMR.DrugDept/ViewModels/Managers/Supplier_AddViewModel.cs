using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using System.Collections.ObjectModel;
using System.Linq;
using aEMR.Common.Collections;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(ISupplierProduct_Add)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Supplier_AddViewModel : Conductor<object>, ISupplierProduct_Add
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Supplier_AddViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            _newSupplier = new DrugDeptSupplier();
            _newSupplier.SupplierType = (long)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE;
            _newSupplier.SupplierDrugDeptPharmOthers = 1;
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
        private bool _IsAll = false;
        public bool IsAll
        {
            get { return _IsAll; }
            set
            {
                if (_IsAll != value)
                {
                    _IsAll = value;
                    NotifyOfPropertyChange(() => IsAll);
                }
            }
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
                            contract.EndAddNewDrugDeptSupplier(out SupplierID, out StrError, asyncResult);
                            if (SupplierID > 0)
                            {
                                if (IsAddFinishClosed)
                                {
                                    NewSupplier.SupplierID = SupplierID;
                                    TryClose();
                                    //add xong dog lai lien va chon thuoc moi nay de lam viec luon
                                    Globals.EventAggregator.Publish(new DrugDeptCloseFinishAddSupplierEvent { SelectedSupplier = NewSupplier });
                                }
                                else
                                {
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.A0079_G1_Msg_InfoThemMoiOK);
                                    TryClose();
                                }
                            }
                            else
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
                Globals.EventAggregator.Publish(new DrugDeptCloseAddSupplierEvent());
            }
        }

        public void Refesh(object sender, RoutedEventArgs e)
        {
            NewSupplier = new DrugDeptSupplier();
            _newSupplier.SupplierType = (long)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE;
        }
        public void chkIsAll_Check(object sender, RoutedEventArgs e)
        {
            NewSupplier.SupplierDrugDeptPharmOthers = 3;
        }
        public void chkIsAll_Uncheck(object sender, RoutedEventArgs e)
        {
            NewSupplier.SupplierDrugDeptPharmOthers = 1;
        }
    }
}
