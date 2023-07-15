using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
namespace DataEntities
{
    [DataContract]
    public partial class PositionInHospital : NotifyChangedBase
    {
        private long _PositionHosID;
        private int _PositionRefID;
        private string _PositionName;
        private string _PositionDesc;
        [DataMemberAttribute]
        public long PositionHosID
        {
            get
            {
                return _PositionHosID;
            }
            set
            {
                if (_PositionHosID == value)
                {
                    return;
                }
                _PositionHosID = value;
                RaisePropertyChanged("PositionHosID");
            }
        }
        [DataMemberAttribute]
        public int PositionRefID
        {
            get
            {
                return _PositionRefID;
            }
            set
            {
                if (_PositionRefID == value)
                {
                    return;
                }
                _PositionRefID = value;
                RaisePropertyChanged("PositionRefID");
            }
        }
        [DataMemberAttribute]
        public string PositionName
        {
            get
            {
                return _PositionName;
            }
            set
            {
                if (_PositionName == value)
                {
                    return;
                }
                _PositionName = value;
                RaisePropertyChanged("PositionName");
            }
        }
        [DataMemberAttribute]
        public string PositionDesc
        {
            get
            {
                return _PositionDesc;
            }
            set
            {
                if (_PositionDesc == value)
                {
                    return;
                }
                _PositionDesc = value;
                RaisePropertyChanged("PositionDesc");
            }
        }
    }
}