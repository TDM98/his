using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataEntities
{
    public class LookupTree : NotifyChangedBase, IEditableObject
    {
        public LookupTree() : base()
        {

        }

        private LookupTree _tempAccidentdrug;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempAccidentdrug = (LookupTree)MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempAccidentdrug)
                CopyFrom(_tempAccidentdrug);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(LookupTree p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        private string nodeText;
        private long nodeID;
        private long? parentID;
        private List<Lookup> children;
        private string description;
        private string code;
        private Lookup parent;
        public LookupTree(string nodeText, long nodeID, long? parentID, string description, string code, Lookup parent)
        {
            this.nodeText = nodeText;
            this.nodeID = nodeID;
            this.parentID = parentID;
            this.description = description;
            children = new List<Lookup>();
            this.parent = parent;
            this.code = code;
        }

        [Required(ErrorMessage = "Node Text is required")]
        [DataMember]
        public string NodeText
        {
            get
            {
                return nodeText;
            }
            set
            {
                ValidateProperty("NodeText", value);
                nodeText = value;
            }
        }

        [DataMember]
        public long? ParentID
        {
            get
            {
                return parentID;
            }
            set
            {
                parentID = value;
            }
        }

        [DataMember]
        public long NodeID
        {
            get
            {
                return nodeID;
            }
            set
            {
                nodeID = value;
            }
        }

        [DataMember]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        [DataMember]
        public List<Lookup> Children
        {
            get
            {
                return children;
            }
            set
            {
                children = value;
            }
        }

        [DataMember]
        public Lookup Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
                RaisePropertyChanged("Parent");
            }
        }

        [DataMember]
        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
            }
        }

        [DataMemberAttribute()]
        public bool IsSimilar
        {
            get
            {
                return _IsSimilar;
            }
            set
            {
                _IsSimilar = value;
                RaisePropertyChanged("IsSimilar");
            }
        }
        private bool _IsSimilar;

        [DataMemberAttribute()]
        public bool IsInteraction
        {
            get
            {
                return _IsInteraction;
            }
            set
            {
                _IsInteraction = value;
                RaisePropertyChanged("IsInteraction");
            }
        }
        private bool _IsInteraction;       

        public bool IsChildren
        {
            get
            {
                return children.Count <= 0;
            }
        }
    }
}
