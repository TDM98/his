using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using System;
using aEMR.Common.Collections;
using eHCMSLanguage;
using System.Linq;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IRouteOfAdministrationContactDrug)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class RouteOfAdministrationContactDrugViewModel : Conductor<object>, IRouteOfAdministrationContactDrug
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        public string OrderBy = "";
        public bool CountTotal = true;

        [ImportingConstructor]
        public RouteOfAdministrationContactDrugViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            _allNewRefMedicalCondition =new ObservableCollection<RefMedContraIndicationTypes>();
            _allV_RouteOfAdministration = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_RouteOfAdministration).ToObservableCollection();
            _allContrainName =new ObservableCollection<string>();
            GetRefMedicalConditionTypesAllPaging(1000, 0
                                                 , OrderBy, CountTotal);
            
        }

        void allV_RouteOfAdministration_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetRefMedicalConditionTypesAllPaging(1000, 0
                                                 , OrderBy, CountTotal);
        }

        public object DrugList { get; set; }

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
        public void butSave()
        {
            Globals.EventAggregator.Publish(new V_RouteOfAdministrationEvent() { V_RouteOfAdministration_Edit = V_RouteOfAdministration_Edit });
            TryClose();
             
        }
        public void butExit()
        {
            TryClose();
        }
        public void btAddChoose()
        {
            if (selectedV_RouteOfAdministration != null)
            {
                if (!V_RouteOfAdministration_Edit.Add(selectedV_RouteOfAdministration))
                {
                    MessageBox.Show("Đường dùng đã có");
                }
            }

        }
        
        public void lnkDelete_Click(object sender,RoutedEventArgs e)
        {
            V_RouteOfAdministration_Edit.Remove(selectedNewV_RouteOfAdministration);
        }
        public void DoubleClick(object sender,Common.EventArgs<object> e)
        {
            if (selectedV_RouteOfAdministration != null)
            {
                if (!V_RouteOfAdministration_Edit.Add(selectedV_RouteOfAdministration))
                {
                    MessageBox.Show("Đường dùng đã có");
                }
            }
        }
        #region properties

        private RefGenMedProductDetails _NewDrug;
        public RefGenMedProductDetails NewDrug
        {
            get
            {
                return _NewDrug;
            }
            set
            {
                if (_NewDrug == value)
                    return;
                _NewDrug = value;
                NotifyOfPropertyChange(() => NewDrug);
            }
        }

        private EntitiesEdit<Lookup> _V_RouteOfAdministration_Edit;
        public EntitiesEdit<Lookup> V_RouteOfAdministration_Edit
        {
            get
            {
                return _V_RouteOfAdministration_Edit;
            }
            set
            {
                if (_V_RouteOfAdministration_Edit == value)
                    return;
                _V_RouteOfAdministration_Edit = value;
                NotifyOfPropertyChange(() => V_RouteOfAdministration_Edit);
            }
        }

        private ObservableCollection<string> _allContrainName;
        public ObservableCollection<string> allContrainName
        {
            get
            {
                return _allContrainName;
            }
            set
            {
                if (_allContrainName == value)
                    return;
                _allContrainName = value;
                NotifyOfPropertyChange(()=>allContrainName);
            }
        }

        private ObservableCollection<Lookup> _allV_RouteOfAdministration;
        public ObservableCollection<Lookup> allV_RouteOfAdministration
        {
            get
            {
                return _allV_RouteOfAdministration;
            }
            set
            {
                if (_allV_RouteOfAdministration == value)
                    return;
                _allV_RouteOfAdministration = value;
                NotifyOfPropertyChange(()=>allV_RouteOfAdministration);
            }
        }

        private Lookup _selectedV_RouteOfAdministration;
        public Lookup selectedV_RouteOfAdministration
        {
            get
            {
                return _selectedV_RouteOfAdministration;
            }
            set
            {
                if (_selectedV_RouteOfAdministration == value)
                    return;
                _selectedV_RouteOfAdministration = value;
                NotifyOfPropertyChange(() => selectedV_RouteOfAdministration);
            }
        }

        private Lookup _selectedNewV_RouteOfAdministration;
        public Lookup selectedNewV_RouteOfAdministration
        {
            get
            {
                return _selectedNewV_RouteOfAdministration;
            }
            set
            {
                if (_selectedNewV_RouteOfAdministration == value)
                    return;
                _selectedNewV_RouteOfAdministration = value;
                NotifyOfPropertyChange(() => selectedNewV_RouteOfAdministration);
            }
        }

        private ObservableCollection<RefMedContraIndicationTypes> _allNewRefMedicalCondition;
        public ObservableCollection<RefMedContraIndicationTypes> allNewRefMedicalCondition
        {
            get
            {
                return _allNewRefMedicalCondition;
            }
            set
            {
                if (_allNewRefMedicalCondition == value)
                    return;
                _allNewRefMedicalCondition = value;
                NotifyOfPropertyChange(() => allNewRefMedicalCondition);
            }
        }

        private ObservableCollection<RefMedContraIndicationTypes> _allDeleteRefMedicalCondition;
        public ObservableCollection<RefMedContraIndicationTypes> allDeleteRefMedicalCondition
        {
            get
            {
                return _allDeleteRefMedicalCondition;
            }
            set
            {
                if (_allDeleteRefMedicalCondition == value)
                    return;
                _allDeleteRefMedicalCondition = value;
                NotifyOfPropertyChange(() => allDeleteRefMedicalCondition);
            }
        }
        #endregion


        #region method

        private void GetRefMedicalConditionTypesAllPaging(int PageSize, int PageIndex, string OrderBy, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            //IsLoading = true;
            //var t = new Thread(() =>
            //{
            //    using (var serviceFactory = new PharmacyDrugServiceClient())
            //    {
            //        var contract = serviceFactory.ServiceInstance;
            //        contract.BeginGetRefMedicalConditionTypesAllPaging(PageSize, PageIndex, OrderBy, CountTotal,Globals.DispatchCallback((asyncResult) =>
            //        {

            //            try
            //            {
            //                int Total = 0;
            //                var results = contract.EndGetRefMedicalConditionTypesAllPaging(out Total,asyncResult);
            //                if (results != null)
            //                {
            //                    if (allV_RouteOfAdministration == null)
            //                    {
            //                        allV_RouteOfAdministration = new PagedSortableCollectionView<RefMedContraIndicationTypes>();
            //                    }
            //                    else
            //                    {
            //                        allV_RouteOfAdministration.Clear();
            //                    }
            //                    foreach (var p in results)
            //                    {
            //                        allV_RouteOfAdministration.Add(p);
            //                    }
                                
            //                    NotifyOfPropertyChange(() => allV_RouteOfAdministration);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //            }
            //            finally
            //            {
            //                IsLoading = false;
            //                Globals.IsBusy = false;
            //            }

            //        }), null);

            //    }

            //});

            //t.Start();
        }
        
        private void DeleteRefMedicalConditions(int MCID, int MCTypeID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteRefMedicalConditions(MCID, MCTypeID, Globals.LoggedUserAccount.StaffID.Value, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndDeleteRefMedicalConditions(asyncResult);
                            //if (results == true)
                            //{
                            //    GetRefMedicalConditions(Convert.ToInt32(selectedV_RouteOfAdministrationType.MCTypeID));
                            //    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.K0537_G1_XoaOk), "");
                            //}
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
    }
}
