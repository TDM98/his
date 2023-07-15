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
 * 20182510 #001 TTM: BM 0002173: Thay đổi cách lưu/ cập nhật/ lấy dữ liệu của tình trạng thể chất từ lấy theo TTTC cuối cùng => theo đăng ký
 */
namespace eHCMS.DAL
{
    public abstract class SummaryProvider: DataProviderBase
    {
        static private SummaryProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public SummaryProvider Instance
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
                    Type t = assem.GetType(Globals.Settings.Consultations.Summary.ProviderType);
                    _instance = (SummaryProvider)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }

        public SummaryProvider()
        {
            this.ConnectionString = Globals.Settings.Consultations.ConnectionString;
        }

        #region 0.Demographics And HealthInsurance

        #endregion

        #region 1.Allergies
        public abstract List<MDAllergy> MDAllergies_ByPatientID(Int64 PatientID, int flag);
        public abstract void MDAllergies_Save(MDAllergy Obj, out string Result);
        public abstract void MDAllergies_IsDeleted(MDAllergy Obj, out string Result);

        //public abstract List<MDAllergy> GetAllMDAllergies();
        //public abstract List<MDAllergy> GetMDAllergiesByPtID(long patientID);
        //public abstract bool DeleteMDAllergy(MDAllergy entity);
        //public abstract bool AddMDAllergy(MDAllergy entity);
        //public abstract bool UpdateMDAllergy(MDAllergy entity);

        #endregion

        #region 2.Warnning
        public abstract List<MDWarning> MDWarnings_ByPatientID(Int64 PatientID, int flag);
        public abstract void MDWarnings_Save(MDWarning Obj, out long Result);
        public abstract void MDWarnings_IsDeleted(MDWarning Obj, out string Result);


        //public abstract List<MDWarning> GetAllMDWarnings();
        //public abstract List<MDWarning> GetMDWarningsByPtID(long patientID);
        //public abstract string GetStMDWarningsByPtID(long patientID);
        //public abstract bool DeleteMDWarning(MDWarning entity);
        //public abstract bool AddMDWarning(MDWarning entity);
        //public abstract bool UpdateMDWarning(MDWarning entity);

        #endregion

        #region 3.PhysicalExamination

        public abstract List<PhysicalExamination> GetPhyExamByPtID(long patientID);
        public abstract PhysicalExamination GetLastPhyExamByPtID(long patientID);

        //▼====== #001
        public abstract PhysicalExamination GetPhyExam_ByPtRegID(long PtRegistrationID, long V_RegistrationType);
        //▲====== #001

        #endregion

        #region 4.Consultation
        public abstract List<PatientServiceRecord> GetConsultationByPtID(long patientID, long processTypeID);
        public abstract List<PatientServiceRecord> GetSumConsulByPtID(long patientID, long processTypeID, int PageIndex, int PageSize, out int Total);
        #endregion

    }
}
