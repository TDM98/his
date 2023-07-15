using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class RefMedicalServicePackage : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefMedicalServicePackage object.

        /// <param name="medSPackageID">Initial value of the MedSPackageID property.</param>
        /// <param name="medSPackageCode">Initial value of the MedSPackageCode property.</param>
        /// <param name="medSPackageName">Initial value of the MedSPackageName property.</param>
        /// <param name="isBreakDown">Initial value of the IsBreakDown property.</param>
        public static RefMedicalServicePackage CreateRefMedicalServicePackage(long medSPackageID, String medSPackageCode, String medSPackageName, Boolean isBreakDown)
        {
            RefMedicalServicePackage refMedicalServicePackage = new RefMedicalServicePackage();
            refMedicalServicePackage.MedSPackageID = medSPackageID;
            refMedicalServicePackage.MedSPackageCode = medSPackageCode;
            refMedicalServicePackage.MedSPackageName = medSPackageName;
            refMedicalServicePackage.IsBreakDown = isBreakDown;
            return refMedicalServicePackage;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long MedSPackageID
        {
            get
            {
                return _MedSPackageID;
            }
            set
            {
                if (_MedSPackageID != value)
                {
                    OnMedSPackageIDChanging(value);
                    ////ReportPropertyChanging("MedSPackageID");
                    _MedSPackageID = value;
                    RaisePropertyChanged("MedSPackageID");
                    OnMedSPackageIDChanged();
                }
            }
        }
        private long _MedSPackageID;
        partial void OnMedSPackageIDChanging(long value);
        partial void OnMedSPackageIDChanged();





        [DataMemberAttribute()]
        public String MedSPackageCode
        {
            get
            {
                return _MedSPackageCode;
            }
            set
            {
                OnMedSPackageCodeChanging(value);
                ////ReportPropertyChanging("MedSPackageCode");
                _MedSPackageCode = value;
                RaisePropertyChanged("MedSPackageCode");
                OnMedSPackageCodeChanged();
            }
        }
        private String _MedSPackageCode;
        partial void OnMedSPackageCodeChanging(String value);
        partial void OnMedSPackageCodeChanged();





        [DataMemberAttribute()]
        public String MedSPackageName
        {
            get
            {
                return _MedSPackageName;
            }
            set
            {
                OnMedSPackageNameChanging(value);
                ////ReportPropertyChanging("MedSPackageName");
                _MedSPackageName = value;
                RaisePropertyChanged("MedSPackageName");
                OnMedSPackageNameChanged();
            }
        }
        private String _MedSPackageName;
        partial void OnMedSPackageNameChanging(String value);
        partial void OnMedSPackageNameChanged();





        [DataMemberAttribute()]
        public Boolean IsBreakDown
        {
            get
            {
                return _IsBreakDown;
            }
            set
            {
                OnIsBreakDownChanging(value);
                ////ReportPropertyChanging("IsBreakDown");
                _IsBreakDown = value;
                RaisePropertyChanged("IsBreakDown");
                OnIsBreakDownChanged();
            }
        }
        private Boolean _IsBreakDown;
        partial void OnIsBreakDownChanging(Boolean value);
        partial void OnIsBreakDownChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_CMEDSVR_REL_HOSFM_REFMEDSPAC", "ChainMedicalServices")]
        public ObservableCollection<ChainMedicalService> ChainMedicalServices
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PROMOTIO_REL_HOSFM_REFMEDIC", "PromotionalServices")]
        public ObservableCollection<PromotionalService> PromotionalServices
        {
            get;
            set;
        }

        #endregion
    }
}
