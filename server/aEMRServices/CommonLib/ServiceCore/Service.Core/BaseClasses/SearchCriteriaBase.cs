using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace eHCMS.Services.Core.Base
{
    /// <summary>
    /// Base class for all the classes implement INotifyPropertyChanged interface
    /// </summary>
    /// 

    public abstract class SearchCriteriaBase:NotifyChangedBase
    {
        public SearchCriteriaBase():base()
        {}
    }
}
