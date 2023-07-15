using DataEntities;
using System.Collections.ObjectModel;

/*
 * 20180920 #001 TBL: Added IsDiagTrmentChanged
 */ 
namespace aEMR.ViewContracts
{
    public interface IPhysicalVitalSigns
    {
        PhysicalExamination PtPhyExamItem { get; }
        Patient PatientInfo { set;  get; }
        PatientRegistration CurrentPatientRegistration { set; get; }
    }
}