using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using Castle.Windsor;
using aEMR.Common.Utilities;
using aEMR.Infrastructure.Utils;

namespace aEMR.Infrastructure.ViewUtils
{
    public abstract class MovingItemBetweenTwoListboxesViewModel<T> : CommonView<T>, IMovingItemBetweenTwoListboxesViewModel<T> where T : class 
    {
        protected MovingItemBetweenTwoListboxesViewModel(IWindsorContainer container)
            : base(container)
        {
        }

        private ObservableCollection<T> _assignedItems;
        private ObservableCollection<T> _unAssignedItems;
        private List<T> _allItems;
        private T _assignedListSelectedItem;
        private T _unAssignedListSelectedItem;

        public List<T> AllItems
        {
            get { return _allItems; }
            set
            {
                _allItems = value;
                if(_allItems == null)
                {
                    _allItems = new List<T>();
                }
                CalcUnAssignedItems();
            }
        }

        public ObservableCollection<T> UnAssignedItems
        {
            get { return _unAssignedItems; }
            private set
            {
                if (Equals(value, _unAssignedItems)) return;
                _unAssignedItems = value;
                NotifyOfPropertyChange(() => UnAssignedItems);
            }
        }

        public ObservableCollection<T> AssignedItems
        {
            get { return _assignedItems; }
            set
            {
                _assignedItems = value;
                if (_assignedItems == null)
                {
                    _assignedItems = new ObservableCollection<T>();
                }
                NotifyOfPropertyChange(() => AssignedItems);

                CalcUnAssignedItems();
            }
        }

        public T AssignedListSelectedItem
        {
            get { return _assignedListSelectedItem; }
            set
            {
                if (Equals(value, _assignedListSelectedItem)) return;
                _assignedListSelectedItem = value;
                NotifyOfPropertyChange(() => AssignedListSelectedItem);
            }
        }

        public T UnAssignedListSelectedItem
        {
            get { return _unAssignedListSelectedItem; }
            set
            {
                if (Equals(value, _unAssignedListSelectedItem)) return;
                _unAssignedListSelectedItem = value;
                NotifyOfPropertyChange(() => UnAssignedListSelectedItem);
            }
        }

        public IEqualityComparer<T> ItemComparer { get; set; }

        private void CalcUnAssignedItems()
        {
            if (AllItems == null)
            {
                UnAssignedItems = new ObservableCollection<T>();
                return;
            }
            if (_assignedItems == null)
            {
                UnAssignedItems = new ObservableCollection<T>(AllItems);
                return;
            }
            UnAssignedItems = AllItems.Except(_assignedItems, ItemComparer).ToObservableCollection();    
        }

        public override void Initial()
        {

        }


        public virtual void AddAllCmd()
        {
            if(AssignedItems == null)
            {
                AssignedItems = UnAssignedItems;
            }
            else 
            {
                if(UnAssignedItems != null)
                {
                    AssignedItems = AssignedItems.Union(UnAssignedItems, ItemComparer).ToObservableCollection(); 
                }
            }
            UnAssignedItems = new ObservableCollection<T>();
        }

        public virtual void AddCmd()
        {
            if (UnAssignedListSelectedItem != null)
            {
                AssignedItems.AddToListIfNotExist(UnAssignedListSelectedItem,ItemComparer);
                UnAssignedItems.Remove(UnAssignedListSelectedItem);
            }
        }

        
        public virtual void RemoveCmd()
        {
            if (AssignedListSelectedItem != null)
            {
                UnAssignedItems.AddToListIfNotExist(AssignedListSelectedItem);
                AssignedItems.Remove(AssignedListSelectedItem);
            }
        }

        public virtual void RemoveAllCmd()
        {
            if (AllItems != null)
            {
                UnAssignedItems = AllItems.ToObservableCollection(); 
            }
            else
            {
                UnAssignedItems = new ObservableCollection<T>();
            }
            AssignedItems = new ObservableCollection<T>();
        }
    }
}
