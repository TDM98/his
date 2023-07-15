/*
20161223 #001 CMN: Add file manager
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using DataEntities;
using System.Data.SqlTypes;
using eHCMS.Configurations;
using eHCMS.Caching;
using ErrorLibrary;
using System.Reflection;
using System.Collections.ObjectModel;

namespace eHCMS.DAL
{
    public class SqlClinicManagementProvider : ClinicManagementProvider
    {
        public SqlClinicManagementProvider()
            : base()
        {

        }
        public override bool InsertConsultationTimeSegments(string SegmentName
                                                                , string SegmentDescription
                                                                , DateTime StartTime
                                                                , DateTime EndTime
                                                                , bool IsActive)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationTimeSegmentsInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SegmentName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SegmentName));
                cmd.AddParameter("@SegmentDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SegmentDescription));
                cmd.AddParameter("@StartTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartTime));
                cmd.AddParameter("@EndTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(EndTime));
                cmd.AddParameter("@IsActive", SqlDbType.NVarChar, ConvertNullObjectToDBNull(IsActive));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool UpdateConsultationTimeSegments(long ConsultationTimeSegmentID
                                                            , string SegmentName
                                                              , string SegmentDescription
                                                              , DateTime StartTime
                                                              , DateTime EndTime)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationTimeSegmentsUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ConsultationTimeSegmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultationTimeSegmentID));
                cmd.AddParameter("@SegmentName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SegmentName));
                cmd.AddParameter("@SegmentDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SegmentDescription));
                cmd.AddParameter("@StartTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartTime));
                cmd.AddParameter("@EndTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(EndTime));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool DeleteConsultationTimeSegments(long ConsultationTimeSegmentID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationTimeSegmentsDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ConsultationTimeSegmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultationTimeSegmentID));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override List<ConsultationTimeSegments> GetAllConsultationTimeSegmentsCache()
        {
            try
            {
                string mainCacheKey = "AllConsultationTimeSegments";
                List<ConsultationTimeSegments> AllConsultationTimeSegments;
                //Kiểm tra nếu có trong cache thì lấy từ trong cache.
                if (ServerAppConfig.CachingEnabled)
                {
                    AllConsultationTimeSegments = (List<ConsultationTimeSegments>)AxCache.Current[mainCacheKey];
                    if (AllConsultationTimeSegments != null)
                    {
                        return AllConsultationTimeSegments;
                    }
                }

                AllConsultationTimeSegments = ClinicManagementProvider.instance.GetAllConsultationTimeSegments();

                //Lưu vào cache để lần sau sử dụng.
                if (ServerAppConfig.CachingEnabled)
                {
                    if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                    {
                        AxCache.Current[mainCacheKey] = AllConsultationTimeSegments;
                    }
                    else
                    {
                        AxCache.Current.Insert(mainCacheKey, AllConsultationTimeSegments, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                    }
                }
                return AllConsultationTimeSegments;
            }
            catch
            {
                return null;
            }
        }

        public override List<ConsultationTimeSegments> GetAllConsultationTimeSegments()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationTimeSegmentGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                List<ConsultationTimeSegments> objList = null;

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetConsultationTimeSegmentsCollectionFromReader(reader);
                }
                reader.Close();
                return objList;

            }
        }

        public override List<PCLTimeSegment> GetAllPCLTimeSegments()
        {
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("spPclTimeSegment_GetAll", cn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                cn.Open();

                List<PCLTimeSegment> objList = null;

                var reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    objList = GetPclTimeSegmentCollectionFromReader(reader);
                    reader.Close();
                }
                
                return objList;

            }
        }

        public override List<ConsultationTimeSegments> ConsultationTimeSegments_ByDeptLocationID(long DeptLocationID)
        {
            List<ConsultationTimeSegments> listRG = new List<ConsultationTimeSegments>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationTimeSegments_ByDeptLocationID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    ConsultationTimeSegments p = new ConsultationTimeSegments();
                    p.ConsultationTimeSegmentID = (long)reader["ConsultationTimeSegmentID"];
                    p.SegmentName = reader["SegmentName"].ToString();
                    p.SegmentDescription = reader["SegmentDescription"].ToString();
                    p.StartTime = (DateTime)reader["StartTime"];
                    p.EndTime = (DateTime)reader["EndTime"];

                    listRG.Add(p);
                }
                reader.Close();
                return listRG;
            }
        }


        public override bool InsertConsultationRoomTarget(ConsultationRoomTarget consultationRoomTarget)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomTargetInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ConsultationTimeSegmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(consultationRoomTarget.ConsultationTimeSegmentID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(consultationRoomTarget.DeptLocationID));
                //cmd.AddParameter("@TargetDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(consultationRoomTarget.TargetDate));
                //cmd.AddParameter("@TargetNumberOfCases", SqlDbType.SmallInt, ConvertNullObjectToDBNull(consultationRoomTarget.TargetNumberOfCases));
                //cmd.AddParameter("@ForDaysInAWeek", SqlDbType.SmallInt, ConvertNullObjectToDBNull(consultationRoomTarget.GetNumOfDay()));
                //cmd.AddParameter("@MaxNumConsultationAllowed", SqlDbType.SmallInt, ConvertNullObjectToDBNull(consultationRoomTarget.MaxNumConsultationAllowed));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool InsertConsultationRoomTargetXML(List<ConsultationRoomTarget> lstCRSA)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomTargetInsertXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, CRTA_ConvertListToXml(lstCRSA));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool UpdateConsultationRoomTargetXML(IList<ConsultationRoomTarget> lstCRSA)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomTargetUpdateXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, CRTA_ConvertListToXml(lstCRSA));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool DeleteConsultationRoomTarget(long ConsultationRoomTargetID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomTargetDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ConsultationRoomTargetID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultationRoomTargetID));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override List<ConsultationRoomTarget> GetConsultationRoomTargetByDeptID(long DeptLocationID)
        {
            List<ConsultationRoomTarget> listRG = new List<ConsultationRoomTarget>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomTargetByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRG = GetConsultationRoomTargetCollectionFromReader(reader);
                reader.Close();
                return listRG;
            }
        }

        public override List<ConsultationRoomTarget> GetConsultationRoomTargetTimeSegment(long DeptLocationID, long ConsultationTimeSegmentID)
        {
            List<ConsultationRoomTarget> listRG = new List<ConsultationRoomTarget>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomTargetByTimeSegment", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@ConsultationTimeSegmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultationTimeSegmentID));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                try
                {
                    listRG = GetConsultationRoomTargetCollectionFromReader(reader);
                }
                catch
                {

                }
                reader.Close();
                return listRG;
            }
        }

        public override void GetAllConsultationRoomTargetCache()
        {
            try
            {
                string mainCacheKey = "AllConsultationRoomTarget";
                List<ConsultationRoomTarget> allConsultationRoomTarget;
                //Kiểm tra nếu có trong cache thì lấy từ trong cache.
                if (ServerAppConfig.CachingEnabled)
                {
                    allConsultationRoomTarget = (List<ConsultationRoomTarget>)AxCache.Current[mainCacheKey];
                    if (allConsultationRoomTarget != null)
                    {
                        return;
                    }
                }

                allConsultationRoomTarget = ClinicManagementProvider.instance.GetConsultationRoomTargetTimeSegment(0, 0);

                //Lưu vào cache để lần sau sử dụng.
                if (ServerAppConfig.CachingEnabled)
                {
                    if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                    {
                        AxCache.Current[mainCacheKey] = allConsultationRoomTarget;
                    }
                    else
                    {
                        AxCache.Current.Insert(mainCacheKey, allConsultationRoomTarget, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                    }
                }                
            }
            catch
            {
                
            }
        }

        public override List<ConsultationRoomTarget> GetConsultationRoomTargetTSegment(long DeptLocationID,bool IsHis)
        {
            List<ConsultationRoomTarget> listRG = new List<ConsultationRoomTarget>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomTargetByTSegment", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@IsHis", SqlDbType.BigInt, ConvertNullObjectToDBNull(IsHis)); 
                

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                try
                {
                    listRG = GetConsultationRoomTargetCollectionFromReader(reader);
                }
                catch
                {

                }
                reader.Close();
                return listRG;
            }
        }

        public override bool InsertConsultationRoomStaffAllocations(long DeptLocationID
                                                           , long ConsultationTimeSegmentID
                                                           , long StaffID
                                                           , long StaffCatgID
                                                           , DateTime AllocationDate)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomStaffAllocationsInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@ConsultationTimeSegmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultationTimeSegmentID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@StaffCatgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffCatgID));
                cmd.AddParameter("@AllocationDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AllocationDate));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool InsertConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomStaffAllocationsInsertXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.AddParameter("@DataXML", SqlDbType.Xml, CRSA_ConvertListToXml(lstCRSA));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private string CRSA_ConvertListToXml(IList<ConsultationRoomStaffAllocations> lstCRSA)
        {
            if (lstCRSA != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (ConsultationRoomStaffAllocations item in lstCRSA)
                {
                    sb.Append("<ConsultationRoomStaffAllocations>");
                    sb.AppendFormat("<DeptLocationID>{0}</DeptLocationID>", item.DeptLocationID);
                    sb.AppendFormat("<ConsultationTimeSegmentID>{0}</ConsultationTimeSegmentID>", item.ConsultationTimeSegmentID);
                    sb.AppendFormat("<StaffID>{0}</StaffID>", item.StaffID);
                    sb.AppendFormat("<StaffCatgID>{0}</StaffCatgID>", item.StaffCatgID);
                    sb.AppendFormat("<IsActive>{0}</IsActive>", item.IsActive);
                    //sb.AppendFormat("<AllocationDate>{0}</AllocationDate>", (SqlDateTime)item.AllocationDate);
                    sb.AppendFormat("<AllocationDate>{0}</AllocationDate>", item.AllocationDate.ToString("MM-dd-yyyy HH:mm:ss"));
                    
                    sb.Append("</ConsultationRoomStaffAllocations>");

                }
                sb.Append("</DS>");
                
                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        private string CRSADelete_ConvertListToXml(IList<ConsultationRoomStaffAllocations> lstCRSA)
        {
            if (lstCRSA != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (ConsultationRoomStaffAllocations item in lstCRSA)
                {
                    sb.Append("<ConsultationRoomStaffAllocations>");
                    
                    sb.AppendFormat("<ConsultationRoomStaffAllocID>{0}</ConsultationRoomStaffAllocID>", item.ConsultationRoomStaffAllocID);
                    sb.AppendFormat("<IsActive>{0}</IsActive>", item.IsActive);
                    
                    sb.Append("</ConsultationRoomStaffAllocations>");

                }
                sb.Append("</DS>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        private string CRTA_ConvertListToXml(IList<ConsultationRoomTarget> lstCRSA)
        {
            if (lstCRSA != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (ConsultationRoomTarget item in lstCRSA)
                {
                    sb.Append("<ConsultationRoomTarget>");
                    
                    sb.AppendFormat("<ConsultationRoomTargetID>{0}</ConsultationRoomTargetID>", item.ConsultationRoomTargetID);
                    
                    sb.AppendFormat("<ConsultationTimeSegmentID>{0}</ConsultationTimeSegmentID>", item.ConsultationTimeSegmentID);
				  
                    sb.AppendFormat("<DeptLocationID>{0}</DeptLocationID>", item.DeptLocationID);
				  
                    sb.AppendFormat("<MondayTargetNumberOfCases>{0}</MondayTargetNumberOfCases>", item.MondayTargetNumberOfCases);
				  
                    sb.AppendFormat("<MondayMaxNumConsultationAllowed>{0}</MondayMaxNumConsultationAllowed>", item.MondayMaxNumConsultationAllowed);
				  
                    sb.AppendFormat("<TuesdayTargetNumberOfCases>{0}</TuesdayTargetNumberOfCases>", item.TuesdayTargetNumberOfCases);
				  
                    sb.AppendFormat("<TuesdayMaxNumConsultationAllowed>{0}</TuesdayMaxNumConsultationAllowed>", item.TuesdayMaxNumConsultationAllowed);
				  
                    sb.AppendFormat("<WednesdayTargetNumberOfCases>{0}</WednesdayTargetNumberOfCases>", item.WednesdayTargetNumberOfCases);
				  
                    sb.AppendFormat("<WednesdayMaxNumConsultationAllowed>{0}</WednesdayMaxNumConsultationAllowed>", item.WednesdayMaxNumConsultationAllowed);
				  
                    sb.AppendFormat("<ThursdayTargetNumberOfCases>{0}</ThursdayTargetNumberOfCases>", item.ThursdayTargetNumberOfCases);
				  
                    sb.AppendFormat("<ThursdayMaxNumConsultationAllowed>{0}</ThursdayMaxNumConsultationAllowed>", item.ThursdayMaxNumConsultationAllowed);
				  
                    sb.AppendFormat("<FridayTargetNumberOfCases>{0}</FridayTargetNumberOfCases>", item.FridayTargetNumberOfCases);
				  
                    sb.AppendFormat("<FridayMaxNumConsultationAllowed>{0}</FridayMaxNumConsultationAllowed>", item.FridayMaxNumConsultationAllowed);
				  
                    sb.AppendFormat("<SaturdayTargetNumberOfCases>{0}</SaturdayTargetNumberOfCases>", item.SaturdayTargetNumberOfCases);
				  
                    sb.AppendFormat("<SaturdayMaxNumConsultationAllowed>{0}</SaturdayMaxNumConsultationAllowed>", item.SaturdayMaxNumConsultationAllowed);
				  
                    sb.AppendFormat("<SundayTargetNumberOfCases>{0}</SundayTargetNumberOfCases>", item.SundayTargetNumberOfCases);
				  
                    sb.AppendFormat("<SundayMaxNumConsultationAllowed>{0}</SundayMaxNumConsultationAllowed>", item.SundayMaxNumConsultationAllowed);


                    //Số bắt đầu hẹn, kết thúc hẹn
                    sb.AppendFormat("<MondayStartSequenceNumber>{0}</MondayStartSequenceNumber>", item.MondayStartSequenceNumber);
                    sb.AppendFormat("<MondayEndSequenceNumber>{0}</MondayEndSequenceNumber>", item.MondayEndSequenceNumber);

                    sb.AppendFormat("<TuesdayStartSequenceNumber>{0}</TuesdayStartSequenceNumber>", item.TuesdayStartSequenceNumber);
                    sb.AppendFormat("<TuesdayEndSequenceNumber>{0}</TuesdayEndSequenceNumber>", item.TuesdayEndSequenceNumber);

                    sb.AppendFormat("<WednesdayStartSequenceNumber>{0}</WednesdayStartSequenceNumber>", item.WednesdayStartSequenceNumber);
                    sb.AppendFormat("<WednesdayEndSequenceNumber>{0}</WednesdayEndSequenceNumber>", item.WednesdayEndSequenceNumber);

                    sb.AppendFormat("<ThursdayStartSequenceNumber>{0}</ThursdayStartSequenceNumber>", item.ThursdayStartSequenceNumber);
                    sb.AppendFormat("<ThursdayEndSequenceNumber>{0}</ThursdayEndSequenceNumber>", item.ThursdayEndSequenceNumber);

                    sb.AppendFormat("<FridayStartSequenceNumber>{0}</FridayStartSequenceNumber>", item.FridayStartSequenceNumber);
                    sb.AppendFormat("<FridayEndSequenceNumber>{0}</FridayEndSequenceNumber>", item.FridayEndSequenceNumber);

                    sb.AppendFormat("<SaturdayStartSequenceNumber>{0}</SaturdayStartSequenceNumber>", item.SaturdayStartSequenceNumber);
                    sb.AppendFormat("<SaturdayEndSequenceNumber>{0}</SaturdayEndSequenceNumber>", item.SaturdayEndSequenceNumber);

                    sb.AppendFormat("<SundayStartSequenceNumber>{0}</SundayStartSequenceNumber>", item.SundayStartSequenceNumber);
                    sb.AppendFormat("<SundayEndSequenceNumber>{0}</SundayEndSequenceNumber>", item.SundayEndSequenceNumber);

                    //Số bắt đầu hẹn, kết thúc hẹn

                    sb.AppendFormat("<IsDeleted>{0}</IsDeleted>", item.IsDeleted);

                    sb.Append("</ConsultationRoomTarget>");

                }
                sb.Append("</DS>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        public override bool UpdateConsultationRoomStaffAllocations(long ConsultationRoomStaffAllocID, bool IsActive)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomStaffAllocationsUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ConsultationRoomStaffAllocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultationRoomStaffAllocID));
                cmd.AddParameter("@IsActive", SqlDbType.BigInt, ConvertNullObjectToDBNull(IsActive));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool UpdateConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomStaffAllocationsUpdateXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, CRSADelete_ConvertListToXml(lstCRSA));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override bool DeleteConsultationRoomStaffAllocations(long DeptLocationID 
                                                                , long ConsultationTimeSegmentID 
                                                                , DateTime AllocationDate )
        {
            
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomStaffAllocationsDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@ConsultationTimeSegmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultationTimeSegmentID));
                cmd.AddParameter("@AllocationDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AllocationDate));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                if (val > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public override List<ConsultationRoomStaffAllocations> GetConsultationRoomStaffAllocations(long DeptLocationID
                                                        , long ConsultationTimeSegmentID)
        {
            List<ConsultationRoomStaffAllocations> listRG = new List<ConsultationRoomStaffAllocations>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomStaffAllocationsGetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@ConsultationTimeSegmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultationTimeSegmentID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRG = GetConsultationRoomStaffAllocationsCollectionFromReader(reader);
                reader.Close();
                return listRG;
            }
        }
        //==== #001
        #region FileManager
        private string ConvertListToXml(List<RefShelves> ListRefShelfDetail)
        {
            try
            {
                if (ListRefShelfDetail != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<DS>");
                    foreach (RefShelves item in ListRefShelfDetail)
                    {
                        sb.Append("<RefShelves>");
                        sb.AppendFormat("<RefShelfID>{0}</RefShelfID>", item.RefShelfID);
                        sb.AppendFormat("<RefShelfCode>{0}</RefShelfCode>", item.RefShelfCode.ToUpper());
                        sb.AppendFormat("<RefShelfName>{0}</RefShelfName>", item.RefShelfName);
                        sb.AppendFormat("<Note>{0}</Note>", item.Note);
                        sb.AppendFormat("<StoreID>{0}</StoreID>", item.StoreID);
                        sb.Append("</RefShelves>");
                    }
                    sb.Append("</DS>");
                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string ConvertListToXml(List<RefShelfDetails> ListRefShelfDetail)
        {
            try
            {
                if (ListRefShelfDetail != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<DS>");
                    foreach (RefShelfDetails item in ListRefShelfDetail)
                    {
                        sb.Append("<RefShelfDetail>");
                        sb.AppendFormat("<RefShelfID>{0}</RefShelfID>", item.RefShelfID);
                        sb.AppendFormat("<RefShelfDetailID>{0}</RefShelfDetailID>", item.RefShelfDetailID);
                        sb.AppendFormat("<LocCode>{0}</LocCode>", item.LocCode.ToUpper());
                        sb.AppendFormat("<LocName>{0}</LocName>", item.LocName);
                        sb.AppendFormat("<Note>{0}</Note>", item.Note);
                        sb.AppendFormat("<CreatedDate>{0}</CreatedDate>", item.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        sb.Append("</RefShelfDetail>");
                    }
                    sb.Append("</DS>");
                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string ConvertListToXml(List<PatientMedicalFileStorage> ListPatientMedicalFileStorage)
        {
            try
            {
                if (ListPatientMedicalFileStorage != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<DS>");
                    foreach (PatientMedicalFileStorage item in ListPatientMedicalFileStorage)
                    {
                        sb.Append("<PatientMedicalFileStorage>");
                        sb.AppendFormat("<PatientMedicalFileStorageID>{0}</PatientMedicalFileStorageID>", item.PatientMedicalFileStorageID);
                        sb.AppendFormat("<PatientMedicalFileID>{0}</PatientMedicalFileID>", item.PatientMedicalFileID);
                        sb.AppendFormat("<RefShelfDetailID>{0}</RefShelfDetailID>", item.RefShelfDetailID);
                        sb.AppendFormat("<CreatedDate>{0}</CreatedDate>", item.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        sb.Append("</PatientMedicalFileStorage>");
                    }
                    sb.Append("</DS>");
                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string ConvertListToXml(List<PatientMedicalFileStorageCheckInCheckOut> ListPatientMedicalFileStorageCheckOut)
        {
            try
            {
                if (ListPatientMedicalFileStorageCheckOut != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<DS>");
                    foreach (PatientMedicalFileStorageCheckInCheckOut item in ListPatientMedicalFileStorageCheckOut)
                    {
                        sb.Append("<PatientMedicalFileStorageCheckInCheckOut>");
                        sb.AppendFormat("<PatientMedicalFileCheckoutID>{0}</PatientMedicalFileCheckoutID>", item.PatientMedicalFileCheckoutID);
                        sb.AppendFormat("<PatientMedicalFileStorageID>{0}</PatientMedicalFileStorageID>", item.PatientMedicalFileStorageID);
                        sb.AppendFormat("<CheckinStaffID>{0}</CheckinStaffID>", item.CheckinStaffID);
                        sb.AppendFormat("<CheckinDate>{0}</CheckinDate>", item.CheckinDate == null ? null : item.CheckinDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        sb.AppendFormat("<CheckoutStaffID>{0}</CheckoutStaffID>", item.CheckoutStaffID);
                        sb.AppendFormat("<CheckoutDate>{0}</CheckoutDate>", item.CheckoutDate == null ? null : item.CheckoutDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                        sb.AppendFormat("<StaffPersonID>{0}</StaffPersonID>", item.StaffPersonID);
                        sb.AppendFormat("<DeptID>{0}</DeptID>", item.DeptID);
                        sb.AppendFormat("<DeptLocID>{0}</DeptLocID>", item.DeptLocID);
                        sb.AppendFormat("<PtRegistrationID>{0}</PtRegistrationID>", item.PtRegistrationID == 0 ? null : (long?)item.PtRegistrationID);
                        sb.AppendFormat("<IsInPT>{0}</IsInPT>", item.IsInPT);
                        sb.AppendFormat("<FileCodeNumber>{0}</FileCodeNumber>", item.FileCodeNumber);
                        sb.Append("</PatientMedicalFileStorageCheckInCheckOut>");
                    }
                    sb.Append("</DS>");
                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<RefShelves> GetRefShelves(long RefShelfID, long StoreID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetRefShelves", cn);
                    cmd.AddParameter("@RefShelfID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefShelfID));
                    cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StoreID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    List<RefShelves> objList = null;
                    IDataReader reader = ExecuteReader(cmd);
                    objList = GetRefShelvesListFromReader(reader);
                    reader.Close();
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<RefShelves> UpdateRefShelves(long StaffID, List<RefShelves> ListRefShelves, long StoreID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateRefShelves", cn);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@ListRefShelves", SqlDbType.Xml, ConvertListToXml(ListRefShelves));
                    cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StoreID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    List<RefShelves> objList = null;
                    IDataReader reader = ExecuteReader(cmd);
                    objList = GetRefShelvesListFromReader(reader);
                    reader.Close();
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<RefShelfDetails> GetRefShelfDetails(long RefShelfID, string LocCode, long StoreID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetRefShelfDetails", cn);
                    cmd.AddParameter("@RefShelfID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefShelfID));
                    cmd.AddParameter("@LocCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(LocCode));
                    cmd.AddParameter("@StoreID", SqlDbType.VarChar, ConvertNullObjectToDBNull(StoreID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetRefShelfDetailListFromReader(reader);
                    reader.Close();
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override bool UpdateRefShelfDetails(long RefShelfID, List<RefShelfDetails> ListRefShelfDetail)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateRefShelfDetails", cn);
                    cmd.AddParameter("@RefShelfID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefShelfID));
                    cmd.AddParameter("@ListRefShelfDetail", SqlDbType.Xml, ConvertListToXml(ListRefShelfDetail));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    return ExecuteNonQuery(cmd) > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<PatientMedicalFileStorage> GetPatientMedicalFileStorage(long RefShelfDetailID, string FileCodeNumber, out long PatientMedicalFileID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetPatientMedicalFileStorage", cn);
                    cmd.AddParameter("@RefShelfDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefShelfDetailID));
                    cmd.AddParameter("@FileCodeNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(FileCodeNumber));
                    cmd.AddParameter("@PatientMedicalFileID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetPatientMedicalFileStorageListFromReader(reader);
                    reader.Close();
                    if (cmd.Parameters["@PatientMedicalFileID"].Value != DBNull.Value)
                        PatientMedicalFileID = (long)cmd.Parameters["@PatientMedicalFileID"].Value;
                    else
                        PatientMedicalFileID = 0;
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<PatientMedicalFileStorage> UpdatePatientMedicalFileStorage(long RefShelfDetailID, List<PatientMedicalFileStorage> ListPatientMedicalFileStorage)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdatePatientMedicalFileStorage", cn);
                    cmd.AddParameter("@RefShelfDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefShelfDetailID));
                    cmd.AddParameter("@ListPatientMedicalFileStorage", SqlDbType.Xml, ConvertListToXml(ListPatientMedicalFileStorage));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetPatientMedicalFileStorageListFromReader(reader);
                    reader.Close();
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<PatientMedicalFileStorageCheckInCheckOut> GetPatientMedicalFileStorageCheckInCheckOut(string FileCodeNumber)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetPatientMedicalFileStorageCheckInCheckOut", cn);
                    cmd.AddParameter("@FileCodeNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(FileCodeNumber));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    return GetPatientMedicalFileStorageCheckInCheckOutListFromReader(reader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<StaffPersons> GetAllStaffPersons()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetAllStaffPersons", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    return GetStaffPersonsListFromReader(reader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override bool UpdatePatientMedicalFileStorageCheckOut(long StaffID, List<PatientMedicalFileStorageCheckInCheckOut> ListPatientMedicalFileStorageCheckOut, out long MedicalFileStorageCheckID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdatePatientMedicalFileStorageCheckInCheckOut", cn);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@ListPatientMedicalFileStorageCheckOut", SqlDbType.Xml, ConvertListToXml(ListPatientMedicalFileStorageCheckOut));
                    cmd.AddParameter("@MedicalFileStorageCheckID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();

                    var mReturnInt = (int)ExecuteNonQuery(cmd);

                    if (cmd.Parameters["@MedicalFileStorageCheckID"].Value != DBNull.Value)
                        MedicalFileStorageCheckID = Convert.ToInt32(cmd.Parameters["@MedicalFileStorageCheckID"].Value);
                    else
                        MedicalFileStorageCheckID = 1;

                    return mReturnInt > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileFromRegistration(long DeptID, long LocationID, DateTime StartDate, DateTime EndDate, bool IsBorrowed, bool IsStored)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetMedicalFileFromRegistration", cn);
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));
                    cmd.AddParameter("@LocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(LocationID));
                    cmd.AddParameter("@StartDate", SqlDbType.DateTime, StartDate);
                    cmd.AddParameter("@EndDate", SqlDbType.DateTime, EndDate);
                    cmd.AddParameter("@IsBorrowed", SqlDbType.Bit, IsBorrowed);
                    cmd.AddParameter("@IsStored", SqlDbType.Bit, IsStored);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetPatientMedicalFileStorageCheckInCheckOutListFromReader(reader);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override string GetXMLPatientMedicalFileStorages(List<PatientMedicalFileStorage> ListPatientMedicalFileStorage)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    var objList = GetXMLPatientMedicalFileStorageFromList(ListPatientMedicalFileStorage);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<PatientMedicalFileStorageCheckInCheckOut> SearchMedicalFilesHistory(DateTime? StartDate, DateTime? EndDate, string FileCodeNumber, long? StaffID, long? ReceiveStaffID, int Status, int PageSize, int PageIndex, out int TotalRow)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spSearchMedicalFilesHistory", cn);
                    cmd.AddParameter("@StartDate", SqlDbType.DateTime, StartDate);
                    cmd.AddParameter("@EndDate", SqlDbType.DateTime, EndDate);
                    cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                    cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                    cmd.AddParameter("@FileCodeNumber", SqlDbType.VarChar, FileCodeNumber);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                    cmd.AddParameter("@ReceiveStaffID", SqlDbType.BigInt, ReceiveStaffID);
                    cmd.AddParameter("@Status", SqlDbType.Int, Status);
                    cmd.AddParameter("@TotalRow", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetPatientMedicalFileStorageCheckInCheckOutListFromReader(reader);
                    if (cmd.Parameters["@TotalRow"].Value != DBNull.Value)
                        TotalRow = Convert.ToInt32(cmd.Parameters["@TotalRow"].Value);
                    else
                        TotalRow = 1;
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override string GetRegistrationXMLFromMedicalFileList(List<PatientMedicalFileStorageCheckInCheckOut> ListItem)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    var objList = GetRegistrationXMLFromMedicalFileListItem(ListItem);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileStorageCheckOutHistory(DateTime? StartDate, DateTime? EndDate, long? StaffID, long? ReceiveStaffID, int PageSize, int PageIndex, out int TotalRow)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spMedicalFileStorageCheckOutHistory", cn);
                    cmd.AddParameter("@StartDate", SqlDbType.DateTime, StartDate);
                    cmd.AddParameter("@EndDate", SqlDbType.DateTime, EndDate);
                    cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                    cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);
                    cmd.AddParameter("@ReceiveStaffID", SqlDbType.BigInt, ReceiveStaffID);
                    cmd.AddParameter("@TotalRow", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetPatientMedicalFileStorageCheckInCheckOutListFromReader(reader);
                    if (cmd.Parameters["@TotalRow"].Value != DBNull.Value)
                        TotalRow = Convert.ToInt32(cmd.Parameters["@TotalRow"].Value);
                    else
                        TotalRow = 1;
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileDetails(string FileCodeNumber, long? RefShelfDetailID, int PageSize, int PageIndex, out int TotalRow)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetMedicalFileDetails", cn);
                    cmd.AddParameter("@FileCodeNumber", SqlDbType.VarChar, FileCodeNumber);
                    cmd.AddParameter("@RefShelfDetailID", SqlDbType.BigInt, RefShelfDetailID);
                    cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                    cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                    cmd.AddParameter("@TotalRow", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetPatientMedicalFileStorageCheckInCheckOutListFromReader(reader);
                    if (cmd.Parameters["@TotalRow"].Value != DBNull.Value)
                        TotalRow = Convert.ToInt32(cmd.Parameters["@TotalRow"].Value);
                    else
                        TotalRow = 1;
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        //==== #001
    }
}

