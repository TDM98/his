using System.Collections.ObjectModel;
using DataEntities;
using System.Collections.Generic;
/*
 * 20180830 #001 TTM: Bổ sung hàm lấy hình ảnh để ViewModel khác gọi thông qua Interface
 */
namespace aEMR.ViewContracts
{
    public interface IPatientPCLDeptImagingResult
    {
        //object UCHeaderInfoPMR { get; set; }
        bool IsEnableButton { get; set; }
        //▼====== #001
        void GetImageInAnotherViewModel(PatientPCLRequest message);
        //▲====== #001
        void LoadData(PatientPCLRequest message);
        // Txd 07/10/2016 : Added the following so that ObjPatientPCLImagingResult is stored in SieuAmTim VM and can be shared between 
        // PatientPCLDeptImagingResult VM and ImageCapture VM 
        PatientPCLImagingResult ObjPatientPCLImagingResult { get; set; }
        List<PCLResultFileStorageDetail> FileForStore { get; set; }
        List<PCLResultFileStorageDetail> FileForDelete { get; set; }
        //==== 20161013 CMN Begin: Add PCL Image Method
        int TotalFile { get; }
        //==== 20161013 CMN End.
        int GetNumOfImageResultFiles();
        string GetImageResultFileStoragePath(int nIdx);
        IList<PCLResultFileStorageDetail> PCLResultFileStorageDetailCollection { get; }
        List<PCLResultFileStorageDetail> FileForUpdate { get; }
    }
}