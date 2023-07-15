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
using eHCMS.DAL;
using System.Collections.ObjectModel;
using System.Collections;
using System.IO;
#region
/*******************************************************************
 * Author: NhanLe
 * Modified date: 2011-01-06
 * Contents: Consultation Services
/*******************************************************************/
#endregion
/*
 * 20170608 #001 CMN: Added PatientType to Update PCL
 * 20180613 #002 TBLD: Get HIRepResourceCode for GetResourcesForMedicalServicesListByTypeID
 * 20210527 #001 TNHX: 317 Thêm mã máy xét nghiệm con
 * 20210705 #002 BLQ: 330 + 381
 * 20230401 #005 QTD: Lấy mã máy theo khoa và nhóm DVKT
 * 20230419 #006 QTD: Thêm người thực hiện XN
 * 20230518 #007 DatTB: Thêm các trường lưu bệnh phẩm xét nghiệm
 * 20230520 #008 DatTB: Thêm cột tên tiếng anh các danh mục xét nghiệm
 * 20230626 #009 QTD: Lấy danh sách hẹn SMS XN
*/
namespace aEMR.DataAccessLayer.Providers
{
    public class PCLsProvider : DataProviderBase
    {
        static private PCLsProvider _instance = null;
        static public PCLsProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PCLsProvider();
                }
                return _instance;
            }
        }
        public PCLsProvider()
        {
            this.ConnectionString = Globals.Settings.Consultations.ConnectionString;
        }

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
                p.V_PCLExamTypeTestItems.V_PCLExamTestItem.PCLExamTestItemUnit = reader["PCLExamTestItemUnit"].ToString();
                p.PCLExamTestItemUnit = reader["PCLExamTestItemUnit"].ToString();
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
                //▼====: #003
                if (reader.HasColumn("IsAllowEditAfterDischarge") && reader["IsAllowEditAfterDischarge"] != DBNull.Value)
                {
                    p.PCLExamType.IsAllowEditAfterDischarge = Convert.ToBoolean(reader["IsAllowEditAfterDischarge"]);
                }
                if (reader.HasColumn("DateAllowEditAfterDischarge") && reader["DateAllowEditAfterDischarge"] != DBNull.Value)
                {
                    p.PCLExamType.DateAllowEditAfterDischarge = Convert.ToInt16(reader["DateAllowEditAfterDischarge"]);
                }
                //▲====: #003
            }
            p.PatientPCLLaboratoryResult = new PatientPCLLaboratoryResult();
            if (reader.HasColumn("Suggest") && reader["Suggest"] != DBNull.Value)
            {
                p.PatientPCLLaboratoryResult.Suggest = (string)reader["Suggest"];
            }
            if (reader.HasColumn("PCLSectionID") && reader["PCLSectionID"] != DBNull.Value)
            {
                p.PCLSectionID = Convert.ToInt64(reader["PCLSectionID"]);
            }
            if (reader.HasColumn("SectionName") && reader["SectionName"] != DBNull.Value)
            {
                p.SectionName = Convert.ToString(reader["SectionName"]);
            }
            //▼====: #001
            if (reader.HasColumn("HIRepResourceCode") && reader["HIRepResourceCode"] != DBNull.Value)
            {
                p.HIRepResourceCode = Convert.ToString(reader["HIRepResourceCode"]);
            }
            //▲====: #001
            //▼====: #006
            if (reader.HasColumn("PerformStaffID") && reader["PerformStaffID"] != DBNull.Value)
            {
                p.PerformStaffID = Convert.ToInt64(reader["PerformStaffID"]);
            }
            //▲====: #006
            //▼====: #001
            if (reader.HasColumn("DigitalSignatureResultPath") && reader["DigitalSignatureResultPath"] != DBNull.Value)
            {
                p.PatientPCLLaboratoryResult.DigitalSignatureResultPath = Convert.ToString(reader["DigitalSignatureResultPath"]);
            }
            //▲====: #001
            //▼==== #007
            if (reader.HasColumn("SpecimenID") && reader["SpecimenID"] != DBNull.Value)
            {
                p.PatientPCLLaboratoryResult.SpecimenID = Convert.ToInt64(reader["SpecimenID"]);
            }
            if (reader.HasColumn("SampleQuality") && reader["SampleQuality"] != DBNull.Value)
            {
                p.PatientPCLLaboratoryResult.SampleQuality = Convert.ToString(reader["SampleQuality"]);
            }
            p.PatientPCLLaboratoryResult.PatientPCLRequest = new PatientPCLRequest();
            if (reader.HasColumn("ReceptionTime") && reader["ReceptionTime"] != DBNull.Value)
            {
                p.PatientPCLLaboratoryResult.PatientPCLRequest.ReceptionTime = Convert.ToDateTime(reader["ReceptionTime"]);
            }
            //▲==== #007
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
            if (reader.HasColumn("RscrID") && reader["RscrID"] != DBNull.Value)
            {
                p.RscrID = Convert.ToInt64(reader["RscrID"]);
            }
            if (reader.HasColumn("HIRepResourceCode") && reader["HIRepResourceCode"] != DBNull.Value)
            {
                p.HIRepResourceCode = reader["HIRepResourceCode"].ToString();
            }
            if (reader.HasColumn("ItemName") && reader["ItemName"] != DBNull.Value)
            {
                p.ItemName = reader["ItemName"].ToString();
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

            p.L2D_Situs = new Lookup();
            p.LDOPPLER_Aortic_Grade = new Lookup();
            p.LDOPPLER_Mitral_Grade = new Lookup();
            p.LDOPPLER_Tricuspid_Grade = new Lookup();
            p.LDOPPLER_Pulmonary_Grade = new Lookup();

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

            if (reader.HasColumn("UltraResParams_FetalEchocardiography2DID") && reader["UltraResParams_FetalEchocardiography2DID"] != DBNull.Value)
            {
                p.UltraResParams_FetalEchocardiography2DID = (long)reader["UltraResParams_FetalEchocardiography2DID"];
            }

            if (reader.HasColumn("NTSize") && reader["NTSize"] != DBNull.Value)
            {
                p.NTSize = (double)reader["NTSize"];
            }


            if (reader.HasColumn("NPSize") && reader["NPSize"] != DBNull.Value)
            {
                p.NPSize = (double)reader["NPSize"];
            }

            if (reader.HasColumn("VanVieussensLeftAtrium") && reader["VanVieussensLeftAtrium"] != DBNull.Value)
            {
                p.VanVieussensLeftAtrium = (bool)reader["VanVieussensLeftAtrium"];
            }

            if (reader.HasColumn("AtrialSeptalDefect") && reader["AtrialSeptalDefect"] != DBNull.Value)
            {
                p.AtrialSeptalDefect = (bool)reader["AtrialSeptalDefect"];
            }

            if (reader.HasColumn("MitralValveSize") && reader["MitralValveSize"] != DBNull.Value)
            {
                p.MitralValveSize = (double)reader["MitralValveSize"];
            }

            if (reader.HasColumn("TriscupidValveSize") && reader["TriscupidValveSize"] != DBNull.Value)
            {
                p.TriscupidValveSize = (double)reader["TriscupidValveSize"];
            }

            if (reader.HasColumn("DifferenceMitralTricuspid") && reader["DifferenceMitralTricuspid"] != DBNull.Value)
            {
                p.DifferenceMitralTricuspid = (bool)reader["DifferenceMitralTricuspid"];
            }

            if (reader.HasColumn("TPTTr") && reader["TPTTr"] != DBNull.Value)
            {
                p.TPTTr = (double)reader["TPTTr"];
            }

            if (reader.HasColumn("VLTTTr") && reader["VLTTTr"] != DBNull.Value)
            {
                p.VLTTTr = (double)reader["VLTTTr"];
            }

            if (reader.HasColumn("TTTTr") && reader["TTTTr"] != DBNull.Value)
            {
                p.TTTTr = (double)reader["TTTTr"];
            }

            if (reader.HasColumn("DKTTTTr_VGd") && reader["DKTTTTr_VGd"] != DBNull.Value)
            {
                p.DKTTTTr_VGd = (double)reader["DKTTTTr_VGd"];
            }

            if (reader.HasColumn("DKTTTT_VGs") && reader["DKTTTT_VGs"] != DBNull.Value)
            {
                p.DKTTTT_VGs = (double)reader["DKTTTT_VGs"];
            }

            if (reader.HasColumn("DKTPTTr_VDd") && reader["DKTPTTr_VDd"] != DBNull.Value)
            {
                p.DKTPTTr_VDd = (double)reader["DKTPTTr_VDd"];
            }

            if (reader.HasColumn("DKTPTT_VDs") && reader["DKTPTT_VDs"] != DBNull.Value)
            {
                p.DKTPTT_VDs = (double)reader["DKTPTT_VDs"];
            }

            if (reader.HasColumn("Systolic") && reader["Systolic"] != DBNull.Value)
            {
                p.Systolic = (bool)reader["Systolic"];
            }

            if (reader.HasColumn("VentricularSeptalDefect") && reader["VentricularSeptalDefect"] != DBNull.Value)
            {
                p.VentricularSeptalDefect = (bool)reader["VentricularSeptalDefect"];
            }

            if (reader.HasColumn("AortaCompatible") && reader["AortaCompatible"] != DBNull.Value)
            {
                p.AortaCompatible = (bool)reader["AortaCompatible"];
            }

            if (reader.HasColumn("AortaSize") && reader["AortaSize"] != DBNull.Value)
            {
                p.AortaSize = (double)reader["AortaSize"];
            }

            if (reader.HasColumn("PulmonaryArterySize") && reader["PulmonaryArterySize"] != DBNull.Value)
            {
                p.PulmonaryArterySize = (double)reader["PulmonaryArterySize"];
            }

            if (reader.HasColumn("AorticArch") && reader["AorticArch"] != DBNull.Value)
            {
                p.AorticArch = (bool)reader["AorticArch"];
            }

            if (reader.HasColumn("AorticCoarctation") && reader["AorticCoarctation"] != DBNull.Value)
            {
                p.AorticCoarctation = (double)reader["AorticCoarctation"];
            }

            if (reader.HasColumn("HeartRateNomal") && reader["HeartRateNomal"] != DBNull.Value)
            {
                p.HeartRateNomal = (bool)reader["HeartRateNomal"];
            }

            if (reader.HasColumn("RequencyHeartRateNomal") && reader["RequencyHeartRateNomal"] != DBNull.Value)
            {
                p.RequencyHeartRateNomal = (double)reader["RequencyHeartRateNomal"];
            }

            if (reader.HasColumn("PericardialEffusion") && reader["PericardialEffusion"] != DBNull.Value)
            {
                p.PericardialEffusion = (bool)reader["PericardialEffusion"];
            }

            if (reader.HasColumn("FetalCardialAxis") && reader["FetalCardialAxis"] != DBNull.Value)
            {
                p.FetalCardialAxis = (double)reader["FetalCardialAxis"];
            }

            if (reader.HasColumn("CardialRateS") && reader["CardialRateS"] != DBNull.Value)
            {
                p.CardialRateS = (double)reader["CardialRateS"];
            }

            if (reader.HasColumn("LN") && reader["LN"] != DBNull.Value)
            {
                p.LN = (double)reader["LN"];
            }

            if (reader.HasColumn("OrderRecord") && reader["OrderRecord"] != DBNull.Value)
            {
                p.OrderRecord = reader["OrderRecord"].ToString();
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
            }

            if (reader.HasColumn("DoctorStaffID") && reader["DoctorStaffID"] != DBNull.Value)
            {
                p.DoctorStaffID = (long)reader["DoctorStaffID"];
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

            p.VAorticValve_Open = new Lookup();
            p.VMitralValve_Open = new Lookup();
            p.VPulmonaryValve_Open = new Lookup();
            p.VTriscupidValve_Open = new Lookup();


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

            p.VStaff = new Staff();

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
                p.CardialAbnormalDetail = reader["CardialAbnormalDetail"].ToString();
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

            p.VStaff = new Staff();

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

            p.VStaff = new Staff();
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
                long StaffID = 0;
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
        public IList<PCLGroup> GetAllPCLGroups(long? pclCategoryID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLGroups_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_PCLCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(pclCategoryID));
                cn.Open();
                List<PCLGroup> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLGroupColectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        public IList<PCLExamType> GetAllActiveExamTypes(PCLExamTypeSearchCriteria searchCriteria, int pageIndex, int pageSize, bool bCountTotal, string orderBy, out int totalCount)
        {
            totalCount = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypes_GetAllActiveItemsByGroupID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@PCLGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchCriteria.PCLGroupID));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(orderBy));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                List<PCLExamType> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    retVal = GetPCLExamTypeColectionFromReader(reader);
                    reader.Close();

                    if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                    {
                        totalCount = (int)cmd.Parameters["@Total"].Value;
                    }
                    else
                        totalCount = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public IList<PCLExamType> GetAllActiveExamTypes(PCLExamTypeSearchCriteria searchCriteria)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypes_GetAllActiveItemsByGroupID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@PCLGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchCriteria.PCLGroupID));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(searchCriteria.OrderBy));
                cn.Open();
                List<PCLExamType> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    retVal = GetPCLExamTypeColectionFromReader(reader);
                    reader.Close();
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        #endregion

        #region 2. PatientPCLRequest

        public bool AddFullPtPCLReq(PatientPCLRequest entity, long PatientID, long? PtRegistrationID, out long newPatientPCLReqID)
        {
            newPatientPCLReqID = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_InsertFully", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter par1 = cmd.Parameters.Add("@PatientID", SqlDbType.BigInt);
                SqlParameter par2 = cmd.Parameters.Add("@PtRegistrationID", SqlDbType.BigInt);
                SqlParameter par3 = cmd.Parameters.Add("@DoctorID", SqlDbType.BigInt);

                SqlParameter par11 = cmd.Parameters.Add("@ReqFromDeptLocID", SqlDbType.BigInt);
                SqlParameter par12 = cmd.Parameters.Add("@PCLDeptLocID", SqlDbType.BigInt);

                SqlParameter par5 = cmd.Parameters.Add("@Diagnosis", SqlDbType.NVarChar);
                SqlParameter par6 = cmd.Parameters.Add("@DoctorComments", SqlDbType.NVarChar);
                SqlParameter par7 = cmd.Parameters.Add("@IsExternalExam", SqlDbType.Bit);
                SqlParameter par8 = cmd.Parameters.Add("@IsCaseOfEmergency", SqlDbType.Bit);
                SqlParameter par9 = cmd.Parameters.Add("@XMLPtPCLRequestDetails", SqlDbType.Xml);
                SqlParameter par10 = cmd.Parameters.Add("@NewPatientPCLReqID", SqlDbType.BigInt);
                par10.Direction = ParameterDirection.Output;

                par1.Value = PatientID;
                par2.Value = PtRegistrationID;
                par3.Value = entity.PatientServiceRecord.StaffID;

                //////par11.Value = entity.ReqFromDeptLocID;
                //////par12.Value = entity.PCLDeptLocID;

                par5.Value = entity.Diagnosis;
                par6.Value = entity.DoctorComments;
                par7.Value = entity.IsExternalExam;
                par8.Value = entity.IsCaseOfEmergency;
                par9.Value = entity.PatientPCLRequestDetailsXML;
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                newPatientPCLReqID = (long)cmd.Parameters["@NewPatientPCLReqID"].Value;
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        public PatientMedicalRecord GetPMRByPtPCLFormRequest(long LoggedDoctorID, long PatientID, long? PtRegistrationID, long? PatientRecID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientMedicalRecords_ByPClFormRequest", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@DoctorID", SqlDbType.BigInt, ConvertNullObjectToDBNull(LoggedDoctorID));
                cmd.AddParameter("@PatientRecIDPara", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientRecID));

                cn.Open();

                PatientMedicalRecord obj = new PatientMedicalRecord();
                IDataReader reader = ExecuteReader(cmd);
                obj = GetPMRItemFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return obj;

            }
        }

        public List<PatientPCLRequest> PatientPCLRequest_ByPatientID_Paging(
            PatientPCLRequestSearchCriteria SearchCriteria, long V_RegistrationType,
          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
      )
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_ByPatientID_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientID));
                cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.Int, ConvertNullObjectToDBNull(SearchCriteria.V_PCLRequestStatus));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.Int, ConvertNullObjectToDBNull(SearchCriteria.PtRegistrationID));
                cmd.AddParameter("@TypeList", SqlDbType.Int, (int)(SearchCriteria.LoaiDanhSach));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));

                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize); 
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public List<PatientPCLRequest> GetPatientPCLRequestList_ByRegistrationID(long RegistrationID, long V_RegistrationType, long V_PCLMainCategory)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPCLRequestByRegistrationIDForConsultation", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLMainCategory));

                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public IList<PatientPCLRequestDetail> PatientPCLRequestDetail_ByPatientPCLReqID(PatientPCLRequestDetailSearchCriteria SearchCriteria, long V_RegistrationType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequestDetails_ByPatientPCLReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientPCLReqID));
                cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, SearchCriteria.V_ExamRegStatus);
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cn.Open();
                IList<PatientPCLRequestDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLRequestDetailsCollectionFromReader(reader);

                //foreach (var patientPclRequestDetail in objLst)
                //{
                //    var ListDeptLoc=ConfigurationManagerProviders.Instance.ListDeptLocation_ByPCLExamTypeID(patientPclRequestDetail.PCLExamType.PCLExamTypeID);
                //    patientPclRequestDetail.PCLExamType.ObjDeptLocationList = new ObservableCollection<DeptLocation>(ListDeptLoc); 
                //}

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public IList<PatientPCLRequestDetail> PatientPCLRequestDetail_ByPtRegistrationID(long PtRegistrationID, long V_RegistrationType, long V_PCLMainCategory)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequestDetails_ByPtRegistrationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLMainCategory));
                cn.Open();
                IList<PatientPCLRequestDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLRequestDetailsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }


        //Phiếu cuối cùng
        public PatientPCLRequest PatientPCLRequest_RequestLastest(Int64 PatientID, long V_RegistrationType, long? V_PCLMainCategory)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_RequestLastest", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLMainCategory));
                cmd.AddParameter("@V_RegistrationType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_RegistrationType));
                cn.Open();

                PatientPCLRequest obj = new PatientPCLRequest();
                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    obj = GetPatientPCLRequestFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return obj;
            }
        }
        //Phiếu cuối cùng


        //ds Phieu cuoi cung trong ngay chua tra tien
        public IList<PatientPCLRequest> PatientPCLReq_RequestLastestInDayNotPaid(long PatientID, long V_PCLRequestType, long ReqFromDeptLocID, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLReq_RequestLastestInDayNotPaid", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLRequestType));
                cmd.AddParameter("@ReqFromDeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ReqFromDeptLocID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cn.Open();

                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                foreach (var item in lst)
                {
                    //Doc detail ra
                    item.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>(PatientPCLRequestDetails_ByPatientPCLReqIDSimple(item.PatientPCLReqID));
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        //ds Phieu cuoi cung trong ngay chua tra tien


        //Chi tiet 1 phieu simple
        private IList<PatientPCLRequestDetail> PatientPCLRequestDetails_ByPatientPCLReqIDSimple(long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequestDetails_ByPatientPCLReqIDSimple", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                IList<PatientPCLRequestDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLRequestDetailsCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        //Chi tiet 1 phieu simple



        //Danh sách phiếu yêu cầu CLS
        public IList<PatientPCLRequest> PatientPCLRequest_SearchPaging(
         PatientPCLRequestSearchCriteria SearchCriteria,

       int PageIndex,
       int PageSize,
       string OrderBy,
       bool CountTotal,
       out int Total
   )
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_SearchPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeLocationsDeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeLocationsDeptLocationID));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeSubCategoryID));
                cmd.AddParameter("@PCLResultParamImpID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLResultParamImpID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.ToDate));
                cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLRequestStatus));
                cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_ExamRegStatus));
                cmd.AddParameter("@PatientCode", SqlDbType.Char, ConvertNullObjectToDBNull(SearchCriteria.PatientCode));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FullName));
                cmd.AddParameter("@PCLRequestNumID", SqlDbType.VarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLRequestNumID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)SearchCriteria.PatientFindBy));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cmd.CommandTimeout = int.MaxValue;

                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        //Danh sách phiếu yêu cầu CLS

        public PatientPCLRequest GetPatientPCLRequestResultsByReqID(PatientPCLRequestSearchCriteria SearchCriteria)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPatientPCLRequestResultsByReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientPCLReqID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)SearchCriteria.PatientFindBy));
                cn.Open();
                List<PatientPCLRequest> mPCLRequestCollection = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    mPCLRequestCollection = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return mPCLRequestCollection.FirstOrDefault();
            }
        }
        public void CheckTemplatePCLResultByReqID(long PatientPCLReqID, bool InPt,out bool IsNewTemplate, out long V_ReportForm, out long PCLImgResultID, out long V_PCLRequestType, out string TemplateResultString)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckTemplatePCLResultByReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cmd.AddParameter("@InPt", SqlDbType.Bit, ConvertNullObjectToDBNull(InPt));
                //cmd.AddParameter("@IsNewTemplate", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);
                //cmd.AddParameter("@V_ReportForm", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                //cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                //cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                //cmd.AddParameter("@TemplateResultString", SqlDbType.NVarChar, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                IsNewTemplate = false;
                V_ReportForm = 0;
                V_PCLRequestType = 0;
                PCLImgResultID = 0;
                TemplateResultString = "";
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    //reader.Read();
                    if (reader.HasColumn("IsNewTemplate") && reader["IsNewTemplate"] != DBNull.Value)
                    {
                        IsNewTemplate = (bool)reader["IsNewTemplate"];
                    }
                    if (reader.HasColumn("V_ReportForm") && reader["V_ReportForm"] != DBNull.Value)
                    {
                        V_ReportForm = (long)reader["V_ReportForm"];
                    }
                    if (reader.HasColumn("V_PCLRequestType") && reader["V_PCLRequestType"] != DBNull.Value)
                    {
                        V_PCLRequestType = (long)reader["V_PCLRequestType"];
                    }
                    if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
                    {
                        PCLImgResultID = (long)reader["PCLImgResultID"];
                    }
                    if (reader.HasColumn("TemplateResultString") && reader["TemplateResultString"] != DBNull.Value)
                    {
                        string filePath = reader["TemplateResultString"].ToString();
                        if (File.Exists(filePath))
                        {
                            string text = System.IO.File.ReadAllText(filePath);
                            TemplateResultString = text;
                        }
                    }
                }
                reader.Close();
                
                CleanUpConnectionAndCommand(cn, cmd);
                //return true;
            }
        }
        public void GetPCLResult_Criterion(long ServiceRecID, out long V_ResultType, out string TemplateResultString)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPCLResult_Criterion", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AdmissionCriterionDetail_PCLResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ServiceRecID));
                cn.Open();

                TemplateResultString = "";
                V_ResultType = 0;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    //reader.Read();
                 
                    if (reader.HasColumn("TemplateResultString") && reader["TemplateResultString"] != DBNull.Value)
                    {
                        //string filePath = reader["TemplateResultString"].ToString();
                        //if (File.Exists(filePath))
                        //{
                        //    string text = System.IO.File.ReadAllText(filePath);
                        //    TemplateResultString = text;
                        //}
                        TemplateResultString = reader["TemplateResultString"].ToString();
                    }
                    if (reader.HasColumn("V_ResultType") && reader["V_ResultType"] != DBNull.Value)
                    {
                        V_ResultType = Convert.ToInt64(reader["V_ResultType"]);
                    }
                }
                reader.Close();
                
                CleanUpConnectionAndCommand(cn, cmd);
                //return true;
            }
        }

        public IList<PatientPCLRequest> PatientPCLRequest_ViewResult_SearchPaging(
                           PatientPCLRequestSearchCriteria SearchCriteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
           out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_ViewResult_SearchPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeSubCategoryID));
                cmd.AddParameter("@PCLResultParamImpID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLResultParamImpID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.ToDate));
                cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLRequestStatus));
                cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_ExamRegStatus));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientID));
                cmd.AddParameter("@PatientCode", SqlDbType.Char, ConvertNullObjectToDBNull(SearchCriteria.PatientCode));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FullName));
                cmd.AddParameter("@PCLRequestNumID", SqlDbType.VarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLRequestNumID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)SearchCriteria.PatientFindBy));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cmd.CommandTimeout = int.MaxValue;

                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        /// <summary>
        /// union thêm ngoại viện 
        /// </summary>

        public IList<PatientPCLRequest> PatientPCLRequest_ViewResult_SearchPagingNew(
                           PatientPCLRequestSearchCriteria SearchCriteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
           out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_ViewResult_SearchPagingNew", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeSubCategoryID));
                cmd.AddParameter("@PCLResultParamImpID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLResultParamImpID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.ToDate));
                cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLRequestStatus));
                cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_ExamRegStatus));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientID));
                cmd.AddParameter("@PatientCode", SqlDbType.Char, ConvertNullObjectToDBNull(SearchCriteria.PatientCode));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FullName));
                cmd.AddParameter("@PCLRequestNumID", SqlDbType.VarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLRequestNumID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)SearchCriteria.PatientFindBy));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cmd.CommandTimeout = int.MaxValue;

                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }


        //Danh sách phiếu các lần trước
        public IList<PatientPCLRequest> PatientPCLRequest_ByPatientIDV_Param_Paging(
         PatientPCLRequestSearchCriteria SearchCriteria,

       int PageIndex,
       int PageSize,
       string OrderBy,
       bool CountTotal,
       out int Total
   )
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_ByPatientIDV_Param_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PatientID));
                cmd.AddParameter("@V_Param", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_Param));
                /*▼====: #001*/
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)SearchCriteria.PatientFindBy));
                /*▲====: #001*/
                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        //Danh sách phiếu các lần trước


        #endregion

        #region 3. PCL result
        public IList<PCLExamGroup> GetPCLResult_PCLExamGroup(long? ptID, long? ptPCLReqID, bool isImported)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLResult_PCLExamGroup_ByPtPCLReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptPCLReqID));
                if (isImported)
                {
                    cmd.AddParameter("@IsImported", SqlDbType.Bit, 1);
                }
                else
                {
                    cmd.AddParameter("@IsImported", SqlDbType.Bit, 0);
                }
                cn.Open();

                List<PCLExamGroup> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetExamGroupCollectionsFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;

            }
        }
        public override PCLResultFileStorageDetail GetPCLResultFileStorageDetailFromReader(IDataReader reader)
        {
            PCLResultFileStorageDetail p = base.GetPCLResultFileStorageDetailFromReader(reader);
            if (reader.HasColumn("ServiceRecID") && reader["ServiceRecID"] != DBNull.Value)
            {
                p.ServiceRecID = reader["ServiceRecID"] as long?;
            }
            if (reader.HasColumn("PCLRequestDate") && reader["PCLRequestDate"] != DBNull.Value)
            {
                p.PCLRequestDate = (DateTime)reader["PCLRequestDate"];
            }
            if (reader.HasColumn("AgencyID") && reader["AgencyID"] != DBNull.Value)
            {
                p.AgencyID = reader["AgencyID"] as long?;
            }
            if (reader.HasColumn("AgencyNameAdd") && reader["AgencyNameAdd"] != DBNull.Value)
            {
                p.AgencyNameAddress = reader["AgencyNameAdd"].ToString();
            }
            if (reader.HasColumn("PCLExamDate") && reader["PCLExamDate"] != DBNull.Value)
            {
                p.PCLExamDate = (DateTime)reader["PCLExamDate"];
            }
            if (reader.HasColumn("DiagnoseOnPCLExam") && reader["DiagnoseOnPCLExam"] != DBNull.Value)
            {
                p.DiagnoseOnPCLExam = reader["DiagnoseOnPCLExam"].ToString();
            }

            if (reader.HasColumn("ExamDoctorID") && reader["ExamDoctorID"] != DBNull.Value)
            {
                p.ExamDoctorID = reader["ExamDoctorID"] as long?;
            }
            if (reader.HasColumn("ExamDoctorFullName") && reader["ExamDoctorFullName"] != DBNull.Value)
            {
                p.ExamDoctorFullName = reader["ExamDoctorFullName"].ToString();
            }
            if (reader.HasColumn("PCLExamForOutPatient") && reader["PCLExamForOutPatient"] != DBNull.Value)
            {
                p.PCLExamForOutPatient = reader["PCLExamForOutPatient"] as bool?;
            }
            try
            {
                if (reader.HasColumn("IsExternalExam") && reader["IsExternalExam"] != DBNull.Value)
                {
                    p.IsExternalExam = reader["IsExternalExam"] as bool?;
                }
            }
            catch
            { }

            //try
            //{
            //    p.PCLExamGroupID = reader["PCLExamGroupID"] as long?;
            //    p.PCLExamTypeID = reader["PCLExamTypeID"] as long?;
            //}
            //catch
            //{ }
            if (reader.HasColumn("RequestDoctor") && reader["RequestDoctor"] != DBNull.Value)
            {
                p.RequestDoctor = reader["RequestDoctor"].ToString();
            }
            if (reader.HasColumn("ResultType") && reader["ResultType"] != DBNull.Value)
            {
                p.ResultType = reader["ResultType"].ToString();
            }
            if (reader.HasColumn("PCLExamTypeCode") && reader["PCLExamTypeCode"] != DBNull.Value)
            {
                p.PCLExamTypeCode = reader["PCLExamTypeCode"].ToString();
            }
            if (reader.HasColumn("PathNameOfResource") && reader["PathNameOfResource"] != DBNull.Value)
            {
                p.PathNameOfResource = reader["PathNameOfResource"].ToString();
            }
            if (reader.HasColumn("IsImage") && reader["IsImage"] != DBNull.Value)
            {
                p.IsImage = (bool)reader["IsImage"];
            }
            if (reader.HasColumn("IsVideo") && reader["IsVideo"] != DBNull.Value)
            {
                p.IsVideo = (bool)reader["IsVideo"];
            }
            if (reader.HasColumn("IsDocument") && reader["IsDocument"] != DBNull.Value)
            {
                p.IsDocument = (bool)reader["IsDocument"];
            }
            if (reader.HasColumn("IsOthers") && reader["IsOthers"] != DBNull.Value)
            {
                p.IsOthers = (bool)reader["IsOthers"];
            }
            //reader.Close();
            return p;
        }
        /*▼====: #002*/
        //public  IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetails(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID)
        public IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetails(long ptID, long? ptPCLReqID, long? PCLGroupID, long? PCLExamTypeID, long V_PCLRequestType = (long)AllLookupValues.V_PCLRequestType.NGOAI_TRU)
        /*▲====: #002*/
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLResultFileStorageDetails_ByPCLExTypeID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                /*▼====: #002*/
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLRequestType));
                /*▲====: #002*/
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ptPCLReqID));
                cmd.AddParameter("@PCLGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLGroupID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                cn.Open();

                List<PCLResultFileStorageDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLResultFileStorageDetailCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetailsExt(PCLResultFileStorageDetailSearchCriteria p)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLResultFileStorageDetails_ByPCLExTypeIDExt", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PatientID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PatientPCLReqID));
                cmd.AddParameter("@PCLGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PCLGroupID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.PCLExamTypeID));
                cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(p.IsExternalExam));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.V_PCLRequestType));
                cn.Open();
                List<PCLResultFileStorageDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLResultFileStorageDetailCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public IList<PCLResultFileStorageDetail> GetPCLResultFileStoreDetailsByPCLImgResultID(long PCLImgResultID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLResultFileStorageDetails_GetByPCLImgResultID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLImgResultID));

                cn.Open();

                List<PCLResultFileStorageDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLResultFileStorageDetailCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        #endregion

        #region 4. PCL Laboratory

        public IList<MedicalSpecimensCategory> GetAllMedSpecCatg()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spMedicalSpecimensCategory_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                List<MedicalSpecimensCategory> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetAllMedSpecCatgColectionsFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public IList<MedicalSpecimen> GetMedSpecsByCatgID(short? medSpecCatgID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spMedicalSpecimens_ByCatgID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@MedSpecCatID", SqlDbType.SmallInt, ConvertNullObjectToDBNull(medSpecCatgID));
                cn.Open();

                List<MedicalSpecimen> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetMedSpecsByCatgIDColectionsFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public string PCLExamTestItems_ByPatientID(long PatientID, PatientPCLRequestSearchCriteria SearchCriteria)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTestItems_ByPatientID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)SearchCriteria.PatientFindBy));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.ToDate));
                cmd.AddParameter("@Results", SqlDbType.Xml, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                cmd.ExecuteNonQuery();
                if (cmd.Parameters["@Results"].Value != DBNull.Value && !string.IsNullOrEmpty(cmd.Parameters["@Results"].Value.ToString()))
                {
                    return cmd.Parameters["@Results"].Value.ToString();
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return "";
            }
        }

        public IList<PCLExamTestItems> PCLExamTestItems_SearchPaging(GeneralSearchCriteria SearchCriteria, int PageIndex, int PageSize, bool bCount, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTestItems_SearchPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTestItemName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FindName));
                cmd.AddParameter("@PCLExamTestItemCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FindCode));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.FindID));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@bCount", SqlDbType.Bit, ConvertNullObjectToDBNull(bCount));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();

                List<PCLExamTestItems> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLExamTestItemsColectionFromReader(reader);
                reader.Close();
                if (cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    Total = Convert.ToInt32(cmd.Parameters["@Total"].Value);
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public bool PCLExamTestItems_Save(PCLExamTestItems Item, out long PCLExamTestItemID)
        {
            PCLExamTestItemID = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTestItems_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTestItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Item.PCLExamTestItemID));
                cmd.AddParameter("@PCLExamTestItemName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemName));
                cmd.AddParameter("@PCLExamTestItemDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemDescription));
                cmd.AddParameter("@PCLExamTestItemCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemCode));
                cmd.AddParameter("@PCLExamTestItemUnit", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemUnit));
                cmd.AddParameter("@PCLExamTestItemRefScale", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemRefScale));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(Item.IsActive));
                cmd.AddParameter("@IsBold", SqlDbType.Bit, ConvertNullObjectToDBNull(Item.IsBold));
                cmd.AddParameter("@TestItemIsExamType", SqlDbType.Bit, ConvertNullObjectToDBNull(Item.TestItemIsExamType));
                cmd.AddParameter("@IsNoNeedResult", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.IsNoNeedResult));
                cmd.AddParameter("@OutputID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@PCLExamTestItemHICode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemHICode));
                cmd.AddParameter("@PCLExamTestItemHIName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemHIName));
                //▼==== #008
                cmd.AddParameter("@PCLExamTestItemNameEng", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemNameEng));
                cmd.AddParameter("@PCLExamTestItemUnitEng", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemUnitEng));
                cmd.AddParameter("@PCLExamTestItemRefScaleEng", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Item.PCLExamTestItemRefScaleEng));
                //▲==== #008
                cn.Open();
                int count = cmd.ExecuteNonQuery();
                if (cmd.Parameters["@OutputID"].Value != DBNull.Value)
                {
                    PCLExamTestItemID = (long)cmd.Parameters["@OutputID"].Value;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }
        }

        public bool RefDepartmentReqCashAdv_Save(RefDepartmentReqCashAdv Target, out long ID)
        {
            ID = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartmentReqCashAdv_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RefDepartmentReqCashAdvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Target.RefDepartmentReqCashAdvID));
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Target.DeptID));
                cmd.AddParameter("@CashAdvAmtReq", SqlDbType.Money, ConvertNullObjectToDBNull(Target.CashAdvAmtReq));
                cmd.AddParameter("@ID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                int count = cmd.ExecuteNonQuery();
                if (cmd.Parameters["@ID"].Value != DBNull.Value)
                {
                    ID = (long)cmd.Parameters["@ID"].Value;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }
        }

        public bool RefDepartmentReqCashAdv_Delete(long RefDepartmentReqCashAdvID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartmentReqCashAdv_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RefDepartmentReqCashAdvID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefDepartmentReqCashAdvID));
                cn.Open();
                CleanUpConnectionAndCommand(cn, cmd);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public List<RefDepartmentReqCashAdv> RefDepartmentReqCashAdv_GetAll(string SearchText, string OrderBy, int PageIndex, int PageSize, bool bCount, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartmentReqCashAdv_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SearchText", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchText));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@bCount", SqlDbType.Bit, ConvertNullObjectToDBNull(bCount));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                List<RefDepartmentReqCashAdv> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetRefDepartmentReqCashAdvCollectionFromReader(reader);
                reader.Close();
                if (cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public bool PCLExamTypeServiceTarget_Save(PCLExamTypeServiceTarget Target, out long ID)
        {
            ID = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeServiceTarget_Save", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeServiceTargetID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Target.PCLExamTypeServiceTargetID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Target.PCLExamTypeID));
                cmd.AddParameter("@MondayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.MondayTargetNumberOfCases));
                cmd.AddParameter("@TuesdayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.TuesdayTargetNumberOfCases));
                cmd.AddParameter("@WednesdayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.WednesdayTargetNumberOfCases));
                cmd.AddParameter("@ThursdayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.ThursdayTargetNumberOfCases));
                cmd.AddParameter("@FridayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.FridayTargetNumberOfCases));
                cmd.AddParameter("@SaturdayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.SaturdayTargetNumberOfCases));
                cmd.AddParameter("@SundayTargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(Target.SundayTargetNumberOfCases));
                cmd.AddParameter("@ID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                int count = cmd.ExecuteNonQuery();
                if (cmd.Parameters["@ID"].Value != DBNull.Value)
                {
                    ID = (long)cmd.Parameters["@ID"].Value;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }
        }

        public bool PCLExamTypeServiceTarget_Delete(long PCLExamTypeServiceTargetID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeServiceTarget_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeServiceTargetID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeServiceTargetID));
                cn.Open();
                int result = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return result > 0;
            }
        }

        public List<PCLExamTypeServiceTarget> PCLExamTypeServiceTarget_GetAll(string SearchText, string OrderBy, int PageIndex, int PageSize, bool bCount, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeServiceTarget_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SearchText", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchText));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@bCount", SqlDbType.Bit, ConvertNullObjectToDBNull(bCount));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                List<PCLExamTypeServiceTarget> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLExamTypeServiceTargetColectionFromReader(reader);
                reader.Close();
                if (cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public bool PCLExamTypeServiceTarget_Checked(long PCLExamTypeID, DateTime Date)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeServiceTarget_Checked", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                cmd.AddParameter("@Date", SqlDbType.DateTime, ConvertNullObjectToDBNull(Date));
                cn.Open();
                cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }

        public bool PCLExamTypeServiceTarget_Checked_Appointment(long PCLExamTypeID, DateTime Date)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamTypeServiceTarget_Checked_Appointment", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                cmd.AddParameter("@Date", SqlDbType.DateTime, ConvertNullObjectToDBNull(Date));
                cn.Open();
                cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }

        public IList<PCLExamTestItems> PatientPCLLaboratoryResults_ByExamTest_Paging(long PatientID, long PCLExamTestItemID, long PCLExamTypeID, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, long PatientFindBy, out int TotalRow, out int MaxRow)
        {
            TotalRow = 0;
            MaxRow = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResults_ByExamTest_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PCLExamTestItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTestItemID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientFindBy));
                cmd.AddParameter("@TotalRow", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cmd.AddParameter("@MaxRow", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();

                List<PCLExamTestItems> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPCLExamTestItemsColectionFromReader(reader);
                reader.Close();
                if (cmd.Parameters["@TotalRow"].Value != DBNull.Value)
                {
                    TotalRow = (int)cmd.Parameters["@TotalRow"].Value;
                }
                if (cmd.Parameters["@MaxRow"].Value != DBNull.Value)
                {
                    MaxRow = (int)cmd.Parameters["@MaxRow"].Value;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public string PatientPCLLaboratoryResults_ByExamTest_Crosstab(long PatientID, string strXml, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalCol)
        {
            TotalCol = 0;
            DataTable nYTEST = PatientPCLLaboratoryResults_ByExamTest_Crosstab_New(PatientID, strXml, FromDate, ToDate, PageIndex, PageSize, out TotalCol);
            DataTable dtResult = Pivot(nYTEST, "PCLExamTestItemName", "SamplingDate", "Value");

            var sb = new StringBuilder();
            sb.Append("<data>");
            sb.Append("<columns>");
            //sb.Append("<column name=\"Name\"></column>");
            //sb.Append("<column name=\"Age\"></column>");
            sb.Append("[ValueCols]");
            sb.Append("</columns>");

            sb.Append("<rows>");
            //sb.Append("<row>");
            //sb.Append("<cell>Bob</cell>");
            //sb.Append("<cell>30</cell>");
            sb.Append("[ValueCells]");
            //sb.Append("</row>");
            sb.Append("</rows>");

            sb.Append("</data>");


            //DataTable Column
            var sbValueCols = new StringBuilder();

            //=================================================
            //Tính toán phun về Client nhận chuỗi
            foreach (DataColumn dc in dtResult.Columns)
            {
                sbValueCols.Append("<column name=\"" + dc.ColumnName.Trim() + "\"></column>");
            }

            //DataTable Row
            var sbValueCells = new StringBuilder();

            for (int i = 0; i < dtResult.Rows.Count; i++)
            {
                sbValueCells.Append("<row>");
                for (int j = 0; j < dtResult.Columns.Count; j++)
                {
                    sbValueCells.Append("<cell>" + dtResult.Rows[i][j].ToString().Replace("<", "&lt;").Replace(">", "&gt;") + "</cell>");
                }
                sbValueCells.Append("</row>");
            }

            sb = sb.Replace("[ValueCols]", sbValueCols.ToString());
            sb = sb.Replace("[ValueCells]", sbValueCells.ToString());

            return sb.ToString();

        }

        public DataTable PatientPCLLaboratoryResults_ByExamTest_Crosstab_New(long PatientID, string strXml, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalCol)
        {
            TotalCol = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResults_ByExamTest_Crosstab_new", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                //cmd.AddParameter("@PCLExamTestItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTestItemID));
                //cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                cmd.AddParameter("@ListID", SqlDbType.Xml, ConvertNullObjectToDBNull(strXml));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@TotalCol", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                // cmd.ExecuteNonQuery();
                IDataReader reader = cmd.ExecuteReader();
                DataTable tbl = null;
                if (reader != null)
                {
                    tbl = new DataTable();
                    tbl.Load(reader);
                }
                reader.Close();
                if (cmd.Parameters["@TotalCol"].Value != DBNull.Value)
                {
                    TotalCol = (int)cmd.Parameters["@TotalCol"].Value;
                }

                CleanUpConnectionAndCommand(cn, cmd);
                return tbl;
                // return "";
            }
        }

        public DataTable Pivot(DataTable src, string VerticalColumnName, string HorizontalColumnName, string ValueColumnName)
        {
            DataTable dst = new DataTable();
            if (src == null || src.Rows.Count == 0)
                return dst;

            // find all distinct names for column and row
            ArrayList ColumnValues = new ArrayList();
            ArrayList RowValues = new ArrayList();
            foreach (DataRow dr in src.Rows)
            {
                // find all column values
                object column = dr[VerticalColumnName];
                if (!ColumnValues.Contains(column))
                    ColumnValues.Add(column);

                //find all row values
                object row = dr[HorizontalColumnName];
                if (!RowValues.Contains(row))
                    RowValues.Add(row);
            }

            //ColumnValues.Sort();
            //RowValues.Sort();

            //create columns
            dst = new DataTable();
            dst.Columns.Add("Tên", src.Columns[VerticalColumnName].DataType);
            // dst.Columns.Add(VerticalColumnName, src.Columns[VerticalColumnName].DataType);
            dst.Columns.Add("ĐVT", src.Columns["PCLExamTestItemUnit"].DataType);
            dst.Columns.Add("Thang Tham Chiếu", src.Columns["PCLExamTestItemRefScale"].DataType);

            Type t = src.Columns[ValueColumnName].DataType;
            foreach (object ColumnNameInRow in RowValues)
            {
                dst.Columns.Add(ColumnNameInRow.ToString(), t);
            }

            //create destination rows
            foreach (object RowName in ColumnValues)
            {
                DataRow NewRow = dst.NewRow();
                NewRow["Tên"] = RowName.ToString();//VerticalColumnName
                dst.Rows.Add(NewRow);
            }

            //fill out pivot table
            foreach (DataRow drSource in src.Rows)
            {
                object key = drSource[VerticalColumnName];
                string ColumnNameInRow = Convert.ToString(drSource[HorizontalColumnName]);
                int index = ColumnValues.IndexOf(key);
                dst.Rows[index][ColumnNameInRow] = drSource[ValueColumnName];// sum(dst.Rows[index][ColumnNameInRow], drSource[ValueColumnName]);

                dst.Rows[index][1] = drSource["PCLExamTestItemUnit"];
                dst.Rows[index][2] = drSource["PCLExamTestItemRefScale"];
            }

            return dst;
        }

        dynamic sum(dynamic a, dynamic b)
        {
            if (a is DBNull && b is DBNull)
                return DBNull.Value;
            else if (a is DBNull && !(b is DBNull))
                return b;
            else if (!(a is DBNull) && b is DBNull)
                return a;
            else
                return a + b;
        }

        public IList<PatientPCLLaboratoryResultDetail> PCLLaboratoryResults_With_ResultOld(long PatientID, long PatientPCLReqID, long V_PCLRequestType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLLaboratoryResults_With_ResultOld", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLRequestType));
                cn.Open();
                List<PatientPCLLaboratoryResultDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLLaboratoryResultDetailCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public string GetPCLExamTypeName(long PatientID, long PatientPCLReqID, long V_PCLRequestType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPCLExamTypeName", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLRequestType));
                cn.Open();

                string result = "";
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    if (reader.HasColumn("PCLExamTypeName") && reader["PCLExamTypeName"] != DBNull.Value)
                    {
                        result = reader["PCLExamTypeName"].ToString();
                    }

                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return result;
            }
        }

        public IList<PatientPCLLaboratoryResultDetail> PCLLaboratoryResults_No_ResultOld(long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResults_ByPatientPCLReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cn.Open();

                List<PatientPCLLaboratoryResultDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLLaboratoryResultDetailCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public IList<PatientPCLLaboratoryResultDetail> GetPtPCLLabExamTypes_ByPtPCLReqID(long PatientID, long? PtPCLReqID, long? PCLExamGroupID, long? PCLExamTypeID, bool isImported)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPtPCLLabExamTypes_ByPtPCLReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtPCLReqID));
                cmd.AddParameter("@PCLExamGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamGroupID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                if (isImported)
                {
                    cmd.AddParameter("@IsImported", SqlDbType.Bit, 1);
                }
                else
                {
                    cmd.AddParameter("@IsImported", SqlDbType.Bit, 0);
                }

                cn.Open();

                List<PatientPCLLaboratoryResultDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPtPCLLabExamTypesByReqIDColectionsFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public IList<MedicalSpecimenInfo> GetMedSpecInfo_ByPtPCLReqID(long? PtPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spMedicalSpecimenInfo_ByPtPCLReqID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtPCLReqID));

                cn.Open();

                List<MedicalSpecimenInfo> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetMedSpecByReqIDColectionsFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public bool AddPatientPCLLaboratoryResultDetail(PatientPCLLaboratoryResultDetail entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResultDetailsInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LabResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.LabResultID));
                cmd.AddParameter("@PCLExamTypeTestItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLExamTypeTestItemID));
                cmd.AddParameter("@Value", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Value));
                cmd.AddParameter("@IsAbnormal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsAbnormal));
                cmd.AddParameter("@Comments", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Comments));
                //▼====: #001
                cmd.AddParameter("@HIRepResourceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.HIRepResourceCode));
                //▲====: #001
                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        // 20181216 TNHX: [BM0005430] Add PatientID for report PCLDEPARTMENT_LABORATORY_RESULT
        public bool UpdatePatientPCLLaboratoryResultDetail(IList<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetailentity, PatientPCLLaboratoryResult entity, long PCLRequestTypeID, long PatientID
            , bool? IsWaitResult, bool? IsDone, out string errorOutput)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                List<SqlCommand> lstCmd = new List<SqlCommand>();

                SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResultDetailsUpdateXml", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                //cmd.AddParameter("@PCLExtRefID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLExtRefID));
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientPCLReqID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.StaffID));
                //cmd.AddParameter("@AgencyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.AgencyID));
                //cmd.AddParameter("@SampleCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.SampleCode));
                //cmd.AddParameter("@DiagnosisOnExam", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DiagnosisOnExam));
                //cmd.AddParameter("@PCLForOutPatient", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PCLForOutPatient));
                //cmd.AddParameter("@IsExternalExam", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.IsExternalExam));
                //if (entity.MedSpecID>0)
                //    cmd.AddParameter("@MedSpecID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.MedSpecID));
                //else
                //    cmd.AddParameter("@MedSpecID", SqlDbType.BigInt, null);

                cmd.AddParameter("@LabResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.LabResultID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLRequestTypeID));
                errorOutput = "";
                cmd.AddParameter("@DataXML", SqlDbType.Xml, PatientPCLLabResDetails_ConvertListToXml(allPatientPCLLaboratoryResultDetailentity));
                cmd.AddParameter("@errorOutput", SqlDbType.NVarChar, ConvertNullObjectToDBNull(errorOutput), ParameterDirection.Output);
                cmd.AddParameter("@Suggest", SqlDbType.NVarChar, entity.Suggest);
                cmd.AddParameter("@IsWaitResult", SqlDbType.Bit, ConvertNullObjectToDBNull(IsWaitResult));
                cmd.AddParameter("@IsDone", SqlDbType.Bit, ConvertNullObjectToDBNull(IsDone));
                //▼==== #007
                cmd.AddParameter("@SpecimenID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.SpecimenID));
                cmd.AddParameter("@SampleQuality", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.SampleQuality));
                //▲==== #007
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                if (cmd.Parameters["@errorOutput"].Value != null)
                {
                    errorOutput = cmd.Parameters["@errorOutput"].Value.ToString();
                }
                else
                    errorOutput = "";
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;

            }
        }

        public bool DeletePatientPCLLaboratoryResult(long PatientPCLReqID, long PCLRequestTypeID, long CancelStaffID, long PCLExamTypeID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResultDelete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                    cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLRequestTypeID));
                    cmd.AddParameter("@CancelStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CancelStaffID));
                    cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));

                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    if (retVal > 0)
                        return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool DeletePatientPCLImagingResult(long PatientPCLReqID, long PCLRequestTypeID, long CancelStaffID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLImagingResultDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLRequestTypeID));
                cmd.AddParameter("@CancelStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(CancelStaffID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;

            }
        }

        public bool DeletePatientPCLLaboratoryResultDetail(PatientPCLLaboratoryResultDetail entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLLaboratoryResultDetailsDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LabResultDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.LabResultDetailID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        private string PatientPCLLabResDetails_ConvertListToXml(IList<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetail)
        {
            if (allPatientPCLLaboratoryResultDetail != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (PatientPCLLaboratoryResultDetail item in allPatientPCLLaboratoryResultDetail)
                {
                    if (item.PCLExamTypeTestItemID > 0)
                    {
                        sb.Append("<PatientPCLLabResDetails>");
                        sb.AppendFormat("<LabResultDetailID>{0}</LabResultDetailID>", item.LabResultDetailID);

                        sb.AppendFormat("<LabResultID>{0}</LabResultID>", item.LabResultID);
                        sb.AppendFormat("<PCLExamTypeTestItemID>{0}</PCLExamTypeTestItemID>", item.PCLExamTypeTestItemID);
                        if (item.Value != null)
                        {
                            sb.AppendFormat("<Value>{0}</Value>", Globals.GetSafeXMLString(item.Value));
                        }
                        sb.AppendFormat("<IsAbnormal>{0}</IsAbnormal>", item.IsAbnormal);
                        sb.AppendFormat("<Comments>{0}</Comments>", Globals.GetSafeXMLString(item.Comments));
                        sb.AppendFormat("<Value_Old>{0}</Value_Old>", Globals.GetSafeXMLString(item.Value_Old));
                        //▼====: #001
                        sb.AppendFormat("<HIRepResourceCode>{0}</HIRepResourceCode>", Globals.GetSafeXMLString(item.HIRepResourceCode));
                        //▲====: #001
                        //▼====: #006
                        sb.AppendFormat("<PerformStaffID>{0}</PerformStaffID>", item.PerformStaffID);
                        //▲====: #006
                        sb.Append("</PatientPCLLabResDetails>");
                    }

                }
                sb.Append("</DS>");



                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        public IList<PatientPCLRequest> ListPatientPCLRequest_LAB_Paging(long PatientID, long? DeptLocID, long V_PCLRequestType,
          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spListPatientPCLRequest_LAB_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLRequestType));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cmd.CommandTimeout = 120;

                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        public IList<PatientPCLRequest> LIS_Order(string SoPhieuChiDinh, bool IsAll)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_LIS_Order", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SoPhieuChiDinh", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SoPhieuChiDinh));
                cmd.AddParameter("@IsAll", SqlDbType.Bit, ConvertNullObjectToDBNull(IsAll));
                cn.Open();

                List<PatientPCLRequest> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLRequestCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }

        }

        public bool LIS_Result(PatientPCLRequest_LABCom ParamLabCom)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {

                SqlCommand cmd = new SqlCommand("sp_LIS_Result", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@SoPhieuChiDinh", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ParamLabCom.SoPhieuChiDinh));
                cmd.AddParameter("@MaDichVu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ParamLabCom.MaDichVu));
                cmd.AddParameter("@KetQua", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ParamLabCom.KetQua));
                cmd.AddParameter("@ChiSoBinhThuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(""));
                cmd.AddParameter("@DonViTinh", SqlDbType.NVarChar, ConvertNullObjectToDBNull(""));
                cmd.AddParameter("@BatThuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull("0"));
                cmd.AddParameter("@SoBenhPham", SqlDbType.NVarChar, ConvertNullObjectToDBNull("1"));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                return false;
            }
        }

        #endregion

        #region 5. Tuyen
        public IList<PCLItem> GetPCLItemsByPCLFormID(long? PCLFormID, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_PCL_GetPCLItems_ByFormID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLFormID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLFormID));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(""));
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<PCLItem> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetPCLItemCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        #endregion


        #region dinh

        #region PCLExamParamResult
        public List<PCLExamParamResult> GetPCLExamParamResultList(long PCLExamResultID, long ParamEnum)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamParamResultGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamResultID));
                cmd.AddParameter("@ParamEnum", SqlDbType.BigInt, ConvertNullObjectToDBNull(ParamEnum));

                cn.Open();
                List<PCLExamParamResult> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPCLExamParamResultCollectionFromReader(reader);
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public PCLExamParamResult GetPCLExamParamResult(long PCLExamResultID, long ParamEnum)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamParamResultGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamResultID));
                cmd.AddParameter("@ParamEnum", SqlDbType.BigInt, ConvertNullObjectToDBNull(ParamEnum));

                cn.Open();
                PCLExamParamResult retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetPCLExamParamResultFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public bool AddPCLExamParamResult(PCLExamParamResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamParamResultInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ParamEnum", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ParamEnum));
                cmd.AddParameter("@PCLExamGroupTemplateResultID", SqlDbType.Int, ConvertNullObjectToDBNull(entity.PCLExamGroupTemplateResultID));
                cmd.AddParameter("@GroupName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.GroupName));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdatePCLExamParamResult(PCLExamParamResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamParamResultUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLExamResultID));
                cmd.AddParameter("@ParamEnum", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ParamEnum));
                cmd.AddParameter("@PCLExamGroupTemplateResultID", SqlDbType.Int, ConvertNullObjectToDBNull(entity.PCLExamGroupTemplateResultID));
                cmd.AddParameter("@GroupName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.GroupName));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeletePCLExamParamResult(PCLExamParamResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamParamResultDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLExamResultID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region PCLExamResultTemplate
        public List<PCLExamResultTemplate> GetPCLExamResultTemplateList(long PCLExamGroupTemplateResultID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamResultTemplateGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamGroupTemplateResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamGroupTemplateResultID));

                cn.Open();
                List<PCLExamResultTemplate> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPCLExamResultTemplateCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public List<PCLExamResultTemplate> GetPCLExamResultTemplateListByTypeID(long PCLExamGroupTemplateResultID, int ParamEnum)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamResultTemplateGetByTypeID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamGroupTemplateResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamGroupTemplateResultID));
                cmd.AddParameter("@ParamEnum", SqlDbType.Int, ConvertNullObjectToDBNull(ParamEnum));

                cn.Open();
                List<PCLExamResultTemplate> retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                retVal = GetPCLExamResultTemplateCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        /*▼====: #003*/
        public List<Resources> GetResourcesForMedicalServicesListByTypeID(long PCLResultParamImpID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourcesForMedicalServices_LoadForPCL", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLResultParamImpID", SqlDbType.VarChar, ConvertNullObjectToDBNull(PCLResultParamImpID));

                cn.Open();
                List<Resources> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetResourcesForMedicalServicesListByTypeIDCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        /*▲====: #003*/
        /*▼====: #003*/
        public List<Resources> GetResourcesForLaboratory()
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourcesForLaboratory", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                List<Resources> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetResourcesForMedicalServicesListByTypeIDCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        /*▲====: #003*/

        public PCLExamResultTemplate GetPCLExamResultTemplate(long PCLExamResultTemplateID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamResultTemplateGetByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultTemplateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamResultTemplateID));

                cn.Open();
                PCLExamResultTemplate retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetPCLExamResultTemplateFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public bool AddPCLExamResultTemplate(PCLExamResultTemplate entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamResultTemplateInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTemplateName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PCLExamTemplateName));
                cmd.AddParameter("@PCLExamGroupTemplateResultID", SqlDbType.Int, ConvertNullObjectToDBNull(entity.PCLExamGroupTemplateResultID));
                cmd.AddParameter("@ResultContent", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ResultContent));
                cmd.AddParameter("@Descriptions", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Descriptions));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdatePCLExamResultTemplate(PCLExamResultTemplate entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamResultTemplateUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultTemplateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLExamResultTemplateID));
                cmd.AddParameter("@PCLExamTemplateName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PCLExamTemplateName));
                cmd.AddParameter("@PCLExamGroupTemplateResultID", SqlDbType.Int, ConvertNullObjectToDBNull(entity.PCLExamGroupTemplateResultID));
                cmd.AddParameter("@ResultContent", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ResultContent));
                cmd.AddParameter("@Descriptions", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Descriptions));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeletePCLExamResultTemplate(PCLExamResultTemplate entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLExamResultTemplateDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamResultTemplateID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLExamResultTemplateID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion






        #region UltraResParams_EchoCardiography
        public UltraResParams_EchoCardiography GetUltraResParams_EchoCardiography(long UltraResParams_EchoCardiographyID, long PCLImgResultID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_EchoCardiographyGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_EchoCardiographyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_EchoCardiographyID));
                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLImgResultID));

                cn.Open();
                UltraResParams_EchoCardiography retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetUltraResParams_EchoCardiographyFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public string GetUltraResParams_EchoCardiographyResult(long UltraResParams_EchoCardiographyID, long PCLImgResultID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_EchoCardiographyGetResult", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_EchoCardiographyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_EchoCardiographyID));
                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLImgResultID));

                cn.Open();
                string resVal = "";

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    if (reader.HasColumn("Conclusion") && reader["Conclusion"] != DBNull.Value)
                    {
                        resVal = reader["Conclusion"].ToString();
                    }

                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return resVal;
            }
        }

        public bool AddUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_EchoCardiographyInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                //cmd.AddParameter("@UltraResParams_EchoCardiographyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_EchoCardiographyID));
                cmd.AddParameter("@TM_Vlt_Ttr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Vlt_Ttr));
                cmd.AddParameter("@TM_Dktt_Ttr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Dktt_Ttr));
                cmd.AddParameter("@TM_Tstt_Ttr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Tstt_Ttr));
                cmd.AddParameter("@TM_Vlt_Tt", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Vlt_Tt));
                cmd.AddParameter("@TM_Dktt_Tt", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Dktt_Tt));
                cmd.AddParameter("@TM_Tstt_Tt", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Tstt_Tt));
                cmd.AddParameter("@TM_Pxcr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Pxcr));
                cmd.AddParameter("@TM_Pxtm", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Pxtm));
                cmd.AddParameter("@TM_RV", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_RV));
                cmd.AddParameter("@TM_Ao", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Ao));
                cmd.AddParameter("@TM_La", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_La));
                cmd.AddParameter("@TM_Ssa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Ssa));
                cmd.AddParameter("@V_2D_Situs", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_2D_Situs));
                cmd.AddParameter("@TwoD_Veins", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Veins));
                cmd.AddParameter("@TwoD_Ivc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Ivc));
                cmd.AddParameter("@TwoD_Svc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Svc));
                cmd.AddParameter("@TwoD_Tvi", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Tvi));
                cmd.AddParameter("@V_2D_Lsvc", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_2D_Lsvc));
                cmd.AddParameter("@V_2D_Azygos", SqlDbType.SmallInt, ConvertNullObjectToDBNull(entity.V_2D_Azygos));
                cmd.AddParameter("@TwoD_Pv", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Pv));
                cmd.AddParameter("@TwoD_Azygos", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Azygos));
                cmd.AddParameter("@TwoD_Atria", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Atria));
                cmd.AddParameter("@TwoD_Valves", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Valves));
                cmd.AddParameter("@TwoD_Cd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Cd));
                cmd.AddParameter("@TwoD_Ma", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Ma));
                cmd.AddParameter("@TwoD_MitralArea", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_MitralArea));
                cmd.AddParameter("@TwoD_Ta", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Ta));
                cmd.AddParameter("@TwoD_LSVC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_LSVC));
                cmd.AddParameter("@TwoD_Ventricles", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Ventricles));
                cmd.AddParameter("@TwoD_Aorte", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Aorte));
                cmd.AddParameter("@TwoD_Asc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Asc));
                cmd.AddParameter("@TwoD_Cr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Cr));
                cmd.AddParameter("@TwoD_Is", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Is));
                cmd.AddParameter("@TwoD_Abd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Abd));
                cmd.AddParameter("@TwoD_D2", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_D2));
                cmd.AddParameter("@TwoD_Ann", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Ann));
                cmd.AddParameter("@TwoD_Tap", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Tap));
                cmd.AddParameter("@TwoD_Rpa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Rpa));
                cmd.AddParameter("@TwoD_Lpa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Lpa));
                cmd.AddParameter("@TwoD_Pericarde", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Pericarde));
                cmd.AddParameter("@TwoD_Pa", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Pa));
                cmd.AddParameter("@TwoD_Others", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Others));
                cmd.AddParameter("@DOPPLER_Mitral_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_VelMax));
                cmd.AddParameter("@DOPPLER_Mitral_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_GdMax));
                cmd.AddParameter("@DOPPLER_Mitral_Ms", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Ms));
                cmd.AddParameter("@V_DOPPLER_Mitral_Mr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.V_DOPPLER_Mitral_Mr));
                cmd.AddParameter("@V_DOPPLER_Mitral_Ea", SqlDbType.Int, ConvertNullObjectToDBNull(entity.V_DOPPLER_Mitral_Ea));
                cmd.AddParameter("@DOPPLER_Mitral_Ea", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Ea));
                cmd.AddParameter("@DOPPLER_Mitral_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Moy));
                cmd.AddParameter("@DOPPLER_Mitral_Sm", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Sm));
                //cmd.AddParameter("@V_DOPPLER_Mitral_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Mitral_Grade));
                cmd.AddParameter("@V_DOPPLER_Mitral_Grade", SqlDbType.BigInt, entity.LDOPPLER_Mitral_Grade != null ? entity.LDOPPLER_Mitral_Grade.LookupID : 0);
                cmd.AddParameter("@DOPPLER_Aortic_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_VelMax));
                cmd.AddParameter("@DOPPLER_Aortic_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_GdMax));
                cmd.AddParameter("@DOPPLER_Aortic_As", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_As));
                cmd.AddParameter("@V_DOPPLER_Aortic_Ar", SqlDbType.Float, ConvertNullObjectToDBNull(entity.V_DOPPLER_Aortic_Ar));
                cmd.AddParameter("@DOPPLER_Aortic_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_Moy));
                cmd.AddParameter("@DOPPLER_Aortic_SAo", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_SAo));
                //cmd.AddParameter("@V_DOPPLER_Aortic_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Aortic_Grade));
                cmd.AddParameter("@V_DOPPLER_Aortic_Grade", SqlDbType.BigInt, entity.LDOPPLER_Aortic_Grade != null ? entity.LDOPPLER_Aortic_Grade.LookupID : 0);
                cmd.AddParameter("@DOPPLER_Aortic_PHT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_PHT));
                cmd.AddParameter("@DOPPLER_Aortic_Dfo", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_Dfo));
                cmd.AddParameter("@DOPPLER_Aortic_Edtd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_Edtd));
                cmd.AddParameter("@DOPPLER_Aortic_ExtSpat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_ExtSpat));
                cmd.AddParameter("@DOPPLER_Tricuspid_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_VelMax));
                cmd.AddParameter("@V_DOPPLER_Tricuspid_Tr", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Tricuspid_Tr));
                cmd.AddParameter("@DOPPLER_Tricuspid_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_GdMax));
                cmd.AddParameter("@DOPPLER_Tricuspid_Paps", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_Paps));
                cmd.AddParameter("@DOPPLER_Tricuspid_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_Moy));
                cmd.AddParameter("@DOPPLER_Pulmonary_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_VelMax));
                cmd.AddParameter("@DOPPLER_Pulmonary_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_GdMax));
                cmd.AddParameter("@V_DOPPLER_Pulmonary_Pr", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Pulmonary_Pr));
                cmd.AddParameter("@DOPPLER_Pulmonary_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Moy));
                cmd.AddParameter("@DOPPLER_Pulmonary_Papm", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Papm));
                cmd.AddParameter("@DOPPLER_Pulmonary_Papd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Papd));
                cmd.AddParameter("@DOPPLER_Pulmonary_Orthers", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Orthers));

                //cmd.AddParameter("@V_DOPPLER_Tricuspid_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Tricuspid_Grade));
                cmd.AddParameter("@V_DOPPLER_Tricuspid_Grade", SqlDbType.BigInt, entity.LDOPPLER_Tricuspid_Grade != null ? entity.LDOPPLER_Tricuspid_Grade.LookupID : 0);

                //cmd.AddParameter("@V_DOPPLER_Pulmonary_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Pulmonary_Grade));
                cmd.AddParameter("@V_DOPPLER_Pulmonary_Grade", SqlDbType.BigInt, entity.LDOPPLER_Pulmonary_Grade != null ? entity.LDOPPLER_Pulmonary_Grade.LookupID : 0);

                cmd.AddParameter("@Conclusion", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Conclusion));
                cmd.AddParameter("@TabIndex", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TabIndex));
                cmd.AddParameter("@Tab1_TM_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab1_TM_Update_Required));
                cmd.AddParameter("@Tab2_2D_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab2_2D_Update_Required));
                cmd.AddParameter("@Tab3_Doppler_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab3_Doppler_Update_Required));
                cmd.AddParameter("@Tab4_Conclusion_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab4_Conclusion_Update_Required));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_EchoCardiographyUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_EchoCardiographyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_EchoCardiographyID));
                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@TM_Vlt_Ttr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Vlt_Ttr));
                cmd.AddParameter("@TM_Dktt_Ttr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Dktt_Ttr));
                cmd.AddParameter("@TM_Tstt_Ttr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Tstt_Ttr));
                cmd.AddParameter("@TM_Vlt_Tt", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Vlt_Tt));
                cmd.AddParameter("@TM_Dktt_Tt", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Dktt_Tt));
                cmd.AddParameter("@TM_Tstt_Tt", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Tstt_Tt));
                cmd.AddParameter("@TM_Pxcr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Pxcr));
                cmd.AddParameter("@TM_Pxtm", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Pxtm));
                cmd.AddParameter("@TM_RV", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_RV));
                cmd.AddParameter("@TM_Ao", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Ao));
                cmd.AddParameter("@TM_La", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_La));
                cmd.AddParameter("@TM_Ssa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TM_Ssa));
                cmd.AddParameter("@V_2D_Situs", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_2D_Situs));
                cmd.AddParameter("@TwoD_Veins", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Veins));
                cmd.AddParameter("@TwoD_Ivc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Ivc));
                cmd.AddParameter("@TwoD_Svc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Svc));
                cmd.AddParameter("@TwoD_Tvi", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Tvi));
                cmd.AddParameter("@V_2D_Lsvc", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_2D_Lsvc));
                cmd.AddParameter("@V_2D_Azygos", SqlDbType.SmallInt, ConvertNullObjectToDBNull(entity.V_2D_Azygos));
                cmd.AddParameter("@TwoD_Pv", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Pv));
                cmd.AddParameter("@TwoD_Azygos", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Azygos));
                cmd.AddParameter("@TwoD_Atria", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Atria));
                cmd.AddParameter("@TwoD_Valves", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Valves));
                cmd.AddParameter("@TwoD_Cd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Cd));
                cmd.AddParameter("@TwoD_Ma", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Ma));
                cmd.AddParameter("@TwoD_MitralArea", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_MitralArea));
                cmd.AddParameter("@TwoD_Ta", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Ta));
                cmd.AddParameter("@TwoD_LSVC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_LSVC));
                cmd.AddParameter("@TwoD_Ventricles", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Ventricles));
                cmd.AddParameter("@TwoD_Aorte", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Aorte));
                cmd.AddParameter("@TwoD_Asc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Asc));
                cmd.AddParameter("@TwoD_Cr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Cr));
                cmd.AddParameter("@TwoD_Is", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Is));
                cmd.AddParameter("@TwoD_Abd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Abd));
                cmd.AddParameter("@TwoD_D2", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_D2));
                cmd.AddParameter("@TwoD_Ann", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Ann));
                cmd.AddParameter("@TwoD_Tap", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Tap));
                cmd.AddParameter("@TwoD_Rpa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Rpa));
                cmd.AddParameter("@TwoD_Lpa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwoD_Lpa));
                cmd.AddParameter("@TwoD_Pericarde", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Pericarde));
                cmd.AddParameter("@TwoD_Pa", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Pa));
                cmd.AddParameter("@TwoD_Others", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TwoD_Others));
                cmd.AddParameter("@DOPPLER_Mitral_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_VelMax));
                cmd.AddParameter("@DOPPLER_Mitral_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_GdMax));
                cmd.AddParameter("@DOPPLER_Mitral_Ms", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Ms));
                cmd.AddParameter("@V_DOPPLER_Mitral_Mr", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.V_DOPPLER_Mitral_Mr));
                cmd.AddParameter("@V_DOPPLER_Mitral_Ea", SqlDbType.Int, ConvertNullObjectToDBNull(entity.V_DOPPLER_Mitral_Ea));
                cmd.AddParameter("@DOPPLER_Mitral_Ea", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Ea));
                cmd.AddParameter("@DOPPLER_Mitral_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Moy));
                cmd.AddParameter("@DOPPLER_Mitral_Sm", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Mitral_Sm));
                //cmd.AddParameter("@V_DOPPLER_Mitral_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Mitral_Grade));
                cmd.AddParameter("@V_DOPPLER_Mitral_Grade", SqlDbType.BigInt, entity.LDOPPLER_Mitral_Grade != null ? entity.LDOPPLER_Mitral_Grade.LookupID : 0);
                cmd.AddParameter("@DOPPLER_Aortic_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_VelMax));
                cmd.AddParameter("@DOPPLER_Aortic_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_GdMax));
                cmd.AddParameter("@DOPPLER_Aortic_As", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_As));
                cmd.AddParameter("@V_DOPPLER_Aortic_Ar", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.V_DOPPLER_Aortic_Ar));
                cmd.AddParameter("@DOPPLER_Aortic_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_Moy));
                cmd.AddParameter("@DOPPLER_Aortic_SAo", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_SAo));
                //cmd.AddParameter("@V_DOPPLER_Aortic_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Aortic_Grade));
                cmd.AddParameter("@V_DOPPLER_Aortic_Grade", SqlDbType.BigInt, entity.LDOPPLER_Aortic_Grade != null ? entity.LDOPPLER_Aortic_Grade.LookupID : 0);
                cmd.AddParameter("@DOPPLER_Aortic_PHT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_PHT));
                cmd.AddParameter("@DOPPLER_Aortic_Dfo", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_Dfo));
                cmd.AddParameter("@DOPPLER_Aortic_Edtd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_Edtd));
                cmd.AddParameter("@DOPPLER_Aortic_ExtSpat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Aortic_ExtSpat));
                cmd.AddParameter("@DOPPLER_Tricuspid_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_VelMax));
                cmd.AddParameter("@V_DOPPLER_Tricuspid_Tr", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.V_DOPPLER_Tricuspid_Tr));
                cmd.AddParameter("@DOPPLER_Tricuspid_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_GdMax));
                cmd.AddParameter("@DOPPLER_Tricuspid_Paps", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_Paps));
                cmd.AddParameter("@DOPPLER_Tricuspid_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Tricuspid_Moy));
                cmd.AddParameter("@DOPPLER_Pulmonary_VelMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_VelMax));
                cmd.AddParameter("@DOPPLER_Pulmonary_GdMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_GdMax));
                cmd.AddParameter("@V_DOPPLER_Pulmonary_Pr", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.V_DOPPLER_Pulmonary_Pr));
                cmd.AddParameter("@DOPPLER_Pulmonary_Moy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Moy));
                cmd.AddParameter("@DOPPLER_Pulmonary_Papm", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Papm));
                cmd.AddParameter("@DOPPLER_Pulmonary_Papd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Papd));
                cmd.AddParameter("@DOPPLER_Pulmonary_Orthers", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.DOPPLER_Pulmonary_Orthers));

                //cmd.AddParameter("@V_DOPPLER_Tricuspid_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Tricuspid_Grade));
                cmd.AddParameter("@V_DOPPLER_Tricuspid_Grade", SqlDbType.BigInt, entity.LDOPPLER_Tricuspid_Grade != null ? entity.LDOPPLER_Tricuspid_Grade.LookupID : 0);
                //cmd.AddParameter("@V_DOPPLER_Pulmonary_Grade", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_DOPPLER_Pulmonary_Grade));
                cmd.AddParameter("@V_DOPPLER_Pulmonary_Grade", SqlDbType.BigInt, entity.LDOPPLER_Pulmonary_Grade != null ? entity.LDOPPLER_Pulmonary_Grade.LookupID : 0);

                cmd.AddParameter("@Conclusion", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Conclusion));
                cmd.AddParameter("@TabIndex", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TabIndex));

                cmd.AddParameter("@Tab1_TM_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab1_TM_Update_Required));
                cmd.AddParameter("@Tab2_2D_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab2_2D_Update_Required));
                cmd.AddParameter("@Tab3_Doppler_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab3_Doppler_Update_Required));
                cmd.AddParameter("@Tab4_Conclusion_UpdReq", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Tab4_Conclusion_Update_Required));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else
                    return false;
            }
        }

        public bool DeleteUltraResParams_EchoCardiography(UltraResParams_EchoCardiography entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_EchoCardiographyDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_EchoCardiographyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_EchoCardiographyID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        //20161214 CMN Begin: Add general inf
        #region UltraResParams_FetalEchocardiography
        public UltraResParams_FetalEchocardiography GetUltraResParams_FetalEchocardiography(long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cn.Open();
                UltraResParams_FetalEchocardiography retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetUltraResParams_FetalEchocardiographyFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        #endregion
        //20161214 CMN End.
        #region UltraResParams_FetalEchocardiography2D

        public UltraResParams_FetalEchocardiography2D GetUltraResParams_FetalEchocardiography2D(long UltraResParams_FetalEchocardiography2DID
                                                                , long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiography2DGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiography2DID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiography2DID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                UltraResParams_FetalEchocardiography2D retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetUltraResParams_FetalEchocardiography2DFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public bool AddUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiography2DInsert", cn);

                cmd.AddParameter("@NTSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NTSize));

                cmd.AddParameter("@NPSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NPSize));

                cmd.AddParameter("@VanVieussensLeftAtrium", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.VanVieussensLeftAtrium));

                cmd.AddParameter("@AtrialSeptalDefect", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.AtrialSeptalDefect));

                cmd.AddParameter("@MitralValveSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MitralValveSize));

                cmd.AddParameter("@TriscupidValveSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TriscupidValveSize));

                cmd.AddParameter("@DifferenceMitralTricuspid", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DifferenceMitralTricuspid));

                cmd.AddParameter("@TPTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TPTTr));

                cmd.AddParameter("@VLTTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.VLTTTr));

                cmd.AddParameter("@TTTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TTTTr));

                cmd.AddParameter("@DKTTTTr_VGd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTTTTr_VGd));

                cmd.AddParameter("@DKTTTT_VGs", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTTTT_VGs));

                cmd.AddParameter("@DKTPTTr_VDd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTPTTr_VDd));

                cmd.AddParameter("@DKTPTT_VDs", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTPTT_VDs));

                cmd.AddParameter("@Systolic", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Systolic));

                cmd.AddParameter("@VentricularSeptalDefect", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.VentricularSeptalDefect));

                cmd.AddParameter("@AortaCompatible", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.AortaCompatible));

                cmd.AddParameter("@AortaSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AortaSize));

                cmd.AddParameter("@PulmonaryArterySize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryArterySize));

                cmd.AddParameter("@AorticArch", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.AorticArch));

                cmd.AddParameter("@AorticCoarctation", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticCoarctation));

                cmd.AddParameter("@HeartRateNomal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.HeartRateNomal));

                cmd.AddParameter("@RequencyHeartRateNomal", SqlDbType.Float, ConvertNullObjectToDBNull(entity.RequencyHeartRateNomal));

                cmd.AddParameter("@PericardialEffusion", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PericardialEffusion));

                cmd.AddParameter("@FetalCardialAxis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FetalCardialAxis));

                cmd.AddParameter("@CardialRateS", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CardialRateS));

                cmd.AddParameter("@LN", SqlDbType.Float, ConvertNullObjectToDBNull(entity.LN));

                cmd.AddParameter("@OrderRecord", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OrderRecord));

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public bool UpdateUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiography2DUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiography2DID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiography2DID));

                cmd.AddParameter("@NTSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NTSize));

                cmd.AddParameter("@NPSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NPSize));

                cmd.AddParameter("@VanVieussensLeftAtrium", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.VanVieussensLeftAtrium));

                cmd.AddParameter("@AtrialSeptalDefect", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.AtrialSeptalDefect));

                cmd.AddParameter("@MitralValveSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MitralValveSize));

                cmd.AddParameter("@TriscupidValveSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TriscupidValveSize));

                cmd.AddParameter("@DifferenceMitralTricuspid", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DifferenceMitralTricuspid));

                cmd.AddParameter("@TPTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TPTTr));

                cmd.AddParameter("@VLTTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.VLTTTr));

                cmd.AddParameter("@TTTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TTTTr));

                cmd.AddParameter("@DKTTTTr_VGd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTTTTr_VGd));

                cmd.AddParameter("@DKTTTT_VGs", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTTTT_VGs));

                cmd.AddParameter("@DKTPTTr_VDd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTPTTr_VDd));

                cmd.AddParameter("@DKTPTT_VDs", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DKTPTT_VDs));

                cmd.AddParameter("@Systolic", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Systolic));

                cmd.AddParameter("@VentricularSeptalDefect", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.VentricularSeptalDefect));

                cmd.AddParameter("@AortaCompatible", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.AortaCompatible));

                cmd.AddParameter("@AortaSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AortaSize));

                cmd.AddParameter("@PulmonaryArterySize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryArterySize));

                cmd.AddParameter("@AorticArch", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.AorticArch));

                cmd.AddParameter("@AorticCoarctation", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticCoarctation));

                cmd.AddParameter("@HeartRateNomal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.HeartRateNomal));

                cmd.AddParameter("@RequencyHeartRateNomal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RequencyHeartRateNomal));

                cmd.AddParameter("@PericardialEffusion", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PericardialEffusion));

                cmd.AddParameter("@FetalCardialAxis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FetalCardialAxis));

                cmd.AddParameter("@CardialRateS", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CardialRateS));

                cmd.AddParameter("@LN", SqlDbType.Float, ConvertNullObjectToDBNull(entity.LN));

                cmd.AddParameter("@OrderRecord", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OrderRecord));

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public bool DeleteUltraResParams_FetalEchocardiography2D(UltraResParams_FetalEchocardiography2D entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiography2DDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@UltraResParams_FetalEchocardiography2DID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiography2DID));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region UltraResParams_FetalEchocardiography Doppler
        public IList<UltraResParams_FetalEchocardiographyDoppler> GetUltraResParams_FetalEchocardiographyDoppler(long UltraResParams_FetalEchocardiographyDopplerID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyDopplerGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyDopplerID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiographyDopplerID));


                cn.Open();
                List<UltraResParams_FetalEchocardiographyDoppler> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetUltraResParams_FetalEchocardiographyDopplerCollectionFromReader(reader);
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public UltraResParams_FetalEchocardiographyDoppler GetUltraResParams_FetalEchocardiographyDopplerByID(
                                    long UltraResParams_FetalEchocardiographyDopplerID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyDopplerGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyDopplerID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiographyDopplerID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));


                cn.Open();
                UltraResParams_FetalEchocardiographyDoppler retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetUltraResParams_FetalEchocardiographyDopplerFromReader(reader);
                }

                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyDopplerInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@MitralValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MitralValve_Vmax));


                cmd.AddParameter("@MitralValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MitralValve_Gdmax));

                cmd.AddParameter("@MitralValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.MitralValve_Open));

                cmd.AddParameter("@MitralValve_Stenosis", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.MitralValve_Stenosis));

                cmd.AddParameter("@TriscupidValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TriscupidValve_Vmax));

                cmd.AddParameter("@TriscupidValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TriscupidValve_Gdmax));

                cmd.AddParameter("@TriscupidValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TriscupidValve_Open));

                cmd.AddParameter("@TriscupidValve_Stenosis", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TriscupidValve_Stenosis));

                cmd.AddParameter("@AorticValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticValve_Vmax));

                cmd.AddParameter("@AorticValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticValve_Gdmax));

                cmd.AddParameter("@AorticValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.AorticValve_Open));

                cmd.AddParameter("@AorticValve_Stenosis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticValve_Stenosis));

                cmd.AddParameter("@PulmonaryValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryValve_Vmax));

                cmd.AddParameter("@PulmonaryValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryValve_Gdmax));

                cmd.AddParameter("@PulmonaryValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PulmonaryValve_Open));

                cmd.AddParameter("@PulmonaryValve_Stenosis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryValve_Stenosis));

                cmd.AddParameter("@AorticCoarctationBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticCoarctationBloodTraffic));

                cmd.AddParameter("@VanViewessensBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.VanViewessensBloodTraffic));

                cmd.AddParameter("@DuctusAteriosusBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DuctusAteriosusBloodTraffic));

                cmd.AddParameter("@DuctusVenosusBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DuctusVenosusBloodTraffic));

                cmd.AddParameter("@PulmonaryVeins_LeftAtrium", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PulmonaryVeins_LeftAtrium));

                cmd.AddParameter("@OrderRecord", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OrderRecord));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public bool UpdateUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyDopplerUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyDopplerID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyDopplerID));

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@MitralValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MitralValve_Vmax));


                cmd.AddParameter("@MitralValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MitralValve_Gdmax));

                cmd.AddParameter("@MitralValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.MitralValve_Open));

                cmd.AddParameter("@MitralValve_Stenosis", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.MitralValve_Stenosis));

                cmd.AddParameter("@TriscupidValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TriscupidValve_Vmax));

                cmd.AddParameter("@TriscupidValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TriscupidValve_Gdmax));

                cmd.AddParameter("@TriscupidValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TriscupidValve_Open));

                cmd.AddParameter("@TriscupidValve_Stenosis", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TriscupidValve_Stenosis));

                cmd.AddParameter("@AorticValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticValve_Vmax));

                cmd.AddParameter("@AorticValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticValve_Gdmax));

                cmd.AddParameter("@AorticValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.AorticValve_Open));

                cmd.AddParameter("@AorticValve_Stenosis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticValve_Stenosis));

                cmd.AddParameter("@PulmonaryValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryValve_Vmax));

                cmd.AddParameter("@PulmonaryValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryValve_Gdmax));

                cmd.AddParameter("@PulmonaryValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PulmonaryValve_Open));

                cmd.AddParameter("@PulmonaryValve_Stenosis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.PulmonaryValve_Stenosis));

                cmd.AddParameter("@AorticCoarctationBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.AorticCoarctationBloodTraffic));

                cmd.AddParameter("@VanViewessensBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.VanViewessensBloodTraffic));

                cmd.AddParameter("@DuctusAteriosusBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DuctusAteriosusBloodTraffic));

                cmd.AddParameter("@DuctusVenosusBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DuctusVenosusBloodTraffic));

                cmd.AddParameter("@PulmonaryVeins_LeftAtrium", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PulmonaryVeins_LeftAtrium));

                cmd.AddParameter("@OrderRecord", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.OrderRecord));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public bool DeleteUltraResParams_FetalEchocardiographyDoppler(UltraResParams_FetalEchocardiographyDoppler entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyDopplerDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@UltraResParams_FetalEchocardiographyDopplerID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyDopplerID));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region UltraResParams_FetalEchocardiographyPostpartum
        public IList<UltraResParams_FetalEchocardiographyPostpartum> GetUltraResParams_FetalEchocardiographyPostpartum(long UltraResParams_FetalEchocardiographyPostpartumID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyPostpartumGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyPostpartumID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiographyPostpartumID));


                cn.Open();
                List<UltraResParams_FetalEchocardiographyPostpartum> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetUltraResParams_FetalEchocardiographyPostpartumCollectionFromReader(reader);
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public UltraResParams_FetalEchocardiographyPostpartum GetUltraResParams_FetalEchocardiographyPostpartumByID(
            long UltraResParams_FetalEchocardiographyPostpartumID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyPostpartumGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyPostpartumID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiographyPostpartumID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                UltraResParams_FetalEchocardiographyPostpartum retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetUltraResParams_FetalEchocardiographyPostpartumFromReader(reader);
                }

                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyPostpartumInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@BabyBirthday", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.BabyBirthday));

                cmd.AddParameter("@BabyWeight", SqlDbType.Float, ConvertNullObjectToDBNull(entity.BabyWeight));

                cmd.AddParameter("@BabySex", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BabySex));

                cmd.AddParameter("@URP_Date", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.URP_Date));

                cmd.AddParameter("@PFO", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PFO));

                cmd.AddParameter("@PCA", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PCA));

                cmd.AddParameter("@AnotherDiagnosic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AnotherDiagnosic));

                cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Notes));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));



                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public bool UpdateUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyPostpartumUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyPostpartumID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyPostpartumID));

                cmd.AddParameter("@BabyBirthday", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.BabyBirthday));

                cmd.AddParameter("@BabyWeight", SqlDbType.Float, ConvertNullObjectToDBNull(entity.BabyWeight));

                cmd.AddParameter("@BabySex", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BabySex));

                cmd.AddParameter("@URP_Date", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.URP_Date));

                cmd.AddParameter("@PFO", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PFO));

                cmd.AddParameter("@PCA", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.PCA));

                cmd.AddParameter("@AnotherDiagnosic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AnotherDiagnosic));

                cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Notes));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public bool DeleteUltraResParams_FetalEchocardiographyPostpartum(UltraResParams_FetalEchocardiographyPostpartum entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyPostpartumDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyPostpartumID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyPostpartumID));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region UltraResParams_FetalEchocardiographyResult
        public IList<UltraResParams_FetalEchocardiographyResult> GetUltraResParams_FetalEchocardiographyResult(long UltraResParams_FetalEchocardiographyResultID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyResultGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiographyResultID));


                cn.Open();
                List<UltraResParams_FetalEchocardiographyResult> retVal = null;

                IDataReader reader = ExecuteReader(cmd);

                retVal = GetUltraResParams_FetalEchocardiographyResultCollectionFromReader(reader);
                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public UltraResParams_FetalEchocardiographyResult GetUltraResParams_FetalEchocardiographyResultByID(
                                    long UltraResParams_FetalEchocardiographyResultID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyResultGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UltraResParams_FetalEchocardiographyResultID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                UltraResParams_FetalEchocardiographyResult retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetUltraResParams_FetalEchocardiographyResultFromReader(reader);
                }

                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyResultInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@CardialAbnormal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CardialAbnormal));

                cmd.AddParameter("@CardialAbnormalDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.CardialAbnormalDetail));

                cmd.AddParameter("@Susgest", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Susgest));

                cmd.AddParameter("@UltraResParamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.UltraResParamDate));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public bool UpdateUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyResultUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyResultID));

                cmd.AddParameter("@CardialAbnormal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CardialAbnormal));

                cmd.AddParameter("@CardialAbnormalDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.CardialAbnormalDetail));

                cmd.AddParameter("@Susgest", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Susgest));

                cmd.AddParameter("@UltraResParamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.UltraResParamDate));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public bool DeleteUltraResParams_FetalEchocardiographyResult(UltraResParams_FetalEchocardiographyResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUltraResParams_FetalEchocardiographyResultDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UltraResParams_FetalEchocardiographyResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyResultID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }


        #endregion

        #region Abdominal Ultrasound

        public bool InsertAbdominalUltrasoundResult(AbdominalUltrasound entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spInsertAbdominalUltrasoundResult", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PatientPCLReqID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                    cmd.AddParameter("@Liver", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Liver));
                    cmd.AddParameter("@Gallbladder", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Gallbladder));
                    cmd.AddParameter("@Pancreas", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Pancreas));
                    cmd.AddParameter("@Spleen", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Spleen));
                    cmd.AddParameter("@RightKidney", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RightKidney));
                    cmd.AddParameter("@LeftKidney", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LeftKidney));
                    cmd.AddParameter("@Bladder", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Bladder));
                    cmd.AddParameter("@Prostate", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Prostate));
                    cmd.AddParameter("@Uterus", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Uterus));
                    cmd.AddParameter("@RightOvary", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RightOvary));
                    cmd.AddParameter("@LeftOvary", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LeftOvary));
                    cmd.AddParameter("@PeritonealFluid", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PeritonealFluid));
                    cmd.AddParameter("@PleuralFluid", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PleuralFluid));
                    cmd.AddParameter("@AbdominalAortic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AbdominalAortic));
                    cmd.AddParameter("@Conclusion", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Conclusion));

                    cn.Open();

                    int retVal = ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    if (retVal > 0)
                    {
                        return true;
                    }
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool UpdateAbdominalUltrasoundResult(AbdominalUltrasound entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateAbdominalUltrasoundResult", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@AbdominalUltrasoundID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.AbdominalUltrasoundID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                    cmd.AddParameter("@Liver", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Liver));
                    cmd.AddParameter("@Gallbladder", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Gallbladder));
                    cmd.AddParameter("@Pancreas", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Pancreas));
                    cmd.AddParameter("@Spleen", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Spleen));
                    cmd.AddParameter("@RightKidney", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RightKidney));
                    cmd.AddParameter("@LeftKidney", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LeftKidney));
                    cmd.AddParameter("@Bladder", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Bladder));
                    cmd.AddParameter("@Prostate", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Prostate));
                    cmd.AddParameter("@Uterus", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Uterus));
                    cmd.AddParameter("@RightOvary", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RightOvary));
                    cmd.AddParameter("@LeftOvary", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LeftOvary));
                    cmd.AddParameter("@PeritonealFluid", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PeritonealFluid));
                    cmd.AddParameter("@PleuralFluid", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.PleuralFluid));
                    cmd.AddParameter("@AbdominalAortic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.AbdominalAortic));
                    cmd.AddParameter("@Conclusion", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Conclusion));

                    cn.Open();

                    int retVal = ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    if (retVal > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public AbdominalUltrasound GetAbdominalUltrasoundResult(long PatientPCLReqID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetAbdominalUltrasoundResult", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                    cn.Open();
                    AbdominalUltrasound retVal = null;

                    IDataReader reader = ExecuteReader(cmd);
                    while (reader.Read())
                    {
                        retVal = GetAbdominalUltrasoundResultFromReader(reader);
                    }

                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return retVal;
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        #endregion


        #region URP_FE_Exam

        public URP_FE_Exam GetURP_FE_Exam(long URP_FE_ExamID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_ExamGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_ExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_ExamID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_Exam retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_ExamFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public bool AddURP_FE_Exam(URP_FE_Exam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_ExamInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));



                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@CaoHuyetAp", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CaoHuyetAp));
                cmd.AddParameter("@CaoHuyetApDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.CaoHuyetApDetail));
                cmd.AddParameter("@Cholesterol", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Cholesterol));
                cmd.AddParameter("@Triglyceride", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Triglyceride));
                cmd.AddParameter("@HDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HDL));
                cmd.AddParameter("@LDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.LDL));
                cmd.AddParameter("@TieuDuong", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TieuDuong));
                cmd.AddParameter("@TieuDuongDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TieuDuongDetail));
                cmd.AddParameter("@ThuocLa", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThuocLa));
                cmd.AddParameter("@Detail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Detail));
                cmd.AddParameter("@ThuocNguaThai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThuocNguaThai));
                cmd.AddParameter("@ThuocNguaThaiDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThuocNguaThaiDetail));

                cmd.AddParameter("NhanApMP", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.NhanApMP));
                cmd.AddParameter("NhanApMT", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.NhanApMT));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_Exam(URP_FE_Exam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_ExamUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@URP_FE_ExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_ExamID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@CaoHuyetAp", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CaoHuyetAp));
                cmd.AddParameter("@CaoHuyetApDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.CaoHuyetApDetail));
                cmd.AddParameter("@Cholesterol", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Cholesterol));
                cmd.AddParameter("@Triglyceride", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Triglyceride));
                cmd.AddParameter("@HDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HDL));
                cmd.AddParameter("@LDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.LDL));
                cmd.AddParameter("@TieuDuong", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TieuDuong));
                cmd.AddParameter("@TieuDuongDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TieuDuongDetail));
                cmd.AddParameter("@ThuocLa", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThuocLa));
                cmd.AddParameter("@Detail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Detail));
                cmd.AddParameter("@ThuocNguaThai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThuocNguaThai));
                cmd.AddParameter("@ThuocNguaThaiDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThuocNguaThaiDetail));

                cmd.AddParameter("NhanApMP", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.NhanApMP));
                cmd.AddParameter("NhanApMT", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.NhanApMT));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_Exam(URP_FE_Exam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_ExamDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_ExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_ExamID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_Oesophagienne
        public URP_FE_Oesophagienne GetURP_FE_Oesophagienne(long URP_FE_OesophagienneID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_OesophagienneID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_OesophagienneID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_Oesophagienne retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_OesophagienneFromReader(reader);
                }

                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddURP_FE_Oesophagienne(URP_FE_Oesophagienne entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@ChiDinh", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChiDinh));
                //cmd.AddParameter("@ChanDoanThanhNguc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChanDoanThanhNguc));
                //cmd.AddParameter("@V_ChanDoanThanhNgucID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_ChanDoanThanhNgucID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_Oesophagienne(URP_FE_Oesophagienne entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@URP_FE_OesophagienneID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_OesophagienneID));
                cmd.AddParameter("@ChiDinh", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChiDinh));
                //cmd.AddParameter("@ChanDoanThanhNguc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChanDoanThanhNguc));
                //cmd.AddParameter("@V_ChanDoanThanhNgucID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_ChanDoanThanhNgucID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_Oesophagienne(URP_FE_Oesophagienne entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_OesophagienneID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_OesophagienneID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region URP_FE_OesophagienneCheck
        public URP_FE_OesophagienneCheck GetURP_FE_OesophagienneCheck(long URP_FE_OesophagienneCheckID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneCheckGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_OesophagienneCheckID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_OesophagienneCheckID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_OesophagienneCheck retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_OesophagienneCheckFromReader(reader);
                }

                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneCheckInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@CatNghia", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CatNghia));
                cmd.AddParameter("@NuotNghen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NuotNghen));
                cmd.AddParameter("@NuotDau", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NuotDau));
                cmd.AddParameter("@OiMau", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.OiMau));
                cmd.AddParameter("@XaTriTrungThat", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.XaTriTrungThat));
                cmd.AddParameter("@CotSongCo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CotSongCo));
                cmd.AddParameter("@ChanThuongLongNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChanThuongLongNguc));
                cmd.AddParameter("@LanKhamNoiSoiGanDay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.LanKhamNoiSoiGanDay));
                cmd.AddParameter("@DiUngThuoc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DiUngThuoc));
                cmd.AddParameter("@NghienRuou", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NghienRuou));
                cmd.AddParameter("@BiTieu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BiTieu));
                cmd.AddParameter("@TangNhanApGocHep", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TangNhanApGocHep));
                cmd.AddParameter("@Suyen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Suyen));
                cmd.AddParameter("@LanAnSauCung", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.LanAnSauCung));
                cmd.AddParameter("@RangGiaHamGia", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RangGiaHamGia));
                cmd.AddParameter("@HuyetApTT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApTT));
                cmd.AddParameter("@HuyetApTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApTTr));
                cmd.AddParameter("@Mach", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Mach));
                cmd.AddParameter("@DoBaoHoaOxy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DoBaoHoaOxy));
                cmd.AddParameter("@ThucHienDuongTruyenTinhMach", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThucHienDuongTruyenTinhMach));
                cmd.AddParameter("@KiemTraDauDoSieuAm", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.KiemTraDauDoSieuAm));
                cmd.AddParameter("@ChinhDauDoTrungTinh", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChinhDauDoTrungTinh));
                cmd.AddParameter("@TeMeBenhNhan", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TeMeBenhNhan));
                cmd.AddParameter("@DatBenhNhanNghiengTrai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DatBenhNhanNghiengTrai));
                cmd.AddParameter("@CotDay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CotDay));
                cmd.AddParameter("@BenhNhanThoaiMai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BenhNhanThoaiMai));
                cmd.AddParameter("@BoiTronDauDo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BoiTronDauDo));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneCheckUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_OesophagienneCheckID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_OesophagienneCheckID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@CatNghia", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CatNghia));
                cmd.AddParameter("@NuotNghen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NuotNghen));
                cmd.AddParameter("@NuotDau", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NuotDau));
                cmd.AddParameter("@OiMau", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.OiMau));
                cmd.AddParameter("@XaTriTrungThat", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.XaTriTrungThat));
                cmd.AddParameter("@CotSongCo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CotSongCo));
                cmd.AddParameter("@ChanThuongLongNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChanThuongLongNguc));
                cmd.AddParameter("@LanKhamNoiSoiGanDay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.LanKhamNoiSoiGanDay));
                cmd.AddParameter("@DiUngThuoc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DiUngThuoc));
                cmd.AddParameter("@NghienRuou", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NghienRuou));
                cmd.AddParameter("@BiTieu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BiTieu));
                cmd.AddParameter("@TangNhanApGocHep", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TangNhanApGocHep));
                cmd.AddParameter("@Suyen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.Suyen));
                cmd.AddParameter("@LanAnSauCung", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.LanAnSauCung));
                cmd.AddParameter("@RangGiaHamGia", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RangGiaHamGia));
                cmd.AddParameter("@HuyetApTT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApTT));
                cmd.AddParameter("@HuyetApTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApTTr));
                cmd.AddParameter("@Mach", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Mach));
                cmd.AddParameter("@DoBaoHoaOxy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DoBaoHoaOxy));
                cmd.AddParameter("@ThucHienDuongTruyenTinhMach", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThucHienDuongTruyenTinhMach));
                cmd.AddParameter("@KiemTraDauDoSieuAm", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.KiemTraDauDoSieuAm));
                cmd.AddParameter("@ChinhDauDoTrungTinh", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChinhDauDoTrungTinh));
                cmd.AddParameter("@TeMeBenhNhan", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TeMeBenhNhan));
                cmd.AddParameter("@DatBenhNhanNghiengTrai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DatBenhNhanNghiengTrai));
                cmd.AddParameter("@CotDay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.CotDay));
                cmd.AddParameter("@BenhNhanThoaiMai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BenhNhanThoaiMai));
                cmd.AddParameter("@BoiTronDauDo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.BoiTronDauDo));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_OesophagienneCheck(URP_FE_OesophagienneCheck entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneCheckDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_OesophagienneCheckID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_OesophagienneCheckID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region URP_FE_OesophagienneDiagnosis

        public URP_FE_OesophagienneDiagnosis GetURP_FE_OesophagienneDiagnosis(long URP_FE_OesophagienneDiagnosisID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneDiagnosisGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_OesophagienneDiagnosisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_OesophagienneDiagnosisID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_OesophagienneDiagnosis retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_OesophagienneDiagnosisFromReader(reader);
                }

                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneDiagnosisInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@ChanDoanQuaThucQuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChanDoanQuaThucQuan));
                //cmd.AddParameter("@V_ChanDoanQuaThucQuanID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_ChanDoanQuaThucQuanID));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneDiagnosisUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_OesophagienneDiagnosisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_OesophagienneDiagnosisID));
                cmd.AddParameter("@ChanDoanQuaThucQuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChanDoanQuaThucQuan));
                //cmd.AddParameter("@V_ChanDoanQuaThucQuanID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_ChanDoanQuaThucQuanID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_OesophagienneDiagnosis(URP_FE_OesophagienneDiagnosis entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_OesophagienneDiagnosisDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_OesophagienneDiagnosisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_OesophagienneDiagnosisID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDipyridamole

        public URP_FE_StressDipyridamole GetURP_FE_StressDipyridamole(long URP_FE_StressDipyridamoleID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDipyridamoleID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDipyridamole retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDipyridamoleFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public bool AddURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@TanSoTimCanDat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTimCanDat));
                cmd.AddParameter("@TNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TNP_HuyetAp_TT));
                cmd.AddParameter("@TNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TNP_HuyetAp_TTr));
                cmd.AddParameter("@TNP_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TNP_TanSoTim));
                cmd.AddParameter("@TNP_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TNP_TacDungPhu));
                cmd.AddParameter("@TruyenDipyridamole056_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipyridamole056_DungLuong));
                cmd.AddParameter("@TruyenDipy056_P2_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_HuyetAp_TT));
                cmd.AddParameter("@TruyenDipy056_P2_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_HuyetAp_TTr));
                cmd.AddParameter("@TruyenDipy056_P2_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_TanSoTim));
                cmd.AddParameter("@TruyenDipy056_P2_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_TacDungPhu));
                cmd.AddParameter("@TruyenDipy056_P4_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_HuyetAp_TT));
                cmd.AddParameter("@TruyenDipy056_P4_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_HuyetAp_TTr));
                cmd.AddParameter("@TruyenDipy056_P4_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_TanSoTim));
                cmd.AddParameter("@TruyenDipy056_P4_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_TacDungPhu));
                cmd.AddParameter("@SauLieuDauP6_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieuDauP6_HuyetAp_TT));
                cmd.AddParameter("@SauLieuDauP6_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieuDauP6_HuyetAp_TTr));
                cmd.AddParameter("@SauLieuDauP6_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieuDauP6_TanSoTim));
                cmd.AddParameter("@SauLieuDauP6_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.SauLieuDauP6_TacDungPhu));
                cmd.AddParameter("@TruyenDipyridamole028_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipyridamole028_DungLuong));
                cmd.AddParameter("@TruyenDipy028_P8_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_HuyetAp_TT));
                cmd.AddParameter("@TruyenDipy028_P8_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_HuyetAp_TTr));
                cmd.AddParameter("@TruyenDipy028_P8_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_TanSoTim));
                cmd.AddParameter("@TruyenDipy028_P8_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_TacDungPhu));
                cmd.AddParameter("@SauLieu2P10_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieu2P10_HuyetAp_TT));
                cmd.AddParameter("@SauLieu2P10_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieu2P10_HuyetAp_TTr));
                cmd.AddParameter("@SauLieu2P10_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieu2P10_TanSoTim));
                cmd.AddParameter("@SauLieu2P10_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.SauLieu2P10_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP12_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP12_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP12_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP12_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP12_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP12_TanSoTim));
                cmd.AddParameter("@ThemAtropineP12_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP12_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP13_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP13_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP13_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP13_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP13_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP13_TanSoTim));
                cmd.AddParameter("@ThemAtropineP13_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP13_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP14_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP14_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP14_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP14_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP14_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP14_TanSoTim));
                cmd.AddParameter("@ThemAtropineP14_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP14_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP15_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP15_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP15_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP15_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP15_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP15_TanSoTim));
                cmd.AddParameter("@ThemAtropineP15_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP15_TacDungPhu));
                cmd.AddParameter("@TheoDoiAtropineP16_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_HuyetAp_TT));
                cmd.AddParameter("@TheoDoiAtropineP16_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_HuyetAp_TTr));
                cmd.AddParameter("@TheoDoiAtropineP16_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_TanSoTim));
                cmd.AddParameter("@TheoDoiAtropineP16_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_TacDungPhu));
                cmd.AddParameter("@ThemAminophyline_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_DungLuong));
                cmd.AddParameter("@ThemAminophyline_Phut", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_Phut));
                cmd.AddParameter("@ThemAminophyline_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_HuyetAp_TT));
                cmd.AddParameter("@ThemAminophyline_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_HuyetAp_TTr));
                cmd.AddParameter("@ThemAminophyline_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_TanSoTim));
                cmd.AddParameter("@ThemAminophyline_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAminophyline_TacDungPhu));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDipyridamoleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@TanSoTimCanDat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTimCanDat));
                cmd.AddParameter("@TNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TNP_HuyetAp_TT));
                cmd.AddParameter("@TNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TNP_HuyetAp_TTr));
                cmd.AddParameter("@TNP_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TNP_TanSoTim));
                cmd.AddParameter("@TNP_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TNP_TacDungPhu));
                cmd.AddParameter("@TruyenDipyridamole056_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipyridamole056_DungLuong));
                cmd.AddParameter("@TruyenDipy056_P2_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_HuyetAp_TT));
                cmd.AddParameter("@TruyenDipy056_P2_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_HuyetAp_TTr));
                cmd.AddParameter("@TruyenDipy056_P2_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_TanSoTim));
                cmd.AddParameter("@TruyenDipy056_P2_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TruyenDipy056_P2_TacDungPhu));
                cmd.AddParameter("@TruyenDipy056_P4_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_HuyetAp_TT));
                cmd.AddParameter("@TruyenDipy056_P4_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_HuyetAp_TTr));
                cmd.AddParameter("@TruyenDipy056_P4_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_TanSoTim));
                cmd.AddParameter("@TruyenDipy056_P4_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TruyenDipy056_P4_TacDungPhu));
                cmd.AddParameter("@SauLieuDauP6_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieuDauP6_HuyetAp_TT));
                cmd.AddParameter("@SauLieuDauP6_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieuDauP6_HuyetAp_TTr));
                cmd.AddParameter("@SauLieuDauP6_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieuDauP6_TanSoTim));
                cmd.AddParameter("@SauLieuDauP6_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.SauLieuDauP6_TacDungPhu));
                cmd.AddParameter("@TruyenDipyridamole028_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipyridamole028_DungLuong));
                cmd.AddParameter("@TruyenDipy028_P8_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_HuyetAp_TT));
                cmd.AddParameter("@TruyenDipy028_P8_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_HuyetAp_TTr));
                cmd.AddParameter("@TruyenDipy028_P8_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_TanSoTim));
                cmd.AddParameter("@TruyenDipy028_P8_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TruyenDipy028_P8_TacDungPhu));
                cmd.AddParameter("@SauLieu2P10_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieu2P10_HuyetAp_TT));
                cmd.AddParameter("@SauLieu2P10_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieu2P10_HuyetAp_TTr));
                cmd.AddParameter("@SauLieu2P10_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.SauLieu2P10_TanSoTim));
                cmd.AddParameter("@SauLieu2P10_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.SauLieu2P10_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP12_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP12_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP12_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP12_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP12_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP12_TanSoTim));
                cmd.AddParameter("@ThemAtropineP12_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP12_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP13_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP13_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP13_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP13_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP13_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP13_TanSoTim));
                cmd.AddParameter("@ThemAtropineP13_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP13_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP14_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP14_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP14_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP14_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP14_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP14_TanSoTim));
                cmd.AddParameter("@ThemAtropineP14_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP14_TacDungPhu));
                cmd.AddParameter("@ThemAtropineP15_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP15_HuyetAp_TT));
                cmd.AddParameter("@ThemAtropineP15_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP15_HuyetAp_TTr));
                cmd.AddParameter("@ThemAtropineP15_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAtropineP15_TanSoTim));
                cmd.AddParameter("@ThemAtropineP15_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAtropineP15_TacDungPhu));
                cmd.AddParameter("@TheoDoiAtropineP16_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_HuyetAp_TT));
                cmd.AddParameter("@TheoDoiAtropineP16_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_HuyetAp_TTr));
                cmd.AddParameter("@TheoDoiAtropineP16_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_TanSoTim));
                cmd.AddParameter("@TheoDoiAtropineP16_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TheoDoiAtropineP16_TacDungPhu));
                cmd.AddParameter("@ThemAminophyline_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_DungLuong));
                cmd.AddParameter("@ThemAminophyline_Phut", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_Phut));
                cmd.AddParameter("@ThemAminophyline_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_HuyetAp_TT));
                cmd.AddParameter("@ThemAminophyline_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_HuyetAp_TTr));
                cmd.AddParameter("@ThemAminophyline_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThemAminophyline_TanSoTim));
                cmd.AddParameter("@ThemAminophyline_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThemAminophyline_TacDungPhu));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_StressDipyridamole(URP_FE_StressDipyridamole entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDipyridamoleElectrocardiogram

        public URP_FE_StressDipyridamoleElectrocardiogram GetURP_FE_StressDipyridamoleElectrocardiogram(long URP_FE_StressDipyridamoleElectrocardiogramID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleElectrocardiogramGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDipyridamoleElectrocardiogramID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDipyridamoleElectrocardiogram retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDipyridamoleElectrocardiogramFromReader(reader);
                }

                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleElectrocardiogramInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@DieuTriConDauThatNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriConDauThatNguc));
                cmd.AddParameter("@DieuTriDIGITALIS", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriDIGITALIS));
                cmd.AddParameter("@LyDoKhongThucHienDuoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LyDoKhongThucHienDuoc));
                cmd.AddParameter("@MucGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MucGangSuc));
                cmd.AddParameter("@ThoiGianGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThoiGianGangSuc));
                cmd.AddParameter("@TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTim));
                cmd.AddParameter("@HuyetApToiDa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApToiDa));
                cmd.AddParameter("@ConDauThatNguc", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ConDauThatNguc));
                cmd.AddParameter("@STChenhXuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.STChenhXuong));
                cmd.AddParameter("@RoiLoanNhipTim", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhipTim));
                cmd.AddParameter("@RoiLoanNhipTimChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipTimChiTiet));
                cmd.AddParameter("@XetNghiemKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.XetNghiemKhac));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleElectrocardiogramUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@URP_FE_StressDipyridamoleElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleElectrocardiogramID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@DieuTriConDauThatNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriConDauThatNguc));
                cmd.AddParameter("@DieuTriDIGITALIS", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriDIGITALIS));
                cmd.AddParameter("@LyDoKhongThucHienDuoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LyDoKhongThucHienDuoc));
                cmd.AddParameter("@MucGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MucGangSuc));
                cmd.AddParameter("@ThoiGianGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThoiGianGangSuc));
                cmd.AddParameter("@TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTim));
                cmd.AddParameter("@HuyetApToiDa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApToiDa));
                cmd.AddParameter("@ConDauThatNguc", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ConDauThatNguc));
                cmd.AddParameter("@STChenhXuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.STChenhXuong));
                cmd.AddParameter("@RoiLoanNhipTim", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhipTim));
                cmd.AddParameter("@RoiLoanNhipTimChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipTimChiTiet));
                cmd.AddParameter("@XetNghiemKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.XetNghiemKhac));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_StressDipyridamoleElectrocardiogram(URP_FE_StressDipyridamoleElectrocardiogram entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleElectrocardiogramDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleElectrocardiogramID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDipyridamoleExam
        public URP_FE_StressDipyridamoleExam GetURP_FE_StressDipyridamoleExam(long URP_FE_StressDipyridamoleExamID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleExamGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDipyridamoleExamID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDipyridamoleExam retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDipyridamoleExamFromReader(reader);
                }

                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleExamInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@TrieuChungHienTai", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungHienTai));
                cmd.AddParameter("@ChiDinhSATGSDipy", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChiDinhSATGSDipy));
                cmd.AddParameter("@ChiDinhDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChiDinhDetail));
                cmd.AddParameter("@TDDTruocNgayKham", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTruocNgayKham));
                cmd.AddParameter("@TDDTrongNgaySATGSDipy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTrongNgaySATGSDipy));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleExamUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDipyridamoleExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleExamID));

                cmd.AddParameter("@TrieuChungHienTai", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungHienTai));
                cmd.AddParameter("@ChiDinhSATGSDipy", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChiDinhSATGSDipy));
                cmd.AddParameter("@ChiDinhDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChiDinhDetail));
                cmd.AddParameter("@TDDTruocNgayKham", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTruocNgayKham));
                cmd.AddParameter("@TDDTrongNgaySATGSDipy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTrongNgaySATGSDipy));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_StressDipyridamoleExam(URP_FE_StressDipyridamoleExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleExamDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleExamID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDipyridamoleImage
        public URP_FE_StressDipyridamoleImage GetURP_FE_StressDipyridamoleImage(long URP_FE_StressDipyridamoleImageID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleImageGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleImageID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDipyridamoleImageID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDipyridamoleImage retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDipyridamoleImageFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public bool AddURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleImageInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleImageUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDipyridamoleImageID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleImageID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_StressDipyridamoleImage(URP_FE_StressDipyridamoleImage entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleImageDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleImageID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleImageID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDipyridamoleResult
        public URP_FE_StressDipyridamoleResult GetURP_FE_StressDipyridamoleResult(long URP_FE_StressDipyridamoleResultID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleResultGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDipyridamoleResultID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDipyridamoleResult retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDipyridamoleResultFromReader(reader);
                }

                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleResultInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@ThayDoiDTD", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThayDoiDTD));
                cmd.AddParameter("@ThayDoiDTDChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThayDoiDTDChiTiet));
                cmd.AddParameter("@RoiLoanNhip", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhip));
                cmd.AddParameter("@RoiLoanNhipChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipChiTiet));
                cmd.AddParameter("@TDPHayBienChung", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TDPHayBienChung));
                cmd.AddParameter("@TrieuChungKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungKhac));
                cmd.AddParameter("@BienPhapDieuTri", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.BienPhapDieuTri));
                cmd.AddParameter("@V_KetQuaSieuAmTim", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetQuaSieuAmTim));
                cmd.AddParameter("@KetQuaSieuAmTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetQuaSieuAmTim));
                cmd.AddParameter("@ThanhTruoc_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_TNP));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_TNP));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_TNP));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_KetLuan));
                cmd.AddParameter("@VanhLienThat_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_TNP));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_KetLuan));
                cmd.AddParameter("@VanhLienThat_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_TNP));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_KetLuan));
                cmd.AddParameter("@VanhLienThat_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_TNP));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_TNP));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_TNP));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_TNP));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_KetLuan));
                cmd.AddParameter("@ThanhSau_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_TNP));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_KetLuan));
                cmd.AddParameter("@ThanhSau_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_TNP));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_KetLuan));
                cmd.AddParameter("@ThanhSau_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_TNP));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_KetLuan));
                cmd.AddParameter("@ThanhBen_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_TNP));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_KetLuan));
                cmd.AddParameter("@ThanhBen_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_TNP));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_KetLuan));
                cmd.AddParameter("@ThanhBen_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_TNP));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_KetLuan));
                cmd.AddParameter("@TruocVach_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_TNP));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_KetLuan));
                cmd.AddParameter("@TruocVach_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_TNP));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_KetLuan));
                cmd.AddParameter("@TruocVach_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_TNP));
                cmd.AddParameter("@TruocVach_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_KetLuan));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleResultUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDipyridamoleResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@ThayDoiDTD", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThayDoiDTD));
                cmd.AddParameter("@ThayDoiDTDChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThayDoiDTDChiTiet));
                cmd.AddParameter("@RoiLoanNhip", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhip));
                cmd.AddParameter("@RoiLoanNhipChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipChiTiet));
                cmd.AddParameter("@TDPHayBienChung", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TDPHayBienChung));
                cmd.AddParameter("@TrieuChungKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungKhac));
                cmd.AddParameter("@BienPhapDieuTri", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.BienPhapDieuTri));
                cmd.AddParameter("@V_KetQuaSieuAmTim", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetQuaSieuAmTim));
                cmd.AddParameter("@KetQuaSieuAmTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetQuaSieuAmTim));
                cmd.AddParameter("@ThanhTruoc_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_TNP));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_TNP));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_TNP));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_KetLuan));
                cmd.AddParameter("@VanhLienThat_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_TNP));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_KetLuan));
                cmd.AddParameter("@VanhLienThat_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_TNP));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_KetLuan));
                cmd.AddParameter("@VanhLienThat_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_TNP));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_TNP));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_TNP));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_TNP));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_KetLuan));
                cmd.AddParameter("@ThanhSau_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_TNP));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_KetLuan));
                cmd.AddParameter("@ThanhSau_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_TNP));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_KetLuan));
                cmd.AddParameter("@ThanhSau_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_TNP));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_KetLuan));
                cmd.AddParameter("@ThanhBen_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_TNP));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_KetLuan));
                cmd.AddParameter("@ThanhBen_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_TNP));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_KetLuan));
                cmd.AddParameter("@ThanhBen_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_TNP));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_KetLuan));
                cmd.AddParameter("@TruocVach_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_TNP));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_KetLuan));
                cmd.AddParameter("@TruocVach_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_TNP));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_KetLuan));
                cmd.AddParameter("@TruocVach_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_TNP));
                cmd.AddParameter("@TruocVach_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_KetLuan));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_StressDipyridamoleResult(URP_FE_StressDipyridamoleResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDipyridamoleResultDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDipyridamoleResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDipyridamoleResultID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDobutamine
        public URP_FE_StressDobutamine GetURP_FE_StressDobutamine(long URP_FE_StressDobutamineID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDobutamineID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDobutamine retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDobutamineFromReader(reader);
                }

                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddURP_FE_StressDobutamine(URP_FE_StressDobutamine entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@TruyenTinhMach", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TruyenTinhMach));
                cmd.AddParameter("@TanSoTimCanDat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTimCanDat));
                cmd.AddParameter("@TD_TNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_TT));
                cmd.AddParameter("@TD_TNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_TTr));
                cmd.AddParameter("@TD_TNP_HuyetAp_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_TanSoTim));
                cmd.AddParameter("@TD_TNP_HuyetAp_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_DoChenhApMin));
                cmd.AddParameter("@TD_TNP_HuyetAp_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_DoChenhApMax));
                cmd.AddParameter("@FiveMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_DungLuong));
                cmd.AddParameter("@FiveMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_HuyetAp_TT));
                cmd.AddParameter("@FiveMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_HuyetAp_TTr));
                cmd.AddParameter("@FiveMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_TanSoTim));
                cmd.AddParameter("@FiveMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_DoChenhApMin));
                cmd.AddParameter("@FiveMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_DoChenhApMax));
                cmd.AddParameter("@TenMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_DungLuong));
                cmd.AddParameter("@TenMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_HuyetAp_TT));
                cmd.AddParameter("@TenMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_HuyetAp_TTr));
                cmd.AddParameter("@TenMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_TanSoTim));
                cmd.AddParameter("@TenMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_DoChenhApMin));
                cmd.AddParameter("@TenMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_DoChenhApMax));
                cmd.AddParameter("@TwentyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_DungLuong));
                cmd.AddParameter("@TwentyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_HuyetAp_TT));
                cmd.AddParameter("@TwentyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_HuyetAp_TTr));
                cmd.AddParameter("@TwentyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_TanSoTim));
                cmd.AddParameter("@TwentyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_DoChenhApMin));
                cmd.AddParameter("@TwentyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_DoChenhApMax));
                cmd.AddParameter("@ThirtyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_DungLuong));
                cmd.AddParameter("@ThirtyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_HuyetAp_TT));
                cmd.AddParameter("@ThirtyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_HuyetAp_TTr));
                cmd.AddParameter("@ThirtyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_TanSoTim));
                cmd.AddParameter("@ThirtyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_DoChenhApMin));
                cmd.AddParameter("@ThirtyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_DoChenhApMax));
                cmd.AddParameter("@FortyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_DungLuong));
                cmd.AddParameter("@FortyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_HuyetAp_TT));
                cmd.AddParameter("@FortyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_HuyetAp_TTr));
                cmd.AddParameter("@FortyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_TanSoTim));
                cmd.AddParameter("@FortyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_DoChenhApMin));
                cmd.AddParameter("@FortyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_DoChenhApMax));
                cmd.AddParameter("@Atropine_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_DungLuong));
                cmd.AddParameter("@Atropine_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_HuyetAp_TT));
                cmd.AddParameter("@Atropine_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_HuyetAp_TTr));
                cmd.AddParameter("@Atropine_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_TanSoTim));
                cmd.AddParameter("@Atropine_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_DoChenhApMin));
                cmd.AddParameter("@Atropine_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_DoChenhApMax));
                cmd.AddParameter("@NgungNP_ThoiGian", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_ThoiGian));
                cmd.AddParameter("@NgungNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_HuyetAp_TT));
                cmd.AddParameter("@NgungNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_HuyetAp_TTr));
                cmd.AddParameter("@NgungNP_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_TanSoTim));
                cmd.AddParameter("@NgungNP_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_DoChenhApMin));
                cmd.AddParameter("@NgungNP_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_DoChenhApMax));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_StressDobutamine(URP_FE_StressDobutamine entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@URP_FE_StressDobutamineID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@TruyenTinhMach", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TruyenTinhMach));
                cmd.AddParameter("@TanSoTimCanDat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTimCanDat));
                cmd.AddParameter("@TD_TNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_TT));
                cmd.AddParameter("@TD_TNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_TTr));
                cmd.AddParameter("@TD_TNP_HuyetAp_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_TanSoTim));
                cmd.AddParameter("@TD_TNP_HuyetAp_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_DoChenhApMin));
                cmd.AddParameter("@TD_TNP_HuyetAp_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TD_TNP_HuyetAp_DoChenhApMax));
                cmd.AddParameter("@FiveMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_DungLuong));
                cmd.AddParameter("@FiveMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_HuyetAp_TT));
                cmd.AddParameter("@FiveMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_HuyetAp_TTr));
                cmd.AddParameter("@FiveMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_TanSoTim));
                cmd.AddParameter("@FiveMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_DoChenhApMin));
                cmd.AddParameter("@FiveMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FiveMicro_DoChenhApMax));
                cmd.AddParameter("@TenMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_DungLuong));
                cmd.AddParameter("@TenMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_HuyetAp_TT));
                cmd.AddParameter("@TenMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_HuyetAp_TTr));
                cmd.AddParameter("@TenMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_TanSoTim));
                cmd.AddParameter("@TenMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_DoChenhApMin));
                cmd.AddParameter("@TenMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TenMicro_DoChenhApMax));
                cmd.AddParameter("@TwentyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_DungLuong));
                cmd.AddParameter("@TwentyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_HuyetAp_TT));
                cmd.AddParameter("@TwentyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_HuyetAp_TTr));
                cmd.AddParameter("@TwentyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_TanSoTim));
                cmd.AddParameter("@TwentyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_DoChenhApMin));
                cmd.AddParameter("@TwentyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TwentyMicro_DoChenhApMax));
                cmd.AddParameter("@ThirtyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_DungLuong));
                cmd.AddParameter("@ThirtyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_HuyetAp_TT));
                cmd.AddParameter("@ThirtyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_HuyetAp_TTr));
                cmd.AddParameter("@ThirtyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_TanSoTim));
                cmd.AddParameter("@ThirtyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_DoChenhApMin));
                cmd.AddParameter("@ThirtyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThirtyMicro_DoChenhApMax));
                cmd.AddParameter("@FortyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_DungLuong));
                cmd.AddParameter("@FortyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_HuyetAp_TT));
                cmd.AddParameter("@FortyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_HuyetAp_TTr));
                cmd.AddParameter("@FortyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_TanSoTim));
                cmd.AddParameter("@FortyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_DoChenhApMin));
                cmd.AddParameter("@FortyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.FortyMicro_DoChenhApMax));
                cmd.AddParameter("@Atropine_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_DungLuong));
                cmd.AddParameter("@Atropine_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_HuyetAp_TT));
                cmd.AddParameter("@Atropine_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_HuyetAp_TTr));
                cmd.AddParameter("@Atropine_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_TanSoTim));
                cmd.AddParameter("@Atropine_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_DoChenhApMin));
                cmd.AddParameter("@Atropine_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.Atropine_DoChenhApMax));
                cmd.AddParameter("@NgungNP_ThoiGian", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_ThoiGian));
                cmd.AddParameter("@NgungNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_HuyetAp_TT));
                cmd.AddParameter("@NgungNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_HuyetAp_TTr));
                cmd.AddParameter("@NgungNP_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_TanSoTim));
                cmd.AddParameter("@NgungNP_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_DoChenhApMin));
                cmd.AddParameter("@NgungNP_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NgungNP_DoChenhApMax));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_StressDobutamine(URP_FE_StressDobutamine entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDobutamineElectrocardiogram
        public URP_FE_StressDobutamineElectrocardiogram GetURP_FE_StressDobutamineElectrocardiogram(long URP_FE_StressDobutamineElectrocardiogramID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineElectrocardiogramGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDobutamineElectrocardiogramID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDobutamineElectrocardiogram retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDobutamineElectrocardiogramFromReader(reader);
                }

                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineElectrocardiogramInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@DieuTriConDauThatNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriConDauThatNguc));
                cmd.AddParameter("@DieuTriDIGITALIS", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriDIGITALIS));
                cmd.AddParameter("@LyDoKhongThucHienDuoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LyDoKhongThucHienDuoc));
                cmd.AddParameter("@MucGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MucGangSuc));
                cmd.AddParameter("@ThoiGianGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThoiGianGangSuc));
                cmd.AddParameter("@TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTim));
                cmd.AddParameter("@HuyetApToiDa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApToiDa));
                cmd.AddParameter("@ConDauThatNguc", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ConDauThatNguc));
                cmd.AddParameter("@STChenhXuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.STChenhXuong));
                cmd.AddParameter("@RoiLoanNhipTim", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhipTim));
                cmd.AddParameter("@RoiLoanNhipTimChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipTimChiTiet));
                cmd.AddParameter("@XetNghiemKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.XetNghiemKhac));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineElectrocardiogramUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@URP_FE_StressDobutamineElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineElectrocardiogramID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@DieuTriConDauThatNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriConDauThatNguc));
                cmd.AddParameter("@DieuTriDIGITALIS", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DieuTriDIGITALIS));
                cmd.AddParameter("@LyDoKhongThucHienDuoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.LyDoKhongThucHienDuoc));
                cmd.AddParameter("@MucGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.MucGangSuc));
                cmd.AddParameter("@ThoiGianGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ThoiGianGangSuc));
                cmd.AddParameter("@TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.TanSoTim));
                cmd.AddParameter("@HuyetApToiDa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.HuyetApToiDa));
                cmd.AddParameter("@ConDauThatNguc", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ConDauThatNguc));
                cmd.AddParameter("@STChenhXuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.STChenhXuong));
                cmd.AddParameter("@RoiLoanNhipTim", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhipTim));
                cmd.AddParameter("@RoiLoanNhipTimChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipTimChiTiet));
                cmd.AddParameter("@XetNghiemKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.XetNghiemKhac));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_StressDobutamineElectrocardiogram(URP_FE_StressDobutamineElectrocardiogram entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineElectrocardiogramDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineElectrocardiogramID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDobutamineExam
        public URP_FE_StressDobutamineExam GetURP_FE_StressDobutamineExam(long URP_FE_StressDobutamineExamID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineExamGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDobutamineExamID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDobutamineExam retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDobutamineExamFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public bool AddURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineExamInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@TrieuChungHienTai", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungHienTai));
                cmd.AddParameter("@ChiDinhSATGSDobu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChiDinhSATGSDobu));
                cmd.AddParameter("@ChiDinhDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChiDinhDetail));
                cmd.AddParameter("@TDDTruocNgayKham", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTruocNgayKham));
                cmd.AddParameter("@TDDTrongNgaySATGSDobu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTrongNgaySATGSDobu));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineExamUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDobutamineExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineExamID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@TrieuChungHienTai", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungHienTai));
                cmd.AddParameter("@ChiDinhSATGSDobu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChiDinhSATGSDobu));
                cmd.AddParameter("@ChiDinhDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ChiDinhDetail));
                cmd.AddParameter("@TDDTruocNgayKham", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTruocNgayKham));
                cmd.AddParameter("@TDDTrongNgaySATGSDobu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TDDTrongNgaySATGSDobu));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_StressDobutamineExam(URP_FE_StressDobutamineExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineExamDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineExamID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_StressDobutamineImages

        public URP_FE_StressDobutamineImages GetURP_FE_StressDobutamineImages(long URP_FE_StressDobutamineImagesID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineImagesGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineImagesID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDobutamineImagesID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDobutamineImages retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDobutamineImagesFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public bool AddURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineImagesInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineImagesUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDobutamineImagesID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineImagesID));
                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_StressDobutamineImages(URP_FE_StressDobutamineImages entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineImagesDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineImagesID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineImagesID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region URP_FE_StressDobutamineResult
        public URP_FE_StressDobutamineResult GetURP_FE_StressDobutamineResult(long URP_FE_StressDobutamineResultID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineResultGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_StressDobutamineResultID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_StressDobutamineResult retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_StressDobutamineResultFromReader(reader);
                }

                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineResultInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@ThayDoiDTD", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThayDoiDTD));
                cmd.AddParameter("@ThayDoiDTDChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThayDoiDTDChiTiet));
                cmd.AddParameter("@RoiLoanNhip", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhip));
                cmd.AddParameter("@RoiLoanNhipChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipChiTiet));
                cmd.AddParameter("@TDPHayBienChung", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TDPHayBienChung));
                cmd.AddParameter("@TrieuChungKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungKhac));
                cmd.AddParameter("@BienPhapDieuTri", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.BienPhapDieuTri));
                cmd.AddParameter("@V_KetQuaSieuAmTim", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetQuaSieuAmTim));
                cmd.AddParameter("@KetQuaSieuAmTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetQuaSieuAmTim));
                cmd.AddParameter("@ThanhTruoc_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_TNP));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_TNP));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_TNP));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_KetLuan));
                cmd.AddParameter("@VanhLienThat_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_TNP));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_KetLuan));
                cmd.AddParameter("@VanhLienThat_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_TNP));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_KetLuan));
                cmd.AddParameter("@VanhLienThat_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_TNP));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_TNP));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_TNP));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_TNP));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_KetLuan));
                cmd.AddParameter("@ThanhSau_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_TNP));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_KetLuan));
                cmd.AddParameter("@ThanhSau_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_TNP));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_KetLuan));
                cmd.AddParameter("@ThanhSau_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_TNP));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_KetLuan));
                cmd.AddParameter("@ThanhBen_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_TNP));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_KetLuan));
                cmd.AddParameter("@ThanhBen_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_TNP));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_KetLuan));
                cmd.AddParameter("@ThanhBen_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_TNP));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_KetLuan));
                cmd.AddParameter("@TruocVach_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_TNP));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_KetLuan));
                cmd.AddParameter("@TruocVach_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_TNP));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_KetLuan));
                cmd.AddParameter("@TruocVach_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_TNP));
                cmd.AddParameter("@TruocVach_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_KetLuan));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineResultUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_StressDobutamineResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineResultID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));

                cmd.AddParameter("@ThayDoiDTD", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThayDoiDTD));
                cmd.AddParameter("@ThayDoiDTDChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ThayDoiDTDChiTiet));
                cmd.AddParameter("@RoiLoanNhip", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.RoiLoanNhip));
                cmd.AddParameter("@RoiLoanNhipChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.RoiLoanNhipChiTiet));
                cmd.AddParameter("@TDPHayBienChung", SqlDbType.Int, ConvertNullObjectToDBNull(entity.TDPHayBienChung));
                cmd.AddParameter("@TrieuChungKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.TrieuChungKhac));
                cmd.AddParameter("@BienPhapDieuTri", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.BienPhapDieuTri));
                cmd.AddParameter("@V_KetQuaSieuAmTim", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetQuaSieuAmTim));
                cmd.AddParameter("@KetQuaSieuAmTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetQuaSieuAmTim));
                cmd.AddParameter("@ThanhTruoc_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_TNP));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Mom_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_TNP));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Giua_KetLuan));
                cmd.AddParameter("@ThanhTruoc_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_TNP));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhTruoc_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhTruoc_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhTruoc_Day_KetLuan));
                cmd.AddParameter("@VanhLienThat_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_TNP));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Mom_KetLuan));
                cmd.AddParameter("@VanhLienThat_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_TNP));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Giua_KetLuan));
                cmd.AddParameter("@VanhLienThat_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_TNP));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuThap));
                cmd.AddParameter("@VanhLienThat_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_DobuLieuCao));
                cmd.AddParameter("@VanhLienThat_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.VanhLienThat_Day_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_TNP));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Mom_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_TNP));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Giua_KetLuan));
                cmd.AddParameter("@ThanhDuoi_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_TNP));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhDuoi_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhDuoi_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhDuoi_Day_KetLuan));
                cmd.AddParameter("@ThanhSau_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_TNP));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Mom_KetLuan));
                cmd.AddParameter("@ThanhSau_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_TNP));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Giua_KetLuan));
                cmd.AddParameter("@ThanhSau_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_TNP));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhSau_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhSau_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhSau_Day_KetLuan));
                cmd.AddParameter("@ThanhBen_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_TNP));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Mom_KetLuan));
                cmd.AddParameter("@ThanhBen_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_TNP));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Giua_KetLuan));
                cmd.AddParameter("@ThanhBen_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_TNP));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuThap));
                cmd.AddParameter("@ThanhBen_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_DobuLieuCao));
                cmd.AddParameter("@ThanhBen_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ThanhBen_Day_KetLuan));
                cmd.AddParameter("@TruocVach_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_TNP));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Mom_KetLuan));
                cmd.AddParameter("@TruocVach_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_TNP));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Giua_KetLuan));
                cmd.AddParameter("@TruocVach_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_TNP));
                cmd.AddParameter("@TruocVach_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuThap));
                cmd.AddParameter("@TruocVach_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_DobuLieuCao));
                cmd.AddParameter("@TruocVach_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.TruocVach_Day_KetLuan));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_StressDobutamineResult(URP_FE_StressDobutamineResult entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_StressDobutamineResultDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_StressDobutamineResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_StressDobutamineResultID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_VasculaireAnother
        public URP_FE_VasculaireAnother GetURP_FE_VasculaireAnother(long URP_FE_VasculaireAnotherID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAnotherGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireAnotherID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_VasculaireAnotherID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_VasculaireAnother retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_VasculaireAnotherFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public bool AddURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAnotherInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@MoTa", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.MoTa));
                cmd.AddParameter("@KetLuanEx", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuanEx));
                cmd.AddParameter("@V_MotaEx", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_MotaEx));
                cmd.AddParameter("@V_KetLuanEx", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuanEx));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAnotherUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_VasculaireAnotherID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireAnotherID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@MoTa", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.MoTa));
                cmd.AddParameter("@KetLuanEx", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuanEx));
                cmd.AddParameter("@V_MotaEx", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_MotaEx));
                cmd.AddParameter("@V_KetLuanEx", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuanEx));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_VasculaireAnother(URP_FE_VasculaireAnother entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAnotherDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireAnotherID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireAnotherID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region URP_FE_VasculaireAorta
        public URP_FE_VasculaireAorta GetURP_FE_VasculaireAorta(long URP_FE_VasculaireAortaID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAortaGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireAortaID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_VasculaireAortaID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_VasculaireAorta retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_VasculaireAortaFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public bool AddURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAortaInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@DMCNgang", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCNgang));
                cmd.AddParameter("@DMCLen", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCLen));
                cmd.AddParameter("@EoDMC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.EoDMC));
                cmd.AddParameter("@DMCXuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCXuong));
                cmd.AddParameter("@DMThanP_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanP_v));
                cmd.AddParameter("@DMThanP_RI", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanP_RI));
                cmd.AddParameter("@DMThanT_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanT_v));
                cmd.AddParameter("@DMThanT_RI", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanT_RI));
                cmd.AddParameter("@DMChauP_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMChauP_v));
                cmd.AddParameter("@DMChauT_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMChauT_v));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));
                cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuan));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAortaUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@URP_FE_VasculaireAortaID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireAortaID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@DMCNgang", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCNgang));
                cmd.AddParameter("@DMCLen", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCLen));
                cmd.AddParameter("@EoDMC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.EoDMC));
                cmd.AddParameter("@DMCXuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCXuong));
                cmd.AddParameter("@DMThanP_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanP_v));
                cmd.AddParameter("@DMThanP_RI", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanP_RI));
                cmd.AddParameter("@DMThanT_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanT_v));
                cmd.AddParameter("@DMThanT_RI", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMThanT_RI));
                cmd.AddParameter("@DMChauP_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMChauP_v));
                cmd.AddParameter("@DMChauT_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMChauT_v));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));
                cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuan));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_VasculaireAorta(URP_FE_VasculaireAorta entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireAortaDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireAortaID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireAortaID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        #endregion

        #region URP_FE_VasculaireCarotid
        public URP_FE_VasculaireCarotid GetURP_FE_VasculaireCarotid(long URP_FE_VasculaireCarotidID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireCarotidGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireCarotidID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_VasculaireCarotidID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_VasculaireCarotid retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_VasculaireCarotidFromReader(reader);
                }

                reader.Close();

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool AddURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireCarotidInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@V_KetQuaSAMMTruoc", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetQuaSAMMTruoc));
                cmd.AddParameter("@KetQuaSAMMTruoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetQuaSAMMTruoc));
                cmd.AddParameter("@DMCP_ECA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_ECA));
                cmd.AddParameter("@DMCP_ICA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_ICA));
                cmd.AddParameter("@DMCP_ICA_SR", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_ICA_SR));
                cmd.AddParameter("@DMCP_CCA_TCC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_CCA_TCC));
                cmd.AddParameter("@DMCP_CCA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_CCA));
                cmd.AddParameter("@DMCT_ECA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_ECA));
                cmd.AddParameter("@DMCT_ICA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_ICA));
                cmd.AddParameter("@DMCT_ICA_SR", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_ICA_SR));
                cmd.AddParameter("@DMCT_CCA_TCC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_CCA_TCC));
                cmd.AddParameter("@DMCT_CCA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_CCA));
                cmd.AddParameter("@DMCotSongP_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongP_d));
                cmd.AddParameter("@DMCotSongP_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongP_r));
                cmd.AddParameter("@DMCotSongT_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongT_d));
                cmd.AddParameter("@DMCotSongT_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongT_r));
                cmd.AddParameter("@DMDuoiDonP_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonP_d));
                cmd.AddParameter("@DMDuoiDonP_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonP_r));
                cmd.AddParameter("@DMDuoiDonT_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonT_d));
                cmd.AddParameter("@DMDuoiDonT_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonT_r));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));
                cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuan));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireCarotidUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_VasculaireCarotidID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireCarotidID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@V_KetQuaSAMMTruoc", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetQuaSAMMTruoc));
                cmd.AddParameter("@KetQuaSAMMTruoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetQuaSAMMTruoc));
                cmd.AddParameter("@DMCP_ECA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_ECA));
                cmd.AddParameter("@DMCP_ICA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_ICA));
                cmd.AddParameter("@DMCP_ICA_SR", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_ICA_SR));
                cmd.AddParameter("@DMCP_CCA_TCC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_CCA_TCC));
                cmd.AddParameter("@DMCP_CCA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCP_CCA));
                cmd.AddParameter("@DMCT_ECA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_ECA));
                cmd.AddParameter("@DMCT_ICA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_ICA));
                cmd.AddParameter("@DMCT_ICA_SR", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_ICA_SR));
                cmd.AddParameter("@DMCT_CCA_TCC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_CCA_TCC));
                cmd.AddParameter("@DMCT_CCA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCT_CCA));
                cmd.AddParameter("@DMCotSongP_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongP_d));
                cmd.AddParameter("@DMCotSongP_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongP_r));
                cmd.AddParameter("@DMCotSongT_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongT_d));
                cmd.AddParameter("@DMCotSongT_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMCotSongT_r));
                cmd.AddParameter("@DMDuoiDonP_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonP_d));
                cmd.AddParameter("@DMDuoiDonP_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonP_r));
                cmd.AddParameter("@DMDuoiDonT_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonT_d));
                cmd.AddParameter("@DMDuoiDonT_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.DMDuoiDonT_r));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));
                cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuan));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_VasculaireCarotid(URP_FE_VasculaireCarotid entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireCarotidDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireCarotidID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireCarotidID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region URP_FE_VasculaireExam
        public URP_FE_VasculaireExam GetURP_FE_VasculaireExam(long URP_FE_VasculaireExamID, long PatientPCLReqID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireExamGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_VasculaireExamID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));

                cn.Open();
                URP_FE_VasculaireExam retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_VasculaireExamFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public bool AddURP_FE_VasculaireExam(URP_FE_VasculaireExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireExamInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@NoiLap", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NoiLap));
                cmd.AddParameter("@ChongMat", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChongMat));
                cmd.AddParameter("@DotQuy", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DotQuy));
                cmd.AddParameter("@GiamTriNho", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.GiamTriNho));
                cmd.AddParameter("@ThoangMu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThoangMu));
                cmd.AddParameter("@NhinMo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NhinMo));
                cmd.AddParameter("@LietNuaNguoi", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.LietNuaNguoi));
                cmd.AddParameter("@TeYeuChanTay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TeYeuChanTay));
                cmd.AddParameter("@DaPThuatDMC", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DaPThuatDMC));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_VasculaireExam(URP_FE_VasculaireExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireExamUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));
                cmd.AddParameter("@URP_FE_VasculaireExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireExamID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@NoiLap", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NoiLap));
                cmd.AddParameter("@ChongMat", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ChongMat));
                cmd.AddParameter("@DotQuy", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DotQuy));
                cmd.AddParameter("@GiamTriNho", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.GiamTriNho));
                cmd.AddParameter("@ThoangMu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ThoangMu));
                cmd.AddParameter("@NhinMo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.NhinMo));
                cmd.AddParameter("@LietNuaNguoi", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.LietNuaNguoi));
                cmd.AddParameter("@TeYeuChanTay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.TeYeuChanTay));
                cmd.AddParameter("@DaPThuatDMC", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.DaPThuatDMC));


                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_VasculaireExam(URP_FE_VasculaireExam entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireExamDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireExamID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        #region URP_FE_VasculaireLeg
        public URP_FE_VasculaireLeg GetURP_FE_VasculaireLeg(long URP_FE_VasculaireLegID, long PCLImgResultID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireLegGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireLegID", SqlDbType.BigInt, ConvertNullObjectToDBNull(URP_FE_VasculaireLegID));
                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLImgResultID));

                cn.Open();
                URP_FE_VasculaireLeg retVal = null;

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetURP_FE_VasculaireLegFromReader(reader);
                }

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        public bool AddURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireLegInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));


                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@CT_EI_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_EI_P));
                cmd.AddParameter("@CT_EI_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_EI_T));
                cmd.AddParameter("@CT_CF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_CF_P));
                cmd.AddParameter("@CT_CF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_CF_T));
                cmd.AddParameter("@CT_SF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_SF_P));
                cmd.AddParameter("@CT_SF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_SF_T));
                cmd.AddParameter("@CT_POP_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_POP_P));
                cmd.AddParameter("@CT_POP_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_POP_T));
                cmd.AddParameter("@CT_AT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_AT_P));
                cmd.AddParameter("@CT_AT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_AT_T));
                cmd.AddParameter("@CT_PER_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PER_P));
                cmd.AddParameter("@CT_PER_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PER_T));
                cmd.AddParameter("@CT_GrSaph_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_GrSaph_P));
                cmd.AddParameter("@CT_GrSaph_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_GrSaph_T));
                cmd.AddParameter("@CT_PT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PT_P));
                cmd.AddParameter("@CT_PT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PT_T));
                cmd.AddParameter("@CD_EI_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_EI_P));
                cmd.AddParameter("@CD_EI_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_EI_T));
                cmd.AddParameter("@CD_CF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_CF_P));
                cmd.AddParameter("@CD_CF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_CF_T));
                cmd.AddParameter("@CD_SF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_SF_P));
                cmd.AddParameter("@CD_SF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_SF_T));
                cmd.AddParameter("@CD_POP_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_POP_P));
                cmd.AddParameter("@CD_POP_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_POP_T));
                cmd.AddParameter("@CD_AT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_AT_P));
                cmd.AddParameter("@CD_AT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_AT_T));
                cmd.AddParameter("@CD_PER_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PER_P));
                cmd.AddParameter("@CD_PER_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PER_T));
                cmd.AddParameter("@CD_GrSaph_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_GrSaph_P));
                cmd.AddParameter("@CD_GrSaph_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_GrSaph_T));
                cmd.AddParameter("@CD_PT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PT_P));
                cmd.AddParameter("@CD_PT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PT_T));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));
                cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuan));


                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool UpdateURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireLegUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLImgResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLImgResultID));

                cmd.AddParameter("@URP_FE_VasculaireLegID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireLegID));

                cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.DoctorStaffID));
                cmd.AddParameter("@CT_EI_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_EI_P));
                cmd.AddParameter("@CT_EI_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_EI_T));
                cmd.AddParameter("@CT_CF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_CF_P));
                cmd.AddParameter("@CT_CF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_CF_T));
                cmd.AddParameter("@CT_SF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_SF_P));
                cmd.AddParameter("@CT_SF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_SF_T));
                cmd.AddParameter("@CT_POP_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_POP_P));
                cmd.AddParameter("@CT_POP_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_POP_T));
                cmd.AddParameter("@CT_AT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_AT_P));
                cmd.AddParameter("@CT_AT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_AT_T));
                cmd.AddParameter("@CT_PER_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PER_P));
                cmd.AddParameter("@CT_PER_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PER_T));
                cmd.AddParameter("@CT_GrSaph_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_GrSaph_P));
                cmd.AddParameter("@CT_GrSaph_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_GrSaph_T));
                cmd.AddParameter("@CT_PT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PT_P));
                cmd.AddParameter("@CT_PT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CT_PT_T));
                cmd.AddParameter("@CD_EI_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_EI_P));
                cmd.AddParameter("@CD_EI_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_EI_T));
                cmd.AddParameter("@CD_CF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_CF_P));
                cmd.AddParameter("@CD_CF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_CF_T));
                cmd.AddParameter("@CD_SF_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_SF_P));
                cmd.AddParameter("@CD_SF_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_SF_T));
                cmd.AddParameter("@CD_POP_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_POP_P));
                cmd.AddParameter("@CD_POP_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_POP_T));
                cmd.AddParameter("@CD_AT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_AT_P));
                cmd.AddParameter("@CD_AT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_AT_T));
                cmd.AddParameter("@CD_PER_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PER_P));
                cmd.AddParameter("@CD_PER_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PER_T));
                cmd.AddParameter("@CD_GrSaph_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_GrSaph_P));
                cmd.AddParameter("@CD_GrSaph_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_GrSaph_T));
                cmd.AddParameter("@CD_PT_P", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PT_P));
                cmd.AddParameter("@CD_PT_T", SqlDbType.Float, ConvertNullObjectToDBNull(entity.CD_PT_T));
                cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.KetLuan));
                cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_KetLuan));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }

        public bool DeleteURP_FE_VasculaireLeg(URP_FE_VasculaireLeg entity)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spURP_FE_VasculaireLegDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@URP_FE_VasculaireLegID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.URP_FE_VasculaireLegID));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        #endregion

        //==== 20161129 CMN Bein: Add button save for all FE pages
        public bool AddAndUpdateFE_Vasculaire(URP_FE_Vasculaire entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAddAndUpdateFE_Vasculaire", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLRequestID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.DoctorStaffID));
                    cmd.AddParameter("@NoiLap", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.NoiLap));
                    cmd.AddParameter("@ChongMat", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.ChongMat));
                    cmd.AddParameter("@DotQuy", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.DotQuy));
                    cmd.AddParameter("@GiamTriNho", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.GiamTriNho));
                    cmd.AddParameter("@ThoangMu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.ThoangMu));
                    cmd.AddParameter("@NhinMo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.NhinMo));
                    cmd.AddParameter("@LietNuaNguoi", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.LietNuaNguoi));
                    cmd.AddParameter("@TeYeuChanTay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.TeYeuChanTay));
                    cmd.AddParameter("@DaPThuatDMC", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.DaPThuatDMC));
                    cmd.AddParameter("@URP_FE_VasculaireExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.URP_FE_VasculaireExamID));
                    cmd.AddParameter("@Tab1_EX_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireExam.Tab_Update_Required));

                    cmd.AddParameter("@V_KetQuaSAMMTruoc", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.V_KetQuaSAMMTruoc));
                    cmd.AddParameter("@KetQuaSAMMTruoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.KetQuaSAMMTruoc));
                    cmd.AddParameter("@DMCP_ECA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCP_ECA));
                    cmd.AddParameter("@DMCP_ICA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCP_ICA));
                    cmd.AddParameter("@DMCP_ICA_SR", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCP_ICA_SR));
                    cmd.AddParameter("@DMCP_CCA_TCC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCP_CCA_TCC));
                    cmd.AddParameter("@DMCP_CCA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCP_CCA));
                    cmd.AddParameter("@DMCT_ECA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCT_ECA));
                    cmd.AddParameter("@DMCT_ICA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCT_ICA));
                    cmd.AddParameter("@DMCT_ICA_SR", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCT_ICA_SR));
                    cmd.AddParameter("@DMCT_CCA_TCC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCT_CCA_TCC));
                    cmd.AddParameter("@DMCT_CCA", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCT_CCA));
                    cmd.AddParameter("@DMCotSongP_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCotSongP_d));
                    cmd.AddParameter("@DMCotSongP_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCotSongP_r));
                    cmd.AddParameter("@DMCotSongT_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCotSongT_d));
                    cmd.AddParameter("@DMCotSongT_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMCotSongT_r));
                    cmd.AddParameter("@DMDuoiDonP_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMDuoiDonP_d));
                    cmd.AddParameter("@DMDuoiDonP_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMDuoiDonP_r));
                    cmd.AddParameter("@DMDuoiDonT_d", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMDuoiDonT_d));
                    cmd.AddParameter("@DMDuoiDonT_r", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.DMDuoiDonT_r));
                    cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.KetLuan));
                    cmd.AddParameter("@V_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.V_KetLuan));
                    cmd.AddParameter("@URP_FE_VasculaireCarotidID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.URP_FE_VasculaireCarotidID));
                    cmd.AddParameter("@Tab1_CA_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireCarotid.Tab_Update_Required));

                    cmd.AddParameter("@URP_FE_VasculaireAortaID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.URP_FE_VasculaireAortaID));
                    cmd.AddParameter("@DMCNgang", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMCNgang));
                    cmd.AddParameter("@DMCLen", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMCLen));
                    cmd.AddParameter("@EoDMC", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.EoDMC));
                    cmd.AddParameter("@DMCXuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMCXuong));
                    cmd.AddParameter("@DMThanP_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMThanP_v));
                    cmd.AddParameter("@DMThanP_RI", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMThanP_RI));
                    cmd.AddParameter("@DMThanT_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMThanT_v));
                    cmd.AddParameter("@DMThanT_RI", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMThanT_RI));
                    cmd.AddParameter("@DMChauP_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMChauP_v));
                    cmd.AddParameter("@DMChauT_v", SqlDbType.Float, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.DMChauT_v));
                    cmd.AddParameter("@KetLuan2", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.KetLuan));
                    cmd.AddParameter("@V_KetLuan2", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.V_KetLuan));
                    cmd.AddParameter("@Tab2_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAorta.Tab_Update_Required));

                    cmd.AddParameter("@KetLuan3", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.curURP_FE_StressDobutamineImages.KetLuan));
                    cmd.AddParameter("@URP_FE_StressDobutamineImagesID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_StressDobutamineImages.URP_FE_StressDobutamineImagesID));
                    cmd.AddParameter("@Tab3_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_StressDobutamineImages.Tab_Update_Required));

                    cmd.AddParameter("@MoTa", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.MoTa));
                    cmd.AddParameter("@KetLuanEx", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.KetLuanEx));
                    cmd.AddParameter("@V_MotaEx", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.V_MotaEx));
                    cmd.AddParameter("@V_KetLuanEx", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.V_KetLuanEx));
                    cmd.AddParameter("@URP_FE_VasculaireAnotherID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.URP_FE_VasculaireAnotherID));
                    cmd.AddParameter("@Tab4_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.Tab_Update_Required));
                    //==== #001
                    cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.curURP_FE_VasculaireAnother.CreateDate));
                    //==== #001

                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    if (retVal > 0)
                        return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool AddAndUpdateUltraResParams_FetalEchocardiography(UltraResParams_FetalEchocardiography entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAddAndUpdateUltraResParams_FetalEchocardiography", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@NTSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.NTSize));
                    cmd.AddParameter("@NPSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.NPSize));
                    cmd.AddParameter("@VanVieussensLeftAtrium", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.VanVieussensLeftAtrium));
                    cmd.AddParameter("@AtrialSeptalDefect", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.AtrialSeptalDefect));
                    cmd.AddParameter("@MitralValveSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.MitralValveSize));
                    cmd.AddParameter("@TriscupidValveSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.TriscupidValveSize));
                    cmd.AddParameter("@DifferenceMitralTricuspid", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.DifferenceMitralTricuspid));
                    cmd.AddParameter("@TPTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.TPTTr));
                    cmd.AddParameter("@VLTTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.VLTTTr));
                    cmd.AddParameter("@TTTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.TTTTr));
                    cmd.AddParameter("@DKTTTTr_VGd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.DKTTTTr_VGd));
                    cmd.AddParameter("@DKTTTT_VGs", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.DKTTTT_VGs));
                    cmd.AddParameter("@DKTPTTr_VDd", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.DKTPTTr_VDd));
                    cmd.AddParameter("@DKTPTT_VDs", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.DKTPTT_VDs));
                    cmd.AddParameter("@Systolic", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.Systolic));
                    cmd.AddParameter("@VentricularSeptalDefect", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.VentricularSeptalDefect));
                    cmd.AddParameter("@AortaCompatible", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.AortaCompatible));
                    cmd.AddParameter("@AortaSize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.AortaSize));
                    cmd.AddParameter("@PulmonaryArterySize", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.PulmonaryArterySize));
                    cmd.AddParameter("@AorticArch", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.AorticArch));
                    cmd.AddParameter("@AorticCoarctation", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.AorticCoarctation));
                    cmd.AddParameter("@HeartRateNomal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.HeartRateNomal));
                    cmd.AddParameter("@RequencyHeartRateNomal", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.RequencyHeartRateNomal));
                    cmd.AddParameter("@PericardialEffusion", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.PericardialEffusion));
                    cmd.AddParameter("@FetalCardialAxis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.FetalCardialAxis));
                    cmd.AddParameter("@CardialRateS", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.CardialRateS));
                    cmd.AddParameter("@LN", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.LN));
                    cmd.AddParameter("@OrderRecord", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.OrderRecord));
                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLRequestID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.DoctorStaffID));
                    cmd.AddParameter("@UltraResParams_FetalEchocardiography2DID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.UltraResParams_FetalEchocardiography2DID));
                    cmd.AddParameter("@Tab1_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiography2D.Tab_Update_Required));
                    cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.CreateDate));
                    //==== 20161213 CMN Begin: Add lookup for FetalEchocardiography
                    cmd.AddParameter("@UltraResParams_FetalEchocardiographyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.UltraResParams_FetalEchocardiographyID));
                    cmd.AddParameter("@FetalAge", SqlDbType.TinyInt, ConvertNullObjectToDBNull(entity.FetalAge));
                    cmd.AddParameter("@NuchalTranslucency", SqlDbType.Float, ConvertNullObjectToDBNull(entity.NuchalTranslucency));
                    if (entity.V_EchographyPosture != null)
                        cmd.AddParameter("@V_EchographyPosture", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_EchographyPosture.LookupID));
                    if (entity.V_MomMedHis != null)
                        cmd.AddParameter("@V_MomMedHis", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.V_MomMedHis.LookupID));
                    cmd.AddParameter("@Notice", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.Notice));
                    //==== 20161213 CMN End.

                    cmd.AddParameter("@MitralValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.MitralValve_Vmax));
                    cmd.AddParameter("@MitralValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.MitralValve_Gdmax));
                    cmd.AddParameter("@MitralValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.MitralValve_Open));
                    cmd.AddParameter("@MitralValve_Stenosis", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.MitralValve_Stenosis));
                    cmd.AddParameter("@TriscupidValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.TriscupidValve_Vmax));
                    cmd.AddParameter("@TriscupidValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.TriscupidValve_Gdmax));
                    cmd.AddParameter("@TriscupidValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.TriscupidValve_Open));
                    cmd.AddParameter("@TriscupidValve_Stenosis", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.TriscupidValve_Stenosis));
                    cmd.AddParameter("@AorticValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.AorticValve_Vmax));
                    cmd.AddParameter("@AorticValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.AorticValve_Gdmax));
                    cmd.AddParameter("@AorticValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.AorticValve_Open));
                    cmd.AddParameter("@AorticValve_Stenosis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.AorticValve_Stenosis));
                    cmd.AddParameter("@PulmonaryValve_Vmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.PulmonaryValve_Vmax));
                    cmd.AddParameter("@PulmonaryValve_Gdmax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.PulmonaryValve_Gdmax));
                    cmd.AddParameter("@PulmonaryValve_Open", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.PulmonaryValve_Open));
                    cmd.AddParameter("@PulmonaryValve_Stenosis", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.PulmonaryValve_Stenosis));
                    cmd.AddParameter("@AorticCoarctationBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.AorticCoarctationBloodTraffic));
                    cmd.AddParameter("@VanViewessensBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.VanViewessensBloodTraffic));
                    cmd.AddParameter("@DuctusAteriosusBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.DuctusAteriosusBloodTraffic));
                    cmd.AddParameter("@DuctusVenosusBloodTraffic", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.DuctusVenosusBloodTraffic));
                    cmd.AddParameter("@PulmonaryVeins_LeftAtrium", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.PulmonaryVeins_LeftAtrium));
                    cmd.AddParameter("@OrderRecord2", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.OrderRecord));
                    cmd.AddParameter("@UltraResParams_FetalEchocardiographyDopplerID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.UltraResParams_FetalEchocardiographyDopplerID));
                    cmd.AddParameter("@Tab2_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyDoppler.Tab_Update_Required));

                    cmd.AddParameter("@UltraResParams_FetalEchocardiographyResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyResult.UltraResParams_FetalEchocardiographyResultID));
                    cmd.AddParameter("@CardialAbnormal", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyResult.CardialAbnormal));
                    cmd.AddParameter("@CardialAbnormalDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyResult.CardialAbnormalDetail));
                    cmd.AddParameter("@Susgest", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyResult.Susgest));
                    cmd.AddParameter("@UltraResParamDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyResult.UltraResParamDate));
                    cmd.AddParameter("@Tab3_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyResult.Tab_Update_Required));

                    cmd.AddParameter("@UltraResParams_FetalEchocardiographyPostpartumID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.UltraResParams_FetalEchocardiographyPostpartumID));
                    cmd.AddParameter("@BabyBirthday", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.BabyBirthday));
                    cmd.AddParameter("@BabyWeight", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.BabyWeight));
                    cmd.AddParameter("@BabySex", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.BabySex));
                    cmd.AddParameter("@URP_Date", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.URP_Date));
                    cmd.AddParameter("@PFO", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.PFO));
                    cmd.AddParameter("@PCA", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.PCA));
                    cmd.AddParameter("@AnotherDiagnosic", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.AnotherDiagnosic));
                    cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.Notes));
                    cmd.AddParameter("@Tab4_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjUltraResParams_FetalEchocardiographyPostpartum.Tab_Update_Required));

                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    if (retVal > 0)
                        return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool AddAndUpdateURP_FE_Oesophagienne(URP_FE_OesophagienneUltra entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAddAndUpdateURP_FE_Oesophagienne", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLRequestID));
                    cmd.AddParameter("@URP_FE_OesophagienneID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.URP_FE_OesophagienneID));
                    cmd.AddParameter("@ChiDinh", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.ChiDinh));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.DoctorStaffID));
                    //==== #001
                    cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.CreateDate));
                    //==== #001
                    cmd.AddParameter("@Tab1_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.Tab_Update_Required));

                    cmd.AddParameter("@URP_FE_OesophagienneDiagnosisID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneDiagnosis.URP_FE_OesophagienneDiagnosisID));
                    cmd.AddParameter("@ChanDoanQuaThucQuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneDiagnosis.ChanDoanQuaThucQuan));
                    cmd.AddParameter("@Tab2_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.Tab_Update_Required));

                    cmd.AddParameter("@URP_FE_OesophagienneCheckID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.URP_FE_OesophagienneCheckID));
                    cmd.AddParameter("@CatNghia", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.CatNghia));
                    cmd.AddParameter("@NuotNghen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.NuotNghen));
                    cmd.AddParameter("@NuotDau", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.NuotDau));
                    cmd.AddParameter("@OiMau", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.OiMau));
                    cmd.AddParameter("@XaTriTrungThat", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.XaTriTrungThat));
                    cmd.AddParameter("@CotSongCo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.CotSongCo));
                    cmd.AddParameter("@ChanThuongLongNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.ChanThuongLongNguc));
                    cmd.AddParameter("@LanKhamNoiSoiGanDay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.LanKhamNoiSoiGanDay));
                    cmd.AddParameter("@DiUngThuoc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.DiUngThuoc));
                    cmd.AddParameter("@NghienRuou", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.NghienRuou));
                    cmd.AddParameter("@BiTieu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.BiTieu));
                    cmd.AddParameter("@TangNhanApGocHep", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.TangNhanApGocHep));
                    cmd.AddParameter("@Suyen", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.Suyen));
                    cmd.AddParameter("@LanAnSauCung", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.LanAnSauCung));
                    cmd.AddParameter("@RangGiaHamGia", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.RangGiaHamGia));
                    cmd.AddParameter("@HuyetApTT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.HuyetApTT));
                    cmd.AddParameter("@HuyetApTTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.HuyetApTTr));
                    cmd.AddParameter("@Mach", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.Mach));
                    cmd.AddParameter("@DoBaoHoaOxy", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.DoBaoHoaOxy));
                    cmd.AddParameter("@ThucHienDuongTruyenTinhMach", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.ThucHienDuongTruyenTinhMach));
                    cmd.AddParameter("@KiemTraDauDoSieuAm", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.KiemTraDauDoSieuAm));
                    cmd.AddParameter("@ChinhDauDoTrungTinh", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.ChinhDauDoTrungTinh));
                    cmd.AddParameter("@TeMeBenhNhan", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.TeMeBenhNhan));
                    cmd.AddParameter("@DatBenhNhanNghiengTrai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.DatBenhNhanNghiengTrai));
                    cmd.AddParameter("@CotDay", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.CotDay));
                    cmd.AddParameter("@BenhNhanThoaiMai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.BenhNhanThoaiMai));
                    cmd.AddParameter("@BoiTronDauDo", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_OesophagienneCheck.BoiTronDauDo));
                    cmd.AddParameter("@Tab3_Update_Required", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Oesophagienne.Tab_Update_Required));

                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    if (retVal > 0)
                        return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool AddAndUpdateURP_FE_StressDipyridamole(URP_FE_StressDipyridamoleUltra entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAddAndUpdateURP_FE_StressDipyridamole", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLRequestID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleImage.DoctorStaffID));
                    cmd.AddParameter("@URP_FE_StressDipyridamoleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.URP_FE_StressDipyridamoleID));
                    cmd.AddParameter("@TanSoTimCanDat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TanSoTimCanDat));
                    cmd.AddParameter("@TNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TNP_HuyetAp_TT));
                    cmd.AddParameter("@TNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TNP_HuyetAp_TTr));
                    cmd.AddParameter("@TNP_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TNP_TanSoTim));
                    cmd.AddParameter("@TNP_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TNP_TacDungPhu));
                    cmd.AddParameter("@TruyenDipyridamole056_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipyridamole056_DungLuong));
                    cmd.AddParameter("@TruyenDipy056_P2_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P2_HuyetAp_TT));
                    cmd.AddParameter("@TruyenDipy056_P2_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P2_HuyetAp_TTr));
                    cmd.AddParameter("@TruyenDipy056_P2_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P2_TanSoTim));
                    cmd.AddParameter("@TruyenDipy056_P2_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P2_TacDungPhu));
                    cmd.AddParameter("@TruyenDipy056_P4_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P4_HuyetAp_TT));
                    cmd.AddParameter("@TruyenDipy056_P4_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P4_HuyetAp_TTr));
                    cmd.AddParameter("@TruyenDipy056_P4_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P4_TanSoTim));
                    cmd.AddParameter("@TruyenDipy056_P4_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy056_P4_TacDungPhu));
                    cmd.AddParameter("@SauLieuDauP6_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieuDauP6_HuyetAp_TT));
                    cmd.AddParameter("@SauLieuDauP6_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieuDauP6_HuyetAp_TTr));
                    cmd.AddParameter("@SauLieuDauP6_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieuDauP6_TanSoTim));
                    cmd.AddParameter("@SauLieuDauP6_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieuDauP6_TacDungPhu));
                    cmd.AddParameter("@TruyenDipyridamole028_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipyridamole028_DungLuong));
                    cmd.AddParameter("@TruyenDipy028_P8_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy028_P8_HuyetAp_TT));
                    cmd.AddParameter("@TruyenDipy028_P8_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy028_P8_HuyetAp_TTr));
                    cmd.AddParameter("@TruyenDipy028_P8_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy028_P8_TanSoTim));
                    cmd.AddParameter("@TruyenDipy028_P8_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TruyenDipy028_P8_TacDungPhu));
                    cmd.AddParameter("@SauLieu2P10_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieu2P10_HuyetAp_TT));
                    cmd.AddParameter("@SauLieu2P10_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieu2P10_HuyetAp_TTr));
                    cmd.AddParameter("@SauLieu2P10_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieu2P10_TanSoTim));
                    cmd.AddParameter("@SauLieu2P10_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.SauLieu2P10_TacDungPhu));
                    cmd.AddParameter("@ThemAtropineP12_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP12_HuyetAp_TT));
                    cmd.AddParameter("@ThemAtropineP12_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP12_HuyetAp_TTr));
                    cmd.AddParameter("@ThemAtropineP12_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP12_TanSoTim));
                    cmd.AddParameter("@ThemAtropineP12_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP12_TacDungPhu));
                    cmd.AddParameter("@ThemAtropineP13_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP13_HuyetAp_TT));
                    cmd.AddParameter("@ThemAtropineP13_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP13_HuyetAp_TTr));
                    cmd.AddParameter("@ThemAtropineP13_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP13_TanSoTim));
                    cmd.AddParameter("@ThemAtropineP13_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP13_TacDungPhu));
                    cmd.AddParameter("@ThemAtropineP14_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP14_HuyetAp_TT));
                    cmd.AddParameter("@ThemAtropineP14_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP14_HuyetAp_TTr));
                    cmd.AddParameter("@ThemAtropineP14_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP14_TanSoTim));
                    cmd.AddParameter("@ThemAtropineP14_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP14_TacDungPhu));
                    cmd.AddParameter("@ThemAtropineP15_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP15_HuyetAp_TT));
                    cmd.AddParameter("@ThemAtropineP15_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP15_HuyetAp_TTr));
                    cmd.AddParameter("@ThemAtropineP15_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP15_TanSoTim));
                    cmd.AddParameter("@ThemAtropineP15_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAtropineP15_TacDungPhu));
                    cmd.AddParameter("@TheoDoiAtropineP16_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TheoDoiAtropineP16_HuyetAp_TT));
                    cmd.AddParameter("@TheoDoiAtropineP16_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TheoDoiAtropineP16_HuyetAp_TTr));
                    cmd.AddParameter("@TheoDoiAtropineP16_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TheoDoiAtropineP16_TanSoTim));
                    cmd.AddParameter("@TheoDoiAtropineP16_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.TheoDoiAtropineP16_TacDungPhu));
                    cmd.AddParameter("@ThemAminophyline_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAminophyline_DungLuong));
                    cmd.AddParameter("@ThemAminophyline_Phut", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAminophyline_Phut));
                    cmd.AddParameter("@ThemAminophyline_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAminophyline_HuyetAp_TT));
                    cmd.AddParameter("@ThemAminophyline_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAminophyline_HuyetAp_TTr));
                    cmd.AddParameter("@ThemAminophyline_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAminophyline_TanSoTim));
                    cmd.AddParameter("@ThemAminophyline_TacDungPhu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamole.ThemAminophyline_TacDungPhu));

                    cmd.AddParameter("@URP_FE_StressDipyridamoleElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.URP_FE_StressDipyridamoleElectrocardiogramID));
                    cmd.AddParameter("@DieuTriConDauThatNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.DieuTriConDauThatNguc));
                    cmd.AddParameter("@DieuTriDIGITALIS", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.DieuTriDIGITALIS));
                    cmd.AddParameter("@LyDoKhongThucHienDuoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.LyDoKhongThucHienDuoc));
                    cmd.AddParameter("@MucGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.MucGangSuc));
                    cmd.AddParameter("@ThoiGianGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.ThoiGianGangSuc));
                    cmd.AddParameter("@TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.TanSoTim));
                    cmd.AddParameter("@HuyetApToiDa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.HuyetApToiDa));
                    cmd.AddParameter("@ConDauThatNguc", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.ConDauThatNguc));
                    cmd.AddParameter("@STChenhXuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.STChenhXuong));
                    cmd.AddParameter("@RoiLoanNhipTim", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.RoiLoanNhipTim));
                    cmd.AddParameter("@RoiLoanNhipTimChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.RoiLoanNhipTimChiTiet));
                    cmd.AddParameter("@XetNghiemKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleElectrocardiogram.XetNghiemKhac));

                    cmd.AddParameter("@URP_FE_StressDipyridamoleExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleExam.URP_FE_StressDipyridamoleExamID));
                    cmd.AddParameter("@TrieuChungHienTai", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleExam.TrieuChungHienTai));
                    cmd.AddParameter("@ChiDinhSATGSDipy", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleExam.ChiDinhSATGSDipy));
                    cmd.AddParameter("@ChiDinhDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleExam.ChiDinhDetail));
                    cmd.AddParameter("@TDDTruocNgayKham", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleExam.TDDTruocNgayKham));
                    cmd.AddParameter("@TDDTrongNgaySATGSDipy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleExam.TDDTrongNgaySATGSDipy));

                    cmd.AddParameter("@URP_FE_StressDipyridamoleImageID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleImage.URP_FE_StressDipyridamoleImageID));
                    cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleImage.KetLuan));
                    //==== #001
                    cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleImage.CreateDate));
                    //==== #001

                    cmd.AddParameter("@URP_FE_StressDipyridamoleResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.URP_FE_StressDipyridamoleResultID));
                    cmd.AddParameter("@ThayDoiDTD", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThayDoiDTD));
                    cmd.AddParameter("@ThayDoiDTDChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThayDoiDTDChiTiet));
                    cmd.AddParameter("@RoiLoanNhip", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.RoiLoanNhip));
                    cmd.AddParameter("@RoiLoanNhipChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.RoiLoanNhipChiTiet));
                    cmd.AddParameter("@TDPHayBienChung", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TDPHayBienChung));
                    cmd.AddParameter("@TrieuChungKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TrieuChungKhac));
                    cmd.AddParameter("@BienPhapDieuTri", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.BienPhapDieuTri));
                    cmd.AddParameter("@V_KetQuaSieuAmTim", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.V_KetQuaSieuAmTim));
                    cmd.AddParameter("@KetQuaSieuAmTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.KetQuaSieuAmTim));
                    cmd.AddParameter("@ThanhTruoc_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Mom_TNP));
                    cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhTruoc_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Mom_KetLuan));
                    cmd.AddParameter("@ThanhTruoc_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Giua_TNP));
                    cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhTruoc_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Giua_KetLuan));
                    cmd.AddParameter("@ThanhTruoc_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Day_TNP));
                    cmd.AddParameter("@ThanhTruoc_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhTruoc_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhTruoc_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhTruoc_Day_KetLuan));
                    cmd.AddParameter("@VanhLienThat_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Mom_TNP));
                    cmd.AddParameter("@VanhLienThat_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Mom_DobuLieuThap));
                    cmd.AddParameter("@VanhLienThat_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Mom_DobuLieuCao));
                    cmd.AddParameter("@VanhLienThat_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Mom_KetLuan));
                    cmd.AddParameter("@VanhLienThat_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Giua_TNP));
                    cmd.AddParameter("@VanhLienThat_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Giua_DobuLieuThap));
                    cmd.AddParameter("@VanhLienThat_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Giua_DobuLieuCao));
                    cmd.AddParameter("@VanhLienThat_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Giua_KetLuan));
                    cmd.AddParameter("@VanhLienThat_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Day_TNP));
                    cmd.AddParameter("@VanhLienThat_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Day_DobuLieuThap));
                    cmd.AddParameter("@VanhLienThat_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Day_DobuLieuCao));
                    cmd.AddParameter("@VanhLienThat_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.VanhLienThat_Day_KetLuan));
                    cmd.AddParameter("@ThanhDuoi_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Mom_TNP));
                    cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhDuoi_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Mom_KetLuan));
                    cmd.AddParameter("@ThanhDuoi_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Giua_TNP));
                    cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhDuoi_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Giua_KetLuan));
                    cmd.AddParameter("@ThanhDuoi_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Day_TNP));
                    cmd.AddParameter("@ThanhDuoi_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhDuoi_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhDuoi_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhDuoi_Day_KetLuan));
                    cmd.AddParameter("@ThanhSau_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Mom_TNP));
                    cmd.AddParameter("@ThanhSau_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhSau_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhSau_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Mom_KetLuan));
                    cmd.AddParameter("@ThanhSau_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Giua_TNP));
                    cmd.AddParameter("@ThanhSau_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhSau_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhSau_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Giua_KetLuan));
                    cmd.AddParameter("@ThanhSau_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Day_TNP));
                    cmd.AddParameter("@ThanhSau_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhSau_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhSau_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhSau_Day_KetLuan));
                    cmd.AddParameter("@ThanhBen_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Mom_TNP));
                    cmd.AddParameter("@ThanhBen_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhBen_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhBen_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Mom_KetLuan));
                    cmd.AddParameter("@ThanhBen_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Giua_TNP));
                    cmd.AddParameter("@ThanhBen_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhBen_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhBen_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Giua_KetLuan));
                    cmd.AddParameter("@ThanhBen_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Day_TNP));
                    cmd.AddParameter("@ThanhBen_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhBen_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhBen_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.ThanhBen_Day_KetLuan));
                    cmd.AddParameter("@TruocVach_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Mom_TNP));
                    cmd.AddParameter("@TruocVach_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Mom_DobuLieuThap));
                    cmd.AddParameter("@TruocVach_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Mom_DobuLieuCao));
                    cmd.AddParameter("@TruocVach_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Mom_KetLuan));
                    cmd.AddParameter("@TruocVach_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Giua_TNP));
                    cmd.AddParameter("@TruocVach_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Giua_DobuLieuThap));
                    cmd.AddParameter("@TruocVach_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Giua_DobuLieuCao));
                    cmd.AddParameter("@TruocVach_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Giua_KetLuan));
                    cmd.AddParameter("@TruocVach_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Day_TNP));
                    cmd.AddParameter("@TruocVach_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Day_DobuLieuThap));
                    cmd.AddParameter("@TruocVach_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Day_DobuLieuCao));
                    cmd.AddParameter("@TruocVach_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDipyridamoleResult.TruocVach_Day_KetLuan));

                    cmd.AddParameter("@URP_FE_ExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.URP_FE_ExamID));
                    cmd.AddParameter("@CaoHuyetAp", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.CaoHuyetAp));
                    cmd.AddParameter("@CaoHuyetApDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.CaoHuyetApDetail));
                    cmd.AddParameter("@Cholesterol", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.Cholesterol));
                    cmd.AddParameter("@Triglyceride", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.Triglyceride));
                    cmd.AddParameter("@HDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.HDL));
                    cmd.AddParameter("@LDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.LDL));
                    cmd.AddParameter("@TieuDuong", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.TieuDuong));
                    cmd.AddParameter("@TieuDuongDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.TieuDuongDetail));
                    cmd.AddParameter("@ThuocLa", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.ThuocLa));
                    cmd.AddParameter("@Detail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.Detail));
                    cmd.AddParameter("@ThuocNguaThai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.ThuocNguaThai));
                    cmd.AddParameter("@ThuocNguaThaiDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.ThuocNguaThaiDetail));
                    cmd.AddParameter("NhanApMP", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.NhanApMP));
                    cmd.AddParameter("NhanApMT", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.NhanApMT));

                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    if (retVal > 0)
                        return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool AddAndUpdateURP_FE_StressDobutamine(URP_FE_StressDobutamineUltra entity)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spAddAndUpdateURP_FE_StressDobutamine", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.PCLRequestID));
                    cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineImages.DoctorStaffID));
                    cmd.AddParameter("@URP_FE_StressDobutamineID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.URP_FE_StressDobutamineID));
                    cmd.AddParameter("@TruyenTinhMach", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TruyenTinhMach));
                    cmd.AddParameter("@TanSoTimCanDat", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TanSoTimCanDat));
                    cmd.AddParameter("@TD_TNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TD_TNP_HuyetAp_TT));
                    cmd.AddParameter("@TD_TNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TD_TNP_HuyetAp_TTr));
                    cmd.AddParameter("@TD_TNP_HuyetAp_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TD_TNP_HuyetAp_TanSoTim));
                    cmd.AddParameter("@TD_TNP_HuyetAp_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TD_TNP_HuyetAp_DoChenhApMin));
                    cmd.AddParameter("@TD_TNP_HuyetAp_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TD_TNP_HuyetAp_DoChenhApMax));
                    cmd.AddParameter("@FiveMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FiveMicro_DungLuong));
                    cmd.AddParameter("@FiveMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FiveMicro_HuyetAp_TT));
                    cmd.AddParameter("@FiveMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FiveMicro_HuyetAp_TTr));
                    cmd.AddParameter("@FiveMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FiveMicro_TanSoTim));
                    cmd.AddParameter("@FiveMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FiveMicro_DoChenhApMin));
                    cmd.AddParameter("@FiveMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FiveMicro_DoChenhApMax));
                    cmd.AddParameter("@TenMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TenMicro_DungLuong));
                    cmd.AddParameter("@TenMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TenMicro_HuyetAp_TT));
                    cmd.AddParameter("@TenMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TenMicro_HuyetAp_TTr));
                    cmd.AddParameter("@TenMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TenMicro_TanSoTim));
                    cmd.AddParameter("@TenMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TenMicro_DoChenhApMin));
                    cmd.AddParameter("@TenMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TenMicro_DoChenhApMax));
                    cmd.AddParameter("@TwentyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TwentyMicro_DungLuong));
                    cmd.AddParameter("@TwentyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TwentyMicro_HuyetAp_TT));
                    cmd.AddParameter("@TwentyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TwentyMicro_HuyetAp_TTr));
                    cmd.AddParameter("@TwentyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TwentyMicro_TanSoTim));
                    cmd.AddParameter("@TwentyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TwentyMicro_DoChenhApMin));
                    cmd.AddParameter("@TwentyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.TwentyMicro_DoChenhApMax));
                    cmd.AddParameter("@ThirtyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.ThirtyMicro_DungLuong));
                    cmd.AddParameter("@ThirtyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.ThirtyMicro_HuyetAp_TT));
                    cmd.AddParameter("@ThirtyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.ThirtyMicro_HuyetAp_TTr));
                    cmd.AddParameter("@ThirtyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.ThirtyMicro_TanSoTim));
                    cmd.AddParameter("@ThirtyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.ThirtyMicro_DoChenhApMin));
                    cmd.AddParameter("@ThirtyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.ThirtyMicro_DoChenhApMax));
                    cmd.AddParameter("@FortyMicro_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FortyMicro_DungLuong));
                    cmd.AddParameter("@FortyMicro_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FortyMicro_HuyetAp_TT));
                    cmd.AddParameter("@FortyMicro_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FortyMicro_HuyetAp_TTr));
                    cmd.AddParameter("@FortyMicro_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FortyMicro_TanSoTim));
                    cmd.AddParameter("@FortyMicro_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FortyMicro_DoChenhApMin));
                    cmd.AddParameter("@FortyMicro_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.FortyMicro_DoChenhApMax));
                    cmd.AddParameter("@Atropine_DungLuong", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.Atropine_DungLuong));
                    cmd.AddParameter("@Atropine_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.Atropine_HuyetAp_TT));
                    cmd.AddParameter("@Atropine_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.Atropine_HuyetAp_TTr));
                    cmd.AddParameter("@Atropine_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.Atropine_TanSoTim));
                    cmd.AddParameter("@Atropine_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.Atropine_DoChenhApMin));
                    cmd.AddParameter("@Atropine_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.Atropine_DoChenhApMax));
                    cmd.AddParameter("@NgungNP_ThoiGian", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.NgungNP_ThoiGian));
                    cmd.AddParameter("@NgungNP_HuyetAp_TT", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.NgungNP_HuyetAp_TT));
                    cmd.AddParameter("@NgungNP_HuyetAp_TTr", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.NgungNP_HuyetAp_TTr));
                    cmd.AddParameter("@NgungNP_TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.NgungNP_TanSoTim));
                    cmd.AddParameter("@NgungNP_DoChenhApMin", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.NgungNP_DoChenhApMin));
                    cmd.AddParameter("@NgungNP_DoChenhApMax", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamine.NgungNP_DoChenhApMax));

                    cmd.AddParameter("@URP_FE_StressDobutamineElectrocardiogramID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.URP_FE_StressDobutamineElectrocardiogramID));
                    cmd.AddParameter("@DieuTriConDauThatNguc", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.DieuTriConDauThatNguc));
                    cmd.AddParameter("@DieuTriDIGITALIS", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.DieuTriDIGITALIS));
                    cmd.AddParameter("@LyDoKhongThucHienDuoc", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.LyDoKhongThucHienDuoc));
                    cmd.AddParameter("@MucGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.MucGangSuc));
                    cmd.AddParameter("@ThoiGianGangSuc", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.ThoiGianGangSuc));
                    cmd.AddParameter("@TanSoTim", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.TanSoTim));
                    cmd.AddParameter("@HuyetApToiDa", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.HuyetApToiDa));
                    cmd.AddParameter("@ConDauThatNguc", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.ConDauThatNguc));
                    cmd.AddParameter("@STChenhXuong", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.STChenhXuong));
                    cmd.AddParameter("@RoiLoanNhipTim", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.RoiLoanNhipTim));
                    cmd.AddParameter("@RoiLoanNhipTimChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.RoiLoanNhipTimChiTiet));
                    cmd.AddParameter("@XetNghiemKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineElectrocardiogram.XetNghiemKhac));

                    cmd.AddParameter("@URP_FE_StressDobutamineExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineExam.URP_FE_StressDobutamineExamID));
                    cmd.AddParameter("@TrieuChungHienTai", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineExam.TrieuChungHienTai));
                    cmd.AddParameter("@ChiDinhSATGSDobu", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineExam.ChiDinhSATGSDobu));
                    cmd.AddParameter("@ChiDinhDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineExam.ChiDinhDetail));
                    cmd.AddParameter("@TDDTruocNgayKham", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineExam.TDDTruocNgayKham));
                    cmd.AddParameter("@TDDTrongNgaySATGSDobu", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineExam.TDDTrongNgaySATGSDobu));

                    cmd.AddParameter("@URP_FE_StressDobutamineImagesID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineImages.URP_FE_StressDobutamineImagesID));
                    cmd.AddParameter("@KetLuan", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineImages.KetLuan));
                    //==== #001
                    cmd.AddParameter("@CreatedDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineImages.CreateDate));
                    //==== #001

                    cmd.AddParameter("@URP_FE_StressDobutamineResultID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.URP_FE_StressDobutamineResultID));
                    cmd.AddParameter("@ThayDoiDTD", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThayDoiDTD));
                    cmd.AddParameter("@ThayDoiDTDChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThayDoiDTDChiTiet));
                    cmd.AddParameter("@RoiLoanNhip", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.RoiLoanNhip));
                    cmd.AddParameter("@RoiLoanNhipChiTiet", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.RoiLoanNhipChiTiet));
                    cmd.AddParameter("@TDPHayBienChung", SqlDbType.Int, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TDPHayBienChung));
                    cmd.AddParameter("@TrieuChungKhac", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TrieuChungKhac));
                    cmd.AddParameter("@BienPhapDieuTri", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.BienPhapDieuTri));
                    cmd.AddParameter("@V_KetQuaSieuAmTim", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.V_KetQuaSieuAmTim));
                    cmd.AddParameter("@KetQuaSieuAmTim", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.KetQuaSieuAmTim));
                    cmd.AddParameter("@ThanhTruoc_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Mom_TNP));
                    cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhTruoc_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhTruoc_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Mom_KetLuan));
                    cmd.AddParameter("@ThanhTruoc_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Giua_TNP));
                    cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhTruoc_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhTruoc_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Giua_KetLuan));
                    cmd.AddParameter("@ThanhTruoc_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Day_TNP));
                    cmd.AddParameter("@ThanhTruoc_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhTruoc_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhTruoc_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhTruoc_Day_KetLuan));
                    cmd.AddParameter("@VanhLienThat_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Mom_TNP));
                    cmd.AddParameter("@VanhLienThat_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Mom_DobuLieuThap));
                    cmd.AddParameter("@VanhLienThat_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Mom_DobuLieuCao));
                    cmd.AddParameter("@VanhLienThat_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Mom_KetLuan));
                    cmd.AddParameter("@VanhLienThat_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Giua_TNP));
                    cmd.AddParameter("@VanhLienThat_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Giua_DobuLieuThap));
                    cmd.AddParameter("@VanhLienThat_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Giua_DobuLieuCao));
                    cmd.AddParameter("@VanhLienThat_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Giua_KetLuan));
                    cmd.AddParameter("@VanhLienThat_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Day_TNP));
                    cmd.AddParameter("@VanhLienThat_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Day_DobuLieuThap));
                    cmd.AddParameter("@VanhLienThat_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Day_DobuLieuCao));
                    cmd.AddParameter("@VanhLienThat_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.VanhLienThat_Day_KetLuan));
                    cmd.AddParameter("@ThanhDuoi_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Mom_TNP));
                    cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhDuoi_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhDuoi_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Mom_KetLuan));
                    cmd.AddParameter("@ThanhDuoi_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Giua_TNP));
                    cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhDuoi_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhDuoi_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Giua_KetLuan));
                    cmd.AddParameter("@ThanhDuoi_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Day_TNP));
                    cmd.AddParameter("@ThanhDuoi_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhDuoi_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhDuoi_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhDuoi_Day_KetLuan));
                    cmd.AddParameter("@ThanhSau_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Mom_TNP));
                    cmd.AddParameter("@ThanhSau_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhSau_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhSau_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Mom_KetLuan));
                    cmd.AddParameter("@ThanhSau_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Giua_TNP));
                    cmd.AddParameter("@ThanhSau_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhSau_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhSau_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Giua_KetLuan));
                    cmd.AddParameter("@ThanhSau_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Day_TNP));
                    cmd.AddParameter("@ThanhSau_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhSau_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhSau_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhSau_Day_KetLuan));
                    cmd.AddParameter("@ThanhBen_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Mom_TNP));
                    cmd.AddParameter("@ThanhBen_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Mom_DobuLieuThap));
                    cmd.AddParameter("@ThanhBen_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Mom_DobuLieuCao));
                    cmd.AddParameter("@ThanhBen_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Mom_KetLuan));
                    cmd.AddParameter("@ThanhBen_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Giua_TNP));
                    cmd.AddParameter("@ThanhBen_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Giua_DobuLieuThap));
                    cmd.AddParameter("@ThanhBen_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Giua_DobuLieuCao));
                    cmd.AddParameter("@ThanhBen_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Giua_KetLuan));
                    cmd.AddParameter("@ThanhBen_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Day_TNP));
                    cmd.AddParameter("@ThanhBen_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Day_DobuLieuThap));
                    cmd.AddParameter("@ThanhBen_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Day_DobuLieuCao));
                    cmd.AddParameter("@ThanhBen_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.ThanhBen_Day_KetLuan));
                    cmd.AddParameter("@TruocVach_Mom_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Mom_TNP));
                    cmd.AddParameter("@TruocVach_Mom_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Mom_DobuLieuThap));
                    cmd.AddParameter("@TruocVach_Mom_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Mom_DobuLieuCao));
                    cmd.AddParameter("@TruocVach_Mom_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Mom_KetLuan));
                    cmd.AddParameter("@TruocVach_Giua_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Giua_TNP));
                    cmd.AddParameter("@TruocVach_Giua_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Giua_DobuLieuThap));
                    cmd.AddParameter("@TruocVach_Giua_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Giua_DobuLieuCao));
                    cmd.AddParameter("@TruocVach_Giua_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Giua_KetLuan));
                    cmd.AddParameter("@TruocVach_Day_TNP", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Day_TNP));
                    cmd.AddParameter("@TruocVach_Day_DobuLieuThap", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Day_DobuLieuThap));
                    cmd.AddParameter("@TruocVach_Day_DobuLieuCao", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Day_DobuLieuCao));
                    cmd.AddParameter("@TruocVach_Day_KetLuan", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_StressDobutamineResult.TruocVach_Day_KetLuan));

                    cmd.AddParameter("@URP_FE_ExamID", SqlDbType.BigInt, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.URP_FE_ExamID));
                    cmd.AddParameter("@CaoHuyetAp", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.CaoHuyetAp));
                    cmd.AddParameter("@CaoHuyetApDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.CaoHuyetApDetail));
                    cmd.AddParameter("@Cholesterol", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.Cholesterol));
                    cmd.AddParameter("@Triglyceride", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.Triglyceride));
                    cmd.AddParameter("@HDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.HDL));
                    cmd.AddParameter("@LDL", SqlDbType.Float, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.LDL));
                    cmd.AddParameter("@TieuDuong", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.TieuDuong));
                    cmd.AddParameter("@TieuDuongDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.TieuDuongDetail));
                    cmd.AddParameter("@ThuocLa", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.ThuocLa));
                    cmd.AddParameter("@Detail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.Detail));
                    cmd.AddParameter("@ThuocNguaThai", SqlDbType.Bit, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.ThuocNguaThai));
                    cmd.AddParameter("@ThuocNguaThaiDetail", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.ThuocNguaThaiDetail));
                    cmd.AddParameter("NhanApMP", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.NhanApMP));
                    cmd.AddParameter("NhanApMT", SqlDbType.NVarChar, ConvertNullObjectToDBNull(entity.ObjURP_FE_Exam.NhanApMT));

                    cn.Open();
                    int retVal = ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    if (retVal > 0)
                        return true;
                    else return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //==== 20161129 CMN End: Add button save for all FE pages
        #endregion
        public IList<PatientPCLRequestDetail> GetPCLExamTypeServiceApmtTarget(long PCLExamTypeID, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection CurrentConnection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand CurrentCommand = new SqlCommand("spGetPCLExamTypeServiceApmtTarget", CurrentConnection);
                CurrentCommand.CommandType = CommandType.StoredProcedure;
                CurrentCommand.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, PCLExamTypeID);
                CurrentCommand.AddParameter("@FromDate", SqlDbType.DateTime, FromDate);
                CurrentCommand.AddParameter("@ToDate", SqlDbType.DateTime, ToDate);
                CurrentConnection.Open();
                IDataReader CurrentReader = ExecuteReader(CurrentCommand);
                IList<PatientPCLRequestDetail> ItemCollection = GetPatientPCLRequestDetailsCollectionFromReader(CurrentReader);
                CurrentReader.Close();
                CleanUpConnectionAndCommand(CurrentConnection, CurrentCommand);
                return ItemCollection;
            }
        }

        public IList<PatientPCLRequest> PatientPCLRequest_SearchPaging_ForMedicalExamination(
         PatientPCLRequestSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total
   )
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPatientPCLRequest_SearchPaging_ForMeicalExamination", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PCLExamTypeLocationsDeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeLocationsDeptLocationID));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLMainCategory));
                cmd.AddParameter("@PCLExamTypeSubCategoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLExamTypeSubCategoryID));
                cmd.AddParameter("@PCLResultParamImpID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.PCLResultParamImpID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteria.ToDate));
                cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_PCLRequestStatus));
                cmd.AddParameter("@V_ExamRegStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteria.V_ExamRegStatus));
                cmd.AddParameter("@PatientCode", SqlDbType.Char, ConvertNullObjectToDBNull(SearchCriteria.PatientCode));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteria.FullName));
                cmd.AddParameter("@PCLRequestNumID", SqlDbType.VarChar, ConvertNullObjectToDBNull(SearchCriteria.PCLRequestNumID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull((long)SearchCriteria.PatientFindBy));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@HosClientID", SqlDbType.BigInt, SearchCriteria.HosClientID);

                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cmd.CommandTimeout = int.MaxValue;

                cn.Open();
                List<PatientPCLRequest> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }

        //▼====: #002
        public void UpdateReceptionTime(long PatientPCLReqID, DateTime ReceptionTime, long V_PCLRequestType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateReceptionTime", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cmd.AddParameter("@ReceptionTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(ReceptionTime));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLRequestType));

                cn.Open();
                ExecuteNonQuery(cmd);

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public void UpdateDateStarted2(long PtRegDetailID, DateTime DateStarted2)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateDateStarted2", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegDetailID));
                cmd.AddParameter("@DateStarted2", SqlDbType.DateTime, ConvertNullObjectToDBNull(DateStarted2));

                cn.Open();
                ExecuteNonQuery(cmd);

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public void UpdatePrescriptionsDate(long PtRegistrationID, DateTime PrescriptionsDate)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdatePrescriptionsDate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@PrescriptionsDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(PrescriptionsDate));

                cn.Open();
                ExecuteNonQuery(cmd);

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public IList<PatientPCLRequestDetail> GetHistoryPCLByPCLExamType(long PatientID, long PCLExamTypeID, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetHistoryPCLByPCLExamType", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PCLExamTypeID));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);


                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cmd.CommandTimeout = int.MaxValue;

                cn.Open();
                List<PatientPCLRequestDetail> lst = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetPatientPCLRequestDetailsCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }
        //▲====: #002
        public IList<PatientPCLImagingResultDetail> PCLImagingResults_With_ResultOld(long PatientID, long PatientPCLReqID, long V_PCLRequestType)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spPCLImagingResults_With_ResultOld", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientID));
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PatientPCLReqID));
                cmd.AddParameter("@V_PCLRequestType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_PCLRequestType));
                cn.Open();
                List<PatientPCLImagingResultDetail> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLImagingResultDetailCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        public virtual List<PatientPCLImagingResultDetail> GetPatientPCLImagingResultDetailCollectionFromReader(IDataReader reader)
        {
            List<PatientPCLImagingResultDetail> p = new List<PatientPCLImagingResultDetail>();
            while (reader.Read())
            {
                p.Add(GetPatientPCLImagingResultDetailFromReader(reader));
            }
            return p;
        }
        protected virtual PatientPCLImagingResultDetail GetPatientPCLImagingResultDetailFromReader(IDataReader reader)
        {
            PatientPCLImagingResultDetail p = new PatientPCLImagingResultDetail();

            if (reader.HasColumn("PCLExamTestItemID") && reader["PCLExamTestItemID"] != DBNull.Value)
            {
                p.PCLExamTestItemID = (long)reader["PCLExamTestItemID"];
            }

            if (reader.HasColumn("PCLExamTypeTestItemID") && reader["PCLExamTypeTestItemID"] != DBNull.Value)
            {
                p.PCLExamTypeTestItemID = (long)reader["PCLExamTypeTestItemID"];
            }

            if (reader.HasColumn("PCLImgResultDetailID") && reader["PCLImgResultDetailID"] != DBNull.Value)
            {
                p.PCLImgResultDetailID = (long)reader["PCLImgResultDetailID"];
            }


            p.V_PCLExamTypeTestItems = new PCLExamTypeTestItems();
            p.V_PCLExamTypeTestItems.V_PCLExamTestItem = new PCLExamTestItems();
            p.V_PCLExamTypeTestItems.V_PCLExamType = new PCLExamType();
            p.V_PCLExamTypeTestItems.V_PCLExamType.ObjV_PCLExamTypeUnit = new Lookup();

            if (reader.HasColumn("PCLImgResultDetailID") && reader["PCLImgResultDetailID"] != DBNull.Value)
            {
                p.PCLImgResultDetailID = (long)reader["PCLImgResultDetailID"];
            }

            if (reader.HasColumn("PCLImgResultID") && reader["PCLImgResultID"] != DBNull.Value)
            {
                p.PCLImgResultID = (long)reader["PCLImgResultID"];
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
                p.V_PCLExamTypeTestItems.V_PCLExamTestItem.PCLExamTestItemUnit = reader["PCLExamTestItemUnit"].ToString();
                p.PCLExamTestItemUnit = reader["PCLExamTestItemUnit"].ToString();
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
            p.PatientPCLLaboratoryResult = new PatientPCLLaboratoryResult();
            if (reader.HasColumn("Suggest") && reader["Suggest"] != DBNull.Value)
            {
                p.PatientPCLLaboratoryResult.Suggest = (string)reader["Suggest"];
            }
            if (reader.HasColumn("PCLSectionID") && reader["PCLSectionID"] != DBNull.Value)
            {
                p.PCLSectionID = Convert.ToInt64(reader["PCLSectionID"]);
            }
            if (reader.HasColumn("SectionName") && reader["SectionName"] != DBNull.Value)
            {
                p.SectionName = Convert.ToString(reader["SectionName"]);
            }
            //▼====: #001
            //if (reader.HasColumn("HIRepResourceCode") && reader["HIRepResourceCode"] != DBNull.Value)
            //{
            //    p.HIRepResourceCode = Convert.ToString(reader["HIRepResourceCode"]);
            //}
            //▲====: #001
            if (reader.HasColumn("IsForMen"))
            {
                if (reader["IsForMen"] == DBNull.Value)
                {
                    p.IsForMen = null;
                }
                else
                {
                    p.IsForMen = Convert.ToBoolean(reader["IsForMen"]);
                }
            }
            if (reader.HasColumn("PrintIdx") && reader["PrintIdx"] != DBNull.Value)
            {
                p.PrintIdx = Convert.ToInt32(reader["PrintIdx"]);
            }
            return p;
        }
        public IList<PatientPCLRequest> GetPatientPCLLaboratoryResultForSendTransaction(PatientPCLRequestSearchCriteria SearchCriteriaSendTransaction
            , int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetPatientPCLLaboratoryResultForSendTransaction", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.ToDate));
                cmd.AddParameter("@PatientFindBy", SqlDbType.Int, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.PatientFindBy));
                cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.PatientCode));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.FullName));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                List<PatientPCLRequest> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLRequestCollectionFromReader(reader);
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        public IList<PatientPCLRequest> GetListHistoryTransaction_Paging(PatientPCLRequestSearchCriteria SearchCriteriaSendTransaction
            , int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetListHistoryTransaction_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.ToDate));
                cmd.AddParameter("@PatientFindBy", SqlDbType.Int, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.PatientFindBy));
                cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.PatientCode));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.FullName));
                cmd.AddParameter("@V_TransactionStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.V_TransactionStatus));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                List<PatientPCLRequest> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLRequestCollectionFromReader(reader);
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }

        public bool UpdatePCLRequestStatusGeneratingResult(string ListPCLResults, long V_PCLRequestStatus)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdatePCLRequestStatusGeneratingResult", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ListPCLResults", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ListPCLResults));
                cmd.AddParameter("@V_PCLRequestStatus", SqlDbType.BigInt, V_PCLRequestStatus);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                {
                    return true;
                }
                return false;
            }
        }
        public bool DeletePCLRequestFromList(PatientPCLRequest request, int PatientFindBy)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeletePCLRequestFromList", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.PatientPCLReqID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.Int, ConvertNullObjectToDBNull(PatientFindBy));
                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool DeleteTransactionHistory(PatientPCLRequest request, int PatientFindBy)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteTransactionHistory", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PatientPCLReqID", SqlDbType.BigInt, ConvertNullObjectToDBNull(request.PatientPCLReqID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.Int, ConvertNullObjectToDBNull(PatientFindBy));
                cn.Open();

                int retVal = ExecuteNonQuery(cmd);
                CleanUpConnectionAndCommand(cn, cmd);
                if (retVal > 0)
                {
                    return true;
                }
                return false;
            }
        }

        //▼====: #005
        public List<Resources> GetResourcesForMedicalServicesListByDeptIDAndTypeID(long DeptID, long PtRegDetailID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourcesForMedicalServicesFor130", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.VarChar, ConvertNullObjectToDBNull(DeptID));
                cmd.AddParameter("@PtRegDetailID", SqlDbType.VarChar, ConvertNullObjectToDBNull(PtRegDetailID));

                cn.Open();
                List<Resources> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetResourcesForMedicalServicesListByTypeIDCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public List<Resources> GetResourcesForMedicalServicesListBySmallProcedureID(long SmallProcedureID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourcesForMedicalServicesBySmallProcedureID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@SmallProcedureID", SqlDbType.BigInt, ConvertNullObjectToDBNull(SmallProcedureID));

                cn.Open();
                List<Resources> retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                retVal = GetResourcesForMedicalServicesListByTypeIDCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        //▲====: #005

        //▼====: #009
        public IList<PatientPCLRequest> GetListAppointmentsLab_Paging(PatientPCLRequestSearchCriteria SearchCriteriaSendTransaction
            , int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetListAppointmentsLab_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.ToDate));
                cmd.AddParameter("@V_LabType", SqlDbType.Int, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.V_LabType));
                cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.PatientCode));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.FullName));
                cmd.AddParameter("@V_SendSMSStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(SearchCriteriaSendTransaction.V_SendSMSStatus));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                List<PatientPCLRequest> objLst = null;
                IDataReader reader = ExecuteReader(cmd);
                objLst = GetPatientPCLRequestCollectionFromReader(reader);
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != DBNull.Value)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    Total = -1;

                CleanUpConnectionAndCommand(cn, cmd);
                return objLst;
            }
        }
        //▲====: #009
    }
}
