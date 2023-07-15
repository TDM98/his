using eHCMS.Services.Core.Base;

namespace DataEntities
{
    public class ShortHandDictionary : NotifyChangedBase
    {
        private long _ShortHandDictionaryID;
        private string _ShortHandDictionaryKey;
        private string _ShortHandDictionaryValue;
        private long _StaffID;
        public long ShortHandDictionaryID
        {
            get => _ShortHandDictionaryID; set
            {
                _ShortHandDictionaryID = value;
                RaisePropertyChanged("ShortHandDictionaryID");
            }
        }
        public string ShortHandDictionaryKey
        {
            get => _ShortHandDictionaryKey; set
            {
                _ShortHandDictionaryKey = value;
                RaisePropertyChanged("ShortHandDictionaryKey");
            }
        }
        public string ShortHandDictionaryValue { get => _ShortHandDictionaryValue; set
            {
                _ShortHandDictionaryValue = value;
                RaisePropertyChanged("ShortHandDictionaryValue");
            }
        }
        public long StaffID
        {
            get => _StaffID; set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }
    }
}