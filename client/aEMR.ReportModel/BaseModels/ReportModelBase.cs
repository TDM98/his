/*
 * 20170413 #001 CMN: Xóa log cho mẫu 02 và receipt
*/
using System;
using System.Windows;
using System.Configuration;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using System.Linq;
using DevExpress.DocumentServices.ServiceModel;
using DevExpress.Xpf.Printing;
using aEMR.Common;
using System.Threading;
using aEMR.ServiceClient;
using DataEntities;
using DevExpress.ReportServer.Printing;
using DevExpress.DocumentServices.ServiceModel.Client;
using System.ServiceModel;

namespace aEMR.ReportModel.BaseModels
{


    public class ReportModelBase
    {
        //KMx: Đổi từ class ReportPreviewModel sang ReportPreviewModelExt để Override hàm CreateDocument() (11/05/2016 16:50).

        readonly ReportPreviewModelExt _previewModel;

        public ReportPreviewModelExt PreviewModel
        {
            get { return _previewModel; }
        }

        protected ReportModelBase(string reportTypeName)
        {
            //_previewModel = CreatePreviewModel();
            _previewModel = new ReportPreviewModelExt();
            //PreviewModel.ReportTypeName = reportTypeName;
            PreviewModel.ReportName = reportTypeName;
        }

        //protected virtual ReportPreviewModelExt CreatePreviewModel()
        //{
        //    //return new ReportPreviewModel(SLResxConfigurationManager.AppSettings["reportServiceUrl"]);
        //    return new ReportPreviewModelExt(ConfigurationSettings.AppSettings["reportServiceUrl"]);                   
        //}


        //readonly ReportPreviewModel _previewModel;

        //public ReportPreviewModel PreviewModel
        //{
        //    get { return _previewModel; }
        //}

        //protected ReportModelBase(string reportTypeName)
        //{
        //    _previewModel = CreatePreviewModel();
        //    //PreviewModel.ReportTypeName = reportTypeName;
        //    PreviewModel.ReportName = reportTypeName;
        //}

        //protected virtual ReportPreviewModel CreatePreviewModel()
        //{
        //    //return new ReportPreviewModel(SLResxConfigurationManager.AppSettings["reportServiceUrl"]);

        //    return new ReportPreviewModel(ClientConfigurationManager.AppSettings["reportServiceUrl"]);
        //}
    }


    public class ReportPreviewModelExt : RemoteDocumentSource
    {
        //public ReportPreviewModelExt(string serviceUri)
        //    : base(serviceUri)
        public ReportPreviewModelExt()            
        {
            base.ReportServiceClientDemanded += create_ReportServiceClientDemanded;
        }

        protected virtual void create_ReportServiceClientDemanded(object sender, ReportServiceClientDemandedEventArgs e)
        {
            e.Client = new ReportServiceClientFactory(new EndpointAddress(ConfigurationSettings.AppSettings["reportServiceUrl"])).Create();
        }

        private bool IsLogReport()
        {
            if (ReportName == "eHCMS.ReportLib.RptPatientRegistration.XRptOutPatientReceiptXML_V2"
                || ReportName == "eHCMS.ReportLib.RptPatientRegistration.BHYT_NgoaiTru"
                || ReportName == "eHCMS.ReportLib.RptTransactions.XRpt_Temp38NgoaiTru"
                || ReportName == "eHCMS.ReportLib.RptPatientRegistration.BHYT_NoiTru"
                || ReportName == "eHCMS.ReportLib.RptPatientRegistration.XRpt_InPatientBillingInvoiceNew"
                || ReportName == "eHCMS.ReportLib.RptPatientRegistration.XRptPhieuDeNghiTamUng"
                || ReportName == "eHCMS.ReportLib.RptPatientRegistration.XRptPatientCashAdvance"
                || ReportName == "eHCMS.ReportLib.RptPatientRegistration.XRptPhieuDeNghiThanhToan"
                || ReportName == "eHCMS.ReportLib.RptPatientRegistration.XRptRepayCashAdvance"
                || ReportName == "eHCMS.ReportLib.RptPatientRegistration.XRptPatientSettlement_V2"
                || ReportName == "eHCMS.ReportLib.RptTransactions.XRpt_Temp38NoiTru"
                || ReportName == "eHCMS.ReportLib.RptTransactions.XRpt_Form02NoiTru"
                || ReportName == "eHCMS.ReportLib.InPatient.Reports.XrptDischargePaper"
                || ReportName == "eHCMS.ReportLib.RptPatientRegistration.XRptOutPatientReceipt_V2"
                || ReportName == "eHCMS.ReportLib.RptConsultations.XRptEPrescriptionNew_V2"
                /*TMA 23/11/2017 BÁO CÁO SUBREPORT*/
                || ReportName == "eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescription_V2_SubReport"
                /*TMA 23/11/2017 BÁO CÁO SUBREPORT*/

                || ReportName == "eHCMS.ReportLib.RptConsultations.XRptEPrescriptionNew_InPt_V2"

                /*TMA 22/11/2017 BÁO CÁO SUBREPORT*/
                || ReportName == "eHCMS.ReportLib.RptConsultations.XtraReports.XRptEPrescriptionInpt_V2_SubReport"
                /*TMA 22/11/2017 BÁO CÁO SUBREPORT*/
                || ReportName == "eHCMS.ReportLib.RptAppointment.XRptPatientApptServiceDetails"
                || ReportName == "eHCMS.ReportLib.RptAppointment.XRptPatientApptPCLRequestsCombo"
                || ReportName == "eHCMS.ReportLib.RptPatientRegistration.XRptHIAppointment"
                || ReportName == "eHCMS.ReportLib.RptConsultations.XRptPatientPCLRequestDetailsByPatientPCLReqID"
                || ReportName == "eHCMS.ReportLib.RptPharmacies.PhieuNhanThuoc"
                || ReportName == "eHCMS.ReportLib.RptPharmacies.PhieuNhanThuocBH"
                || ReportName == "eHCMS.ReportLib.RptPharmacies.XptReturnDrugInsurance"
                || ReportName == "eHCMS.ReportLib.RptPharmacies.XptReturnDrug"
                || ReportName == "eHCMS.ReportLib.RptPharmacies.XtraReports.Order.XRpt_PurchaseOrderPharmacy"
                || ReportName == "eHCMS.ReportLib.RptPharmacies.XRptInwardDrugSupplier"
                || ReportName == "eHCMS.ReportLib.RptPharmacies.XRpt_XuatNoiBo"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestDrugDept"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestDrugDeptDetails"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestDrugDeptApproved"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Request.XRptRequestDrugDeptDetailApproved"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutInternalDrugDeptToClinicDept"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutInternalDrugDept"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutDemageDrugDept"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.OutwardFromPrescription.XRptOutwardFromPrescription"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.OutInternal.XRptOutDrugDeptAddictive"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardMedDeptSupplier"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Returns.XptReturnMedDept"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Estimation.XRptEstimateDrugDept"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Estimation.XRptEstimateDrugDeptKeToan"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Estimation.XRptEstimateDrugDeptThuKho"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Order.XRpt_PurchaseOrderDrugDept"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardMedDeptSupplierTrongNuoc"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardMedDeptSupplier"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardMedDeptFromClinicDept"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardCostTable"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.SupplierDrugDeptPaymentReqs.SupplierDrugDeptPaymentReqs"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.SupplierDrugDeptPaymentReqs.PhieuDeNghiThanhToan"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptInwardFromMedDeptForClinicDept"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward.XRptFromClinicDeptToDrugDept"
                || ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Outward.XRptFromClinicDeptToPatient"
                /*==== #001 ====*/
                || ReportName == "eHCMS.ReportLib.RptPatientRegistration.XRptOutPatientReceipt_V4"
                || ReportName == "eHCMS.ReportLib.RptTransactions.XRpt_Temp02NoiTruNew"
                /*==== #001 ====*/

                ///*TMA 23/10/2017*/
                //|| ReportName == "eHCMS.ReportLib.RptDrugDepts.XtraReports.Inwards.XRptDrugMedDept"
                ///*TMA 23/10/2017*/
                )
            {
                return false;
            }
            return true;
        }

    }
}
