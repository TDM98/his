using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataEntities;
using eHCMS.Services.Core;
using ErrorLibrary;

namespace PharmacyService
{
    [ServiceContract]                     
    public interface IPharmacySaleAndOutward
    {
        
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool TestMethod1(long nInItemID, out string strOutMsg);

        #region Update & Delete Inward-Drugs From Supplier or Non-Supplier 

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int InwardDrug_Update_Pst(InwardDrug invoicedrug, long StaffID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardDrug_Pst(long invoicedrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int DeleteInwardInvoiceDrug_Pst(long ID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddInwardDrug_Pst(InwardDrug invoicedrug);

        #endregion

        #region Sales without Prescription

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutwardDrugInvoice_SaveByType_Pst(OutwardDrugInvoice Invoice, out long ID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int OutWardDrugInvoiceVisitor_Cancel_Pst(OutwardDrugInvoice InvoiceDrug, out long TransItemID);

        #endregion

        #region Sales with Prescription

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveDrugs_Pst(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice OutwardInvoice, out OutwardDrugInvoice SavedOutwardInvoice);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool SaveDrugs_Pst_V2(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice OutwardInvoice, out OutwardDrugInvoice SavedOutwardInvoice, out IList<OutwardDrug> SavedOutwardDrugs);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int OutWardDrugInvoicePrescriptChuaThuTien_Cancel_Pst(OutwardDrugInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool CancelOutwardDrugInvoice_Pst(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice invoice, long? V_TradingPlaces);

        #endregion

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool ListDrugExpiryDate_Save_Pst(OutwardDrugInvoice Invoice, out long ID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        int OutWardDrugInvoice_Delete_Pst(long id);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateInvoicePayed_Pst(OutwardDrugInvoice Outward, out long outiID, out long PaymentID, out string StrError);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddOutwardDrugReturn_Pst(long StaffID, int? Apply15HIPercent, OutwardDrugInvoice Outward, List<OutwardDrug> Details, out long outiID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddOutWardDrugInvoiceReturnVisitor_Pst(OutwardDrugInvoice InvoiceDrug, long ReturnStaffID, out long outwardid);

        #region
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool OutwardDrugInvoice_SaveByType_Balance(OutwardDrugInvoice Invoice, int ViewCase, out long ID, out string StrError);
        #endregion
    }
}
