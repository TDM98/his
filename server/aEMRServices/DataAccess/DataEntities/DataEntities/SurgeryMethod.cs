using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class SurgeryMethod : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new SurgeryMethod object.

        /// <param name="sMethodCode">Initial value of the SMethodCode property.</param>
        /// <param name="sMethodName">Initial value of the SMethodName property.</param>
        public static SurgeryMethod CreateSurgeryMethod(String sMethodCode, String sMethodName)
        {
            SurgeryMethod surgeryMethod = new SurgeryMethod();
            surgeryMethod.SMethodCode = sMethodCode;
            surgeryMethod.SMethodName = sMethodName;
            return surgeryMethod;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public String SMethodCode
        {
            get
            {
                return _SMethodCode;
            }
            set
            {
                if (_SMethodCode != value)
                {
                    OnSMethodCodeChanging(value);
                    ////ReportPropertyChanging("SMethodCode");
                    _SMethodCode = value;
                    RaisePropertyChanged("SMethodCode");
                    OnSMethodCodeChanged();
                }
            }
        }
        private String _SMethodCode;
        partial void OnSMethodCodeChanging(String value);
        partial void OnSMethodCodeChanged();





        [DataMemberAttribute()]
        public String SurgeryTypeCode
        {
            get
            {
                return _SurgeryTypeCode;
            }
            set
            {
                OnSurgeryTypeCodeChanging(value);
                ////ReportPropertyChanging("SurgeryTypeCode");
                _SurgeryTypeCode = value;
                RaisePropertyChanged("SurgeryTypeCode");
                OnSurgeryTypeCodeChanged();
            }
        }
        private String _SurgeryTypeCode;
        partial void OnSurgeryTypeCodeChanging(String value);
        partial void OnSurgeryTypeCodeChanged();





        [DataMemberAttribute()]
        public String SMethodName
        {
            get
            {
                return _SMethodName;
            }
            set
            {
                OnSMethodNameChanging(value);
                ////ReportPropertyChanging("SMethodName");
                _SMethodName = value;
                RaisePropertyChanged("SMethodName");
                OnSMethodNameChanged();
            }
        }
        private String _SMethodName;
        partial void OnSMethodNameChanging(String value);
        partial void OnSMethodNameChanged();





        [DataMemberAttribute()]
        public String SMethodDescription
        {
            get
            {
                return _SMethodDescription;
            }
            set
            {
                OnSMethodDescriptionChanging(value);
                ////ReportPropertyChanging("SMethodDescription");
                _SMethodDescription = value;
                RaisePropertyChanged("SMethodDescription");
                OnSMethodDescriptionChanged();
            }
        }
        private String _SMethodDescription;
        partial void OnSMethodDescriptionChanging(String value);
        partial void OnSMethodDescriptionChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SURGERIE_REL_PCMD0_SURGERYM", "Surgeries")]
        public ObservableCollection<Surgery> Surgeries
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_SURGERYM_REL_PCMD0_SURGERYT", "SurgeryType")]
        public SurgeryType SurgeryType
        {
            get;
            set;
        }




        #endregion
    }
}
