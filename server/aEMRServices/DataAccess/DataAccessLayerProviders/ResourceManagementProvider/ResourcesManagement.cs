/*
 * 20180529 #001: TTM Thêm parameter HIRepResourceCode
 * 20180607 #002: TTM Thay đổi toàn bộ điều kiện kiểm tra của các thuộc tính trong hàm GetResourcesObjFromReader (từ kiểm tra != null sang kiểm tra != DBNull.Value)
 * 20180607 #003: Thêm 2 hàm abstract GetAllResources và GetResourcesForMedicalServices_LoadNotPaging
 * 20180307 #004: Thêm hàm trang thiết bị máy để hàm GetResourcesForMedicalServices_LoadNotPaging sử dụng.
 * 20180307 #005: Thêm parameter để đọc V_PCLMainCategory 
* 20230424 #006 DatTB: 
* + Gộp view/model thêm mới và chỉnh sửa lại
* + Thay đổi cách truyền biến một số function
* + Thêm function xuất excel thiết bị 
* 20230501 #007 DatTB:
* + Model thiết bị đổi từ ItemBrand sang ResourceCode
* + Lưu thêm parameter V_PCLMainCategory
* 20230609 #008 DatTB: Thêm cột tên tiếng anh
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
using eHCMS.DAL;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace aEMR.DataAccessLayer.Providers
{
    public class ResourcesManagement : DataProviderBase
    {
        static private ResourcesManagement _instance = null;
        static public ResourcesManagement Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ResourcesManagement();
                }
                return _instance;
            }
        }

        public ResourcesManagement()
        {
            this.ConnectionString = Globals.Settings.resourcesManage.ConnectionString;

        }
        #region 0. Resource member

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
                //▼==== #006
                if (reader.HasColumn("STT") && reader["STT"] != DBNull.Value)
                {
                    p.STT = (long)reader["STT"];
                }
                if (reader.HasColumn("RsrcHisCode") && reader["RsrcHisCode"] != DBNull.Value)
                {
                    p.RsrcHisCode = reader["RsrcHisCode"].ToString();
                }
                if (reader.HasColumn("HisItemName") && reader["HisItemName"] != DBNull.Value)
                {
                    p.HisItemName = reader["HisItemName"].ToString();
                }
                if (reader.HasColumn("CirculationNumber") && reader["CirculationNumber"] != DBNull.Value)
                {
                    p.CirculationNumber = reader["CirculationNumber"].ToString();
                }
                if (reader.HasColumn("ContractFrom") && reader["ContractFrom"] != DBNull.Value)
                {
                    p.ContractFrom = Convert.ToDateTime(reader["ContractFrom"]);
                }
                if (reader.HasColumn("ContractTo") && reader["ContractTo"] != DBNull.Value)
                {
                    p.ContractTo = Convert.ToDateTime(reader["ContractTo"]);
                }
                if (reader.HasColumn("ContractNumber") && reader["ContractNumber"] != DBNull.Value)
                {
                    p.ContractNumber = reader["ContractNumber"].ToString();
                }
                if (reader.HasColumn("SeriNumber") && reader["SeriNumber"] != DBNull.Value)
                {
                    p.SeriNumber = reader["SeriNumber"].ToString();
                }
                if (reader.HasColumn("UseForDeptID") && reader["UseForDeptID"] != DBNull.Value)
                {
                    p.UseForDeptID = (long)reader["UseForDeptID"];
                }
                if (reader.HasColumn("Manufacturer") && reader["Manufacturer"] != DBNull.Value)
                {
                    p.Manufacturer = (long)reader["Manufacturer"];
                }
                if (reader.HasColumn("ManufactureCountry") && reader["ManufactureCountry"] != DBNull.Value)
                {
                    p.ManufactureCountry = (long)reader["ManufactureCountry"];
                }
                if (reader.HasColumn("ManufactureDate") && reader["ManufactureDate"] != DBNull.Value)
                {
                    p.ManufactureDate = Convert.ToDateTime(reader["ManufactureDate"]);
                }
                if (reader.HasColumn("UseDate") && reader["UseDate"] != DBNull.Value)
                {
                    p.UseDate = Convert.ToDateTime(reader["UseDate"]);
                }
                if (reader.HasColumn("PCOName") && reader["PCOName"] != DBNull.Value)
                {
                    p.ManufacturerStr = reader["PCOName"].ToString();
                }
                p.V_RscrStatus = new Lookup();
                try
                {
                    if (reader.HasColumn("V_RscrStatus") && reader["V_RscrStatus"] != DBNull.Value)
                    {
                        p.V_RscrStatus.LookupID = (long)reader["V_RscrStatus"];
                    }
                    if (reader.HasColumn("VRscrStatus") && reader["VRscrStatus"] != DBNull.Value)
                    {
                        p.V_RscrStatus.ObjectValue = reader["VRscrStatus"].ToString();
                    }
                }
                catch
                {
                }
                if (reader.HasColumn("CreatedStaffID") && reader["CreatedStaffID"] != DBNull.Value)
                {
                    p.CreatedStaffID = (long)reader["CreatedStaffID"];
                }
                if (reader.HasColumn("CreatedDate") && reader["CreatedDate"] != DBNull.Value)
                {
                    p.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);
                }
                if (reader.HasColumn("LastUpdateStaffID") && reader["LastUpdateStaffID"] != DBNull.Value)
                {
                    p.LastUpdateStaffID = (long)reader["LastUpdateStaffID"];
                }
                if (reader.HasColumn("LastUpdateDate") && reader["LastUpdateDate"] != DBNull.Value)
                {
                    p.LastUpdateDate = Convert.ToDateTime(reader["LastUpdateDate"]);
                }
                if (reader.HasColumn("RTypeLists") && reader["RTypeLists"] != DBNull.Value)
                {
                    try
                    {
                        var RTypeLists = reader["RTypeLists"].ToString().Split(';');

                        p.RscrTypeLists = new ObservableCollection<ResourceTypeLists>();

                        foreach (var item in RTypeLists)
                        {
                            var tmp = new ResourceTypeLists();
                            tmp.RscrID = p.RscrID;
                            tmp.MedicalServiceTypeID = Convert.ToInt64(item);

                            p.RscrTypeLists.Add(tmp);
                        }
                    }
                    catch
                    { }
                }
                //▲==== #006
                //▼==== #007
                if (reader.HasColumn("ResourceCode") && reader["ResourceCode"] != DBNull.Value)
                {
                    p.ResourceCode = reader["ResourceCode"].ToString();
                }
                //▲==== #007
            }
            catch
            { }

            return p;        
        }
        //▲=====#002

        #endregion

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

        #region New Resource Allocation

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


        #region Common
        public  List<Lookup> GetLookupRsrcUnit()
        {
            List<Lookup> objLst = new List<Lookup>();
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.RESOURCE_UNIT);
            return objLst;
        }
        public  List<Lookup> GetLookupAllocStatus()
        {
            List<Lookup> objLst = new List<Lookup>();
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.RESOURCE_ALLOC_STATUS);
            return objLst;
        }
        public  List<Lookup> GetLookupStorageStatus()
        {
            List<Lookup> objLst = new List<Lookup>();
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.RESOURCE_STORE_REASON);
            return objLst;
        }
        public  List<Lookup> GetLookupResGroupCategory()
        {
            List<Lookup> objLst = new List<Lookup>();
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.V_ResGroupCategory);
            return objLst;
        }
        public  List<Lookup> GetLookupSupplierType()
        {
            List<Lookup> objLst = new List<Lookup>();
            objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.V_SupplierType);
            return objLst;
        }
        //V_AllocStatus
        //public  List<ResourceGroup> GetRefResourceGroup()
        //{
        //    List<ResourceGroup> objLst = null;
        //    //objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.VSIGN_DATA_TYPE);
        //    return objLst;
        //}
        //public  List<ResourceType> GetRefResourceType()
        //{
        //    List<ResourceType> objLst = null;
        //    //objLst = CommonProvider.Lookups.GetAllLookupsByType(LookupValues.VSIGN_DATA_TYPE);
        //    return objLst;
        //}
        #endregion
        public  List<ResourceGroup> GetAllResourceGroup()
        {
            List<ResourceGroup> listRG = new List<ResourceGroup>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceGroups_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@ParDeptID", SqlDbType.Decimal, 2);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    ResourceGroup p = new ResourceGroup();
                    p.RscrGroupID = (long)reader["RscrGroupID"];
                    p.GroupName = reader["GroupName"].ToString();
                    p.Description = reader["Description"].ToString();
                    listRG.Add(p);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }
        public  List<ResourceGroup> GetAllResourceGroupType(long V_ResGroupCategory)
        {
            List<ResourceGroup> listRG = new List<ResourceGroup>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceGroups_GetAllType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_ResGroupCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_ResGroupCategory));
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    ResourceGroup p = new ResourceGroup();

                    if (reader.HasColumn("RscrGroupID") && reader["RscrGroupID"] != DBNull.Value)
                    {
                        p.RscrGroupID = (long)reader["RscrGroupID"];
                    }

                    if (reader.HasColumn("GroupName") && reader["GroupName"] != DBNull.Value)
                    {
                        p.GroupName = reader["GroupName"].ToString();
                    }

                    if (reader.HasColumn("Description") && reader["Description"] != DBNull.Value)
                    {
                        p.Description = reader["Description"].ToString();
                    }
                    p.VResGroupCategory = new Lookup();
                    if (reader.HasColumn("V_ResGroupCategory") && reader["V_ResGroupCategory"] != DBNull.Value)
                    {
                        p.V_ResGroupCategory = (long)reader["V_ResGroupCategory"];
                        p.VResGroupCategory.LookupID = (long)reader["V_ResGroupCategory"];
                    }

                    listRG.Add(p);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return listRG;
            }
        }
        public  List<ResourceType> GetAllResourceType()
        {
            List<ResourceType> listRG = new List<ResourceType>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceTypes_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.AddParameter("@ParDeptID", SqlDbType.Decimal, 2);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    ResourceType p = new ResourceType();
                    p.RscrTypeID = (long)reader["RscrTypeID"];
                    p.TypeName = reader["TypeName"].ToString();
                    p.Description = reader["Description"].ToString();
                    listRG.Add(p);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRG;
        }
        public  List<ResourceType> GetAllResourceTypeByGID(long RscrGroupID)
        {
            List<ResourceType> listRG = new List<ResourceType>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceTypes_GetByGID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrGroupID));
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    ResourceType p = new ResourceType();
                    p.RscrTypeID = (long)reader["RscrTypeID"];
                    p.TypeName = reader["TypeName"].ToString();
                    p.Description = reader["Description"].ToString();
                    listRG.Add(p);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRG;
        }
        public  List<Resources> GetAllResource()
        {
            List<Resources> listReVal = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResources_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listReVal = GetResourceCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }
        //▼=====#003
        public  List<Resources> GetAllResources()
        {
            List<Resources> listReVal = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetAllResources", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listReVal = GetResourceCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }

        public  List<Resources> GetResourcesForMedicalServices_LoadNotPaging(long PCLExamTypeID)
        {
            List<Resources> listReVal = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourcesForMedicalServices_LoadNotPaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PCLExamTypeID", SqlDbType.BigInt, PCLExamTypeID);
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourcesForMedicalServicesCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }

        //▲=====#003
        public  List<Resources> GetAllResource_GetAllPage(int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<Resources> listRs = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResources_GetAllPage", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listRs = GetResourceCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRs;
        }
        public  List<Resources> GetAllResourceByChoicePaging(int mChoice
                                               , long RscrID
                                               , string Text
                                                , long V_ResGroupCategory
                                               , int PageSize
                                                , int PageIndex
                                                , string OrderBy
                                                , bool CountTotal
                                                , out int Total)
        {

            List<Resources> listReVal = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                //▼==== #007
                //SqlCommand cmd = new SqlCommand("spResources_GetByChoicePaging", cn);
                SqlCommand cmd = new SqlCommand("spResources_GetByChoicePaging_V2", cn);
                //▲==== #007
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@Choice", SqlDbType.TinyInt, ConvertNullObjectToDBNull(mChoice));
                cmd.AddParameter("@RsID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@Text", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Text));

                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));
                cmd.AddParameter("@V_ResGroupCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_ResGroupCategory));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourceCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }
        public  List<Resources> GetAllResourceByAllFilterPage(long GroupID
                                                , long TypeID
                                                , long DeptID //==== #007
                                                , string RsName
                                                , string RsBrand
                                                , int PageSize
                                                , int PageIndex
                                                , string OrderBy
                                                , bool CountTotal
                                                , out int Total)
        {
            List<Resources> listReVal = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                //▼==== #007
                //SqlCommand cmd = new SqlCommand("spResources_GetAllFilterPage", cn);
                SqlCommand cmd = new SqlCommand("spResources_GetAllFilterPage_V2", cn);
                //▲==== #007
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@GroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(GroupID));
                cmd.AddParameter("@TypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(TypeID));
                cmd.AddParameter("@DeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptID));//==== #007

                cmd.AddParameter("@RsName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RsName));
                cmd.AddParameter("@RsBrand", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RsBrand));

                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourceCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }


        public  List<Resources> GetAllResourceByChoice(int mChoice
                                               , long RscrID
                                               , string Text)
        {
            List<Resources> listReVal = new List<Resources>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResources_GetByChoice", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@Choice", SqlDbType.TinyInt, ConvertNullObjectToDBNull(mChoice));
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@Text", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Text));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourceCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }
        public  List<Supplier> GetAllSupplier()
        {
            List<Supplier> listReVal = new List<Supplier>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSupplier_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;



                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetSupplierCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }

        public  List<Supplier> GetAllSupplierType(long V_SupplierType)
        {
            List<Supplier> listReVal = new List<Supplier>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSupplier_GetAllType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@V_SupplierType", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_SupplierType));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetSupplierCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }
        public  bool AddNewResourceGroup(string GroupName, string Description, long V_ResGroupCategory)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceGroups_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@GroupName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GroupName));
                cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));
                cmd.AddParameter("@V_ResGroupCategory", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_ResGroupCategory));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }
        }
        public  bool UpdateResourceGroup(long RscrGroupID, string GroupName, string Description)
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceGroups_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrGroupID));
                cmd.AddParameter("@GroupName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(GroupName));
                cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }
        }
        public  bool DeleteResourceGroup(long RscrGroupID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceGroups_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrGroupID));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }
        }
        public  bool AddNewResourceType(long RscrGroupID, string TypeName, string Description)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceTypes_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrGroupID));
                cmd.AddParameter("@TypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(TypeName));
                cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }
        }
        public  bool UpdateResourceType(long RscrTypeID, long RscrGroupID, string TypeName, string Description)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceTypes_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrTypeID));
                cmd.AddParameter("@RscrGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrGroupID));
                cmd.AddParameter("@TypeName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(TypeName));
                cmd.AddParameter("@Description", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Description));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }


        }
        public  bool DeleteResourceType(long RscrTypeID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRe_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrTypeID));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }

        }

        //▼=====#006
        public bool AddNewResources(Resources resource, string MServiceTypeListIDStr)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResource_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeprecTypeID", SqlDbType.TinyInt, ConvertNullObjectToDBNull(resource.DeprecTypeID));
                cmd.AddParameter("@RscrGroupID", SqlDbType.TinyInt, ConvertNullObjectToDBNull(resource.@RscrGroupID));
                cmd.AddParameter("@RscrTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.RscrTypeID));
                cmd.AddParameter("@ItemName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.ItemName));
                //▼====#007
                //cmd.AddParameter("@ItemBrand", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.ItemBrand));
                cmd.AddParameter("@ResourceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.ResourceCode));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.V_PCLMainCategory));
                //▲====#007
                cmd.AddParameter("@Functions", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.Functions));
                cmd.AddParameter("@GeneralTechInfo", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.GeneralTechInfo));
                cmd.AddParameter("@SupplierID", SqlDbType.SmallInt, ConvertNullObjectToDBNull(resource.SupplierID));

                cmd.AddParameter("@DepreciationByTimeRate", SqlDbType.Float, ConvertNullObjectToDBNull(resource.DepreciationByTimeRate));
                cmd.AddParameter("@DepreciationByUsageRate", SqlDbType.Float, ConvertNullObjectToDBNull(resource.DepreciationByUsageRate));
                cmd.AddParameter("@BuyPrice", SqlDbType.Decimal, ConvertNullObjectToDBNull(resource.BuyPrice));
                cmd.AddParameter("@V_RscrUnit", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.V_RscrUnit));
                cmd.AddParameter("@OnHIApprovedList", SqlDbType.Bit, ConvertNullObjectToDBNull(resource.OnHIApprovedList));
                cmd.AddParameter("@WarrantyTime", SqlDbType.SmallInt, ConvertNullObjectToDBNull(resource.WarrantyTime));
                //▼====#002
                cmd.AddParameter("@HIRepResourceCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(resource.HIRepResourceCode));
                //▲====#002
                cmd.AddParameter("@IsLocatable", SqlDbType.Bit, ConvertNullObjectToDBNull(resource.IsLocatable));
                cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(resource.IsDeleted));

                cmd.AddParameter("@V_ExpenditureSource", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.V_ExpenditureSource));
                //▼====#006
                cmd.AddParameter("@HisItemName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.HisItemName));
                cmd.AddParameter("@UseForDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.UseForDeptID));
                cmd.AddParameter("@ManufactureDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(resource.ManufactureDate));
                cmd.AddParameter("@UseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(resource.UseDate));
                cmd.AddParameter("@Manufacturer", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.Manufacturer));
                cmd.AddParameter("@ManufactureCountry", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.ManufactureCountry));
                cmd.AddParameter("@CirculationNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.CirculationNumber));
                cmd.AddParameter("@ContractFrom", SqlDbType.DateTime, ConvertNullObjectToDBNull(resource.ContractFrom));
                cmd.AddParameter("@ContractTo", SqlDbType.DateTime, ConvertNullObjectToDBNull(resource.ContractTo));
                cmd.AddParameter("@ContractNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.ContractNumber));
                cmd.AddParameter("@SeriNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.SeriNumber));
                cmd.AddParameter("@V_RscrStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.V_RscrStatus.LookupID));
                cmd.AddParameter("@CreatedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.CreatedStaffID));
                cmd.AddParameter("@MServiceTypeListIDStr", SqlDbType.VarChar, ConvertNullObjectToDBNull(MServiceTypeListIDStr));
                //▲====#006
                //▼====#008
                cmd.AddParameter("@ItemNameEng", SqlDbType.VarChar, ConvertNullObjectToDBNull(resource.ItemNameEng));
                //▲====#008

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }


        }

        public  bool UpdateResources(Resources resource, string MServiceTypeListIDStr)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResource_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.RscrID));
                cmd.AddParameter("@DeprecTypeID", SqlDbType.TinyInt, ConvertNullObjectToDBNull(resource.DeprecTypeID));
                cmd.AddParameter("@RscrGroupID", SqlDbType.TinyInt, ConvertNullObjectToDBNull(resource.RscrGroupID));
                cmd.AddParameter("@RscrTypeID", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.RscrTypeID));
                cmd.AddParameter("@ItemName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.ItemName));
                //▼====#007
                //cmd.AddParameter("@ItemBrand", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.ItemBrand));
                cmd.AddParameter("@ResourceCode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.ResourceCode));
                cmd.AddParameter("@V_PCLMainCategory", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.V_PCLMainCategory));
                //▲====#007
                cmd.AddParameter("@Functions", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.Functions));
                cmd.AddParameter("@GeneralTechInfo", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.GeneralTechInfo));
                cmd.AddParameter("@SupplierID", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.SupplierID));

                cmd.AddParameter("@DepreciationByTimeRate", SqlDbType.Float, ConvertNullObjectToDBNull(resource.DepreciationByTimeRate));
                cmd.AddParameter("@DepreciationByUsageRate", SqlDbType.Float, ConvertNullObjectToDBNull(resource.DepreciationByUsageRate));
                cmd.AddParameter("@BuyPrice", SqlDbType.Decimal, ConvertNullObjectToDBNull(resource.BuyPrice));
                cmd.AddParameter("@V_RscrUnit", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.V_RscrUnit));
                cmd.AddParameter("@OnHIApprovedList", SqlDbType.Bit, ConvertNullObjectToDBNull(resource.OnHIApprovedList));
                cmd.AddParameter("@WarrantyTime", SqlDbType.SmallInt, ConvertNullObjectToDBNull(resource.WarrantyTime));
                //▼====#002
                cmd.AddParameter("@HIRepResourceCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(resource.HIRepResourceCode));
                //▲====#002

                cmd.AddParameter("@IsLocatable", SqlDbType.Bit, ConvertNullObjectToDBNull(resource.IsLocatable));
                cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(resource.IsDeleted));
                cmd.AddParameter("@V_ExpenditureSource", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.V_ExpenditureSource));
                //▼====#006
                cmd.AddParameter("@HisItemName", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.HisItemName));
                cmd.AddParameter("@UseForDeptID", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.UseForDeptID));
                cmd.AddParameter("@ManufactureDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(resource.ManufactureDate));
                cmd.AddParameter("@UseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(resource.UseDate));
                cmd.AddParameter("@Manufacturer", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.Manufacturer));
                cmd.AddParameter("@ManufactureCountry", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.ManufactureCountry));
                cmd.AddParameter("@CirculationNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.CirculationNumber));
                cmd.AddParameter("@ContractFrom", SqlDbType.DateTime, ConvertNullObjectToDBNull(resource.ContractFrom));
                cmd.AddParameter("@ContractTo", SqlDbType.DateTime, ConvertNullObjectToDBNull(resource.ContractTo));
                cmd.AddParameter("@ContractNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.ContractNumber));
                cmd.AddParameter("@SeriNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(resource.SeriNumber));
                cmd.AddParameter("@V_RscrStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.V_RscrStatus.LookupID));
                cmd.AddParameter("@LastUpdateStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(resource.LastUpdateStaffID));
                cmd.AddParameter("@MServiceTypeListIDStr", SqlDbType.VarChar, ConvertNullObjectToDBNull(MServiceTypeListIDStr));
                //▲====#006
                //▼====#008
                cmd.AddParameter("@ItemNameEng", SqlDbType.VarChar, ConvertNullObjectToDBNull(resource.ItemNameEng));
                //▲====#008

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }

        }

        public List<List<string>> ExportExcelAllResources()
        {
            try
            {
                DataSet dsExportToExcellAll = new DataSet();
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spResources_ExportAll", cn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = int.MaxValue
                    };

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dsExportToExcellAll);
                    List<List<string>> returnAllExcelData = new List<List<string>>();

                    //Add the below 4 lines to add the column names to show on the Excel file
                    List<string> colname = new List<string>();
                    for (int i = 0; i <= dsExportToExcellAll.Tables[0].Columns.Count - 1; i++)
                    {
                        colname.Add(dsExportToExcellAll.Tables[0].Columns[i].ToString().Trim());
                    }

                    returnAllExcelData.Add(colname);
                    for (int i = 0; i <= dsExportToExcellAll.Tables[0].Rows.Count - 1; i++)
                    {
                        List<string> rowData = new List<string>();
                        for (int j = 0; j <= dsExportToExcellAll.Tables[0].Columns.Count - 1; j++)
                        {
                            rowData.Add(Convert.ToString(dsExportToExcellAll.Tables[0].Rows[i][j]).Replace("<", "&lt;").Replace(">", "&gt;"));
                        }
                        returnAllExcelData.Add(rowData);
                    }

                    CleanUpConnectionAndCommand(cn, cmd);
                    return returnAllExcelData;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<RefMedicalServiceType> GetMedicalServiceTypes_ByResourceGroup(long RscrGroupID)
        {
            List<RefMedicalServiceType> listRG = new List<RefMedicalServiceType>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetMedicalServiceTypes_ByResourceGroup", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrGroupID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrGroupID));
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    RefMedicalServiceType p = new RefMedicalServiceType();
                    p.MedicalServiceTypeID = (long)reader["MedicalServiceTypeID"];
                    p.MedicalServiceGroupID = (long)reader["MedicalServiceGroupID"];
                    p.MedicalServiceTypeCode = reader["MedicalServiceTypeCode"].ToString();
                    p.MedicalServiceTypeName = reader["MedicalServiceTypeName"].ToString();
                    p.MedicalServiceTypeDescription = reader["MedicalServiceTypeDescription"].ToString();
                    p.V_RefMedicalServiceInOutOthers = (long)reader["V_RefMedicalServiceInOutOthers"];
                    p.V_RefMedicalServiceTypes = (long)reader["V_RefMedicalServiceTypes"];
                    listRG.Add(p);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRG;
        }
        //▲=====#006
        public bool DeleteResources(long RscrID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResource_Delete", cn);
                bool IsDeleted = true;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@IsDeleted", SqlDbType.BigInt, ConvertNullObjectToDBNull(IsDeleted));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }

        }

        #region Resources Allocations
        public  List<RefDepartment> GetAllRefDepartments()
        {
            List<RefDepartment> listRG = new List<RefDepartment>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetRefDepartments_All", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listRG = GetRefDepartmentCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRG;
        }
        public  List<RefDepartment> GetAllRefDepartmentsByType(int type)
        {
            List<RefDepartment> listRG = new List<RefDepartment>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefDepartments_ByType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@Type", SqlDbType.Int, (type));
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listRG = GetRefDepartmentCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRG;
        }
        public  List<Location> GetAllLocations()
        {
            List<Location> listRG = new List<Location>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spLocations_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listRG = GetLocationCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRG;
        }
        public  List<Location> GetAllLocationsByType(int type)
        {
            List<Location> listRG = new List<Location>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spLocations_AllByType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@Type", SqlDbType.Int, (type));
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listRG = GetLocationCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listRG;
        }
        public  List<RoomType> GetAllRoomType()
        {
            List<RoomType> listReVal = new List<RoomType>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRoomType_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listReVal = GetRoomTypeCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }
        public  List<RefStaffCategory> GetRefStaffCategoriesByType(long V_StaffCatType)
        {
            List<RefStaffCategory> listReVal = new List<RefStaffCategory>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefStaffCategories_GetByType", cn);
                cmd.AddParameter("@V_StaffCatType", SqlDbType.BigInt, (V_StaffCatType));
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    RefStaffCategory p = new RefStaffCategory();
                    p.StaffCatgID = (long)reader["StaffCatgID"];
                    p.StaffCatgDescription = reader["StaffCatgDescription"].ToString();
                    listReVal.Add(p);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return listReVal;
            }
        }
        public  List<RefStaffCategory> GetAllRefStaffCategories()
        {
            List<RefStaffCategory> listReVal = new List<RefStaffCategory>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spRefStaffCategories_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read())
                {
                    RefStaffCategory p = new RefStaffCategory();
                    p.StaffCatgID = (long)reader["StaffCatgID"];
                    p.StaffCatgDescription = reader["StaffCatgDescription"].ToString();
                    //20181208 TBL: Lay them V_StaffCatType de biet thuoc loai nhan vien nao de validate
                    if (reader.HasColumn("V_StaffCatType") && reader["V_StaffCatType"] != DBNull.Value)
                    {
                        p.V_StaffCatType = (long)reader["V_StaffCatType"];
                    }
                    listReVal.Add(p);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return listReVal;
            }
        }
        public  List<Staff> GetAllStaff(long StaffCatgID)
        {
            List<Staff> listReVal = new List<Staff>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffs_GetAllByStaffCatgID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@StaffCatgID", SqlDbType.Int, (StaffCatgID));
                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                while (reader.Read()) 
                {
                    Staff p = new Staff();
                    p.RefStaffCategory = new RefStaffCategory();
                    p.StaffID = (long)reader["StaffID"];
                    if (reader.HasColumn("DeptID") && reader["DeptID"] != DBNull.Value)
                    {
                        p.DeptID = (long)reader["DeptID"];
                    }
                    if (reader.HasColumn("StaffCatgID") && reader["StaffCatgID"] != DBNull.Value)
                    {
                        p.StaffCatgID = (long)reader["StaffCatgID"];
                        p.RefStaffCategory.StaffCatgID = (long)p.StaffCatgID;
                        if (reader.HasColumn("V_StaffCatType") && reader["V_StaffCatType"] != DBNull.Value)
                        {
                            p.RefStaffCategory.V_StaffCatType = Convert.ToInt64(reader["V_StaffCatType"]);
                        }
                    }
                    if (reader.HasColumn("FullName") && reader["FullName"] != DBNull.Value)
                    {
                        p.FullName = reader["FullName"].ToString();
                    }
                    if (reader.HasColumn("UserAccountsName") && reader["UserAccountsName"] != DBNull.Value)
                    {
                        p.UserAccountsName = reader["UserAccountsName"].ToString();
                    }
                    listReVal.Add(p);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return listReVal;
            }
        }

        public  List<Staff> GetAllStaffByStaffCategory(long StaffCategory)
        {
            List<Staff> listReVal = new List<Staff>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffs_GetAllByStaffCategory", cn);
                // SqlCommand cmd = new SqlCommand("spStaffs_GetAllStaffCatType", cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@StaffCategory", SqlDbType.Int, (StaffCategory));

                cn.Open();
                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetStaffCollectionFromReader(reader);
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
                return listReVal;
            }
        }
        public  List<DeptLocation> GetDeptLocationByExamType(long examTypeId)
        {
            if (examTypeId <= 0)
            {
                return null;
            }
            var listReVal = new List<DeptLocation>();
            using (var cn = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand("sp_PclExamType_GetAllDeptLocations", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                cmd.AddParameter("@ExamTypeID", SqlDbType.BigInt, examTypeId);

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listReVal = GetDepartmentCollectionFromReader(reader);
                    reader.Close();
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }

        // Txd 16/11/2013 
        //public  List<PCLExamTypeLocation> GetPclExamTypeLocationsByExamTypeList(List<PCLExamType> examTypes)
        //{
        //    if(examTypes == null || examTypes.Count == 0)
        //    {
        //        return null;
        //    }
        //    var listReVal = new List<PCLExamTypeLocation>();
        //    using (var cn = new SqlConnection(this.ConnectionString))
        //    {
        //        var cmd = new SqlCommand("sp_PclExamType_GetAllPCLExamTypeLocations", cn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cn.Open();
        //        var idList = new IDListOutput<long>() { Ids = examTypes.Select(item => item.PCLExamTypeID).ToList() };
        //        cmd.AddParameter("@ExamTypeIDList", SqlDbType.Xml, ConvertNullObjectToDBNull(SerializeToXml(idList)));

        //        IDataReader reader = ExecuteReader(cmd);
        //        if (reader != null)
        //        {
        //            listReVal = GetPCLExamTypeLocationCollectionFromReader(reader);
        //            reader.Close();

        //        }
        //    }
        //    return listReVal;
        //}

        public List<PCLExamTypeLocation> GetPclExamTypeLocationsByExamTypeList(List<PCLExamType> examTypes)
        {
            if (ListAllPCLExamTypeLocations != null)
            {
                return ListAllPCLExamTypeLocations;
            }
            return null;
        }

        public  List<DeptLocation> GetAllDeptLocation()
        {
            List<DeptLocation> listReVal = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listReVal = GetDepartmentCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }

        public  List<DeptLocation> GetAllDeptLocationByType(int choice)
        {
            List<DeptLocation> listReVal = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_GetAll_Type", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@Choice", SqlDbType.BigInt, (choice));
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    listReVal = GetDepartmentCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }
        public  List<ResourceAllocations> GetResourceAllocationsPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<ResourceAllocations> listReVal = new List<ResourceAllocations>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceAllocations_PagingByDLID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, (DeptLocationID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourceAllocationsFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }

        public  List<ResourceStorages> GetResourceStoragesPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<ResourceStorages> listReVal = new List<ResourceStorages>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceStorages_PagingByDLID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, (DeptLocationID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourceStoragesFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }
        public  List<DeptLocation> GetAllPageDeptLocation(int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<DeptLocation> listReVal = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_GetAllPage", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetDepartmentCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }
        public  List<DeptLocation> GetAllPageDeptLocationByType(int choice, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<DeptLocation> listReVal = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_GetAllPage_ByType", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@Choice", SqlDbType.Int, (choice));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetDepartmentCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }

        public  List<DeptLocation> GetAllPageDeptLocationGetByChoicePaging(int Choice, long RsID, string Text,
            int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<DeptLocation> listReVal = new List<DeptLocation>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spDeptLocation_GetByChoicePaging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@Choice", SqlDbType.Int, (Choice));
                cmd.AddParameter("@RsID", SqlDbType.Int, (RsID));
                cmd.AddParameter("@Text", SqlDbType.NVarChar, (Text));


                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);



                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetDepartmentCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }
        public  bool AddNewStoragesAllocations(
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
                                                , bool IsDeleted)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceStorages_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, ConvertNullObjectToDBNull(GuidID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                cmd.AddParameter("@StorageStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StorageStaffID));
                cmd.AddParameter("@StorageDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StorageDate));

                cmd.AddParameter("@V_StorageStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_StorageStatus));
                cmd.AddParameter("@V_StorageReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_StorageReason));




                cmd.AddParameter("@HasIdentity", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HasIdentity));
                cmd.AddParameter("@RscrCode ", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrCode));
                cmd.AddParameter("@RscrBarcode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrBarcode));
                cmd.AddParameter("@SerialNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SerialNumber));

                cmd.AddParameter("@QtyStorage", SqlDbType.SmallInt, ConvertNullObjectToDBNull(QtyStorage));
                //cmd.AddParameter("@QtyInUse", SqlDbType.NVarChar, ConvertNullObjectToDBNull(QtyInUse));
                cmd.AddParameter("@ResponsibleStaffID ", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));
                cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(IsDeleted));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }

        }
        public  bool AddNewResourceAllocations(
                                                    long RscrID
                                                    , String GuidID
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
                                                   , int QtyAlloc
                                                   , int QtyInUse
                                                   , long ResponsibleStaffID
                                                   , bool IsActive)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceAllocations_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, ConvertNullObjectToDBNull(GuidID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                cmd.AddParameter("@AllocStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AllocStaffID));
                cmd.AddParameter("@AllocDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AllocDate));
                cmd.AddParameter("@StartUseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartUseDate));
                cmd.AddParameter("@V_AllocStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AllocStatus));
                cmd.AddParameter("@HasIdentity", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HasIdentity));

                cmd.AddParameter("@RscrCode ", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrCode));
                cmd.AddParameter("@RscrBarcode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrBarcode));
                cmd.AddParameter("@SerialNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SerialNumber));

                cmd.AddParameter("@QtyAlloc", SqlDbType.SmallInt, ConvertNullObjectToDBNull(QtyAlloc));
                cmd.AddParameter("@QtyInUse", SqlDbType.NVarChar, ConvertNullObjectToDBNull(QtyInUse));
                cmd.AddParameter("@ResponsibleStaffID ", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                cmd.AddParameter("@IsActive", SqlDbType.NVarChar, ConvertNullObjectToDBNull(IsActive));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }

        }

        public  bool UpdateResourceAllocations(long RscrAllocID
                                                    , long RscrID
                                                    , String GuidID
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
                                                   , int QtyAlloc
                                                   , int QtyInUse
                                                   , long ResponsibleStaffID
                                                   , bool IsActive)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceAllocations_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrAllocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrAllocID));
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, ConvertNullObjectToDBNull(GuidID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                cmd.AddParameter("@AllocStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AllocStaffID));
                cmd.AddParameter("@AllocDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AllocDate));
                cmd.AddParameter("@StartUseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartUseDate));
                cmd.AddParameter("@V_AllocStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AllocStatus));
                cmd.AddParameter("@HasIdentity", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HasIdentity));

                cmd.AddParameter("@RscrCode ", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrCode));
                cmd.AddParameter("@RscrBarcode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrBarcode));
                cmd.AddParameter("@SerialNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SerialNumber));

                cmd.AddParameter("@QtyAlloc", SqlDbType.SmallInt, ConvertNullObjectToDBNull(QtyAlloc));
                cmd.AddParameter("@QtyInUse", SqlDbType.NVarChar, ConvertNullObjectToDBNull(QtyInUse));
                cmd.AddParameter("@ResponsibleStaffID ", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                cmd.AddParameter("@IsActive", SqlDbType.NVarChar, ConvertNullObjectToDBNull(IsActive));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }

        }
        public  bool UpdateResourceStorages(
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
                                                 , bool IsDeleted)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceStorages_Update", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrStorageID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrStorageID));
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, ConvertNullObjectToDBNull(RscrGUID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                cmd.AddParameter("@StorageStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(StorageStaffID));
                cmd.AddParameter("@StorageDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StorageDate));
                cmd.AddParameter("@V_StorageReason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(V_StorageReason));
                cmd.AddParameter("@V_StorageStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_StorageStatus));
                cmd.AddParameter("@HasIdentity", SqlDbType.NVarChar, ConvertNullObjectToDBNull(HasIdentity));

                cmd.AddParameter("@RscrCode ", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrCode));
                cmd.AddParameter("@RscrBarcode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrBarcode));
                cmd.AddParameter("@SerialNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SerialNumber));

                cmd.AddParameter("@QtyStorage", SqlDbType.SmallInt, ConvertNullObjectToDBNull(QtyStorage));
                cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(IsDeleted));
                cmd.AddParameter("@ResponsibleStaffID ", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                cmd.AddParameter("@IsActive", SqlDbType.NVarChar, ConvertNullObjectToDBNull(IsActive));

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;
            }

        }
        public  bool ResourceAllocationsTranferExec(int Choice
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
                                                    )
        {

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceAllocations_TranferExec", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@Choice", SqlDbType.Int, ConvertNullObjectToDBNull(Choice));
                cmd.AddParameter("@RscrAllocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrAllocID));

                cmd.AddParameter("@GuidID", SqlDbType.VarChar, ConvertNullObjectToDBNull(GuidID));
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                cmd.AddParameter("@AllocStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AllocStaffID));
                cmd.AddParameter("@AllocDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AllocDate));
                cmd.AddParameter("@StartUseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartUseDate));
                cmd.AddParameter("@V_AllocStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AllocStatus));
                cmd.AddParameter("@HasIdentity", SqlDbType.Bit, ConvertNullObjectToDBNull(HasIdentity));
                cmd.AddParameter("@RscrCode ", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrCode));
                cmd.AddParameter("@RscrBarcode", SqlDbType.NVarChar, ConvertNullObjectToDBNull(RscrBarcode));
                cmd.AddParameter("@SerialNumber", SqlDbType.NVarChar, ConvertNullObjectToDBNull(SerialNumber));
                cmd.AddParameter("@NewQtyAlloc", SqlDbType.SmallInt, ConvertNullObjectToDBNull(NewQtyAlloc));
                cmd.AddParameter("@NewQtyInUse", SqlDbType.SmallInt, ConvertNullObjectToDBNull(NewQtyInUse));
                cmd.AddParameter("@ResponsibleStaffID ", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));

                cmd.AddParameter("@V_StorageReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_StorageReason));



                cmd.AddParameter("@RequestStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RequestStaffID));
                cmd.AddParameter("@MoveReason", SqlDbType.NVarChar, ConvertNullObjectToDBNull(MoveReason));
                cmd.AddParameter("@EffectiveMoveDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(EffectiveMoveDate));
                cmd.AddParameter("@FromDeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FromDeptLocID));
                cmd.AddParameter("@ToDeptLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ToDeptLocID));
                cmd.AddParameter("@ApprovedStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ApprovedStaffID));
                cmd.AddParameter("@Note", SqlDbType.NVarChar, ConvertNullObjectToDBNull(Note));

                cmd.AddParameter("@QtyAllocEx", SqlDbType.Int, ConvertNullObjectToDBNull(QtyAllocEx));
                cmd.AddParameter("@QtyInUseEx", SqlDbType.Int, ConvertNullObjectToDBNull(QtyInUseEx));


                cn.Open();
                cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return true;
            }
        }

        #endregion

        #region New Resource Allocation

        public  bool AddNewResourceProperty(long RscrID
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
                                                       , long ResponsibleStaffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spResourceProperty_NewInsert", cn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParameter("@RsID", SqlDbType.BigInt, ConvertNullObjectToDBNull(0));
                    cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrID));
                    cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, ConvertNullObjectToDBNull(RscrGUID));
                    cmd.AddParameter("@HasIdentity", SqlDbType.Bit, ConvertNullObjectToDBNull(HasIdentity));
                    cmd.AddParameter("@RscrCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(RscrCode));
                    cmd.AddParameter("@RscrBarcode", SqlDbType.VarChar, ConvertNullObjectToDBNull(RscrBarcode));
                    cmd.AddParameter("@SerialNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(SerialNumber));
                    cmd.AddParameter("@QtyAlloc", SqlDbType.SmallInt, ConvertNullObjectToDBNull(QtyAlloc));
                    cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(IsActive));
                    cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(IsDeleted));

                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                    cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                    cmd.AddParameter("@AllocStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AllocStaffID));
                    cmd.AddParameter("@AllocDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AllocDate));
                    cmd.AddParameter("@StartUseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartUseDate));
                    cmd.AddParameter("@V_AllocStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AllocStatus));
                    cmd.AddParameter("@V_StorageReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_StorageReason));
                    cmd.AddParameter("@ResponsibleStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    CleanUpConnectionAndCommand(cn, cmd);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public  bool MoveResourcePropertyLocation(long RscrPropLocID
                                                    , long RscrPropertyID

                                                       , long DeptLocationID
                                                       , long RscrMoveRequestID
                                                       , long AllocStaffID
                                                       , DateTime? AllocDate
                                                       , DateTime? StartUseDate
                                                       , long V_AllocStatus
                                                        , long V_StorageReason
                                                       , long ResponsibleStaffID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("spResourcePropLocations_Insert", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //SqlTransaction transaction;
                    cn.Open();
                    // =====▼ #001                                                        
                    // transaction = cn.BeginTransaction();
                    // =====▲ #001

                    cmd.AddParameter("@RscrPropertyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrPropertyID));

                    cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(DeptLocationID));
                    cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrMoveRequestID));
                    cmd.AddParameter("@AllocStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(AllocStaffID));
                    cmd.AddParameter("@AllocDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(AllocDate));
                    cmd.AddParameter("@StartUseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(StartUseDate));
                    cmd.AddParameter("@V_AllocStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_AllocStatus));
                    cmd.AddParameter("@V_StorageReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(V_StorageReason));
                    cmd.AddParameter("@ResponsibleStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(ResponsibleStaffID));
                    cmd.AddParameter("@IsActive", SqlDbType.VarChar, ConvertNullObjectToDBNull(true));
                    cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(false));

                    SqlCommand cmd2 = new SqlCommand("spResourcePropLocations_SetActive", cn);

                    cmd2.CommandType = CommandType.StoredProcedure;

                    cmd2.AddParameter("@RscrPropLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrPropLocID));
                    cmd2.AddParameter("@IsActive", SqlDbType.VarChar, ConvertNullObjectToDBNull(false));
                    cmd2.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(false));

                    // =====▼ #001                                                                            
                    // cmd.Transaction = transaction;
                    // cmd2.Transaction = transaction;                                                 
                    // =====▲ #001
                    try
                    {
                        cmd.ExecuteNonQuery();

                        cmd2.ExecuteNonQuery();
                        // =====▼ #001                                                        
                        // transaction.Commit();                        
                        // =====▲ #001
                    }
                    catch
                    {
                        // =====▼ #001                                                        
                        // transaction.Rollback();                        
                        // =====▲ #001
                    }
                    CleanUpConnectionAndCommand(cn, cmd);
                    cmd2.Dispose();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public  bool BreakResourceProperty(List<ResourcePropLocations> lstResourcePropLocations, long RscrPropertyID, long RscrPropLocID)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(this.ConnectionString))
                {
                    List<SqlCommand> lstCmd = new List<SqlCommand>();
                    // SqlTransaction transaction;
                    cn.Open();

                    // =====▼ #001                                                                            
                    // transaction = cn.BeginTransaction();                                                    
                    // =====▲ #001

                    foreach (ResourcePropLocations rPL in lstResourcePropLocations)
                    {
                        SqlCommand cmd = new SqlCommand("spResourceProperty_NewInsert", cn);

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.AddParameter("@RsID", SqlDbType.BigInt, ConvertNullObjectToDBNull(0));
                        cmd.AddParameter("@RscrID", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VRscrProperty.RscrID));
                        cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, ConvertNullObjectToDBNull(rPL.VRscrProperty.RscrGUID));
                        cmd.AddParameter("@HasIdentity", SqlDbType.Bit, ConvertNullObjectToDBNull(rPL.VRscrProperty.HasIdentity));
                        cmd.AddParameter("@RscrCode", SqlDbType.VarChar, ConvertNullObjectToDBNull(rPL.VRscrProperty.RscrCode));
                        cmd.AddParameter("@RscrBarcode", SqlDbType.VarChar, ConvertNullObjectToDBNull(rPL.VRscrProperty.RscrBarcode));
                        cmd.AddParameter("@SerialNumber", SqlDbType.VarChar, ConvertNullObjectToDBNull(rPL.VRscrProperty.SerialNumber));
                        cmd.AddParameter("@QtyAlloc", SqlDbType.SmallInt, ConvertNullObjectToDBNull(rPL.VRscrProperty.QtyAlloc));
                        cmd.AddParameter("@IsActive", SqlDbType.Bit, ConvertNullObjectToDBNull(rPL.VRscrProperty.IsActive));
                        cmd.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(rPL.VRscrProperty.IsDelete));

                        cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VDeptLocation.DeptLocationID));
                        cmd.AddParameter("@RscrMoveRequestID", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VRscrMoveRequest.RscrMoveRequestID));
                        cmd.AddParameter("@AllocStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VAllocStaff.StaffID));
                        cmd.AddParameter("@AllocDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(rPL.AllocDate));
                        cmd.AddParameter("@StartUseDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(rPL.StartUseDate));
                        cmd.AddParameter("@V_AllocStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VAllocStatus.LookupID));
                        cmd.AddParameter("@V_StorageReason", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VStorageReason.LookupID));
                        cmd.AddParameter("@ResponsibleStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(rPL.VResponsibleStaff.StaffID));

                        // =====▼ #001                                                        
                        // cmd.Transaction = transaction;                                                    
                        // =====▲ #001

                        lstCmd.Add(cmd);
                    }
                    //status o day

                    SqlCommand cmd1 = new SqlCommand("spResourceProperty_SetActive", cn);
                    SqlCommand cmd2 = new SqlCommand("spResourcePropLocations_SetActive", cn);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd1.AddParameter("@RscrPropertyID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrPropertyID));
                    cmd1.AddParameter("@IsActive", SqlDbType.VarChar, ConvertNullObjectToDBNull(false));
                    cmd1.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(false));

                    cmd2.AddParameter("@RscrPropLocID", SqlDbType.BigInt, ConvertNullObjectToDBNull(RscrPropLocID));
                    cmd2.AddParameter("@IsActive", SqlDbType.VarChar, ConvertNullObjectToDBNull(false));
                    cmd2.AddParameter("@IsDeleted", SqlDbType.Bit, ConvertNullObjectToDBNull(false));

                    // =====▼ #001                                                        
                    // cmd1.Transaction = transaction;
                    // cmd2.Transaction = transaction;
                    // =====▲ #001

                    try
                    {
                        foreach (SqlCommand sQLCmd in lstCmd)
                        {
                            sQLCmd.ExecuteNonQuery();
                        }
                        cmd1.ExecuteNonQuery();
                        cmd2.ExecuteNonQuery();
                        // =====▼ #001                                                                                
                        // transaction.Commit();                                                      
                        // =====▲ #001
                    }
                    catch
                    {
                        // =====▼ #001                                                                                
                        // transaction.Rollback();                                                  
                        // =====▲ #001
                    }
                    cn.Dispose();
                    cmd1.Dispose();
                    cmd2.Dispose();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }


        public  List<ResourcePropLocations> GetResourcePropLocationsPagingByVType(long Choice, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<ResourcePropLocations> listReVal = new List<ResourcePropLocations>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourcePropLocations_PagingByVType", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@Choice", SqlDbType.BigInt, (Choice));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourcePropLocationsFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }
        public  List<ResourcePropLocations> GetResourcePropLocationsPagingByDLID(long DeptLocationID, int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<ResourcePropLocations> listReVal = new List<ResourcePropLocations>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourcePropLocations_PagingByDLID", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@DeptLocationID", SqlDbType.BigInt, (DeptLocationID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourcePropLocationsFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }
        public  List<ResourcePropLocations> GetResourcePropLocationsPagingByFilter(int Choice, string RscrGUID, long RscrPropertyID
             , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<ResourcePropLocations> listReVal = new List<ResourcePropLocations>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourcePropLocations_PagingByFilter", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@Choice", SqlDbType.Int, (Choice));
                cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, (RscrGUID));
                cmd.AddParameter("@RscrPropertyID", SqlDbType.BigInt, (RscrPropertyID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourcePropLocationsFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }
        public  List<ResourceMaintenanceLogStatus> GetspResourceMaintenanceLogStatusByID(long RscrMaintLogID
             , int PageSize, int PageIndex, string OrderBy, bool CountTotal, out int Total)
        {
            List<ResourceMaintenanceLogStatus> listReVal = new List<ResourceMaintenanceLogStatus>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLogStatus_GetAll", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, (RscrMaintLogID));
                cmd.AddParameter("@PageSize", SqlDbType.Int, (PageSize));
                cmd.AddParameter("@PageIndex", SqlDbType.Int, (PageIndex));
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, (OrderBy));
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                //cmd.AddParameter("@Total", SqlDbType.Bit, ParameterDirection.Output(Total));

                SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                paramTotal.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(paramTotal);
                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                listReVal = GetResourceMaintenanceLogStatusCollectionFromReader(reader);
                reader.Close();
                if (paramTotal.Value != null)
                {
                    Total = (int)paramTotal.Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return listReVal;
        }

        public  long GetResourcePropertyFilterSum(int Choice, string RscrGUID, long RscrID)
        {
            long Sum = 0;
            List<ResourcePropLocations> listReVal = new List<ResourcePropLocations>();
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceProperty_FilterSum", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@Choice", SqlDbType.Int, (Choice));
                cmd.AddParameter("@RscrGUID", SqlDbType.VarChar, (RscrGUID));
                cmd.AddParameter("@RscrID", SqlDbType.BigInt, (RscrID));

                cn.Open();
                Object objectSum = cmd.ExecuteScalar();
                if (objectSum != null)
                {
                    Sum = long.Parse(objectSum.ToString());
                }
                else
                {
                    return 0;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return Sum;
        }

        #endregion




        #region ResourceMaintenance

        public  List<Staff> GetStaffs_All()
        {
            List<Staff> lst = new List<Staff>();

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spStaffs_Simple_AllCols", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetStaffCollectionFromReader_Simple(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return lst;
        }

        public  List<Supplier> GetSupplierIsMaintenance_All()
        {
            List<Supplier> lst = new List<Supplier>();

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetSupplierIsMaintenance", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetSupplierCollectionFromReader(reader);
                }
                reader.Close();
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return lst;
        }

        public  bool AddNewResourceMaintenanceLog(ResourceMaintenanceLog obj, out long RscrMaintLogID)
        {
            bool results = false;
            RscrMaintLogID = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLog_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrPropertyID", SqlDbType.BigInt, obj.RscrPropertyID);
                cmd.AddParameter("@LoggingDate", SqlDbType.DateTime, obj.LoggingDate);
                cmd.AddParameter("@LoggingIssue", SqlDbType.NVarChar, (obj.LoggingIssue == null ? "" : obj.LoggingIssue));
                cmd.AddParameter("@LoggerStaffID", SqlDbType.BigInt, obj.LoggerStaffID);
                cmd.AddParameter("@AssignStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.AssignStaffID));
                cmd.AddParameter("@ExternalFixSupplierID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.ExternalFixSupplierID));

                cmd.AddParameter("@Comments", SqlDbType.NVarChar, (obj.Comments == null ? "" : obj.Comments));

                cmd.AddParameter("@V_RscrInitialStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.V_RscrInitialStatus));

                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                //IDataReader reader = ExecuteReader(cmd);
                //if (reader.Read())
                //{
                //    results = true;
                //    RscrMaintLogID = (long)reader["RscrMaintLogID"];
                //}
                //reader.Close();

                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                {
                    results = true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }
        public  List<ResourceMaintenanceLog> GetResourceMaintenanceLogSearch_Paging(
            ResourceMaintenanceLogSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            )
        {
            Total = 0;
            List<ResourceMaintenanceLog> lst = new List<ResourceMaintenanceLog>();

            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLogSearch_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Criteria.ToDate));
                cmd.AddParameter("@V_StatusIssueMaintenance", SqlDbType.BigInt, Criteria.V_StatusIssueMaintenance);
                cmd.AddParameter("@LoggingIssue", SqlDbType.NVarChar, Criteria.LoggingIssue);

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();

                IDataReader reader = ExecuteReader(cmd);
                if (reader != null)
                {
                    lst = GetResourceMaintenanceLogCollectionFromReader(reader);
                }
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return lst;
        }


        public bool ResourceMaintenanceLogCanEdit(Int64 RscrMaintLogID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLogCanEdit", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);

                cn.Open();

                DataTable dt = ExecuteDataTable(cmd);

                int iRec = 0;

                if (dt != null && dt.Rows.Count > 0)
                {
                    iRec = Convert.ToInt16(dt.Rows[0][0]);
                }
                if (iRec > 0)
                {
                    CleanUpConnectionAndCommand(cn, cmd);
                    return true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return false;
            }
        }

        public  ResourceMaintenanceLog GetResourceMaintenanceLogByID(Int64 RscrMaintLogID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetResourceMaintenanceDetail_Full", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);

                cn.Open();

                ResourceMaintenanceLog obj = null;

                IDataReader reader = ExecuteReader(cmd);

                if (reader.Read())
                {
                    obj = GetResourceMaintenanceLogFromReader(reader);
                }
                reader.Close();

                return obj;
            }
        }

        public  bool SaveResourceMaintenanceLogOfFix(
            Int64 RscrMaintLogID,
            Nullable<Int64> FixStaffID,
            Nullable<Int64> FixSupplierID,
            DateTime FixDate,
            string FixSolutions,
            string FixComments,
            Int64 VerifiedStaffID,
            Int64 V_CurrentStatus
            )
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spSaveResourceMaintenanceLogOfFix", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);

                cmd.AddParameter("@FixStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FixStaffID));

                cmd.AddParameter("@FixSupplierID", SqlDbType.BigInt, ConvertNullObjectToDBNull(FixSupplierID));

                cmd.AddParameter("@FixDate", SqlDbType.DateTime, FixDate);

                cmd.AddParameter("@FixSolutions", SqlDbType.NVarChar, FixSolutions);

                cmd.AddParameter("@FixComments", SqlDbType.NVarChar, FixComments);

                cmd.AddParameter("@VerifiedStaffID", SqlDbType.BigInt, VerifiedStaffID);

                cmd.AddParameter("@V_CurrentStatus", SqlDbType.BigInt, V_CurrentStatus);

                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                //int iRec = ExecuteNonQuery(cmd);
                //if(iRec > 0)
                //    return true;
                //return false;
                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                {
                    CleanUpConnectionAndCommand(cn, cmd);
                    return true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return false;
        }


        public bool UpdateResourceMaintenanceLog_FinalStatus(
             Int64 RscrMaintLogID,
            Int64 VerifiedStaffID,
            Int64 V_CurrentStatus
            )
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spUpdateResourceMaintenanceLog_FinalStatus", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);

                //cmd.AddParameter("@FixStaffID", SqlDbType.BigInt, FixStaffID);                

                cmd.AddParameter("@VerifiedStaffID", SqlDbType.BigInt, VerifiedStaffID);

                cmd.AddParameter("@V_CurrentStatus", SqlDbType.BigInt, V_CurrentStatus);

                cn.Open();

                int iRec = ExecuteNonQuery(cmd);
                if (iRec > 0)
                {
                    CleanUpConnectionAndCommand(cn, cmd);
                    return true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return false;
            }
        }

        public  bool CheckRscrDiBaoTri(Int64 RscrPropertyID)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spCheckRscrDiBaoTri", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrPropertyID", SqlDbType.BigInt, RscrPropertyID);
                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                //IDataReader reader = ExecuteReader(cmd);
                //if (reader.Read())
                //{
                //    results = true;
                //    RscrMaintLogID = (long)reader["RscrMaintLogID"];
                //}
                //reader.Close();

                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal == 1)//Duoc phep di bao tri
                {
                    results = true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }


        public  bool ResourceMaintenanceLog_Delete(Int64 RscrMaintLogID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLog_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);
                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal == 1)//Xóa thành công
                {
                    return true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return false;
            }
        }


        public  bool GetCheckForVerified(Int64 RscrMaintLogID)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("GetCheckForVerified", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);
                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                //IDataReader reader = ExecuteReader(cmd);
                //if (reader.Read())
                //{
                //    results = true;
                //    RscrMaintLogID = (long)reader["RscrMaintLogID"];
                //}
                //reader.Close();

                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal == 1)//Duoc phep di bao tri
                {
                    results = true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }



        public  bool DeleteAndCreateNewResourceMaintenanceLog(long RscrMaintLogID_Delete, ResourceMaintenanceLog obj)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                //////SqlCommand cmd = new SqlCommand("spResourceMaintenanceLog_Insert", cn);
                //////cmd.CommandType = CommandType.StoredProcedure;

                //////cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID_Delete);

                //////cmd.AddParameter("@RscrPropertyID", SqlDbType.BigInt, obj.RscrPropertyID);
                //////cmd.AddParameter("@LoggingDate", SqlDbType.DateTime, obj.LoggingDate);
                //////cmd.AddParameter("@LoggingIssue", SqlDbType.NVarChar, (obj.LoggingIssue == null ? "" : obj.LoggingIssue));
                //////cmd.AddParameter("@LoggerStaffID", SqlDbType.BigInt, obj.LoggerStaffID);
                //////cmd.AddParameter("@AssignStaffID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.AssignStaffID));
                //////cmd.AddParameter("@ExternalFixSupplierID", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.ExternalFixSupplierID));

                //////cmd.AddParameter("@Comments", SqlDbType.NVarChar, (obj.Comments == null ? "" : obj.Comments));

                //////cmd.AddParameter("@V_RscrInitialStatus", SqlDbType.BigInt, ConvertNullObjectToDBNull(obj.V_RscrInitialStatus));

                //////cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                //////cn.Open();

                ////////IDataReader reader = ExecuteReader(cmd);
                ////////if (reader.Read())
                ////////{
                ////////    results = true;
                ////////    RscrMaintLogID = (long)reader["RscrMaintLogID"];
                ////////}
                ////////reader.Close();

                //////ExecuteNonQuery(cmd);
                //////int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                //////if (ReturnVal > 0)
                //////    results = true;
            }
            return results;
        }












        public  List<ResourceMaintenanceLogStatus> GetResourceMaintenanceLogStatus_Detail_Paging(
            ResourceMaintenanceLogStatusSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            )
        {
            Total = 0;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetResourceMaintenanceLogStatus_Detail_Paging", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, Criteria.RscrMaintLogID);
                cmd.AddParameter("@FromDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Criteria.FromDate));
                cmd.AddParameter("@ToDate", SqlDbType.DateTime, ConvertNullObjectToDBNull(Criteria.ToDate));

                cmd.AddParameter("@PageIndex", SqlDbType.Int, PageIndex);
                cmd.AddParameter("@PageSize", SqlDbType.Int, PageSize);
                cmd.AddParameter("@OrderBy", SqlDbType.NVarChar, "");
                cmd.AddParameter("@CountTotal", SqlDbType.Bit, CountTotal);
                cmd.AddParameter("@Total", SqlDbType.Int, DBNull.Value, ParameterDirection.Output);

                cn.Open();
                List<ResourceMaintenanceLogStatus> lst = null;

                IDataReader reader = ExecuteReader(cmd);

                lst = GetResourceMaintenanceLogStatusCollectionFromReader(reader);
                reader.Close();

                if (CountTotal && cmd.Parameters["@Total"].Value != null)
                {
                    Total = (int)cmd.Parameters["@Total"].Value;
                }
                else
                {
                    Total = -1;
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return lst;
            }
        }


        public  DateTime GetFixDateLast(Int64 RscrMaintLogID)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spGetFixDateLast", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, RscrMaintLogID);

                cn.Open();

                DataTable dt = ExecuteDataTable(cmd);

                if (dt != null && dt.Rows.Count > 0)
                {
                    CleanUpConnectionAndCommand(cn, cmd);
                    return Convert.ToDateTime(dt.Rows[0]["FixDate"]);
                }
                CleanUpConnectionAndCommand(cn, cmd);
                return DateTime.Now;
            }
        }


        public  bool AddNewResourceMaintenanceLogStatus(ResourceMaintenanceLogStatus obj)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLogStatus_Insert", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, obj.RscrMaintLogID);

                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);
                cn.Open();

                ExecuteNonQuery(cmd);
                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                {
                    results = true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }
        //dinh them
        public  bool AddResourceMaintenanceLogStatus_New(ResourceMaintenanceLogStatus obj)
        {
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLogStatus_InsertNew", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMaintLogID", SqlDbType.BigInt, obj.RscrMaintLogID);
                cmd.AddParameter("@StatusChangeDate", SqlDbType.DateTime, obj.StatusChangeDate);
                cmd.AddParameter("@UpdateStatusStaffID", SqlDbType.BigInt, obj.UpdateStatusStaffID);
                cmd.AddParameter("@V_CurrentStatus", SqlDbType.BigInt, obj.V_CurrentStatus);

                cn.Open();
                int count = cmd.ExecuteNonQuery();
                CleanUpConnectionAndCommand(cn, cmd);
                return count > 0;

            }
        }

        public  bool DeleteResourceMaintenanceLogStatus(Int64 RscrMainLogStatusID)
        {
            bool results = false;
            using (SqlConnection cn = new SqlConnection(this.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("spResourceMaintenanceLogStatus_Delete", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParameter("@RscrMainLogStatusID", SqlDbType.BigInt, RscrMainLogStatusID);

                cmd.AddParameter("@ReturnVal", SqlDbType.Int, DBNull.Value, ParameterDirection.ReturnValue);

                cn.Open();

                ExecuteNonQuery(cmd);

                int ReturnVal = (cmd.Parameters["@ReturnVal"].Value != null ? Convert.ToInt16(cmd.Parameters["@ReturnVal"].Value) : 0);
                if (ReturnVal > 0)
                {
                    results = true;
                }
                CleanUpConnectionAndCommand(cn, cmd);
            }
            return results;
        }


        #endregion




    }
}
