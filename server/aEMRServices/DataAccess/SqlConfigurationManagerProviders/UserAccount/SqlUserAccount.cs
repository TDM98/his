/*
  * 20180409 #002 TTM: Them parameter insert va update cho Certificate va Scode
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using AxLogging;
namespace eHCMS.DAL
{
    public class SqlUserAccount : UserAccounts
    {
        public SqlUserAccount()
            : base()
        {

            
        }

        public override List<Lookup> GetLookupOperationType()
        {
            List<Lookup> objLst = new List<Lookup>();
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.V_Operation);
            return objLst;
        }
        #region userAccount

        
        #endregion


        #region Group
        public override List<Group> GetAllGroupByGroupID(long GroupID)
        {
            List<Group> listRs = new List<Group>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_Groups_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@GroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(GroupID));
                
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetGroupCollectionFromReader(reader);                
                reader.Close();
                
            }
            return listRs;
        }
        public override List<UserGroup> GetAllUserGroupGetByID(long AccountID, long GroupID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<UserGroup> listRs = new List<UserGroup>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserGroups_GetByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AccountID));
                cmd.AddParameter("@GroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(GroupID));
                
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(CountTotal));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetUserGroupCollectionFromReader(reader);                
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

        #region Role Group

        public override List<GroupRole> GetAllGroupRolesGetByID(long GroupID , long RoleID 
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<GroupRole> listRs = new List<GroupRole>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_GroupRoles_GetByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@GroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(GroupID));
                cmd.AddParameter("@RoleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RoleID));

                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(CountTotal));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetGroupRoleCollectionFromReader(reader);
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

        #region Permission

        public override List<Permission> GetAllPermissions_GetByID(long RoleID , long OperationID 
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<Permission> listRs = new List<Permission>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_Permissions_GetByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RoleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RoleID));
                cmd.AddParameter("@OperationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OperationID));

                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(CountTotal));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetPermissionCollectionFromReader(reader);
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

        #region Modules
        public override List<Module> GetAllModules()
        {
            List<Module> listRs = new List<Module>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_Modules_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRs = GetModuleCollectionFromReader(reader);
                reader.Close();

            }
            return listRs;
        }
        public override bool AddNewModules(string ModuleName, int eNum, string Description, int Idx)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Modules_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@ModuleName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ModuleName));
                    cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));
                    cmd.AddParameter("@Idx", SqlDbType.Int, ConvertNullObjectToDBNull(Idx));
                    cmd.AddParameter("@eNum", SqlDbType.Int, ConvertNullObjectToDBNull(eNum));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool UpdateModules(long ModuleID, int eNum, string ModuleName, string Description, int Idx)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Modules_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@ModuleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ModuleID));
                    cmd.AddParameter("@ModuleName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ModuleName));
                    cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));
                    cmd.AddParameter("@Idx", SqlDbType.Int, ConvertNullObjectToDBNull(Idx));
                    cmd.AddParameter("@eNum", SqlDbType.Int, ConvertNullObjectToDBNull(eNum));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool UpdateModulesEnum(long ModuleID, int eNum)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Modules_UpdateEnum", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@ModuleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ModuleID));
                    cmd.AddParameter("@eNum", SqlDbType.Int, ConvertNullObjectToDBNull(eNum));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool DeleteModules(long ModuleID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Modules_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@ModuleID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(ModuleID));
                    
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        
        #endregion

        #region Function
        
        public override List<Function> GetAllFunction(long ModuleID)
        {
            List<Function> listRs = new List<Function>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_Functions_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ModuleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ModuleID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRs = GetFunctionCollectionFromReader(reader);
                reader.Close();

            }
            return listRs;
        }

        public override List<Function> GetAllFunctionsPaging(long ModuleID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<Function> listRs = new List<Function>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_Functions_GetAllPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@ModuleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ModuleID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(CountTotal));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetFunctionCollectionFromReader(reader);
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
        public override bool AddNewFunctions(int ModuleID, int eNum, string FunctionName, string FunctionDescription, int Idx) 
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Functions_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@ModuleID", SqlDbType.Int, ConvertNullObjectToDBNull(ModuleID));
                    cmd.AddParameter("@FunctionName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(FunctionName));
                    cmd.AddParameter("@FunctionDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(FunctionDescription));
                    cmd.AddParameter("@Idx", SqlDbType.TinyInt, ConvertNullObjectToDBNull(Idx));
                    cmd.AddParameter("@eNum", SqlDbType.TinyInt, ConvertNullObjectToDBNull(eNum));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        
        public override bool UpdateFunctionsEnum(long FunctionID, int eNum)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Functions_UpdateEnum", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@FunctionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FunctionID));
                    cmd.AddParameter("@eNum", SqlDbType.TinyInt, ConvertNullObjectToDBNull(eNum));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool UpdateFunctions(long FunctionID, int eNum, int ModuleID, string FunctionName, string FunctionDescription, byte Idx)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Functions_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@FunctionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FunctionID));
                    cmd.AddParameter("@ModuleID", SqlDbType.Int, ConvertNullObjectToDBNull(ModuleID));
                    cmd.AddParameter("@FunctionName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(FunctionName));
                    cmd.AddParameter("@FunctionDescription", SqlDbType.NVarChar, ConvertNullObjectToDBNull(FunctionDescription));
                    cmd.AddParameter("@Idx", SqlDbType.TinyInt, ConvertNullObjectToDBNull(Idx));
                    cmd.AddParameter("@eNum", SqlDbType.TinyInt, ConvertNullObjectToDBNull(eNum));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool DeleteFunctions(long FunctionID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Functions_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@FunctionID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(FunctionID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }

        #endregion

        #region Operation

        public override List<Operation> GetAllOperations(long OperationID)
        {
            List<Operation> listRs = new List<Operation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_Operations_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@OperationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OperationID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRs = GetOperationCollectionFromReader(reader);
                reader.Close();

            }
            return listRs;
        }
        public override List<Operation> GetAllOperationsByFuncID(long FunctionID)
        {
            List<Operation> listRs = new List<Operation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_Operations_GetByFuncID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FunctionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FunctionID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRs = GetOperationCollectionFromReader(reader);
                reader.Close();

            }
            return listRs;
        }

        // 20180921 TNHX: Add method for get all detail Modules Tree
        public override List<Um_ModuleFunctionOperations> GetAllDetails_ModuleFunctionOperations()
        {
            List<Um_ModuleFunctionOperations> listUm = new List<Um_ModuleFunctionOperations>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_AllDetails_ModuleFunctionOperations", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listUm = GetAllDetails_ModuleFunctionOperations(reader);
                reader.Close();
            }
            return listUm;
        }

        public override List<Operation> GetAllOperationsByFuncIDPaging(long FunctionID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<Operation> listRs = new List<Operation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_Operations_FuncIDPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@FunctionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FunctionID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(CountTotal));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetOperationCollectionFromReader(reader);
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
        public override bool AddNewOperations(long FunctionID, string OperationName, string Description)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Operations_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@FunctionID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FunctionID));
                    cmd.AddParameter("@OperationName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OperationName));
                    cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));
                    
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool UpdateOperations(long OperationID, int Enum, string OperationName, string Description)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Operations_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@OperationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OperationID));
                    cmd.AddParameter("@eNum", SqlDbType.BigInt, ConvertNullObjectToDBNull(Enum));
                    cmd.AddParameter("@OperationName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OperationName));
                    cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        
        public override bool DeleteOperations(long OperationID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Operations_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@OperationID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OperationID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return true;
        }

        #endregion

        #region Permission

        public override List<Permission> GetAllPermissions(long PermissionItemID)
        {
            List<Permission> listRs = new List<Permission>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_Permissions_GetAllByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PermissionItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PermissionItemID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRs = GetPermissionCollectionFromReaderEx(reader);
                reader.Close();

            }
            return listRs;
        }
        public override bool AddNewPermissions(int RoleID, long OperationID
                                                ,bool FullControl
			                                    ,bool pView  
			                                    ,bool pAdd  
			                                    ,bool pUpdate  
			                                    ,bool pDelete  
			                                    ,bool pReport
                                                , bool pPrint)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Permissions_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@RoleID", SqlDbType.Int, ConvertNullObjectToDBNull(RoleID));
                    cmd.AddParameter("@OperationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OperationID));

                    cmd.AddParameter("@PermissionFullControl", SqlDbType.Bit, ConvertNullObjectToDBNull(FullControl));
                    cmd.AddParameter("@PermissionView", SqlDbType.Bit, ConvertNullObjectToDBNull(pView));
                    cmd.AddParameter("@PermissionAdd", SqlDbType.Bit, ConvertNullObjectToDBNull(pAdd));
                    cmd.AddParameter("@PermissionUpdate", SqlDbType.Bit, ConvertNullObjectToDBNull(pUpdate));
                    cmd.AddParameter("@PermissionDelete", SqlDbType.Bit, ConvertNullObjectToDBNull(pDelete));
                    cmd.AddParameter("@PermissionReport", SqlDbType.Bit, ConvertNullObjectToDBNull(pReport));
                    cmd.AddParameter("@PermissionPrint", SqlDbType.Bit, ConvertNullObjectToDBNull(pPrint));                                                

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }

        public override bool UpdatePermissionsFull(long PermissionItemID
                                                ,bool FullControl
			                                    ,bool pView  
			                                    ,bool pAdd  
			                                    ,bool pUpdate  
			                                    ,bool pDelete  
			                                    ,bool pReport
                                                , bool pPrint)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Permissions_UpdateFull", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PermissionItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PermissionItemID));
                    
                    cmd.AddParameter("@PermissionFullControl", SqlDbType.Bit, ConvertNullObjectToDBNull(FullControl));
                    cmd.AddParameter("@PermissionView", SqlDbType.Bit, ConvertNullObjectToDBNull(pView));
                    cmd.AddParameter("@PermissionAdd", SqlDbType.Bit, ConvertNullObjectToDBNull(pAdd));
                    cmd.AddParameter("@PermissionUpdate", SqlDbType.Bit, ConvertNullObjectToDBNull(pUpdate));
                    cmd.AddParameter("@PermissionDelete", SqlDbType.Bit, ConvertNullObjectToDBNull(pDelete));
                    cmd.AddParameter("@PermissionReport", SqlDbType.Bit, ConvertNullObjectToDBNull(pReport));
                    cmd.AddParameter("@PermissionPrint", SqlDbType.Bit, ConvertNullObjectToDBNull(pPrint));                                                

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return false;
            }
            return true;
        }
        public override bool UpdatePermissions(long PermissionItemID ,int RoleID ,long OperationID )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Permissions_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PermissionItemID", SqlDbType.BigInt, ConvertNullObjectToDBNull(PermissionItemID));
                    cmd.AddParameter("@RoleID", SqlDbType.Int, ConvertNullObjectToDBNull(RoleID));
                    cmd.AddParameter("@OperationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(OperationID));
                    
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool DeletePermissions(long PermissionItemID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Permissions_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@PermissionItemID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(PermissionItemID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }

        #endregion

        #region Roles

        public override List<Role> GetAllRoles()
        {
            List<Role> listRs = new List<Role>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_Roles_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRs = GetRoleCollectionFromReader(reader);
                reader.Close();

            }
            return listRs;
        }

        public override List<Role> GetAllRolesAllPaging(long RoleID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<Role> listRs = new List<Role>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_Roles_GetAllPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RoleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RoleID));

                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(CountTotal));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetRoleCollectionFromReader(reader);    
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
        public override bool AddNewRoles(string RoleName, string Description)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Roles_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@RoleName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RoleName));
                    cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool UpdateRoles(int RoleID, string RoleName, string Description)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Roles_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@RoleID", SqlDbType.Int, ConvertNullObjectToDBNull(RoleID));
                    cmd.AddParameter("@RoleName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RoleName));
                    cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool DeleteRoles(int RoleID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Roles_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@RoleID", SqlDbType.Int, ConvertNullObjectToDBNull(RoleID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }

        #endregion


        #region UserAccount
        public override List<UserAccount> GetAllUserAccount(long AccountID)
        {
            List<UserAccount> listRs = new List<UserAccount>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserAccounts_GetAll", cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@AccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AccountID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRs = GetUserAccountCollectionFromReader(reader);
                reader.Close();

            }
            return listRs;
        }

        
        public override List<UserAccount> GetAllUserAccountsPaging(string AccountName,int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<UserAccount> listRs = new List<UserAccount>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserAccounts_GetAllPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AccountName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(AccountName));
                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(CountTotal));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetUserAccountCollectionFromReader(reader);
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

        public override bool AddNewUserAccount(long StaffID ,string AccountName ,string AccountPassword ,bool IsActivated )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUserAccounts_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                    cmd.AddParameter("@AccountName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(AccountName));
                    cmd.AddParameter("@AccountPassword", SqlDbType.NVarChar, ConvertNullObjectToDBNull(AccountPassword));
                    cmd.AddParameter("@IsActivated", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActivated));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool UpdateUserAccount(int AccountID ,long StaffID ,string AccountName ,string AccountPassword ,bool IsActivated )
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserAccounts_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AccountID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@AccountName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(AccountName));
                cmd.AddParameter("@AccountPassword", SqlDbType.NVarChar, ConvertNullObjectToDBNull(AccountPassword));
                cmd.AddParameter("@IsActivated", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActivated));

                cn.Open();
                int val=cmd.ExecuteNonQuery();
                return val > 0;
            }
            
        }
        public override bool DeleteUserAccount(int AccountID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUserAccounts_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@AccountID", SqlDbType.Int, ConvertNullObjectToDBNull(AccountID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }

        #endregion


        #region UserGroup
        public override List<refModule> GetAllUserGroupByAccountID(long AccountID)
        {
            List<refModule> listRs = new List<refModule>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserGroups_GetByAccountID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AccountID));

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetUserGroupExCollectionFromReader(reader);
                reader.Close();

            }
            return listRs;
        }

        public override bool AddNewUserGroup(long AccountID, int GroupID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUserGroups_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@AccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AccountID));
                    cmd.AddParameter("@GroupID", SqlDbType.Int, ConvertNullObjectToDBNull(GroupID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }

        public override bool AddNewUserGroupXML(IList<UserGroup> lstUserGroup)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUserGroups_InsertXML", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@DataXML", SqlDbType.Xml, UserGroup_ConvertListToXml(lstUserGroup));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        private string UserGroup_ConvertListToXml(IList<UserGroup> allUserGroup)
        {
            if (allUserGroup != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (UserGroup item in allUserGroup)
                {
                    sb.Append("<UserGroups>");
                    sb.AppendFormat("<AccountID>{0}</AccountID>", item.AccountID);
                    sb.AppendFormat("<GroupID>{0}</GroupID>", item.GroupID);
                    
                    sb.Append("</UserGroups>");
                    
                }
                sb.Append("</DS>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        public override bool UpdateUserGroup(long UGID ,long AccountID ,int GroupID )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUserGroups_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@UGID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UGID));
                    cmd.AddParameter("@AccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AccountID));
                    cmd.AddParameter("@GroupID", SqlDbType.Int, ConvertNullObjectToDBNull(GroupID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool DeleteUserGroup(int UGID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUserGroups_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@UGID", SqlDbType.BigInt, ConvertNullObjectToDBNull(UGID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return true;
        }

        #endregion

        #region Group Role

        public override bool AddNewGroupRoles(int GroupID, int RoleID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_GroupRoles_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@GroupID", SqlDbType.Int, ConvertNullObjectToDBNull(GroupID));
                    cmd.AddParameter("@RoleID", SqlDbType.Int, ConvertNullObjectToDBNull(RoleID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }


        public override bool AddNewGroupRolesXML(IList<GroupRole> lstGroupRole)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_GroupRoles_InsertXML", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@DataXML", SqlDbType.Xml, GroupRole_ConvertListToXml(lstGroupRole));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return true;
        }

        private string GroupRole_ConvertListToXml(IList<GroupRole> allGroupRole)
        {
            if (allGroupRole != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (GroupRole item in allGroupRole)
                {
                    sb.Append("<UM_GroupRoles>");
                    sb.AppendFormat("<GroupID>{0}</GroupID>", item.GroupID);
                    sb.AppendFormat("<RoleID>{0}</RoleID>", item.RoleID);

                    sb.Append("</UM_GroupRoles>");

                }
                sb.Append("</DS>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }
        public override bool UpdateGroupRoles(long GroupRoleID, int GroupID, int RoleID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_GroupRoles_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@GroupRoleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(GroupRoleID));
                    cmd.AddParameter("@GroupID", SqlDbType.Int, ConvertNullObjectToDBNull(GroupID));
                    cmd.AddParameter("@RoleID", SqlDbType.Int, ConvertNullObjectToDBNull(RoleID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool DeleteGroupRoles(long GroupRoleID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_GroupRoles_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@GroupRoleID", SqlDbType.BigInt, ConvertNullObjectToDBNull(GroupRoleID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }

        #endregion


        #region Group
        public override List<Group> GetAllGroupAllPaging(long GroupID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<Group> listRs = new List<Group>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_Groups_GetAllPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@GroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(GroupID));

                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(CountTotal));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetGroupCollectionFromReader(reader);    
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
        public override bool AddNewGroup(string GroupName, string Description)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Groups_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@GroupName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GroupName));
                    cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool UpdateGroup(int GroupID ,string GroupName ,string Description )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Groups_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@GroupID", SqlDbType.Int, ConvertNullObjectToDBNull(GroupID));
                    cmd.AddParameter("@GroupName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GroupName));
                    cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool DeleteGroup(int GroupID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spUM_Groups_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@GroupID", SqlDbType.Int, ConvertNullObjectToDBNull(GroupID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch { return false; }
            return true;
        }

        #endregion

        #region User Login
        public override List<UserLoginHistory> GetUserLogHisGByUserAccount()
        {
            List<UserLoginHistory> listRs = new List<UserLoginHistory>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserLoginHistory_GetUserAccount", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRs = GetUserLoginHistoryCollectionFromReader(reader);
                reader.Close();
            }
            return listRs;
        }
        public override List<UserLoginHistory> GetUserLogHisGByHostName()
        {
            List<UserLoginHistory> listRs = new List<UserLoginHistory>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserLoginHistory_GetHostName", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRs = GetUserLoginHistoryCollectionFromReader(reader);
                reader.Close();
            }
            return listRs;
        }

        public override List<UserLoginHistory> GetAllUserLoginHistoryPaging(UserLoginHistory ulh, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<UserLoginHistory> listRs = new List<UserLoginHistory>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserLoginHistory_GetPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LoggedHistoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ulh.LoggedHistoryID));
                cmd.AddParameter("@AccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ulh.AccountID));
                cmd.AddParameter("@LoggedDateTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(ulh.LogDateTime));
                cmd.AddParameter("@HostName", SqlDbType.VarChar, ConvertNullObjectToDBNull(ulh.HostName));
                cmd.AddParameter("@HostIPV4", SqlDbType.VarChar, ConvertNullObjectToDBNull(ulh.HostIPV4));

                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(CountTotal));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetUserLoginHistoryCollectionFromReader(reader);    
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

        public override bool AddUserLoginHistory(UserLoginHistory Ulh)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserLoginHistory_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AccountID", SqlDbType.BigInt, Ulh.AccountID);
                cmd.AddParameter("@LogDateTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(Ulh.LogDateTime));
                cmd.AddParameter("@HostName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Ulh.HostName));
                cmd.AddParameter("@HostIPV4", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Ulh.HostIPV4));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }
        public override bool UpdateUserLoginHistory(UserLoginHistory Ulh)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserLoginHistory_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LoggedHistoryID", SqlDbType.BigInt, Ulh.LoggedHistoryID);
                cmd.AddParameter("@AccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Ulh.AccountID));
                cmd.AddParameter("@LogDateTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(Ulh.LogDateTime));
                cmd.AddParameter("@HostName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Ulh.HostName));
                cmd.AddParameter("@HostIPV4", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Ulh.HostIPV4));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }
        public override bool DeleteUserLoginHistory(long LoggedHistoryID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserLoginHistory_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LoggedHistoryID", SqlDbType.BigInt, LoggedHistoryID);

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

        #endregion

#region staff

        public override Staff GetStaffsByID(long StaffID)
        {
            Staff curStaff = new Staff();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetStaffsByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    curStaff = GetStaffsFromReader(reader);
                }
                reader.Close();
                
            }
            return curStaff;
        }
        public override List<Staff> GetAllStaffAllPaging(Staff searchStaff, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<Staff> listRs = new List<Staff>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffs_GetPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchStaff.DeptID));
                cmd.AddParameter("@StaffCatgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchStaff.StaffCatgID));
                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchStaff.CityProvinceID));
                
                cmd.AddParameter("@SName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(searchStaff.FullName));
                cmd.AddParameter("@Sex", SqlDbType.Bit, ConvertNullObjectToDBNull(searchStaff.Sex));
                cmd.AddParameter("@CountryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchStaff.CountryID));
                
                cmd.AddParameter("@SMobiPhoneNumber", SqlDbType.Char, ConvertNullObjectToDBNull(searchStaff.SMobiPhoneNumber));
                cmd.AddParameter("@SEmailAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(searchStaff.SEmailAddress));
                cmd.AddParameter("@SEmployDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(searchStaff.SEmployDate));

                cmd.AddParameter("@V_Religion", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchStaff.V_Religion));
                cmd.AddParameter("@V_MaritalStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchStaff.V_MaritalStatus));
                cmd.AddParameter("@V_Ethnic", SqlDbType.BigInt, ConvertNullObjectToDBNull(searchStaff.V_Ethnic));



                cmd.AddParameter("@PageSize", SqlDbType.Int, ConvertNullObjectToDBNull(PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, ConvertNullObjectToDBNull(PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, ConvertNullObjectToDBNull(OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, ConvertNullObjectToDBNull(CountTotal));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramTotal);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetStaffsCollectionFromReader(reader);
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

        public override bool AddNewStaff(Staff newStaff)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffs_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@CountryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.CountryID));
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.DeptID));
                cmd.AddParameter("@StaffCatgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.StaffCatgID));
                cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.CityProvinceID));
                cmd.AddParameter("@RoleCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.RoleCode));
                cmd.AddParameter("@SFirstName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SFirstName));
                cmd.AddParameter("@SMiddleName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SMiddleName));
                cmd.AddParameter("@SLastName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SLastName));
                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.FullName));
                cmd.AddParameter("@Sex", SqlDbType.Bit, ConvertNullObjectToDBNull(newStaff.Sex));
                cmd.AddParameter("@SDOB", SqlDbType.DateTime, ConvertNullObjectToDBNull(newStaff.SDOB));
                cmd.AddParameter("@SBirthPlace", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SBirthPlace));
                cmd.AddParameter("@SIDNumber", SqlDbType.Char, ConvertNullObjectToDBNull(newStaff.SIDNumber));
                cmd.AddParameter("@SPlaceOfIssue", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SPlaceOfIssue));
                cmd.AddParameter("@SStreetAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SStreetAddress));
                cmd.AddParameter("@SSurburb", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SSurburb));
                cmd.AddParameter("@SState", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SState));
                cmd.AddParameter("@SPhoneNumber", SqlDbType.Char, ConvertNullObjectToDBNull(newStaff.SPhoneNumber));
                cmd.AddParameter("@SMobiPhoneNumber", SqlDbType.Char, ConvertNullObjectToDBNull(newStaff.SMobiPhoneNumber));
                cmd.AddParameter("@SEmailAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SEmailAddress));
                cmd.AddParameter("@SEmployDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(newStaff.SEmployDate));
                cmd.AddParameter("@SLeftDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(newStaff.SLeftDate));
                cmd.AddParameter("@V_Religion", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_Religion));
                cmd.AddParameter("@V_MaritalStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_MaritalStatus));
                cmd.AddParameter("@V_Ethnic", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_Ethnic));
                cmd.AddParameter("@SAccountNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SAccountNumber));
                cmd.AddParameter("@V_BankName", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_BankName));
                cmd.AddParameter("@SEmploymentHistory", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SEmploymentHistory));
                cmd.AddParameter("@SImage", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SImage));
                cmd.AddParameter("@PImage", SqlDbType.Image, ConvertNullObjectToDBNull(newStaff.PImage));
                //*▼====: #002
                cmd.AddParameter("@CertificateNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SCertificateNumber));
                cmd.AddParameter("@SCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(newStaff.SCode));
                //*▲====: #002
                /*▼====: #001*/
                cmd.AddParameter("@V_AcademicRank", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_AcademicRank));
                cmd.AddParameter("@V_AcademicDegree", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_AcademicDegree));
                cmd.AddParameter("@V_Education", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_Education));
                cmd.AddParameter("@IsFund", SqlDbType.Bit, ConvertNullObjectToDBNull(newStaff.IsFund));
                cmd.AddParameter("@IsReport", SqlDbType.Bit, ConvertNullObjectToDBNull(newStaff.IsReport));
                /*▲====: #001*/
                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return true;
        }
        public override bool UpdateStaff(Staff newStaff)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spStaffs_Update", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.StaffID));
                    cmd.AddParameter("@CountryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.CountryID));
                    cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.DeptID));
                    cmd.AddParameter("@StaffCatgID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.StaffCatgID));
                    cmd.AddParameter("@CityProvinceID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.CityProvinceID));
                    cmd.AddParameter("@RoleCode", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.RoleCode));
                    cmd.AddParameter("@SFirstName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SFirstName));
                    cmd.AddParameter("@SMiddleName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SMiddleName));
                    cmd.AddParameter("@SLastName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SLastName));
                    cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.FullName));
                    cmd.AddParameter("@Sex", SqlDbType.Bit, ConvertNullObjectToDBNull(newStaff.Sex));
                    cmd.AddParameter("@SDOB", SqlDbType.DateTime, ConvertNullObjectToDBNull(newStaff.SDOB));
                    cmd.AddParameter("@SBirthPlace", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SBirthPlace));
                    cmd.AddParameter("@SIDNumber", SqlDbType.Char, ConvertNullObjectToDBNull(newStaff.SIDNumber));
                    cmd.AddParameter("@SPlaceOfIssue", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SPlaceOfIssue));
                    cmd.AddParameter("@SStreetAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SStreetAddress));
                    cmd.AddParameter("@SSurburb", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SSurburb));
                    cmd.AddParameter("@SState", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SState));
                    cmd.AddParameter("@SPhoneNumber", SqlDbType.Char, ConvertNullObjectToDBNull(newStaff.SPhoneNumber));
                    cmd.AddParameter("@SMobiPhoneNumber", SqlDbType.Char, ConvertNullObjectToDBNull(newStaff.SMobiPhoneNumber));
                    cmd.AddParameter("@SEmailAddress", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SEmailAddress));
                    cmd.AddParameter("@SEmployDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(newStaff.SEmployDate));
                    cmd.AddParameter("@SLeftDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(newStaff.SLeftDate));
                    cmd.AddParameter("@V_Religion", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_Religion));
                    cmd.AddParameter("@V_MaritalStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_MaritalStatus));
                    cmd.AddParameter("@V_Ethnic", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_Ethnic));
                    cmd.AddParameter("@SAccountNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SAccountNumber));
                    cmd.AddParameter("@V_BankName", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_BankName));
                    cmd.AddParameter("@SEmploymentHistory", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SEmploymentHistory));
                    cmd.AddParameter("@SImage", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SImage));
                    cmd.AddParameter("@PImage", SqlDbType.Image, ConvertNullObjectToDBNull(newStaff.PImage));
                    //*▼====: #002
                    cmd.AddParameter("@CertificateNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SCertificateNumber));
                    cmd.AddParameter("@SCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(newStaff.SCode));
                    //*▲====: #002
                    /*▼====: #001*/
                    cmd.AddParameter("@V_AcademicRank", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_AcademicRank));
                    cmd.AddParameter("@V_AcademicDegree", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_AcademicDegree));
                    cmd.AddParameter("@V_Education", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_Education));
                    cmd.AddParameter("@IsFund", SqlDbType.Bit, ConvertNullObjectToDBNull(newStaff.IsFund));
                    cmd.AddParameter("@IsReport", SqlDbType.Bit, ConvertNullObjectToDBNull(newStaff.IsReport));
                    /*▲====: #001*/
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
        public override bool DeleteStaff(int StaffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spStaffs_Delete", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@StaffID", SqlDbType.Int, ConvertNullObjectToDBNull(StaffID));

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch  { return false; }
            return true;
        }
#endregion

        #region StaffDeptResponsibility
        public override List<StaffDeptResponsibilities> GetStaffDeptResponsibilitiesByDeptID(StaffDeptResponsibilities p,bool isHis)
        {
            List<StaffDeptResponsibilities> listRG = new List<StaffDeptResponsibilities>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffDeptResponsibilities", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@StaffDeptResposibilitiesID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.StaffDeptResponsibilitiesID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.StaffID));
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.DeptID));
                cmd.AddParameter("@IsHis", SqlDbType.Bit, ConvertNullObjectToDBNull(isHis));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRG = GetStaffDeptResponsibilitiesCollectionFromReader(reader);
                reader.Close();
                return listRG;
            }
        }
        public override bool InsertStaffDeptResponsibilitiesXML(List<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffDeptResponsibilitiesInsertXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, StaffDeptRes_ConvertListToXml(lstStaffDeptRes));

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

        public override bool UpdateStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffDeptResponsibilitiesUpdateXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, StaffDeptRes_ConvertListToXml(lstStaffDeptRes));

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

        public override bool DeleteStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffDeptResponsibilitiesDeleteXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, StaffDeptRes_ConvertListToXml(lstStaffDeptRes));

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


        private string StaffDeptRes_ConvertListToXml(IList<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            if (lstStaffDeptRes != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (StaffDeptResponsibilities item in lstStaffDeptRes)
                {
                    sb.Append("<StaffDeptResponsibilities>");
                    if (item.StaffDeptResponsibilitiesID>0)
                        sb.AppendFormat("<StaffDeptResponsibilitiesID>{0}</StaffDeptResponsibilitiesID>", item.StaffDeptResponsibilitiesID);
                    sb.AppendFormat("<StaffID>{0}</StaffID>", item.StaffID);
                    sb.AppendFormat("<DeptID>{0}</DeptID>", item.DeptID);
                    item.Responsibilities_32 = item.GetTotalValue();
                    sb.AppendFormat("<Responsibilities_32>{0}</Responsibilities_32>", item.Responsibilities_32);
                    sb.AppendFormat("<IsDeleted>{0}</IsDeleted>", item.IsDeleted);

                    sb.Append("</StaffDeptResponsibilities>");

                }
                sb.Append("</DS>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region StaffStoreDeptResponsibility
        public override List<StaffStoreDeptResponsibilities> GetStaffStoreDeptResponsibilitiesByDeptID(StaffStoreDeptResponsibilities p, bool isHis)
        {
            List<StaffStoreDeptResponsibilities> listRG = new List<StaffStoreDeptResponsibilities>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffStoreDeptResponsibilities", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@StaffStoreDeptResponsibilitiesID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.StaffStoreDeptResponsibilitiesID));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.StaffID));
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.DeptID));
                cmd.AddParameter("@StoreID", SqlDbType.BigInt, ConvertNullObjectToDBNull(p.StoreID));
                cmd.AddParameter("@IsHis", SqlDbType.Bit, ConvertNullObjectToDBNull(isHis));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRG = GetStaffStoreDeptResponsibilitiesCollectionFromReader(reader);
                reader.Close();
                return listRG;
            }
        }
        public override bool InsertStaffStoreDeptResponsibilitiesXML(List<StaffStoreDeptResponsibilities> lstStaffDeptRes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffStoreDeptResponsibilitiesInsertXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, StaffStoreDeptRes_ConvertListToXml(lstStaffDeptRes));

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

        public override bool UpdateStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffStoreDeptResponsibilitiesUpdateXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, StaffStoreDeptRes_ConvertListToXml(lstStaffDeptRes));

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

        public override bool DeleteStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffStoreDeptResponsibilitiesDeleteXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, StaffStoreDeptRes_ConvertListToXml(lstStaffDeptRes));

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


        private string StaffStoreDeptRes_ConvertListToXml(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes)
        {
            if (lstStaffDeptRes != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (StaffStoreDeptResponsibilities item in lstStaffDeptRes)
                {
                    sb.Append("<StaffStoreDeptResponsibilities>");
                    if (item.StaffStoreDeptResponsibilitiesID> 0)
                        sb.AppendFormat("<StaffStoreDeptResponsibilitiesID>{0}</StaffStoreDeptResponsibilitiesID>", item.StaffStoreDeptResponsibilitiesID);
                    sb.AppendFormat("<StaffID>{0}</StaffID>", item.StaffID);
                    sb.AppendFormat("<DeptID>{0}</DeptID>", item.DeptID);
                    sb.AppendFormat("<StoreID>{0}</StoreID>", item.StoreID);
                    item.Responsibilities_32 = item.GetTotalValue();
                    sb.AppendFormat("<Responsibilities_32>{0}</Responsibilities_32>", item.Responsibilities_32);
                    sb.AppendFormat("<IsDeleted>{0}</IsDeleted>", item.IsDeleted);

                    sb.Append("</StaffStoreDeptResponsibilities>");

                }
                sb.Append("</DS>");

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        #endregion

    }
}
