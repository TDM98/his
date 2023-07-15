using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Dung cho ben thuoc cua Ny.
    /// </summary>
    public interface ISearchRegistration
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
    }
}
