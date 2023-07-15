using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DataEntities
{
    public class RefTreatmentRegimenServiceDetail: NotifyChangedBase
    {
        #region Private 
        private long _TreatmentRegimenServiceDetailID;
        private long _TreatmentRegimenID;
        private long _V_TreatmentPeriodic;
        private long _MedServiceID;
        private long _MedicalServiceTypeID;
        private string _MedicalServiceTypeName;
        private string _MedServiceCode;
        private string _MedServiceName;
        private string _TreatmentRegimenServiceDetailNote;
        private bool _IsDeleted;
        private DateTime _RecCreatedDate;
        private long _CreatedStaffID;
        private DateTime _LastUpdatedDate;
        private long _LastUpdatedStaffID;
        private string _TreatmentPeriodic;
        #endregion

        #region Public
        public long TreatmentRegimenServiceDetailID
        {
            get
            {
                return _TreatmentRegimenServiceDetailID;
            }
            set
            {
                _TreatmentRegimenServiceDetailID = value;
                RaisePropertyChanged("TreatmentRegimenServiceDetailID");
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

       
        public string TreatmentRegimenServiceDetailNote
        {
            get
            {
                return _TreatmentRegimenServiceDetailNote;
            }
            set
            {
                _TreatmentRegimenServiceDetailNote = value;
                RaisePropertyChanged("TreatmentRegimenServiceDetailNote");
            }
        }
        public long MedServiceID
        {
            get
            {
                return _MedServiceID;
            }
            set
            {
                _MedServiceID = value;
                RaisePropertyChanged("MedServiceID");
            }
        }
        public long MedicalServiceTypeID
        {
            get
            {
                return _MedicalServiceTypeID;
            }
            set
            {
                _MedicalServiceTypeID = value;
                RaisePropertyChanged("MedicalServiceTypeID");
            }
        }
        public string MedicalServiceTypeName
        {
            get
            {
                return _MedicalServiceTypeName;
            }
            set
            {
                _MedicalServiceTypeName = value;
                RaisePropertyChanged("MedicalServiceTypeName");
            }
        }
        public string MedServiceCode
        {
            get
            {
                return _MedServiceCode;
            }
            set
            {
                _MedServiceCode = value;
                RaisePropertyChanged("MedServiceCode");
            }
        }
        public string MedServiceName
        {
            get
            {
                return _MedServiceName;
            }
            set
            {
                _MedServiceName = value;
                RaisePropertyChanged("MedServiceName");
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
    }
    #endregion


    public static class RefTreatmentRegimenServiceDetailExt
    {
        public static string ConvertToXML(this IList<RefTreatmentRegimenServiceDetail> items)
        {
            if (items == null)
            {
                return null;
            }
            var mXDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), new XElement("root",
            from item in items
            select new XElement("RefTreatmentRegimenServiceDetail",
                new XElement("TreatmentRegimenServiceDetailID", item.TreatmentRegimenServiceDetailID),
                new XElement("TreatmentRegimenID", item.TreatmentRegimenID),
                new XElement("V_TreatmentPeriodic", item.V_TreatmentPeriodic),
                new XElement("MedServiceID", item.MedServiceID),
                new XElement("MedicalServiceTypeID", item.MedicalServiceTypeID),
                new XElement("TreatmentRegimenPCLDetailNote", item.TreatmentRegimenServiceDetailNote),
                new XElement("IsDeleted", item.IsDeleted),
                new XElement("LastUpdatedStaffID", item.LastUpdatedStaffID)
            )));
            return mXDocument.ToString();
        }
    }
}