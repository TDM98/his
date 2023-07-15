
using System;
using System.Collections.ObjectModel;
using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IModify_ReqQty
    {
        RequestDrugInwardClinicDept RequestDrug { get; set; }
        //ObservableCollection<ReqOutwardDrugClinicDeptPatient> RequestDetails { get; set; }
        RequestDrugInwardForHiStore RequestDrugHIStore { get; set; }
    }
}
