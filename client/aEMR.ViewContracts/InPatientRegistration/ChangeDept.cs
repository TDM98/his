using DataEntities;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IChangeDept
    {
        /// <summary>
        /// Khoa sẽ chuyển đến.
        /// </summary>
        IDepartmentListing DestinationDepartmentContent { get; set; }
        //RefDepartment DestinationDepartment { get; set; }
        DeptLocation DestinationDeptLocation { get; }
        //RefDepartment OriginalRefDepartment { get; set; }
        DeptLocation OriginalDeptLocation { get; set; }
        InPatientAdmDisDetails CurrentAdmission { get; set; }
        //InPatientDeptDetail CurInPatientDeptDetail { get; set; }
        ObservableCollection<long> LstRefDepartment { get; set; }
        void LoadData();
        IList<DiagnosisTreatment> DiagnosisTreatmentCollection { get; set; }
        long CurrentDeptID { get; set; }
        bool IsChangedDept { get; set; }
    }
}