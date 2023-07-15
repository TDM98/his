using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
   // [DataContract]
    public class PatientServicesTree : NotifyChangedBase, IEditableObject
    {
        public PatientServicesTree(): base()
        {

        }

        private PatientServicesTree _tempAccidentdrug;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempAccidentdrug = (PatientServicesTree)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempAccidentdrug)
                CopyFrom(_tempAccidentdrug);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PatientServicesTree p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        private string nodeText;
        private long nodeID;
        private System.Nullable<long> parentID;
        private List<PatientServicesTree> children;
        private string description;
        private string code;
        private DateTime examDate;
        private PatientServicesTree parent;
        private int _PatientSummaryType;
        private bool _InPt;
        private bool _IsShowInPt;
        private bool _IsShowOutPt;
        public PatientServicesTree(DateTime _examDate,bool _inPt)
        {
            this.examDate = _examDate;
            this.nodeText = this.examDate.Date.ToString("dd/MM/yyyy");
            this.InPt = _inPt;
            this.IsShowInPt = _inPt;
            this.IsShowOutPt = !_inPt;
        }
        public PatientServicesTree(long nodeID, string nodeText, string description, int level, int PatientSummaryType, PatientServicesTree parent, System.Nullable<long> parentID, string code)
        {
            this.nodeID = nodeID;
            this.nodeText = nodeText;
            this.Level = level;
            this.parentID = parentID;
            this.description = description;
            this.PatientSummaryType = PatientSummaryType;
            this.children = new List<PatientServicesTree>();
            this.parent = parent;
            this.code = code;
            this.InPt = parent.InPt;
        }
        //public PatientServicesTree(long nodeID, string nodeText, string description, int level, int eNum, PatientServicesTree parent, System.Nullable<long> parentID, string code)
        //{
        //    this.nodeID = nodeID;
        //    this.nodeText = nodeText;
        //    this.Level = level;
        //    this.parentID = parentID;
        //    this.description = description;
        //    this.eNum = eNum;
        //    this.children = new List<PatientServicesTree>();
        //    this.parent = parent;
        //    this.code = code;
        //}
        public PatientServicesTree(long nodeID, string nodeText, string description, int level,  PatientServicesTree parent, System.Nullable<long> parentID, string code)
        {
            this.nodeID = nodeID;
            this.nodeText = nodeText;
            this.Level = level;
            this.parentID = parentID;
            this.description = description;
            this.children = new List<PatientServicesTree>();
            this.parent = parent;
            this.code = code;
            this.InPt = parent.InPt;
        }

        [Required(ErrorMessage="Node Text is required")]
        [DataMember]
        public string NodeText
        {
            get
            {
                return this.nodeText;
            }
            set
            {
                ValidateProperty("NodeText", value);
                this.nodeText = value;
               
                
            }
        }

        [DataMember]
        public System.Nullable<int> _Level;
        public System.Nullable<int> Level
        {
            get
            {
                return _Level;
            }
            set
            {
                _Level = value;
            }
        }

        [DataMember]
        public System.Nullable<int> _eNum;
        public System.Nullable<int> eNum
        {
            get
            {
                return _eNum;
            }
            set
            {
                _eNum = value;
            }
        }

        [DataMember]
        public System.Nullable<long> ParentID
        {
            get
            {
                return this.parentID;
            }
            set
            {
                this.parentID = value;
            }
        }

        [DataMember]
        public long NodeID
        {
            get
            {
                return this.nodeID;
            }
            set
            {
                this.nodeID = value;
            }
        }
        [DataMember]
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }
        [DataMember]
        public List<PatientServicesTree> Children
        {
            get
            {
                return this.children;
            }
            set
            {
                this.children = value;
            }
        }

        [DataMember]
        public PatientServicesTree Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                this.parent = value;
                RaisePropertyChanged("Parent");
            }
        }

        [DataMember]
        public string Code
        {
            get
            {
                return this.code;
            }
            set
            {
                this.code = value;
            }
        }

        [DataMember]
        public DateTime ExamDate
        {
            get
            {
                return examDate;
            }
            set
            {
                examDate = value;
            }
        }

        [DataMember] 
        public int PatientSummaryType
        {
            get
            {
                return _PatientSummaryType;
            }
            set
            {
                if (_PatientSummaryType == value)
                    return;
                _PatientSummaryType = value;
            }
        }
        [DataMember] 
        public bool InPt
        {
            get
            {
                return _InPt;
            }
            set
            {
                if (_InPt == value)
                    return;
                _InPt = value;
            }
        }
        [DataMember] 
        public bool IsShowInPt
        {
            get
            {
                return _IsShowInPt;
            }
            set
            {
                if (_IsShowInPt == value)
                    return;
                _IsShowInPt = value;
            }
        }
        [DataMember] 
        public bool IsShowOutPt
        {
            get
            {
                return _IsShowOutPt;
            }
            set
            {
                if (_IsShowOutPt == value)
                    return;
                _IsShowOutPt = value;
            }
        }
        private bool _IsWarning;
        [DataMember]
        public bool IsWarning
        {
            get
            {
                return _IsWarning;
            }
            set
            {
                if (_IsWarning == value)
                    return;
                _IsWarning = value;
            }
        }
    }
}


