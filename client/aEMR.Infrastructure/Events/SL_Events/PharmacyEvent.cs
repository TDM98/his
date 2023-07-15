/*
 * 201470803 #001 CMN: Add HI Store Service
*/
using DataEntities;
namespace aEMR.Infrastructure.Events
{
    public class PharmacyCloseAddGenDrugEvent
    {
    }
    public class PharmacyCloseFinishAddGenDrugEvent
    {
        public object SupplierDrug { get; set; }
    }

    public class PharmacyCloseFinishAddSupplierEvent
    {
        public object SelectedSupplier { get; set; }
    }

    public class PharmacyCloseEditGenDrugEvent
    {

    }
    public class PharmacyCloseAddSupplierEvent
    {

    }
    public class PharmacyCloseEditSupplierEvent
    { }

    public class PharmacyCloseSearchDrugEvent
    {
        public object SupplierDrug { get; set; }
    }
    public class PharmacyCloseSearchSupplierEvent
    {
        public object SelectedSupplier { get; set; }
    }
    public class PharmacySupplierToEstimationEvent
    {
        public object SelectedSupplier { get; set; }
    }
    public class PharmacyCloseSearchVisitorEvent
    {
        public object SelectedOutwardInfo { get; set; }
    }
    public class PharmacyCloseSearchPrescriptionEvent
    {
        public object SelectedPrescription { get; set; }
    }
    public class PharmacyCloseSearchEditPrescriptionEvent
    {
        public object SelectedPrescription { get; set; }
    }
    public class PharmacyCloseSearchPrescriptionInvoiceEvent
    {
        public object SelectedInvoice { get; set; }
    }
    public class PharmacyCloseCollectionMultiEvent
    {
    }
    public class PharmacyCloseSearchReturnEvent
    {
        public object SelectedInvoice { get; set; }
    }
    public class PharmacyCloseSearchReturnInvoiceEvent
    {
        public object SelectedInvoice { get; set; }
    }
    public class PharmacyCloseSearchEstimationEvent
    {
        public object SelectedEstimation { get; set; }
    }
    public class PharmacyCloseSearchPurchaseOrderEstimationEvent
    {
        public object SelectedEstimation { get; set; }
    }

    public class PharmacyCloseSearchPurchaseOrderEvent
    {
        public object SelectedPurchaseOrder { get; set; }
    }

    public class PharmacyCloseSearchInwardIncoiceEvent
    {
        public object SelectedInwardInvoice { get; set; }
    }
    public class PharmacyCloseEditInwardEvent
    {
    }
    public class PharmacyContraIndicatorEvent
    {
        public object PharmacyContraIndicator { get; set; }
    }
    public class PharmacyContraIndicatorAddEvent
    {
        public object PharmacyContraIndicatorAdd { get; set; }
    }
    public class PharContraEvent
    {
        //public PharContraEvent(object _lstPharContra, object _lstPharContraName)
        //{
        //    lstPharContra = _lstPharContra;
        //    lstPharContraName = _lstPharContraName;
        //}
        public object lstPharContra{ get; set; }
        public object lstPharContraName { get; set; }
        public EntitiesEdit<RefMedContraIndicationTypes> refMedicalConditionType_Edit { get; set; }
    }

    public class MedDeptContraEvent
    {
        //public PharContraEvent(object _lstPharContra, object _lstPharContraName)
        //{
        //    lstPharContra = _lstPharContra;
        //    lstPharContraName = _lstPharContraName;
        //}
        public object lstPharContra { get; set; }
        public object lstPharContraName { get; set; }
        public EntitiesEdit<RefMedContraIndicationTypes> refMedicalConditionType_Edit { get; set; }
    }
    public class V_RouteOfAdministrationEvent
    {
        //public PharContraEvent(object _lstPharContra, object _lstPharContraName)
        //{
        //    lstPharContra = _lstPharContra;
        //    lstPharContraName = _lstPharContraName;
        //}
        //public object lstPharContra { get; set; }
        //public object lstPharContraName { get; set; }
        public EntitiesEdit<Lookup> V_RouteOfAdministration_Edit { get; set; }
    }
    
    public class ChooseBatchNumberVisitorEvent
    {
        public GetDrugForSellVisitor BatchNumberVisitorSelected { get; set; }
    }
    public class EditChooseBatchNumberVisitorEvent
    {
        public GetDrugForSellVisitor BatchNumberVisitorSelected { get; set; }
    }
    public class ChooseBatchNumberVisitorResetQtyEvent
    {
        public GetDrugForSellVisitor BatchNumberVisitorSelected { get; set; }
    }
    public class EditChooseBatchNumberVisitorResetQtyEvent
    {
        public GetDrugForSellVisitor BatchNumberVisitorSelected { get; set; }
    }

    public class PharmacyCloseSearchStockTakesEvent
    {
        public object SelectedPharmacyStockTakes { get; set; }
    }
    public class PharmacyCloseSearchDemageDrugEvent
    {
        public object SelectedOutwardDrugInvoice { get; set; }
    }
    public class PharmacyCloseSearchOutReportDrugEvent
    {
        public object SelectedPharmacyOutwardDrugReport { get; set; }
    }
    public class PharmacyCloseSearchRequestEvent
    {
        public object SelectedRequest { get; set; }
    }
    public class PharmacyCloseSearchSupplierPharmacyPaymentReqEvent
    {
        public object SelectedPaymentReq { get; set; }
    }

    public class PharmacyPayEvent
    {
        public PatientTransactionPayment CurPatientPayment { get; set; }
    }
    public class PharmacyCloseFormReturnEvent
    {

    }

    public class PharmacyCloseEditPayed
    {
        public OutwardDrugInvoice SelectedOutwardInvoice { get; set; }
    }

    public class PharmacyCloseEditPayedPrescription
    {
        public OutwardDrugInvoice SelectedOutwardInvoice { get; set; }
    }

    public class PharmacyCloseSearchPharmaceuticalCompanyEvent
    {
        public object SelectedPharmaceuticalCompany { get; set; }
    }

    public class PharmaceuticalCompanyToEstimationEvent
    {
        public object SelectedPharmaceuticalCompany { get; set; }
    }

    public class EventSaveRefItemPriceSuccess
    {
    }
    //==== #001
    public class DeletedOutwardDrug
    {
        public OutwardDrug DeleteOutwardDrug { get; set; }
    }
    public class ChangedQtyOutwardDrug
    {
        public OutwardDrug DeleteOutwardDrug { get; set; }
    }
    //==== #001
    public class ElectronicPrescriptionPharmacyDeleteEvent
    {
        public object DeleteElectronicPrescriptionPharmacy { get; set; }
    }
}

