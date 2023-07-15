using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PharmacyService
{
    [DataContract]    
    public class FamilyTherapyTreeview
    {
        private string nodeText;
		private decimal nodeID;
		private System.Nullable<decimal> parentID;
		private List<FamilyTherapyTreeview> children;
        private string description;

        public FamilyTherapyTreeview(string nodeText, decimal nodeID, System.Nullable<decimal> parentID,string description)
		{
			this.nodeText = nodeText;
			this.nodeID = nodeID;
			this.parentID = parentID;
            this.description = description;
            this.children = new List<FamilyTherapyTreeview>();
		}

        [DataMember]
		public string NodeText
		{
			get
			{
				return this.nodeText;
			}
            set
            {
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
        public List<FamilyTherapyTreeview> Children
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
    }
}
