using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataEntities;
using eHCMS.Services.Core;
using eHCMS.Services.Core.Base;
using ErrorLibrary;
using System.Data.Common;
namespace AppointmentService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IAppointmentService
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientAppointment> GetAppointments(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePatientAppointments(long AppointmentID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientAppointment> GetAppointmentsDay(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientAppointment> GetAppointmentsOfPatient(long patientID, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void InsertAppointment(PatientAppointment appointment, out long AppointmentID, out PatientAppointment AddedAppointment, out List<PatientApptServiceDetails> RequestSeqNoFailedList, out List<PatientApptServiceDetails> InsertFailedList);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void UpdateAppointment(PatientAppointment appointment, out PatientAppointment UpdatedAppointment, out List<PatientApptServiceDetails> RequestSeqNoFailedList, out List<PatientApptServiceDetails> InsertFailedList, out List<PatientApptServiceDetails> DeleteFailedList);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientApptTimeSegment> GetAllAppointmentSegments();


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedicalServiceItem> GetAllServicesByAppointmentType(long appointmentType);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<Location> GetAllLocationsByService(long serviceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetAllDeptLocationsByService(long serviceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetAllDeptLocationsByService_WithSeqNumberSegment(long MedServiceID, DateTime ApptDate);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ConsultationTimeSegments> Segments_WithAppDateDeptLocIDSeqNumber(long DeptLocationID, DateTime ApptDate);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientAppointment GetAppointmentByID(long appointmentID, long? IssueID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientAppointment GetAppointmentOfPatientByDate(long patientID, DateTime ApptDate);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int GetNumberOfAppointments(DateTime ApptDate, long DeptLocID, short segmentID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void GetNumberOfAvailablePosition(DateTime ApptDate, long DeptLocID, short segmentID, out int MaxNumOfAppointments, out int NumOfAppts);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool PatientQueue_Insert(PatientQueue ObjPatientQueue);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool PatientQueue_InsertList(List<PatientQueue> lstPatientQueue,out List<string> lstPatientQueueInsertFail);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> AllDeptLocation_LAB();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> AllDeptLocation_NotLAB(long V_PCLMainCategory, long PCLExamTypeSubCategoryID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientApptPCLRequestDetails> PatientApptPCLRequestDetails_ByID(long AppointmentID, long PatientPCLReqID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientAppointment> SearchAppointments(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int TotalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientAppointments_Save(PatientAppointment ObjPatientAppointment
            , bool PassCheckFullTarget
            , long? IssueID
            , out long AppointmentID
            , out string ErrorDetail
            , out string ListNotConfig
            , out string ListTargetFull
            , out string ListMax
            , out string ListRequestID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientApptPCLRequests_UpdateTemplate(PatientApptPCLRequests ObjPatientAppointment);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Holiday> GetAllHoliday();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientApptServiceDetails> PatientApptServiceDetailsGetAll(PatientApptServiceDetailsSearchCriteria searchCriteria,
                                    int PageIndex,
                                     int PageSize,
                                     string OrderBy,
                                     bool CountTotal,
                                     out int Total);
        #region ApptService

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<RefDepartments> RefMedicalServiceItems_GetDeptIDIsKhamBenhNotPCL();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedicalServiceItem> RefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging(
         RefMedicalServiceItemsSearchCriteria Criteria,

         int PageIndex,
         int PageSize,
         string OrderBy,
         bool CountTotal,
         out int Total
         );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Lookup> ApptService_GetByMedServiceID(Int64 MedServiceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void ApptService_XMLSave(Int64 MedServiceID, IEnumerable<Lookup> ObjList, out string Result);

        #endregion


        #region "PatientApptLocTargets"
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PatientApptLocTargets> PatientApptLocTargetsByDepartmentLocID(long DepartmentLocID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PatientApptLocTargets_Save(PatientApptLocTargets Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PatientApptLocTargets_Delete(long PatientApptTargetID);

        #endregion

        #region HealthExaminationRecord
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<HospitalClient> GetHospitalClients();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        HospitalClient EditHospitalClient(HospitalClient aHospitalClient, out bool ExCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        HospitalClientContract EditHospitalClientContract(HospitalClientContract aClientContract, List<HosClientContractPatientGroup> PatientGroup, out bool ExCode);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<HospitalClientContract> GetHospitalClientContracts(HospitalClientContract aClientContract);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        HospitalClientContract GetHospitalClientContractDetails(HospitalClientContract aClientContract);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ActiveHospitalClientContract(long HosClientContractID, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<HosClientContractPatient> GetContractPaidAmount(long HosClientContractID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void CompleteHospitalClientContract(long HosClientContractID, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void FinalizeHospitalClientContract(long HosClientContractID, long StaffID, decimal TotalContractAmount, bool IsConfirmFinalized);
        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void SaveConsultationRoomStaffAllocationServiceList(long ConsultationRoomStaffAllocationServiceListID, string ConsultationRoomStaffAllocationServiceListTitle, long[] MedServiceIDCollection);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ConsultationRoomStaffAllocationServiceList> GetConsultationRoomStaffAllocationServiceLists();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void SaveHosClientContractPatientGroup(HosClientContractPatientGroup PatientGroup, out long OutHosClientContractPatientGroupID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<HosClientContractPatientGroup> GetHosClientContractPatientGroups(long HosClientContractID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<HospitalClientContract> GetContractName_ByHosClientID(long HosClientID);

        #region API For Medpro
        [OperationContract]
        [FaultContract(typeof(AxException))]
        PatientAppointment MedPro_PatientAppointments_Save(PatientAppointment ObjPatientAppointment);
        #endregion
    }
}