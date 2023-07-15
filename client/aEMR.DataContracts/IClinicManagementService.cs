/*
20161223 #001 CMN: Add file manager
*/
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using DataEntities;
using System.Collections.ObjectModel;
/*
 * 20220811 #002 QTD: Tìm danh sách Hồ sơ cho màn hình đặt hồ sơ vào kệ
 * 20220814 #003 QTD: 
 *  Quản lý Dãy kệ ngăn
 *  Quản lý hồ sơ
 * 20221116 #004 BLQ: Thêm chức năng lịch khá bệnh ngoài giờ
 */
namespace ClinicManagementProxy
{
    [ServiceContract]
    public interface IClinicManagementService
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertConsultationTimeSegments(string SegmentName, string SegmentDescription, DateTime StartTime,
                                            DateTime EndTime, DateTime? StartTime2 , DateTime? EndTime2, bool IsActive, AsyncCallback callback, object state);
        bool EndInsertConsultationTimeSegments(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginEditStaffConsultationTimeSegments(string SegmentXmlContent, long DeptLocationID, AsyncCallback callback, object state);
        void EndEditStaffConsultationTimeSegments(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetStaffConsultationTimeSegmentByDate(long DeptLocationID, DateTime FromDate, DateTime ToDate, AsyncCallback callback, object state);
        List<ConsultationRoomStaffAllocations> EndGetStaffConsultationTimeSegmentByDate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateConsultationTimeSegments(ObservableCollection<ConsultationTimeSegments> consultationTimeSegments, AsyncCallback callback, object state);
        bool EndUpdateConsultationTimeSegments(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteConsultationTimeSegments(long ConsultationTimeSegmentID, AsyncCallback callback, object state);
        bool EndDeleteConsultationTimeSegments(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllConsultationTimeSegments(AsyncCallback callback, object state);
        List<ConsultationTimeSegments> EndGetAllConsultationTimeSegments(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllPclTimeSegments(AsyncCallback callback, object state);
        List<PCLTimeSegment> EndGetAllPclTimeSegments(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginConsultationTimeSegments_ByDeptLocationID(long DeptLocationID, AsyncCallback callback, object state);
        List<ConsultationTimeSegments> EndConsultationTimeSegments_ByDeptLocationID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertConsultationRoomTarget(ConsultationRoomTarget consultationRoomTarget,
                                                       AsyncCallback callback, object state);
        bool EndInsertConsultationRoomTarget(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertConsultationRoomTargetXML(IList<ConsultationRoomTarget> lstCRSA,
                                                       AsyncCallback callback, object state);
        bool EndInsertConsultationRoomTargetXML(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateConsultationRoomTargetXML(IList<ConsultationRoomTarget> lstCRSA, AsyncCallback callback, object state);
        bool EndUpdateConsultationRoomTargetXML(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteConsultationRoomTarget(long ConsultationRoomTargetID, AsyncCallback callback, object state);
        bool EndDeleteConsultationRoomTarget(IAsyncResult asyncResult);



        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetConsultationRoomTargetByDeptID(long DeptLocationID, AsyncCallback callback, object state);
        List<ConsultationRoomTarget> EndGetConsultationRoomTargetByDeptID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetConsultationRoomTargetTimeSegment(long DeptLocationID, long ConsultationTimeSegmentID, AsyncCallback callback, object state);
        List<ConsultationRoomTarget> EndGetConsultationRoomTargetTimeSegment(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetConsultationRoomTargetTSegment(long DeptLocationID,bool IsHis, AsyncCallback callback, object state);
        List<ConsultationRoomTarget> EndGetConsultationRoomTargetTSegment(out DateTime curDate,IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertConsultationRoomStaffAllocations(long DeptLocationID
                                                  , long ConsultationTimeSegmentID
                                                  , long StaffID
                                                  , long StaffCatgID
                                                  , DateTime AllocationDate
                                                    , AsyncCallback callback, object state);
        bool EndInsertConsultationRoomStaffAllocations( IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA
                                                    , AsyncCallback callback, object state);
        bool EndInsertConsultationRoomStaffAllocationsXML(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateConsultationRoomStaffAllocations(long ConsultationRoomStaffAllocID, bool IsActive
                                                    , AsyncCallback callback, object state);
        bool EndUpdateConsultationRoomStaffAllocations(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA
                                                    , AsyncCallback callback, object state);
        bool EndUpdateConsultationRoomStaffAllocationsXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetConsultationRoomStaffAllocations(long DeptLocationID, long ConsultationTimeSegmentID,AsyncCallback callback, object state);
        List<ConsultationRoomStaffAllocations> EndGetConsultationRoomStaffAllocations(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteConsultationRoomStaffAllocations(long DeptLocationID, long ConsultationTimeSegmentID
                                                                , DateTime AllocationDate, AsyncCallback callback, object state);
        bool EndDeleteConsultationRoomStaffAllocations(IAsyncResult asyncResult);

        //==== #001
        #region FileManager
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefShelves(long RefShelfID, long RefRowID, string RefShelfName, AsyncCallback callback, object state);
        ObservableCollection<RefShelves> EndGetRefShelves(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefShelfDetails(long RefShelfID, string LocCode, long RefShelfDetailID, string LocName, AsyncCallback callback, object state);
        ObservableCollection<RefShelfDetails> EndGetRefShelfDetails(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateRefShelfDetails(long RefShelfID, long StaffID, List<RefShelfDetails> ListRefShelfDetail, List<RefShelfDetails> ListRefShelfDetailDeleted, bool IsPopup, AsyncCallback callback, object state);
        bool EndUpdateRefShelfDetails(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientMedicalFileStorage(long RefShelfDetailID, string FileCodeNumber, AsyncCallback callback, object state);
        ObservableCollection<PatientMedicalFileStorage> EndGetPatientMedicalFileStorage(out long PatientMedicalFileID, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdatePatientMedicalFileStorage(long RefShelfDetailID, List<PatientMedicalFileStorage> ListPatientMedicalFileStorage, long StaffID, AsyncCallback callback, object state);
        ObservableCollection<PatientMedicalFileStorage> EndUpdatePatientMedicalFileStorage(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientMedicalFileStorageCheckInCheckOut(string FileCodeNumber, AsyncCallback callback, object state);
        ObservableCollection<PatientMedicalFileStorageCheckInCheckOut> EndGetPatientMedicalFileStorageCheckInCheckOut(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllStaffPersons(AsyncCallback callback, object state);
        ObservableCollection<StaffPersons> EndGetAllStaffPersons(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdatePatientMedicalFileStorageCheckOut(long StaffID, List<PatientMedicalFileStorageCheckInCheckOut> ListPatientMedicalFileStorageCheckOut, AsyncCallback callback, object state);
        bool EndUpdatePatientMedicalFileStorageCheckOut(out long MedicalFileStorageCheckID, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateRefShelves(long StaffID, List<RefShelves> ListRefShelves, List<RefShelves> ListRefShelvesDeleted, long RefRowID, bool IsPopup, AsyncCallback callback, object state);
        ObservableCollection<RefShelves> EndUpdateRefShelves(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetMedicalFileFromRegistration(long DeptID, long LocationID, DateTime StartDate, DateTime EndDate, bool IsBorrowed, bool IsStored, AsyncCallback callback, object state);
        ObservableCollection<PatientMedicalFileStorageCheckInCheckOut> EndGetMedicalFileFromRegistration(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetXMLPatientMedicalFileStorages(List<PatientMedicalFileStorage> ListPatientMedicalFileStorage, AsyncCallback callback, object state);
        string EndGetXMLPatientMedicalFileStorages(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchMedicalFilesHistory(DateTime? StartDate, DateTime? EndDate, string FileCodeNumber, long? StaffID, long? ReceiveStaffID, int Status, int PageSize, int PageIndex, AsyncCallback callback, object state);
        ObservableCollection<PatientMedicalFileStorageCheckInCheckOut> EndSearchMedicalFilesHistory(out int TotalRow, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRegistrationXMLFromMedicalFileList(List<PatientMedicalFileStorageCheckInCheckOut> ListItem, AsyncCallback callback, object state);
        string EndGetRegistrationXMLFromMedicalFileList(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetMedicalFileStorageCheckOutHistory(DateTime? StartDate, DateTime? EndDate, long? StaffID, long? ReceiveStaffID, int PageSize, int PageIndex, AsyncCallback callback, object state);
        ObservableCollection<PatientMedicalFileStorageCheckInCheckOut> EndGetMedicalFileStorageCheckOutHistory(out int TotalRow, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetMedicalFileDetails(string FileCodeNumber, long? RefShelfDetailID, int PageSize, int PageIndex, AsyncCallback callback, object state);
        ObservableCollection<PatientMedicalFileStorageCheckInCheckOut> EndGetMedicalFileDetails(out int TotalRow, IAsyncResult asyncResult);
        #endregion
        //==== #001

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchRegistrationsForOutMedicalFileManagement(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, int ViewCase, AsyncCallback callback, object state);
        IList<PatientRegistration> EndSearchRegistrationsForOutMedicalFileManagement(IAsyncResult asyncResult);
        //▼====: #002
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchPatientMedicalFileManager(long V_MedicalFileType, string FileCodeNumber, string PatientName, DateTime FromDate, DateTime ToDate, AsyncCallback callback, object state);
        ObservableCollection<PatientMedicalFileStorage> EndSearchPatientMedicalFileManager(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientMedicalFileStorage_InsertXML(long RefShelfDetailID, List<PatientMedicalFileStorage> ListPatientMedicalFileStorage, long StaffID, AsyncCallback callback, object state);
        ObservableCollection<PatientMedicalFileStorage> EndPatientMedicalFileStorage_InsertXML(IAsyncResult asyncResult);
        //▲====: #002
        //▼====: #003
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefRows(long StoreID, string RefRowName, long RefRowID, AsyncCallback callback, object state);
        ObservableCollection<RefRows> EndGetRefRows(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateRefRows(long StaffID, List<RefRows> ListRefRows, List<RefRows> ListRefRowDeleted, long StoreID, AsyncCallback callback, object state);
        ObservableCollection<RefRows> EndUpdateRefRows(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchMedicalFileDetails(long? StoreID, long? RefRowID, long? RefShelfID, long? RefShelfDetailID, long V_MedicalFileType, string FileCodeNumber, string PatientCode, string PatientName,
            long V_MedicalFileStatus, int PageSize, int PageIndex, AsyncCallback callback, object state);
        ObservableCollection<PatientMedicalFileStorageCheckInCheckOut> EndSearchMedicalFileDetails(out int TotalRow, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPatientMedicalFileStorageCheckInCheckOut_V2(long StoreID, long RefRowID, long RefShelfID, long RefShelfDetailID, DateTime FromDate, DateTime ToDate,
            string PatientCode, string PatientName, string FileCodeNumber, bool IsCheckIn, int PageSize, int PageIndex, AsyncCallback callback, object state);
        ObservableCollection<PatientMedicalFileStorageCheckInCheckOut> EndGetPatientMedicalFileStorageCheckInCheckOut_V2(out int TotalRow, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdatePatientMedicalFileStorageCheckOut_V2(long StaffID, long StaffIDCheckOut, long StaffPersonID, int BorrowingDay,
            string Notes, long V_ReasonType, bool IsCheckIn, List<PatientMedicalFileStorageCheckInCheckOut> ListPatientMedicalFileStorageCheckOut, AsyncCallback callback, object state);
        bool EndUpdatePatientMedicalFileStorageCheckOut_V2(out long MedicalFileStorageCheckID, IAsyncResult asyncResult);
        //▲====: #003
        //▼====: #004
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOvertimeWorkingWeekByDate(int Week, DateTime FromDate, DateTime ToDate, AsyncCallback callback, object state);
        OvertimeWorkingWeek EndGetOvertimeWorkingWeekByDate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOvertimeWorkingScheduleByDate(DateTime FromDate, DateTime ToDate, AsyncCallback callback, object state);
        List<OvertimeWorkingSchedule> EndGetOvertimeWorkingScheduleByDate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLocationForOvertimeWorkingWeek(AsyncCallback callback, object state);
        List<DeptLocation> EndGetLocationForOvertimeWorkingWeek(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDoctorForOvertimeWorkingWeek(AsyncCallback callback, object state);
        List<Staff> EndGetDoctorForOvertimeWorkingWeek(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveOvertimeWorkingSchedule(OvertimeWorkingWeek OTWObj, OvertimeWorkingSchedule OTSObj, long StaffID, DateTime DateUpdate, AsyncCallback callback, object state);
        bool EndSaveOvertimeWorkingSchedule(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteOvertimeWorkingSchedule(long OvertimeWorkingScheduleID, long StaffID, DateTime DateUpdate, AsyncCallback callback, object state);
        bool EndDeleteOvertimeWorkingSchedule(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateOvertimeWorkingWeekStatus(OvertimeWorkingWeek OTWObj, long StaffID, DateTime DateUpdate, AsyncCallback callback, object state);
        bool EndUpdateOvertimeWorkingWeekStatus(IAsyncResult asyncResult);
        //▲====: #004
    }
}
