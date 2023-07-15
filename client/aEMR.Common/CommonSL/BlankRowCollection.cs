using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using eHCMS.Services.Core.Base;

namespace aEMR.Common.Collections
{
    //public class BlankRowCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged, new()
    public class BlankRowCollection<T> : ObservableCollection<T> where T : EntityBase, new()
    {
        private T t;
        public BlankRowCollection()
        {
            t = new T();
            base.InsertItem(0, t);
            t.PropertyChanged += t_PropertyChanged;
        }

        public BlankRowCollection(IEnumerable<T> collection)
        {
            if(collection != null)
            {
                foreach (T item in collection)
                {
                    this.Add(item);
                }
            }
            t = new T();
            base.InsertItem(this.Items.Count, t);
            t.PropertyChanged += t_PropertyChanged;
        }

        protected override void InsertItem(int index, T item)
        {
            if (index == this.Items.Count)
            {
                index = this.Items.Count - 1;
            }
            if (index < 0)
            {
                index = 0;
            }
            base.InsertItem(index, item);
            FireRealItemsChanged();
        }

        void t_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            t.PropertyChanged -= t_PropertyChanged;
            t = new T();
            t.PropertyChanged += t_PropertyChanged;
            base.InsertItem(this.Items.Count, t);
            FireRealItemsChanged();
        }
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            FireRealItemsChanged();
        }

        public IEnumerable<T> RealItems
        {
            get
            {
                for (int i = 0; i < this.Items.Count - 1; i++)
                {
                    yield return this.Items[i];
                }
            }
        }
        void FireRealItemsChanged()
        {
            base.OnPropertyChanged(new PropertyChangedEventArgs("RealItems"));
        }
    }
}
