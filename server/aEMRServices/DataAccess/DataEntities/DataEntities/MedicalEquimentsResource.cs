using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
namespace DataEntities
{
    public partial class MedicalEquimentsResource : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new MedicalEquimentsResource object.

        /// <param name="rscrID">Initial value of the RscrID property.</param>
        /// <param name="dateExam">Initial value of the DateExam property.</param>
        /// <param name="examCycle">Initial value of the ExamCycle property.</param>
        /// <param name="maintenanceCycle">Initial value of the MaintenanceCycle property.</param>
        /// <param name="isExpiredDate">Initial value of the IsExpiredDate property.</param>
        public static MedicalEquimentsResource CreateMedicalEquimentsResource(Int64 rscrID, DateTime dateExam, Int32 examCycle, Int32 maintenanceCycle, Boolean isExpiredDate)
        {
            MedicalEquimentsResource medicalEquimentsResource = new MedicalEquimentsResource();
            medicalEquimentsResource.RscrID = rscrID;
            medicalEquimentsResource.DateExam = dateExam;
            medicalEquimentsResource.ExamCycle = examCycle;
            medicalEquimentsResource.MaintenanceCycle = maintenanceCycle;
            medicalEquimentsResource.IsExpiredDate = isExpiredDate;
            return medicalEquimentsResource;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public Int64 RscrID
        {
            get
            {
                return _RscrID;
            }
            set
            {
                if (_RscrID != value)
                {
                    OnRscrIDChanging(value);
                    ////ReportPropertyChanging("RscrID");
                    _RscrID = value;
                    RaisePropertyChanged("RscrID");
                    OnRscrIDChanged();
                }
            }
        }
        private Int64 _RscrID;
        partial void OnRscrIDChanging(Int64 value);
        partial void OnRscrIDChanged();





        [DataMemberAttribute()]
        public String SerialNumber
        {
            get
            {
                return _SerialNumber;
            }
            set
            {
                OnSerialNumberChanging(value);
                ////ReportPropertyChanging("SerialNumber");
                _SerialNumber = value;
                RaisePropertyChanged("SerialNumber");
                OnSerialNumberChanged();
            }
        }
        private String _SerialNumber;
        partial void OnSerialNumberChanging(String value);
        partial void OnSerialNumberChanged();





        [DataMemberAttribute()]
        public String Model
        {
            get
            {
                return _Model;
            }
            set
            {
                OnModelChanging(value);
                ////ReportPropertyChanging("Model");
                _Model = value;
                RaisePropertyChanged("Model");
                OnModelChanged();
            }
        }
        private String _Model;
        partial void OnModelChanging(String value);
        partial void OnModelChanged();





        [DataMemberAttribute()]
        public String Barcode
        {
            get
            {
                return _Barcode;
            }
            set
            {
                OnBarcodeChanging(value);
                ////ReportPropertyChanging("Barcode");
                _Barcode = value;
                RaisePropertyChanged("Barcode");
                OnBarcodeChanged();
            }
        }
        private String _Barcode;
        partial void OnBarcodeChanging(String value);
        partial void OnBarcodeChanged();





        [DataMemberAttribute()]
        public DateTime DateExam
        {
            get
            {
                return _DateExam;
            }
            set
            {
                OnDateExamChanging(value);
                ////ReportPropertyChanging("DateExam");
                _DateExam = value;
                RaisePropertyChanged("DateExam");
                OnDateExamChanged();
            }
        }
        private DateTime _DateExam;
        partial void OnDateExamChanging(DateTime value);
        partial void OnDateExamChanged();





        [DataMemberAttribute()]
        public Int32 ExamCycle
        {
            get
            {
                return _ExamCycle;
            }
            set
            {
                OnExamCycleChanging(value);
                ////ReportPropertyChanging("ExamCycle");
                _ExamCycle = value;
                RaisePropertyChanged("ExamCycle");
                OnExamCycleChanged();
            }
        }
        private Int32 _ExamCycle;
        partial void OnExamCycleChanging(Int32 value);
        partial void OnExamCycleChanged();





        [DataMemberAttribute()]
        public Int32 MaintenanceCycle
        {
            get
            {
                return _MaintenanceCycle;
            }
            set
            {
                OnMaintenanceCycleChanging(value);
                ////ReportPropertyChanging("MaintenanceCycle");
                _MaintenanceCycle = value;
                RaisePropertyChanged("MaintenanceCycle");
                OnMaintenanceCycleChanged();
            }
        }
        private Int32 _MaintenanceCycle;
        partial void OnMaintenanceCycleChanging(Int32 value);
        partial void OnMaintenanceCycleChanged();





        [DataMemberAttribute()]
        public Boolean IsExpiredDate
        {
            get
            {
                return _IsExpiredDate;
            }
            set
            {
                OnIsExpiredDateChanging(value);
                ////ReportPropertyChanging("IsExpiredDate");
                _IsExpiredDate = value;
                RaisePropertyChanged("IsExpiredDate");
                OnIsExpiredDateChanged();
            }
        }
        private Boolean _IsExpiredDate;
        partial void OnIsExpiredDateChanging(Boolean value);
        partial void OnIsExpiredDateChanged();





        [DataMemberAttribute()]
        public Nullable<Boolean> IsLocatable
        {
            get
            {
                return _IsLocatable;
            }
            set
            {
                OnIsLocatableChanging(value);
                ////ReportPropertyChanging("IsLocatable");
                _IsLocatable = value;
                RaisePropertyChanged("IsLocatable");
                OnIsLocatableChanged();
            }
        }
        private Nullable<Boolean> _IsLocatable;
        partial void OnIsLocatableChanging(Nullable<Boolean> value);
        partial void OnIsLocatableChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_MEDICALE_REL_INHER_RESOURCE", "Resources")]
        public Resource Resource
        {
            get;
            set;
        }

        #endregion
    }
}
