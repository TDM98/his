using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    [DataContract]
    public partial class PrescriptionTemplate : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new PrescriptionTemplate object.
     
        /// <param name="prescriptID">Initial value of the PrescriptID property.</param>
        public static PrescriptionTemplate CreatePrescriptionTemplate(long PrescriptionTemplateID)
        {
            PrescriptionTemplate PrescriptionTemplate = new PrescriptionTemplate();
            PrescriptionTemplate.PrescriptionTemplateID = PrescriptionTemplateID;
            return PrescriptionTemplate;
        }

        #endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long PrescriptionTemplateID
        {
            get
            {
                return _PrescriptionTemplateID;
            }
            set
            {
                if (_PrescriptionTemplateID != value)
                {
                    OnPrescriptionTemplateIDChanging(value);
                    _PrescriptionTemplateID = value;
                    RaisePropertyChanged("PrescriptionTemplateID");
                    OnPrescriptionTemplateIDChanged();
                }
            }
        }
        private long _PrescriptionTemplateID;
        partial void OnPrescriptionTemplateIDChanging(long value);
        partial void OnPrescriptionTemplateIDChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<long> DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                OnDoctorStaffIDChanging(value);
                _DoctorStaffID = value;
                RaisePropertyChanged("DoctorStaffID");
                OnDoctorStaffIDChanged();
            }
        }
        private Nullable<long> _DoctorStaffID;
        partial void OnDoctorStaffIDChanging(Nullable<long> value);
        partial void OnDoctorStaffIDChanged();

        [DataMemberAttribute()]
        public long PrescriptID
        {
            get
            {
                return _PrescriptID;
            }
            set
            {
                OnPrescriptIDChanging(value);
                _PrescriptID = value;
                RaisePropertyChanged("PrescriptID");
                OnPrescriptIDChanged();
            }
        }
        private long _PrescriptID;
        partial void OnPrescriptIDChanging(long value);
        partial void OnPrescriptIDChanged();


        [DataMemberAttribute()]
        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                OnCommentChanging(value);
                _Comment = value;
                RaisePropertyChanged("Comment");
                OnCommentChanged();
            }
        }
        private string _Comment;
        partial void OnCommentChanging(string value);
        partial void OnCommentChanged();

     
        [DataMemberAttribute()]
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                OnStaffIDChanging(value);
                _StaffID = value;
                RaisePropertyChanged("StaffID");
                OnStaffIDChanged();
            }
        }
        private long _StaffID ;
        partial void OnStaffIDChanging(long value);
        partial void OnStaffIDChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Boolean> IsMarkDeleted
        {
            get
            {
                return _IsMarkDeleted;
            }
            set
            {
                OnIsMarkDeletedChanging(value);
                _IsMarkDeleted = value;
                RaisePropertyChanged("IsMarkDeleted");
                OnIsMarkDeletedChanged();
            }
        }
        private Nullable<Boolean> _IsMarkDeleted;
        partial void OnIsMarkDeletedChanging(Nullable<Boolean> value);
        partial void OnIsMarkDeletedChanged();

     
        
     
       
        #endregion

        #region Navigation Properties

        private Prescription _prescription;
        [DataMemberAttribute()]
        public Prescription prescription
        {
            get
            {
                return _prescription;
            }
            set
            {
                if (_prescription != value)
                {
                    _prescription = value;
                    RaisePropertyChanged("prescription");
                }
            }
        }

        private ObservableCollection<OutwardDMedRscr> _OutwardDMedRscrs;
        [DataMemberAttribute()]
        public ObservableCollection<OutwardDMedRscr> OutwardDMedRscrs
        {
            get
            {
                return _OutwardDMedRscrs;
            }
            set
            {
                if (_OutwardDMedRscrs != value)
                {
                    _OutwardDMedRscrs = value;
                    RaisePropertyChanged("OutwardDMedRscrs");
                }
            }
        }

      
     
        private Staff _Doctor;
        [DataMemberAttribute()]
        public Staff Doctor
        {
            get
            {
                return _Doctor;
            }
            set
            {
                if (_Doctor != value)
                {
                    _Doctor = value;
                    RaisePropertyChanged("Doctor");
                }
            }
        }


        private Staff _SecretaryStaff;
        [DataMemberAttribute()]
        public Staff SecretaryStaff
        {
            get
            {
                return _SecretaryStaff;
            }
            set
            {
                if (_SecretaryStaff != value)
                {
                    _SecretaryStaff = value;
                    RaisePropertyChanged("SecretaryStaff");
                }
            }
        }
        
     
        [DataMemberAttribute()]
        public Int64 CreatorStaffID
        {
            get
            {
                return _CreatorStaffID;
            }
            set
            {
                if(_CreatorStaffID!=value)
                {
                    OnCreatorStaffIDChanging(value);
                    _CreatorStaffID = value;
                    RaisePropertyChanged("CreatorStaffID");
                    OnCreatorStaffIDChanged();
                }
            }
        }
        private Int64 _CreatorStaffID;
        partial void OnCreatorStaffIDChanging(Int64 value);
        partial void OnCreatorStaffIDChanged();


        [DataMemberAttribute()]
        public DateTime RecDateCreated
        {
            get { return _RecDateCreated; }
            set
            {
                if(_RecDateCreated!=value)
                {
                    OnRecDateCreatedChanging(value);
                    _RecDateCreated = value;
                    RaisePropertyChanged("RecDateCreated");
                    OnRecDateCreatedChanged();
                }
            }
        }
        private DateTime _RecDateCreated;
        partial void OnRecDateCreatedChanging(DateTime value);
        partial void OnRecDateCreatedChanged();



        #endregion

       

    }
}
