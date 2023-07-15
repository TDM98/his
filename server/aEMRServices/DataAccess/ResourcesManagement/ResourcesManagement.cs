/*
 * 20180529 #001: TTM Thêm parameter HIRepResourceCode
 * 20180607 #002: TTM Thay đổi toàn bộ điều kiện kiểm tra của các thuộc tính trong hàm GetResourcesObjFromReader (từ kiểm tra != null sang kiểm tra != DBNull.Value)
 * 20180607 #003: Thêm 2 hàm abstract GetAllResources và GetResourcesForMedicalServices_LoadNotPaging
 * 20180307 #004: Thêm hàm trang thiết bị máy để hàm GetResourcesForMedicalServices_LoadNotPaging sử dụng.
 * 20180307 #005: Thêm parameter để đọc V_PCLMainCategory 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Reflection;
using eHCMS.Configurations;
using System.Data;

using eHCMS.Services.Core;

namespace eHCMS.DAL
{
    public abstract class ResourcesManagement : DataProviderBase
    {
        static private ResourcesManagement _instance = null;
        static public ResourcesManagement Instance
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
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.resourcesManage.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type[] types = assem.GetExportedTypes();
                    Type t = assem.GetType(Globals.Settings.resourcesManage.ProviderType);
                    _instance = (ResourcesManagement)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }

        public ResourcesManagement()
        {
            this.ConnectionString = Globals.Settings.resourcesManage.ConnectionString;

        }

        #region 0. Resource member
       public abstract List<Lookup> GetLookupRsrcUnit();
       public abstract List<Lookup> GetLookupResGroupCategory();
       public abstract List<Lookup> GetLookupSupplierType();
       
        //V_AllocStatus
       public abstract List<ResourceGroup> GetAllResourceGroup();
       public abstract List<ResourceType> GetAllResourceType();
       public abstract List<Resources> GetAllResource();
        //▼=====#003
        public abstract List<Resources> GetAllResources();

        public abstract List<Resources> GetResourcesForMedicalServices_LoadNotPaging(long PCLExamTypeID);
        //▲=====#003
        public abstract List<Resources> GetAllResource_GetAllPage(int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);
       
       public abstract List<ResourceType> GetAllResourceTypeByGID(long RscrGroupID);
       public abstract List<Resources> GetAllResourceByChoice(int mChoice, long RscrID, string Text);
       public abstract List<Resources> GetAllResourceByChoicePaging(int mChoice
                                              , long RscrID
                                              , string Text
                                                ,long V_ResGroupCategory
                                              , int PageSize
                                               , int PageIndex
                                               , string OrderBy
                                               , bool CountTotal
                                               , out int Total);
       public abstract List<Resources> GetAllResourceByAllFilterPage(long GroupID
                                               , long TypeID
                                               , long SuplierID
                                               , string RsName
                                               , string RsBrand
                                               , int PageSize
                                               , int PageIndex
                                               , string OrderBy
                                               , bool CountTotal
                                               , out int Total);
       public abstract List<Supplier> GetAllSupplier();
       public abstract List<Supplier> GetAllSupplierType(long V_SupplierType);
       public abstract List<ResourceGroup> GetAllResourceGroupType(long V_ResGroupCategory);

        public abstract bool AddNewResourceGroup(string GroupName, string Description, long V_ResGroupCategory);
        public abstract bool UpdateResourceGroup(long RscrGroupID, string GroupName, string Description);
        public abstract bool DeleteResourceGroup(long RscrGroupID);
        public abstract bool AddNewResourceType(long RscrGroupID,string TypeName, string Description);
        public abstract bool UpdateResourceType(long RscrTypeID,long RscrGroupID, string TypeName, string Description);
        public abstract bool DeleteResourceType(long RscrTypeID);

        public abstract bool AddNewResources( int DeprecTypeID
                                            , long RscrGroupID
                                               , long RscrTypeID
                                               , long SupplierID
                                               , string ItemName
                                               , string ItemBrand
                                               , string Functions
                                               , string GeneralTechInfo
                                               , float DepreciationByTimeRate
                                               , float DepreciationByUsageRate
                                               , decimal BuyPrice
                                               , long V_RscrUnit
                                               , bool OnHIApprovedList
                                               , int WarrantyTime
                                               //▼=====#001
                                               , string HIRepResourceCode
                                               //▲=====#001
                                               , bool IsLocatable
                                               , bool IsDeleted
            , long V_ExpenditureSource);
        public abstract bool UpdateResources(long RscrID
                                                , int DeprecTypeID
                                                , long RscrGroupID
                                               , long RscrTypeID
                                               , long SupplierID
                                               , string ItemName
                                               , string ItemBrand
                                               , string Functions
                                               , string GeneralTechInfo
                                               , float DepreciationByTimeRate
                                               , float DepreciationByUsageRate
                                               , decimal BuyPrice
                                               , long V_RscrUnit
                                               , bool OnHIApprovedList
                                               , int WarrantyTime
                                               //▼=====#001
                                               , string HIRepResourceCode
                                               //▲=====#001
                                               , bool IsLocatable
                                               , bool IsDeleted
            , long V_ExpenditureSource);
        public abstract bool DeleteResources(long RscrID);

        protected virtual ResourceGroup GetResourceGroupObjFromReader(IDataReader reader)
        {
            ResourceGroup p = new ResourceGroup();
            try
            {
                if (reader.HasColumn("RscrGroupID") && reader["RscrGroupID"] != null)
                {
                    p.RscrGroupID = (long)reader["RscrGroupID"];
                }

                if (reader.HasColumn("GroupName") && reader["GroupName"] != null)
                {
                    p.GroupName = reader["GroupName"].ToString();
                }

                if (reader.HasColumn("Description") && reader["Description"] != null)
                {
                    p.Description = reader["Description"].ToString();
                }
            }
            catch 
            { return null; }
            return p;
        }
        protected virtual ResourceType GetResourceTypeObjFromReader(IDataReader reader)
        {
            ResourceType p = new ResourceType();
            try
            {

                if (reader.HasColumn("RscrTypeID") && reader["RscrTypeID"] != null)
                {
                    p.RscrTypeID = (long)reader["RscrTypeID"];
                }

                if (reader.HasColumn("TypeName") && reader["TypeName"] != null)
                {
                    p.TypeName = reader["TypeName"].ToString();
                }
                
                if (reader.HasColumn("Description") && reader["Description"] != null)
                {
                    p.Description = reader["Description"].ToString();
                }
            }
            catch 
            { return null; }
            return p;
        }
        protected virtual List<ResourceGroup> GetResourceGroupCollectionFromReader(IDataReader reader)
        {
            List<ResourceGroup> lst = new List<ResourceGroup>();
            while (reader.Read())
            {
                lst.Add(GetResourceGroupObjFromReader(reader));
            }
            return lst;
        }
        protected virtual List<ResourceType> GetResourceTypeCollectionFromReader(IDataReader reader)
        {
            List<ResourceType> lst = new List<ResourceType>();
            while (reader.Read())
            {
                lst.Add(GetResourceTypeObjFromReader(reader));
            }
            return lst;
        }
      
        protected virtual List<Resources> GetResourceCollectionFromReader(IDataReader reader)
        {
            List<Resources> lst = new List<Resources>();
            while (reader.Read())
            {
                lst.Add(GetResourcesObjFromReader(reader));
            }
            return lst;
        }
        //▼=====#004
        protected virtual List<Resources> GetResourcesForMedicalServicesCollectionFromReader(IDataReader reader)
        {
            List<Resources> lst = new List<Resources>();
            while (reader.Read())
            {
                lst.Add(GetResourcesForMedicalServicesObjFromReader(reader));
            }
            return lst;
        }

        protected virtual Resources GetResourcesForMedicalServicesObjFromReader(IDataReader reader)
        {
            Resources p = new Resources();

            if (reader.HasColumn("RscrID") && reader["RscrID"] != DBNull.Value)
            {
                p.RscrID = (long)reader["RscrID"];
            }
            if (reader.HasColumn("ItemName") && reader["ItemName"] != DBNull.Value)
            {
                p.ItemName = reader["ItemName"].ToString();
            }
            if (reader.HasColumn("HIRepResourceCode") && reader["HIRepResourceCode"] != DBNull.Value)
            {
                p.HIRepResourceCode = reader["HIRepResourceCode"].ToString();
            }

            return p;
        }
        //▲=====#004
        //▼=====#002
        protected virtual Resources GetResourcesObjFromReader(IDataReader reader) 
        {
            Resources p = new Resources();
            try
            {
                //if (reader.HasColumn("RscrID") && reader["RscrID"] != null)
                //{
                //    p.RscrID = (long)reader["RscrID"];
                //}

                //if (reader.HasColumn("DeprecTypeID") && reader["DeprecTypeID"] != null)
                //{
                //    p.DeprecTypeID = Convert.ToInt16(reader["DeprecTypeID"]);
                //}

                //if (reader.HasColumn("RscrGroupID") && reader["RscrGroupID"] != null)
                //{
                //    p.RscrGroupID = (long)reader["RscrGroupID"];
                //}

                //if (reader.HasColumn("RscrTypeID") && reader["RscrTypeID"] != null)
                //{
                //    p.RscrTypeID = (long)reader["RscrTypeID"];
                //}

                //if (reader.HasColumn("SupplierID") && reader["SupplierID"] != null)
                //{
                //    p.SupplierID = (long)reader["SupplierID"];
                //}

                //if (reader.HasColumn("ItemName") && reader["ItemName"] != null)
                //{
                //    p.ItemName = reader["ItemName"].ToString();
                //}

                //if (reader.HasColumn("ItemBrand") && reader["ItemBrand"] != null)
                //{
                //    p.ItemBrand = reader["ItemBrand"].ToString();
                //}

                //if (reader.HasColumn("Functions") && reader["Functions"] != null)
                //{
                //    p.Functions = reader["Functions"].ToString();
                //}

                //if (reader.HasColumn("GeneralTechInfo") && reader["GeneralTechInfo"] != null)
                //{
                //    p.GeneralTechInfo = reader["GeneralTechInfo"].ToString();
                //}
                //Comment lai de test DBNull.Value
                //if (reader.HasColumn("BuyPrice") && reader["BuyPrice"] != null)
                //{
                //    p.BuyPrice = (decimal)reader["BuyPrice"];
                //}

                //if (reader.HasColumn("V_RscrUnit") && reader["V_RscrUnit"] != null)
                //{
                //    p.V_RscrUnit = (long)reader["V_RscrUnit"];
                //}

                //if (reader.HasColumn("OnHIApprovedList") && reader["OnHIApprovedList"] != null)
                //{
                //    p.OnHIApprovedList = (bool)reader["OnHIApprovedList"];
                //}

                //if (reader.HasColumn("WarrantyTime") && reader["WarrantyTime"] != null)
                //{
                //    p.WarrantyTime = Convert.ToInt16(reader["WarrantyTime"]);
                //}

                ////▼=====#001
                ////Kiểm tra xem đọc HIRepResourceCode lên có tồn tại và null không
                //if (reader.HasColumn("HIRepResourceCode") && reader["HIRepResourceCode"] != null)
                //{
                //    p.HIRepResourceCode = reader["HIRepResourceCode"].ToString();
                //}
                ////▲=====#001

                //if (reader.HasColumn("IsLocatable") && reader["IsLocatable"] != null)
                //{
                //    p.IsLocatable = (bool)reader["IsLocatable"];
                //}

                //if (reader.HasColumn("IsDeleted") && reader["IsDeleted"] != null)
                //{
                //    p.IsDeleted = (bool)reader["IsDeleted"];
                //}
                //p.V_UnitLookup = new Lookup();
                //try
                //{
                //    if (reader.HasColumn("V_RscrUnit") && reader["V_RscrUnit"] != null)
                //    {
                //        p.V_UnitLookup.LookupID = (long)reader["V_RscrUnit"];
                //    }

                //    if (reader.HasColumn("V_RscrUnitValue") && reader["V_RscrUnitValue"] != null)
                //    {
                //        p.V_UnitLookup.ObjectValue = reader["V_RscrUnitValue"].ToString();
                //    }

                //    if (reader.HasColumn("V_RscrUnit") && reader["V_RscrUnit"] != null)
                //    {
                //        p.V_UnitLookup.ObjectName = "V_RscrUnit";
                //    }
                //    p.V_UnitLookup.ObjectNotes = "";

                //    p.V_UnitLookup.ObjectTypeID = (long)LookupValues.RESOURCE_UNIT;

                //}
                //catch
                //{
                //    p.V_UnitLookup.LookupID = -1;
                //    p.V_UnitLookup.ObjectValue = "";
                //    p.V_UnitLookup.ObjectName = "V_RscrUnit";
                //    p.V_UnitLookup.ObjectNotes = "";
                //    p.V_UnitLookup.ObjectTypeID = -1;
                //}
                //p.VResourceGroup = new ResourceGroup();
                //try
                //{
                //    if (reader.HasColumn("RscrGroupID") && reader["RscrGroupID"] != null)
                //    {
                //        p.VResourceGroup.RscrGroupID = (long)reader["RscrGroupID"];
                //    }

                //    if (reader.HasColumn("GroupName") && reader["GroupName"] != null)
                //    {
                //        p.VResourceGroup.GroupName = reader["GroupName"].ToString();
                //    }

                //    if (reader.HasColumn("Description") && reader["Description"] != null)
                //    {
                //        p.VResourceGroup.Description = "Description";
                //    }
                //}
                //catch
                //{
                //}
                //p.VResourceType = new ResourceType();
                //try
                //{
                //    if (reader.HasColumn("RscrTypeID") && reader["RscrTypeID"] != null)
                //    {
                //        p.VResourceType.RscrTypeID = (long)reader["RscrTypeID"];
                //    }
                //    if (reader.HasColumn("Description") && reader["Description"] != null)
                //    {
                //        p.VResourceType.Description = "Description";
                //    }

                //    if (reader.HasColumn("TypeName") && reader["TypeName"] != null)
                //    {
                //        p.VResourceType.TypeName = reader["TypeName"].ToString();
                //    }
                //}
                //catch
                //{
                //}

                //p.VSupplier = new Supplier();
                //try
                //{
                //    if (reader.HasColumn("SupplierID") && reader["SupplierID"] != null)
                //    {
                //        p.VSupplier.SupplierID = (long)reader["SupplierID"];
                //    }

                //    if (reader.HasColumn("SupplierName") && reader["SupplierName"] != null)
                //    {
                //        p.VSupplier.SupplierName = reader["SupplierName"].ToString();
                //    }
                //}
                //catch
                //{
                //}


                //if (reader.HasColumn("DepreciationByTimeRate") && reader["DepreciationByTimeRate"] != null)
                //{
                //    p.DepreciationByTimeRate = (float.Parse(reader["DepreciationByTimeRate"].ToString()));
                //}

                //if (reader.HasColumn("DepreciationByUsageRate") && reader["DepreciationByUsageRate"] != null)
                //{
                //    p.DepreciationByUsageRate = (float.Parse(reader["DepreciationByUsageRate"].ToString()));
                //}
                //if (reader.HasColumn("V_ExpenditureSource") && reader["V_ExpenditureSource"] != null)
                //{
                //    p.V_ExpenditureSource = (long)reader["V_ExpenditureSource"];
                //}
                if (reader.HasColumn("RscrID") && reader["RscrID"] != DBNull.Value)
                {
                    p.RscrID = (long)reader["RscrID"];
                }

                if (reader.HasColumn("DeprecTypeID") && reader["DeprecTypeID"] != DBNull.Value)
                {
                    p.DeprecTypeID = Convert.ToInt16(reader["DeprecTypeID"]);
                }

                if (reader.HasColumn("RscrGroupID") && reader["RscrGroupID"] != DBNull.Value)
                {
                    p.RscrGroupID = (long)reader["RscrGroupID"];
                }

                if (reader.HasColumn("RscrTypeID") && reader["RscrTypeID"] != DBNull.Value)
                {
                    p.RscrTypeID = (long)reader["RscrTypeID"];
                }

                if (reader.HasColumn("SupplierID") && reader["SupplierID"] != DBNull.Value)
                {
                    p.SupplierID = (long)reader["SupplierID"];
                }

                if (reader.HasColumn("ItemName") && reader["ItemName"] != DBNull.Value)
                {
                    p.ItemName = reader["ItemName"].ToString();
                }

                if (reader.HasColumn("ItemBrand") && reader["ItemBrand"] != DBNull.Value)
                {
                    p.ItemBrand = reader["ItemBrand"].ToString();
                }

                if (reader.HasColumn("Functions") && reader["Functions"] != DBNull.Value)
                {
                    p.Functions = reader["Functions"].ToString();
                }

                if (reader.HasColumn("GeneralTechInfo") && reader["GeneralTechInfo"] != DBNull.Value)
                {
                    p.GeneralTechInfo = reader["GeneralTechInfo"].ToString();
                }
                if (reader.HasColumn("BuyPrice") && reader["BuyPrice"] != DBNull.Value)
                {
                    p.BuyPrice = (decimal)reader["BuyPrice"];
                }

                if (reader.HasColumn("V_RscrUnit") && reader["V_RscrUnit"] != DBNull.Value)
                {
                    p.V_RscrUnit = (long)reader["V_RscrUnit"];
                }

                if (reader.HasColumn("OnHIApprovedList") && reader["OnHIApprovedList"] != DBNull.Value)
                {
                    p.OnHIApprovedList = (bool)reader["OnHIApprovedList"];
                }

                if (reader.HasColumn("WarrantyTime") && reader["WarrantyTime"] != DBNull.Value)
                {
                    p.WarrantyTime = Convert.ToInt16(reader["WarrantyTime"]);
                }

                //▼=====#001
                //Kiểm tra xem đọc HIRepResourceCode lên có tồn tại và null không
                if (reader.HasColumn("HIRepResourceCode") && reader["HIRepResourceCode"] != DBNull.Value)
                {
                    p.HIRepResourceCode = reader["HIRepResourceCode"].ToString();
                }
                //▲=====#001

                //▼=====#005

                if (reader.HasColumn("V_PCLMainCategory") && reader["V_PCLMainCategory"] != DBNull.Value)

                {
                    p.V_PCLMainCategory = (long)reader["V_PCLMainCategory"];
                }
                //▲=====#005

                if (reader.HasColumn("IsLocatable") && reader["IsLocatable"] != DBNull.Value)
                {
                    p.IsLocatable = (bool)reader["IsLocatable"];
                }

                if (reader.HasColumn("IsDeleted") && reader["IsDeleted"] != DBNull.Value)
                {
                    p.IsDeleted = (bool)reader["IsDeleted"];
                }
                p.V_UnitLookup = new Lookup();
                try
                {
                    if (reader.HasColumn("V_RscrUnit") && reader["V_RscrUnit"] != DBNull.Value)
                    {
                        p.V_UnitLookup.LookupID = (long)reader["V_RscrUnit"];
                    }

                    if (reader.HasColumn("V_RscrUnitValue") && reader["V_RscrUnitValue"] != DBNull.Value)
                    {
                        p.V_UnitLookup.ObjectValue = reader["V_RscrUnitValue"].ToString();
                    }

                    if (reader.HasColumn("V_RscrUnit") && reader["V_RscrUnit"] != DBNull.Value)
                    {
                        p.V_UnitLookup.ObjectName = "V_RscrUnit";
                    }
                    p.V_UnitLookup.ObjectNotes = "";

                    p.V_UnitLookup.ObjectTypeID = (long)LookupValues.RESOURCE_UNIT;

                }
                catch
                {
                    p.V_UnitLookup.LookupID = -1;
                    p.V_UnitLookup.ObjectValue = "";
                    p.V_UnitLookup.ObjectName = "V_RscrUnit";
                    p.V_UnitLookup.ObjectNotes = "";
                    p.V_UnitLookup.ObjectTypeID = -1;
                }
                p.VResourceGroup = new ResourceGroup();
                try
                {
                    if (reader.HasColumn("RscrGroupID") && reader["RscrGroupID"] != DBNull.Value)
                    {
                        p.VResourceGroup.RscrGroupID = (long)reader["RscrGroupID"];
                    }

                    if (reader.HasColumn("GroupName") && reader["GroupName"] != DBNull.Value)
                    {
                        p.VResourceGroup.GroupName = reader["GroupName"].ToString();
                    }

                    if (reader.HasColumn("Description") && reader["Description"] != DBNull.Value)
                    {
                        p.VResourceGroup.Description = "Description";
                    }
                }
                catch
                {
                }
                p.VResourceType = new ResourceType();
                try
                {
                    if (reader.HasColumn("RscrTypeID") && reader["RscrTypeID"] != DBNull.Value)
                    {
                        p.VResourceType.RscrTypeID = (long)reader["RscrTypeID"];
                    }
                    if (reader.HasColumn("Description") && reader["Description"] != DBNull.Value)
                    {
                        p.VResourceType.Description = "Description";
                    }

                    if (reader.HasColumn("TypeName") && reader["TypeName"] != DBNull.Value)
                    {
                        p.VResourceType.TypeName = reader["TypeName"].ToString();
                    }
                }
                catch
                {
                }

                p.VSupplier = new Supplier();
                try
                {
                    if (reader.HasColumn("SupplierID") && reader["SupplierID"] != DBNull.Value)
                    {
                        p.VSupplier.SupplierID = (long)reader["SupplierID"];
                    }

                    if (reader.HasColumn("SupplierName") && reader["SupplierName"] != DBNull.Value)
                    {
                        p.VSupplier.SupplierName = reader["SupplierName"].ToString();
                    }
                }
                catch
                {
                }


                if (reader.HasColumn("DepreciationByTimeRate") && reader["DepreciationByTimeRate"] != DBNull.Value)
                {
                    p.DepreciationByTimeRate = (float.Parse(reader["DepreciationByTimeRate"].ToString()));
                }

                if (reader.HasColumn("DepreciationByUsageRate") && reader["DepreciationByUsageRate"] != DBNull.Value)
                {
                    p.DepreciationByUsageRate = (float.Parse(reader["DepreciationByUsageRate"].ToString()));
                }
                if (reader.HasColumn("V_ExpenditureSource") && reader["V_ExpenditureSource"] != DBNull.Value)
                {
                    p.V_ExpenditureSource = (long)reader["V_ExpenditureSource"];
                }
            }
            catch
            { }

            return p;        
        }
        //▲=====#002

        #endregion

        #region ResourcesAllocation

        public abstract List<Lookup> GetLookupAllocStatus();
        public abstract List<Lookup> GetLookupStorageStatus();
        
        public abstract List<Location> GetAllLocationsByType(int type);
        public abstract List<RefDepartment> GetAllRefDepartmentsByType(int type);

        public abstract List<RefDepartment> GetAllRefDepartments();
        public abstract List<Location> GetAllLocations();
        public abstract List<RoomType> GetAllRoomType();
        public abstract List<DeptLocation> GetAllDeptLocation();
        public abstract List<PCLExamTypeLocation> GetPclExamTypeLocationsByExamTypeList(List<PCLExamType> examTypes);
        public abstract List<DeptLocation> GetDeptLocationByExamType(long examTypeId);
        public abstract List<DeptLocation> GetAllDeptLocationByType(int choice);
        public abstract List<RefStaffCategory> GetRefStaffCategoriesByType(long V_StaffCatType);
        public abstract List<RefStaffCategory> GetAllRefStaffCategories();
        public abstract List<Staff> GetAllStaff(long StaffCatgID);
        public abstract List<Staff> GetAllStaffByStaffCategory(long StaffCategory);
        public abstract List<DeptLocation> GetAllPageDeptLocationByType(int choice, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);
        public abstract List<DeptLocation> GetAllPageDeptLocation(int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);
        public abstract List<ResourceAllocations> GetResourceAllocationsPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);
        public abstract List<ResourceStorages> GetResourceStoragesPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        public abstract List<DeptLocation> GetAllPageDeptLocationGetByChoicePaging(int Choice, long RsID, string Text,
            int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);
        
        #region refDepartment
        protected virtual RefDepartment GetRefDepartmentObjFromReader(IDataReader reader)
        {
            RefDepartment p = new RefDepartment();
            try
            {
                if (reader.HasColumn("DeptID") && reader["DeptID"] != null)
                {
                    p.DeptID = (long)reader["DeptID"];
                }

                if (reader.HasColumn("V_DeptType") && reader["V_DeptType"] != null)
                {
                    p.V_DeptType = (long)reader["V_DeptType"];
                }

                if (reader.HasColumn("DeptName") && reader["DeptName"] != null)
                {
                    p.DeptName = reader["DeptName"].ToString();
                }

                if (reader.HasColumn("DeptDescription") && reader["DeptDescription"] != null)
                {
                    p.DeptDescription = reader["DeptDescription"].ToString();
                }
            }
            catch
            { return null; }
            return p;
        }
        protected virtual List<RefDepartment> GetRefDepartmentCollectionFromReader(IDataReader reader)
        {
            List<RefDepartment> lst = new List<RefDepartment>();
            while (reader.Read())
            {
                lst.Add(GetRefDepartmentObjFromReader(reader));
            }
            return lst;
        }
        #endregion

        // 16/11/2013 Txd: The following 3 methods are NOW replaced by the BASE class DataProviderBase.

        //protected virtual PCLExamTypeLocation GetPCLExamTypeLocationFromReader(IDataReader reader)
        //{
        //    var p = new PCLExamTypeLocation();
        //    p.PCLExamTypeLocID = (long)reader["PCLExamTypeLocID"];
        //    p.PCLExamTypeID = (long)reader["PCLExamTypeID"];
        //    p.DeptLocationID = (long)reader["DeptLocationID"];
        //    p.DeptLocation = GetDepartmentObjFromReader(reader);
        //    return p;
        //}

        //protected virtual DeptLocation GetDepartmentObjFromReader(IDataReader reader)   
        //{
        //    DeptLocation p = new DeptLocation();
        //    try
        //    {
        //        if (reader.HasColumn("LID") && reader["LID"] != null)
        //        {
        //            p.LID = (long)reader["LID"];
        //        }

        //        if (reader.HasColumn("DeptID") && reader["DeptID"] != null)
        //        {
        //            p.DeptID = (long)reader["DeptID"];
        //        }

        //        if (reader.HasColumn("DeptLocationID") && reader["DeptLocationID"] != null)
        //        {
        //            p.DeptLocationID = (long)reader["DeptLocationID"];
        //        }

        //        p.RefDepartment = new RefDepartment();
        //        try
        //        {
        //            if (reader.HasColumn("DeptName") && reader["DeptName"] != null)
        //            {
        //                p.RefDepartment.DeptName = reader["DeptName"].ToString();
        //            }

        //            if (reader.HasColumn("DeptDescription") && reader["DeptDescription"] != null)
        //            {
        //                p.RefDepartment.DeptDescription = reader["DeptDescription"].ToString();
        //            }

        //            if (reader.HasColumn("V_DeptType") && reader["V_DeptType"] != null)
        //            {
        //                p.RefDepartment.V_DeptType = (long)reader["V_DeptType"];
        //            }
        //        }
        //        catch { }

        //        p.Location = new Location();
        //        try
        //        {
        //            if (reader.HasColumn("LocationName") && reader["LocationName"] != null)
        //            {
        //                p.Location.LocationName = reader["LocationName"].ToString();
        //            }

        //            if (reader.HasColumn("LocationDescription") && reader["LocationDescription"] != null)
        //            {
        //                p.Location.LocationDescription = reader["LocationDescription"].ToString();
        //            }
        //            p.Location.RoomType = new RoomType();
        //            if (reader.HasColumn("RmTypeName") && reader["RmTypeName"] != null)
        //            {
        //                p.Location.RoomType.RmTypeName = reader["RmTypeName"].ToString();
        //            }

        //            if (reader.HasColumn("RmTypeDescription") && reader["RmTypeDescription"] != null)
        //            {
        //                p.Location.RoomType.RmTypeDescription = reader["RmTypeDescription"].ToString();
        //            }
        //        }
        //        catch { }
        //    }
        //    catch
        //    { return null; }
        //    return p;
        //}

        //protected virtual List<PCLExamTypeLocation> GetPCLExamTypeLocationCollectionFromReader(IDataReader reader)
        //{
        //    List<PCLExamTypeLocation> lst = new List<PCLExamTypeLocation>();
        //    while (reader.Read())
        //    {
        //        lst.Add(GetPCLExamTypeDeptLocInfoFromReader(reader));
        //    }
        //    return lst;
        //}


        protected virtual List<DeptLocation> GetDepartmentCollectionFromReader(IDataReader reader)
        {
            List<DeptLocation> lst = new List<DeptLocation>();
            while (reader.Read())
            {
                lst.Add(GetDeptLocAndDeptInfoFromReader(reader));
            }
            return lst;
        }
        protected virtual ResourceAllocations GetResourceAllocationsObjFromReader(IDataReader reader)
        {
            ResourceAllocations p = new ResourceAllocations();
            try
            {
                if (reader.HasColumn("RscrAllocID") && reader["RscrAllocID"] != null)
                {
                    p.RscrAllocID = (long)reader["RscrAllocID"];
                }

                if (reader.HasColumn("RscrID") && reader["RscrID"] != null)
                {
                    p.RscrID = (long)reader["RscrID"];
                }

                if (reader.HasColumn("RscrGUID") && reader["RscrGUID"] != null)
                {
                    p.RscrGUID = reader["RscrGUID"].ToString();
                }

                if (reader.HasColumn("DeptLocationID") && reader["DeptLocationID"] != null)
                {
                    p.DeptLocationID = (long)reader["DeptLocationID"];
                }

                if (reader.HasColumn("RscrMoveRequestID") && reader["RscrMoveRequestID"] != null)
                {
                    p.RscrMoveRequestID = (long)reader["RscrMoveRequestID"];
                }

                if (reader.HasColumn("AllocStaffID") && reader["AllocStaffID"] != null)
                {
                    p.AllocStaffID = (long)reader["AllocStaffID"];
                }

                if (reader.HasColumn("ResponsibleStaffID") && reader["ResponsibleStaffID"] != null)
                {
                    p.ResponsibleStaffID = (long)reader["ResponsibleStaffID"];
                }

                if (reader.HasColumn("RecDateCreated") && reader["RecDateCreated"] != null)
                {
                    p.RecDateCreated = (DateTime)reader["RecDateCreated"];
                }

                if (reader.HasColumn("AllocDate") && reader["AllocDate"] != null)
                {
                    p.AllocDate = (DateTime?)reader["AllocDate"];
                }

                if (reader.HasColumn("StartUseDate") && reader["StartUseDate"] != null)
                {
                    p.StartUseDate = (DateTime?)reader["StartUseDate"];
                }

                if (reader.HasColumn("V_AllocStatus") && reader["V_AllocStatus"] != null)
                {
                    p.V_AllocStatus = (long)reader["V_AllocStatus"];
                }

                if (reader.HasColumn("HasIdentity") && reader["HasIdentity"] != null)
                {
                    p.HasIdentity = (bool)reader["HasIdentity"];
                }

                if (reader.HasColumn("IsActive") && reader["IsActive"] != null)
                {
                    p.IsActive = (bool)reader["IsActive"];
                }

                if (reader.HasColumn("RscrCode") && reader["RscrCode"] != null)
                {
                    p.RscrCode = reader["RscrCode"].ToString();
                }

                if (reader.HasColumn("RscrBarcode") && reader["RscrBarcode"] != null)
                {
                    p.RscrBarcode = reader["RscrBarcode"].ToString();
                }

                if (reader.HasColumn("SerialNumber") && reader["SerialNumber"] != null)
                {
                    p.SerialNumber = reader["SerialNumber"].ToString();
                }

                if (reader.HasColumn("QtyAlloc") && reader["QtyAlloc"] != null)
                {
                    p.QtyAlloc = Convert.ToInt32(reader["QtyAlloc"].ToString());
                }

                if (reader.HasColumn("QtyInUse") && reader["QtyInUse"] != null)
                {
                    p.QtyInUse = Convert.ToInt32(reader["QtyInUse"].ToString());
                }
                p.VAllocStatus=new Lookup();
                try 
                {
                    if (reader.HasColumn("V_AllocStatus") && reader["V_AllocStatus"] != null)
                    {
                        p.VAllocStatus.LookupID = (long)reader["V_AllocStatus"];
                    }

                    if (reader.HasColumn("V_AllocStatusValue") && reader["V_AllocStatusValue"] != null)
                    {
                        p.VAllocStatus.ObjectValue = reader["V_AllocStatusValue"].ToString();
                    }
                }
                catch
                {}
                p.VResources = new Resources();
                try 
                {
                    if (reader.HasColumn("RscrID") && reader["RscrID"] != null)
                    {
                        p.VResources.RscrID = (long)reader["RscrID"];
                    }

                    if (reader.HasColumn("ItemName") && reader["ItemName"] != null)
                    {
                        p.VResources.ItemName = reader["ItemName"].ToString();
                    }

                    if (reader.HasColumn("ItemBrand") && reader["ItemBrand"] != null)
                    {
                        p.VResources.ItemBrand = reader["ItemBrand"].ToString();
                    }
                }
                catch
                {}

                p.VResponsibleStaff = new Staff();
                try 
                {
                    if (reader.HasColumn("ResponsibleStaffID") && reader["ResponsibleStaffID"] != null)
                    {
                        p.VResponsibleStaff.StaffID = (long)reader["ResponsibleStaffID"];
                    }

                    if (reader.HasColumn("ResponsibleFullName") && reader["ResponsibleFullName"] != null)
                    {
                        p.VResponsibleStaff.FullName = reader["ResponsibleFullName"].ToString();
                    }
                }
                catch
                {}
                p.VAllocStaff = new Staff();
                try
                {
                    if (reader.HasColumn("AllocStaffID") && reader["AllocStaffID"] != null)
                    {
                        p.VAllocStaff.StaffID = (long)reader["AllocStaffID"];
                    }

                    if (reader.HasColumn("AllocFullName") && reader["AllocFullName"] != null)
                    {
                        p.VAllocStaff.FullName = reader["AllocFullName"].ToString();
                    }
                }
                catch
                { }
                p.VRscrMoveRequest = new ResourceMoveRequests();
                try
                {

                    if (reader.HasColumn("RscrMoveRequestID") && reader["RscrMoveRequestID"] != null)
                    {
                        p.VRscrMoveRequest.RscrMoveRequestID = (long)reader["RscrMoveRequestID"];
                    }
                }
                catch { }

                p.VDeptLocation = new DeptLocation();
                try
                {
                    p.VDeptLocation.RefDepartment = new RefDepartment();
                    try 
                    {
                        if (reader.HasColumn("DeptName") && reader["DeptName"] != null)
                        {
                            p.VDeptLocation.RefDepartment.DeptName = reader["DeptName"].ToString();
                        }

                        if (reader.HasColumn("DeptDescription") && reader["DeptDescription"] != null)
                        {
                            p.VDeptLocation.RefDepartment.DeptDescription = reader["DeptDescription"].ToString();
                        }

                        if (reader.HasColumn("V_DeptType") && reader["V_DeptType"] != null)
                        {
                            p.VDeptLocation.RefDepartment.V_DeptType = (long)reader["V_DeptType"];
                        }
                    }
                    catch 
                    { }
                    p.VDeptLocation.Location = new Location();
                    try
                    {
                        if (reader.HasColumn("LocationName") && reader["LocationName"] != null)
                        {
                            p.VDeptLocation.Location.LocationName = reader["LocationName"].ToString();
                        }

                        if (reader.HasColumn("LocationDescription") && reader["LocationDescription"] != null)
                        {
                            p.VDeptLocation.Location.LocationDescription = reader["LocationDescription"].ToString();
                        }
                        p.VDeptLocation.Location.RoomType = new RoomType();

                        if (reader.HasColumn("RmTypeName") && reader["RmTypeName"] != null)
                        {
                            p.VDeptLocation.Location.RoomType.RmTypeName = reader["RmTypeName"].ToString();
                        }

                        if (reader.HasColumn("RmTypeDescription") && reader["RmTypeDescription"] != null)
                        {
                            p.VDeptLocation.Location.RoomType.RmTypeDescription = reader["RmTypeDescription"].ToString();
                        }
                    }
                    catch { }
                }
                catch { }

                
            }
            catch
            { return null; }
            return p;
        }
        protected virtual ResourceStorages GetResourceStoragesObjFromReader(IDataReader reader)
        {
            ResourceStorages p = new ResourceStorages();
            try
            {
                if (reader.HasColumn("RscrStorageID") && reader["RscrStorageID"] != null)
                {
                    p.RscrStorageID = (long)reader["RscrStorageID"];
                }

                if (reader.HasColumn("RscrGUID") && reader["RscrGUID"] != null)
                {
                    p.RscrGUID = reader["RscrGUID"].ToString();
                }

                if (reader.HasColumn("RscrID") && reader["RscrID"] != null)
                {
                    p.RscrID = (long)reader["RscrID"];
                }

                if (reader.HasColumn("DeptLocationID") && reader["DeptLocationID"] != null)
                {
                    p.DeptLocationID = (long)reader["DeptLocationID"];
                }

                if (reader.HasColumn("RscrMoveRequestID") && reader["RscrMoveRequestID"] != null)
                {
                    p.RscrMoveRequestID = (long)reader["RscrMoveRequestID"];
                }

                if (reader.HasColumn("StorageStaffID") && reader["StorageStaffID"] != null)
                {
                    p.StorageStaffID = (long)reader["StorageStaffID"];
                }

                if (reader.HasColumn("ResponsibleStaffID") && reader["ResponsibleStaffID"] != null)
                {
                    p.ResponsibleStaffID = (long)reader["ResponsibleStaffID"];
                }

                if (reader.HasColumn("RecDateCreated") && reader["RecDateCreated"] != null)
                {
                    p.RecDateCreated = (DateTime)reader["RecDateCreated"];
                }

                if (reader.HasColumn("StorageDate") && reader["StorageDate"] != null)
                {
                    p.StorageDate = (DateTime?)reader["StorageDate"];
                }
                //p.StartUseDate = (DateTime?)reader["StartUseDate"];

                if (reader.HasColumn("V_StorageStatus") && reader["V_StorageStatus"] != null)
                {
                    p.V_StorageStatus = (long)reader["V_StorageStatus"];
                }

                if (reader.HasColumn("HasIdentity") && reader["HasIdentity"] != null)
                {
                    p.HasIdentity = (bool)reader["HasIdentity"];
                }

                if (reader.HasColumn("IsActive") && reader["IsActive"] != null)
                {
                    p.IsActive = (bool)reader["IsActive"];
                }

                if (reader.HasColumn("IsDeleted") && reader["IsDeleted"] != null)
                {
                    p.IsDeleted = (bool)reader["IsDeleted"];
                }

                if (reader.HasColumn("RscrCode") && reader["RscrCode"] != null)
                {
                    p.RscrCode = reader["RscrCode"].ToString();
                }

                if (reader.HasColumn("RscrBarcode") && reader["RscrBarcode"] != null)
                {
                    p.RscrBarcode = reader["RscrBarcode"].ToString();
                }

                if (reader.HasColumn("SerialNumber") && reader["SerialNumber"] != null)
                {
                    p.SerialNumber = reader["SerialNumber"].ToString();
                }

                if (reader.HasColumn("QtyStorage") && reader["QtyStorage"] != null)
                {
                    p.QtyStorage = Convert.ToInt32(reader["QtyStorage"].ToString());
                }

                if (reader.HasColumn("V_StorageReason") && reader["V_StorageReason"] != null)
                {
                    p.V_StorageReason = (long)reader["V_StorageReason"];
                }
                p.VStorageStatus = new Lookup();
                try
                {

                    if (reader.HasColumn("V_StorageStatus") && reader["V_StorageStatus"] != null)
                    {
                        p.VStorageStatus.LookupID = (long)reader["V_StorageStatus"];
                    }

                    if (reader.HasColumn("V_StorageStatusValue") && reader["V_StorageStatusValue"] != null)
                    {
                        p.VStorageStatus.ObjectValue = reader["V_StorageStatusValue"].ToString();
                    }
                }
                catch
                { }
                p.VResources = new Resources();
                try
                {
                    if (reader.HasColumn("RscrID") && reader["RscrID"] != null)
                    {
                        p.VResources.RscrID = (long)reader["RscrID"];
                    }

                    if (reader.HasColumn("ItemName") && reader["ItemName"] != null)
                    {
                        p.VResources.ItemName = reader["ItemName"].ToString();
                    }

                    if (reader.HasColumn("ItemBrand") && reader["ItemBrand"] != null)
                    {
                        p.VResources.ItemBrand = reader["ItemBrand"].ToString();
                    }
                }
                catch
                { }

                p.VResponsibleStaff = new Staff();
                try
                {
                    if (reader.HasColumn("ResponsibleStaffID") && reader["ResponsibleStaffID"] != null)
                    {
                        p.VResponsibleStaff.StaffID = (long)reader["ResponsibleStaffID"];
                    }

                    if (reader.HasColumn("ResponsibleFullName") && reader["ResponsibleFullName"] != null)
                    {
                        p.VResponsibleStaff.FullName = reader["ResponsibleFullName"].ToString();
                    }
                }
                catch
                { }
                p.VStorageStaff= new Staff();
                try
                {
                    if (reader.HasColumn("StorageStaffID") && reader["StorageStaffID"] != null)
                    {
                        p.VStorageStaff.StaffID = (long)reader["StorageStaffID"];
                    }

                    if (reader.HasColumn("StorageFullName") && reader["StorageFullName"] != null)
                    {
                        p.VStorageStaff.FullName = reader["StorageFullName"].ToString();
                    }
                }
                catch
                { }
                p.VRscrMoveRequest = new ResourceMoveRequests();
                try
                {
                    if (reader.HasColumn("RscrMoveRequestID") && reader["RscrMoveRequestID"] != null)
                    {
                        p.VRscrMoveRequest.RscrMoveRequestID = (long)reader["RscrMoveRequestID"];                        
                    }
                }
                catch { }
                p.VDeptLocation = new DeptLocation();
                try
                {
                    p.VDeptLocation.RefDepartment = new RefDepartment();
                    try
                    {

                        if (reader.HasColumn("DeptName") && reader["DeptName"] != null)
                        {
                            p.VDeptLocation.RefDepartment.DeptName = reader["DeptName"].ToString();
                        }

                        if (reader.HasColumn("DeptDescription") && reader["DeptDescription"] != null)
                        {
                            p.VDeptLocation.RefDepartment.DeptDescription = reader["DeptDescription"].ToString();
                        }

                        if (reader.HasColumn("V_DeptType") && reader["V_DeptType"] != null)
                        {
                            p.VDeptLocation.RefDepartment.V_DeptType = (long)reader["V_DeptType"];
                        }
                    }
                    catch
                    { }
                    p.VDeptLocation.Location = new Location();
                    try
                    {
                        if (reader.HasColumn("LocationName") && reader["LocationName"] != null)
                        {
                            p.VDeptLocation.Location.LocationName = reader["LocationName"].ToString();
                        }

                        if (reader.HasColumn("LocationDescription") && reader["LocationDescription"] != null)
                        {
                            p.VDeptLocation.Location.LocationDescription = reader["LocationDescription"].ToString();
                        }
                        p.VDeptLocation.Location.RoomType = new RoomType();

                        if (reader.HasColumn("RmTypeName") && reader["RmTypeName"] != null)
                        {
                            p.VDeptLocation.Location.RoomType.RmTypeName = reader["RmTypeName"].ToString();
                        }

                        if (reader.HasColumn("RmTypeDescription") && reader["RmTypeDescription"] != null)
                        {
                            p.VDeptLocation.Location.RoomType.RmTypeDescription = reader["RmTypeDescription"].ToString();
                        }
                    }
                    catch { }
                }
                catch { }
            }
            catch 
            { return null; }
            return p;
        }
        protected virtual List<ResourceStorages> GetResourceStoragesFromReader(IDataReader reader)
        {
            List<ResourceStorages> lst = new List<ResourceStorages>();
            while (reader.Read())
            {
                lst.Add(GetResourceStoragesObjFromReader(reader));
            }
            return lst;
        }
        protected virtual List<ResourceAllocations> GetResourceAllocationsFromReader(IDataReader reader)
        {
            List<ResourceAllocations> lst = new List<ResourceAllocations>();
            while (reader.Read())
            {
                lst.Add(GetResourceAllocationsObjFromReader(reader));
            }
            return lst;
        }
        public abstract bool AddNewStoragesAllocations(
                                                    long RscrID
                                                    , String GuidID
                                                    , long DeptLocationID
                                                   , long RscrMoveRequestID
                                                   , long StorageStaffID
                                                   , DateTime? StorageDate
                                                    , long V_StorageStatus
                                                    , long V_StorageReason
                                                   , bool HasIdentity
                                                   , string RscrCode
                                                   , string RscrBarcode
                                                   , string SerialNumber
                                                   , int QtyStorage
            //,int QtyInUse 
                                                   , long ResponsibleStaffID
                                                   , bool IsActive
                                                , bool IsDeleted);
        public abstract bool AddNewResourceAllocations(
                                                    long RscrID
                                                    , String GuidID
                                                    ,long DeptLocationID
                                                   , long RscrMoveRequestID
                                                   , long AllocStaffID
                                                   , DateTime? AllocDate
                                                   , DateTime? StartUseDate
                                                   , long V_AllocStatus
                                                   , bool HasIdentity
                                                   , string RscrCode
                                                   , string RscrBarcode
                                                   , string SerialNumber
                                                   , int QtyAlloc
                                                   , int QtyInUse
                                                   , long ResponsibleStaffID
                                                   , bool IsActive);

        public abstract bool UpdateResourceAllocations(long RscrAllocID
                                                    ,long RscrID
                                                    , String GuidID
                                                    ,long DeptLocationID
                                                   , long RscrMoveRequestID
                                                   , long AllocStaffID
                                                   , DateTime? AllocDate
                                                   , DateTime? StartUseDate
                                                   , long V_AllocStatus
                                                   , bool HasIdentity
                                                   , string RscrCode
                                                   , string RscrBarcode
                                                   , string SerialNumber
                                                   , int QtyAlloc
                                                   , int QtyInUse
                                                   , long ResponsibleStaffID
                                                   , bool IsActive);

        public abstract bool UpdateResourceStorages(
                                                    long RscrStorageID
                                                    , long RscrID
                                                    , string RscrGUID
                                                    , long DeptLocationID
                                                    , DateTime RecDateCreated
                                                    , long RscrMoveRequestID
                                                    , long StorageStaffID
                                                    , DateTime? StorageDate
                                                    , long V_StorageStatus
                                                    , long V_StorageReason
                                                    , bool HasIdentity
                                                    , string RscrCode
                                                    , String RscrBarcode
                                                    , string SerialNumber
                                                    , int QtyStorage
                                                    , long ResponsibleStaffID
                                                    , bool IsActive
                                                    , bool IsDeleted);
        public abstract bool ResourceAllocationsTranferExec(int Choice
                                                        , long RscrAllocID
                                                                , String GuidID
                                                                , long RscrID
                                                               , long DeptLocationID
                                                               , long RscrMoveRequestID
                                                               , long AllocStaffID
                                                               , DateTime? AllocDate
                                                               , DateTime? StartUseDate
                                                               , long V_AllocStatus
                                                               , bool HasIdentity
                                                               , string RscrCode
                                                               , string RscrBarcode
                                                               , string SerialNumber
                                                                , int NewQtyAlloc
                                                                , int NewQtyInUse
                                                               , long ResponsibleStaffID
                                                               , bool IsActive
            //------------
                                                                , long V_StorageReason 
                                                                , long RequestStaffID
                                                               , string MoveReason
                                                               , DateTime? EffectiveMoveDate
                                                               , long FromDeptLocID
                                                               , long ToDeptLocID
                                                               , long ApprovedStaffID
                                                               , string Note
                                                               
            //------------
                                                               , int QtyAllocEx
                                                               , int QtyInUseEx
            // department moi
                                                               );

        #endregion
        #region New Resource Allocation

        public abstract bool AddNewResourceProperty(long RscrID
                                                    , string RscrGUID
                                                    , bool HasIdentity
                                                    , string RscrCode
                                                    , string RscrBarcode
                                                    , string SerialNumber
                                                    , int QtyAlloc
                                                    , bool IsActive
                                                    , bool IsDeleted
            //location
                                                       , long DeptLocationID
                                                       , long RscrMoveRequestID
                                                       , long AllocStaffID
                                                       , DateTime? AllocDate
                                                       , DateTime? StartUseDate
                                                       , long V_AllocStatus
                                                        , long V_StorageReason
                                                       , long ResponsibleStaffID);
        public abstract bool MoveResourcePropertyLocation(long RscrPropLocID
                                                    , long RscrPropertyID

                                                       , long DeptLocationID
                                                       , long RscrMoveRequestID
                                                       , long AllocStaffID
                                                       , DateTime? AllocDate
                                                       , DateTime? StartUseDate
                                                       , long V_AllocStatus
                                                        , long V_StorageReason
                                                       , long ResponsibleStaffID);
        public abstract bool BreakResourceProperty(List<ResourcePropLocations> lstResourcePropLocations, long RscrPropertyID, long RscrPropLocID);
        public abstract List<ResourcePropLocations> GetResourcePropLocationsPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);
        public abstract List<ResourcePropLocations> GetResourcePropLocationsPagingByVType(long Choice, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);
        public abstract List<ResourcePropLocations> GetResourcePropLocationsPagingByFilter(int Choice, string RscrGUID, long RscrPropertyID
             , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);

        public abstract long GetResourcePropertyFilterSum(int Choice, string RscrGUID, long RscrID);
        protected virtual ResourcePropLocations GetResourcePropLocationsObjFromReader(IDataReader reader)
        {
            ResourcePropLocations p = new ResourcePropLocations();
            
            try
            {
                if (reader.HasColumn("RscrPropLocID") && reader["RscrPropLocID"] != null)
                {
                    p.RscrPropLocID = (long)reader["RscrPropLocID"];
                }

                if (reader.HasColumn("AllocDate") && reader["AllocDate"] != null)
                {
                    p.AllocDate = (DateTime)reader["AllocDate"];
                }

                if (reader.HasColumn("StartUseDate") && reader["StartUseDate"] != null)
                {
                    p.StartUseDate = (DateTime)reader["StartUseDate"];
                }

                if (reader.HasColumn("IsActive") && reader["IsActive"] != null)
                {
                    p.IsActive = (bool)reader["IsActive"];
                }

                if (reader.HasColumn("IsDeleted") && reader["IsDeleted"] != null)
                {
                    p.IsDeleted = (bool)reader["IsDeleted"];
                } 
                p.VRscrProperty = new ResourceProperty();
                try 
                {

                    if (reader.HasColumn("RscrPropertyID") && reader["RscrPropertyID"] != null)
                    {
                        p.VRscrProperty.RscrPropertyID = (long)reader["RscrPropertyID"];
                    }

                    if (reader.HasColumn("RscrGUID") && reader["RscrGUID"] != null)
                    {
                        p.VRscrProperty.RscrGUID = reader["RscrGUID"].ToString();
                    }

                    if (reader.HasColumn("RscrID") && reader["RscrID"] != null)
                    {
                        p.VRscrProperty.RscrID = (long)reader["RscrID"];
                    }

                    if (reader.HasColumn("RecDateCreated") && reader["RecDateCreated"] != null)
                    {
                        p.VRscrProperty.RecDateCreated = (DateTime)reader["RecDateCreated"];
                    }

                    if (reader.HasColumn("HasIdentity") && reader["HasIdentity"] != null)
                    {
                        p.VRscrProperty.HasIdentity = (bool)reader["HasIdentity"];
                    }

                    if (reader.HasColumn("RscrCode") && reader["RscrCode"] != null)
                    {
                        p.VRscrProperty.RscrCode = reader["RscrCode"].ToString();
                    }

                    if (reader.HasColumn("RscrBarcode") && reader["RscrBarcode"] != null)
                    {
                        p.VRscrProperty.RscrBarcode = reader["RscrBarcode"].ToString();
                    }

                    if (reader.HasColumn("SerialNumber") && reader["SerialNumber"] != null)
                    {
                        p.VRscrProperty.SerialNumber = reader["SerialNumber"].ToString();
                    }

                    if (reader.HasColumn("QtyAlloc") && reader["QtyAlloc"] != null)
                    {
                        p.VRscrProperty.QtyAlloc = Convert.ToInt32(reader["QtyAlloc"]);
                    }

                    if (reader.HasColumn("IsActive") && reader["IsActive"] != null)
                    {
                        p.VRscrProperty.IsActive = (bool)reader["IsActive"];
                    }

                    if (reader.HasColumn("IsDeleted") && reader["IsDeleted"] != null)
                    {
                        p.VRscrProperty.IsDelete = (bool)reader["IsDeleted"];
                    }

                    if (reader.HasColumn("RscrPropertyID") && reader["RscrPropertyID"] != null)
                    {
                        p.VRscrProperty.RscrPropertyID = (long)reader["RscrPropertyID"];
                    }
                    p.VRscrProperty.VResources = new Resources();
                    try
                    {

                        if (reader.HasColumn("RscrID") && reader["RscrID"] != null)
                        {
                            p.VRscrProperty.VResources.RscrID = (long)reader["RscrID"];
                        }

                        if (reader.HasColumn("ItemName") && reader["ItemName"] != null)
                        {
                            p.VRscrProperty.VResources.ItemName = reader["ItemName"].ToString();
                        }

                        if (reader.HasColumn("ItemBrand") && reader["ItemBrand"] != null)
                        {
                            p.VRscrProperty.VResources.ItemBrand = reader["ItemBrand"].ToString();
                        }

                    }
                    catch
                    { }
           
                }
                catch { }					
					   
                p.VAllocStatus = new Lookup();
                try
                {

                    if (reader.HasColumn("V_AllocStatus") && reader["V_AllocStatus"] != null)
                    {
                        p.VAllocStatus.LookupID = (long)reader["V_AllocStatus"];
                    }

                    if (reader.HasColumn("AllocStatusValue") && reader["AllocStatusValue"] != null)
                    {
                        p.VAllocStatus.ObjectValue = reader["AllocStatusValue"].ToString();
                    }

                }
                catch
                { }

                p.VStorageReason = new Lookup();
                try
                {
                    if (reader.HasColumn("V_StorageReason") && reader["V_StorageReason"] != null)
                    {
                        p.VStorageReason.LookupID = (long)reader["V_StorageReason"];
                    }

                    if (reader.HasColumn("StorageReasonValue") && reader["StorageReasonValue"] != null)
                    {
                        p.VStorageReason.ObjectValue = reader["StorageReasonValue"].ToString();
                    }

                }
                catch
                { }

                p.VResponsibleStaff = new Staff();
                try
                {
                    if (reader.HasColumn("ResponsibleStaffID") && reader["ResponsibleStaffID"] != null)
                    {
                        p.VResponsibleStaff.StaffID = (long)reader["ResponsibleStaffID"];
                    }

                    if (reader.HasColumn("ResponsibleFullName") && reader["ResponsibleFullName"] != null)
                    {
                        p.VResponsibleStaff.FullName = reader["ResponsibleFullName"].ToString();
                    }

                }
                catch
                { }
                p.VAllocStaff = new Staff();
                try
                {
                    if (reader.HasColumn("AllocStaffID") && reader["AllocStaffID"] != null)
                    {
                        p.VAllocStaff.StaffID = (long)reader["AllocStaffID"];
                    }

                    if (reader.HasColumn("AllocStaffFullName") && reader["AllocStaffFullName"] != null)
                    {
                        p.VAllocStaff.FullName = reader["AllocStaffFullName"].ToString();
                    }
                }
                catch
                { }
                p.VRscrMoveRequest = new ResourceMoveRequests();
                try
                {
                    if (reader.HasColumn("RscrMoveRequestID") && reader["RscrMoveRequestID"] != null)
                    {
                        p.VRscrMoveRequest.RscrMoveRequestID = (long)reader["RscrMoveRequestID"];
                    }

                }
                catch { }
                p.VDeptLocation = new DeptLocation();
                try
                {                    
                    p.VDeptLocation.RefDepartment = new RefDepartment();

                    try
                    {
                        if (reader.HasColumn("DeptName") && reader["DeptName"] != null)
                        {
                            p.VDeptLocation.RefDepartment.DeptName = reader["DeptName"].ToString();
                        }


                        if (reader.HasColumn("DeptDescription") && reader["DeptDescription"] != null)
                        {
                            p.VDeptLocation.RefDepartment.DeptDescription = reader["DeptDescription"].ToString();
                        }


                        if (reader.HasColumn("V_DeptType") && reader["V_DeptType"] != null)
                        {
                            p.VDeptLocation.RefDepartment.V_DeptType = (long)reader["V_DeptType"];
                        }

                    }
                    catch
                    { }
                    p.VDeptLocation.Location = new Location();
                    try
                    {
                        if (reader.HasColumn("LocationName") && reader["LocationName"] != null)
                        {
                            p.VDeptLocation.Location.LocationName = reader["LocationName"].ToString();
                        }

                        if (reader.HasColumn("LocationDescription") && reader["LocationDescription"] != null)
                        {
                            p.VDeptLocation.Location.LocationDescription = reader["LocationDescription"].ToString();
                        }

                        p.VDeptLocation.Location.RoomType = new RoomType();

                        if (reader.HasColumn("RmTypeName") && reader["RmTypeName"] != null)
                        {
                            p.VDeptLocation.Location.RoomType.RmTypeName = reader["RmTypeName"].ToString();
                        }

                        p.VDeptLocation.Location.RoomType.RmTypeDescription = reader["RmTypeDescription"].ToString();
                        if (reader.HasColumn("RmTypeDescription") && reader["RmTypeDescription"] != null)
                        {
                            p.VDeptLocation.Location.RoomType.RmTypeDescription = reader["RmTypeDescription"].ToString();
                        }

                    }
                    catch { }

                    if (reader.HasColumn("DeptLocationID") && reader["DeptLocationID"] != null)
                    {
                        p.VDeptLocation.DeptLocationID = (long)reader["DeptLocationID"];
                    }

                    if (reader.HasColumn("DeptID") && reader["DeptID"] != null)
                    {
                        p.VDeptLocation.DeptID = (long)reader["DeptID"];
                    }

                    if (reader.HasColumn("LID") && reader["LID"] != null)
                    {
                        p.VDeptLocation.LID = (long)reader["LID"];
                    }

                }
                catch { }
            }
            catch
            { return null; }
            return p;
        }
        protected virtual List<ResourcePropLocations> GetResourcePropLocationsFromReader(IDataReader reader)
        {
            List<ResourcePropLocations> lst = new List<ResourcePropLocations>();
            while (reader.Read())
            {
                lst.Add(GetResourcePropLocationsObjFromReader(reader));
            }
            return lst;
        }


        #endregion
        


        #region ResourceMaintenance
        public abstract List<ResourceMaintenanceLogStatus> GetspResourceMaintenanceLogStatusByID(long RscrMaintLogID
             , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total);


        public abstract List<Staff> GetStaffs_All();

        public abstract List<Supplier> GetSupplierIsMaintenance_All();

        public abstract bool AddNewResourceMaintenanceLog(ResourceMaintenanceLog obj, out long RscrMaintLogID);        
  
        public abstract List<ResourceMaintenanceLog> GetResourceMaintenanceLogSearch_Paging(
            ResourceMaintenanceLogSearchCriteria Criteria, 
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );

        
        public abstract bool ResourceMaintenanceLogCanEdit(Int64 RscrMaintLogID);
        
        public abstract ResourceMaintenanceLog GetResourceMaintenanceLogByID(Int64 RscrMaintLogID);        

        public abstract bool SaveResourceMaintenanceLogOfFix(
            Int64 RscrMaintLogID,
            Nullable<Int64> FixStaffID,
            Nullable<Int64> FixSupplierID,
            DateTime FixDate,
            string FixSolutions,
            string FixComments,
            Int64 VerifiedStaffID,
            Int64 V_CurrentStatus
            );

        public abstract bool UpdateResourceMaintenanceLog_FinalStatus(
            Int64 RscrMaintLogID,
            Int64 VerifiedStaffID,
            Int64 V_CurrentStatus);

        public abstract bool CheckRscrDiBaoTri(Int64 RscrPropertyID);

        public abstract bool ResourceMaintenanceLog_Delete(Int64 RscrMaintLogID);
        
        public abstract bool GetCheckForVerified(Int64 RscrMaintLogID);

        public abstract bool DeleteAndCreateNewResourceMaintenanceLog(Int64 RscrMaintLogID_Delete, ResourceMaintenanceLog obj);
        
        public abstract List<ResourceMaintenanceLogStatus> GetResourceMaintenanceLogStatus_Detail_Paging(
            ResourceMaintenanceLogStatusSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );

        public abstract DateTime GetFixDateLast(Int64 RscrMaintLogID);

        public abstract bool AddNewResourceMaintenanceLogStatus(ResourceMaintenanceLogStatus obj);            


        public abstract bool AddResourceMaintenanceLogStatus_New(ResourceMaintenanceLogStatus obj);

        public abstract bool DeleteResourceMaintenanceLogStatus(Int64 RscrMainLogStatusID);

        #endregion

    }
}
