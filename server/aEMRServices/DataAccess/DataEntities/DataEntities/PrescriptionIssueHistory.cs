using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
/*
 * 20181026 #001 TTM:   BM 0004204: Thêm Property IsOutCatConfirmed để xác định xem bệnh nhân có đồng ý mua thuốc ngoài danh mục không.
 * 20230220 #002 QTD:   Thêm cột lưu thông tin cho DTDT nhà thuốc
 * 20230615 #003 DatTB: Thêm cột user bsi mượn OfficialAccountID
 */
namespace DataEntities
{
    public partial class PrescriptionIssueHistory : NotifyChangedBase
    {
        public enum PrescriptionIssuedCase
        {
            ISSUED_AND_TRANSFER_TO_PHARMACY = 2301,
            ISSUED_AND_DONOT_TRANSFER_TO_PHARMACY = 2302,
            DONOT_ISSUE_AND_DONOT_TRANSFER_TO_PHARMACY = 2303,
            UNKNOW=2300

        }

        //#region Factory Method
     
        ///// Create a new PrescriptionIssueHistory object.
     
        ///// <param name="issueID">Initial value of the IssueID property.</param>
        ///// <param name="issuedDateTime">Initial value of the IssuedDateTime property.</param>
        ////public static PrescriptionIssueHistory CreatePrescriptionIssueHistory(long issueID, DateTime issuedDateTime)
        ////{
        ////    PrescriptionIssueHistory prescriptionIssueHistory = new PrescriptionIssueHistory();
        ////    prescriptionIssueHistory.IssueID = issueID;
        ////    prescriptionIssueHistory.IssuedDateTime = issuedDateTime;
        ////    return prescriptionIssueHistory;
        ////}
        //#endregion

        #region Primitive Properties

        [DataMemberAttribute()]
        public long IssueID
        {
            get
            {
                return _IssueID;
            }
            set
            {
                _IssueID = value;
                RaisePropertyChanged("IssueID");
            }
        }
        private long _IssueID;



        [DataMemberAttribute()]
        public Nullable<long> PrescriptID
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
        private Nullable<long> _PrescriptID;
        partial void OnPrescriptIDChanging(Nullable<long> value);
        partial void OnPrescriptIDChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<long> ServiceRecID
        {
            get
            {
                return _ServiceRecID;
            }
            set
            {
                OnServiceRecIDChanging(value);
                _ServiceRecID = value;
                RaisePropertyChanged("ServiceRecID");
                OnServiceRecIDChanged();
            }
        }
        private Nullable<long> _ServiceRecID;
        partial void OnServiceRecIDChanging(Nullable<long> value);
        partial void OnServiceRecIDChanged();


        [DataMemberAttribute()]
        public Nullable<long> PtRegDetailID
        {
            get
            {
                return _PtRegDetailID;
            }
            set
            {
                OnPtRegDetailIDChanging(value);
                _PtRegDetailID = value;
                RaisePropertyChanged("PtRegDetailID");
                OnPtRegDetailIDChanged();
            }
        }
        private Nullable<long> _PtRegDetailID;
        partial void OnPtRegDetailIDChanging(Nullable<long> value);
        partial void OnPtRegDetailIDChanged();

        
     
        [DataMemberAttribute()]
        public Nullable<Int32> AppointmentID
        {
            get
            {
                return _AppointmentID;
            }
            set
            {
                OnAppointmentIDChanging(value);
                _AppointmentID = value;
                RaisePropertyChanged("AppointmentID");
                OnAppointmentIDChanged();
            }
        }
        private Nullable<Int32> _AppointmentID;
        partial void OnAppointmentIDChanging(Nullable<Int32> value);
        partial void OnAppointmentIDChanged();
        
     
        [DataMemberAttribute()]
        public Nullable<DateTime> IssuedDateTime
        {
            get
            {
                return _IssuedDateTime;
            }
            set
            {
                OnIssuedDateTimeChanging(value);
                _IssuedDateTime = value;
                RaisePropertyChanged("IssuedDateTime");
                OnIssuedDateTimeChanged();
            }
        }
        private Nullable<DateTime> _IssuedDateTime;
        partial void OnIssuedDateTimeChanging(Nullable<DateTime> value);
        partial void OnIssuedDateTimeChanged();


        [DataMemberAttribute()]
        public Nullable<DateTime> OrigIssuedDateTime
        {
            get
            {
                return _OrigIssuedDateTime;
            }
            set
            {
                OnOrigIssuedDateTimeChanging(value);
                _OrigIssuedDateTime = value;
                RaisePropertyChanged("OrigIssuedDateTime");
                OnOrigIssuedDateTimeChanged();
            }
        }
        private Nullable<DateTime> _OrigIssuedDateTime;
        partial void OnOrigIssuedDateTimeChanging(Nullable<DateTime> value);
        partial void OnOrigIssuedDateTimeChanged();


        [DataMemberAttribute()]
        public Int64 IssuerStaffID
        {
            get
            {
                return _IssuerStaffID;
            }
            set
            {
                OnIssuerStaffIDChanging(value);
                _IssuerStaffID = value;
                RaisePropertyChanged("IssuerStaffID");
                OnIssuerStaffIDChanged();
            }
        }
        private Int64 _IssuerStaffID;
        partial void OnIssuerStaffIDChanging(Int64 value);
        partial void OnIssuerStaffIDChanged();

        
        [DataMemberAttribute()]
        public long SecretaryStaffID
        {
            get
            {
                return _SecretaryStaffID;
            }
            set
            {
                OnSecretaryStaffIDChanging(value);
                _SecretaryStaffID = value;
                RaisePropertyChanged("SecretaryStaffID");
                OnSecretaryStaffIDChanged();
            }
        }
        private long _SecretaryStaffID;
        partial void OnSecretaryStaffIDChanging(long value);
        partial void OnSecretaryStaffIDChanged();


        [DataMemberAttribute()]
        public Nullable<long> ReIssuerStaffID
        {
            get
            {
                return _ReIssuerStaffID;
            }
            set
            {
                OnReIssuerStaffIDChanging(value);
                _ReIssuerStaffID = value;
                RaisePropertyChanged("ReIssuerStaffID");
                OnReIssuerStaffIDChanged();
            }
        }
        private Nullable<long> _ReIssuerStaffID;
        partial void OnReIssuerStaffIDChanging(Nullable<long> value);
        partial void OnReIssuerStaffIDChanged();
        
     
        [DataMemberAttribute()]
        public Nullable<Byte> TimesNumberIsPrinted
        {
            get
            {
                return _TimesNumberIsPrinted;
            }
            set
            {
                OnTimesNumberIsPrintedChanging(value);
                _TimesNumberIsPrinted = value;
                RaisePropertyChanged("TimesNumberIsPrinted");
                OnTimesNumberIsPrintedChanged();
            }
        }
        private Nullable<Byte> _TimesNumberIsPrinted;
        partial void OnTimesNumberIsPrintedChanging(Nullable<Byte> value);
        partial void OnTimesNumberIsPrintedChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Int64> V_PrescriptionNotes
        {
            get
            {
                return _V_PrescriptionNotes;
            }
            set
            {
                OnV_PrescriptionNotesChanging(value);
                _V_PrescriptionNotes = value;
                RaisePropertyChanged("V_PrescriptionNotes");
                OnV_PrescriptionNotesChanged();
            }
        }
        private Nullable<Int64> _V_PrescriptionNotes;
        partial void OnV_PrescriptionNotesChanging(Nullable<Int64> value);
        partial void OnV_PrescriptionNotesChanged();

     
        
     
        [DataMemberAttribute()]
        public Nullable<Int64> V_PrescriptionIssuedCase
        {
            get
            {
                return _V_PrescriptionIssuedCase;
            }
            set
            {
                OnV_PrescriptionIssuedCaseChanging(value);
                _V_PrescriptionIssuedCase = value;
                RaisePropertyChanged("V_PrescriptionIssuedCase");
                OnV_PrescriptionIssuedCaseChanged();
            }
        }
        private Nullable<Int64> _V_PrescriptionIssuedCase;
        partial void OnV_PrescriptionIssuedCaseChanging(Nullable<Int64> value);
        partial void OnV_PrescriptionIssuedCaseChanged();

        [DataMemberAttribute()]
        public string OrigCreatorDoctorNames
        {
            get
            {
                return _OrigCreatorDoctorNames;
            }
            set
            {
                OnOrigCreatorDoctorNamesChanging(value);
                _OrigCreatorDoctorNames = value;
                RaisePropertyChanged("OrigCreatorDoctorNames");
                OnOrigCreatorDoctorNamesChanged();
            }
        }
        private string _OrigCreatorDoctorNames="";
        partial void OnOrigCreatorDoctorNamesChanging(string value);
        partial void OnOrigCreatorDoctorNamesChanged();

        #endregion

        #region Navigation Properties

     
        
     
       
        [DataMemberAttribute()]
        public PatientServiceRecord PatientServiceRecord
        {
            get
            {
                return _PatientServiceRecord;
            }
            set
            {
                if (_PatientServiceRecord != value)
                {
                    _PatientServiceRecord = value;
                    RaisePropertyChanged("PatientServiceRecord");
                }
            }
        }
        private PatientServiceRecord _PatientServiceRecord;
     
        [DataMemberAttribute()]
        public Prescription Prescription
        {
            get
            {
                return _Prescription;
            }
            set
            {
                if (_Prescription != value)
                {
                    _Prescription = value;
                    RaisePropertyChanged("Prescription");
                }
            }
        }
        private Prescription _Prescription;
        #endregion

     
        
       
        [DataMemberAttribute()]
        public Lookup LookupPrescriptionNotes
        {
            get
            {
                return _LookupPrescriptionNotes;
            }
            set
            {
                if (_LookupPrescriptionNotes != value)
                {
                    _LookupPrescriptionNotes = value;
                    RaisePropertyChanged("LookupPrescriptionNotes");
                }
            }
        }
        private Lookup _LookupPrescriptionNotes;
     
        [DataMemberAttribute()]
        public Lookup LookupPrescriptionIssuedCase
        {
            get
            {
                return _LookupPrescriptionIssuedCase;
            }
            set
            {
                if (_LookupPrescriptionIssuedCase != value)
                {
                    _LookupPrescriptionIssuedCase = value;
                    RaisePropertyChanged("LookupPrescriptionIssuedCase");
                }
            }
        }
        private Lookup _LookupPrescriptionIssuedCase;
        
        [DataMemberAttribute()]
        public Staff ObjIssuerStaffID
        {
            get
            {
                return _ObjIssuerStaffID;
            }
            set
            {
                if (_ObjIssuerStaffID != value)
                {
                    _ObjIssuerStaffID = value;
                    RaisePropertyChanged("ObjIssuerStaffID");
                }
            }
        }
        private Staff _ObjIssuerStaffID;

        
        [DataMemberAttribute()]
        public Staff ObjReIssuerStaffID
        {
            get
            {
                return _ObjReIssuerStaffID;
            }
            set
            {
                if (_ObjReIssuerStaffID != value)
                {
                    _ObjReIssuerStaffID = value;
                    RaisePropertyChanged("ObjReIssuerStaffID");
                }
            }
        }
        private Staff _ObjReIssuerStaffID;


        [DataMemberAttribute()]
        public Int16 StoreServiceSeqNum
        {
            get
            {
                return _StoreServiceSeqNum;
            }
            set
            {
                if (_StoreServiceSeqNum != value)
                {
                    _StoreServiceSeqNum = value;
                    RaisePropertyChanged("StoreServiceSeqNum");
                }
            }
        }
        private Int16 _StoreServiceSeqNum;


        [DataMemberAttribute()]
        public Byte StoreServiceSeqNumType
        {
            get
            {
                return _StoreServiceSeqNumType;
            }
            set
            {
                if (_StoreServiceSeqNumType != value)
                {
                    _StoreServiceSeqNumType = value;
                    RaisePropertyChanged("StoreServiceSeqNumType");
                }
            }
        }
        private Byte _StoreServiceSeqNumType;



        [DataMemberAttribute()]
        public Int32 IX_IssuedDateTime 
        {
            get
            {
                if (IssuedDateTime == null)
                    IssuedDateTime = DateTime.Now;

                return IssuedDateTime.Value.Year * 10000 + IssuedDateTime.Value.Month * 100 + IssuedDateTime.Value.Day;
            }
        }


        #region IEditableObject Members
        private PrescriptionIssueHistory _tempPrescriptionHistory;
        public void BeginEdit()
        {
            _tempPrescriptionHistory = (PrescriptionIssueHistory)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempPrescriptionHistory)
                CopyFrom(_tempPrescriptionHistory);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PrescriptionIssueHistory p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        /// <summary>
        /// 31-08-2012 Dinh
        /// Thêm trạng thái để phân biệt nội trú và ngoại trú
        /// </summary>
        private RegistrationType _RegistrationType;
        [DataMemberAttribute()]
        public RegistrationType RegistrationType
        {
            get
            {
                return _RegistrationType;
            }
            set
            {
                _RegistrationType = value;
                RaisePropertyChanged("RegistrationType");
            }
        }


        private AllLookupValues.RegistrationType _V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU;
        [DataMemberAttribute()]
        public AllLookupValues.RegistrationType V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                RaisePropertyChanged("V_RegistrationType");
            }
        }
        
        //▼====== #001
        private bool _IsOutCatConfirmed;
        [DataMemberAttribute()]
        public bool IsOutCatConfirmed
        {
            get
            {
                return _IsOutCatConfirmed;
            }
            set
            {
                _IsOutCatConfirmed = value;
                RaisePropertyChanged("IsOutCatConfirmed");
            }
        }
        //▲====== #001
        private long _V_PrescriptionIssuedType = (long)AllLookupValues.V_PrescriptionIssuedType.Ngoai_Tru;
        [DataMemberAttribute()]
        public long V_PrescriptionIssuedType
        {
            get
            {
                return _V_PrescriptionIssuedType;
            }
            set
            {
                _V_PrescriptionIssuedType = value;
                RaisePropertyChanged("V_PrescriptionIssuedType");
            }
        }

        //▼====: #002
        private long _DQGReportID = 0;
        [DataMemberAttribute()]
        public long DQGReportID
        {
            get { return _DQGReportID; }
            set
            {
                _DQGReportID = value;
                RaisePropertyChanged("DQGReportID");
            }
        }
        private string _CancelReason;
        public string CancelReason
        {
            get { return _CancelReason; }
            set
            {
                _CancelReason = value;
                RaisePropertyChanged("CancelReason");
            }
        }
        private bool _IsCanSelect;
        public bool IsCanSelect
        {
            get { return _IsCanSelect; }
            set
            {
                _IsCanSelect = value;
                RaisePropertyChanged("IsCanSelect");
            }
        }
        //▲====: #002
        //▼==== #003
        [DataMemberAttribute()]
        public long? UserOfficialAccountID
        {
            get
            {
                return _UserOfficialAccountID;
            }
            set
            {
                _UserOfficialAccountID = value;
                RaisePropertyChanged("UserOfficialAccountID");
            }
        }
        private long? _UserOfficialAccountID;
        //▲==== #003
    }
}
