using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.IO;

namespace DataEntities
{

    public partial class PCLResultFileStorageDetail : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PCLResultFileStorageDetail object.

        /// <param name="pCLResultFileItemID">Initial value of the PCLResultFileItemID property.</param>
        public static PCLResultFileStorageDetail CreatePCLResultFileStorageDetail(long pCLResultFileItemID)
        {
            PCLResultFileStorageDetail pCLResultFileStorageDetail = new PCLResultFileStorageDetail();
            pCLResultFileStorageDetail.PCLResultFileItemID = pCLResultFileItemID;
            return pCLResultFileStorageDetail;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public long PCLResultFileItemID
        {
            get
            {
                return _PCLResultFileItemID;
            }
            set
            {
                if (_PCLResultFileItemID != value)
                {
                    OnPCLResultFileItemIDChanging(value);
                    _PCLResultFileItemID = value;
                    RaisePropertyChanged("PCLResultFileItemID");
                    OnPCLResultFileItemIDChanged();
                }
            }
        }
        private long _PCLResultFileItemID;
        partial void OnPCLResultFileItemIDChanging(long value);
        partial void OnPCLResultFileItemIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> PCLImgResultID
        {
            get
            {
                return _PCLImgResultID;
            }
            set
            {
                OnPCLImgResultIDChanging(value);
                _PCLImgResultID = value;
                RaisePropertyChanged("PCLImgResultID");
                OnPCLImgResultIDChanged();
            }
        }
        private Nullable<long> _PCLImgResultID;
        partial void OnPCLImgResultIDChanging(Nullable<long> value);
        partial void OnPCLImgResultIDChanged();

        [DataMemberAttribute()]
        public String PCLResultLocation
        {
            get
            {
                return _PCLResultLocation;
            }
            set
            {
                OnPCLResultLocationChanging(value);
                _PCLResultLocation = value;
                RaisePropertyChanged("PCLResultLocation");
                OnPCLResultLocationChanged();
            }
        }
        private String _PCLResultLocation;
        partial void OnPCLResultLocationChanging(String value);
        partial void OnPCLResultLocationChanged();


        [DataMemberAttribute()]
        public String PCLResultFileName
        {
            get
            {
                return _PCLResultFileName;
            }
            set
            {
                OnPCLResultFileNameChanging(value);
                _PCLResultFileName = value;
                RaisePropertyChanged("PCLResultFileName");
                OnPCLResultFileNameChanged();
            }
        }
        private String _PCLResultFileName;
        partial void OnPCLResultFileNameChanging(String value);
        partial void OnPCLResultFileNameChanged();


        [DataMemberAttribute()]
        public String PCLResultFileNotes
        {
            get
            {
                return _PCLResultFileNotes;
            }
            set
            {
                OnPCLResultFileNotesChanging(value);
                ////ReportPropertyChanging("PCLResultFileNotes");
                _PCLResultFileNotes = value;
                RaisePropertyChanged("PCLResultFileNotes");
                OnPCLResultFileNotesChanged();
            }
        }
        private String _PCLResultFileNotes;
        partial void OnPCLResultFileNotesChanging(String value);
        partial void OnPCLResultFileNotesChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_ResultType
        {
            get
            {
                return _V_ResultType;
            }
            set
            {
                OnV_ResultTypeChanging(value);
                _V_ResultType = value;
                RaisePropertyChanged("V_ResultType");
                OnV_ResultTypeChanged();
            }
        }
        private Nullable<Int64> _V_ResultType;
        partial void OnV_ResultTypeChanging(Nullable<Int64> value);
        partial void OnV_ResultTypeChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> Flag
        {
            get
            {
                return _Flag;
            }
            set
            {
                _Flag = value;
                RaisePropertyChanged("Flag");
            }
        }
        private Nullable<Int64> _Flag;


        [DataMemberAttribute()]
        public String FullPath
        {
            get
            {
                return _FullPath;
            }
            set
            {
                _FullPath = value;
                RaisePropertyChanged("FullPath");
            }
        }
        private String _FullPath;


        //[DataMemberAttribute()]
        //public Stream IOStream
        //{
        //    get
        //    {
        //        return _IOStream;
        //    }
        //    set
        //    {
        //        _IOStream = value;
        //        RaisePropertyChanged("IOStream");
        //    }
        //}
        //private Stream _IOStream;

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

        #endregion


        #region Navigation Properties

        [DataMemberAttribute()]
        public PatientPCLImagingResult PatientPCLExamResult
        {
            get;
            set;
        }

        private bool _IsUseForPrinting = false;
        [DataMemberAttribute()]
        public bool IsUseForPrinting
        {
            get
            {
                return _IsUseForPrinting;
            }
            set
            {
                _IsUseForPrinting = value;
                RaisePropertyChanged("IsUseForPrinting");
            }
        }

        #endregion
    }

    public partial class ScanImageFileStorageDetail : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PCLResultFileStorageDetail object.

        /// <param name="pCLResultFileItemID">Initial value of the PCLResultFileItemID property.</param>
        public static ScanImageFileStorageDetail CreateScanImageFileStorageDetail(long nScanImageFileItemID)
        {
            ScanImageFileStorageDetail pCLResultFileStorageDetail = new ScanImageFileStorageDetail();
            pCLResultFileStorageDetail.ScanImageFileItemID = nScanImageFileItemID;
            return pCLResultFileStorageDetail;
        }

        #endregion

        #region Primitive Properties

        private long _PtRegistrationID = 0;
        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                _PtRegistrationID = value;
                RaisePropertyChanged("PtRegistrationID");
            }
        }

        private long _StaffID = 0;
        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                _StaffID = value;
                RaisePropertyChanged("StaffID");
            }
        }
        private long _PatientID = 0;
        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                _PatientID = value;
                RaisePropertyChanged("PatientID");
            }
        }

        private bool _IsDeleted = false;
        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get
            {
                return _IsDeleted;
            }
            set
            {
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        [DataMemberAttribute()]
        public long ScanImageFileItemID
        {
            get
            {
                return _ScanImageFileItemID;
            }
            set
            {
                if (_ScanImageFileItemID != value)
                {
                    OnScanImageFileItemIDChanging(value);
                    _ScanImageFileItemID = value;
                    RaisePropertyChanged("ScanImageFileItemID");
                    OnScanImageFileItemIDChanged();
                }
            }
        }
        private long _ScanImageFileItemID;
        partial void OnScanImageFileItemIDChanging(long value);
        partial void OnScanImageFileItemIDChanged();


        [DataMemberAttribute()]
        public String ScanFileStorageLocation
        {
            get
            {
                return _ScanFileStorageLocation;
            }
            set
            {
                OnScanFileStorageLocationChanging(value);
                _ScanFileStorageLocation = value;
                RaisePropertyChanged("ScanFileStorageLocation");
                OnScanFileStorageLocationChanged();
            }
        }
        private String _ScanFileStorageLocation;
        partial void OnScanFileStorageLocationChanging(String value);
        partial void OnScanFileStorageLocationChanged();


        [DataMemberAttribute()]
        public String ScanImageFileName
        {
            get
            {
                return _ScanImageFileName;
            }
            set
            {
                OnScanImageFileNameChanging(value);
                _ScanImageFileName = value;
                RaisePropertyChanged("ScanImageFileName");
                OnScanImageFileNameChanged();
            }
        }
        private String _ScanImageFileName;
        partial void OnScanImageFileNameChanging(String value);
        partial void OnScanImageFileNameChanged();


        [DataMemberAttribute()]
        public String ScanImageFileNotes
        {
            get
            {
                return _ScanImageFileNotes;
            }
            set
            {
                OnScanImageFileNotesChanging(value);
                ////ReportPropertyChanging("ScanImageFileNotes");
                _ScanImageFileNotes = value;
                RaisePropertyChanged("ScanImageFileNotes");
                OnScanImageFileNotesChanged();
            }
        }
        private String _ScanImageFileNotes;
        partial void OnScanImageFileNotesChanging(String value);
        partial void OnScanImageFileNotesChanged();

        [DataMemberAttribute()]
        public Nullable<Int64> V_ScanImageOfType
        {
            get
            {
                return _V_ScanImageOfType;
            }
            set
            {
                OnV_ScanImageOfTypeChanging(value);
                _V_ScanImageOfType = value;
                RaisePropertyChanged("V_ScanImageOfType");
                OnV_ScanImageOfTypeChanged();
            }
        }
        private Nullable<Int64> _V_ScanImageOfType;
        partial void OnV_ScanImageOfTypeChanging(Nullable<Int64> value);
        partial void OnV_ScanImageOfTypeChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> Flag
        {
            get
            {
                return _Flag;
            }
            set
            {
                _Flag = value;
                RaisePropertyChanged("Flag");
            }
        }
        private Nullable<Int64> _Flag;


        [DataMemberAttribute()]
        public String FullPath
        {
            get
            {
                return _FullPath;
            }
            set
            {
                _FullPath = value;
                RaisePropertyChanged("FullPath");
            }
        }
        private String _FullPath;


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

        private Stream _IOStream;
        public Stream IOStream
        {
            get
            {
                return _IOStream;
            }
            set
            {
                if (_IOStream != value)
                {
                    _IOStream = value;
                    RaisePropertyChanged("IOStream");
                }
            }
        }
        #endregion


    }
}
