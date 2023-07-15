using eHCMSLanguage;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IAddDrugContrain)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddDrugContrainViewModel : Conductor<object>, IAddDrugContrain, IHandle<PharmacyContraIndicatorEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public AddDrugContrainViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            var RefGenDrugListVM = Globals.GetViewModel<IRefGenDrugListEx>();
            DrugList = RefGenDrugListVM;
            this.ActivateItem(RefGenDrugListVM);
            _allNewRefGenericDrugDetail=new ObservableCollection<RefGenericDrugDetail>();

        }

        public object DrugList { get; set; }

        //protected override void OnActivate()
        //{
        //    base.OnActivate();
        //    _allNewRefGenericDrugDetail = new ObservableCollection<RefGenericDrugDetail>();
        //    Globals.EventAggregator.Subscribe(this);
        //}

        //protected override void OnDeactivate(bool close)
        //{
        //    base.OnDeactivate(close);
        //    Globals.EventAggregator.Unsubscribe(this);
        //}
#region properties

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


        private ObservableCollection<RefGenericDrugDetail> _allNewRefGenericDrugDetail;
        public ObservableCollection<RefGenericDrugDetail> allNewRefGenericDrugDetail
        {
            get
            {
                return _allNewRefGenericDrugDetail;
            }
            set
            {
                if (_allNewRefGenericDrugDetail == value)
                    return;
                _allNewRefGenericDrugDetail = value;
                NotifyOfPropertyChange(() => allNewRefGenericDrugDetail);
            }
        }
#endregion
        public void butSave()
        {
            if (allNewRefGenericDrugDetail.Count > 0)
            {
                Globals.EventAggregator.Publish(new PharmacyContraIndicatorAddEvent{PharmacyContraIndicatorAdd = allNewRefGenericDrugDetail});
                TryClose();
            }else
            {
                Globals.ShowMessage(eHCMSResources.Z1601_G1_ChonThuocCCD, "");
            }
        }

        public void butExit()
        {
            TryClose();
        }
        public void Handle(PharmacyContraIndicatorEvent obj)
        {
            if(obj!=null)
            {
                allNewRefGenericDrugDetail.Add((RefGenericDrugDetail)obj.PharmacyContraIndicator);
            }
        }
    }
}
