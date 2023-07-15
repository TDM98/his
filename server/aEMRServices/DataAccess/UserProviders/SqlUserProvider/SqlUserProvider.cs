using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;

namespace eHCMS.DAL
{
    public class SqlUserProvider : UserProvider
    {
        public SqlUserProvider()
            : base()
        {

        }
        #region Manage Users

        public override UserAccount AuthenticateUser(string userName, string password)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {   
                SqlCommand cmd = new SqlCommand("spGetUserAccount", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                
                cmd.AddParameter("@AccountName", SqlDbType.VarChar, userName);
                cmd.AddParameter("@AccountPassword", SqlDbType.VarChar, password);

                cn.Open();
                UserAccount retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    retVal = GetUserFromReader(reader);
                    retVal.Staff = GetStaffFromReader(reader);
                }
                return retVal;
            }
        }

        public override List<UserAccount> GetAllUsers()
        {
            throw new NotImplementedException();
        }
        public override bool AddModule(Module newModule, out int ModuleID)
        {
            throw new NotImplementedException();
        }
        public override bool AddUser(UserAccount newUser, out long UserID)
        {
            UserID = -1;
            //For testing only
            if (string.IsNullOrWhiteSpace(newUser.AccountPassword))
            {
                newUser.AccountPassword = "1";
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInsertUser", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UserName", SqlDbType.VarChar, ConvertNullObjectToDBNull(newUser.AccountName));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(newUser.StaffID));
                cmd.AddParameter("@Password", SqlDbType.VarChar, ConvertNullObjectToDBNull(newUser.AccountPassword));
                cmd.AddParameter("@LastLogin", SqlDbType.DateTime, ConvertNullObjectToDBNull(newUser.LastLogin));
                cmd.AddParameter("@UserID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);

                UserID = (long)cmd.Parameters["@UserID"].Value;

                return true;
            }
        }
        public override bool DeleteUserByID(long userID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteUserByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UserID", SqlDbType.BigInt, userID);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);

                return true;
            }
        }
        public override bool DeleteUserByName(string userName)
        {
            throw new NotImplementedException();
        }
        protected override List<Module> GetModuleCollectionFromReader(IDataReader reader)
        {
            return base.GetModuleCollectionFromReader(reader);
        }
        protected override Module GetModuleFromReader(IDataReader reader)
        {
            return base.GetModuleFromReader(reader);
        }
        public override UserAccount GetUserByID(long userID)
        {
            throw new NotImplementedException();
        }
        public override bool UpdateUser(UserAccount info)
        {
            throw new NotImplementedException();
        }
        public override List<UserAccount> SearchUsers(UserSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("proc_SearchUsers", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UserName", SqlDbType.VarChar, ConvertNullObjectToDBNull(criteria.UserName));
                cmd.AddParameter("@GroupID", SqlDbType.Int, criteria.GroupID);

                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);

                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, null, ParameterDirection.Output);

                cn.Open();
                List<UserAccount> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetUserCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;
                return retVal;
            }
        }

        public override bool CheckIfUserNameExists(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckIfUserNameExists", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UserName", SqlDbType.VarChar, ConvertNullObjectToDBNull(userName));

                cn.Open();

                object obj = ExecuteScalar(cmd);

                if (obj == null)
                    return false;

                return (int)obj > 0;
            }
        }
        public override UserAccount GetUserWithGroupInfo(long userID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetUserWithGroupInfo", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UserID", SqlDbType.Int, userID);
                cn.Open();
                UserAccount retVal = null;
                Group tempGroup = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    retVal = GetUserFromReader(reader);
                    if (reader["GroupID"] != DBNull.Value)
                        tempGroup = GetUserGroupFromReader(reader);
                }
                if (retVal != null)
                {
                    retVal.UserGroups = new ObservableCollection<Group>();
                    if (tempGroup != null)
                        retVal.UserGroups.Add(tempGroup);

                    while (reader.Read())
                    {
                        if (reader["GroupID"] != DBNull.Value)
                        {
                            tempGroup = GetUserGroupFromReader(reader);
                            retVal.UserGroups.Add(tempGroup);
                        }
                    }
                }

                return retVal;
            }
        }
        public override void AssignUserToGroups(long userID, List<int> groupIDs)
        {
            //spAssignUserToGroups
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spAssignUserToGroups", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UserID", SqlDbType.Int, userID);
                string temp = "";
                if(groupIDs != null && groupIDs.Count > 0)
                {
                    temp = String.Join(",", groupIDs);
                }

                cmd.AddParameter("@GroupIDs", SqlDbType.VarChar, temp);
                cn.Open();
                ExecuteNonQuery(cmd);
            }
        }
        #endregion
#region Manage UserGroups
        public override Group GetGroupWithRoleInfo(int groupID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetGroupWithRoleInfo", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@GroupID", SqlDbType.Int, groupID);
                cn.Open();
                Group retVal = null;
                Role tempRole = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    retVal = GetUserGroupFromReader(reader);
                    if (reader["RoleID"] != DBNull.Value)
                        tempRole = GetRoleFromReader(reader);
                }
                if (retVal != null)
                {
                    retVal.Roles = new ObservableCollection<Role>();
                    if (tempRole != null)
                        retVal.Roles.Add(tempRole);

                    while (reader.Read())
                    {
                        if (reader["RoleID"] != DBNull.Value)
                        {
                            tempRole = GetRoleFromReader(reader);
                            retVal.Roles.Add(tempRole);
                        }
                    }
                }

                return retVal;
            }
        }
        public override void AssignGroupToRoles(int groupID, List<int> roleIDs)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spAssignGroupToRoles", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@GroupID", SqlDbType.Int, groupID);
                string temp = "";
                if (roleIDs != null && roleIDs.Count > 0)
                {
                    temp = String.Join(",", roleIDs);
                }

                cmd.AddParameter("@RoleIDs", SqlDbType.VarChar, temp);
                cn.Open();
                ExecuteNonQuery(cmd);
            }
        }

        public override bool DeleteGroupByID(int groupID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteUserGroupByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UserGroupID", SqlDbType.Int, groupID);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);

                return true;
            }
        }
        public override bool AddGroup(Group newGroup, out int GroupID)
        {
            GroupID = -1;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInsertUserGroup", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@GroupName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newGroup.GroupName));
                cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newGroup.Description)); 
                cmd.AddParameter("@GroupID", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);

                GroupID = (int)cmd.Parameters["@GroupID"].Value;

                return true;
            }
        }
        public override bool CheckIfGroupNameExists(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                return false;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckIfUserGroupNameExists", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@GroupName", SqlDbType.VarChar, ConvertNullObjectToDBNull(groupName));

                cn.Open();

                object obj = ExecuteScalar(cmd);

                if (obj == null)
                    return false;

                return (int)obj > 0;
            }
        }
        public override List<Group> GetAllGroups()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllUserGroups", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<Group> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetUserGroupCollectionFromReader(reader);

                return retVal;
            }
        }

#endregion

#region Manage Roles
        public override bool AddRole(Role newRole, out int RoleID)
        {
            RoleID = -1;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spInsertRole", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RoleName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newRole.RoleName));
                cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newRole.Description));
                cmd.AddParameter("@RoleID", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);

                RoleID = (int)cmd.Parameters["@RoleID"].Value;

                return true;
            }
        }
        public override bool CheckIfRoleNameExists(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckIfRoleNameExists", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RoleName", SqlDbType.VarChar, ConvertNullObjectToDBNull(roleName));

                cn.Open();

                object obj = ExecuteScalar(cmd);

                if (obj == null)
                    return false;

                return (int)obj > 0;
            }
        }
        public override bool DeleteRoleByID(int roleID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteRoleByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RoleID", SqlDbType.Int, roleID);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);

                return true;
            }
        }
        public override List<Role> GetAllRoles()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllRoles", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<Role> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetRoleCollectionFromReader(reader);

                return retVal;
            }
        }

        public override Role GetRoleWithOperationInfo(int roleID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetRoleWithOperationInfo", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RoleID", SqlDbType.Int, roleID);
                cn.Open();
                Role retVal = null;
                Operation tempOp = null;
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null && reader.Read())
                {
                    retVal = GetRoleFromReader(reader);
                    if (reader["OperationID"] != DBNull.Value)
                        tempOp = GetOperationFromReader(reader);
                }
                if (retVal != null)
                {
                    retVal.Operations = new ObservableCollection<Operation>();
                    if (tempOp != null)
                        retVal.Operations.Add(tempOp);

                    while (reader.Read())
                    {
                        if (reader["OperationID"] != DBNull.Value)
                        {
                            tempOp = GetOperationFromReader(reader);
                            retVal.Operations.Add(tempOp);
                        }
                    }
                }

                return retVal;
            }
        }
        public override void AssignRoleToOperations(int roleID, List<int> operationIDs)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spAssignRoleToOperations", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RoleID", SqlDbType.Int, roleID);
                string temp = "";
                if (operationIDs != null && operationIDs.Count > 0)
                {
                    temp = String.Join(",", operationIDs);
                }

                cmd.AddParameter("@OperationIDs", SqlDbType.VarChar, temp);
                cn.Open();
                ExecuteNonQuery(cmd);
            }
        }
#endregion
#region Manage Modules
        public override List<DataEntities.Module> GetAllModules()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllModules", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<DataEntities.Module> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetModuleCollectionFromReader(reader);

                return retVal;
            }
        }
#endregion

#region Manage Operations
        public override List<Operation> GetAllOperations()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllOperations", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<Operation> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetOperationCollectionFromReader(reader);

                return retVal;
            }
        }
#endregion

#region Manage Staffs
        public override List<Staff> SearchStaffs(StaffSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            totalCount = 0;
            if (criteria == null)
            {
                return null;
            }

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("proc_SearchStaffs", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@FullName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(criteria.FullName));
                cmd.AddParameter("@DepartmentID", SqlDbType.BigInt, criteria.DepartmentID);

                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);

                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, null, ParameterDirection.Output);

                cn.Open();
                List<Staff> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetStaffCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;
                return retVal;
            }
        }
#endregion

#region User Login
        public override bool AddUserLoginHistory(UserLoginHistory Ulh)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserLoginHistory_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AccountID", SqlDbType.BigInt, Ulh.AccountID);
                cmd.AddParameter("@HostName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Ulh.HostName));
                cmd.AddParameter("@HostIPV4", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Ulh.HostIPV4));
                
                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                return retVal>0;
            }
        }
        public override bool UpdateUserLoginHistory(UserLoginHistory Ulh)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserLoginHistory_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LoggedHistoryID", SqlDbType.BigInt, Ulh.LoggedHistoryID);
                cmd.AddParameter("@AccountID", SqlDbType.BigInt,ConvertNullObjectToDBNull(Ulh.AccountID));
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

#region User authorization
        public override List<UserSubAuthorization> GetUserSubAuthorizationPaging(UserSubAuthorization Usa, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserSubAuthorizationSearch", cn);
                cmd.AddParameter("@AuthPwd", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Usa.AuthPwd));
                cmd.AddParameter("@AccountIDAuth", SqlDbType.BigInt, ConvertNullObjectToDBNull(Usa.AccountIDAuth));
                cmd.AddParameter("@AccountIDSub", SqlDbType.BigInt, ConvertNullObjectToDBNull(Usa.AccountIDSub));
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);

                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, null, ParameterDirection.Output);

                cn.Open();
                List<UserSubAuthorization> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetUserSubAuthorizationCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;
                return retVal;
            }
        }
        public override UserSubAuthorization GetUserSubAuthorization(UserSubAuthorization Usa)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserSubAuthorizationGet", cn);
                cmd.AddParameter("@AuthPwd", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Usa.AuthPwd));
                cmd.AddParameter("@AccountIDAuth", SqlDbType.BigInt, ConvertNullObjectToDBNull(Usa.AccountIDAuth));
                cmd.AddParameter("@AccountIDSub", SqlDbType.BigInt, ConvertNullObjectToDBNull(Usa.AccountIDSub));
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                UserSubAuthorization retVal = null;
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    retVal = GetUserSubAuthorizationFromReader(reader);
                }
                
                reader.Close();

                return retVal;
            }
        }
        public override bool AddUserSubAuthorization(UserSubAuthorization Usa)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserSubAuthorizationInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AuthPwd", SqlDbType.NVarChar, Usa.AuthPwd);
                cmd.AddParameter("@AccountIDAuth", SqlDbType.BigInt, ConvertNullObjectToDBNull(Usa.AccountIDAuth));
                cmd.AddParameter("@AccountIDSub", SqlDbType.BigInt, ConvertNullObjectToDBNull(Usa.AccountIDSub));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }
        public override bool UpdateUserSubAuthorization(UserSubAuthorization Usa)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserSubAuthorizationUpdate", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AuthPwd", SqlDbType.NVarChar, Usa.AuthPwd);
                cmd.AddParameter("@AccountIDAuth", SqlDbType.BigInt, ConvertNullObjectToDBNull(Usa.AccountIDAuth));
                cmd.AddParameter("@AccountIDSub", SqlDbType.BigInt, ConvertNullObjectToDBNull(Usa.AccountIDSub));

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }
        public override bool DeleteUserSubAuthorization(long SubUserAuthorizationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserSubAuthorizationDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@SubUserAuthorizationID", SqlDbType.BigInt, SubUserAuthorizationID);

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                return retVal > 0;
            }
        }

#endregion

    }
}
