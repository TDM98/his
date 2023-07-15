/*
 * 201470803 #001 CMN: Add HI Store Service
*/
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataEntities;
using eHCMS.Configurations;
using eHCMS.Services.Core;
using ErrorLibrary;
using System.Runtime.Serialization;
using AxLogging;
using ErrorLibrary.Resources;
using System.Data.SqlClient;
using eHCMSLanguage;
using aEMR.DataAccessLayer.Providers;

namespace PharmacyService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.Single)]
    [KnownType(typeof(AxException))]
    public class SaleAndOutward : eHCMS.WCFServiceCustomHeader, IPharmacySaleAndOutward
    {
        static int nObjInstCnt = 0;
        object TheLock = new object();
        int nInstID = 0;
        public SaleAndOutward()
        {
            nInstID = ++nObjInstCnt;
            //DateTime curTime = DateTime.Now;            
            //int curThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            //System.Diagnostics.Debug.WriteLine("[{0}] ====> PharmacyService.SaleAndOutward Object CREATED Instance Number = {1} ON THREAD = {2}.", curTime.ToString("dd/MM/yyyy h:mm:ss.ff"), ++nObjInstCnt, curThreadID);
        }

        public bool TestMethod1(long nInItemID, out string strOutMsg)
        {
            int curThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            DateTime curTime = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("[{0}] ====> BEGIN PharmacyService.SaleAndOutward TestMethod1 Instance Number = {1} ON THREAD = {2}.", curTime.ToString("dd/MM/yyyy h:mm:ss.ff"), nInstID, curThreadID);
            strOutMsg = "====> PharmacyService.SaleAndOutward SERVICE TestMethod1 Instance Number = {" + nInstID.ToString() + "} ON SERVICE THREAD = {" + curThreadID.ToString() + "}";
            System.Threading.Thread.Sleep(200);
            curTime = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("[{0}] ====> END PharmacyService.SaleAndOutward TestMethod1 Instance Number = {1} ON THREAD = {2}.", curTime.ToString("dd/MM/yyyy h:mm:ss.ff"), nInstID, curThreadID);
            return true;
        }

        #region Update & Delete Inward-Drugs From Supplier and Non-Supplier

        public int InwardDrug_Update_Pst(InwardDrug invoicedrug, long StaffID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.InwardDrug_Update(invoicedrug, StaffID);
            }
            catch (SqlException ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving InwardDrug_Update. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARD_CANNOT_UPDATE);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public int DeleteInwardDrug_Pst(long invoicedrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardDrug(invoicedrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving DeleteInwardDrug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARD_CANNOT_DELETE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public int DeleteInwardInvoiceDrug_Pst(long ID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.DeleteInwardDrugInvoice(ID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateInwardInvoiceDrug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARDINV_CANNOT_DELETE);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddInwardDrug_Pst(InwardDrug invoicedrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddInwardDrug(invoicedrug);
            }
            catch (SqlException ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving InwardDrug_Insert. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_INWARD_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #endregion

        #region Sales without Prescription

        public bool OutwardDrugInvoice_SaveByType_Pst(OutwardDrugInvoice Invoice, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugInvoice_SaveByType(Invoice, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutwardDrugInvoice_SaveByType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_OutwardDrugInvoice_SaveByType);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool UpdateInvoicePayed_Pst(OutwardDrugInvoice Outward, out long outiID, out long PaymentID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.UpdateInvoicePayed(Outward, out outiID, out PaymentID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateInvoicePayed. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_LOAD);


                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public int OutWardDrugInvoiceVisitor_Cancel_Pst(OutwardDrugInvoice InvoiceDrug, out long TransItemID)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutWardDrugInvoiceVisitor_Cancel(InvoiceDrug, out TransItemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutWardDrugInvoiceVisitor_Cancel. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_OutWardDrugInvoiceVisitor_Cancel);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #endregion


        public bool SaveDrugs_Pst(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice OutwardInvoice, out OutwardDrugInvoice SavedOutwardInvoice)
        {
            IList<OutwardDrug> SavedOutwardDrugs = new List<OutwardDrug>();
            return SaveDrugs_Pst_V2(StaffID, CollectorDeptLocID, Apply15HIPercent, OutwardInvoice, out SavedOutwardInvoice, out SavedOutwardDrugs);
        }
        public bool SaveDrugs_Pst_V2(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice OutwardInvoice, out OutwardDrugInvoice SavedOutwardInvoice, out IList<OutwardDrug> SavedOutwardDrugs)
        {
            try
            {
                //==== #001
                bool HI = OutwardInvoice.IsOutHIPt && Globals.AxServerSettings.CommonItems.EnableHIStore;
                //==== #001
                SavedOutwardInvoice = null;
                long registrationID = -1;
                if (OutwardInvoice.PtRegistrationID.HasValue && OutwardInvoice.PtRegistrationID.Value > 0)
                {
                    registrationID = OutwardInvoice.PtRegistrationID.Value;
                }
                if (registrationID == -1)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1694_G1_ChuaChonDKChoToa));
                }

                PatientRegistration registrationInfo = eHCMSBillPaymt.RegAndPaymentProcessorBase.GetRegistrationTxd(registrationID, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);//chi su dung cho ngoai tru

                //PatientRegistration registrationInfo = PatientProvider.Instance.GetRegistration(registrationID
                //    , (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);//chi su dung cho ngoai tru

                if (registrationInfo == null)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1695_G1_KgTimThayDKCuaToa));
                }

                eHCMSBillPaymt.RegAndPaymentProcessorBase paymentProcessor = eHCMSBillPaymt.RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.InitNewTxd(registrationInfo, false);
                List<long> newInvoiceIDList;
                paymentProcessor.AddOutwardDrugInvoice(StaffID, CollectorDeptLocID, Apply15HIPercent, OutwardInvoice, out newInvoiceIDList);

                SavedOutwardDrugs = null;

                if (newInvoiceIDList != null && newInvoiceIDList.Count > 0)
                {
                    long? newOutiID = newInvoiceIDList[0];
                    if (newOutiID == null)
                    {
                        // Something MUST BE VERY WRONG HERE, JUST throw an EXCEPTION
                        throw new Exception(string.Format("{0}.", eHCMSResources.Z1843_G1_LuuPhXuatKgTraVeGTriOutID));
                    }
                    //KMx: Thay vì AddOutwardDrugInvoice lưu phiếu xuất xuống CSDL rồi lấy phiếu lên lại, nhưng chỉ lấy outiID lên thôi, dẫn đến việc thiếu thông tin.
                    //      Nên phải dùng outiID xuống CSDL 1 lần nữa để lấy thông tin lên (tức là xuống CSDL lần 2).
                    //      A.Tuấn dặn khi nào có time thì sửa lại stored sp_AddUpdateServiceForRegistration_New để xuống CSDL 1 lần thôi. 
                    //==== #001
                    if (HI)
                    {
                        OutwardInvoice = RefDrugGenericDetailsProvider.Instance.GetHIOutWardDrugInvoiceByID(newOutiID);
                        SavedOutwardInvoice = OutwardInvoice;
                        SavedOutwardInvoice.OutwardDrugs = RefDrugGenericDetailsProvider.Instance.GetHIOutwardDrugDetailsByOutwardInvoice(OutwardInvoice.outiID).ToObservableCollection();

                    }
                    else
                    {
                        OutwardInvoice = RefDrugGenericDetailsProvider.Instance.GetOutWardDrugInvoiceByID(newOutiID);
                        SavedOutwardInvoice = OutwardInvoice;
                        SavedOutwardInvoice.OutwardDrugs = RefDrugGenericDetailsProvider.Instance.GetOutwardDrugDetailsByOutwardInvoice(OutwardInvoice.outiID).ToObservableCollection();
                    }
                    //==== #001
                    if (newInvoiceIDList.Count > 1)
                    {
                        SavedOutwardDrugs = RefDrugGenericDetailsProvider.Instance.GetOutwardDrugDetailsByOutwardInvoice(OutwardInvoice.outiID, HI, newInvoiceIDList.ToArray()).ToObservableCollection();
                    }
                    else
                    {
                        SavedOutwardDrugs = SavedOutwardInvoice.OutwardDrugs;
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddOutwardDrugAndDetails_Prescription. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_TRANSACTION_CANNOT_ADD);
                if (ex is SqlException)
                {
                    throw new Exception(ex.Message);
                    //throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                }
                else
                {
                    // Handle generic ones here.
                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                }
            }
        }

        public int OutWardDrugInvoicePrescriptChuaThuTien_Cancel_Pst(OutwardDrugInvoice InvoiceDrug)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutWardDrugInvoicePrescriptChuaThuTien_Cancel(InvoiceDrug);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutWardDrugInvoicePrescriptChuaThuTien_Cancel. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_OutWardDrugInvoiceVisitor_Cancel);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool CancelOutwardDrugInvoice_Pst(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice invoice, long? V_TradingPlaces)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start returning outward drug invoice.", CurrentUser);
                //Tim lai dang ky, neu chua co dang ky thi bao loi.
                //Neu dang ky nay chua co transaction cung khong duoc. Vi day la truong hop xoa nhung item da tinh tien roi -> da ton tai Transaction
                PatientRegistration registrationInfo = null;
                if (!invoice.PtRegistrationID.HasValue)
                {
                    throw new Exception(eHCMSResources.Z1793_G1_KgCoDK);
                }
                try
                {
                    //registrationInfo = PatientProvider.Instance.GetRegistration(invoice.PtRegistrationID.Value
                    //    , (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);
                    registrationInfo = eHCMSBillPaymt.RegAndPaymentProcessorBase.GetRegistrationTxd(invoice.PtRegistrationID.Value, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                if (registrationInfo == null || registrationInfo.PatientTransaction == null)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1700_G1_KgTimThayGDichCuaDK));
                }

                eHCMSBillPaymt.RegAndPaymentProcessorBase paymentProcessor = eHCMSBillPaymt.RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.InitNewTxd(registrationInfo, false);
                paymentProcessor.CancelOutwardDrugInvoice(StaffID, CollectorDeptLocID, Apply15HIPercent, invoice, V_TradingPlaces);
                AxLogger.Instance.LogInfo("End of returning outward drug invoice.", CurrentUser);
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of returning outward drug invoice. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CancelOutwardDrugInvoice);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool ListDrugExpiryDate_Save_Pst(OutwardDrugInvoice Invoice, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.ListDrugExpiryDate_Save(Invoice, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving ListDrugExpiryDate_Save. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_ListDrugExpiryDate_Save);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public int OutWardDrugInvoice_Delete_Pst(long id)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutWardDrugInvoice_Delete(id);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutWardDrugInvoice_Delete. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARDINV_CANNOT_DELETE);


                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddOutwardDrugReturn_Pst(long StaffID, int? Apply15HIPercent, OutwardDrugInvoice Outward, List<OutwardDrug> Details, out long outiID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start returning outward drug invoice.", CurrentUser);

                //Tim lai dang ky, neu chua co dang ky thi bao loi.
                //Neu dang ky nay chua co transaction cung khong duoc. Vi day la truong hop xoa nhung item da tinh tien roi -> da ton tai Transaction
                PatientRegistration registrationInfo = null;
                if (Outward.PtRegistrationID <= 0)
                {
                    //throw new Exception(eHCMSResources.Z1793_G1_KgCoDK);
                }

                try
                {
                    //registrationInfo = PatientProvider.Instance.GetRegistration(Outward.PtRegistrationID.Value
                    //    , (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);
                    registrationInfo = eHCMSBillPaymt.RegAndPaymentProcessorBase.GetRegistrationTxd(Outward.PtRegistrationID.Value, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU, true);
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                if (registrationInfo == null || registrationInfo.PatientTransaction == null)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1700_G1_KgTimThayGDichCuaDK));
                }
                var paymentProcessor = eHCMSBillPaymt.RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.InitNewTxd(registrationInfo, false);
                paymentProcessor.AddOutwardDrugReturn(StaffID, Apply15HIPercent, Outward, Details, out outiID);
                AxLogger.Instance.LogInfo("End of returning outward drug invoice.", CurrentUser);
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of returning outward drug invoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.DRUG_OUTWARD_CANNOT_ADD);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddOutWardDrugInvoiceReturnVisitor_Pst(OutwardDrugInvoice InvoiceDrug, long ReturnStaffID, out long outwardid)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.AddOutWardDrugInvoiceReturnVisitor(InvoiceDrug, ReturnStaffID, out outwardid);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving AddOutWardDrugInvoiceReturnVisitor. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_AddOutWardDrugInvoiceReturnVisitor);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool OutwardDrugInvoice_SaveByType_Balance(OutwardDrugInvoice Invoice, int ViewCase, out long ID, out string StrError)
        {
            try
            {
                return RefDrugGenericDetailsProvider.Instance.OutwardDrugInvoice_SaveByType_Balance(Invoice, ViewCase, out ID, out StrError);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving OutwardDrugInvoice_SaveByType. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.INWARDDRUG_SERVICE_OutwardDrugInvoice_SaveByType);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
    }
}