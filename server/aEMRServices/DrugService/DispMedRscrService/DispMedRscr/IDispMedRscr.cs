using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataEntities;
using eHCMS.Services.Core;
using ErrorLibrary;

namespace DispMedRscrService
{
    [ServiceContract]
    public interface IDispMedRscr
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDMedRscrInvoice> GetAllInwardDMedRscrInvoice(int V_Reason, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        IList<InwardDMedRscrInvoice> SearchInwardDMedRscrInvoice(int V_Reason, InwardInvoiceSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        InwardDMedRscrInvoice GetInwardDMedRscrInvoiceByID(int V_Reason, long InvDMedRscrID);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool AddInwardDMedRscrInvoice(InwardDMedRscrInvoice InvoiceDrug, out long inwardid);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool UpdateInwardDMedRscrInvoice(InwardDMedRscrInvoice InvoiceDrug);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool DeleteInwardDMedRscrInvoice(long ID);

      
    }
}
