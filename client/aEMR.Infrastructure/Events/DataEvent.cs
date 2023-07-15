using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace aEMR.Infrastructure.Events
{
    public class DataEvent<T>
    {
        public EventTypeEnum EventType { get; set; }
        public T Data { get; set; }
    }
    public class UpdateItemEvent<T>
    {
        public EventTypeEnum EventType { get; set; }
        public T Data { get; set; }
    }

    public class AddDeleteItemEvent<T>
    {
        public ObservableCollection<T> newData { get; set; }
        public ObservableCollection<T> deleteData { get; set; }
    }

    public class SaveItemToInward<T>
    {
        public EventTypeEnum EventType { get; set; }
        public ObservableCollection<T> Data { get; set; }
    }

    public class UpdateBuyprice
    {       
        
    }

    public class FieldValueChange
    {
        
    }

    public class UpdateInventoryPriceEvent
    {
        
    }
}
