using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
 
    /// Mo ta trang thai cua 1 item trong database.
 
    public interface IDBRecordState
    {
        RecordState RecordState
        {
            get;
            set;
        }
    }
}
