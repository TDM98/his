using System;
using System.Collections.Generic;

using DataEntities;
using System.Data.SqlClient;
using System.Data;

using System.Reflection;
using eHCMS.Configurations;
using eHCMS.DAL;
using System.Collections.ObjectModel;

namespace aEMR.DataAccessLayer.Providers
{
    public class UserProvider : DataProviderBase
    {
        static private UserProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public UserProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserProvider();
                }
                return _instance;
            }
        }

        public UserProvider()
        {
            this.ConnectionString = Globals.Settings.Patients.ConnectionString;
        }

        protected virtual UserAccount GetUserFromReader(IDataReader reader)
        {
            UserAccount p = new UserAccount();

            p.AccountID = (long)reader["AccountID"];
            p.AccountName = (string)reader["AccountName"];
            p.AccountPassword = reader["AccountPassword"].ToString();
            p.StaffID = reader["StaffID"] as Int64?;

            return p;
        }

        protected virtual List<UserAccount> GetUserCollectionFromReader(IDataReader reader)
        {
            List<UserAccount> retVal = new List<UserAccount>();
            while (reader.Read())
            {
                retVal.Add(GetUserFromReader(reader));
            }
            return retVal;
        }

        #region Manage Modules
        protected DataEntities.Module GetModuleFromReader(IDataReader reader)
        {
            DataEntities.Module p = new DataEntities.Module();

            p.ModuleID = (int)reader["ModuleID"];
            p.ModuleName = reader["ModuleName"].ToString();
            p.Description = reader["Description"].ToString();

            return p;
        }

        protected List<DataEntities.Module> GetModuleCollectionFromReader(IDataReader reader)
        {
            List<DataEntities.Module> retVal = new List<DataEntities.Module>();
            while (reader.Read())
            {
                retVal.Add(GetModuleFromReader(reader));
            }
            return retVal;
        }


        #endregion
        #region Manage UserGroups


        protected virtual Group GetUserGroupFromReader(IDataReader reader)
        {
            Group p = new Group();

            p.GroupID = (int)reader["GroupID"];
            p.GroupName = (string)reader["GroupName"];
            p.Description = reader["Description"].ToString();

            return p;
        }

        protected virtual List<Group> GetUserGroupCollectionFromReader(IDataReader reader)
        {
            List<Group> retVal = new List<Group>();
            while (reader.Read())
            {
                retVal.Add(GetUserGroupFromReader(reader));
            }
            return retVal;
        }
        #endregion
        #region Manage Roles

        protected virtual Role GetRoleFromReader(IDataReader reader)
        {
            Role p = new Role();

            p.RoleID = (int)reader["RoleID"];
            p.RoleName = (string)reader["RoleName"];
            p.Description = reader["Description"].ToString();

            return p;
        }

        protected virtual List<Role> GetRoleCollectionFromReader(IDataReader reader)
        {
            List<Role> retVal = new List<Role>();
            while (reader.Read())
            {
                retVal.Add(GetRoleFromReader(reader));
            }
            return retVal;
        }
        #endregion


        #region Manage Operations
        protected virtual Operation GetOperationFromReader(IDataReader reader)
        {
            Operation p = new Operation();

            p.OperationID = (int)reader["OperationID"];
            if (reader["FunctionID"] != DBNull.Value)
            {
                p.FunctionID = (int)reader["FunctionID"];
            }
            p.OperationName = reader["OperationName"] as string;
            p.Description = reader["Description"] as string;

            return p;
        }

        protected virtual List<Operation> GetOperationCollectionFromReader(IDataReader reader)
        {
            List<Operation> retVal = new List<Operation>();
            while (reader.Read())
            {
                retVal.Add(GetOperationFromReader(reader));
            }
            return retVal;
        }

        
        #endregion

        #region Manage Staffs
        protected new Staff GetStaffFromReader(IDataReader reader)
        {
            Staff p = new Staff();
            if (reader.HasColumn("StaffID") && reader["StaffID"]!=DBNull.Value)
            {
                p.StaffID = (long)reader["StaffID"];    
            }

            if (reader.HasColumn("CountryID") && reader["CountryID"] != DBNull.Value)
            {
                p.CountryID = reader["CountryID"] as long?;
            }

            if (reader.HasColumn("DeptID") && reader["DeptID"] != DBNull.Value)
            {
                p.DeptID = reader["DeptID"] as long?;
            }

            if (reader.HasColumn("StaffCatgID") && reader["StaffCatgID"] != DBNull.Value)
            {
                p.StaffCatgID = reader["StaffCatgID"] as long?;
            }

            if (reader.HasColumn("CityProvinceID") && reader["CityProvinceID"] != DBNull.Value)
            {
                p.CityProvinceID = reader["CityProvinceID"] as long?;
            }

            if (reader.HasColumn("RoleCode") && reader["RoleCode"] != DBNull.Value)
            {
                p.RoleCode = reader["RoleCode"] as long?;
            }

            if (reader.HasColumn("SFirstName") && reader["SFirstName"] != DBNull.Value)
            {
                p.SFirstName = reader["SFirstName"] as string;
            }

            if (reader.HasColumn("SMiddleName") && reader["SMiddleName"] != DBNull.Value)
            {
                p.SMiddleName = reader["SMiddleName"] as string;
            }

            if (reader.HasColumn("SLastName") && reader["SLastName"] != DBNull.Value)
            {
                p.SLastName = reader["SLastName"] as string;
            }

            if (reader.HasColumn("SDOB") && reader["SDOB"] != DBNull.Value)
            {
                p.SDOB = reader["SDOB"] as DateTime?;
            }

            if (reader.HasColumn("SBirthPlace") && reader["SBirthPlace"] != DBNull.Value)
            {
                p.SBirthPlace = reader["SBirthPlace"] as string;
            }

            if (reader.HasColumn("SIDNumber") && reader["SIDNumber"] != DBNull.Value)
            {
                p.SIDNumber = reader["SIDNumber"] as string;
            }

            if (reader.HasColumn("SPlaceOfIssue") && reader["SPlaceOfIssue"] != DBNull.Value)
            {
                p.SPlaceOfIssue = reader["SPlaceOfIssue"] as string;
            }

            if (reader.HasColumn("SStreetAddress") && reader["SStreetAddress"] != DBNull.Value)
            {
                p.SStreetAddress = reader["SStreetAddress"] as string;
            }

            if (reader.HasColumn("SSurburb") && reader["SSurburb"] != DBNull.Value)
            {
                p.SSurburb = reader["SSurburb"] as string;
            }

            if (reader.HasColumn("SState") && reader["SState"] != DBNull.Value)
            {
                p.SState = reader["SState"] as string;
            }

            if (reader.HasColumn("SPhoneNumber") && reader["SPhoneNumber"] != DBNull.Value)
            {
                p.SPhoneNumber = reader["SPhoneNumber"] as string;
            }

            if (reader.HasColumn("SMobiPhoneNumber") && reader["SMobiPhoneNumber"] != DBNull.Value)
            {
                p.SMobiPhoneNumber = reader["SMobiPhoneNumber"] as string;
            }

            if (reader.HasColumn("SEmailAddress") && reader["SEmailAddress"] != DBNull.Value)
            {
                p.SEmailAddress = reader["SEmailAddress"] as string;
            }

            if (reader.HasColumn("SEmployDate") && reader["SEmployDate"] != DBNull.Value)
            {
                p.SEmployDate = reader["SEmployDate"] as DateTime?;
            }

            if (reader.HasColumn("SLeftDate") && reader["SLeftDate"] != DBNull.Value)
            {
                p.SLeftDate = reader["SLeftDate"] as DateTime?;
            }

            if (reader.HasColumn("V_Religion") && reader["V_Religion"] != DBNull.Value)
            {
                p.V_Religion = reader["V_Religion"] as long?;
            }

            if (reader.HasColumn("V_MaritalStatus") && reader["V_MaritalStatus"] != DBNull.Value)
            {
                p.V_MaritalStatus = reader["V_MaritalStatus"] as long?;
            }

            if (reader.HasColumn("V_Ethnic") && reader["V_Ethnic"] != DBNull.Value)
            {
                p.V_Ethnic = reader["V_Ethnic"] as long?;
            }

            if (reader.HasColumn("SAccountNumber") && reader["SAccountNumber"] != DBNull.Value)
            {
                p.SAccountNumber = reader["SAccountNumber"] as string;
            }

            if (reader.HasColumn("V_BankName") && reader["V_BankName"] != DBNull.Value)
            {
                p.V_BankName = reader["V_BankName"] as long?;
            }

            if (reader.HasColumn("SEmploymentHistory") && reader["SEmploymentHistory"] != DBNull.Value)
            {
                p.SEmploymentHistory = reader["SEmploymentHistory"] as string;
            }

            if (reader.HasColumn("SImage") && reader["SImage"] != DBNull.Value)
            {
                p.SImage = reader["SImage"] as string;
            }

            if (reader.HasColumn("SCreateDate") && reader["SCreateDate"] != DBNull.Value)
            {
                p.SCreateDate = reader["SCreateDate"] as DateTime?;
            }

            if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
            {
                p.FullName = reader["FullName"] as string;
            }
            if (reader.HasColumn("StaffCatgID") && reader["StaffCatgID"] != DBNull.Value)
            {
                p.RefStaffCategory = new RefStaffCategory();
                p.StaffCatgID = (long)reader["StaffCatgID"];
                if (reader.HasColumn("V_StaffCatType") && reader["V_StaffCatType"] != DBNull.Value)
                {
                    p.RefStaffCategory.V_StaffCatType = (long)reader["V_StaffCatType"];
                }
                if (reader.HasColumn("StaffCatgDescription") && reader["StaffCatgDescription"] != DBNull.Value)
                {
                    p.RefStaffCategory.StaffCatgDescription = reader["StaffCatgDescription"].ToString();
                }
            }

            if (reader.HasColumn("AllowRegWithoutTicket") && reader["AllowRegWithoutTicket"] != DBNull.Value)
            {
                p.AllowRegWithoutTicket = (bool)reader["AllowRegWithoutTicket"];
            }

            return p;
        }

        protected new List<Staff> GetStaffCollectionFromReader(IDataReader reader)
        {
            List<Staff> retVal = new List<Staff>();
            while (reader.Read())
            {
                retVal.Add(GetStaffFromReader(reader));
            }
            return retVal;
        }

        
        #endregion


        #region Manage UserLoginHistory

        protected UserLoginHistory GetUserLoginHistoryFromReader(IDataReader reader)
        {
            UserLoginHistory p = new UserLoginHistory();
            return p;
        }

        protected List<UserLoginHistory> GetUserLoginHistoryCollectionFromReader(IDataReader reader)
        {
            List<UserLoginHistory> retVal = new List<UserLoginHistory>();
            while (reader.Read())
            {
                retVal.Add(GetUserLoginHistoryFromReader(reader));
            }
            return retVal;
        }

        #endregion




        public UserAccount AuthenticateUser(string userName, string password)
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

        public List<UserAccount> GetAllUsers()
        {
            throw new NotImplementedException();
        }
        public bool AddModule(DataEntities.Module newModule, out int ModuleID)
        {
            throw new NotImplementedException();
        }
        public bool AddUser(UserAccount newUser, out long UserID)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }
        public bool DeleteUserByID(long userID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteUserByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UserID", SqlDbType.BigInt, userID);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);

                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }
        public bool DeleteUserByName(string userName)
        {
            throw new NotImplementedException();
        }


        public UserAccount GetUserByID(long userID)
        {
            throw new NotImplementedException();
        }
        public bool UpdateUser(UserAccount info)
        {
            throw new NotImplementedException();
        }
        public List<UserAccount> SearchUsers(UserSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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
                {
                    totalCount = -1;
                }

                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public bool CheckIfUserNameExists(string userName)
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

                CleanUpConnectionAndCommand(cn, cmd);
                if (obj == null)
                    return false;

                return (int)obj > 0;
            }
        }
        public UserAccount GetUserWithGroupInfo(long userID)
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

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public void AssignUserToGroups(long userID, List<int> groupIDs)
        {
            //spAssignUserToGroups
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spAssignUserToGroups", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UserID", SqlDbType.Int, userID);
                string temp = "";
                if (groupIDs != null && groupIDs.Count > 0)
                {
                    temp = String.Join(",", groupIDs);
                }

                cmd.AddParameter("@GroupIDs", SqlDbType.VarChar, temp);
                cn.Open();
                ExecuteNonQuery(cmd);

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        #region Manage UserGroups
        public Group GetGroupWithRoleInfo(int groupID)
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

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public void AssignGroupToRoles(int groupID, List<int> roleIDs)
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

                CleanUpConnectionAndCommand(cn, cmd);
            }
        }

        public bool DeleteGroupByID(int groupID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteUserGroupByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@UserGroupID", SqlDbType.Int, groupID);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);

                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }
        public bool AddGroup(Group newGroup, out int GroupID)
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

                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }

        public bool CheckIfGroupNameExists(string groupName)
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

                CleanUpConnectionAndCommand(cn, cmd);
                if (obj == null)
                    return false;

                return (int)obj > 0;
            }
        }
        public List<Group> GetAllGroups()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllUserGroups", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<Group> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetUserGroupCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);

                return retVal;
            }
        }

        #endregion

        #region Manage Roles
        public bool AddRole(Role newRole, out int RoleID)
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

                CleanUpConnectionAndCommand(cn, cmd);

                return true;
            }
        }
        public bool CheckIfRoleNameExists(string roleName)
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

                CleanUpConnectionAndCommand(cn, cmd);
                if (obj == null)
                    return false;

                return (int)obj > 0;
            }
        }
        public bool DeleteRoleByID(int roleID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeleteRoleByID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RoleID", SqlDbType.Int, roleID);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);

                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }
        public List<Role> GetAllRoles()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllRoles", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<Role> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetRoleCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public Role GetRoleWithOperationInfo(int roleID)
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

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public void AssignRoleToOperations(int roleID, List<int> operationIDs)
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
                
                CleanUpConnectionAndCommand(cn, cmd);
            }
        }
        #endregion
        #region Manage Modules
        public List<DataEntities.Module> GetAllModules()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllModules", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<DataEntities.Module> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetModuleCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        #endregion

        #region Manage Operations
        public List<Operation> GetAllOperations()
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllOperations", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                List<Operation> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetOperationCollectionFromReader(reader);

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        #endregion

        #region Manage Staffs
        public  List<Staff> SearchStaffs(StaffSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        #endregion

        #region User Login
        public long AddUserLoginHistory(UserLoginHistory Ulh)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserLoginHistory_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AccountID", SqlDbType.BigInt, Ulh.AccountID);
                cmd.AddParameter("@HostName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Ulh.HostName));
                cmd.AddParameter("@HostIPV4", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Ulh.HostIPV4));
                cmd.AddParameter("@LoggedHistoryID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                ExecuteNonQuery(cmd);
                object o = cmd.Parameters["@LoggedHistoryID"].Value;
                if (o != DBNull.Value)
                {
                    return (long)o;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return 0;
            }
        }
        public  bool UpdateUserLoginHistory(UserLoginHistory Ulh)
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
                
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        public  bool DeleteUserLoginHistory(long LoggedHistoryID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserLoginHistory_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LoggedHistoryID", SqlDbType.BigInt, LoggedHistoryID);

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        #endregion

        #region User authorization
        public  List<UserSubAuthorization> GetUserSubAuthorizationPaging(UserSubAuthorization Usa, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
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

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public  UserSubAuthorization GetUserSubAuthorization(UserSubAuthorization Usa)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        public  bool AddUserSubAuthorization(UserSubAuthorization Usa)
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
                
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        public  bool UpdateUserSubAuthorization(UserSubAuthorization Usa)
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
                
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }
        public  bool DeleteUserSubAuthorization(long SubUserAuthorizationID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserSubAuthorizationDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@SubUserAuthorizationID", SqlDbType.BigInt, SubUserAuthorizationID);

                cn.Open();
                int retVal = ExecuteNonQuery(cmd);
                
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal > 0;
            }
        }

        #endregion
        #region User account official
        public long AddUserOfficialHistory(UserOfficialHistory Usa)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserOfficialHistoryInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@OfficialAccountID", SqlDbType.NVarChar, Usa.OfficialAccountID);
                cmd.AddParameter("@LoggedAccountID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Usa.LoggedAccountID));
                cmd.AddParameter("@LoggedHistoryID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Usa.LoggedHistoryID));
                cmd.AddParameter("@UOHistoryID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                ExecuteNonQuery(cmd);
                object o = cmd.Parameters["@UOHistoryID"].Value;
                if (o != DBNull.Value)
                {
                    return (long)o;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return 0;
            }
        }

        public List<UserOfficialHistory> GetUserOfficialHistoryPaging(long LoggedStaffID, int pageIndex, int pageSize
            , bool bCountTotal, out int totalCount)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserOfficialHistorySearch", cn);
                cmd.AddParameter("@LoggedStaffID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(LoggedStaffID));
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);

                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, null, ParameterDirection.Output);

                cn.Open();
                List<UserOfficialHistory> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetUserOfficialHistoryCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }

        public long AddManagementUserOfficial(ManagementUserOfficial Usa)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spManagementUserOfficialInsert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@LoginUserID", SqlDbType.NVarChar, Usa.LoginUserID);
                cmd.AddParameter("@UserOfficialID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Usa.UserOfficialID));
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Usa.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Usa.ToDate));
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(Usa.StaffID));
                cmd.AddParameter("@PatientFindBy", SqlDbType.Int, ConvertNullObjectToDBNull(Usa.PatientFindBy));

                cmd.AddParameter("@ManagementUserOfficialID", SqlDbType.BigInt, DBNull.Value, ParameterDirection.Output);
                cn.Open();
                ExecuteNonQuery(cmd);
                object o = cmd.Parameters["@ManagementUserOfficialID"].Value;
                if (o != DBNull.Value)
                {
                    return (long)o;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return 0;
            }
        }

        public bool DeleteManagementUserOfficial(long ManagementUserOfficialID, long StaffID)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spManagementUserOfficialDelete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@ManagementUserOfficialID", SqlDbType.BigInt, ManagementUserOfficialID);
                cmd.AddParameter("@StaffID", SqlDbType.BigInt, StaffID);

                cn.Open();

                int retVal = ExecuteNonQuery(cmd);

                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }

        public List<ManagementUserOfficial> GetManagementUserOfficialPaging(long LoginUserID, long UserOfficialID
            , int pageIndex, int pageSize
            , bool bCountTotal, out int totalCount)
        {
            using (SqlConnection cn = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spManagementUserOfficialSearch", cn);
                cmd.AddParameter("@LoginUserID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(LoginUserID));
                cmd.AddParameter("@UserOfficialID", SqlDbType.NVarChar, ConvertNullObjectToDBNull(UserOfficialID));
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@PageSize", SqlDbType.Int, pageSize);
                cmd.AddParameter("@PageIndex", SqlDbType.Int, pageIndex);

                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, bCountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, null, ParameterDirection.Output);

                cn.Open();
                List<ManagementUserOfficial> retVal = null;
                IDataReader reader = ExecuteReader(cmd);

                retVal = GetManagementUserOfficialCollectionFromReader(reader);
                reader.Close();

                if (bCountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    totalCount = (int)cmd.Parameters["@Total"].Value;
                }
                else
                    totalCount = -1;

                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return retVal;
            }
        }
        #endregion

    }
}
