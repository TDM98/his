using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public partial class MedicalRecordTemplateOutline : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new MedicalRecordTemplateOutline object.

        /// <param name="medRecTempOutlineID">Initial value of the MedRecTempOutlineID property.</param>
        /// <param name="mDRptTemplateID">Initial value of the MDRptTemplateID property.</param>
        /// <param name="paragraphID">Initial value of the ParagraphID property.</param>
        public static MedicalRecordTemplateOutline CreateMedicalRecordTemplateOutline(long medRecTempOutlineID, Int64 mDRptTemplateID, long paragraphID)
        {
            MedicalRecordTemplateOutline medicalRecordTemplateOutline = new MedicalRecordTemplateOutline();
            medicalRecordTemplateOutline.MedRecTempOutlineID = medRecTempOutlineID;
            medicalRecordTemplateOutline.MDRptTemplateID = mDRptTemplateID;
            medicalRecordTemplateOutline.ParagraphID = paragraphID;
            return medicalRecordTemplateOutline;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long MedRecTempOutlineID
        {
            get
            {
                return _MedRecTempOutlineID;
            }
            set
            {
                if (_MedRecTempOutlineID != value)
                {
                    OnMedRecTempOutlineIDChanging(value);
                    ////ReportPropertyChanging("MedRecTempOutlineID");
                    _MedRecTempOutlineID = value;
                    RaisePropertyChanged("MedRecTempOutlineID");
                    OnMedRecTempOutlineIDChanged();
                }
            }
        }
        private long _MedRecTempOutlineID;
        partial void OnMedRecTempOutlineIDChanging(long value);
        partial void OnMedRecTempOutlineIDChanged();





        [DataMemberAttribute()]
        public Int64 MDRptTemplateID
        {
            get
            {
                return _MDRptTemplateID;
            }
            set
            {
                OnMDRptTemplateIDChanging(value);
                ////ReportPropertyChanging("MDRptTemplateID");
                _MDRptTemplateID = value;
                RaisePropertyChanged("MDRptTemplateID");
                OnMDRptTemplateIDChanged();
            }
        }
        private Int64 _MDRptTemplateID;
        partial void OnMDRptTemplateIDChanging(Int64 value);
        partial void OnMDRptTemplateIDChanged();





        [DataMemberAttribute()]
        public long ParagraphID
        {
            get
            {
                return _ParagraphID;
            }
            set
            {
                OnParagraphIDChanging(value);
                ////ReportPropertyChanging("ParagraphID");
                _ParagraphID = value;
                RaisePropertyChanged("ParagraphID");
                OnParagraphIDChanged();
            }
        }
        private long _ParagraphID;
        partial void OnParagraphIDChanging(long value);
        partial void OnParagraphIDChanged();





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
                ////ReportPropertyChanging("Idx");
                _Idx = value;
                RaisePropertyChanged("Idx");
                OnIdxChanged();
            }
        }
        private Nullable<Byte> _Idx;
        partial void OnIdxChanging(Nullable<Byte> value);
        partial void OnIdxChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> IsRequired
        {
            get
            {
                return _IsRequired;
            }
            set
            {
                OnIsRequiredChanging(value);
                ////ReportPropertyChanging("IsRequired");
                _IsRequired = value;
                RaisePropertyChanged("IsRequired");
                OnIsRequiredChanged();
            }
        }
        private Nullable<Boolean> _IsRequired;
        partial void OnIsRequiredChanging(Nullable<Boolean> value);
        partial void OnIsRequiredChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> IsCommon
        {
            get
            {
                return _IsCommon;
            }
            set
            {
                OnIsCommonChanging(value);
                ////ReportPropertyChanging("IsCommon");
                _IsCommon = value;
                RaisePropertyChanged("IsCommon");
                OnIsCommonChanged();
            }
        }
        private Nullable<Boolean> _IsCommon;
        partial void OnIsCommonChanging(Nullable<Boolean> value);
        partial void OnIsCommonChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> IsSummaryForm
        {
            get
            {
                return _IsSummaryForm;
            }
            set
            {
                OnIsSummaryFormChanging(value);
                ////ReportPropertyChanging("IsSummaryForm");
                _IsSummaryForm = value;
                RaisePropertyChanged("IsSummaryForm");
                OnIsSummaryFormChanged();
            }
        }
        private Nullable<Boolean> _IsSummaryForm;
        partial void OnIsSummaryFormChanging(Nullable<Boolean> value);
        partial void OnIsSummaryFormChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MEDICALR_REL_PMR33_MEDICALR", "MedicalRecordTemplates")]
        public MedicalRecordTemplate MedicalRecordTemplate
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MEDICALR_REL_PMR34_PARAGRAP", "Paragraphs")]
        public Paragraph Paragraph
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MRTDETAI_REL_PMR35_MEDICALR", "MRTDetails")]
        public ObservableCollection<MRTDetail> MRTDetails
        {
            get;
            set;
        }

        #endregion
    }
}
