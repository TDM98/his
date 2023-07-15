using eHCMS.Services.Core.Base;
/*
 * 20190603 #001 TNHX: [BM0011782] Init RefPurposeForAccountant
*/
namespace DataEntities
{
    public class RefPurposeForAccountant : NotifyChangedBase
    {
        private int _PurposeID;
        private string _PurposeName;
        private string _PurposeCode;
        private int _PurposeType;
        private string _PurposeTypeName;
        private bool _IsActive;
        private string _Descriptions;
        public int PurposeID
        {
            get
            {
                return _PurposeID;
            }
            set
            {
                _PurposeID = value;
                RaisePropertyChanged("PurposeID");
            }
        }

        public string PurposeName
        {
            get
            {
                return _PurposeName;
            }
            set
            {
                _PurposeName = value;
                RaisePropertyChanged("PurposeName");
            }
        }

        public string PurposeCode
        {
            get
            {
                return _PurposeCode;
            }
            set
            {
                _PurposeCode = value;
                RaisePropertyChanged("PurposeCode");
            }
        }

        public int PurposeType
        {
            get
            {
                return _PurposeType;
            }
            set
            {
                _PurposeType = value;
                RaisePropertyChanged("PurposeType");
            }
        }

        public string PurposeTypeName
        {
            get
            {
                return _PurposeTypeName;
            }
            set
            {
                _PurposeTypeName = value;
                RaisePropertyChanged("PurposeTypeName");
            }
        }

        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                _IsActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        public string Descriptions
        {
            get
            {
                return _Descriptions;
            }
            set
            {
                _Descriptions = value;
                RaisePropertyChanged("Descriptions");
            }
        }
    }
}
