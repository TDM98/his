using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.ServiceModel;
using DataEntities;
//using eHCMS.Services.Core;
using aEMR.DataContracts;

namespace AppointmentServiceProxy
{
    [ServiceContract]
    public interface IAppointmentService
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAppointments(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<PatientAppointment> EndGetAppointments(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeletePatientAppointments(long AppointmentID, AsyncCallback callback, object state);
        bool EndDeletePatientAppointments(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginSearchAppointments(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<PatientAppointment> EndSearchAppointments(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAppointmentsDay(AppointmentSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<PatientAppointment> EndGetAppointmentsDay(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAppointmentsOfPatient(long patientID, int pageIndex, int pageSize, bool bCountTotal, AsyncCallback callback, object state);
        List<PatientAppointment> EndGetAppointmentsOfPatient(out int TotalCount, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertAppointment(PatientAppointment appointment, AsyncCallback callback, object state);
        void EndInsertAppointment(out long AppointmentID, out PatientAppointment AddedAppointment, out ObservableCollection<PatientApptServiceDetails> RequestSeqNoFailedList, out ObservableCollection<PatientApptServiceDetails> InsertFailedList, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateAppointment(PatientAppointment appointment, AsyncCallback callback, object state);
        void EndUpdateAppointment(out PatientAppointment UpdatedAppointment, out ObservableCollection<PatientApptServiceDetails> RequestSeqNoFailedList, out ObservableCollection<PatientApptServiceDetails> InsertFailedList, out ObservableCollection<PatientApptServiceDetails> DeleteFailedList, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllAppointmentSegments(AsyncCallback callback, object state);
        ObservableCollection<PatientApptTimeSegment> EndGetAllAppointmentSegments(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllServicesByAppointmentType(long appointmentType, AsyncCallback callback, object state);
        ObservableCollection<RefMedicalServiceItem> EndGetAllServicesByAppointmentType(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetAllLocationsByService(long serviceID, AsyncCallback callback, object state);
        //List<Location> EndGetAllLocationsByService(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllDeptLocationsByService(long serviceID, AsyncCallback callback, object state);
        ObservableCollection<DeptLocation> EndGetAllDeptLocationsByService(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllDeptLocationsByService_WithSeqNumberSegment(long MedServiceID, DateTime ApptDate, AsyncCallback callback, object state);
        ObservableCollection<DeptLocation> EndGetAllDeptLocationsByService_WithSeqNumberSegment(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSegments_WithAppDateDeptLocIDSeqNumber(long DeptLocationID, DateTime ApptDate, AsyncCallback callback, object state);
        ObservableCollection<ConsultationTimeSegments> EndSegments_WithAppDateDeptLocIDSeqNumber(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAppointmentByID(long appointmentID, long? IssueID, AsyncCallback callback, object state);
        PatientAppointment EndGetAppointmentByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAppointmentOfPatientByDate(long patientID, DateTime ApptDate, AsyncCallback callback, object state);
        PatientAppointment EndGetAppointmentOfPatientByDate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetNumberOfAppointments(DateTime ApptDate, long DeptLocID, short segmentID, AsyncCallback callback, object state);
        int EndGetNumberOfAppointments(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetNumberOfAvailablePosition(DateTime ApptDate, long DeptLocID, short segmentID, AsyncCallback callback, object state);
        void EndGetNumberOfAvailablePosition(out int MaxNumOfAppointments, out int NumOfAppts, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientQueue_Insert(PatientQueue ObjPatientQueue, AsyncCallback callback, object state);
        bool EndPatientQueue_Insert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientQueue_InsertList(IList<PatientQueue> lstPatientQueue, AsyncCallback callback, object state);
        bool EndPatientQueue_InsertList(out IList<string> lstPatientQueueInsertFail, IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginRefMedicalServiceItems_GetDeptIDIsKhamBenhNotPCL(AsyncCallback callback, object state);
        //List<RefDepartments> EndRefMedicalServiceItems_GetDeptIDIsKhamBenhNotPCL(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging(
         RefMedicalServiceItemsSearchCriteria Criteria,

         int PageIndex,
         int PageSize,
         string OrderBy,
         bool CountTotal
        , AsyncCallback callback, object state);
        List<RefMedicalServiceItem> EndRefMedicalServiceItems_GetByDeptIDIsKhamBenhNotPCL_Paging(
         out int Total
         , IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientApptServiceDetailsGetAll(PatientApptServiceDetailsSearchCriteria searchCriteria,
            int PageIndex,
                                     int PageSize,
                                     string OrderBy,
                                     bool CountTotal,
                                     AsyncCallback callback, object state);
        List<PatientApptServiceDetails> EndPatientApptServiceDetailsGetAll(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginApptService_GetByMedServiceID(Int64 MedServiceID, AsyncCallback callback, object state);
        List<Lookup> EndApptService_GetByMedServiceID(Int64 MedServiceID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginApptService_XMLSave(Int64 MedServiceID, IEnumerable<Lookup> ObjList, AsyncCallback callback, object state);
        void EndApptService_XMLSave(out string Result, IAsyncResult asyncResult);


        //Hẹn Bệnh
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientAppointments_Save(PatientAppointment ObjPatientAppointment, bool PassCheckFullTarget, long? IssueID, AsyncCallback callback, object state);
        bool EndPatientAppointments_Save(out long AppointmentID, out string ErrorDetail,
            out string ListNotConfig, out string ListTargetFull, out string ListMax, out string ListRequestID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientApptPCLRequests_UpdateTemplate(PatientApptPCLRequests ObjPatientAppointment, AsyncCallback callback, object state);
        bool EndPatientApptPCLRequests_UpdateTemplate(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAllDeptLocation_LAB(AsyncCallback callback, object state);
        List<DeptLocation> EndAllDeptLocation_LAB(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAllDeptLocation_NotLAB(long V_PCLMainCategory, long PCLExamTypeSubCategoryID, AsyncCallback callback, object state);
        List<DeptLocation> EndAllDeptLocation_NotLAB(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientApptPCLRequestDetails_ByID(long AppointmentID, long PatientPCLReqID, AsyncCallback callback, object state);
        List<PatientApptPCLRequestDetails> EndPatientApptPCLRequestDetails_ByID(IAsyncResult asyncResult);
        //Hẹn Bệnh

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllHoliday(AsyncCallback callback, object state);
        List<Holiday> EndGetAllHoliday(IAsyncResult asyncResult);

        #region "PatientApptLocTargets"
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientApptLocTargetsByDepartmentLocID(long DepartmentLocID, AsyncCallback callback, object state);
        List<PatientApptLocTargets> EndPatientApptLocTargetsByDepartmentLocID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientApptLocTargets_Save(PatientApptLocTargets Obj, AsyncCallback callback, object state);
        void EndPatientApptLocTargets_Save(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPatientApptLocTargets_Delete(long PatientApptTargetID, AsyncCallback callback, object state);
        bool EndPatientApptLocTargets_Delete(IAsyncResult asyncResult);

        #endregion

        #region HealthExaminationRecord
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetHospitalClients(AsyncCallback callback, object state);
        List<HospitalClient> EndGetHospitalClients(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEditHospitalClient(HospitalClient aHospitalClient, AsyncCallback callback, object state);
        HospitalClient EndEditHospitalClient(out bool ExCode, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginEditHospitalClientContract(HospitalClientContract aClientContract, List<HosClientContractPatientGroup> PatientGroup, AsyncCallback callback, object state);
        HospitalClientContract EndEditHospitalClientContract(out bool ExCode, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetHospitalClientContracts(HospitalClientContract aClientContract, AsyncCallback callback, object state);
        ObservableCollection<HospitalClientContract> EndGetHospitalClientContracts(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetHospitalClientContractDetails(HospitalClientContract aClientContract, AsyncCallback callback, object state);
        HospitalClientContract EndGetHospitalClientContractDetails(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginActiveHospitalClientContract(long HosClientContractID, long StaffID, AsyncCallback callback, object state);
        bool EndActiveHospitalClientContract(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetContractPaidAmount(long HosClientContractID, AsyncCallback callback, object state);
        List<HosClientContractPatient> EndGetContractPaidAmount(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginCompleteHospitalClientContract(long HosClientContractID, long StaffID, AsyncCallback callback, object state);
        void EndCompleteHospitalClientContract(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginFinalizeHospitalClientContract(long HosClientContractID, long StaffID, decimal TotalContractAmount, bool IsConfirmFinalized, AsyncCallback callback, object state);
        void EndFinalizeHospitalClientContract(IAsyncResult asyncResult);
        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveConsultationRoomStaffAllocationServiceList(long ConsultationRoomStaffAllocationServiceListID, string ConsultationRoomStaffAllocationServiceListTitle, long[] MedServiceIDCollection, AsyncCallback callback, object state);
        void EndSaveConsultationRoomStaffAllocationServiceList(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetConsultationRoomStaffAllocationServiceLists(AsyncCallback callback, object state);
        List<ConsultationRoomStaffAllocationServiceList> EndGetConsultationRoomStaffAllocationServiceLists(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveHosClientContractPatientGroup(HosClientContractPatientGroup PatientGroup, AsyncCallback callback, object state);
        void EndSaveHosClientContractPatientGroup(out long OutHosClientContractPatientGroupID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetHosClientContractPatientGroups(long HosClientContractID, AsyncCallback callback, object state);
        List<HosClientContractPatientGroup> EndGetHosClientContractPatientGroups(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetContractName_ByHosClientID(long HosClientID, AsyncCallback callback, object state);
        List<HospitalClientContract> EndGetContractName_ByHosClientID(IAsyncResult asyncResult);
    }
}