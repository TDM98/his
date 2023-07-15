/*
 * 20170608 #001 CMN: Added PatientType to Update PCL
 * 20180613 #002 TBLD: Get HIRepResourceCode for GetResourcesForMedicalServicesListByTypeID
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using Service.Core.Common;
using eHCMS.Configurations;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2011-01-06
 * Contents: Consultation Services
/*******************************************************************/
#endregion

namespace eHCMS.DAL
{
    public abstract class PCLsProvider : DataProviderBase
    {
        static private PCLsProvider _instance = null;
        static public PCLsProvider Instance
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
                    Type t = assem.GetType(Globals.Settings.Consultations.PCLs.ProviderType);
                    _instance = (PCLsProvider)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }
        public PCLsProvider()
        {
            this.ConnectionString = Globals.Settings.Consultations.ConnectionString;
        }       

        #region 0. ExamTypes Member
        //public abstract List<PCLExamType> PCLExamTypes_ByPCLFormID(Int64 PCLFormID);
        //public abstract IList<PCLExamType> GetPCLExamTypes_ByPCLFormID_Edit(long PCLFormID, long PatientPCLReqID, int? ClassPatient);
        #endregion

        #region 1. PCLForm
        public abstract IList<PCLGroup> GetAllPCLGroups(long? pclCategoryID);
        public abstract IList<PCLExamType> GetAllActiveExamTypes(PCLExamTypeSearchCriteria searchCriteria, int pageIndex, int pageSize, bool bCountTotal, string orderBy, out int totalCount);
        public abstract IList<PCLExamType> GetAllActiveExamTypes(PCLExamTypeSearchCriteria searchCriteria);
        #endregion

        #region 2. PatientPCLRequest
        public abstract PatientMedicalRecord GetPMRByPtPCLFormRequest(long LoggedDoctorID, long PatientID, long? PtRegistrationID,long? PatientRecID);
        public abstract bool AddFullPtPCLReq(PatientPCLRequest entity, long PatientID, long? PtRegistrationID, out long newPatientPCLReqID);
        //public abstract bool AddFullPtPCLReqServiceRecID(PatientPCLRequest entity, out long newPatientPCLReqID);
        //public abstract bool UpdateFullPtPCLReqServiceRecID(PatientPCLRequest entity);

        
        public abstract List<PatientPCLRequest> PatientPCLRequest_ByPatientID_Paging(
            PatientPCLRequestSearchCriteria SearchCriteria, long V_RegistrationType,
            
             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total);

        public abstract List<PatientPCLRequest> GetPatientPCLRequestList_ByRegistrationID(long RegistrationID, long V_RegistrationType, long V_PCLMainCategory);

        public abstract IList<PatientPCLRequestDetail> PatientPCLRequestDetail_ByPatientPCLReqID(PatientPCLRequestDetailSearchCriteria SearchCriteria, long V_RegistrationType);
        public abstract IList<PatientPCLRequestDetail> PatientPCLRequestDetail_ByPtRegistrationID(long PtRegistrationID, long V_RegistrationType, long V_PCLMainCategory);

        //Phiếu cuối cùng
        public abstract PatientPCLRequest PatientPCLRequest_RequestLastest(Int64 PatientID, long V_RegistrationType, long? V_PCLMainCategory);
        //Phiếu cuối cùng


        //ds Phieu cuoi cung trong ngay chua tra tien
        public abstract IList<PatientPCLRequest> PatientPCLReq_RequestLastestInDayNotPaid(long PatientID, long V_PCLRequestType, long ReqFromDeptLocID, long StaffID);
        //ds Phieu cuoi cung trong ngay chua tra tien


        //Danh sách phiếu yêu cầu CLS
        public abstract IList<PatientPCLRequest> PatientPCLRequest_SearchPaging(
          PatientPCLRequestSearchCriteria SearchCriteria,

        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        out int Total);
        //Danh sách phiếu yêu cầu CLS

        public abstract PatientPCLRequest GetPatientPCLRequestResultsByReqID(PatientPCLRequestSearchCriteria SearchCriteria);

        public abstract IList<PatientPCLRequest> PatientPCLRequest_ViewResult_SearchPaging(
                      PatientPCLRequestSearchCriteria SearchCriteria,
       int PageIndex,
       int PageSize,
       string OrderBy,
       bool CountTotal,
      out int Total);
        /// <summary>
        /// union them ngoai vien
        /// </summary>
        public abstract IList<PatientPCLRequest> PatientPCLRequest_ViewResult_SearchPagingNew(
                      PatientPCLRequestSearchCriteria SearchCriteria,
       int PageIndex,
       int PageSize,
       string OrderBy,
       bool CountTotal,
      out int Total);

        //Danh sách phiếu các lần trước
        public abstract IList<PatientPCLRequest> PatientPCLRequest_ByPatientIDV_Param_Paging(
            PatientPCLRequestSearchCriteria SearchCriteria,

        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        out int Total);
        //Danh sách phiếu các lần trước

        #endregion

        #region 3. PCL result

        //public abstract IList<PatientPCLRequest> GetPCLResult_PtPCLRequest(long? registrationID, bool isImported, long V_PCLCategory);

        public abstract IList<PCLExamGroup> GetPCLResult_PCLExamGroup(long? ptID, long? ptPCLReqID, bool isImported);
  
        //public abstract IList<PCLExamType> GetPCLResult_PCLExamType(long? ptID, long? PCLExamGroupID, bool isImported);

        /*▼====: #001*/
        //public abstract IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetails(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID);
        public abstract IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetails(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID, long V_PCLRequestType = (long)AllLookupValues.V_PCLRequestType.NGOAI_TRU);
        /*▲====: #001*/

        public abstract IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetailsExt(PCLResultFileStorageDetailSearchCriteria p);

        public abstract IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetailsByPCLImgResultID(long PCLImgResultID);
    
        #endregion

        #region 4. PCL Laboratory

        protected virtual PatientPCLLaboratoryResultDetail GetPatientPCLLaboratoryResultDetailFromReader(IDataReader reader)
        {
            PatientPCLLaboratoryResultDetail p = new PatientPCLLaboratoryResultDetail();

            if (reader.HasColumn("PCLExamTestItemID") && reader["PCLExamTestItemID"] != DBNull.Value)
            {
                p.PCLExamTestItemID = (long)reader["PCLExamTestItemID"];
            }

            if (reader.HasColumn("PCLExamTypeTestItemID") && reader["PCLExamTypeTestItemID"] != DBNull.Value)
            {
                p.PCLExamTypeTestItemID = (long)reader["PCLExamTypeTestItemID"];
            }

            if (reader.HasColumn("LabResultDetailID") && reader["LabResultDetailID"] != DBNull.Value)
            {
                p.LabResultDetailID = (long)reader["LabResultDetailID"];
            }
            

            p.V_PCLExamTypeTestItems = new PCLExamTypeTestItems();
            p.V_PCLExamTypeTestItems.V_PCLExamTestItem = new PCLExamTestItems();
            p.V_PCLExamTypeTestItems.V_PCLExamType = new PCLExamType();
            p.V_PCLExamTypeTestItems.V_PCLExamType.ObjV_PCLExamTypeUnit = new Lookup();

            if (reader.HasColumn("LabResultDetailID") && reader["LabResultDetailID"] != DBNull.Value)
            {
                p.LabResultDetailID = (long)reader["LabResultDetailID"];
            }

            if (reader.HasColumn("LabResultID") && reader["LabResultID"] != DBNull.Value)
            {
                p.LabResultID = (long)reader["LabResultID"];
            }

            if (reader.HasColumn("Value") && reader["Value"] != DBNull.Value)
            {
                p.Value = reader["Value"].ToString();
            }

            if (reader.HasColumn("IsAbnormal") && reader["IsAbnormal"] != DBNull.Value)
            {
                p.IsAbnormal = (bool)reader["IsAbnormal"];
            }

            if (reader.HasColumn("Comments") && reader["Comments"] != DBNull.Value)
            {
                p.Comments = reader["Comments"].ToString();
            }

            if (reader.HasColumn("PCLExamTestItemName") && reader["PCLExamTestItemName"] != DBNull.Value)
            {
                p.V_PCLExamTypeTestItems.V_PCLExamTestItem.PCLExamTestItemName = reader["PCLExamTestItemName"].ToString();
                p.PCLExamTestItemName = reader["PCLExamTestItemName"].ToString();
            }
            
            if (reader.HasColumn("PCLExamTestItemUnit") && reader["PCLExamTestItemUnit"] != DBNull.Value)
            {
                p.V_PCLExamTypeTestItems.V_PCLExamTestItem.PCLExamTestItemUnit= reader["PCLExamTestItemUnit"].ToString();
                p.PCLExamTestItemUnit= reader["PCLExamTestItemUnit"].ToString();
            }

            if (reader.HasColumn("PCLExamTestItemRefScale") && reader["PCLExamTestItemRefScale"] != DBNull.Value)
            {
                p.V_PCLExamTypeTestItems.V_PCLExamTestItem.PCLExamTestItemRefScale = reader["PCLExamTestItemRefScale"].ToString();
                p.PCLExamTestItemRefScale = reader["PCLExamTestItemRefScale"].ToString();
            }

            if (reader.HasColumn("IsBold") && reader["IsBold"] != DBNull.Value)
            {
                p.IsBold = (bool)reader["IsBold"];
                p.V_PCLExamTypeTestItems.V_PCLExamTestItem.IsBold = (bool)reader["IsBold"];
            }
            
            if (reader.HasColumn("IsNoNeedResult"))
            {
                if (reader["IsNoNeedResult"] == DBNull.Value)
                {
                    p.IsNoNeedResult = false;
                }
                else
                {
                    p.IsNoNeedResult = Convert.ToBoolean(reader["IsNoNeedResult"]);
                }
            }

            if (reader.HasColumn("Value_Old") && reader["Value_Old"] != DBNull.Value)
            {
                p.Value_Old = reader["Value_Old"].ToString();
            }

            p.IsNoNeedResult = !p.IsNoNeedResult;

            if (reader.HasColumn("PCLExamTypeID") && reader["PCLExamTypeID"] != DBNull.Value)
            {
                p.PCLExamTypeID = (long)reader["PCLExamTypeID"];
            }
            if (reader.HasColumn("PatientPCLReqID") && reader["PatientPCLReqID"] != DBNull.Value)
            {
                p.PatientPCLReqID = (long)reader["PatientPCLReqID"];
            }
            if (reader.HasColumn("PCLExamTypeName") && reader["PCLExamTypeName"] != DBNull.Value)
            {
                if (p.PCLExamType == null)
                {
                    p.PCLExamType = new PCLExamType();
                }
                p.PCLExamType.PCLExamTypeName = Convert.ToString(reader["PCLExamTypeName"]);
            }

            return p;
        }
        public virtual List<PatientPCLLaboratoryResultDetail> GetPatientPCLLaboratoryResultDetailCollectionFromReader(IDataReader reader)
        {
            List<PatientPCLLaboratoryResultDetail> p = new List<PatientPCLLaboratoryResultDetail>();
            while (reader.Read())
            {
                p.Add(GetPatientPCLLaboratoryResultDetailFromReader(reader));
            }
            return p;
        }


        public abstract IList<MedicalSpecimensCategory> GetAllMedSpecCatg();
     
        public abstract IList<MedicalSpecimen> GetMedSpecsByCatgID(short? medSpecCatgID);

       
   
        //public abstract IList<PCLExamType> GetPtPCLLabExamTypes_RefByPtPCLReqID(long? PtPCLReqID, long? PCLExamGroupID);

        public abstract IList<PatientPCLLaboratoryResultDetail> GetPtPCLLabExamTypes_ByPtPCLReqID(long PatientID, long? PtPCLReqID, long? PCLExamGroupID, long? PCLExamTypeID, bool isImported);
     
        //Medical Specimen Info
        public abstract IList<MedicalSpecimenInfo> GetMedSpecInfo_ByPtPCLReqID(long? PtPCLReqID);

        public abstract string PCLExamTestItems_ByPatientID(long PatientID);


        public abstract IList<PCLExamTestItems> PatientPCLLaboratoryResults_ByExamTest_Paging(long PatientID, long PCLExamTestItemID, long PCLExamTypeID, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRow, out int MaxRow);

        public abstract string PatientPCLLaboratoryResults_ByExamTest_Crosstab(long PatientID, string strXml, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalCol);

        public abstract IList<PCLExamTestItems> PCLExamTestItems_SearchPaging(GeneralSearchCriteria SearchCriteria,int PageIndex,int PageSize,bool bCount,out int Total);

        public abstract bool PCLExamTestItems_Save(PCLExamTestItems Item, out long PCLExamTestItemID);

        public abstract bool RefDepartmentReqCashAdv_Save(RefDepartmentReqCashAdv Target, out long ID);

        public abstract bool RefDepartmentReqCashAdv_Delete(long RefDepartmentReqCashAdvID);

        public abstract List<RefDepartmentReqCashAdv> RefDepartmentReqCashAdv_GetAll(string SearchText, string OrderBy, int PageIndex, int PageSize, bool bCount, out int Total);

        public abstract bool PCLExamTypeServiceTarget_Save(PCLExamTypeServiceTarget Target, out long ID);

        public abstract bool PCLExamTypeServiceTarget_Delete(long PCLExamTypeServiceTargetID);

        public abstract List<PCLExamTypeServiceTarget> PCLExamTypeServiceTarget_GetAll(string SearchText, string OrderBy, int PageIndex, int PageSize,bool bCount ,out int Total);
      
        public abstract bool PCLExamTypeServiceTarget_Checked(long PCLExamTypeID, DateTime Date);

        public abstract bool PCLExamTypeServiceTarget_Checked_Appointment(long PCLExamTypeID, DateTime Date);

        public abstract IList<PatientPCLLaboratoryResultDetail> PCLLaboratoryResults_With_ResultOld(long PatientID, long PatientPCLReqID, long V_PCLRequestType);
        public abstract IList<PatientPCLLaboratoryResultDetail> PCLLaboratoryResults_No_ResultOld( long PatientPCLReqID);

        public abstract bool AddPatientPCLLaboratoryResultDetail(PatientPCLLaboratoryResultDetail entity);

        //public abstract bool UpdatePatientPCLLaboratoryResultDetail(IList<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetailentity);
        // 20181216 TNHX: [BM0005430] Add PatientID for report PCLDEPARTMENT_LABORATORY_RESULT
        public abstract bool UpdatePatientPCLLaboratoryResultDetail(IList<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetailentity, PatientPCLLaboratoryResult entity, long PCLRequestTypeID, long PatientID, out string errorOutput);
        public abstract bool DeletePatientPCLLaboratoryResult(long PatientPCLReqID, long PCLRequestTypeID, long CancelStaffID, long PCLExamTypeID);
        public abstract bool DeletePatientPCLLaboratoryResultDetail(PatientPCLLaboratoryResultDetail entity);
        //public abstract bool AddFullPtPCLLabResult(PatientPCLLaboratoryResult entity, out long newLabResultID);

        public abstract bool DeletePatientPCLImagingResult(long PatientPCLReqID, long PCLRequestTypeID, long CancelStaffID);
        public abstract IList<PatientPCLRequest> ListPatientPCLRequest_LAB_Paging(long PatientID,long? DeptLocID, long V_PCLRequestType,
        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        out int Total);

        public abstract IList<PatientPCLRequest> LIS_Order(string SoPhieuChiDinh, bool IsAll);
        public abstract bool LIS_Result(PatientPCLRequest_LABCom ParamLabCom);


        #endregion

        #region 5. Tuyen: 
        public abstract IList<PCLItem> GetPCLItemsByPCLFormID(long? PCLFormID, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        #endregion

        #region dinh UltraResParams

        
        protected virtual PCLExamParamResult GetPCLExamParamResultFromReader(IDataReader reader)
        {
            PCLExamParamResult p = new PCLExamParamResult();

            if (reader.HasColumn("PCLExamResultID") && reader["PCLExamResultID"] != DBNull.Value)
            {
                p.PCLExamResultID = (long)reader["PCLExamResultID"];

            }


            if (reader.HasColumn("ParamEnum") && reader["ParamEnum"] != DBNull.Value)
            {
                p.ParamEnum = (int)reader["ParamEnum"];

            }


            if (reader.HasColumn("PCLExamGroupTemplateResultID") && reader["PCLExamGroupTemplateResultID"] != DBNull.Value)
            {
                p.PCLExamGroupTemplateResultID = (int)reader["PCLExamGroupTemplateResultID"];

            }


            if (reader.HasColumn("GroupName") && reader["GroupName"] != DBNull.Value)
            {
                p.GroupName = (string)reader["GroupName"];

            }


            return p;
        }
        public virtual List<PCLExamParamResult> GetPCLExamParamResultCollectionFromReader(IDataReader reader)
        {
            List<PCLExamParamResult> p = new List<PCLExamParamResult>();
            while (reader.Read())
            {
                p.Add(GetPCLExamParamResultFromReader(reader));
            }
            return p;
        }



        protected virtual PCLExamResultTemplate GetPCLExamResultTemplateFromReader(IDataReader reader)
        {
            PCLExamResultTemplate p = new PCLExamResultTemplate();


            if (reader.HasColumn("PCLExamResultTemplateID") && reader["PCLExamResultTemplateID"] != DBNull.Value)
            {
                p.PCLExamResultTemplateID = (long)reader["PCLExamResultTemplateID"];

            }


            if (reader.HasColumn("PCLExamTemplateName") && reader["PCLExamTemplateName"] != DBNull.Value)
            {
                p.PCLExamTemplateName = (string)reader["PCLExamTemplateName"];

            }


            if (reader.HasColumn("PCLExamGroupTemplateResultID") && reader["PCLExamGroupTemplateResultID"] != DBNull.Value)
            {
                p.PCLExamGroupTemplateResultID = (int)reader["PCLExamGroupTemplateResultID"];

            }


            if (reader.HasColumn("ResultContent") && reader["ResultContent"] != DBNull.Value)
            {
                p.ResultContent = (string)reader["ResultContent"];

            }


            if (reader.HasColumn("Descriptions") && reader["Descriptions"] != DBNull.Value)
            {
                p.Descriptions = (string)reader["Descriptions"];

            }

            return p;
        }
        public virtual List<PCLExamResultTemplate> GetPCLExamResultTemplateCollectionFromReader(IDataReader reader)
        {
            List<PCLExamResultTemplate> p = new List<PCLExamResultTemplate>();
            while (reader.Read())
            {
                p.Add(GetPCLExamResultTemplateFromReader(reader));
            }
            return p;
        }

        /*▼====: #002*/
        protected virtual Resources GetResourcesForMedicalServicesListByTypeIDFromReader(IDataReader reader)
        {
            Resources p = new Resources();
            if (reader.HasColumn("HIRepResourceCode") && reader["HIRepResourceCode"] != DBNull.Value)
            {
                p.HIRepResourceCode = reader["HIRepResourceCode"].ToString();
            }
            return p;
        }
        public virtual List<Resources> GetResourcesForMedicalServicesListByTypeIDCollectionFromReader(IDataReader reader)
        {
            List<Resources> p = new List<Resources>();
            while (reader.Read())
            {
                p.Add(GetResourcesForMedicalServicesListByTypeIDFromReader(reader));
            }
            return p;
        }
        /*▲====: #002*/

        protected virtual UltraResParams_EchoCardiography GetUltraResParams_EchoCardiographyFromReader(IDataReader reader)
        {
            UltraResParams_EchoCardiography p = new UltraResParams_EchoCardiography();
            
            p.L2D_Situs=new Lookup();
            p.LDOPPLER_Aortic_Grade=new Lookup();
            p.LDOPPLER_Mitral_Grade=new Lookup();
            p.LDOPPLER_Tricuspid_Grade= new Lookup();
            p.LDOPPLER_Pulmonary_Grade= new Lookup();

            if (reader.HasColumn("UltraResParams_EchoCardiographyID") && reader["UltraResParams_EchoCardiographyID"] != DBNull.Value)
            {
                p.UltraResParams_EchoCardiographyID = (long)reader["UltraResParams_EchoCardiographyID"];
            }

            if (reader.HasColumn("TM_Vlt_Ttr") && reader["TM_Vlt_Ttr"] != DBNull.Value)
            {
                p.TM_Vlt_Ttr = (double)reader["TM_Vlt_Ttr"];
            }

            if (reader.HasColumn("TM_Dktt_Ttr") && reader["TM_Dktt_Ttr"] != DBNull.Value)
            {
                p.TM_Dktt_Ttr = (double)reader["TM_Dktt_Ttr"];
            }

            if (reader.HasColumn("TM_Tstt_Ttr") && reader["TM_Tstt_Ttr"] != DBNull.Value)
            {
                p.TM_Tstt_Ttr = (double)reader["TM_Tstt_Ttr"];
            }

            if (reader.HasColumn("TM_Vlt_Tt") && reader["TM_Vlt_Tt"] != DBNull.Value)
            {
                p.TM_Vlt_Tt = (double)reader["TM_Vlt_Tt"];
            }

            if (reader.HasColumn("TM_Dktt_Tt") && reader["TM_Dktt_Tt"] != DBNull.Value)
            {
                p.TM_Dktt_Tt = (double)reader["TM_Dktt_Tt"];
            }

            if (reader.HasColumn("TM_Tstt_Tt") && reader["TM_Tstt_Tt"] != DBNull.Value)
            {
                p.TM_Tstt_Tt = (double)reader["TM_Tstt_Tt"];
            }

            if (reader.HasColumn("TM_Pxcr") && reader["TM_Pxcr"] != DBNull.Value)
            {
                p.TM_Pxcr = (double)reader["TM_Pxcr"];
            }

            if (reader.HasColumn("TM_Pxtm") && reader["TM_Pxtm"] != DBNull.Value)
            {
                p.TM_Pxtm = (double)reader["TM_Pxtm"];
            }

            if (reader.HasColumn("TM_RV") && reader["TM_RV"] != DBNull.Value)
            {
                p.TM_RV = (double)reader["TM_RV"];
            }

            if (reader.HasColumn("TM_Ao") && reader["TM_Ao"] != DBNull.Value)
            {
                p.TM_Ao = (double)reader["TM_Ao"];
            }

            if (reader.HasColumn("TM_La") && reader["TM_La"] != DBNull.Value)
            {
                p.TM_La = (double)reader["TM_La"];
            }

            if (reader.HasColumn("TM_Ssa") && reader["TM_Ssa"] != DBNull.Value)
            {
                p.TM_Ssa = (double)reader["TM_Ssa"];
            }

            if (reader.HasColumn("V_2D_Situs") && reader["V_2D_Situs"] != DBNull.Value)
            {
                p.V_2D_Situs = (long)reader["V_2D_Situs"];
                p.L2D_Situs.LookupID = (long)reader["V_2D_Situs"];
            }

            if (reader.HasColumn("L2D_Situs") && reader["L2D_Situs"] != DBNull.Value)
            {
                p.L2D_Situs.ObjectValue = reader["L2D_Situs"].ToString();
            }

            if (reader.HasColumn("TwoD_Veins") && reader["TwoD_Veins"] != DBNull.Value)
            {
                p.TwoD_Veins = (string)reader["TwoD_Veins"];
            }


            if (reader.HasColumn("TwoD_Ivc") && reader["TwoD_Ivc"] != DBNull.Value)
            {
                p.TwoD_Ivc = (string)reader["TwoD_Ivc"];
            }


            if (reader.HasColumn("TwoD_Svc") && reader["TwoD_Svc"] != DBNull.Value)
            {
                p.TwoD_Svc = (string)reader["TwoD_Svc"];
            }

            if (reader.HasColumn("TwoD_Tvi") && reader["TwoD_Tvi"] != DBNull.Value)
            {
                p.TwoD_Tvi = (string)reader["TwoD_Tvi"];
            }

            if (reader.HasColumn("V_2D_Lsvc") && reader["V_2D_Lsvc"] != DBNull.Value)
            {
                p.V_2D_Lsvc = (long)reader["V_2D_Lsvc"];
            }

            if (reader.HasColumn("V_2D_Azygos") && reader["V_2D_Azygos"] != DBNull.Value)
            {
                p.V_2D_Azygos = (short)reader["V_2D_Azygos"];
            }

            if (reader.HasColumn("TwoD_Pv") && reader["TwoD_Pv"] != DBNull.Value)
            {
                p.TwoD_Pv = (string)reader["TwoD_Pv"];
            }

            if (reader.HasColumn("TwoD_Azygos") && reader["TwoD_Azygos"] != DBNull.Value)
            {
                p.TwoD_Azygos = (string)reader["TwoD_Azygos"];
            }

            if (reader.HasColumn("TwoD_Atria") && reader["TwoD_Atria"] != DBNull.Value)
            {
                p.TwoD_Atria = (string)reader["TwoD_Atria"];
            }

            if (reader.HasColumn("TwoD_Valves") && reader["TwoD_Valves"] != DBNull.Value)
            {
                p.TwoD_Valves = (string)reader["TwoD_Valves"];
            }

            if (reader.HasColumn("TwoD_Cd") && reader["TwoD_Cd"] != DBNull.Value)
            {
                p.TwoD_Cd = (double)reader["TwoD_Cd"];
            }

            if (reader.HasColumn("TwoD_Ma") && reader["TwoD_Ma"] != DBNull.Value)
            {
                p.TwoD_Ma = (double)reader["TwoD_Ma"];
            }

            if (reader.HasColumn("TwoD_MitralArea") && reader["TwoD_MitralArea"] != DBNull.Value)
            {
                p.TwoD_MitralArea = (double)reader["TwoD_MitralArea"];
            }

            if (reader.HasColumn("TwoD_Ta") && reader["TwoD_Ta"] != DBNull.Value)
            {
                p.TwoD_Ta = (double)reader["TwoD_Ta"];
            }

            if (reader.HasColumn("TwoD_LSVC") && reader["TwoD_LSVC"] != DBNull.Value)
            {
                p.TwoD_LSVC = (bool)reader["TwoD_LSVC"];
            }

            if (reader.HasColumn("TwoD_Ventricles") && reader["TwoD_Ventricles"] != DBNull.Value)
            {
                p.TwoD_Ventricles = (string)reader["TwoD_Ventricles"];
            }

            if (reader.HasColumn("TwoD_Aorte") && reader["TwoD_Aorte"] != DBNull.Value)
            {
                p.TwoD_Aorte = (string)reader["TwoD_Aorte"];
            }

            if (reader.HasColumn("TwoD_Asc") && reader["TwoD_Asc"] != DBNull.Value)
            {
                p.TwoD_Asc = (double)reader["TwoD_Asc"];
            }

            if (reader.HasColumn("TwoD_Cr") && reader["TwoD_Cr"] != DBNull.Value)
            {
                p.TwoD_Cr = (double)reader["TwoD_Cr"];
            }

            if (reader.HasColumn("TwoD_Is") && reader["TwoD_Is"] != DBNull.Value)
            {
                p.TwoD_Is = (double)reader["TwoD_Is"];
            }

            if (reader.HasColumn("TwoD_Abd") && reader["TwoD_Abd"] != DBNull.Value)
            {
                p.TwoD_Abd = (double)reader["TwoD_Abd"];
            }

            if (reader.HasColumn("TwoD_D2") && reader["TwoD_D2"] != DBNull.Value)
            {
                p.TwoD_D2 = (double)reader["TwoD_D2"];
            }

            if (reader.HasColumn("TwoD_Ann") && reader["TwoD_Ann"] != DBNull.Value)
            {
                p.TwoD_Ann = (double)reader["TwoD_Ann"];
            }

            if (reader.HasColumn("TwoD_Tap") && reader["TwoD_Tap"] != DBNull.Value)
            {
                p.TwoD_Tap = (double)reader["TwoD_Tap"];
            }

            if (reader.HasColumn("TwoD_Rpa") && reader["TwoD_Rpa"] != DBNull.Value)
            {
                p.TwoD_Rpa = (double)reader["TwoD_Rpa"];
            }

            if (reader.HasColumn("TwoD_Lpa") && reader["TwoD_Lpa"] != DBNull.Value)
            {
                p.TwoD_Lpa = (double)reader["TwoD_Lpa"];
            }

            if (reader.HasColumn("TwoD_Pericarde") && reader["TwoD_Pericarde"] != DBNull.Value)
            {
                p.TwoD_Pericarde = (string)reader["TwoD_Pericarde"];
            }

            if (reader.HasColumn("TwoD_Pa") && reader["TwoD_Pa"] != DBNull.Value)
            {
                p.TwoD_Pa = (string)reader["TwoD_Pa"];
            }

            if (reader.HasColumn("TwoD_Others") && reader["TwoD_Others"] != DBNull.Value)
            {
                p.TwoD_Others = (string)reader["TwoD_Others"];
            }

            if (reader.HasColumn("DOPPLER_Mitral_VelMax") && reader["DOPPLER_Mitral_VelMax"] != DBNull.Value)
            {
                p.DOPPLER_Mitral_VelMax = (double)reader["DOPPLER_Mitral_VelMax"];
            }

            if (reader.HasColumn("DOPPLER_Mitral_GdMax") && reader["DOPPLER_Mitral_GdMax"] != DBNull.Value)
            {
                p.DOPPLER_Mitral_GdMax = (double)reader["DOPPLER_Mitral_GdMax"];
            }

            if (reader.HasColumn("DOPPLER_Mitral_Ms") && reader["DOPPLER_Mitral_Ms"] != DBNull.Value)
            {
                p.DOPPLER_Mitral_Ms = (double)reader["DOPPLER_Mitral_Ms"];
            }

            if (reader.HasColumn("V_DOPPLER_Mitral_Mr") && reader["V_DOPPLER_Mitral_Mr"] != DBNull.Value)
            {
                p.V_DOPPLER_Mitral_Mr = (bool)reader["V_DOPPLER_Mitral_Mr"];
            }

            if (reader.HasColumn("V_DOPPLER_Mitral_Ea") && reader["V_DOPPLER_Mitral_Ea"] != DBNull.Value)
            {
                p.V_DOPPLER_Mitral_Ea = Convert.ToInt32(reader["V_DOPPLER_Mitral_Ea"]);
            }

            if (reader.HasColumn("DOPPLER_Mitral_Ea") && reader["DOPPLER_Mitral_Ea"] != DBNull.Value)
            {
                p.DOPPLER_Mitral_Ea = reader["DOPPLER_Mitral_Ea"].ToString();

            }


            if (reader.HasColumn("DOPPLER_Mitral_Moy") && reader["DOPPLER_Mitral_Moy"] != DBNull.Value)
            {
                p.DOPPLER_Mitral_Moy = (double)reader["DOPPLER_Mitral_Moy"];

            }


            if (reader.HasColumn("DOPPLER_Mitral_Sm") && reader["DOPPLER_Mitral_Sm"] != DBNull.Value)
            {
                p.DOPPLER_Mitral_Sm = (double)reader["DOPPLER_Mitral_Sm"];

            }


            if (reader.HasColumn("V_DOPPLER_Mitral_Grade") && reader["V_DOPPLER_Mitral_Grade"] != DBNull.Value)
            {
                //p.V_DOPPLER_Mitral_Grade = (long)reader["V_DOPPLER_Mitral_Grade"];
                if (p.LDOPPLER_Mitral_Grade == null)
                {
                    p.LDOPPLER_Mitral_Grade = new Lookup();
                }
                p.LDOPPLER_Mitral_Grade.LookupID = (long)reader["V_DOPPLER_Mitral_Grade"];
            }

            if (reader.HasColumn("LDOPPLER_Mitral_Grade") && reader["LDOPPLER_Mitral_Grade"] != DBNull.Value)
            {
                if (p.LDOPPLER_Mitral_Grade == null)
                {
                    p.LDOPPLER_Mitral_Grade = new Lookup();
                }
                p.LDOPPLER_Mitral_Grade.ObjectValue = reader["LDOPPLER_Mitral_Grade"].ToString();
            }

            if (reader.HasColumn("DOPPLER_Aortic_VelMax") && reader["DOPPLER_Aortic_VelMax"] != DBNull.Value)
            {
                p.DOPPLER_Aortic_VelMax = (double)reader["DOPPLER_Aortic_VelMax"];

            }


            if (reader.HasColumn("DOPPLER_Aortic_GdMax") && reader["DOPPLER_Aortic_GdMax"] != DBNull.Value)
            {
                p.DOPPLER_Aortic_GdMax = (double)reader["DOPPLER_Aortic_GdMax"];

            }


            if (reader.HasColumn("DOPPLER_Aortic_As") && reader["DOPPLER_Aortic_As"] != DBNull.Value)
            {
                p.DOPPLER_Aortic_As = (double)reader["DOPPLER_Aortic_As"];

            }


            if (reader.HasColumn("V_DOPPLER_Aortic_Ar") && reader["V_DOPPLER_Aortic_Ar"] != DBNull.Value)
            {
                p.V_DOPPLER_Aortic_Ar = (bool)reader["V_DOPPLER_Aortic_Ar"];

            }


            if (reader.HasColumn("DOPPLER_Aortic_Moy") && reader["DOPPLER_Aortic_Moy"] != DBNull.Value)
            {
                p.DOPPLER_Aortic_Moy = (double)reader["DOPPLER_Aortic_Moy"];

            }


            if (reader.HasColumn("DOPPLER_Aortic_SAo") && reader["DOPPLER_Aortic_SAo"] != DBNull.Value)
            {
                p.DOPPLER_Aortic_SAo = (double)reader["DOPPLER_Aortic_SAo"];

            }


            if (reader.HasColumn("V_DOPPLER_Aortic_Grade") && reader["V_DOPPLER_Aortic_Grade"] != DBNull.Value)
            {
                //p.V_DOPPLER_Aortic_Grade = (long)reader["V_DOPPLER_Aortic_Grade"];
                if (p.LDOPPLER_Aortic_Grade == null)
                {
                    p.LDOPPLER_Aortic_Grade = new Lookup();
                }
                p.LDOPPLER_Aortic_Grade.LookupID = (long)reader["V_DOPPLER_Aortic_Grade"];
            }

            if (reader.HasColumn("LDOPPLER_Aortic_Grade") && reader["LDOPPLER_Aortic_Grade"] != DBNull.Value)
            {
                if (p.LDOPPLER_Aortic_Grade == null)
                {
                    p.LDOPPLER_Aortic_Grade = new Lookup();
                }
                p.LDOPPLER_Aortic_Grade.ObjectValue = reader["LDOPPLER_Aortic_Grade"].ToString();
            }


            if (reader.HasColumn("DOPPLER_Aortic_PHT") && reader["DOPPLER_Aortic_PHT"] != DBNull.Value)
            {
                p.DOPPLER_Aortic_PHT = (double)reader["DOPPLER_Aortic_PHT"];

            }


            if (reader.HasColumn("DOPPLER_Aortic_Dfo") && reader["DOPPLER_Aortic_Dfo"] != DBNull.Value)
            {
                p.DOPPLER_Aortic_Dfo = (double)reader["DOPPLER_Aortic_Dfo"];

            }


            if (reader.HasColumn("DOPPLER_Aortic_Edtd") && reader["DOPPLER_Aortic_Edtd"] != DBNull.Value)
            {
                p.DOPPLER_Aortic_Edtd = (double)reader["DOPPLER_Aortic_Edtd"];

            }


            if (reader.HasColumn("DOPPLER_Aortic_ExtSpat") && reader["DOPPLER_Aortic_ExtSpat"] != DBNull.Value)
            {
                p.DOPPLER_Aortic_ExtSpat = (double)reader["DOPPLER_Aortic_ExtSpat"];

            }


            if (reader.HasColumn("DOPPLER_Tricuspid_VelMax") && reader["DOPPLER_Tricuspid_VelMax"] != DBNull.Value)
            {
                p.DOPPLER_Tricuspid_VelMax = (double)reader["DOPPLER_Tricuspid_VelMax"];

            }


            if (reader.HasColumn("V_DOPPLER_Tricuspid_Tr") && reader["V_DOPPLER_Tricuspid_Tr"] != DBNull.Value)
            {
                p.V_DOPPLER_Tricuspid_Tr = (bool)reader["V_DOPPLER_Tricuspid_Tr"];
            }

            if (reader.HasColumn("V_DOPPLER_Tricuspid_Grade") && reader["V_DOPPLER_Tricuspid_Grade"] != DBNull.Value)
            {
                //p.V_DOPPLER_Tricuspid_Grade = (long)reader["V_DOPPLER_Tricuspid_Grade"];
                if (p.LDOPPLER_Tricuspid_Grade == null)
                {
                    p.LDOPPLER_Tricuspid_Grade = new Lookup();
                }
                p.LDOPPLER_Tricuspid_Grade.LookupID = (long)reader["V_DOPPLER_Tricuspid_Grade"];
            }

            if (reader.HasColumn("LDOPPLER_Tricuspid_Grade") && reader["LDOPPLER_Tricuspid_Grade"] != DBNull.Value)
            {
                if (p.LDOPPLER_Tricuspid_Grade == null)
                {
                    p.LDOPPLER_Tricuspid_Grade = new Lookup();
                }
                p.LDOPPLER_Tricuspid_Grade.ObjectValue = reader["LDOPPLER_Tricuspid_Grade"].ToString();
            }

            if (reader.HasColumn("DOPPLER_Tricuspid_GdMax") && reader["DOPPLER_Tricuspid_GdMax"] != DBNull.Value)
            {
                p.DOPPLER_Tricuspid_GdMax = (double)reader["DOPPLER_Tricuspid_GdMax"];

            }


            if (reader.HasColumn("DOPPLER_Tricuspid_Paps") && reader["DOPPLER_Tricuspid_Paps"] != DBNull.Value)
            {
                p.DOPPLER_Tricuspid_Paps = (double)reader["DOPPLER_Tricuspid_Paps"];

            }


            if (reader.HasColumn("DOPPLER_Tricuspid_Moy") && reader["DOPPLER_Tricuspid_Moy"] != DBNull.Value)
            {
                p.DOPPLER_Tricuspid_Moy = (double)reader["DOPPLER_Tricuspid_Moy"];

            }


            if (reader.HasColumn("DOPPLER_Pulmonary_VelMax") && reader["DOPPLER_Pulmonary_VelMax"] != DBNull.Value)
            {
                p.DOPPLER_Pulmonary_VelMax = (double)reader["DOPPLER_Pulmonary_VelMax"];

            }


            if (reader.HasColumn("DOPPLER_Pulmonary_GdMax") && reader["DOPPLER_Pulmonary_GdMax"] != DBNull.Value)
            {
                p.DOPPLER_Pulmonary_GdMax = (double)reader["DOPPLER_Pulmonary_GdMax"];

            }


            if (reader.HasColumn("V_DOPPLER_Pulmonary_Pr") && reader["V_DOPPLER_Pulmonary_Pr"] != DBNull.Value)
            {
                p.V_DOPPLER_Pulmonary_Pr = (bool)reader["V_DOPPLER_Pulmonary_Pr"];
            }

            if (reader.HasColumn("V_DOPPLER_Pulmonary_Grade") && reader["V_DOPPLER_Pulmonary_Grade"] != DBNull.Value)
            {
                //p.V_DOPPLER_Pulmonary_Grade = (long)reader["V_DOPPLER_Pulmonary_Grade"];
                if (p.LDOPPLER_Pulmonary_Grade == null)
                {
                    p.LDOPPLER_Pulmonary_Grade = new Lookup();
                }
                p.LDOPPLER_Pulmonary_Grade.LookupID = (long)reader["V_DOPPLER_Pulmonary_Grade"];
            }

            if (reader.HasColumn("LDOPPLER_Pulmonary_Grade") && reader["LDOPPLER_Pulmonary_Grade"] != DBNull.Value)
            {
                p.LDOPPLER_Pulmonary_Grade.ObjectValue = reader["LDOPPLER_Pulmonary_Grade"].ToString();
            }

            if (reader.HasColumn("DOPPLER_Pulmonary_Moy") && reader["DOPPLER_Pulmonary_Moy"] != DBNull.Value)
            {
                p.DOPPLER_Pulmonary_Moy = (double)reader["DOPPLER_Pulmonary_Moy"];

            }


            if (reader.HasColumn("DOPPLER_Pulmonary_Papm") && reader["DOPPLER_Pulmonary_Papm"] != DBNull.Value)
            {
                p.DOPPLER_Pulmonary_Papm = (double)reader["DOPPLER_Pulmonary_Papm"];

            }


            if (reader.HasColumn("DOPPLER_Pulmonary_Papd") && reader["DOPPLER_Pulmonary_Papd"] != DBNull.Value)
            {
                p.DOPPLER_Pulmonary_Papd = (double)reader["DOPPLER_Pulmonary_Papd"];

            }


            if (reader.HasColumn("DOPPLER_Pulmonary_Orthers") && reader["DOPPLER_Pulmonary_Orthers"] != DBNull.Value)
            {
                p.DOPPLER_Pulmonary_Orthers = (string)reader["DOPPLER_Pulmonary_Orthers"];

            }


            if (reader.HasColumn("Conclusion") && reader["Conclusion"] != DBNull.Value)
            {
                p.Conclusion = (string)reader["Conclusion"];

            }


            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];

            }


            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.DoctorStaffID = (long)reader["DoctorStaffID"];

            }

            // TxD 26/06/2015: Reset the Tab Change Flags because these were set during setting of the above properties 
            //                  and they were there to indicate when properties have been modified (changed) at the Client side by the GUI
            p.Tab1_TM_Changed = false;
            p.Tab2_2D_Changed = false;
            p.Tab3_Doppler_Changed = false;
            p.Tab4_Conclusion_Changed = false;

            return p;
        }
        public virtual List<UltraResParams_EchoCardiography> GetUltraResParams_EchoCardiographyCollectionFromReader(IDataReader reader)
        {
            List<UltraResParams_EchoCardiography> p = new List<UltraResParams_EchoCardiography>();
            while (reader.Read())
            {
                p.Add(GetUltraResParams_EchoCardiographyFromReader(reader));
            }
            return p;
        }
        //20161214 CMN Begin: Add general inf
        protected virtual UltraResParams_FetalEchocardiography GetUltraResParams_FetalEchocardiographyFromReader(IDataReader reader)
        {
            UltraResParams_FetalEchocardiography p = new UltraResParams_FetalEchocardiography();
            if (reader.HasColumn("UltraResParams_FetalEchocardiographyID") && reader["UltraResParams_FetalEchocardiographyID"] != DBNull.Value)
            {
                p.UltraResParams_FetalEchocardiographyID = (long)reader["UltraResParams_FetalEchocardiographyID"];
            }
            if (reader.HasColumn("PatientPCLReqID") && reader["PatientPCLReqID"] != DBNull.Value)
            {
                p.PatientPCLReqID = (long)reader["PatientPCLReqID"];
            }
            if (reader.HasColumn("FetalAge") && reader["FetalAge"] != DBNull.Value)
            {
                p.FetalAge = Convert.ToInt16(reader["FetalAge"]);
            }
            if (reader.HasColumn("NuchalTranslucency") && reader["NuchalTranslucency"] != DBNull.Value)
            {
                p.NuchalTranslucency = (double)reader["NuchalTranslucency"];
            }
            if (reader.HasColumn("V_EchographyPosture") && reader["V_EchographyPosture"] != DBNull.Value)
            {
                p.V_EchographyPosture = new Lookup { LookupID = (long)reader["V_EchographyPosture"] };
            }
            if (reader.HasColumn("V_MomMedHis") && reader["V_MomMedHis"] != DBNull.Value)
            {
                p.V_MomMedHis = new Lookup { LookupID = (long)reader["V_MomMedHis"] };
            }
            if (reader.HasColumn("Notice") && reader["Notice"] != DBNull.Value)
            {
                p.Notice = (string)reader["Notice"];
            }
            if (reader.HasColumn("CreatedDate") && reader["CreatedDate"] != DBNull.Value)
            {
                p.CreatedDate = (DateTime)reader["CreatedDate"];
            }
            return p;
        }
        //20161214 CMN End.
        protected virtual UltraResParams_FetalEchocardiography2D GetUltraResParams_FetalEchocardiography2DFromReader(IDataReader reader)
        {
            UltraResParams_FetalEchocardiography2D p = new UltraResParams_FetalEchocardiography2D();
            
            if (reader.HasColumn("UltraResParams_FetalEchocardiography2DID") && reader["UltraResParams_FetalEchocardiography2DID"]!=DBNull.Value)
            {
                p.UltraResParams_FetalEchocardiography2DID = (long)reader["UltraResParams_FetalEchocardiography2DID"] ;
            }

            if (reader.HasColumn("NTSize") && reader["NTSize"]!=DBNull.Value)
            {
                p.NTSize = (double)reader["NTSize"] ;
            }

	        
            if (reader.HasColumn("NPSize") && reader["NPSize"]!=DBNull.Value)
            {
                p.NPSize = (double)reader["NPSize"] ;
            }
	        
            if (reader.HasColumn("VanVieussensLeftAtrium") && reader["VanVieussensLeftAtrium"]!=DBNull.Value)
            {
                p.VanVieussensLeftAtrium = (bool)reader["VanVieussensLeftAtrium"];
            }
	        
            if (reader.HasColumn("AtrialSeptalDefect") && reader["AtrialSeptalDefect"]!=DBNull.Value)
            {
                p.AtrialSeptalDefect = (bool)reader["AtrialSeptalDefect"];
            }
	        
            if (reader.HasColumn("MitralValveSize") && reader["MitralValveSize"]!=DBNull.Value)
            {
                p.MitralValveSize = (double)reader["MitralValveSize"];
            }
	        
            if (reader.HasColumn("TriscupidValveSize") && reader["TriscupidValveSize"]!=DBNull.Value)
            {
                p.TriscupidValveSize = (double)reader["TriscupidValveSize"];
            }
	        
            if (reader.HasColumn("DifferenceMitralTricuspid") && reader["DifferenceMitralTricuspid"]!=DBNull.Value)
            {
                p.DifferenceMitralTricuspid = (bool)reader["DifferenceMitralTricuspid"];
            }
	        
            if (reader.HasColumn("TPTTr") && reader["TPTTr"]!=DBNull.Value)
            {
                p.TPTTr = (double)reader["TPTTr"];
            }
	        
            if (reader.HasColumn("VLTTTr") && reader["VLTTTr"]!=DBNull.Value)
            {
                p.VLTTTr = (double)reader["VLTTTr"];
            }
	        
            if (reader.HasColumn("TTTTr") && reader["TTTTr"]!=DBNull.Value)
            {
                p.TTTTr = (double)reader["TTTTr"];
            }
	        
            if (reader.HasColumn("DKTTTTr_VGd") && reader["DKTTTTr_VGd"]!=DBNull.Value)
            {
                p.DKTTTTr_VGd = (double)reader["DKTTTTr_VGd"];
            }
	        
            if (reader.HasColumn("DKTTTT_VGs") && reader["DKTTTT_VGs"]!=DBNull.Value)
            {
                p.DKTTTT_VGs = (double)reader["DKTTTT_VGs"];
            }
	        
            if (reader.HasColumn("DKTPTTr_VDd") && reader["DKTPTTr_VDd"]!=DBNull.Value)
            {
                p.DKTPTTr_VDd = (double)reader["DKTPTTr_VDd"];
            }
	        
            if (reader.HasColumn("DKTPTT_VDs") && reader["DKTPTT_VDs"]!=DBNull.Value)
            {
                p.DKTPTT_VDs = (double)reader["DKTPTT_VDs"];
            }
	        
            if (reader.HasColumn("Systolic") && reader["Systolic"]!=DBNull.Value)
            {
                p.Systolic = (bool)reader["Systolic"];
            }
	        
            if (reader.HasColumn("VentricularSeptalDefect") && reader["VentricularSeptalDefect"]!=DBNull.Value)
            {
                p.VentricularSeptalDefect = (bool)reader["VentricularSeptalDefect"];
            }
	        
            if (reader.HasColumn("AortaCompatible") && reader["AortaCompatible"]!=DBNull.Value)
            {
                p.AortaCompatible = (bool)reader["AortaCompatible"];
            }
	        
            if (reader.HasColumn("AortaSize") && reader["AortaSize"]!=DBNull.Value)
            {
                p.AortaSize = (double)reader["AortaSize"];
            }
	        
            if (reader.HasColumn("PulmonaryArterySize") && reader["PulmonaryArterySize"]!=DBNull.Value)
            {
                p.PulmonaryArterySize = (double)reader["PulmonaryArterySize"];
            }
	        
            if (reader.HasColumn("AorticArch") && reader["AorticArch"]!=DBNull.Value)
            {
                p.AorticArch = (bool)reader["AorticArch"];
            }
	        
            if (reader.HasColumn("AorticCoarctation") && reader["AorticCoarctation"]!=DBNull.Value)
            {
                p.AorticCoarctation = (double)reader["AorticCoarctation"];
            }
	        
            if (reader.HasColumn("HeartRateNomal") && reader["HeartRateNomal"]!=DBNull.Value)
            {
                p.HeartRateNomal = (bool)reader["HeartRateNomal"];
            }
	        
            if (reader.HasColumn("RequencyHeartRateNomal") && reader["RequencyHeartRateNomal"]!=DBNull.Value)
            {
                p.RequencyHeartRateNomal = (double)reader["RequencyHeartRateNomal"];
            }
	        
            if (reader.HasColumn("PericardialEffusion") && reader["PericardialEffusion"]!=DBNull.Value)
            {
                p.PericardialEffusion = (bool)reader["PericardialEffusion"];
            }
	        
            if (reader.HasColumn("FetalCardialAxis") && reader["FetalCardialAxis"]!=DBNull.Value)
            {
                p.FetalCardialAxis = (double)reader["FetalCardialAxis"];
            }
	        
            if (reader.HasColumn("CardialRateS") && reader["CardialRateS"]!=DBNull.Value)
            {
                p.CardialRateS = (double)reader["CardialRateS"];
            }
	        
            if (reader.HasColumn("LN") && reader["LN"]!=DBNull.Value)
            {
                p.LN = (double)reader["LN"];
            }
	        
            if (reader.HasColumn("OrderRecord") && reader["OrderRecord"]!=DBNull.Value)
            {
                p.OrderRecord = reader["OrderRecord"].ToString() ;
            }
	        
            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"]!=DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.DoctorStaffID = (long)reader["DoctorStaffID"] ;
            }
            return p;
        }
        public virtual List<UltraResParams_FetalEchocardiography2D> GetUltraResParams_FetalEchocardiography2DCollectionFromReader(IDataReader reader)
        {
            List<UltraResParams_FetalEchocardiography2D> p = new List<UltraResParams_FetalEchocardiography2D>();
            while (reader.Read())
            {
                p.Add(GetUltraResParams_FetalEchocardiography2DFromReader(reader));
            }
            return p;
        }

        protected virtual UltraResParams_FetalEchocardiographyDoppler GetUltraResParams_FetalEchocardiographyDopplerFromReader(IDataReader reader)
        {
            UltraResParams_FetalEchocardiographyDoppler p = new UltraResParams_FetalEchocardiographyDoppler();

            p.VAorticValve_Open=new Lookup();
            p.VMitralValve_Open=new Lookup();
            p.VPulmonaryValve_Open=new Lookup();
            p.VTriscupidValve_Open=new Lookup();


            if (reader.HasColumn("UltraResParams_FetalEchocardiographyDopplerID") && reader["UltraResParams_FetalEchocardiographyDopplerID"] != DBNull.Value)
            {
                p.UltraResParams_FetalEchocardiographyDopplerID = (long)reader["UltraResParams_FetalEchocardiographyDopplerID"];
            }
	        
            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }
	        
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.DoctorStaffID = (long)reader["DoctorStaffID"];
            }
	        
            if (reader.HasColumn("MitralValve_Vmax") && reader["MitralValve_Vmax"] != DBNull.Value)
            {
                p.MitralValve_Vmax = (double)reader["MitralValve_Vmax"];
            }
	        
            if (reader.HasColumn("MitralValve_Gdmax") && reader["MitralValve_Gdmax"] != DBNull.Value)
            {
                p.MitralValve_Gdmax = (double)reader["MitralValve_Gdmax"];
            }
	        
            if (reader.HasColumn("MitralValve_Open") && reader["MitralValve_Open"] != DBNull.Value)
            {
                p.VMitralValve_Open.LookupID = (long)reader["MitralValve_Open"];
                p.MitralValve_Open = (long)reader["MitralValve_Open"];
            }

            if (reader.HasColumn("MitralValve_OpenValue") && reader["MitralValve_OpenValue"] != DBNull.Value)
            {
                p.VMitralValve_Open.ObjectValue = reader["MitralValve_OpenValue"].ToString();
            }

            if (reader.HasColumn("MitralValve_Stenosis") && reader["MitralValve_Stenosis"] != DBNull.Value)
            {
                p.MitralValve_Stenosis = (bool)reader["MitralValve_Stenosis"];
            }
	        
            if (reader.HasColumn("TriscupidValve_Vmax") && reader["TriscupidValve_Vmax"] != DBNull.Value)
            {
                p.TriscupidValve_Vmax = (double)reader["TriscupidValve_Vmax"];
            }
	        
            if (reader.HasColumn("TriscupidValve_Gdmax") && reader["TriscupidValve_Gdmax"] != DBNull.Value)
            {
                p.TriscupidValve_Gdmax = (double)reader["TriscupidValve_Gdmax"];
            }
	        
            if (reader.HasColumn("TriscupidValve_Open") && reader["TriscupidValve_Open"] != DBNull.Value)
            {
                p.VTriscupidValve_Open.LookupID = (long)reader["TriscupidValve_Open"];
                p.TriscupidValve_Open = (long)reader["TriscupidValve_Open"];
            }

            if (reader.HasColumn("TriscupidValve_OpenValue") && reader["TriscupidValve_OpenValue"] != DBNull.Value)
            {
                p.VTriscupidValve_Open.ObjectValue = reader["TriscupidValve_OpenValue"].ToString();
            }
	        
            if (reader.HasColumn("TriscupidValve_Stenosis") && reader["TriscupidValve_Stenosis"] != DBNull.Value)
            {
                p.TriscupidValve_Stenosis = (bool)reader["TriscupidValve_Stenosis"];
            }
	        
            if (reader.HasColumn("AorticValve_Vmax") && reader["AorticValve_Vmax"] != DBNull.Value)
            {
                p.AorticValve_Vmax = (double)reader["AorticValve_Vmax"];
            }
	        
            if (reader.HasColumn("AorticValve_Gdmax") && reader["AorticValve_Gdmax"] != DBNull.Value)
            {
                p.AorticValve_Gdmax = (double)reader["AorticValve_Gdmax"];
            }
	        
            if (reader.HasColumn("AorticValve_Open") && reader["AorticValve_Open"] != DBNull.Value)
            {
                p.VAorticValve_Open.LookupID = (long)reader["AorticValve_Open"];
                p.AorticValve_Open = (long)reader["AorticValve_Open"];
            }

            if (reader.HasColumn("AorticValve_OpenValue") && reader["AorticValve_OpenValue"] != DBNull.Value)
            {
                p.VAorticValve_Open.ObjectValue = reader["AorticValve_OpenValue"].ToString();
            }

            if (reader.HasColumn("AorticValve_Stenosis") && reader["AorticValve_Stenosis"] != DBNull.Value)
            {
                p.AorticValve_Stenosis = (bool)reader["AorticValve_Stenosis"];
            }
	        
            if (reader.HasColumn("PulmonaryValve_Vmax") && reader["PulmonaryValve_Vmax"] != DBNull.Value)
            {
                p.PulmonaryValve_Vmax = (double)reader["PulmonaryValve_Vmax"];
            }
	        
            if (reader.HasColumn("PulmonaryValve_Gdmax") && reader["PulmonaryValve_Gdmax"] != DBNull.Value)
            {
                p.PulmonaryValve_Gdmax = (double)reader["PulmonaryValve_Gdmax"];
            }
	        
            if (reader.HasColumn("PulmonaryValve_Open") && reader["PulmonaryValve_Open"] != DBNull.Value)
            {
                p.VPulmonaryValve_Open.LookupID = (long)reader["PulmonaryValve_Open"];
                p.PulmonaryValve_Open = (long)reader["PulmonaryValve_Open"];
            }

            if (reader.HasColumn("PulmonaryValve_OpenValue") && reader["PulmonaryValve_OpenValue"] != DBNull.Value)
            {
                p.VPulmonaryValve_Open.ObjectValue = reader["PulmonaryValve_OpenValue"].ToString();
            }
	        
            if (reader.HasColumn("PulmonaryValve_Stenosis") && reader["PulmonaryValve_Stenosis"] != DBNull.Value)
            {
                p.PulmonaryValve_Stenosis = (bool)reader["PulmonaryValve_Stenosis"];
            }
	        
            if (reader.HasColumn("AorticCoarctationBloodTraffic") && reader["AorticCoarctationBloodTraffic"] != DBNull.Value)
            {
                p.AorticCoarctationBloodTraffic = (double)reader["AorticCoarctationBloodTraffic"];
            }
	        
            if (reader.HasColumn("VanViewessensBloodTraffic") && reader["VanViewessensBloodTraffic"] != DBNull.Value)
            {
                p.VanViewessensBloodTraffic = (double)reader["VanViewessensBloodTraffic"];
            }
	        
            if (reader.HasColumn("DuctusAteriosusBloodTraffic") && reader["DuctusAteriosusBloodTraffic"] != DBNull.Value)
            {
                p.DuctusAteriosusBloodTraffic = (double)reader["DuctusAteriosusBloodTraffic"];
            }
	        
            if (reader.HasColumn("DuctusVenosusBloodTraffic") && reader["DuctusVenosusBloodTraffic"] != DBNull.Value)
            {
                p.DuctusVenosusBloodTraffic = (double)reader["DuctusVenosusBloodTraffic"];
            }
	        
            if (reader.HasColumn("PulmonaryVeins_LeftAtrium") && reader["PulmonaryVeins_LeftAtrium"] != DBNull.Value)
            {
                p.PulmonaryVeins_LeftAtrium = (bool)reader["PulmonaryVeins_LeftAtrium"];
            }

            if (reader.HasColumn("OrderRecord") && reader["OrderRecord"] != DBNull.Value)
            {
                p.OrderRecord = reader["OrderRecord"].ToString();
            }
            

            return p;
        }
        public virtual List<UltraResParams_FetalEchocardiographyDoppler> GetUltraResParams_FetalEchocardiographyDopplerCollectionFromReader(IDataReader reader)
        {
            List<UltraResParams_FetalEchocardiographyDoppler> p = new List<UltraResParams_FetalEchocardiographyDoppler>();
            while (reader.Read())
            {
                p.Add(GetUltraResParams_FetalEchocardiographyDopplerFromReader(reader));
            }
            return p;
        }

        protected virtual UltraResParams_FetalEchocardiographyResult GetUltraResParams_FetalEchocardiographyResultFromReader(IDataReader reader)
        {
            UltraResParams_FetalEchocardiographyResult p = new UltraResParams_FetalEchocardiographyResult();

            p.VStaff=new Staff();

            if (reader.HasColumn("UltraResParams_FetalEchocardiographyResultID") && reader["UltraResParams_FetalEchocardiographyResultID"] != DBNull.Value)
            {
                p.UltraResParams_FetalEchocardiographyResultID = (long)reader["UltraResParams_FetalEchocardiographyResultID"];
            }
            
            if (reader.HasColumn("CardialAbnormal") && reader["CardialAbnormal"] != DBNull.Value)
            {
                p.CardialAbnormal = (bool)reader["CardialAbnormal"];
            }
	        
            if (reader.HasColumn("CardialAbnormalDetail") && reader["CardialAbnormalDetail"] != DBNull.Value)
            {
                p.CardialAbnormalDetail =reader["CardialAbnormalDetail"].ToString();
            }
	        
            if (reader.HasColumn("Susgest") && reader["Susgest"] != DBNull.Value)
            {
                p.Susgest = reader["Susgest"].ToString();
            }
	        
            if (reader.HasColumn("UltraResParamDate") && reader["UltraResParamDate"] != DBNull.Value)
            {
                p.UltraResParamDate = (DateTime)reader["UltraResParamDate"];
            }

            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.DoctorStaffID = (long)reader["DoctorStaffID"];
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            return p;
        }
        public virtual List<UltraResParams_FetalEchocardiographyResult> GetUltraResParams_FetalEchocardiographyResultCollectionFromReader(IDataReader reader)
        {
            List<UltraResParams_FetalEchocardiographyResult> p = new List<UltraResParams_FetalEchocardiographyResult>();
            while (reader.Read())
            {
                p.Add(GetUltraResParams_FetalEchocardiographyResultFromReader(reader));
            }
            return p;
        }

        protected virtual UltraResParams_FetalEchocardiographyPostpartum GetUltraResParams_FetalEchocardiographyPostpartumFromReader(IDataReader reader)
        {
            UltraResParams_FetalEchocardiographyPostpartum p = new UltraResParams_FetalEchocardiographyPostpartum();

            p.VStaff=new Staff();

            if (reader.HasColumn("UltraResParams_FetalEchocardiographyPostpartumID") && reader["UltraResParams_FetalEchocardiographyPostpartumID"] != DBNull.Value)
            {
                p.UltraResParams_FetalEchocardiographyPostpartumID = (long)reader["UltraResParams_FetalEchocardiographyPostpartumID"];
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }
	        
            if (reader.HasColumn("BabyBirthday") && reader["BabyBirthday"] != DBNull.Value)
            {
                p.BabyBirthday = (DateTime)reader["BabyBirthday"];
            }
	        
            if (reader.HasColumn("BabyWeight") && reader["BabyWeight"] != DBNull.Value)
            {
                p.BabyWeight = (double)reader["BabyWeight"];
            }
	        
            if (reader.HasColumn("BabySex") && reader["BabySex"] != DBNull.Value)
            {
                p.BabySex = (bool)reader["BabySex"];
            }
	        
            if (reader.HasColumn("URP_Date") && reader["URP_Date"] != DBNull.Value)
            {
                p.URP_Date = (DateTime)reader["URP_Date"];
            }
	        
            if (reader.HasColumn("PFO") && reader["PFO"] != DBNull.Value)
            {
                p.PFO = (bool)reader["PFO"];
            }
	        
            if (reader.HasColumn("PCA") && reader["PCA"] != DBNull.Value)
            {
                p.PCA = (bool)reader["PCA"];
            }
	        
            if (reader.HasColumn("AnotherDiagnosic") && reader["AnotherDiagnosic"] != DBNull.Value)
            {
                p.AnotherDiagnosic = reader["AnotherDiagnosic"].ToString();
            }
	        
            if (reader.HasColumn("Notes") && reader["Notes"] != DBNull.Value)
            {
                p.Notes = reader["Notes"].ToString();
            }

            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.DoctorStaffID = (long)reader["DoctorStaffID"];
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }
            return p;
        }
        public virtual List<UltraResParams_FetalEchocardiographyPostpartum> GetUltraResParams_FetalEchocardiographyPostpartumCollectionFromReader(IDataReader reader)
        {
            List<UltraResParams_FetalEchocardiographyPostpartum> p = new List<UltraResParams_FetalEchocardiographyPostpartum>();
            while (reader.Read())
            {
                p.Add(GetUltraResParams_FetalEchocardiographyPostpartumFromReader(reader));
            }
            return p;
        }




        protected virtual URP_FE_Exam GetURP_FE_ExamFromReader(IDataReader reader)
        {
            URP_FE_Exam p = new URP_FE_Exam();

            if (reader.HasColumn("URP_FE_ExamID") && reader["URP_FE_ExamID"] != DBNull.Value)
            {
                p.URP_FE_ExamID = (long)reader["URP_FE_ExamID"];
            }


            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff=new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }
            

            if (reader.HasColumn("CaoHuyetAp") && reader["CaoHuyetAp"] != DBNull.Value)
            {
                p.CaoHuyetAp = (bool)reader["CaoHuyetAp"];
            }


            if (reader.HasColumn("CaoHuyetApDetail") && reader["CaoHuyetApDetail"] != DBNull.Value)
            {
                p.CaoHuyetApDetail = (string)reader["CaoHuyetApDetail"];
            }


            if (reader.HasColumn("Cholesterol") && reader["Cholesterol"] != DBNull.Value)
            {
                p.Cholesterol = (string)reader["Cholesterol"];
            }


            if (reader.HasColumn("Triglyceride") && reader["Triglyceride"] != DBNull.Value)
            {
                p.Triglyceride = (double)reader["Triglyceride"];
            }


            if (reader.HasColumn("HDL") && reader["HDL"] != DBNull.Value)
            {
                p.HDL = (double)reader["HDL"];
            }


            if (reader.HasColumn("LDL") && reader["LDL"] != DBNull.Value)
            {
                p.LDL = (double)reader["LDL"];
            }


            if (reader.HasColumn("TieuDuong") && reader["TieuDuong"] != DBNull.Value)
            {
                p.TieuDuong = (bool)reader["TieuDuong"];
            }


            if (reader.HasColumn("TieuDuongDetail") && reader["TieuDuongDetail"] != DBNull.Value)
            {
                p.TieuDuongDetail = (string)reader["TieuDuongDetail"];
            }


            if (reader.HasColumn("ThuocLa") && reader["ThuocLa"] != DBNull.Value)
            {
                p.ThuocLa = (bool)reader["ThuocLa"];
            }


            if (reader.HasColumn("Detail") && reader["Detail"] != DBNull.Value)
            {
                p.Detail = (string)reader["Detail"];
            }


            if (reader.HasColumn("ThuocNguaThai") && reader["ThuocNguaThai"] != DBNull.Value)
            {
                p.ThuocNguaThai = (bool)reader["ThuocNguaThai"];
            }


            if (reader.HasColumn("ThuocNguaThaiDetail") && reader["ThuocNguaThaiDetail"] != DBNull.Value)
            {
                p.ThuocNguaThaiDetail = (string)reader["ThuocNguaThaiDetail"];
            }


            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("NhanApMP") && reader["NhanApMP"] != DBNull.Value)
            {
                p.NhanApMP = (string)reader["NhanApMP"];
            }


            if (reader.HasColumn("NhanApMT") && reader["NhanApMT"] != DBNull.Value)
            {
                p.NhanApMT = (string)reader["NhanApMT"];
            }

            return p;
        }
        public virtual List<URP_FE_Exam> GetURP_FE_ExamCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_Exam> p = new List<URP_FE_Exam>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_ExamFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_Oesophagienne GetURP_FE_OesophagienneFromReader(IDataReader reader)
        {
            URP_FE_Oesophagienne p = new URP_FE_Oesophagienne();
            if (reader.HasColumn("URP_FE_OesophagienneID") && reader["URP_FE_OesophagienneID"] != DBNull.Value)
            {
                p.URP_FE_OesophagienneID = (long)reader["URP_FE_OesophagienneID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("ChiDinh") && reader["ChiDinh"] != DBNull.Value)
            {
                p.ChiDinh = (string)reader["ChiDinh"];
            }


            if (reader.HasColumn("Conclusion") && reader["Conclusion"] != DBNull.Value)
            {
                p.ChanDoanThanhNguc = reader["Conclusion"].ToString();
            }


            //if (reader.HasColumn("V_ChanDoanThanhNgucID") && reader["V_ChanDoanThanhNgucID"] != DBNull.Value)
            //{
            //    p.V_ChanDoanThanhNgucID = (long)reader["V_ChanDoanThanhNgucID"];
            //}


            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }

            return p;
        }
        public virtual List<URP_FE_Oesophagienne> GetURP_FE_OesophagienneCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_Oesophagienne> p = new List<URP_FE_Oesophagienne>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_OesophagienneFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_OesophagienneCheck GetURP_FE_OesophagienneCheckFromReader(IDataReader reader)
        {
            URP_FE_OesophagienneCheck p = new URP_FE_OesophagienneCheck();

            if (reader.HasColumn("URP_FE_OesophagienneCheckID") && reader["URP_FE_OesophagienneCheckID"] != DBNull.Value)
            {
                p.URP_FE_OesophagienneCheckID = (long)reader["URP_FE_OesophagienneCheckID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }

            if (reader.HasColumn("CatNghia") && reader["CatNghia"] != DBNull.Value)
            {
                p.CatNghia = (bool)reader["CatNghia"];
            }

            if (reader.HasColumn("NuotNghen") && reader["NuotNghen"] != DBNull.Value)
            {
                p.NuotNghen = (bool)reader["NuotNghen"];
            }


            if (reader.HasColumn("NuotDau") && reader["NuotDau"] != DBNull.Value)
            {
                p.NuotDau = (bool)reader["NuotDau"];
            }


            if (reader.HasColumn("OiMau") && reader["OiMau"] != DBNull.Value)
            {
                p.OiMau = (bool)reader["OiMau"];
            }


            if (reader.HasColumn("XaTriTrungThat") && reader["XaTriTrungThat"] != DBNull.Value)
            {
                p.XaTriTrungThat = (bool)reader["XaTriTrungThat"];
            }


            if (reader.HasColumn("CotSongCo") && reader["CotSongCo"] != DBNull.Value)
            {
                p.CotSongCo = (bool)reader["CotSongCo"];
            }


            if (reader.HasColumn("ChanThuongLongNguc") && reader["ChanThuongLongNguc"] != DBNull.Value)
            {
                p.ChanThuongLongNguc = (bool)reader["ChanThuongLongNguc"];
            }


            if (reader.HasColumn("LanKhamNoiSoiGanDay") && reader["LanKhamNoiSoiGanDay"] != DBNull.Value)
            {
                p.LanKhamNoiSoiGanDay = (bool)reader["LanKhamNoiSoiGanDay"];
            }


            if (reader.HasColumn("DiUngThuoc") && reader["DiUngThuoc"] != DBNull.Value)
            {
                p.DiUngThuoc = (bool)reader["DiUngThuoc"];
            }


            if (reader.HasColumn("NghienRuou") && reader["NghienRuou"] != DBNull.Value)
            {
                p.NghienRuou = (bool)reader["NghienRuou"];
            }


            if (reader.HasColumn("BiTieu") && reader["BiTieu"] != DBNull.Value)
            {
                p.BiTieu = (bool)reader["BiTieu"];
            }


            if (reader.HasColumn("TangNhanApGocHep") && reader["TangNhanApGocHep"] != DBNull.Value)
            {
                p.TangNhanApGocHep = (bool)reader["TangNhanApGocHep"];
            }


            if (reader.HasColumn("Suyen") && reader["Suyen"] != DBNull.Value)
            {
                p.Suyen = (bool)reader["Suyen"];
            }


            if (reader.HasColumn("LanAnSauCung") && reader["LanAnSauCung"] != DBNull.Value)
            {
                p.LanAnSauCung = (bool)reader["LanAnSauCung"];
            }


            if (reader.HasColumn("RangGiaHamGia") && reader["RangGiaHamGia"] != DBNull.Value)
            {
                p.RangGiaHamGia = (bool)reader["RangGiaHamGia"];
            }


            if (reader.HasColumn("HuyetApTT") && reader["HuyetApTT"] != DBNull.Value)
            {
                p.HuyetApTT = (double)reader["HuyetApTT"];
            }


            if (reader.HasColumn("HuyetApTTr") && reader["HuyetApTTr"] != DBNull.Value)
            {
                p.HuyetApTTr = (double)reader["HuyetApTTr"];
            }


            if (reader.HasColumn("Mach") && reader["Mach"] != DBNull.Value)
            {
                p.Mach = (double)reader["Mach"];
            }


            if (reader.HasColumn("DoBaoHoaOxy") && reader["DoBaoHoaOxy"] != DBNull.Value)
            {
                p.DoBaoHoaOxy = (double)reader["DoBaoHoaOxy"];
            }


            if (reader.HasColumn("ThucHienDuongTruyenTinhMach") && reader["ThucHienDuongTruyenTinhMach"] != DBNull.Value)
            {
                p.ThucHienDuongTruyenTinhMach = (bool)reader["ThucHienDuongTruyenTinhMach"];
            }


            if (reader.HasColumn("KiemTraDauDoSieuAm") && reader["KiemTraDauDoSieuAm"] != DBNull.Value)
            {
                p.KiemTraDauDoSieuAm = (bool)reader["KiemTraDauDoSieuAm"];
            }


            if (reader.HasColumn("ChinhDauDoTrungTinh") && reader["ChinhDauDoTrungTinh"] != DBNull.Value)
            {
                p.ChinhDauDoTrungTinh = (bool)reader["ChinhDauDoTrungTinh"];
            }


            if (reader.HasColumn("TeMeBenhNhan") && reader["TeMeBenhNhan"] != DBNull.Value)
            {
                p.TeMeBenhNhan = (bool)reader["TeMeBenhNhan"];
            }


            if (reader.HasColumn("DatBenhNhanNghiengTrai") && reader["DatBenhNhanNghiengTrai"] != DBNull.Value)
            {
                p.DatBenhNhanNghiengTrai = (bool)reader["DatBenhNhanNghiengTrai"];
            }


            if (reader.HasColumn("CotDay") && reader["CotDay"] != DBNull.Value)
            {
                p.CotDay = (bool)reader["CotDay"];
            }


            if (reader.HasColumn("BenhNhanThoaiMai") && reader["BenhNhanThoaiMai"] != DBNull.Value)
            {
                p.BenhNhanThoaiMai = (bool)reader["BenhNhanThoaiMai"];
            }


            if (reader.HasColumn("BoiTronDauDo") && reader["BoiTronDauDo"] != DBNull.Value)
            {
                p.BoiTronDauDo = (bool)reader["BoiTronDauDo"];
            }

            return p;
        }
        public virtual List<URP_FE_OesophagienneCheck> GetURP_FE_OesophagienneCheckCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_OesophagienneCheck> p = new List<URP_FE_OesophagienneCheck>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_OesophagienneCheckFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_OesophagienneDiagnosis GetURP_FE_OesophagienneDiagnosisFromReader(IDataReader reader)
        {
            URP_FE_OesophagienneDiagnosis p = new URP_FE_OesophagienneDiagnosis();

            if (reader.HasColumn("URP_FE_OesophagienneDiagnosisID") && reader["URP_FE_OesophagienneDiagnosisID"] != DBNull.Value)
            {
                p.URP_FE_OesophagienneDiagnosisID = (long)reader["URP_FE_OesophagienneDiagnosisID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("ChanDoanQuaThucQuan") && reader["ChanDoanQuaThucQuan"] != DBNull.Value)
            {
                p.ChanDoanQuaThucQuan = (string)reader["ChanDoanQuaThucQuan"];
            }


            //if (reader.HasColumn("V_ChanDoanQuaThucQuanID") && reader["V_ChanDoanQuaThucQuanID"] != DBNull.Value)
            //{
            //    p.V_ChanDoanQuaThucQuanID = (long)reader["V_ChanDoanQuaThucQuanID"];
            //}

            return p;
        }
        public virtual List<URP_FE_OesophagienneDiagnosis> GetURP_FE_OesophagienneDiagnosisCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_OesophagienneDiagnosis> p = new List<URP_FE_OesophagienneDiagnosis>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_OesophagienneDiagnosisFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_StressDipyridamole GetURP_FE_StressDipyridamoleFromReader(IDataReader reader)
        {
            URP_FE_StressDipyridamole p = new URP_FE_StressDipyridamole();

            if (reader.HasColumn("URP_FE_StressDipyridamoleID") && reader["URP_FE_StressDipyridamoleID"] != DBNull.Value)
            {
                p.URP_FE_StressDipyridamoleID = (long)reader["URP_FE_StressDipyridamoleID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }

            if (reader.HasColumn("TanSoTimCanDat") && reader["TanSoTimCanDat"] != DBNull.Value)
            {
                p.TanSoTimCanDat = (double)reader["TanSoTimCanDat"];
            }


            if (reader.HasColumn("TNP_HuyetAp_TT") && reader["TNP_HuyetAp_TT"] != DBNull.Value)
            {
                p.TNP_HuyetAp_TT = (double)reader["TNP_HuyetAp_TT"];
            }


            if (reader.HasColumn("TNP_HuyetAp_TTr") && reader["TNP_HuyetAp_TTr"] != DBNull.Value)
            {
                p.TNP_HuyetAp_TTr = (double)reader["TNP_HuyetAp_TTr"];
            }


            if (reader.HasColumn("TNP_TanSoTim") && reader["TNP_TanSoTim"] != DBNull.Value)
            {
                p.TNP_TanSoTim = (double)reader["TNP_TanSoTim"];
            }


            if (reader.HasColumn("TNP_TacDungPhu") && reader["TNP_TacDungPhu"] != DBNull.Value)
            {
                p.TNP_TacDungPhu = (string)reader["TNP_TacDungPhu"];
            }


            if (reader.HasColumn("TruyenDipyridamole056_DungLuong") && reader["TruyenDipyridamole056_DungLuong"] != DBNull.Value)
            {
                p.TruyenDipyridamole056_DungLuong = (double)reader["TruyenDipyridamole056_DungLuong"];
            }


            if (reader.HasColumn("TruyenDipy056_P2_HuyetAp_TT") && reader["TruyenDipy056_P2_HuyetAp_TT"] != DBNull.Value)
            {
                p.TruyenDipy056_P2_HuyetAp_TT = (double)reader["TruyenDipy056_P2_HuyetAp_TT"];
            }


            if (reader.HasColumn("TruyenDipy056_P2_HuyetAp_TTr") && reader["TruyenDipy056_P2_HuyetAp_TTr"] != DBNull.Value)
            {
                p.TruyenDipy056_P2_HuyetAp_TTr = (double)reader["TruyenDipy056_P2_HuyetAp_TTr"];
            }


            if (reader.HasColumn("TruyenDipy056_P2_TanSoTim") && reader["TruyenDipy056_P2_TanSoTim"] != DBNull.Value)
            {
                p.TruyenDipy056_P2_TanSoTim = (double)reader["TruyenDipy056_P2_TanSoTim"];
            }


            if (reader.HasColumn("TruyenDipy056_P2_TacDungPhu") && reader["TruyenDipy056_P2_TacDungPhu"] != DBNull.Value)
            {
                p.TruyenDipy056_P2_TacDungPhu = (string)reader["TruyenDipy056_P2_TacDungPhu"];
            }


            if (reader.HasColumn("TruyenDipy056_P4_HuyetAp_TT") && reader["TruyenDipy056_P4_HuyetAp_TT"] != DBNull.Value)
            {
                p.TruyenDipy056_P4_HuyetAp_TT = (double)reader["TruyenDipy056_P4_HuyetAp_TT"];
            }


            if (reader.HasColumn("TruyenDipy056_P4_HuyetAp_TTr") && reader["TruyenDipy056_P4_HuyetAp_TTr"] != DBNull.Value)
            {
                p.TruyenDipy056_P4_HuyetAp_TTr = (double)reader["TruyenDipy056_P4_HuyetAp_TTr"];
            }


            if (reader.HasColumn("TruyenDipy056_P4_TanSoTim") && reader["TruyenDipy056_P4_TanSoTim"] != DBNull.Value)
            {
                p.TruyenDipy056_P4_TanSoTim = (double)reader["TruyenDipy056_P4_TanSoTim"];
            }


            if (reader.HasColumn("TruyenDipy056_P4_TacDungPhu") && reader["TruyenDipy056_P4_TacDungPhu"] != DBNull.Value)
            {
                p.TruyenDipy056_P4_TacDungPhu = (string)reader["TruyenDipy056_P4_TacDungPhu"];
            }


            if (reader.HasColumn("SauLieuDauP6_HuyetAp_TT") && reader["SauLieuDauP6_HuyetAp_TT"] != DBNull.Value)
            {
                p.SauLieuDauP6_HuyetAp_TT = (double)reader["SauLieuDauP6_HuyetAp_TT"];
            }


            if (reader.HasColumn("SauLieuDauP6_HuyetAp_TTr") && reader["SauLieuDauP6_HuyetAp_TTr"] != DBNull.Value)
            {
                p.SauLieuDauP6_HuyetAp_TTr = (double)reader["SauLieuDauP6_HuyetAp_TTr"];
            }


            if (reader.HasColumn("SauLieuDauP6_TanSoTim") && reader["SauLieuDauP6_TanSoTim"] != DBNull.Value)
            {
                p.SauLieuDauP6_TanSoTim = (double)reader["SauLieuDauP6_TanSoTim"];
            }


            if (reader.HasColumn("SauLieuDauP6_TacDungPhu") && reader["SauLieuDauP6_TacDungPhu"] != DBNull.Value)
            {
                p.SauLieuDauP6_TacDungPhu = (string)reader["SauLieuDauP6_TacDungPhu"];
            }


            if (reader.HasColumn("TruyenDipyridamole028_DungLuong") && reader["TruyenDipyridamole028_DungLuong"] != DBNull.Value)
            {
                p.TruyenDipyridamole028_DungLuong = (double)reader["TruyenDipyridamole028_DungLuong"];
            }


            if (reader.HasColumn("TruyenDipy028_P8_HuyetAp_TT") && reader["TruyenDipy028_P8_HuyetAp_TT"] != DBNull.Value)
            {
                p.TruyenDipy028_P8_HuyetAp_TT = (double)reader["TruyenDipy028_P8_HuyetAp_TT"];
            }


            if (reader.HasColumn("TruyenDipy028_P8_HuyetAp_TTr") && reader["TruyenDipy028_P8_HuyetAp_TTr"] != DBNull.Value)
            {
                p.TruyenDipy028_P8_HuyetAp_TTr = (double)reader["TruyenDipy028_P8_HuyetAp_TTr"];
            }


            if (reader.HasColumn("TruyenDipy028_P8_TanSoTim") && reader["TruyenDipy028_P8_TanSoTim"] != DBNull.Value)
            {
                p.TruyenDipy028_P8_TanSoTim = (double)reader["TruyenDipy028_P8_TanSoTim"];
            }


            if (reader.HasColumn("TruyenDipy028_P8_TacDungPhu") && reader["TruyenDipy028_P8_TacDungPhu"] != DBNull.Value)
            {
                p.TruyenDipy028_P8_TacDungPhu = (string)reader["TruyenDipy028_P8_TacDungPhu"];
            }


            if (reader.HasColumn("SauLieu2P10_HuyetAp_TT") && reader["SauLieu2P10_HuyetAp_TT"] != DBNull.Value)
            {
                p.SauLieu2P10_HuyetAp_TT = (double)reader["SauLieu2P10_HuyetAp_TT"];
            }


            if (reader.HasColumn("SauLieu2P10_HuyetAp_TTr") && reader["SauLieu2P10_HuyetAp_TTr"] != DBNull.Value)
            {
                p.SauLieu2P10_HuyetAp_TTr = (double)reader["SauLieu2P10_HuyetAp_TTr"];
            }


            if (reader.HasColumn("SauLieu2P10_TanSoTim") && reader["SauLieu2P10_TanSoTim"] != DBNull.Value)
            {
                p.SauLieu2P10_TanSoTim = (double)reader["SauLieu2P10_TanSoTim"];
            }


            if (reader.HasColumn("SauLieu2P10_TacDungPhu") && reader["SauLieu2P10_TacDungPhu"] != DBNull.Value)
            {
                p.SauLieu2P10_TacDungPhu = (string)reader["SauLieu2P10_TacDungPhu"];
            }


            if (reader.HasColumn("ThemAtropineP12_HuyetAp_TT") && reader["ThemAtropineP12_HuyetAp_TT"] != DBNull.Value)
            {
                p.ThemAtropineP12_HuyetAp_TT = (double)reader["ThemAtropineP12_HuyetAp_TT"];
            }


            if (reader.HasColumn("ThemAtropineP12_HuyetAp_TTr") && reader["ThemAtropineP12_HuyetAp_TTr"] != DBNull.Value)
            {
                p.ThemAtropineP12_HuyetAp_TTr = (double)reader["ThemAtropineP12_HuyetAp_TTr"];
            }


            if (reader.HasColumn("ThemAtropineP12_TanSoTim") && reader["ThemAtropineP12_TanSoTim"] != DBNull.Value)
            {
                p.ThemAtropineP12_TanSoTim = (double)reader["ThemAtropineP12_TanSoTim"];
            }


            if (reader.HasColumn("ThemAtropineP12_TacDungPhu") && reader["ThemAtropineP12_TacDungPhu"] != DBNull.Value)
            {
                p.ThemAtropineP12_TacDungPhu = (string)reader["ThemAtropineP12_TacDungPhu"];
            }


            if (reader.HasColumn("ThemAtropineP13_HuyetAp_TT") && reader["ThemAtropineP13_HuyetAp_TT"] != DBNull.Value)
            {
                p.ThemAtropineP13_HuyetAp_TT = (double)reader["ThemAtropineP13_HuyetAp_TT"];
            }


            if (reader.HasColumn("ThemAtropineP13_HuyetAp_TTr") && reader["ThemAtropineP13_HuyetAp_TTr"] != DBNull.Value)
            {
                p.ThemAtropineP13_HuyetAp_TTr = (double)reader["ThemAtropineP13_HuyetAp_TTr"];
            }


            if (reader.HasColumn("ThemAtropineP13_TanSoTim") && reader["ThemAtropineP13_TanSoTim"] != DBNull.Value)
            {
                p.ThemAtropineP13_TanSoTim = (double)reader["ThemAtropineP13_TanSoTim"];
            }


            if (reader.HasColumn("ThemAtropineP13_TacDungPhu") && reader["ThemAtropineP13_TacDungPhu"] != DBNull.Value)
            {
                p.ThemAtropineP13_TacDungPhu = (string)reader["ThemAtropineP13_TacDungPhu"];
            }


            if (reader.HasColumn("ThemAtropineP14_HuyetAp_TT") && reader["ThemAtropineP14_HuyetAp_TT"] != DBNull.Value)
            {
                p.ThemAtropineP14_HuyetAp_TT = (double)reader["ThemAtropineP14_HuyetAp_TT"];
            }


            if (reader.HasColumn("ThemAtropineP14_HuyetAp_TTr") && reader["ThemAtropineP14_HuyetAp_TTr"] != DBNull.Value)
            {
                p.ThemAtropineP14_HuyetAp_TTr = (double)reader["ThemAtropineP14_HuyetAp_TTr"];
            }


            if (reader.HasColumn("ThemAtropineP14_TanSoTim") && reader["ThemAtropineP14_TanSoTim"] != DBNull.Value)
            {
                p.ThemAtropineP14_TanSoTim = (double)reader["ThemAtropineP14_TanSoTim"];
            }


            if (reader.HasColumn("ThemAtropineP14_TacDungPhu") && reader["ThemAtropineP14_TacDungPhu"] != DBNull.Value)
            {
                p.ThemAtropineP14_TacDungPhu = (string)reader["ThemAtropineP14_TacDungPhu"];
            }


            if (reader.HasColumn("ThemAtropineP15_HuyetAp_TT") && reader["ThemAtropineP15_HuyetAp_TT"] != DBNull.Value)
            {
                p.ThemAtropineP15_HuyetAp_TT = (double)reader["ThemAtropineP15_HuyetAp_TT"];
            }


            if (reader.HasColumn("ThemAtropineP15_HuyetAp_TTr") && reader["ThemAtropineP15_HuyetAp_TTr"] != DBNull.Value)
            {
                p.ThemAtropineP15_HuyetAp_TTr = (double)reader["ThemAtropineP15_HuyetAp_TTr"];
            }


            if (reader.HasColumn("ThemAtropineP15_TanSoTim") && reader["ThemAtropineP15_TanSoTim"] != DBNull.Value)
            {
                p.ThemAtropineP15_TanSoTim = (double)reader["ThemAtropineP15_TanSoTim"];
            }


            if (reader.HasColumn("ThemAtropineP15_TacDungPhu") && reader["ThemAtropineP15_TacDungPhu"] != DBNull.Value)
            {
                p.ThemAtropineP15_TacDungPhu = (string)reader["ThemAtropineP15_TacDungPhu"];
            }


            if (reader.HasColumn("TheoDoiAtropineP16_HuyetAp_TT") && reader["TheoDoiAtropineP16_HuyetAp_TT"] != DBNull.Value)
            {
                p.TheoDoiAtropineP16_HuyetAp_TT = (double)reader["TheoDoiAtropineP16_HuyetAp_TT"];
            }


            if (reader.HasColumn("TheoDoiAtropineP16_HuyetAp_TTr") && reader["TheoDoiAtropineP16_HuyetAp_TTr"] != DBNull.Value)
            {
                p.TheoDoiAtropineP16_HuyetAp_TTr = (double)reader["TheoDoiAtropineP16_HuyetAp_TTr"];
            }


            if (reader.HasColumn("TheoDoiAtropineP16_TanSoTim") && reader["TheoDoiAtropineP16_TanSoTim"] != DBNull.Value)
            {
                p.TheoDoiAtropineP16_TanSoTim = (double)reader["TheoDoiAtropineP16_TanSoTim"];
            }


            if (reader.HasColumn("TheoDoiAtropineP16_TacDungPhu") && reader["TheoDoiAtropineP16_TacDungPhu"] != DBNull.Value)
            {
                p.TheoDoiAtropineP16_TacDungPhu = (string)reader["TheoDoiAtropineP16_TacDungPhu"];
            }


            if (reader.HasColumn("ThemAminophyline_DungLuong") && reader["ThemAminophyline_DungLuong"] != DBNull.Value)
            {
                p.ThemAminophyline_DungLuong = (double)reader["ThemAminophyline_DungLuong"];
            }


            if (reader.HasColumn("ThemAminophyline_Phut") && reader["ThemAminophyline_Phut"] != DBNull.Value)
            {
                p.ThemAminophyline_Phut = (double)reader["ThemAminophyline_Phut"];
            }


            if (reader.HasColumn("ThemAminophyline_HuyetAp_TT") && reader["ThemAminophyline_HuyetAp_TT"] != DBNull.Value)
            {
                p.ThemAminophyline_HuyetAp_TT = (double)reader["ThemAminophyline_HuyetAp_TT"];
            }


            if (reader.HasColumn("ThemAminophyline_HuyetAp_TTr") && reader["ThemAminophyline_HuyetAp_TTr"] != DBNull.Value)
            {
                p.ThemAminophyline_HuyetAp_TTr = (double)reader["ThemAminophyline_HuyetAp_TTr"];
            }


            if (reader.HasColumn("ThemAminophyline_TanSoTim") && reader["ThemAminophyline_TanSoTim"] != DBNull.Value)
            {
                p.ThemAminophyline_TanSoTim = (double)reader["ThemAminophyline_TanSoTim"];
            }


            if (reader.HasColumn("ThemAminophyline_TacDungPhu") && reader["ThemAminophyline_TacDungPhu"] != DBNull.Value)
            {
                p.ThemAminophyline_TacDungPhu = (string)reader["ThemAminophyline_TacDungPhu"];
            }

            return p;
        }
        public virtual List<URP_FE_StressDipyridamole> GetURP_FE_StressDipyridamoleCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_StressDipyridamole> p = new List<URP_FE_StressDipyridamole>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_StressDipyridamoleFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_StressDipyridamoleElectrocardiogram GetURP_FE_StressDipyridamoleElectrocardiogramFromReader(IDataReader reader)
        {
            URP_FE_StressDipyridamoleElectrocardiogram p = new URP_FE_StressDipyridamoleElectrocardiogram();

            if (reader.HasColumn("URP_FE_StressDipyridamoleElectrocardiogramID") && reader["URP_FE_StressDipyridamoleElectrocardiogramID"] != DBNull.Value)
            {
                p.URP_FE_StressDipyridamoleElectrocardiogramID = (long)reader["URP_FE_StressDipyridamoleElectrocardiogramID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("DieuTriConDauThatNguc") && reader["DieuTriConDauThatNguc"] != DBNull.Value)
            {
                p.DieuTriConDauThatNguc = (bool)reader["DieuTriConDauThatNguc"];
            }


            if (reader.HasColumn("DieuTriDIGITALIS") && reader["DieuTriDIGITALIS"] != DBNull.Value)
            {
                p.DieuTriDIGITALIS = (bool)reader["DieuTriDIGITALIS"];
            }


            if (reader.HasColumn("LyDoKhongThucHienDuoc") && reader["LyDoKhongThucHienDuoc"] != DBNull.Value)
            {
                p.LyDoKhongThucHienDuoc = (string)reader["LyDoKhongThucHienDuoc"];
            }


            if (reader.HasColumn("MucGangSuc") && reader["MucGangSuc"] != DBNull.Value)
            {
                p.MucGangSuc = (double)reader["MucGangSuc"];
            }


            if (reader.HasColumn("ThoiGianGangSuc") && reader["ThoiGianGangSuc"] != DBNull.Value)
            {
                p.ThoiGianGangSuc = (double)reader["ThoiGianGangSuc"];
            }


            if (reader.HasColumn("TanSoTim") && reader["TanSoTim"] != DBNull.Value)
            {
                p.TanSoTim = (double)reader["TanSoTim"];
            }


            if (reader.HasColumn("HuyetApToiDa") && reader["HuyetApToiDa"] != DBNull.Value)
            {
                p.HuyetApToiDa = (double)reader["HuyetApToiDa"];
            }


            if (reader.HasColumn("ConDauThatNguc") && reader["ConDauThatNguc"] != DBNull.Value)
            {
                p.ConDauThatNguc = Convert.ToInt32(reader["ConDauThatNguc"]);
            }


            if (reader.HasColumn("STChenhXuong") && reader["STChenhXuong"] != DBNull.Value)
            {
                p.STChenhXuong = (string)reader["STChenhXuong"];
            }


            if (reader.HasColumn("RoiLoanNhipTim") && reader["RoiLoanNhipTim"] != DBNull.Value)
            {
                p.RoiLoanNhipTim = (bool)reader["RoiLoanNhipTim"];
            }


            if (reader.HasColumn("RoiLoanNhipTimChiTiet") && reader["RoiLoanNhipTimChiTiet"] != DBNull.Value)
            {
                p.RoiLoanNhipTimChiTiet = (string)reader["RoiLoanNhipTimChiTiet"];
            }


            if (reader.HasColumn("XetNghiemKhac") && reader["XetNghiemKhac"] != DBNull.Value)
            {
                p.XetNghiemKhac = (string)reader["XetNghiemKhac"];
            }

            return p;
        }
        public virtual List<URP_FE_StressDipyridamoleElectrocardiogram> GetURP_FE_StressDipyridamoleElectrocardiogramCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_StressDipyridamoleElectrocardiogram> p = new List<URP_FE_StressDipyridamoleElectrocardiogram>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_StressDipyridamoleElectrocardiogramFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_StressDipyridamoleExam GetURP_FE_StressDipyridamoleExamFromReader(IDataReader reader)
        {
            URP_FE_StressDipyridamoleExam p = new URP_FE_StressDipyridamoleExam();

            if (reader.HasColumn("URP_FE_StressDipyridamoleExamID") && reader["URP_FE_StressDipyridamoleExamID"] != DBNull.Value)
            {
                p.URP_FE_StressDipyridamoleExamID = (long)reader["URP_FE_StressDipyridamoleExamID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("TrieuChungHienTai") && reader["TrieuChungHienTai"] != DBNull.Value)
            {
                p.TrieuChungHienTai = (string)reader["TrieuChungHienTai"];
            }


            if (reader.HasColumn("ChiDinhSATGSDipy") && reader["ChiDinhSATGSDipy"] != DBNull.Value)
            {
                p.ChiDinhSATGSDipy = (bool)reader["ChiDinhSATGSDipy"];
            }


            if (reader.HasColumn("ChiDinhDetail") && reader["ChiDinhDetail"] != DBNull.Value)
            {
                p.ChiDinhDetail = (string)reader["ChiDinhDetail"];
            }


            if (reader.HasColumn("TDDTruocNgayKham") && reader["TDDTruocNgayKham"] != DBNull.Value)
            {
                p.TDDTruocNgayKham = (string)reader["TDDTruocNgayKham"];
            }


            if (reader.HasColumn("TDDTrongNgaySATGSDipy") && reader["TDDTrongNgaySATGSDipy"] != DBNull.Value)
            {
                p.TDDTrongNgaySATGSDipy = (string)reader["TDDTrongNgaySATGSDipy"];
            }

            return p;
        }
        public virtual List<URP_FE_StressDipyridamoleExam> GetURP_FE_StressDipyridamoleExamCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_StressDipyridamoleExam> p = new List<URP_FE_StressDipyridamoleExam>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_StressDipyridamoleExamFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_StressDipyridamoleImage GetURP_FE_StressDipyridamoleImageFromReader(IDataReader reader)
        {
            URP_FE_StressDipyridamoleImage p = new URP_FE_StressDipyridamoleImage();

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("URP_FE_StressDipyridamoleImageID") && reader["URP_FE_StressDipyridamoleImageID"] != DBNull.Value)
            {
                p.URP_FE_StressDipyridamoleImageID = (long)reader["URP_FE_StressDipyridamoleImageID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("KetLuan") && reader["KetLuan"] != DBNull.Value)
            {
                p.KetLuan = reader["KetLuan"].ToString();
            }
            //==== 20161205 CMN Begin: Get create date else
            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = Convert.ToDateTime(reader["CreateDate"].ToString());
            }
            //==== 20161205 CMN End
            return p;
        }
        public virtual List<URP_FE_StressDipyridamoleImage> GetURP_FE_StressDipyridamoleImageCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_StressDipyridamoleImage> p = new List<URP_FE_StressDipyridamoleImage>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_StressDipyridamoleImageFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_StressDipyridamoleResult GetURP_FE_StressDipyridamoleResultFromReader(IDataReader reader)
        {
            URP_FE_StressDipyridamoleResult p = new URP_FE_StressDipyridamoleResult();

            if (reader.HasColumn("URP_FE_StressDipyridamoleResultID") && reader["URP_FE_StressDipyridamoleResultID"] != DBNull.Value)
            {
                p.URP_FE_StressDipyridamoleResultID = (long)reader["URP_FE_StressDipyridamoleResultID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("ThayDoiDTD") && reader["ThayDoiDTD"] != DBNull.Value)
            {
                p.ThayDoiDTD = (bool)reader["ThayDoiDTD"];
            }


            if (reader.HasColumn("ThayDoiDTDChiTiet") && reader["ThayDoiDTDChiTiet"] != DBNull.Value)
            {
                p.ThayDoiDTDChiTiet = (string)reader["ThayDoiDTDChiTiet"];
            }


            if (reader.HasColumn("RoiLoanNhip") && reader["RoiLoanNhip"] != DBNull.Value)
            {
                p.RoiLoanNhip = (bool)reader["RoiLoanNhip"];
            }


            if (reader.HasColumn("RoiLoanNhipChiTiet") && reader["RoiLoanNhipChiTiet"] != DBNull.Value)
            {
                p.RoiLoanNhipChiTiet = (string)reader["RoiLoanNhipChiTiet"];
            }


            if (reader.HasColumn("TDPHayBienChung") && reader["TDPHayBienChung"] != DBNull.Value)
            {
                p.TDPHayBienChung = Convert.ToInt32(reader["TDPHayBienChung"]);
            }


            if (reader.HasColumn("TrieuChungKhac") && reader["TrieuChungKhac"] != DBNull.Value)
            {
                p.TrieuChungKhac = (string)reader["TrieuChungKhac"];
            }


            if (reader.HasColumn("BienPhapDieuTri") && reader["BienPhapDieuTri"] != DBNull.Value)
            {
                p.BienPhapDieuTri = (string)reader["BienPhapDieuTri"];
            }


            if (reader.HasColumn("V_KetQuaSieuAmTim") && reader["V_KetQuaSieuAmTim"] != DBNull.Value)
            {
                p.V_KetQuaSieuAmTim = (long)reader["V_KetQuaSieuAmTim"];
            }


            if (reader.HasColumn("KetQuaSieuAmTim") && reader["KetQuaSieuAmTim"] != DBNull.Value)
            {
                p.KetQuaSieuAmTim = (string)reader["KetQuaSieuAmTim"];
            }

            if (reader.HasColumn("Conclusion") && reader["Conclusion"] != DBNull.Value)
            {
                p.KetQuaSieuAmTim = reader["Conclusion"].ToString();
            }


            #region V Lookup
            p.V_ThanhTruoc_Mom_TNP = new Lookup();


            p.V_ThanhTruoc_Mom_DobuLieuThap = new Lookup();
            p.V_ThanhTruoc_Mom_DobuLieuCao = new Lookup();
            p.V_ThanhTruoc_Mom_KetLuan = new Lookup();
            p.V_ThanhTruoc_Giua_TNP = new Lookup();
            p.V_ThanhTruoc_Giua_DobuLieuThap = new Lookup();
            p.V_ThanhTruoc_Giua_DobuLieuCao = new Lookup();
            p.V_ThanhTruoc_Giua_KetLuan = new Lookup();
            p.V_ThanhTruoc_Day_TNP = new Lookup();
            p.V_ThanhTruoc_Day_DobuLieuThap = new Lookup();
            p.V_ThanhTruoc_Day_DobuLieuCao = new Lookup();
            p.V_ThanhTruoc_Day_KetLuan = new Lookup();
            p.V_VanhLienThat_Mom_TNP = new Lookup();
            p.V_VanhLienThat_Mom_DobuLieuThap = new Lookup();
            p.V_VanhLienThat_Mom_DobuLieuCao = new Lookup();
            p.V_VanhLienThat_Mom_KetLuan = new Lookup();
            p.V_VanhLienThat_Giua_TNP = new Lookup();
            p.V_VanhLienThat_Giua_DobuLieuThap = new Lookup();
            p.V_VanhLienThat_Giua_DobuLieuCao = new Lookup();
            p.V_VanhLienThat_Giua_KetLuan = new Lookup();
            p.V_VanhLienThat_Day_TNP = new Lookup();
            p.V_VanhLienThat_Day_DobuLieuThap = new Lookup();
            p.V_VanhLienThat_Day_DobuLieuCao = new Lookup();
            p.V_VanhLienThat_Day_KetLuan = new Lookup();
            p.V_ThanhDuoi_Mom_TNP = new Lookup();
            p.V_ThanhDuoi_Mom_DobuLieuThap = new Lookup();
            p.V_ThanhDuoi_Mom_DobuLieuCao = new Lookup();
            p.V_ThanhDuoi_Mom_KetLuan = new Lookup();
            p.V_ThanhDuoi_Giua_TNP = new Lookup();
            p.V_ThanhDuoi_Giua_DobuLieuThap = new Lookup();
            p.V_ThanhDuoi_Giua_DobuLieuCao = new Lookup();
            p.V_ThanhDuoi_Giua_KetLuan = new Lookup();
            p.V_ThanhDuoi_Day_TNP = new Lookup();
            p.V_ThanhDuoi_Day_DobuLieuThap = new Lookup();
            p.V_ThanhDuoi_Day_DobuLieuCao = new Lookup();
            p.V_ThanhDuoi_Day_KetLuan = new Lookup();
            p.V_ThanhSau_Mom_TNP = new Lookup();
            p.V_ThanhSau_Mom_DobuLieuThap = new Lookup();
            p.V_ThanhSau_Mom_DobuLieuCao = new Lookup();
            p.V_ThanhSau_Mom_KetLuan = new Lookup();
            p.V_ThanhSau_Giua_TNP = new Lookup();
            p.V_ThanhSau_Giua_DobuLieuThap = new Lookup();
            p.V_ThanhSau_Giua_DobuLieuCao = new Lookup();
            p.V_ThanhSau_Giua_KetLuan = new Lookup();
            p.V_ThanhSau_Day_TNP = new Lookup();
            p.V_ThanhSau_Day_DobuLieuThap = new Lookup();
            p.V_ThanhSau_Day_DobuLieuCao = new Lookup();
            p.V_ThanhSau_Day_KetLuan = new Lookup();
            p.V_ThanhBen_Mom_TNP = new Lookup();
            p.V_ThanhBen_Mom_DobuLieuThap = new Lookup();
            p.V_ThanhBen_Mom_DobuLieuCao = new Lookup();
            p.V_ThanhBen_Mom_KetLuan = new Lookup();
            p.V_ThanhBen_Giua_TNP = new Lookup();
            p.V_ThanhBen_Giua_DobuLieuThap = new Lookup();
            p.V_ThanhBen_Giua_DobuLieuCao = new Lookup();
            p.V_ThanhBen_Giua_KetLuan = new Lookup();
            p.V_ThanhBen_Day_TNP = new Lookup();
            p.V_ThanhBen_Day_DobuLieuThap = new Lookup();
            p.V_ThanhBen_Day_DobuLieuCao = new Lookup();
            p.V_ThanhBen_Day_KetLuan = new Lookup();
            p.V_TruocVach_Mom_TNP = new Lookup();
            p.V_TruocVach_Mom_DobuLieuThap = new Lookup();
            p.V_TruocVach_Mom_DobuLieuCao = new Lookup();
            p.V_TruocVach_Mom_KetLuan = new Lookup();
            p.V_TruocVach_Giua_TNP = new Lookup();
            p.V_TruocVach_Giua_DobuLieuThap = new Lookup();
            p.V_TruocVach_Giua_DobuLieuCao = new Lookup();
            p.V_TruocVach_Giua_KetLuan = new Lookup();
            p.V_TruocVach_Day_TNP = new Lookup();
            p.V_TruocVach_Day_DobuLieuThap = new Lookup();
            p.V_TruocVach_Day_DobuLieuCao = new Lookup();
            p.V_TruocVach_Day_KetLuan = new Lookup();

            #endregion

            #region VLookup Reader

            if (reader.HasColumn("V_ThanhTruoc_Mom_TNP") && reader["V_ThanhTruoc_Mom_TNP"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Mom_TNP.ObjectValue = reader["V_ThanhTruoc_Mom_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Mom_DobuLieuThap") && reader["V_ThanhTruoc_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Mom_DobuLieuThap.ObjectValue = reader["V_ThanhTruoc_Mom_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Mom_DobuLieuCao") && reader["V_ThanhTruoc_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Mom_DobuLieuCao.ObjectValue = reader["V_ThanhTruoc_Mom_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Mom_KetLuan") && reader["V_ThanhTruoc_Mom_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Mom_KetLuan.ObjectValue = reader["V_ThanhTruoc_Mom_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Giua_TNP") && reader["V_ThanhTruoc_Giua_TNP"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Giua_TNP.ObjectValue = reader["V_ThanhTruoc_Giua_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Giua_DobuLieuThap") && reader["V_ThanhTruoc_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Giua_DobuLieuThap.ObjectValue = reader["V_ThanhTruoc_Giua_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Giua_DobuLieuCao") && reader["V_ThanhTruoc_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Giua_DobuLieuCao.ObjectValue = reader["V_ThanhTruoc_Giua_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Giua_KetLuan") && reader["V_ThanhTruoc_Giua_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Giua_KetLuan.ObjectValue = reader["V_ThanhTruoc_Giua_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Day_TNP") && reader["V_ThanhTruoc_Day_TNP"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Day_TNP.ObjectValue = reader["V_ThanhTruoc_Day_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Day_DobuLieuThap") && reader["V_ThanhTruoc_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Day_DobuLieuThap.ObjectValue = reader["V_ThanhTruoc_Day_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Day_DobuLieuCao") && reader["V_ThanhTruoc_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Day_DobuLieuCao.ObjectValue = reader["V_ThanhTruoc_Day_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Day_KetLuan") && reader["V_ThanhTruoc_Day_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Day_KetLuan.ObjectValue = reader["V_ThanhTruoc_Day_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Mom_TNP") && reader["V_VanhLienThat_Mom_TNP"] != DBNull.Value)
            {
                p.V_VanhLienThat_Mom_TNP.ObjectValue = reader["V_VanhLienThat_Mom_TNP"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Mom_DobuLieuThap") && reader["V_VanhLienThat_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.V_VanhLienThat_Mom_DobuLieuThap.ObjectValue = reader["V_VanhLienThat_Mom_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Mom_DobuLieuCao") && reader["V_VanhLienThat_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.V_VanhLienThat_Mom_DobuLieuCao.ObjectValue = reader["V_VanhLienThat_Mom_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Mom_KetLuan") && reader["V_VanhLienThat_Mom_KetLuan"] != DBNull.Value)
            {
                p.V_VanhLienThat_Mom_KetLuan.ObjectValue = reader["V_VanhLienThat_Mom_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Giua_TNP") && reader["V_VanhLienThat_Giua_TNP"] != DBNull.Value)
            {
                p.V_VanhLienThat_Giua_TNP.ObjectValue = reader["V_VanhLienThat_Giua_TNP"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Giua_DobuLieuThap") && reader["V_VanhLienThat_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.V_VanhLienThat_Giua_DobuLieuThap.ObjectValue = reader["V_VanhLienThat_Giua_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Giua_DobuLieuCao") && reader["V_VanhLienThat_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.V_VanhLienThat_Giua_DobuLieuCao.ObjectValue = reader["V_VanhLienThat_Giua_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Giua_KetLuan") && reader["V_VanhLienThat_Giua_KetLuan"] != DBNull.Value)
            {
                p.V_VanhLienThat_Giua_KetLuan.ObjectValue = reader["V_VanhLienThat_Giua_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Day_TNP") && reader["V_VanhLienThat_Day_TNP"] != DBNull.Value)
            {
                p.V_VanhLienThat_Day_TNP.ObjectValue = reader["V_VanhLienThat_Day_TNP"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Day_DobuLieuThap") && reader["V_VanhLienThat_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.V_VanhLienThat_Day_DobuLieuThap.ObjectValue = reader["V_VanhLienThat_Day_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Day_DobuLieuCao") && reader["V_VanhLienThat_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.V_VanhLienThat_Day_DobuLieuCao.ObjectValue = reader["V_VanhLienThat_Day_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Day_KetLuan") && reader["V_VanhLienThat_Day_KetLuan"] != DBNull.Value)
            {
                p.V_VanhLienThat_Day_KetLuan.ObjectValue = reader["V_VanhLienThat_Day_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Mom_TNP") && reader["V_ThanhDuoi_Mom_TNP"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Mom_TNP.ObjectValue = reader["V_ThanhDuoi_Mom_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Mom_DobuLieuThap") && reader["V_ThanhDuoi_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Mom_DobuLieuThap.ObjectValue = reader["V_ThanhDuoi_Mom_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Mom_DobuLieuCao") && reader["V_ThanhDuoi_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Mom_DobuLieuCao.ObjectValue = reader["V_ThanhDuoi_Mom_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Mom_KetLuan") && reader["V_ThanhDuoi_Mom_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Mom_KetLuan.ObjectValue = reader["V_ThanhDuoi_Mom_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Giua_TNP") && reader["V_ThanhDuoi_Giua_TNP"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Giua_TNP.ObjectValue = reader["V_ThanhDuoi_Giua_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Giua_DobuLieuThap") && reader["V_ThanhDuoi_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Giua_DobuLieuThap.ObjectValue = reader["V_ThanhDuoi_Giua_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Giua_DobuLieuCao") && reader["V_ThanhDuoi_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Giua_DobuLieuCao.ObjectValue = reader["V_ThanhDuoi_Giua_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Giua_KetLuan") && reader["V_ThanhDuoi_Giua_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Giua_KetLuan.ObjectValue = reader["V_ThanhDuoi_Giua_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Day_TNP") && reader["V_ThanhDuoi_Day_TNP"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Day_TNP.ObjectValue = reader["V_ThanhDuoi_Day_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Day_DobuLieuThap") && reader["V_ThanhDuoi_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Day_DobuLieuThap.ObjectValue = reader["V_ThanhDuoi_Day_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Day_DobuLieuCao") && reader["V_ThanhDuoi_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Day_DobuLieuCao.ObjectValue = reader["V_ThanhDuoi_Day_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Day_KetLuan") && reader["V_ThanhDuoi_Day_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Day_KetLuan.ObjectValue = reader["V_ThanhDuoi_Day_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Mom_TNP") && reader["V_ThanhSau_Mom_TNP"] != DBNull.Value)
            {
                p.V_ThanhSau_Mom_TNP.ObjectValue = reader["V_ThanhSau_Mom_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Mom_DobuLieuThap") && reader["V_ThanhSau_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhSau_Mom_DobuLieuThap.ObjectValue = reader["V_ThanhSau_Mom_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Mom_DobuLieuCao") && reader["V_ThanhSau_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhSau_Mom_DobuLieuCao.ObjectValue = reader["V_ThanhSau_Mom_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Mom_KetLuan") && reader["V_ThanhSau_Mom_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhSau_Mom_KetLuan.ObjectValue = reader["V_ThanhSau_Mom_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Giua_TNP") && reader["V_ThanhSau_Giua_TNP"] != DBNull.Value)
            {
                p.V_ThanhSau_Giua_TNP.ObjectValue = reader["V_ThanhSau_Giua_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Giua_DobuLieuThap") && reader["V_ThanhSau_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhSau_Giua_DobuLieuThap.ObjectValue = reader["V_ThanhSau_Giua_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Giua_DobuLieuCao") && reader["V_ThanhSau_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhSau_Giua_DobuLieuCao.ObjectValue = reader["V_ThanhSau_Giua_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Giua_KetLuan") && reader["V_ThanhSau_Giua_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhSau_Giua_KetLuan.ObjectValue = reader["V_ThanhSau_Giua_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Day_TNP") && reader["V_ThanhSau_Day_TNP"] != DBNull.Value)
            {
                p.V_ThanhSau_Day_TNP.ObjectValue = reader["V_ThanhSau_Day_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Day_DobuLieuThap") && reader["V_ThanhSau_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhSau_Day_DobuLieuThap.ObjectValue = reader["V_ThanhSau_Day_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Day_DobuLieuCao") && reader["V_ThanhSau_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhSau_Day_DobuLieuCao.ObjectValue = reader["V_ThanhSau_Day_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Day_KetLuan") && reader["V_ThanhSau_Day_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhSau_Day_KetLuan.ObjectValue = reader["V_ThanhSau_Day_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Mom_TNP") && reader["V_ThanhBen_Mom_TNP"] != DBNull.Value)
            {
                p.V_ThanhBen_Mom_TNP.ObjectValue = reader["V_ThanhBen_Mom_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Mom_DobuLieuThap") && reader["V_ThanhBen_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhBen_Mom_DobuLieuThap.ObjectValue = reader["V_ThanhBen_Mom_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Mom_DobuLieuCao") && reader["V_ThanhBen_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhBen_Mom_DobuLieuCao.ObjectValue = reader["V_ThanhBen_Mom_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Mom_KetLuan") && reader["V_ThanhBen_Mom_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhBen_Mom_KetLuan.ObjectValue = reader["V_ThanhBen_Mom_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Giua_TNP") && reader["V_ThanhBen_Giua_TNP"] != DBNull.Value)
            {
                p.V_ThanhBen_Giua_TNP.ObjectValue = reader["V_ThanhBen_Giua_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Giua_DobuLieuThap") && reader["V_ThanhBen_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhBen_Giua_DobuLieuThap.ObjectValue = reader["V_ThanhBen_Giua_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Giua_DobuLieuCao") && reader["V_ThanhBen_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhBen_Giua_DobuLieuCao.ObjectValue = reader["V_ThanhBen_Giua_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Giua_KetLuan") && reader["V_ThanhBen_Giua_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhBen_Giua_KetLuan.ObjectValue = reader["V_ThanhBen_Giua_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Day_TNP") && reader["V_ThanhBen_Day_TNP"] != DBNull.Value)
            {
                p.V_ThanhBen_Day_TNP.ObjectValue = reader["V_ThanhBen_Day_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Day_DobuLieuThap") && reader["V_ThanhBen_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhBen_Day_DobuLieuThap.ObjectValue = reader["V_ThanhBen_Day_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Day_DobuLieuCao") && reader["V_ThanhBen_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhBen_Day_DobuLieuCao.ObjectValue = reader["V_ThanhBen_Day_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Day_KetLuan") && reader["V_ThanhBen_Day_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhBen_Day_KetLuan.ObjectValue = reader["V_ThanhBen_Day_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Mom_TNP") && reader["V_TruocVach_Mom_TNP"] != DBNull.Value)
            {
                p.V_TruocVach_Mom_TNP.ObjectValue = reader["V_TruocVach_Mom_TNP"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Mom_DobuLieuThap") && reader["V_TruocVach_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.V_TruocVach_Mom_DobuLieuThap.ObjectValue = reader["V_TruocVach_Mom_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Mom_DobuLieuCao") && reader["V_TruocVach_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.V_TruocVach_Mom_DobuLieuCao.ObjectValue = reader["V_TruocVach_Mom_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Mom_KetLuan") && reader["V_TruocVach_Mom_KetLuan"] != DBNull.Value)
            {
                p.V_TruocVach_Mom_KetLuan.ObjectValue = reader["V_TruocVach_Mom_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Giua_TNP") && reader["V_TruocVach_Giua_TNP"] != DBNull.Value)
            {
                p.V_TruocVach_Giua_TNP.ObjectValue = reader["V_TruocVach_Giua_TNP"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Giua_DobuLieuThap") && reader["V_TruocVach_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.V_TruocVach_Giua_DobuLieuThap.ObjectValue = reader["V_TruocVach_Giua_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Giua_DobuLieuCao") && reader["V_TruocVach_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.V_TruocVach_Giua_DobuLieuCao.ObjectValue = reader["V_TruocVach_Giua_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Giua_KetLuan") && reader["V_TruocVach_Giua_KetLuan"] != DBNull.Value)
            {
                p.V_TruocVach_Giua_KetLuan.ObjectValue = reader["V_TruocVach_Giua_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Day_TNP") && reader["V_TruocVach_Day_TNP"] != DBNull.Value)
            {
                p.V_TruocVach_Day_TNP.ObjectValue = reader["V_TruocVach_Day_TNP"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Day_DobuLieuThap") && reader["V_TruocVach_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.V_TruocVach_Day_DobuLieuThap.ObjectValue = reader["V_TruocVach_Day_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Day_DobuLieuCao") && reader["V_TruocVach_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.V_TruocVach_Day_DobuLieuCao.ObjectValue = reader["V_TruocVach_Day_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Day_KetLuan") && reader["V_TruocVach_Day_KetLuan"] != DBNull.Value)
            {
                p.V_TruocVach_Day_KetLuan.ObjectValue = reader["V_TruocVach_Day_KetLuan"].ToString();
            }








            #endregion

            #region primative
            if (reader.HasColumn("ThanhTruoc_Mom_TNP") && reader["ThanhTruoc_Mom_TNP"] != DBNull.Value)
            {
                p.ThanhTruoc_Mom_TNP = (long)reader["ThanhTruoc_Mom_TNP"];
                p.V_ThanhTruoc_Mom_TNP.LookupID = (long)reader["ThanhTruoc_Mom_TNP"];
            }


            if (reader.HasColumn("ThanhTruoc_Mom_DobuLieuThap") && reader["ThanhTruoc_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhTruoc_Mom_DobuLieuThap = (long)reader["ThanhTruoc_Mom_DobuLieuThap"];
                p.V_ThanhTruoc_Mom_DobuLieuThap.LookupID = (long)reader["ThanhTruoc_Mom_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhTruoc_Mom_DobuLieuCao") && reader["ThanhTruoc_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhTruoc_Mom_DobuLieuCao = (long)reader["ThanhTruoc_Mom_DobuLieuCao"];
                p.V_ThanhTruoc_Mom_DobuLieuCao.LookupID = (long)reader["ThanhTruoc_Mom_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhTruoc_Mom_KetLuan") && reader["ThanhTruoc_Mom_KetLuan"] != DBNull.Value)
            {
                p.ThanhTruoc_Mom_KetLuan = (long)reader["ThanhTruoc_Mom_KetLuan"];
                p.V_ThanhTruoc_Mom_KetLuan.LookupID = (long)reader["ThanhTruoc_Mom_KetLuan"];
            }


            if (reader.HasColumn("ThanhTruoc_Giua_TNP") && reader["ThanhTruoc_Giua_TNP"] != DBNull.Value)
            {
                p.ThanhTruoc_Giua_TNP = (long)reader["ThanhTruoc_Giua_TNP"];
                p.V_ThanhTruoc_Giua_TNP.LookupID = (long)reader["ThanhTruoc_Giua_TNP"];
            }


            if (reader.HasColumn("ThanhTruoc_Giua_DobuLieuThap") && reader["ThanhTruoc_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhTruoc_Giua_DobuLieuThap = (long)reader["ThanhTruoc_Giua_DobuLieuThap"];
                p.V_ThanhTruoc_Giua_DobuLieuThap.LookupID = (long)reader["ThanhTruoc_Giua_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhTruoc_Giua_DobuLieuCao") && reader["ThanhTruoc_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhTruoc_Giua_DobuLieuCao = (long)reader["ThanhTruoc_Giua_DobuLieuCao"];
                p.V_ThanhTruoc_Giua_DobuLieuCao.LookupID = (long)reader["ThanhTruoc_Giua_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhTruoc_Giua_KetLuan") && reader["ThanhTruoc_Giua_KetLuan"] != DBNull.Value)
            {
                p.ThanhTruoc_Giua_KetLuan = (long)reader["ThanhTruoc_Giua_KetLuan"];
                p.V_ThanhTruoc_Giua_KetLuan.LookupID = (long)reader["ThanhTruoc_Giua_KetLuan"];
            }


            if (reader.HasColumn("ThanhTruoc_Day_TNP") && reader["ThanhTruoc_Day_TNP"] != DBNull.Value)
            {
                p.ThanhTruoc_Day_TNP = (long)reader["ThanhTruoc_Day_TNP"];
                p.V_ThanhTruoc_Day_TNP.LookupID = (long)reader["ThanhTruoc_Day_TNP"];
            }


            if (reader.HasColumn("ThanhTruoc_Day_DobuLieuThap") && reader["ThanhTruoc_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhTruoc_Day_DobuLieuThap = (long)reader["ThanhTruoc_Day_DobuLieuThap"];
                p.V_ThanhTruoc_Day_DobuLieuThap.LookupID = (long)reader["ThanhTruoc_Day_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhTruoc_Day_DobuLieuCao") && reader["ThanhTruoc_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhTruoc_Day_DobuLieuCao = (long)reader["ThanhTruoc_Day_DobuLieuCao"];
                p.V_ThanhTruoc_Day_DobuLieuCao.LookupID = (long)reader["ThanhTruoc_Day_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhTruoc_Day_KetLuan") && reader["ThanhTruoc_Day_KetLuan"] != DBNull.Value)
            {
                p.ThanhTruoc_Day_KetLuan = (long)reader["ThanhTruoc_Day_KetLuan"];
                p.V_ThanhTruoc_Day_KetLuan.LookupID = (long)reader["ThanhTruoc_Day_KetLuan"];
            }


            if (reader.HasColumn("VanhLienThat_Mom_TNP") && reader["VanhLienThat_Mom_TNP"] != DBNull.Value)
            {
                p.VanhLienThat_Mom_TNP = (long)reader["VanhLienThat_Mom_TNP"];
                p.V_VanhLienThat_Mom_TNP.LookupID = (long)reader["VanhLienThat_Mom_TNP"];
            }


            if (reader.HasColumn("VanhLienThat_Mom_DobuLieuThap") && reader["VanhLienThat_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.VanhLienThat_Mom_DobuLieuThap = (long)reader["VanhLienThat_Mom_DobuLieuThap"];
                p.V_VanhLienThat_Mom_DobuLieuThap.LookupID = (long)reader["VanhLienThat_Mom_DobuLieuThap"];
            }


            if (reader.HasColumn("VanhLienThat_Mom_DobuLieuCao") && reader["VanhLienThat_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.VanhLienThat_Mom_DobuLieuCao = (long)reader["VanhLienThat_Mom_DobuLieuCao"];
                p.V_VanhLienThat_Mom_DobuLieuCao.LookupID = (long)reader["VanhLienThat_Mom_DobuLieuCao"];
            }


            if (reader.HasColumn("VanhLienThat_Mom_KetLuan") && reader["VanhLienThat_Mom_KetLuan"] != DBNull.Value)
            {
                p.VanhLienThat_Mom_KetLuan = (long)reader["VanhLienThat_Mom_KetLuan"];
                p.V_VanhLienThat_Mom_KetLuan.LookupID = (long)reader["VanhLienThat_Mom_KetLuan"];
            }


            if (reader.HasColumn("VanhLienThat_Giua_TNP") && reader["VanhLienThat_Giua_TNP"] != DBNull.Value)
            {
                p.VanhLienThat_Giua_TNP = (long)reader["VanhLienThat_Giua_TNP"];
                p.V_VanhLienThat_Giua_TNP.LookupID = (long)reader["VanhLienThat_Giua_TNP"];
            }


            if (reader.HasColumn("VanhLienThat_Giua_DobuLieuThap") && reader["VanhLienThat_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.VanhLienThat_Giua_DobuLieuThap = (long)reader["VanhLienThat_Giua_DobuLieuThap"];
                p.V_VanhLienThat_Giua_DobuLieuThap.LookupID = (long)reader["VanhLienThat_Giua_DobuLieuThap"];
            }


            if (reader.HasColumn("VanhLienThat_Giua_DobuLieuCao") && reader["VanhLienThat_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.VanhLienThat_Giua_DobuLieuCao = (long)reader["VanhLienThat_Giua_DobuLieuCao"];
                p.V_VanhLienThat_Giua_DobuLieuCao.LookupID = (long)reader["VanhLienThat_Giua_DobuLieuCao"];
            }


            if (reader.HasColumn("VanhLienThat_Giua_KetLuan") && reader["VanhLienThat_Giua_KetLuan"] != DBNull.Value)
            {
                p.VanhLienThat_Giua_KetLuan = (long)reader["VanhLienThat_Giua_KetLuan"];
                p.V_VanhLienThat_Giua_KetLuan.LookupID = (long)reader["VanhLienThat_Giua_KetLuan"];
            }


            if (reader.HasColumn("VanhLienThat_Day_TNP") && reader["VanhLienThat_Day_TNP"] != DBNull.Value)
            {
                p.VanhLienThat_Day_TNP = (long)reader["VanhLienThat_Day_TNP"];
                p.V_VanhLienThat_Day_TNP.LookupID = (long)reader["VanhLienThat_Day_TNP"];
            }


            if (reader.HasColumn("VanhLienThat_Day_DobuLieuThap") && reader["VanhLienThat_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.VanhLienThat_Day_DobuLieuThap = (long)reader["VanhLienThat_Day_DobuLieuThap"];
                p.V_VanhLienThat_Day_DobuLieuThap.LookupID = (long)reader["VanhLienThat_Day_DobuLieuThap"];
            }


            if (reader.HasColumn("VanhLienThat_Day_DobuLieuCao") && reader["VanhLienThat_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.VanhLienThat_Day_DobuLieuCao = (long)reader["VanhLienThat_Day_DobuLieuCao"];
                p.V_VanhLienThat_Day_DobuLieuCao.LookupID = (long)reader["VanhLienThat_Day_DobuLieuCao"];
            }


            if (reader.HasColumn("VanhLienThat_Day_KetLuan") && reader["VanhLienThat_Day_KetLuan"] != DBNull.Value)
            {
                p.VanhLienThat_Day_KetLuan = (long)reader["VanhLienThat_Day_KetLuan"];
                p.V_VanhLienThat_Day_KetLuan.LookupID = (long)reader["VanhLienThat_Day_KetLuan"];
            }


            if (reader.HasColumn("ThanhDuoi_Mom_TNP") && reader["ThanhDuoi_Mom_TNP"] != DBNull.Value)
            {
                p.ThanhDuoi_Mom_TNP = (long)reader["ThanhDuoi_Mom_TNP"];
                p.V_ThanhDuoi_Mom_TNP.LookupID = (long)reader["ThanhDuoi_Mom_TNP"];
            }


            if (reader.HasColumn("ThanhDuoi_Mom_DobuLieuThap") && reader["ThanhDuoi_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhDuoi_Mom_DobuLieuThap = (long)reader["ThanhDuoi_Mom_DobuLieuThap"];
                p.V_ThanhDuoi_Mom_DobuLieuThap.LookupID = (long)reader["ThanhDuoi_Mom_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhDuoi_Mom_DobuLieuCao") && reader["ThanhDuoi_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhDuoi_Mom_DobuLieuCao = (long)reader["ThanhDuoi_Mom_DobuLieuCao"];
                p.V_ThanhDuoi_Mom_DobuLieuCao.LookupID = (long)reader["ThanhDuoi_Mom_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhDuoi_Mom_KetLuan") && reader["ThanhDuoi_Mom_KetLuan"] != DBNull.Value)
            {
                p.ThanhDuoi_Mom_KetLuan = (long)reader["ThanhDuoi_Mom_KetLuan"];
                p.V_ThanhDuoi_Mom_KetLuan.LookupID = (long)reader["ThanhDuoi_Mom_KetLuan"];
            }


            if (reader.HasColumn("ThanhDuoi_Giua_TNP") && reader["ThanhDuoi_Giua_TNP"] != DBNull.Value)
            {
                p.ThanhDuoi_Giua_TNP = (long)reader["ThanhDuoi_Giua_TNP"];
                p.V_ThanhDuoi_Giua_TNP.LookupID = (long)reader["ThanhDuoi_Giua_TNP"];
            }


            if (reader.HasColumn("ThanhDuoi_Giua_DobuLieuThap") && reader["ThanhDuoi_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhDuoi_Giua_DobuLieuThap = (long)reader["ThanhDuoi_Giua_DobuLieuThap"];
                p.V_ThanhDuoi_Giua_DobuLieuThap.LookupID = (long)reader["ThanhDuoi_Giua_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhDuoi_Giua_DobuLieuCao") && reader["ThanhDuoi_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhDuoi_Giua_DobuLieuCao = (long)reader["ThanhDuoi_Giua_DobuLieuCao"];
                p.V_ThanhDuoi_Giua_DobuLieuCao.LookupID = (long)reader["ThanhDuoi_Giua_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhDuoi_Giua_KetLuan") && reader["ThanhDuoi_Giua_KetLuan"] != DBNull.Value)
            {
                p.ThanhDuoi_Giua_KetLuan = (long)reader["ThanhDuoi_Giua_KetLuan"];
                p.V_ThanhDuoi_Giua_KetLuan.LookupID = (long)reader["ThanhDuoi_Giua_KetLuan"];
            }


            if (reader.HasColumn("ThanhDuoi_Day_TNP") && reader["ThanhDuoi_Day_TNP"] != DBNull.Value)
            {
                p.ThanhDuoi_Day_TNP = (long)reader["ThanhDuoi_Day_TNP"];
                p.V_ThanhDuoi_Day_TNP.LookupID = (long)reader["ThanhDuoi_Day_TNP"];
            }


            if (reader.HasColumn("ThanhDuoi_Day_DobuLieuThap") && reader["ThanhDuoi_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhDuoi_Day_DobuLieuThap = (long)reader["ThanhDuoi_Day_DobuLieuThap"];
                p.V_ThanhDuoi_Day_DobuLieuThap.LookupID = (long)reader["ThanhDuoi_Day_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhDuoi_Day_DobuLieuCao") && reader["ThanhDuoi_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhDuoi_Day_DobuLieuCao = (long)reader["ThanhDuoi_Day_DobuLieuCao"];
                p.V_ThanhDuoi_Day_DobuLieuCao.LookupID = (long)reader["ThanhDuoi_Day_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhDuoi_Day_KetLuan") && reader["ThanhDuoi_Day_KetLuan"] != DBNull.Value)
            {
                p.ThanhDuoi_Day_KetLuan = (long)reader["ThanhDuoi_Day_KetLuan"];
                p.V_ThanhDuoi_Day_KetLuan.LookupID = (long)reader["ThanhDuoi_Day_KetLuan"];
            }


            if (reader.HasColumn("ThanhSau_Mom_TNP") && reader["ThanhSau_Mom_TNP"] != DBNull.Value)
            {
                p.ThanhSau_Mom_TNP = (long)reader["ThanhSau_Mom_TNP"];
                p.V_ThanhSau_Mom_TNP.LookupID = (long)reader["ThanhSau_Mom_TNP"];
            }


            if (reader.HasColumn("ThanhSau_Mom_DobuLieuThap") && reader["ThanhSau_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhSau_Mom_DobuLieuThap = (long)reader["ThanhSau_Mom_DobuLieuThap"];
                p.V_ThanhSau_Mom_DobuLieuThap.LookupID = (long)reader["ThanhSau_Mom_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhSau_Mom_DobuLieuCao") && reader["ThanhSau_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhSau_Mom_DobuLieuCao = (long)reader["ThanhSau_Mom_DobuLieuCao"];
                p.V_ThanhSau_Mom_DobuLieuCao.LookupID = (long)reader["ThanhSau_Mom_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhSau_Mom_KetLuan") && reader["ThanhSau_Mom_KetLuan"] != DBNull.Value)
            {
                p.ThanhSau_Mom_KetLuan = (long)reader["ThanhSau_Mom_KetLuan"];
                p.V_ThanhSau_Mom_KetLuan.LookupID = (long)reader["ThanhSau_Mom_KetLuan"];
            }


            if (reader.HasColumn("ThanhSau_Giua_TNP") && reader["ThanhSau_Giua_TNP"] != DBNull.Value)
            {
                p.ThanhSau_Giua_TNP = (long)reader["ThanhSau_Giua_TNP"];
                p.V_ThanhSau_Giua_TNP.LookupID = (long)reader["ThanhSau_Giua_TNP"];
            }


            if (reader.HasColumn("ThanhSau_Giua_DobuLieuThap") && reader["ThanhSau_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhSau_Giua_DobuLieuThap = (long)reader["ThanhSau_Giua_DobuLieuThap"];
                p.V_ThanhSau_Giua_DobuLieuThap.LookupID = (long)reader["ThanhSau_Giua_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhSau_Giua_DobuLieuCao") && reader["ThanhSau_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhSau_Giua_DobuLieuCao = (long)reader["ThanhSau_Giua_DobuLieuCao"];
                p.V_ThanhSau_Giua_DobuLieuCao.LookupID = (long)reader["ThanhSau_Giua_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhSau_Giua_KetLuan") && reader["ThanhSau_Giua_KetLuan"] != DBNull.Value)
            {
                p.ThanhSau_Giua_KetLuan = (long)reader["ThanhSau_Giua_KetLuan"];
                p.V_ThanhSau_Giua_KetLuan.LookupID = (long)reader["ThanhSau_Giua_KetLuan"];
            }


            if (reader.HasColumn("ThanhSau_Day_TNP") && reader["ThanhSau_Day_TNP"] != DBNull.Value)
            {
                p.ThanhSau_Day_TNP = (long)reader["ThanhSau_Day_TNP"];
                p.V_ThanhSau_Day_TNP.LookupID = (long)reader["ThanhSau_Day_TNP"];
            }


            if (reader.HasColumn("ThanhSau_Day_DobuLieuThap") && reader["ThanhSau_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhSau_Day_DobuLieuThap = (long)reader["ThanhSau_Day_DobuLieuThap"];
                p.V_ThanhSau_Day_DobuLieuThap.LookupID = (long)reader["ThanhSau_Day_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhSau_Day_DobuLieuCao") && reader["ThanhSau_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhSau_Day_DobuLieuCao = (long)reader["ThanhSau_Day_DobuLieuCao"];
                p.V_ThanhSau_Day_DobuLieuCao.LookupID = (long)reader["ThanhSau_Day_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhSau_Day_KetLuan") && reader["ThanhSau_Day_KetLuan"] != DBNull.Value)
            {
                p.ThanhSau_Day_KetLuan = (long)reader["ThanhSau_Day_KetLuan"];
                p.V_ThanhSau_Day_KetLuan.LookupID = (long)reader["ThanhSau_Day_KetLuan"];
            }


            if (reader.HasColumn("ThanhBen_Mom_TNP") && reader["ThanhBen_Mom_TNP"] != DBNull.Value)
            {
                p.ThanhBen_Mom_TNP = (long)reader["ThanhBen_Mom_TNP"];
                p.V_ThanhBen_Mom_TNP.LookupID = (long)reader["ThanhBen_Mom_TNP"];
            }


            if (reader.HasColumn("ThanhBen_Mom_DobuLieuThap") && reader["ThanhBen_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhBen_Mom_DobuLieuThap = (long)reader["ThanhBen_Mom_DobuLieuThap"];
                p.V_ThanhBen_Mom_DobuLieuThap.LookupID = (long)reader["ThanhBen_Mom_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhBen_Mom_DobuLieuCao") && reader["ThanhBen_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhBen_Mom_DobuLieuCao = (long)reader["ThanhBen_Mom_DobuLieuCao"];
                p.V_ThanhBen_Mom_DobuLieuCao.LookupID = (long)reader["ThanhBen_Mom_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhBen_Mom_KetLuan") && reader["ThanhBen_Mom_KetLuan"] != DBNull.Value)
            {
                p.ThanhBen_Mom_KetLuan = (long)reader["ThanhBen_Mom_KetLuan"];
                p.V_ThanhBen_Mom_KetLuan.LookupID = (long)reader["ThanhBen_Mom_KetLuan"];
            }


            if (reader.HasColumn("ThanhBen_Giua_TNP") && reader["ThanhBen_Giua_TNP"] != DBNull.Value)
            {
                p.ThanhBen_Giua_TNP = (long)reader["ThanhBen_Giua_TNP"];
                p.V_ThanhBen_Giua_TNP.LookupID = (long)reader["ThanhBen_Giua_TNP"];
            }


            if (reader.HasColumn("ThanhBen_Giua_DobuLieuThap") && reader["ThanhBen_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhBen_Giua_DobuLieuThap = (long)reader["ThanhBen_Giua_DobuLieuThap"];
                p.V_ThanhBen_Giua_DobuLieuThap.LookupID = (long)reader["ThanhBen_Giua_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhBen_Giua_DobuLieuCao") && reader["ThanhBen_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhBen_Giua_DobuLieuCao = (long)reader["ThanhBen_Giua_DobuLieuCao"];
                p.V_ThanhBen_Giua_DobuLieuCao.LookupID = (long)reader["ThanhBen_Giua_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhBen_Giua_KetLuan") && reader["ThanhBen_Giua_KetLuan"] != DBNull.Value)
            {
                p.ThanhBen_Giua_KetLuan = (long)reader["ThanhBen_Giua_KetLuan"];
                p.V_ThanhBen_Giua_KetLuan.LookupID = (long)reader["ThanhBen_Giua_KetLuan"];
            }


            if (reader.HasColumn("ThanhBen_Day_TNP") && reader["ThanhBen_Day_TNP"] != DBNull.Value)
            {
                p.ThanhBen_Day_TNP = (long)reader["ThanhBen_Day_TNP"];
                p.V_ThanhBen_Day_TNP.LookupID = (long)reader["ThanhBen_Day_TNP"];
            }


            if (reader.HasColumn("ThanhBen_Day_DobuLieuThap") && reader["ThanhBen_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhBen_Day_DobuLieuThap = (long)reader["ThanhBen_Day_DobuLieuThap"];
                p.V_ThanhBen_Day_DobuLieuThap.LookupID = (long)reader["ThanhBen_Day_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhBen_Day_DobuLieuCao") && reader["ThanhBen_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhBen_Day_DobuLieuCao = (long)reader["ThanhBen_Day_DobuLieuCao"];
                p.V_ThanhBen_Day_DobuLieuCao.LookupID = (long)reader["ThanhBen_Day_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhBen_Day_KetLuan") && reader["ThanhBen_Day_KetLuan"] != DBNull.Value)
            {
                p.ThanhBen_Day_KetLuan = (long)reader["ThanhBen_Day_KetLuan"];
                p.V_ThanhBen_Day_KetLuan.LookupID = (long)reader["ThanhBen_Day_KetLuan"];
            }


            if (reader.HasColumn("TruocVach_Mom_TNP") && reader["TruocVach_Mom_TNP"] != DBNull.Value)
            {
                p.TruocVach_Mom_TNP = (long)reader["TruocVach_Mom_TNP"];
                p.V_TruocVach_Mom_TNP.LookupID = (long)reader["TruocVach_Mom_TNP"];
            }


            if (reader.HasColumn("TruocVach_Mom_DobuLieuThap") && reader["TruocVach_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.TruocVach_Mom_DobuLieuThap = (long)reader["TruocVach_Mom_DobuLieuThap"];
                p.V_TruocVach_Mom_DobuLieuThap.LookupID = (long)reader["TruocVach_Mom_DobuLieuThap"];
            }


            if (reader.HasColumn("TruocVach_Mom_DobuLieuCao") && reader["TruocVach_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.TruocVach_Mom_DobuLieuCao = (long)reader["TruocVach_Mom_DobuLieuCao"];
                p.V_TruocVach_Mom_DobuLieuCao.LookupID = (long)reader["TruocVach_Mom_DobuLieuCao"];
            }


            if (reader.HasColumn("TruocVach_Mom_KetLuan") && reader["TruocVach_Mom_KetLuan"] != DBNull.Value)
            {
                p.TruocVach_Mom_KetLuan = (long)reader["TruocVach_Mom_KetLuan"];
                p.V_TruocVach_Mom_KetLuan.LookupID = (long)reader["TruocVach_Mom_KetLuan"];
            }


            if (reader.HasColumn("TruocVach_Giua_TNP") && reader["TruocVach_Giua_TNP"] != DBNull.Value)
            {
                p.TruocVach_Giua_TNP = (long)reader["TruocVach_Giua_TNP"];
                p.V_TruocVach_Giua_TNP.LookupID = (long)reader["TruocVach_Giua_TNP"];
            }


            if (reader.HasColumn("TruocVach_Giua_DobuLieuThap") && reader["TruocVach_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.TruocVach_Giua_DobuLieuThap = (long)reader["TruocVach_Giua_DobuLieuThap"];
                p.V_TruocVach_Giua_DobuLieuThap.LookupID = (long)reader["TruocVach_Giua_DobuLieuThap"];
            }


            if (reader.HasColumn("TruocVach_Giua_DobuLieuCao") && reader["TruocVach_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.TruocVach_Giua_DobuLieuCao = (long)reader["TruocVach_Giua_DobuLieuCao"];
                p.V_TruocVach_Giua_DobuLieuCao.LookupID = (long)reader["TruocVach_Giua_DobuLieuCao"];
            }


            if (reader.HasColumn("TruocVach_Giua_KetLuan") && reader["TruocVach_Giua_KetLuan"] != DBNull.Value)
            {
                p.TruocVach_Giua_KetLuan = (long)reader["TruocVach_Giua_KetLuan"];
                p.V_TruocVach_Giua_KetLuan.LookupID = (long)reader["TruocVach_Giua_KetLuan"];
            }


            if (reader.HasColumn("TruocVach_Day_TNP") && reader["TruocVach_Day_TNP"] != DBNull.Value)
            {
                p.TruocVach_Day_TNP = (long)reader["TruocVach_Day_TNP"];
                p.V_TruocVach_Day_TNP.LookupID = (long)reader["TruocVach_Day_TNP"];
            }


            if (reader.HasColumn("TruocVach_Day_DobuLieuThap") && reader["TruocVach_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.TruocVach_Day_DobuLieuThap = (long)reader["TruocVach_Day_DobuLieuThap"];
                p.V_TruocVach_Day_DobuLieuThap.LookupID = (long)reader["TruocVach_Day_DobuLieuThap"];
            }


            if (reader.HasColumn("TruocVach_Day_DobuLieuCao") && reader["TruocVach_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.TruocVach_Day_DobuLieuCao = (long)reader["TruocVach_Day_DobuLieuCao"];
                p.V_TruocVach_Day_DobuLieuCao.LookupID = (long)reader["TruocVach_Day_DobuLieuCao"];
            }


            if (reader.HasColumn("TruocVach_Day_KetLuan") && reader["TruocVach_Day_KetLuan"] != DBNull.Value)
            {
                p.TruocVach_Day_KetLuan = (long)reader["TruocVach_Day_KetLuan"];
                p.V_TruocVach_Day_KetLuan.LookupID = (long)reader["TruocVach_Day_KetLuan"];
            }






            #endregion
            return p;
        }
        public virtual List<URP_FE_StressDipyridamoleResult> GetURP_FE_StressDipyridamoleResultCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_StressDipyridamoleResult> p = new List<URP_FE_StressDipyridamoleResult>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_StressDipyridamoleResultFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_StressDobutamine GetURP_FE_StressDobutamineFromReader(IDataReader reader)
        {
            URP_FE_StressDobutamine p = new URP_FE_StressDobutamine();

            if (reader.HasColumn("URP_FE_StressDobutamineID") && reader["URP_FE_StressDobutamineID"] != DBNull.Value)
            {
                p.URP_FE_StressDobutamineID = (long)reader["URP_FE_StressDobutamineID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("TruyenTinhMach") && reader["TruyenTinhMach"] != DBNull.Value)
            {
                p.TruyenTinhMach = Convert.ToBoolean(reader["TruyenTinhMach"]);
            }


            if (reader.HasColumn("TanSoTimCanDat") && reader["TanSoTimCanDat"] != DBNull.Value)
            {
                p.TanSoTimCanDat = (double)reader["TanSoTimCanDat"];
            }


            if (reader.HasColumn("TD_TNP_HuyetAp_TT") && reader["TD_TNP_HuyetAp_TT"] != DBNull.Value)
            {
                p.TD_TNP_HuyetAp_TT = (double)reader["TD_TNP_HuyetAp_TT"];
            }


            if (reader.HasColumn("TD_TNP_HuyetAp_TTr") && reader["TD_TNP_HuyetAp_TTr"] != DBNull.Value)
            {
                p.TD_TNP_HuyetAp_TTr = (double)reader["TD_TNP_HuyetAp_TTr"];
            }


            if (reader.HasColumn("TD_TNP_HuyetAp_TanSoTim") && reader["TD_TNP_HuyetAp_TanSoTim"] != DBNull.Value)
            {
                p.TD_TNP_HuyetAp_TanSoTim = (double)reader["TD_TNP_HuyetAp_TanSoTim"];
            }


            if (reader.HasColumn("TD_TNP_HuyetAp_DoChenhApMin") && reader["TD_TNP_HuyetAp_DoChenhApMin"] != DBNull.Value)
            {
                p.TD_TNP_HuyetAp_DoChenhApMin = (double)reader["TD_TNP_HuyetAp_DoChenhApMin"];
            }

            if (reader.HasColumn("TD_TNP_HuyetAp_DoChenhApMax") && reader["TD_TNP_HuyetAp_DoChenhApMax"] != DBNull.Value)
            {
                p.TD_TNP_HuyetAp_DoChenhApMax = (double)reader["TD_TNP_HuyetAp_DoChenhApMax"];
            }


            if (reader.HasColumn("FiveMicro_DungLuong") && reader["FiveMicro_DungLuong"] != DBNull.Value)
            {
                p.FiveMicro_DungLuong = (double)reader["FiveMicro_DungLuong"];
            }


            if (reader.HasColumn("FiveMicro_HuyetAp_TT") && reader["FiveMicro_HuyetAp_TT"] != DBNull.Value)
            {
                p.FiveMicro_HuyetAp_TT = (double)reader["FiveMicro_HuyetAp_TT"];
            }


            if (reader.HasColumn("FiveMicro_HuyetAp_TTr") && reader["FiveMicro_HuyetAp_TTr"] != DBNull.Value)
            {
                p.FiveMicro_HuyetAp_TTr = (double)reader["FiveMicro_HuyetAp_TTr"];
            }


            if (reader.HasColumn("FiveMicro_TanSoTim") && reader["FiveMicro_TanSoTim"] != DBNull.Value)
            {
                p.FiveMicro_TanSoTim = (double)reader["FiveMicro_TanSoTim"];
            }


            if (reader.HasColumn("FiveMicro_DoChenhApMin") && reader["FiveMicro_DoChenhApMin"] != DBNull.Value)
            {
                p.FiveMicro_DoChenhApMin = (double)reader["FiveMicro_DoChenhApMin"];
            }

            if (reader.HasColumn("FiveMicro_DoChenhApMax") && reader["FiveMicro_DoChenhApMax"] != DBNull.Value)
            {
                p.FiveMicro_DoChenhApMax = (double)reader["FiveMicro_DoChenhApMax"];
            }


            if (reader.HasColumn("TenMicro_DungLuong") && reader["TenMicro_DungLuong"] != DBNull.Value)
            {
                p.TenMicro_DungLuong = (double)reader["TenMicro_DungLuong"];
            }


            if (reader.HasColumn("TenMicro_HuyetAp_TT") && reader["TenMicro_HuyetAp_TT"] != DBNull.Value)
            {
                p.TenMicro_HuyetAp_TT = (double)reader["TenMicro_HuyetAp_TT"];
            }


            if (reader.HasColumn("TenMicro_HuyetAp_TTr") && reader["TenMicro_HuyetAp_TTr"] != DBNull.Value)
            {
                p.TenMicro_HuyetAp_TTr = (double)reader["TenMicro_HuyetAp_TTr"];
            }


            if (reader.HasColumn("TenMicro_TanSoTim") && reader["TenMicro_TanSoTim"] != DBNull.Value)
            {
                p.TenMicro_TanSoTim = (double)reader["TenMicro_TanSoTim"];
            }


            if (reader.HasColumn("TenMicro_DoChenhApMin") && reader["TenMicro_DoChenhApMin"] != DBNull.Value)
            {
                p.TenMicro_DoChenhApMin = (double)reader["TenMicro_DoChenhApMin"];
            }

            if (reader.HasColumn("TenMicro_DoChenhApMax") && reader["TenMicro_DoChenhApMax"] != DBNull.Value)
            {
                p.TenMicro_DoChenhApMax = (double)reader["TenMicro_DoChenhApMax"];
            }


            if (reader.HasColumn("TwentyMicro_DungLuong") && reader["TwentyMicro_DungLuong"] != DBNull.Value)
            {
                p.TwentyMicro_DungLuong = (double)reader["TwentyMicro_DungLuong"];
            }


            if (reader.HasColumn("TwentyMicro_HuyetAp_TT") && reader["TwentyMicro_HuyetAp_TT"] != DBNull.Value)
            {
                p.TwentyMicro_HuyetAp_TT = (double)reader["TwentyMicro_HuyetAp_TT"];
            }


            if (reader.HasColumn("TwentyMicro_HuyetAp_TTr") && reader["TwentyMicro_HuyetAp_TTr"] != DBNull.Value)
            {
                p.TwentyMicro_HuyetAp_TTr = (double)reader["TwentyMicro_HuyetAp_TTr"];
            }


            if (reader.HasColumn("TwentyMicro_TanSoTim") && reader["TwentyMicro_TanSoTim"] != DBNull.Value)
            {
                p.TwentyMicro_TanSoTim = (double)reader["TwentyMicro_TanSoTim"];
            }


            if (reader.HasColumn("TwentyMicro_DoChenhApMax") && reader["TwentyMicro_DoChenhApMax"] != DBNull.Value)
            {
                p.TwentyMicro_DoChenhApMax = (double)reader["TwentyMicro_DoChenhApMax"];
            }

            if (reader.HasColumn("TwentyMicro_DoChenhApMin") && reader["TwentyMicro_DoChenhApMin"] != DBNull.Value)
            {
                p.TwentyMicro_DoChenhApMin = (double)reader["TwentyMicro_DoChenhApMin"];
            }


            if (reader.HasColumn("ThirtyMicro_DungLuong") && reader["ThirtyMicro_DungLuong"] != DBNull.Value)
            {
                p.ThirtyMicro_DungLuong = (double)reader["ThirtyMicro_DungLuong"];
            }


            if (reader.HasColumn("ThirtyMicro_HuyetAp_TT") && reader["ThirtyMicro_HuyetAp_TT"] != DBNull.Value)
            {
                p.ThirtyMicro_HuyetAp_TT = (double)reader["ThirtyMicro_HuyetAp_TT"];
            }


            if (reader.HasColumn("ThirtyMicro_HuyetAp_TTr") && reader["ThirtyMicro_HuyetAp_TTr"] != DBNull.Value)
            {
                p.ThirtyMicro_HuyetAp_TTr = (double)reader["ThirtyMicro_HuyetAp_TTr"];
            }


            if (reader.HasColumn("ThirtyMicro_TanSoTim") && reader["ThirtyMicro_TanSoTim"] != DBNull.Value)
            {
                p.ThirtyMicro_TanSoTim = (double)reader["ThirtyMicro_TanSoTim"];
            }


            if (reader.HasColumn("ThirtyMicro_DoChenhApMin") && reader["ThirtyMicro_DoChenhApMin"] != DBNull.Value)
            {
                p.ThirtyMicro_DoChenhApMin = (double)reader["ThirtyMicro_DoChenhApMin"];
            }

            if (reader.HasColumn("ThirtyMicro_DoChenhApMax") && reader["ThirtyMicro_DoChenhApMax"] != DBNull.Value)
            {
                p.ThirtyMicro_DoChenhApMax = (double)reader["ThirtyMicro_DoChenhApMax"];
            }


            if (reader.HasColumn("FortyMicro_DungLuong") && reader["FortyMicro_DungLuong"] != DBNull.Value)
            {
                p.FortyMicro_DungLuong = (double)reader["FortyMicro_DungLuong"];
            }


            if (reader.HasColumn("FortyMicro_HuyetAp_TT") && reader["FortyMicro_HuyetAp_TT"] != DBNull.Value)
            {
                p.FortyMicro_HuyetAp_TT = (double)reader["FortyMicro_HuyetAp_TT"];
            }


            if (reader.HasColumn("FortyMicro_HuyetAp_TTr") && reader["FortyMicro_HuyetAp_TTr"] != DBNull.Value)
            {
                p.FortyMicro_HuyetAp_TTr = (double)reader["FortyMicro_HuyetAp_TTr"];
            }


            if (reader.HasColumn("FortyMicro_TanSoTim") && reader["FortyMicro_TanSoTim"] != DBNull.Value)
            {
                p.FortyMicro_TanSoTim = (double)reader["FortyMicro_TanSoTim"];
            }


            if (reader.HasColumn("FortyMicro_DoChenhApMin") && reader["FortyMicro_DoChenhApMin"] != DBNull.Value)
            {
                p.FortyMicro_DoChenhApMin = (double)reader["FortyMicro_DoChenhApMin"];
            }

            if (reader.HasColumn("FortyMicro_DoChenhApMax") && reader["FortyMicro_DoChenhApMax"] != DBNull.Value)
            {
                p.FortyMicro_DoChenhApMax = (double)reader["FortyMicro_DoChenhApMax"];
            }


            if (reader.HasColumn("Atropine_DungLuong") && reader["Atropine_DungLuong"] != DBNull.Value)
            {
                p.Atropine_DungLuong = (double)reader["Atropine_DungLuong"];
            }


            if (reader.HasColumn("Atropine_HuyetAp_TT") && reader["Atropine_HuyetAp_TT"] != DBNull.Value)
            {
                p.Atropine_HuyetAp_TT = (double)reader["Atropine_HuyetAp_TT"];
            }


            if (reader.HasColumn("Atropine_HuyetAp_TTr") && reader["Atropine_HuyetAp_TTr"] != DBNull.Value)
            {
                p.Atropine_HuyetAp_TTr = (double)reader["Atropine_HuyetAp_TTr"];
            }


            if (reader.HasColumn("Atropine_TanSoTim") && reader["Atropine_TanSoTim"] != DBNull.Value)
            {
                p.Atropine_TanSoTim = (double)reader["Atropine_TanSoTim"];
            }


            if (reader.HasColumn("Atropine_DoChenhApMin") && reader["Atropine_DoChenhApMin"] != DBNull.Value)
            {
                p.Atropine_DoChenhApMin = (double)reader["Atropine_DoChenhApMin"];
            }

            if (reader.HasColumn("Atropine_DoChenhApMax") && reader["Atropine_DoChenhApMax"] != DBNull.Value)
            {
                p.Atropine_DoChenhApMax = (double)reader["Atropine_DoChenhApMax"];
            }


            if (reader.HasColumn("NgungNP_ThoiGian") && reader["NgungNP_ThoiGian"] != DBNull.Value)
            {
                p.NgungNP_ThoiGian = (double)reader["NgungNP_ThoiGian"];
            }


            if (reader.HasColumn("NgungNP_HuyetAp_TT") && reader["NgungNP_HuyetAp_TT"] != DBNull.Value)
            {
                p.NgungNP_HuyetAp_TT = (double)reader["NgungNP_HuyetAp_TT"];
            }


            if (reader.HasColumn("NgungNP_HuyetAp_TTr") && reader["NgungNP_HuyetAp_TTr"] != DBNull.Value)
            {
                p.NgungNP_HuyetAp_TTr = (double)reader["NgungNP_HuyetAp_TTr"];
            }


            if (reader.HasColumn("NgungNP_TanSoTim") && reader["NgungNP_TanSoTim"] != DBNull.Value)
            {
                p.NgungNP_TanSoTim = (double)reader["NgungNP_TanSoTim"];
            }


            if (reader.HasColumn("NgungNP_DoChenhApMin") && reader["NgungNP_DoChenhApMin"] != DBNull.Value)
            {
                p.NgungNP_DoChenhApMin = (double)reader["NgungNP_DoChenhApMin"];
            }

            if (reader.HasColumn("NgungNP_DoChenhApMax") && reader["NgungNP_DoChenhApMax"] != DBNull.Value)
            {
                p.NgungNP_DoChenhApMax = (double)reader["NgungNP_DoChenhApMax"];
            }


            return p;
        }
        public virtual List<URP_FE_StressDobutamine> GetURP_FE_StressDobutamineCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_StressDobutamine> p = new List<URP_FE_StressDobutamine>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_StressDobutamineFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_StressDobutamineElectrocardiogram GetURP_FE_StressDobutamineElectrocardiogramFromReader(IDataReader reader)
        {
            URP_FE_StressDobutamineElectrocardiogram p = new URP_FE_StressDobutamineElectrocardiogram();

            if (reader.HasColumn("URP_FE_StressDobutamineElectrocardiogramID") && reader["URP_FE_StressDobutamineElectrocardiogramID"] != DBNull.Value)
            {
                p.URP_FE_StressDobutamineElectrocardiogramID = (long)reader["URP_FE_StressDobutamineElectrocardiogramID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("DieuTriConDauThatNguc") && reader["DieuTriConDauThatNguc"] != DBNull.Value)
            {
                p.DieuTriConDauThatNguc = (bool)reader["DieuTriConDauThatNguc"];
            }


            if (reader.HasColumn("DieuTriDIGITALIS") && reader["DieuTriDIGITALIS"] != DBNull.Value)
            {
                p.DieuTriDIGITALIS = (bool)reader["DieuTriDIGITALIS"];
            }


            if (reader.HasColumn("LyDoKhongThucHienDuoc") && reader["LyDoKhongThucHienDuoc"] != DBNull.Value)
            {
                p.LyDoKhongThucHienDuoc = (string)reader["LyDoKhongThucHienDuoc"];
            }


            if (reader.HasColumn("MucGangSuc") && reader["MucGangSuc"] != DBNull.Value)
            {
                p.MucGangSuc = (double)reader["MucGangSuc"];
            }


            if (reader.HasColumn("ThoiGianGangSuc") && reader["ThoiGianGangSuc"] != DBNull.Value)
            {
                p.ThoiGianGangSuc = (double)reader["ThoiGianGangSuc"];
            }


            if (reader.HasColumn("TanSoTim") && reader["TanSoTim"] != DBNull.Value)
            {
                p.TanSoTim = (double)reader["TanSoTim"];
            }


            if (reader.HasColumn("HuyetApToiDa") && reader["HuyetApToiDa"] != DBNull.Value)
            {
                p.HuyetApToiDa = (double)reader["HuyetApToiDa"];
            }


            if (reader.HasColumn("ConDauThatNguc") && reader["ConDauThatNguc"] != DBNull.Value)
            {
                p.ConDauThatNguc = Convert.ToInt32(reader["ConDauThatNguc"]);
            }


            if (reader.HasColumn("STChenhXuong") && reader["STChenhXuong"] != DBNull.Value)
            {
                p.STChenhXuong = (string)reader["STChenhXuong"];
            }


            if (reader.HasColumn("RoiLoanNhipTim") && reader["RoiLoanNhipTim"] != DBNull.Value)
            {
                p.RoiLoanNhipTim = (bool)reader["RoiLoanNhipTim"];
            }


            if (reader.HasColumn("RoiLoanNhipTimChiTiet") && reader["RoiLoanNhipTimChiTiet"] != DBNull.Value)
            {
                p.RoiLoanNhipTimChiTiet = (string)reader["RoiLoanNhipTimChiTiet"];
            }


            if (reader.HasColumn("XetNghiemKhac") && reader["XetNghiemKhac"] != DBNull.Value)
            {
                p.XetNghiemKhac = (string)reader["XetNghiemKhac"];
            }

            return p;
        }
        public virtual List<URP_FE_StressDobutamineElectrocardiogram> GetURP_FE_StressDobutamineElectrocardiogramCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_StressDobutamineElectrocardiogram> p = new List<URP_FE_StressDobutamineElectrocardiogram>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_StressDobutamineElectrocardiogramFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_StressDobutamineExam GetURP_FE_StressDobutamineExamFromReader(IDataReader reader)
        {
            URP_FE_StressDobutamineExam p = new URP_FE_StressDobutamineExam();

            if (reader.HasColumn("URP_FE_StressDobutamineExamID") && reader["URP_FE_StressDobutamineExamID"] != DBNull.Value)
            {
                p.URP_FE_StressDobutamineExamID = (long)reader["URP_FE_StressDobutamineExamID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("TrieuChungHienTai") && reader["TrieuChungHienTai"] != DBNull.Value)
            {
                p.TrieuChungHienTai = (string)reader["TrieuChungHienTai"];
            }


            if (reader.HasColumn("ChiDinhSATGSDobu") && reader["ChiDinhSATGSDobu"] != DBNull.Value)
            {
                p.ChiDinhSATGSDobu = (bool)reader["ChiDinhSATGSDobu"];
            }


            if (reader.HasColumn("ChiDinhDetail") && reader["ChiDinhDetail"] != DBNull.Value)
            {
                p.ChiDinhDetail = (string)reader["ChiDinhDetail"];
            }


            if (reader.HasColumn("TDDTruocNgayKham") && reader["TDDTruocNgayKham"] != DBNull.Value)
            {
                p.TDDTruocNgayKham = (string)reader["TDDTruocNgayKham"];
            }


            if (reader.HasColumn("TDDTrongNgaySATGSDobu") && reader["TDDTrongNgaySATGSDobu"] != DBNull.Value)
            {
                p.TDDTrongNgaySATGSDobu = (string)reader["TDDTrongNgaySATGSDobu"];
            }

            return p;
        }
        public virtual List<URP_FE_StressDobutamineExam> GetURP_FE_StressDobutamineExamCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_StressDobutamineExam> p = new List<URP_FE_StressDobutamineExam>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_StressDobutamineExamFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_StressDobutamineImages GetURP_FE_StressDobutamineImagesFromReader(IDataReader reader)
        {
            URP_FE_StressDobutamineImages p = new URP_FE_StressDobutamineImages();

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                long StaffID=0;
                long.TryParse(reader["DoctorStaffID"].ToString(), out StaffID);
                if (StaffID > 0)
                {
                    p.VStaff = GetStaffFromReader_Simple(reader);
                }
            }

            if (reader.HasColumn("URP_FE_StressDobutamineImagesID") && reader["URP_FE_StressDobutamineImagesID"] != DBNull.Value)
            {
                p.URP_FE_StressDobutamineImagesID = (long)reader["URP_FE_StressDobutamineImagesID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("KetLuan") && reader["KetLuan"] != DBNull.Value)
            {
                p.KetLuan = reader["KetLuan"].ToString();
            }


            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = Convert.ToDateTime(reader["CreateDate"]);
            }

            
            return p;
        }
        public virtual List<URP_FE_StressDobutamineImages> GetURP_FE_StressDobutamineImagesCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_StressDobutamineImages> p = new List<URP_FE_StressDobutamineImages>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_StressDobutamineImagesFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_StressDobutamineResult GetURP_FE_StressDobutamineResultFromReader(IDataReader reader)
        {
            URP_FE_StressDobutamineResult p = new URP_FE_StressDobutamineResult();

            if (reader.HasColumn("URP_FE_StressDobutamineResultID") && reader["URP_FE_StressDobutamineResultID"] != DBNull.Value)
            {
                p.URP_FE_StressDobutamineResultID = (long)reader["URP_FE_StressDobutamineResultID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("ThayDoiDTD") && reader["ThayDoiDTD"] != DBNull.Value)
            {
                p.ThayDoiDTD = (bool)reader["ThayDoiDTD"];
            }


            if (reader.HasColumn("ThayDoiDTDChiTiet") && reader["ThayDoiDTDChiTiet"] != DBNull.Value)
            {
                p.ThayDoiDTDChiTiet = (string)reader["ThayDoiDTDChiTiet"];
            }


            if (reader.HasColumn("RoiLoanNhip") && reader["RoiLoanNhip"] != DBNull.Value)
            {
                p.RoiLoanNhip = (bool)reader["RoiLoanNhip"];
            }


            if (reader.HasColumn("RoiLoanNhipChiTiet") && reader["RoiLoanNhipChiTiet"] != DBNull.Value)
            {
                p.RoiLoanNhipChiTiet = (string)reader["RoiLoanNhipChiTiet"];
            }


            if (reader.HasColumn("TDPHayBienChung") && reader["TDPHayBienChung"] != DBNull.Value)
            {
                p.TDPHayBienChung = Convert.ToInt32(reader["TDPHayBienChung"]);
            }


            if (reader.HasColumn("TrieuChungKhac") && reader["TrieuChungKhac"] != DBNull.Value)
            {
                p.TrieuChungKhac = (string)reader["TrieuChungKhac"];
            }


            if (reader.HasColumn("BienPhapDieuTri") && reader["BienPhapDieuTri"] != DBNull.Value)
            {
                p.BienPhapDieuTri = (string)reader["BienPhapDieuTri"];
            }


            if (reader.HasColumn("V_KetQuaSieuAmTim") && reader["V_KetQuaSieuAmTim"] != DBNull.Value)
            {
                p.V_KetQuaSieuAmTim = (long)reader["V_KetQuaSieuAmTim"];
            }


            if (reader.HasColumn("KetQuaSieuAmTim") && reader["KetQuaSieuAmTim"] != DBNull.Value)
            {
                p.KetQuaSieuAmTim = (string)reader["KetQuaSieuAmTim"];
            }
            if (reader.HasColumn("Conclusion") && reader["Conclusion"] != DBNull.Value)
            {
                p.KetQuaSieuAmTim = reader["Conclusion"].ToString();
            }


            #region V Lookup
            p.V_ThanhTruoc_Mom_TNP = new Lookup();
            

            p.V_ThanhTruoc_Mom_DobuLieuThap = new Lookup();
            p.V_ThanhTruoc_Mom_DobuLieuCao = new Lookup();
            p.V_ThanhTruoc_Mom_KetLuan = new Lookup();
            p.V_ThanhTruoc_Giua_TNP = new Lookup();
            p.V_ThanhTruoc_Giua_DobuLieuThap = new Lookup();
            p.V_ThanhTruoc_Giua_DobuLieuCao = new Lookup();
            p.V_ThanhTruoc_Giua_KetLuan = new Lookup();
            p.V_ThanhTruoc_Day_TNP = new Lookup();
            p.V_ThanhTruoc_Day_DobuLieuThap = new Lookup();
            p.V_ThanhTruoc_Day_DobuLieuCao = new Lookup();
            p.V_ThanhTruoc_Day_KetLuan = new Lookup();
            p.V_VanhLienThat_Mom_TNP = new Lookup();
            p.V_VanhLienThat_Mom_DobuLieuThap = new Lookup();
            p.V_VanhLienThat_Mom_DobuLieuCao = new Lookup();
            p.V_VanhLienThat_Mom_KetLuan = new Lookup();
            p.V_VanhLienThat_Giua_TNP = new Lookup();
            p.V_VanhLienThat_Giua_DobuLieuThap = new Lookup();
            p.V_VanhLienThat_Giua_DobuLieuCao = new Lookup();
            p.V_VanhLienThat_Giua_KetLuan = new Lookup();
            p.V_VanhLienThat_Day_TNP = new Lookup();
            p.V_VanhLienThat_Day_DobuLieuThap = new Lookup();
            p.V_VanhLienThat_Day_DobuLieuCao = new Lookup();
            p.V_VanhLienThat_Day_KetLuan = new Lookup();
            p.V_ThanhDuoi_Mom_TNP = new Lookup();
            p.V_ThanhDuoi_Mom_DobuLieuThap = new Lookup();
            p.V_ThanhDuoi_Mom_DobuLieuCao = new Lookup();
            p.V_ThanhDuoi_Mom_KetLuan = new Lookup();
            p.V_ThanhDuoi_Giua_TNP = new Lookup();
            p.V_ThanhDuoi_Giua_DobuLieuThap = new Lookup();
            p.V_ThanhDuoi_Giua_DobuLieuCao = new Lookup();
            p.V_ThanhDuoi_Giua_KetLuan = new Lookup();
            p.V_ThanhDuoi_Day_TNP = new Lookup();
            p.V_ThanhDuoi_Day_DobuLieuThap = new Lookup();
            p.V_ThanhDuoi_Day_DobuLieuCao = new Lookup();
            p.V_ThanhDuoi_Day_KetLuan = new Lookup();
            p.V_ThanhSau_Mom_TNP = new Lookup();
            p.V_ThanhSau_Mom_DobuLieuThap = new Lookup();
            p.V_ThanhSau_Mom_DobuLieuCao = new Lookup();
            p.V_ThanhSau_Mom_KetLuan = new Lookup();
            p.V_ThanhSau_Giua_TNP = new Lookup();
            p.V_ThanhSau_Giua_DobuLieuThap = new Lookup();
            p.V_ThanhSau_Giua_DobuLieuCao = new Lookup();
            p.V_ThanhSau_Giua_KetLuan = new Lookup();
            p.V_ThanhSau_Day_TNP = new Lookup();
            p.V_ThanhSau_Day_DobuLieuThap = new Lookup();
            p.V_ThanhSau_Day_DobuLieuCao = new Lookup();
            p.V_ThanhSau_Day_KetLuan = new Lookup();
            p.V_ThanhBen_Mom_TNP = new Lookup();
            p.V_ThanhBen_Mom_DobuLieuThap = new Lookup();
            p.V_ThanhBen_Mom_DobuLieuCao = new Lookup();
            p.V_ThanhBen_Mom_KetLuan = new Lookup();
            p.V_ThanhBen_Giua_TNP = new Lookup();
            p.V_ThanhBen_Giua_DobuLieuThap = new Lookup();
            p.V_ThanhBen_Giua_DobuLieuCao = new Lookup();
            p.V_ThanhBen_Giua_KetLuan = new Lookup();
            p.V_ThanhBen_Day_TNP = new Lookup();
            p.V_ThanhBen_Day_DobuLieuThap = new Lookup();
            p.V_ThanhBen_Day_DobuLieuCao = new Lookup();
            p.V_ThanhBen_Day_KetLuan = new Lookup();
            p.V_TruocVach_Mom_TNP = new Lookup();
            p.V_TruocVach_Mom_DobuLieuThap = new Lookup();
            p.V_TruocVach_Mom_DobuLieuCao = new Lookup();
            p.V_TruocVach_Mom_KetLuan = new Lookup();
            p.V_TruocVach_Giua_TNP = new Lookup();
            p.V_TruocVach_Giua_DobuLieuThap = new Lookup();
            p.V_TruocVach_Giua_DobuLieuCao = new Lookup();
            p.V_TruocVach_Giua_KetLuan = new Lookup();
            p.V_TruocVach_Day_TNP = new Lookup();
            p.V_TruocVach_Day_DobuLieuThap = new Lookup();
            p.V_TruocVach_Day_DobuLieuCao = new Lookup();
            p.V_TruocVach_Day_KetLuan = new Lookup();

            #endregion

            #region VLookup Reader

            if (reader.HasColumn("V_ThanhTruoc_Mom_TNP") && reader["V_ThanhTruoc_Mom_TNP"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Mom_TNP.ObjectValue = reader["V_ThanhTruoc_Mom_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Mom_DobuLieuThap") && reader["V_ThanhTruoc_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Mom_DobuLieuThap.ObjectValue = reader["V_ThanhTruoc_Mom_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Mom_DobuLieuCao") && reader["V_ThanhTruoc_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Mom_DobuLieuCao.ObjectValue = reader["V_ThanhTruoc_Mom_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Mom_KetLuan") && reader["V_ThanhTruoc_Mom_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Mom_KetLuan.ObjectValue = reader["V_ThanhTruoc_Mom_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Giua_TNP") && reader["V_ThanhTruoc_Giua_TNP"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Giua_TNP.ObjectValue = reader["V_ThanhTruoc_Giua_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Giua_DobuLieuThap") && reader["V_ThanhTruoc_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Giua_DobuLieuThap.ObjectValue = reader["V_ThanhTruoc_Giua_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Giua_DobuLieuCao") && reader["V_ThanhTruoc_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Giua_DobuLieuCao.ObjectValue = reader["V_ThanhTruoc_Giua_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Giua_KetLuan") && reader["V_ThanhTruoc_Giua_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Giua_KetLuan.ObjectValue = reader["V_ThanhTruoc_Giua_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Day_TNP") && reader["V_ThanhTruoc_Day_TNP"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Day_TNP.ObjectValue = reader["V_ThanhTruoc_Day_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Day_DobuLieuThap") && reader["V_ThanhTruoc_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Day_DobuLieuThap.ObjectValue = reader["V_ThanhTruoc_Day_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Day_DobuLieuCao") && reader["V_ThanhTruoc_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Day_DobuLieuCao.ObjectValue = reader["V_ThanhTruoc_Day_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhTruoc_Day_KetLuan") && reader["V_ThanhTruoc_Day_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhTruoc_Day_KetLuan.ObjectValue = reader["V_ThanhTruoc_Day_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Mom_TNP") && reader["V_VanhLienThat_Mom_TNP"] != DBNull.Value)
            {
                p.V_VanhLienThat_Mom_TNP.ObjectValue = reader["V_VanhLienThat_Mom_TNP"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Mom_DobuLieuThap") && reader["V_VanhLienThat_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.V_VanhLienThat_Mom_DobuLieuThap.ObjectValue = reader["V_VanhLienThat_Mom_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Mom_DobuLieuCao") && reader["V_VanhLienThat_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.V_VanhLienThat_Mom_DobuLieuCao.ObjectValue = reader["V_VanhLienThat_Mom_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Mom_KetLuan") && reader["V_VanhLienThat_Mom_KetLuan"] != DBNull.Value)
            {
                p.V_VanhLienThat_Mom_KetLuan.ObjectValue = reader["V_VanhLienThat_Mom_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Giua_TNP") && reader["V_VanhLienThat_Giua_TNP"] != DBNull.Value)
            {
                p.V_VanhLienThat_Giua_TNP.ObjectValue = reader["V_VanhLienThat_Giua_TNP"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Giua_DobuLieuThap") && reader["V_VanhLienThat_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.V_VanhLienThat_Giua_DobuLieuThap.ObjectValue = reader["V_VanhLienThat_Giua_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Giua_DobuLieuCao") && reader["V_VanhLienThat_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.V_VanhLienThat_Giua_DobuLieuCao.ObjectValue = reader["V_VanhLienThat_Giua_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Giua_KetLuan") && reader["V_VanhLienThat_Giua_KetLuan"] != DBNull.Value)
            {
                p.V_VanhLienThat_Giua_KetLuan.ObjectValue = reader["V_VanhLienThat_Giua_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Day_TNP") && reader["V_VanhLienThat_Day_TNP"] != DBNull.Value)
            {
                p.V_VanhLienThat_Day_TNP.ObjectValue = reader["V_VanhLienThat_Day_TNP"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Day_DobuLieuThap") && reader["V_VanhLienThat_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.V_VanhLienThat_Day_DobuLieuThap.ObjectValue = reader["V_VanhLienThat_Day_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Day_DobuLieuCao") && reader["V_VanhLienThat_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.V_VanhLienThat_Day_DobuLieuCao.ObjectValue = reader["V_VanhLienThat_Day_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_VanhLienThat_Day_KetLuan") && reader["V_VanhLienThat_Day_KetLuan"] != DBNull.Value)
            {
                p.V_VanhLienThat_Day_KetLuan.ObjectValue = reader["V_VanhLienThat_Day_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Mom_TNP") && reader["V_ThanhDuoi_Mom_TNP"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Mom_TNP.ObjectValue = reader["V_ThanhDuoi_Mom_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Mom_DobuLieuThap") && reader["V_ThanhDuoi_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Mom_DobuLieuThap.ObjectValue = reader["V_ThanhDuoi_Mom_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Mom_DobuLieuCao") && reader["V_ThanhDuoi_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Mom_DobuLieuCao.ObjectValue = reader["V_ThanhDuoi_Mom_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Mom_KetLuan") && reader["V_ThanhDuoi_Mom_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Mom_KetLuan.ObjectValue = reader["V_ThanhDuoi_Mom_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Giua_TNP") && reader["V_ThanhDuoi_Giua_TNP"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Giua_TNP.ObjectValue = reader["V_ThanhDuoi_Giua_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Giua_DobuLieuThap") && reader["V_ThanhDuoi_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Giua_DobuLieuThap.ObjectValue = reader["V_ThanhDuoi_Giua_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Giua_DobuLieuCao") && reader["V_ThanhDuoi_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Giua_DobuLieuCao.ObjectValue = reader["V_ThanhDuoi_Giua_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Giua_KetLuan") && reader["V_ThanhDuoi_Giua_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Giua_KetLuan.ObjectValue = reader["V_ThanhDuoi_Giua_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Day_TNP") && reader["V_ThanhDuoi_Day_TNP"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Day_TNP.ObjectValue = reader["V_ThanhDuoi_Day_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Day_DobuLieuThap") && reader["V_ThanhDuoi_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Day_DobuLieuThap.ObjectValue = reader["V_ThanhDuoi_Day_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Day_DobuLieuCao") && reader["V_ThanhDuoi_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Day_DobuLieuCao.ObjectValue = reader["V_ThanhDuoi_Day_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhDuoi_Day_KetLuan") && reader["V_ThanhDuoi_Day_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhDuoi_Day_KetLuan.ObjectValue = reader["V_ThanhDuoi_Day_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Mom_TNP") && reader["V_ThanhSau_Mom_TNP"] != DBNull.Value)
            {
                p.V_ThanhSau_Mom_TNP.ObjectValue = reader["V_ThanhSau_Mom_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Mom_DobuLieuThap") && reader["V_ThanhSau_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhSau_Mom_DobuLieuThap.ObjectValue = reader["V_ThanhSau_Mom_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Mom_DobuLieuCao") && reader["V_ThanhSau_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhSau_Mom_DobuLieuCao.ObjectValue = reader["V_ThanhSau_Mom_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Mom_KetLuan") && reader["V_ThanhSau_Mom_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhSau_Mom_KetLuan.ObjectValue = reader["V_ThanhSau_Mom_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Giua_TNP") && reader["V_ThanhSau_Giua_TNP"] != DBNull.Value)
            {
                p.V_ThanhSau_Giua_TNP.ObjectValue = reader["V_ThanhSau_Giua_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Giua_DobuLieuThap") && reader["V_ThanhSau_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhSau_Giua_DobuLieuThap.ObjectValue = reader["V_ThanhSau_Giua_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Giua_DobuLieuCao") && reader["V_ThanhSau_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhSau_Giua_DobuLieuCao.ObjectValue = reader["V_ThanhSau_Giua_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Giua_KetLuan") && reader["V_ThanhSau_Giua_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhSau_Giua_KetLuan.ObjectValue = reader["V_ThanhSau_Giua_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Day_TNP") && reader["V_ThanhSau_Day_TNP"] != DBNull.Value)
            {
                p.V_ThanhSau_Day_TNP.ObjectValue = reader["V_ThanhSau_Day_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Day_DobuLieuThap") && reader["V_ThanhSau_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhSau_Day_DobuLieuThap.ObjectValue = reader["V_ThanhSau_Day_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Day_DobuLieuCao") && reader["V_ThanhSau_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhSau_Day_DobuLieuCao.ObjectValue = reader["V_ThanhSau_Day_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhSau_Day_KetLuan") && reader["V_ThanhSau_Day_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhSau_Day_KetLuan.ObjectValue = reader["V_ThanhSau_Day_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Mom_TNP") && reader["V_ThanhBen_Mom_TNP"] != DBNull.Value)
            {
                p.V_ThanhBen_Mom_TNP.ObjectValue = reader["V_ThanhBen_Mom_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Mom_DobuLieuThap") && reader["V_ThanhBen_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhBen_Mom_DobuLieuThap.ObjectValue = reader["V_ThanhBen_Mom_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Mom_DobuLieuCao") && reader["V_ThanhBen_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhBen_Mom_DobuLieuCao.ObjectValue = reader["V_ThanhBen_Mom_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Mom_KetLuan") && reader["V_ThanhBen_Mom_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhBen_Mom_KetLuan.ObjectValue = reader["V_ThanhBen_Mom_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Giua_TNP") && reader["V_ThanhBen_Giua_TNP"] != DBNull.Value)
            {
                p.V_ThanhBen_Giua_TNP.ObjectValue = reader["V_ThanhBen_Giua_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Giua_DobuLieuThap") && reader["V_ThanhBen_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhBen_Giua_DobuLieuThap.ObjectValue = reader["V_ThanhBen_Giua_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Giua_DobuLieuCao") && reader["V_ThanhBen_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhBen_Giua_DobuLieuCao.ObjectValue = reader["V_ThanhBen_Giua_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Giua_KetLuan") && reader["V_ThanhBen_Giua_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhBen_Giua_KetLuan.ObjectValue = reader["V_ThanhBen_Giua_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Day_TNP") && reader["V_ThanhBen_Day_TNP"] != DBNull.Value)
            {
                p.V_ThanhBen_Day_TNP.ObjectValue = reader["V_ThanhBen_Day_TNP"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Day_DobuLieuThap") && reader["V_ThanhBen_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.V_ThanhBen_Day_DobuLieuThap.ObjectValue = reader["V_ThanhBen_Day_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Day_DobuLieuCao") && reader["V_ThanhBen_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.V_ThanhBen_Day_DobuLieuCao.ObjectValue = reader["V_ThanhBen_Day_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_ThanhBen_Day_KetLuan") && reader["V_ThanhBen_Day_KetLuan"] != DBNull.Value)
            {
                p.V_ThanhBen_Day_KetLuan.ObjectValue = reader["V_ThanhBen_Day_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Mom_TNP") && reader["V_TruocVach_Mom_TNP"] != DBNull.Value)
            {
                p.V_TruocVach_Mom_TNP.ObjectValue = reader["V_TruocVach_Mom_TNP"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Mom_DobuLieuThap") && reader["V_TruocVach_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.V_TruocVach_Mom_DobuLieuThap.ObjectValue = reader["V_TruocVach_Mom_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Mom_DobuLieuCao") && reader["V_TruocVach_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.V_TruocVach_Mom_DobuLieuCao.ObjectValue = reader["V_TruocVach_Mom_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Mom_KetLuan") && reader["V_TruocVach_Mom_KetLuan"] != DBNull.Value)
            {
                p.V_TruocVach_Mom_KetLuan.ObjectValue = reader["V_TruocVach_Mom_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Giua_TNP") && reader["V_TruocVach_Giua_TNP"] != DBNull.Value)
            {
                p.V_TruocVach_Giua_TNP.ObjectValue = reader["V_TruocVach_Giua_TNP"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Giua_DobuLieuThap") && reader["V_TruocVach_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.V_TruocVach_Giua_DobuLieuThap.ObjectValue = reader["V_TruocVach_Giua_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Giua_DobuLieuCao") && reader["V_TruocVach_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.V_TruocVach_Giua_DobuLieuCao.ObjectValue = reader["V_TruocVach_Giua_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Giua_KetLuan") && reader["V_TruocVach_Giua_KetLuan"] != DBNull.Value)
            {
                p.V_TruocVach_Giua_KetLuan.ObjectValue = reader["V_TruocVach_Giua_KetLuan"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Day_TNP") && reader["V_TruocVach_Day_TNP"] != DBNull.Value)
            {
                p.V_TruocVach_Day_TNP.ObjectValue = reader["V_TruocVach_Day_TNP"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Day_DobuLieuThap") && reader["V_TruocVach_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.V_TruocVach_Day_DobuLieuThap.ObjectValue = reader["V_TruocVach_Day_DobuLieuThap"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Day_DobuLieuCao") && reader["V_TruocVach_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.V_TruocVach_Day_DobuLieuCao.ObjectValue = reader["V_TruocVach_Day_DobuLieuCao"].ToString();
            }


            if (reader.HasColumn("V_TruocVach_Day_KetLuan") && reader["V_TruocVach_Day_KetLuan"] != DBNull.Value)
            {
                p.V_TruocVach_Day_KetLuan.ObjectValue = reader["V_TruocVach_Day_KetLuan"].ToString();
            }








            #endregion

#region primative
            if (reader.HasColumn("ThanhTruoc_Mom_TNP") && reader["ThanhTruoc_Mom_TNP"] != DBNull.Value)
            {
                p.ThanhTruoc_Mom_TNP = (long)reader["ThanhTruoc_Mom_TNP"];
                p.V_ThanhTruoc_Mom_TNP.LookupID = (long)reader["ThanhTruoc_Mom_TNP"];
            }


            if (reader.HasColumn("ThanhTruoc_Mom_DobuLieuThap") && reader["ThanhTruoc_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhTruoc_Mom_DobuLieuThap = (long)reader["ThanhTruoc_Mom_DobuLieuThap"];
                p.V_ThanhTruoc_Mom_DobuLieuThap.LookupID = (long)reader["ThanhTruoc_Mom_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhTruoc_Mom_DobuLieuCao") && reader["ThanhTruoc_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhTruoc_Mom_DobuLieuCao = (long)reader["ThanhTruoc_Mom_DobuLieuCao"];
                p.V_ThanhTruoc_Mom_DobuLieuCao.LookupID = (long)reader["ThanhTruoc_Mom_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhTruoc_Mom_KetLuan") && reader["ThanhTruoc_Mom_KetLuan"] != DBNull.Value)
            {
                p.ThanhTruoc_Mom_KetLuan = (long)reader["ThanhTruoc_Mom_KetLuan"];
                p.V_ThanhTruoc_Mom_KetLuan.LookupID = (long)reader["ThanhTruoc_Mom_KetLuan"];
            }


            if (reader.HasColumn("ThanhTruoc_Giua_TNP") && reader["ThanhTruoc_Giua_TNP"] != DBNull.Value)
            {
                p.ThanhTruoc_Giua_TNP = (long)reader["ThanhTruoc_Giua_TNP"];
                p.V_ThanhTruoc_Giua_TNP.LookupID = (long)reader["ThanhTruoc_Giua_TNP"];
            }


            if (reader.HasColumn("ThanhTruoc_Giua_DobuLieuThap") && reader["ThanhTruoc_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhTruoc_Giua_DobuLieuThap = (long)reader["ThanhTruoc_Giua_DobuLieuThap"];
                p.V_ThanhTruoc_Giua_DobuLieuThap.LookupID = (long)reader["ThanhTruoc_Giua_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhTruoc_Giua_DobuLieuCao") && reader["ThanhTruoc_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhTruoc_Giua_DobuLieuCao = (long)reader["ThanhTruoc_Giua_DobuLieuCao"];
                p.V_ThanhTruoc_Giua_DobuLieuCao.LookupID = (long)reader["ThanhTruoc_Giua_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhTruoc_Giua_KetLuan") && reader["ThanhTruoc_Giua_KetLuan"] != DBNull.Value)
            {
                p.ThanhTruoc_Giua_KetLuan = (long)reader["ThanhTruoc_Giua_KetLuan"];
                p.V_ThanhTruoc_Giua_KetLuan.LookupID = (long)reader["ThanhTruoc_Giua_KetLuan"];
            }


            if (reader.HasColumn("ThanhTruoc_Day_TNP") && reader["ThanhTruoc_Day_TNP"] != DBNull.Value)
            {
                p.ThanhTruoc_Day_TNP = (long)reader["ThanhTruoc_Day_TNP"];
                p.V_ThanhTruoc_Day_TNP.LookupID = (long)reader["ThanhTruoc_Day_TNP"];
            }


            if (reader.HasColumn("ThanhTruoc_Day_DobuLieuThap") && reader["ThanhTruoc_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhTruoc_Day_DobuLieuThap = (long)reader["ThanhTruoc_Day_DobuLieuThap"];
                p.V_ThanhTruoc_Day_DobuLieuThap.LookupID = (long)reader["ThanhTruoc_Day_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhTruoc_Day_DobuLieuCao") && reader["ThanhTruoc_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhTruoc_Day_DobuLieuCao = (long)reader["ThanhTruoc_Day_DobuLieuCao"];
                p.V_ThanhTruoc_Day_DobuLieuCao.LookupID = (long)reader["ThanhTruoc_Day_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhTruoc_Day_KetLuan") && reader["ThanhTruoc_Day_KetLuan"] != DBNull.Value)
            {
                p.ThanhTruoc_Day_KetLuan = (long)reader["ThanhTruoc_Day_KetLuan"];
                p.V_ThanhTruoc_Day_KetLuan.LookupID = (long)reader["ThanhTruoc_Day_KetLuan"];
            }


            if (reader.HasColumn("VanhLienThat_Mom_TNP") && reader["VanhLienThat_Mom_TNP"] != DBNull.Value)
            {
                p.VanhLienThat_Mom_TNP = (long)reader["VanhLienThat_Mom_TNP"];
                p.V_VanhLienThat_Mom_TNP.LookupID = (long)reader["VanhLienThat_Mom_TNP"];
            }


            if (reader.HasColumn("VanhLienThat_Mom_DobuLieuThap") && reader["VanhLienThat_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.VanhLienThat_Mom_DobuLieuThap = (long)reader["VanhLienThat_Mom_DobuLieuThap"];
                p.V_VanhLienThat_Mom_DobuLieuThap.LookupID = (long)reader["VanhLienThat_Mom_DobuLieuThap"];
            }


            if (reader.HasColumn("VanhLienThat_Mom_DobuLieuCao") && reader["VanhLienThat_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.VanhLienThat_Mom_DobuLieuCao = (long)reader["VanhLienThat_Mom_DobuLieuCao"];
                p.V_VanhLienThat_Mom_DobuLieuCao.LookupID = (long)reader["VanhLienThat_Mom_DobuLieuCao"];
            }


            if (reader.HasColumn("VanhLienThat_Mom_KetLuan") && reader["VanhLienThat_Mom_KetLuan"] != DBNull.Value)
            {
                p.VanhLienThat_Mom_KetLuan = (long)reader["VanhLienThat_Mom_KetLuan"];
                p.V_VanhLienThat_Mom_KetLuan.LookupID = (long)reader["VanhLienThat_Mom_KetLuan"];
            }


            if (reader.HasColumn("VanhLienThat_Giua_TNP") && reader["VanhLienThat_Giua_TNP"] != DBNull.Value)
            {
                p.VanhLienThat_Giua_TNP = (long)reader["VanhLienThat_Giua_TNP"];
                p.V_VanhLienThat_Giua_TNP.LookupID = (long)reader["VanhLienThat_Giua_TNP"];
            }


            if (reader.HasColumn("VanhLienThat_Giua_DobuLieuThap") && reader["VanhLienThat_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.VanhLienThat_Giua_DobuLieuThap = (long)reader["VanhLienThat_Giua_DobuLieuThap"];
                p.V_VanhLienThat_Giua_DobuLieuThap.LookupID = (long)reader["VanhLienThat_Giua_DobuLieuThap"];
            }


            if (reader.HasColumn("VanhLienThat_Giua_DobuLieuCao") && reader["VanhLienThat_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.VanhLienThat_Giua_DobuLieuCao = (long)reader["VanhLienThat_Giua_DobuLieuCao"];
                p.V_VanhLienThat_Giua_DobuLieuCao.LookupID = (long)reader["VanhLienThat_Giua_DobuLieuCao"];
            }


            if (reader.HasColumn("VanhLienThat_Giua_KetLuan") && reader["VanhLienThat_Giua_KetLuan"] != DBNull.Value)
            {
                p.VanhLienThat_Giua_KetLuan = (long)reader["VanhLienThat_Giua_KetLuan"];
                p.V_VanhLienThat_Giua_KetLuan.LookupID = (long)reader["VanhLienThat_Giua_KetLuan"];
            }


            if (reader.HasColumn("VanhLienThat_Day_TNP") && reader["VanhLienThat_Day_TNP"] != DBNull.Value)
            {
                p.VanhLienThat_Day_TNP = (long)reader["VanhLienThat_Day_TNP"];
                p.V_VanhLienThat_Day_TNP.LookupID = (long)reader["VanhLienThat_Day_TNP"];
            }


            if (reader.HasColumn("VanhLienThat_Day_DobuLieuThap") && reader["VanhLienThat_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.VanhLienThat_Day_DobuLieuThap = (long)reader["VanhLienThat_Day_DobuLieuThap"];
                p.V_VanhLienThat_Day_DobuLieuThap.LookupID = (long)reader["VanhLienThat_Day_DobuLieuThap"];
            }


            if (reader.HasColumn("VanhLienThat_Day_DobuLieuCao") && reader["VanhLienThat_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.VanhLienThat_Day_DobuLieuCao = (long)reader["VanhLienThat_Day_DobuLieuCao"];
                p.V_VanhLienThat_Day_DobuLieuCao.LookupID = (long)reader["VanhLienThat_Day_DobuLieuCao"];
            }


            if (reader.HasColumn("VanhLienThat_Day_KetLuan") && reader["VanhLienThat_Day_KetLuan"] != DBNull.Value)
            {
                p.VanhLienThat_Day_KetLuan = (long)reader["VanhLienThat_Day_KetLuan"];
                p.V_VanhLienThat_Day_KetLuan.LookupID = (long)reader["VanhLienThat_Day_KetLuan"];
            }


            if (reader.HasColumn("ThanhDuoi_Mom_TNP") && reader["ThanhDuoi_Mom_TNP"] != DBNull.Value)
            {
                p.ThanhDuoi_Mom_TNP = (long)reader["ThanhDuoi_Mom_TNP"];
                p.V_ThanhDuoi_Mom_TNP.LookupID = (long)reader["ThanhDuoi_Mom_TNP"];
            }


            if (reader.HasColumn("ThanhDuoi_Mom_DobuLieuThap") && reader["ThanhDuoi_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhDuoi_Mom_DobuLieuThap = (long)reader["ThanhDuoi_Mom_DobuLieuThap"];
                p.V_ThanhDuoi_Mom_DobuLieuThap.LookupID = (long)reader["ThanhDuoi_Mom_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhDuoi_Mom_DobuLieuCao") && reader["ThanhDuoi_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhDuoi_Mom_DobuLieuCao = (long)reader["ThanhDuoi_Mom_DobuLieuCao"];
                p.V_ThanhDuoi_Mom_DobuLieuCao.LookupID = (long)reader["ThanhDuoi_Mom_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhDuoi_Mom_KetLuan") && reader["ThanhDuoi_Mom_KetLuan"] != DBNull.Value)
            {
                p.ThanhDuoi_Mom_KetLuan = (long)reader["ThanhDuoi_Mom_KetLuan"];
                p.V_ThanhDuoi_Mom_KetLuan.LookupID = (long)reader["ThanhDuoi_Mom_KetLuan"];
            }


            if (reader.HasColumn("ThanhDuoi_Giua_TNP") && reader["ThanhDuoi_Giua_TNP"] != DBNull.Value)
            {
                p.ThanhDuoi_Giua_TNP = (long)reader["ThanhDuoi_Giua_TNP"];
                p.V_ThanhDuoi_Giua_TNP.LookupID = (long)reader["ThanhDuoi_Giua_TNP"];
            }


            if (reader.HasColumn("ThanhDuoi_Giua_DobuLieuThap") && reader["ThanhDuoi_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhDuoi_Giua_DobuLieuThap = (long)reader["ThanhDuoi_Giua_DobuLieuThap"];
                p.V_ThanhDuoi_Giua_DobuLieuThap.LookupID = (long)reader["ThanhDuoi_Giua_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhDuoi_Giua_DobuLieuCao") && reader["ThanhDuoi_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhDuoi_Giua_DobuLieuCao = (long)reader["ThanhDuoi_Giua_DobuLieuCao"];
                p.V_ThanhDuoi_Giua_DobuLieuCao.LookupID = (long)reader["ThanhDuoi_Giua_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhDuoi_Giua_KetLuan") && reader["ThanhDuoi_Giua_KetLuan"] != DBNull.Value)
            {
                p.ThanhDuoi_Giua_KetLuan = (long)reader["ThanhDuoi_Giua_KetLuan"];
                p.V_ThanhDuoi_Giua_KetLuan.LookupID = (long)reader["ThanhDuoi_Giua_KetLuan"];
            }


            if (reader.HasColumn("ThanhDuoi_Day_TNP") && reader["ThanhDuoi_Day_TNP"] != DBNull.Value)
            {
                p.ThanhDuoi_Day_TNP = (long)reader["ThanhDuoi_Day_TNP"];
                p.V_ThanhDuoi_Day_TNP.LookupID = (long)reader["ThanhDuoi_Day_TNP"];
            }


            if (reader.HasColumn("ThanhDuoi_Day_DobuLieuThap") && reader["ThanhDuoi_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhDuoi_Day_DobuLieuThap = (long)reader["ThanhDuoi_Day_DobuLieuThap"];
                p.V_ThanhDuoi_Day_DobuLieuThap.LookupID = (long)reader["ThanhDuoi_Day_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhDuoi_Day_DobuLieuCao") && reader["ThanhDuoi_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhDuoi_Day_DobuLieuCao = (long)reader["ThanhDuoi_Day_DobuLieuCao"];
                p.V_ThanhDuoi_Day_DobuLieuCao.LookupID = (long)reader["ThanhDuoi_Day_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhDuoi_Day_KetLuan") && reader["ThanhDuoi_Day_KetLuan"] != DBNull.Value)
            {
                p.ThanhDuoi_Day_KetLuan = (long)reader["ThanhDuoi_Day_KetLuan"];
                p.V_ThanhDuoi_Day_KetLuan.LookupID = (long)reader["ThanhDuoi_Day_KetLuan"];
            }


            if (reader.HasColumn("ThanhSau_Mom_TNP") && reader["ThanhSau_Mom_TNP"] != DBNull.Value)
            {
                p.ThanhSau_Mom_TNP = (long)reader["ThanhSau_Mom_TNP"];
                p.V_ThanhSau_Mom_TNP.LookupID = (long)reader["ThanhSau_Mom_TNP"];
            }


            if (reader.HasColumn("ThanhSau_Mom_DobuLieuThap") && reader["ThanhSau_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhSau_Mom_DobuLieuThap = (long)reader["ThanhSau_Mom_DobuLieuThap"];
                p.V_ThanhSau_Mom_DobuLieuThap.LookupID = (long)reader["ThanhSau_Mom_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhSau_Mom_DobuLieuCao") && reader["ThanhSau_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhSau_Mom_DobuLieuCao = (long)reader["ThanhSau_Mom_DobuLieuCao"];
                p.V_ThanhSau_Mom_DobuLieuCao.LookupID = (long)reader["ThanhSau_Mom_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhSau_Mom_KetLuan") && reader["ThanhSau_Mom_KetLuan"] != DBNull.Value)
            {
                p.ThanhSau_Mom_KetLuan = (long)reader["ThanhSau_Mom_KetLuan"];
                p.V_ThanhSau_Mom_KetLuan.LookupID = (long)reader["ThanhSau_Mom_KetLuan"];
            }


            if (reader.HasColumn("ThanhSau_Giua_TNP") && reader["ThanhSau_Giua_TNP"] != DBNull.Value)
            {
                p.ThanhSau_Giua_TNP = (long)reader["ThanhSau_Giua_TNP"];
                p.V_ThanhSau_Giua_TNP.LookupID = (long)reader["ThanhSau_Giua_TNP"];
            }


            if (reader.HasColumn("ThanhSau_Giua_DobuLieuThap") && reader["ThanhSau_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhSau_Giua_DobuLieuThap = (long)reader["ThanhSau_Giua_DobuLieuThap"];
                p.V_ThanhSau_Giua_DobuLieuThap.LookupID = (long)reader["ThanhSau_Giua_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhSau_Giua_DobuLieuCao") && reader["ThanhSau_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhSau_Giua_DobuLieuCao = (long)reader["ThanhSau_Giua_DobuLieuCao"];
                p.V_ThanhSau_Giua_DobuLieuCao.LookupID = (long)reader["ThanhSau_Giua_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhSau_Giua_KetLuan") && reader["ThanhSau_Giua_KetLuan"] != DBNull.Value)
            {
                p.ThanhSau_Giua_KetLuan = (long)reader["ThanhSau_Giua_KetLuan"];
                p.V_ThanhSau_Giua_KetLuan.LookupID = (long)reader["ThanhSau_Giua_KetLuan"];
            }


            if (reader.HasColumn("ThanhSau_Day_TNP") && reader["ThanhSau_Day_TNP"] != DBNull.Value)
            {
                p.ThanhSau_Day_TNP = (long)reader["ThanhSau_Day_TNP"];
                p.V_ThanhSau_Day_TNP.LookupID = (long)reader["ThanhSau_Day_TNP"];
            }


            if (reader.HasColumn("ThanhSau_Day_DobuLieuThap") && reader["ThanhSau_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhSau_Day_DobuLieuThap = (long)reader["ThanhSau_Day_DobuLieuThap"];
                p.V_ThanhSau_Day_DobuLieuThap.LookupID = (long)reader["ThanhSau_Day_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhSau_Day_DobuLieuCao") && reader["ThanhSau_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhSau_Day_DobuLieuCao = (long)reader["ThanhSau_Day_DobuLieuCao"];
                p.V_ThanhSau_Day_DobuLieuCao.LookupID = (long)reader["ThanhSau_Day_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhSau_Day_KetLuan") && reader["ThanhSau_Day_KetLuan"] != DBNull.Value)
            {
                p.ThanhSau_Day_KetLuan = (long)reader["ThanhSau_Day_KetLuan"];
                p.V_ThanhSau_Day_KetLuan.LookupID = (long)reader["ThanhSau_Day_KetLuan"];
            }


            if (reader.HasColumn("ThanhBen_Mom_TNP") && reader["ThanhBen_Mom_TNP"] != DBNull.Value)
            {
                p.ThanhBen_Mom_TNP = (long)reader["ThanhBen_Mom_TNP"];
                p.V_ThanhBen_Mom_TNP.LookupID = (long)reader["ThanhBen_Mom_TNP"];
            }


            if (reader.HasColumn("ThanhBen_Mom_DobuLieuThap") && reader["ThanhBen_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhBen_Mom_DobuLieuThap = (long)reader["ThanhBen_Mom_DobuLieuThap"];
                p.V_ThanhBen_Mom_DobuLieuThap.LookupID = (long)reader["ThanhBen_Mom_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhBen_Mom_DobuLieuCao") && reader["ThanhBen_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhBen_Mom_DobuLieuCao = (long)reader["ThanhBen_Mom_DobuLieuCao"];
                p.V_ThanhBen_Mom_DobuLieuCao.LookupID = (long)reader["ThanhBen_Mom_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhBen_Mom_KetLuan") && reader["ThanhBen_Mom_KetLuan"] != DBNull.Value)
            {
                p.ThanhBen_Mom_KetLuan = (long)reader["ThanhBen_Mom_KetLuan"];
                p.V_ThanhBen_Mom_KetLuan.LookupID = (long)reader["ThanhBen_Mom_KetLuan"];
            }


            if (reader.HasColumn("ThanhBen_Giua_TNP") && reader["ThanhBen_Giua_TNP"] != DBNull.Value)
            {
                p.ThanhBen_Giua_TNP = (long)reader["ThanhBen_Giua_TNP"];
                p.V_ThanhBen_Giua_TNP.LookupID = (long)reader["ThanhBen_Giua_TNP"];
            }


            if (reader.HasColumn("ThanhBen_Giua_DobuLieuThap") && reader["ThanhBen_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhBen_Giua_DobuLieuThap = (long)reader["ThanhBen_Giua_DobuLieuThap"];
                p.V_ThanhBen_Giua_DobuLieuThap.LookupID = (long)reader["ThanhBen_Giua_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhBen_Giua_DobuLieuCao") && reader["ThanhBen_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhBen_Giua_DobuLieuCao = (long)reader["ThanhBen_Giua_DobuLieuCao"];
                p.V_ThanhBen_Giua_DobuLieuCao.LookupID = (long)reader["ThanhBen_Giua_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhBen_Giua_KetLuan") && reader["ThanhBen_Giua_KetLuan"] != DBNull.Value)
            {
                p.ThanhBen_Giua_KetLuan = (long)reader["ThanhBen_Giua_KetLuan"];
                p.V_ThanhBen_Giua_KetLuan.LookupID = (long)reader["ThanhBen_Giua_KetLuan"];
            }


            if (reader.HasColumn("ThanhBen_Day_TNP") && reader["ThanhBen_Day_TNP"] != DBNull.Value)
            {
                p.ThanhBen_Day_TNP = (long)reader["ThanhBen_Day_TNP"];
                p.V_ThanhBen_Day_TNP.LookupID = (long)reader["ThanhBen_Day_TNP"];
            }


            if (reader.HasColumn("ThanhBen_Day_DobuLieuThap") && reader["ThanhBen_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.ThanhBen_Day_DobuLieuThap = (long)reader["ThanhBen_Day_DobuLieuThap"];
                p.V_ThanhBen_Day_DobuLieuThap.LookupID = (long)reader["ThanhBen_Day_DobuLieuThap"];
            }


            if (reader.HasColumn("ThanhBen_Day_DobuLieuCao") && reader["ThanhBen_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.ThanhBen_Day_DobuLieuCao = (long)reader["ThanhBen_Day_DobuLieuCao"];
                p.V_ThanhBen_Day_DobuLieuCao.LookupID = (long)reader["ThanhBen_Day_DobuLieuCao"];
            }


            if (reader.HasColumn("ThanhBen_Day_KetLuan") && reader["ThanhBen_Day_KetLuan"] != DBNull.Value)
            {
                p.ThanhBen_Day_KetLuan = (long)reader["ThanhBen_Day_KetLuan"];
                p.V_ThanhBen_Day_KetLuan.LookupID = (long)reader["ThanhBen_Day_KetLuan"];
            }


            if (reader.HasColumn("TruocVach_Mom_TNP") && reader["TruocVach_Mom_TNP"] != DBNull.Value)
            {
                p.TruocVach_Mom_TNP = (long)reader["TruocVach_Mom_TNP"];
                p.V_TruocVach_Mom_TNP.LookupID = (long)reader["TruocVach_Mom_TNP"];
            }


            if (reader.HasColumn("TruocVach_Mom_DobuLieuThap") && reader["TruocVach_Mom_DobuLieuThap"] != DBNull.Value)
            {
                p.TruocVach_Mom_DobuLieuThap = (long)reader["TruocVach_Mom_DobuLieuThap"];
                p.V_TruocVach_Mom_DobuLieuThap.LookupID = (long)reader["TruocVach_Mom_DobuLieuThap"];
            }


            if (reader.HasColumn("TruocVach_Mom_DobuLieuCao") && reader["TruocVach_Mom_DobuLieuCao"] != DBNull.Value)
            {
                p.TruocVach_Mom_DobuLieuCao = (long)reader["TruocVach_Mom_DobuLieuCao"];
                p.V_TruocVach_Mom_DobuLieuCao.LookupID = (long)reader["TruocVach_Mom_DobuLieuCao"];
            }


            if (reader.HasColumn("TruocVach_Mom_KetLuan") && reader["TruocVach_Mom_KetLuan"] != DBNull.Value)
            {
                p.TruocVach_Mom_KetLuan = (long)reader["TruocVach_Mom_KetLuan"];
                p.V_TruocVach_Mom_KetLuan.LookupID = (long)reader["TruocVach_Mom_KetLuan"];
            }


            if (reader.HasColumn("TruocVach_Giua_TNP") && reader["TruocVach_Giua_TNP"] != DBNull.Value)
            {
                p.TruocVach_Giua_TNP = (long)reader["TruocVach_Giua_TNP"];
                p.V_TruocVach_Giua_TNP.LookupID = (long)reader["TruocVach_Giua_TNP"];
            }


            if (reader.HasColumn("TruocVach_Giua_DobuLieuThap") && reader["TruocVach_Giua_DobuLieuThap"] != DBNull.Value)
            {
                p.TruocVach_Giua_DobuLieuThap = (long)reader["TruocVach_Giua_DobuLieuThap"];
                p.V_TruocVach_Giua_DobuLieuThap.LookupID = (long)reader["TruocVach_Giua_DobuLieuThap"];
            }


            if (reader.HasColumn("TruocVach_Giua_DobuLieuCao") && reader["TruocVach_Giua_DobuLieuCao"] != DBNull.Value)
            {
                p.TruocVach_Giua_DobuLieuCao = (long)reader["TruocVach_Giua_DobuLieuCao"];
                p.V_TruocVach_Giua_DobuLieuCao.LookupID = (long)reader["TruocVach_Giua_DobuLieuCao"];
            }


            if (reader.HasColumn("TruocVach_Giua_KetLuan") && reader["TruocVach_Giua_KetLuan"] != DBNull.Value)
            {
                p.TruocVach_Giua_KetLuan = (long)reader["TruocVach_Giua_KetLuan"];
                p.V_TruocVach_Giua_KetLuan.LookupID = (long)reader["TruocVach_Giua_KetLuan"];
            }


            if (reader.HasColumn("TruocVach_Day_TNP") && reader["TruocVach_Day_TNP"] != DBNull.Value)
            {
                p.TruocVach_Day_TNP = (long)reader["TruocVach_Day_TNP"];
                p.V_TruocVach_Day_TNP.LookupID = (long)reader["TruocVach_Day_TNP"];
            }


            if (reader.HasColumn("TruocVach_Day_DobuLieuThap") && reader["TruocVach_Day_DobuLieuThap"] != DBNull.Value)
            {
                p.TruocVach_Day_DobuLieuThap = (long)reader["TruocVach_Day_DobuLieuThap"];
                p.V_TruocVach_Day_DobuLieuThap.LookupID = (long)reader["TruocVach_Day_DobuLieuThap"];
            }


            if (reader.HasColumn("TruocVach_Day_DobuLieuCao") && reader["TruocVach_Day_DobuLieuCao"] != DBNull.Value)
            {
                p.TruocVach_Day_DobuLieuCao = (long)reader["TruocVach_Day_DobuLieuCao"];
                p.V_TruocVach_Day_DobuLieuCao.LookupID = (long)reader["TruocVach_Day_DobuLieuCao"];
            }


            if (reader.HasColumn("TruocVach_Day_KetLuan") && reader["TruocVach_Day_KetLuan"] != DBNull.Value)
            {
                p.TruocVach_Day_KetLuan = (long)reader["TruocVach_Day_KetLuan"];
                p.V_TruocVach_Day_KetLuan.LookupID = (long)reader["TruocVach_Day_KetLuan"];
            }






#endregion
            return p;
        }
        public virtual List<URP_FE_StressDobutamineResult> GetURP_FE_StressDobutamineResultCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_StressDobutamineResult> p = new List<URP_FE_StressDobutamineResult>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_StressDobutamineResultFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_VasculaireAnother GetURP_FE_VasculaireAnotherFromReader(IDataReader reader)
        {
            URP_FE_VasculaireAnother p = new URP_FE_VasculaireAnother();

            if (reader.HasColumn("URP_FE_VasculaireAnotherID") && reader["URP_FE_VasculaireAnotherID"] != DBNull.Value)
            {
                p.URP_FE_VasculaireAnotherID = (long)reader["URP_FE_VasculaireAnotherID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("MoTa") && reader["MoTa"] != DBNull.Value)
            {
                p.MoTa = (string)reader["MoTa"];
            }


            if (reader.HasColumn("KetLuanEx") && reader["KetLuanEx"] != DBNull.Value)
            {
                p.KetLuanEx = (string)reader["KetLuanEx"];
            }


            if (reader.HasColumn("V_MotaEx") && reader["V_MotaEx"] != DBNull.Value)
            {
                p.V_MotaEx = (long)reader["V_MotaEx"];
            }


            if (reader.HasColumn("V_KetLuanEx") && reader["V_KetLuanEx"] != DBNull.Value)
            {
                p.V_KetLuanEx = (long)reader["V_KetLuanEx"];
            }

            return p;
        }
        public virtual List<URP_FE_VasculaireAnother> GetURP_FE_VasculaireAnotherCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_VasculaireAnother> p = new List<URP_FE_VasculaireAnother>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_VasculaireAnotherFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_VasculaireAorta GetURP_FE_VasculaireAortaFromReader(IDataReader reader)
        {
            URP_FE_VasculaireAorta p = new URP_FE_VasculaireAorta();

            if (reader.HasColumn("URP_FE_VasculaireAortaID") && reader["URP_FE_VasculaireAortaID"] != DBNull.Value)
            {
                p.URP_FE_VasculaireAortaID = (long)reader["URP_FE_VasculaireAortaID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("DMCNgang") && reader["DMCNgang"] != DBNull.Value)
            {
                p.DMCNgang = (double)reader["DMCNgang"];
            }


            if (reader.HasColumn("DMCLen") && reader["DMCLen"] != DBNull.Value)
            {
                p.DMCLen = (double)reader["DMCLen"];
            }


            if (reader.HasColumn("EoDMC") && reader["EoDMC"] != DBNull.Value)
            {
                p.EoDMC = (double)reader["EoDMC"];
            }


            if (reader.HasColumn("DMCXuong") && reader["DMCXuong"] != DBNull.Value)
            {
                p.DMCXuong = (double)reader["DMCXuong"];
            }


            if (reader.HasColumn("DMThanP_v") && reader["DMThanP_v"] != DBNull.Value)
            {
                p.DMThanP_v = (double)reader["DMThanP_v"];
            }


            if (reader.HasColumn("DMThanP_RI") && reader["DMThanP_RI"] != DBNull.Value)
            {
                p.DMThanP_RI = (double)reader["DMThanP_RI"];
            }


            if (reader.HasColumn("DMThanT_v") && reader["DMThanT_v"] != DBNull.Value)
            {
                p.DMThanT_v = (double)reader["DMThanT_v"];
            }


            if (reader.HasColumn("DMThanT_RI") && reader["DMThanT_RI"] != DBNull.Value)
            {
                p.DMThanT_RI = (double)reader["DMThanT_RI"];
            }


            if (reader.HasColumn("DMChauP_v") && reader["DMChauP_v"] != DBNull.Value)
            {
                p.DMChauP_v = (double)reader["DMChauP_v"];
            }


            if (reader.HasColumn("DMChauT_v") && reader["DMChauT_v"] != DBNull.Value)
            {
                p.DMChauT_v = (double)reader["DMChauT_v"];
            }


            if (reader.HasColumn("KetLuan") && reader["KetLuan"] != DBNull.Value)
            {
                p.KetLuan = (string)reader["KetLuan"];
            }


            if (reader.HasColumn("V_KetLuan") && reader["V_KetLuan"] != DBNull.Value)
            {
                p.V_KetLuan = (long)reader["V_KetLuan"];
            }

            return p;
        }
        public virtual List<URP_FE_VasculaireAorta> GetURP_FE_VasculaireAortaCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_VasculaireAorta> p = new List<URP_FE_VasculaireAorta>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_VasculaireAortaFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_VasculaireCarotid GetURP_FE_VasculaireCarotidFromReader(IDataReader reader)
        {
            URP_FE_VasculaireCarotid p = new URP_FE_VasculaireCarotid();

            if (reader.HasColumn("URP_FE_VasculaireCarotidID") && reader["URP_FE_VasculaireCarotidID"] != DBNull.Value)
            {
                p.URP_FE_VasculaireCarotidID = (long)reader["URP_FE_VasculaireCarotidID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("V_KetQuaSAMMTruoc") && reader["V_KetQuaSAMMTruoc"] != DBNull.Value)
            {
                p.V_KetQuaSAMMTruoc = (long)reader["V_KetQuaSAMMTruoc"];
            }


            if (reader.HasColumn("KetQuaSAMMTruoc") && reader["KetQuaSAMMTruoc"] != DBNull.Value)
            {
                p.KetQuaSAMMTruoc = (string)reader["KetQuaSAMMTruoc"];
            }


            if (reader.HasColumn("DMCP_ECA") && reader["DMCP_ECA"] != DBNull.Value)
            {
                p.DMCP_ECA = (double)reader["DMCP_ECA"];
            }


            if (reader.HasColumn("DMCP_ICA") && reader["DMCP_ICA"] != DBNull.Value)
            {
                p.DMCP_ICA = (double)reader["DMCP_ICA"];
            }


            if (reader.HasColumn("DMCP_ICA_SR") && reader["DMCP_ICA_SR"] != DBNull.Value)
            {
                p.DMCP_ICA_SR = (double)reader["DMCP_ICA_SR"];
            }


            if (reader.HasColumn("DMCP_CCA_TCC") && reader["DMCP_CCA_TCC"] != DBNull.Value)
            {
                p.DMCP_CCA_TCC = (double)reader["DMCP_CCA_TCC"];
            }


            if (reader.HasColumn("DMCP_CCA") && reader["DMCP_CCA"] != DBNull.Value)
            {
                p.DMCP_CCA = (double)reader["DMCP_CCA"];
            }


            if (reader.HasColumn("DMCT_ECA") && reader["DMCT_ECA"] != DBNull.Value)
            {
                p.DMCT_ECA = (double)reader["DMCT_ECA"];
            }


            if (reader.HasColumn("DMCT_ICA") && reader["DMCT_ICA"] != DBNull.Value)
            {
                p.DMCT_ICA = (double)reader["DMCT_ICA"];
            }


            if (reader.HasColumn("DMCT_ICA_SR") && reader["DMCT_ICA_SR"] != DBNull.Value)
            {
                p.DMCT_ICA_SR = (double)reader["DMCT_ICA_SR"];
            }


            if (reader.HasColumn("DMCT_CCA_TCC") && reader["DMCT_CCA_TCC"] != DBNull.Value)
            {
                p.DMCT_CCA_TCC = (double)reader["DMCT_CCA_TCC"];
            }


            if (reader.HasColumn("DMCT_CCA") && reader["DMCT_CCA"] != DBNull.Value)
            {
                p.DMCT_CCA = (double)reader["DMCT_CCA"];
            }


            if (reader.HasColumn("DMCotSongP_d") && reader["DMCotSongP_d"] != DBNull.Value)
            {
                p.DMCotSongP_d = (double)reader["DMCotSongP_d"];
            }


            if (reader.HasColumn("DMCotSongP_r") && reader["DMCotSongP_r"] != DBNull.Value)
            {
                p.DMCotSongP_r = (double)reader["DMCotSongP_r"];
            }


            if (reader.HasColumn("DMCotSongT_d") && reader["DMCotSongT_d"] != DBNull.Value)
            {
                p.DMCotSongT_d = (double)reader["DMCotSongT_d"];
            }


            if (reader.HasColumn("DMCotSongT_r") && reader["DMCotSongT_r"] != DBNull.Value)
            {
                p.DMCotSongT_r = (double)reader["DMCotSongT_r"];
            }


            if (reader.HasColumn("DMDuoiDonP_d") && reader["DMDuoiDonP_d"] != DBNull.Value)
            {
                p.DMDuoiDonP_d = (double)reader["DMDuoiDonP_d"];
            }


            if (reader.HasColumn("DMDuoiDonP_r") && reader["DMDuoiDonP_r"] != DBNull.Value)
            {
                p.DMDuoiDonP_r = (double)reader["DMDuoiDonP_r"];
            }


            if (reader.HasColumn("DMDuoiDonT_d") && reader["DMDuoiDonT_d"] != DBNull.Value)
            {
                p.DMDuoiDonT_d = (double)reader["DMDuoiDonT_d"];
            }


            if (reader.HasColumn("DMDuoiDonT_r") && reader["DMDuoiDonT_r"] != DBNull.Value)
            {
                p.DMDuoiDonT_r = (double)reader["DMDuoiDonT_r"];
            }


            if (reader.HasColumn("KetLuan") && reader["KetLuan"] != DBNull.Value)
            {
                p.KetLuan = (string)reader["KetLuan"];
            }


            if (reader.HasColumn("V_KetLuan") && reader["V_KetLuan"] != DBNull.Value)
            {
                p.V_KetLuan = (long)reader["V_KetLuan"];
            }

            return p;
        }
        public virtual List<URP_FE_VasculaireCarotid> GetURP_FE_VasculaireCarotidCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_VasculaireCarotid> p = new List<URP_FE_VasculaireCarotid>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_VasculaireCarotidFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_VasculaireExam GetURP_FE_VasculaireExamFromReader(IDataReader reader)
        {
            URP_FE_VasculaireExam p = new URP_FE_VasculaireExam();

            if (reader.HasColumn("URP_FE_VasculaireExamID") && reader["URP_FE_VasculaireExamID"] != DBNull.Value)
            {
                p.URP_FE_VasculaireExamID = (long)reader["URP_FE_VasculaireExamID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("NoiLap") && reader["NoiLap"] != DBNull.Value)
            {
                p.NoiLap = (bool)reader["NoiLap"];
            }


            if (reader.HasColumn("ChongMat") && reader["ChongMat"] != DBNull.Value)
            {
                p.ChongMat = (bool)reader["ChongMat"];
            }


            if (reader.HasColumn("DotQuy") && reader["DotQuy"] != DBNull.Value)
            {
                p.DotQuy = (bool)reader["DotQuy"];
            }


            if (reader.HasColumn("GiamTriNho") && reader["GiamTriNho"] != DBNull.Value)
            {
                p.GiamTriNho = (bool)reader["GiamTriNho"];
            }


            if (reader.HasColumn("ThoangMu") && reader["ThoangMu"] != DBNull.Value)
            {
                p.ThoangMu = (bool)reader["ThoangMu"];
            }


            if (reader.HasColumn("NhinMo") && reader["NhinMo"] != DBNull.Value)
            {
                p.NhinMo = (bool)reader["NhinMo"];
            }


            if (reader.HasColumn("LietNuaNguoi") && reader["LietNuaNguoi"] != DBNull.Value)
            {
                p.LietNuaNguoi = (bool)reader["LietNuaNguoi"];
            }


            if (reader.HasColumn("TeYeuChanTay") && reader["TeYeuChanTay"] != DBNull.Value)
            {
                p.TeYeuChanTay = (bool)reader["TeYeuChanTay"];
            }


            if (reader.HasColumn("DaPThuatDMC") && reader["DaPThuatDMC"] != DBNull.Value)
            {
                p.DaPThuatDMC = (bool)reader["DaPThuatDMC"];
            }
            return p;
        }
        public virtual List<URP_FE_VasculaireExam> GetURP_FE_VasculaireExamCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_VasculaireExam> p = new List<URP_FE_VasculaireExam>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_VasculaireExamFromReader(reader));
            }
            return p;
        }

        protected virtual URP_FE_VasculaireLeg GetURP_FE_VasculaireLegFromReader(IDataReader reader)
        {
            URP_FE_VasculaireLeg p = new URP_FE_VasculaireLeg();

            if (reader.HasColumn("URP_FE_VasculaireLegID") && reader["URP_FE_VasculaireLegID"] != DBNull.Value)
            {
                p.URP_FE_VasculaireLegID = (long)reader["URP_FE_VasculaireLegID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            p.VStaff = new Staff();
            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.VStaff.StaffID = (long)reader["DoctorStaffID"];
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.VStaff.FullName = reader["FullName"].ToString();
            }

            if (reader.HasColumn("CreateDate") && reader["CreateDate"] != DBNull.Value)
            {
                p.CreateDate = (DateTime)reader["CreateDate"];
            }


            if (reader.HasColumn("CT_EI_P") && reader["CT_EI_P"] != DBNull.Value)
            {
                p.CT_EI_P = (double)reader["CT_EI_P"];
            }


            if (reader.HasColumn("CT_EI_T") && reader["CT_EI_T"] != DBNull.Value)
            {
                p.CT_EI_T = (double)reader["CT_EI_T"];
            }


            if (reader.HasColumn("CT_CF_P") && reader["CT_CF_P"] != DBNull.Value)
            {
                p.CT_CF_P = (double)reader["CT_CF_P"];
            }


            if (reader.HasColumn("CT_CF_T") && reader["CT_CF_T"] != DBNull.Value)
            {
                p.CT_CF_T = (double)reader["CT_CF_T"];
            }


            if (reader.HasColumn("CT_SF_P") && reader["CT_SF_P"] != DBNull.Value)
            {
                p.CT_SF_P = (double)reader["CT_SF_P"];
            }


            if (reader.HasColumn("CT_SF_T") && reader["CT_SF_T"] != DBNull.Value)
            {
                p.CT_SF_T = (double)reader["CT_SF_T"];
            }


            if (reader.HasColumn("CT_POP_P") && reader["CT_POP_P"] != DBNull.Value)
            {
                p.CT_POP_P = (double)reader["CT_POP_P"];
            }


            if (reader.HasColumn("CT_POP_T") && reader["CT_POP_T"] != DBNull.Value)
            {
                p.CT_POP_T = (double)reader["CT_POP_T"];
            }


            if (reader.HasColumn("CT_AT_P") && reader["CT_AT_P"] != DBNull.Value)
            {
                p.CT_AT_P = (double)reader["CT_AT_P"];
            }


            if (reader.HasColumn("CT_AT_T") && reader["CT_AT_T"] != DBNull.Value)
            {
                p.CT_AT_T = (double)reader["CT_AT_T"];
            }


            if (reader.HasColumn("CT_PER_P") && reader["CT_PER_P"] != DBNull.Value)
            {
                p.CT_PER_P = (double)reader["CT_PER_P"];
            }


            if (reader.HasColumn("CT_PER_T") && reader["CT_PER_T"] != DBNull.Value)
            {
                p.CT_PER_T = (double)reader["CT_PER_T"];
            }


            if (reader.HasColumn("CT_GrSaph_P") && reader["CT_GrSaph_P"] != DBNull.Value)
            {
                p.CT_GrSaph_P = (double)reader["CT_GrSaph_P"];
            }


            if (reader.HasColumn("CT_GrSaph_T") && reader["CT_GrSaph_T"] != DBNull.Value)
            {
                p.CT_GrSaph_T = (double)reader["CT_GrSaph_T"];
            }


            if (reader.HasColumn("CT_PT_P") && reader["CT_PT_P"] != DBNull.Value)
            {
                p.CT_PT_P = (double)reader["CT_PT_P"];
            }


            if (reader.HasColumn("CT_PT_T") && reader["CT_PT_T"] != DBNull.Value)
            {
                p.CT_PT_T = (double)reader["CT_PT_T"];
            }


            if (reader.HasColumn("CD_EI_P") && reader["CD_EI_P"] != DBNull.Value)
            {
                p.CD_EI_P = (double)reader["CD_EI_P"];
            }


            if (reader.HasColumn("CD_EI_T") && reader["CD_EI_T"] != DBNull.Value)
            {
                p.CD_EI_T = (double)reader["CD_EI_T"];
            }


            if (reader.HasColumn("CD_CF_P") && reader["CD_CF_P"] != DBNull.Value)
            {
                p.CD_CF_P = (double)reader["CD_CF_P"];
            }


            if (reader.HasColumn("CD_CF_T") && reader["CD_CF_T"] != DBNull.Value)
            {
                p.CD_CF_T = (double)reader["CD_CF_T"];
            }


            if (reader.HasColumn("CD_SF_P") && reader["CD_SF_P"] != DBNull.Value)
            {
                p.CD_SF_P = (double)reader["CD_SF_P"];
            }


            if (reader.HasColumn("CD_SF_T") && reader["CD_SF_T"] != DBNull.Value)
            {
                p.CD_SF_T = (double)reader["CD_SF_T"];
            }


            if (reader.HasColumn("CD_POP_P") && reader["CD_POP_P"] != DBNull.Value)
            {
                p.CD_POP_P = (double)reader["CD_POP_P"];
            }


            if (reader.HasColumn("CD_POP_T") && reader["CD_POP_T"] != DBNull.Value)
            {
                p.CD_POP_T = (double)reader["CD_POP_T"];
            }


            if (reader.HasColumn("CD_AT_P") && reader["CD_AT_P"] != DBNull.Value)
            {
                p.CD_AT_P = (double)reader["CD_AT_P"];
            }


            if (reader.HasColumn("CD_AT_T") && reader["CD_AT_T"] != DBNull.Value)
            {
                p.CD_AT_T = (double)reader["CD_AT_T"];
            }


            if (reader.HasColumn("CD_PER_P") && reader["CD_PER_P"] != DBNull.Value)
            {
                p.CD_PER_P = (double)reader["CD_PER_P"];
            }


            if (reader.HasColumn("CD_PER_T") && reader["CD_PER_T"] != DBNull.Value)
            {
                p.CD_PER_T = (double)reader["CD_PER_T"];
            }


            if (reader.HasColumn("CD_GrSaph_P") && reader["CD_GrSaph_P"] != DBNull.Value)
            {
                p.CD_GrSaph_P = (double)reader["CD_GrSaph_P"];
            }


            if (reader.HasColumn("CD_GrSaph_T") && reader["CD_GrSaph_T"] != DBNull.Value)
            {
                p.CD_GrSaph_T = (double)reader["CD_GrSaph_T"];
            }


            if (reader.HasColumn("CD_PT_P") && reader["CD_PT_P"] != DBNull.Value)
            {
                p.CD_PT_P = (double)reader["CD_PT_P"];
            }


            if (reader.HasColumn("CD_PT_T") && reader["CD_PT_T"] != DBNull.Value)
            {
                p.CD_PT_T = (double)reader["CD_PT_T"];
            }


            if (reader.HasColumn("KetLuan") && reader["KetLuan"] != DBNull.Value)
            {
                p.KetLuan = (string)reader["KetLuan"];
            }


            if (reader.HasColumn("V_KetLuan") && reader["V_KetLuan"] != DBNull.Value)
            {
                p.V_KetLuan = (long)reader["V_KetLuan"];
            }

            return p;
        }
        public virtual List<URP_FE_VasculaireLeg> GetURP_FE_VasculaireLegCollectionFromReader(IDataReader reader)
        {
            List<URP_FE_VasculaireLeg> p = new List<URP_FE_VasculaireLeg>();
            while (reader.Read())
            {
                p.Add(GetURP_FE_VasculaireLegFromReader(reader));
            }
            return p;
        }

        #region PCLExamParamResult

        public abstract List<PCLExamParamResult> GetPCLExamParamResultList(long PCLExamResultID, long ParamEnum);
        public abstract PCLExamParamResult GetPCLExamParamResult(long PCLExamResultTemplateID, long ParamEnum);

        public abstract bool AddPCLExamParamResult(PCLExamParamResult entity);

        public abstract bool UpdatePCLExamParamResult(PCLExamParamResult entity);

        public abstract bool DeletePCLExamParamResult(PCLExamParamResult entity);

        #endregion

        #region PCLExamResultTemplate

        public abstract List<PCLExamResultTemplate> GetPCLExamResultTemplateList(long PCLExamGroupTemplateResultID);

        public abstract List<PCLExamResultTemplate> GetPCLExamResultTemplateListByTypeID(
            long PCLExamGroupTemplateResultID, int ParamEnum);

        public abstract PCLExamResultTemplate GetPCLExamResultTemplate(long PCLExamResultTemplateID);

        public abstract bool AddPCLExamResultTemplate(PCLExamResultTemplate entity);

        public abstract bool UpdatePCLExamResultTemplate(PCLExamResultTemplate entity);

        public abstract bool DeletePCLExamResultTemplate(PCLExamResultTemplate entity);

        #endregion

        public abstract List<Resources> GetResourcesForMedicalServicesListByTypeID(long PCLResultParamImpID);



        #region UltraResParams_EchoCardiography

        public abstract UltraResParams_EchoCardiography GetUltraResParams_EchoCardiography(
            long UltraResParams_EchoCardiographyID, long PCLImgResultID);

        public abstract string GetUltraResParams_EchoCardiographyResult(long UltraResParams_EchoCardiographyID,
                                                                        long PCLImgResultID);

        public abstract bool AddUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity);

        public abstract bool UpdateUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity);

        public abstract bool DeleteUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity);

        #endregion

        #region UltraResParams_FetalEchocardiography2D
        //==== 20161214 CMN Begin: Add general inf
        public abstract UltraResParams_FetalEchocardiography GetUltraResParams_FetalEchocardiography(long PatientPCLReqID);
        //==== 20161214 CMN End.
        //public abstract IList<UltraResParams_FetalEchocardiography2D> GetUltraResParams_FetalEchocardiography2D(
        //    long UltraResParams_FetalEchocardiography2DID, long PCLImgResultID);
        public abstract UltraResParams_FetalEchocardiography2D GetUltraResParams_FetalEchocardiography2D(
            long UltraResParams_FetalEchocardiography2DID, long PCLImgResultID);

        public abstract bool AddUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity);

        public abstract bool UpdateUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity);

        public abstract bool DeleteUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity);
        
        #endregion

        #region UltraResParams_FetalEchocardiography Doppler

        public abstract IList<UltraResParams_FetalEchocardiographyDoppler>
            GetUltraResParams_FetalEchocardiographyDoppler(long UltraResParams_FetalEchocardiographyDopplerID);

        public abstract UltraResParams_FetalEchocardiographyDoppler
            GetUltraResParams_FetalEchocardiographyDopplerByID(long UltraResParams_FetalEchocardiographyDopplerID
            , long PCLImgResultID);

        public abstract bool AddUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity);

        public abstract bool UpdateUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity);

        public abstract bool DeleteUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity);
        
        #endregion

        #region UltraResParams_FetalEchocardiographyPostpartum

        public abstract IList<UltraResParams_FetalEchocardiographyPostpartum>
            GetUltraResParams_FetalEchocardiographyPostpartum(long UltraResParams_FetalEchocardiographyPostpartumID);
        
        public abstract UltraResParams_FetalEchocardiographyPostpartum GetUltraResParams_FetalEchocardiographyPostpartumByID(
            long UltraResParams_FetalEchocardiographyPostpartumID,long PCLImgResultID);
        public abstract bool AddUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity);

        public abstract bool UpdateUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity);

        public abstract bool DeleteUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity);
        
        #endregion

        #region UltraResParams_FetalEchocardiographyResult

        public abstract IList<UltraResParams_FetalEchocardiographyResult> GetUltraResParams_FetalEchocardiographyResult(
            long UltraResParams_FetalEchocardiographyResultID);
        
        public abstract UltraResParams_FetalEchocardiographyResult GetUltraResParams_FetalEchocardiographyResultByID(
            long UltraResParams_FetalEchocardiographyResultID,long PCLImgResultID);

        public abstract bool AddUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity);

        public abstract bool UpdateUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity);

        public abstract bool DeleteUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity);

        #endregion

        #region Abdominal Ultrasound


        public abstract bool InsertAbdominalUltrasoundResult(AbdominalUltrasound entity);

        public abstract bool UpdateAbdominalUltrasoundResult(AbdominalUltrasound entity);

        public abstract AbdominalUltrasound GetAbdominalUltrasoundResult(long PatientPCLReqID);

        #endregion

        #region URP_FE_Exam

        public abstract URP_FE_Exam GetURP_FE_Exam(
            long URP_FE_ExamID, long PCLImgResultID);

        public abstract bool AddURP_FE_Exam(URP_FE_Exam entity);

        public abstract bool UpdateURP_FE_Exam(URP_FE_Exam entity);

        public abstract bool DeleteURP_FE_Exam(URP_FE_Exam entity);

        #endregion

        #region URP_FE_Oesophagienne

        public abstract URP_FE_Oesophagienne GetURP_FE_Oesophagienne(
            long URP_FE_OesophagienneID, long PCLImgResultID);

        public abstract bool AddURP_FE_Oesophagienne(URP_FE_Oesophagienne entity);

        public abstract bool UpdateURP_FE_Oesophagienne(URP_FE_Oesophagienne entity);

        public abstract bool DeleteURP_FE_Oesophagienne(URP_FE_Oesophagienne entity);

        #endregion

        #region URP_FE_OesophagienneCheck

        public abstract URP_FE_OesophagienneCheck GetURP_FE_OesophagienneCheck(
            long URP_FE_OesophagienneCheckID, long PCLImgResultID);

        public abstract bool AddURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity);

        public abstract bool UpdateURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity);

        public abstract bool DeleteURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity);

        #endregion

        #region URP_FE_OesophagienneDiagnosis

        public abstract URP_FE_OesophagienneDiagnosis GetURP_FE_OesophagienneDiagnosis(
            long URP_FE_OesophagienneDiagnosisID, long PCLImgResultID);

        public abstract bool AddURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity);

        public abstract bool UpdateURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity);

        public abstract bool DeleteURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity);

        #endregion

        #region URP_FE_StressDipyridamole

        public abstract URP_FE_StressDipyridamole GetURP_FE_StressDipyridamole(
            long URP_FE_StressDipyridamoleID, long PCLImgResultID);

        public abstract bool AddURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity);

        public abstract bool UpdateURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity);

        public abstract bool DeleteURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity);

        #endregion

        #region URP_FE_StressDipyridamoleElectrocardiogram

        public abstract URP_FE_StressDipyridamoleElectrocardiogram GetURP_FE_StressDipyridamoleElectrocardiogram(
            long URP_FE_StressDipyridamoleElectrocardiogramID, long PCLImgResultID);

        public abstract bool AddURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity);

        public abstract bool UpdateURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity);

        public abstract bool DeleteURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity);

        #endregion

        #region URP_FE_StressDipyridamoleExam

        public abstract URP_FE_StressDipyridamoleExam GetURP_FE_StressDipyridamoleExam(
            long URP_FE_StressDipyridamoleExamID, long PCLImgResultID);

        public abstract bool AddURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity);

        public abstract bool UpdateURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity);

        public abstract bool DeleteURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity);

        #endregion

        #region URP_FE_StressDipyridamoleImage

        public abstract URP_FE_StressDipyridamoleImage GetURP_FE_StressDipyridamoleImage(
            long URP_FE_StressDipyridamoleImageID, long PCLImgResultID);

        public abstract bool AddURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity);

        public abstract bool UpdateURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity);

        public abstract bool DeleteURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity);

        #endregion

        #region URP_FE_StressDobutamineResult

        public abstract URP_FE_StressDipyridamoleResult GetURP_FE_StressDipyridamoleResult(
            long URP_FE_StressDipyridamoleResultID, long PCLImgResultID);

        public abstract bool AddURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity);

        public abstract bool UpdateURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity);

        public abstract bool DeleteURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity);

        #endregion

        #region URP_FE_StressDobutamine

        public abstract URP_FE_StressDobutamine GetURP_FE_StressDobutamine(
            long URP_FE_StressDobutamineID, long PCLImgResultID);

        public abstract bool AddURP_FE_StressDobutamine(URP_FE_StressDobutamine entity);

        public abstract bool UpdateURP_FE_StressDobutamine(URP_FE_StressDobutamine entity);

        public abstract bool DeleteURP_FE_StressDobutamine(URP_FE_StressDobutamine entity);

        #endregion

        #region URP_FE_StressDobutamineElectrocardiogram

        public abstract URP_FE_StressDobutamineElectrocardiogram GetURP_FE_StressDobutamineElectrocardiogram(
            long URP_FE_StressDobutamineElectrocardiogramID, long PCLImgResultID);

        public abstract bool AddURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity);

        public abstract bool UpdateURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity);

        public abstract bool DeleteURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity);

        #endregion

        #region URP_FE_StressDobutamineExam

        public abstract URP_FE_StressDobutamineExam GetURP_FE_StressDobutamineExam(
            long URP_FE_StressDobutamineExamID, long PCLImgResultID);

        public abstract bool AddURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity);

        public abstract bool UpdateURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity);

        public abstract bool DeleteURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity);

        #endregion

        #region URP_FE_StressDobutamineImages

        public abstract URP_FE_StressDobutamineImages GetURP_FE_StressDobutamineImages(
            long URP_FE_StressDobutamineImagesID, long PCLImgResultID);

        public abstract bool AddURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity);

        public abstract bool UpdateURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity);

        public abstract bool DeleteURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity);

        #endregion

        #region URP_FE_StressDobutamineResult

        public abstract URP_FE_StressDobutamineResult GetURP_FE_StressDobutamineResult(
            long URP_FE_StressDobutamineResultID, long PCLImgResultID);

        public abstract bool AddURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity);

        public abstract bool UpdateURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity);

        public abstract bool DeleteURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity);

        #endregion

        #region URP_FE_VasculaireAnother

        public abstract URP_FE_VasculaireAnother GetURP_FE_VasculaireAnother(
            long URP_FE_VasculaireAnotherID, long PCLImgResultID);

        public abstract bool AddURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity);

        public abstract bool UpdateURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity);

        public abstract bool DeleteURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity);

        #endregion

        #region URP_FE_VasculaireAorta

        public abstract URP_FE_VasculaireAorta GetURP_FE_VasculaireAorta(
            long URP_FE_VasculaireAortaID, long PCLImgResultID);

        public abstract bool AddURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity);

        public abstract bool UpdateURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity);

        public abstract bool DeleteURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity);

        #endregion

        #region URP_FE_VasculaireCarotid

        public abstract URP_FE_VasculaireCarotid GetURP_FE_VasculaireCarotid(
            long URP_FE_VasculaireCarotidID, long PCLImgResultID);

        public abstract bool AddURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity);

        public abstract bool UpdateURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity);

        public abstract bool DeleteURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity);

        #endregion

        #region URP_FE_VasculaireExam

        public abstract URP_FE_VasculaireExam GetURP_FE_VasculaireExam(
            long URP_FE_VasculaireExamID, long PCLImgResultID);

        public abstract bool AddURP_FE_VasculaireExam(URP_FE_VasculaireExam entity);

        public abstract bool UpdateURP_FE_VasculaireExam(URP_FE_VasculaireExam entity);

        public abstract bool DeleteURP_FE_VasculaireExam(URP_FE_VasculaireExam entity);

        #endregion

        #region URP_FE_VasculaireLeg

        public abstract URP_FE_VasculaireLeg GetURP_FE_VasculaireLeg(
            long URP_FE_VasculaireLegID, long PCLImgResultID);

        public abstract bool AddURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity);

        public abstract bool UpdateURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity);

        public abstract bool DeleteURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity);

        #endregion

        //==== 20161129 CMN Bein: Add button save for all FE pages
        public abstract bool AddAndUpdateFE_Vasculaire(URP_FE_Vasculaire entity);
        public abstract bool AddAndUpdateUltraResParams_FetalEchocardiography(UltraResParams_FetalEchocardiography entity);
        public abstract bool AddAndUpdateURP_FE_Oesophagienne(URP_FE_OesophagienneUltra entity);
        public abstract bool AddAndUpdateURP_FE_StressDipyridamole(URP_FE_StressDipyridamoleUltra entity);
        public abstract bool AddAndUpdateURP_FE_StressDobutamine(URP_FE_StressDobutamineUltra entity);
        //==== 20161129 CMN End: Add button save for all FE pages
        #endregion
    }
}