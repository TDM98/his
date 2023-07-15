using eHCMS.Services.Core.Base;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace DataEntities
{
    public class RefTreatmentRegimenDrugDetail : NotifyChangedBase
    {
        private long _TreatmentRegimenDrugDetailID;
        private long _TreatmentRegimenID;
        private long _V_TreatmentPeriodic;
        private long _GenericID;
        private float _MDose;
        private float _ADose;
        private float _EDose;
        private float _NDose;
        private double _Quantity;
        private string _TreatmentRegimenDrugDetailNote;
        private bool _IsDeleted;
        private DateTime _RecCreatedDate;
        private long _CreatedStaffID;
        private DateTime _LastUpdatedDate;
        private long _LastUpdatedStaffID;
        private string _TreatmentPeriodic;
        private string _GenericCode;
        private string _GenericName;
        public long TreatmentRegimenDrugDetailID
        {
            get
            {
                return _TreatmentRegimenDrugDetailID;
            }
            set
            {
                _TreatmentRegimenDrugDetailID = value;
                RaisePropertyChanged("TreatmentRegimenDrugDetailID");
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
        public long GenericID
        {
            get
            {
                return _GenericID;
            }
            set
            {
                _GenericID = value;
                RaisePropertyChanged("GenericID");
            }
        }
        public float MDose
        {
            get
            {
                return _MDose;
            }
            set
            {
                _MDose = value;
                RaisePropertyChanged("MDose");
            }
        }
        public float ADose
        {
            get
            {
                return _ADose;
            }
            set
            {
                _ADose = value;
                RaisePropertyChanged("ADose");
            }
        }
        public float EDose
        {
            get
            {
                return _EDose;
            }
            set
            {
                _EDose = value;
                RaisePropertyChanged("EDose");
            }
        }
        public float NDose
        {
            get
            {
                return _NDose;
            }
            set
            {
                _NDose = value;
                RaisePropertyChanged("NDose");
            }
        }
        public double Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                _Quantity = value;
                RaisePropertyChanged("Quantity");
            }
        }
        public string TreatmentRegimenDrugDetailNote
        {
            get
            {
                return _TreatmentRegimenDrugDetailNote;
            }
            set
            {
                _TreatmentRegimenDrugDetailNote = value;
                RaisePropertyChanged("TreatmentRegimenDrugDetailNote");
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
        public string GenericCode
        {
            get
            {
                return _GenericCode;
            }
            set
            {
                _GenericCode = value;
                RaisePropertyChanged("GenericCode");
            }
        }
        public string GenericName
        {
            get
            {
                return _GenericName;
            }
            set
            {
                _GenericName = value;
                RaisePropertyChanged("GenericName");
            }
        }
    }
    public static class RefTreatmentRegimenDrugDetailExt
    {
        public static string ConvertToXML(this IList<RefTreatmentRegimenDrugDetail> items)
        {
            if (items == null)
                return null;
            var mXDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), new XElement("root",
            from item in items
            select new XElement("RefTreatmentRegimenDrugDetail",
                new XElement("TreatmentRegimenDrugDetailID", item.TreatmentRegimenDrugDetailID),
                new XElement("TreatmentRegimenID", item.TreatmentRegimenID),
                new XElement("V_TreatmentPeriodic", item.V_TreatmentPeriodic),
                new XElement("GenericID", item.GenericID),
                new XElement("MDose", item.MDose),
                new XElement("ADose", item.ADose),
                new XElement("EDose", item.EDose),
                new XElement("NDose", item.NDose),
                new XElement("Quantity", item.Quantity),
                new XElement("TreatmentRegimenDrugDetailNote", item.TreatmentRegimenDrugDetailNote),
                new XElement("IsDeleted", item.IsDeleted),
                new XElement("LastUpdatedStaffID", item.LastUpdatedStaffID)
            )));
            return mXDocument.ToString();
        }
    }
}