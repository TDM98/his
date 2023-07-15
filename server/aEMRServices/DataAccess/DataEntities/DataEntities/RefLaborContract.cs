using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefLaborContract : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefLaborContract object.

        /// <param name="lContracID">Initial value of the LContracID property.</param>
        /// <param name="laborContractName">Initial value of the LaborContractName property.</param>
        public static RefLaborContract CreateRefLaborContract(long lContracID, String laborContractName)
        {
            RefLaborContract refLaborContract = new RefLaborContract();
            refLaborContract.LContracID = lContracID;
            refLaborContract.LaborContractName = laborContractName;
            return refLaborContract;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long LContracID
        {
            get
            {
                return _LContracID;
            }
            set
            {
                if (_LContracID != value)
                {
                    OnLContracIDChanging(value);
                    ////ReportPropertyChanging("LContracID");
                    _LContracID = value;
                    RaisePropertyChanged("LContracID");
                    OnLContracIDChanged();
                }
            }
        }
        private long _LContracID;
        partial void OnLContracIDChanging(long value);
        partial void OnLContracIDChanged();





        [DataMemberAttribute()]
        public String LaborContractName
        {
            get
            {
                return _LaborContractName;
            }
            set
            {
                OnLaborContractNameChanging(value);
                ////ReportPropertyChanging("LaborContractName");
                _LaborContractName = value;
                RaisePropertyChanged("LaborContractName");
                OnLaborContractNameChanged();
            }
        }
        private String _LaborContractName;
        partial void OnLaborContractNameChanging(String value);
        partial void OnLaborContractNameChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_STAFFCON_REL_HR14_REFLABOR", "StaffContract")]
        public ObservableCollection<StaffContract> StaffContracts
        {
            get;
            set;
        }

        #endregion
    }
}
