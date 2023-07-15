/*
20161223 #001 CMN: Add file manager
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using DataEntities;
using eHCMS.Configurations;
using eHCMS.DAL;
using eHCMS.Caching;
using System.Collections.ObjectModel;
/*
 * 20220812 #002 QTD: Tìm DS hồ sơ màn hình đặt hồ sơ vào kệ
 * 20220813 #003 QTD: Quản lý Dãy kệ ngăn, Quản lý mượn trả hồ sơ
 * 20221116 #004 BLQ: Thêm chức năng lịch làm việc ngoài giờ
 */
namespace aEMR.DataAccessLayer.Providers
{
    public class ClinicManagementProvider: DataProviderBase
    {
        private static ClinicManagementProvider _instance=null;
        public static ClinicManagementProvider instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClinicManagementProvider();
                }
                return _instance;
            }
        }
        public ClinicManagementProvider()
        {
            this.ConnectionString =Globals.Settings.Common.ConnectionString;
        }
        protected virtual ConsultationRoomStaffAllocations GetConsultationRoomStaffAllocationsObjFromReader(IDataReader reader)
        {
            ConsultationRoomStaffAllocations p = new ConsultationRoomStaffAllocations();
            try
            {
                p.Staff = new Staff();
                p.ConsultationTimeSegments = new ConsultationTimeSegments();
                if (reader.HasColumn("ConsultationTimeSegmentID") && reader["ConsultationTimeSegmentID"] != DBNull.Value)
                {
                    p.ConsultationTimeSegmentID = (long)reader["ConsultationTimeSegmentID"];
                    p.ConsultationTimeSegments.ConsultationTimeSegmentID = (long)reader["ConsultationTimeSegmentID"];
                }

                if (reader.HasColumn("SegmentName") && reader["SegmentName"] != DBNull.Value)
                {
                    p.ConsultationTimeSegments.SegmentName = reader["SegmentName"].ToString();
                }
                if (reader.HasColumn("SegmentStartDate") && reader["SegmentStartDate"] != DBNull.Value)
                {
                    p.ConsultationTimeSegments.StartTime = Convert.ToDateTime(reader["SegmentStartDate"]);
                }
                if (reader.HasColumn("SegmentEndDate") && reader["SegmentEndDate"] != DBNull.Value)
                {
                    p.ConsultationTimeSegments.EndTime = Convert.ToDateTime(reader["SegmentEndDate"]);
                }
                if (reader.HasColumn("ConsultationRoomStaffAllocID") && reader["ConsultationRoomStaffAllocID"] != DBNull.Value)
                {
                    p.ConsultationRoomStaffAllocID = (long)reader["ConsultationRoomStaffAllocID"];
                }
                if (reader.HasColumn("DeptLocationID") && reader["DeptLocationID"] != DBNull.Value)
                {
                    p.DeptLocationID = (long)reader["DeptLocationID"];
                    p.DeptLocation = new DeptLocation { DeptLocationID = (long)reader["DeptLocationID"] };
                    if (reader.HasColumn("LID") && reader["LID"] != DBNull.Value)
                    {
                        p.DeptLocation.Location = new Location { LID = (long)reader["LID"] };
                        if (reader.HasColumn("LocationName") && reader["LocationName"] != DBNull.Value)
                        {
                            p.DeptLocation.Location.LocationName = Convert.ToString(reader["LocationName"]);
                        }
                    }
                }
                if (reader.HasColumn("StaffCatgID") && reader["StaffCatgID"] != DBNull.Value)
                {
                    p.StaffCatgID = Convert.ToInt32(reader["StaffCatgID"]);
                    p.Staff.StaffCatgID = Convert.ToInt32(reader["StaffCatgID"]);
                }
                if (reader.HasColumn("StaffID") && reader["StaffID"] != DBNull.Value)
                {
                    p.StaffID = (long)reader["StaffID"];
                    p.Staff.StaffID = (long)reader["StaffID"];
                }

                if (reader.HasColumn("IsActive") && reader["IsActive"] != DBNull.Value)
                {
                    p.IsActive = (bool)reader["IsActive"];
                }

                if (reader.HasColumn("AllocationDate") && reader["AllocationDate"] != DBNull.Value)
                {
                    p.AllocationDate = (DateTime)reader["AllocationDate"];
                    if (p.AllocationDate > DateTime.Now)
                    {
                        p.isEdit = true;
                    }
                }


                if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
                {
                    p.Staff.FullName = reader["FullName"].ToString();
                }

                p.Staff.RefStaffCategory = new RefStaffCategory();
                if (reader.HasColumn("V_StaffCatType") && reader["V_StaffCatType"] != DBNull.Value)
                {
                    p.Staff.RefStaffCategory.V_StaffCatType = (long)reader["V_StaffCatType"];
                }
                if (reader.HasColumn("StaffCatgDescription") && reader["StaffCatgDescription"] != DBNull.Value)
                {
                    p.Staff.RefStaffCategory.StaffCatgDescription = reader["StaffCatgDescription"].ToString();
                }

            }
            catch
            {

            }
            if (reader.HasColumn("ConsultationRoomStaffAllocationServiceListID") && reader["ConsultationRoomStaffAllocationServiceListID"] != DBNull.Value)
            {
                p.ConsultationRoomStaffAllocationServiceListID = Convert.ToInt64(reader["ConsultationRoomStaffAllocationServiceListID"]);
            }
            return p;
        }
        protected virtual List<ConsultationRoomStaffAllocations> GetConsultationRoomStaffAllocationsCollectionFromReader(IDataReader reader)
        {
            List<ConsultationRoomStaffAllocations> lst = new List<ConsultationRoomStaffAllocations>();
            while (reader.Read())
            {
                lst.Add(GetConsultationRoomStaffAllocationsObjFromReader(reader));
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    if (reader.HasColumn("ConsultationRoomStaffAllocID") && reader["ConsultationRoomStaffAllocID"] != DBNull.Value)
                    {
                        var ConsultationRoomStaffAllocID = (long)reader["ConsultationRoomStaffAllocID"];
                        if (lst.First(x => x.ConsultationRoomStaffAllocID == ConsultationRoomStaffAllocID).ServiceItemCollection == null)
                        {
                            lst.First(x => x.ConsultationRoomStaffAllocID == ConsultationRoomStaffAllocID).ServiceItemCollection = new List<RefMedicalServiceItem>();
                        }
                        if (reader.HasColumn("MedServiceID") && reader["MedServiceID"] != DBNull.Value)
                        {
                            lst.First(x => x.ConsultationRoomStaffAllocID == ConsultationRoomStaffAllocID).ServiceItemCollection.Add(new RefMedicalServiceItem { MedServiceID = (long)reader["MedServiceID"] });
                        }
                    }
                }
            }
            reader.Close();
            return lst;
        }
        public  bool InsertConsultationTimeSegments(string SegmentName
                                                                , string SegmentDescription
                                                                , DateTime StartTime
                                                                , DateTime EndTime
                                                                , DateTime? StartTime2
                                                                , DateTime? EndTime2
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
                cmd.AddParameter("@StartTime2", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartTime2));
                cmd.AddParameter("@EndTime2", SqlDbType.DateTime, ConvertNullObjectToDBNull(EndTime2));
                cmd.AddParameter("@IsActive", SqlDbType.NVarChar, ConvertNullObjectToDBNull(IsActive));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
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
        public void EditStaffConsultationTimeSegments(string SegmentXmlContent, long DeptLocationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spEditStaffConsultationTimeSegments", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@SegmentXmlContent", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SegmentXmlContent));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cn.Open();
                cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        public  bool UpdateConsultationTimeSegments(ObservableCollection<ConsultationTimeSegments> consultationTimeSegments)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationTimeSegmentsUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DataXML", SqlDbType.Xml, ConvertListToXml_TimeSegment(consultationTimeSegments.ToList()));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
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

        public  bool DeleteConsultationTimeSegments(long ConsultationTimeSegmentID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationTimeSegmentsDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ConsultationTimeSegmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultationTimeSegmentID));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
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

        public  List<ConsultationTimeSegments> GetAllConsultationTimeSegmentsCache()
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

        public  List<ConsultationTimeSegments> GetAllConsultationTimeSegments()
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
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;

            }
        }

        public  List<PCLTimeSegment> GetAllPCLTimeSegments()
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
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return objList;

            }
        }

        public  List<ConsultationTimeSegments> ConsultationTimeSegments_ByDeptLocationID(long DeptLocationID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }


        public  bool InsertConsultationRoomTarget(ConsultationRoomTarget consultationRoomTarget)
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
                CleanUpConnectionAndCommand(cn, cmd);
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
        public  bool InsertConsultationRoomTargetXML(List<ConsultationRoomTarget> lstCRSA)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomTargetInsertXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, CRTA_ConvertListToXml(lstCRSA));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
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

        public  bool UpdateConsultationRoomTargetXML(IList<ConsultationRoomTarget> lstCRSA)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomTargetUpdateXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, CRTA_ConvertListToXml(lstCRSA));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
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
        public  bool DeleteConsultationRoomTarget(long ConsultationRoomTargetID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomTargetDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ConsultationRoomTargetID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultationRoomTargetID));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
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

        public  List<ConsultationRoomTarget> GetConsultationRoomTargetByDeptID(long DeptLocationID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }

        public  List<ConsultationRoomTarget> GetConsultationRoomTargetTimeSegment(long DeptLocationID, long ConsultationTimeSegmentID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }

        public  void GetAllConsultationRoomTargetCache()
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

        public  List<ConsultationRoomTarget> GetConsultationRoomTargetTSegment(long DeptLocationID, bool IsHis)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }

        public  bool InsertConsultationRoomStaffAllocations(long DeptLocationID
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
                CleanUpConnectionAndCommand(cn, cmd);
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
        public  bool InsertConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomStaffAllocationsInsertXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DataXML", SqlDbType.Xml, CRSA_ConvertListToXml(lstCRSA));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
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

        public  bool UpdateConsultationRoomStaffAllocations(long ConsultationRoomStaffAllocID, bool IsActive)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomStaffAllocationsUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ConsultationRoomStaffAllocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultationRoomStaffAllocID));
                cmd.AddParameter("@IsActive", SqlDbType.BigInt, ConvertNullObjectToDBNull(IsActive));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
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
        public  bool UpdateConsultationRoomStaffAllocationsXML(IList<ConsultationRoomStaffAllocations> lstCRSA)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomStaffAllocationsUpdateXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, CRSADelete_ConvertListToXml(lstCRSA));

                cn.Open();
                int val = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
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
        public  bool DeleteConsultationRoomStaffAllocations(long DeptLocationID
                                                                , long ConsultationTimeSegmentID
                                                                , DateTime AllocationDate)
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
                CleanUpConnectionAndCommand(cn, cmd);
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
        public  List<ConsultationRoomStaffAllocations> GetConsultationRoomStaffAllocations(long DeptLocationID, long ConsultationTimeSegmentID)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }
        public List<ConsultationRoomStaffAllocations> GetConsultationRoomStaffAllocations_ForXML(IList<DeptLocation> lstDeptLocation, long ConsultationTimeSegmentID)
        {
            List<ConsultationRoomStaffAllocations> listRG = new List<ConsultationRoomStaffAllocations>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spConsultationRoomStaffAllocationsGetAll_ForXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@XML", SqlDbType.Xml, ConvertListToXml(lstDeptLocation));
                cmd.AddParameter("@ConsultationTimeSegmentID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ConsultationTimeSegmentID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRG = GetConsultationRoomStaffAllocationsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }
        private string ConvertListToXml(IList<DeptLocation> lstDeptLocation)
        {
            try
            {
                if (lstDeptLocation != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<DS>");
                    foreach (DeptLocation item in lstDeptLocation)
                    {
                        sb.AppendFormat("<DeptLocationID>{0}</DeptLocationID>", item.DeptLocationID);
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
        public List<ConsultationRoomStaffAllocations> GetStaffConsultationTimeSegmentByDate(long DeptLocationID, DateTime FromDate, DateTime ToDate)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                List<ConsultationRoomStaffAllocations> mItemCollection = new List<ConsultationRoomStaffAllocations>();
                SqlCommand cmd = new SqlCommand("spGetStaffConsultationTimeSegmentByDate", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                mItemCollection = GetConsultationRoomStaffAllocationsCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return mItemCollection;
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
                        //▼==== #003
                        sb.AppendFormat("<RefRowID>{0}</RefRowID>", item.RefRowID);
                        //▲==== #003
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
                        sb.AppendFormat("<ExpiryTime>{0}</ExpiryTime>", item.ExpiryTime);
                        sb.AppendFormat("<OutPtTreatmentProgramID>{0}</OutPtTreatmentProgramID>", item.OutPtTreatmentProgramID);
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
        public  List<RefShelves> GetRefShelves(long RefShelfID, long RefRowID, string RefShelfName)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetRefShelves", cn);
                    cmd.AddParameter("@RefShelfID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefShelfID));
                    //▼==== #003
                    //cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StoreID));
                    cmd.AddParameter("@RefRowID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefRowID));
                    cmd.AddParameter("@RefShelfName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RefShelfName));
                    //▲==== #003
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    List<RefShelves> objList = null;
                    IDataReader reader = ExecuteReader(cmd);
                    objList = GetRefShelvesListFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  List<RefShelves> UpdateRefShelves(long StaffID, List<RefShelves> ListRefShelves, List<RefShelves> ListRefShelvesDeleted, long RefRowID, bool IsPopup)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    //▼====003
                    SqlCommand cmd = new SqlCommand();
                    if(IsPopup)
                    {
                        cmd = new SqlCommand("spUpdateRefShelves_V2", cn);
                        cmd.AddParameter("@ListRefShelvesDeleted", SqlDbType.Xml, ConvertListToXml(ListRefShelvesDeleted));
                    }  
                    else
                    {
                        cmd = new SqlCommand("spUpdateRefShelves", cn);
                    }
                    //▲====003
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@ListRefShelves", SqlDbType.Xml, ConvertListToXml(ListRefShelves));
                    //▼====003
                    //cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StoreID));
                    cmd.AddParameter("@RefRowID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefRowID));
                    //▲====003
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    List<RefShelves> objList = null;
                    IDataReader reader = ExecuteReader(cmd);
                    objList = GetRefShelvesListFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  List<RefShelfDetails> GetRefShelfDetails(long RefShelfID, string LocCode, long RefShelfDetailID, string LocName)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetRefShelfDetails", cn);
                    cmd.AddParameter("@RefShelfID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefShelfID));
                    cmd.AddParameter("@LocCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(LocCode));
                    cmd.AddParameter("@RefShelfDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefShelfDetailID));
                    cmd.AddParameter("@LocName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(LocName));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetRefShelfDetailListFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  bool UpdateRefShelfDetails(long RefShelfID, long StaffID, List<RefShelfDetails> ListRefShelfDetail, List<RefShelfDetails> ListRefShelfDetailDeleted, bool IsPopup)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    //▼====003
                    SqlCommand cmd = new SqlCommand();
                    if (IsPopup)
                    {
                        cmd = new SqlCommand("spUpdateRefShelfDetails_V2", cn);
                        cmd.AddParameter("@ListRefShelfDetailDeleted", SqlDbType.Xml, ConvertListToXml(ListRefShelfDetailDeleted));
                    }
                    else
                    {
                        cmd = new SqlCommand("spUpdateRefShelfDetails", cn);
                    }
                    //▲====003
                    cmd.AddParameter("@RefShelfID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefShelfID));
                    cmd.AddParameter("@ListRefShelfDetail", SqlDbType.Xml, ConvertListToXml(ListRefShelfDetail));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    int val = ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    return val > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  List<PatientMedicalFileStorage> GetPatientMedicalFileStorage(long RefShelfDetailID, string FileCodeNumber, out long PatientMedicalFileID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  List<PatientMedicalFileStorage> UpdatePatientMedicalFileStorage(long RefShelfDetailID, List<PatientMedicalFileStorage> ListPatientMedicalFileStorage, long StaffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdatePatientMedicalFileStorage", cn);
                    cmd.AddParameter("@RefShelfDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefShelfDetailID));
                    cmd.AddParameter("@ListPatientMedicalFileStorage", SqlDbType.Xml, ConvertListToXml(ListPatientMedicalFileStorage));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetPatientMedicalFileStorageListFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  List<PatientMedicalFileStorageCheckInCheckOut> GetPatientMedicalFileStorageCheckInCheckOut(string FileCodeNumber)
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
                    var objList = GetPatientMedicalFileStorageCheckInCheckOutListFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  List<StaffPersons> GetAllStaffPersons()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetAllStaffPersons", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetStaffPersonsListFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  bool UpdatePatientMedicalFileStorageCheckOut(long StaffID, List<PatientMedicalFileStorageCheckInCheckOut> ListPatientMedicalFileStorageCheckOut, out long MedicalFileStorageCheckID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                    return mReturnInt > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileFromRegistration(long DeptID, long LocationID, DateTime StartDate, DateTime EndDate, bool IsBorrowed, bool IsStored)
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
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  string GetXMLPatientMedicalFileStorages(List<PatientMedicalFileStorage> ListPatientMedicalFileStorage)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    var objList = GetXMLPatientMedicalFileStorageFromList(ListPatientMedicalFileStorage);
                    cn.Dispose();
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  List<PatientMedicalFileStorageCheckInCheckOut> SearchMedicalFilesHistory(DateTime? StartDate, DateTime? EndDate, string FileCodeNumber, long? StaffID, long? ReceiveStaffID, int Status, int PageSize, int PageIndex, out int TotalRow)
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
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  string GetRegistrationXMLFromMedicalFileList(List<PatientMedicalFileStorageCheckInCheckOut> ListItem)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    var objList = GetRegistrationXMLFromMedicalFileListItem(ListItem);
                    cn.Dispose();
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileStorageCheckOutHistory(DateTime? StartDate, DateTime? EndDate, long? StaffID, long? ReceiveStaffID, int PageSize, int PageIndex, out int TotalRow)
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
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  List<PatientMedicalFileStorageCheckInCheckOut> GetMedicalFileDetails(string FileCodeNumber, long? RefShelfDetailID, int PageSize, int PageIndex, out int TotalRow)
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
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string ConvertListToXml_TimeSegment(IList<ConsultationTimeSegments> consultationTimeSegments)
        {
            try
            {
                if (consultationTimeSegments != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<DS>");
                    foreach (ConsultationTimeSegments item in consultationTimeSegments)
                    {
                        sb.Append("<ConsultationTimeSegment>");
                        sb.AppendFormat("<ConsultationTimeSegmentID>{0}</ConsultationTimeSegmentID>", item.ConsultationTimeSegmentID);
                        sb.AppendFormat("<SegmentName>{0}</SegmentName>", item.SegmentName);
                        sb.AppendFormat("<SegmentDescription>{0}</SegmentDescription>", item.SegmentDescription);
                        sb.AppendFormat("<StartTime>{0}</StartTime>", item.StartTime.ToString("yyyy-MM-dd HH:mm:ss:mmm"));
                        sb.AppendFormat("<EndTime>{0}</EndTime>", item.EndTime.ToString("yyyy-MM-dd HH:mm:ss:mmm"));
                        sb.AppendFormat("<StartTime2>{0}</StartTime2>", item.StartTime2 == null ? null : item.StartTime2.Value.ToString("yyyy-MM-dd HH:mm:ss:mmm"));
                        sb.AppendFormat("<EndTime2>{0}</EndTime2>", item.EndTime2 == null ? null : item.EndTime2.Value.ToString("yyyy-MM-dd HH:mm:ss:mmm"));
                        sb.Append("</ConsultationTimeSegment>");
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
        #endregion
        public IList<PatientRegistration> SearchRegistrationsForOutMedicalFileManagement(SeachPtRegistrationCriteria aSeachPtRegistrationCriteria, int ViewCase)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spSearchRegistrationsForOutMedicalFileManagement", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = int.MaxValue;
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.ToDate));
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptID));
                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.DeptLocationID));
                    cmd.AddParameter("@IsReported", SqlDbType.Bit, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.IsReported));
                    cmd.AddParameter("@V_TreatmentType", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.V_TreatmentType));

                    cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.FullName));
                    cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PatientCode));
                    cmd.AddParameter("@PtRegistrationCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PtRegistrationCode));
                    cmd.AddParameter("@PatientFindBy", SqlDbType.BigInt, ConvertNullObjectToDBNull(aSeachPtRegistrationCriteria.PatientFindBy));
                    //▼===== #011
                    cmd.AddParameter("@ViewCase", SqlDbType.Int, ConvertNullObjectToDBNull(ViewCase));
                    //▲===== #011

                    cn.Open();
                    var mReader = cmd.ExecuteReader();
                    List<PatientRegistration> mPatientRegistrationCollection = GetPatientRegistrationCollectionFromReader(mReader);
                    mReader.Close();

                    CleanUpConnectionAndCommand(cn, cmd);
                    return mPatientRegistrationCollection;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //▼===== #002
        public List<PatientMedicalFileStorage> SearchPatientMedicalFileManager(long V_MedicalFileType, string FileCodeNumber, string PatientName, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spSearchPatientMedicalFileManage", cn);
                    cmd.AddParameter("@V_MedicalFileType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_MedicalFileType));
                    cmd.AddParameter("@FileCodeNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(FileCodeNumber));
                    cmd.AddParameter("@PatientName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PatientName));
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetPatientMedicalFileStorageListFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<PatientMedicalFileStorage> PatientMedicalFileStorage_InsertXML(long RefShelfDetailID, List<PatientMedicalFileStorage> ListPatientMedicalFileStorage, long StaffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spPatientMedicalFileStorage_InsertXML", cn);
                    cmd.AddParameter("@RefShelfDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefShelfDetailID));
                    cmd.AddParameter("@ListPatientMedicalFileStorage", SqlDbType.Xml, ConvertListToXml(ListPatientMedicalFileStorage));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetPatientMedicalFileStorageListFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //▲===== #002
        //▼===== #003
        private string ConvertListToXml(List<RefRows> ListRefRow)
        {
            try
            {
                if (ListRefRow != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<DS>");
                    foreach (RefRows item in ListRefRow)
                    {
                        sb.Append("<RefRows>");
                        sb.AppendFormat("<RefRowID>{0}</RefRowID>", item.RefRowID);
                        sb.AppendFormat("<RefRowCode>{0}</RefRowCode>", item.RefRowCode.ToUpper());
                        sb.AppendFormat("<RefRowName>{0}</RefRowName>", item.RefRowName);
                        sb.AppendFormat("<Note>{0}</Note>", item.Note);
                        sb.AppendFormat("<StoreID>{0}</StoreID>", item.StoreID);
                        sb.Append("</RefRows>");
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
        public List<RefRows> GetRefRows(long StoreID, string RefRowName, long RefRowID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetRefRows", cn);
                    cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StoreID));
                    cmd.AddParameter("@RefRowName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RefRowName));
                    cmd.AddParameter("@RefRowID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefRowID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    List<RefRows> objList = null;
                    IDataReader reader = ExecuteReader(cmd);
                    objList = GetRefRowsListFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<RefRows> UpdateRefRows(long StaffID, List<RefRows> ListRefRows, List<RefRows> ListRefRowDeleted, long StoreID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateRefRows", cn);
                    cmd.AddParameter("@ListRefRows", SqlDbType.Xml, ConvertListToXml(ListRefRows));
                    cmd.AddParameter("@ListRefRowDeleted", SqlDbType.Xml, ConvertListToXml(ListRefRowDeleted));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StoreID));
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    List<RefRows> objList = null;
                    IDataReader reader = ExecuteReader(cmd);
                    objList = GetRefRowsListFromReader(reader);
                    reader.Close();
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<PatientMedicalFileStorageCheckInCheckOut> SearchMedicalFileDetails(long? StoreID, long? RefRowID, long? RefShelfID, long? RefShelfDetailID, long V_MedicalFileType, string FileCodeNumber,
            string PatientCode, string PatientName, long V_MedicalFileStatus, int PageSize, int PageIndex, out int TotalRow)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetMedicalFileDetails", cn);
                    cmd.AddParameter("@StoreID", SqlDbType.BigInt, StoreID);
                    cmd.AddParameter("@RefRowID", SqlDbType.BigInt, RefRowID);
                    cmd.AddParameter("@RefShelfID", SqlDbType.BigInt, RefShelfID);
                    cmd.AddParameter("@RefShelfDetailID", SqlDbType.BigInt, RefShelfDetailID);
                    cmd.AddParameter("@V_MedicalFileType", SqlDbType.BigInt, V_MedicalFileType);
                    cmd.AddParameter("@FileCodeNumber", SqlDbType.VarChar, FileCodeNumber);
                    cmd.AddParameter("@PatientCode", SqlDbType.VarChar, PatientCode);
                    cmd.AddParameter("@PatientName", SqlDbType.NVarChar, PatientName);
                    cmd.AddParameter("@V_MedicalFileStatus", SqlDbType.BigInt, V_MedicalFileStatus);
                    cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                    cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                    cmd.AddParameter("@TotalRow", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetPatientMedicalFileStorageCheckInCheckOutListFromReader(reader);
                    reader.Close();
                    if (cmd.Parameters["@TotalRow"].Value != DBNull.Value)
                        TotalRow = Convert.ToInt32(cmd.Parameters["@TotalRow"].Value);
                    else
                        TotalRow = 1;
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PatientMedicalFileStorageCheckInCheckOut> GetPatientMedicalFileStorageCheckInCheckOut_V2(long StoreID, long RefRowID, long RefShelfID, long RefShelfDetailID, DateTime FromDate, DateTime ToDate,
            string PatientCode, string PatientName, string FileCodeNumber, bool IsCheckIn, int PageSize, int PageIndex, out int TotalRow)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetPatientMedicalFileStorageCheckInCheckOut_V2", cn);
                    cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StoreID));
                    cmd.AddParameter("@RefRowID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefRowID));
                    cmd.AddParameter("@RefShelfID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefShelfID));
                    cmd.AddParameter("@RefShelfDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RefShelfDetailID));
                    cmd.AddParameter("@FileCodeNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(FileCodeNumber));
                    cmd.AddParameter("@PatientCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(PatientCode));
                    cmd.AddParameter("@PatientName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PatientName));
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                    cmd.AddParameter("@IsCheckIn", SqlDbType.Bit, ConvertNullObjectToDBNull(IsCheckIn));
                    cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                    cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                    cmd.AddParameter("@TotalRow", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    IDataReader reader = ExecuteReader(cmd);
                    var objList = GetPatientMedicalFileStorageCheckInCheckOutListFromReader(reader);
                    reader.Close();
                    if (cmd.Parameters["@TotalRow"].Value != DBNull.Value)
                        TotalRow = Convert.ToInt32(cmd.Parameters["@TotalRow"].Value);
                    else
                        TotalRow = 1;
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdatePatientMedicalFileStorageCheckOut_V2(long StaffID, long StaffIDCheckOut, long StaffPersonID, int BorrowingDay,
            string Notes, long V_ReasonType, bool IsCheckIn, List<PatientMedicalFileStorageCheckInCheckOut> ListPatientMedicalFileStorageCheckOut, out long MedicalFileStorageCheckID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdatePatientMedicalFileStorageCheckInCheckOut_V2", cn);
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@StaffIDCheckOut", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffIDCheckOut));
                    cmd.AddParameter("@StaffPersonID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffPersonID));
                    cmd.AddParameter("@BorrowingDay", SqlDbType.Int, ConvertNullObjectToDBNull(BorrowingDay));
                    cmd.AddParameter("@Notes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Notes));
                    cmd.AddParameter("@V_ReasonType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_ReasonType));
                    cmd.AddParameter("@IsCheckIn", SqlDbType.Bit, ConvertNullObjectToDBNull(IsCheckIn));
                    cmd.AddParameter("@ListPatientMedicalFileStorageCheckOut", SqlDbType.Xml, ConvertListToXml(ListPatientMedicalFileStorageCheckOut));
                    cmd.AddParameter("@MedicalFileStorageCheckID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();

                    var mReturnInt = (int)ExecuteNonQuery(cmd);

                    if (cmd.Parameters["@MedicalFileStorageCheckID"].Value != DBNull.Value)
                        MedicalFileStorageCheckID = Convert.ToInt32(cmd.Parameters["@MedicalFileStorageCheckID"].Value);
                    else
                        MedicalFileStorageCheckID = 1;
                    CleanUpConnectionAndCommand(cn, cmd);
                    return mReturnInt > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //▲===== #003 
        //▼===== #004
        public OvertimeWorkingWeek GetOvertimeWorkingWeekByDate(int Week, DateTime FromDate, DateTime ToDate)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetOvertimeWorkingWeekByDate", cn);
                    cmd.AddParameter("@Week", SqlDbType.Int, ConvertNullObjectToDBNull(Week));
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                  
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    OvertimeWorkingWeek obj = null;
                    IDataReader reader = ExecuteReader(cmd);
                    if (reader != null && reader.Read())
                    {
                        obj = GetOvertimeWorkingWeekFromReader(reader);
                    }
                    reader.Close();
                    
                    CleanUpConnectionAndCommand(cn, cmd);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<OvertimeWorkingSchedule> GetOvertimeWorkingScheduleByDate(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetOvertimeWorkingScheduleByDate", cn);
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(ToDate));
                  
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();

                    IDataReader reader = ExecuteReader(cmd);
                    List<OvertimeWorkingSchedule> objList = null;
                    if (reader != null)
                    {
                        objList = GetOvertimeWorkingScheduleCollectionFromReader(reader);
                    }
           
                    reader.Close();
                    
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<DeptLocation> GetLocationForOvertimeWorkingWeek()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetLocationForOvertimeWorkingWeek", cn);
                 
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();

                    IDataReader reader = ExecuteReader(cmd);
                    List<DeptLocation> objList = null;
                    if (reader != null)
                    {
                        objList = GetDeptLocationCollectionFromReader(reader);
                    }
                    reader.Close();
                    
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Staff> GetDoctorForOvertimeWorkingWeek()
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spGetDoctorForOvertimeWorkingWeek", cn);
                 
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();

                    IDataReader reader = ExecuteReader(cmd);
                    List<Staff> objList = null;
                    if (reader != null)
                    {
                        objList = GetStaffFullNameCollectionFromReader(reader);
                    }
                    reader.Close();
                    
                    CleanUpConnectionAndCommand(cn, cmd);
                    return objList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool SaveOvertimeWorkingSchedule(OvertimeWorkingWeek OTWObj, OvertimeWorkingSchedule OTSObj, long StaffID, DateTime DateUpdate)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spSaveOvertimeWorkingSchedule", cn);
                    cmd.AddParameter("@OvertimeWorkingWeekID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OTWObj.OvertimeWorkingWeekID));
                    cmd.AddParameter("@Week", SqlDbType.Int, ConvertNullObjectToDBNull(OTWObj.Week));
                    cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(OTWObj.FromDate));
                    cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(OTWObj.ToDate));
                    cmd.AddParameter("@OvertimeWorkingNotes", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OTWObj.OvertimeWorkingNotes));
                    cmd.AddParameter("@V_OvertimeWorkingWeekStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(OTWObj.V_OvertimeWorkingWeekStatus));
                    if(OTSObj != null)
                    {
                        cmd.AddParameter("@OvertimeWorkingScheduleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OTSObj.OvertimeWorkingScheduleID));
                        cmd.AddParameter("@DoctorStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OTSObj.DoctorStaffID));
                        cmd.AddParameter("@WorkDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(OTSObj.WorkDate));
                        cmd.AddParameter("@FromTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(OTSObj.FromTime));
                        cmd.AddParameter("@ToTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(OTSObj.ToTime));
                        cmd.AddParameter("@DeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OTSObj.DeptLocID));
                    }
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@DateUpdate", SqlDbType.DateTime, ConvertNullObjectToDBNull(DateUpdate));

                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();

                    var mReturnInt = (int)ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    return mReturnInt > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool DeleteOvertimeWorkingSchedule(long OvertimeWorkingScheduleID, long StaffID, DateTime DateUpdate)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeleteOvertimeWorkingSchedule", cn);
                    cmd.AddParameter("@OvertimeWorkingScheduleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OvertimeWorkingScheduleID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@DateUpdate", SqlDbType.DateTime, ConvertNullObjectToDBNull(DateUpdate));

                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();

                    var mReturnInt = (int)ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    return mReturnInt > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateOvertimeWorkingWeekStatus(OvertimeWorkingWeek OTWObj, long StaffID, DateTime DateUpdate)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUpdateOvertimeWorkingWeekStatus", cn);
                    cmd.AddParameter("@OvertimeWorkingWeekID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OTWObj.OvertimeWorkingWeekID));
                    cmd.AddParameter("@V_OvertimeWorkingWeekStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(OTWObj.V_OvertimeWorkingWeekStatus));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@DateUpdate", SqlDbType.DateTime, ConvertNullObjectToDBNull(DateUpdate));

                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();

                    var mReturnInt = (int)ExecuteNonQuery(cmd);
                    CleanUpConnectionAndCommand(cn, cmd);
                    return mReturnInt > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //▲===== #004
    }
}
