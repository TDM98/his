using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class ArchitecturalResource : NotifyChangedBase, IEditableObject
    {
        public ArchitecturalResource()
            : base()
        {

        }

        private ArchitecturalResource _tempArchitecturalResource;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempArchitecturalResource = (ArchitecturalResource)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempArchitecturalResource)
                CopyFrom(_tempArchitecturalResource);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ArchitecturalResource p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new ArchitecturalResource object.

        /// <param name="pARescrID">Initial value of the PARescrID property.</param>
        /// <param name="physicalZone">Initial value of the PhysicalZone property.</param>
        public static ArchitecturalResource CreateArchitecturalResource(Int64 pARescrID, String physicalZone)
        {
            ArchitecturalResource architecturalResource = new ArchitecturalResource();
            architecturalResource.PARescrID = pARescrID;
            architecturalResource.PhysicalZone = physicalZone;
            return architecturalResource;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 PARescrID
        {
            get
            {
                return _PARescrID;
            }
            set
            {
                if (_PARescrID != value)
                {
                    OnPARescrIDChanging(value);
                    _PARescrID = value;
                    RaisePropertyChanged("PARescrID");
                    OnPARescrIDChanged();
                }
            }
        }
        private Int64 _PARescrID;
        partial void OnPARescrIDChanging(Int64 value);
        partial void OnPARescrIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> ParPARescrID
        {
            get
            {
                return _ParPARescrID;
            }
            set
            {
                OnParPARescrIDChanging(value);
                _ParPARescrID = value;
                RaisePropertyChanged("ParPARescrID");
                OnParPARescrIDChanged();
            }
        }
        private Nullable<Int64> _ParPARescrID;
        partial void OnParPARescrIDChanging(Nullable<Int64> value);
        partial void OnParPARescrIDChanged();





        [DataMemberAttribute()]
        public String PhysicalZone
        {
            get
            {
                return _PhysicalZone;
            }
            set
            {
                OnPhysicalZoneChanging(value);
                _PhysicalZone = value;
                RaisePropertyChanged("PhysicalZone");
                OnPhysicalZoneChanged();
            }
        }
        private String _PhysicalZone;
        partial void OnPhysicalZoneChanging(String value);
        partial void OnPhysicalZoneChanged();





        [DataMemberAttribute()]
        public String PhysicalBuilding
        {
            get
            {
                return _PhysicalBuilding;
            }
            set
            {
                OnPhysicalBuildingChanging(value);
                _PhysicalBuilding = value;
                RaisePropertyChanged("PhysicalBuilding");
                OnPhysicalBuildingChanged();
            }
        }
        private String _PhysicalBuilding;
        partial void OnPhysicalBuildingChanging(String value);
        partial void OnPhysicalBuildingChanged();





        [DataMemberAttribute()]
        public String PhysicalRoom
        {
            get
            {
                return _PhysicalRoom;
            }
            set
            {
                OnPhysicalRoomChanging(value);
                _PhysicalRoom = value;
                RaisePropertyChanged("PhysicalRoom");
                OnPhysicalRoomChanged();
            }
        }
        private String _PhysicalRoom;
        partial void OnPhysicalRoomChanging(String value);
        partial void OnPhysicalRoomChanged();





        [DataMemberAttribute()]
        public String PhysicalAddress
        {
            get
            {
                return _PhysicalAddress;
            }
            set
            {
                OnPhysicalAddressChanging(value);
                _PhysicalAddress = value;
                RaisePropertyChanged("PhysicalAddress");
                OnPhysicalAddressChanged();
            }
        }
        private String _PhysicalAddress;
        partial void OnPhysicalAddressChanging(String value);
        partial void OnPhysicalAddressChanged();





        [DataMemberAttribute()]
        public Nullable<Double> PDepreciationRate
        {
            get
            {
                return _PDepreciationRate;
            }
            set
            {
                OnPDepreciationRateChanging(value);
                _PDepreciationRate = value;
                RaisePropertyChanged("PDepreciationRate");
                OnPDepreciationRateChanged();
            }
        }
        private Nullable<Double> _PDepreciationRate;
        partial void OnPDepreciationRateChanging(Nullable<Double> value);
        partial void OnPDepreciationRateChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<Allocation> Allocations
        {
            get;
            set;
        }

        #endregion
    }
}
