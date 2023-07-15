using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using DataEntities;
using eHCMS.Services.Core;
using ErrorLibrary;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-10-06
 * Contents: Consultation Services Iterfaces
/*******************************************************************/
#endregion
/*
 * 20182510 #001 TTM: BM 0002173: Thay đổi cách lưu/ cập nhật/ lấy dữ liệu của tình trạng thể chất từ lấy theo TTTC cuối cùng => theo đăng ký
 */
namespace ConsultationsService.PtDashboard.Summary
{
    [ServiceContract]
    public interface ISummary
    {
        #region 1.Allergies
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<MDAllergy> MDAllergies_ByPatientID(Int64 PatientID, int flag);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void MDAllergies_Save(MDAllergy Obj,out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void MDAllergies_IsDeleted(MDAllergy Obj, out string Result);



        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<MDAllergy> GetAllMDAllergies();

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<MDAllergy> GetMDAllergiesByPtID(long patientID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool DeleteMDAllergy(MDAllergy entity);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool AddMDAllergy(MDAllergy entity);

        //[OperationContract]
        //bool UpdateMDAllergy(MDAllergy entity);

        #endregion

        #region 2.Warnning
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<MDWarning> MDWarnings_ByPatientID(Int64 PatientID, int flag);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void MDWarnings_Save(MDWarning Obj, out long Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void MDWarnings_IsDeleted(MDWarning Obj, out string Result);


        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<MDWarning> GetAllMDWarnings();

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<MDWarning> GetMDWarningsByPtID(long patientID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //string GetStMDWarningsByPtID(long patientID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool DeleteMDWarning(MDWarning entity);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool AddMDWarning(MDWarning entity);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool UpdateMDWarning(MDWarning entity);

        #endregion

        #region 3.PhysicalExamination

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PhysicalExamination> GetPhyExamByPtID(long patientID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PhysicalExamination> GetPhyExamByPtID_InPT(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PhysicalExamination GetLastPhyExamByPtID(long patientID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewPhysicalExamination(PhysicalExamination entity, long? staffID);
        //▼====== #001
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewPhysicalExamination_V2(PhysicalExamination entity, long? staffID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePhysicalExamination_V2(PhysicalExamination entity, long? StaffID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePhysicalExamination_InPT(PhysicalExamination entity, long? StaffID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        PhysicalExamination GetPhyExam_ByPtRegID(long PtRegistrationID, long V_RegistrationType);
        //▲====== #001
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewPhysicalExamination_InPT(PhysicalExamination entity, long? staffID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdatePhysicalExamination(PhysicalExamination entity, long? StaffID, long? PhyExamID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePhysicalExamination(long? StaffID, long? PhyExamID, long? CommonMedRecID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeletePhysicalExamination_InPT(long? PhyExamID, long? CommonMedRecID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Lookup> GetLookupSmokeStatus();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<Lookup> GetLookupAlcoholDrinkingStatus();
        #endregion

        #region 4.Consultation
        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientServiceRecord> GetConsultationByPtID(long patientID, long processTypeID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PatientServiceRecord> GetSumConsulByPtID(long patientID, long processTypeID, int PageIndex, int PageSize, out int Total);
        #endregion
        #region 6.Family History
        
        //Manupulate on FamilyHistory
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool DeleteFamilyHistory(long? StaffID, long? FHCode, long? CommonMedRecID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool AddNewFamilyHistory(FamilyHistory entity, long? staffID);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //bool UpdateFamilyHistory(FamilyHistory entity, long? StaffID
        //                                                    , long? FHCode);
        #endregion
    }
}
