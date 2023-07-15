/*
 * 20170512 #001 CMN: Added method to get pcl room by catid
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataEntities;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using eHCMS.Configurations;
using System.Xml.Linq;
using eHCMS.Services.Core;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace eHCMS.DAL
{
    public abstract class ConfigurationManagerProviders : DataProviderBase
    {
        static private ConfigurationManagerProviders _instance = null;
        static public ConfigurationManagerProviders Instance
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
                    Type t = assem.GetType(Globals.Settings.ConfigurationManager.ProviderType);
                    _instance = (ConfigurationManagerProviders)Activator.CreateInstance(t);
                }
                return _instance;
            }
        }
        public ConfigurationManagerProviders()
        {
            this.ConnectionString = Globals.Settings.ConfigurationManager.ConnectionString;
        }

        #region RefDepartments member
        public abstract RefDepartments GetRefDepartmentsByID(long DeptID);
        public abstract List<RefDepartments> GetRefDepartments_AllParent();
        public abstract List<RefDepartments> GetRefDepartments_All();
        public abstract List<RefDepartments> RefDepartments_ByParDeptID(Int64 ParDeptID);


        public abstract bool DeleteRefDepartmentsByID(long DeptID);
        public abstract void UpdateRefDepartments(RefDepartments obj, out string Result);
        public abstract bool AddNewRefDepartments(RefDepartments obj);
        public abstract List<RefDepartments> SearchRefDepartments(RefDepartmentsSearchCriteria SearchCriteria);

        public abstract List<DeptLocation> GetDeptLocationFunc(long V_DeptType, long V_RoomFunction);
        public abstract List<DeptLocation> GetDeptLocationFuncExt(long V_DeptType, long V_RoomFunction, long V_DeptTypeOperation);
        public abstract List<DeptLocation> GetAllDeptLocationByDeptIDFunction(long DeptID, long RoomFunction);
        public abstract List<DeptLocation> GetAllDeptLocationByDeptID(long DeptID);
        public abstract List<DeptLocation> GetAllDeptLocLaboratory();
        protected virtual DeptLocation GetDepartmentObjFromReader(IDataReader reader)
        {
            DeptLocation p = new DeptLocation();
            try
            {
                if (reader.HasColumn("LID") && reader["LID"] != DBNull.Value)
                {
                    p.LID = (long)reader["LID"];
                }

                if (reader.HasColumn("DeptID") && reader["DeptID"] != DBNull.Value)
                {
                    p.DeptID = (long)reader["DeptID"];
                }

                if (reader.HasColumn("DeptLocationID") && reader["DeptLocationID"] != DBNull.Value)
                {
                    p.DeptLocationID = (long)reader["DeptLocationID"];
                }


                p.RefDepartment = new RefDepartment();
                try
                {
                    if (reader.HasColumn("DeptName") && reader["DeptName"] != DBNull.Value)
                    {
                        p.RefDepartment.DeptName = reader["DeptName"].ToString();
                    }

                    if (reader.HasColumn("DeptDescription") && reader["DeptDescription"] != DBNull.Value)
                    {
                        p.RefDepartment.DeptDescription = reader["DeptDescription"].ToString();
                    }

                    if (reader.HasColumn("V_DeptType") && reader["V_DeptType"] != DBNull.Value)
                    {
                        p.RefDepartment.V_DeptType = (long)reader["V_DeptType"];
                    }

                }
                catch { }

                p.Location = new Location();
                try
                {
                    p.Location.LocationName = reader["LocationName"].ToString();
                    p.Location.LocationDescription = reader["LocationDescription"].ToString();
                    p.Location.RoomType = new RoomType();
                    p.Location.RoomType.RmTypeName = reader["RmTypeName"].ToString();
                    p.Location.RoomType.RmTypeDescription = reader["RmTypeDescription"].ToString();
                }
                catch { }
            }
            catch
            { return null; }
            return p;
        }
        protected virtual List<DeptLocation> GetDepartmentCollectionFromReader(IDataReader reader)
        {
            List<DeptLocation> lst = new List<DeptLocation>();
            while (reader.Read())
            {
                lst.Add(GetDepartmentObjFromReader(reader));
            }
            return lst;
        }

        //List<RefDepartments> by collect V_DeptID
        public abstract List<RefDepartments> RefDepartments_ByInStrV_DeptType(string strV_DeptType, string V_DeptTypeOperation);
        //List<RefDepartments> by collect V_DeptID

        //<GetRefDepartment in Table DeptMedServiceItems>
        public abstract List<RefDepartments> GetRefDepartmentTree_InTable_DeptMedServiceItems();
        //<GetRefDepartment in Table DeptMedServiceItems>


        #endregion

        #region RefMedicalServiceItems
        public abstract List<RefMedicalServiceItem> RefMedicalServiceItemsByMedicalServiceTypeID(Int64 MedicalServiceTypeID);


        public abstract List<RefMedicalServiceItem> RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(
           RefMedicalServiceItemsSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );


        public abstract List<RefMedicalServiceItem> RefMedicalServiceItems_ByMedicalServiceTypeID_Paging(
           RefMedicalServiceItemsSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );

        public abstract List<RefMedicalServiceItem> RefMedicalServiceItems_IsPCLByMedServiceID_Paging(
        RefMedicalServiceItemsSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );

        public abstract void RefMedicalServiceItems_IsPCL_Save(
        RefMedicalServiceItem Obj, out string Result);

        public abstract void RefMedicalServiceItems_MarkDeleted(
        Int64 MedServiceID, out string Result);

        public abstract void RefMedicalServiceItems_EditInfo(
        RefMedicalServiceItem Obj, out string Result);


        public abstract void RefMedicalServiceItems_NotPCL_Add(
        DeptMedServiceItems Obj, out string Res);

        public abstract bool RefMedicalServiceItems_NotPCL_Add(
            RefMedicalServiceItem Obj);//, out string Res);
        public abstract bool RefMedicalServiceItems_NotPCL_Update(
            RefMedicalServiceItem Obj);

        //public abstract List<RefMedicalServiceItem> RefMedicalServiceItems_IsAllowRegistrationExam_ByDeptID_Paging(
        //RefMedicalServiceItemsSearchCriteria SearchCriteria,

        //     int PageIndex,
        //     int PageSize,
        //     string OrderBy,
        //     bool CountTotal,
        //     out int Total
        //     );


        public abstract List<RefMedicalServiceItem> RefMedicalServiceItems_In_DeptLocMedServices(
           RefMedicalServiceItemsSearchCriteria SearchCriteria);

        public abstract List<RefMedicalServiceItem> GetMedServiceItems_Paging(
            RefMedicalServiceItemsSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );

        #endregion

        #region PCLExamGroup
        public abstract List<PCLExamGroup> GetPCLExamGroup_ByMedServiceID_NoPaging(Int64 MedServiceID);
        public abstract List<RefDepartments> PCLExamGroup_GetListDeptID();
        #endregion

        #region PCLExamTypes

        public abstract List<PCLExamType> PCLExamTypes_NotYetPCLLabResultSectionID_Paging(
              PCLExamTypeSearchCriteria SearchCriteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );

        public abstract List<PCLExamType> GetPCLExamTypes_Paging(
              PCLExamTypeSearchCriteria SearchCriteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );
        //public abstract List<RefMedicalServiceItem> PCLExamTypes_GetListMedServiceID();


        public abstract List<PCLExamType> PCLExamTypesAndPriceIsActive_Paging(
            PCLExamTypeSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );


        public abstract void PCLExamTypes_Save_NotIsLab(
            PCLExamType Obj, out string Result, out long PCLExamTypeID_New);


        public abstract void PCLExamTypes_Save_IsLab(
            PCLExamType Obj,

            /*ExamType la ExamTest*/
            bool TestItemIsExamType,
            string PCLExamTestItemUnitForPCLExamType,
            string PCLExamTestItemRefScaleForPCLExamType,
            /*ExamType la ExamTest*/

            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Insert,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Update,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Delete,
            out string Result, out long PCLExamTypeID_New);


        public abstract void PCLExamTypes_MarkDelete(Int64 PCLExamTypeID, out string Result);

        public abstract List<PCLExamType> PCLExamTypes_List_Paging(
          PCLExamTypeSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
          );

        public abstract List<PCLExamType> PCLExamTypesByDeptLocationID_LAB(long DeptLocationID);
        public abstract List<PCLExamType> PCLExamTypesByDeptLocationID_NotLAB(long V_PCLMainCategory, long PCLExamTypeSubCategoryID, long DeptLocationID);



        #endregion

        #region PCLExamTypeCombo

        public abstract List<PCLExamTypeCombo> PCLExamTypeCombo_Search(GeneralSearchCriteria SearchCriteria);

        public abstract List<PCLExamTypeComboItem> PCLExamTypeComboItems_ByComboID(long ComboID);

        public abstract List<PCLExamTypeComboItem> PCLExamTypeComboItems_All();

        public abstract bool PCLExamTypeCombo_Save(PCLExamTypeCombo item, List<PCLExamTypeComboItem> ComboXML_Insert, List<PCLExamTypeComboItem> ComboXML_Update, List<PCLExamTypeComboItem> ComboXML_Delete, out long ID);

        public abstract bool PCLExamTypeCombo_Delete(long ID);
        #endregion

        #region PCLExamTypeMedServiceDefItems
        public abstract List<PCLExamType> PCLExamTypeMedServiceDefItems_ByMedServiceID(PCLExamTypeSearchCriteria SearchCriteria, Int64 MedServiceID);

        //Save list choose
        public abstract bool PCLExamTypeMedServiceDefItems_XMLInsert(Int64 MedServiceID, IEnumerable<PCLExamType> objCollect);
        //Save list choose

        #endregion

        #region DeptMedServiceItems
        public abstract List<MedServiceItemPrice> GetDeptMedServiceItems_Paging(
            DeptMedServiceItemsSearchCriteria SearchCriteria,
              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        public abstract List<DeptMedServiceItems> GetDeptMedServiceItems_DeptIDPaging(
            DeptMedServiceItemsSearchCriteria SearchCriteria,
              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        public abstract List<MedServiceItemPrice> GetMedServiceItemPrice_Paging(
            MedServiceItemPriceSearchCriteria SearchCriteria,
              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        public abstract bool DeptMedServiceItems_TrueDelete(
            Int64 DeptMedServItemID,
            Int64 MedServItemPriceID,
            Int64 MedServiceID);

        public abstract bool DeptMedServiceItems_InsertXML(IList<DeptMedServiceItems> lstDeptMedServiceItems);
        public abstract bool DeptMedServiceItems_DeleteXML(IList<DeptMedServiceItems> lstDeptMedServiceItems);
        //public abstract List<RefDepartments> DeptMedServiceItems_GetDeptHasMedserviceIsPCL();

        #endregion

        #region RefMedicalServiceTypes
        public abstract List<RefMedicalServiceType> GroupRefMedicalServiceTypes_ByMedicalServiceTypeID(Int64 DeptID, string MedServiceName);

        public abstract List<RefMedicalServiceType> GetAllMedicalServiceTypes();

        public abstract List<RefMedicalServiceType> RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(Int64 DeptIDint, int V);

        public abstract List<RefMedicalServiceType> GetAllMedicalServiceTypes_SubtractPCL();


        public abstract List<RefMedicalServiceType> RefMedicalServiceTypes_Paging(
           RefMedicalServiceTypeSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );

        public abstract void RefMedicalServiceTypes_CheckBeforeInsertUpdate(
        RefMedicalServiceType Obj,
            out string Result);

        public abstract void RefMedicalServiceTypes_AddEdit(RefMedicalServiceType Obj, out string Result);

        public abstract bool RefMedicalServiceTypes_MarkDelete(Int64 MedicalServiceTypeID);

        public abstract IList<RefMedicalServiceType> RefMedicalServiceTypes_ByV_RefMedicalServiceTypes(Int64 V_RefMedicalServiceTypes);


        #endregion

        #region "MedServiceItemPrice"
        //Danh sach gia cua dich vu
        public abstract List<MedServiceItemPrice> MedServiceItemPriceByDeptMedServItemID_Paging(
            MedServiceItemPriceSearchCriteria SearchCriteria,

              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        protected override MedServiceItemPrice GetMedServiceItemPriceFromReader(IDataReader reader)
        {
            MedServiceItemPrice p = base.GetMedServiceItemPriceFromReader(reader);

            //Xét CanEdit, CanDelete cho Items này
            bool CanEdit = false;
            bool CanDelete = false;
            string PriceType = "";


            Int64 MedServItemPriceID = 0;
            Int64.TryParse(reader["MedServItemPriceID"].ToString(), out MedServItemPriceID);
            if (MedServItemPriceID > 0)
            {
                MedServiceItemPrice_CheckCanEditCanDelete(p.MedServItemPriceID, out CanEdit, out CanDelete, out PriceType);
            }
            p.CanEdit = CanEdit;
            p.CanDelete = CanDelete;
            p.PriceType = PriceType;
            //Xét CanEdit, CanDelete cho Items này
            return p;
        }

        public abstract void MedServiceItemPrice_CheckCanEditCanDelete(
            Int64 MedServItemPriceID,
            out bool CanEdit,
            out bool CanDelete,
            out string PriceType);

        public abstract void MedServiceItemPrice_Save(MedServiceItemPrice Obj, out string Result);

        public abstract void MedServiceItemPrice_MarkDelete(Int64 MedServItemPriceID, out string Result);

        public abstract MedServiceItemPrice MedServiceItemPrice_ByMedServItemPriceID(Int64 MedServItemPriceID);

        #endregion

        #region PCLItem
        //public abstract void PCLItemsInsertUpdate(
        //PCLItem Obj, bool SaveToDB, out string Result);                                

        //Save list choose
        public abstract bool PCLItems_XMLInsert(Int64 PCLFormID, IEnumerable<PCLExamType> ObjPCLExamTypeList);
        //Save list choose

        //public abstract List<PCLItem> PCLItems_GetPCLExamTypeIDByMedServiceID(Int64 MedServiceID);

        public abstract List<PCLExamType> PCLItems_GetPCLExamTypeIDByPCLSectionID(string PCLExamTypeName, Int64 PCLSectionID);

        public abstract List<PCLExamType> PCLItems_ByPCLFormID(PCLExamTypeSearchCriteria SearchCriteria, Int64 PCLFormID);

        public abstract List<PCLExamType> PCLExamType_WithDeptLocIDs_GetAll();

        public abstract List<PCLExamType> PCLItems_SearchAutoComplete(
           PCLExamTypeSearchCriteria SearchCriteria,

           int PageIndex,
           int PageSize,
           string OrderBy,
           bool CountTotal,
           out int Total
           );

        public abstract List<PCLExamType> GetPCLExamType_byComboID(long PCLExamTypeComboID);

        public abstract List<PCLExamType> GetPCLExamType_byHosID(long HosID);
        #endregion

        #region PCLForms
        public abstract List<PCLForm> PCLForms_GetList_Paging(
 PCLFormsSearchCriteria SearchCriteria,

   int PageIndex,
   int PageSize,
   string OrderBy,
   bool CountTotal,
   out int Total
           );

        public abstract void PCLForms_Save(PCLForm Obj, out string Result);

        public abstract void PCLForms_MarkDelete(Int64 PCLFormID, out string Result);


        #endregion

        #region PCLSections

        public abstract List<PCLSection> PCLSections_All();

        //public abstract List<PCLSection> PCLSectionsByPCLFormID(Int64 PCLFormID);

        public abstract List<PCLSection> PCLSections_GetList_Paging(
PCLSectionsSearchCriteria SearchCriteria,

int PageIndex,
int PageSize,
string OrderBy,
bool CountTotal,
out int Total
        );

        public abstract void PCLSections_Save(PCLSection Obj, out string Result);

        public abstract void PCLSections_MarkDelete(Int64 PCLSectionID, out string Result);



        #endregion

        #region Locations
        public abstract void Locations_InsertUpdate(
        Location Obj, bool SaveToDB, out string Result);

        //Save list 
        public abstract bool Locations_XMLInsert(Location objCollect);
        //Save list 

        //list_paging
        public abstract List<Location> Locations_ByRmTypeID_Paging(
           LocationSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );
        //list_paging


        public abstract void Locations_MarkDeleted(
            Int64 LID, out string Result);


        #endregion

        #region RoomType
        public abstract List<RoomType> RoomType_GetAll();

        public abstract List<RoomType> RoomType_GetList_Paging(
           RoomTypeSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );

        public abstract void RoomType_Save(
        RoomType Obj, out string Result);

        public abstract void RoomType_MarkDelete(
        Int64 RmTypeID, out string Result);


        #endregion

        #region DeptLocation
        public abstract List<Location> DeptLocation_ByDeptID(Int64 DeptID, Int64 RmTypeID, string LocationName);

        public abstract List<RoomType> DeptLocation_GetRoomTypeByDeptID(Int64 DeptID);

        public abstract bool DeptLocation_CheckLIDExists(Int64 DeptID, Int64 LID);

        public abstract bool DeptLocation_XMLInsert(Int64 DeptID, IEnumerable<Location> objCollect);

        public abstract void DeptLocation_MarkDeleted(Int64 DeptLocationID, out string Result);

        public abstract List<DeptLocation> DeptLocation_ByMedicalServiceTypeIDDeptID(Int64 MedicalServiceTypeID, Int64 DeptID);

        public abstract List<DeptLocation> GetAllLocationsByDeptID(long? DeptID, long? V_RoomFunction = null);

        public abstract List<DeptLocation> ListDeptLocation_ByPCLExamTypeID(long PCLExamTypeID);

        #endregion

        #region DeptLocMedServices
        //ds dv not paging
        //public abstract List<RefMedicalServiceItem> DeptMedServiceItems_GetListMedServiceIDByDeptID_AllForChoose(
        //     Int64 DeptID,               
        //     Int64 MedicalServiceTypeID,  
        //     string MedServiceName);

        //Group loại dv từ ds dv not paging
        public abstract List<RefMedicalServiceType> DeptMedServiceItems_GroupMedSerTypeID_AllForChoose(
            Int64 DeptID,
            Int64 MedicalServiceTypeID,
            string MedServiceName);

        //Ở DB
        //ds dv not paging
        public abstract List<RefMedicalServiceItem> DeptLocMedServices_GetListMedServiceIDByDeptID_HasChoose(
             Int64 DeptID,
             Int64 DeptLocationID,
             Int64 MedicalServiceTypeID,
             string MedServiceName);

        //Group loại dv từ ds dv not paging
        public abstract List<RefMedicalServiceType> DeptLocMedServices_GroupMedSerTypeID_HasChoose(
            Int64 DeptID,
            Int64 DeptLocationID,
            Int64 MedicalServiceTypeID,
            string MedServiceName);
        //Ở DB

        //Save list 
        public abstract bool DeptLocMedServices_XMLInsert(Int64 DeptLocationID, IEnumerable<RefMedicalServiceItem> objCollect);
        //Save list 

        #endregion

        #region PCLExamTypePrices

        public abstract List<PCLExamTypePrice> PCLExamTypePrices_ByPCLExamTypeID_Paging(
            PCLExamTypePriceSearchCriteria SearchCriteria,

              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        public abstract void PCLExamTypePrices_CheckCanEditCanDelete(
           Int64 PCLExamTypePriceID,

           out bool CanEdit,
           out bool CanDelete,
           out string PriceType);

        protected override PCLExamTypePrice GetPCLExamTypePriceFromReader(IDataReader reader)
        {
            PCLExamTypePrice p = base.GetPCLExamTypePriceFromReader(reader);
            ////Xét CanEdit, CanDelete cho Items này
            //bool CanEdit = false;
            //bool CanDelete = false;
            //string PriceType = "";

            //Int64 PCLExamTypePriceID = 0;
            //Int64.TryParse(reader["PCLExamTypePriceID"].ToString(), out PCLExamTypePriceID);
            ////if (PCLExamTypePriceID > 0)
            ////{
            ////    PCLExamTypePrices_CheckCanEditCanDelete(Convert.ToInt64(reader["PCLExamTypePriceID"]), out CanEdit, out CanDelete, out PriceType);
            ////}
            //p.CanEdit = CanEdit;
            //p.CanDelete = CanDelete;
            //p.PriceType = PriceType;
            ////Xét CanEdit, CanDelete cho Items này
            return p;
        }

        public abstract void PCLExamTypePrices_Save(PCLExamTypePrice Obj, out string Result);

        public abstract void PCLExamTypePrices_MarkDelete(Int64 PCLExamTypePriceID, out string Result);

        public abstract PCLExamTypePrice PCLExamTypePrices_ByPCLExamTypePriceID(Int64 PCLExamTypePriceID);

        #endregion

        #region "PCLGroups"
        public abstract IList<PCLGroup> PCLGroups_GetAll(long? V_PCLCategory);

        public abstract List<PCLGroup> PCLGroups_GetList_Paging(
PCLGroupsSearchCriteria SearchCriteria,

int PageIndex,
int PageSize,
string OrderBy,
bool CountTotal,
out int Total
     );

        public abstract void PCLGroups_Save(PCLGroup Obj, out string Result);

        public abstract void PCLGroups_MarkDelete(Int64 PCLGroupID, out string Result);

        #endregion

        #region MedServiceItemPriceList
        public abstract void MedServiceItemPriceList_AddNew(MedServiceItemPriceList Obj, IEnumerable<MedServiceItemPrice> ObjMedServiceItemPrice, out string Result_PriceList);

        public abstract List<MedServiceItemPriceList> MedServiceItemPriceList_GetList_Paging(
    MedServiceItemPriceListSearchCriteria SearchCriteria,

     int PageIndex,
     int PageSize,
     string OrderBy,
     bool CountTotal,
     out int Total
     );

        protected override MedServiceItemPriceList GetMedServiceItemPriceListFromReader(IDataReader reader)
        {
            MedServiceItemPriceList p = base.GetMedServiceItemPriceListFromReader(reader);

            //Xét CanEdit, CanDelete cho Items này
            bool CanEdit = false;
            bool CanDelete = false;
            string PriceListType = "";


            Int64 MedServiceItemPriceListID = 0;
            Int64.TryParse(reader["MedServiceItemPriceListID"].ToString(), out MedServiceItemPriceListID);
            if (MedServiceItemPriceListID > 0)
            {
                MedServiceItemPriceList_CheckCanEditCanDelete(p.MedServiceItemPriceListID, out CanEdit, out CanDelete, out PriceListType);
            }
            p.CanEdit = CanEdit;
            p.CanDelete = CanDelete;
            p.PriceListType = PriceListType;
            //Xét CanEdit, CanDelete cho Items này
            return p;
        }

        public abstract void MedServiceItemPriceList_CheckCanEditCanDelete(
         Int64 MedServiceItemPriceListID,
         out bool CanEdit,
         out bool CanDelete,
         out string PriceListType);


        public abstract void MedServiceItemPriceList_MarkDelete(
        Int64 MedServiceItemPriceListID, out string Result);


        public abstract List<MedServiceItemPrice> MedServiceItemPriceList_Detail(
       DeptMedServiceItemsSearchCriteria SearchCriteria,
      int PageIndex,
      int PageSize,
      string OrderBy,
      bool CountTotal,
      out int Total
      );

        public abstract void MedServiceItemPriceList_Update(MedServiceItemPriceList Obj, IEnumerable<MedServiceItemPrice> ObjCollection, IEnumerable<MedServiceItemPrice> ObjCollection_Insert, IEnumerable<MedServiceItemPrice> ObjCollection_Update, out string Result_PriceList);

        public abstract void MedServiceItemPriceList_CheckCanAddNew(Int64 DeptID, Int64 MedicalServiceTypeID, out bool CanAddNew);

        #endregion

        #region "PCLExamTypePriceList"

        public abstract void PCLExamTypePriceList_CheckCanAddNew(out bool CanAddNew);

        public abstract void PCLExamTypePriceList_AddNew(PCLExamTypePriceList Obj, IEnumerable<PCLExamType> ObjPCLExamType, out string Result_PriceList);

        public abstract List<PCLExamTypePriceList> PCLExamTypePriceList_GetList_Paging(
    PCLExamTypePriceListSearchCriteria SearchCriteria,

     int PageIndex,
     int PageSize,
     string OrderBy,
     bool CountTotal,
     out int Total
     );

        protected override PCLExamTypePriceList GetPCLExamTypePriceListFromReader(IDataReader reader)
        {
            PCLExamTypePriceList p = base.GetPCLExamTypePriceListFromReader(reader);

            //Xét CanEdit, CanDelete cho Items này
            //bool CanEdit = false;
            //bool CanDelete = false;
            //string PriceListType = "";


            Int64 PCLExamTypePriceListID = 0;
            Int64.TryParse(reader["PCLExamTypePriceListID"].ToString(), out PCLExamTypePriceListID);
            //if (PCLExamTypePriceListID > 0)
            //{
            //    PCLExamTypePriceList_CheckCanEditCanDelete(p.PCLExamTypePriceListID, out CanEdit, out CanDelete, out PriceListType);
            //}
            //p.CanEdit = CanEdit;
            //p.CanDelete = CanDelete;
            //p.PriceListType = PriceListType;
            //Xét CanEdit, CanDelete cho Items này
            return p;
        }

        public abstract void PCLExamTypePriceList_CheckCanEditCanDelete(
         Int64 PCLExamTypePriceListID,
         out bool CanEdit,
         out bool CanDelete,
         out string PriceListType);


        public abstract void PCLExamTypePriceList_Delete(Int64 PCLExamTypePriceListID, out string Result);


        public abstract List<PCLExamType> PCLExamTypePriceList_Detail(
        long PCLExamTypePriceListID,
        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        out int Total
        );

        public abstract void PCLExamTypePriceList_Update(PCLExamTypePriceList Obj, IEnumerable<PCLExamType> ObjCollection_Update, out string Result_PriceList);


        #endregion

        #region PCLExamTestItems
        public abstract List<PCLExamTypeTestItems> PCLExamTestItems_ByPCLExamTypeID(Int64 PCLExamTypeID);
        #endregion

        #region PCLResultParamImplementations
        /*==== #001 ====*/
        public abstract List<PCLResultParamImplementations> PCLResultParamImplementations_GetAll(long? PCLExamTypeSubCategoryID = null);
        /*==== #001 ====*/
        #endregion

        #region PCLExamTypeSubCategory
        public abstract List<PCLExamTypeSubCategory> PCLExamTypeSubCategory_ByV_PCLMainCategory(Int64 V_PCLMainCategory);
        #endregion

        #region PCLExamTypeLocations
        public abstract IList<PCLExamType> PCLExamTypeLocations_ByDeptLocationID(string PCLExamTypeName, Int64 DeptLocationID);
        public abstract bool PCLExamTypeLocations_XMLInsert(Int64 DeptLocationID, IEnumerable<PCLExamType> ObjList);
        public abstract void PCLExamTypeLocations_MarkDeleted(Int64 PCLExamTypeID, Int64 DeptLocationID, out string Result);
        #endregion

        #region "HITransactionType"
        public abstract IList<HITransactionType> HITransactionType_GetListNoParentID();
        //▼===== 25072018 TTM
        public abstract IList<HITransactionType> HITransactionType_GetListNoParentID_New();
        //▲===== 25072018 TTM
        #endregion

        #region "RefMedicalServiceGroups"
        public abstract IList<RefMedicalServiceGroups> RefMedicalServiceGroups_GetAll();
        #endregion

        #region PCLExamTypeExamTestPrint
        public abstract IList<PCLExamTypeExamTestPrint> PCLExamTypeExamTestPrint_GetList_Paging(
            PCLExamTypeExamTestPrintSearchCriteria SearchCriteria,

     int PageIndex,
     int PageSize,
     string OrderBy,
     bool CountTotal,
     out int Total);

        public abstract IList<PCLExamTypeExamTestPrint> PCLExamTypeExamTestPrintIndex_GetAll();

        public abstract void PCLExamTypeExamTestPrint_Save(IEnumerable<PCLExamTypeExamTestPrint> ObjList, out string Result);

        public abstract void PCLExamTypeExamTestPrintIndex_Save(IEnumerable<PCLExamTypeExamTestPrint> ObjList, out string Result);
        #endregion

        #region MAPPCLExamTypeDeptLoc
        public abstract new Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc();
        #endregion

    }
}


