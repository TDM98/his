using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class SurgeryType : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new SurgeryType object.

        /// <param name="surgeryTypeCode">Initial value of the SurgeryTypeCode property.</param>
        public static SurgeryType CreateSurgeryType(String surgeryTypeCode)
        {
            SurgeryType surgeryType = new SurgeryType();
            surgeryType.SurgeryTypeCode = surgeryTypeCode;
            return surgeryType;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public String SurgeryTypeCode
        {
            get
            {
                return _SurgeryTypeCode;
            }
            set
            {
                if (_SurgeryTypeCode != value)
                {
                    OnSurgeryTypeCodeChanging(value);
                    ////ReportPropertyChanging("SurgeryTypeCode");
                    _SurgeryTypeCode = value;
                    RaisePropertyChanged("SurgeryTypeCode");
                    OnSurgeryTypeCodeChanged();
                }
            }
        }
        private String _SurgeryTypeCode;
        partial void OnSurgeryTypeCodeChanging(String value);
        partial void OnSurgeryTypeCodeChanged();





        [DataMemberAttribute()]
        public String SurgeryTypeName
        {
            get
            {
                return _SurgeryTypeName;
            }
            set
            {
                OnSurgeryTypeNameChanging(value);
                ////ReportPropertyChanging("SurgeryTypeName");
                _SurgeryTypeName = value;
                RaisePropertyChanged("SurgeryTypeName");
                OnSurgeryTypeNameChanged();
            }
        }
        private String _SurgeryTypeName;
        partial void OnSurgeryTypeNameChanging(String value);
        partial void OnSurgeryTypeNameChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SURGERYM_REL_PCMD0_SURGERYT", "SurgeryMethods")]
        public ObservableCollection<SurgeryMethod> SurgeryMethods
        {
            get;
            set;
        }

        #endregion
    }
}
