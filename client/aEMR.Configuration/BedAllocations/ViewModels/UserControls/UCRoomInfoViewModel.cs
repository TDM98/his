using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Configuration.BedAllocations.ViewModels
{
    [Export(typeof(IUCRoomInfo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UCRoomInfoViewModel : Conductor<object>, IUCRoomInfo, IHandle<DeptLocBedSelectedEvent>
    {

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UCRoomInfoViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

        }
        protected override void OnActivate()
        {
            base.OnActivate();
            GetAllBedLocType();
            Globals.EventAggregator.Subscribe(this);
        }

        private RefDepartmentsTree _SeletedRefDepartmentsTree;
        public RefDepartmentsTree SeletedRefDepartmentsTree
        {
            get
            {
                return _SeletedRefDepartmentsTree;
            }
            set
            {
                if (_SeletedRefDepartmentsTree == value)
                    return;
                _SeletedRefDepartmentsTree = value;
                NotifyOfPropertyChange(() => SeletedRefDepartmentsTree);
            }
        }

        public void Handle(DeptLocBedSelectedEvent obj)
        {
            if (obj != null)
            {
                SeletedRefDepartmentsTree=(RefDepartmentsTree)obj.curDeptLoc;
            }
        }

        public ObservableCollection<BedAllocType> _allBedAllocType;
        public ObservableCollection<BedAllocType> allBedAllocType
        {
            get
            {
                return _allBedAllocType;
            }
            set
            {
                if (_allBedAllocType == value)
                    return;
                _allBedAllocType = value;
                NotifyOfPropertyChange(() => allBedAllocType);
            }
        }
        private void GetAllBedLocType()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                IsLoading=true;

                using (var serviceFactory = new BedAllocationsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllBedLocType( Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllBedLocType(asyncResult);
                            if (results != null)
                            {
                                if (allBedAllocType == null)
                                {
                                    allBedAllocType = new ObservableCollection<BedAllocType>();
                                }
                                else
                                {
                                    allBedAllocType.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allBedAllocType.Add(p);
                                }
                                NotifyOfPropertyChange(() => allBedAllocType);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
    }
}
