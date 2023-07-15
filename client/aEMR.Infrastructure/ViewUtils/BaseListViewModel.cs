using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Castle.Windsor;

namespace aEMR.Infrastructure.ViewUtils
{
    public abstract class BaseListViewModel<T> : CommonView<T> where T : class
    {
        protected BaseListViewModel(IWindsorContainer container) : base(container)
        {
        }

        public IList<AdapterView<T>> SelectedItems { get; set; }

        private string _searchText;
        public virtual string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                NotifyOfPropertyChange(() => SearchText);
            }
        }

        private ObservableCollection<AdapterView<T>> _adapterViews;
        public ObservableCollection<AdapterView<T>> AdapterViews
        {
            get { return _adapterViews; }
            set
            {
                _adapterViews = value;
                NotifyOfPropertyChange(() => AdapterViews);
            }
        }

        public CheckBox MasterChecked { get; set; }

        public abstract void NewCmd();
        public abstract void DeleteCmd();
        public abstract void SearchCmd();
        public abstract void EditCmd();
        public virtual void ClearSearchCmd() { }

        private AdapterView<T> _selectedItem;
        public AdapterView<T> SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        public virtual void MasterCheck(object source, object eventArgs)
        {
            var chkSource = source as CheckBox;

            MasterChecked = chkSource;

            if (chkSource != null)
            {
                bool isChecked = chkSource.IsChecked.HasValue && chkSource.IsChecked.Value;

                foreach (var item in AdapterViews)
                {
                    item.IsSelected = isChecked;
                    SelectedItems.Add(item);
                }

                if (!isChecked)
                {
                    SelectedItems.Clear();
                }
            }
        }

        public virtual void SingleCheck(object source, AdapterView<T> dataItem)
        {
            if (MasterChecked != null)
            {
                MasterChecked.IsChecked = false;
            }

            var chkSource = source as CheckBox;
            if (chkSource != null
                && chkSource.IsChecked.HasValue
                && chkSource.IsChecked.Value)
            {
                SelectedItems.Add(dataItem);
                if (SelectedItems.Count == AdapterViews.Count)
                {
                    if (MasterChecked != null)
                    {
                        MasterChecked.IsChecked = true;
                    }
                }
            }
            else
            {
                if (dataItem != null && SelectedItems.Contains(dataItem))
                {
                    SelectedItems.Remove(dataItem);
                }
            }
        }

    }
}
