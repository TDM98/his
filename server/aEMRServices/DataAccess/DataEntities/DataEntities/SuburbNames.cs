using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
/*
 * 20181113 #001 TTM: BM 0005228: Tạo mới Class WardNames nằm trong file SuburbNames.cs
 */
namespace DataEntities
{
    public partial class SuburbNames : NotifyChangedBase, IEditableObject
    {
        public SuburbNames()
            : base()
        {

        }

        private SuburbNames _tempSuburbNames;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempSuburbNames = (SuburbNames)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempSuburbNames)
                CopyFrom(_tempSuburbNames);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(SuburbNames p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }
        public override bool Equals(object obj)
        {
            SuburbNames info = obj as SuburbNames;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.SuburbNameID > 0 && this.SuburbNameID == info.SuburbNameID;
        }
        #endregion
        #region Factory Method


        /// Create a new SuburbNames object.

        /// <param name="cityProvinceID">Initial value of the CityProvinceID property.</param>
        /// <param name="SuburbName">Initial value of the SuburbName property.</param>
        /// <param name="DescNote">Initial value of the DescNote property.</param>
        public static SuburbNames CreateSuburbNames(long cityProvinceID, String SuburbName, String DescNote)
        {
            SuburbNames SuburbNames = new SuburbNames();
            SuburbNames.CityProvinceID = cityProvinceID;
            SuburbNames.SuburbName = SuburbName;
            SuburbNames.DescNote = DescNote;
            return SuburbNames;
        }

        #endregion
        #region Primitive Properties

        [DataMemberAttribute()]
        public long SuburbNameID
        {
            get
            {
                return _SuburbNameID;
            }
            set
            {
                if (_SuburbNameID != value)
                {
                    OnSuburbNameIDChanging(value);
                    _SuburbNameID = value;
                    RaisePropertyChanged("SuburbNameID");
                    OnSuburbNameIDChanged();
                }
            }
        }
        private long _SuburbNameID;
        partial void OnSuburbNameIDChanging(long value);
        partial void OnSuburbNameIDChanged();

        [DataMemberAttribute()]
        public long CityProvinceID
        {
            get
            {
                return _CityProvinceID;
            }
            set
            {
                if (_CityProvinceID != value)
                {
                    OnCityProvinceIDChanging(value);
                    _CityProvinceID = value;
                    RaisePropertyChanged("CityProvinceID");
                    OnCityProvinceIDChanged();
                }
            }
        }
        private long _CityProvinceID;
        partial void OnCityProvinceIDChanging(long value);
        partial void OnCityProvinceIDChanged();

        [DataMemberAttribute()]
        public string SuburbCode
        {
            get
            {
                return _SuburbCode;
            }
            set
            {
                _SuburbCode = value;
                RaisePropertyChanged("SuburbCode");
            }
        }
        private string _SuburbCode;

        [DataMemberAttribute()]
        public String SuburbName
        {
            get
            {
                return _SuburbName;
            }
            set
            {
                OnSuburbNameChanging(value);
                _SuburbName = value;
                RaisePropertyChanged("SuburbName");
                if (DescNote != "")
                {
                    DisplayName = DescNote + " " + SuburbName;
                }
                else 
                {
                    DisplayName = SuburbName;
                }
                OnSuburbNameChanged();
            }
        }
        private String _SuburbName;
        partial void OnSuburbNameChanging(String value);
        partial void OnSuburbNameChanged();

        [DataMemberAttribute()]
        public String DescNote
        {
            get
            {
                return _DescNote;
            }
            set
            {
                OnDescNoteChanging(value);
                _DescNote = value;
                RaisePropertyChanged("DescNote");
                OnDescNoteChanged();
            }
        }
        private String _DescNote;
        partial void OnDescNoteChanging(String value);
        partial void OnDescNoteChanged();

        [DataMemberAttribute()]
        public String DisplayName
        {
            get
            {
                return _DisplayName;
            }
            set
            {
                OnDisplayNameChanging(value);
                _DisplayName = value;                
                RaisePropertyChanged("DisplayName");
                OnDisplayNameChanged();
            }
        }
        private String _DisplayName;
        partial void OnDisplayNameChanging(String value);
        partial void OnDisplayNameChanged();

        [DataMemberAttribute]
        public DateTime DateModified
        {
            get
            {
                return _DateModified;
            }
            set
            {
                if (_DateModified != value)
                {
                    OnDateModifiedChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _DateModified = value;
                    RaisePropertyChanged("DateModified");
                    OnDateModifiedChanged();
                }
            }
        }
        private DateTime _DateModified;
        partial void OnDateModifiedChanging(DateTime value);
        partial void OnDateModifiedChanged();



        [DataMemberAttribute]
        public String ModifiedLog
        {
            get
            {
                return _ModifiedLog;
            }
            set
            {
                if (_ModifiedLog != value)
                {
                    OnModifiedLogChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _ModifiedLog = value;
                    RaisePropertyChanged("ModifiedLog");
                    OnModifiedLogChanged();
                }
            }
        }
        private String _ModifiedLog;
        partial void OnModifiedLogChanging(String value);
        partial void OnModifiedLogChanged();

        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public CitiesProvince CitiesProvince
        {
            get
            {
                return _CitiesProvince;
            }
            set
            {
                OnCitiesProvinceChanging(value);
                _CitiesProvince = value;
                if (_CitiesProvince!=null)
                {
                    CityProvinceID = _CitiesProvince.CityProvinceID;
                }
                RaisePropertyChanged("CitiesProvince");
                OnCitiesProvinceChanged();
            }
        }
        private CitiesProvince _CitiesProvince;
        partial void OnCitiesProvinceChanging(CitiesProvince value);
        partial void OnCitiesProvinceChanged();

        

        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    //▼====== #001
    public partial class WardNames : NotifyChangedBase
    {
        #region Primitive Properties

        [DataMemberAttribute()]
        public long WardNameID
        {
            get
            {
                return _WardNameID;
            }
            set
            {
                if (_WardNameID != value)
                {
                    _WardNameID = value;
                    RaisePropertyChanged("WardNameID");
                }
            }
        }
        private long _WardNameID;

        [DataMemberAttribute()]
        public long SuburbNameID
        {
            get
            {
                return _SuburbNameID;
            }
            set
            {
                if (_SuburbNameID != value)
                {
                    _SuburbNameID = value;
                    RaisePropertyChanged("SuburbNameID");
                }
            }
        }
        private long _SuburbNameID;

        [DataMemberAttribute()]
        public string WardCode
        {
            get
            {
                return _WardCode;
            }
            set
            {
                _WardCode = value;
                RaisePropertyChanged("WardCode");
            }
        }
        private string _WardCode;

        [DataMemberAttribute()]
        public String WardName
        {
            get
            {
                return _WardName;
            }
            set
            {
                _WardName = value;
                RaisePropertyChanged("WardName");
            }
        }
        private String _WardName;
        //▲====== #001

        [DataMemberAttribute]
        public DateTime DateModified
        {
            get
            {
                return _DateModified;
            }
            set
            {
                if (_DateModified != value)
                {
                    OnDateModifiedChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _DateModified = value;
                    RaisePropertyChanged("DateModified");
                    OnDateModifiedChanged();
                }
            }
        }
        private DateTime _DateModified;
        partial void OnDateModifiedChanging(DateTime value);
        partial void OnDateModifiedChanged();



        [DataMemberAttribute]
        public String ModifiedLog
        {
            get
            {
                return _ModifiedLog;
            }
            set
            {
                if (_ModifiedLog != value)
                {
                    OnModifiedLogChanging(value);
                    ////ReportPropertyChanging("IBID");
                    _ModifiedLog = value;
                    RaisePropertyChanged("ModifiedLog");
                    OnModifiedLogChanged();
                }
            }
        }
        private String _ModifiedLog;
        partial void OnModifiedLogChanging(String value);
        partial void OnModifiedLogChanged();
        #endregion
    }
}
