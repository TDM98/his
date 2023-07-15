using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class PrescriptionIssueHistory : NotifyChangedBase
    {

        #region Primitive Properties

        [DataMemberAttribute()]
        public Nullable<long> IssueIDOld
        {
            get
            {
                return _IssueIDOld;
            }
            set
            {
                _IssueIDOld = value;
                RaisePropertyChanged("IssueIDOld");
            }
        }
        private Nullable<long> _IssueIDOld;


        [DataMemberAttribute()]
        public Nullable<long> OriginalPrescriptID
        {
            get
            {
                return _OriginalPrescriptID;
            }
            set
            {
                OnOriginalPrescriptIDChanging(value);
                _OriginalPrescriptID = value;
                RaisePropertyChanged("OriginalPrescriptID");
                OnOriginalPrescriptIDChanged();
            }
        }
        private Nullable<long> _OriginalPrescriptID;
        partial void OnOriginalPrescriptIDChanging(Nullable<long> value);
        partial void OnOriginalPrescriptIDChanged();

        [DataMemberAttribute()]
        public string Creator
        {
            get
            {
                return _Creator;
            }
            set
            {
                OnCreatorChanging(value);
                _Creator = value;
                RaisePropertyChanged("Creator");
                OnCreatorChanged();
            }
        }
        private string _Creator;
        partial void OnCreatorChanging(string value);
        partial void OnCreatorChanged();


        [DataMemberAttribute()]
        public long HisID
        {
            get
            {
                return _HisID;
            }
            set
            {
                OnHisIDChanging(value);
                _HisID = value;
                RaisePropertyChanged("HisID");
                OnHisIDChanged();
            }
        }
        private long _HisID;
        partial void OnHisIDChanging(long value);
        partial void OnHisIDChanged();


        [DataMemberAttribute()]
        public string Author
        {
            get
            {
                return _Author;
            }
            set
            {
                OnAuthorChanging(value);
                _Author = value;
                RaisePropertyChanged("Author");
                OnAuthorChanged();
            }
        }
        private string _Author;
        partial void OnAuthorChanging(string value);
        partial void OnAuthorChanged();
     
        [DataMemberAttribute()]
        public string PrescriptionNotes
        {
            get
            {
                return _PrescriptionNotes;
            }
            set
            {
                OnPrescriptionNotesChanging(value);
                _PrescriptionNotes = value;
                RaisePropertyChanged("PrescriptionNotes");
                OnPrescriptionNotesChanged();
            }
        }
        private string _PrescriptionNotes;
        partial void OnPrescriptionNotesChanging(string value);
        partial void OnPrescriptionNotesChanged();


        [DataMemberAttribute()]
        public long DeptLocID
        {
            get
            {
                return _DeptLocID;
            }
            set
            {
                OnDeptLocIDChanging(value);
                _DeptLocID = value;
                RaisePropertyChanged("DeptLocID");
                OnDeptLocIDChanged();
            }
        }
        private long _DeptLocID;
        partial void OnDeptLocIDChanging(long value);
        partial void OnDeptLocIDChanged();

        #endregion
    }
}
