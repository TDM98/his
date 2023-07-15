using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aEMR.Common
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DisplayAttribute : Attribute
    {
        #region Properties
        public bool AutoGenerateField { get; set; }

        public bool AutoGenerateFilter { get; set; }

        public string Description { get; set; }

        public string GroupName { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public string Prompt { get; set; }

        public Type ResourceType { get; set; }

        public string ShortName { get; set; }
        #endregion

        public bool GetAutoGenerateField()
        {
            return this.AutoGenerateField;
        }

        public bool GetAutoGenerateFilter()
        {
            return this.AutoGenerateFilter;
        }

        public string GetDescription()
        {
            return this.Description;
        }

        public string GetGroupName()
        {
            return this.GroupName;
        }

        public string GetName()
        {
            return this.Name;
        }

        public Nullable<int> GetOrder()
        {
            return this.Order;
        }

        public string GetPrompt()
        {
            return this.Prompt;
        }

        public Type GetResourceType()
        {
            return this.ResourceType;
        }

        public string GetShortName()
        {
            return this.ShortName;
        }

        public DisplayAttribute()
        {
            this.AutoGenerateField = true;
            
        }
    }
}
