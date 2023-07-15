using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using eHCMSLanguage;

namespace aEMR.Pharmacy.ViewModels
{
     [Export(typeof(IRefGenDrugListEx)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefGenDrugListExViewModel : Conductor<object>, IRefGenDrugListEx
    {
         public string TitleForm { get; set; }

         private long V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        private bool _IsPopUp=false;
        public bool IsPopUp
        {
            get
            {
                return _IsPopUp;
            }
            set
            {
                if (_IsPopUp != value)
                {
                    _IsPopUp = value;
                    NotifyOfPropertyChange(() => IsPopUp);
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
        public RefGenDrugListExViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching; 

            DrugsResearch = new PagedSortableCollectionView<RefGenericDrugDetail>();
             DrugsResearch.OnRefresh += new EventHandler<RefreshEventArgs>(DrugsResearch_OnRefresh);
             DrugsResearch.PageSize = Globals.PageSize;
             DrugsResearch.PageSize = 10;
             GetFamilytherapies(V_MedProductType);
             SearchDrugs(0,DrugsResearch.PageSize);

         }

         void DrugsResearch_OnRefresh(object sender, RefreshEventArgs e)
         {
             SearchDrugs(DrugsResearch.PageIndex,DrugsResearch.PageSize);
         }
         #region Properties Member
         private const string ALLITEMS = "[ALL]";

         private string _BrandName;
         public string BrandName
         {
             get { return _BrandName; }
             set
             {
                 _BrandName = value;
                 NotifyOfPropertyChange(() => BrandName);
             }
         }

         public enum Insurance
         {
             All = 1,
             Yes = 2,
             No = 3
         }
         public enum Consult
         {
             All = 1,
             Yes = 2,
             No = 3
         }

         private byte IsInsurance = (byte)Insurance.All;
         private byte IsConsult=(byte)Consult.All;

         private DrugSearchCriteria _searchCriteria;
         public DrugSearchCriteria SearchCriteria
         {
             get
             {
                 return _searchCriteria;
             }
             set
             {
                 _searchCriteria = value;
                 NotifyOfPropertyChange(()=>SearchCriteria);
             }
         }

         private PagedSortableCollectionView<RefGenericDrugDetail> _drugsresearch;
         public PagedSortableCollectionView<RefGenericDrugDetail> DrugsResearch
         {
             get
             {
                 return _drugsresearch;
             }
             set
             {
                 if (_drugsresearch != value)
                 {
                     _drugsresearch = value;
                     NotifyOfPropertyChange(()=>DrugsResearch);
                 }
             }
         }

         private RefGenericDrugDetail _CurrentDrug;
         public RefGenericDrugDetail CurrentDrug
         {
             get
             {
                 return _CurrentDrug;
             }
             set
             {
                 if (_CurrentDrug != value)
                 {
                     _CurrentDrug = value;
                     NotifyOfPropertyChange(() => CurrentDrug);
                 }
             }
         }


         private ObservableCollection<DrugClass> _familytherapies;
         public ObservableCollection<DrugClass> FamilyTherapies
         {
             get
             {
                 return _familytherapies;
             }
             set
             {
                 if (_familytherapies != value)
                 {
                     _familytherapies = value;
                     NotifyOfPropertyChange(()=>FamilyTherapies);
                 }
             }
         }

         #endregion
         private void SearchDrugs(int PageIndex,int PageSize)
         {
             GetCondition();
             Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
             IsLoading = true;
             int totalCount = 0;
             var t = new Thread(() =>
             {
                 using (var serviceFactory = new PharmacyDrugServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;
                     contract.BeginSearchRefDrugGenericDetails_Simple(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                     {

                         try
                         {
                             var results = contract.EndSearchRefDrugGenericDetails_Simple(out totalCount, asyncResult);
                             if (results != null)
                             {
                                  DrugsResearch.Clear();
                                 DrugsResearch.TotalItemCount = totalCount;
                                 foreach (RefGenericDrugDetail p in results)
                                 {
                                     DrugsResearch.Add(p);
                                 }
                                 NotifyOfPropertyChange(() => DrugsResearch);
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

         private void GetCondition()
         {
             if (SearchCriteria == null)
             {
                 SearchCriteria = new DrugSearchCriteria();
             }
             SearchCriteria.IsConsult = IsConsult;
             SearchCriteria.IsInsurance = IsInsurance;
         }

         private void GetFamilytherapies(long V_MedProductType)
         {
             Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
             IsLoading = true;
             var t = new Thread(() =>
             {
                 using (var serviceFactory = new PharmacyDrugServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;
                     contract.BeginGetFamilyTherapies(V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                     {

                         try
                         {
                             var results = contract.EndGetFamilyTherapies(asyncResult);
                             if (results != null)
                             {
                                 if (FamilyTherapies == null)
                                 {
                                     FamilyTherapies = new ObservableCollection<DrugClass>();
                                 }
                                 else
                                 {
                                     FamilyTherapies.Clear();
                                 }
                                 DrugClass ite = new DrugClass();
                                 ite.DrugClassID = 0;
                                 ite.FaName = ALLITEMS;
                                 FamilyTherapies.Add(ite);
                                 foreach (DrugClass p in results)
                                 {
                                     FamilyTherapies.Add(p);
                                 }
                                 NotifyOfPropertyChange(() => FamilyTherapies);
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

         public void Search(object sender, RoutedEventArgs e)
         {
             DrugsResearch.PageIndex = 0;
             SearchDrugs(0,DrugsResearch.PageSize);
         }

         public void txt_search_KeyUp(object sender, KeyEventArgs e)
         {
             if (e.Key == Key.Enter)
             {
                 DrugsResearch.PageIndex = 0;
                 SearchDrugs(0, DrugsResearch.PageSize);
             }
         }

         public void cbxFamilyTherapy_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             DrugsResearch.PageIndex = 0;
             SearchDrugs(0, DrugsResearch.PageSize);
         }

         public void griddrug_DblClick(object sender, Common.EventArgs<object> e)
         {
             Globals.EventAggregator.Publish(new PharmacyContraIndicatorEvent() { PharmacyContraIndicator = e.Value });
         }

         public void IsInsurance1_Checked(object sender, System.Windows.RoutedEventArgs e)
         {
             IsInsurance = (byte)Insurance.All;
         }

         public void IsInsurance2_Checked(object sender, System.Windows.RoutedEventArgs e)
         {
             IsInsurance = (byte)Insurance.Yes;
         }

         public void IsInsurance3_Checked(object sender, System.Windows.RoutedEventArgs e)
         {
             IsInsurance = (byte)Insurance.No;
         }

         public void IsConsult1_Checked(object sender, System.Windows.RoutedEventArgs e)
         {
             IsConsult = (byte)Consult.All;
         }

         public void IsConsult2_Checked(object sender, System.Windows.RoutedEventArgs e)
         {
             IsConsult = (byte)Consult.Yes;
         }

         public void IsConsult3_Checked(object sender, System.Windows.RoutedEventArgs e)
         {
             IsConsult = (byte)Consult.No;
         }
    }
}
