/*
 * 20171409 #002 CMN: Added EmployeesReport
 * 20180409 #003 TTM: Kiem tra gia tri cua Certificate va SCode
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using eHCMS.Configurations;
using DataEntities;
using System.Data;
using eHCMS.Services.Core;
using System.Collections.ObjectModel;
using AxLogging;
namespace eHCMS.DAL
{
    public abstract class UserAccounts: DataProviderBase
    {
        static private UserAccounts _instance = null;
        static public UserAccounts Instance
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
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.ConfigurationManager.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type[] types = assem.GetExportedTypes();
                    Type t = assem.GetType(Globals.Settings.ConfigurationManager.userAccount.ProviderType);
                    _instance = (UserAccounts)Activator.CreateInstance(t);
                }
                return _instance;
                
            }
        }

        public UserAccounts()
        {
            this.ConnectionString = Globals.Settings.ConfigurationManager.ConnectionString;
        }

        public abstract List<Lookup> GetLookupOperationType();
        #region user account
        public abstract List<UserAccount> GetAllUserAccount(long AccountID);

        public abstract List<UserAccount> GetAllUserAccountsPaging(string AccountName, int PageSize, int PageIndex, string OrderBy,
                                                                   bool CountTotal, out int Total);
        protected virtual List<UserAccount> GetUserAccountCollectionFromReader(IDataReader reader)
        {
            List<UserAccount> lst = new List<UserAccount>();
            while (reader.Read())
            {
                lst.Add(GetUserAccountObjFromReader(reader));
            }
            return lst;
        }
        protected virtual UserAccount GetUserAccountObjFromReader(IDataReader reader)
        {
            UserAccount p = new UserAccount();
            try
            {
                if (reader.HasColumn("AccountID") && reader["AccountID"] != DBNull.Value)
                {
                    p.AccountID = (long)(reader["AccountID"]);
                }

                p.Staff = new Staff();

                if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
                {
                    p.Staff.FullName = reader["FullName"].ToString();
                }

		        if (reader.HasColumn("StaffID") && reader["StaffID"] != DBNull.Value)
                {
                    p.StaffID=(long)reader["StaffID"];
                }

                if (reader.HasColumn("AccountName") && reader["AccountName"] != DBNull.Value)
                {
                    p.AccountName = reader["AccountName"].ToString();
                }

                if (reader.HasColumn("AccountPassword") && reader["AccountPassword"] != DBNull.Value)
                {
                    p.AccountPassword = reader["AccountPassword"].ToString();
                }

                if (reader.HasColumn("IsActivated") && reader["IsActivated"] != DBNull.Value)
                {
                    p.IsActivated =(bool)reader["IsActivated"];
                }

            }
            catch 
            { return null; }
            return p;
        }

        public abstract bool AddNewUserAccount(long StaffID, string AccountName, string AccountPassword, bool IsActivated);
        public abstract bool UpdateUserAccount(int AccountID, long StaffID, string AccountName, string AccountPassword, bool IsActivated);
        public abstract bool DeleteUserAccount(int AccountID);        

        #endregion

        #region Group
        public abstract List<Group> GetAllGroupByGroupID(long GroupID);
        public abstract List<Group> GetAllGroupAllPaging(long GroupID, int PageSize, int PageIndex, string OrderBy,
                                                         bool CountTotal, out int Total);
        protected virtual List<Group> GetGroupCollectionFromReader(IDataReader reader)
        {
            List<Group> lst = new List<Group>();
            while (reader.Read())
            {
                lst.Add(GetGroupObjFromReader(reader));
            }
            return lst;
        }
        protected virtual Group GetGroupObjFromReader(IDataReader reader)
        {
            Group p = new Group();
            try
            {
                if (reader.HasColumn("GroupID") && reader["GroupID"] != DBNull.Value)
                {
                    p.GroupID = Convert.ToInt32(reader["GroupID"]);
                }

                if (reader.HasColumn("GroupName") && reader["GroupName"] != DBNull.Value)
                {
                    p.GroupName = reader["GroupName"].ToString();
                }

                if (reader.HasColumn("Description") && reader["Description"] != DBNull.Value)
                {
                    p.Description = reader["Description"].ToString();
                }
            }
            catch 
            { return null; }
            return p;
        }

        public abstract bool AddNewGroup(string GroupName, string Description);
        public abstract bool UpdateGroup(int GroupID, string GroupName, string Description);
        public abstract bool DeleteGroup(int GroupID);
        #endregion

        #region User Group 
        public abstract List<UserGroup> GetAllUserGroupGetByID(long AccountID, long GroupID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);
        protected virtual List<UserGroup> GetUserGroupCollectionFromReader(IDataReader reader)
        {
            List<UserGroup> lst = new List<UserGroup>();
            while (reader.Read())
            {
                lst.Add(GetUserGroupObjFromReader(reader));
            }
            return lst;
        }
        protected virtual UserGroup GetUserGroupObjFromReader(IDataReader reader)
        {
            UserGroup p = new UserGroup();
            try
            {
                
                if (reader.HasColumn("UGID") && reader["UGID"] != DBNull.Value)
                {
                    p.UGID = (long)(reader["UGID"]);
                }

                if (reader.HasColumn("AccountID") && reader["AccountID"] != DBNull.Value)
                {
                    p.AccountID = (long)reader["AccountID"];
                }

                if (reader.HasColumn("GroupID") && reader["GroupID"] != DBNull.Value)
                {
                    p.GroupID = Convert.ToInt32(reader["GroupID"]);
                }

                p.Group = new Group();

                if (reader.HasColumn("GroupName") && reader["GroupName"] != DBNull.Value)
                {
                    p.Group.GroupName = reader["GroupName"].ToString();
                }

                if (reader.HasColumn("Description") && reader["Description"] != DBNull.Value)
                {
                    p.Group.Description = reader["Description"].ToString();
                }


                p.UserAccount = new UserAccount();
                try 
                {
                    if (reader.HasColumn("StaffID") && reader["StaffID"] != DBNull.Value)
                    {
                        p.UserAccount.StaffID= (long?)(reader["StaffID"]);
                    }                    
           
                   if (reader.HasColumn("AccountName") && reader["AccountName"] != DBNull.Value)
                    {
                        p.UserAccount.AccountName= reader["AccountName"].ToString();
                    }
           
                   if (reader.HasColumn("AccountPassword") && reader["AccountPassword"] != DBNull.Value)
                    {
                        p.UserAccount.AccountPassword= reader["AccountPassword"].ToString();
                    }

                    if (reader.HasColumn("IsActivated") && reader["IsActivated"] != DBNull.Value)
                    {
                        p.UserAccount.IsActivated = (bool)(reader["IsActivated"]);
                    }
                   p.UserAccount.Staff = new Staff();


                   if (reader.HasColumn("StaffID") && reader["StaffID"] != DBNull.Value)
                   {
                       p.UserAccount.Staff.StaffID = (long)reader["StaffID"];
                   }

                   if (reader.HasColumn("CountryID") && reader["CountryID"] != DBNull.Value)
                   {
                       p.UserAccount.Staff.CountryID = reader["CountryID"] as long?;
                   }

                   if (reader.HasColumn("DeptID") && reader["DeptID"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.DeptID = reader["DeptID"] as long?;
                    }

                   if (reader.HasColumn("StaffCatgID") && reader["StaffCatgID"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.StaffCatgID = reader["StaffCatgID"] as long?;
                    }

                   if (reader.HasColumn("CityProvinceID") && reader["CityProvinceID"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.CityProvinceID = reader["CityProvinceID"] as long?;
                    }

                   if (reader.HasColumn("RoleCode") && reader["RoleCode"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.RoleCode = reader["RoleCode"] as long?;
                    }

                   if (reader.HasColumn("SFirstName") && reader["SFirstName"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SFirstName = reader["SFirstName"] as string;
                    }

                   if (reader.HasColumn("SMiddleName") && reader["SMiddleName"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SMiddleName = reader["SMiddleName"] as string;
                    }

                   if (reader.HasColumn("SLastName") && reader["SLastName"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SLastName = reader["SLastName"] as string;
                    }

                   if (reader.HasColumn("SDOB") && reader["SDOB"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SDOB = reader["SDOB"] as DateTime?;
                    }

                   if (reader.HasColumn("SBirthPlace") && reader["SBirthPlace"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SBirthPlace = reader["SBirthPlace"] as string;
                    }

                   if (reader.HasColumn("SIDNumber") && reader["SIDNumber"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SIDNumber = reader["SIDNumber"] as string;
                    }

                   if (reader.HasColumn("SPlaceOfIssue") && reader["SPlaceOfIssue"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SPlaceOfIssue = reader["SPlaceOfIssue"] as string;
                    }

                   if (reader.HasColumn("SStreetAddress") && reader["SStreetAddress"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SStreetAddress = reader["SStreetAddress"] as string;
                    }

                   if (reader.HasColumn("SSurburb") && reader["SSurburb"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SSurburb = reader["SSurburb"] as string;
                    }

                   if (reader.HasColumn("SState") && reader["SState"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SState = reader["SState"] as string;
                    }

                   if (reader.HasColumn("SPhoneNumber") && reader["SPhoneNumber"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SPhoneNumber = reader["SPhoneNumber"] as string;
                    }

                   if (reader.HasColumn("SMobiPhoneNumber") && reader["SMobiPhoneNumber"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SMobiPhoneNumber = reader["SMobiPhoneNumber"] as string;
                    }

                   if (reader.HasColumn("SEmailAddress") && reader["SEmailAddress"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SEmailAddress = reader["SEmailAddress"] as string;
                    }

                   if (reader.HasColumn("SEmployDate") && reader["SEmployDate"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SEmployDate = reader["SEmployDate"] as DateTime?;
                    }

                   if (reader.HasColumn("SLeftDate") && reader["SLeftDate"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SLeftDate = reader["SLeftDate"] as DateTime?;
                    }

                   if (reader.HasColumn("V_Religion") && reader["V_Religion"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.V_Religion = reader["V_Religion"] as long?;
                    }

                   if (reader.HasColumn("V_MaritalStatus") && reader["V_MaritalStatus"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.V_MaritalStatus = reader["V_MaritalStatus"] as long?;
                    }

                   if (reader.HasColumn("V_Ethnic") && reader["V_Ethnic"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.V_Ethnic = reader["V_Ethnic"] as long?;
                    }

                   if (reader.HasColumn("SAccountNumber") && reader["SAccountNumber"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SAccountNumber = reader["SAccountNumber"] as string;
                    }

                   if (reader.HasColumn("V_BankName") && reader["V_BankName"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.V_BankName = reader["V_BankName"] as long?;
                    }

                   if (reader.HasColumn("SEmploymentHistory") && reader["SEmploymentHistory"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SEmploymentHistory = reader["SEmploymentHistory"] as string;
                    }

                   if (reader.HasColumn("SImage") && reader["SImage"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SImage = reader["SImage"] as string;
                    }

                   if (reader.HasColumn("SCreateDate") && reader["SCreateDate"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.SCreateDate = reader["SCreateDate"] as DateTime?;
                    }

                   if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
                    {
                        p.UserAccount.Staff.FullName = reader["FullName"] as string;
                    }
                }catch
                {
                }
            }
            catch 
            { return null; }
            return p;
        }

        public abstract bool AddNewUserGroup(long AccountID, int GroupID);
        public abstract bool AddNewUserGroupXML(IList<UserGroup> lstUserGroup);
        public abstract bool UpdateUserGroup(long UGID, long AccountID, int GroupID);
        public abstract bool DeleteUserGroup(int UGID);

        public abstract List<refModule> GetAllUserGroupByAccountID(long AccountID);
        private int checkCountOperationEnum(int modulueEnum)
        {
            switch (modulueEnum)
            {
                case    2: 
                    return (int)oRegistrionEx.mCount;
                case    3: 
                    return (int)oConsultationEx.mCount;
                case    4:
                    return (int) oParaClinicalEx.mCount;
                case    5:
                    return (int) oPharmacyEx.mCount;
                case    6:
                    return (int)oTransaction_ManagementEx.mCount;
                case    7:
                    return (int) oConfigurationEx.mCount;
                case    8:
                    return (int)oSystem_ManagementEx.mCount;
                case    9:
                    return (int) oResourcesEx.mCount;
                case    10:
                    return (int) oResources_MaintenanceEx.mCount;
                case    11:
                    return (int) oAppointmentEx.mCount;
                case    12:
                    return (int) oUserAccountEx.mCount;
                
                case    14:
                    return (int) oKhoaDuocEx.mCount;
                //case    15:
                //    return (int) oTransaction;
                case    16:
                    return (int) oModuleGeneralEX.mCount;
                case    17:
                    return (int) oClinicManagementEx.mCount;
                case    18:
                    return (int) oCLSLaboratoryEx.mCount;
                case    19:
                    return (int) oCLSImagingEX.mCount;
                case    20:
                    return (int) oKhoPhongEx.mCount;
                case  21:
                    return (int)oAdmin.mCount;
                case 22:
                    return (int)oYVu_ManagementEx.mCount;
                default:
                    return 1;
            }
            
        }

        private int checkCountFunctionEnum(int modulueEnum)
        {
            switch (modulueEnum)
            {
                case 2:
                    return (int)ePatient.mCount;
                case 3:
                    return (int)eConsultation.mCount;
                case 4:
                    return (int)eParaClinical.mCount;
                case 5:
                    return (int)ePharmacy.mCount;
                case 6:
                    return (int)eTransaction_Management.mCount;
                case 7:
                    return (int)eConfiguration_Management.mCount;
                case 8:
                    return (int)eSystem_Management.mCount;
                case 9:
                    return (int)eResources.mCount;
                case 10:
                    return (int)eResources_Maintenance.mCount;
                case 11:
                    return (int)eAppointment_System.mCount;
                case 12:
                    return (int)eUserAccount.mCount;

                case 14:
                    return (int)eKhoaDuoc.mCount;
                //case    15:
                //    return (int) oTransaction;
                case 16:
                    return (int)eModuleGeneral.mCount;
                case 17:
                    return (int)eClinicManagement.mCount;
                case 18:
                    return (int)eCLSLaboratory.mCount;
                case 19:
                    return (int)eCLSImaging.mCount;
                case 20:
                    return (int)eKhoPhong.mCount;
                case 21:
                    return (int)eAdmin.mCount;
                case 22:
                    return (int)eYVu_Management.mCount;
                default:
                    return 1;
            }

        }

        protected virtual List<refModule> GetUserGroupExCollectionFromReader(IDataReader reader)
        {
            
            List<refModule> lstModule = new List<refModule>();
            List<string> lstModuleName = new List<string>();
            
            //--Khoi tao range cho list
            
            for (int i = 1; i <= (int)eModules.mCount; i++)
            {
                refModule rfm = new refModule();
                lstModule.Add(rfm);
            }
            while (reader.Read())
            {
                int mEnum = 0;
                int fEnum = 0;
                int oEnum = 0;
                #region modules
                if (reader.HasColumn("meNum") && reader["meNum"] != DBNull.Value)
                {
                    mEnum =Convert.ToInt32(reader["meNum"]) ;
                    if (reader.HasColumn("ModuleName") && reader["ModuleName"] != DBNull.Value)
                    {
                        if (lstModule[mEnum].mModule == null)
                        {
                            lstModule[mEnum].mModule = new DataEntities.Module();
                        }
                        lstModule[mEnum].mModule.ModuleName = reader["ModuleName"].ToString();

                        if (reader.HasColumn("ModuleID") && reader["ModuleID"] != DBNull.Value)
                        {
                            lstModule[mEnum].mModule.ModuleID = Convert.ToInt32(reader["ModuleID"]);                            
                        }

                        if (reader.HasColumn("mIdx") && reader["mIdx"] != DBNull.Value)
                        {
                            lstModule[mEnum].mModule.Idx = reader["mIdx"] as byte?;
                        }

                        //tao list function
                        if (lstModule[mEnum].lstFunction == null)
                        {
                            lstModule[mEnum].lstFunction = new ObservableCollection<refFunction>();
                        }                        
                    }
                }
                #endregion

                #region function
                if (reader.HasColumn("feNum") && reader["feNum"] != DBNull.Value)
                {
                    fEnum = Convert.ToInt32(reader["feNum"]);
                    int count = checkCountFunctionEnum(mEnum)+1;
                    if (lstModule[mEnum].lstFunction.Count<1)
                    {
                        for (int i = 1; i <= count; i++)
                        {
                            refFunction rff = new refFunction();
                            lstModule[mEnum].lstFunction.Add(rff);
                        }
                    }

                    if (reader.HasColumn("FunctionName") && reader["FunctionName"] != DBNull.Value)
                    {
                        if (lstModule[mEnum].lstFunction[fEnum]==null)
                        {
                            lstModule[mEnum].lstFunction[fEnum] = new refFunction();
                        }
                        if (lstModule[mEnum].lstFunction[fEnum].mFunction == null)
                        {
                            lstModule[mEnum].lstFunction[fEnum].mFunction = new Function();
                        }
                        lstModule[mEnum].lstFunction[fEnum].mFunction.FunctionName = reader["FunctionName"].ToString();


                        //tao list operation
                        if (lstModule[mEnum].lstFunction[fEnum].lstOperation == null)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation = new ObservableCollection<refOperation>();
                        }
                    }
                }
                #endregion 

                #region operation
                if (reader.HasColumn("oeNum") && reader["oeNum"] != DBNull.Value)
                {
                    oEnum = Convert.ToInt32(reader["oeNum"]);
                    if (lstModule[mEnum].lstFunction[fEnum].lstOperation.Count < 1)
                    {
                        int count = checkCountOperationEnum(mEnum);
                        for (int i = 1; i <= count; i++)
                        {
                            refOperation rfo = new refOperation();
                            lstModule[mEnum].lstFunction[fEnum].lstOperation.Add(rfo);
                        }
                    }

                    if (reader.HasColumn("OperationName") && reader["OperationName"] != DBNull.Value)
                    {
                        if (lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mOperation == null)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mOperation = new Operation();
                        }
                        lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mOperation.OperationName = reader["OperationName"].ToString();
                        lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission = new Permission();
                        
                        if (reader.HasColumn("PermissionItemID") && reader["PermissionItemID"] != DBNull.Value)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.PermissionItemID = Convert.ToInt32(reader["PermissionItemID"]);
                        }

                        if (reader.HasColumn("PermissionFullControl") && reader["PermissionFullControl"] != DBNull.Value
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pFullControl==false)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pFullControl = (bool)(reader["PermissionFullControl"]);
                        }

                        if (reader.HasColumn("PermissionView") && reader["PermissionView"] != DBNull.Value
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pView==false)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pView = (bool)(reader["PermissionView"]);
                        }

                        if (reader.HasColumn("PermissionAdd") && reader["PermissionAdd"] != DBNull.Value
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pAdd==false)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pAdd = (bool)(reader["PermissionAdd"]);
                        }

                        if (reader.HasColumn("PermissionUpdate") && reader["PermissionUpdate"] != DBNull.Value
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pUpdate==false)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pUpdate = (bool)(reader["PermissionUpdate"]);
                        }

                        if (reader.HasColumn("PermissionDelete") && reader["PermissionDelete"] != DBNull.Value
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pDelete==false)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pDelete = (bool)(reader["PermissionDelete"]);
                        }

                        if (reader.HasColumn("PermissionReport") && reader["PermissionReport"] != DBNull.Value
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pReport==false)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pReport = (bool)(reader["PermissionReport"]);
                        }

                        if (reader.HasColumn("PermissionPrint") && reader["PermissionPrint"] != DBNull.Value
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pPrint==false)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pPrint = (bool)(reader["PermissionPrint"]);
                        }

                        if (reader.HasColumn("PermissionItemID") && reader["PermissionItemID"] != DBNull.Value)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.PermissionItemID = Convert.ToInt32(reader["PermissionItemID"]);
                        }
                    }
                }
                #endregion
                //lstModule.Add(GetUserGroupExObjFromReader(reader));
            }
            return lstModule;
        }
        protected virtual refModule GetUserGroupExObjFromReader(IDataReader reader)
        {
            refModule p = new refModule();
            try
            {

                //if (reader.HasColumn("UGID") && reader["UGID"] != DBNull.Value)
                //{
                //    p.UGID = (long)(reader["UGID"]);
                //}

                //refModule p = new refModule();

            }
            catch 
            { return null; }
            return p;
        }
        #endregion

        #region Group Role
        public abstract List<GroupRole> GetAllGroupRolesGetByID(long GroupID, long RoleID
                                        , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);
        protected virtual List<GroupRole> GetGroupRoleCollectionFromReader(IDataReader reader)
        {
            List<GroupRole> lst = new List<GroupRole>();
            while (reader.Read())
            {
                lst.Add(GetGroupRoleObjFromReader(reader));
            }
            return lst;
        }
        protected virtual GroupRole GetGroupRoleObjFromReader(IDataReader reader)
        {
            GroupRole p = new GroupRole();
            try
            {
                if (reader.HasColumn("GroupRoleID") && reader["GroupRoleID"] != DBNull.Value)
                {
                    p.GroupRoleID = Convert.ToInt32(reader["GroupRoleID"]);
                }

                if (reader.HasColumn("GroupID") && reader["GroupID"] != DBNull.Value)
                {
                    p.GroupID = Convert.ToInt32(reader["GroupID"]);
                }
                p.Group = new Group();
                if (reader.HasColumn("GroupName") && reader["GroupName"] != DBNull.Value)
                {
                    p.Group.GroupName = reader["GroupName"].ToString();
                }

                if (reader.HasColumn("gDescription") && reader["gDescription"] != DBNull.Value)
                {
                    p.Group.Description = reader["gDescription"].ToString();
                }

                
                p.Role =new Role();

                if (reader.HasColumn("RoleID") && reader["RoleID"] != DBNull.Value)
                {
                    p.RoleID = Convert.ToInt32(reader["RoleID"]);
                    p.Role.RoleID = Convert.ToInt32(reader["RoleID"]);
                }

                if (reader.HasColumn("RoleName") && reader["RoleName"] != DBNull.Value)
                {
                    p.Role.RoleName = reader["RoleName"].ToString();
                }

                if (reader.HasColumn("Description") && reader["Description"] != DBNull.Value)
                {
                    p.Role.Description = reader["Description"].ToString();
                }
                
            }
            catch 
            { return null; }
            return p;
        }


        public abstract bool AddNewGroupRoles(int GroupID, int RoleID);

        public abstract bool AddNewGroupRolesXML(IList<GroupRole> lstGroupRole);

        public abstract bool UpdateGroupRoles(long GroupRoleID, int GroupID, int RoleID);
        
        public abstract bool DeleteGroupRoles(long GroupRoleID);
        
        #endregion

        #region Permission
        public abstract List<Permission> GetAllPermissions_GetByID(long RoleID, long OperationID
            , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);
        protected virtual List<Permission> GetPermissionCollectionFromReader(IDataReader reader)
        {
            List<Permission> lst = new List<Permission>();
            while (reader.Read())
            {
                lst.Add(GetPermissionObjFromReader(reader));
            }
            return lst;
        }
        protected virtual Permission GetPermissionObjFromReader(IDataReader reader)
        {
            Permission p = new Permission();
            try
            {
                if (reader.HasColumn("PermissionItemID") && reader["PermissionItemID"] != DBNull.Value)
                {
                    p.PermissionItemID = Convert.ToInt32(reader["PermissionItemID"]);
                }

                if (reader.HasColumn("PermissionFullControl") && reader["PermissionFullControl"] != DBNull.Value && p.pFullControl == false)
                {
                    p.pFullControl = (bool)(reader["PermissionFullControl"]);
                }

                if (reader.HasColumn("PermissionView") && reader["PermissionView"] != DBNull.Value && p.pView == false)
                {
                    p.pView = (bool)(reader["PermissionView"]);
                }

                if (reader.HasColumn("PermissionAdd") && reader["PermissionAdd"] != DBNull.Value && p.pAdd == false)
                {
                    p.pAdd = (bool)(reader["PermissionAdd"]);
                }

                if (reader.HasColumn("PermissionUpdate") && reader["PermissionUpdate"] != DBNull.Value && p.pUpdate == false)
                {
                    p.pUpdate = (bool)(reader["PermissionUpdate"]);
                }

                if (reader.HasColumn("PermissionDelete") && reader["PermissionDelete"] != DBNull.Value && p.pDelete == false)
                {
                    p.pDelete = (bool)(reader["PermissionDelete"]);
                }

                if (reader.HasColumn("PermissionReport") && reader["PermissionReport"] != DBNull.Value && p.pReport == false)
                {
                    p.pReport = (bool)(reader["PermissionReport"]);
                }

                if (reader.HasColumn("PermissionPrint") && reader["PermissionPrint"] != DBNull.Value && p.pPrint == false)
                {
                    p.pPrint = (bool)(reader["PermissionPrint"]);
                }

                if (reader.HasColumn("PermissionItemID") && reader["PermissionItemID"] != DBNull.Value)
                {
                    p.PermissionItemID = Convert.ToInt32(reader["PermissionItemID"]);
                }

                p.Role = new Role();
                p.Operation = new Operation();

                if (reader.HasColumn("OperationID") && reader["OperationID"] != DBNull.Value)
                {
                    p.OperationID = Convert.ToInt32(reader["OperationID"]);
                    p.Operation.OperationID = Convert.ToInt32(reader["OperationID"]);
                }
                
                if (reader.HasColumn("RoleID") && reader["RoleID"] != DBNull.Value)
                {
                    p.RoleID = Convert.ToInt32(reader["RoleID"]);
                    p.Role.RoleID = Convert.ToInt32(reader["RoleID"]);
                }

                if (reader.HasColumn("OperationName") && reader["OperationName"] != DBNull.Value)
                {
                    p.Operation.OperationName = reader["OperationName"].ToString();
                }

                if (reader.HasColumn("Description") && reader["Description"] != DBNull.Value)
                {
                    p.Operation.Description = reader["Description"].ToString();
                }

                p.Operation.Function = new Function();
                if (reader.HasColumn("FunctionID") && reader["FunctionID"] != DBNull.Value)
                {
                    p.Operation.FunctionID =Convert.ToInt32(reader["FunctionID"]);
                    p.Operation.Function.FunctionID = Convert.ToInt32(reader["FunctionID"]);
                }

                if (reader.HasColumn("FunctionID") && reader["FunctionID"] != DBNull.Value)
                {
                    p.Operation.Function.FunctionID = Convert.ToInt32(reader["FunctionID"]);
                }

                if (reader.HasColumn("FunctionName") && reader["FunctionName"] != DBNull.Value)
                {
                    p.Operation.Function.FunctionName = reader["FunctionName"].ToString();
                }

                if (reader.HasColumn("FunctionDescription") && reader["FunctionDescription"] != DBNull.Value)
                {
                    p.Operation.Function.FunctionDescription = reader["FunctionDescription"].ToString();
                }

                if (reader.HasColumn("fIdx") && reader["fIdx"] != DBNull.Value)
                {
                    p.Operation.Function.Idx = reader["fIdx"] as byte?;
                }

                p.Operation.Function.Module = new DataEntities.Module();
                if (reader.HasColumn("ModuleID") && reader["ModuleID"] != DBNull.Value)
                {
                    p.Operation.Function.ModuleID = reader["ModuleID"] as int?;
                    p.Operation.Function.Module.ModuleID =Convert.ToInt32(reader["ModuleID"]);
                }

                if (reader.HasColumn("ModuleName") && reader["ModuleName"] != DBNull.Value)
                {
                    p.Operation.Function.Module.ModuleName = (reader["ModuleName"].ToString());
                }

                if (reader.HasColumn("mDescription") && reader["mDescription"] != DBNull.Value)
                {
                    p.Operation.Function.Module.Description = (reader["mDescription"].ToString());
                }

                if (reader.HasColumn("mIdx") && reader["mIdx"] != DBNull.Value)
                {
                    p.Operation.Function.Module.Idx = reader["mIdx"] as byte?;
                }
            }
            catch 
            { return null; }
            return p;
        }
        #endregion

        #region Modules
        public abstract List<DataEntities.Module> GetAllModules();
    
        public abstract bool AddNewModules(string ModuleName,int eNum ,string Description, int Idx);

        public abstract bool UpdateModules(long ModuleID, int eNum, string ModuleName, string Description, int Idx);
        public abstract bool UpdateModulesEnum(long ModuleID, int eNum);

        public abstract bool DeleteModules(long ModuleID);
        protected virtual List<DataEntities.Module> GetModuleCollectionFromReader(IDataReader reader)
        {
            List<DataEntities.Module> lst = new List<DataEntities.Module>();
            while (reader.Read())
            {
                lst.Add(GetModuleObjFromReader(reader));
            }
            return lst;
        }
        protected virtual DataEntities.Module GetModuleObjFromReader(IDataReader reader)
        {
            DataEntities.Module p = new DataEntities.Module();
            try
            {
                if (reader.HasColumn("ModuleID") && reader["ModuleID"] != DBNull.Value)
                {
                    p.ModuleID = Convert.ToInt32(reader["ModuleID"]);
                }

                if (reader.HasColumn("ModuleName") && reader["ModuleName"] != DBNull.Value)
                {
                    p.ModuleName = reader["ModuleName"].ToString();
                }
                if (reader.HasColumn("eNum") && reader["eNum"] != DBNull.Value)
                {
                    p.eNum = Convert.ToInt32(reader["eNum"]);
                }

                if (reader.HasColumn("Description") && reader["Description"] != DBNull.Value)
                {
                    p.Description = reader["Description"].ToString();
                }

                if (reader.HasColumn("Idx") && reader["Idx"] != DBNull.Value)
                {
                    p.Idx = reader["Idx"] as byte?;
                }
            }
            catch 
            { return null; }
            return p;
        }

        #endregion

        #region Functions
        public abstract List<Function> GetAllFunction(long ModuleID);

        public abstract List<Function> GetAllFunctionsPaging(long ModuleID
                                                             , int PageSize, int PageIndex, string OrderBy,
                                                             bool CountTotal, out int Total);
        public abstract bool AddNewFunctions(int ModuleID,int eNum, string FunctionName, string FunctionDescription, int Idx);
        public abstract bool UpdateFunctions(long FunctionID, int eNum, int ModuleID, string FunctionName, string FunctionDescription, byte Idx);
        public abstract bool UpdateFunctionsEnum(long FunctionID, int eNum);
        public abstract bool DeleteFunctions(long FunctionID);
        protected virtual List<Function> GetFunctionCollectionFromReader(IDataReader reader)
        {
            List<Function> lst = new List<Function>();
            while (reader.Read())
            {
                lst.Add(GetFunctionObjFromReader(reader));
            }
            return lst;
        }
        protected virtual Function GetFunctionObjFromReader(IDataReader reader)
        {
            Function p = new DataEntities.Function();
            try
            {

                if (reader.HasColumn("FunctionID") && reader["FunctionID"] != DBNull.Value)
                {
                    p.FunctionID = Convert.ToInt32(reader["FunctionID"]);
                }

                if (reader.HasColumn("feNum") && reader["feNum"] != DBNull.Value)
                {
                    p.eNum = Convert.ToInt32(reader["feNum"]);
                }

                if (reader.HasColumn("FunctionName") && reader["FunctionName"] != DBNull.Value)
                {
                    p.FunctionName = reader["FunctionName"].ToString();
                }
                
                if (reader.HasColumn("FunctionDescription") && reader["FunctionDescription"] != DBNull.Value)
                {
                    p.FunctionDescription = reader["FunctionDescription"].ToString();
                }

                if (reader.HasColumn("fIdx") && reader["fIdx"] != DBNull.Value)
                {
                    p.Idx = (reader["fIdx"] as byte?);
                }

                p.Module = new DataEntities.Module();

                if (reader.HasColumn("ModuleID") && reader["ModuleID"] != DBNull.Value)
                {
                    p.Module.ModuleID = Convert.ToInt32(reader["ModuleID"]);
                    p.ModuleID = Convert.ToInt32(reader["ModuleID"]);
                }

                if (reader.HasColumn("ModuleName") && reader["ModuleName"] != DBNull.Value)
                {
                    p.Module.ModuleName = reader["ModuleName"].ToString();
                }

                if (reader.HasColumn("meNum") && reader["meNum"] != DBNull.Value)
                {
                    p.Module.eNum = Convert.ToInt32(reader["meNum"]);
                }

                if (reader.HasColumn("Description") && reader["Description"] != DBNull.Value)
                {
                    p.Module.Description = reader["Description"].ToString();
                }

                if (reader.HasColumn("mIdx") && reader["mIdx"] != DBNull.Value)
                {
                    p.Module.Idx = reader["mIdx"] as byte?;
                }
            }
            catch 
            { return null; }
            return p;
        }

        #endregion

        #region Operations
        public abstract List<Operation> GetAllOperations(long OperationID);
        public abstract List<Operation> GetAllOperationsByFuncID(long FunctionID);
        // 20180921 TNHX: Add method for get all detail Modules Tree
        public abstract List<Um_ModuleFunctionOperations> GetAllDetails_ModuleFunctionOperations();
        public abstract List<Operation> GetAllOperationsByFuncIDPaging(long FunctionID
                                                                       , int PageSize, int PageIndex, string OrderBy,
                                                                       bool CountTotal, out int Total);
        public abstract bool AddNewOperations(long FunctionID, string OperationName, string Description);
        public abstract bool UpdateOperations(long OperationID, int Enum, string OperationName, string Description);
        public abstract bool DeleteOperations(long OperationID);
        protected virtual List<Operation> GetOperationCollectionFromReader(IDataReader reader)
        {
            List<Operation> lst = new List<Operation>();
            while (reader.Read())
            {
                lst.Add(GetOperationObjFromReader(reader));
            }
            return lst;
        }
        protected virtual Operation GetOperationObjFromReader(IDataReader reader)
        {
            Operation p = new Operation();
            try
            {
                if (reader.HasColumn("OperationID") && reader["OperationID"] != DBNull.Value)
                {
                    p.OperationID = Convert.ToInt32(reader["OperationID"]);
                }

                if (reader.HasColumn("OperationName") && reader["OperationName"] != DBNull.Value)
                {
                    p.OperationName = reader["OperationName"].ToString();
                }

                if (reader.HasColumn("oDescription") && reader["oDescription"] != DBNull.Value)
                {
                    p.Description = reader["oDescription"].ToString();
                }

                if (reader.HasColumn("oeNum") && reader["oeNum"] != DBNull.Value)
                {
                    p.Enum = Convert.ToInt32(reader["oeNum"]);
                }

                p.Function = new Function();
                p.Function.Module = new DataEntities.Module();

                if (reader.HasColumn("FunctionID") && reader["FunctionID"] != DBNull.Value)
                {
                    p.FunctionID = Convert.ToInt32(reader["FunctionID"]);
                    p.Function.FunctionID = Convert.ToInt32(reader["FunctionID"]);
                }

                if (reader.HasColumn("FunctionName") && reader["FunctionName"] != DBNull.Value)
                {
                    p.Function.FunctionName = reader["FunctionName"].ToString();
                }

                if (reader.HasColumn("feNum") && reader["feNum"] != DBNull.Value)
                {
                    p.Function.eNum = Convert.ToInt32(reader["feNum"]);
                }

                if (reader.HasColumn("FunctionDescription") && reader["FunctionDescription"] != DBNull.Value)
                {
                    p.Function.FunctionDescription = reader["FunctionDescription"].ToString();
                }

                if (reader.HasColumn("fIdx") && reader["fIdx"] != DBNull.Value)
                {
                    p.Function.Idx = (reader["fIdx"] as byte?);
                }

                if (reader.HasColumn("ModuleID") && reader["ModuleID"] != DBNull.Value)
                {
                    p.Function.Module.ModuleID = Convert.ToInt32(reader["ModuleID"]);
                    p.Function.ModuleID = Convert.ToInt32(reader["ModuleID"]);
                }

                if (reader.HasColumn("ModuleName") && reader["ModuleName"] != DBNull.Value)
                {
                    p.Function.Module.ModuleName = reader["ModuleName"].ToString();
                }

                if (reader.HasColumn("meNum") && reader["meNum"] != DBNull.Value)
                {
                    p.Function.Module.eNum = Convert.ToInt32(reader["meNum"]);                    
                }

                if (reader.HasColumn("mDescription") && reader["mDescription"] != DBNull.Value)
                {
                    p.Function.Module.Description = reader["mDescription"].ToString();
                }

                if (reader.HasColumn("mIdx") && reader["mIdx"] != DBNull.Value)
                {
                    p.Function.Module.Idx = reader["mIdx"] as byte?;
                }
            }
            catch 
            { return null; }
            return p;
        }

        #endregion

        #region Permission
        
        public abstract List<Permission> GetAllPermissions(long PermissionItemID);
        public abstract bool AddNewPermissions(int RoleID, long OperationID
                                                , bool FullControl
                                                , bool pView
                                                , bool pAdd
                                                , bool pUpdate
                                                , bool pDelete
                                                , bool pReport
                                                , bool pPrint);

        public abstract bool UpdatePermissionsFull(long PermissionItemID
                                                   , bool FullControl
                                                   , bool pView
                                                   , bool pAdd
                                                   , bool pUpdate
                                                   , bool pDelete
                                                   , bool pReport
                                                   , bool pPrint);
        public abstract bool UpdatePermissions(long PermissionItemID ,int RoleID ,long OperationID );
        public abstract bool DeletePermissions(long PermissionItemID);
        protected virtual List<Permission> GetPermissionCollectionFromReaderEx(IDataReader reader)
        {
            List<Permission> lst = new List<Permission>();
            while (reader.Read())
            {
                lst.Add(GetPermissionObjFromReaderEx(reader));
            }
            return lst;
        }
        protected virtual Permission GetPermissionObjFromReaderEx(IDataReader reader)
        {
            Permission p = new Permission();
            try
            {
                p.Role = new Role();
                p.Operation = new Operation();
                if (reader.HasColumn("OperationID") && reader["OperationID"] != DBNull.Value)
                {
                    p.OperationID = Convert.ToInt32(reader["OperationID"]);
                    p.Operation.OperationID = Convert.ToInt32(reader["OperationID"]);
                }

                if (reader.HasColumn("RoleID") && reader["RoleID"] != DBNull.Value)
                {
                    p.RoleID = Convert.ToInt32(reader["RoleID"]);
                    p.Role.RoleID = Convert.ToInt32(reader["RoleID"]);
                }

                if (reader.HasColumn("OperationName") && reader["OperationName"] != DBNull.Value)
                {
                    p.Operation.OperationName = reader["OperationName"].ToString();
                }

                if (reader.HasColumn("Description") && reader["Description"] != DBNull.Value)
                {
                    p.Operation.Description = reader["Description"].ToString();
                }

                p.Operation.Function = new Function();
                p.Operation.Function.Module = new DataEntities.Module();

                if (reader.HasColumn("FunctionID") && reader["FunctionID"] != DBNull.Value)
                {
                    p.Operation.FunctionID = Convert.ToInt32(reader["FunctionID"]);
                    p.Operation.Function.FunctionID = Convert.ToInt32(reader["FunctionID"]);
                }

                if (reader.HasColumn("FunctionName") && reader["FunctionName"] != DBNull.Value)
                {
                    p.Operation.Function.FunctionName = reader["FunctionName"].ToString();
                }

                if (reader.HasColumn("FunctionDescription") && reader["FunctionDescription"] != DBNull.Value)
                {
                    p.Operation.Function.FunctionDescription = reader["FunctionDescription"].ToString();
                }

                if (reader.HasColumn("fIdx") && reader["fIdx"] != DBNull.Value)
                {
                    p.Operation.Function.Idx = (reader["fIdx"] as byte?);
                }

                if (reader.HasColumn("ModuleID") && reader["ModuleID"] != DBNull.Value)
                {
                    p.Operation.Function.Module.ModuleID = Convert.ToInt32(reader["ModuleID"]);
                    p.Operation.Function.ModuleID = Convert.ToInt32(reader["ModuleID"]);
                }

                if (reader.HasColumn("ModuleName") && reader["ModuleName"] != DBNull.Value)
                {
                    p.Operation.Function.Module.ModuleName = reader["ModuleName"].ToString();
                }

                if (reader.HasColumn("Description") && reader["Description"] != DBNull.Value)
                {
                    p.Operation.Function.Module.Description = reader["Description"].ToString();
                }

                if (reader.HasColumn("mIdx") && reader["mIdx"] != DBNull.Value)
                {
                    p.Operation.Function.Module.Idx = reader["mIdx"] as byte?;
                }
            }
            catch 
            { return null; }
            return p;
        }

        #endregion

        #region Role
        public abstract List<Role> GetAllRoles();

        public abstract List<Role> GetAllRolesAllPaging(long RoleID, int PageSize, int PageIndex, string OrderBy,
                                                        bool CountTotal, out int Total);
        public abstract bool AddNewRoles(string RoleName, string Description);
        public abstract bool UpdateRoles(int RoleID, string RoleName, string Description);
        public abstract bool DeleteRoles(int RoleID);
        protected virtual List<Role> GetRoleCollectionFromReader(IDataReader reader)
        {
            List<Role> lst = new List<Role>();
            while (reader.Read())
            {
                lst.Add(GetRoleObjFromReader(reader));
            }
            return lst;
        }
        protected virtual Role GetRoleObjFromReader(IDataReader reader)
        {
            Role p = new Role();
            try
            {
                if (reader.HasColumn("RoleID") && reader["RoleID"] != DBNull.Value)
                {
                    p.RoleID = Convert.ToInt32(reader["RoleID"]);
                }

                if (reader.HasColumn("RoleName") && reader["RoleName"] != DBNull.Value)
                {
                    p.RoleName = reader["RoleName"].ToString();
                }

                if (reader.HasColumn("Description") && reader["Description"] != DBNull.Value)
                {
                    p.Description = reader["Description"].ToString();
                }

                
            }
            catch
            { return null; }
            return p;
        }

        #endregion

        #region Manage User Login History

        protected virtual UserLoginHistory GetUserLoginHistoryFromReader(IDataReader reader)
        {
            UserLoginHistory p = new UserLoginHistory();
            p.UserAccount = new UserAccount();
            try
            {
                if (reader.HasColumn("LoggedHistoryID") && reader["LoggedHistoryID"] != DBNull.Value)
                {
                    p.LoggedHistoryID = Convert.ToInt32(reader["LoggedHistoryID"]);
                }
                if (reader.HasColumn("AccountID") && reader["AccountID"] != DBNull.Value)
                {
                    p.AccountID = Convert.ToInt32(reader["AccountID"]);
                    p.UserAccount.AccountID = p.AccountID;
                }

                if (reader.HasColumn("LoggedDateTime") && reader["LoggedDateTime"] != DBNull.Value)
                {
                    p.LogDateTime = (DateTime)reader["LoggedDateTime"];
                }

                if (reader.HasColumn("HostName") && reader["HostName"] != DBNull.Value)
                {
                    p.HostName = reader["HostName"].ToString();
                }

                if (reader.HasColumn("HostIPV4") && reader["HostIPV4"] != DBNull.Value)
                {
                    p.HostIPV4 = reader["HostIPV4"].ToString();
                }
                
                if (reader.HasColumn("AccountName") && reader["AccountName"] != DBNull.Value)
                {
                    p.UserAccount.AccountName = reader["AccountName"].ToString();
                }
            }
            catch 
            { return null; }
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

        public abstract List<UserLoginHistory> GetUserLogHisGByUserAccount();
        public abstract List<UserLoginHistory> GetUserLogHisGByHostName();

        public abstract List<UserLoginHistory> GetAllUserLoginHistoryPaging(UserLoginHistory ulh, int PageSize,
                                                                            int PageIndex, string OrderBy,
                                                                            bool CountTotal, out int Total);
        public abstract bool AddUserLoginHistory(UserLoginHistory Ulh);
        public abstract bool UpdateUserLoginHistory(UserLoginHistory Ulh);
        public abstract bool DeleteUserLoginHistory(long LoggedHistoryID);
        

        #endregion

#region staff

        public abstract Staff GetStaffsByID(long StaffID);
        public abstract List<Staff> GetAllStaffAllPaging(Staff searchStaff, int PageSize, int PageIndex, string OrderBy,
                                                         bool CountTotal, out int Total);
        public abstract bool AddNewStaff(Staff newStaff);
        public abstract bool UpdateStaff(Staff newStaff);
        public abstract bool DeleteStaff(int StaffID);
        protected virtual Staff GetStaffsFromReader(IDataReader reader)
        {
            Staff p = new Staff();
            p.CitiesProvince = new CitiesProvince();
            p.RefCountry = new RefCountry();
            p.RefStaffCategory = new RefStaffCategory();
            p.Ethnic = new Lookup();
            p.MaritalStatus = new Lookup();
            p.Religion = new Lookup();
            p.RefDepartment = new RefDepartment();
            p.BankName=new Lookup();
            try
            {
                if (reader.HasColumn("StaffID") && reader["StaffID"] != DBNull.Value)
                {
                    p.StaffID = Convert.ToInt32(reader["StaffID"]);
                }
                
                if (reader.HasColumn("CountryID") && reader["CountryID"] != DBNull.Value)
                {
                    p.CountryID = Convert.ToInt32(reader["CountryID"]);
                    p.RefCountry.CountryID = (long)p.CountryID;
                }
                
                if (reader.HasColumn("DeptID") && reader["DeptID"] != DBNull.Value)
                {
                    p.DeptID = Convert.ToInt32(reader["DeptID"]);
                    p.RefDepartment.DeptID =(long)p.DeptID;
                }
                
                if (reader.HasColumn("StaffCatgID") && reader["StaffCatgID"] != DBNull.Value)
                {
                    p.StaffCatgID = Convert.ToInt32(reader["StaffCatgID"]);
                    p.RefStaffCategory.StaffCatgID = (long)p.StaffCatgID;
                }
                
                if (reader.HasColumn("CityProvinceID") && reader["CityProvinceID"] != DBNull.Value)
                {
                    p.CityProvinceID = Convert.ToInt32(reader["CityProvinceID"]);
                    p.CitiesProvince.CityProvinceID = (long)p.CityProvinceID;
                }
                
                if (reader.HasColumn("RoleCode") && reader["RoleCode"] != DBNull.Value)
                {
                    p.RoleCode = Convert.ToInt32(reader["RoleCode"]);
                }
                
                if (reader.HasColumn("SFirstName") && reader["SFirstName"] != DBNull.Value)
                {
                    p.SFirstName = reader["SFirstName"].ToString();
                }
                
                if (reader.HasColumn("SMiddleName") && reader["SMiddleName"] != DBNull.Value)
                {
                    p.SMiddleName = reader["SMiddleName"].ToString();
                }
                
                if (reader.HasColumn("SLastName") && reader["SLastName"] != DBNull.Value)
                {
                    p.SLastName = reader["SLastName"].ToString();
                }
                
                if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
                {
                    p.FullName = reader["FullName"].ToString();
                }
                
                if (reader.HasColumn("Sex") && reader["Sex"] != DBNull.Value)
                {
                    p.Sex =(bool)reader["Sex"];
                }
                
                if (reader.HasColumn("SDOB") && reader["SDOB"] != DBNull.Value)
                {
                    p.SDOB = (DateTime)reader["SDOB"];
                }
                
                if (reader.HasColumn("SBirthPlace") && reader["SBirthPlace"] != DBNull.Value)
                {
                    p.SBirthPlace = reader["SBirthPlace"].ToString();
                }
                
                if (reader.HasColumn("SIDNumber") && reader["SIDNumber"] != DBNull.Value)
                {
                    p.SIDNumber = reader["SIDNumber"].ToString();
                }

                if (reader.HasColumn("SPlaceOfIssue") && reader["SPlaceOfIssue"] != DBNull.Value)
                {
                    p.SPlaceOfIssue = reader["SPlaceOfIssue"].ToString();
                }
                
                if (reader.HasColumn("SSurburb") && reader["SSurburb"] != DBNull.Value)
                {
                    p.SSurburb = reader["SSurburb"].ToString();
                }
                
                if (reader.HasColumn("SState") && reader["SState"] != DBNull.Value)
                {
                    p.SState = reader["SState"].ToString();
                }
                
                if (reader.HasColumn("SPhoneNumber") && reader["SPhoneNumber"] != DBNull.Value)
                {
                    p.SPhoneNumber = reader["SPhoneNumber"].ToString();
                }
                
                if (reader.HasColumn("SMobiPhoneNumber") && reader["SMobiPhoneNumber"] != DBNull.Value)
                {
                    p.SMobiPhoneNumber = reader["SMobiPhoneNumber"].ToString();
                }
                
                if (reader.HasColumn("SEmailAddress") && reader["SEmailAddress"] != DBNull.Value)
                {
                    p.SEmailAddress = reader["SEmailAddress"].ToString();
                }
                
                if (reader.HasColumn("SEmployDate") && reader["SEmployDate"] != DBNull.Value)
                {
                    p.SEmployDate = (DateTime)reader["SEmployDate"];
                }
                
                if (reader.HasColumn("SLeftDate") && reader["SLeftDate"] != DBNull.Value)
                {
                    p.SLeftDate = (DateTime)reader["SLeftDate"];
                }

                //▼====: #003
                if (reader.HasColumn("CertificateNumber") && reader["CertificateNumber"] != DBNull.Value)
                {
                    p.SCertificateNumber = (String)reader["CertificateNumber"];
                }

                if (reader.HasColumn("SCode") && reader["SCode"] != DBNull.Value)
                {
                    p.SCode = (String)reader["SCode"];
                }
                //▲====: #003
                if (reader.HasColumn("V_Religion") && reader["V_Religion"] != DBNull.Value)
                {
                    p.V_Religion = (long)reader["V_Religion"];
                    p.Religion.LookupID = (long)p.V_Religion;
                }
                
                if (reader.HasColumn("V_MaritalStatus") && reader["V_MaritalStatus"] != DBNull.Value)
                {
                    p.V_MaritalStatus = (long)reader["V_MaritalStatus"];
                    p.MaritalStatus.LookupID = (long) p.V_MaritalStatus;
                }
                
                if (reader.HasColumn("V_Ethnic") && reader["V_Ethnic"] != DBNull.Value)
                {
                    p.V_Ethnic = (long)reader["V_Ethnic"];
                    p.Ethnic.LookupID = (long) p.V_Ethnic;
                }
                
                if (reader.HasColumn("SAccountNumber") && reader["SAccountNumber"] != DBNull.Value)
                {
                    p.SAccountNumber = reader["SAccountNumber"].ToString();
                }
                
                if (reader.HasColumn("V_BankName") && reader["V_BankName"] != DBNull.Value)
                {
                    p.V_BankName = (long)reader["V_BankName"];
                    p.BankName.LookupID=(long)p.V_BankName;
                }
                
                if (reader.HasColumn("SEmploymentHistory") && reader["SEmploymentHistory"] != DBNull.Value)
                {
                    p.SEmploymentHistory = reader["SEmploymentHistory"].ToString();
                }
                
                if (reader.HasColumn("SImage") && reader["SImage"] != DBNull.Value)
                {
                    p.SImage = reader["SImage"].ToString();
                }

                if (reader.HasColumn("PImage") && reader["PImage"] != DBNull.Value)
                {
                    p.PImage = (Byte[])reader["PImage"];
                }
                //-------------------------------------
                
                if (reader.HasColumn("CityProvinceName") && reader["CityProvinceName"] != DBNull.Value)
                {
                    
                    p.CitiesProvince.CityProvinceName = reader["CityProvinceName"].ToString();
                }
				
                if (reader.HasColumn("CountryName") && reader["CountryName"] != DBNull.Value)
                {
                    
                    p.RefCountry.CountryName = reader["CountryName"].ToString();
                }
				
                if (reader.HasColumn("StaffCatgDescription") && reader["StaffCatgDescription"] != DBNull.Value)
                {
                    
                    p.RefStaffCategory.StaffCatgDescription = reader["StaffCatgDescription"].ToString();
                }
				
                if (reader.HasColumn("Ethnic") && reader["Ethnic"] != DBNull.Value)
                {
                    
                    p.Ethnic.ObjectValue = reader["Ethnic"].ToString();
                }
				
                if (reader.HasColumn("MaritalStatus") && reader["MaritalStatus"] != DBNull.Value)
                {
                    
                    p.MaritalStatus.ObjectValue = reader["MaritalStatus"].ToString();
                }
									
                if (reader.HasColumn("Religion") && reader["Religion"] != DBNull.Value)
                {
                    
                    p.Religion.ObjectValue = reader["Religion"].ToString();
                }

                if (reader.HasColumn("DeptName") && reader["DeptName"] != DBNull.Value)
                {
                    
                    p.RefDepartment.DeptName = reader["DeptName"].ToString();
                }
                /*▼====: #001*/
                if (reader.HasColumn("V_AcademicRank") && reader["V_AcademicRank"] != DBNull.Value)
                {
                    p.V_AcademicRank = (long)reader["V_AcademicRank"];
                }
                if (reader.HasColumn("V_AcademicDegree") && reader["V_AcademicDegree"] != DBNull.Value)
                {
                    p.V_AcademicDegree = (long)reader["V_AcademicDegree"];
                }
                if (reader.HasColumn("V_Education") && reader["V_Education"] != DBNull.Value)
                {
                    p.V_Education = (long)reader["V_Education"];
                }
                if (reader.HasColumn("IsFund") && reader["IsFund"] != DBNull.Value)
                {
                    p.IsFund = Convert.ToBoolean(reader["IsFund"]);
                }
                /*▲====: #001*/
                if (reader.HasColumn("IsReport") && reader["IsReport"] != DBNull.Value)
                {
                    p.IsReport = Convert.ToBoolean(reader["IsReport"]);
                }
                if (reader.HasColumn("V_StaffCatType") && reader["V_StaffCatType"] != DBNull.Value)
                {
                    p.RefStaffCategory.V_StaffCatType = (long)reader["V_StaffCatType"];
                }
            }
            catch 
            { return null; }
            return p;
        }
        protected virtual List<Staff> GetStaffsCollectionFromReader(IDataReader reader)
        {
            List<Staff> staffs = new List<Staff>();
            while (reader.Read())
            {
                staffs.Add(GetStaffsFromReader(reader));
            }
            return staffs;
        }
        #endregion

        #region StaffDeptResponsibility

        public abstract List<StaffDeptResponsibilities> GetStaffDeptResponsibilitiesByDeptID(
            StaffDeptResponsibilities p, bool isHis);
        public abstract bool InsertStaffDeptResponsibilitiesXML(List<StaffDeptResponsibilities> lstStaffDeptRes);
        public abstract bool UpdateStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes);
        public abstract bool DeleteStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes);
        protected virtual StaffDeptResponsibilities GetStaffDeptResponsibilitiesFromReader(IDataReader reader)
        {
            StaffDeptResponsibilities p = new StaffDeptResponsibilities();
            p.Staff=new Staff();
            p.RefDepartment=new RefDepartment();
            try
            {
                if (reader.HasColumn("StaffDeptResponsibilitiesID") && reader["StaffDeptResponsibilitiesID"] != DBNull.Value)
                {
                    p.StaffDeptResponsibilitiesID = (long)(reader["StaffDeptResponsibilitiesID"]);
                }

                if (reader.HasColumn("StaffID") && reader["StaffID"] != DBNull.Value)
                {
                    p.StaffID = (long)(reader["StaffID"]);
                    p.Staff.StaffID = (long)(reader["StaffID"]);
                }

                if (reader.HasColumn("DeptID") && reader["DeptID"] != DBNull.Value)
                {
                    p.DeptID = (long)(reader["DeptID"]);
                    p.RefDepartment.DeptID = (long)(reader["DeptID"]);
                }

                if (reader.HasColumn("DeptName") && reader["DeptName"] != DBNull.Value)
                {
                    p.RefDepartment.DeptName = (reader["DeptName"].ToString());
                }

                if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
                {
                    p.Staff.FullName = (reader["FullName"].ToString());
                }


                if (reader.HasColumn("Responsibilities_32") && reader["Responsibilities_32"] != DBNull.Value)
                {
                    p.Responsibilities_32 = Convert.ToInt32(reader["Responsibilities_32"]);
                    p.CheckValue(p.Responsibilities_32);
                }
                
                if (reader.HasColumn("RecCreatedDate") && reader["RecCreatedDate"] != DBNull.Value)
                {
                    p.RecCreatedDate = (DateTime)(reader["RecCreatedDate"]);
                }

                if (reader.HasColumn("IsDeleted") && reader["IsDeleted"] != DBNull.Value)
                {
                    p.IsDeleted = (bool)(reader["IsDeleted"]);
                }

                
            }
            catch
            { return null; }
            return p;
        }
        protected virtual List<StaffDeptResponsibilities> GetStaffDeptResponsibilitiesCollectionFromReader(IDataReader reader)
        {
            List<StaffDeptResponsibilities> StaffDeptResponsibilities = new List<StaffDeptResponsibilities>();
            while (reader.Read())
            {
                StaffDeptResponsibilities.Add(GetStaffDeptResponsibilitiesFromReader(reader));
            }
            return StaffDeptResponsibilities;
        }

        #endregion

        #region StaffStoreDeptResponsibility

        public abstract List<StaffStoreDeptResponsibilities> GetStaffStoreDeptResponsibilitiesByDeptID(
            StaffStoreDeptResponsibilities p, bool isHis);
        public abstract bool InsertStaffStoreDeptResponsibilitiesXML(List<StaffStoreDeptResponsibilities> lstStaffDeptRes);
        public abstract bool UpdateStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes);
        public abstract bool DeleteStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes);
        protected virtual StaffStoreDeptResponsibilities GetStaffStoreDeptResponsibilitiesFromReader(IDataReader reader)
        {
            StaffStoreDeptResponsibilities p = new StaffStoreDeptResponsibilities();
            p.Staff = new Staff();
            p.RefDepartment = new RefDepartment();
            p.RefStorageWarehouseLocation=new RefStorageWarehouseLocation();
            try
            {
                if (reader.HasColumn("StaffStoreDeptResponsibilitiesID") && reader["StaffStoreDeptResponsibilitiesID"] != DBNull.Value)
                {
                    p.StaffStoreDeptResponsibilitiesID = (long)(reader["StaffStoreDeptResponsibilitiesID"]);
                }

                if (reader.HasColumn("StaffID") && reader["StaffID"] != DBNull.Value)
                {
                    p.StaffID = (long)(reader["StaffID"]);
                    p.Staff.StaffID = (long)(reader["StaffID"]);
                }

                if (reader.HasColumn("DeptID") && reader["DeptID"] != DBNull.Value)
                {
                    p.DeptID = (long)(reader["DeptID"]);
                    p.RefDepartment.DeptID = (long)(reader["DeptID"]);
                }

                if (reader.HasColumn("StoreID") && reader["StoreID"] != DBNull.Value)
                {
                    p.StoreID = (long)(reader["StoreID"]);
                    p.RefStorageWarehouseLocation.StoreID = p.StoreID;
                }

                if (reader.HasColumn("swhlName") && reader["swhlName"] != DBNull.Value)
                {
                    p.RefStorageWarehouseLocation.swhlName = reader["swhlName"].ToString();
                }

                if (reader.HasColumn("DeptName") && reader["DeptName"] != DBNull.Value)
                {
                    p.RefDepartment.DeptName = (reader["DeptName"].ToString());
                }

                if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
                {
                    p.Staff.FullName = (reader["FullName"].ToString());
                }


                if (reader.HasColumn("Responsibilities_32") && reader["Responsibilities_32"] != DBNull.Value)
                {
                    p.Responsibilities_32 = Convert.ToInt32(reader["Responsibilities_32"]);
                    p.CheckValue(p.Responsibilities_32);
                }

                if (reader.HasColumn("RecCreatedDate") && reader["RecCreatedDate"] != DBNull.Value)
                {
                    p.RecCreatedDate = (DateTime)(reader["RecCreatedDate"]);
                }

                if (reader.HasColumn("IsDeleted") && reader["IsDeleted"] != DBNull.Value)
                {
                    p.IsDeleted = (bool)(reader["IsDeleted"]);
                }


            }
            catch(Exception ex)
            {
                AxLogger.Instance.LogError(ex);
            }
            return p;
        }
        protected virtual List<StaffStoreDeptResponsibilities> GetStaffStoreDeptResponsibilitiesCollectionFromReader(IDataReader reader)
        {
            List<StaffStoreDeptResponsibilities> StaffStoreDeptResponsibilities = new List<StaffStoreDeptResponsibilities>();
            while (reader.Read())
            {
                StaffStoreDeptResponsibilities.Add(GetStaffStoreDeptResponsibilitiesFromReader(reader));
            }
            return StaffStoreDeptResponsibilities;
        }

        #endregion
    }
}

