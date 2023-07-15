using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using System.Reflection;
using eHCMS.Configurations;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2010-10-13
 * Contents: Consultation Services
/*******************************************************************/
#endregion
/*
 * 20171002 #001 CMN: Added GetAllSugeriesByPtRegistrationID
*/
namespace eHCMS.DAL
{
    public abstract class CommonUtilsProvider: DataProviderBase
    {
        static private CommonUtilsProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public CommonUtilsProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                        tempPath = AppDomain.CurrentDomain.BaseDirectory;
                    else
                        tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Consultations.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type t = assem.GetType(Globals.Settings.Consultations.CommonUtils.ProviderType);
                    _instance = (CommonUtilsProvider)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }

        public CommonUtilsProvider()
        {
            this.ConnectionString = Globals.Settings.Consultations.ConnectionString;
        }

        #region 1.Drugs
        public abstract List<RefGenericDrugDetail> SearchRefDrugNames(string brandName, long pageIndex, long pageSize, byte type);
        #endregion

        #region 2.DiseasesReference
        public abstract List<DiseasesReference> SearchRefDiseases(string searchKey, long pageIndex, long pageSize, byte type,out int Total);
      
        #endregion

        #region RefICD9
        public abstract List<RefICD9> SearchRefICD9(string searchKey, long pageIndex, long pageSize, byte ICD9SearchType, out int Total);

        #endregion

        #region 3.Staff
        public abstract List<Staff> SearchStaffFullName(string searchKey, long pageIndex, long pageSize);

        public abstract List<Staff> SearchStaffCat(Staff Staff, long pageIndex, long pageSize);

        public abstract List<Staff> GetAllStaffs();

        public abstract List<Staff> GetAllStaffs_FromStaffID(long nFromStaffID);

        #endregion

        #region 4.Departments

        public abstract RefDepartment GetRefDepartmentByLID(long locationID);
    
        #endregion

        #region 5.Lookup

        #endregion

        #region 7.PhysicalExamination

        public abstract void PhysicalExamination_Insert(PhysicalExamination p, long? staffID);
        public abstract void PhysicalExamination_Delete(long CommonMedRecID);
        public abstract void PhysicalExamination_Update(long CommonMedRecID
                                                            , DateTime RecordDate
                                                            , float Height
                                                            , float Weight
                                                            , float SystolicPressure
                                                            , float DiastolicPressure
                                                            , float Pulse
                                                            , float Cholesterol
                                                            , int Smoke_EveryDay
                                                            , int Smoke_OnOccasion
                                                            , int Smoke_Never
                                                            , int Alcohol_CurrentHeavy
                                                            , int Alcohol_HeavyInThePast
                                                            , int Alcohol_CurrentLight
                                                            , int Alcohol_Never
                                                            , float CVRisk);
        public abstract PhysicalExamination PhysicalExamination_GetData(long CommonMedRecID);
        public abstract List<PhysicalExamination> PhysicalExamination_ListData(long CommonMedRecID);
        
#endregion

        #region PatientServiceRecord

        public abstract List<PatientServiceRecord> PatientServiceRecordsGetForKhamBenh(PatientServiceRecord entity);

        public abstract List<PatientServiceRecord> PatientServiceRecordsGetForKhamBenh_InPt(PatientServiceRecord entity);

        #endregion

        #region ConsultingDiagnosys
        public abstract bool UpdateConsultingDiagnosys(ConsultingDiagnosys aConsultingDiagnosys, out long ConsultingDiagnosysID);
        public abstract ConsultingDiagnosys GetConsultingDiagnosys(ConsultingDiagnosys aConsultingDiagnosys);
        public abstract List<ConsultingDiagnosys> GetReportConsultingDiagnosys(ConsultingDiagnosysSearchCriteria aSearchCriteria, out int TotalItemCount);
        public abstract List<SurgerySchedule> GetSurgerySchedules();
        public abstract List<SurgeryScheduleDetail> GetSurgeryScheduleDetails(long ConsultingDiagnosysID);
        public abstract List<SurgeryScheduleDetail_TeamMember> GetSurgeryScheduleDetail_TeamMembers(long ConsultingDiagnosysID);
        public abstract bool EditSurgerySchedule(SurgerySchedule aEditSurgerySchedule, out long SurgeryScheduleID);
        /*▼====: #001*/
        public abstract List<RefMedicalServiceItem> GetAllSugeriesByPtRegistrationID(long PtRegistrationID);
        public abstract bool SaveCatastropheByPtRegDetailID(long PtRegDetailID, long V_CatastropheType);
        public abstract void GetFirstExamDate(long PatientID, out DateTime? MinExamDate);
        public abstract void GetNextAppointment(long PatientID, long MedServiceID, DateTime CurentDate, out DateTime? ApptDate);
        /*▲====: #001*/
        #endregion
        public abstract bool EditSmallProcedure(SmallProcedure aSmallProcedure, long StaffID, out long SmallProcedureID);
        public abstract SmallProcedure GetSmallProcedure(long PtRegDetailID, long? V_RegistrationType = null);
        public abstract bool ChangeStatus(long PtRegDetailID, long StaffChangeStatus, string ReasonChangeStatus);
    }
}
