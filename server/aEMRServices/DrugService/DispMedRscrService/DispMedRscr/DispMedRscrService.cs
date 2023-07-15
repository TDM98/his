using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;
using ErrorLibrary;
using System.Runtime.Serialization;
using AxLogging;
using ErrorLibrary.Resources;
using eHCMSLanguage;
using aEMR.DataAccessLayer.Providers;

namespace DispMedRscrService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class DispMedRscrService : eHCMS.WCFServiceCustomHeader, IDispMedRscr
    {
        public DispMedRscrService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }
        public IList<InwardDMedRscrInvoice> GetAllInwardDMedRscrInvoice(int V_Reason, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return DispMedRscrProviders.Instance.GetAllInwardDMedRscrInvoice(V_Reason, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetAllInwardDMedRscrInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDITEM_CANNOT_LOAD);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<InwardDMedRscrInvoice> SearchInwardDMedRscrInvoice(int V_Reason, InwardInvoiceSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return DispMedRscrProviders.Instance.SearchInwardDMedRscrInvoice(V_Reason, criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchInwardDMedRscrInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDITEM_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public InwardDMedRscrInvoice GetInwardDMedRscrInvoiceByID(int V_Reason, long InvDMedRscrID)
        {
            try
            {
                return DispMedRscrProviders.Instance.GetInwardDMedRscrInvoiceByID(V_Reason, InvDMedRscrID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetInwardDMedRscrInvoiceByID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDITEM_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool AddInwardDMedRscrInvoice(InwardDMedRscrInvoice InvoiceDrug, out long inwardid)
        {
            try
            {
                return DispMedRscrProviders.Instance.AddInwardDMedRscrInvoice(InvoiceDrug, out inwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddInwardDMedRscrInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDITEM_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdateInwardDMedRscrInvoice(InwardDMedRscrInvoice InvoiceDrug)
        {
            try
            {
                return DispMedRscrProviders.Instance.UpdateInwardDMedRscrInvoice(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateInwardDMedRscrInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDITEM_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteInwardDMedRscrInvoice(long ID)
        {
            try
            {
                return DispMedRscrProviders.Instance.DeleteInwardDMedRscrInvoice(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteInwardDMedRscrInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.MEDITEM_CANNOT_DELETE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
    }
}
