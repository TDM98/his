using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
    public partial class RefDisposableMedicalResource : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefDisposableMedicalResource object.

        /// <param name="dMedRscrID">Initial value of the DMedRscrID property.</param>
        /// <param name="dMedRscrName">Initial value of the DMedRscrName property.</param>
        /// <param name="dMedRscrNameBrand">Initial value of the DMedRscrNameBrand property.</param>
        /// <param name="dMedRscrFunctions">Initial value of the DMedRscrFunctions property.</param>
        /// <param name="dMedRscrContainerUnit">Initial value of the DMedRscrContainerUnit property.</param>
        /// <param name="dMedRscr_PiecesUnit">Initial value of the DMedRscr_PiecesUnit property.</param>
        /// <param name="packagingRecipe">Initial value of the PackagingRecipe property.</param>
        public static RefDisposableMedicalResource CreateRefDisposableMedicalResource(long dMedRscrID, String dMedRscrName, String dMedRscrNameBrand, String dMedRscrFunctions, String dMedRscrContainerUnit, Int32 dMedRscr_PiecesUnit, String packagingRecipe)
        {
            RefDisposableMedicalResource refDisposableMedicalResource = new RefDisposableMedicalResource();
            refDisposableMedicalResource.DMedRscrID = dMedRscrID;
            refDisposableMedicalResource.DMedRscrName = dMedRscrName;
            refDisposableMedicalResource.DMedRscrNameBrand = dMedRscrNameBrand;
            refDisposableMedicalResource.DMedRscrFunctions = dMedRscrFunctions;
            refDisposableMedicalResource.DMedRscrContainerUnit = dMedRscrContainerUnit;
            refDisposableMedicalResource.DMedRscr_PiecesUnit = dMedRscr_PiecesUnit;
            refDisposableMedicalResource.Packaging = packagingRecipe;
            return refDisposableMedicalResource;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long DMedRscrID
        {
            get
            {
                return _DMedRscrID;
            }
            set
            {
                if (_DMedRscrID != value)
                {
                    OnDMedRscrIDChanging(value);
                    _DMedRscrID = value;
                    RaisePropertyChanged("DMedRscrID");
                    OnDMedRscrIDChanged();
                }
            }
        }
        private long _DMedRscrID;
        partial void OnDMedRscrIDChanging(long value);
        partial void OnDMedRscrIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> DMedRscrTypeID
        {
            get
            {
                return _DMedRscrTypeID;
            }
            set
            {
                OnDMedRscrTypeIDChanging(value);
                _DMedRscrTypeID = value;
                RaisePropertyChanged("DMedRscrTypeID");
                OnDMedRscrTypeIDChanged();
            }
        }
        private Nullable<long> _DMedRscrTypeID;
        partial void OnDMedRscrTypeIDChanging(Nullable<long> value);
        partial void OnDMedRscrTypeIDChanged();

        [Required(ErrorMessage = "Nhập Tên Y Cụ!")]
        [StringLength(256, MinimumLength = 0, ErrorMessage = "Phải <= 256 Ký Tự")]
        [DataMemberAttribute()]
        public String DMedRscrName
        {
            get
            {
                return _DMedRscrName;
            }
            set
            {
                OnDMedRscrNameChanging(value);
                ValidateProperty("DMedRscrName", value);
                _DMedRscrName = value;
                RaisePropertyChanged("DMedRscrName");
                OnDMedRscrNameChanged();
            }
        }
        private String _DMedRscrName;
        partial void OnDMedRscrNameChanging(String value);
        partial void OnDMedRscrNameChanged();

        [DataMemberAttribute()]
        public String DMedRscrNameBrand
        {
            get
            {
                return _DMedRscrNameBrand;
            }
            set
            {
                OnDMedRscrNameBrandChanging(value);
                _DMedRscrNameBrand = value;
                RaisePropertyChanged("DMedRscrNameBrand");
                OnDMedRscrNameBrandChanged();
            }
        }
        private String _DMedRscrNameBrand;
        partial void OnDMedRscrNameBrandChanging(String value);
        partial void OnDMedRscrNameBrandChanged();

        [DataMemberAttribute()]
        public String DMedRscrFunctions
        {
            get
            {
                return _DMedRscrFunctions;
            }
            set
            {
                OnDMedRscrFunctionsChanging(value);
                _DMedRscrFunctions = value;
                RaisePropertyChanged("DMedRscrFunctions");
                OnDMedRscrFunctionsChanged();
            }
        }
        private String _DMedRscrFunctions;
        partial void OnDMedRscrFunctionsChanging(String value);
        partial void OnDMedRscrFunctionsChanged();

        [DataMemberAttribute()]
        public String DMedRscrTechInfo
        {
            get
            {
                return _DMedRscrTechInfo;
            }
            set
            {
                OnDMedRscrTechInfoChanging(value);
                _DMedRscrTechInfo = value;
                RaisePropertyChanged("DMedRscrTechInfo");
                OnDMedRscrTechInfoChanged();
            }
        }
        private String _DMedRscrTechInfo;
        partial void OnDMedRscrTechInfoChanging(String value);
        partial void OnDMedRscrTechInfoChanged();

        [DataMemberAttribute()]
        public String DMedRscrMaterial
        {
            get
            {
                return _DMedRscrMaterial;
            }
            set
            {
                OnDMedRscrMaterialChanging(value);
                _DMedRscrMaterial = value;
                RaisePropertyChanged("DMedRscrMaterial");
                OnDMedRscrMaterialChanged();
            }
        }
        private String _DMedRscrMaterial;
        partial void OnDMedRscrMaterialChanging(String value);
        partial void OnDMedRscrMaterialChanged();

        [DataMemberAttribute()]
        public String DMedRscrContainerUnit
        {
            get
            {
                return _DMedRscrContainerUnit;
            }
            set
            {
                OnDMedRscrContainerUnitChanging(value);
                _DMedRscrContainerUnit = value;
                RaisePropertyChanged("DMedRscrContainerUnit");
                OnDMedRscrContainerUnitChanged();
            }
        }
        private String _DMedRscrContainerUnit;
        partial void OnDMedRscrContainerUnitChanging(String value);
        partial void OnDMedRscrContainerUnitChanged();

        [DataMemberAttribute()]
        public Int32 DMedRscr_PiecesUnit
        {
            get
            {
                return _DMedRscr_PiecesUnit;
            }
            set
            {
                OnDMedRscr_PiecesUnitChanging(value);
                _DMedRscr_PiecesUnit = value;
                RaisePropertyChanged("DMedRscr_PiecesUnit");
                OnDMedRscr_PiecesUnitChanged();
            }
        }
        private Int32 _DMedRscr_PiecesUnit;
        partial void OnDMedRscr_PiecesUnitChanging(Int32 value);
        partial void OnDMedRscr_PiecesUnitChanged();

        [DataMemberAttribute()]
        public String Packaging
        {
            get
            {
                return _Packaging;
            }
            set
            {
                OnPackagingChanging(value);
                _Packaging = value;
                RaisePropertyChanged("Packaging");
                OnPackagingChanged();
            }
        }
        private String _Packaging;
        partial void OnPackagingChanging(String value);
        partial void OnPackagingChanged();

        [DataMemberAttribute()]
        public Nullable<Single> ProcessPeriod
        {
            get
            {
                return _ProcessPeriod;
            }
            set
            {
                OnProcessPeriodChanging(value);
                _ProcessPeriod = value;
                RaisePropertyChanged("ProcessPeriod");
                OnProcessPeriodChanged();
            }
        }
        private Nullable<Single> _ProcessPeriod;
        partial void OnProcessPeriodChanging(Nullable<Single> value);
        partial void OnProcessPeriodChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_TimeUnit
        {
            get
            {
                return _V_TimeUnit;
            }
            set
            {
                OnV_TimeUnitChanging(value);
                _V_TimeUnit = value;
                RaisePropertyChanged("V_TimeUnit");
                OnV_TimeUnitChanged();
            }
        }
        private Nullable<Int64> _V_TimeUnit;
        partial void OnV_TimeUnitChanging(Nullable<Int64> value);
        partial void OnV_TimeUnitChanged();

        [DataMemberAttribute()]
        public Nullable<Double> DefectPercentage
        {
            get
            {
                return _DefectPercentage;
            }
            set
            {
                OnDefectPercentageChanging(value);
                _DefectPercentage = value;
                RaisePropertyChanged("DefectPercentage");
                OnDefectPercentageChanged();
            }
        }
        private Nullable<Double> _DefectPercentage;
        partial void OnDefectPercentageChanging(Nullable<Double> value);
        partial void OnDefectPercentageChanged();

        #endregion

        #region Navigation Properties
        [DataMemberAttribute()]
        public DisposableMedicalResourceType DisposableMedicalResourceType
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<DMedRscrSellingPriceTable> DMedRscrSellingPriceTables
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<InwardDMedRscr> InwardDMedRscrs
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public PrescriptionDetail PrescriptionDetail
        {
            get;
            set;
        }

        #endregion
    }
}
