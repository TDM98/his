using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using eHCMSLanguage;
/*
* 20180524 #001 TBLD: Added V_ExamRegStatus 
* 20220905 #002 BLQ: Thêm trường kết luận vào cls để load mặc định
*/
namespace DataEntities
{
    public partial class PatientPCLRequest : IDBRecordState
    {
        #region Dung Cho Luu Ket Qua CLS

        [DataMemberAttribute()]
        public int? ParamEnum
        {
            get
            {
                return _ParamEnum;
            }
            set
            {
                if (_ParamEnum != value)
                {
                    _ParamEnum = value;
                    RaisePropertyChanged("ParamEnum");
                }
            }
        }
        private int? _ParamEnum;

        [DataMemberAttribute()]
        public long? PCLExamTypeID
        {
            get
            {
                return _PCLExamTypeID;
            }
            set
            {
                if (_PCLExamTypeID != value)
                {
                    _PCLExamTypeID = value;
                    RaisePropertyChanged("PCLExamTypeID");
                }
            }
        }
        private long? _PCLExamTypeID;

        [DataMemberAttribute()]
        public long? PCLReqItemID
        {
            get
            {
                return _PCLReqItemID;
            }
            set
            {
                if (_PCLReqItemID != value)
                {
                    _PCLReqItemID = value;
                    RaisePropertyChanged("PCLReqItemID");
                }
            }
        }
        private long? _PCLReqItemID;

        //xai lai cua a Dinh luon  
        //Dinh them cai nay de dung cho tree view bao cao
        [DataMemberAttribute]
        public string PCLExamTypeName
        {
            get { return _PCLExamTypeName; }
            set
            {
                if (_PCLExamTypeName != value)
                {
                    OnPCLExamTypeNameChanging(value);
                    _PCLExamTypeName = value;
                    RaisePropertyChanged("PCLExamTypeName");
                    OnPCLExamTypeNameChanged();
                }
            }
        }
        private string _PCLExamTypeName;
        partial void OnPCLExamTypeNameChanging(string value);
        partial void OnPCLExamTypeNameChanged();
        //▼====: #002
       
        [DataMemberAttribute]
        public string PCLExamTypeTemplateResult
        {
            get { return _PCLExamTypeTemplateResult; }
            set
            {
                if (_PCLExamTypeTemplateResult != value)
                {
                    _PCLExamTypeTemplateResult = value;
                    RaisePropertyChanged("PCLExamTypeTemplateResult");
                }
            }
        }
        private string _PCLExamTypeTemplateResult;
        //▲====: #002
        [DataMemberAttribute]
        public string V_ExamRegStatusName
        {
            get { return _V_ExamRegStatusName; }
            set
            {
                if (_V_ExamRegStatusName != value)
                {
                    _V_ExamRegStatusName = value;
                    RaisePropertyChanged("V_ExamRegStatusName");
                }
            }
        }
        private string _V_ExamRegStatusName;

        #endregion

        #region Extended Properties

        [DataMemberAttribute()]
        public string PatientPCLRequestDetailsXML
        {
            get
            {
                return _PatientPCLRequestDetailsXML;
            }
            set
            {
                if (_PatientPCLRequestDetailsXML != value)
                {
                    OnPatientPCLRequestDetailsXMLChanging(value);
                    _PatientPCLRequestDetailsXML = value;
                    RaisePropertyChanged("PatientPCLRequestDetailsXML");
                    OnPatientPCLRequestDetailsXMLChanged();
                }
            }
        }
        private string _PatientPCLRequestDetailsXML;
        partial void OnPatientPCLRequestDetailsXMLChanging(string value);
        partial void OnPatientPCLRequestDetailsXMLChanged();

        [DataMemberAttribute()]
        public String RequestedDoctorName
        {
            get
            {
                return _RequestedDoctorName;
            }
            set
            {
                OnRequestedDoctorNameChanging(value);
                _RequestedDoctorName = value;
                RaisePropertyChanged("RequestedDoctorName");
                OnRequestedDoctorNameChanged();
            }
        }
        private String _RequestedDoctorName;
        partial void OnRequestedDoctorNameChanging(String value);
        partial void OnRequestedDoctorNameChanged();
        [DataMemberAttribute()]
        public string RequestedDoctorCode
        {
            get
            {
                return _RequestedDoctorCode;
            }
            set
            {
                OnRequestedDoctorCodeChanging(value);
                _RequestedDoctorCode = value;
                RaisePropertyChanged("RequestedDoctorCode");
                OnRequestedDoctorCodeChanged();
            }
        }
        private string _RequestedDoctorCode;
        partial void OnRequestedDoctorCodeChanging(string value);
        partial void OnRequestedDoctorCodeChanged();

        [DataMemberAttribute()]
        public Nullable<DateTime> ExamDate
        {
            get
            {
                return _ExamDate;
            }
            set
            {
                OnExamDateChanging(value);
                _ExamDate = value;
                RaisePropertyChanged("ExamDate");
                OnExamDateChanged();
            }
        }
        private Nullable<DateTime> _ExamDate;
        partial void OnExamDateChanging(Nullable<DateTime> value);
        partial void OnExamDateChanged();

        [DataMemberAttribute()]
        public string PCLFormName
        {
            get
            {
                return _PCLFormName;
            }
            set
            {
                _PCLFormName = value;
                RaisePropertyChanged("PCLFormName");
            }
        }
        private string _PCLFormName;


        #endregion

        public override bool Equals(object obj)
        {
            PatientPCLRequest info = obj as PatientPCLRequest;
            if (info == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;
           
              //  return this.PatientPCLReqID > 0 && this.PatientPCLReqID == info.PatientPCLReqID && (this.PCLExamTypeID == info.PCLExamTypeID);
           
            return this.PatientPCLReqID > 0 && this.PatientPCLReqID == info.PatientPCLReqID;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("PCL Request {0}", this._PCLRequestNumID);
        }

        private long? _StaffID;
        [DataMemberAttribute()]
        public new long? StaffID
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

        private long? _DoctorStaffID;
        [DataMemberAttribute()]
        public long? DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                _DoctorStaffID = value;
                RaisePropertyChanged("DoctorStaffID");
            }
        }

        [DataMemberAttribute()]
        public String StaffIDName
        {
            get
            {
                return _StaffIDName;
            }
            set
            {
                OnStaffIDNameChanging(value);
                _StaffIDName = value;
                RaisePropertyChanged("StaffIDName");
                OnStaffIDNameChanged();
            }
        }
        private String _StaffIDName;
        partial void OnStaffIDNameChanging(String value);
        partial void OnStaffIDNameChanged();


        [DataMemberAttribute()]
        public String StaffIDPhoneNumber
        {
            get
            {
                return _StaffIDPhoneNumber;
            }
            set
            {
                OnStaffIDPhoneNumberChanging(value);
                _StaffIDPhoneNumber = value;
                RaisePropertyChanged("StaffIDPhoneNumber");
                OnStaffIDPhoneNumberChanged();
            }
        }
        private String _StaffIDPhoneNumber;
        partial void OnStaffIDPhoneNumberChanging(String value);
        partial void OnStaffIDPhoneNumberChanged();


        [DataMemberAttribute()]
        public String V_PCLRequestStatusName
        {
            get
            {
                return _V_PCLRequestStatusName;
            }
            set
            {
                OnV_PCLRequestStatusNameChanging(value);
                _V_PCLRequestStatusName = value;
                RaisePropertyChanged("V_PCLRequestStatusName");
                OnV_PCLRequestStatusNameChanged();
            }
        }
        private String _V_PCLRequestStatusName;
        partial void OnV_PCLRequestStatusNameChanging(String value);
        partial void OnV_PCLRequestStatusNameChanged();

      
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
                    OnPtRegistrationIDChanging(value);
                    _PtRegistrationID = value;
                    RaisePropertyChanged("PtRegistrationID");
                    OnStaffIDNameChanged();
                }
            }
        }
        private long _PtRegistrationID;
        partial void OnPtRegistrationIDChanging(long value);
        partial void OnPtRegistrationIDChanged();


        [DataMemberAttribute()]
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                if (_V_RegistrationType != value)
                {
                    OnV_RegistrationTypeChanging(value);
                    _V_RegistrationType = value;
                    RaisePropertyChanged("V_RegistrationType");
                    OnV_RegistrationTypeChanged();
                }
            }
        }
        private long _V_RegistrationType;
        partial void OnV_RegistrationTypeChanging(long value);
        partial void OnV_RegistrationTypeChanged();

        [DataMemberAttribute()]
        public long V_RegistrationStatus
        {
            get
            {
                return _V_RegistrationStatus;
            }
            set
            {
                if (_V_RegistrationStatus != value)
                {
                    OnV_RegistrationStatusChanging(value);
                    _V_RegistrationStatus = value;
                    RaisePropertyChanged("V_RegistrationStatus");
                    OnV_RegistrationStatusChanged();
                }
            }
        }
        private long _V_RegistrationStatus;
        partial void OnV_RegistrationStatusChanging(long value);
        partial void OnV_RegistrationStatusChanged();
        


        private AllLookupValues.V_PCLRequestType _V_PCLRequestType = AllLookupValues.V_PCLRequestType.UNKNOWN;
        [DataMemberAttribute()]
        public AllLookupValues.V_PCLRequestType V_PCLRequestType
        {
            get
            {
                return _V_PCLRequestType;
            }
            set 
            {
                if (_V_PCLRequestType != value)
                {
                    _V_PCLRequestType = value;
                    RaisePropertyChanged("V_PCLRequestType");
                }
            }
        }

        private AllLookupValues.V_PCLRequestStatus _V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
        [DataMemberAttribute()]
        public AllLookupValues.V_PCLRequestStatus V_PCLRequestStatus
        {
            get
            {
                return _V_PCLRequestStatus;
            }
            set
            {
                if (_V_PCLRequestStatus != value)
                {
                    _V_PCLRequestStatus = value;
                    RaisePropertyChanged("V_PCLRequestStatus");
                }
            }
        }

        [DataMemberAttribute()]
        public string HosName
        {
            get
            {
                return _HosName;
            }
            set
            {
                OnHosNameChanging(value);
                _HosName = value;
                RaisePropertyChanged("HosName");
                OnHosNameChanged();
            }
        }
        private string _HosName;
        partial void OnHosNameChanging(string value);
        partial void OnHosNameChanged();

        #region IDBRecordState Members
        private RecordState _RecordState = RecordState.DETACHED;
        [DataMemberAttribute()]
        public new RecordState RecordState
        {
            get
            {
                return _RecordState;
            }
            set
            {
                _RecordState = value;
                RaisePropertyChanged("RecordState");
            }
        }

        #endregion

        //Để biết Khoa nào chỉ định làm phiếu CLS
        [DataMemberAttribute()]
        public Nullable<Int64>  DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                if (_DeptID != value)
                {
                    OnDeptIDChanging(value);
                    _DeptID = value;
                    RaisePropertyChanged("DeptID");
                    OnDeptIDChanged();
                }
            }
        }
        private Nullable<Int64> _DeptID;
        partial void OnDeptIDChanging(Nullable<Int64> value);
        partial void OnDeptIDChanged();
        //Để biết Khoa nào chỉ định làm phiếu CLS

        //Cac item trong 1 phieu chac chan cung cho LAM cho nen co truong nay
        [DataMemberAttribute()]
        public long DeptLocID
        {
            get
            {
                return _DeptLocID;
            }
            set
            {
                if (_DeptLocID != value)
                {
                    OnDeptLocIDChanging(value);
                    _DeptLocID = value;
                    RaisePropertyChanged("DeptLocID");
                    OnDeptLocIDChanged();
                }
            }
        }
        private long _DeptLocID;
        partial void OnDeptLocIDChanging(long value);
        partial void OnDeptLocIDChanged();


        
        [DataMemberAttribute()]
        public int ServiceSeqNum
        {
            get
            {
                return _serviceSeqNum;
            }
            set
            {
                _serviceSeqNum = value;
                RaisePropertyChanged("ServiceSeqNum");
            }
        }
        private int _serviceSeqNum;
        
        [DataMemberAttribute()]
        public byte ServiceSeqNumType
        {
            get
            {
                return _serviceSeqNumType;
            }
            set
            {
                _serviceSeqNumType = value;
                RaisePropertyChanged("ServiceSeqNumType");
            }
        }
        private byte _serviceSeqNumType;


        //Cac item trong 1 phieu chac chan cung cho LAM cho nen co truong nay

        [DataMemberAttribute()]
        public String FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                OnFullNameChanging(value);
                _FullName = value;
                RaisePropertyChanged("FullName");
                OnFullNameChanged();
            }
        }
        private String _FullName;
        partial void OnFullNameChanging(String value);
        partial void OnFullNameChanged();


        [DataMemberAttribute()]
        public String PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                OnPatientCodeChanging(value);
                _PatientCode = value;
                RaisePropertyChanged("PatientCode");
                OnPatientCodeChanged();
            }
        }
        private String _PatientCode;
        partial void OnPatientCodeChanging(String value);
        partial void OnPatientCodeChanged();

        [DataMemberAttribute()]
        public String DOB
        {
            get
            {
                return _DOB;
            }
            set
            {
                OnDOBChanging(value);
                _DOB = value;
                RaisePropertyChanged("DOB");
                OnDOBChanged();
            }
        }
        private String _DOB;
        partial void OnDOBChanging(String value);
        partial void OnDOBChanged();



        #region "Phiếu này thuộc Disgnostic nào, RegisPatient Nào, RegisPatientDetail nào"

        [DataMemberAttribute()]
        public Int64 PatientID
        {
            get
            {
                return _PatientID;
            }
            set
            {
                OnPatientIDChanging(value);
                _PatientID = value;
                RaisePropertyChanged("PatientID");
                OnPatientIDChanged();
            }
        }
        private Int64 _PatientID;
        partial void OnPatientIDChanging(Int64 value);
        partial void OnPatientIDChanged();



        [DataMemberAttribute()]
        public Int64 DTItemID
        {
            get
            {
                return _DTItemID;
            }
            set
            {
                OnDTItemIDChanging(value);
                _DTItemID = value;
                RaisePropertyChanged("DTItemID");
                OnDTItemIDChanged();
            }
        }
        private Int64 _DTItemID;
        partial void OnDTItemIDChanging(Int64 value);
        partial void OnDTItemIDChanged();
       


        #endregion


        #region LABCom

        [DataMemberAttribute()]
        public PatientPCLRequest_LABCom ObjPatientPCLRequest_LABCom
        {
            get { return _ObjPatientPCLRequest_LABCom; }
            set
            {
                OnObjPatientPCLRequest_LABComChanging(value);
                _ObjPatientPCLRequest_LABCom = value;
                RaisePropertyChanged("ObjPatientPCLRequest_LABCom");
                OnObjPatientPCLRequest_LABComChanged();
            }
        }
        private PatientPCLRequest_LABCom _ObjPatientPCLRequest_LABCom;
        partial void OnObjPatientPCLRequest_LABComChanging(PatientPCLRequest_LABCom value);
        partial void OnObjPatientPCLRequest_LABComChanged();
        #endregion

        public bool CheckTraTien(PatientPCLRequest p, ref string errorStr, int EffectedPCLHours = 48, int EditDiagDays=7)
        {
            errorStr = "";
            //if (p.V_RegistrationType != (long)AllLookupValues.RegistrationType.NOI_TRU)
            if (p.V_PCLRequestType != AllLookupValues.V_PCLRequestType.NOI_TRU)
            {
                if ((p.IsAllowToPayAfter == 0 && !p.AllowToPayAfter && p.PatientClassID != (long)ePatientClassification.PayAfter && p.PatientClassID != (long)ePatientClassification.CompanyHealthRecord) && p.PaidTime == null)
                {
                    errorStr += string.Format(eHCMSResources.Z2399_G1_E, p.FullName.Trim(), Environment.NewLine);
                    return false;
                }
                if (p.RefundTime != null)
                {
                    errorStr += string.Format(eHCMSResources.Z2400_G1_E, p.FullName.Trim(), Environment.NewLine);
                    return false;
                }
                if (p.V_PCLRequestStatus == DataEntities.AllLookupValues.V_PCLRequestStatus.OPEN
                    && ((p.IsAllowToPayAfter == 0 && !p.AllowToPayAfter && p.PaidTime != null && p.PaidTime.Value.AddHours(EffectedPCLHours) < DateTime.Now)
                    || (p.IsAllowToPayAfter > 0 && p.CreatedDate.AddHours(EffectedPCLHours) < DateTime.Now)))
                {
                    errorStr += string.Format(eHCMSResources.Z2401_G1_E, EffectedPCLHours);
                    return false;
                }
                if (p.V_PCLRequestStatus == DataEntities.AllLookupValues.V_PCLRequestStatus.CLOSE
                    && ((p.IsAllowToPayAfter == 0 && !p.AllowToPayAfter && p.PaidTime != null && p.PaidTime.Value.AddHours(EffectedPCLHours) < DateTime.Now)
                    || (p.IsAllowToPayAfter > 0 && p.CreatedDate.AddHours(EffectedPCLHours) < DateTime.Now)))
                {
                    errorStr += string.Format(eHCMSResources.Z2402_G1_E, EditDiagDays);
                    return false;
                }
                return true;
            }
            else/*VIP,Nội Trú*/
            {
                //if (p.V_RegistrationType == (long)AllLookupValues.RegistrationType.DANGKY_VIP)
                //{
                //    return true;
                //}
                //else
                {
                    if ((p.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.COMPLETED)
                        &&
                        (p.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.INVALID)
                        //&&
                        //(p.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.LOCKED)
                        &&
                        (p.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.PENDING)
                    )
                    {
                        return true;
                    }
                    else
                    {
                        switch (p.V_RegistrationStatus)
                        {
                            case (long)AllLookupValues.RegistrationStatus.COMPLETED:
                                {
                                    errorStr += ("'" + p.FullName.Trim() + "'(Nội Trú)" + Environment.NewLine + "Không thể chẩn đoán được vì đăng ký đã đóng");
                                    break;
                                }
                            case (long)AllLookupValues.RegistrationStatus.INVALID:
                                {
                                    errorStr += ("'" + p.FullName.Trim() + "'(Nội Trú)" + Environment.NewLine + "Không thể chẩn đoán được vì đăng ký không hợp lệ");
                                    break;
                                }
                            //case (long)AllLookupValues.RegistrationStatus.LOCKED:
                            //    {
                            //        errorStr+=("'" + p.FullName.Trim() + "'(Nội Trú)" + Environment.NewLine + "Không thể chẩn đoán được vì đăng ký bị khóa");
                            //        break;
                            //    }
                            case (long)AllLookupValues.RegistrationStatus.PENDING:
                                {
                                    errorStr += ("'" + p.FullName.Trim() + "'(Nội Trú)" + Environment.NewLine + "Không thể chẩn đoán được vì đăng ký chưa hoàn tất");
                                    break;
                                }
                        }
                        return false;
                    }
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
        public bool CheckNewPCL(PatientPCLRequest CurrentPclRequest) 
        {
            if (CurrentPclRequest.PatientPCLRequestIndicators == null 
                || CurrentPclRequest.PatientPCLRequestIndicators.Count <1)
                return false;

            foreach (var item in CurrentPclRequest.PatientPCLRequestIndicators)
            {
                if (item.PCLReqItemID < 1)
                {
                    return true;
                }
            }
            return false;
        }

        /*▼====: #001*/
        [DataMemberAttribute()]
        public long V_ExamRegStatus
        {
            get { return _V_ExamRegStatus; }
            set
            {
                if (_V_ExamRegStatus != value)
                {
                    _V_ExamRegStatus = value;
                    RaisePropertyChanged("V_ExamRegStatus");
                }
            }
        }
        private long _V_ExamRegStatus;
        /*▲====: #001*/
        private string _DoctorIDName = "";
        [DataMemberAttribute()]
        public string DoctorIDName
        {
            get
            {
                return _DoctorIDName;
            }
            set
            {
                _DoctorIDName = value;
                RaisePropertyChanged("DoctorIDName");
            }
        }

        private DateTime _RecCreatedDate;
        [DataMemberAttribute()]
        public DateTime RecCreatedDate
        {
            get
            {
                return _RecCreatedDate;
            }
            set
            {
                _RecCreatedDate = value;
                RaisePropertyChanged("RecCreatedDate");
            }
        }
    }
}
