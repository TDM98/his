using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace DataEntities
{
    public class RefTreatmentRegimenPCLDetail : NotifyChangedBase
    {
        private long _TreatmentRegimenPCLDetailID;
        private long _TreatmentRegimenID;
        private long _V_TreatmentPeriodic;
        private long _PCLExamTypeID;
        private string _PCLExamTypeCode;
        private string _PCLExamTypeName;
        private string _TreatmentRegimenPCLDetailNote;
        private bool _IsDeleted;
        private DateTime _RecCreatedDate;
        private long _CreatedStaffID;
        private DateTime _LastUpdatedDate;
        private long _LastUpdatedStaffID;
        private string _TreatmentPeriodic;
        private long _V_PCLMainCategory;
        private string _V_PCLMainCategoryValue;
        public long TreatmentRegimenPCLDetailID
        {
            get
            {
                return _TreatmentRegimenPCLDetailID;
            }
            set
            {
                _TreatmentRegimenPCLDetailID = value;
                RaisePropertyChanged("TreatmentRegimenPCLDetailID");
            }
        }
        public long TreatmentRegimenID
        {
            get
            {
                return _TreatmentRegimenID;
            }
            set
            {
                _TreatmentRegimenID = value;
                RaisePropertyChanged("TreatmentRegimenID");
            }
        }
        public long V_TreatmentPeriodic
        {
            get
            {
                return _V_TreatmentPeriodic;
            }
            set
            {
                _V_TreatmentPeriodic = value;
                RaisePropertyChanged("V_TreatmentPeriodic");
            }
        }
        public long PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                _PCLExamTypeID = value;
                RaisePropertyChanged("PCLExamTypeID");
            }
        }
        public string PCLExamTypeCode
        {
            get
            {
                return _PCLExamTypeCode;
            }
            set
            {
                _PCLExamTypeCode = value;
                RaisePropertyChanged("PCLExamTypeCode");
            }
        }
        public string PCLExamTypeName
        {
            get
            {
                return _PCLExamTypeName;
            }
            set
            {
                _PCLExamTypeName = value;
                RaisePropertyChanged("PCLExamTypeName");
            }
        }
        public string TreatmentRegimenPCLDetailNote
        {
            get
            {
                return _TreatmentRegimenPCLDetailNote;
            }
            set
            {
                _TreatmentRegimenPCLDetailNote = value;
                RaisePropertyChanged("TreatmentRegimenPCLDetailNote");
            }
        }
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
            }
        }
        public long CreatedStaffID
        {
            get
            {
                return _CreatedStaffID;
            }
            set
            {
                _CreatedStaffID = value;
                RaisePropertyChanged("CreatedStaffID");
            }
        }
        public DateTime LastUpdatedDate
        {
            get
            {
                return _LastUpdatedDate;
            }
            set
            {
                _LastUpdatedDate = value;
                RaisePropertyChanged("LastUpdatedDate");
            }
        }
        public long LastUpdatedStaffID
        {
            get
            {
                return _LastUpdatedStaffID;
            }
            set
            {
                _LastUpdatedStaffID = value;
                RaisePropertyChanged("LastUpdatedStaffID");
            }
        }
        public string TreatmentPeriodic
        {
            get
            {
                return _TreatmentPeriodic;
            }
            set
            {
                _TreatmentPeriodic = value;
                RaisePropertyChanged("TreatmentPeriodic");
            }
        }
        public long V_PCLMainCategory
        {
            get
            {
                return _V_PCLMainCategory;
            }
            set
            {
                _V_PCLMainCategory = value;
                RaisePropertyChanged("V_PCLMainCategory");
            }
        }
        public string V_PCLMainCategoryValue
        {
            get
            {
                return _V_PCLMainCategoryValue;
            }
            set
            {
                _V_PCLMainCategoryValue = value;
                RaisePropertyChanged("V_PCLMainCategoryValue");
            }
        }
    }
    public static class RefTreatmentRegimenPCLDetailExt
    {
        public static string ConvertToXML(this IList<RefTreatmentRegimenPCLDetail> items)
        {
            if (items == null)
                return null;
            var mXDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), new XElement("root",
            from item in items
            select new XElement("RefTreatmentRegimenPCLDetail",
                new XElement("TreatmentRegimenPCLDetailID", item.TreatmentRegimenPCLDetailID),
                new XElement("TreatmentRegimenID", item.TreatmentRegimenID),
                new XElement("V_TreatmentPeriodic", item.V_TreatmentPeriodic),
                new XElement("PCLExamTypeID", item.PCLExamTypeID),
                new XElement("V_PCLMainCategory", item.V_PCLMainCategory),
                new XElement("TreatmentRegimenPCLDetailNote", item.TreatmentRegimenPCLDetailNote),
                new XElement("IsDeleted", item.IsDeleted),
                new XElement("LastUpdatedStaffID", item.LastUpdatedStaffID)
            )));
            return mXDocument.ToString();
        }
    }
}