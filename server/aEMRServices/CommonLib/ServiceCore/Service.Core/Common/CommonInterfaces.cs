using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Service.Core.Common
{
    public interface IEntityState
    {
        EntityState EntityState
        {
            get;
            set;
        }

        bool IsChecked { get; set; }
        bool CanEdit { get; set; }
        bool CanDelete { get; set; }
    }
}