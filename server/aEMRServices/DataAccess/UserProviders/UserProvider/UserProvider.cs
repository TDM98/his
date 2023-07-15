using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;

using System.Reflection;
using eHCMS.Configurations;
using eHCMS.Services.Core;

namespace eHCMS.DAL
{
    public abstract class UserProvider : DataProviderBase
    {
        static private UserProvider _instance = null;
        /// <summary>
        /// Returns an instance of the provider type specified in the config file (using reflection)
        /// </summary>
        static public UserProvider Instance
        {
            get
            {
                lock (typeof(UserProvider))
                {
                    if (_instance == null)
                    {
                        string tempPath = AppDomain.CurrentDomain.RelativeSearchPath;
                        if (string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                            tempPath = AppDomain.CurrentDomain.BaseDirectory;
                        else
                            tempPath = AppDomain.CurrentDomain.RelativeSearchPath;

                        string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Users.Assembly + ".dll");
                        Assembly assem = Assembly.LoadFrom(assemblyPath);
                        Type t = assem.GetType(Globals.Settings.Users.ProviderType);
                        _instance = (UserProvider)Activator.CreateInstance(t);
                    }
                }
                return _instance;
            }
        }

        public UserProvider()
        {
            this.ConnectionString = Globals.Settings.Patients.ConnectionString;

        }
        public abstract UserAccount GetUserByID(long userID);
        public abstract UserAccount AuthenticateUser(string userName, string password);
        public abstract UserAccount GetUserWithGroupInfo(long userID);
        public abstract bool DeleteUserByID(long userID);
        public abstract bool DeleteUserByName(string userName);
        public abstract bool UpdateUser(UserAccount info);
        public abstract bool AddUser(UserAccount newUser, out long UserID);
        public abstract bool CheckIfUserNameExists(string userName);
        public abstract List<UserAccount> GetAllUsers();
        public abstract List<UserAccount> SearchUsers(UserSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract void AssignUserToGroups(long userID, List<int> groupIDs);

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
        protected virtual DataEntities.Module GetModuleFromReader(IDataReader reader)
        {
            DataEntities.Module p = new DataEntities.Module();

            p.ModuleID = (int)reader["ModuleID"];
            p.ModuleName = reader["ModuleName"].ToString();
            p.Description = reader["Description"].ToString();

            return p;
        }

        protected virtual List<DataEntities.Module> GetModuleCollectionFromReader(IDataReader reader)
        {
            List<DataEntities.Module> retVal = new List<DataEntities.Module>();
            while (reader.Read())
            {
                retVal.Add(GetModuleFromReader(reader));
            }
            return retVal;
        }

        public abstract bool AddModule(DataEntities.Module newModule, out int ModuleID);
        public abstract List<DataEntities.Module> GetAllModules();
        #endregion
        #region Manage UserGroups

        public abstract bool DeleteGroupByID(int groupID);
        public abstract bool AddGroup(Group newGroup, out int GroupID);
        public abstract bool CheckIfGroupNameExists(string groupName);
        public abstract List<Group> GetAllGroups();
        public abstract Group GetGroupWithRoleInfo(int groupID);
        public abstract void AssignGroupToRoles(int groupID, List<int> roleIDs);

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

        public abstract bool DeleteRoleByID(int roleID);
        public abstract bool AddRole(Role newRole, out int RoleID);
        public abstract bool CheckIfRoleNameExists(string roleName);
        public abstract List<Role> GetAllRoles();

        public abstract Role GetRoleWithOperationInfo(int roleID);
        public abstract void AssignRoleToOperations(int roleID, List<int> operationIDs);

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

        public abstract List<Operation> GetAllOperations();
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

        public abstract List<Staff> SearchStaffs(StaffSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        #endregion


        #region Manage UserLoginHistory

        protected virtual UserLoginHistory GetUserLoginHistoryFromReader(IDataReader reader)
        {
            UserLoginHistory p = new UserLoginHistory();
            return p;
        }

        protected virtual List<UserLoginHistory> GetUserLoginHistoryCollectionFromReader(IDataReader reader)
        {
            List<UserLoginHistory> retVal = new List<UserLoginHistory>();
            while (reader.Read())
            {
                retVal.Add(GetUserLoginHistoryFromReader(reader));
            }
            return retVal;
        }

        public abstract bool AddUserLoginHistory(UserLoginHistory Ulh);
        public abstract bool UpdateUserLoginHistory(UserLoginHistory Ulh);
        public abstract bool DeleteUserLoginHistory(long LoggedHistoryID);
        //public abstract List<UserLoginHistory> SearchUserLoginHistorys(StaffSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        #endregion

        #region Manage UserSubAuthorization
        public abstract List<UserSubAuthorization> GetUserSubAuthorizationPaging(UserSubAuthorization Usa
            , int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract UserSubAuthorization GetUserSubAuthorization(UserSubAuthorization Usa);

        public abstract bool AddUserSubAuthorization(UserSubAuthorization Usa);
        public abstract bool UpdateUserSubAuthorization(UserSubAuthorization Usa);
        public abstract bool DeleteUserSubAuthorization(long SubUserAuthorizationID);
        //public abstract List<UserLoginHistory> SearchUserLoginHistorys(StaffSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        #endregion
    }
}
