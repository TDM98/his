using DataEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace aEMR.ViewContracts
{
    public interface IIntravenous
    {
        ObservableCollection<Intravenous> IntravenousList { get; set; }
        void ReloadIntravenousList();
        List<ReqOutwardDrugClinicDeptPatient> DeletedIntravenousDetails { get; set; }
        ObservableCollection<ReqOutwardDrugClinicDeptPatient> IntravenousDetailsList { get; set; }
        ICollectionView CVReqDetails { get; set; }
        ObservableCollection<AntibioticTreatment> AntibioticTreatmentCollection { get; set; }
        IList<ReqOutwardDrugClinicDeptPatient> AntibioticTreatmentUsageHistories { get; set; }
        DateTime? MedicalInstructionDate { get; set; }
        bool IsDialogViewObject { get; set; }
        bool IsCOVID { get; set; }
        void ReloadIntravenousListForCreateNewFromOld();
    }
}