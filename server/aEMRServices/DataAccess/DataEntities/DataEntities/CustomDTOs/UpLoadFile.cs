using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;

namespace DataEntities
{
    [DataContract]
    public class UpLoadFile : NotifyChangedBase
    {
        private string _FileName;
        [DataMember]
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    RaisePropertyChanged("FileName");
                }
            }
        }

        private byte[] _File;
        [DataMember]
        public byte[] File
        {
            get
            {
                return _File;
            }
            set
            {
                if (_File != value)
                {
                    _File = value;
                    RaisePropertyChanged("File");
                }
            }
        }

        private string _FileStoreUrl;
        [DataMember]
        public string FileStoreUrl
        {
            get { return _FileStoreUrl; }
            set
            {
                _FileStoreUrl = value;
                RaisePropertyChanged("FileStoreUrl");
            }
        }

        private double _Size;
        [DataMember]
        public double Size { get { return _Size; } set { _Size = value; RaisePropertyChanged("Size"); } }

        private List<byte[]> _Contents;
        [DataMember]
        public List<byte[]> Contents
        {
            get { return _Contents; }
            set
            {
                _Contents = value;
                RaisePropertyChanged("Contents");
            }
        }

    }
}
