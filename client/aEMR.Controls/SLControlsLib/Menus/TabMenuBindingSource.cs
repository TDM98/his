
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace aEMR.Controls
{
    public class TabMenuBindingSource : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public TabMenuBindingSource() { }
        public TabMenuBindingSource(string headerText, string navigateURL, string targetName)
        {
            _headerText = headerText;
            _navigateURL = navigateURL;
            _targetName = targetName;
        }
        public TabMenuBindingSource(string headerText, string navigateURL, string targetName, TabMenuBindingSource parent)
        {
            _headerText = headerText;
            _navigateURL = navigateURL;
            _targetName = targetName;
            _parent = parent;
        }
        private string _headerText;
        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                if (_headerText != value)
                {
                    _headerText = value;
                    OnPropertyChanged("HeaderText");
                }
            }
        }
        private string _navigateURL;
        public string NavigateURL
        {
            get { return _navigateURL; }
            set
            {
                if (_navigateURL != value)
                {
                    _navigateURL = value;
                    OnPropertyChanged("NavigateURL");
                }
            }
        }
        private string _targetName;
        public string TargetName
        {
            get { return _targetName; }
            set
            {
                if (_targetName != value)
                {
                    _targetName = value;
                    OnPropertyChanged("TargetName");
                }
            }
        }
        private ObservableCollection<TabMenuBindingSource> _items;
        public IEnumerable<TabMenuBindingSource> Items
        {
            get { return _items; }
        }

        private TabMenuBindingSource _parent;
        public TabMenuBindingSource Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    OnPropertyChanged("Parent");
                }
            }
        }

        public void AddItem(TabMenuBindingSource source)
        {
            if (_items == null)
                _items = new ObservableCollection<TabMenuBindingSource>();
            _items.Add(source);
            OnPropertyChanged("Items");
        }

        public bool RemoveItem(TabMenuBindingSource source)
        {
            bool hasBeenRemoved = false;
            if (_items != null)
            {
                hasBeenRemoved = _items.Remove(source);
                if (hasBeenRemoved)
                    OnPropertyChanged("Items");
            }
            return hasBeenRemoved;
        }

        public void Clear()
        {
            if (_items != null && _items.Count > 0)
            {
                _items.Clear();
                OnPropertyChanged("Items");
            }
        }

        public void Insert(int index, TabMenuBindingSource item)
        {
            if (_items == null)
                _items = new ObservableCollection<TabMenuBindingSource>();
            _items.Insert(index, item);
            OnPropertyChanged("Items");
        }

        public override string ToString()
        {
            return _headerText;
        }
    }
}
