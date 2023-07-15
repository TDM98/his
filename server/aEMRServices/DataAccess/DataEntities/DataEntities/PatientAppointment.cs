using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Service.Core.Common;

namespace DataEntities
{
    [DataContract]
    public partial class PatientAppointment : EntityBase, IEditableObject
    {
        public PatientAppointment()
            : base()
        {
            V_ApptStatus = AllLookupValues.ApptStatus.UNKNOWN;
            Staff = new Staff();
        }

        private PatientAppointment _tempAppointment;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempAppointment = (PatientAppointment)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempAppointment)
                CopyFrom(_tempAppointment);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(PatientAppointment p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method
        /// Create a new Appointment object.

        /// <param name="appointmentID">Initial value of the ApointmentID property.</param>
        /// <param name="dateCreated">Initial value of the DateCreated property.</param>
        /// <param name="dateModified">Initial value of the DateModified property.</param>
        public static PatientAppointment CreateAppointment(Int32 appointmentID, DateTime dateCreated, DateTime dateModified)
        {
            PatientAppointment appointment = new PatientAppointment();
            appointment.AppointmentID = appointmentID;
            appointment.RecDateCreated = dateCreated;
            appointment.DateModified = dateModified;
            return appointment;
        }

        #endregion

        private long _AppointmentID;
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
                    _AppointmentID = value;
                    RaisePropertyChanged("AppointmentID");
                }
            }
        }

        private long? _StaffID;
        [DataMemberAttribute()]
        public long? StaffID
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


        private Staff _Staff;
        [DataMemberAttribute()]
        public Staff Staff
        {
            get
            {
                return _Staff;
            }
            set
            {

                _Staff = value;
                RaisePropertyChanged("Staff");
            }
        }


        private Staff _DoctorStaff;
        [DataMemberAttribute()]
        public Staff DoctorStaff
        {
            get
            {
                return _DoctorStaff;
            }
            set
            {

                _DoctorStaff = value;
                RaisePropertyChanged("DoctorStaff");
            }
        }


        private long? _doctorStaffID;
        [DataMemberAttribute()]
        public long? DoctorStaffID
        {
            get
            {
                return _doctorStaffID;
            }
            set
            {

                _doctorStaffID = value;
                RaisePropertyChanged("DoctorStaffID");
            }
        }
        private long? _serviceRecID;
        [DataMemberAttribute()]
        public long? ServiceRecID
        {
            get
            {
                return _serviceRecID;
            }
            set
            {

                _serviceRecID = value;
                RaisePropertyChanged("ServiceRecID");
            }
        }
        //private int? _NumDay;
        //[DataMemberAttribute()]
        //public int? NumDay
        //{
        //    get
        //    {
        //        return _NumDay;
        //    }
        //    set
        //    {

        //        _NumDay = value;
        //        RaisePropertyChanged("NumDay");
        //    }
        //}

        private int? _NDay;
        [DataMemberAttribute()]
        public int? NDay
        {
            get
            {
                return _NDay;
            }
            set
            {

                _NDay = value;
                RaisePropertyChanged("NDay");
            }
        }

        private long _patientID;
        [DataMemberAttribute()]
        public long PatientID
        {
            get
            {
                return _patientID;
            }
            set
            {

                _patientID = value;
                RaisePropertyChanged("PatientID");
            }
        }
        private Patient _Patient;
        [DataMemberAttribute()]
        public Patient Patient
        {
            get
            {
                return _Patient;
            }
            set
            {
                if (_Patient != value)
                {
                    _Patient = value;
                    RaisePropertyChanged("Patient");
                }
            }
        }

        private AllLookupValues.ApptStatus _V_ApptStatus;
        [DataMemberAttribute()]
        public AllLookupValues.ApptStatus V_ApptStatus
        {
            get
            {
                return _V_ApptStatus;
            }
            set
            {
                if (_V_ApptStatus != value)
                {
                    ValidateProperty("V_ApptStatus", value);
                    _V_ApptStatus = value;
                    RaisePropertyChanged("V_ApptStatus");
                }
            }
        }

        private Lookup _ApptStatus;
        [Required(ErrorMessage = "Status is required")]
        [DataMemberAttribute()]
        public Lookup ApptStatus
        {
            get
            {
                return _ApptStatus;
            }
            set
            {
                if (_ApptStatus != value)
                {
                    ValidateProperty("ApptStatus", value);
                    _ApptStatus = value;
                    RaisePropertyChanged("ApptStatus");
                }
            }
        }

        private long _PtRegistrationID;
        [DataMemberAttribute()]
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID != value)
                {
                    ValidateProperty("PtRegistrationID", value);
                    _PtRegistrationID = value;
                    RaisePropertyChanged("PtRegistrationID");
                }
            }
        }

        private bool _isHIAppt;
        [DataMemberAttribute()]
        public bool isHIAppt
        {
            get
            {
                return _isHIAppt;
            }
            set
            {
                if (_isHIAppt != value)
                {
                    ValidateProperty("isHIAppt", value);
                    _isHIAppt = value;
                    RaisePropertyChanged("isHIAppt");
                }
            }
        }

        private bool _CreatedByInPtRegis;
        [DataMemberAttribute()]
        public bool CreatedByInPtRegis
        {
            get
            {
                return _CreatedByInPtRegis;
            }
            set
            {
                if (_CreatedByInPtRegis != value)
                {
                    ValidateProperty("CreatedByInPtRegis", value);
                    _CreatedByInPtRegis = value;
                    RaisePropertyChanged("CreatedByInPtRegis");
                }
            }
        }

        private DateTime _RecDateCreated;
        [DataMemberAttribute()]
        public DateTime RecDateCreated
        {
            get
            {
                return _RecDateCreated;
            }
            set
            {
                if (_RecDateCreated != value)
                {
                    _RecDateCreated = value;
                    RaisePropertyChanged("RecDateCreated");
                }
            }
        }

        private DateTime? _DateModified;
        [DataMemberAttribute()]
        public DateTime? DateModified
        {
            get
            {
                return _DateModified;
            }
            set
            {
                if (_DateModified != value)
                {
                    _DateModified = value;
                    RaisePropertyChanged("DateModified");
                }
            }
        }

        private DateTime? _ApptDate;
        [Required(ErrorMessage = "Chọn ngày hẹn")]
        [CustomValidation(typeof(PatientAppointment), "ValidateApptDate")]
        [DataMemberAttribute()]
        public DateTime? ApptDate
        {
            get
            {
                return _ApptDate;
            }
            set
            {
                if (_ApptDate != value)
                {
                    ValidateProperty("ApptDate", value);
                    _ApptDate = value;
                    RaisePropertyChanged("ApptDate");
                }
            }
        }

        public static ValidationResult ValidateApptDate(DateTime apptDate, ValidationContext context)
        {
            if (apptDate.Date < DateTime.Now.Date)
            {
                return new ValidationResult("Ngày Hẹn Không Hợp Lệ!", new string[] { "ApptDate" });
            }
            return ValidationResult.Success;
        }

        private long? _WPID;
        [DataMemberAttribute()]
        public long? WPID
        {
            get
            {
                return _WPID;
            }
            set
            {
                if (_WPID != value)
                {
                    _WPID = value;
                    RaisePropertyChanged("WPID");
                }
            }
        }

        private string _MedServiceNames;

        /// Danh sach cac service su dung trong cuoc hen nay. (1 chuoi da duoc ghep lai roi)

        [DataMemberAttribute()]
        public string MedServiceNames
        {
            get
            {
                return _MedServiceNames;
            }
            set
            {
                _MedServiceNames = value;
                RaisePropertyChanged("MedServiceNames");
            }
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

        private bool? _hasChronicDisease;
        [DataMemberAttribute()]
        public bool? HasChronicDisease
        {
            get
            {
                return _hasChronicDisease;
            }
            set
            {
                _hasChronicDisease = value;
                RaisePropertyChanged("HasChronicDisease");
            }
        }

        private bool _allowPaperReferralUseNextConsult;
        [DataMemberAttribute()]
        public bool AllowPaperReferralUseNextConsult
        {
            get
            {
                return _allowPaperReferralUseNextConsult;
            }
            set
            {
                _allowPaperReferralUseNextConsult = value;
                RaisePropertyChanged("AllowPaperReferralUseNextConsult");
            }
        }

        private string _reasonToAllowPaperReferral;
        [DataMemberAttribute()]
        public string ReasonToAllowPaperReferral
        {
            get
            {
                return _reasonToAllowPaperReferral;
            }
            set
            {
                _reasonToAllowPaperReferral = value;
                RaisePropertyChanged("ReasonToAllowPaperReferral");
            }
        }

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_PatientApptServiceDetailList);
        }
        public string ConvertDetailsListToXml(IEnumerable<PatientApptServiceDetails> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<AppointmentDetails>");
                foreach (PatientApptServiceDetails details in items)
                {
                    sb.Append("<RecInfo>");

                    sb.AppendFormat("<ApptSvcDetailID>{0}</ApptSvcDetailID>", details.ApptSvcDetailID);
                    sb.AppendFormat("<AppointmentID>{0}</AppointmentID>", details.AppointmentID);
                    sb.AppendFormat("<MedServiceID>{0}</MedServiceID>", details.MedService != null ? details.MedService.MedServiceID : details.MedServiceID);
                    sb.AppendFormat("<DeptLocationID>{0}</DeptLocationID>", details.DeptLocation != null ? details.DeptLocation.LID : 0);
                    sb.AppendFormat("<ApptTimeSegmentID>{0}</ApptTimeSegmentID>", details.ApptTimeSegment != null ? details.ApptTimeSegment.ConsultationTimeSegmentID : details.ApptTimeSegmentID);

                    //sb.AppendFormat("<StartTime>{0}</StartTime>", details.StartTime);
                    //sb.AppendFormat("<EndTime>{0}</EndTime>", details.EndTime);
                    sb.AppendFormat("<ServiceSeqNum>{0}</ServiceSeqNum>", details.ServiceSeqNum);
                    sb.AppendFormat("<ServiceSeqNumType>{0}</ServiceSeqNumType>", details.ServiceSeqNumType);
                    sb.AppendFormat("<V_AppointmentType>{0}</V_AppointmentType>", details.AppointmentType != null ? details.AppointmentType.LookupID : details.V_AppointmentType);

                    sb.Append("</RecInfo>");
                }
                sb.Append("</AppointmentDetails>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        #region 2 danh sách: DV và CLS

        private ObservableCollection<PatientApptServiceDetails> _PatientApptServiceDetailList;
        [DataMemberAttribute()]
        public ObservableCollection<PatientApptServiceDetails> PatientApptServiceDetailList
        {
            get
            {
                return _PatientApptServiceDetailList;
            }
            set
            {
                _PatientApptServiceDetailList = value;
                RaisePropertyChanged("PatientApptServiceDetailList");
            }
        }

        [DataMemberAttribute()]
        public ObservableCollection<PatientApptServiceDetails> ObjApptServiceDetailsList_Add
        {
            get
            {
                return _ObjApptServiceDetailsList_Add;
            }
            set
            {
                if (_ObjApptServiceDetailsList_Add != value)
                {
                    OnObjApptServiceDetailsList_AddChanging(value);
                    _ObjApptServiceDetailsList_Add = value;
                    RaisePropertyChanged("ObjApptServiceDetailsList_Add");
                    OnObjApptServiceDetailsList_AddChanged();
                }
            }
        }
        private ObservableCollection<PatientApptServiceDetails> _ObjApptServiceDetailsList_Add;
        partial void OnObjApptServiceDetailsList_AddChanging(ObservableCollection<PatientApptServiceDetails> value);
        partial void OnObjApptServiceDetailsList_AddChanged();


        [DataMemberAttribute()]
        public ObservableCollection<PatientApptServiceDetails> ObjApptServiceDetailsList_Update
        {
            get
            {
                return _ObjApptServiceDetailsList_Update;
            }
            set
            {
                if (_ObjApptServiceDetailsList_Update != value)
                {
                    OnObjApptServiceDetailsList_UpdateChanging(value);
                    _ObjApptServiceDetailsList_Update = value;
                    RaisePropertyChanged("ObjApptServiceDetailsList_Update");
                    OnObjApptServiceDetailsList_UpdateChanged();
                }
            }
        }
        private ObservableCollection<PatientApptServiceDetails> _ObjApptServiceDetailsList_Update;
        partial void OnObjApptServiceDetailsList_UpdateChanging(ObservableCollection<PatientApptServiceDetails> value);
        partial void OnObjApptServiceDetailsList_UpdateChanged();


        [DataMemberAttribute()]
        public ObservableCollection<PatientApptServiceDetails> ObjApptServiceDetailsList_Delete
        {
            get
            {
                return _ObjApptServiceDetailsList_Delete;
            }
            set
            {
                if (_ObjApptServiceDetailsList_Delete != value)
                {
                    OnObjApptServiceDetailsList_DeleteChanging(value);
                    _ObjApptServiceDetailsList_Delete = value;
                    RaisePropertyChanged("ObjApptServiceDetailsList_Delete");
                    OnObjApptServiceDetailsList_DeleteChanged();
                }
            }
        }
        private ObservableCollection<PatientApptServiceDetails> _ObjApptServiceDetailsList_Delete;
        partial void OnObjApptServiceDetailsList_DeleteChanging(ObservableCollection<PatientApptServiceDetails> value);
        partial void OnObjApptServiceDetailsList_DeleteChanged();

        //Hẹn CLS

        private ObservableCollection<PatientApptPCLRequests> _PatientApptPCLRequestsList;
        [DataMemberAttribute()]
        public ObservableCollection<PatientApptPCLRequests> PatientApptPCLRequestsList
        {
            get
            {
                return _PatientApptPCLRequestsList;
            }
            set
            {
                _PatientApptPCLRequestsList = value;
                RaisePropertyChanged("PatientApptPCLRequestsList");
            }
        }

        [DataMemberAttribute()]
        public ObservableCollection<PatientApptPCLRequests> ObjApptPCLRequestsList_Add
        {
            get
            {
                return _ObjApptPCLRequestsList_Add;
            }
            set
            {
                if (_ObjApptPCLRequestsList_Add != value)
                {
                    OnObjApptPCLRequestsList_AddChanging(value);
                    _ObjApptPCLRequestsList_Add = value;
                    RaisePropertyChanged("ObjApptPCLRequestsList_Add");
                    OnObjApptPCLRequestsList_AddChanged();
                }
            }
        }
        private ObservableCollection<PatientApptPCLRequests> _ObjApptPCLRequestsList_Add;
        partial void OnObjApptPCLRequestsList_AddChanging(ObservableCollection<PatientApptPCLRequests> value);
        partial void OnObjApptPCLRequestsList_AddChanged();


        [DataMemberAttribute()]
        public ObservableCollection<PatientApptPCLRequests> ObjApptPCLRequestsList_Update
        {
            get
            {
                return _ObjApptPCLRequestsList_Update;
            }
            set
            {
                if (_ObjApptPCLRequestsList_Update != value)
                {
                    OnObjApptPCLRequestsList_UpdateChanging(value);
                    _ObjApptPCLRequestsList_Update = value;
                    RaisePropertyChanged("ObjApptPCLRequestsList_Update");
                    OnObjApptPCLRequestsList_UpdateChanged();
                }
            }
        }
        private ObservableCollection<PatientApptPCLRequests> _ObjApptPCLRequestsList_Update;
        partial void OnObjApptPCLRequestsList_UpdateChanging(ObservableCollection<PatientApptPCLRequests> value);
        partial void OnObjApptPCLRequestsList_UpdateChanged();


        [DataMemberAttribute()]
        public ObservableCollection<PatientApptPCLRequests> ObjApptPCLRequestsList_Delete
        {
            get
            {
                return _ObjApptPCLRequestsList_Delete;
            }
            set
            {
                if (_ObjApptPCLRequestsList_Delete != value)
                {
                    OnObjApptPCLRequestsList_DeleteChanging(value);
                    _ObjApptPCLRequestsList_Delete = value;
                    RaisePropertyChanged("ObjApptPCLRequestsList_Delete");
                    OnObjApptPCLRequestsList_DeleteChanged();
                }
            }
        }
        private ObservableCollection<PatientApptPCLRequests> _ObjApptPCLRequestsList_Delete;
        partial void OnObjApptPCLRequestsList_DeleteChanging(ObservableCollection<PatientApptPCLRequests> value);
        partial void OnObjApptPCLRequestsList_DeleteChanged();



        #endregion


        private bool _IsCanEdit = true;
        [DataMemberAttribute()]
        public bool IsCanEdit
        {
            get
            {
                return _IsCanEdit;
            }
            set
            {
                if (_IsCanEdit != value)
                {
                    _IsCanEdit = value;
                    RaisePropertyChanged("IsCanEdit");
                }
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

        private string _LaboratoryPCLRequestIDListXml;
        [DataMemberAttribute()]
        public string LaboratoryPCLRequestIDListXml
        {
            get
            {
                return _LaboratoryPCLRequestIDListXml;
            }
            set
            {
                if (_LaboratoryPCLRequestIDListXml != value)
                {
                    _LaboratoryPCLRequestIDListXml = value;
                    RaisePropertyChanged("LaboratoryPCLRequestIDListXml");
                }
            }
        }

        private string _ImagingPCLRequestIDListXml;
        [DataMemberAttribute()]
        public string ImagingPCLRequestIDListXml
        {
            get
            {
                return _ImagingPCLRequestIDListXml;
            }
            set
            {
                if (_ImagingPCLRequestIDListXml != value)
                {
                    _ImagingPCLRequestIDListXml = value;
                    RaisePropertyChanged("ImagingPCLRequestIDListXml");
                }
            }
        }

        private AllLookupValues.AppServiceDetailPrintType _serviceDetailPrintType;
        [DataMemberAttribute()]
        public AllLookupValues.AppServiceDetailPrintType ServiceDetailPrintType
        {
            get
            {
                return _serviceDetailPrintType;
            }
            set
            {
                if (_serviceDetailPrintType != value)
                {
                    _serviceDetailPrintType = value;
                    RaisePropertyChanged("ServiceDetailPrintType");
                }
            }
        }


        private bool _isPrintLaboratoryPCLApp;
        [DataMemberAttribute()]
        public bool IsPrintLaboratoryPCLApp
        {
            get
            {
                return _isPrintLaboratoryPCLApp;
            }
            set
            {
                if (_isPrintLaboratoryPCLApp != value)
                {
                    _isPrintLaboratoryPCLApp = value;
                    RaisePropertyChanged("IsPrintLaboratoryPCLApp");
                }
            }
        }

        private bool _isPrintImagingPCLApp;
        [DataMemberAttribute()]
        public bool IsPrintImagingPCLApp
        {
            get
            {
                return _isPrintImagingPCLApp;
            }
            set
            {
                if (_isPrintImagingPCLApp != value)
                {
                    _isPrintImagingPCLApp = value;
                    RaisePropertyChanged("IsPrintImagingPCLApp");
                }
            }
        }

        private bool _IsEmergInPtReExamApp;
        private long _V_AppointmentType;
        [DataMemberAttribute]
        public bool IsEmergInPtReExamApp
        {
            get
            {
                return _IsEmergInPtReExamApp;
            }
            set
            {
                _IsEmergInPtReExamApp = value;
                RaisePropertyChanged("IsEmergInPtReExamApp");
            }
        }
        [DataMemberAttribute]
        public long V_AppointmentType
        {
            get
            {
                return _V_AppointmentType;
            }
            set
            {
                _V_AppointmentType = value;
                RaisePropertyChanged("V_AppointmentType");
            }
        }

        private long _HosClientContractID;
        [DataMemberAttribute]
        public long HosClientContractID
        {
            get
            {
                return _HosClientContractID;
            }
            set
            {
                _HosClientContractID = value;
                RaisePropertyChanged("HosClientContractID");
            }
        }

        private HospitalClientContract _ClientContract;
        [DataMemberAttribute]
        public HospitalClientContract ClientContract
        {
            get
            {
                return _ClientContract;
            }
            set
            {
                if (_ClientContract == value)
                {
                    return;
                }
                _ClientContract = value;
                RaisePropertyChanged("ClientContract");
            }
        }

        private DateTime? _EndDate;
        [DataMemberAttribute]
        public DateTime? EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                if (_EndDate == value)
                {
                    return;
                }
                _EndDate = value;
                RaisePropertyChanged("EndDate");
            }
        }

        private long? _OutPtTreatmentProgramID;
        [DataMemberAttribute]
        public long? OutPtTreatmentProgramID
        {
            get
            {
                return _OutPtTreatmentProgramID;
            }
            set
            {
                if (_OutPtTreatmentProgramID != value)
                {
                    _OutPtTreatmentProgramID = value;
                    RaisePropertyChanged("OutPtTreatmentProgramID");
                    RaisePropertyChanged("IsInTreatmentProgram");
                }
            }
        }
        public bool IsInTreatmentProgram
        {
            get
            {
                return OutPtTreatmentProgramID.HasValue && OutPtTreatmentProgramID > 0;
            }
        }
    }
}