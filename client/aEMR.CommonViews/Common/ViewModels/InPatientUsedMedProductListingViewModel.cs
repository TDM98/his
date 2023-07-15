using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.DataContracts;
using Service.Core.Common;
using aEMR.Common.Collections;
using System.ComponentModel.DataAnnotations;
using aEMR.Common.DataValidation;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IInPatientUsedMedProductListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientUsedMedProductListingViewModel : Conductor<object>, IInPatientUsedMedProductListing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientUsedMedProductListingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }
        private AllLookupValues.MedProductType _medProductType = AllLookupValues.MedProductType.THUOC;
        public AllLookupValues.MedProductType MedProductType
        {
            get { return _medProductType; }
            set
            {
                if (_medProductType != value)
                {
                    _medProductType = value;
                    NotifyOfPropertyChange(() => MedProductType); 
                }
            }
        }

        private ObservableCollection<RefGenMedProductSummaryInfo> _allItems;

        public ObservableCollection<RefGenMedProductSummaryInfo> AllItems
        {
            get { return _allItems; }
            set
            {
                _allItems = value;
                NotifyOfPropertyChange(() => AllItems);
            }
        }

        private PatientRegistration _registration;

        public PatientRegistration Registration
        {
            get { return _registration; }
            set
            {
                if (_registration != value)
                {
                    _registration = value;
                    NotifyOfPropertyChange(() => Registration); 
                }
            }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }
        public void LoadData(long? DeptID)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetRefGenMedProductSummaryByRegistration(Registration.PtRegistrationID,_medProductType,DeptID,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    IList<RefGenMedProductSummaryInfo> items = contract.EndGetRefGenMedProductSummaryByRegistration(asyncResult);
                                    AllItems = items != null ? items.ToObservableCollection() : null;
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    error = new AxErrorEventArgs(ex);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsLoading = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = error });
                }
            });
            t.Start();
        }

        public bool ValidateInfo(out ObservableCollection<ValidationResult> validationResults)
        {
            bool bValid = true;
            validationResults = null;
            if (_allItems != null)
            {
                foreach (var item in _allItems)
                {
                    bValid = ValidationExtensions.ValidateObject(item, out validationResults);
                    if (!bValid)
                    {
                        break;
                    }
                }
            }

            return bValid;
        }

        //public void BeginningEdit(object source, DataGridBeginningEditEventArgs eventArgs)
        //{

        //}
        //public void CellEditEnded(object source, DataGridCellEditEndingEventArgs eventArgs)
        //{
        //    var item = eventArgs.Row.DataContext as RefGenMedProductSummaryInfo;

        //    if (item != null && eventArgs.Row.IsValid)
        //    {
        //        item.EntityState = EntityState.MODIFIED;
        //    }
        //}
        public List<RefGenMedProductSummaryInfo> GetModifiedItems()
        {
            var retVal = new List<RefGenMedProductSummaryInfo>();
            if (_allItems != null)
            {
                retVal.AddRange(_allItems.Where(item => item.EntityState == EntityState.MODIFIED));
            }
            return retVal;
        }
        public List<RefGenMedProductSummaryInfo> GetReturnItems()
        {
            var retVal = new List<RefGenMedProductSummaryInfo>();
            if (_allItems != null)
            {
                retVal.AddRange(_allItems.Where(item => item.EntityState == EntityState.MODIFIED && item.QtyReturn > 0));
            }
            return retVal;
        }
    }
}
