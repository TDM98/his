using System;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    /// <summary>
    /// Su dung cho autocomplete box
    /// </summary>
    public interface IHospitalAutoCompleteListing
    {
        void ResetSelectedHospitalInit();
        int Count { get; set; }
        
        PagedSortableCollectionView<Hospital> HospitalList { get; }
        string SearchCriteria { get; }
        Hospital SelectedHospital { get; set; }
        bool IsLoading { get; }
        bool DisplayHiCode { get; set; }
        bool IsPaperReferal { get; set; }
                
        void SetSelHospital(Hospital selectedHospital);
        void InitBlankBindingValue();
        HealthInsurance CurSelHealthInsuranceCard { get; set; }

        bool IsSearchAll { get; set; }
        long HosID { get; set; }
        IPaperReferalFull Parent { get; set; }        
    }
}
