/*
 * 20170310 #001 CMN: Add Variable for visible admission checkbox
*/
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;

namespace aEMR.ViewContracts
{
    public interface IFindRegistrationInPt
    {
        void CancelCmd();
        void OkCmd();
        void SearchCmd();
        void ResetFilterCmd();
        void DoubleClick(object args);
        void LoadRefDeparments();

        bool IsLoading { get; set; }
        PatientRegistration SelectedRegistration { get; set; }
        SeachPtRegistrationCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<PatientRegistration> Registrations { get; }
        ObservableCollection<Staff> Staffs { get; set; }
        ObservableCollection<Lookup> RegStatusList { get; set; }
        ObservableCollection<DeptLocation> DeptLocations { get; set; }
        DeptLocation SelectedLocation { get; set; }

        void CopyExistingPatientList(IList<PatientRegistration> items, SeachPtRegistrationCriteria criteria, int total);
        bool CloseAfterSelection { get; set; }
        string pageTitle { get; set; }
        //tìm để Khám bệnh -> tìm trong ngày -> không có thì tìm bệnh nhân
        bool IsSearchGoToKhamBenh { get; set; }
        bool IsPopup { get; set; }
        bool IsEnableCbx { get; set; }

        bool CanSearhRegAllDept { get; set; }

        LeftModuleActive LeftModule { get; set; }

        bool? SearchAdmittedInPtRegOnly { get; set; }
        //==== #001
        bool SearchOnlyNotAdmitted { get; set; }
        //==== #001

        bool IsSearchPtByNameChecked { get; set; }
        bool IsAllowSearchingPtByName_Visible { get; set; }

        bool IsProcedureEdit { get; set; }
        void SetValueForDischargedStatus(bool SearchAdmittedInPtRegOnly);
        bool IsSearchForCashAdvance { get; set; }
    }
}
