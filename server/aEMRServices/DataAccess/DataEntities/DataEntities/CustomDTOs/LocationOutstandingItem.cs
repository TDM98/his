using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{

    public class LocationOutstandingItem : OutstandingItem
    {

        public override string HeaderText
        {
            get
            {
                return _LocationName + "(" + NumOfPatients.ToString()+ " patients)";
            }
            set
            {
                RaisePropertyChanged("HeaderText");
            }
        }

        public override object ID
        {
            get
            {
                return _LID;
            }
            set
            {
                RaisePropertyChanged("ID");
            }
        }

        [DataMemberAttribute()]
        public long LID
        {
            get
            {
                return _LID;
            }
            set
            {
                if (_LID != value)
                {
                    _LID = value;
                    RaisePropertyChanged("LID");
                }
            }
        }
        private long _LID;


        [DataMemberAttribute()]
        public String LocationName
        {
            get
            {
                return _LocationName;
            }
            set
            {
                _LocationName = value;
                RaisePropertyChanged("LocationName");
            }
        }
        private String _LocationName;



        [DataMemberAttribute()]
        public String LocationDescription
        {
            get
            {
                return _LocationDescription;
            }
            set
            {
                _LocationDescription = value;
                RaisePropertyChanged("LocationDescription");
            }
        }
        private String _LocationDescription;

        public int NumOfPatients { get; set; }
    }
}
