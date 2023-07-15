using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-10-22
 * Contents: Hospitalization extend
/*******************************************************************/
#endregion

namespace DataEntities
{
    public partial class HospitalizationHistory
    {
        private const int ADMISSION_TYPE_EMERGENCY = 1101;
        private const int ADMISSION_TYPE_EXAM_DEPT = 1102;
        private const int ADMISSION_TYPE_TREATM_DEPT = 1103;
        
        public bool VAdmissionType_Emergency
        {
            get
            {
                return (this.V_AdmissionType == ADMISSION_TYPE_EMERGENCY);
            }
            set
            {
            }
        }
        public bool VAdmissionType_ExamDept
        {
            get
            {
                return (this.V_AdmissionType == ADMISSION_TYPE_EXAM_DEPT);
            }
            set
            {
            }
        }
        public bool VAdmissionType_TreatmDept
        {
            get
            {
                return (this.V_AdmissionType == ADMISSION_TYPE_TREATM_DEPT);
            }
            set
            {
            }
        }

    }
}
