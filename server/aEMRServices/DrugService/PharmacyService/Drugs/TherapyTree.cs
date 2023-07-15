using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;

using System.ComponentModel.DataAnnotations;

namespace PharmacyService
{
   // [DataContract]
    public class TherapyTree : NotifyChangedBase, IEditableObject
    {
        public TherapyTree(): base()
        {

        }

        private TherapyTree _tempAccidentdrug;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempAccidentdrug = (TherapyTree)this.MemberwiseClone();
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

        public void CopyFrom(TherapyTree p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        private string nodeText;
        private decimal nodeID;
        private System.Nullable<decimal> parentID;
        private List<TherapyTree> children;
        private string description;
        private string code;

        public TherapyTree(string nodeText, decimal nodeID, System.Nullable<decimal> parentID, string description,string code)
        {
            this.nodeText = nodeText;
            this.nodeID = nodeID;
            this.parentID = parentID;
            this.description = description;
            this.children = new List<TherapyTree>();
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
        public System.Nullable<decimal> ParentID
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
        public decimal NodeID
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
        public List<TherapyTree> Children
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
