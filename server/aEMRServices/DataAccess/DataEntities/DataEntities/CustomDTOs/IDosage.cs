using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public interface IDosage
    {
        string MDoseStr
        {
            get;
            set;
        }

        string ADoseStr
        {
            get;
            set;
        }

        string EDoseStr
        {
            get;
            set;
        }

        string NDoseStr
        {
            get;
            set;
        }

        Single MDose
        {
            get;
            set;
        }

        Single ADose
        {
            get;
            set;
        }

        Single EDose
        {
            get;
            set;
        }

        Single NDose
        {
            get;
            set;
        }
    }
}
