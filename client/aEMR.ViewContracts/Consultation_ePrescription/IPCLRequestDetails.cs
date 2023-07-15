

using System.Collections.ObjectModel;
using DataEntities;
using System;
namespace aEMR.ViewContracts
{
    public interface IPCLRequestDetails
    {
        PatientPCLRequest PCLRequest { get; set; }
        ObservableCollection<PatientPCLRequestDetail> MaskedDetailList { get; }
        void AddItemToView(PatientPCLRequestDetail requestDetail);
        void RemoveItemFromView(PatientPCLRequestDetail requestDetail);
        void ResetCollection();
        ObservableCollection<PatientPCLRequest> ObjPatientPCLRequest_ByRegistrationID { get; set; }
        //event EventHandler detailListChanged;
        bool IsEnableListPCL { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}
