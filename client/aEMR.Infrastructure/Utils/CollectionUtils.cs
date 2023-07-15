using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace aEMR.Infrastructure.Utils
{
    public static class CollectionUtils
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
        {
            if (enumerableList != null)
            {
                // Create an emtpy observable collection object
                var observableCollection = new ObservableCollection<T>();

                // Loop through all the records and add to observable collection object
                foreach (var item in enumerableList)
                {
                    observableCollection.Add(item);
                }

                // Return the populated observable collection
                return observableCollection;
            }
            return null;
        }
    }
}
