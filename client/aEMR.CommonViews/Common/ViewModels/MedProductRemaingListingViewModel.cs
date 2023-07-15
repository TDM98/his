using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IMedProductRemaingListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedProductRemaingListingViewModel : Conductor<object>, IMedProductRemaingListing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MedProductRemaingListingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        private ObservableCollection<RefGenMedProductDetails> _medProductList;

        public ObservableCollection<RefGenMedProductDetails> MedProductList
        {
            get { return _medProductList; }
            set
            {
                _medProductList = value;
                NotifyOfPropertyChange(() => MedProductList);
            }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    NotifyOfPropertyChange(() => Title);
                }
            }
        }
        
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (!_isLoading)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        private void LoadData(Dictionary<long, List<long>> drugIdList)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetGenMedProductsRemainingInStore(drugIdList, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<RefGenMedProductDetails> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetGenMedProductsRemainingInStore(asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }
                            finally
                            {
                                if (bOK)
                                {
                                    MedProductList = new ObservableCollection<RefGenMedProductDetails>(allItems);
                                }
                                if (error != null)
                                {
                                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                                }
                            }
                           
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsLoading = false;
                    if (error != null)
                    {
                        Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                    }
                }
               
            });
            t.Start();
        }

        public void StartLoadingByIdList(Dictionary<long, List<long>> drugIdList)
        {
            LoadData(drugIdList);
        }

        public void ClearItems()
        {
            MedProductList = new ObservableCollection<RefGenMedProductDetails>();
        }

        public void CloseCmd()
        {
            TryClose();
        }
    }
}