using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Service.Core.Common;
using eHCMS.Services.Core.Base;

namespace DataEntities
{

    public partial class PatientApptPCLRequests : EntityBase
    {
        #region Primitive Properties
        [DataMemberAttribute()]
        public long PatientPCLReqID
        {
            get
            {
                return _PatientPCLReqID;
            }
            set
            {
                if (_PatientPCLReqID != value)
                {
                    OnPatientPCLReqIDChanging(value);
                    _PatientPCLReqID = value;
                    RaisePropertyChanged("PatientPCLReqID");
                    OnPatientPCLReqIDChanged();
                }
            }
        }
        private long _PatientPCLReqID;
        partial void OnPatientPCLReqIDChanging(long value);
        partial void OnPatientPCLReqIDChanged();


        [DataMemberAttribute()]
        public long AppointmentID
        {
            get
            {
                return _AppointmentID;
            }
            set
            {
                if (_AppointmentID != value)
                {
                    OnAppointmentIDChanging(value);
                    _AppointmentID = value;
                    RaisePropertyChanged("AppointmentID");
                    OnAppointmentIDChanged();
                }
            }
        }
        private long _AppointmentID;
        partial void OnAppointmentIDChanging(long value);
        partial void OnAppointmentIDChanged();


        [DataMemberAttribute()]
        public long ReqFromDeptLocID
        {
            get
            {
                return _ReqFromDeptLocID;
            }
            set
            {
                OnReqFromDeptLocIDChanging(value);
                _ReqFromDeptLocID = value;
                RaisePropertyChanged("ReqFromDeptLocID");
                OnReqFromDeptLocIDChanged();
            }
        }
        private long _ReqFromDeptLocID;
        partial void OnReqFromDeptLocIDChanging(long value);
        partial void OnReqFromDeptLocIDChanged();


        [DataMemberAttribute()]
        public String ReqFromDeptLocIDName
        {
            get
            {
                return _ReqFromDeptLocIDName;
            }
            set
            {
                OnReqFromDeptLocIDNameChanging(value);
                _ReqFromDeptLocIDName = value;
                RaisePropertyChanged("ReqFromDeptLocIDName");
                OnReqFromDeptLocIDNameChanged();
            }
        }
        private String _ReqFromDeptLocIDName;
        partial void OnReqFromDeptLocIDNameChanging(String value);
        partial void OnReqFromDeptLocIDNameChanged();


        [DataMemberAttribute()]
        public string DoctorComments
        {
            get
            {
                return _DoctorComments;
            }
            set
            {
                OnDoctorCommentsChanging(value);
                _DoctorComments = value;
                RaisePropertyChanged("DoctorComments");
                OnDoctorCommentsChanged();
            }
        }
        private string _DoctorComments;
        partial void OnDoctorCommentsChanging(string value);
        partial void OnDoctorCommentsChanged();


        [DataMemberAttribute()]
        public string PCLRequestNumID
        {
            get
            {
                return _PCLRequestNumID;
            }
            set
            {
                if (_PCLRequestNumID != value)
                {
                    OnPCLRequestNumIDChanging(value);
                    _PCLRequestNumID = value;
                    RaisePropertyChanged("PCLRequestNumID");
                    OnPCLRequestNumIDChanged();
                }
            }
        }
        private string _PCLRequestNumID;
        partial void OnPCLRequestNumIDChanging(string value);
        partial void OnPCLRequestNumIDChanged();


        [DataMemberAttribute()]
        public Nullable<long> StaffID
        {
            get
            {
                return _StaffID;
            }
            set
            {
                if (_StaffID != value)
                {
                    OnStaffIDChanging(value);
                    _StaffID = value;
                    RaisePropertyChanged("StaffID");
                    OnStaffIDChanged();
                }
            }
        }
        private Nullable<long> _StaffID;
        partial void OnStaffIDChanging(Nullable<long> value);
        partial void OnStaffIDChanged();

        [DataMemberAttribute()]
        public Staff ObjStaffID
        {
            get
            {
                return _ObjStaffID;
            }
            set
            {
                if (_ObjStaffID != value)
                {
                    OnObjStaffIDChanging(value);
                    _ObjStaffID = value;
                    RaisePropertyChanged("ObjStaffID");
                    OnObjStaffIDChanged();
                }
            }
        }
        private Staff _ObjStaffID;
        partial void OnObjStaffIDChanging(Staff value);
        partial void OnObjStaffIDChanged();



        [DataMemberAttribute()]
        public String Diagnosis
        {
            get
            {
                return _Diagnosis;
            }
            set
            {
                if (_Diagnosis != value)
                {
                    OnDiagnosisChanging(value);
                    _Diagnosis = value;
                    RaisePropertyChanged("Diagnosis");
                    OnDiagnosisChanged();
                }
            }
        }
        private String _Diagnosis;
        partial void OnDiagnosisChanging(String value);
        partial void OnDiagnosisChanged();

        
        [DataMemberAttribute()]
        public string ApptPCLNote
        {
            get
            {
                return _ApptPCLNote;
            }
            set
            {
                if (_ApptPCLNote != value)
                {
                    OnApptPCLNoteChanging(value);
                    _ApptPCLNote = value;
                    RaisePropertyChanged("ApptPCLNote");
                    OnApptPCLNoteChanged();
                }
            }
        }
        private string _ApptPCLNote;
        partial void OnApptPCLNoteChanging(String value);
        partial void OnApptPCLNoteChanged();

        [DataMemberAttribute()]
        public Int64 V_PCLMainCategory
        {
            get { return _V_PCLMainCategory; }
            set
            {
                if (_V_PCLMainCategory != value)
                {
                    OnV_PCLMainCategoryChanging(value);
                    _V_PCLMainCategory = value;
                    RaisePropertyChanged("V_PCLMainCategory");
                    OnV_PCLMainCategoryChanged();
                }
            }
        }
        private Int64 _V_PCLMainCategory;
        partial void OnV_PCLMainCategoryChanging(Int64 value);
        partial void OnV_PCLMainCategoryChanged();

        #endregion

        [DataMemberAttribute()]
        public PatientAppointment PatientAppointment { get; set; }
        [DataMemberAttribute()]
        public ObservableCollection<PatientApptPCLRequestDetails> ObjPatientApptPCLRequestDetailsList
        {
            get
            {
                return _ObjPatientApptPCLRequestDetailsList;
            }
            set
            {
                if (_ObjPatientApptPCLRequestDetailsList != value)
                {
                    OnObjPatientApptPCLRequestDetailsListChanging(value);
                    _ObjPatientApptPCLRequestDetailsList = value;
                    RaisePropertyChanged("ObjPatientApptPCLRequestDetailsList");
                    OnObjPatientApptPCLRequestDetailsListChanged();
                }
            }
        }
        private ObservableCollection<PatientApptPCLRequestDetails> _ObjPatientApptPCLRequestDetailsList;
        partial void OnObjPatientApptPCLRequestDetailsListChanging(ObservableCollection<PatientApptPCLRequestDetails> value);
        partial void OnObjPatientApptPCLRequestDetailsListChanged();

        public override bool Equals(object obj)
        {
            PatientApptPCLRequests info = obj as PatientApptPCLRequests;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.PatientPCLReqID > 0 && this.PatientPCLReqID == info.PatientPCLReqID;
        }


        public override int GetHashCode()
        {
            return this.PatientPCLReqID.GetHashCode();
        }



        private EntityState _EntityState = EntityState.NEW;
        [DataMemberAttribute()]
        public override EntityState EntityState
        {
            get
            {
                return _EntityState;
            }
            set
            {
                _EntityState = value;
                RaisePropertyChanged("EntityState");
            }
        }


        private string _ICD10List;
        [DataMemberAttribute()]
        public string ICD10List
        {
            get
            {
                return _ICD10List;
            }
            set
            {
                if (_ICD10List != value)
                {
                    _ICD10List = value;
                    RaisePropertyChanged("ICD10List");
                }
            }
        }

        public bool CheckNewPCL(PatientApptPCLRequests EditingApptPCLRequest)
        {
            if (EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList == null
                || EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Count < 1)
                return false;

            foreach (var item in EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList)
            {
                if (item.PCLReqItemID < 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
