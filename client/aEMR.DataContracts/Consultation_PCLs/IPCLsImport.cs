/*
 * 20170608 #001 CMN: Added PatientType to Update PCL
*/
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;

namespace PCLsService
{
    [ServiceContract]
    public interface IPCLsImport
    {

        #region 0. examGroup Member
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetExamGroup_All(AsyncCallback callback, object state);
        IList<PCLExamGroup> EndGetExamGroup_All(IAsyncResult asyncResult) ;

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetExamGroup_ByPatientReqID(long PatientPCLReqID, AsyncCallback callback, object state);
        //IList<PCLExamGroup> EndGetExamGroup_ByPatientReqID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetExamGroup_ByID(long PCLExamGroupID, AsyncCallback callback, object state);
        PCLExamGroup EndGetExamGroup_ByID(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetExamGroup_ByType(long PCLExamTypeID, AsyncCallback callback, object state);
        //PCLExamGroup EndGetExamGroup_ByType(IAsyncResult asyncResult);
        #endregion

        #region 1. ExamTypes Member
        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetExamTypes_ByGroup(long PCLExamGroupID, AsyncCallback callback, object state);
        //IList<PCLExamType> EndGetExamTypes_ByGroup(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetExamTypes_ByPatientReqID(long PatientPCLReqID, long PCLExamGroupID, AsyncCallback callback, object state);
        //IList<PCLExamType> EndGetExamTypes_ByPatientReqID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypes_ByPatientPCLReqID(long PatientPCLReqID, AsyncCallback callback, object state);
        IList<PCLExamType> EndPCLExamTypes_ByPatientPCLReqID(IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetExamTypeGroup_TreeView(long PatientPCLReqID, AsyncCallback callback, object state);
        //IList<PCLExamType> EndGetExamTypeGroup_TreeView(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTreeView_ExamGroupType(long PatientPCLReqID, AsyncCallback callback, object state);
        List<ExamGroupTypeTree> EndGetTreeView_ExamGroupType(IAsyncResult asyncResult);
        #endregion

        #region 2. PatientPCLImagingResults Member
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientPCLImagingResults_ByPtID(long PatientID, AsyncCallback callback, object state);
        PatientPCLImagingResult EndGetPatientPCLImagingResults_ByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientPCLImagingResults_ByID(long PatientPCLReqID, long PCLExamTypeID, AsyncCallback callback, object state);
        PatientPCLImagingResult EndGetPatientPCLImagingResults_ByID(IAsyncResult asyncResult);

        /*▼====: #001*/
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientPCLImagingResults_ByID_V2(long PatientPCLReqID, long PCLExamTypeID, long V_PCLRequestType, AsyncCallback callback, object state);
        PatientPCLImagingResult EndGetPatientPCLImagingResults_ByID_V2(IAsyncResult asyncResult);
        /*▲====: #001*/

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientPCLImagingResults_ByIDExt(PCLResultFileStorageDetailSearchCriteria p, AsyncCallback callback, object state);
        PatientPCLImagingResult EndGetPatientPCLImagingResults_ByIDExt(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientPCLImagingResults_ByPatientPCLReqID(long PatientPCLReqID, AsyncCallback callback, object state);
        IList<PatientPCLImagingResult> EndGetPatientPCLImagingResults_ByPatientPCLReqID(IAsyncResult asyncResult);
        #endregion

        #region 3. PatientMedicalRecords

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginspMedicalRecord_BlankByPtID(long PatientID, AsyncCallback callback, object state);
        PatientMedicalRecord EndspMedicalRecord_BlankByPtID(IAsyncResult asyncResult);
        #endregion

        #region 4. PatientPCLRequest member
        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetPatientPCLRequest_ByPtID(long PatientID, AsyncCallback callback, object state);
        //IList<PatientPCLRequest> EndGetPatientPCLRequest_ByPtID(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginPatientPCLRequest_ByPtRegIDV_PCLCategory(long PtRegistrationID, long V_PCLCategory, AsyncCallback callback, object state);
        //IList<PatientPCLRequest> EndPatientPCLRequest_ByPtRegIDV_PCLCategory(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetPatientPCLRequest_ByRegistrationID(long regID, AsyncCallback callback, object state);
        //IList<PatientPCLRequest> EndGetPatientPCLRequest_ByRegistrationID(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetPatientPCLRequest_ByPtIDAll(long PatientID, AsyncCallback callback, object state);
        //IList<PatientPCLRequest> EndGetPatientPCLRequest_ByPtIDAll(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetPatientPCLRequest_ByServiceRecID(long PatientID, long? ServiceRecID, long? PtRegistrationID, DateTime? ExamDate, bool? IsImport, AsyncCallback callback, object state);
        //IList<PatientPCLRequest> EndGetPatientPCLRequest_ByServiceRecID(IAsyncResult asyncResult);

        
        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginPatientPCLRequest_OldByPatientIDPtRegistrationID(Int64 PatientID, Int64 PtRegistrationID, AsyncCallback callback, object state);
        //IList<PatientPCLRequest> EndPatientPCLRequest_OldByPatientIDPtRegistrationID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientPCLRequest_UpdateV_PCLRequestStatus(Int64 PatientPCLReqID, Int64 V_PCLRequestStatus, Int64 PCLReqItemID, Int64 V_ExamRegStatus, AsyncCallback callback, object state);
        void EndPatientPCLRequest_UpdateV_PCLRequestStatus(out string Result, IAsyncResult asyncResult);


        #endregion

        #region 6. PCLResultFileStorageDetails member
        //==== 20161005 CMN Begin: Combine Upload file
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUploadImageToDatabase(PatientPCLImagingResult ExamResult, List<PCLResultFileStorageDetail> FileForStorage, List<PCLResultFileStorageDetail> FileForDelete, AsyncCallback callback, object state);
        bool EndUploadImageToDatabase(IAsyncResult asyncResult);
        //==== 20161005 CMN End.

        //==== 20161007 CMN Begin: Add List for delete file
        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginAddPCLResultFileStorageDetails(PatientPCLImagingResult ImagingResult, IList<PCLResultFileStorageDetail> ResultFile, AsyncCallback callback, object state);
        //bool EndAddPCLResultFileStorageDetails(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddPCLResultFileStorageDetails(PatientPCLImagingResult ImagingResult, IList<PCLResultFileStorageDetail> ResultFile, IList<PCLResultFileStorageDetail> FileForDelete, IList<PCLResultFileStorageDetail> FileForUpdate, AsyncCallback callback, object state);
        bool EndAddPCLResultFileStorageDetails(IAsyncResult asyncResult);
        //==== 20161007 CMN End.

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLResultFileStorageDetails_ByID(long PatientPCLReqID, long PCLExamTypeID, AsyncCallback callback, object state);
        IList<PCLResultFileStorageDetail> EndGetPCLResultFileStorageDetails_ByID(IAsyncResult asyncResult);

        #endregion

        #region Save Scan Image to File and Details to DB

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddScanFileStorageDetails(long StaffID, long PatientID, long PtRegistrationID, string strPatientCode, List<ScanImageFileStorageDetail> NewFileToSave, List<ScanImageFileStorageDetail> SavedFileToDelete, AsyncCallback callback, object state);
        bool EndAddScanFileStorageDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSavedScanFileStorageDetails_ByPtRegID(long PtRegistrationID, AsyncCallback callback, object state);
        IList<ScanImageFileStorageDetail> EndGetSavedScanFileStorageDetails_ByPtRegID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSavedScanFileStorageDetails_ByPatientID(long PatientID, AsyncCallback callback, object state);
        IList<ScanImageFileStorageDetail> EndGetSavedScanFileStorageDetails_ByPatientID(IAsyncResult asyncResult);

        #endregion

        #region 7. TestingAgency member
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetTestingAgency_All(AsyncCallback callback, object state);
        IList<TestingAgency> EndGetTestingAgency_All(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginTestingAgency_Insert(TestingAgency Agency, AsyncCallback callback, object state);
        bool EndTestingAgency_Insert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginTestingAgency_Update(TestingAgency Agency, AsyncCallback callback, object state);
        bool EndTestingAgency_Update(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginTestingAgency_Delete(TestingAgency Agency, AsyncCallback callback, object state);
        bool EndTestingAgency_Delete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginTestingAgency_InsertXML(List<TestingAgency> Agency, AsyncCallback callback, object state);
        bool EndTestingAgency_InsertXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginTestingAgency_DeleteXML(List<TestingAgency> Agency, AsyncCallback callback, object state);
        bool EndTestingAgency_DeleteXML(IAsyncResult asyncResult);

        #endregion

        #region 8. Hopitals member
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetHospital_Auto(string HosName, int PageIndex, int PageSize, AsyncCallback callback, object state);
        IList<Hospital> EndGetHospital_Auto(IAsyncResult asyncResult);
        #endregion

        #region explorer file
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginFillDirectoryAll(string dir, int type, AsyncCallback callback, object state);
        IList<FolderTree> EndFillDirectoryAll(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetFolderList(string dir, AsyncCallback callback, object state);
        IList<String> EndGetFolderList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCopyAll(IList<PCLResultFileStorageDetail> ListDetails, string ImageStore, AsyncCallback callback, object state);
        bool EndCopyAll(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteFile(string path, List<String> ListPath, AsyncCallback callback, object state);
        bool EndDeleteFile(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveImageClipBoard(List<byte[]> buffer, string ImageStore, AsyncCallback callback, object state);
        bool EndSaveImageClipBoard(IAsyncResult asyncResult);
        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSavePCLImagingResultPDF(byte[] FileExport, string FileName, AsyncCallback callback, object state);
        bool EndSavePCLImagingResultPDF(IAsyncResult asyncResult);
    }
}
