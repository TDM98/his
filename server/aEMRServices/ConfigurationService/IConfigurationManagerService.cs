/*
 * 20170512 #001 CMN: Added method to get pcl room by catid
 * 20190603 #002 TNHX: [BM0011782] get all RefPurposeForAccountant
 * 20190926 #003 TNHX: [BM] get list FilterPrescriptionsHasHIPayTable
 * 20220620 #004 DatTB: Thêm filter danh mục xét nghiệm
 * 20230509 #005 DatTB: IssueID: 3254 | Thêm nút xuất excel cho các danh mục/cấu hình
 * 20230518 #006 DatTB: Thêm service cho mẫu bệnh phẩm
 * 20230531 #007 QTD:   Thêm quản lý danh mục
 * 20230601 #008 DatTB: IssueID: 3254 | Chỉnh sửa/Gộp các function xuất excel danh mục/cấu hình (Bỏ Func cũ)
 * 20230622 #009 DatTB:
 * + Thêm filter theo mã ICD đã lưu của nhóm bệnh
 * + Thêm function chỉnh sửa ICD của nhóm bệnh
 * + Thay đổi điều kiện gàng buộc ICD
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ErrorLibrary;
using DataEntities;
using System.Collections.ObjectModel;
using eHCMS.Configurations;
using DataEntities.MedicalInstruction;

namespace ConfigurationManagerService
{
    [ServiceContract]
    public interface IConfigurationManagerService
    {
        #region RefDepartments

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartmentsTree> GetRefDepartments_TreeFunction(string strV_DeptType, bool ShowDeptLocation, long RoomFunction);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartmentsTree> GetRefDepartments_TreeSegment(string strV_DeptType, bool ShowDeptLocation, long RoomFunction);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartmentsTree> RefDepartments_Tree(string strV_DeptType, bool ShowDeptLocation, string V_DeptTypeOperation);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartmentsTree> RefDepartments_Tree_ByDeptID(string strV_DeptType, bool ShowDeptLocation, long DeptID, long DeptLocID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartments> RefDepartment_SubtractAllChild_ByDeptID(Int64 DeptID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartments> RefDepartments_RecursiveByDeptID(Int64 DeptID);


        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<RefDepartments> GetRefDepartments_All();        

        [OperationContract]
        [FaultContract(typeof(AxException))]
        RefDepartments GetRefDepartmentsByID(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartments> RefDepartments_ByParDeptID(Int64 ParDeptID);


        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<RefDepartmentsTree> GetTreeViewRefDepartmentsParent();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddNewRefDepartments(RefDepartments obj);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void UpdateRefDepartments(RefDepartments obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteRefDepartments(long DeptID);



        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //IList<RefDepartments> SearchRefDepartments(RefDepartmentsSearchCriteria criteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartmentsTree> GetSearchTreeView(RefDepartmentsSearchCriteria SearchCriteria);


        //<GetRefDepartmentForDeptMedServiceItems>
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<RefDepartments> GetRefDepartmentForDeptMedServiceItems();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefDepartmentsTree> TreeRefDepartment_InTable_DeptMedServiceItems();
        //</GetRefDepartmentForDeptMedServiceItems>

        //#region Tree 2 nút gốc cha Khoa và Phòng
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<RefDepartmentsTree> TreeRefDepartment_Khoa_Phong();
        //#endregion


        ////Get List Root
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<RefDepartments> GetRefDepartments_AllParent();
        ////Get List Root



        //#region "Tree Khoa trong Bảng DeptMedServiceItems là PCL"
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<RefDepartmentsTree> DeptMedServiceItems_TreeDeptHasMedserviceIsPCL();
        //#endregion



        #endregion

        #region RefMedicalServiceItems
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedicalServiceItem> RefMedicalServiceItemsByMedicalServiceTypeID(Int64 MedicalServiceTypeID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedicalServiceItem> RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(
           RefMedicalServiceItemsSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedicalServiceItem> RefMedicalServiceItems_ByMedicalServiceTypeID_Paging(
           RefMedicalServiceItemsSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedicalServiceItem> RefMedicalServiceItems_IsPCLByMedServiceID_Paging(
        RefMedicalServiceItemsSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RefMedicalServiceItems_IsPCL_Save(RefMedicalServiceItem Obj, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RefMedicalServiceItems_MarkDeleted(Int64 MedServiceID, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RefMedicalServiceItems_EditInfo(RefMedicalServiceItem Obj, out string Result);



        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RefMedicalServiceItems_NotPCL_Add(DeptMedServiceItems Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RefMedicalServiceItems_NotPCL_Insert(RefMedicalServiceItem Obj, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RefMedicalServiceItems_NotPCL_Update(RefMedicalServiceItem Obj);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<RefMedicalServiceItem> RefMedicalServiceItems_IsAllowRegistrationExam_ByDeptID_Paging(
        //RefMedicalServiceItemsSearchCriteria SearchCriteria,

        //    int PageIndex,
        //    int PageSize,
        //    string OrderBy,
        //    bool CountTotal,
        //    out int Total
        //    );


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedicalServiceItem> RefMedicalServiceItems_In_DeptLocMedServices(
        RefMedicalServiceItemsSearchCriteria SearchCriteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedicalServiceItem> GetMedServiceItems_Paging(
            RefMedicalServiceItemsSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );
        #endregion

        #region PCLExamGroup
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamGroup> GetPCLExamGroup_ByMedServiceID_NoPaging(Int64 MedServiceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefDepartments> PCLExamGroup_GetListDeptID();
        #endregion

        #region PCLExamTypes

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> PCLExamTypes_NotYetPCLLabResultSectionID_Paging(
              PCLExamTypeSearchCriteria SearchCriteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> GetPCLExamTypes_Paging(
              PCLExamTypeSearchCriteria SearchCriteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<RefMedicalServiceItem> PCLExamTypes_GetListMedServiceID();


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> PCLExamTypesAndPriceIsActive_Paging(
            PCLExamTypeSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLExamTypes_Save_NotIsLab(PCLExamType Obj, bool IsInsert, long StaffID, out string Result, out long PCLExamTypeID_New);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLExamTypes_Save_IsLab(
            PCLExamType Obj,
            bool IsInsert,
            /*ExamType la ExamTest*/
            bool TestItemIsExamType,
            string PCLExamTestItemUnitForPCLExamType,
            string PCLExamTestItemRefScaleForPCLExamType,
            /*ExamType la ExamTest*/
            long StaffID,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Insert,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Update,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Delete,
            out string Result, out long PCLExamTypeID_New);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLExamTypes_MarkDelete(Int64 PCLExamTypeID, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> PCLExamTypes_List_Paging(
            PCLExamTypeSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> PCLExamTypesByDeptLocationID_LAB(long DeptLocationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> PCLExamTypesByDeptLocationID_NotLAB(long V_PCLMainCategory, long PCLExamTypeSubCategoryID, long DeptLocationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLExamTypes_Save_General(
            PCLExamType Obj,
            bool IsInsert,
            bool TestItemIsExamType,
            string PCLExamTestItemUnitForPCLExamType,
            string PCLExamTestItemRefScaleForPCLExamType,
            long StaffID,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Insert,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Update,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Delete,
            out string Result, out long PCLExamTypeID_New);

        #endregion

        #region PCLExamTypeCombo

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamTypeCombo> PCLExamTypeCombo_Search(GeneralSearchCriteria SearchCriteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamTypeComboItem> PCLExamTypeComboItems_ByComboID(long ComboID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamTypeComboItem> PCLExamTypeComboItems_All(long? PCLExamTypePriceListID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLExamTypeCombo_Save(PCLExamTypeCombo item, List<PCLExamTypeComboItem> ComboXML_Insert, List<PCLExamTypeComboItem> ComboXML_Update, List<PCLExamTypeComboItem> ComboXML_Delete, out long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLExamTypeCombo_Delete(long ID);
        #endregion

        #region PCLExamTypeMedServiceDefItem

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> PCLExamTypeMedServiceDefItems_ByMedServiceID(PCLExamTypeSearchCriteria SearchCriteria, Int64 MedServiceID);


        //Save danh sach vua chon tu UI
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLExamTypeMedServiceDefItems_XMLInsert(Int64 MedServiceID, IEnumerable<PCLExamType> objCollect);
        //Save danh sach vua chon tu UI
        #endregion

        #region DeptMedServiceItems
        //DeptMedServiceItems
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<MedServiceItemPrice> GetDeptMedServiceItems_Paging(
            DeptMedServiceItemsSearchCriteria SearchCriteria,
              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptMedServiceItems> GetDeptMedServiceItems_DeptIDPaging(
            DeptMedServiceItemsSearchCriteria SearchCriteria,
              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<MedServiceItemPrice> GetMedServiceItemPrice_Paging(
            MedServiceItemPriceSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeptMedServiceItems_TrueDelete(
            Int64 DeptMedServItemID,
            Int64 MedServItemPriceID,
            Int64 MedServiceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeptMedServiceItems_InsertXML(IList<DeptMedServiceItems> lstDeptMedServiceItems);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeptMedServiceItems_DeleteXML(IList<DeptMedServiceItems> lstDeptMedServiceItems);
        //DeptMedServiceItems
        #endregion

        #region "RefMedicalServiceTypes"
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceType> GroupRefMedicalServiceTypes_ByMedicalServiceTypeID(Int64 DeptID, string MedServiceName);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceType> GetAllMedicalServiceTypes();


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceType> RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(Int64 DeptID, int V);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceType> GetAllMedicalServiceTypes_SubtractPCL();


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceType> RefMedicalServiceTypes_Paging(
           RefMedicalServiceTypeSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RefMedicalServiceTypes_CheckBeforeInsertUpdate(
        RefMedicalServiceType Obj,
            out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RefMedicalServiceTypes_AddEdit(RefMedicalServiceType Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool RefMedicalServiceTypes_MarkDelete(Int64 MedicalServiceTypeID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceType> RefMedicalServiceTypes_ByV_RefMedicalServiceTypes(Int64 V_RefMedicalServiceTypes);


        #endregion

        #region "MedServiceItemPrice"
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<MedServiceItemPrice> MedServiceItemPriceByDeptMedServItemID_Paging(
            MedServiceItemPriceSearchCriteria SearchCriteria,

              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void MedServiceItemPrice_Save(MedServiceItemPrice Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void MedServiceItemPrice_MarkDelete(Int64 MedServItemPriceID, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        MedServiceItemPrice MedServiceItemPrice_ByMedServItemPriceID(Int64 MedServItemPriceID);

        #endregion

        #region PCLItemID
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void PCLItemsInsertUpdate(
        //PCLItem Obj, bool SaveToDB, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLItems_XMLInsert(Int64 PCLFormID, IEnumerable<PCLExamType> ObjPCLExamTypeList);



        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<PCLItem> PCLItems_GetPCLExamTypeIDByMedServiceID(Int64 MedServiceID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> PCLItems_GetPCLExamTypeIDByPCLSectionID(string PCLExamTypeName, Int64 PCLSectionID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> PCLItems_ByPCLFormID(PCLExamTypeSearchCriteria SearchCriteria, Int64 PCLFormID, long? PCLExamTypePriceListID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> PCLExamType_WithDeptLocIDs_GetAll();


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> PCLItems_SearchAutoComplete(
           PCLExamTypeSearchCriteria SearchCriteria,

           int PageIndex,
           int PageSize,
           string OrderBy,
           bool CountTotal,
           out int Total
           );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> GetPCLExamType_byComboID(long PCLExamTypeComboID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> GetPCLExamType_byHosID(long HosID);

        #endregion

        #region PCLForms

        //list paging
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLForm> PCLForms_GetList_Paging(
 PCLFormsSearchCriteria SearchCriteria,

   int PageIndex,
   int PageSize,
   string OrderBy,
   bool CountTotal,
   out int Total
          );


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLForms_Save(PCLForm Obj, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLForms_MarkDelete(Int64 PCLFormID, out string Result);


        #endregion

        #region PCLSections

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLSection> PCLSections_All();

        //▼==== #004
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> PCLExamTypes_All();
        //▲==== #004

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<PCLSection> PCLSectionsByPCLFormID(Int64 PCLFormID);


        //list paging
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLSection> PCLSections_GetList_Paging(
 PCLSectionsSearchCriteria SearchCriteria,

   int PageIndex,
   int PageSize,
   string OrderBy,
   bool CountTotal,
   out int Total
          );


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLSections_Save(PCLSection Obj, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLSections_MarkDelete(Int64 PCLSectionID, out string Result);


        #endregion

        #region Locations
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void Locations_InsertUpdate(
        Location Obj, bool SaveToDB, out string Result);

        //Save list
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool Locations_XMLInsert(Location objCollect);
        //Save list

        //list paging
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Location> Locations_ByRmTypeID_Paging(
        LocationSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
          );
        //list pagind

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void Locations_MarkDeleted(Int64 LID, out string Result);

        #endregion

        #region RoomType
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RoomType> RoomType_GetAll();


        //list paging
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RoomType> RoomType_GetList_Paging(
        RoomTypeSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
          );
        //list pagind


        //Save list
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RoomType_Save(RoomType Obj, out string Result);
        //Save list


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RoomType_MarkDelete(Int64 RmTypeID, out string Result);


        #endregion

        #region ICD
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ICD> ICD_GetAll();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DiseaseChapters> DiseaseChapters_GetAll();

        //list paging
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DiseaseChapters> Chapter_Paging(string SearchChapter, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Diseases> Diseases_Paging(int DiseaseChapterID, string SearchDisease, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ICD> ICD_ByIDCode_Paging(ICDSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ICD> ICD_ByDiseaseID_Paging(long Disease_ID, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Diseases> Diseases_ByChapterID(int DiseaseChapterID);
        //list pagind
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void Chapter_Save(DiseaseChapters Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void Diseases_Save(Diseases Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void ICD_Save(ICD Obj, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void ICD_MarkDelete(Int64 IDCode, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ICD_XMLInsert(Int64 Disease_ID, IEnumerable<ICD> objCollect);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ICD> SearchICD_Paging(long DiseaseChapterID, long Disease_ID, string ICD10Code, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);
        #endregion

        #region InsuranceBenefit

        //list paging
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<InsuranceBenefitCategories_Data> InsuranceBenefitPaging(
          string HIPCode,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
          );
        //list pagind

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void InsuranceBenefitCategories_Save(InsuranceBenefitCategories_Data Obj, out string Result);


        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void ICD_MarkDelete(Int64 IDCode, out string Result);

        #endregion

        #region Hospital
        //list paging
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Hospital> HospitalPaging(
        HospitalSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
          );
        //list pagind

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void ICD_Save(ICD Obj, out string Result);


        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void ICD_MarkDelete(Int64 IDCode, out string Result);


        #endregion

        #region CitiesProvince
        //list paging
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<CitiesProvince> CitiesProvince_Paging(
        string SearchCitiesProvinces,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
          );
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SuburbNames> SuburbNames_Paging(
          long CityProvinceID, string SearchSuburbNames,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
          );
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<WardNames> WardNames_Paging(
          long CityProvinceID, long SuburbNameID, string SearchWardNames,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
          );
        //list pagind

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void CitiesProvinces_Save(CitiesProvince Obj, out string Result);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void SuburbNames_Save(SuburbNames Obj, out string Result);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void WardNames_Save(WardNames Obj, out string Result);

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void ICD_MarkDelete(Int64 IDCode, out string Result);


        #endregion

        #region Job
        //list paging
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Lookup> Job_Paging(
        string SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
          );
        //list pagind

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void Job_Save(Lookup Obj, out string Result);


        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void ICD_MarkDelete(Int64 IDCode, out string Result);
        #endregion

        #region AdmissionCriteria
  
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<AdmissionCriteria> AdmissionCriteria_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);
        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void AdmissionCriteria_Save(AdmissionCriteria Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<AdmissionCriteria> GetListAdmissionCriteria();

        #endregion

        #region AdmissionCriterion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SymptomCategory> SymptomCategory_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void SymptomCategory_Save(SymptomCategory Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<SymptomCategory> GetAllSymptom();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<AdmissionCriterion> AdmissionCriterion_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void AdmissionCriterion_Save(AdmissionCriterion Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<AdmissionCriterionAttachICD> GetICDListByAdmissionCriterionID(long AdmissionCriterionID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertAdmissionCriterionAttachICD(List<AdmissionCriterionAttachICD> listAdmissionCriterionAttachICD, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteAdmissionCriterionAttachICD(long ACAI_ID, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<AdmissionCriterionAttachICD> GetAllAdmissionCriterionAttachICD();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<AdmissionCriterionAttachSymptom> GetSymptomListByAdmissionCriterionID(long AdmissionCriterionID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertAdmissionCriterionAttachSymptom(List<AdmissionCriterionAttachSymptom> listAdmissionCriterionAttachSymptom, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteAdmissionCriterionAttachSymptom(long ACAS_ID, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<GroupPCLs> GroupPCLs_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void GroupPCLs_Save(GroupPCLs Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<GroupPCLs> GetAllGroupPCLs();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<AdmissionCriterionAttachGroupPCL> GetGroupPCLListByAdmissionCriterionID(long AdmissionCriterionID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool InsertAdmissionCriterionAttachGroupPCL(List<AdmissionCriterionAttachGroupPCL> listAdmissionCriterionAttachGroupPCL, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteAdmissionCriterionAttachGroupPCL(long ACAG_ID, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamType> PCLExamType_ByGroupPCLID(long GroupPCLID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLExamTypeGroupPCL_XMLInsert(long GroupPCLID, IEnumerable<PCLExamType> ObjList, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<GroupPCLs> GroupPCL_PCLExamType_ByAdmissionCriterionID(long AdmissionCriterionID, long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        AdmissionCriterionDetail GetAdmissionCriterionDetailByPtRegistrationID(long PtRegistrationID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveAdmissionCriterionDetail(AdmissionCriterionDetail CurrentAdmissionCriterionDetail);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveAdmissionCriterionDetail_PCLResult(AdmissionCriterionDetail_PCLResult CurrentAdmissionCriterionDetail_PCLResult);
        #endregion
        #region BedCategory
        //list paging
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<BedCategory> BedCategory_Paging(
        BedCategorySearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
          );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<BedCategory> BedCategory_ByDeptLocID_Paging(
        BedCategorySearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
          );
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool BedCategory_InsertXML(IList<BedCategory> lstBedCategory);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool BedCategory_DeleteXML(IList<BedCategory> lstBedCategory);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CheckValidBedCategory(long BedCategoryID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void BedCategory_Save(BedCategory Obj, out string Result);

        #endregion

        #region DeptLocation

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetAllDeptLocLaboratory();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetDeptLocationFunc(long V_DeptType, long V_RoomFunction);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetDeptLocationFuncExt(long V_DeptType, long V_RoomFunction, long V_DeptTypeOperation);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Location> DeptLocation_ByDeptID(Int64 DeptID, Int64 RmTypeID, string LocationName);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetAllDeptLocationByDeptID(long DeptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RoomType> DeptLocation_GetRoomTypeByDeptID(Int64 DeptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeptLocation_CheckLIDExists(Int64 DeptID, Int64 LID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeptLocation_XMLInsert(Int64 DeptID, IEnumerable<Location> objCollect);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DeptLocation_MarkDeleted(Int64 DeptLocationID, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> DeptLocation_ByMedicalServiceTypeIDDeptID(Int64 MedicalServiceTypeID, Int64 DeptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> ListDeptLocation_ByPCLExamTypeID(long PCLExamTypeID);


        #endregion

        #region DeptLocMedServices
        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //List<RefMedicalServiceItem> DeptMedServiceItems_GetListMedServiceIDByDeptID_AllForChoose(
        //     Int64 DeptID,
        //     Int64 MedicalServiceTypeID,
        //     string MedServiceName);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedicalServiceType> DeptMedServiceItems_GroupMedSerTypeID_AllForChoose(
             Int64 DeptID,
             Int64 MedicalServiceTypeID,
             string MedServiceName);

        //Ở DB
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedicalServiceItem> DeptLocMedServices_GetListMedServiceIDByDeptID_HasChoose(
             Int64 DeptID,
            Int64 DeptLocationID,
             Int64 MedicalServiceTypeID,
             string MedServiceName);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefMedicalServiceType> DeptLocMedServices_GroupMedSerTypeID_HasChoose(
             Int64 DeptID,
            Int64 DeptLocationID,
             Int64 MedicalServiceTypeID,
             string MedServiceName);
        //Ở DB

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeptLocMedServices_XMLInsert(Int64 DeptLocationID, IEnumerable<RefMedicalServiceItem> objCollect);

        #endregion

        #region PCLExamTypePrices
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamTypePrice> PCLExamTypePrices_ByPCLExamTypeID_Paging(
            PCLExamTypePriceSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLExamTypePrices_Save(PCLExamTypePrice Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLExamTypePrices_MarkDelete(Int64 PCLExamTypePriceID, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        PCLExamTypePrice PCLExamTypePrices_ByPCLExamTypePriceID(Int64 PCLExamTypePriceID);


        #endregion

        #region PCLGroups
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLGroup> PCLGroups_GetAll(long? V_PCLCategory);


        //list paging
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLGroup> PCLGroups_GetList_Paging(
 PCLGroupsSearchCriteria SearchCriteria,

   int PageIndex,
   int PageSize,
   string OrderBy,
   bool CountTotal,
   out int Total
          );


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLGroups_Save(PCLGroup Obj, out string Result);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLGroups_MarkDelete(Int64 PCLGroupID, out string Result);

        #endregion

        #region MedServiceItemPriceList
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void MedServiceItemPriceList_AddNew(MedServiceItemPriceList Obj, IEnumerable<MedServiceItemPrice> ObjMedServiceItemPrice, out string Result_PriceList);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<MedServiceItemPriceList> MedServiceItemPriceList_GetList_Paging(
        MedServiceItemPriceListSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total, out DateTime CurDate
             );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void MedServiceItemPriceList_MarkDelete(Int64 MedServiceItemPriceListID, out string Result);


        //MedServiceItemPriceList_Detail
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<MedServiceItemPrice> MedServiceItemPriceList_Detail(
            DeptMedServiceItemsSearchCriteria SearchCriteria,
              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void MedServiceItemPriceList_Update(MedServiceItemPriceList Obj, IEnumerable<MedServiceItemPrice> ObjCollection, IEnumerable<MedServiceItemPrice> ObjCollection_Insert, IEnumerable<MedServiceItemPrice> ObjCollection_Update, out string Result_PriceList);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void MedServiceItemPriceList_CheckCanAddNew(Int64 DeptID, Int64 MedicalServiceTypeID, out bool CanAddNew);

        #endregion


        #region PCLExamTypePriceList

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLExamTypePriceList_CheckCanAddNew(out bool CanAddNew);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLExamTypePriceList_AddNew(PCLExamTypePriceList Obj, IEnumerable<PCLExamType> ObjPCLExamType, out string Result_PriceList);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamTypePriceList> PCLExamTypePriceList_GetList_Paging(
        PCLExamTypePriceListSearchCriteria SearchCriteria,

        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        out int Total, out DateTime curDate
        );


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLExamTypePriceList_Delete(Int64 PCLExamTypePriceListID, out string Result);


        //PCLExamTypePriceList_Detail
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamType> PCLExamTypePriceList_Detail(
             long PCLExamTypePriceListID,
              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        //Update
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLExamTypePriceList_Update(PCLExamTypePriceList Obj, IEnumerable<PCLExamType> ObjCollection_Update, out string Result_PriceList);




        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetAllLocationsByDeptID(long? deptID, long? V_RoomFunction = null);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DeptLocation> GetAllLocationsByDeptIDOld(long? deptID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamTypeLocation> GetPCLExamTypeLocations(List<PCLExamType> allPCLExamTypes);

        #region PCLExamTestItems
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamTypeTestItems> PCLExamTestItems_ByPCLExamTypeID(Int64 PCLExamTypeID);
        #endregion

        #region PCLResultParamImplementations
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLResultParamImplementations> PCLResultParamImplementations_GetAll();

        /*==== #001 ====*/
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLResultParamImplementations> GetPCLResultParamByCatID(long? PCLExamTypeSubCategoryID);
        /*==== #001 ====*/
        #endregion


        #region PCLExamTypeSubCategory
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamTypeSubCategory> PCLExamTypeSubCategory_ByV_PCLMainCategory(Int64 V_PCLMainCategory);
        #endregion

        #region PCLExamTypeLocations
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamType> PCLExamTypeLocations_ByDeptLocationID(string PCLExamTypeName, Int64 DeptLocationID);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLExamTypeLocations_XMLInsert(Int64 DeptLocationID, IEnumerable<PCLExamType> ObjList);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLExamTypeLocations_MarkDeleted(Int64 PCLExamTypeID, Int64 DeptLocationID, out string Result);

        #endregion

        #region "HITransactionType"
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<HITransactionType> HITransactionType_GetListNoParentID();

        //▼===== 25072018 TTM
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<HITransactionType> HITransactionType_GetListNoParentID_New();
        //▲===== 25072018 TTM

        #endregion

        #region "RefMedicalServiceGroups"
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<RefMedicalServiceGroups> RefMedicalServiceGroups_GetAll();
        #endregion


        #region PCLExamTypeExamTestPrint
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamTypeExamTestPrint> PCLExamTypeExamTestPrint_GetList_Paging(
            PCLExamTypeExamTestPrintSearchCriteria SearchCriteria,

     int PageIndex,
     int PageSize,
     string OrderBy,
     bool CountTotal,
     out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamTypeExamTestPrint> PCLExamTypeExamTestPrintIndex_GetAll();


        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLExamTypeExamTestPrint_Save(IEnumerable<PCLExamTypeExamTestPrint> ObjList, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PCLExamTypeExamTestPrintIndex_Save(IEnumerable<PCLExamTypeExamTestPrint> ObjList, out string Result);

        #endregion


        #region MAPPCLExamTypeDeptLoc
        [OperationContract]
        [FaultContract(typeof(AxException))]
        Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc();
        #endregion

        //▼====: #002
        #region RefPurposeForAccountant
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefPurposeForAccountant> GetAllRefPurposeForAccountant();
        #endregion
        //▲====: #002
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ReCaching();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<MedServiceItemPrice> GetBedAllocationAll_ByDeptID(DeptMedServiceItemsSearchCriteria SearchCriteria);
        //▼====: #003
        #region FilterPrescriptionsHasHIPayTable
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<FilterPrescriptionsHasHIPay> GetFilterPrescriptionsHasHIPay();
        #endregion
        //▲====: #003

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PCLExamTestItems> PCLExamTestItem_ByPCLExamTypeID(long PCLExamTypeID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SavePCLExamTestItem(List<PCLExamTestItems> listDetail);

        #region Exemptions

        //[OperationContract]
        //[FaultContract(typeof(AxException))]
        //void Locations_InsertUpdate(
        //Location Obj, bool SaveToDB, out string Result);

        //Save Exemptions
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool Exemptions_InsertUpdate(PromoDiscountProgram objCollect, out string Result, out long NewID);
        //Save Exemptions

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ExemptionsMedServiceItems_InsertXML(IList<PromoDiscountItems> lstMedServiceItems);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ExemptionsMedServiceItems_DeleteXML(IList<PromoDiscountItems> lstMedServiceItems);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLExamTypeExemptions_XMLInsert(Int64 PromoDiscProgID, IEnumerable<PCLExamType> ObjList);
        //list paging
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PromoDiscountProgram> Exemptions_Paging(string SearchCriteria,int PageIndex,int PageSize,string OrderBy,bool CountTotal,out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PromoDiscountItems> GetExemptionsMedServiceItems_Paging(ExemptionsMedServiceItemsSearchCriteria SearchCriteria,int PageIndex, int PageSize, string OrderBy, bool CountTotal,out int Total );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamType> PCLExamTypeExemptions(string PCLExamTypeName, Int64 PromoDiscProgID);
        //list pagind

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void Exemptions_MarkDeleted(Int64 PromoDiscProgID, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PromoDiscountProgram> GetAllExemptions();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PromoDiscountItems> GetPromoDiscountItems_ByID(long PromoDiscProgID);
        #endregion

        #region DiseaseProgression
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DiseaseProgression> DiseaseProgression_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DiseaseProgression_Save(DiseaseProgression Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DiseaseProgression_MarkDelete(long DiseaseProgressionID, long StaffID, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<DiseaseProgressionDetails> DiseaseProgressionDetails_Paging(long DiseaseProgressionID, string SearchDiseaseProgressionDetails, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total );

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DiseaseProgressionDetails_Save(DiseaseProgressionDetails Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void DiseaseProgressionDetail_MarkDelete(long DiseaseProgressionDetailID, long StaffID, out string Result);
        #endregion

        #region Gói DVKT PackageTechnicalService
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PackageTechnicalService_InsertUpdate(PackageTechnicalService objCollect, long LoggedStaffID, out string Result, out long NewID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PackageTechnicalServiceMedServiceItems_InsertXML(IList<PackageTechnicalServiceDetail> lstMedServiceItems);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PackageTechnicalServiceMedServiceItems_DeleteXML(IList<PackageTechnicalServiceDetail> lstMedServiceItems);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool PCLExamTypePackageTechnicalService_XMLInsert(long PackageTechnicalServiceID, IEnumerable<PCLExamType> ObjList);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PackageTechnicalService> PackageTechnicalService_Paging(string SearchCriteria, int PageIndex, int PageSize
            , string OrderBy, bool CountTotal, out int Total);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PackageTechnicalServiceDetail> GetPackageTechnicalServiceMedServiceItems_Paging(ExemptionsMedServiceItemsSearchCriteria SearchCriteria
            , int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<PCLExamType> PCLExamTypePackageTechnicalService(string PCLExamTypeName, long PackageTechnicalServiceID);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void PackageTechnicalService_MarkDeleted(long PackageTechnicalServiceID, out string Result);
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PackageTechnicalService> GetAllPackageTechnicalServices();

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PackageTechnicalService> PackageTechnicalService_Search(GeneralSearchCriteria SearchCriteria);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PackageTechnicalServiceDetail> PackageTechnicalServiceDetail_ByID(long PackageTechnicalServiceID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<PackageTechnicalServiceDetail> PackageTechnicalServiceDetail_All(long? PCLExamTypePriceListID);
        #endregion

        #region Từ điển viết tắt
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<ShortHandDictionary> ShortHandDictionary_Paging(string SearchValue, long StaffID, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void ShortHandDictionary_Save(ShortHandDictionary Obj, out string Result);

        #endregion

        #region Cấu hình mã nhóm bệnh
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutpatientTreatmentType> GetOutpatientTreatmentType_Paging(string SearchCode, string SearchName, bool IsDelete, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void OutPatientTreatmentType_Save(OutpatientTreatmentType Obj, out string Result);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutpatientTreatmentType> OutpatientTreatmentType_GetAll();

        //▼==== #009
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutpatientTreatmentTypeICD10Link> OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(long OutpatientTreatmentTypeID, string ICD10, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);
        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutpatientTreatmentTypeICD10Link_Edit(OutpatientTreatmentTypeICD10Link Obj);


        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<OutpatientTreatmentTypeICD10Link> OutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID(long OutpatientTreatmentTypeID);
        //▲==== #009

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutpatientTreatmentTypeICD10Link_XMLInsert(ObservableCollection<OutpatientTreatmentTypeICD10Link> objCollect);
        #endregion

        #region Cấu hình RefApplicationConfig
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<RefApplicationConfig> RefApplicationConfig_Paging(string SearchRefApplicationConfig, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void RefApplicationConfig_Save(RefApplicationConfig Obj, out string Result);
        #endregion

        //▼==== #006
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<Specimen> GetAllSpecimen();
        //▲==== #006
        
        //▼==== #008
        [OperationContract]
        [FaultContract(typeof(AxException))]
        List<List<string>> ExportExcelConfigurationManager(ConfigurationReportParams Params);
        //▲==== #008

        //▼==== #007
        [OperationContract]
        [FaultContract(typeof(AxException))]
        void Lookup_Save(Lookup Obj, long StaffID, out string Result);
        //▲==== #007
    }
}