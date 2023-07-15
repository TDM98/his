/*
 * 20170608 #001 CMN: Added PatientType to Update PCL
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using DataEntities;
using eHCMS.Services.Core;
using System.IO;
using ErrorLibrary;
namespace ConsultationsService.ParaClinical
{
    [ServiceContract]
    public interface IPCLsImport
    {
        #region 0. examGroup Member
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamGroup> GetExamGroup_All();

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PCLExamGroup> GetExamGroup_ByPatientReqID(long PatientPCLReqID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PCLExamGroup GetExamGroup_ByID(long PCLExamGroupID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //PCLExamGroup GetExamGroup_ByType(long PCLExamTypeID);
        #endregion

        #region 1. ExamTypes Member
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PCLExamType> GetExamTypes_ByGroup(long PCLExamGroupID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PCLExamType> GetExamTypes_ByPatientReqID(long PatientPCLReqID, long PCLExamGroupID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamType> PCLExamTypes_ByPatientPCLReqID(long PatientPCLReqID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PCLExamType> GetExamTypeGroup_TreeView(long PatientPCLReqID);

        

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<ExamGroupTypeTree> GetTreeView_ExamGroupType(long PatientPCLReqID);
        #endregion

        #region 2. PatientPCLImagingResults Member
        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientPCLImagingResult GetPatientPCLImagingResults_ByPtID(long PatientID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientPCLImagingResult GetPatientPCLImagingResults_ByID(long PatientPCLReqID, long PCLExamTypeID);

        /*▼====: #001*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientPCLImagingResult GetPatientPCLImagingResults_ByID_V2(long PatientPCLReqID, long PCLExamTypeID, long V_PCLRequestType);
        /*▲====: #001*/

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientPCLImagingResult GetPatientPCLImagingResults_ByIDExt(PCLResultFileStorageDetailSearchCriteria p);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        IList<PatientPCLImagingResult> GetPatientPCLImagingResults_ByPatientPCLReqID(long PatientPCLReqID);
        #endregion

        #region 3. PatientMedicalRecords

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientMedicalRecord spMedicalRecord_BlankByPtID(long PatientID);
        #endregion

        #region 4. PatientPCLRequest member
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PatientPCLRequest> GetPatientPCLRequest_ByPtID(long PatientID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PatientPCLRequest> GetPatientPCLRequest_ByRegistrationID(long regID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PatientPCLRequest> PatientPCLRequest_ByPtRegIDV_PCLCategory(long PtRegistrationID, long V_PCLCategory);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PatientPCLRequest> GetPatientPCLRequest_ByPtIDAll(long PatientID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PatientPCLRequest> GetPatientPCLRequest_ByServiceRecID(long PatientID, long? ServiceRecID, long? PtRegistrationID, DateTime? ExamDate,bool? IsImport);

        


        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<PatientPCLRequest> PatientPCLRequest_OldByPatientIDPtRegistrationID(Int64 PatientID, Int64 PtRegistrationID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PatientPCLRequest_UpdateV_PCLRequestStatus(Int64 PatientPCLReqID, Int64 V_PCLRequestStatus, Int64 PCLReqItemID, Int64 V_ExamRegStatus, out string Result);

        #endregion

        #region 6. PCLResultFileStorageDetails member
        //==== 20161005 CMN Begin: Combine Upload file
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UploadImageToDatabase(PatientPCLImagingResult ExamResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete);
        //==== 20161005 CMN End.

        //==== 20161007 CMN Begin: Add List for delete file
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddPCLResultFileStorageDetails(PatientPCLImagingResult ImagingResult, List<PCLResultFileStorageDetail> ResultFile, List<PCLResultFileStorageDetail> FileForDelete, List<PCLResultFileStorageDetail> FileForUpdate);
        //bool AddPCLResultFileStorageDetails(PatientPCLImagingResult ImagingResult, IList<PCLResultFileStorageDetail> ResultFile);
        //==== 20161007 CMN End.

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLResultFileStorageDetail> GetPCLResultFileStorageDetails_ByID(long PatientPCLReqID, long PCLExamTypeID);

        #endregion

        #region Save Scan Image to File and Details to DB

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddScanFileStorageDetails(long StaffID, long PatientID, long PtRegistrationID, string strPatientCode, List<ScanImageFileStorageDetail> NewFileToSave, List<ScanImageFileStorageDetail> SavedFileToDelete);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ScanImageFileStorageDetail> GetSavedScanFileStorageDetails_ByPtRegID(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<ScanImageFileStorageDetail> GetSavedScanFileStorageDetails_ByPatientID(long PatientID);

        #endregion

        #region 7. TestingAgency member
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<TestingAgency> GetTestingAgency_All();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool TestingAgency_Insert(TestingAgency Agency);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool TestingAgency_InsertXML(List<TestingAgency> Agency);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool TestingAgency_DeleteXML(List<TestingAgency> Agency);

        #endregion

        #region 8. Hopitals member
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Hospital> GetHospital_Auto(string HosName, int PageIndex, int PageSize);
        #endregion

        #region explorer file
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<FolderTree> FillDirectoryAll(string dir,int type);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<String> GetFolderList(string dir);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CopyAll(IList<PCLResultFileStorageDetail> ListDetails, string ImageStore);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteFile(string path, List<String> ListPath);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveImageClipBoard(List<byte[]> buffer, string ImageStore);
        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SavePCLImagingResultPDF(byte[] FileExport, string FileName);
    }
}
