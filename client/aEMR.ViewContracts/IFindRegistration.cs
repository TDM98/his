using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    public interface IFindRegistration
    {
        void CancelCmd();
        void OkCmd();
        void SearchCmd();
        void ResetFilterCmd();
        void DoubleClick(object args);

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
        bool IsSearchGoToKhamBenh { get;set;}
        bool IsPopup { get; set; }
        bool IsEnableCbx { get; set; }
        bool IsSearchPtByNameChecked { get; set; }
        bool IsAllowSearchingPtByName_Visible { get; set; }
    }
}
