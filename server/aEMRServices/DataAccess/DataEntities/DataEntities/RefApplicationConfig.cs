using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
       public partial class RefApplicationConfig : NotifyChangedBase
    {
        #region Factory Method

      
        /// Create a new RefApplicationConfig object.
        
        /// <param name="configItemID">Initial value of the ConfigItemID property.</param>
        /// <param name="configItemKey">Initial value of the ConfigItemKey property.</param>
        /// <param name="configItemValue">Initial value of the ConfigItemValue property.</param>
        public static RefApplicationConfig CreateRefApplicationConfig(long configItemID, String configItemKey, String configItemValue)
        {
            RefApplicationConfig refApplicationConfig = new RefApplicationConfig();
            refApplicationConfig.ConfigItemID = configItemID;
            refApplicationConfig.ConfigItemKey = configItemKey;
            refApplicationConfig.ConfigItemValue = configItemValue;
            return refApplicationConfig;
        }

        #endregion
        #region Primitive Properties

      
        
        
       
        [DataMemberAttribute()]
        public long ConfigItemID
        {
            get
            {
                return _ConfigItemID;
            }
            set
            {
                if (_ConfigItemID != value)
                {
                    OnConfigItemIDChanging(value);
                    ////ReportPropertyChanging("ConfigItemID");
                    _ConfigItemID =value;
                    RaisePropertyChanged("ConfigItemID");
                    OnConfigItemIDChanged();
                }
            }
        }
        private long _ConfigItemID;
        partial void OnConfigItemIDChanging(long value);
        partial void OnConfigItemIDChanged();

      
        
        
       
        [DataMemberAttribute()]
        public String ConfigItemKey
        {
            get
            {
                return _ConfigItemKey;
            }
            set
            {
                OnConfigItemKeyChanging(value);
                ////ReportPropertyChanging("ConfigItemKey");
                _ConfigItemKey = value;
                RaisePropertyChanged("ConfigItemKey");
                OnConfigItemKeyChanged();
            }
        }
        private String _ConfigItemKey;
        partial void OnConfigItemKeyChanging(String value);
        partial void OnConfigItemKeyChanged();

      
        
        
       
        [DataMemberAttribute()]
        public String ConfigItemValue
        {
            get
            {
                return _ConfigItemValue;
            }
            set
            {
                OnConfigItemValueChanging(value);
                ////ReportPropertyChanging("ConfigItemValue");
                _ConfigItemValue = value;
                RaisePropertyChanged("ConfigItemValue");
                OnConfigItemValueChanged();
            }
        }
        private String _ConfigItemValue;
        partial void OnConfigItemValueChanging(String value);
        partial void OnConfigItemValueChanged();

      
        
        
       
        [DataMemberAttribute()]
        public String ConfigItemNotes
        {
            get
            {
                return _ConfigItemNotes;
            }
            set
            {
                OnConfigItemNotesChanging(value);
                ////ReportPropertyChanging("ConfigItemNotes");
                _ConfigItemNotes = value;
                RaisePropertyChanged("ConfigItemNotes");
                OnConfigItemNotesChanged();
            }
        }
        private String _ConfigItemNotes;
        partial void OnConfigItemNotesChanging(String value);
        partial void OnConfigItemNotesChanged();

        #endregion

    }
}
