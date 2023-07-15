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
    public class ModulesTree : NotifyChangedBase, IEditableObject
    {
        public ModulesTree(): base()
        {

        }

        private ModulesTree _tempAccidentdrug;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempAccidentdrug = (ModulesTree)this.MemberwiseClone();
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

        public void CopyFrom(ModulesTree p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        private string nodeText;
        private long nodeID;
        private System.Nullable<long> parentID;
        private List<ModulesTree> children;
        private string description;
        private string code;
        private ModulesTree parent;
        public ModulesTree(long nodeID, string nodeText, string description, int level, int eNum, ModulesTree parent, System.Nullable<long> parentID,string code)
        {
            this.nodeID = nodeID;
            this.nodeText = nodeText;
            this.Level = level;
            this.parentID = parentID;
            this.description = description;
            this.eNum = eNum;
            this.children = new List<ModulesTree>();
            this.parent = parent;
            this.code = code;
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
        public List<ModulesTree> Children
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
        public ModulesTree Parent
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
    }
}

