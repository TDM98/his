using DataEntities;
using System.Collections.Generic;

namespace aEMR.ViewContracts
{
    public interface IPatientPCLGeneralResult
    {
        string ContentBody { get; set; }
        string GetBodyValue(IList<PCLResultFileStorageDetail> imagesSavedInCurrentSession, IList<PCLResultFileStorageDetail> imagesNeedToDelete, out string Description, out string Result);
        void ApplyElementValues(string aValueArray, PatientPCLRequest aPatientPCLRequest, IList<PCLResultFileStorageDetail> aPCLResultFileStorageDetailCollection, string PtRegistrationCode, string StaffFullName, string Suggest);
        string FileName { get; set; }
        void ApplyImages(IList<PCLResultFileStorageDetail> PCLResultFileStorageDetailCollection);
        IAucHoldConsultDoctor aucHoldConsultDoctor { get; set; }

        PatientPCLImagingResult ObjPatientPCLImagingResult_General { get; set; }

        IAucHoldConsultDoctor aucDoctorResult { get; set; }
        System.Collections.ObjectModel.ObservableCollection<Resources> HIRepResourceCode { get; set; }
        void RenewWebContent();
    }
}