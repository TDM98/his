using System;
using System.Net;
using System.Windows;
using System.Runtime.Serialization;
using eHCMS.Services.Core.Base;
using System.Collections.ObjectModel;
using System.Collections.Generic;

/*
 * 20220725 #001 DatTB: Thêm thông tin nhịp thở RespiratoryRate vào InPtDiagAndDoctorInstruction.
 */
namespace DataEntities
{
    public class InPatientInstruction : NotifyChangedBase
    {
        private long _intPtDiagDrInstructionID;
        [DataMemberAttribute()]
        public long IntPtDiagDrInstructionID
        {
            get
            {
                return _intPtDiagDrInstructionID;
            }
            set
            {
                _intPtDiagDrInstructionID = value;
                RaisePropertyChanged("IntPtDiagDrInstructionID");
            }
        }

        private RefDepartment _department;
        [DataMemberAttribute()]
        public RefDepartment Department
        {
            get
            {
                return _department;
            }
            set
            {
                _department = value;
                RaisePropertyChanged("Department");
            }
        }

        private DeptLocation _locationInDept;
        public DeptLocation LocationInDept
        {
            get
            {
                return _locationInDept;
            }
            set
            {
                _locationInDept = value;
                RaisePropertyChanged("LocationInDept");
            }
        }

        private DateTime _instructionDate;
        [DataMemberAttribute()]
        public DateTime InstructionDate
        {
            get
            {
                return _instructionDate;
            }
            set
            {
                _instructionDate = value;
                RaisePropertyChanged("InstructionDate");
            }
        }

        private Staff _staff;
        [DataMemberAttribute()]
        public Staff Staff
        {
            get
            {
                return _staff;
            }
            set
            {
                _staff = value;
                RaisePropertyChanged("Staff");
            }
        }

        private Staff _doctorStaff;
        [DataMemberAttribute()]
        public Staff DoctorStaff
        {
            get
            {
                return _doctorStaff;
            }
            set
            {
                _doctorStaff = value;
                RaisePropertyChanged("DoctorStaff");
            }
        }

        private string _pulseAndBloodPressure;
        [DataMemberAttribute()]
        public string PulseAndBloodPressure
        {
            get
            {
                return _pulseAndBloodPressure;
            }
            set
            {
                _pulseAndBloodPressure = value;
                RaisePropertyChanged("PulseAndBloodPressure");
            }
        }

        private string _spO2;
        [DataMemberAttribute()]
        public string SpO2
        {
            get
            {
                return _spO2;
            }
            set
            {
                _spO2 = value;
                RaisePropertyChanged("SpO2");
            }
        }

        private string _temperature;
        [DataMemberAttribute()]
        public string Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
                RaisePropertyChanged("Temperature");
            }
        }

        private string _sense;
        [DataMemberAttribute()]
        public string Sense
        {
            get
            {
                return _sense;
            }
            set
            {
                _sense = value;
                RaisePropertyChanged("Sense");
            }
        }

        private string _bloodSugar;
        [DataMemberAttribute()]
        public string BloodSugar
        {
            get
            {
                return _bloodSugar;
            }
            set
            {
                _bloodSugar = value;
                RaisePropertyChanged("BloodSugar");
            }
        }

        private string _urine;
        [DataMemberAttribute()]
        public string Urine
        {
            get
            {
                return _urine;
            }
            set
            {
                _urine = value;
                RaisePropertyChanged("Urine");
            }
        }

        private string _ecg;
        [DataMemberAttribute()]
        public string ECG
        {
            get
            {
                return _ecg;
            }
            set
            {
                _ecg = value;
                RaisePropertyChanged("ECG");
            }
        }

        private string _physicalExamOther;
        [DataMemberAttribute()]
        public string PhysicalExamOther
        {
            get
            {
                return _physicalExamOther;
            }
            set
            {
                _physicalExamOther = value;
                RaisePropertyChanged("PhysicalExamOther");
            }
        }

        private string _nebulized;
        [DataMemberAttribute()]
        public string Nebulized
        {
            get
            {
                return _nebulized;
            }
            set
            {
                _nebulized = value;
                RaisePropertyChanged("Nebulized");
            }
        }

        private string _diet;
        [DataMemberAttribute()]
        public string Diet
        {
            get
            {
                return _diet;
            }
            set
            {
                _diet = value;
                RaisePropertyChanged("Diet");
            }
        }

        private long _levelCare;
        [DataMemberAttribute()]
        public long LevelCare
        {
            get
            {
                return _levelCare;
            }
            set
            {
                _levelCare = value;
                RaisePropertyChanged("LevelCare");
            }
        }

        private Lookup _V_levelCare;
        [DataMemberAttribute()]
        public Lookup V_LevelCare
        {
            get
            {
                return _V_levelCare;
            }
            set
            {
                _V_levelCare = value;
                RaisePropertyChanged("V_LevelCare");
            }
        }

        private DiagnosisTreatment _dailyDiagnosisTreatment;
        [DataMemberAttribute()]
        public DiagnosisTreatment DailyDiagnosisTreatment
        {
            get
            {
                return _dailyDiagnosisTreatment;
            }
            set
            {
                _dailyDiagnosisTreatment = value;
                RaisePropertyChanged("DailyDiagnosisTreatment");
            }
        }

        private ObservableCollection<PatientRegistrationDetail> _registrationDetails;
        [DataMemberAttribute()]
        public ObservableCollection<PatientRegistrationDetail> RegistrationDetails
        {
            get
            {
                return _registrationDetails;
            }
            set
            {
                _registrationDetails = value;
                RaisePropertyChanged("RegistrationDetails");
            }
        }

        private ObservableCollection<PatientPCLRequest> _pclRequests;
        [DataMemberAttribute()]
        public ObservableCollection<PatientPCLRequest> PclRequests
        {
            get
            {
                return _pclRequests;
            }
            set
            {
                _pclRequests = value;
                RaisePropertyChanged("PclRequests");
            }
        }

        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> _reqOutwardDetails;
        [DataMemberAttribute()]
        public ObservableCollection<ReqOutwardDrugClinicDeptPatient> ReqOutwardDetails
        {
            get
            {
                return _reqOutwardDetails;
            }
            set
            {
                _reqOutwardDetails = value;
                RaisePropertyChanged("ReqOutwardDetails");
            }
        }

        private List<Intravenous> _intravenousPlan;
        [DataMemberAttribute()]
        public List<Intravenous> IntravenousPlan
        {
            get
            {
                return _intravenousPlan;
            }
            set
            {
                _intravenousPlan = value;
                RaisePropertyChanged("IntravenousPlan");
            }
        }

        private long _InPatientDeptDetailID;
        [DataMemberAttribute()]
        public long InPatientDeptDetailID
        {
            get
            {
                return _InPatientDeptDetailID;
            }
            set
            {
                _InPatientDeptDetailID = value;
                RaisePropertyChanged("InPatientDeptDetailID");
            }
        }

        private long? _BedPatientID;
        [DataMemberAttribute()]
        public long? BedPatientID
        {
            get
            {
                return _BedPatientID;
            }
            set
            {
                _BedPatientID = value;
                RaisePropertyChanged("BedPatientID");
            }
        }

        private bool _IsChecked;
        [DataMemberAttribute]
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                if (_IsChecked == value)
                {
                    return;
                }
                _IsChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }

        private bool _IsCreatedOutward;
        [DataMemberAttribute]
        public bool IsCreatedOutward
        {
            get
            {
                return _IsCreatedOutward;
            }
            set
            {
                if (_IsCreatedOutward == value)
                {
                    return;
                }
                _IsCreatedOutward = value;
                RaisePropertyChanged("IsCreatedOutward");
            }
        }

        private DateTime _LastModifiedDate;
        [DataMemberAttribute]
        public DateTime LastModifiedDate
        {
            get
            {
                return _LastModifiedDate;
            }
            set
            {
                if (_LastModifiedDate == value)
                {
                    return;
                }
                _LastModifiedDate = value;
                RaisePropertyChanged("LastModifiedDate");
            }
        }
        private string _InstructionOther;
        [DataMemberAttribute]
        public string InstructionOther
        {
            get
            {
                return _InstructionOther;
            }
            set
            {
                if (_InstructionOther == value)
                {
                    return;
                }
                _InstructionOther = value;
                RaisePropertyChanged("InstructionOther");
            }
        }

        //▼==== #001
        private string _RespiratoryRate;
        [DataMemberAttribute()]
        public string RespiratoryRate
        {
            get
            {
                return _RespiratoryRate;
            }
            set
            {
                _RespiratoryRate = value;
                RaisePropertyChanged("RespiratoryRate");
            }
        }
        //▲==== #001
    }
}