using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;
/*
 * 20211028 #001 TNHX: 757 Thêm đánh dấu khoa dùng để điều trị COVID
 * 20220225 #002 QTD:  Thêm trường số ngày cho phép nhập y lệnh so với ngày hiện tại
 */
namespace DataEntities
{
    public partial class RefDepartment : NotifyChangedBase
    {
        #region Factory Method


        /// Create a new RefDepartment object.

        /// <param name="deptID">Initial value of the DeptID property.</param>
        /// <param name="deptName">Initial value of the DeptName property.</param>
        public static RefDepartment CreateRefDepartment(long deptID, String deptName)
        {
            RefDepartment refDepartment = new RefDepartment();
            refDepartment.DeptID = deptID;
            refDepartment.DeptName = deptName;
            return refDepartment;
        }

        #endregion
        #region Primitive Properties
        //▼====: #001
        [DataMemberAttribute()]
        public bool IsTreatmentForCOVID
        {
            get { return _IsTreatmentForCOVID; }
            set
            {
                _IsTreatmentForCOVID = value;
                RaisePropertyChanged("IsTreatmentForCOVID");
            }
        }
        private bool _IsTreatmentForCOVID = false;
        //▲====: #001
        [DataMemberAttribute()]
        public long DeptID
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
                    ////ReportPropertyChanging("DeptID");
                    _DeptID = value;
                    RaisePropertyChanged("DeptID");
                    OnDeptIDChanged();
                }
            }
        }
        private long _DeptID;
        partial void OnDeptIDChanging(long value);
        partial void OnDeptIDChanged();

        [DataMemberAttribute()]
        public Nullable<long> ParDeptID
        {
            get
            {
                return _ParDeptID;
            }
            set
            {
                OnParDeptIDChanging(value);
                ////ReportPropertyChanging("ParDeptID");
                _ParDeptID = value;
                RaisePropertyChanged("ParDeptID");
                OnParDeptIDChanged();
            }
        }
        private Nullable<long> _ParDeptID;
        partial void OnParDeptIDChanging(Nullable<long> value);
        partial void OnParDeptIDChanged();


        [DataMemberAttribute()]
        public String DeptName
        {
            get
            {
                return _DeptName;
            }
            set
            {
                OnDeptNameChanging(value);
                ////ReportPropertyChanging("DeptName");
                _DeptName = value;
                RaisePropertyChanged("DeptName");
                OnDeptNameChanged();
            }
        }
        private String _DeptName;
        partial void OnDeptNameChanging(String value);
        partial void OnDeptNameChanged();

        [DataMemberAttribute()]
        public String DeptShortName
        {
            get
            {
                return _DeptShortName;
            }
            set
            {
                OnDeptShortNameChanging(value);
                ////ReportPropertyChanging("DeptName");
                _DeptShortName = value;
                RaisePropertyChanged("DeptShortName");
                OnDeptShortNameChanged();
            }
        }
        private String _DeptShortName;
        partial void OnDeptShortNameChanging(String value);
        partial void OnDeptShortNameChanged();

        [DataMemberAttribute()]
        public long? V_DeptTypeOperation
        {
            get
            {
                return _V_DeptTypeOperation;
            }
            set
            {
                OnV_DeptTypeOperationChanging(value);
                _V_DeptTypeOperation = value;
                RaisePropertyChanged("V_DeptTypeOperation");
                OnV_DeptTypeOperationChanged();
            }
        }
        private long? _V_DeptTypeOperation;
        partial void OnV_DeptTypeOperationChanging(long? value);
        partial void OnV_DeptTypeOperationChanged();


        [DataMemberAttribute()]
        public String DeptDescription
        {
            get
            {
                return _DeptDescription;
            }
            set
            {
                OnDeptDescriptionChanging(value);
                ////ReportPropertyChanging("DeptDescription");
                _DeptDescription = value;
                RaisePropertyChanged("DeptDescription");
                OnDeptDescriptionChanged();
            }
        }
        private String _DeptDescription;
        partial void OnDeptDescriptionChanging(String value);
        partial void OnDeptDescriptionChanged();

        [DataMemberAttribute()]
        public long V_DeptType
        {
            get
            {
                return _V_DeptType;
            }
            set
            {
                OnV_DeptTypeChanging(value);
                ////ReportPropertyChanging("V_DeptType");
                _V_DeptType = value;
                RaisePropertyChanged("V_DeptType");
                OnV_DeptTypeChanged();
            }
        }
        private long _V_DeptType;
        partial void OnV_DeptTypeChanging(long value);
        partial void OnV_DeptTypeChanged();

        [DataMemberAttribute()]
        public Lookup VDeptType
        {
            get
            {
                return _VDeptType;
            }
            set
            {
                OnVDeptTypeChanging(value);
                ////ReportPropertyChanging("VDeptType");
                _VDeptType = value;
                RaisePropertyChanged("VDeptType");
                OnVDeptTypeChanged();
            }
        }
        private Lookup _VDeptType;
        partial void OnVDeptTypeChanging(Lookup value);
        partial void OnVDeptTypeChanged();

        [DataMemberAttribute()]
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            set
            {
                _IsDeleted = value;
                RaisePropertyChanged("IsDeleted");
            }
        }
        private bool _IsDeleted = false;

        // TxD 09/05/2015: Added the following attribute to indicate that if a department is selected then you must also select a room within that department.
        //                 Initially this flag is used for creating In-Patient Bill of the USIC-CC department
        [DataMemberAttribute()]
        public bool SelectDeptReqSelectRoom
        {
            get { return _SelectDeptReqSelectRoom; }
            set
            {
                _SelectDeptReqSelectRoom = value;
                RaisePropertyChanged("SelectDeptReqSelectRoom");
            }
        }
        private bool _SelectDeptReqSelectRoom = false;

        [DataMemberAttribute()]
        public bool IsAllowableInTemp
        {
            get { return _IsAllowableInTemp; }
            set
            {
                _IsAllowableInTemp = value;
                RaisePropertyChanged("IsAllowableInTemp");
            }
        }
        private bool _IsAllowableInTemp = false;
        #endregion

        #region Navigation Properties

        [DataMemberAttribute()]
        public ObservableCollection<DeptLocation> DeptLocations
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<DiseasesReference> DiseasesReferences
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        public ObservableCollection<HospitalizationHistoryDetail> HospitalizationHistoryDetails
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HOSPITAL_REL_DM04_REFDEPAR", "HospitalSpecialists")]
        public ObservableCollection<HospitalSpecialist> HospitalSpecialists
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PATIENTR_REL_PTINF_REFDEPAR", "PatientRegistration")]
        public ObservableCollection<PatientRegistration> PatientRegistrations
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_PCLEXAMG_REL_REQPC_REFDEPAR", "PCLExamGroup")]
        public ObservableCollection<PCLExamGroup> PCLExamGroups
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_REFMEDIC_REL_HOSFM_REFDEPAR", "RefMedicalServiceItems")]
        public ObservableCollection<RefMedicalServiceItem> RefMedicalServiceItems
        {
            get;
            set;
        }






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_STAFFS_REL_HR05_REFDEPAR", "Staffs")]
        public ObservableCollection<Staff> Staffs
        {
            get;
            set;
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(this, obj))
                return true;

            RefDepartment cond = obj as RefDepartment;
            if (cond == null)
                return false;

            return this._DeptID == cond.DeptID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //▼====: #002
        [DataMemberAttribute()]
        public int NumOfDayAllowMedicalInstruction
        {
            get { return _NumOfDayAllowMedicalInstruction; }
            set
            {
                _NumOfDayAllowMedicalInstruction = value;
                RaisePropertyChanged("NumOfDayAllowMedicalInstruction");
            }
        }
        private int _NumOfDayAllowMedicalInstruction = 0;
        //▲====: #002
    }
}
