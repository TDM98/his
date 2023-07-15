using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    /// <summary>
    /// Hiển thị danh sách các item đã đăng ký của một bill trong đăng ký nội trú.
    /// Không có chi tiết của billing invoice.
    /// </summary>
    [Export(typeof(IInPatientBedRegDetailListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientBedRegDetailListingViewModel : Conductor<object>, IInPatientBedRegDetailListing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InPatientBedRegDetailListingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }


        private ObservableCollection<BedPatientRegDetail> _allItems;

        public ObservableCollection<BedPatientRegDetail> AllItems
        {
            get { return _allItems; }
            set
            {
                _allItems = value;
                NotifyOfPropertyChange(() => AllItems);
            }
        }

        private long _bedPatientId;

        public long BedPatientID
        {
            get { return _bedPatientId; }
            set
            {
                _bedPatientId = value;
                NotifyOfPropertyChange(() => BedPatientID);
                LoadData();
            }
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }

        public void LoadData()
        {
            if (BedPatientID <= 0)
            {
                return;
            }

            var t = new Thread(() =>
            {
                IsLoading = true;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllBedPatientRegDetailsByBedPatientID(BedPatientID, false,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var allBedPatientRegDetails = contract.EndGetAllBedPatientRegDetailsByBedPatientID(asyncResult);

                                    if (allBedPatientRegDetails != null)
                                    {
                                        AllItems = allBedPatientRegDetails.ToObservableCollection();
                                    }
                                    else
                                    {
                                        AllItems = null;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                finally
                                {
                                    IsLoading = false;
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    IsLoading = false;
                }
            });
            t.Start();
        }
    }

}