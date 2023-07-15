using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class ChainMedicalService : NotifyChangedBase, IEditableObject
    {
        public ChainMedicalService()
            : base()
        {

        }

        private ChainMedicalService _tempChainMedicalService;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempChainMedicalService = (ChainMedicalService)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempChainMedicalService)
                CopyFrom(_tempChainMedicalService);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(ChainMedicalService p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method

        /// Create a new ChainMedicalService object.

        /// <param name="chainID">Initial value of the ChainID property.</param>
        public static ChainMedicalService CreateChainMedicalService(Int64 chainID)
        {
            ChainMedicalService chainMedicalService = new ChainMedicalService();
            chainMedicalService.ChainID = chainID;
            return chainMedicalService;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public Int64 ChainID
        {
            get
            {
                return _ChainID;
            }
            set
            {
                if (_ChainID != value)
                {
                    OnChainIDChanging(value);
                    _ChainID = value;
                    RaisePropertyChanged("ChainID");
                    OnChainIDChanged();
                }
            }
        }
        private Int64 _ChainID;
        partial void OnChainIDChanging(Int64 value);
        partial void OnChainIDChanged();


        [DataMemberAttribute()]
        public Nullable<long> MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                OnMedServiceIDChanging(value);
                _MedServiceID = value;
                RaisePropertyChanged("MedServiceID");
                OnMedServiceIDChanged();
            }
        }
        private Nullable<long> _MedServiceID;
        partial void OnMedServiceIDChanging(Nullable<long> value);
        partial void OnMedServiceIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> MedSPackageID
        {
            get
            {
                return _MedSPackageID;
            }
            set
            {
                OnMedSPackageIDChanging(value);
                _MedSPackageID = value;
                RaisePropertyChanged("MedSPackageID");
                OnMedSPackageIDChanged();
            }
        }
        private Nullable<long> _MedSPackageID;
        partial void OnMedSPackageIDChanging(Nullable<long> value);
        partial void OnMedSPackageIDChanged();

        [DataMemberAttribute()]
        public Nullable<Byte> Idx
        {
            get
            {
                return _Idx;
            }
            set
            {
                OnIdxChanging(value);
                _Idx = value;
                RaisePropertyChanged("Idx");
                OnIdxChanged();
            }
        }
        private Nullable<Byte> _Idx;
        partial void OnIdxChanging(Nullable<Byte> value);
        partial void OnIdxChanged();

        [DataMemberAttribute()]
        public Nullable<Double> Weight
        {
            get
            {
                return _Weight;
            }
            set
            {
                OnWeightChanging(value);
                _Weight = value;
                RaisePropertyChanged("Weight");
                OnWeightChanged();
            }
        }
        private Nullable<Double> _Weight;
        partial void OnWeightChanging(Nullable<Double> value);
        partial void OnWeightChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public RefMedicalServiceItem RefMedicalServiceItem
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public RefMedicalServicePackage RefMedicalServicePackage
        {
            get;
            set;
        }



        #endregion
    }
}
