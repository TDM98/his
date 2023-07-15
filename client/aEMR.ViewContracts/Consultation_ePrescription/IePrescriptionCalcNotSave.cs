using System;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IePrescriptionCalcNotSave
    {
        Prescription ObjPrescription { get; set; }
        long StoreID { get;set;}
        Int16 RegistrationType { get; set; }
        void Calc();
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void CalcAll();
    }
}