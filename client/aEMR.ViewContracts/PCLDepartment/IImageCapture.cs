using System.Windows.Media.Imaging;
using DataEntities;
using System.Collections.Generic;
using System.Windows;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace aEMR.ViewContracts
{
    //[Obsolete("ImageCapture is deprecated, please use ImageCapture_V2 instead")]
    public interface IImageCapture
    {
        WriteableBitmap GetCapturedImage();
        PatientPCLImagingResult ObjPatientPCLImagingResult { get; set; }
        List<PCLResultFileStorageDetail> FileForStore { get; set; }
        void ClearSelectedImage();
        void ClearAllCapturedImage();
    }

    public interface IImageCapture_V2
    {
        WriteableBitmap GetCapturedImage();
        PatientPCLImagingResult ObjPatientPCLImagingResult { get; set; }
        List<PCLResultFileStorageDetail> FileForStore { get; set; }
        void ClearSelectedImage();
        void ClearAllCapturedImage();
        ObservableCollection<WriteableBitmap> gImages { get; set; }
        Image gMap { get; set; }
        void CaptureVideoImage();
        bool SnapshotVisible { get; set; }
    }

    public interface IImageCapture_V3
    {
        WriteableBitmap GetCapturedImage();
        PatientPCLImagingResult ObjPatientPCLImagingResult { get; set; }
        List<PCLResultFileStorageDetail> FileForStore { get; set; }
        void ClearSelectedImage();
        void ClearAllCapturedImage();
        ObservableCollection<WriteableBitmap> gImages { get; set; }
        Image gMap { get; set; }
        void CaptureVideoImage();
        bool SnapshotVisible { get; set; }
    }

    public interface IImageCapture_V4
    {
        WriteableBitmap GetCapturedImage();

        // TxD 22/02/2019: Replaced FileForStore with Method GetFileForStore to pass in PatientCode which is being used for local image filename         
        //List<PCLResultFileStorageDetail> FileForStore { get; set; }
        List<PCLResultFileStorageDetail> GetFileForStore(string strPatientCode, bool SaveImageToLocalDrv, bool SaveImageToLocalDrv_WhenCapturing, int CountImagePrintSelect = 0);

        void ClearSelectedImage();
        void ClearAllCapturedImage();
        ObservableCollection<WriteableBitmap> gImages { get; set; }        
        void CaptureVideoImage();
        string PatientCode { get; set; }
    }


    public interface IScanImageCapture
    {
        long StaffID { get;  set; }
        long PtRegistrationID { get;  set; }
        long PatientID { get;  set; }
        string PatientCode { get;  set; }
        WriteableBitmap GetCapturedImage();
        PatientPCLImagingResult ObjPatientPCLImagingResult { get; set; }
        List<ScanImageFileStorageDetail> ScanImageFileToBeSaved { get; set; }
        List<ScanImageFileStorageDetail> ScanImageFileToBeDeleted { get; set; }

        ScanImageFileStorageDetail SavedScanFileStorageDetailSelected { get; set; }

        void ClearSelectedImage();
        void ClearAllCapturedImage();
                        
    }

    public interface IScanImageCaptureView
    {
        Window theScanImageViewWindow { get; }
    }

}
