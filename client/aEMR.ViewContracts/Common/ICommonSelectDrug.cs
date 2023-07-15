
using DataEntities;
using System;
namespace aEMR.ViewContracts
{
    public interface ICommonSelectDrug
    {
        Action<ReqOutwardDrugClinicDeptPatient> AddItemCallback { get; set; }
    }
}