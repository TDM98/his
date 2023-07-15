using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;


namespace DataEntities
{
    public partial class VitalSign : EntityBase
    {
        public override bool Equals(object obj)
        {
            VitalSign cond = obj as VitalSign;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.VSignID == cond.VSignID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
