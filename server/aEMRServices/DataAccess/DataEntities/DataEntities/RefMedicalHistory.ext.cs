using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefMedicalHistory : NotifyChangedBase
    {
        public override bool Equals(object obj)
        {
            RefMedicalHistory cond = obj as RefMedicalHistory;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.MedHistCode== cond.MedHistCode;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
