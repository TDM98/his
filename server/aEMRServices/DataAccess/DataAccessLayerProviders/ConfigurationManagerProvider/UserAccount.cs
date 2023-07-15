/*
 * 20171409 #002 CMN: Added EmployeesReport
 * 20180409 #003 TTM: Kiem tra gia tri cua Certificate va SCode
 * 20210107 #004 THYX: Add StaffEditID
 * 20221210 #005 TNHX: Thêm thông tin đơn thuốc điện tử
 * 20230710 #006 BLQ: Thêm trường chức vụ
*/

using System;
using System.Collections.Generic;

using System.Text;
using System.Reflection;
using eHCMS.Configurations;
using DataEntities;
using System.Data;

using System.Collections.ObjectModel;
using AxLogging;
using System.Data.SqlClient;
using eHCMS.DAL;

namespace aEMR.DataAccessLayer.Providers
{
    public class UserAccounts : DataProviderBase
    {
        static private UserAccounts _instance = null;
        static public UserAccounts Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserAccounts();
                }
                return _instance;

            }
        }

        public UserAccounts()
        {
            this.ConnectionString = Globals.Settings.ConfigurationManager.ConnectionString;
        }

        protected virtual List<UserAccount> GetUserAccountCollectionFromReader(IDataReader reader)
        {
            List<UserAccount> lst = new List<UserAccount>();
            while (reader.Read())
            {
                lst.Add(GetUserAccountObjFromReader(reader));
            }
            reader.Close();
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
                    p.StaffID = (long)reader["StaffID"];
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
                    p.IsActivated = (bool)reader["IsActivated"];
                }

            }
            catch
            { return null; }
            return p;
        }


        protected virtual List<Group> GetGroupCollectionFromReader(IDataReader reader)
        {
            List<Group> lst = new List<Group>();
            while (reader.Read())
            {
                lst.Add(GetGroupObjFromReader(reader));
            }
            reader.Close();
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


        protected virtual List<UserGroup> GetUserGroupCollectionFromReader(IDataReader reader)
        {
            List<UserGroup> lst = new List<UserGroup>();
            while (reader.Read())
            {
                lst.Add(GetUserGroupObjFromReader(reader));
            }
            reader.Close();
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
                        p.UserAccount.StaffID = (long?)(reader["StaffID"]);
                    }

                    if (reader.HasColumn("AccountName") && reader["AccountName"] != DBNull.Value)
                    {
                        p.UserAccount.AccountName = reader["AccountName"].ToString();
                    }

                    if (reader.HasColumn("AccountPassword") && reader["AccountPassword"] != DBNull.Value)
                    {
                        p.UserAccount.AccountPassword = reader["AccountPassword"].ToString();
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
                }
                catch
                {
                }
            }
            catch
            { return null; }
            return p;
        }

        private int checkCountOperationEnum(int modulueEnum)
        {
            switch (modulueEnum)
            {
                case 2:
                    return (int)oRegistrionEx.mCount;
                case 3:
                    return (int)oConsultationEx.mCount;
                case 4:
                    return (int)oParaClinicalEx.mCount;
                case 5:
                    return (int)oPharmacyEx.mCount;
                case 6:
                    return (int)oTransaction_ManagementEx.mCount;
                case 7:
                    return (int)oConfigurationEx.mCount;
                case 8:
                    return (int)oSystem_ManagementEx.mCount;
                case 9:
                    return (int)oResourcesEx.mCount;
                case 10:
                    return (int)oResources_MaintenanceEx.mCount;
                case 11:
                    return (int)oAppointmentEx.mCount;
                case 12:
                    return (int)oUserAccountEx.mCount;

                case 14:
                    return (int)oKhoaDuocEx.mCount;
                //case    15:
                //    return (int) oTransaction;
                case 16:
                    return (int)oModuleGeneralEX.mCount;
                case 17:
                    return (int)oClinicManagementEx.mCount;
                case 18:
                    return (int)oCLSLaboratoryEx.mCount;
                case 19:
                    return (int)oCLSImagingEX.mCount;
                case 20:
                    return (int)oKhoPhongEx.mCount;
                case 21:
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
                    mEnum = Convert.ToInt32(reader["meNum"]);
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
                    int count = checkCountFunctionEnum(mEnum) + 1;
                    if (lstModule[mEnum].lstFunction.Count < 1)
                    {
                        for (int i = 1; i <= count; i++)
                        {
                            refFunction rff = new refFunction();
                            lstModule[mEnum].lstFunction.Add(rff);
                        }
                    }

                    if (reader.HasColumn("FunctionName") && reader["FunctionName"] != DBNull.Value)
                    {
                        if (lstModule[mEnum].lstFunction[fEnum] == null)
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
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pFullControl == false)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pFullControl = (bool)(reader["PermissionFullControl"]);
                        }

                        if (reader.HasColumn("PermissionView") && reader["PermissionView"] != DBNull.Value
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pView == false)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pView = (bool)(reader["PermissionView"]);
                        }

                        if (reader.HasColumn("PermissionAdd") && reader["PermissionAdd"] != DBNull.Value
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pAdd == false)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pAdd = (bool)(reader["PermissionAdd"]);
                        }

                        if (reader.HasColumn("PermissionUpdate") && reader["PermissionUpdate"] != DBNull.Value
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pUpdate == false)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pUpdate = (bool)(reader["PermissionUpdate"]);
                        }

                        if (reader.HasColumn("PermissionDelete") && reader["PermissionDelete"] != DBNull.Value
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pDelete == false)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pDelete = (bool)(reader["PermissionDelete"]);
                        }

                        if (reader.HasColumn("PermissionReport") && reader["PermissionReport"] != DBNull.Value
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pReport == false)
                        {
                            lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pReport = (bool)(reader["PermissionReport"]);
                        }

                        if (reader.HasColumn("PermissionPrint") && reader["PermissionPrint"] != DBNull.Value
                            && lstModule[mEnum].lstFunction[fEnum].lstOperation[oEnum].mPermission.pPrint == false)
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
            reader.Close();
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


        #region Group Role

        protected virtual List<GroupRole> GetGroupRoleCollectionFromReader(IDataReader reader)
        {
            List<GroupRole> lst = new List<GroupRole>();
            while (reader.Read())
            {
                lst.Add(GetGroupRoleObjFromReader(reader));
            }
            reader.Close();
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


                p.Role = new Role();

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


        #endregion

        #region Permission
        protected virtual List<Permission> GetPermissionCollectionFromReader(IDataReader reader)
        {
            List<Permission> lst = new List<Permission>();
            while (reader.Read())
            {
                lst.Add(GetPermissionObjFromReader(reader));
            }
            reader.Close();
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
                    p.Operation.FunctionID = Convert.ToInt32(reader["FunctionID"]);
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
                    p.Operation.Function.Module.ModuleID = Convert.ToInt32(reader["ModuleID"]);
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

        protected virtual List<DataEntities.Module> GetModuleCollectionFromReader(IDataReader reader)
        {
            List<DataEntities.Module> lst = new List<DataEntities.Module>();
            while (reader.Read())
            {
                lst.Add(GetModuleObjFromReader(reader));
            }
            reader.Close();
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

        protected virtual List<Function> GetFunctionCollectionFromReader(IDataReader reader)
        {
            List<Function> lst = new List<Function>();
            while (reader.Read())
            {
                lst.Add(GetFunctionObjFromReader(reader));
            }
            reader.Close();
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

        protected virtual List<Operation> GetOperationCollectionFromReader(IDataReader reader)
        {
            List<Operation> lst = new List<Operation>();
            while (reader.Read())
            {
                lst.Add(GetOperationObjFromReader(reader));
            }
            reader.Close();
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

        protected virtual List<Permission> GetPermissionCollectionFromReaderEx(IDataReader reader)
        {
            List<Permission> lst = new List<Permission>();
            while (reader.Read())
            {
                lst.Add(GetPermissionObjFromReaderEx(reader));
            }
            reader.Close();
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
        protected virtual List<Role> GetRoleCollectionFromReader(IDataReader reader)
        {
            List<Role> lst = new List<Role>();
            while (reader.Read())
            {
                lst.Add(GetRoleObjFromReader(reader));
            }
            reader.Close();
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
            reader.Close();
            return retVal;
        }


        #endregion

        #region staff


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
            p.BankName = new Lookup();
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
                    p.RefDepartment.DeptID = (long)p.DeptID;
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
                    p.Sex = (bool)reader["Sex"];
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
                    p.MaritalStatus.LookupID = (long)p.V_MaritalStatus;
                }

                if (reader.HasColumn("V_Ethnic") && reader["V_Ethnic"] != DBNull.Value)
                {
                    p.V_Ethnic = (long)reader["V_Ethnic"];
                    p.Ethnic.LookupID = (long)p.V_Ethnic;
                }

                if (reader.HasColumn("SAccountNumber") && reader["SAccountNumber"] != DBNull.Value)
                {
                    p.SAccountNumber = reader["SAccountNumber"].ToString();
                }

                if (reader.HasColumn("V_BankName") && reader["V_BankName"] != DBNull.Value)
                {
                    p.V_BankName = (long)reader["V_BankName"];
                    p.BankName.LookupID = (long)p.V_BankName;
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
                if (reader.HasColumn("PrintTitle") && reader["PrintTitle"] != DBNull.Value)
                {
                    p.PrintTitle = reader["PrintTitle"].ToString();
                }
                if (reader.HasColumn("SocialInsuranceNumber") && reader["SocialInsuranceNumber"] != DBNull.Value)
                {
                    p.SocialInsuranceNumber = reader["SocialInsuranceNumber"] as string;
                }
                if (reader.HasColumn("IsStopUsing") && reader["IsStopUsing"] != DBNull.Value)
                {
                    p.IsStopUsing = Convert.ToBoolean(reader["IsStopUsing"]);
                }
                //▼====: #005
                if (reader.HasColumn("MaDinhDanhBsi") && reader["MaDinhDanhBsi"] != DBNull.Value)
                {
                    p.MaDinhDanhBsi = reader["MaDinhDanhBsi"].ToString();
                }
                if (reader.HasColumn("MaLienThongBacSi") && reader["MaLienThongBacSi"] != DBNull.Value)
                {
                    p.MaLienThongBacSi = reader["MaLienThongBacSi"].ToString();
                }
                //▲====: #005
                //▼====: #006
                if (reader.HasColumn("V_JobPosition") && reader["V_JobPosition"] != DBNull.Value)
                {
                    p.V_JobPosition = Convert.ToInt64(reader["V_JobPosition"]);
                }
                //▲====: #006
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
            reader.Close();
            return staffs;
        }
        #endregion

        #region StaffDeptResponsibility

        protected virtual StaffDeptResponsibilities GetStaffDeptResponsibilitiesFromReader(IDataReader reader)
        {
            StaffDeptResponsibilities p = new StaffDeptResponsibilities();
            p.Staff = new Staff();
            p.RefDepartment = new RefDepartment();
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
            reader.Close();
            return StaffDeptResponsibilities;
        }

        #endregion

        #region StaffStoreDeptResponsibility


        protected virtual StaffStoreDeptResponsibilities GetStaffStoreDeptResponsibilitiesFromReader(IDataReader reader)
        {
            StaffStoreDeptResponsibilities p = new StaffStoreDeptResponsibilities();
            p.Staff = new Staff();
            p.RefDepartment = new RefDepartment();
            p.RefStorageWarehouseLocation = new RefStorageWarehouseLocation();
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
            catch (Exception ex)
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
            reader.Close();
            return StaffStoreDeptResponsibilities;
        }

        #endregion


        public List<Lookup> GetLookupOperationType()
        {
            List<Lookup> objLst = new List<Lookup>();
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.V_Operation);
            return objLst;
        }
        #region userAccount


        #endregion


        #region Group
        public List<Group> GetAllGroupByGroupID(long GroupID)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        public List<UserGroup> GetAllUserGroupGetByID(long AccountID, long GroupID
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        #endregion

        #region Role Group

        public List<GroupRole> GetAllGroupRolesGetByID(long GroupID, long RoleID
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        #endregion

        #region Permission

        public List<Permission> GetAllPermissions_GetByID(long RoleID, long OperationID
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        #endregion

        #region Modules
        public List<DataEntities.Module> GetAllModules()
        {
            List<DataEntities.Module> listRs = new List<DataEntities.Module>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUM_Modules_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listRs = GetModuleCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        public bool AddNewModules(string ModuleName, int eNum, string Description, int Idx)
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
            catch { return false; }
            return true;
        }
        public bool UpdateModules(long ModuleID, int eNum, string ModuleName, string Description, int Idx)
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
            catch { return false; }
            return true;
        }
        public bool UpdateModulesEnum(long ModuleID, int eNum)
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
            catch { return false; }
            return true;
        }
        public bool DeleteModules(long ModuleID)
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
            catch { return false; }
            return true;
        }

        #endregion

        #region Function

        public List<Function> GetAllFunction(long ModuleID)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }

        public List<Function> GetAllFunctionsPaging(long ModuleID
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        public bool AddNewFunctions(int ModuleID, int eNum, string FunctionName, string FunctionDescription, int Idx)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        public bool UpdateFunctionsEnum(long FunctionID, int eNum)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public bool UpdateFunctions(long FunctionID, int eNum, int ModuleID, string FunctionName, string FunctionDescription, byte Idx)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public bool DeleteFunctions(long FunctionID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        #endregion

        #region Operation

        public List<Operation> GetAllOperations(long OperationID)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        public List<Operation> GetAllOperationsByFuncID(long FunctionID)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }

        // 20180921 TNHX: Add method for get all detail Modules Tree
        public List<Um_ModuleFunctionOperations> GetAllDetails_ModuleFunctionOperations()
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listUm;
        }

        public List<Operation> GetAllOperationsByFuncIDPaging(long FunctionID
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        public bool AddNewOperations(long FunctionID, string OperationName, string Description)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public bool UpdateOperations(long OperationID, int Enum, string OperationName, string Description)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        public bool DeleteOperations(long OperationID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        #endregion

        #region Permission

        public List<Permission> GetAllPermissions(long PermissionItemID)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        public bool AddNewPermissions(int RoleID, long OperationID
                                                , bool FullControl
                                                , bool pView
                                                , bool pAdd
                                                , bool pUpdate
                                                , bool pDelete
                                                , bool pReport
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        public bool UpdatePermissionsFull(long PermissionItemID
                                                , bool FullControl
                                                , bool pView
                                                , bool pAdd
                                                , bool pUpdate
                                                , bool pDelete
                                                , bool pReport
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return false;
            }
            return true;
        }
        public bool UpdatePermissions(long PermissionItemID, int RoleID, long OperationID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public bool DeletePermissions(long PermissionItemID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        #endregion

        #region Roles

        public List<Role> GetAllRoles()
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }

        public List<Role> GetAllRolesAllPaging(long RoleID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        public bool AddNewRoles(string RoleName, string Description)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public bool UpdateRoles(int RoleID, string RoleName, string Description)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public bool DeleteRoles(int RoleID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        #endregion


        #region UserAccount
        public List<UserAccount> GetAllUserAccount(long AccountID)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }


        public List<UserAccount> GetAllUserAccountsPaging(string AccountName, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }

        public bool AddNewUserAccount(long StaffID, string AccountName, string AccountPassword, bool IsActivated)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public bool UpdateUserAccount(int AccountID, long StaffID, string AccountName, string AccountPassword, bool IsActivated)
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
                int val = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return val > 0;
            }

        }
        public bool DeleteUserAccount(int AccountID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        #endregion


        #region UserGroup
        public List<refModule> GetAllUserGroupByAccountID(long AccountID)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }

        public bool AddNewUserGroup(long AccountID, int GroupID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        public bool AddNewUserGroupXML(IList<UserGroup> lstUserGroup)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
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
        public bool UpdateUserGroup(long UGID, long AccountID, int GroupID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public bool DeleteUserGroup(int UGID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        #endregion

        #region Group Role

        public bool AddNewGroupRoles(int GroupID, int RoleID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }


        public bool AddNewGroupRolesXML(IList<GroupRole> lstGroupRole)
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
                    CleanUpConnectionAndCommand(cn, cmd);
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
        public bool UpdateGroupRoles(long GroupRoleID, int GroupID, int RoleID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public bool DeleteGroupRoles(long GroupRoleID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        #endregion


        #region Group
        public List<Group> GetAllGroupAllPaging(long GroupID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        public bool AddNewGroup(string GroupName, string Description)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public bool UpdateGroup(int GroupID, string GroupName, string Description)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        public bool DeleteGroup(int GroupID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }

        #endregion

        #region User Login
        public List<UserLoginHistory> GetUserLogHisGByUserAccount()
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        public List<UserLoginHistory> GetUserLogHisGByHostName()
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }

        public List<UserLoginHistory> GetAllUserLoginHistoryPaging(UserLoginHistory ulh, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }

        public long AddUserLoginHistory(UserLoginHistory Ulh)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUserLoginHistory_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@AccountID", SqlDbType.BigInt, Ulh.AccountID);
                cmd.AddParameter("@LogDateTime", SqlDbType.DateTime, ConvertNullObjectToDBNull(Ulh.LogDateTime));
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
        public bool UpdateUserLoginHistory(UserLoginHistory Ulh)
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
        public bool DeleteUserLoginHistory(long LoggedHistoryID)
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

        #region staff

        public Staff GetStaffsByID(long StaffID)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return curStaff;
        }
        public List<Staff> GetAllStaffAllPaging(Staff searchStaff, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
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
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }

        public bool AddNewStaff(Staff newStaff)
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
                cmd.AddParameter("@PrintTitle", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.PrintTitle));
                cmd.AddParameter("@SocialInsuranceNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SocialInsuranceNumber));
                cmd.AddParameter("@IsStopUsing", SqlDbType.Bit, ConvertNullObjectToDBNull(newStaff.IsStopUsing));
                //▼====: #005
                cmd.AddParameter("@MatKhauLienThongBacSi", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.MatKhauLienThongBacSi));
                cmd.AddParameter("@MaDinhDanhBsi", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.MaDinhDanhBsi));
                cmd.AddParameter("@MaLienThongBacSi", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.MaLienThongBacSi));
                cmd.AddParameter("@ModifiedPasswordDT", SqlDbType.Bit, ConvertNullObjectToDBNull(newStaff.ModifiedPasswordDT));
                //▲====: #005
                //▼====: #006
                cmd.AddParameter("@V_JobPosition", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_JobPosition));
                //▲====: #006
                cn.Open();
                cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return true;
        }

        //▼====: #004
        public bool UpdateStaff(Staff newStaff, long? StaffEditID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
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
                    cmd.AddParameter("@PrintTitle", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.PrintTitle));
                    cmd.AddParameter("@StaffEditID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffEditID));
                    cmd.AddParameter("@SocialInsuranceNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.SocialInsuranceNumber));
                    cmd.AddParameter("@IsStopUsing", SqlDbType.Bit, ConvertNullObjectToDBNull(newStaff.IsStopUsing));
                    //▼====: #005
                    cmd.AddParameter("@MatKhauLienThongBacSi", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.MatKhauLienThongBacSi));
                    cmd.AddParameter("@MaDinhDanhBsi", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.MaDinhDanhBsi));
                    cmd.AddParameter("@MaLienThongBacSi", SqlDbType.NVarChar, ConvertNullObjectToDBNull(newStaff.MaLienThongBacSi));
                    cmd.AddParameter("@ModifiedPasswordDT", SqlDbType.Bit, ConvertNullObjectToDBNull(newStaff.ModifiedPasswordDT));
                    //▲====: #005
                    //▼====: #006
                    cmd.AddParameter("@V_JobPosition", SqlDbType.BigInt, ConvertNullObjectToDBNull(newStaff.V_JobPosition));
                    //▲====: #006
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        //▲====: #004

        public bool DeleteStaff(int StaffID)
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
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch { return false; }
            return true;
        }
        #endregion

        #region StaffDeptResponsibility
        public List<StaffDeptResponsibilities> GetStaffDeptResponsibilitiesByDeptID(StaffDeptResponsibilities p, bool isHis)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }
        public bool InsertStaffDeptResponsibilitiesXML(List<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffDeptResponsibilitiesInsertXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, StaffDeptRes_ConvertListToXml(lstStaffDeptRes));

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

        public bool UpdateStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffDeptResponsibilitiesUpdateXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, StaffDeptRes_ConvertListToXml(lstStaffDeptRes));

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

        public bool DeleteStaffDeptResponsibilitiesXML(IList<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffDeptResponsibilitiesDeleteXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, StaffDeptRes_ConvertListToXml(lstStaffDeptRes));

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


        private string StaffDeptRes_ConvertListToXml(IList<StaffDeptResponsibilities> lstStaffDeptRes)
        {
            if (lstStaffDeptRes != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (StaffDeptResponsibilities item in lstStaffDeptRes)
                {
                    sb.Append("<StaffDeptResponsibilities>");
                    if (item.StaffDeptResponsibilitiesID > 0)
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
        public List<StaffStoreDeptResponsibilities> GetStaffStoreDeptResponsibilitiesByDeptID(StaffStoreDeptResponsibilities p, bool isHis)
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
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }
        public bool InsertStaffStoreDeptResponsibilitiesXML(List<StaffStoreDeptResponsibilities> lstStaffDeptRes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffStoreDeptResponsibilitiesInsertXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, StaffStoreDeptRes_ConvertListToXml(lstStaffDeptRes));

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

        public bool UpdateStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffStoreDeptResponsibilitiesUpdateXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, StaffStoreDeptRes_ConvertListToXml(lstStaffDeptRes));

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

        public bool DeleteStaffStoreDeptResponsibilitiesXML(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffStoreDeptResponsibilitiesDeleteXML", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@DataXML", SqlDbType.Xml, StaffStoreDeptRes_ConvertListToXml(lstStaffDeptRes));

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


        private string StaffStoreDeptRes_ConvertListToXml(IList<StaffStoreDeptResponsibilities> lstStaffDeptRes)
        {
            if (lstStaffDeptRes != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<DS>");
                foreach (StaffStoreDeptResponsibilities item in lstStaffDeptRes)
                {
                    sb.Append("<StaffStoreDeptResponsibilities>");
                    if (item.StaffStoreDeptResponsibilitiesID > 0)
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

        public List<ConsultationTimeSegments> GetAllConsultationTimeSegmentsTodayByAccountID(long StaffID, out bool? IsOutOfSegments)
        {
            List<ConsultationTimeSegments> listCTS = new List<ConsultationTimeSegments>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllConsultationTimeSegmentsTodayByAccountID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@StaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StaffID));
                cmd.AddParameter("@IsOutOfSegments", SqlDbType.Bit, DBNull.Value, ParameterDirection.Output);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listCTS = GetConsultationTimeSegmentsCollectionFromReader(reader);
                reader.Close();
                IsOutOfSegments = (bool)cmd.Parameters["@IsOutOfSegments"].Value;
                CleanUpConnectionAndCommand(cn, cmd);
               
            }
            return listCTS;
        }
    }
}

