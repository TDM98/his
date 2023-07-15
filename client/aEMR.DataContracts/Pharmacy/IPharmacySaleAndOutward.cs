using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using DataEntities;

namespace PharmacySaleAndOutwardProxy
{
    [ServiceContract]
    public interface IPharmacySaleAndOutward
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginTestMethod1(long supplierID, AsyncCallback callback, object state);
        bool EndTestMethod1(out string strOutMsg, IAsyncResult asyncResult);

        #region Update & Delete Inward-Drugs From Supplier or Non-Supplier

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInwardDrug_Update_Pst(InwardDrug invoicedrug, long StaffID, AsyncCallback callback, object state);
        int EndInwardDrug_Update_Pst(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardDrug_Pst(long invoicedrug, AsyncCallback callback, object state);
        int EndDeleteInwardDrug_Pst(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginDeleteInwardInvoiceDrug_Pst(long ID, AsyncCallback callback, object state);
        int EndDeleteInwardInvoiceDrug_Pst(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddInwardDrug_Pst(InwardDrug invoicedrug, AsyncCallback callback, object state);
        bool EndAddInwardDrug_Pst(IAsyncResult asyncResult);

        #endregion

        #region Sales without Prescription

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugInvoice_SaveByType_Pst(OutwardDrugInvoice Invoice, AsyncCallback callback, object state);
        bool EndOutwardDrugInvoice_SaveByType_Pst(out long ID, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutWardDrugInvoiceVisitor_Cancel_Pst(OutwardDrugInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndOutWardDrugInvoiceVisitor_Cancel_Pst(out long TransItemID, IAsyncResult asyncResult);

        #endregion

        #region Sales with Prescription

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveDrugs_Pst(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice OutwardInvoice, AsyncCallback callback, object state);
        bool EndSaveDrugs_Pst(out OutwardDrugInvoice SavedOutwardInvoice, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginSaveDrugs_Pst_V2(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice OutwardInvoice, AsyncCallback callback, object state);
        bool EndSaveDrugs_Pst_V2(out OutwardDrugInvoice SavedOutwardInvoice, out IList<OutwardDrug> SavedOutwardDrugs, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutWardDrugInvoicePrescriptChuaThuTien_Cancel_Pst(OutwardDrugInvoice InvoiceDrug, AsyncCallback callback, object state);
        int EndOutWardDrugInvoicePrescriptChuaThuTien_Cancel_Pst(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginCancelOutwardDrugInvoice_Pst(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice invoice, long? V_TradingPlaces, AsyncCallback callback, object state);
        bool EndCancelOutwardDrugInvoice_Pst(IAsyncResult asyncResult);

        #endregion

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginListDrugExpiryDate_Save_Pst(OutwardDrugInvoice Invoice, AsyncCallback callback, object state);
        bool EndListDrugExpiryDate_Save_Pst(out long ID, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutWardDrugInvoice_Delete_Pst(long id, AsyncCallback callback, object state);
        int EndOutWardDrugInvoice_Delete_Pst(IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdateInvoicePayed_Pst(OutwardDrugInvoice Outward, AsyncCallback callback, object state);
        bool EndUpdateInvoicePayed_Pst(out long outiID, out long PaymentID, out string StrError, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddOutwardDrugReturn_Pst(long StaffID, int? Apply15HIPercent, OutwardDrugInvoice Outward, List<OutwardDrug> Details, AsyncCallback callback, object state);
        bool EndAddOutwardDrugReturn_Pst(out long outiID, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginAddOutWardDrugInvoiceReturnVisitor_Pst(OutwardDrugInvoice InvoiceDrug, long ReturnStaffID, AsyncCallback callback, object state);
        bool EndAddOutWardDrugInvoiceReturnVisitor_Pst(out long outwardid, IAsyncResult asyncResult);


        #region
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginOutwardDrugInvoice_SaveByType_Balance(OutwardDrugInvoice Invoice, int ViewCase, AsyncCallback callback, object state);
        bool EndOutwardDrugInvoice_SaveByType_Balance(out long ID, out string StrError, IAsyncResult asyncResult);
        #endregion
    }
}
