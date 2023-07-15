using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;


namespace DataEntities
{
    public partial class ChemicalResource : NotifyChangedBase, IEditableObject
    {
        public ChemicalResource()
            : base()
        {

        }

        private ChemicalResource _tempChemicalResource;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempChemicalResource = (ChemicalResource)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempChemicalResource)
                CopyFrom(_tempChemicalResource);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ChemicalResource p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new ChemicalResource object.

        /// <param name="rscrID">Initial value of the RscrID property.</param>
        public static ChemicalResource CreateChemicalResource(Int64 rscrID)
        {
            ChemicalResource chemicalResource = new ChemicalResource();
            chemicalResource.RscrID = rscrID;
            return chemicalResource;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 RscrID
        {
            get
            {
                return _RscrID;
            }
            set
            {
                if (_RscrID != value)
                {
                    OnRscrIDChanging(value);
                    _RscrID = value;
                    RaisePropertyChanged("RscrID");
                    OnRscrIDChanged();
                }
            }
        }
        private Int64 _RscrID;
        partial void OnRscrIDChanging(Int64 value);
        partial void OnRscrIDChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> V_OrganismsType
        {
            get
            {
                return _V_OrganismsType;
            }
            set
            {
                OnV_OrganismsTypeChanging(value);
                _V_OrganismsType = value;
                RaisePropertyChanged("V_OrganismsType");
                OnV_OrganismsTypeChanged();
            }
        }
        private Nullable<Int64> _V_OrganismsType;
        partial void OnV_OrganismsTypeChanging(Nullable<Int64> value);
        partial void OnV_OrganismsTypeChanged();





        [DataMemberAttribute()]
        public Nullable<Int64> V_Level
        {
            get
            {
                return _V_Level;
            }
            set
            {
                OnV_LevelChanging(value);
                _V_Level = value;
                RaisePropertyChanged("V_Level");
                OnV_LevelChanged();
            }
        }
        private Nullable<Int64> _V_Level;
        partial void OnV_LevelChanging(Nullable<Int64> value);
        partial void OnV_LevelChanged();





        [DataMemberAttribute()]
        public String Dosage
        {
            get
            {
                return _Dosage;
            }
            set
            {
                OnDosageChanging(value);
                _Dosage = value;
                RaisePropertyChanged("Dosage");
                OnDosageChanged();
            }
        }
        private String _Dosage;
        partial void OnDosageChanging(String value);
        partial void OnDosageChanged();



        [DataMemberAttribute()]
        public String Usage
        {
            get
            {
                return _Usage;
            }
            set
            {
                OnUsageChanging(value);
                _Usage = value;
                RaisePropertyChanged("Usage");
                OnUsageChanged();
            }
        }
        private String _Usage;
        partial void OnUsageChanging(String value);
        partial void OnUsageChanged();





        [DataMemberAttribute()]
        public String Notes
        {
            get
            {
                return _Notes;
            }
            set
            {
                OnNotesChanging(value);
                _Notes = value;
                RaisePropertyChanged("Notes");
                OnNotesChanged();
            }
        }
        private String _Notes;
        partial void OnNotesChanging(String value);
        partial void OnNotesChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public Resource Resource
        {
            get;
            set;
        }

        #endregion
    }
}
