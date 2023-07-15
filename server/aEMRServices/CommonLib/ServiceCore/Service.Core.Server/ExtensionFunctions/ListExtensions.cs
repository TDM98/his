using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace eHCMS.Services.Core
{
    public static class ListExtensions
    {
        public static void AddRange_V1<T>(this List<T> root, IEnumerable<T> collection)
        {
            if (collection == null || root == null)
            {
                return;
            }
            foreach (var item in collection)
            {
                root.Add(item);
            }
        }

        public static void AddRange<T>(this List<T> root, params IEnumerable<T>[] collectionList)
        {
            if (collectionList == null)
            {
                return;
            }
            foreach (var collection in collectionList)
            {
                root.AddRange_V1(collection);
            }
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return null;
            }
            var retVal = new ObservableCollection<T>();
            foreach (var item in collection)
            {
                retVal.Add(item);
            }
            return retVal;
        }

    }
}
