using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class MedicalRecordTemplate : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new MedicalRecordTemplate object.

        /// <param name="mDRptTemplateID">Initial value of the MDRptTemplateID property.</param>
        /// <param name="templateName">Initial value of the TemplateName property.</param>
        public static MedicalRecordTemplate CreateMedicalRecordTemplate(Int64 mDRptTemplateID, String templateName)
        {
            MedicalRecordTemplate medicalRecordTemplate = new MedicalRecordTemplate();
            medicalRecordTemplate.MDRptTemplateID = mDRptTemplateID;
            medicalRecordTemplate.TemplateName = templateName;
            return medicalRecordTemplate;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 MDRptTemplateID
        {
            get
            {
                return _MDRptTemplateID;
            }
            set
            {
                if (_MDRptTemplateID != value)
                {
                    OnMDRptTemplateIDChanging(value);
                    ////ReportPropertyChanging("MDRptTemplateID");
                    _MDRptTemplateID = value;
                    RaisePropertyChanged("MDRptTemplateID");
                    OnMDRptTemplateIDChanged();
                }
            }
        }
        private Int64 _MDRptTemplateID;
        partial void OnMDRptTemplateIDChanging(Int64 value);
        partial void OnMDRptTemplateIDChanged();





        [DataMemberAttribute()]
        public String TemplateName
        {
            get
            {
                return _TemplateName;
            }
            set
            {
                OnTemplateNameChanging(value);
                ////ReportPropertyChanging("TemplateName");
                _TemplateName = value;
                RaisePropertyChanged("TemplateName");
                OnTemplateNameChanged();
            }
        }
        private String _TemplateName;
        partial void OnTemplateNameChanging(String value);
        partial void OnTemplateNameChanged();





        [DataMemberAttribute()]
        public String TemplateFilePath
        {
            get
            {
                return _TemplateFilePath;
            }
            set
            {
                OnTemplateFilePathChanging(value);
                ////ReportPropertyChanging("TemplateFilePath");
                _TemplateFilePath = value;
                RaisePropertyChanged("TemplateFilePath");
                OnTemplateFilePathChanged();
            }
        }
        private String _TemplateFilePath;
        partial void OnTemplateFilePathChanging(String value);
        partial void OnTemplateFilePathChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MEDICALR_REL_PMR33_MEDICALR", "MedicalRecordTemplateOutline")]
        public ObservableCollection<MedicalRecordTemplateOutline> MedicalRecordTemplateOutlines
        {
            get;
            set;
        }

        #endregion
        public override bool Equals(object obj)
        {
            MedicalRecordTemplate cond = obj as MedicalRecordTemplate;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.MDRptTemplateID == cond.MDRptTemplateID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
