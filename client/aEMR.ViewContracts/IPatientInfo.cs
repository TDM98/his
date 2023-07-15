using System.Windows;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPatientInfo
    {
        Patient CurrentPatient { get; set; }
        PatientRegistration CurrentRegistration { get; set; }
        double? HiBenefit { get; set; }
        //string CurrentHealthInsuranceNo { get; set; }
        void SetPCLNum(string PCLNum);
        //void InitData();
        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
        ICS_DataStorage CS_DS { get; set; }
        void InitData();
        Visibility IsShowPCL { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        Visibility IsShowPCL_V2 { get; set; }
    }
}