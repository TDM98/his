
using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using eHCMS.Configurations;

/*
 * #0001: 20161215 CMN Begin: Add converter for xml safe to success save on SQL
 * #0002: 20170316 CMN: Add converter for xml safe to success save on SQL
 * 20180920 #003 TBL:   Chinh sua chuc nang bug mantis ID 0000061, kiem tra tung property trong dataentities 
 *                      neu co su thay doi IsDataChanged = true
 * 20180925 #004 TBL: Kiem tra toa khong co thuoc khi luu
 * 20221216 #005 BLQ: Thêm đường dùng mới và khoảng cách dùng 
*/
namespace DataEntities
{
    public partial class Prescription : NotifyChangedBase
    {
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

        #region Adding Datamember Attributes
        #region Adding Prescriptio
        [DataMemberAttribute()]
        public bool NeedToHoldConsultation
        {
            get
            {
                return _NeedToHoldConsultation;
            }
            set
            {
                if (_NeedToHoldConsultation != value)
                {
                    /*▼====: #003*/
                    if ((IsObjectBeingUsedByClient) && _NeedToHoldConsultation != value)
                    {
                        IsDataChanged = true;
                    }
                    /*▲====: #003*/
                    _NeedToHoldConsultation = value;
                    RaisePropertyChanged("NeedToHoldConsultation");
                }
            }
        }
        private bool _NeedToHoldConsultation = false;

        [DataMemberAttribute()]
        public String HICardNo
        {
            get
            {
                return _HICardNo;
            }
            set
            {
                _HICardNo = value;
                RaisePropertyChanged("HICardNo");
            }
        }
        private String _HICardNo;

        [DataMemberAttribute()]
        public bool IsSold
        {
            get
            {
                return _IsSold;
            }
            set
            {
                _IsSold = value;
                RaisePropertyChanged("IsSold");
            }
        }
        private bool _IsSold;
        #endregion

        #region PrescriptionDetails

        [DataMemberAttribute()]
        public string PrescriptDetailsStr
        {
            get
            {
                return _PrescriptDetailsStr;
            }
            set
            {
                OnPrescriptDetailsStrChanging(value);
                _PrescriptDetailsStr = value;
                RaisePropertyChanged("PrescriptDetailsStr");
                OnPrescriptDetailsStrChanged();
            }
        }
        private string _PrescriptDetailsStr;
        partial void OnPrescriptDetailsStrChanging(string value);
        partial void OnPrescriptDetailsStrChanged();

        #endregion

        #region Adding PrescriptionHistory


        [DataMemberAttribute()]
        public long IssueID
        {
            get
            {
                return _IssueID;
            }
            set
            {
                OnIssueIDChanging(value);
                _IssueID = value;
                RaisePropertyChanged("IssueID");
                OnIssueIDChanged();
            }
        }
        private long _IssueID;
        partial void OnIssueIDChanging(long value);
        partial void OnIssueIDChanged();



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
        public Nullable<long> AppointmentID
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
        private Nullable<long> _AppointmentID;
        partial void OnAppointmentIDChanging(Nullable<long> value);
        partial void OnAppointmentIDChanged();




        [DataMemberAttribute()]
        public Nullable<DateTime> AppointmentDate
        {
            get
            {
                return _AppointmentDate;
            }
            set
            {
                OnAppointmentDateChanging(value);
                _AppointmentDate = value;
                RaisePropertyChanged("AppointmentDate");
                OnAppointmentDateChanged();
            }
        }
        private Nullable<DateTime> _AppointmentDate;
        partial void OnAppointmentDateChanging(Nullable<DateTime> value);
        partial void OnAppointmentDateChanged();





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



        private Staff _ObjIssuerStaffID;
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


        private Staff _ObjReIssuerStaffID;
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



        //BS Chẩn Đoán
        private Staff _ObjDoctorStaffID;
        [DataMemberAttribute()]
        public Staff ObjDoctorStaffID
        {
            get
            {
                return _ObjDoctorStaffID;
            }
            set
            {
                if (_ObjDoctorStaffID != value)
                {
                    _ObjDoctorStaffID = value;
                    RaisePropertyChanged("ObjDoctorStaffID");
                }
            }
        }
        //BS Chẩn Đoán


        [DataMemberAttribute()]
        public Nullable<byte> TimesNumberIsPrinted
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
        private Nullable<byte> _TimesNumberIsPrinted;
        partial void OnTimesNumberIsPrintedChanging(Nullable<byte> value);
        partial void OnTimesNumberIsPrintedChanged();




        [DataMemberAttribute()]
        public Nullable<long> OriginalPrescriptID
        {
            get
            {
                return _OriginalPrescriptID;
            }
            set
            {
                OriginalPrescriptIDChanging(value);
                _OriginalPrescriptID = value;
                RaisePropertyChanged("TimesNumberIsPrinted");
                OriginalPrescriptIDChanged();
            }
        }
        private Nullable<long> _OriginalPrescriptID;
        partial void OriginalPrescriptIDChanging(Nullable<long> value);
        partial void OriginalPrescriptIDChanged();




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


        public string PrintedStatus
        {
            get
            {
                string _PrintedStatus = string.Empty;
                if (this.TimesNumberIsPrinted == 0)
                    _PrintedStatus += "Never been printed";
                else
                    _PrintedStatus += "Printed [" + this.TimesNumberIsPrinted.ToString() + " time(s)]";
                return _PrintedStatus;
            }
            set
            {
            }
        }




        [DataMemberAttribute()]
        public string AllergiesString
        {
            get
            {
                return _AllergiesString;
            }
            set
            {
                OnAllergiesStringChanging(value);
                _AllergiesString = value;
                RaisePropertyChanged("AllergiesString");
                OnAllergiesStringChanged();
            }
        }
        private string _AllergiesString;
        partial void OnAllergiesStringChanging(string value);
        partial void OnAllergiesStringChanged();

        [DataMemberAttribute()]
        public string WarningString
        {
            get
            {
                return _WarningString;
            }
            set
            {
                OnWarningStringChanging(value);
                _WarningString = value;
                RaisePropertyChanged("WarningString");
                OnWarningStringChanged();
            }
        }
        private string _WarningString;
        partial void OnWarningStringChanging(string value);
        partial void OnWarningStringChanged();


        [DataMemberAttribute()]
        public string ChangeLogs
        {
            get
            {
                return _ChangeLogs;
            }
            set
            {
                OnChangeLogsChanging(value);
                _ChangeLogs = value;
                RaisePropertyChanged("ChangeLogs");
                OnChangeLogsChanged();
            }
        }
        private string _ChangeLogs;
        partial void OnChangeLogsChanging(string value);
        partial void OnChangeLogsChanged();
        #endregion

        #region Adding PatientServiceRecords

        [DataMemberAttribute()]
        public Nullable<Int64> PtRegistrationID
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
        private Nullable<Int64> _PtRegistrationID;

        [DataMemberAttribute()]
        public string PtRegistrationCode
        {
            get
            {
                return _PtRegistrationCode;
            }
            set
            {
                _PtRegistrationCode = value;
                RaisePropertyChanged("PtRegistrationCode");
            }
        }
        private string _PtRegistrationCode;

        
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
        public Nullable<Int64> V_ProcessingType
        {
            get
            {
                return _V_ProcessingType;
            }
            set
            {
                OnV_ProcessingTypeChanging(value);
                _V_ProcessingType = value;
                RaisePropertyChanged("V_ProcessingType");
                OnV_ProcessingTypeChanged();
            }
        }
        private Nullable<Int64> _V_ProcessingType;
        partial void OnV_ProcessingTypeChanging(Nullable<Int64> value);
        partial void OnV_ProcessingTypeChanged();


        [DataMemberAttribute()]
        public String ProcessingType
        {
            get
            {
                return _ProcessingType;
            }
            set
            {
                OnProcessingTypeChanging(value);
                _ProcessingType = value;
                RaisePropertyChanged("ProcessingType");
                OnProcessingTypeChanged();
            }
        }
        private String _ProcessingType;
        partial void OnProcessingTypeChanging(String value);
        partial void OnProcessingTypeChanged();


        [DataMemberAttribute()]
        public Nullable<Int64> V_Behaving
        {
            get
            {
                return _V_Behaving;
            }
            set
            {
                OnV_BehavingChanging(value);
                _V_Behaving = value;
                RaisePropertyChanged("V_Behaving");
                OnV_BehavingChanged();
            }
        }
        private Nullable<Int64> _V_Behaving;
        partial void OnV_BehavingChanging(Nullable<Int64> value);
        partial void OnV_BehavingChanged();


        [DataMemberAttribute()]
        public String Behaving
        {
            get
            {
                return _Behaving;
            }
            set
            {
                OnBehavingChanging(value);
                _Behaving = value;
                RaisePropertyChanged("Behaving");
                OnBehavingChanged();
            }
        }
        private String _Behaving;
        partial void OnBehavingChanging(String value);
        partial void OnBehavingChanged();

        #endregion

        #region Adding PatientMedicalRecord

        [DataMemberAttribute()]
        public long PatientRecID
        {
            get
            {
                return _PatientRecID;
            }
            set
            {
                if (_PatientRecID != value)
                {
                    OnPatientRecIDChanging(value);
                    _PatientRecID = value;
                    RaisePropertyChanged("PatientRecID");
                    OnPatientRecIDChanged();
                }
            }
        }
        private long _PatientRecID;
        partial void OnPatientRecIDChanging(long value);
        partial void OnPatientRecIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> PatientID
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
        private Nullable<long> _PatientID;
        partial void OnPatientIDChanging(Nullable<long> value);
        partial void OnPatientIDChanged();

        [DataMemberAttribute()]
        public string PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                _PatientCode = value;
                RaisePropertyChanged("PatientCode");
            }
        }
        private string _PatientCode;

        [DataMemberAttribute()]
        public String NationalMedicalCode
        {
            get
            {
                return _NationalMedicalCode;
            }
            set
            {
                OnNationalMedicalCodeChanging(value);
                _NationalMedicalCode = value;
                RaisePropertyChanged("NationalMedicalCode");
                OnNationalMedicalCodeChanged();
            }
        }
        private String _NationalMedicalCode;
        partial void OnNationalMedicalCodeChanging(String value);
        partial void OnNationalMedicalCodeChanged();

        [DataMemberAttribute()]
        public String PatientFullName
        {
            get
            {
                return _PatientFullName;
            }
            set
            {
                OnPatientFullNameChanging(value);
                _PatientFullName = value;
                RaisePropertyChanged("PatientFullName");
                OnPatientFullNameChanged();
            }
        }
        private String _PatientFullName;
        partial void OnPatientFullNameChanging(String value);
        partial void OnPatientFullNameChanged();

        [DataMemberAttribute()]
        public String PatientRecBarCode
        {
            get
            {
                return _PatientRecBarCode;
            }
            set
            {
                OnPatientRecBarCodeChanging(value);
                _PatientRecBarCode = value;
                RaisePropertyChanged("PatientRecBarCode");
                OnPatientRecBarCodeChanged();
            }
        }
        private String _PatientRecBarCode;
        partial void OnPatientRecBarCodeChanging(String value);
        partial void OnPatientRecBarCodeChanged();

        //[DataMemberAttribute()]
        //public Nullable<DateTime> CreatedDate
        //{
        //    get
        //    {
        //        return _CreatedDate;
        //    }
        //    set
        //    {
        //        OnCreatedDateChanging(value);
        //        _CreatedDate = value;
        //        RaisePropertyChanged("CreatedDate");
        //        OnCreatedDateChanged();
        //    }
        //}
        //private Nullable<DateTime> _CreatedDate;
        //partial void OnCreatedDateChanging(Nullable<DateTime> value);
        //partial void OnCreatedDateChanged();

        //[DataMemberAttribute()]
        //public Nullable<DateTime> FinishedDate
        //{
        //    get
        //    {
        //        return _FinishedDate;
        //    }
        //    set
        //    {
        //        OnFinishedDateChanging(value);
        //        _FinishedDate = value;
        //        RaisePropertyChanged("FinishedDate");
        //        OnFinishedDateChanged();
        //    }
        //}
        //private Nullable<DateTime> _FinishedDate;
        //partial void OnFinishedDateChanging(Nullable<DateTime> value);
        //partial void OnFinishedDateChanged();

        #endregion

        //#region AddingRelatedStaffInfo

        //[DataMemberAttribute()]
        //public Nullable<Int64> AuthorID
        //{
        //    get
        //    {
        //        return _AuthorID;
        //    }
        //    set
        //    {
        //        OnAuthorIDChanging(value);
        //        _AuthorID = value;
        //        RaisePropertyChanged("AuthorID");
        //        OnAuthorIDChanged();
        //    }
        //}
        //private Nullable<Int64> _AuthorID;
        //partial void OnAuthorIDChanging(Nullable<Int64> value);
        //partial void OnAuthorIDChanged();

        //[DataMemberAttribute()]
        //public String AuthorFullName
        //{
        //    get
        //    {
        //        return _AuthorFullName;
        //    }
        //    set
        //    {
        //        OnAuthorFullNameChanging(value);
        //        _AuthorFullName = value;
        //        RaisePropertyChanged("AuthorFullName");
        //        OnAuthorFullNameChanged();
        //    }
        //}
        //private String _AuthorFullName;
        //partial void OnAuthorFullNameChanging(String value);
        //partial void OnAuthorFullNameChanged();


        //[DataMemberAttribute()]
        //public Nullable<Int64> CreatorID
        //{
        //    get
        //    {
        //        return _CreatorID;
        //    }
        //    set
        //    {
        //        OnCreatorIDChanging(value);
        //        _CreatorID = value;
        //        RaisePropertyChanged("CreatorID");
        //        OnCreatorIDChanged();
        //    }
        //}
        //private Nullable<Int64> _CreatorID;
        //partial void OnCreatorIDChanging(Nullable<Int64> value);
        //partial void OnCreatorIDChanged();


        //[DataMemberAttribute()]
        //public String CreatorFullName
        //{
        //    get
        //    {
        //        return _CreatorFullName;
        //    }
        //    set
        //    {
        //        OnCreatorFullNameChanging(value);
        //        _CreatorFullName = value;
        //        RaisePropertyChanged("CreatorFullName");
        //        OnCreatorFullNameChanged();
        //    }
        //}
        //private String _CreatorFullName;
        //partial void OnCreatorFullNameChanging(String value);
        //partial void OnCreatorFullNameChanged();
        //#endregion


        [DataMemberAttribute()]
        public bool IsAllowEditNDay
        {
            get
            {
                return _IsAllowEditNDay;
            }
            set
            {
                OnIsAllowEditNDayChanging(value);
                _IsAllowEditNDay = value;
                RaisePropertyChanged("IsAllowEditNDay");
                OnIsAllowEditNDayChanged();
            }
        }
        private bool _IsAllowEditNDay;
        partial void OnIsAllowEditNDayChanging(bool value);
        partial void OnIsAllowEditNDayChanged();

        public bool IsPrescriptionDataChanged
        {
            get
            {
                if (!IsDataChanged && !IsPrescriptionDetailsChanged)
                    return false;
                return true;
            }
        }

        /*▼====: #003*/
        //private bool _IsDataChanged = false;
        private bool IsDataChanged
        {
            //get
            //{
            //    return _IsDataChanged;
            //}
            //set
            //{
            //    _IsDataChanged = value;
            //    RaisePropertyChanged("IsDataChanged");
            //}
            get; set;
        }
        /*▲====: #003*/
        public void SetDataChanged()
        {
            IsDataChanged = true;
        }
        public void ResetDataChanged()
        {
            IsDataChanged = false;
            foreach( var itemDetail in PrescriptionDetails)
            {
                itemDetail.IsDataChanged = false;
            }
        }

        private bool _IsObjectBeingUsedByClient = false;
        [DataMemberAttribute()]
        public bool IsObjectBeingUsedByClient
        {
            get
            {
                return _IsObjectBeingUsedByClient;
            }
            set
            {
                if (_IsObjectBeingUsedByClient != value)
                {
                    if(PrescriptionDetails != null)
                    {
                        foreach(var presdetail in PrescriptionDetails)
                        {
                            presdetail.IsObjectBeingUsedByClient = value;
                        }
                    }
                    _IsObjectBeingUsedByClient = value;
                    RaisePropertyChanged("IsObjectBeingUsedByClient");
                }
            }
        }

        #region IEditableObject Members
        private Prescription _tempPrescription;
        public void BeginEdit()
        {
            _tempPrescription = (Prescription)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempPrescription)
                CopyFrom(_tempPrescription);
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(Prescription p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion

        #endregion

        #region Convert XML

        public string ConvertDetailsListToXml()
        {
            return ConvertDetailsListToXml(_PrescriptionDetails);
        }
        public string ConvertDetailsListToXml(IEnumerable<PrescriptionDetail> items)
        {
            if (items != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<PrescriptionDetails>");
                foreach (PrescriptionDetail details in items)
                {
                    if (details != null && details.DrugID > 0)
                    {
                        int EntityState = (int)details.EntityState;
                        sb.Append("<RecInfo>");
                        sb.AppendFormat("<PrescriptDetailID>{0}</PrescriptDetailID>", details.PrescriptDetailID);
                        sb.AppendFormat("<DrugID>{0}</DrugID>", details.DrugID);
                        sb.AppendFormat("<PrescriptID>{0}</PrescriptID>", details.PrescriptID);
                        sb.AppendFormat("<Strength>{0}</Strength>", details.Strength);
                        sb.AppendFormat("<BeOfHIMedicineList>{0}</BeOfHIMedicineList>", details.BeOfHIMedicineList);
                        sb.AppendFormat("<V_DrugUsage>{0}</V_DrugUsage>", details.V_DrugUsage);
                        sb.AppendFormat("<MDose>{0}</MDose>", details.MDose);
                        sb.AppendFormat("<ADose>{0}</ADose>", details.ADose);
                        sb.AppendFormat("<EDose>{0}</EDose>", details.EDose);
                        sb.AppendFormat("<NDose>{0}</NDose>", details.NDose);
                        sb.AppendFormat("<DayRpts>{0}</DayRpts>", details.DayRpts);
                        sb.AppendFormat("<Qty>{0}</Qty>", details.Qty);
                        sb.AppendFormat("<DrugInstructionNotes>{0}</DrugInstructionNotes>", details.DrugInstructionNotes);
                        sb.AppendFormat("<Administration>{0}</Administration>", details.Administration);
                        sb.AppendFormat("<V_DrugType>{0}</V_DrugType>", details.V_DrugType);
                        sb.AppendFormat("<EntityState>{0}</EntityState>", EntityState);
                        sb.Append("</RecInfo>");
                    }
                }
                sb.Append("</PrescriptionDetails>");
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }


        //Lưu Toa Thuốc + Schedule
        public string ConvertPrescriptionDetailsWithScheduleToXML(ObservableCollection<PrescriptionDetail> items)
        {
            if (items != null)
            {
                StringBuilder sb_Has = new StringBuilder();

                foreach (PrescriptionDetail details in items)
                {
                    if (details != null)
                    {
                        sb_Has.Append("<PrescriptionDetails>");
                        sb_Has.Append("<RecInfo>");
                        sb_Has.AppendFormat("<PrescriptDetailID>{0}</PrescriptDetailID>", details.PrescriptDetailID);
                        sb_Has.AppendFormat("<DrugID>{0}</DrugID>", details.DrugID);
                        sb_Has.AppendFormat("<PrescriptID>{0}</PrescriptID>", details.PrescriptID);
                        sb_Has.AppendFormat("<Strength>{0}</Strength>", details.Strength);
                        sb_Has.AppendFormat("<BeOfHIMedicineList>{0}</BeOfHIMedicineList>",details.BeOfHIMedicineList);
                        sb_Has.AppendFormat("<V_DrugUsage>{0}</V_DrugUsage>", details.V_DrugUsage);
                        sb_Has.AppendFormat("<MDose>{0,2:f5}</MDose>", details.MDose.ToString().Replace(",","."));
                        sb_Has.AppendFormat("<ADose>{0,2:f5}</ADose>", details.ADose.ToString().Replace(",", "."));
                        sb_Has.AppendFormat("<EDose>{0,2:f5}</EDose>", details.EDose.ToString().Replace(",", "."));
                        sb_Has.AppendFormat("<NDose>{0,2:f5}</NDose>", details.NDose.ToString().Replace(",", "."));

                        sb_Has.AppendFormat("<MDoseStr>{0}</MDoseStr>", details.MDoseStr);
                        sb_Has.AppendFormat("<ADoseStr>{0}</ADoseStr>", details.ADoseStr);
                        sb_Has.AppendFormat("<EDoseStr>{0}</EDoseStr>", details.EDoseStr);
                        sb_Has.AppendFormat("<NDoseStr>{0}</NDoseStr>", details.NDoseStr);
                        
                        sb_Has.AppendFormat("<DayRpts>{0}</DayRpts>", details.DayRpts.ToString());
                        sb_Has.AppendFormat("<DayExtended>{0}</DayExtended>", details.DayExtended);
                        sb_Has.AppendFormat("<Qty>{0,2:f3}</Qty>", string.Format(details.Qty.ToString(),CultureInfo.InvariantCulture.NumberFormat));
                        sb_Has.AppendFormat("<QtyMaxAllowed>{0,2:f3}</QtyMaxAllowed>", string.Format(details.QtyMaxAllowed.ToString(), CultureInfo.InvariantCulture.NumberFormat));

                        sb_Has.AppendFormat("<QtyForDay>{0,2:f5}</QtyForDay>", details.QtyForDay.ToString().Replace(",", "."));
                        sb_Has.AppendFormat("<QtySchedMon>{0,2:f5}</QtySchedMon>", details.QtySchedMon.ToString().Replace(",", "."));
                        sb_Has.AppendFormat("<QtySchedTue>{0,2:f5}</QtySchedTue>", details.QtySchedTue.ToString().Replace(",", "."));
                        sb_Has.AppendFormat("<QtySchedWed>{0,2:f5}</QtySchedWed>", details.QtySchedWed.ToString().Replace(",", "."));
                        sb_Has.AppendFormat("<QtySchedThu>{0,2:f5}</QtySchedThu>", details.QtySchedThu.ToString().Replace(",", "."));
                        sb_Has.AppendFormat("<QtySchedFri>{0,2:f5}</QtySchedFri>", details.QtySchedFri.ToString().Replace(",", "."));
                        sb_Has.AppendFormat("<QtySchedSat>{0,2:f5}</QtySchedSat>", details.QtySchedSat.ToString().Replace(",", "."));
                        sb_Has.AppendFormat("<QtySchedSun>{0,2:f5}</QtySchedSun>", details.QtySchedSun.ToString().Replace(",", "."));

                        sb_Has.AppendFormat("<SchedBeginDOW>{0}</SchedBeginDOW>", details.SchedBeginDOW);

                        //==== #0001
                        //sb_Has.AppendFormat("<DrugInstructionNotes>{0}</DrugInstructionNotes>", details.DrugInstructionNotes);
                        sb_Has.AppendFormat("<DrugInstructionNotes>{0}</DrugInstructionNotes>", Globals.GetSafeXMLString(details.DrugInstructionNotes));
                        //==== #0001
                        sb_Has.AppendFormat("<Administration>{0}</Administration>", details.Administration);
                        sb_Has.AppendFormat("<V_DrugType>{0}</V_DrugType>", details.V_DrugType);
                        sb_Has.AppendFormat("<IsContraIndicator>{0}</IsContraIndicator>", details.IsContraIndicator);

                        //Các thuộc tính thêm cho Thuốc Ngoài Danh Mục
                        //==== #0002
                        sb_Has.AppendFormat("<BrandName>{0}</BrandName>", Globals.GetSafeXMLString(details.SelectedDrugForPrescription.BrandName));
                        //==== #0002
                        sb_Has.AppendFormat("<Content>{0}</Content>",
                                            details.SelectedDrugForPrescription.Content);
                        sb_Has.AppendFormat("<UnitName>{0}</UnitName>",
                                            details.SelectedDrugForPrescription.UnitName);
                        sb_Has.AppendFormat("<UnitUse>{0}</UnitUse>",
                                            details.SelectedDrugForPrescription.UnitUse);
                        sb_Has.AppendFormat("<Administration>{0}</Administration>",
                                            details.SelectedDrugForPrescription.Administration);
                        //if (details.isNotInCat)//details.V_DrugType==(long)AllLookupValues.V_DrugType.THUOC_NGOAIDANH_MUC)
                        //{
                        //    details.IsDrugNotInCat = true;
                        //}
                        sb_Has.AppendFormat("<IsDrugNotInCat>{0}</IsDrugNotInCat>", details.IsDrugNotInCat);
                        //Các thuộc tính thêm cho Thuốc Ngoài Danh Mục
                        //▼====: #005
                        sb_Has.AppendFormat("<V_RouteOfAdministration>{0}</V_RouteOfAdministration>", details.V_RouteOfAdministration);
                        sb_Has.AppendFormat("<UsageDistance>{0}</UsageDistance>", details.UsageDistance);
                        //▲====: #005
                        if (details.HasSchedules)
                        {
                            if (details.ObjPrescriptionDetailSchedules != null &&
                                details.ObjPrescriptionDetailSchedules.Count > 0)
                            {
                                string Schedule = ConvertScheduleXML(details.ObjPrescriptionDetailSchedules);
                                sb_Has.Append(Schedule);
                            }
                            sb_Has.Append("</RecInfo>");
                            sb_Has.Append("</PrescriptionDetails>");
                        }
                        else
                        {
                            sb_Has.Append("</RecInfo>");
                            sb_Has.Append("</PrescriptionDetails>");
                        }
                    }
                }

                return sb_Has.ToString();
            }
            return "";
        }

        public string ConvertScheduleXML(ObservableCollection<PrescriptionDetailSchedules> ObjPrescriptionDetailSchedules)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<PrescriptionDetailSchedules>");

            foreach (PrescriptionDetailSchedules items in ObjPrescriptionDetailSchedules)
            {
                sb.Append("<RecInfo>");
                sb.AppendFormat("<PrescriptDetailID>{0}</PrescriptDetailID>", items.PrescriptDetailID);
                sb.AppendFormat("<V_PeriodOfDay>{0}</V_PeriodOfDay>", items.V_PeriodOfDay);
                sb.AppendFormat("<Monday>{0}</Monday>", items.Monday.ToString().Replace(",", "."));
                sb.AppendFormat("<Tuesday>{0}</Tuesday>", items.Tuesday.ToString().Replace(",", "."));
                sb.AppendFormat("<Wednesday>{0}</Wednesday>", items.Wednesday.ToString().Replace(",", "."));
                sb.AppendFormat("<Thursday>{0}</Thursday>", items.Thursday.ToString().Replace(",", "."));
                sb.AppendFormat("<Friday>{0}</Friday>", items.Friday.ToString().Replace(",", "."));
                sb.AppendFormat("<Saturday>{0}</Saturday>", items.Saturday.ToString().Replace(",", "."));
                sb.AppendFormat("<Sunday>{0}</Sunday>", items.Sunday.ToString().Replace(",", "."));

                sb.AppendFormat("<MondayStr>{0}</MondayStr>", items.MondayStr);
                sb.AppendFormat("<TuesdayStr>{0}</TuesdayStr>", items.TuesdayStr);
                sb.AppendFormat("<WednesdayStr>{0}</WednesdayStr>", items.WednesdayStr);
                sb.AppendFormat("<ThursdayStr>{0}</ThursdayStr>", items.ThursdayStr);
                sb.AppendFormat("<FridayStr>{0}</FridayStr>", items.FridayStr);
                sb.AppendFormat("<SaturdayStr>{0}</SaturdayStr>", items.SaturdayStr);
                sb.AppendFormat("<SundayStr>{0}</SundayStr>", items.SundayStr);

                sb.AppendFormat("<UsageNote>{0}</UsageNote>", items.UsageNote);
                sb.Append("</RecInfo>");
            }
            sb.Append("</PrescriptionDetailSchedules>");

            return sb.ToString();
        }
        //Lưu Toa Thuốc + Schedule


        [DataMemberAttribute()]
        public Boolean CanEdit
        {
            get
            {
                return _CanEdit;
            }
            set
            {
                OnCanEditChanging(value);
                _CanEdit = value;
                RaisePropertyChanged("CanEdit");
                OnCanEditChanged();
            }
        }
        private Boolean _CanEdit;
        partial void OnCanEditChanging(Boolean value);
        partial void OnCanEditChanged();

        [DataMemberAttribute()]
        public String ReasonCanEdit
        {
            get
            {
                return _ReasonCanEdit;
            }
            set
            {
                OnReasonCanEditChanging(value);
                _ReasonCanEdit = value;
                RaisePropertyChanged("ReasonCanEdit");
                OnReasonCanEditChanged();
            }
        }
        private String _ReasonCanEdit;
        partial void OnReasonCanEditChanging(String value);
        partial void OnReasonCanEditChanged();



        #endregion
    }
}
