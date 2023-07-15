using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
/*
 * 20230622 #011 DatTB: Thêm function xuất excel ICD của nhóm bệnh
 */ 
namespace DataEntities
{
    public enum ConfigurationName : int
    {
        [Description("Danh mục chương ICD")]
        DiseaseChapters = 1,
        [Description("Danh mục nhóm ICD")]
        Diseases = 2,
        [Description("Danh mục ICD")]
        DiseasesReferences = 3,
        [Description("Danh mục loại dịch vụ")]
        RefMedicalServiceTypes = 4,
        [Description("Danh mục phòng")]
        Locations = 5,
        [Description("Danh mục CLS")]
        PCLExamTypes = 6,
        [Description("Danh mục diễn tiến bệnh")]
        DiseaseProgression = 7,
        [Description("Danh mục tiêu chuẩn nhập viện - ICD")]
        AdmissionCriterionAttachICD = 8,
        [Description("Danh mục tiêu chuẩn nhập viện - nhóm ICD")]
        AdmissionCriterionAttachGroupPCL = 9,
        [Description("Danh mục tiêu chuẩn nhập viện - Triệu chứng")]
        AdmissionCriterionAttachSymptom = 10,
        [Description("Quản lý dịch vụ của khoa")]
        RefMedicalServiceItems = 11,
        [Description("ICD của nhóm bệnh")]
        OutpatientTreatmentTypeICD10Link = 12,

    }

    public partial class ConfigurationReportParams : EntityBase
    {
        private ConfigurationName _ConfigurationName;
        [DataMemberAttribute()]
        public ConfigurationName ConfigurationName
        {
            get { return _ConfigurationName; }
            set
            {
                if (_ConfigurationName != value)
                {
                    _ConfigurationName = value;
                    RaisePropertyChanged("ConfigurationName");
                }
            }
        }

        [DataMemberAttribute()]
        public long DiseaseChapterID
        {
            get
            {
                return _DiseaseChapterID;
            }
            set
            {
                if (_DiseaseChapterID == value)
                {
                    return;
                }
                _DiseaseChapterID = value;
                RaisePropertyChanged("DiseaseChapterID");
            }
        }
        private long _DiseaseChapterID;

        [DataMemberAttribute()]
        public long DiseaseID
        {
            get
            {
                return _DiseaseID;
            }
            set
            {
                if (_DiseaseID == value)
                {
                    return;
                }
                _DiseaseID = value;
                RaisePropertyChanged("DiseaseID");
            }
        }
        private long _DiseaseID;

        [DataMemberAttribute()]
        public long V_PCLMainCategory
        {
            get
            {
                return _V_PCLMainCategory;
            }
            set
            {
                if (_V_PCLMainCategory == value)
                {
                    return;
                }
                _V_PCLMainCategory = value;
                RaisePropertyChanged("V_PCLMainCategory");
            }
        }
        private long _V_PCLMainCategory;

        [DataMemberAttribute()]
        public long PCLExamTypeSubCategoryID
        {
            get
            {
                return _PCLExamTypeSubCategoryID;
            }
            set
            {
                if (_PCLExamTypeSubCategoryID == value)
                {
                    return;
                }
                _PCLExamTypeSubCategoryID = value;
                RaisePropertyChanged("PCLExamTypeSubCategoryID");
            }
        }
        private long _PCLExamTypeSubCategoryID;

        [DataMemberAttribute()]
        public long AdmissionCriterionID
        {
            get
            {
                return _AdmissionCriterionID;
            }
            set
            {
                if (_AdmissionCriterionID == value)
                {
                    return;
                }
                _AdmissionCriterionID = value;
                RaisePropertyChanged("AdmissionCriterionID");
            }
        }
        private long _AdmissionCriterionID;

        [DataMemberAttribute()]
        public long MedicalServiceTypeID
        {
            get
            {
                return _MedicalServiceTypeID;
            }
            set
            {
                if (_MedicalServiceTypeID == value)
                {
                    return;
                }
                _MedicalServiceTypeID = value;
                RaisePropertyChanged("MedicalServiceTypeID");
            }
        }
        private long _MedicalServiceTypeID;

        //▼==== #001
        [DataMemberAttribute()]
        public long OutpatientTreatmentTypeID
        {
            get
            {
                return _OutpatientTreatmentTypeID;
            }
            set
            {
                if (_OutpatientTreatmentTypeID == value)
                {
                    return;
                }
                _OutpatientTreatmentTypeID = value;
                RaisePropertyChanged("OutpatientTreatmentTypeID");
            }
        }
        private long _OutpatientTreatmentTypeID;
        //▲==== #001
    }
}
