using System;
using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    public interface IDepartmentListing
    {
        ObservableCollection<RefDepartment> Departments { get; set; }
        RefDepartment SelectedItem { get; set; }
        ObservableCollection<long> LstRefDepartment { get; set; }
        long curDeptID { get; set; }

        bool AddSelectOneItem { get; set; }
        bool AddSelectedAllItem { get; set; }
        
        void LoadData(long curDeptID = 0, bool bLoadAllDepts = false, bool isPKVVRhm = false, bool isLoadAllDeptsForKTHT = false);
        
        bool SetSelDeptExt { get; set; }

        void SetSelectedDeptItem(long selDeptID);
        bool IsShowOnlyAllowableInTemp { get; set; }
        bool IsLoadDrugDept { get; set; }
        bool IsAdmRequest { get; set; }
        bool IsNotShowDeptDeleted { get; set; }
        bool IsShowExaminationDept { get; set; }
    }
}
