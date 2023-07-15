using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using aEMR.CommonTasks;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using System.Linq;
using aEMR.Common.Collections;
using aEMR.Infrastructure;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(ILookupValueListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class LookupValueListingViewModel : Conductor<object>, ILookupValueListing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public LookupValueListingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            AddSelectOneItem = true;
            AddSelectedAllItem = false;
        }

        public void LoadData()
        {
            Coroutine.BeginExecute(LoadLookupValues());
        }

        private ObservableCollection<Lookup> _lookupList;

        public ObservableCollection<Lookup> LookupList
        {
            get { return _lookupList; }
            set { _lookupList = value;
            NotifyOfPropertyChange(() => LookupList);
            }
        }

        private Lookup _selectedItem;

        public Lookup SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value;
            NotifyOfPropertyChange(()=>SelectedItem);}
        }

        private LookupValues _type;

        public LookupValues Type
        {
            get { return _type; }
            set { _type = value;
            NotifyOfPropertyChange(() => Type);
            }
        }

        public bool AddSelectOneItem { get; set; }

        public bool AddSelectedAllItem { get; set; }

        private IEnumerator<IResult> LoadLookupValues()
        {
            var currencyTask = new LoadLookupListTask(Type);
            yield return currencyTask;
            if(currencyTask.Error == null)
            {
                LookupList = currencyTask.LookupList;
            }
            else
            {
                LookupList = null;
            }
            yield break;
        }


        public void SetSelectedID(long itemID)
        {
            if(LookupList != null)
            {
                Lookup foundItem = null;
                foreach (var item in LookupList)
                {
                    if(item.LookupID == itemID)
                    {
                        foundItem = item;
                        break;
                    }
                }
                SelectedItem = foundItem;
            }
        }
    }
}

