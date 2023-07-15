using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using eHCMSLanguage;
using AxLogging;

/*
 * 20180508 #001 TxD: Commented out all BeginTransaction in this class because it could have caused dead lock in DB (suspection only at this stage)
*/
namespace eHCMS.DAL
{
    public class SqlBedLocations : BedAllocations
    {
        public SqlBedLocations()
            : base()
        {

            
        }

#region department

        public override List<DeptLocation> GetAllDeptLocationByDeptID(long DeptID)
        {
            List<DeptLocation> listRG = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_ByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, DeptID);
                
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRG = GetDepartmentCollectionFromReader(reader);

                reader.Close();
                return listRG;
            }
        }

        public override bool AddNewRoomPrices(long DeptLocationID
                                                ,long StaffID
                                                ,DateTime? EffectiveDate
                                                ,Decimal NormalPrice
                                                ,Decimal PriceForHIPatient
                                                ,Decimal HIAllowedPrice
                                                ,string Note
                                                ,bool IsActive)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spRoomPrice_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;                                                 
                                               
                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));

                    cmd.AddParameter("@EffectiveDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(EffectiveDate));
                    cmd.AddParameter("@NormalPrice", SqlDbType.Decimal, ConvertNullObjectToDBNull(NormalPrice));

                    cmd.AddParameter("@PriceForHIPatient", SqlDbType.Decimal, ConvertNullObjectToDBNull(PriceForHIPatient));
                    cmd.AddParameter("@HIAllowedPrice", SqlDbType.Decimal, ConvertNullObjectToDBNull(HIAllowedPrice));

                    cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Note));
                    cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return true;
        }

        public override List<RoomPrices> GetAllRoomPricesByDeptID(long DeptLocationID) 
        {
            List<RoomPrices> listRG = new List<RoomPrices>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRoomPrice_GetByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRG = GetRoomPricesCollectionFromReader(reader);

                reader.Close();
                return listRG;
            }
        }
        public override bool DeleteBedAllocation(long BedAllocationID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedAllocation_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BedAllocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedAllocationID));
                    

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool AddNewBedAllocation(int times,long DeptLocationID 
                                                   ,string BedNumber 
                                                   ,long MedServiceID
                                                    , string BAGuid
                                                   ,long V_BedLocType 
                                                   ,bool IsActive )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    List<SqlCommand> lstCmd=new List<SqlCommand>();
                    SqlTransaction transaction;
                    cn.Open();
                    // =====▼ #001                                    
                    // transaction = cn.BeginTransaction();
                    // =====▲ #001

                    for (int i=0;i<times;i++ )
                    {
                        SqlCommand cmd = new SqlCommand("spBedAllocation_Insert", cn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                        cmd.AddParameter("@BedNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(BedNumber));

                        cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServiceID));
                        cmd.AddParameter("@V_BedLocType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_BedLocType));

                        cmd.AddParameter("@BAGuid", SqlDbType.VarChar, ConvertNullObjectToDBNull(BAGuid));
                        cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));
                        // =====▼ #001                
                        // cmd.Transaction = transaction;
                        // =====▲ #001

                        lstCmd.Add(cmd);
                    }

                    try
                    {
                        foreach (SqlCommand sQLCmd in lstCmd)
                        {
                            sQLCmd.ExecuteNonQuery();
                        }
                        // =====▼ #001                                        
                        // transaction.Commit();
                        // =====▲ #001
                    }
                    catch (Exception exObj) 
                    {
                        AxLogger.Instance.LogError(exObj);
                        // =====▼ #001                
                        //transaction.Rollback();        
                        // =====▲ #001
                    }
                }
            }
            catch { return false; }
            return true;
        }
        public override bool UpdateBedAllocationList(IList<BedAllocation> LstBedAllocation)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    List<SqlCommand> lstCmd = new List<SqlCommand>();
                    //SqlTransaction transaction;
                    cn.Open();

                    // =====▼ #001                                                        
                    // transaction = cn.BeginTransaction();
                    // =====▲ #001

                    foreach (BedAllocation ba in LstBedAllocation)
                    {
                        SqlCommand cmd = new SqlCommand("spBedAllocation_Update", cn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.AddParameter("@BedAllocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.BedAllocationID));
                        cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.DeptLocationID));
                        cmd.AddParameter("@BedNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ba.BedNumber));

                        cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.MedServiceID));
                        cmd.AddParameter("@V_BedLocType", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.V_BedLocType));

                        cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(ba.IsActive));
                        cmd.AddParameter("@HIBedCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(ba.HIBedCode));
                        // =====▼ #001                                                        
                        // cmd.Transaction = transaction;
                        // =====▲ #001

                        lstCmd.Add(cmd);
                    }
                    try
                    {
                        foreach (SqlCommand sQLCmd in lstCmd)
                        {
                            sQLCmd.ExecuteNonQuery();
                        }
                        // =====▼ #001                                                        
                        // transaction.Commit();                                                                              
                        // =====▲ #001
                    }
                    catch(Exception exObj) 
                    {
                        AxLogger.Instance.LogError(exObj);
                        // =====▼ #001                                                        
                        // transaction.Rollback();                                                  
                        // =====▲ #001
                    }
                }
            }
            catch { return false; }
            return true;
        }
        public override bool UpdateBedAllocationMedSer(string BAGuid 
                                                           ,long MedServiceID 
                                                           ,long V_BedLocType )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedAllocation_UpdateMedSer", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BAGuid", SqlDbType.VarChar, ConvertNullObjectToDBNull(BAGuid));
                    cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(MedServiceID));
                    cmd.AddParameter("@V_BedLocType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_BedLocType));

                    cn.Open();
                    cmd.ExecuteNonQuery();                   
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool AddNewBedAllocationList(IList<BedAllocation> LstBedAllocation)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    List<SqlCommand> lstCmd = new List<SqlCommand>();
                    //SqlTransaction transaction;
                    cn.Open();
                    // =====▼ #001                                                        
                    // transaction = cn.BeginTransaction();                                                      
                    // =====▲ #001
                    foreach (BedAllocation ba in LstBedAllocation)
                    {
                        SqlCommand cmd = new SqlCommand("spBedAllocation_Insert", cn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.DeptLocationID));
                        cmd.AddParameter("@BedNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ba.BedNumber));

                        cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.VRefMedicalServiceItem.MedServiceID));
                        cmd.AddParameter("@V_BedLocType", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.VBedLocType.LookupID));

                        cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(ba.IsActive));
                        cmd.AddParameter("@BAGuid", SqlDbType.VarChar, ConvertNullObjectToDBNull(ba.BAGuid));
                        cmd.AddParameter("@HIBedCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(ba.HIBedCode));

                        // =====▼ #001                                                        
                        // cmd.Transaction = transaction;
                        // =====▲ #001

                        lstCmd.Add(cmd);
                    }
                    try
                    {
                        foreach (SqlCommand sQLCmd in lstCmd)
                        {
                            sQLCmd.ExecuteNonQuery();
                        }
                        // =====▼ #001                                                        
                        // transaction.Commit();
                        // =====▲ #001

                    }
                    catch(Exception exObj) 
                    {
                        AxLogger.Instance.LogError(exObj);
                        // =====▼ #001                                                        
                        // transaction.Rollback();                                                                              
                        // =====▲ #001
                    }
                }
            }
            catch  { return false; }
            return true;
        }
        public override string UpdateBedAllocation(IList<BedAllocation> LstBedAllocation)
        {
            string faultUpdate="";
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    List<SqlCommand> lstCmd = new List<SqlCommand>();
                    
                    cn.Open();
                    
                    foreach (BedAllocation ba in LstBedAllocation)
                    {
                        SqlCommand cmd = new SqlCommand("spBedAllocation_Update", cn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.AddParameter("@BedAllocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.BedAllocationID));
                        cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.DeptLocationID));
                        cmd.AddParameter("@BedNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ba.BedNumber));

                        cmd.AddParameter("@MedServiceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.MedServiceID));
                        cmd.AddParameter("@V_BedLocType", SqlDbType.BigInt, ConvertNullObjectToDBNull(ba.V_BedLocType));

                        cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(ba.IsActive));
                        cmd.AddParameter("@HIBedCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(ba.HIBedCode));

                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            faultUpdate +="\n"+ ba.BedNumber;
                        }
                    }
                }
            }
            catch { return "Error"; }
            return faultUpdate;
        }
        public override List<BedAllocation> GetAllBedAllocationByDeptID(long DeptLocationID,int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<BedAllocation> listRs = new List<BedAllocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedAllocation_GetAllPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, (DeptLocationID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                
                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetBedAllocationCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listRs;
        }
        //Ham tinh count group by MedServiceID
        public override List<BedAllocation> GetCountBedAllocByDeptID(long DeptLocationID, int Choice)
        {
            List<BedAllocation> listRs = new List<BedAllocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedAllocation_GetCount", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, (DeptLocationID));
                cmd.AddParameter("@Choice", SqlDbType.BigInt, (Choice));

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetBedAllocationCollectionFromReader(reader);
                reader.Close();                
            }
            return listRs;
        }
        public override List<BedAllocation> GetAllBedAllocByDeptID(long DeptLocationID,int IsActive,out int Total)
        {
            List<BedAllocation> listRs = new List<BedAllocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedAllocation_GetByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, (DeptLocationID));
                cmd.AddParameter("@IsActive", SqlDbType.BigInt, (IsActive));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);
                
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetBedAllocationCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listRs;
        }
        public override List<MedServiceItemPrice> GetAllDeptMSItemsByDeptIDSerTypeID(long DeptID, int MedicalServiceTypeID)
        {
            List<MedServiceItemPrice> listRs = new List<MedServiceItemPrice>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetDeptMSItems_ByDeptIDSerTypeID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptID", SqlDbType.BigInt, (DeptID));
                cmd.AddParameter("@MedicalServiceTypeID", SqlDbType.Int, (MedicalServiceTypeID));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRs = GetMedServiceItemPriceCollectionFromReader(reader);
                reader.Close();
              
            }
            return listRs;
        }
        
#endregion
        #region
        public override bool UpdateBedLocType(long BedLocTypeID 
	                                            ,string BedLocTypeName 
	                                            ,string Description )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedLocType_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BedLocTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedLocTypeID));
                    cmd.AddParameter("@BedLocTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(BedLocTypeName));
                    cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool AddNewBedLocType(string BedLocTypeName, string Description)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedLocTypeInsert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BedLocTypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(BedLocTypeName));
                    cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool DeleteBedLocType(long BedLocTypeID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedLocType_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BedLocTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedLocTypeID));
                    
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }

        public override List<BedAllocType> GetAllBedLocType()
        {
            List<BedAllocType> listRs = new List<BedAllocType>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedLocType_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    BedAllocType p = new BedAllocType();
                    p.BedLocTypeID = (long)reader["BedLocTypeID"];
                    p.BedLocTypeName = reader["BedLocTypeName"].ToString();
                    p.Description = reader["Description"].ToString();

                    listRs.Add(p);
                }                
                reader.Close();
            }
            return listRs;
        }
        #endregion

        #region BedPatient Alloc

        public override bool UpdateBedPatientAllocs(long BedPatientID
                                                  , long BedAllocationID
                                                  , long PtRegistrationID
                                                  , DateTime? AdmissionDate
                                                  , int ExpectedStayingDays
                                                  , DateTime? DischargeDate
                                                  , bool IsActive)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedPatientAllocs_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BedPatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedPatientID));
                    cmd.AddParameter("@BedAllocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedAllocationID));
                    cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                    cmd.AddParameter("@AdmissionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AdmissionDate));
                    cmd.AddParameter("@ExpectedStayingDays", SqlDbType.SmallInt, ConvertNullObjectToDBNull(ExpectedStayingDays));
                    cmd.AddParameter("@DischargeDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(DischargeDate));
                    cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return true;
        }
        public override int AddNewBedPatientAllocs(BedPatientAllocs alloc)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return AddNewBedPatientAllocs(alloc,cn,null);
            }
        }
        public override int AddNewBedPatientAllocs(BedPatientAllocs alloc, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "spBedPatientAllocs_Insert";
            long bedAllocId = 0;

            if(alloc.VBedAllocation != null)
            {
                bedAllocId = alloc.VBedAllocation.BedAllocationID;
            }
            else
            {
                bedAllocId = alloc.BedAllocationID;
            }

            if(bedAllocId == 0)
            {
                throw new Exception(eHCMSResources.Z1682_G1_ChonGiuong);
            }

            long responsibleDeptID;
            if (alloc.ResponsibleDepartment != null)
            {
                responsibleDeptID = alloc.ResponsibleDepartment.DeptID;
            }
            else
            {
                responsibleDeptID = alloc.ResponsibleDeptID;
            }

            if(responsibleDeptID == 0)
            {
                throw new Exception(eHCMSResources.Z1683_G1_ChonKhoaChiuTrachNhiem);
            }

            cmd.AddParameter("@BedAllocationID", SqlDbType.BigInt, bedAllocId);
            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, alloc.PtRegistrationID);
            cmd.AddParameter("@ResponsibleDeptID", SqlDbType.BigInt, responsibleDeptID);
            cmd.AddParameter("@CheckInDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(alloc.CheckInDate));
            cmd.AddParameter("@ExpectedStayingDays", SqlDbType.SmallInt, alloc.ExpectedStayingDays);
            cmd.AddParameter("@CheckOutDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(alloc.CheckOutDate));
            cmd.AddParameter("@IsActive", SqlDbType.Bit, alloc.IsActive);
            cmd.AddParameter("@PatientInBed", SqlDbType.Bit, alloc.PatientInBed);
            cmd.AddParameter("@InPatientDeptDetailID", SqlDbType.BigInt, ConvertNullObjectToDBNull(alloc.InPatientDeptDetailID));

            var retVal = cmd.ExecuteNonQuery();
            return retVal;
        }
        public override bool AddNewBedPatientAllocs(long BedAllocationID
                                                  , long PtRegistrationID
                                                  , DateTime? AdmissionDate
                                                  , int ExpectedStayingDays
                                                  , DateTime? DischargeDate
                                                  , bool IsActive)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedPatientAllocs_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@BedAllocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedAllocationID));
                cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PtRegistrationID));
                cmd.AddParameter("@AdmissionDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AdmissionDate));
                cmd.AddParameter("@ExpectedStayingDays", SqlDbType.SmallInt, ConvertNullObjectToDBNull(ExpectedStayingDays));
                cmd.AddParameter("@DischargeDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(DischargeDate));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));

                cn.Open();
                var retVal=cmd.ExecuteNonQuery();
                if (retVal > 0)
                    return true;
                else return false;
            }
        }
        public override bool DeleteBedPatientAllocs(long BedPatientID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spBedPatientAllocs_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@BedPatientID", SqlDbType.BigInt, ConvertNullObjectToDBNull(BedPatientID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return true;
        }
         
	//IsActive 

        public override List<BedPatientAllocs> GetAllBedPatientAllocByDeptID(long DeptLocationID, out int Total)
        {
            List<BedPatientAllocs> listRs = new List<BedPatientAllocs>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spBedPatientAlloc_GetByDeptID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@IsActive", SqlDbType.BigInt, 1);


                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetBedPatientAllocsCollectionFromReader(reader);                
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                    Total = -1;
            }
            return listRs;
        }
        #endregion

        public override BedPatientAllocs GetActiveBedAllocationByRegistrationID(long registrationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                return GetActiveBedAllocationByRegistrationID(registrationID, cn, null);
            }
        }
        public override BedPatientAllocs GetActiveBedAllocationByRegistrationID(long registrationID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_GetActiveBedAllocationByRegistrationID";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(registrationID));

            IDataReader reader = ExecuteReader(cmd);
            BedPatientAllocs retVal = null;
            if(reader != null)
            {
                if(reader.Read())
                {
                    retVal = GetBedPatientAllocsObjFromReader(reader);
                }
                reader.Close();
            }
           
            return retVal;
        }

        public override List<BedPatientAllocs> GetAllBedAllocationsByRegistrationID(long registrationID,long? DeptID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            SqlCommand cmd;
            cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_GetAllBedAllocationByRegistrationID";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@PtRegistrationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(registrationID));
            cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));

            IDataReader reader = ExecuteReader(cmd);
            List<BedPatientAllocs> retVal = null;
            if (reader != null)
            {
                retVal = GetBedPatientAllocCollectionFromReader(reader);
                reader.Close();
            }

            return retVal;
        }

        public override bool MarkDeleteBedPatientAlloc(long bedPatientID, DbConnection conn, DbTransaction tran)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            var cmd = (SqlCommand)conn.CreateCommand();
            cmd.Transaction = (SqlTransaction)tran;
            cmd.CommandText = "sp_BedPatientAllocs_MarkDelete";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameter("@BedPatientID", SqlDbType.BigInt, bedPatientID);

            return cmd.ExecuteNonQuery() > 0;
        }
        public override bool MarkDeleteBedPatientAlloc(long bedPatientID)
        {
            using (var cn = new SqlConnection(this.ConnectionString))
            {
                return MarkDeleteBedPatientAlloc(bedPatientID, cn, null);
            }
        }
    }
}
