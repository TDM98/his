using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

/* Create By : Nguyen Lin Ny
   Date : 2/12/2010
 */
namespace DataEntities
{
    public class ChooseDose:NotifyChangedBase
    {
        private int _ID;
        private String _Name;
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (_ID != value)
                {
                    _ID = value;
                    RaisePropertyChanged("ID");
                }
            }
        }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public override bool Equals(object obj)
        {
            ChooseDose seletedStore = obj as ChooseDose;
            if (seletedStore == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.ID == seletedStore.ID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
