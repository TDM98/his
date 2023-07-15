/*
 * 20161223 #001 CMN: Add file manager
 * 20220812 #002 QTD: Add SearchPatientMedicalFileManager
 * 20221116 #003 BLQ: Thêm chức năng lịch làm việc ngoài giờ
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using DataEntities;
using ErrorLibrary;

namespace ClinicManagementService
{
    [ServiceContract]
    public interface IClinicManagementService
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertConsultationTimeSegments(string SegmentName
                                                      , string SegmentDescription
                                                      , DateTime StartTime
                                                      , DateTime EndTime
                                                      , DateTime? StartTime2
                                                      , DateTime? EndTime2
                                                      , bool IsActive);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void EditStaffConsultationTimeSegments(string SegmentXmlContent, long DeptLocationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateConsultationTimeSegments(ObservableCollection<ConsultationTimeSegments> consultationTimeSegments);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteConsultationTimeSegments(long ConsultationTimeSegmentID);

        [OperationContract]
        [FaultContract(typeof (AxException))]
        List<ConsultationTimeSegments> GetAllConsultationTimeSegments();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLTimeSegment> GetAllPclTimeSegments();
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ConsultationTimeSegments> ConsultationTimeSegments_ByDeptLocationID(long DeptLocationID);



        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertConsultationRoomTarget(ConsultationRoomTarget consultationRoomTarget);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertConsultationRoomTargetXML(List<ConsultationRoomTarget> lstCRSA);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateConsultationRoomTargetXML(IList<ConsultationRoomTarget> lstCRSA);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteConsultationRoomTarget(long ConsultationRoomTargetID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ConsultationRoomTarget> GetConsultationRoomTargetByDeptID(long DeptLocationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ConsultationRoomTarget> GetConsultationRoomTargetTimeSegment(long DeptLocationID, long ConsultationTimeSegmentID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ConsultationRoomTarget> GetConsultationRoomTargetTSegment(out DateTime curDate,long DeptLocationID, bool IsHis);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertConsultationRoomStaffAllocations(long DeptLocationID
                                                  , long ConsultationTimeSegmentID
                                                  , long StaffID
                                                  , long StaffCatgID
                                                  , DateTime AllocationDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateConsultationRoomStaffAllocations(long ConsultationRoomStaffAllocID, bool IsActive);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ConsultationRoomStaffAllocations> GetConsultationRoomStaffAllocations(long DeptLocationID
                                                                                    ,long ConsultationTimeSegmentID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ConsultationRoomStaffAllocations> GetStaffConsultationTimeSegmentByDate(long DeptLocationID, DateTime FromDate, DateTime ToDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteConsultationRoomStaffAllocations(long DeptLocationID , long ConsultationTimeSegmentID , DateTime AllocationDate );

        //==== #001
        #region FileManager
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefShelves> GetRefShelves(long RefShelfID, long RefRowID, string RefShelfName);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefShelfDetails> GetRefShelfDetails(long RefShelfID, string LocCode, long RefShelfDetailID, string LocName);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateRefShelfDetails(long RefShelfID, long StaffID, List<RefShelfDetails> ListRefShelfDetail, List<RefShelfDetails> ListRefShelfDetailDeleted, bool IsPopup);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientMedicalFileStorage> GetPatientMedicalFileStorage(long RefShelfDetailID, string FileCodeNumber, out long PatientMedicalFileID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientMedicalFileStorage> UpdatePatientMedicalFileStorage(long RefShelfDetailID, List<PatientMedicalFileStorage> ListPatientMedicalFileStorage, long StaffID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientMedicalFileStorageCheckInCheckOut> GetPatientMedicalFileStorageCheckInCheckOut(string FileCodeNumber);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<StaffPersons> GetAllStaffPersons();
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePatientMedicalFileStorageCheckOut(long StaffID, List<PatientMedicalFileStorageCheckInCheckOut> ListPatientMedicalFileStorageCheckOut, out long MedicalFileStorageCheckID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefShelves> UpdateRefShelves(long StaffID, List<RefShelves> ListRefShelves, List<RefShelves> ListRefShelvesDeleted, long RefRowID, bool IsPopup);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileFromRegistration(long DeptID, long LocationID, DateTime StartDate, DateTime EndDate, bool IsBorrowed, bool IsStored);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        string GetXMLPatientMedicalFileStorages(List<PatientMedicalFileStorage> ListPatientMedicalFileStorage);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientMedicalFileStorageCheckInCheckOut> SearchMedicalFilesHistory(DateTime? StartDate, DateTime? EndDate, string FileCodeNumber, long? StaffID, long? ReceiveStaffID, int Status, int PageSize, int PageIndex, out int TotalRow);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        string GetRegistrationXMLFromMedicalFileList(List<PatientMedicalFileStorageCheckInCheckOut> ListItem);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileStorageCheckOutHistory(DateTime? StartDate, DateTime? EndDate, long? StaffID, long? ReceiveStaffID, int PageSize, int PageIndex, out int TotalRow);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileDetails(string FileCodeNumber, long? RefShelfDetailID, int PageSize, int PageIndex, out int TotalRow);
        #endregion
        //==== #001

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientRegistration> SearchRegistrationsForOutMedicalFileManagement(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, int ViewCase);
        //▼==== #002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientMedicalFileStorage> SearchPatientMedicalFileManager(long V_MedicalFileType, string FileCodeNumber, string PatientName, DateTime FromDate, DateTime ToDate);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientMedicalFileStorage> PatientMedicalFileStorage_InsertXML(long RefShelfDetailID, List<PatientMedicalFileStorage> ListPatientMedicalFileStorage, long StaffID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefRows> UpdateRefRows(long StaffID, List<RefRows> ListRefRows, List<RefRows> ListRefRowDeleted, long StoreID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefRows> GetRefRows(long StoreID, string RefRowName, long RefRowID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientMedicalFileStorageCheckInCheckOut> SearchMedicalFileDetails(long? StoreID, long? RefRowID, long? RefShelfID, long? RefShelfDetailID, long V_MedicalFileType, string FileCodeNumber, 
            string PatientCode, string PatientName, long V_MedicalFileStatus, int PageSize, int PageIndex, out int TotalRow);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientMedicalFileStorageCheckInCheckOut> GetPatientMedicalFileStorageCheckInCheckOut_V2(long StoreID, long RefRowID, long RefShelfID, long RefShelfDetailID, DateTime FromDate, DateTime ToDate,
            string PatientCode, string PatientName, string FileCodeNumber, bool IsCheckIn, int PageSize, int PageIndex, out int TotalRow);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePatientMedicalFileStorageCheckOut_V2(long StaffID, long StaffIDCheckOut, long StaffPersonID, int BorrowingDay, string Notes, long V_ReasonType, bool IsCheckIn,
            List<PatientMedicalFileStorageCheckInCheckOut> ListPatientMedicalFileStorageCheckOut, out long MedicalFileStorageCheckID);
        //▲==== #002
        //▼==== #003
        [OperationContract]
        [FaultContract(typeof(AxException))]
        OvertimeWorkingWeek GetOvertimeWorkingWeekByDate(int Week, DateTime FromDate, DateTime ToDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OvertimeWorkingSchedule> GetOvertimeWorkingScheduleByDate(DateTime FromDate, DateTime ToDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetLocationForOvertimeWorkingWeek();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Staff> GetDoctorForOvertimeWorkingWeek();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveOvertimeWorkingSchedule(OvertimeWorkingWeek OTWObj, OvertimeWorkingSchedule OTSObj, long StaffID, DateTime DateUpdate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteOvertimeWorkingSchedule(long OvertimeWorkingScheduleID, long StaffID, DateTime DateUpdate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateOvertimeWorkingWeekStatus(OvertimeWorkingWeek OTWObj, long StaffID, DateTime DateUpdate);
        //▲==== #003
    }
}
