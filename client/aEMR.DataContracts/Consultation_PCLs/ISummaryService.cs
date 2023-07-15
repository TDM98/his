using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;
using System.Collections.ObjectModel;
/*
 * 20181023 #001 TTM: BM 0002173: Thay đổi cách lưu, cập nhật và lấy lên của tình trạng thể chất => tất cả đều dựa vào lần đăng ký.
 */
namespace SummaryService
{
    
    [ServiceContract]
    public interface ISummary
    {
        #region 1.Allergies
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMDAllergies_ByPatientID(Int64 PatientID, int flag, AsyncCallback callback, object state);
        List<MDAllergy> EndMDAllergies_ByPatientID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMDAllergies_Save(MDAllergy Obj, AsyncCallback callback, object state);
        void EndMDAllergies_Save(out string Result,IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMDAllergies_IsDeleted(MDAllergy Obj, AsyncCallback callback, object state);
        void EndMDAllergies_IsDeleted(out string Result, IAsyncResult asyncResult);
        #endregion

        #region 2.Warnning

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMDWarnings_ByPatientID(Int64 PatientID, int flag, AsyncCallback callback, object state);
        List<MDWarning> EndMDWarnings_ByPatientID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMDWarnings_Save(MDWarning Obj, AsyncCallback callback, object state);
        void EndMDWarnings_Save(out long Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMDWarnings_IsDeleted(MDWarning Obj, AsyncCallback callback, object state);
        void EndMDWarnings_IsDeleted(out string Result, IAsyncResult asyncResult);
        #endregion

        #region 3.PhysicalExamination
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPhyExamByPtID(long patientID, AsyncCallback callback, object state);
        IList<PhysicalExamination> EndGetPhyExamByPtID(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPhyExamByPtID_InPT(long PtRegistrationID, AsyncCallback callback, object state);
        IList<PhysicalExamination> EndGetPhyExamByPtID_InPT(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLastPhyExamByPtID(long patientID, AsyncCallback callback, object state);
        PhysicalExamination EndGetLastPhyExamByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewPhysicalExamination(PhysicalExamination entity, long? staffID, AsyncCallback callback, object state);
        bool EndAddNewPhysicalExamination(IAsyncResult asyncResult);

        //▼====== #001
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewPhysicalExamination_V2(PhysicalExamination entity, long? staffID, AsyncCallback callback, object state);
        bool EndAddNewPhysicalExamination_V2(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdatePhysicalExamination_V2(PhysicalExamination entity, long? StaffID, AsyncCallback callback, object state);
        bool EndUpdatePhysicalExamination_V2(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdatePhysicalExamination_InPT(PhysicalExamination entity, long? StaffID, AsyncCallback callback, object state);
        bool EndUpdatePhysicalExamination_InPT(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPhyExam_ByPtRegID(long PtRegistrationID, long V_RegistrationType, AsyncCallback callback, object state);
        PhysicalExamination EndGetPhyExam_ByPtRegID(IAsyncResult asyncResult);
        //▲====== #001
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewPhysicalExamination_InPT(PhysicalExamination entity, long? staffID, AsyncCallback callback, object state);
        bool EndAddNewPhysicalExamination_InPT(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdatePhysicalExamination(PhysicalExamination entity, long? StaffID, long? PhyExamID, AsyncCallback callback, object state);
        bool EndUpdatePhysicalExamination(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeletePhysicalExamination(long? StaffID, long? PhyExamID, long? CommonMedRecID, AsyncCallback callback, object state);
        bool EndDeletePhysicalExamination(IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeletePhysicalExamination_InPT(long? PhyExamID, long? CommonMedRecID, AsyncCallback callback, object state);
        bool EndDeletePhysicalExamination_InPT(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupSmokeStatus( AsyncCallback callback, object state);
        IList<Lookup> EndGetLookupSmokeStatus(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetLookupAlcoholDrinkingStatus(AsyncCallback callback, object state);
        IList<Lookup> EndGetLookupAlcoholDrinkingStatus(IAsyncResult asyncResult);    
        #endregion

        #region 4.Consultation
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetConsultationByPtID(long patientID, long processTypeID,AsyncCallback callback, object state);
        IList<PatientServiceRecord> EndGetConsultationByPtID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSumConsulByPtID(long patientID, long processTypeID,int PageIndex,int PageSize, AsyncCallback callback, object state);
        IList<PatientServiceRecord> EndGetSumConsulByPtID(out int Total,IAsyncResult asyncResult);       

        #endregion
    
    }
}
