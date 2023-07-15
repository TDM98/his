using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class Lookup : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new Lookup object.

        /// <param name="lookupID">Initial value of the LookupID property.</param>
        /// <param name="objectTypeID">Initial value of the ObjectTypeID property.</param>
        /// <param name="objectName">Initial value of the ObjectName property.</param>
        /// <param name="objectValue">Initial value of the ObjectValue property.</param>
        public static Lookup CreateLookup(Int64 lookupID, Int64 objectTypeID, String objectName, String objectValue)
        {
            Lookup lookup = new Lookup();
            lookup.LookupID = lookupID;
            lookup.ObjectTypeID = objectTypeID;
            lookup.ObjectName = objectName;
            lookup.ObjectValue = objectValue;
            return lookup;
        }

        #endregion
        #region Primitive Properties
        [DataMemberAttribute()]
        public Int64 LookupID
        {
            get
            {
                return _LookupID;
            }
            set
            {
                if (_LookupID != value)
                {
                    OnLookupIDChanging(value);
                    ////ReportPropertyChanging("LookupID");
                    _LookupID = value;
                    RaisePropertyChanged("LookupID");
                    OnLookupIDChanged();
                }
            }
        }
        private Int64 _LookupID;
        partial void OnLookupIDChanging(Int64 value);
        partial void OnLookupIDChanged();

        [DataMemberAttribute()]
        public Int64 ObjectTypeID
        {
            get
            {
                return _ObjectTypeID;
            }
            set
            {
                OnObjectTypeIDChanging(value);
                ////ReportPropertyChanging("ObjectTypeID");
                _ObjectTypeID = value;
                RaisePropertyChanged("ObjectTypeID");
                OnObjectTypeIDChanged();
            }
        }
        private Int64 _ObjectTypeID;
        partial void OnObjectTypeIDChanging(Int64 value);
        partial void OnObjectTypeIDChanged();

        [DataMemberAttribute()]
        public String ObjectName
        {
            get
            {
                return _ObjectName;
            }
            set
            {
                OnObjectNameChanging(value);
                ////ReportPropertyChanging("ObjectName");
                _ObjectName = value;
                RaisePropertyChanged("ObjectName");
                OnObjectNameChanged();
            }
        }
        private String _ObjectName;
        partial void OnObjectNameChanging(String value);
        partial void OnObjectNameChanged();

        [DataMemberAttribute()]
        public String ObjectValue
        {
            get
            {
                return _ObjectValue;
            }
            set
            {
                OnObjectValueChanging(value);
                ////ReportPropertyChanging("ObjectValue");
                _ObjectValue = value;
                RaisePropertyChanged("ObjectValue");
                OnObjectValueChanged();
            }
        }
        private String _ObjectValue;
        partial void OnObjectValueChanging(String value);
        partial void OnObjectValueChanged();

        [DataMemberAttribute()]
        public String ObjectNotes
        {
            get
            {
                return _ObjectNotes;
            }
            set
            {
                OnObjectNotesChanging(value);
                ////ReportPropertyChanging("ObjectNotes");
                _ObjectNotes = value;
                RaisePropertyChanged("ObjectNotes");
                OnObjectNotesChanged();
            }
        }
        private String _ObjectNotes;
        partial void OnObjectNotesChanging(String value);
        partial void OnObjectNotesChanged();

        [DataMemberAttribute()]
        public String Code
        {
            get
            {
                return _Code;
            }
            set
            {
                OnCodeChanging(value);
                ////ReportPropertyChanging("ObjectNotes");
                _Code = value;
                RaisePropertyChanged("Code");
                OnCodeChanged();
            }
        }
        private String _Code;
        partial void OnCodeChanging(String value);
        partial void OnCodeChanged();

        [DataMemberAttribute()]
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    RaisePropertyChanged("IsChecked");
                }
            }
        }
        private bool _IsChecked = false;

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

        public override bool Equals(object obj)
        {
            Lookup cond = obj as Lookup;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.LookupID == cond.LookupID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(ObjectValue) ? ObjectValue.ToString() : null;
        }

        [DataMemberAttribute()]
        public String Code130
        {
            get
            {
                return _Code130;
            }
            set
            {
                OnCode130Changing(value);
                _Code130 = value;
                RaisePropertyChanged("Code130");
                OnCode130Changed();
            }
        }
        private String _Code130;
        partial void OnCode130Changing(String value);
        partial void OnCode130Changed();

        [DataMemberAttribute()]
        public DateTime? DateActive
        {
            get
            {
                return _DateActive;
            }
            set
            {
                OnDateActiveChanging(value);
                _DateActive = value;
                RaisePropertyChanged("DateActive");
                OnDateActiveChanged();
            }
        }
        private DateTime? _DateActive;
        partial void OnDateActiveChanging(DateTime? value);
        partial void OnDateActiveChanged();

        [DataMemberAttribute()]
        public bool IsActiveLookup
        {
            get
            {
                return _IsActiveLookup;
            }
            set
            {
                OnIsActiveLookupChanging(value);
                _IsActiveLookup = value;
                RaisePropertyChanged("IsActiveLookup");
                OnIsActiveLookupChanged();
            }
        }
        private bool _IsActiveLookup;
        partial void OnIsActiveLookupChanging(bool value);
        partial void OnIsActiveLookupChanged();

        [DataMemberAttribute()]
        public string StaffFullName
        {
            get
            {
                return _StaffFullName;
            }
            set
            {
                OnStaffFullNameChanging(value);
                _StaffFullName = value;
                RaisePropertyChanged("StaffFullName");
                OnStaffFullNameChanged();
            }
        }
        private string _StaffFullName;
        partial void OnStaffFullNameChanging(string value);
        partial void OnStaffFullNameChanged();
    }
}