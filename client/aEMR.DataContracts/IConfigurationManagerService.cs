/*
 * 20170512 #001 CMN: Added New Service get PCL Room type CatID
 * 20190603 #002 TNHX: [BM0011782] Add new service getAllPurposeForAccountant
 * 20190912 #003 TTM:   BM 0014330: Sửa cách thức lấy dữ liệu giường cho màn hình quản lý giường bệnh.
 * 20190926 #004 TNHX:  BM: Add server get List FilterPrecriptionMaxHIPay
 * 20220620 #005 DatTB: Thêm filter danh mục xét nghiệm
 * 20230509 #006 DatTB: IssueID: 3254 | Thêm nút xuất excel cho các danh mục/cấu hình
 * 20230518 #007 DatTB: Thêm service cho mẫu bệnh phẩm
 * 20230531 #008 QTD:   Thêm lưu danh mục
 * 20230601 #009 DatTB: IssueID: 3254 | Chỉnh sửa/Gộp các function xuất excel danh mục/cấu hình (Bỏ Func cũ)
 * 20230622 #010 DatTB:
 * + Thêm filter theo mã ICD đã lưu của nhóm bệnh
 * + Thêm function chỉnh sửa ICD của nhóm bệnh
 * + Thay đổi điều kiện gàng buộc ICD
*/
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using aEMR.DataContracts;
using DataEntities;
using System.Collections.ObjectModel;
using DataEntities.MedicalInstruction;

namespace ConfigurationManagerServiceProxy
{
    [ServiceContract]
    public interface IConfigurationManagerService
    {
        #region RefDepartments

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefDepartments_TreeFunction(string strV_DeptType, bool ShowDeptLocation, long RoomFunction, AsyncCallback callback, object state);
        List<RefDepartmentsTree> EndGetRefDepartments_TreeFunction(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefDepartments_TreeSegment(string strV_DeptType, bool ShowDeptLocation, long RoomFunction, AsyncCallback callback, object state);
        List<RefDepartmentsTree> EndGetRefDepartments_TreeSegment(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefDepartments_Tree(string strV_DeptType, bool ShowDeptLocation, string V_DeptTypeOperation, AsyncCallback callback, object state);
        List<RefDepartmentsTree> EndRefDepartments_Tree(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefDepartments_Tree_ByDeptID(string strV_DeptType, bool ShowDeptLocation, long DeptID, long DeptLocID, AsyncCallback callback, object state);
        List<RefDepartmentsTree> EndRefDepartments_Tree_ByDeptID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefDepartment_SubtractAllChild_ByDeptID(Int64 DeptID, AsyncCallback callback, object state);
        List<RefDepartments> EndRefDepartment_SubtractAllChild_ByDeptID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefDepartments_RecursiveByDeptID(Int64 DeptID, AsyncCallback callback, object state);
        List<RefDepartments> EndRefDepartments_RecursiveByDeptID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSearchTreeView(RefDepartmentsSearchCriteria SearchCriteria, AsyncCallback callback, object state);
        List<RefDepartmentsTree> EndGetSearchTreeView(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetRefDepartmentsByID(Int64 DeptID, AsyncCallback callback, object state);
        RefDepartments EndGetRefDepartmentsByID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefDepartments_ByParDeptID(Int64 ParDeptID, AsyncCallback callback, object state);
        List<RefDepartments> EndRefDepartments_ByParDeptID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddNewRefDepartments(RefDepartments obj, AsyncCallback callback, object state);
        bool EndAddNewRefDepartments(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateRefDepartments(RefDepartments obj, AsyncCallback callback, object state);
        void EndUpdateRefDepartments(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteRefDepartments(Int64 DeptID, AsyncCallback callback, object state);
        bool EndDeleteRefDepartments(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginTreeRefDepartment_InTable_DeptMedServiceItems(AsyncCallback callback, object state);
        List<RefDepartmentsTree> EndTreeRefDepartment_InTable_DeptMedServiceItems(IAsyncResult asyncResult);


        //#region "Tree Khoa trong Bảng DeptMedServiceItems là PCL"
        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginDeptMedServiceItems_TreeDeptHasMedserviceIsPCL(AsyncCallback callback, object state);
        //List<RefDepartmentsTree> EndDeptMedServiceItems_TreeDeptHasMedserviceIsPCL(IAsyncResult asyncResult);
        //#endregion



        #endregion

        #region RefMedicalServiceItems
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceItemsByMedicalServiceTypeID(Int64 MedicalServiceTypeID, AsyncCallback callback, object state);
        List<RefMedicalServiceItem> EndRefMedicalServiceItemsByMedicalServiceTypeID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(RefMedicalServiceItemsSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             AsyncCallback callback, object state);
        List<RefMedicalServiceItem> EndRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(out int Total, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceItems_ByMedicalServiceTypeID_Paging(RefMedicalServiceItemsSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             AsyncCallback callback, object state);
        List<RefMedicalServiceItem> EndRefMedicalServiceItems_ByMedicalServiceTypeID_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceItems_IsPCLByMedServiceID_Paging(RefMedicalServiceItemsSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             AsyncCallback callback, object state);

        List<RefMedicalServiceItem> EndRefMedicalServiceItems_IsPCLByMedServiceID_Paging(out int Total, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceItems_IsPCL_Save(RefMedicalServiceItem Obj, AsyncCallback callback, object state);
        void EndRefMedicalServiceItems_IsPCL_Save(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceItems_EditInfo(RefMedicalServiceItem Obj, AsyncCallback callback, object state);
        void EndRefMedicalServiceItems_EditInfo(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceItems_MarkDeleted(Int64 MedServiceID, AsyncCallback callback, object state);
        void EndRefMedicalServiceItems_MarkDeleted(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceItems_NotPCL_Add(DeptMedServiceItems Obj, AsyncCallback callback, object state);
        void EndRefMedicalServiceItems_NotPCL_Add(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceItems_NotPCL_Insert(RefMedicalServiceItem Obj, long StaffID, AsyncCallback callback, object state);
        void EndRefMedicalServiceItems_NotPCL_Insert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceItems_NotPCL_Update(RefMedicalServiceItem Obj, AsyncCallback callback, object state);
        bool EndRefMedicalServiceItems_NotPCL_Update(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginRefMedicalServiceItems_IsAllowRegistrationExam_ByDeptID_Paging(RefMedicalServiceItemsSearchCriteria SearchCriteria,

        //     int PageIndex,
        //     int PageSize,
        //     string OrderBy,
        //     bool CountTotal,
        //     AsyncCallback callback, object state);

        //List<RefMedicalServiceItem> EndRefMedicalServiceItems_IsAllowRegistrationExam_ByDeptID_Paging(out int Total, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceItems_In_DeptLocMedServices(
        RefMedicalServiceItemsSearchCriteria SearchCriteria,
            AsyncCallback callback, object state);

        List<RefMedicalServiceItem> EndRefMedicalServiceItems_In_DeptLocMedServices(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetMedServiceItems_Paging(RefMedicalServiceItemsSearchCriteria Criteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            AsyncCallback callback, object state);
        List<RefMedicalServiceItem> EndGetMedServiceItems_Paging(out int Total, IAsyncResult asyncResult);



        #endregion

        #region PCLExamGroup
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLExamGroup_ByMedServiceID_NoPaging(Int64 MedServiceID, AsyncCallback callback, object state);
        IList<PCLExamGroup> EndGetPCLExamGroup_ByMedServiceID_NoPaging(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamGroup_GetListDeptID(AsyncCallback callback, object state);
        IList<RefDepartments> EndPCLExamGroup_GetListDeptID(IAsyncResult asyncResult);
        #endregion

        #region PCLExamTypes
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLExamTypes_Paging(PCLExamTypeSearchCriteria SearchCriteria,
            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            AsyncCallback callback, object state);

        List<PCLExamType> EndGetPCLExamTypes_Paging(out int Total, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypesByDeptLocationID_LAB(long DeptLocationID, AsyncCallback callback, object state);
        List<PCLExamType> EndPCLExamTypesByDeptLocationID_LAB(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypesByDeptLocationID_NotLAB(long V_PCLMainCategory, long PCLExamTypeSubCategoryID, long DeptLocationID, AsyncCallback callback, object state);
        List<PCLExamType> EndPCLExamTypesByDeptLocationID_NotLAB(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypesAndPriceIsActive_Paging(PCLExamTypeSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            AsyncCallback callback, object state);
        List<PCLExamType> EndPCLExamTypesAndPriceIsActive_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypes_Save_NotIsLab(PCLExamType Obj, bool IsInsert, long StaffID, AsyncCallback callback, object state);
        void EndPCLExamTypes_Save_NotIsLab(out string Result, out long PCLExamTypeID_New, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypes_Save_IsLab(PCLExamType Obj, bool IsInsert, bool TestItemIsExamType, string PCLExamTestItemUnitForPCLExamType, string PCLExamTestItemRefScaleForPCLExamType,
            long StaffID,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Insert,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Update,
            IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Delete, AsyncCallback callback, object state);
        void EndPCLExamTypes_Save_IsLab(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypes_MarkDelete(Int64 PCLExamTypeID, AsyncCallback callback, object state);
        void EndPCLExamTypes_MarkDelete(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypes_List_Paging(PCLExamTypeSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            AsyncCallback callback, object state);
        List<PCLExamType> EndPCLExamTypes_List_Paging(out int Total, IAsyncResult asyncResult);


        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginPCLExamTypes_GroupV_PCLSubCategory(Int64 V_PCLMainCategory, AsyncCallback callback, object state);
        //List<Lookup> EndPCLExamTypes_GroupV_PCLSubCategory(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypes_NotYetPCLLabResultSectionID_Paging(PCLExamTypeSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            AsyncCallback callback, object state);
        List<PCLExamType> EndPCLExamTypes_NotYetPCLLabResultSectionID_Paging(out int Total, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypes_ByPCLLabResultSectionID(
            PCLExamTypeSearchCriteria SearchCriteria
            , AsyncCallback callback, object state);
        List<PCLExamType> EndPCLExamTypes_ByPCLLabResultSectionID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypes_Save_General(PCLExamType Obj, bool IsInsert, bool TestItemIsExamType, string PCLExamTestItemUnitForPCLExamType, string PCLExamTestItemRefScaleForPCLExamType,
           long StaffID,
           IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Insert,
           IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Update,
           IEnumerable<PCLExamTypeTestItems> DataPCLExamTestItems_Delete, AsyncCallback callback, object state);
        void EndPCLExamTypes_Save_General(out string Result, IAsyncResult asyncResult);

        #endregion

        #region PCLExamTypeCombo

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeCombo_Search(GeneralSearchCriteria SearchCriteria, AsyncCallback callback, object state);
        List<PCLExamTypeCombo> EndPCLExamTypeCombo_Search(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeComboItems_ByComboID(long ComboID, AsyncCallback callback, object state);
        List<PCLExamTypeComboItem> EndPCLExamTypeComboItems_ByComboID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeComboItems_All(long? PCLExamTypePriceListID, AsyncCallback callback, object state);
        List<PCLExamTypeComboItem> EndPCLExamTypeComboItems_All(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeCombo_Save(PCLExamTypeCombo item, List<PCLExamTypeComboItem> ComboXML_Insert, List<PCLExamTypeComboItem> ComboXML_Update, List<PCLExamTypeComboItem> ComboXML_Delete, AsyncCallback callback, object state);
        bool EndPCLExamTypeCombo_Save(out long ID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeCombo_Delete(long ID, AsyncCallback callback, object state);
        bool EndPCLExamTypeCombo_Delete(IAsyncResult asyncResult);

        #endregion

        #region PCLExamTypeMedServiceDefItem
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeMedServiceDefItems_ByMedServiceID(PCLExamTypeSearchCriteria SearchCriteria, Int64 MedServiceID, AsyncCallback callback, object state);
        List<PCLExamType> EndPCLExamTypeMedServiceDefItems_ByMedServiceID(IAsyncResult asyncResult);


        //Save danh sach vua chon tu UI
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeMedServiceDefItems_XMLInsert(Int64 MedServiceID, IEnumerable<PCLExamType> objCollect, AsyncCallback callback, object state);
        bool EndPCLExamTypeMedServiceDefItems_XMLInsert(IAsyncResult asyncResult);
        //Save danh sach vua chon tu UI

        #endregion

        #region DeptMedServiceItems
        //DeptMedServiceItems
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDeptMedServiceItems_Paging(DeptMedServiceItemsSearchCriteria SearchCriteria,
              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              AsyncCallback callback, object state);
        List<MedServiceItemPrice> EndGetDeptMedServiceItems_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDeptMedServiceItems_DeptIDPaging(DeptMedServiceItemsSearchCriteria SearchCriteria,
              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              AsyncCallback callback, object state);
        List<DeptMedServiceItems> EndGetDeptMedServiceItems_DeptIDPaging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetMedServiceItemPrice_Paging(MedServiceItemPriceSearchCriteria Criteria,
            int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);

        List<MedServiceItemPrice> EndGetMedServiceItemPrice_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeptMedServiceItems_TrueDelete(Int64 DeptMedServItemID,
            Int64 MedServItemPriceID,
            Int64 MedServiceID, AsyncCallback callback, object state);
        bool EndDeptMedServiceItems_TrueDelete(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeptMedServiceItems_InsertXML(IList<DeptMedServiceItems> lstDeptMedServiceItems, AsyncCallback callback, object state);
        bool EndDeptMedServiceItems_InsertXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeptMedServiceItems_DeleteXML(IList<DeptMedServiceItems> lstDeptMedServiceItems, AsyncCallback callback, object state);
        bool EndDeptMedServiceItems_DeleteXML(IAsyncResult asyncResult);

        //DeptMedServiceItems
        #endregion

        #region "RefMedicalServiceTypes"
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGroupRefMedicalServiceTypes_ByMedicalServiceTypeID(Int64 DeptID, string MedServiceName, AsyncCallback callback, object state);
        IList<RefMedicalServiceType> EndGroupRefMedicalServiceTypes_ByMedicalServiceTypeID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllMedicalServiceTypes(AsyncCallback callback, object state);
        IList<RefMedicalServiceType> EndGetAllMedicalServiceTypes(IAsyncResult asyncResult);

        /*@V:  0: all, 1: not PCL(Subtract PCL)*/
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(Int64 DeptID, int V, AsyncCallback callback, object state);
        IList<RefMedicalServiceType> EndRefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllMedicalServiceTypes_SubtractPCL(AsyncCallback callback, object state);
        IList<RefMedicalServiceType> EndGetAllMedicalServiceTypes_SubtractPCL(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceTypes_Paging(RefMedicalServiceTypeSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             AsyncCallback callback, object state);
        IList<RefMedicalServiceType> EndRefMedicalServiceTypes_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceTypes_CheckBeforeInsertUpdate(RefMedicalServiceType Obj, AsyncCallback callback, object state);
        void EndRefMedicalServiceTypes_CheckBeforeInsertUpdate(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceTypes_AddEdit(RefMedicalServiceType Obj, AsyncCallback callback, object state);
        void EndRefMedicalServiceTypes_AddEdit(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceTypes_MarkDelete(Int64 MedicalServiceTypeID, AsyncCallback callback, object state);
        bool EndRefMedicalServiceTypes_MarkDelete(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceTypes_ByV_RefMedicalServiceTypes(Int64 V_RefMedicalServiceTypes, AsyncCallback callback, object state);
        IList<RefMedicalServiceType> EndRefMedicalServiceTypes_ByV_RefMedicalServiceTypes(IAsyncResult asyncResult);


        #endregion

        #region "MedServiceItemPrice"
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMedServiceItemPriceByDeptMedServItemID_Paging(MedServiceItemPriceSearchCriteria SearchCriteria,

              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              AsyncCallback callback, object state);
        List<MedServiceItemPrice> EndMedServiceItemPriceByDeptMedServItemID_Paging(out int Total, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMedServiceItemPrice_Save(MedServiceItemPrice Obj, AsyncCallback callback, object state);
        void EndMedServiceItemPrice_Save(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMedServiceItemPrice_MarkDelete(Int64 MedServItemPriceID, AsyncCallback callback, object state);
        void EndMedServiceItemPrice_MarkDelete(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMedServiceItemPrice_ByMedServItemPriceID(Int64 MedServItemPriceID, AsyncCallback callback, object state);
        MedServiceItemPrice EndMedServiceItemPrice_ByMedServItemPriceID(IAsyncResult asyncResult);


        #endregion

        #region PCLItemID
        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginPCLItemsInsertUpdate(PCLItem Obj, bool SaveToDB, AsyncCallback callback, object state);
        //void EndPCLItemsInsertUpdate(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLItems_XMLInsert(Int64 PCLFormID, IEnumerable<PCLExamType> ObjPCLExamTypeList, AsyncCallback callback, object state);
        bool EndPCLItems_XMLInsert(IAsyncResult asyncResult);



        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginPCLItems_GetPCLExamTypeIDByMedServiceID(Int64 MedServiceID, AsyncCallback callback, object state);
        //List<PCLItem> EndPCLItems_GetPCLExamTypeIDByMedServiceID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLItems_GetPCLExamTypeIDByPCLSectionID(string PCLExamTypeName, Int64 PCLSectionID, AsyncCallback callback, object state);
        List<PCLExamType> EndPCLItems_GetPCLExamTypeIDByPCLSectionID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLItems_ByPCLFormID(PCLExamTypeSearchCriteria SearchCriteria, Int64 PCLFormID, long? PCLExamTypePriceListID, AsyncCallback callback, object state);
        List<PCLExamType> EndPCLItems_ByPCLFormID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamType_WithDeptLocIDs_GetAll(AsyncCallback callback, object state);
        List<PCLExamType> EndPCLExamType_WithDeptLocIDs_GetAll(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLItems_SearchAutoComplete(
           PCLExamTypeSearchCriteria SearchCriteria,

           int PageIndex,
           int PageSize,
           string OrderBy,
           bool CountTotal, AsyncCallback callback, object state);
        List<PCLExamType> EndPCLItems_SearchAutoComplete(out int Total, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLExamType_byComboID(long PCLExamTypeComboID, AsyncCallback callback, object state);
        List<PCLExamType> EndGetPCLExamType_byComboID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLExamType_byHosID(long HosID, AsyncCallback callback, object state);
        List<PCLExamType> EndGetPCLExamType_byHosID(IAsyncResult asyncResult);
        #endregion

        #region PCLForms

        //list paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLForms_GetList_Paging(PCLFormsSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          AsyncCallback callback, object state);
        List<PCLForm> EndPCLForms_GetList_Paging(out int Total, IAsyncResult asyncResult);


        //Save list
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLForms_Save(PCLForm Obj, AsyncCallback callback, object state);
        void EndPCLForms_Save(out string Result, IAsyncResult asyncResult);



        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLForms_MarkDelete(Int64 PCLFormID, AsyncCallback callback, object state);
        void EndPCLForms_MarkDelete(out string Result, IAsyncResult asyncResult);

        #endregion

        #region PCLSections

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLSections_All(AsyncCallback callback, object state);
        List<PCLSection> EndPCLSections_All(IAsyncResult asyncResult);

        //▼==== #005
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypes_All(AsyncCallback callback, object state);
        List<PCLExamType> EndPCLExamTypes_All(IAsyncResult asyncResult);
        //▲==== #005

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginPCLSectionsByPCLFormID(Int64 PCLFormID, AsyncCallback callback, object state);
        //List<PCLSection> EndPCLSectionsByPCLFormID(IAsyncResult asyncResult);


        //list paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLSections_GetList_Paging(PCLSectionsSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          AsyncCallback callback, object state);
        List<PCLSection> EndPCLSections_GetList_Paging(out int Total, IAsyncResult asyncResult);


        //Save list
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLSections_Save(PCLSection Obj, AsyncCallback callback, object state);
        void EndPCLSections_Save(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLSections_MarkDelete(Int64 PCLSectionID, AsyncCallback callback, object state);
        void EndPCLSections_MarkDelete(out string Result, IAsyncResult asyncResult);


        #endregion

        #region Locations
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginLocations_InsertUpdate(Location Obj, bool SaveToDB, AsyncCallback callback, object state);
        void EndLocations_InsertUpdate(out string Result, IAsyncResult asyncResult);

        //Save list
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginLocations_XMLInsert(Location objCollect, AsyncCallback callback, object state);
        bool EndLocations_XMLInsert(IAsyncResult asyncResult);
        //Save list

        //list paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginLocations_ByRmTypeID_Paging(
        LocationSearchCriteria SearchCriteria,

        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        AsyncCallback callback, object state);
        List<Location> EndLocations_ByRmTypeID_Paging(out int Total, IAsyncResult asyncResult);

        //list pagind

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginLocations_MarkDeleted(Int64 LID, AsyncCallback callback, object state);
        void EndLocations_MarkDeleted(out string Result, IAsyncResult asyncResult);

        #endregion
        
        #region RoomType
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRoomType_GetAll(AsyncCallback callback, object state);
        List<RoomType> EndRoomType_GetAll(IAsyncResult asyncResult);


        //list paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRoomType_GetList_Paging(RoomTypeSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          AsyncCallback callback, object state);
        List<RoomType> EndRoomType_GetList_Paging(out int Total, IAsyncResult asyncResult);
        //list paging




        //Save list
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRoomType_Save(RoomType Obj, AsyncCallback callback, object state);
        void EndRoomType_Save(out string Result, IAsyncResult asyncResult);
        //Save list


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRoomType_MarkDelete(Int64 RmTypeID, AsyncCallback callback, object state);
        void EndRoomType_MarkDelete(out string Result, IAsyncResult asyncResult);


        #endregion

        #region ICD
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginICD_GetAll(AsyncCallback callback, object state);
        List<ICD> EndICD_GetAll(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDiseaseChapters_GetAll(AsyncCallback callback, object state);
        List<DiseaseChapters> EndDiseaseChapters_GetAll(IAsyncResult asyncResult);

        //list paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginICD_ByIDCode_Paging(ICDSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<ICD> EndICD_ByIDCode_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginICD_ByDiseaseID_Paging(long Disease_ID , int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<ICD> EndICD_ByDiseaseID_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginChapter_Paging(string SearchChapter, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<DiseaseChapters> EndChapter_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDiseases_Paging(int DiseaseChapterID, string SearchDisease, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<Diseases> EndDiseases_Paging(out int Total, IAsyncResult asyncResult);
        //list paging
        
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDiseases_ByChapterID(int DiseaseChapterID, AsyncCallback callback, object state);
        List<Diseases> EndDiseases_ByChapterID(IAsyncResult asyncResult);

        //Save list
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginChapter_Save(DiseaseChapters Obj, AsyncCallback callback, object state);
        void EndChapter_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDiseases_Save(Diseases Obj, AsyncCallback callback, object state);
        void EndDiseases_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginICD_Save(ICD Obj, AsyncCallback callback, object state);
        void EndICD_Save(out string Result, IAsyncResult asyncResult);
        //Save list


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginICD_MarkDelete(Int64 IDCode, AsyncCallback callback, object state);
        void EndICD_MarkDelete(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginICD_XMLInsert(Int64 Disease_ID, ObservableCollection<ICD> objCollect, AsyncCallback callback, object state);
        bool EndICD_XMLInsert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSearchICD_Paging(long DiseaseChapterID, long Disease_ID, string ICD10Code, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<ICD> EndSearchICD_Paging(out int Total, IAsyncResult asyncResult);
        #endregion

        #region InsuranceBenefit



        //list paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsuranceBenefitPaging(string HIPCode,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          AsyncCallback callback, object state);
        List<InsuranceBenefitCategories_Data> EndInsuranceBenefitPaging(out int Total, IAsyncResult asyncResult);
        //list paging


        //Save list
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsuranceBenefitCategories_Save(InsuranceBenefitCategories_Data Obj, AsyncCallback callback, object state);
        void EndInsuranceBenefitCategories_Save(out string Result, IAsyncResult asyncResult);
        //Save list


        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginICD_MarkDelete(Int64 IDCode, AsyncCallback callback, object state);
        //void EndICD_MarkDelete(out string Result, IAsyncResult asyncResult);

        #endregion

        #region Hospital



        //list paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginHospitalPaging(HospitalSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          AsyncCallback callback, object state);
        List<Hospital> EndHospitalPaging(out int Total, IAsyncResult asyncResult);
        //list paging

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginHospital_MarkDelete(Int64 HosID, AsyncCallback callback, object state);
        void EndHospital_MarkDelete(out string Result, IAsyncResult asyncResult);


        #endregion

        #region CitiesProvinces
      

        //list paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCitiesProvince_Paging(string SearchCitiesProvinces,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          AsyncCallback callback, object state);
        List<CitiesProvince> EndCitiesProvince_Paging(out int Total, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSuburbNames_Paging(long CityProvinceID, string SearchSuburbNames,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          AsyncCallback callback, object state);
        List<SuburbNames> EndSuburbNames_Paging(out int Total, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginWardNames_Paging(long CityProvinceID, long SuburbNameID, string SearchWardNames,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          AsyncCallback callback, object state);
        List<WardNames> EndWardNames_Paging(out int Total, IAsyncResult asyncResult);
        //list paging

        //Save list
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCitiesProvinces_Save(CitiesProvince Obj, AsyncCallback callback, object state);
        void EndCitiesProvinces_Save(out string Result, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSuburbNames_Save(SuburbNames Obj, AsyncCallback callback, object state);
        void EndSuburbNames_Save(out string Result, IAsyncResult asyncResult);
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginWardNames_Save(WardNames Obj, AsyncCallback callback, object state);
        void EndWardNames_Save(out string Result, IAsyncResult asyncResult);
        //Save list

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginCitiesProvince_MarkDelete(Int64 CitiesProvinceID, AsyncCallback callback, object state);
        //void EndICD_MarkDelete(out string Result, IAsyncResult asyncResult);


        #endregion

        #region Job

        //list paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginJob_Paging(string SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          AsyncCallback callback, object state);
        List<Lookup> EndJob_Paging(out int Total, IAsyncResult asyncResult);
        //list paging


        //Save list
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginJob_Save(Lookup Obj, AsyncCallback callback, object state);
        void EndJob_Save(out string Result, IAsyncResult asyncResult);
        //Save list


        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginJob_MarkDelete(Int64 IDCode, AsyncCallback callback, object state);
        //void EndJob_MarkDelete(out string Result, IAsyncResult asyncResult);


        #endregion

        #region AdmissionCriteria

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAdmissionCriteria_Paging(string SearchCriteria,int PageIndex,int PageSize,string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<AdmissionCriteria> EndAdmissionCriteria_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAdmissionCriteria_Save(AdmissionCriteria Obj, AsyncCallback callback, object state);
        void EndAdmissionCriteria_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetListAdmissionCriteria(AsyncCallback callback, object state);
        List<AdmissionCriteria> EndGetListAdmissionCriteria(IAsyncResult asyncResult);

        #endregion

        #region AdmissionCreatition

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSymptomCategory_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<SymptomCategory> EndSymptomCategory_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSymptomCategory_Save(SymptomCategory Obj, AsyncCallback callback, object state);
        void EndSymptomCategory_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllSymptom(AsyncCallback callback, object state);
        List<SymptomCategory> EndGetAllSymptom(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAdmissionCriterion_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<AdmissionCriterion> EndAdmissionCriterion_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAdmissionCriterion_Save(AdmissionCriterion Obj, AsyncCallback callback, object state);
        void EndAdmissionCriterion_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetICDListByAdmissionCriterionID(long AdmissionCriterionID, AsyncCallback callback, object state);
        List<AdmissionCriterionAttachICD> EndGetICDListByAdmissionCriterionID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertAdmissionCriterionAttachICD(List<AdmissionCriterionAttachICD> listAdmissionCriterionAttachICD, long StaffID, AsyncCallback callback, object state);
        bool EndInsertAdmissionCriterionAttachICD(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteAdmissionCriterionAttachICD(long ACAI_ID, long StaffID, AsyncCallback callback, object state);
        bool EndDeleteAdmissionCriterionAttachICD(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllAdmissionCriterionAttachICD(AsyncCallback callback, object state);
        List<AdmissionCriterionAttachICD> EndGetAllAdmissionCriterionAttachICD(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetSymptomListByAdmissionCriterionID(long AdmissionCriterionID, AsyncCallback callback, object state);
        List<AdmissionCriterionAttachSymptom> EndGetSymptomListByAdmissionCriterionID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertAdmissionCriterionAttachSymptom(List<AdmissionCriterionAttachSymptom> listAdmissionCriterionAttachSymptom, long StaffID, AsyncCallback callback, object state);
        bool EndInsertAdmissionCriterionAttachSymptom(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteAdmissionCriterionAttachSymptom(long ACAS_ID, long StaffID, AsyncCallback callback, object state);
        bool EndDeleteAdmissionCriterionAttachSymptom(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGroupPCLs_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<GroupPCLs> EndGroupPCLs_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGroupPCLs_Save(GroupPCLs Obj, AsyncCallback callback, object state);
        void EndGroupPCLs_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllGroupPCLs(AsyncCallback callback, object state);
        List<GroupPCLs> EndGetAllGroupPCLs(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetGroupPCLListByAdmissionCriterionID(long AdmissionCriterionID, AsyncCallback callback, object state);
        List<AdmissionCriterionAttachGroupPCL> EndGetGroupPCLListByAdmissionCriterionID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInsertAdmissionCriterionAttachGroupPCL(List<AdmissionCriterionAttachGroupPCL> listAdmissionCriterionAttachGroupPCL, long StaffID, AsyncCallback callback, object state);
        bool EndInsertAdmissionCriterionAttachGroupPCL(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteAdmissionCriterionAttachGroupPCL(long ACAG_ID, long StaffID, AsyncCallback callback, object state);
        bool EndDeleteAdmissionCriterionAttachGroupPCL(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamType_ByGroupPCLID(long GroupPCLID, AsyncCallback callback, object state);
        IList<PCLExamType> EndPCLExamType_ByGroupPCLID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeGroupPCL_XMLInsert(long GroupPCLID, IEnumerable<PCLExamType> ObjList,long StaffID , AsyncCallback callback, object state);
        bool EndPCLExamTypeGroupPCL_XMLInsert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGroupPCL_PCLExamType_ByAdmissionCriterionID(long AdmissionCriterionID, long PtRegistrationID, AsyncCallback callback, object state);
        IList<GroupPCLs> EndGroupPCL_PCLExamType_ByAdmissionCriterionID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAdmissionCriterionDetailByPtRegistrationID(long PtRegistrationID, AsyncCallback callback, object state);
        AdmissionCriterionDetail EndGetAdmissionCriterionDetailByPtRegistrationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveAdmissionCriterionDetail(AdmissionCriterionDetail CurrentAdmissionCriterionDetail, AsyncCallback callback, object state);
        bool EndSaveAdmissionCriterionDetail(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveAdmissionCriterionDetail_PCLResult(AdmissionCriterionDetail_PCLResult CurrentAdmissionCriterionDetail_PCLResult, 
            AsyncCallback callback, object state);
        bool EndSaveAdmissionCriterionDetail_PCLResult(IAsyncResult asyncResult);
        #endregion

        #region BedCategory

        //list paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginBedCategory_Paging(BedCategorySearchCriteria SearchCriteria,int PageIndex,int PageSize,string OrderBy,bool CountTotal,AsyncCallback callback, object state);
        List<BedCategory> EndBedCategory_Paging(out int Total, IAsyncResult asyncResult);
        //list paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginBedCategory_ByDeptLocID_Paging(BedCategorySearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<BedCategory> EndBedCategory_ByDeptLocID_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginBedCategory_InsertXML(IList<BedCategory> lstBedCategory, AsyncCallback callback, object state);
        bool EndBedCategory_InsertXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginBedCategory_DeleteXML(IList<BedCategory> lstBedCategory, AsyncCallback callback, object state);
        bool EndBedCategory_DeleteXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCheckValidBedCategory(long BedCategoryID, AsyncCallback callback, object state);
        bool EndCheckValidBedCategory(IAsyncResult asyncResult);

        //Save list
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginBedCategory_Save(BedCategory Obj, AsyncCallback callback, object state);
        void EndBedCategory_Save(out string Result, IAsyncResult asyncResult);
        //Save list

        #endregion
        #region DeptLocation

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllDeptLocationByDeptID(Int64 DeptID, AsyncCallback callback, object state);
        List<DeptLocation> EndGetAllDeptLocationByDeptID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDeptLocationFunc(long V_DeptType, long V_RoomFunction, AsyncCallback callback, object state);
        List<DeptLocation> EndGetDeptLocationFunc(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetDeptLocationFuncExt(long V_DeptType, long V_RoomFunction, long V_DeptTypeOperation, AsyncCallback callback, object state);
        List<DeptLocation> EndGetDeptLocationFuncExt(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllDeptLocLaboratory(AsyncCallback callback, object state);
        List<DeptLocation> EndGetAllDeptLocLaboratory(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeptLocation_ByDeptID(Int64 DeptID, Int64 RmTypeID, string LocationName, AsyncCallback callback, object state);
        List<Location> EndDeptLocation_ByDeptID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeptLocation_GetRoomTypeByDeptID(Int64 DeptID, AsyncCallback callback, object state);
        List<RoomType> EndDeptLocation_GetRoomTypeByDeptID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeptLocation_CheckLIDExists(Int64 DeptID, Int64 LID, AsyncCallback callback, object state);
        bool EndDeptLocation_CheckLIDExists(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeptLocation_XMLInsert(Int64 DeptID, ObservableCollection<Location> objCollect, AsyncCallback callback, object state);
        bool EndDeptLocation_XMLInsert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeptLocation_MarkDeleted(Int64 DeptLocationID, AsyncCallback callback, object state);
        void EndDeptLocation_MarkDeleted(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeptLocation_ByMedicalServiceTypeIDDeptID(Int64 MedicalServiceTypeID, Int64 DeptID, AsyncCallback callback, object state);
        List<DeptLocation> EndDeptLocation_ByMedicalServiceTypeIDDeptID(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginListDeptLocation_ByPCLExamTypeID(long PCLExamTypeID, AsyncCallback callback, object state);
        List<DeptLocation> EndListDeptLocation_ByPCLExamTypeID(IAsyncResult asyncResult);

        #endregion

        #region DeptLocMedServices
        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginDeptMedServiceItems_GetListMedServiceIDByDeptID_AllForChoose(Int64 DeptID,
        //     Int64 MedicalServiceTypeID,
        //     string MedServiceName, AsyncCallback callback, object state);
        //List<RefMedicalServiceItem> EndDeptMedServiceItems_GetListMedServiceIDByDeptID_AllForChoose(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeptMedServiceItems_GroupMedSerTypeID_AllForChoose(Int64 DeptID,
             Int64 MedicalServiceTypeID,
             string MedServiceName, AsyncCallback callback, object state);
        List<RefMedicalServiceType> EndDeptMedServiceItems_GroupMedSerTypeID_AllForChoose(IAsyncResult asyncResult);

        //Ở DB
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeptLocMedServices_GetListMedServiceIDByDeptID_HasChoose(Int64 DeptID,
            Int64 DeptLocationID,
            Int64 MedicalServiceTypeID,
            string MedServiceName, AsyncCallback callback, object state);
        List<RefMedicalServiceItem> EndDeptLocMedServices_GetListMedServiceIDByDeptID_HasChoose(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeptLocMedServices_GroupMedSerTypeID_HasChoose(Int64 DeptID,
            Int64 DeptLocationID,
            Int64 MedicalServiceTypeID,
            string MedServiceName, AsyncCallback callback, object state);
        List<RefMedicalServiceType> EndDeptLocMedServices_GroupMedSerTypeID_HasChoose(IAsyncResult asyncResult);
        //Ở DB

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeptLocMedServices_XMLInsert(Int64 DeptLocationID, ObservableCollection<RefMedicalServiceItem> objCollect, AsyncCallback callback, object state);
        bool EndDeptLocMedServices_XMLInsert(IAsyncResult asyncResult);
        #endregion

        #region PCLExamTypePrices
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypePrices_ByPCLExamTypeID_Paging(PCLExamTypePriceSearchCriteria SearchCriteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            AsyncCallback callback, object state);
        List<PCLExamTypePrice> EndPCLExamTypePrices_ByPCLExamTypeID_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypePrices_Save(PCLExamTypePrice Obj, AsyncCallback callback, object state);
        void EndPCLExamTypePrices_Save(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypePrices_MarkDelete(Int64 PCLExamTypePriceID, AsyncCallback callback, object state);
        void EndPCLExamTypePrices_MarkDelete(out string Result, IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypePrices_ByPCLExamTypePriceID(Int64 PCLExamTypePriceID, AsyncCallback callback, object state);
        PCLExamTypePrice EndPCLExamTypePrices_ByPCLExamTypePriceID(IAsyncResult asyncResult);

        #endregion

        #region PCLGroups
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLGroups_GetAll(long? V_PCLCategory, AsyncCallback callback, object state);
        IList<PCLGroup> EndPCLGroups_GetAll(IAsyncResult asyncResult);


        //list paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLGroups_GetList_Paging(PCLGroupsSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          AsyncCallback callback, object state);
        List<PCLGroup> EndPCLGroups_GetList_Paging(out int Total, IAsyncResult asyncResult);


        //Save list
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLGroups_Save(PCLGroup Obj, AsyncCallback callback, object state);
        void EndPCLGroups_Save(out string Result, IAsyncResult asyncResult);



        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLGroups_MarkDelete(Int64 PCLGroupID, AsyncCallback callback, object state);
        void EndPCLGroups_MarkDelete(out string Result, IAsyncResult asyncResult);


        #endregion

        #region MedServiceItemPriceList
        //Save
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMedServiceItemPriceList_AddNew(MedServiceItemPriceList Obj, IEnumerable<MedServiceItemPrice> ObjMedServiceItemPrice, AsyncCallback callback, object state);
        void EndMedServiceItemPriceList_AddNew(out string Result_PriceList, IAsyncResult asyncResult);

        //Paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMedServiceItemPriceList_GetList_Paging(MedServiceItemPriceListSearchCriteria SearchCriteria,

        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        AsyncCallback callback, object state);
        List<MedServiceItemPriceList> EndMedServiceItemPriceList_GetList_Paging(out int Total, out DateTime curDate, IAsyncResult asyncResult);

        //MarkDelete
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMedServiceItemPriceList_MarkDelete(Int64 MedServiceItemPriceListID, AsyncCallback callback, object state);
        void EndMedServiceItemPriceList_MarkDelete(out string Result, IAsyncResult asyncResult);


        //MedServiceItemPriceList_Detail
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMedServiceItemPriceList_Detail(DeptMedServiceItemsSearchCriteria SearchCriteria,
              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              AsyncCallback callback, object state);
        List<MedServiceItemPrice> EndMedServiceItemPriceList_Detail(out int Total, IAsyncResult asyncResult);


        //Update
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMedServiceItemPriceList_Update(MedServiceItemPriceList Obj, IEnumerable<MedServiceItemPrice> ObjCollection, IEnumerable<MedServiceItemPrice> ObjCollection_Insert, IEnumerable<MedServiceItemPrice> ObjCollection_Update, AsyncCallback callback, object state);
        void EndMedServiceItemPriceList_Update(out string Result_PriceList, IAsyncResult asyncResult);


        //Check CanAddNew
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginMedServiceItemPriceList_CheckCanAddNew(Int64 DeptID, Int64 MedicalServiceTypeID, AsyncCallback callback, object state);
        void EndMedServiceItemPriceList_CheckCanAddNew(out bool CanAddNew, IAsyncResult asyncResult);

        #endregion

        #region PCLExamTypePriceList
        //Check CanAddNew
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypePriceList_CheckCanAddNew(AsyncCallback callback, object state);
        void EndPCLExamTypePriceList_CheckCanAddNew(out bool CanAddNew, IAsyncResult asyncResult);

        //Save AddNew
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypePriceList_AddNew(PCLExamTypePriceList Obj, IEnumerable<PCLExamType> ObjPCLExamType, AsyncCallback callback, object state);
        void EndPCLExamTypePriceList_AddNew(out string Result_PriceList, IAsyncResult asyncResult);

        //PriceList Paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypePriceList_GetList_Paging(PCLExamTypePriceListSearchCriteria SearchCriteria,

        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        AsyncCallback callback, object state);
        List<PCLExamTypePriceList> EndPCLExamTypePriceList_GetList_Paging(out int Total, out DateTime curDate, IAsyncResult asyncResult);

        //MarkDelete
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypePriceList_Delete(Int64 PCLExamTypePriceListID, AsyncCallback callback, object state);
        void EndPCLExamTypePriceList_Delete(out string Result, IAsyncResult asyncResult);


        //PCLExamTypePriceList_Detail
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypePriceList_Detail(long PCLExamTypePriceListID,
        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        AsyncCallback callback, object state);
        List<PCLExamType> EndPCLExamTypePriceList_Detail(out int Total, IAsyncResult asyncResult);

        //Update
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypePriceList_Update(PCLExamTypePriceList Obj, IEnumerable<PCLExamType> ObjCollection_Update, AsyncCallback callback, object state);
        void EndPCLExamTypePriceList_Update(out string Result_PriceList, IAsyncResult asyncResult);

        #endregion


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllLocationsByDeptIDOld(long? deptID, AsyncCallback callback, object state);
        List<DeptLocation> EndGetAllLocationsByDeptIDOld(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllLocationsByDeptID(long? deptID, long? V_RoomFunction, AsyncCallback callback, object state);
        List<DeptLocation> EndGetAllLocationsByDeptID(IAsyncResult asyncResult);

        #region PCLExamTestItems
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTestItems_ByPCLExamTypeID(Int64 PCLExamTypeID, AsyncCallback callback, object state);
        List<PCLExamTypeTestItems> EndPCLExamTestItems_ByPCLExamTypeID(IAsyncResult asyncResult);
        #endregion

        #region PCLResultParamImplementations
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLResultParamImplementations_GetAll(AsyncCallback callback, object state);
        List<PCLResultParamImplementations> EndPCLResultParamImplementations_GetAll(IAsyncResult asyncResult);

        /*==== #001 ====*/
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPCLResultParamByCatID(long? PCLExamTypeSubCategoryID, AsyncCallback callback, object state);
        ObservableCollection<PCLResultParamImplementations> EndGetPCLResultParamByCatID(IAsyncResult asyncResult);
        /*==== #001 ====*/
        #endregion


        #region PCLExamTypeSubCategory
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeSubCategory_ByV_PCLMainCategory(Int64 V_PCLMainCategory, AsyncCallback callback, object state);
        List<PCLExamTypeSubCategory> EndPCLExamTypeSubCategory_ByV_PCLMainCategory(IAsyncResult asyncResult);
        #endregion

        #region DeptLocation
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeLocations_ByDeptLocationID(string PCLExamTypeName, Int64 DeptLocationID, AsyncCallback callback, object state);
        IList<PCLExamType> EndPCLExamTypeLocations_ByDeptLocationID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeLocations_XMLInsert(Int64 DeptLocationID, IEnumerable<PCLExamType> ObjList, AsyncCallback callback, object state);
        bool EndPCLExamTypeLocations_XMLInsert(IAsyncResult asyncResult);


        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeLocations_MarkDeleted(Int64 PCLExamTypeID, Int64 DeptLocationID, AsyncCallback callback, object state);
        void EndPCLExamTypeLocations_MarkDeleted(out string Result, IAsyncResult asyncResult);

        #endregion

        #region "HITransactionType"
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginHITransactionType_GetListNoParentID(AsyncCallback callback, object state);
        IList<HITransactionType> EndHITransactionType_GetListNoParentID(IAsyncResult asyncResult);
        #endregion

        #region "RefMedicalServiceGroups"
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefMedicalServiceGroups_GetAll(AsyncCallback callback, object state);
        IList<RefMedicalServiceGroups> EndRefMedicalServiceGroups_GetAll(IAsyncResult asyncResult);
        #endregion


        #region PCLExamTypeExamTestPrint
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeExamTestPrint_GetList_Paging(
          PCLExamTypeExamTestPrintSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          AsyncCallback callback, object state);
        IList<PCLExamTypeExamTestPrint> EndPCLExamTypeExamTestPrint_GetList_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeExamTestPrintIndex_GetAll(AsyncCallback callback, object state);
        IList<PCLExamTypeExamTestPrint> EndPCLExamTypeExamTestPrintIndex_GetAll(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeExamTestPrint_Save(ObservableCollection<PCLExamTypeExamTestPrint> ObjList, AsyncCallback callback, object state);
        void EndPCLExamTypeExamTestPrint_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeExamTestPrintIndex_Save(ObservableCollection<PCLExamTypeExamTestPrint> ObjList, AsyncCallback callback, object state);
        void EndPCLExamTypeExamTestPrintIndex_Save(out string Result, IAsyncResult asyncResult);


        #endregion
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetPCLExamTypeLocations(List<PCLExamType> allPCLExamTypes, AsyncCallback callback, object state);
        List<PCLExamTypeLocation> EndGetPCLExamTypeLocations(IAsyncResult asyncResult);


        #region MAPPCLExamTypeDeptLoc
        //[FaultContractAttribute(typeof(AxException))]
        //IAsyncResult BeginMAPPCLExamTypeDeptLoc(AsyncCallback callback, object state);
        //Dictionary<long, PCLExamType> EndMAPPCLExamTypeDeptLoc(IAsyncResult asyncResult);
        #endregion

        #region "Xáp nhập nhà thuốc - khoa dược"
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginHITransactionType_GetListNoParentID_New(AsyncCallback callback, object state);
        IList<HITransactionType> EndHITransactionType_GetListNoParentID_New(IAsyncResult asyncResult);
        #endregion

        //▼====: #002
        #region RefPurposeForAccountant
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllRefPurposeForAccountant(AsyncCallback callback, object state);
        List<RefPurposeForAccountant> EndGetAllRefPurposeForAccountant(IAsyncResult asyncResult);
        #endregion
        //▲====: #002

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginReCaching(AsyncCallback callback, object state);
        bool EndReCaching(IAsyncResult asyncResult);

        //▼=====: #003
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetBedAllocationAll_ByDeptID(DeptMedServiceItemsSearchCriteria SearchCriteria,AsyncCallback callback, object state);
        List<MedServiceItemPrice> EndGetBedAllocationAll_ByDeptID(IAsyncResult asyncResult);
        //▲=====: #003
        //▼====: #004
        #region FilterPrescriptionsHasHIPay
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetFilterPrescriptionsHasHIPay(AsyncCallback callback, object state);
        List<FilterPrescriptionsHasHIPay> EndGetFilterPrescriptionsHasHIPay(IAsyncResult asyncResult);
        #endregion
        //▲====: #004

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTestItem_ByPCLExamTypeID(long PCLExamTypeID, AsyncCallback callback, object state);
        List<PCLExamTestItems> EndPCLExamTestItem_ByPCLExamTypeID(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSavePCLExamTestItem(List<PCLExamTestItems> listDetail, AsyncCallback callback, object state);
        bool EndSavePCLExamTestItem(IAsyncResult asyncResult);

        #region Exemptions
       
        //Save Exemption
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExemptions_InsertUpdate(PromoDiscountProgram objCollect, AsyncCallback callback, object state);
        bool EndExemptions_InsertUpdate(out string Result, out long NewID, IAsyncResult asyncResult);
        //Save Exemption

        //Save List MedService
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExemptionsMedServiceItems_InsertXML(IList<PromoDiscountItems> lstMedServiceItems, AsyncCallback callback, object state);
        bool EndExemptionsMedServiceItems_InsertXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExemptionsMedServiceItems_DeleteXML(IList<PromoDiscountItems> lstMedServiceItems, AsyncCallback callback, object state);
        bool EndExemptionsMedServiceItems_DeleteXML(IAsyncResult asyncResult);
        //Save List MedService

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeExemptions_XMLInsert(Int64 PromoDiscProgID, IEnumerable<PCLExamType> ObjList, AsyncCallback callback, object state);
        bool EndPCLExamTypeExemptions_XMLInsert(IAsyncResult asyncResult);

        //list paging
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExemptions_Paging(string SearchCriteria,
                                            int PageIndex,
                                            int PageSize,
                                            string OrderBy,
                                            bool CountTotal,
                                            AsyncCallback callback, object state);
        List<PromoDiscountProgram> EndExemptions_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetExemptionsMedServiceItems_Paging(ExemptionsMedServiceItemsSearchCriteria SearchCriteria,
        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        AsyncCallback callback, object state);
        List<PromoDiscountItems> EndGetExemptionsMedServiceItems_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypeExemptions(string PCLExamTypeName, Int64 PromoDiscProgID, AsyncCallback callback, object state);
        IList<PCLExamType> EndPCLExamTypeExemptions(IAsyncResult asyncResult);
        //list paging

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExemptions_MarkDeleted(Int64 PromoDiscProgID, AsyncCallback callback, object state);
        void EndExemptions_MarkDeleted(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllExemptions(AsyncCallback callback, object state);
        List<PromoDiscountProgram> EndGetAllExemptions(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPromoDiscountItems_ByID(long PromoDiscProgID, AsyncCallback callback, object state);
        List<PromoDiscountItems> EndGetPromoDiscountItems_ByID(IAsyncResult asyncResult);
        #endregion

        #region DiseaseProgression
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDiseaseProgression_Paging(string SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<DiseaseProgression> EndDiseaseProgression_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDiseaseProgression_Save(DiseaseProgression Obj, AsyncCallback callback, object state);
        void EndDiseaseProgression_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDiseaseProgression_MarkDelete(long DiseaseProgressionID, long StaffID, AsyncCallback callback, object state);
        void EndDiseaseProgression_MarkDelete(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDiseaseProgressionDetails_Paging(long DiseaseProgressionID, string SearchDiseaseProgressionDetails, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<DiseaseProgressionDetails> EndDiseaseProgressionDetails_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDiseaseProgressionDetails_Save(DiseaseProgressionDetails Obj, AsyncCallback callback, object state);
        void EndDiseaseProgressionDetails_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDiseaseProgressionDetail_MarkDelete(long DiseaseProgressionDetailID, long StaffID, AsyncCallback callback, object state);
        void EndDiseaseProgressionDetail_MarkDelete(out string Result, IAsyncResult asyncResult);
        #endregion

        #region Cấu hình gói DVKT
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPackageTechnicalService_Search(GeneralSearchCriteria SearchCriteria, AsyncCallback callback, object state);
        List<PackageTechnicalService> EndPackageTechnicalService_Search(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPackageTechnicalServiceDetail_ByID(long PackageTechnicalServiceID, AsyncCallback callback, object state);
        List<PackageTechnicalServiceDetail> EndPackageTechnicalServiceDetail_ByID(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPackageTechnicalServiceDetail_All(long? PCLExamTypePriceListID, AsyncCallback callback, object state);
        List<PackageTechnicalServiceDetail> EndPackageTechnicalServiceDetail_All(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPackageTechnicalService_InsertUpdate(PackageTechnicalService objCollect, long LoggedStaffID, AsyncCallback callback, object state);
        bool EndPackageTechnicalService_InsertUpdate(out string Result, out long NewID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPackageTechnicalServiceMedServiceItems_InsertXML(IList<PackageTechnicalServiceDetail> lstMedServiceItems, AsyncCallback callback, object state);
        bool EndPackageTechnicalServiceMedServiceItems_InsertXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPackageTechnicalServiceMedServiceItems_DeleteXML(IList<PackageTechnicalServiceDetail> lstMedServiceItems, AsyncCallback callback, object state);
        bool EndPackageTechnicalServiceMedServiceItems_DeleteXML(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypePackageTechnicalService_XMLInsert(long PackageTechnicalServiceID, IEnumerable<PCLExamType> ObjList, AsyncCallback callback, object state);
        bool EndPCLExamTypePackageTechnicalService_XMLInsert(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPackageTechnicalService_Paging(string SearchCriteria,
                                            int PageIndex,
                                            int PageSize,
                                            string OrderBy,
                                            bool CountTotal,
                                            AsyncCallback callback, object state);
        List<PackageTechnicalService> EndPackageTechnicalService_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetPackageTechnicalServiceMedServiceItems_Paging(ExemptionsMedServiceItemsSearchCriteria SearchCriteria,
        int PageIndex,
        int PageSize,
        string OrderBy,
        bool CountTotal,
        AsyncCallback callback, object state);
        List<PackageTechnicalServiceDetail> EndGetPackageTechnicalServiceMedServiceItems_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPCLExamTypePackageTechnicalService(string PCLExamTypeName, long PackageTechnicalServiceID, AsyncCallback callback, object state);
        IList<PCLExamType> EndPCLExamTypePackageTechnicalService(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginPackageTechnicalService_MarkDeleted(long PackageTechnicalServiceID, AsyncCallback callback, object state);
        void EndPackageTechnicalService_MarkDeleted(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetAllPackageTechnicalServices(AsyncCallback callback, object state);
        List<PackageTechnicalService> EndGetAllPackageTechnicalServices(IAsyncResult asyncResult);

        //[OperationContract(AsyncPattern = true)]
        //IAsyncResult BeginGetPromoDiscountItems_ByID(long PromoDiscProgID, AsyncCallback callback, object state);
        //List<PromoDiscountItems> EndGetPromoDiscountItems_ByID(IAsyncResult asyncResult);
        #endregion

        #region Cấu hình viết tắt
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginShortHandDictionary_Paging(string SearchValue,long StaffID, int PageIndex, int PageSize, string OrderBy, bool CountTotal, AsyncCallback callback, object state);
        List<ShortHandDictionary> EndShortHandDictionary_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginShortHandDictionary_Save(ShortHandDictionary Obj, AsyncCallback callback, object state);
        void EndShortHandDictionary_Save(out string Result, IAsyncResult asyncResult);
        #endregion

        #region Cấu hình mã nhóm bệnh
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginGetOutpatientTreatmentType_Paging(string SearchCode, string SearchName, bool IsDelete, int PageIndex, int PageSize, string OrderBy, bool CountTotal, 
            AsyncCallback callback, object state);
        List<OutpatientTreatmentType> EndGetOutpatientTreatmentType_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutPatientTreatmentType_Save(OutpatientTreatmentType Obj, AsyncCallback callback, object state);
        void EndOutPatientTreatmentType_Save(out string Result, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutpatientTreatmentType_GetAll(AsyncCallback callback, object state);
        List<OutpatientTreatmentType> EndOutpatientTreatmentType_GetAll(IAsyncResult asyncResult);

        //▼==== #010
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(long OutpatientTreatmentTypeID, string ICD10, int PageIndex, int PageSize, string OrderBy, bool CountTotal,
            AsyncCallback callback, object state);
        List<OutpatientTreatmentTypeICD10Link> EndOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutpatientTreatmentTypeICD10Link_Edit(OutpatientTreatmentTypeICD10Link Obj, AsyncCallback callback, object state);
        bool EndOutpatientTreatmentTypeICD10Link_Edit(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID(long OutpatientTreatmentTypeID, AsyncCallback callback, object state);
        List<OutpatientTreatmentTypeICD10Link> EndOutpatientTreatmentTypeICD10Link_ByOutpatientTreatmentTypeID(IAsyncResult asyncResult);
        //▲==== #010

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutpatientTreatmentTypeICD10Link_XMLInsert(ObservableCollection<OutpatientTreatmentTypeICD10Link> objCollect, AsyncCallback callback, object state);
        bool EndOutpatientTreatmentTypeICD10Link_XMLInsert(IAsyncResult asyncResult);
        #endregion

        #region Cấu hình RefApplicationConfig
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefApplicationConfig_Paging(string SearchRefApplicationConfig, int PageIndex, int PageSize, string OrderBy
            , bool CountTotal, AsyncCallback callback, object state);
        List<RefApplicationConfig> EndRefApplicationConfig_Paging(out int Total, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginRefApplicationConfig_Save(RefApplicationConfig Obj, AsyncCallback callback, object state);
        void EndRefApplicationConfig_Save(out string Result, IAsyncResult asyncResult);
        #endregion

        //▼==== #007
        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginGetAllSpecimen(AsyncCallback callback, object state);
        List<Specimen> EndGetAllSpecimen(IAsyncResult asyncResult);
        //▲==== #007

        //▼==== #008
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginLookup_Save(Lookup Obj, long StaffID, AsyncCallback callback, object state);
        void EndLookup_Save(out string Result, IAsyncResult asyncResult);
        //▲==== #008

        //▼==== #009
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginExportExcelConfigurationManager(ConfigurationReportParams Params, AsyncCallback callback, object state);
        List<List<string>> EndExportExcelConfigurationManager(IAsyncResult asyncResult);
        //▲==== #009
    }
}