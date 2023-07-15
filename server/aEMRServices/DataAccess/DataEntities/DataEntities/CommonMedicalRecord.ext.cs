using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;


namespace DataEntities
{
    public partial class CommonMedicalRecord : NotifyChangedBase, IEditableObject
    {
        private const string PATIENTID_REQUIRED     = "Patient ID is required.";
        private const string STAFFID_REQUIRED       = "Staff is required.";
        private const string CMRMODDIFIED_REQUIRED  = "CMR Modified Date  is required.";

        partial void OnPatientIDChanging(Nullable<long> value)
        {
            if (value == null)
            {
                AddError("PatientID", PATIENTID_REQUIRED, false);
            }
            else
            {
                RemoveError("PatientID", PATIENTID_REQUIRED);
            }
        }

        public override bool Equals(object obj)
        {
            CommonMedicalRecord cond = obj as CommonMedicalRecord;
            if (cond == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            return this.CommonMedRecID == cond.CommonMedRecID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
