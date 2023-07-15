using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DataEntities
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
        private long nodeID;
        private System.Nullable<long> parentID;
        private List<TherapyTree> children;
        private string description;
        private string code;
        private DrugClass parent;
        public TherapyTree(string nodeText, long nodeID, System.Nullable<long> parentID, string description,string code,DrugClass parent)
        {
            this.nodeText = nodeText;
            this.nodeID = nodeID;
            this.parentID = parentID;
            this.description = description;
            this.children = new List<TherapyTree>();
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
        public DrugClass Parent
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

        [DataMemberAttribute()]
        public Lookup V_InteractionWarningLevel
        {
            get
            {
                return _V_InteractionWarningLevel;
            }
            set
            {
                _V_InteractionWarningLevel = value;
                RaisePropertyChanged("V_InteractionWarningLevel");
            }
        }
        private Lookup _V_InteractionWarningLevel;

        [DataMemberAttribute()]
        public Lookup V_InteractionSeverityLevel
        {
            get
            {
                return _V_InteractionSeverityLevel;
            }
            set
            {
                _V_InteractionSeverityLevel = value;
                RaisePropertyChanged("V_InteractionSeverityLevel");
            }
        }
        private Lookup _V_InteractionSeverityLevel;

        public bool IsChildren
        {
            get
            {
                return this.children.Count <= 0;
            }
        }
    }

    public class RefGeneric : NotifyChangedBase
    {
        private long _GenericID;
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

        private string _GenericCode;
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

        private string _GenericName;
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

        private bool _Active;
        public bool Active
        {
            get
            {
                return _Active;
            }
            set
            {
                _Active = value;
                RaisePropertyChanged("Active");
            }
        }

        private string _GenericDescription;
        public string GenericDescription
        {
            get
            {
                return _GenericDescription;
            }
            set
            {
                _GenericDescription = value;
                RaisePropertyChanged("GenericDescription");
            }
        }
    }
    public class RefGenericRelation : NotifyChangedBase
    {
        public static Dictionary<long, List<RefGenericRelation>> CreateMapDictionary(List<RefGenericRelation> aGenericRelationCollection)
        {
            return aGenericRelationCollection.GroupBy(x => x.FirstGeneric.GenericID).ToDictionary(x => x.Key, x => x.ToList());
        }
        private long _GenericRelationID;
        public long GenericRelationID
        {
            get
            {
                return _GenericRelationID;
            }
            set
            {
                _GenericRelationID = value;
                RaisePropertyChanged("GenericRelationID");
            }
        }

        private RefGeneric _FirstGeneric;
        public RefGeneric FirstGeneric
        {
            get
            {
                return _FirstGeneric;
            }
            set
            {
                _FirstGeneric = value;
                RaisePropertyChanged("FirstGeneric");
            }
        }

        private RefGeneric _SecondGeneric;
        public RefGeneric SecondGeneric
        {
            get
            {
                return _SecondGeneric;
            }
            set
            {
                _SecondGeneric = value;
                RaisePropertyChanged("SecondGeneric");
            }
        }

        private bool _IsSimilar;
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

        private bool _IsInteraction;
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

        private Lookup _V_InteractionSeverityLevel;
        public Lookup V_InteractionSeverityLevel
        {
            get
            {
                return _V_InteractionSeverityLevel;
            }
            set
            {
                _V_InteractionSeverityLevel = value;
                RaisePropertyChanged("V_InteractionSeverityLevel");
            }
        }

        private Lookup _V_InteractionWarningLevel;
        public Lookup V_InteractionWarningLevel
        {
            get
            {
                return _V_InteractionWarningLevel;
            }
            set
            {
                _V_InteractionWarningLevel = value;
                RaisePropertyChanged("V_InteractionWarningLevel");
            }
        }
    }
}