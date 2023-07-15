using DataEntities;

namespace aEMR.Infrastructure.Events
{
    public class ClinicDeptInPtSelReqFormForOutward
    {
        public object SelectedReqForm { get; set; }
    }

    public class DrugDeptCloseSearchRequestEvent
    {
        public object SelectedRequest { get; set; }
        public bool IsCreateNewFromExisting { get; set; }
    }
    //20181219 TTM: Tạo sự kiện để tìm kiếm phiếu lĩnh kho BHYT - Nhà thuốc sẽ bắn dữ liệu từ popup vào
    //              Lập phiếu lĩnh, duyệt và xuất.
    public class DrugDeptCloseSearchRequestForHIStoreEvent
    {
        public object SelectedRequest { get; set; }
        public bool IsCreateNewFromExisting { get; set; }
    }
    public class DrugDeptCloseSearchOutMedDeptInvoiceEvent
    {
        public object SelectedOutMedDeptInvoice { get; set; }
    }

    public class DrugDeptCloseEditMedDeptInvoiceEvent
    {
        public OutwardDrugMedDeptInvoice SelectedOutMedDeptInvoice { get; set; }
    }

    public class DrugDeptCloseEditPayedEvent
    {
        public OutwardDrugMedDeptInvoice SelectedOutMedDeptInvoice { get; set; }
    }
    public class DrugDeptCloseSearchOutClinicDeptInvoiceEvent
    {
        public object SelectedOutClinicDeptInvoice { get; set; }
    }
    public class DrugDeptCloseAddSupplierEvent
    {

    }


    public class DrugDeptCloseSearchSupplierDrugDeptPaymentReqEvent
    {
        public object SelectedPaymentReq { get; set; }
    }

    public class DrugDeptCloseFinishAddSupplierEvent
    {
        public object SelectedSupplier { get; set; }
    }
    public class DrugDeptCloseEditSupplierEvent
    { }

    public class DrugDeptCloseEditInwardEvent
    {
    }
    public class DrugDeptLoadAgainReqOutwardClinicDeptEvent
    {
        public long RequestID { get; set; }
    }
    public class ClinicDrugDeptCloseEditInwardEvent
    {
    }
    public class DrugDeptCloseSearchSupplierEvent
    {
        public object SelectedSupplier { get; set; }
    }

    public class DrugDeptCloseSearchSupplierEvent_V1
    {
        public DrugDeptSupplier SelectedSupplier { get; set; }
    }

    public class DrugDeptCloseSearchSupplierEvent_V2
    {
        public DrugDeptSupplier SelectedSupplier { get; set; }
    }


    public class DrugDeptCloseSearchPharmaceuticalCompanyEvent
    {
        public object SelectedPharmaceuticalCompany { get; set; }
    }

    public class DrugDeptCloseSearchStorageEvent
    {
        public object SelectedStorage { get; set; }
    }

    public class DrugDeptCloseSearchHospitalEvent
    {
        public object SelectedHospital { get; set; }
    }
    public class DrugDeptCloseSearchStaffEvent
    {
        public object SelectedStaff { get; set; }
    }

    public class DrugDeptCloseSearchInwardIncoiceEvent
    {
        public object SelectedInwardInvoice { get; set; }
    }
    public class DrugDeptCloseSearchInwardCostListEvent
    {
        public object SelectedInwardInvoice { get; set; }
    }
    public class ClinicDrugDeptCloseSearchInwardIncoiceEvent
    {
        public object SelectedInwardInvoice { get; set; }
    }
   
    public class ClinicDeptChooseBatchNumberResetQtyEvent
    {
        public RefGenMedProductDetails BatchNumberSelected { get; set; }
    }
    public class ClinicDeptEditChooseBatchNumberResetQtyEvent
    {
        public RefGenMedProductDetails BatchNumberSelected { get; set; }
    }
    public class ClinicDeptChooseBatchNumberEvent
    {
        public RefGenMedProductDetails BatchNumberSelected { get; set; }
    }

    public class ClinicDeptEditChooseBatchNumberEvent
    {
        public RefGenMedProductDetails BatchNumberSelected { get; set; }
    }

    public class DrugDeptChooseBatchNumberResetQtyEvent
    {
        public RefGenMedProductDetails BatchNumberSelected { get; set; }
    }
    public class DrugDeptEditChooseBatchNumberResetQtyEvent
    {
        public RefGenMedProductDetails BatchNumberSelected { get; set; }
    }
    public class DrugDeptChooseBatchNumberEvent
    {
        public RefGenMedProductDetails BatchNumberSelected { get; set; }
    }
    public class DrugDeptEditChooseBatchNumberEvent
    {
        public RefGenMedProductDetails BatchNumberSelected { get; set; }
    }
    public class DrugDeptCloseSearchEstimationEvent
    {
        public object SelectedEstimation { get; set; }
    }
    public class DrugDeptCloseSearchDrugEvent
    {
        public object SupplierDrug { get; set; }
    }
    public class DrugDeptCloseSearchPurchaseOrderEvent
    {
        public object SelectedPurchaseOrder { get; set; }
    }
    public class DrugDeptCloseSearchPurchaseOrderEstimationEvent
    {
        public object SelectedEstimation { get; set; }
    }
    public class DrugDeptCloseSearchDemageDrugEvent
    {
        public object SelectedOutwardDrugMedDeptInvoice { get; set; }
    }

    public class DrugDeptCloseSearchStockTakesEvent
    {
        public object SelectedDrugDeptStockTakes { get; set; }
    }
    public class ClinicDeptCloseSearchStockTakesEvent
    {
        public object SelectedClinicDeptStockTakes { get; set; }
    }

    public class MedDeptCloseSearchPrescriptionEvent
    {
        public object SelectedPrescription { get; set; }
    }

    public class MedDeptCloseSearchPrescriptionInvoiceEvent
    {
        public object SelectedInvoice { get; set; }
    }

    public class MedDeptCloseEditPrescription
    {
        public OutwardDrugMedDeptInvoice SelectedOutwardInvoice { get; set; }
    }

    public class ChooseBatchNumberForPrescriptionEvent
    {
        public GetGenMedProductForSell BatchNumberVisitorSelected { get; set; }
    }
    public class EditChooseBatchNumberForPrescriptionEvent
    {
        public GetGenMedProductForSell BatchNumberVisitorSelected { get; set; }
    }
    public class ChooseBatchNumberForPrescriptionResetQtyEvent
    {
        public GetGenMedProductForSell BatchNumberVisitorSelected { get; set; }
    }
    public class EditChooseBatchNumberForPrescriptionResetQtyEvent
    {
        public GetGenMedProductForSell BatchNumberVisitorSelected { get; set; }
    }
    public class ChooseDrugClass
    {
        public TherapyTree ParentSelected { get; set; }
        public TherapyTree ChildrenSelected { get; set; }
    }
    public class ReloadInPatientRequestingDrugListByReqID
    {

    }
}
