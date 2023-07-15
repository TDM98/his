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
using System.Threading;
using DataEntities;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(ISupplierAndProduct)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SupplierAndProductViewModel : Conductor<object>, ISupplierAndProduct
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SupplierAndProductViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;


            var uc1 = Globals.GetViewModel<IAddSupplierForProduct>();
            AddSupplierForDrug = uc1;
            ActivateItem(AddSupplierForDrug);

            var uc2 = Globals.GetViewModel<IAddProductForSupplier>();
            AddDrugForSupplier = uc2;
            ActivateItem(AddDrugForSupplier);
        }
        public object AddSupplierForDrug
        {
            get;
            set;
        }
        public object AddDrugForSupplier
        {
            get;
            set;
        }
    }
}
