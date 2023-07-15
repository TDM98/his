/*
 * 20170803 #001 CMN: Added HI Store Service
 * 20170810 #002 CMN: Added Bid Service
 * 20170821 #003 CMN: Added AdjustClinicPrice Service
 * 20180801 #004 TTM: Add RefGenMedProductDetails_Save_New
 * 20181124 #005 TTM: BM 0005309: Tạo mới hàm lấy dữ liệu cho cbb và dữ liệu phiếu xuất cho màn hình nhập trả 
 *                    từ kho BHYT - Nhà thuốc => Khoa dược
 * 20180412 #006 TTM: BM 0005324: 
 * 20181219 #007 TTM: BM 0005443: Tạo mới hàm phục vụ cho việc lập, tìm kiếm và duyệt phiếu lĩnh hàng kho BHYT - nhà thuốc.
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
using System.Data.Common;
using System.Collections.ObjectModel;
using System.Collections;

namespace eHCMS.DAL
{
    public abstract class RefDrugGenericDetailsProvider : DataProviderBase
    {
        static private RefDrugGenericDetailsProvider _instance = null;
        static public RefDrugGenericDetailsProvider Instance
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
                    string assemblyPath = System.IO.Path.Combine(tempPath, Globals.Settings.Pharmacies.Assembly + ".dll");
                    Assembly assem = Assembly.LoadFrom(assemblyPath);
                    Type[] types = assem.GetExportedTypes();
                    Type t = assem.GetType(Globals.Settings.Pharmacies.ProviderType);
                    _instance = (RefDrugGenericDetailsProvider)Activator.CreateInstance(t);
                    //_instance = (PatientProvider)Activator.CreateInstance(Type.GetType(Globals.Settings.Patients.ProviderType));
                }
                return _instance;
            }
        }

        #region 1. Drug member
        public abstract OutwardDrugInvoice GetOutWardDrugInvoiceByID(long? OutiID);
        //==== #001
        public abstract OutwardDrugInvoice GetHIOutWardDrugInvoiceByID(long? OutiID);
        //==== #001
        public abstract RefGenericDrugDetail GetRefDrugGenericDetailsByID(long drugID);
        public abstract bool DeleteRefDrugGenericDetailByID(long drugID);
        //▼===== 25072018 TTM
        public abstract bool DeleteRefDrugGenericDetailByID_New(long drugID);
        //▲===== 25072018 TTM
        public abstract bool UpdateRefDrugGenericDetail(RefGenericDrugDetail drug);
        //▼===== 25072018 TTM
        public abstract bool UpdateRefDrugGenericDetail_New(RefGenericDrugDetail drug);
        //▲===== 25072018 TTM
        
        public abstract bool InsertRefDrugGenericDetail(RefGenericDrugDetail drug, out long Total);
        //▼===== 25072018 TTM
        public abstract bool InsertRefDrugGenericDetail_New(RefGenericDrugDetail drug, out long Total);
        //▲===== 25072018 TTM
        public abstract bool DeleteConIndicatorDrugsRelToMedCond(long DrugsMCTypeID);
        public abstract List<RefGenericDrugDetail> GetAllRefDrugGenericDetails();

        public abstract List<RefGenericDrugDetail> SearchRefDrugGenericDetails(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract List<RefGenericDrugDetail> SearchRefDrugGenericDetails_Simple(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        //▼===== 25072018 TTM
        public abstract List<RefGenericDrugDetail> SearchRefDrugGenericDetails_Simple_New(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        //▲===== 25072018 TTM

        public abstract List<RefGenericDrugDetail> SearchRefDrugGenericDetails_ItemPrice(DrugSearchCriteria criteria,
                                                                           int pageIndex,
                                                                           int pageSize, bool bCountTotal,
                                                                           out int totalCount);

        public abstract List<RefGenericDrugDetail> SearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string BrandName, long? SupplierID, int pageIndex, int pageSize, out int totalCount);
        
        public abstract List<RefGenericDrugSimple> SearchRefGenericDrugName_SimpleAutoPaging(bool? IsCode, string BrandName,
                                                                                       int pageIndex, int pageSize,
                                                                                     out int totalCount);
        public abstract List<RefGenericDrugDetail> SearchRefDrugGenericDetails_RefAutoPaging(bool? IsCode, string BrandName, int pageIndex, int pageSize, out int totalCount);
        public abstract List<RefGenericDrugDetail> GetRefDrugGenericDetails(int IsMedDept, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public RefDrugGenericDetailsProvider()
        {
            this.ConnectionString = Globals.Settings.Patients.ConnectionString;

        }

        #endregion

        #region 5. unit member
        #region 5.1 unit member
        public abstract List<RefUnit> GetUnits();
        public abstract List<RefUnit> GetPagingUnits(int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract List<RefUnit> SearchUnit(string UnitName, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract bool DeleteUnitByID(long unitID);
        public abstract bool UpdateUnit(RefUnit unit);
        public abstract bool AddNewUnit(RefUnit newunit);
        public abstract RefUnit GetUnitByID(long UnitID);
        #endregion

        #region 5.2 unit member
        public abstract List<RefUnit> GetDrugDeptUnits();
        // -----------DPT 03/10/2017 hoạt động chỉ đạo tuyến  và hoạt động ngiên cứu khoa học
        //public abstract List<RefGeneralUnits> LoadRefGeneralUnits();
        public abstract List<ScientificResearchActivities> GetScientificResearchActivityList(ScientificResearchActivities Activity);
        public abstract bool InsertUpdateTrainingForSubOrg(bool ISAdd, TrainingForSubOrg objTraining, out int Result);
        public abstract List<TrainingForSubOrg> GetTrainingForSubOrgList(TrainingForSubOrg Training);
        public abstract bool DeleteTrainingForSubOrg(long TrainingID, out int Result);
        public abstract List<Lookup> GetTrainningTypeList();
        public abstract bool InsertUpdateScientificResearchActivity(bool ISAdd, ScientificResearchActivities objActivity, out int Result);
        public abstract bool DeleteScientificResearchActivity(long ActivityID, out int Result);
        public abstract List<ActivityClasses> ActivityClassListAll(long ClassID);
        // DPT 03/10/2017 hoạt động chỉ đạo tuyến  và hoạt động ngiên cứu khoa học-------------------------------
        public abstract List<RefUnit> GetPagingDrugDeptUnits(int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract List<RefUnit> SearchDrugDeptUnit(string UnitName, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract bool DeleteDrugDeptUnitByID(long unitID);
        public abstract bool UpdateDrugDeptUnit(RefUnit unit);
        public abstract bool AddNewDrugDeptUnit(RefUnit newunit);
        public abstract RefUnit GetDrugDeptUnitByID(long UnitID);
        #endregion

        #endregion

        #region 6. familytherapy member
        #region 6.1 DrugClass member

        public abstract List<DrugClass> GetFamilyTherapies(long V_MedProductType);
        public abstract DrugClass GetFamilyTherapyByID(long ID);
        public abstract bool AddNewFamilyTherapy(DrugClass newfamily, out string StrError);
        public abstract bool UpdateFamilyTherapy(DrugClass updatefamily, out string StrError);
        public abstract bool DeleteFamilyTherapy(long deletefamily, out string StrError);
        public abstract List<DrugClass> GetFamilyTherapyParent(long V_MedProductType);
        public abstract List<DrugClass> GetSearchFamilyTherapies(string faname, long V_MedProductType);
        #endregion

        #region 6.2 DrugDeptClass member

        public abstract List<DrugClass> GetDrugDeptClasses(long V_MedProductType);
        public abstract DrugClass GetDrugDeptClassesByID(long ID);
        public abstract bool AddNewDrugDeptClasses(DrugClass newfamily, out string StrError);
        public abstract bool UpdateDrugDeptClasses(DrugClass updatefamily, out string StrError);
        public abstract bool DeleteDrugDeptClasses(long deletefamily, out string StrError);
        public abstract List<DrugClass> GetDrugDeptClassesParent(long V_MedProductType);
        public abstract List<DrugClass> GetSearchDrugDeptClasses(string faname, long V_MedProductType);
        public abstract List<DrugClass> GetAllRefGeneric();
        #endregion
        #endregion

        #region 7. Supplier member
        public abstract Supplier GetSupplierByID(long supplierID);
        public abstract bool DeleteSupplierByID(long supplierID);
        public abstract bool UpdateSupplier(Supplier supplier, out string StrError);
        public abstract bool AddNewSupplier(Supplier supplier, out long SupplierID, out string StrError);
        public abstract List<Supplier> GetAllSupplierCbx(int supplierType);

        #endregion

        #region 8. Storage member
        public abstract List<RefStorageWarehouseType> GetRefStorageWarehouseType_All();
        public abstract List<RefDepartment> GetRefDepartment_ByDeptType(long DeptType);
        public abstract RefStorageWarehouseLocation GetStorageByID(long storageID);
        public abstract int DeleteStorageByID(long storageID);
        public abstract int UpdateStorage(RefStorageWarehouseLocation storage);
        public abstract int AddNewStorage(RefStorageWarehouseLocation storage);
        public abstract List<RefStorageWarehouseLocation> GetAllStorages(int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract List<RefStorageWarehouseLocation> GetAllStorages(long? type, bool? bNo, long? DeptID, bool? IsNotSubStorage);

        public abstract List<RefStorageWarehouseLocation> GetAllStorages_ForRespon(List<long> allListStoreID, long? type,
                                                                           bool? bNo);
        public abstract List<RefStorageWarehouseLocation> SearchStorage(string StorageName, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract List<RefStorageWarehouseLocation> GetStoragesByStaffID(long StaffID, long? StoreTypeID);
        #endregion

        #region 9. InwardDrug member
        public abstract List<InwardDrugInvoice> GetAllInwardDrugInvoice(int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract List<InwardDrugInvoice> SearchInwardDrugInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract InwardDrugInvoice GetInwardDrugInvoiceByID(long invoiceID);

        public abstract bool InwardDrugInvoice_Save(InwardDrugInvoice InvoiceDrug, out long inwardid, out string StrError);

        public abstract int AddInwardDrugInvoice(InwardDrugInvoice InvoiceDrug, out long inwardid);
        public abstract int UpdateInwardDrugInvoice(InwardDrugInvoice InvoiceDrug);
        public abstract int DeleteInwardDrugInvoice(long ID);

        public abstract List<InwardDrug> GetspInwardDrugDetailsByID(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            ,out decimal TongTienSPChuaVAT
            ,out decimal CKTrenSP
            ,out decimal TongTienTrenSPDaTruCK
            ,out decimal TongCKTrenHoaDon
            ,out decimal TongTienHoaDonCoThueNK
            ,out decimal TongTienHoaDonCoVAT
            ,out decimal TotalVATDifferenceAmount);

        public abstract List<InwardDrug> InwardDrugDetails_ByID(long inviID
                                                                                    , out decimal TongTienSPChuaVAT
                                                                                  , out decimal CKTrenSP
                                                                                  , out decimal TongTienTrenSPDaTruCK
                                                                                  , out decimal TongCKTrenHoaDon
                                                                                  , out decimal TongTienHoaDonCoThueNK
                                                                                  , out decimal TongTienHoaDonCoVAT);
        public abstract bool InwardDrug_Insert(PharmacyPurchaseOrderDetail invoicedrug, long inviID);

        public abstract int InwardDrug_Update(InwardDrug invoicedrug, long StaffID);
        public abstract bool AddInwardDrug(InwardDrug invoicedrug);
        public abstract int DeleteInwardDrug(long invoicedrug);

        public abstract List<PharmacyPurchaseOrder> PharmacyPurchaseOrder_BySupplierID(long SupplierID);

        public abstract RefGenericDrugDetail GetRefGenericDrugDetail(long drugID, long? drugVersionID);

        #endregion

        #region 10. Pharmaceutical member
        public abstract List<PharmaceuticalCompany> GetPharmaceuticalCompanyCbx();
        public abstract int PharmaceuticalCompany_Insert(PharmaceuticalCompany Pharmaceatical);
        public abstract int PharmaceuticalCompany_Update(PharmaceuticalCompany Pharmaceatical);
        public abstract bool PharmaceuticalCompany_Delete(long PCOID);
        public abstract List<PharmaceuticalCompany> PharmaceuticalCompany_SearchPaging(string PCOName, int PageSize, int PageIndex, out int TotalCount);

        public abstract List<Supplier> GetSupplier_NotPCOID(long? PCOID, long V_SupplierType, int PageSize, int PageIndex, out int TotalCount);
        public abstract List<Supplier> GetSupplier_ByPCOID(long? PCOID, long V_SupplierType, int PageSize, int PageIndex, out int TotalCount);
        public abstract List<Supplier> GetSupplier_ByPCOIDNotPaging(long? PCOID, long V_SupplierType);

        public abstract int PharmaceuticalSuppliers_Insert(PharmaceuticalSupplier S);
        public abstract bool PharmaceuticalSuppliers_Delete(PharmaceuticalSupplier S);

        #endregion

        #region  10.1 DrugDept Pharmaceutical member

        public abstract List<DrugDeptPharmaceuticalCompany> GetDrugDeptPharmaceuticalCompanyCbx();

        public abstract int DrugDeptPharmaceuticalCompany_Insert(DrugDeptPharmaceuticalCompany Pharmaceatical);

        public abstract int DrugDeptPharmaceuticalCompany_Update(DrugDeptPharmaceuticalCompany Pharmaceatical);

        public abstract bool DrugDeptPharmaceuticalCompany_Delete(long PCOID);

        public abstract List<DrugDeptPharmaceuticalCompany> DrugDeptPharmaceuticalCompany_SearchPaging(string PCOName, int PageSize, int PageIndex, out int TotalCount);

        public abstract List<DrugDeptSupplier> GetSupplierDrugDept_NotPCOID(string SearchText, long? PCOID, long V_SupplierType, int PageSize, int PageIndex, out int TotalCount);

        public abstract List<DrugDeptSupplier> GetSupplierDrugDept_ByPCOID(string SearchText, long? PCOID, long V_SupplierType, int PageSize, int PageIndex, out int TotalCount);

        public abstract List<DrugDeptSupplier> GetSupplierDrugDept_ByPCOIDNotPaging(long? PCOID, long V_SupplierType);

        public abstract int DrugDeptPharmaceuticalSuppliers_Insert(DrugDeptPharmaceuticalSupplier S);

        public abstract bool DrugDeptPharmaceuticalSuppliers_Delete(DrugDeptPharmaceuticalSupplier S);

        public abstract List<RefGenMedProductDetails> RefGenMedProductDetails_ByPCOID(long PCOID, long? V_MedProductType, int PageIndex, int PageSize, bool bcount, out int totalcount);
        #endregion

        #region 11. RefShelfDrugLocation member
        public abstract List<RefShelfDrugLocation> GetRefShelfDrugLocationAutoComplete(string Name, int pageIndex, int pageSize);
        public abstract bool InsertRefShelfDrugLocation(RefShelfDrugLocation Location, out long ID);

        #endregion

        #region 12. OutWardDrugInvoices Member
        public abstract bool AddOutWardDrugInvoiceReturnVisitor(OutwardDrugInvoice InvoiceDrug, long ReturnStaffID, out long outwardid);
        public abstract int OutWardDrugInvoiceVisitor_Cancel(OutwardDrugInvoice InvoiceDrug, out long TransItemID);
        public abstract int OutWardDrugInvoicePrescriptChuaThuTien_Cancel(OutwardDrugInvoice InvoiceDrug);

        public abstract int OutWardDrugInvoice_Delete(long id);
        public abstract int OutWardDrugInvoice_UpdateStatus(OutwardDrugInvoice InvoiceDrug);

        public abstract bool CountMoneyForVisitorPharmacy(long outiID, out decimal AmountPaid);

        public abstract bool AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug, out long PaymentID, long? StaffID, long? CollectorDeptLocID);
        public abstract bool AddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug, out long PaymentID,
                                                 long? StaffID, long? CollectorDeptLocID);

        public abstract bool AddTransactionMedDept(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug, out long PaymentID, long? StaffID);
        public abstract bool AddTransactionMedDeptHoanTien(PatientTransactionPayment payment, OutwardDrugMedDeptInvoice InvoiceDrug, out long PaymentID,
                                                 long? StaffID);

        public abstract OutwardDrugInvoice GetOutWardDrugInvoiceVisitorByID(long outwardid);
        public abstract List<OutwardDrugInvoice> GetOutWardDrugInvoiceSearchAllByStatus(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, bool? bFlagStoreHI, bool bFlagPaidTime, out int totalCount);
        public abstract List<OutwardDrugInvoice> GetOutWardDrugInvoiceSearchReturn(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract OutwardDrugInvoice GetOutWardDrugInvoiceReturnByID(long? OutiID);

        public abstract List<GetDrugForSellVisitor> GetDrugForSellVisitorAutoComplete_New(string BrandName, long StoreID, bool? IsCode);

        public abstract List<GetDrugForSellVisitor> RefGennericDrugDetails_GetRemaining_Paging(string BrandName, long StoreID,
                                                                                        bool? IsCode, int PageSize, int PageIndex, out int Total);

        public abstract List<GetDrugForSellVisitor> GetDrugForSellVisitorBatchNumber(long DrugID, long StoreID, bool? IsHIPatient);
        public abstract List<GetDrugForSellVisitor> GetDrugForSellVisitorBatchNumber_V2(long DrugID, long StoreID, bool? IsHIPatient, bool? InsuranceCover = null);

        public abstract List<OutwardDrug> GetOutwardDrugDetailsByOutwardInvoice(long OutiID, bool HI = false, long[] OutiIDArray = null, long? PtRegistrationID = null);
        //==== #001
        public abstract List<OutwardDrug> GetHIOutwardDrugDetailsByOutwardInvoice(long OutiID);
        //==== #001

        public abstract bool UpdateRemainingWhenCancel(OutwardDrugInvoice invoice);

        #endregion

        #region 13. Outward Drug member

        public abstract bool AddOutwardDrug(OutwardDrug Outward, long outwardid);

        public abstract bool InsertOrUpdateDrugInvoices(OutwardDrugInvoice Outward, out long outiID);
        public abstract bool InsertOrUpdateDrugInvoices(OutwardDrugInvoice Outward, out long outiID, DbConnection connection, DbTransaction tran);

        public abstract bool UpdateInvoicePayed(OutwardDrugInvoice Outward, out long outiID, out long PaymentID, out string StrError);

        public abstract bool UpdateInvoiceInfo(OutwardDrugInvoice Outward);

        public abstract bool UpdateOutwardDrugInvoicePrice(OutwardDrugInvoice invoice);
        public abstract bool UpdateOutwardDrugInvoicePrice(OutwardDrugInvoice invoice, DbConnection conn, DbTransaction tran);

        public abstract bool BackupOutwardDrugInvoice(long invoiceID, DbConnection conn, DbTransaction tran);
        public abstract bool BackupOutwardDrugInvoiceOfRegistration(long registrationID, DbConnection conn, DbTransaction tran);

        public abstract List<OutwardDrug> spGetInBatchNumberAndPrice_List(long StoreID);
        public abstract List<OutwardDrug> spGetInBatchNumberAndPrice_ByPresciption(long PrescriptID, long StoreID, Int16 IsObject, bool HI = false, bool IsIssueID = true, long? PtRegistrationID = null);
        //==== #001
        public abstract List<OutwardDrug> spGetInBatchNumberAndPrice_ByPresciptionHI(long PrescriptID, long StoreID, Int16 IsObject, bool IsIssueID = true);
        //==== #001

        public abstract List<OutwardDrug> spGetInBatchNumberAndPrice_ByPresciption_NotSave(Prescription ObjPrescription, long StoreID, Int16 RegistrationType);

        public abstract List<OutwardDrug> spGetOutwardDrugAll_ByPtRegistrationID(long PtRegistrationID);
        public abstract List<OutwardDrug> spGetOutwardDrugAll_ByPtRegistrationID(long PtRegistrationID, DbConnection connection, DbTransaction tran);


        #endregion

        #region 14. GetDrugForPrescription
        public abstract IList<GetDrugForSellVisitor> GetDrugForPrescription_Auto(String BrandName, int IsInsurance, int PageSize, int PageIndex, int IsMedDept);

        public abstract List<GetDrugForSellVisitor> GetDrugForSellVisitorAutoComplete_ForPrescription(byte HI, bool? IsHIPatient, string BrandName, long StoreID, long? PrescriptID, bool? IsCode);

        public abstract List<GetDrugForSellVisitor> GetDrugForSellVisitorAutoComplete_ForPrescriptionByID(byte HI, bool? IsHIPatient, long StoreID, long? PrescriptID);
        #endregion

        #region 15. Selling Price Fomular
        public abstract SellingPriceFormular GetSellingPriceFormularByID(long ItemID);
        public abstract SellingPriceFormular GetSellingPriceFormularIsActive();
        public abstract List<SellingPriceFormular> GetSellingPriceFormularAll(DateTime? FromDate, DateTime? Todate, int PageIndex, int PageSize, out int TotalCount);
        public abstract bool AddNewSellingPriceFormular(SellingPriceFormular p);
        public abstract bool UpdateSellingPriceFormular(SellingPriceFormular p);
        public abstract bool DeleteSellingPriceFormular(long ItemID);

        #endregion

        #region 16. Drug Contraindicator

        public abstract List<RefMedicalConditionType> GetRefMedicalConditionTypesAllPaging(
                                                int PageSize
                                                , int PageIndex
                                                , string OrderBy
                                                , bool CountTotal
                                                , out int Total);
        public abstract bool GetConIndicatorDrugsRelToMedCondAll(int MCTypeID, long DrugID);
        public abstract void InsertConIndicatorDrugsRelToMedCond(IList<RefMedicalConditionType> lstRefMedicalCondition, long DrugID);
        //▼===== 25072018 TTM
        public abstract void InsertConIndicatorDrugsRelToMedCond_New(IList<RefMedicalConditionType> lstRefMedicalCondition, long DrugID);
        //▲===== 25072018 TTM
        public abstract bool InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML(IList<ContraIndicatorDrugsRelToMedCond> lstInsert
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstDelete
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstUpdate);

        //▼===== 25072018 TTM
        public abstract bool InsertDeleteUpdateConIndicatorDrugsRelToMedCondXML_New(IList<ContraIndicatorDrugsRelToMedCond> lstInsert
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstDelete
                                                    , IList<ContraIndicatorDrugsRelToMedCond> lstUpdate);
        //▲===== 25072018 TTM

        public abstract void InsertConIndicatorDrugsRelToMedCondEx(IList<RefGenericDrugDetail> lstRefGenericDrugDetail, long MCTypeID);
        public abstract List<ContraIndicatorDrugsRelToMedCond> GetConIndicatorDrugsRelToMedCond(IList<long> lstMCTpe, long DrugID);
        protected virtual RefMedicalConditionType GetRefMedicalConditionTypesAllPagingObjFromReader(IDataReader reader)
        {
            RefMedicalConditionType p = new RefMedicalConditionType();
            try
            {
                p.MCTypeID = (long)Convert.ToInt32(reader["MCTypeID"]);
            }
            catch { }

            try
            {
                p.MedConditionType = reader["MedConditionType"].ToString();
            }
            catch { }

            return p;
        }
        protected virtual List<RefMedicalConditionType> GetRefMedicalConditionTypesAllPagingCollectionFromReader(IDataReader reader)
        {
            List<RefMedicalConditionType> lst = new List<RefMedicalConditionType>();
            while (reader.Read())
            {
                lst.Add(GetRefMedicalConditionTypesAllPagingObjFromReader(reader));
            }
            return lst;
        }
        public abstract List<ContraIndicatorDrugsRelToMedCond> GetContraIndicatorDrugsRelToMedCondList(int MCTypeID, long DrugID);
        //▼===== 25072018 TTM
        public abstract List<ContraIndicatorDrugsRelToMedCond> GetContraIndicatorDrugsRelToMedCondList_New(int MCTypeID, long DrugID);
        //▲===== 25072018 TTM
        public abstract List<ContraIndicatorDrugsRelToMedCond> GetAllContrainIndicatorDrugs();

        public abstract List<RefMedicalConditionType> GetRefMedCondType();

        public abstract List<ContraIndicatorDrugsRelToMedCond> GetContraIndicatorDrugsRelToMedCondPaging(
                                                long MCTypeID
                                                , int PageSize
                                                , int PageIndex
                                                , string OrderBy
                                                , bool CountTotal
                                                , out int Total);
        #endregion

        #region 16*. Medical Condition

        public abstract List<RefMedicalConditionType> GetRefMedicalConditionTypes();
        public abstract bool DeleteRefMedicalConditionTypes(int MCTypeID);
        public abstract bool InsertRefMedicalConditionTypes(string MedConditionType, int Idx);
        public abstract bool UpdateRefMedicalConditionTypes(int MCTypeID, string MedConditionType, int Idx);


        public abstract List<RefMedicalCondition> GetRefMedicalConditions(int MCTypeID);
        public abstract bool DeleteRefMedicalConditions(int MCID, int MCTypeID);
        public abstract bool InsertRefMedicalConditions(int MCTypeID, string MCDescription);
        public abstract bool UpdateRefMedicalConditions(int MCID, int MCTypeID, string MCDescription);

        #endregion

        #region ContraIndicatorMedProductsRelToMedCond member

        public abstract bool DeleteConIndicatorMedProductsRelToMedCond(long MedProductsMCTypeID);

        public abstract List<ContraIndicatorMedProductsRelToMedCond> GetContraIndicatorMedProductsRelToMedCondList(int MCTypeID,
                                                                                                 long GenMedProductID);

        public abstract bool GetConIndicatorMedProductsRelToMedCondAll(int MCTypeID, long GenMedProductID);

        public abstract void InsertConIndicatorMedProductsRelToMedCond(IList<RefMedicalConditionType> lstRefMedicalCondition,
                                                                long GenMedProductID);
        public abstract bool InsertDeleteUpdateConIndicatorMedProductsRelToMedCondXML(IList<ContraIndicatorMedProductsRelToMedCond> lstInsert
                                                            , IList<ContraIndicatorMedProductsRelToMedCond> lstDelete
                                                       , IList<ContraIndicatorMedProductsRelToMedCond> lstUpdate);

        public abstract void InsertConIndicatorMedProductsRelToMedCondEx(IList<RefGenMedProductDetails> lstRefGenericDrugDetail,
                                                                  long MCTypeID);
        public abstract List<ContraIndicatorMedProductsRelToMedCond> GetContraIndicatorMedProductsRelToMedCond(IList<long> lstMCTpe,
                                                                                              long GenMedProductID);
        #endregion

        #region 17. Inward Drug For Clinic Dept

        public abstract List<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        public abstract IList<InwardDrugClinicDeptInvoice> SearchInwardDrugClinicDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract InwardDrugClinicDeptInvoice GetInwardDrugClinicDeptInvoice_ByID(long ID);
        public abstract InwardDrugClinicDeptInvoice GetInwardDrugClinicDeptInvoice_ByID_V2(long ID, long? V_MedProductType);
        public abstract IList<InwardDrugClinicDept> GetInwardDrugClinicDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
        , out decimal TongTienSPChuaVAT
        , out decimal CKTrenSP
        , out decimal TongTienTrenSPDaTruCK
        , out decimal TongCKTrenHoaDon
        , out decimal TongTienHoaDonCoVAT);

        public abstract IList<InwardDrugClinicDept> GetInwardDrugClinicDept_ByIDInvoiceNotPaging(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoVAT);

        public abstract IList<InwardDrugClinicDept> GetInwardDrugClinicDept_ByIDInvoiceNotPaging_V2(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoVAT
            , long? V_MedProductType
            , bool IsMedDeptSubStorage);

        public abstract int AddInwardDrugClinicDeptInvoice(InwardDrugClinicDeptInvoice InvoiceDrug, out long inwardid);
        public abstract int DeleteInwardDrugClinicDept(long invoicedrug);
        public abstract int DeleteInwardDrugClinicDeptInvoice(long ID);
        public abstract int UpdateInwardDrugClinicDept(InwardDrugClinicDept invoicedrug);
        public abstract int UpdateInwardDrugClinicDeptInvoice(InwardDrugClinicDeptInvoice InvoiceDrug);
        public abstract List<RefGenMedProductDetails> spGetInBatchNumberAllDrugDept_Clinic_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);
        public abstract bool AddInwardDrugClinicDept(DrugDeptPurchaseOrderDetail invoicedrug, long inviID);
        public abstract int InwardDrugClinicDeptInvoice_SaveXML(InwardDrugClinicDeptInvoice InvoiceDrug, out long id);
        public abstract bool AcceptAutoUpdateInwardClinicInvoice(long inviID);

        #endregion

        #region 18. Inward Drug For Med Dept

        public abstract List<OutwardDrugClinicDeptInvoice> OutwardDrugClinicDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        public abstract IList<InwardDrugMedDeptInvoice> SearchInwardDrugMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract InwardDrugMedDeptInvoice GetInwardDrugMedDeptInvoice_ByID(long ID);
        public abstract InwardDrugMedDeptInvoice GetInwardDrugMedDeptInvoice_ByID_V2(long ID, long? V_MedProductType);
        public abstract int AddInwardDrugMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);
        public abstract int UpdateInwardDrugMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);
        public abstract int DeleteInwardDrugMedDeptInvoice(long ID);

        public abstract IList<InwardDrugMedDept> GetInwardDrugMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            ,out decimal TongTienSPChuaVAT
            ,out decimal CKTrenSP
            ,out decimal TongTienTrenSPDaTruCK
            ,out decimal TongCKTrenHoaDon
            ,out decimal TongTienHoaDonCoThueNK
            ,out decimal TongTienHoaDonCoVAT
            ,out decimal TotalVATDifferenceAmount);

        public abstract IList<InwardDrugMedDept> GetInwardDrugMedDept_ByIDInvoiceNotPaging(long inviID
             , out decimal TongTienSPChuaVAT
             , out decimal CKTrenSP
             , out decimal TongTienTrenSPDaTruCK
             , out decimal TongCKTrenHoaDon
             , out decimal TongTienHoaDonCoThueNK
             , out decimal TongTienHoaDonCoVAT);

        public abstract IList<InwardDrugMedDept> GetInwardDrugMedDept_ByIDInvoiceNotPaging_V2(long inviID
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , long? V_MedProductType);

        public abstract int UpdateInwardDrugMedDept(InwardDrugMedDept invoicedrug, long StaffID);
        public abstract int DeleteInwardDrugMedDept(long invoicedrug);

        public abstract bool DeleteInwardDrugMedDeptTemp(long InID);

        public abstract List<RefGenMedProductDetails> spGetInBatchNumberAllClinicDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);

        public abstract List<RefGenMedProductDetails> spGetInBatchNumberAllDrugDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);

        public abstract bool AddInwardDrugMedDept(DrugDeptPurchaseOrderDetail invoicedrug, long inviID);

        public abstract bool InwardDrugMedDeptInvoice_UpdateCost(long inviID);

        public abstract bool UpdateStatusDrugDeptPurchaseOrder(long DrugDeptPoID);

        public abstract int InwardDrugMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        public abstract RefGenMedProductDetails GetRefGenMedProductDetails(long genMedProductID, long? genMedVersionID);

        #endregion

        #region 19. Request Form Drug And DMedRscr

        public abstract bool SaveOutwardDrugClinicDeptTemplate(OutwardDrugClinicDeptTemplate OutwardTemplate, out long id);

        public abstract OutwardDrugClinicDeptTemplate GetOutwardDrugClinicDeptTemplate(long OutwardDrugClinicDeptTemplateID);

        public abstract List<OutwardDrugClinicDeptTemplate> GetAllOutwardTemplate(long V_MedProductType, long DeptID);

        public abstract bool DeleteOutwardDrugClinicDeptTemplate(long OutwardTemplateID);

        public abstract bool FullOperatorRequestDMedRscrInwardClinicDept(RequestDMedRscrInwardClinicDept Request, out long id);
        public abstract bool FullOperatorRequestDrugInwardClinicDept(RequestDrugInwardClinicDept Request, long V_MedProductType, out long id);

        public abstract bool FullOperatorRequestDrugInwardClinicDeptNew(RequestDrugInwardClinicDept Request,
                                                                 long V_MedProductType, out long id);

        public abstract RequestDrugInwardClinicDept GetRequestDrugInwardClinicDeptByID(long ReqDrugInClinicDeptID);

        public abstract bool RequestDrugInwardClinicDept_Approved(RequestDrugInwardClinicDept Request);

        public abstract List<RequestDrugInwardClinicDeptDetail> GetRequestDrugInwardClinicDeptDetailByID(long ReqDrugInClinicDeptID);

        public abstract List<RequestDrugInwardClinicDeptDetail> GetRequestDrugInwardClinicDeptDetailByRequestID(long ReqDrugInClinicDeptID);

        public abstract List<ReqOutwardDrugClinicDeptPatient> GetRemainingQtyForInPtRequestDrug(long? StoreID, long V_MedProductType, long RefGenDrugCatID_1);

        public abstract List<ReqOutwardDrugClinicDeptPatient> GetReqOutwardDrugClinicDeptPatientByID(long ReqDrugInClinicDeptID, bool bGetExistingReqToCreateNew);

        public abstract List<ReqOutwardDrugClinicDeptPatient> ReqOutwardDrugClinicFromInstruction(long DeptID, long V_MedProductType);

        public abstract List<ReqOutwardDrugClinicDeptPatient> GetReqOutwardDrugClinicDeptPatientSumByID(long ReqDrugInClinicDeptID);


        public abstract List<OutwardDrugMedDept> GetRequestDrugInwardClinicDeptDetailByRequestIDNew(long ReqDrugInClinicDeptID);

        public abstract List<RequestDrugInwardClinicDept> SearchRequestDrugInwardClinicDept(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount);

        public abstract List<RequestDrugInwardClinicDept> GetRequestDrugInwardClinicDept_ByRegistrationID(long PtRegistrationID, long V_MedProductType, long StoreID, long? outiID);

        public abstract bool DeleteRequestDrugInwardClinicDept(long ReqDrugInClinicDeptID);

        #endregion

        #region 20. Outward Drug DrugDept By Request


        public abstract List<OutwardDrugClinicDept> spGetInBatchNumberAndPrice_ListForRequestClinicDept(bool? IsCost,
                                                                                         List<RequestDrugInwardClinicDept> ReqDrugInClinicDeptID,
                                                                                         long OutwardTemplateID,
                                                                                         long StoreID,
                                                                                         long V_MedProductType, long PtRegistrationID, bool? IsHIPatient, DateTime OutDate);

        public abstract List<OutwardDrugMedDept> spGetInBatchNumberAndPrice_ListForRequest(bool? IsCost, long ReqDrugInClinicDeptID, long StoreID, long V_MedProductType);

        public abstract List<OutwardDrugMedDept> GetRequestDrugDeptList_ForDepositGoods(long ReqDrugInClinicDeptID, long StoreID, long V_MedProductType);

        public abstract List<OutwardDrugMedDept> GetOutwardDrugMedDeptDetailByInvoice(long ID, long V_MedProductType, bool FromClinicDept);

        public abstract List<OutwardDrugClinicDept> GetOutwardDrugClinicDeptDetailByInvoice(long ID);
        public abstract List<OutwardDrugClinicDept> GetOutwardDrugClinicDeptDetailByInvoice_V2(long ID, long? V_MedProductType);

        public abstract List<OutwardDrugMedDept> GetOutwardDrugMedDeptDetailByInvoice_HangKyGoi(long ID);

        public abstract List<OutwardDrugMedDept> GetListDrugExpiryDate_DrugDept(long? StoreID, int Type, long V_MedProductType);

        public abstract List<OutwardDrugClinicDept> OutwardDrugClinicDeptInvoices_SearchTKPaging(SearchOutwardInfo SearchCriteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int TotalCount);

        public abstract bool OutwardDrugMedDept_Delete(long id, long StaffID, long V_MedProductType);

        public abstract OutwardDrugClinicDeptInvoice GetOutwardDrugClinicDeptInvoice(long ID);
        public abstract OutwardDrugClinicDeptInvoice GetOutwardDrugClinicDeptInvoice_V2(long ID, long? V_MedProductType);

        public abstract List<OutwardDrugClinicDeptInvoice> OutwardDrugClinicDeptInvoice_SearchByType(MedDeptInvoiceSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount);

        public abstract OutwardDrugMedDeptInvoice GetOutwardDrugMedDeptInvoice(long ID, long V_MedProductType);

        public abstract List<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoice_SearchByType(MedDeptInvoiceSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount);

        public abstract List<RefGenMedProductDetails> GetRefGenMedProductDetailsAuto_ByRequestID(string BrandName, long V_MedProductType, long? RequestID, int pageIndex, int pageSize, out int totalcount);

        public abstract List<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient);

        public abstract List<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestDrugDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID, bool? IsCode);
        
        public abstract List<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, long? RequestID, bool? IsCode, long? OutputToStoreID = null);

        public abstract List<GetGenMedProductForSell> GetGenMedProductForSellAutoComplete_ForPrescriptionByID(byte HI, bool? IsHIPatient, long StoreID, long V_MedProductType, long? IssueID, long RefGenDrugCatID_1);

        public abstract List<OutwardDrugMedDept> spGetInBatchNumberAndPrice_ByPresciption_InPt(long PrescriptID, long StoreID, Int16 IsObject, long V_MedProductType, long RefGenDrugCatID_1);

        public abstract bool SaveOutwardInvoice(OutwardDrugMedDeptInvoice Invoice, out long outiID);

        public abstract bool UpdateOutwardInvoice(OutwardDrugMedDeptInvoice Invoice, out long outiID_New);

        public abstract List<OutwardDrugMedDeptInvoice> GetOutWardDrugInvoiceSearchAllByStatus_InPt(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract List<OutwardDrugMedDept> GetOutwardDrugDetailsByOutwardInvoice_InPt(long OutiID);

        public abstract OutwardDrugMedDeptInvoice GetOutWardDrugInvoiceByID_InPt(long outiID);

        public abstract List<GetGenMedProductForSell> GetGenMedProductForSellAutoComplete_ForPrescription(byte HI, bool? IsHIPatient, string BrandName, long StoreID, long? IssueID, bool? IsCode, long V_MedProductType, long RefGenDrugCatID_1);

        public abstract bool DeleteOutwardInvoice(long outiID, long staffID);

        public abstract List<GetGenMedProductForSell> GetInBatchNumberAllDrugDept_ByGenMedProductID_ForPrescription(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);

        public abstract bool CreateBillForOutwardFromPrescription(OutwardDrugMedDeptInvoice Invoice, long StaffID, out long InPatientBillingInvID);

        public abstract bool DeleteBillForOutwardFromPrescription(long InPatientBillingInvID, long StaffID);

        public abstract List<RefGenMedProductDetails> GetDrugForSellVisitorAutoComplete_ForRequestDrugDept_HangKyGoi(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1,
                                                                                                                        long? RequestID, bool? IsCode, int PageSize, int PageIndex, out int Total);

        public abstract bool OutwardDrugClinicDeptInvoice_SaveByType(OutwardDrugClinicDeptInvoice Invoice, out long ID, out string StrError);

        public abstract bool OutwardDrugClinicDeptInvoice_UpdateByType(OutwardDrugClinicDeptInvoice Invoice, out long ID, out string StrError);

        public abstract bool OutwardDrugMedDeptInvoice_SaveByType(OutwardDrugMedDeptInvoice Invoice, out long ID, out string StrError);

        public abstract bool OutwardDrugMedDeptInvoice_Update(OutwardDrugMedDeptInvoice Invoice, List<OutwardDrugMedDept> NewOutwardDrugMedDepts, List<OutwardDrugMedDept> UpdateOutwardDrugMedDepts, List<OutwardDrugMedDept> DeleteOutwardDrugMedDepts, out long ID, out string StrError);

        public abstract bool RequireUnlockOutMedDeptInvoice(long outiID);

        public abstract bool OutwardDrugMedDeptInvoice_SaveByType_HangKyGoi(OutwardDrugMedDeptInvoice Invoice, List<OutwardDrugMedDept> NewOutwardDrugMedDepts, List<OutwardDrugMedDept> UpdateOutwardDrugMedDepts, List<OutwardDrugMedDept> DeleteOutwardDrugMedDepts, out long ID, out string StrError);

        public abstract bool OutwardDrugMedDeptInvoices_HangKyGoi_Delete(long outiID);

        public abstract List<RefOutputType> RefOutputType_Get(bool? All);

        public abstract IList<Prescription> SearchPrescription_InPt(PrescriptionSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract StaffDeptPresence GetAllStaffDeptPresenceInfo(long DeptID, DateTime StaffCountDate);

        public abstract StaffDeptPresence SaveAllStaffDeptPresenceInfo(StaffDeptPresence CurStaffDeptPresence, bool IsUpdateRequiredNumber);

        #endregion

        #region 21. Drug Expiry
        public abstract List<OutwardDrug> GetListDrugExpiryDate(long? StoreID, int type);

        public abstract bool ListDrugExpiryDate_Save(OutwardDrugInvoice Invoice, out long ID, out string StrError);

        public abstract List<OutwardDrugInvoice> OutWardDrugInvoice_SearchByType(SearchOutwardInfo SearchCriteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        #endregion

        #region 22. RefDisposableMedicalResources
        //Danh sach
        public abstract List<RefDisposableMedicalResource> RefDisposableMedicalResources_Paging(
              RefDisposableMedicalResourceSearchCriteria Criteria,
              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );
        //Danh sach

        public abstract void RefDisposableMedicalResourcesInsertUpdate(
        RefDisposableMedicalResource Obj, out string Result);

        public abstract bool RefDisposableMedicalResources_MarkDelete(
        Int64 DMedRscrID);

        public abstract RefDisposableMedicalResource RefDisposableMedicalResources_ByDMedRscrID(Int64 DMedRscrID);
        #endregion

        #region 23. DisposableMedicalResourceTypes
        public abstract List<DisposableMedicalResourceType> DisposableMedicalResourceTypes_GetAll();
        #endregion

        #region 24. Drug Dept Member

        #region 24.0 RefGenMedProductDetails

        public abstract List<RefGenMedProductDetails> GetRefGenMedProductDetails_GetALL(long V_MedProductType);

        public abstract List<RefGenMedProductDetails> GetRefGenMedProductDetails_Auto(bool? IsCode, string BrandName, long V_MedProductType, int PageSize, int PageIndex, out int TotalCount);

        public abstract List<RefGenMedProductDetails> RefGenMedProductDetails_SearchAutoPaging(bool? IsCode, string BrandName, long? SupplierID, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, out int TotalCount);
        public abstract List<RefGenMedProductDetails> RefGenMedProductDetails_SearchAutoPaging_V2(bool? IsCode, string BrandName, long? SupplierID, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, out int TotalCount, long? RefGenDrugCatID_2 = null);

        public abstract List<RefGenMedProductSimple> RefGenMedProductDetails_SimpleAutoPaging(bool? IsCode,
                                                                                             string BrandName,
                                                                                             long V_MedProductType,
                                                                                             long? RefGenDrugCatID_1,
                                                                                             int PageSize,
                                                                                             int PageIndex,
                                                                                             out int TotalCount);

        //▼===== 25072018 TTM
        public abstract List<RefGenMedProductSimple> RefGenMedProductDetails_SimpleAutoPaging_New(bool? IsCode,
                                                                                     string BrandName,
                                                                                     long V_MedProductType,
                                                                                     long? RefGenDrugCatID_1,
                                                                                     int PageSize,
                                                                                     int PageIndex,
                                                                                     out int TotalCount);
        //▲===== 25072018 TTM


        #endregion

        #region 24.1 Estimation For Med Dept
        public abstract List<DrugDeptEstimationForPoDetail> GetEstimationForMonth(long V_MedProductType, DrugDeptEstimationForPO CurrentDrugDeptEstimationForPO);

        public abstract DrugDeptEstimationForPoDetail DrugDeptEstimationForPoDetail_GenMedProductID(long GenMedProductID, string Code, DateTime EstimateDate, long? V_EstimateType, long V_MedProductType, long? RefGenDrugCatID_1);

        public abstract long DrugDeptEstimationForPO_FullOperator(long GenMedProductID, DrugDeptEstimationForPO Estimate);

        public abstract DrugDeptEstimationForPO DrugDeptEstimationForPO_ByID(long ID);

        public abstract bool DrugDeptEstimationForPO_CheckExists(long V_EstimateType, DateTime DateOfEstimation, long V_MedProductType);

        public abstract List<DrugDeptEstimationForPoDetail> DrugDeptEstimationForPoDetail_ByParentID(long ID);

        public abstract List<DrugDeptEstimationForPO> DrugDeptEstimationForPO_Search(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount);

        public abstract List<DrugDeptEstimationForPO> DrugDeptEstimationForPO_Load(long V_MedProductType);

        public abstract List<DrugDeptEstimationForPoDetail> DrugDeptEstimationForPoDetail_AllDrugAuto(long V_MedProductType, DateTime EstimationDate, long? V_EstimateType, long? RefGenDrugCatID_1, string BrandName, int pageIndex, int pageSize, out int totalcount, bool? IsCode);
        //CRUD
        public abstract bool DrugDeptEstimationForPO_Delete(long DrugDeptEstimatePoID);

        public abstract List<RefGenMedProductDetails> RefGenMedProductDetails_Paging(
            RefGenMedProductDetailsSearchCriteria SearchCriteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );

        public abstract List<List<string>> ExportToExcel_ListRefGenMedProductDetail(ReportParameters criteria);

        public abstract void RefGenMedProductDetails_Save(
        RefGenMedProductDetails Obj, out string Res, out long id);

        //▼===== #004
        public abstract void RefGenMedProductDetails_Save_New(RefGenMedProductDetails Obj, out string Res, out long id);
        //▲===== #004

        public abstract void RefGenMedProductDetails_MarkDelete(
        Int64 GenMedProductID, out string Res);
        //CRUD

        //Properties Navigate
        #region 24.1 DrugClasses
        public abstract List<DrugClass> DrugClasses_SearchPaging(DrugClassSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract List<DrugClass> DrugDeptClasses_SearchPaging(DrugClassSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        #endregion

        #region 24.2 RefCountries
        public abstract List<RefCountry> RefCountries_SearchPaging(RefCountrySearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        #endregion

        #region 24.3 RefUnits
        public abstract List<RefUnit> RefUnits_SearchPaging(RefUnitSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract List<RefUnit> DrugDeptRefUnits_SearchPaging(RefUnitSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        #endregion

        //Properties Navigate

        #endregion

        #region 24.6 DrugDept Purchase Order Member

        public abstract List<DrugDeptPurchaseOrder> DrugDeptPurchaseOrder_BySupplierID(long SupplierID, long V_MedProductType);

        public abstract List<DrugDeptPurchaseOrderDetail> DrugDeptPurchaseOrderDetail_ByParentID(long ID, byte IsInput);

        public abstract List<DrugDeptPurchaseOrder> DrugDeptPurchaseOrder_Search(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount);

        public abstract List<DrugDeptPurchaseOrderDetail> DrugDeptPurchaseOrderDetail_GetFirst(long? DrugDeptEstimatePoID, long? SupplierID, long? PCOID, long V_MedProductType);

        public abstract bool DrugDeptPurchaseOrder_FullOperator(DrugDeptPurchaseOrder PurchaseOrder, out long id);

        public abstract DrugDeptPurchaseOrder DrugDeptPurchaseOrder_ByID(long ID);

        public abstract List<RefGenMedProductDetails> RefGenMedProductDetails_AutoPurchaseOrder(string BrandName, long? DrugDeptEstimatePoID, long V_MedProductType, long? SupplierID, int PageIndex, int PageSize, out int totalcount, bool? IsCode);

        public abstract List<RefGenMedProductDetails> RefGenMedProductDetails_WarningOrder(long V_MedProductType, int PageIndex, int PageSize, long? SupplierID, bool IsAll, out int totalcount);

        public abstract bool DrugDeptPurchaseOrders_Delete(long DrugDeptPoID);

        public abstract bool DrugDeptPurchaseOrderDetail_UpdateNoWaiting(long DrugDeptPoDetailID, bool? NoWaiting);

        public abstract bool DrugDeptPurchaseOrders_UpdateStatus(long ID, long V_PurchaseOrderStatus);
        #endregion

        public abstract bool MedDeptInvoice_UpdateInvoicePayed(OutwardDrugMedDeptInvoice Outward, out long outiID, out long PaymentID,
                                            out string StrError);

        public abstract bool MedDeptInvoice_UpdateInvoiceInfo(OutwardDrugMedDeptInvoice Outward);

        #region 24.8 MedDeptInStock

        public abstract List<InwardDrugMedDept> InStock_MedDept(string BrandName, long StoreID, bool IsDetail, long V_MedProductType, int PageIndex, int PageSize, out int TotalCount, out decimal TotalMoney);

        #endregion

        #region 24.9 Adjust Out Price

        public abstract List<InwardDrugMedDept> GetInwardDrugMedDeptForAdjustOutPrice(long StoreID, bool IsCode, string BrandName, long V_MedProductType);

        public abstract bool MedDeptAdjustOutPrice(ObservableCollection<InwardDrugMedDept> InwardDrugMedDeptList);

        #endregion

        #endregion

        #region 25. Estimation For Pharmacy

        public abstract List<PharmacyEstimationForPODetail> GetEstimationForMonthPharmacy(long V_MedProductType, DateTime EstimateDate, long? V_EstimateType, bool IsHIStorage);

        public abstract long PharmacyEstimationForPO_FullOperator(long V_MedProductType, PharmacyEstimationForPO Estimate, bool IsHIStorage, out long id);

        public abstract bool PharmacyEstimationForPO_CheckExists(long V_EstimateType, DateTime DateOfEstimation, bool IsHIStorage);

        public abstract PharmacyEstimationForPO PharmacyEstimationForPO_ByID(long ID);

        public abstract List<PharmacyEstimationForPODetail> PharmacyEstimationForPODetail_ByParentID(long ID);

        public abstract List<List<string>> ExportToExcel_EstimationForPODetail(ReportParameters criteria);

        public abstract List<PharmacyEstimationForPO> PharmacyEstimationForPO_Search(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, bool IsHIStorage, out int totalcount);

        public abstract List<PharmacyEstimationForPO> PharmacyEstimationForPO_Load(long V_MedProductType);

        public abstract PharmacyEstimationForPODetail PharmacyEstimationForPODetail_ByDrugID(DateTime EstimationDate, long DrugID, string DrugCode, long? V_EstimateType, bool IsHIStorage);

        public abstract List<PharmacyEstimationForPODetail> PharmacyEstimationForPODetail_AllDrugAuto(long V_MedProductType, DateTime EstimationDate, long? V_EstimateType, string BrandName, int pageIndex, int pageSize, out int totalcount, bool? IsCode);

        public abstract bool PharmacyEstimationForPO_Delete(long PharmacyEstimatePoID);

        #endregion

        #region 26. RefGenMedProductSellingPrices

        public abstract List<RefGenMedProductSellingPrices> RefGenMedProductSellingPrices_ByGenMedProductID_Paging(
            RefGenMedProductSellingPricesSearchCriteria Criteria,

              int PageIndex,
              int PageSize,
              string OrderBy,
              bool CountTotal,
              out int Total
              );

        public abstract void RefGenMedProductSellingPrices_CheckCanEditCanDelete(
         Int64 GenMedSellPriceID,
         out bool CanEdit,
         out bool CanDelete,
         out string PriceType);

        protected override RefGenMedProductSellingPrices GetRefGenMedProductSellingPricesFromReader(IDataReader reader)
        {
            RefGenMedProductSellingPrices p = base.GetRefGenMedProductSellingPricesFromReader(reader);

            //Xét CanEdit, CanDelete cho Items này
            bool CanEdit = false;
            bool CanDelete = false;
            string PriceType = "";

            Int64 GenMedSellPriceID = 0;
            Int64.TryParse(reader["GenMedSellPriceID"].ToString(), out GenMedSellPriceID);
            if (GenMedSellPriceID > 0)
            {
                RefGenMedProductSellingPrices_CheckCanEditCanDelete(GenMedSellPriceID, out CanEdit, out CanDelete, out PriceType);
            }
            p.CanEdit = CanEdit;
            p.CanDelete = CanDelete;
            p.PriceType = PriceType;
            //Xét CanEdit, CanDelete cho Items này
            return p;
        }

        public abstract void RefGenMedProductSellingPrices_Save(RefGenMedProductSellingPrices Obj, out string Result);

        public abstract void RefGenMedProductSellingPrices_MarkDelete(Int64 GenMedSellPriceID, out string Result);

        //Đọc by ID
        public abstract RefGenMedProductSellingPrices RefGenMedProductSellingPrices_ByGenMedSellPriceID(Int64 ID);
        //Đọc by ID


        #endregion

        #region 27. Supplier Product Member
        public abstract List<Supplier> SearchSupplierAutoPaging(SupplierSearchCriteria Criteria, int PageSize, int PageIndex, bool bcount, out int TotalCount);

        #region 27.0 Supplier Med Product

        public abstract int SupplierGenMedProduct_Insert(SupplierGenMedProduct Supplier);
        public abstract int SupplierGenMedProduct_Update(SupplierGenMedProduct Supplier);
        public abstract bool SupplierGenMedProduct_Delete(long ID);
        public abstract List<DrugDeptSupplier> DrugDeptSupplier_LoadDrugIDNotPaging(long GenMedProductID);
        public abstract List<SupplierGenMedProduct> SupplierGenMedProduct_LoadDrugIDNotPaging(long GenMedProductID,out List<RefMedicalServiceItem> ServiceList);
        public abstract List<SupplierGenMedProduct> SupplierGenMedProduct_LoadDrugID(long GenMedProductID, int PageSize, int PageIndex, bool bcount, out int TotalCount);
        public abstract List<SupplierGenMedProduct> SupplierGenMedProduct_LoadSupplierID(long SupplierID, int PageSize, int PageIndex, bool bcount, out int TotalCount);

        #endregion

        #region 27.1 Supplier Drug
        public abstract int SupplierGenericDrug_Insert(SupplierGenericDrug Supplier);
        public abstract int SupplierGenericDrug_Update(SupplierGenericDrug Supplier);
        public abstract bool SupplierGenericDrug_Delete(long ID);

        public abstract List<SupplierGenericDrug> SupplierGenericDrug_LoadDrugIDNotPaging(long DrugID);

        //▼===== 25072018 TTM
        public abstract List<SupplierGenericDrug> SupplierGenericDrug_LoadDrugIDNotPaging_New(long DrugID);
        //▲===== 25072018 TTM

        public abstract List<SupplierGenericDrug> SupplierGenericDrug_LoadDrugID(long DrugID, int PageSize, int PageIndex, bool bcount, out int TotalCount);
        public abstract List<SupplierGenericDrug> SupplierGenericDrug_LoadSupplierID(long SupplierID, int PageSize, int PageIndex, bool bcount, out int TotalCount);

        #endregion

        #region 27.2 SupplierGenericDrugPrice
        public abstract List<Supplier> SupplierGenericDrugPrice_GetListSupplier_Paging(
          SupplierGenericDrugPriceSearchCriteria Criteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );

        public abstract List<SupplierGenericDrugPrice> SupplierGenericDrugPrice_ListDrugBySupplierID_Paging(
          SupplierGenericDrugPriceSearchCriteria Criteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );


        //Quản Lý Giá
        public abstract List<SupplierGenericDrugPrice> SupplierGenericDrugPrice_ListPrice_Paging(
           SupplierGenericDrugPriceSearchCriteria Criteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );

        public abstract void SupplierGenericDrugPrice_CheckCanEditCanDelete(
         Int64 PKID,

         out bool CanEdit,
         out bool CanDelete,
         out string PriceType);

        protected override SupplierGenericDrugPrice GetSupplierGenericDrugPriceFromReader(IDataReader reader)
        {
            SupplierGenericDrugPrice p = base.GetSupplierGenericDrugPriceFromReader(reader);

            //Xét CanEdit, CanDelete cho Items này
            bool CanEdit = false;
            bool CanDelete = false;
            string PriceType = "";

            Int64 PKID = 0;
            Int64.TryParse(reader["PKID"].ToString(), out PKID);
            if (PKID > 0)
            {
                SupplierGenericDrugPrice_CheckCanEditCanDelete(PKID, out CanEdit, out CanDelete, out PriceType);
            }
            p.CanEdit = CanEdit;
            p.CanDelete = CanDelete;
            p.PriceType = PriceType;
            //Xét CanEdit, CanDelete cho Items này
            return p;
        }

        public abstract void SupplierGenericDrugPrice_Save(SupplierGenericDrugPrice Obj, out string Result);

        public abstract void SupplierGenericDrugPrice_MarkDelete(Int64 PKID, out string Result);

        public abstract SupplierGenericDrugPrice SupplierGenericDrugPrice_ByPKID(Int64 PKID);

        //Quản Lý Giá


        public abstract bool SupplierGenericDrugPrice_XMLSave(IList<SupplierGenericDrugPrice> ObjCollect);


        #endregion

        #region 27.3 SupplierGenMedProductsPrice
        public abstract List<Supplier> SupplierGenMedProductsPrice_GetListSupplier_Paging(
          SupplierGenMedProductsPriceSearchCriteria Criteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );

        public abstract List<SupplierGenMedProductsPrice> SupplierGenMedProductsPrice_ListGenMedProductIDBySupplierID_Paging(
          SupplierGenMedProductsPriceSearchCriteria Criteria,

            int PageIndex,
            int PageSize,
            string OrderBy,
            bool CountTotal,
            out int Total
            );


        //Quản Lý Giá
        public abstract List<SupplierGenMedProductsPrice> SupplierGenMedProductsPrice_ListPrice_Paging(
           SupplierGenMedProductsPriceSearchCriteria Criteria,

             int PageIndex,
             int PageSize,
             string OrderBy,
             bool CountTotal,
             out int Total
             );

        public abstract void SupplierGenMedProductsPrice_CheckCanEditCanDelete(
         Int64 PKID,

         out bool CanEdit,
         out bool CanDelete,
         out string PriceType);

        protected override SupplierGenMedProductsPrice GetSupplierGenMedProductsPriceFromReader(IDataReader reader)
        {
            SupplierGenMedProductsPrice p = base.GetSupplierGenMedProductsPriceFromReader(reader);

            //Xét CanEdit, CanDelete cho Items này
            bool CanEdit = false;
            bool CanDelete = false;
            string PriceType = "";

            Int64 PKID = 0;
            Int64.TryParse(reader["PKID"].ToString(), out PKID);
            if (PKID > 0)
            {
                SupplierGenMedProductsPrice_CheckCanEditCanDelete(PKID, out CanEdit, out CanDelete, out PriceType);
            }
            p.CanEdit = CanEdit;
            p.CanDelete = CanDelete;
            p.PriceType = PriceType;
            //Xét CanEdit, CanDelete cho Items này
            return p;
        }

        public abstract void SupplierGenMedProductsPrice_Save(SupplierGenMedProductsPrice Obj, out string Result);

        public abstract void SupplierGenMedProductsPrice_MarkDelete(Int64 PKID, out string Result);

        public abstract SupplierGenMedProductsPrice SupplierGenMedProductsPrice_ByPKID(Int64 PKID);

        //Quản Lý Giá
        #endregion

        #region 27.4 DrugDept Supplier

        public abstract List<DrugDeptSupplier> DrugDeptSupplier_SearchAutoPaging(SupplierSearchCriteria Criteria, int PageSize, int PageIndex, bool bcount, out int TotalCount);

        public abstract List<DrugDeptSupplier> DrugDeptSupplierXapNhapPXTemp_SearchPaging(SupplierSearchCriteria Criteria, int PageIndex, int PageSize, bool CountTotal, out int Total);

        public abstract bool DeleteDrugDeptSupplierByID(long supplierID);

        public abstract bool UpdateDrugDeptSupplier(DrugDeptSupplier supplier, out string StrError);

        public abstract bool AddNewDrugDeptSupplier(DrugDeptSupplier supplier, out long SupplierID, out string StrError);

        public abstract List<DrugDeptSupplier> DrugDeptSupplier_GetCbx(int supplierType);

        #endregion
        #endregion

        #region 28. Purchase Order For Drug

        public abstract List<PharmacyPurchaseOrderDetail> PharmacyPurchaseOrderDetail_GetFirst(long? PharmacyEstimatePoID, long? SupplierID, long? PCOID);

        public abstract bool PharmacyPurchaseOrderDetail_UpdateNoWaiting(long PharmacyPoDetailID, bool? NoWaiting);

        public abstract bool PharmacyPurchaseOrder_FullOperator(PharmacyPurchaseOrder PurchaseOrder, out long id);

        public abstract PharmacyPurchaseOrder PharmacyPurchaseOrder_ByID(long ID);

        public abstract bool PharmacyPurchaseOrder_UpdateStatus(long ID, long V_PurchaseOrderStatus);

        public abstract List<PharmacyPurchaseOrderDetail> PharmacyPurchaseOrderDetail_ByParentID(long ID, byte IsInput);

        public abstract List<PharmacyPurchaseOrder> PharmacyPurchaseOrder_Search(RequestSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount);

        public abstract bool PharmacyPurchaseOrders_Delete(long PharmacyPoID);

        public abstract List<RefGenericDrugDetail> RefGenericDrugDetail_AutoRequest(string BrandName, long? PharmacyEstimatePoID, long? SupplierID, int PageIndex, int PageSize, out int totalcount, bool? IsCode);

        public abstract List<RefGenericDrugDetail> RefGenericDrugDetail_WarningOrder(int PageIndex, int PageSize, long? SupplierID, bool IsAll, out int totalcount);

        #endregion

        #region 29. Report member

        public abstract List<List<string>> ExportExcel_RemainInward(ReportParameters criteria);

        public abstract List<InwardDrug> InwardDrugs_TonKho(string BrandName, long StoreID, bool IsDetail, int PageIndex, int PageSize, out int TotalCount, out decimal TotalMoney);

        public abstract List<OutwardDrug> OutwardDrugs_SellOnDate(string BrandName, long StoreID, DateTime FromDate, DateTime ToDate, bool IsDetail, int PageIndex, int PageSize, out int TotalCount, out decimal TotalMoney);

        #endregion

        #region 30. StockTakes member

        public abstract List<PharmacyStockTakes> PharmacyStockTakes_Search(PharmacyStockTakesSearchCriteria SearchCriteria, int PageIndex, int PageSize, out int TotalCount);

        public abstract List<PharmacyStockTakeDetails> PharmacyStockTakeDetails_Get(long StoreID, DateTime StockTakeDate);

        public abstract List<PharmacyStockTakeDetails> PharmacyStockTakeDetails_Load(long PharmacyStockTakeID);

        public abstract List<PharmacyStockTakeDetails> GetDrugForAutoCompleteStockTake(string BrandName, long StoreID, bool IsCode);

        public abstract bool PharmacyStockTake_Save(PharmacyStockTakes StockTake, out long ID, out string StrError);

        public abstract bool KetChuyenTonKho(long StoreID, long StaffID, string CheckPointName);

        #endregion

        #region 30.1 DrugDept StockTakes member

        public abstract List<DrugDeptStockTakes> DrugDeptStockTakes_Search(DrugDeptStockTakesSearchCriteria SearchCriteria, int PageIndex, int PageSize, out int TotalCount);

        public abstract List<DrugDeptStockTakeDetails> DrugDeptStockTakeDetails_Get(long StoreID, long V_MedProductType, DateTime StockTakeDate);

        public abstract List<DrugDeptStockTakeDetails> DrugDeptStockTakeDetails_Load(long DrugDeptStockTakeID);

        public abstract List<DrugDeptStockTakeDetails> GetProductForDrugDeptStockTake(string BrandName, long StoreID, bool IsCode, long V_MedProductType);

        public abstract bool DrugDeptStockTake_Save(DrugDeptStockTakes StockTake, bool IsConfirmFinished, out long ID, out string StrError);
        public abstract bool DrugDeptStockTake_Remove(long DrugDeptStockTakeID, long StaffID);

        public abstract bool KetChuyenTonKho_DrugDept(long StoreID, long StaffID, string CheckPointName, long V_MedProductType);

        public abstract List<List<string>> ExportExcelStockTake(ReportParameters criteria);

        #endregion

        #region 30.2 ClinicDept StockTakes member

        public abstract List<ClinicDeptStockTakes> ClinicDeptStockTakes_Search(ClinicDeptStockTakesSearchCriteria SearchCriteria, int PageIndex, int PageSize, out int TotalCount);

        public abstract List<ClinicDeptStockTakeDetails> ClinicDeptStockTakeDetails_Get(long StoreID, long V_MedProductType, DateTime StockTakeDate);

        public abstract bool ClinicDeptLockAndUnlockStore(long StoreID, long V_MedProductType, bool IsLock);

        public abstract List<ClinicDeptStockTakeDetails> ClinicDeptStockTakeDetails_Load(long ClinicDeptStockTakeID);

        public abstract List<ClinicDeptStockTakeDetails> GetProductForClinicDeptStockTake(string BrandName, long StoreID, bool IsCode, long V_MedProductType);

        public abstract bool ClinicDeptStockTake_Save(ClinicDeptStockTakes StockTake, out long ID, out string StrError);

        public abstract bool KetChuyenTonKho_ClinicDept(long StoreID, long StaffID, string CheckPointName, long V_MedProductType, DateTime CheckPointDate);

        #endregion

        #region 31. Request Form Drug For Pharmacy

        public abstract bool FullOperatorRequestDrugInward(RequestDrugInward Request, out long id);

        public abstract RequestDrugInward GetRequestDrugInwardByID(long ReqDrugInID);
        public abstract List<RequestDrugInwardDetail> GetRequestDrugInwardDetailByID(long ReqDrugInID);

        public abstract List<RequestDrugInwardDetail> GetRequestDrugInwardDetailByRequestID(long ReqDrugInID);

        public abstract List<OutwardDrug> GetRequestDrugInwardDetailByRequestIDNew(long ReqDrugInID);

        public abstract List<RequestDrugInward> SearchRequestDrugInward(RequestSearchCriteria Criteria, int pageIndex, int pageSize, bool bCount, out int totalcount);
        public abstract bool DeleteRequestDrugInward(long ReqDrugInID);

        public abstract List<OutwardDrug> spGetInBatchNumberAndPrice_ByRequestPharmacy(bool? IsCost, long RequestID, long StoreID);

        public abstract List<GetDrugForSellVisitor> GetDrugForSellVisitorAutoComplete_ForRequestPharmacy(bool? IsCost, string BrandName, long StoreID, long? RequestID, bool? IsCode);

        public abstract DateTime? OutwardDrug_GetMaxDayBuyInsurance(long PatientID, long outiID);

        public abstract bool OutwardDrugInvoice_SaveByType(OutwardDrugInvoice Invoice, out long ID, out string StrError);
        #endregion

        #region 32. SupplierPharmacyPaymentReqs Member

        public abstract List<InwardDrugInvoice> SupplierPharmacyPaymentReqs_Details(InwardInvoiceSearchCriteria criteria);

        public abstract bool SupplierPharmacyPaymentReqs_Save(SupplierPharmacyPaymentReqs PaymentReqs, out long id);

        public abstract List<InwardDrugInvoice> SupplierPharmacyPaymentReqs_DetailsByReqID(long PharmacySupplierPaymentReqID);

        public abstract SupplierPharmacyPaymentReqs SupplierPharmacyPaymentReqs_ID(long PharmacySupplierPaymentReqID);

        public abstract List<SupplierPharmacyPaymentReqs> SupplierPharmacyPaymentReqs_Search(RequestSearchCriteria Criteria, int PageSize, int PageIndex, bool bcount, out int TotalCount);

        public abstract bool SupplierPharmacyPaymentReqs_UpdateStatus(long ID, long V_PaymentReqStatus, long? StaffID);

        public abstract bool SupplierPharmacyPaymentReqs_Delete(long ID);

        #endregion

        #region 32.0 SupplierDrugDeptPaymentReqs Member

        public abstract List<InwardDrugMedDeptInvoice> SupplierDrugDeptPaymentReqs_Details(InwardInvoiceSearchCriteria criteria, long? V_MedProductType);

        public abstract bool SupplierDrugDeptPaymentReqs_Save(SupplierDrugDeptPaymentReqs PaymentReqs, out long id);

        public abstract List<InwardDrugMedDeptInvoice> SupplierDrugDeptPaymentReqs_DetailsByReqID(long DrugDeptSupplierPaymentReqID);

        public abstract SupplierDrugDeptPaymentReqs SupplierDrugDeptPaymentReqs_ID(long DrugDeptSupplierPaymentReqID);

        public abstract List<SupplierDrugDeptPaymentReqs> SupplierDrugDeptPaymentReqs_Search(RequestSearchCriteria Criteria, long? V_MedProductType, int PageSize, int PageIndex, bool bcount, out int TotalCount);

        public abstract bool SupplierDrugDeptPaymentReqs_UpdateStatus(long ID, long V_PaymentReqStatus, long? StaffID);

        public abstract bool SupplierDrugDeptPaymentReqs_Delete(long ID);

        #endregion

        #region 33. Hopital member
        public abstract List<Hospital> Hopital_IsFriends();
        #endregion

        //<Giá Nhà Thuốc>
        #region 34. Giá Bán Thuốc Của Nhà Thuốc PharmacySellingItemPrices
        public abstract List<RefGenericDrugDetail> RefGenericDrugDetailsAndPriceIsActive_Paging(DrugSearchCriteria criteria, int IsMedDept, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);


        public abstract List<PharmacySellingItemPrices> PharmacySellingItemPrices_ByDrugID_Paging(
        PharmacySellingItemPricesSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
          );


        //public abstract void PharmacySellingItemPrices_CheckCanEditCanDelete(
        //Int64 PharmacySellingItemPriceID,

        //out bool CanEdit,
        //out bool CanDelete,
        //out string PriceType);

        //KMx: Sau khi kiểm tra, thấy hàm này không cần dùng nữa. Mỗi lần xem lại bảng giá cũ, có bao nhiêu thuốc là về Database bấy nhiêu lần (31/05/2014 17:55).
        //protected override PharmacySellingItemPrices GetPharmacySellingItemPricesFromReader(IDataReader reader)
        //{
        //    PharmacySellingItemPrices p = base.GetPharmacySellingItemPricesFromReader(reader);

        //    //Xét CanEdit, CanDelete cho Items này
        //    bool CanEdit = false;
        //    bool CanDelete = false;
        //    string PriceType = "";

        //    Int64 PharmacySellingItemPriceID = 0;
        //    Int64.TryParse(reader["PharmacySellingItemPriceID"].ToString(), out PharmacySellingItemPriceID);
        //    if (PharmacySellingItemPriceID > 0)
        //    {
        //        PharmacySellingItemPrices_CheckCanEditCanDelete(PharmacySellingItemPriceID, out CanEdit, out CanDelete, out PriceType);
        //    }
        //    p.CanEdit = CanEdit;
        //    p.CanDelete = CanDelete;
        //    p.PriceType = PriceType;
        //    //Xét CanEdit, CanDelete cho Items này
        //    return p;
        //}

        public abstract void PharmacySellingItemPrices_Save(PharmacySellingItemPrices Obj, out string Result);

        public abstract void PharmacySellingItemPrices_Item_Save(PharmacySellingItemPrices Obj, out string Result);

        public abstract bool PharmacySellingItemPrices_SaveRow(PharmacySellingItemPrices Obj);

        public abstract void PharmacySellingItemPrices_MarkDelete(Int64 PharmacySellingItemPriceID, out string Result);

        //KMx: Sau khi kiểm tra, thấy hàm này không còn sử dụng nữa (31/05/2014 17:19).
        //public abstract PharmacySellingItemPrices PharmacySellingItemPrices_ByPharmacySellingItemPriceID(Int64 PharmacySellingItemPriceID);

        #endregion

        #region "35. Bảng Giá Bán Thuốc Của Nhà Thuốc PharmacySellingPriceList"

        public abstract List<PharmacyReferenceItemPrice> PharmacyRefPriceList_AutoCreate();

        public abstract bool PharmacyRefPriceList_AddNew(PharmacyReferencePriceList Obj, out long ReferencePriceListID);

        public abstract bool PharmacyRefPriceList_Update(PharmacyReferencePriceList Obj);

        public abstract List<PharmacyReferencePriceList> GetReferencePriceList(PharmacySellingPriceListSearchCriteria SearchCriteria, int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total);

        public abstract List<PharmacyReferenceItemPrice> GetPharmacyRefItemPrice(Int64 ReferencePriceListID);

        public abstract void PharmacySellingPriceList_CheckCanAddNew(out bool CanAddNew);

        public abstract List<PharmacySellingItemPrices> PharmacySellingPriceList_AutoCreate(out string Result);

        public abstract List<PharmacySellingItemPrices> PharmacySellingPriceList_AutoCreate_V2(out string Result);

        public abstract void PharmacySellingPriceList_AddNew(PharmacySellingPriceList Obj, out string Result);

        public abstract Nullable<DateTime> PharmacySellingItemPrices_EffectiveDateMax();

        public abstract List<PharmacySellingPriceList> PharmacySellingPriceList_GetList_Paging(
    PharmacySellingPriceListSearchCriteria SearchCriteria,

     int PageIndex,
     int PageSize,
     string OrderBy,
     bool CountTotal,
     out int Total
            , out DateTime curDate
     );
        public abstract void PharmacySellingPriceList_Delete(Int64 PharmacySellingPriceListID, out string Result);

        public abstract List<PharmacySellingItemPrices> PharmacySellingPriceList_Detail(Int64 PharmacySellingPriceListID);

        public abstract List<PharmacySellingItemPrices> PharmacySellingPriceList_Detail_V2(Int64 PharmacySellingPriceListID);

        public abstract void PharmacySellingPriceList_Update(PharmacySellingPriceList Obj, out string Result);

        public abstract List<List<string>> ExportToExcelAllItemsPriceList(Int64 PriceListID, int PriceListType);

        public abstract List<List<string>> ExportToExcelAllItemsPriceList_New(ReportParameters criteria);

        #endregion

        #region "36. Thang Giá Bán Của Nhà Thuốc PharmacySellPriceProfitScale"
        public abstract IList<PharmacySellPriceProfitScale> PharmacySellPriceProfitScale_GetList_Paging(
bool IsActive,

int PageIndex,
int PageSize,
string OrderBy,
bool CountTotal,
out int Total
     );

        public abstract void PharmacySellPriceProfitScale_AddEdit(PharmacySellPriceProfitScale Obj, out string Result);

        public abstract void PharmacySellPriceProfitScale_IsActive(Int64 PharmacySellPriceProfitScaleID, Boolean IsActive, out string Result);

        #endregion
        //</Giá Nhà Thuốc>

        //Nytest
        public abstract string NytestXML();

        #region "37.RefGenDrugBHYT_Category"
        public abstract List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Load(bool? IsClassification, bool? IsCombined);

        public abstract bool Combine_RefGenDrugBHYT_Category(string CategoryCheckedListXml, out List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Out);

        public abstract bool DeleteRefGenDrugBHYT_CategoryCombine(long RefGenDrugBHYT_CatID, out List<RefGenDrugBHYT_Category> RefGenDrugBHYT_Category_Out);
        #endregion

        //<Giá Khoa Dược>

        #region 38. Giá Bán Thuốc Của Khoa Dược DrugDeptSellingItemPrices
        public abstract List<DrugDeptSellingItemPrices> DrugDeptSellingItemPrices_ByDrugID_Paging(
        DrugDeptSellingItemPricesSearchCriteria SearchCriteria,

          int PageIndex,
          int PageSize,
          string OrderBy,
          bool CountTotal,
          out int Total
          );


        //public abstract void DrugDeptSellingItemPrices_CheckCanEditCanDelete(
        //Int64 DrugDeptSellingItemPriceID,

        //out bool CanEdit,
        //out bool CanDelete,
        //out string PriceType);

        //KMx: Sau khi kiểm tra, thấy hàm này không cần dùng nữa. Mỗi lần xem lại bảng giá cũ, có bao nhiêu thuốc là về Database bấy nhiêu lần (22/06/2014 16:08).
        //protected override DrugDeptSellingItemPrices GetDrugDeptSellingItemPricesFromReader(IDataReader reader)
        //{
        //    DrugDeptSellingItemPrices p = base.GetDrugDeptSellingItemPricesFromReader(reader);

        //    //Xét CanEdit, CanDelete cho Items này
        //    bool CanEdit = false;
        //    bool CanDelete = false;
        //    string PriceType = "";

        //    Int64 DrugDeptSellingItemPriceID = 0;
        //    Int64.TryParse(reader["DrugDeptSellingItemPriceID"].ToString(), out DrugDeptSellingItemPriceID);
        //    if (DrugDeptSellingItemPriceID > 0)
        //    {
        //        DrugDeptSellingItemPrices_CheckCanEditCanDelete(DrugDeptSellingItemPriceID, out CanEdit, out CanDelete, out PriceType);
        //    }
        //    p.CanEdit = CanEdit;
        //    p.CanDelete = CanDelete;
        //    p.PriceType = PriceType;
        //    //Xét CanEdit, CanDelete cho Items này
        //    return p;
        //}

        public abstract void DrugDeptSellingItemPrices_Save(DrugDeptSellingItemPrices Obj, out string Result);

        public abstract void DrugDeptSellingItemPrices_MarkDelete(Int64 DrugDeptSellingItemPriceID, out string Result);

        public abstract DrugDeptSellingItemPrices DrugDeptSellingItemPrices_ByDrugDeptSellingItemPriceID(Int64 DrugDeptSellingItemPriceID);

        #endregion

        #region 39. Bảng Giá Bán Thuốc Của Khoa Dược DrugDeptSellingPriceList

        public abstract void DrugDeptSellingPriceList_CheckCanAddNew(out bool CanAddNew);

        public abstract List<DrugDeptSellingItemPrices> DrugDeptSellingPriceList_AutoCreate(long V_MedProductType, out string Result);

        public abstract void DrugDeptSellingPriceList_AddNew(DrugDeptSellingPriceList Obj, out string Result);

        public abstract Nullable<DateTime> DrugDeptSellingItemPrices_EffectiveDateMax();


        public abstract List<DrugDeptSellingPriceList> DrugDeptSellingPriceList_GetList_Paging(
    DrugDeptSellingPriceListSearchCriteria SearchCriteria,

     int PageIndex,
     int PageSize,
     string OrderBy,
     bool CountTotal,
     out int Total
            , out DateTime curDate
     );

        public abstract void DrugDeptSellingPriceList_Delete(Int64 DrugDeptSellingPriceListID, out string Result);

        public abstract List<DrugDeptSellingItemPrices> DrugDeptSellingPriceList_Detail(Int64 DrugDeptSellingPriceListID);

        public abstract void DrugDeptSellingPriceList_Update(DrugDeptSellingPriceList Obj, out string Result);

        #endregion

        #region "40. Thang Giá Bán Của Khoa Dược DrugDeptSellPriceProfitScale"
        public abstract IList<DrugDeptSellPriceProfitScale> DrugDeptSellPriceProfitScale_GetList_Paging(
long V_MedProductType,
bool IsActive,
int PageIndex,
int PageSize,
string OrderBy,
bool CountTotal,
out int Total
     );

        public abstract void DrugDeptSellPriceProfitScale_AddEdit(DrugDeptSellPriceProfitScale Obj, out string Result);

        public abstract void DrugDeptSellPriceProfitScale_IsActive(Int64 DrugDeptSellPriceProfitScaleID, Boolean IsActive, out string Result);

        #endregion
        //</Giá Khoa Dược>

        #region 39.CostTableForMedDeptInvoice Member

        public abstract bool CostTableMedDept_Insert(CostTableMedDept Item, out long CoID, out string StrCoNumber);

        public abstract IList<CostTableMedDept> CostTableMedDept_Search(InwardInvoiceSearchCriteria criteria, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract IList<CostTableMedDeptList> CostTableMedDeptList_ByID(long CoID);

        public abstract IList<InwardDrugMedDeptInvoice> CostTableForMedDeptInvoice_ByCoID(long CoID);

        public abstract IList<CostTableMedDeptList> InwardDrugMedDeptInvoice_GetListCost(long inviID);

        #endregion

        #region 40. Return MedDept Member

        public abstract bool OutWardDrugMedDeptInvoiceReturn_Insert(OutwardDrugMedDeptInvoice InvoiceDrug, out long outwardid);

        #endregion

        #region "41 Rpt Khoa Dược Xuất Đến khoa phòng"
        public abstract DataTable OutwardDrugMedDeptInvoices_ListOutToKhoKhoaPhong(long V_MedProductType, DateTime FromDate, DateTime ToDate, long StoreID, long? StoreClinicID, bool? IsShowHave);

        public abstract DataTable OutwardDrugMedDept_OutToClinicDept(long V_MedProductType, DateTime FromDate, DateTime ToDate, long StoreID, List<long> OutputToID);

        public abstract DataTable OutwardDrugMedDeptInvoices_ListGenMedProductIDNhap_Ton(long V_MedProductType, DateTime FromDate, DateTime ToDate, long StoreID, bool? IsShowHaveMedProduct);

        public abstract double SLXuatByOutputToID(long V_MedProductType, DateTime FromDate, DateTime ToDate, long StoreID, long OutputToID, long GenMedProductID);

        #endregion

        #region Xáp nhập phiếu nhập tạm
        public abstract IList<InwardDrugMedDept> InwardDrugMedDeptIsInputTemp_BySupplierID(long SupplierID, long V_MedProductType);

        public abstract bool InwardDrugInvoices_XapNhapInputTemp_Save(long inviIDJoin, IEnumerable<InwardDrugMedDept> ObjInwardDrugMedDeptList, out string Result);

        #endregion

        #region 43. PharmacyOutwardDrugReport Member

        public abstract IList<PharmacyOutwardDrugReportDetail> PharmacyOutwardDrugReportDetail_GetReport(PharmacyOutwardDrugReport para, long loggedStaffID);

        public abstract bool PharmacyOutwardDrugReport_Save(PharmacyOutwardDrugReport para, out long id);

        public abstract IList<PharmacyOutwardDrugReport> PharmacyOutwardDrugReport_Search(SearchOutwardReport para, long loggedStaffID, int PageIndex, int PageSize, out int TotalCount);

        public abstract IList<PharmacyOutwardDrugReportDetail> PharmacyOutwardDrugReportDetail_GetID(long ID);

        #endregion

        public abstract List<DrugDeptPurchaseCheckOrder> DrugDeptPurchaseOrderDetails_CheckOrder(long GenMedProductID, DateTime FromDate, DateTime ToDate, out List<DrugDeptPurchaseCheckOrderInward> InwardList);

        public abstract List<PharmacyPurchaseCheckOrder> PharmacyPurchaseOrderDetails_CheckOrder(long DrugID, DateTime FromDate, DateTime ToDate, out List<PharmacyPurchaseCheckOrderInward> InwardList);

        public abstract List<OutwardDrugInvoice> OutwardDrugInvoice_CollectMultiDrug(int top, DateTime FromDate, DateTime ToDate, long StoreID);
        public abstract bool OutwardDrugInvoice_UpdateCollectMulti(IEnumerable<OutwardDrugInvoice> lst);

        public abstract List<List<string>> ExportToExcel_PharmacyReports(ReportParameters criteria);
        /*▼====: #002*/
        public abstract List<Bid> GetAllBids();
        public abstract List<BidDetail> GetBidDetails(long BidID, string DrugCode, bool IsMedDept);
        public abstract bool SaveBidDetails(long BidID, List<BidDetail> ModBidDetails, List<BidDetail> RemovedBidDetails, bool IsMedDept);
        public abstract bool RemoveBidDetails(long BidID, long DrugID, bool IsMedDept);
        //▼===== 25072018 TTM
        public abstract bool RemoveBidDetails_New(long BidID, long DrugID, bool IsMedDept);
        //▼===== 25072018 TTM

        public abstract bool EditDrugBid(Bid aBid, out long? BidIDOut);
        /*▲====: #002*/
        /*▼====: #003*/
        #region Adjust In Clinic Price
        public abstract List<InwardDrugClinicDept> GetInwardDrugClinicDeptForAdjustOutPrice(long StoreID, bool IsCode, string BrandName, long V_MedProductType);
        public abstract bool ClinicDeptAdjustOutPrice(IList<InwardDrugClinicDept> InwardDrugMedDeptList, long? StaffID);
        #endregion
        /*▲====: #003*/

        //▼====== #005
        public abstract List<OutwardDrugInvoice> OutwardPharmacyDeptInvoice_Cbx(long? StoreID);

        public abstract List<OutwardDrug> GetOutwardDrugDetailsByOutwardInvoiceForDrugDept(long OutiID);
        //▲====== #005
        
        //▼====== #006
        public abstract int InwardDrugInvoice_SaveXML(InwardDrugClinicDeptInvoice InvoiceDrug, out long id);

        public abstract List<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoice_Cbx_V2(long? StoreID);

        public abstract IList<InwardDrugClinicDeptInvoice> SearchInwardDrugInvoiceForPharmacy(InwardInvoiceSearchCriteria criteria, long? TypID, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        public abstract IList<InwardDrugClinicDept> GetInwardDrugPharmacy_ByIDInvoiceNotPaging(long inviID
        , out decimal TongTienSPChuaVAT
        , out decimal CKTrenSP
        , out decimal TongTienTrenSPDaTruCK
        , out decimal TongCKTrenHoaDon
        , out decimal TongTienHoaDonCoVAT);
        //▲====== #006
        //▼====== #007
        public abstract bool RequestDrugInwardHIStore_Save(RequestDrugInwardForHiStore Request,
                                                        long V_MedProductType, out long id);
        public abstract RequestDrugInwardForHiStore GetRequestDrugInwardHIStoreByID(long ReqDrugHIStoreID);

        public abstract List<RequestDrugInwardForHiStoreDetails> GetRequestDrugInwardHIStoreDetailByID(long RequestDrugInwardHiStoreID, bool bGetExistingReqToCreateNew);

        public abstract List<RequestDrugInwardForHiStore> SearchRequestDrugInwardHIStore(RequestSearchCriteria Criteria, long V_MedProductType, int pageIndex, int pageSize, bool bCount, out int totalcount);

        public abstract bool RequestDrugInwardHIStore_Approved(RequestDrugInwardForHiStore Request);

        public abstract List<OutwardDrugMedDept> GetInBatchNumberAndPrice_ListForRequest(bool? IsCost, long RequestDrugInwardHiStoreID, long StoreID, long V_MedProductType);
        public abstract List<RefGenMedProductDetails> RefGenMedProductDetails_SearchAutoPaging_ForHIStore(bool? IsCode, string BrandName, long? SupplierID, long V_MedProductType, long? RefGenDrugCatID_1, int PageSize, int PageIndex, out int TotalCount, long? RefGenDrugCatID_2 = null);
        public abstract bool DeleteRequestDrugInwardHIStore(long RequestDrugInwardHiStoreID);
        //▲====== #007

        #region 45. Inward VTYTTH Med Dept
        public abstract List<OutwardDrugMedDeptInvoice> OutwardVTYTTHMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        public abstract IList<InwardDrugMedDeptInvoice> SearchInwardVTYTTHMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract InwardDrugMedDeptInvoice GetInwardVTYTTHMedDeptInvoice_ByID(long ID);
        public abstract int AddInwardVTYTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);
        public abstract int UpdateInwardVTYTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);
        public abstract int DeleteInwardVTYTTHMedDeptInvoice(long ID);

        public abstract IList<InwardDrugMedDept> GetInwardVTYTTHMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        public abstract IList<InwardDrugMedDept> GetInwardVTYTTHMedDept_ByIDInvoiceNotPaging(long inviID
             , out decimal TongTienSPChuaVAT
             , out decimal CKTrenSP
             , out decimal TongTienTrenSPDaTruCK
             , out decimal TongCKTrenHoaDon
             , out decimal TongTienHoaDonCoThueNK
             , out decimal TongTienHoaDonCoVAT);

        public abstract int UpdateInwardVTYTTHMedDept(InwardDrugMedDept invoicedrug, long StaffID);
        public abstract int DeleteInwardVTYTTHMedDept(long invoicedrug);

        public abstract bool DeleteInwardVTYTTHMedDeptTemp(long InID);

        //public abstract List<RefGenMedProductDetails> spGetInBatchNumberAllClinicDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);

        //public abstract List<RefGenMedProductDetails> spGetInBatchNumberAllDrugDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);

        public abstract bool AddInwardVTYTTHMedDept(DrugDeptPurchaseOrderDetail invoicedrug, long inviID);

        public abstract bool InwardVTYTTHMedDeptInvoice_UpdateCost(long inviID);

        public abstract bool UpdateStatusVTYTTHMedDeptPurchaseOrder(long DrugDeptPoID);

        public abstract int InwardVTYTTHMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        public abstract IList<CostTableMedDeptList> InwardVTYTTHMedDeptInvoice_GetListCost(long inviID);
        #endregion

        #region 46. Inward TiemNgua Med Dept
        public abstract List<OutwardDrugMedDeptInvoice> OutwardTiemNguaMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        public abstract IList<InwardDrugMedDeptInvoice> SearchInwardTiemNguaMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract InwardDrugMedDeptInvoice GetInwardTiemNguaMedDeptInvoice_ByID(long ID);
        public abstract int AddInwardTiemNguaMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);
        public abstract int UpdateInwardTiemNguaMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);
        public abstract int DeleteInwardTiemNguaMedDeptInvoice(long ID);

        public abstract IList<InwardDrugMedDept> GetInwardTiemNguaMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        public abstract IList<InwardDrugMedDept> GetInwardTiemNguaMedDept_ByIDInvoiceNotPaging(long inviID
             , out decimal TongTienSPChuaVAT
             , out decimal CKTrenSP
             , out decimal TongTienTrenSPDaTruCK
             , out decimal TongCKTrenHoaDon
             , out decimal TongTienHoaDonCoThueNK
             , out decimal TongTienHoaDonCoVAT);

        public abstract int UpdateInwardTiemNguaMedDept(InwardDrugMedDept invoicedrug, long StaffID);
        public abstract int DeleteInwardTiemNguaMedDept(long invoicedrug);

        public abstract bool DeleteInwardTiemNguaMedDeptTemp(long InID);

        //public abstract List<RefGenMedProductDetails> spGetInBatchNumberAllClinicDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);

        //public abstract List<RefGenMedProductDetails> spGetInBatchNumberAllDrugDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);

        public abstract bool AddInwardTiemNguaMedDept(DrugDeptPurchaseOrderDetail invoicedrug, long inviID);

        public abstract bool InwardTiemNguaMedDeptInvoice_UpdateCost(long inviID);

        public abstract bool UpdateStatusTiemNguaMedDeptPurchaseOrder(long DrugDeptPoID);

        public abstract int InwardTiemNguaMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        public abstract IList<CostTableMedDeptList> InwardTiemNguaMedDeptInvoice_GetListCost(long inviID);
        #endregion

        #region 47. Inward Chemical Med Dept
        public abstract List<OutwardDrugMedDeptInvoice> OutwardChemicalMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        public abstract IList<InwardDrugMedDeptInvoice> SearchInwardChemicalMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract InwardDrugMedDeptInvoice GetInwardChemicalMedDeptInvoice_ByID(long ID);
        public abstract int AddInwardChemicalMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);
        public abstract int UpdateInwardChemicalMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);
        public abstract int DeleteInwardChemicalMedDeptInvoice(long ID);

        public abstract IList<InwardDrugMedDept> GetInwardChemicalMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        public abstract IList<InwardDrugMedDept> GetInwardChemicalMedDept_ByIDInvoiceNotPaging(long inviID
             , out decimal TongTienSPChuaVAT
             , out decimal CKTrenSP
             , out decimal TongTienTrenSPDaTruCK
             , out decimal TongCKTrenHoaDon
             , out decimal TongTienHoaDonCoThueNK
             , out decimal TongTienHoaDonCoVAT);

        public abstract int UpdateInwardChemicalMedDept(InwardDrugMedDept invoicedrug, long StaffID);
        public abstract int DeleteInwardChemicalMedDept(long invoicedrug);

        public abstract bool DeleteInwardChemicalMedDeptTemp(long InID);

        //public abstract List<RefGenMedProductDetails> spGetInBatchNumberAllClinicDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);

        //public abstract List<RefGenMedProductDetails> spGetInBatchNumberAllDrugDept_ByGenMedProductID(long GenMedProductID, long V_MedProductType, long StoreID, bool? IsHIPatient);

        public abstract bool AddInwardChemicalMedDept(DrugDeptPurchaseOrderDetail invoicedrug, long inviID);

        public abstract bool InwardChemicalMedDeptInvoice_UpdateCost(long inviID);

        public abstract bool UpdateStatusChemicalMedDeptPurchaseOrder(long DrugDeptPoID);

        public abstract int InwardChemicalMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        public abstract IList<CostTableMedDeptList> InwardChemicalMedDeptInvoice_GetListCost(long inviID);
        #endregion
        #region 48.Inward Blood Med Dept
        public abstract List<OutwardDrugMedDeptInvoice> OutwardBloodMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        public abstract IList<InwardDrugMedDeptInvoice> SearchInwardBloodMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract InwardDrugMedDeptInvoice GetInwardBloodMedDeptInvoice_ByID(long ID);
        public abstract int AddInwardBloodMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);
        public abstract int UpdateInwardBloodMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);
        public abstract int DeleteInwardBloodMedDeptInvoice(long ID);

        public abstract IList<InwardDrugMedDept> GetInwardBloodMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        public abstract IList<InwardDrugMedDept> GetInwardBloodMedDept_ByIDInvoiceNotPaging(long inviID
             , out decimal TongTienSPChuaVAT
             , out decimal CKTrenSP
             , out decimal TongTienTrenSPDaTruCK
             , out decimal TongCKTrenHoaDon
             , out decimal TongTienHoaDonCoThueNK
             , out decimal TongTienHoaDonCoVAT);

        public abstract int UpdateInwardBloodMedDept(InwardDrugMedDept invoicedrug, long StaffID);
        public abstract int DeleteInwardBloodMedDept(long invoicedrug);

        public abstract bool DeleteInwardBloodMedDeptTemp(long InID);

        public abstract bool AddInwardBloodMedDept(DrugDeptPurchaseOrderDetail invoicedrug, long inviID);

        public abstract bool InwardBloodMedDeptInvoice_UpdateCost(long inviID);

        public abstract bool UpdateStatusBloodMedDeptPurchaseOrder(long DrugDeptPoID);

        public abstract int InwardBloodMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        public abstract IList<CostTableMedDeptList> InwardBloodMedDeptInvoice_GetListCost(long inviID);
        #endregion
        #region 49.Inward ThanhTrung Med Dept
        public abstract List<OutwardDrugMedDeptInvoice> OutwardThanhTrungMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        public abstract IList<InwardDrugMedDeptInvoice> SearchInwardThanhTrungMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract InwardDrugMedDeptInvoice GetInwardThanhTrungMedDeptInvoice_ByID(long ID);
        public abstract int AddInwardThanhTrungMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);
        public abstract int UpdateInwardThanhTrungMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);
        public abstract int DeleteInwardThanhTrungMedDeptInvoice(long ID);

        public abstract IList<InwardDrugMedDept> GetInwardThanhTrungMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        public abstract IList<InwardDrugMedDept> GetInwardThanhTrungMedDept_ByIDInvoiceNotPaging(long inviID
             , out decimal TongTienSPChuaVAT
             , out decimal CKTrenSP
             , out decimal TongTienTrenSPDaTruCK
             , out decimal TongCKTrenHoaDon
             , out decimal TongTienHoaDonCoThueNK
             , out decimal TongTienHoaDonCoVAT);

        public abstract int UpdateInwardThanhTrungMedDept(InwardDrugMedDept invoicedrug, long StaffID);
        public abstract int DeleteInwardThanhTrungMedDept(long invoicedrug);

        public abstract bool DeleteInwardThanhTrungMedDeptTemp(long InID);

        public abstract bool AddInwardThanhTrungMedDept(DrugDeptPurchaseOrderDetail invoicedrug, long inviID);

        public abstract bool InwardThanhTrungMedDeptInvoice_UpdateCost(long inviID);

        public abstract bool UpdateStatusThanhTrungMedDeptPurchaseOrder(long DrugDeptPoID);

        public abstract int InwardThanhTrungMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        public abstract IList<CostTableMedDeptList> InwardThanhTrungMedDeptInvoice_GetListCost(long inviID);
        #endregion
        #region 50.Inward VPP Med Dept
        public abstract List<OutwardDrugMedDeptInvoice> OutwardVPPMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        public abstract IList<InwardDrugMedDeptInvoice> SearchInwardVPPMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract InwardDrugMedDeptInvoice GetInwardVPPMedDeptInvoice_ByID(long ID);
        public abstract int AddInwardVPPMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);
        public abstract int UpdateInwardVPPMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);
        public abstract int DeleteInwardVPPMedDeptInvoice(long ID);

        public abstract IList<InwardDrugMedDept> GetInwardVPPMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        public abstract IList<InwardDrugMedDept> GetInwardVPPMedDept_ByIDInvoiceNotPaging(long inviID
             , out decimal TongTienSPChuaVAT
             , out decimal CKTrenSP
             , out decimal TongTienTrenSPDaTruCK
             , out decimal TongCKTrenHoaDon
             , out decimal TongTienHoaDonCoThueNK
             , out decimal TongTienHoaDonCoVAT);

        public abstract int UpdateInwardVPPMedDept(InwardDrugMedDept invoicedrug, long StaffID);
        public abstract int DeleteInwardVPPMedDept(long invoicedrug);

        public abstract bool DeleteInwardVPPMedDeptTemp(long InID);

        public abstract bool AddInwardVPPMedDept(DrugDeptPurchaseOrderDetail invoicedrug, long inviID);

        public abstract bool InwardVPPMedDeptInvoice_UpdateCost(long inviID);

        public abstract bool UpdateStatusVPPMedDeptPurchaseOrder(long DrugDeptPoID);

        public abstract int InwardVPPMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        public abstract IList<CostTableMedDeptList> InwardVPPMedDeptInvoice_GetListCost(long inviID);
        #endregion
        #region AutoComplete Xuất khác
        public abstract List<RefGenMedProductDetails> GetVTYTTHForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient);
        public abstract List<RefGenMedProductDetails> GetBloodForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient);
        public abstract List<RefGenMedProductDetails> GetThanhTrungForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient);
        public abstract List<RefGenMedProductDetails> GetChemicalForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient);
        public abstract List<RefGenMedProductDetails> GetVPPForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient);
        public abstract List<RefGenMedProductDetails> GetTiemNguaForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient);
        public abstract List<RefGenMedProductDetails> GetVTTHForSellVisitorAutoComplete_ForRequestClinicDept(bool? IsCost, string BrandName, long StoreID, long V_MedProductType, long? RefGenDrugCatID_1, List<RequestDrugInwardClinicDept> RequestDrugList, bool? IsCode, long? PtRegistrationID, bool? IsHIPatient);

        #endregion

        #region 51.Inward VTTH Med Dept
        public abstract List<OutwardDrugMedDeptInvoice> OutwardVTTHMedDeptInvoice_Cbx(long? StoreID, long V_MedProductType);

        public abstract IList<InwardDrugMedDeptInvoice> SearchInwardVTTHMedDeptInvoice(InwardInvoiceSearchCriteria criteria, long? TypID, long? V_MedProductType, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);
        public abstract InwardDrugMedDeptInvoice GetInwardVTTHMedDeptInvoice_ByID(long ID);
        public abstract int AddInwardVTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug, out long inwardid);
        public abstract int UpdateInwardVTTHMedDeptInvoice(InwardDrugMedDeptInvoice InvoiceDrug);
        public abstract int DeleteInwardVTTHMedDeptInvoice(long ID);

        public abstract IList<InwardDrugMedDept> GetInwardVTTHMedDept_ByIDInvoice(long inviID, int pageSize, int pageIndex, bool bCountTotal, out int totalCount
            , out decimal TongTienSPChuaVAT
            , out decimal CKTrenSP
            , out decimal TongTienTrenSPDaTruCK
            , out decimal TongCKTrenHoaDon
            , out decimal TongTienHoaDonCoThueNK
            , out decimal TongTienHoaDonCoVAT
            , out decimal TotalVATDifferenceAmount);

        public abstract IList<InwardDrugMedDept> GetInwardVTTHMedDept_ByIDInvoiceNotPaging(long inviID
             , out decimal TongTienSPChuaVAT
             , out decimal CKTrenSP
             , out decimal TongTienTrenSPDaTruCK
             , out decimal TongCKTrenHoaDon
             , out decimal TongTienHoaDonCoThueNK
             , out decimal TongTienHoaDonCoVAT);

        public abstract int UpdateInwardVTTHMedDept(InwardDrugMedDept invoicedrug, long StaffID);
        public abstract int DeleteInwardVTTHMedDept(long invoicedrug);

        public abstract bool DeleteInwardVTTHMedDeptTemp(long InID);

        public abstract bool AddInwardVTTHMedDept(DrugDeptPurchaseOrderDetail invoicedrug, long inviID);

        public abstract bool InwardVTTHMedDeptInvoice_UpdateCost(long inviID);

        public abstract bool UpdateStatusVTTHMedDeptPurchaseOrder(long DrugDeptPoID);

        public abstract int InwardVTTHMedDeptInvoice_SaveXML(InwardDrugMedDeptInvoice InvoiceDrug, out long id);

        public abstract IList<CostTableMedDeptList> InwardVTTHMedDeptInvoice_GetListCost(long inviID);
        public abstract List<RequestDrugInwardClinicDept> SearchRequestDrugInwardClinicDept_NotPaging(long V_MedProductType, long StoreID);
        #endregion
    }
}
