using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public partial class HospitalSpecialist : NotifyChangedBase, IEditableObject
    {
        public HospitalSpecialist()
            : base()
        {

        }

        private HospitalSpecialist _tempHospitalSpecialist;
        #region IEditableObject Members

        public void BeginEdit()
        {
            _tempHospitalSpecialist = (HospitalSpecialist)this.MemberwiseClone();
        }

        public void CancelEdit()
        {
            if (null != _tempHospitalSpecialist)
                CopyFrom(_tempHospitalSpecialist);
            //_tempPatient = null;
        }

        public void EndEdit()
        {
        }

        public void CopyFrom(HospitalSpecialist p)
        {
            PropertyCopierHelper.CopyPropertiesTo(p, this);
        }

        #endregion
        #region Factory Method


        /// Create a new HospitalSpecialist object.

        /// <param name="hospSpecID">Initial value of the HospSpecID property.</param>
        /// <param name="hosID">Initial value of the HosID property.</param>
        /// <param name="deptID">Initial value of the DeptID property.</param>
        public static HospitalSpecialist CreateHospitalSpecialist(long hospSpecID, long hosID, long deptID)
        {
            HospitalSpecialist hospitalSpecialist = new HospitalSpecialist();
            hospitalSpecialist.HospSpecID = hospSpecID;
            hospitalSpecialist.HosID = hosID;
            hospitalSpecialist.DeptID = deptID;
            return hospitalSpecialist;
        }

        #endregion
        #region Primitive Properties





        [DataMemberAttribute()]
        public long HospSpecID
        {
            get
            {
                return _HospSpecID;
            }
            set
            {
                if (_HospSpecID != value)
                {
                    OnHospSpecIDChanging(value);
                    ////ReportPropertyChanging("HospSpecID");
                    _HospSpecID = value;
                    RaisePropertyChanged("HospSpecID");
                    OnHospSpecIDChanged();
                }
            }
        }
        private long _HospSpecID;
        partial void OnHospSpecIDChanging(long value);
        partial void OnHospSpecIDChanged();





        [DataMemberAttribute()]
        public long HosID
        {
            get
            {
                return _HosID;
            }
            set
            {
                OnHosIDChanging(value);
                ////ReportPropertyChanging("HosID");
                _HosID = value;
                RaisePropertyChanged("HosID");
                OnHosIDChanged();
            }
        }
        private long _HosID;
        partial void OnHosIDChanging(long value);
        partial void OnHosIDChanged();





        [DataMemberAttribute()]
        public long DeptID
        {
            get
            {
                return _DeptID;
            }
            set
            {
                OnDeptIDChanging(value);
                ////ReportPropertyChanging("DeptID");
                _DeptID = value;
                RaisePropertyChanged("DeptID");
                OnDeptIDChanged();
            }
        }
        private long _DeptID;
        partial void OnDeptIDChanging(long value);
        partial void OnDeptIDChanged();

        #endregion

        #region Navigation Properties






        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HOSPITAL_REL_DM02_HOSPITAL", "Hospitals")]
        public Hospital Hospital
        {
            get;
            set;
        }

        [DataMemberAttribute()]
        // [EdmRelationshipNavigationPropertyAttribute("eHCMS_DBModel", "FK_HOSPITAL_REL_DM04_REFDEPAR", "RefDepartments")]
        public RefDepartment RefDepartment
        {
            get;
            set;
        }

        #endregion
    }
}
