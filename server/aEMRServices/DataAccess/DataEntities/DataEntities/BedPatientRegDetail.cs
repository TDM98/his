using System;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using System.ComponentModel;

namespace DataEntities
{
    public partial class BedPatientRegDetail : EntityBase
    {
        public BedPatientRegDetail()
            : base()
        {

        }
        private long _ptBedAllocRegDetailID;
        [DataMemberAttribute()]
        public long PtBedAllocRegDetailID
        {
            get
            {
                return _ptBedAllocRegDetailID;
            }
            set
            {
                _ptBedAllocRegDetailID = value;
                RaisePropertyChanged("PtBedAllocRegDetailID");
            }
        }
        public override bool Equals(object obj)
        {
            var info = obj as BedPatientRegDetail;
            if (info == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return this.PtBedAllocRegDetailID > 0 && this.PtBedAllocRegDetailID == info.PtBedAllocRegDetailID;
        }

        public override int GetHashCode()
        {
            return this.PtBedAllocRegDetailID.GetHashCode();
        }

        private long _ptRegDetailID;
        [DataMemberAttribute()]
        public long PtRegDetailID
        {
            get
            {
                return _ptRegDetailID;
            }
            set
            {
                _ptRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
            }
        }

        private long _bedPatientID;
        [DataMemberAttribute()]
        public long BedPatientID
        {
            get
            {
                return _bedPatientID;
            }
            set
            {
                _bedPatientID = value;
                RaisePropertyChanged("BedPatientID");
            }
        }
        private DateTime _recCreatedDate;
        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _recCreatedDate;
            }
            set
            {
                _recCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
            }
        }

        private DateTime _billFromDate;
        [DataMemberAttribute()]
        public DateTime BillFromDate
        {
            get
            {
                return _billFromDate;
            }
            set
            {
                _billFromDate = value;
                RaisePropertyChanged("BillFromDate");
            }
        }

        private DateTime _billToDate;
        [DataMemberAttribute()]
        public DateTime BillToDate
        {
            get
            {
                return _billToDate;
            }
            set
            {
                _billToDate = value;
                RaisePropertyChanged("BillToDate");
            }
        }

        private bool _isDeleted;
        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }
            set
            {
                _isDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }

        private BedPatientAllocs _bedPatientAlloc;
        [DataMemberAttribute()]
        public BedPatientAllocs BedPatientAlloc
        {
            get
            {
                return _bedPatientAlloc;
            }
            set
            {
                _bedPatientAlloc = value;
                RaisePropertyChanged("BedPatientAlloc");
            }
        }

        private PatientRegistrationDetail _patientRegistrationDetail;
        [DataMemberAttribute()]
        public PatientRegistrationDetail PatientRegistrationDetail
        {
            get
            {
                return _patientRegistrationDetail;
            }
            set
            {
                _patientRegistrationDetail = value;
                RaisePropertyChanged("PatientRegistrationDetail");
            }
        }
    }
}