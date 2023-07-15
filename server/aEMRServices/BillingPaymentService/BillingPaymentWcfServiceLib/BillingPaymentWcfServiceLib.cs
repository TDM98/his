/*
 * 20170218 #001 CMN: Add Checkbox AllDept for InPtBills
 * 20170522 #002 CMN: Added variable to check InPt 5 year HI without paid enough
 * 20170609 #003 CMN: Fix SupportFund About TT04 with TT04
 * 20191102 #004 TNHX:  0017411 Add func for ReCalBillingInvoice With PriceList
 * 20210717 #005 TNHX:  Truyền phương thức thanh toán + mã code thanh toán online
 * 20220121 #006 TNHX: 	Thêm func cập nhật chi tiết đăng ký nội trú
 * 20220526 #007 DatTB: Thêm chức năng Xác nhận bn hoãn/ miễn tạm ứng
 * 20220531 #008 DatTB: Thêm biến IsWithOutBill Hủy quyết toán bỏ qua HDDT và phân quyền
 * 20220511 #009 QTD:   Thêm func lấy lần thanh toán cuối cho màn hình xác nhận BHYT
*/
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Service.Core.HelperClasses;
using System.Data.Common;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using AxLogging;
using DataEntities;
using System.Text;

using eHCMS.Services.Core;
using ErrorLibrary;
using ErrorLibrary.Resources;

using System.Data.SqlClient;
using eHCMSLanguage;
using eHCMSBillPaymt;
using eHCMS.Configurations;
using aEMR.DataAccessLayer.Providers;
using BillingPaymentWcfServiceLib.BankingPaymentServiceReference;
using GenLib;

namespace BillingPaymentWcfServiceLib
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    [ServiceKnownTypeAttribute(typeof(IInvoiceItem))]
    public class BillingPaymentWcfServiceLib : eHCMS.WCFServiceCustomHeader, IBillingPaymentWcfServiceLib
    {
        public BillingPaymentWcfServiceLib()
        {
        }
        /// <summary>
        /// Tính tiền thuốc cho danh sách thuốc này
        /// </summary>
        /// <param name="registrationID">Mã số đăng ký hiện tại</param>
        /// <param name="drugList">Danh sách thuốc</param>
        /// <param name="CalculatedDrugList">Danh sách thuốc đã được tính</param>
        /// <param name="PayableSum">Tổng số tiền bệnh nhân phải trả cho bệnh viện (tính chung cho cả đăng ký này)</param>
        /// <param name="TotalPaid">Tổng số tiền bệnh nhân đã trả cho bệnh viện</param>
        public void CalcOutwardDrugInvoice(long registrationID, OutwardDrugInvoice drugInvoice, out OutwardDrugInvoice CalculatedDrugInvoice, out PayableSum PayableSum, out decimal TotalPaid)
        {
            throw new Exception(eHCMSResources.Z1785_G1_NotImplemented);
            //RegAndPaymentProcessor paymentProcesssor = new RegAndPaymentProcessor(registrationID);

            //PatientRegistration curRegistration;
            //paymentProcesssor.CalcOutwardDrugInvoice(registrationID, drugInvoice, out curRegistration);
            //CalculatedDrugInvoice = drugInvoice;
            //if (curRegistration.PatientTransaction != null)
            //{
            //    PayableSum = curRegistration.PatientTransaction.PayableSum;
            //}
            //else
            //{
            //    PayableSum = null;
            //}
            ////TODO: Tu tu lam
            //TotalPaid = 0;
        }

        public void CreateBillingInvoiceFromExistingItems(PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, out long NewBillingInvoiceID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start saving inpatient Registration.", CurrentUser);

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.CreateBillingInvoiceFromExistingItems(registrationInfo, billingInv, out NewBillingInvoiceID);

                AxLogger.Instance.LogInfo("End of saving inpatient Registration.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving inpatient Registration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool CreateSuggestCashAdvance(long InPatientBillingInvID, long StaffID, out long RptPtCashAdvRemID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start CreateSuggestCashAdvance.", CurrentUser);

                return PatientProvider.Instance.CreateSuggestCashAdvance(InPatientBillingInvID, StaffID, out RptPtCashAdvRemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End CreateSuggestCashAdvance. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateSuggestCashAdvance, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void AddInPatientBillingInvoice(int? Apply15HIPercent, PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, bool CalcPaymentToEndOfDay
            , out long PatientRegistrationID, out Dictionary<long, List<long>> DrugIDList_Error, out long NewBillingInvoiceID, bool IsNotCheckInvalid)
        {
            PatientRegistrationID = 0;
            DrugIDList_Error = null;
            NewBillingInvoiceID = 0;
            try
            {
                AxLogger.Instance.LogInfo("Start saving inpatient Registration.", CurrentUser);

                PatientRegistrationID = -1;
                var paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);

                paymentProcessor.InitNewTxd(registrationInfo, false);

                paymentProcessor.AddInPatientBillingInvoice(Apply15HIPercent, billingInv, CalcPaymentToEndOfDay, IsNotCheckInvalid, out DrugIDList_Error, out NewBillingInvoiceID);
                PatientRegistrationID = registrationInfo.PtRegistrationID;

                AxLogger.Instance.LogInfo("End of saving inpatient Registration.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving inpatient Registration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void UpdateInPatientBillingInvoice(long? StaffID, InPatientBillingInvoice billingInv
             , List<PatientRegistrationDetail> newRegDetails
            , List<PatientRegistrationDetail> deletedRegDetails
            , List<PatientPCLRequest> newPclRequests
            , List<PatientPCLRequestDetail> newPclRequestDetails
            , List<PatientPCLRequestDetail> deletedPclRequestDetails
            , List<OutwardDrugClinicDeptInvoice> newOutwardDrugClinicDeptInvoices
            , List<OutwardDrugClinicDeptInvoice> savedOutwardDrugClinicDeptInvoices
            , List<OutwardDrugClinicDeptInvoice> modifiedOutwardDrugClinicDeptInvoices
            , List<OutwardDrugClinicDeptInvoice> deleteOutwardDrugClinicDeptInvoices
            , List<PatientRegistrationDetail> modifiedRegDetails
            , List<PatientPCLRequestDetail> modifiedPclRequestDetails
            , bool IsNotCheckInvalid
            , out Dictionary<long, List<long>> DrugIDList_Error)
        {
            try
            {
                if (billingInv == null || (billingInv.PtRegistrationID <= 0 && billingInv.OutPtRegistrationID == 0))
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }
                AxLogger.Instance.LogInfo("Start update billing invoice.", CurrentUser);
                PatientRegistration registrationInfo = null;
                try
                {
                    int V_FindPatientType = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
                    if (billingInv.OutPtRegistrationID > 0)
                    {
                        V_FindPatientType = (int)AllLookupValues.V_FindPatientType.NGOAI_TRU;
                        registrationInfo = PatientProvider.Instance.GetRegistration(billingInv.OutPtRegistrationID, V_FindPatientType);
                    }
                    else
                    {
                        registrationInfo = PatientProvider.Instance.GetRegistration(billingInv.PtRegistrationID, V_FindPatientType);//SEARCH NOI TRU
                    }
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.UpdateInPatientBillingInvoice(StaffID, registrationInfo, billingInv
                                                                    , newRegDetails
                                                                    , deletedRegDetails
                                                                    , newPclRequests
                                                                    , newPclRequestDetails
                                                                    , deletedPclRequestDetails
                                                                    , newOutwardDrugClinicDeptInvoices
                                                                    , savedOutwardDrugClinicDeptInvoices
                                                                    , modifiedOutwardDrugClinicDeptInvoices
                                                                    , deleteOutwardDrugClinicDeptInvoices
                                                                    , modifiedRegDetails
                                                                    , modifiedPclRequestDetails
                                                                    , IsNotCheckInvalid
                                                                    , out DrugIDList_Error);

                AxLogger.Instance.LogInfo("End of billing invoice.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of billing invoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateInPatientBillingInvoice, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool InsertAdditionalFee(long InPatientBillingInvID, long StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start InsertAdditionalFee.", CurrentUser);

                bool bRet = PatientProvider.Instance.InsertAdditionalFee(InPatientBillingInvID, StaffID);

                AxLogger.Instance.LogInfo("End InsertAdditionalFee.", CurrentUser);
                return bRet;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End InsertAdditionalFee. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_InsertAdditionalFee, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void PayForRegistration(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient
            , PatientTransactionPayment paymentDetails,// PatientPayment paymentDetails,
            List<PatientRegistrationDetail> colPaidRegDetails,
            List<PatientPCLRequest> colPaidPclRequests,
            List<OutwardDrugInvoice> colPaidDrugInvoice,
            List<InPatientBillingInvoice> colBillingInvoices,
            out PatientTransaction Transaction,
            out PatientTransactionPayment paymentInfo,
            out List<PaymentAndReceipt> paymentInfoList,
            out V_RegistrationError error,
			out string responseMsg,
			bool checkBeforePay = false)
        {
            PayForRegistration_V2(StaffID, CollectorDeptLocID, Apply15HIPercent, registrationID, FindPatient
                , paymentDetails
                , colPaidRegDetails
                , colPaidPclRequests
                , colPaidDrugInvoice
                , colBillingInvoices
                , null
                , out Transaction
                , out paymentInfo
                , out paymentInfoList
                , out error
				, out responseMsg
				, checkBeforePay);
        }
        public void PayForRegistration_V2(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient
            , PatientTransactionPayment paymentDetails,// PatientPayment paymentDetails,
            List<PatientRegistrationDetail> colPaidRegDetails,
            List<PatientPCLRequest> colPaidPclRequests,
            List<OutwardDrugInvoice> colPaidDrugInvoice,
            List<InPatientBillingInvoice> colBillingInvoices,
            PromoDiscountProgram PromoDiscountProgramObj,
            out PatientTransaction Transaction,
            out PatientTransactionPayment paymentInfo,
            out List<PaymentAndReceipt> paymentInfoList,
            out V_RegistrationError error,
			out string responseMsg,
			bool checkBeforePay = false)//out PatientPayment PaymentInfo
        {
            PayForRegistration_V3(StaffID, CollectorDeptLocID, Apply15HIPercent, registrationID, FindPatient
                , paymentDetails
                , colPaidRegDetails
                , colPaidPclRequests
                , colPaidDrugInvoice
                , colBillingInvoices
                , null
                , out Transaction
                , out paymentInfo
                , out paymentInfoList
                , out error
				, out responseMsg
                , checkBeforePay);
        }

        public void PayForRegistration_V3(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient
            , PatientTransactionPayment paymentDetails,// PatientPayment paymentDetails,
            List<PatientRegistrationDetail> colPaidRegDetails,
            List<PatientPCLRequest> colPaidPclRequests,
            List<OutwardDrugInvoice> colPaidDrugInvoice,
            List<InPatientBillingInvoice> colBillingInvoices,
            PromoDiscountProgram PromoDiscountProgramObj,
            out PatientTransaction Transaction,
            out PatientTransactionPayment paymentInfo,
            out List<PaymentAndReceipt> paymentInfoList,
            out V_RegistrationError error,
			out string responseMsg,
            bool checkBeforePay = false,
            long? ConfirmHIStaffID = null,
            string OutputBalanceServicesXML = null,
            bool IsReported = false,
            bool IsUpdateHisID = false,
            long? HIID = null,
            double? PtInsuranceBenefit = null,
            PatientRegistration existingPtRegisInfo = null,
            bool IsNotCheckInvalid = false,
            bool IsRefundBilling = false, bool IsProcess = false
            //▼====: #005
            , string TranPaymtNote = null, long? V_PaymentMode = null)
            //▲====: #005
        {
            long _PatientId = 0;
            try
            {
				responseMsg = string.Empty;
                if (registrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }
                AxLogger.Instance.LogInfo("Start processing payment.", CurrentUser);
                PatientRegistration regInfo = existingPtRegisInfo;
                Transaction = new PatientTransaction();
                error = V_RegistrationError.mNone;

                if (existingPtRegisInfo == null)
                {                    
                    try
                    {
                        // TxD 31/12/2013: Use RegAndPaymentProcessorBase get PatientRegistration then 
                        //                  assign back into RegAndPaymentProcessorBase to use without reloading in InitTxd function
                        regInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(registrationID, FindPatient, true);
                        //regInfo = GetRegistrationAndOtherDetails(registrationID, FindPatient, true, true);
                    }
                    catch
                    {
                        throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                    }
                }

                //KMx: Chặn trường hợp 2 màn hình giữa ĐKBH (Bên A) và ĐKDV (Bên B). Bên A nhận bệnh BH, Bên B load BN lên thêm DV và bấm LƯU.
                //Bên A hủy đăng ký. Bên B vẫn tính tiền các DV đã lưu được. (07/04/2014 14:35)
                if (regInfo != null && regInfo.V_RegistrationStatus == (long)AllLookupValues.RegistrationStatus.REFUND)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1693_G1_DKBiHuyKgTheTToan));
                }
                _PatientId = regInfo.PatientID.Value;

                if (PromoDiscountProgramObj != null && regInfo.PromoDiscountProgramObj == null)
                {
                    PromoDiscountProgramObj.RecordState = RecordState.ADDED;
                    regInfo.PromoDiscountProgramObj = PromoDiscountProgramObj;
                }
                else if (PromoDiscountProgramObj != null && !regInfo.PromoDiscountProgramObj.CompareValues(PromoDiscountProgramObj))
                {
                    PromoDiscountProgramObj.RecordState = RecordState.MODIFIED;
                    regInfo.PromoDiscountProgramObj = PromoDiscountProgramObj;
                }
                else if (regInfo.PromoDiscountProgramObj != null)
                {
                    regInfo.PromoDiscountProgramObj.RecordState = RecordState.UNCHANGED;
                }

                ////------kiem tra nhung danh sach them--------------------
                if (checkBeforePay)
                {
                    if (colPaidRegDetails != null)
                    {
                        var addRegDetailPaidList = CheckServiceHasPaidExist(regInfo.PatientRegistrationDetails.ToList(), colPaidRegDetails, regInfo);
                        if (addRegDetailPaidList != null && addRegDetailPaidList.Count > 0)
                        {
                            error = V_RegistrationError.mRefresh;
                            Transaction = new PatientTransaction();
                            paymentInfo = new PatientTransactionPayment();
                            paymentInfoList = new List<PaymentAndReceipt>();
                            return;
                        }
                    }
                    if (colPaidPclRequests != null)
                    {
                        var addPclRequestPaidList = CheckPCLPaidExist(regInfo.PCLRequests.ToList(), colPaidPclRequests, regInfo);
                        if (addPclRequestPaidList != null && addPclRequestPaidList.Count > 0)
                        {
                            error = V_RegistrationError.mRefresh;
                            Transaction = new PatientTransaction();
                            paymentInfo = new PatientTransactionPayment();
                            paymentInfoList = new List<PaymentAndReceipt>();
                            return;
                        }
                    }
                    if (colBillingInvoices != null && colBillingInvoices.Count > 0)
                    {
                        if (regInfo.InPatientBillingInvoices == null || regInfo.InPatientBillingInvoices.Count == 0 || colBillingInvoices.Any(x => x.V_InPatientBillingInvStatus != AllLookupValues.V_InPatientBillingInvStatus.NGUNG_TRA_TIEN_LAI
                            && !regInfo.InPatientBillingInvoices.Any(o => o.V_InPatientBillingInvStatus != AllLookupValues.V_InPatientBillingInvStatus.NGUNG_TRA_TIEN_LAI && o.InPatientBillingInvID == x.InPatientBillingInvID && x.ModifiedCount == o.ModifiedCount)))
                        {
                            error = V_RegistrationError.mRefresh;
                            Transaction = new PatientTransaction();
                            paymentInfo = new PatientTransactionPayment();
                            paymentInfoList = new List<PaymentAndReceipt>();
                            return;
                        }
                    }
                }

				StringBuilder msg = new StringBuilder();
				// VUTTM Begin
				ConcurrentDictionary<string, decimal> _HospitalPaymentList = null;
				ConcurrentDictionary<string, decimal> _PharmacyPaymentList = null;
				// Tinh tong tien tat ca dich vu/CLS can, thuoc bao hiem thanh toan
				GetHospitalPaymentList(ref _HospitalPaymentList, colPaidRegDetails,
					colPaidPclRequests, colPaidDrugInvoice, regInfo);
				// Tinh tien toa thuoc ban le(Nha thuoc)
				GetPharmacyPaymentList(ref _PharmacyPaymentList, colPaidDrugInvoice, regInfo);
				string _Salt = null;
				if (paymentDetails.PaymentMode.LookupID == 4803)
				{
					msg.AppendLine("[Xác Nhận Kết Quả Thanh Toán Qua Thẻ Khám Bệnh]");
					_Salt = StringUtil.RandomString();
					PatientProvider.Instance.AddPatientSalt(regInfo.PatientID.Value, _Salt);
				}

				bool _CanRefund = true;
				StringBuilder _CanRefundMsg = new StringBuilder();
				List<OutPatientCashAdvanceDetails> _OutPatientCashAdvanceDetailsLst = null;
                // Chi thuc hien khi hoan tra
				if (paymentDetails.PaymentType.LookupID == 5003)
				{
					string _PaymentType = paymentDetails.PaymentMode.LookupID == 4803
                        ? "Tiền Mặt" : "Thẻ Khám Bệnh";
                    string _Con1 = this.GetConditionsByServiceId(_HospitalPaymentList);
                    string _Con2 = this.GetConditionsByServiceId(_PharmacyPaymentList);

					_CanRefundMsg.AppendLine(String.Format(
						"Không thể hoàn trả do tồn tại các dịch vụ thanh toán [{0}] như sau:",
						_PaymentType));
                    List<string> _DiffReceiptNumLst = PatientProvider.Instance.GetReceiptNumWithDiffPaymentMode(
                        regInfo.PtRegistrationID, _Con1, _Con2, paymentDetails.PaymentMode.LookupID);
                    _CanRefund = null != _DiffReceiptNumLst && !_DiffReceiptNumLst.Any();
                    foreach (string _ReceiptNum in _DiffReceiptNumLst)
                    {
					    _CanRefundMsg.AppendLine(_ReceiptNum);
                    }
				}
				if (!_CanRefund)
				{
					responseMsg = _CanRefundMsg.ToString();
					error = V_RegistrationError.mRefresh;
					Transaction = new PatientTransaction();
					paymentInfo = new PatientTransactionPayment();
					paymentInfoList = new List<PaymentAndReceipt>();
                    PatientProvider.Instance.RemovePatientSalt(regInfo.PatientID.Value);
                    return;
				}

				bool _CanPay = true;
				TransactionResponse _VerificationTransactionResponse = null;
				if (paymentDetails.PaymentMode.LookupID == 4803
                    && paymentDetails.PaymentType.LookupID == 5001)
				{
					_VerificationTransactionResponse = this.CanPay(_Salt, _HospitalPaymentList,
						_PharmacyPaymentList, true, regInfo.PtRegistrationID, StaffID,
						regInfo.PatientID.Value);
					_CanPay = (null != _VerificationTransactionResponse
						&& _VerificationTransactionResponse.IsSucceed);
				}

				if (!_CanPay)
				{
					msg.AppendLine("Bệnh nhân không thể thanh toán qua Thẻ Khám Bệnh.");
					if (null != _VerificationTransactionResponse)
					{
						msg.AppendLine("Lỗi - Mô tả:"
							+ " " + _VerificationTransactionResponse.ResponseCode
							+ " - " + _VerificationTransactionResponse.ResponseDescription);
					}
					responseMsg = msg.ToString();
					error = V_RegistrationError.mRefresh;
					Transaction = new PatientTransaction();
					paymentInfo = new PatientTransactionPayment();
					paymentInfoList = new List<PaymentAndReceipt>();
                    PatientProvider.Instance.RemovePatientSalt(regInfo.PatientID.Value);
                    return;
				}
                // VuTTM End
                //▼====: #005
                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
                paymentProcessor.InitNewTxd(regInfo, false, 0, IsProcess);
                paymentProcessor.PayForRegistration(StaffID, CollectorDeptLocID, Apply15HIPercent, regInfo, paymentDetails, colPaidRegDetails, colPaidPclRequests, colPaidDrugInvoice, colBillingInvoices, out Transaction, out paymentInfo, out paymentInfoList, ConfirmHIStaffID, OutputBalanceServicesXML, IsReported, IsUpdateHisID
                    , HIID, PtInsuranceBenefit, IsNotCheckInvalid, IsRefundBilling, IsProcess, false, TranPaymtNote, V_PaymentMode);
                //▲====: #005
                // VuTTM Begin
                if (paymentDetails.PaymentMode.LookupID == 4803)
				{
					_OutPatientCashAdvanceDetailsLst =
						PatientProvider.Instance.GetOutPatientCashAdvanceDetailsLst(
							regInfo.PtRegistrationID);
				}

				if (paymentDetails.PaymentMode.LookupID == 4803)
				{
					DoBankingRequest(ref msg, true, _HospitalPaymentList,
						_OutPatientCashAdvanceDetailsLst, regInfo.PtRegistrationID,
						regInfo.PatientID.Value, StaffID, 4803, _Salt);
					DoBankingRequest(ref msg, false, _PharmacyPaymentList,
						_OutPatientCashAdvanceDetailsLst, regInfo.PtRegistrationID,
						regInfo.PatientID.Value, StaffID, 4803, _Salt);
					responseMsg = msg.ToString();
					PatientProvider.Instance.RemovePatientSalt(regInfo.PatientID.Value);
				}
				// VuTTM End
				AxLogger.Instance.LogInfo("End of processing payment.", CurrentUser);
            }
            catch (Exception ex)
            {
                PatientProvider.Instance.RemovePatientSalt(_PatientId);

                AxLogger.Instance.LogInfo("End of processing payment. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PayForRegistration, CurrentUser);

                //throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

		// VuTTM Begin
		private bool CanRefund(List<OutPatientCashAdvanceDetails> outPatientCashAdvanceDetailsLst,
			ConcurrentDictionary<string, decimal> hosRefList,
			ConcurrentDictionary<string, decimal> pharRefList,
			long curPaymentMode, out string msg)
		{
			if ((null == outPatientCashAdvanceDetailsLst
					|| !outPatientCashAdvanceDetailsLst.Any())
				|| ((null == hosRefList || !hosRefList.Any()))
					&& (null == pharRefList || !pharRefList.Any()))
			{
				msg = null;
				return true;
			}

			bool _Result = true;
			StringBuilder _Sb = new StringBuilder();
			foreach (OutPatientCashAdvanceDetails _OutPtCashAdvDtls in outPatientCashAdvanceDetailsLst)
			{
				string _Key = null != _OutPtCashAdvDtls.PtRegDetailID
					? PatientProvider.PT_REG_PREFIX + _OutPtCashAdvDtls.PtRegDetailID
					: (null != _OutPtCashAdvDtls.PCLRequestID
						? PatientProvider.PCL_REG_PREFIX + _OutPtCashAdvDtls.PCLRequestID
							: PatientProvider.OUTI_REG_PREFIX + _OutPtCashAdvDtls.outiID);
				if (_OutPtCashAdvDtls.V_PaymentMode.CompareTo(curPaymentMode) != 0
					&& _OutPtCashAdvDtls.PaymentAmount >= 0
					&& ((null != hosRefList && hosRefList.ContainsKey(_Key))
						|| (null != pharRefList && pharRefList.ContainsKey(_Key))))
				{
					_Sb.AppendLine(">>> Mã Phiếu Thu: "
						+ _OutPtCashAdvDtls.CashAdvReceiptNum
						+ " - "
						+ _OutPtCashAdvDtls.ServiceName);
					_Result = false;
				}
			}
			msg = _Sb.ToString();
			return _Result;
		}
		
		public TransactionResponse HasCard(long patientID, long staffID)
		{
			try
			{
				string _Salt = StringUtil.RandomString();
				PatientProvider.Instance.AddPatientSalt(patientID, _Salt);

				TransactionResponse _Response = null;
				using (var _BankingPaymentService = new BankingPaymentServiceClient())
				{
					_Response = _BankingPaymentService.Func1(
						GenLib.HashUtil.Encrypt(
							String.Format("{0}||||||||||||||", staffID), _Salt),
						GenLib.HashUtil.Encrypt(patientID.ToString()));
				}
				PatientProvider.Instance.RemovePatientSalt(patientID);

				return _Response;
			}
			catch (Exception _Ex)
			{
				AxLogger.Instance.LogInfo("Có lỗi xảy ra. Vui lòng liên hệ quản trị viên.",
					CurrentUser);
				PatientProvider.Instance.RemovePatientSalt(patientID);
				AxException axErr = AxException.CatchExceptionAndLogMessage(_Ex,
					ErrorNames.COMMON_SERVICE_PayForRegistration, CurrentUser);
				throw new FaultException<AxException>(axErr, new FaultReason(_Ex.Message));
			}
		}

		public TransactionResponse MapCard(bool isHospital, long patientID, string acctNo,
			string pan, string issueDate, long staffID)
		{
			try
			{
				TransactionResponse _Response = null;

				string _Salt = StringUtil.RandomString();
				PatientProvider.Instance.AddPatientSalt(patientID, _Salt);
				using (var _BankingPaymentService = new BankingPaymentServiceClient())
				{
					_Response = _BankingPaymentService.Func2(
						GenLib.HashUtil.Encrypt(
							String.Format("{0}|{1}|{2}|{3}|{4}||||||||||",
								staffID, isHospital, acctNo, pan, issueDate), _Salt),
						GenLib.HashUtil.Encrypt(patientID.ToString()));
				}
				PatientProvider.Instance.RemovePatientSalt(patientID);

				return _Response;
			}
			catch (Exception _Ex)
			{
				AxLogger.Instance.LogInfo("Có lỗi xảy ra. Vui lòng liên hệ quản trị viên.",
					CurrentUser);
				PatientProvider.Instance.RemovePatientSalt(patientID);
				AxException axErr = AxException.CatchExceptionAndLogMessage(_Ex,
					ErrorNames.COMMON_SERVICE_PayForRegistration, CurrentUser);
				throw new FaultException<AxException>(axErr, new FaultReason(_Ex.Message));
			}
		}

		public TransactionResponse UnmapCard(bool isHospital, long patientID, long staffID)
		{
			try
			{
				TransactionResponse _Response = null;

				string _Salt = StringUtil.RandomString();
				PatientProvider.Instance.AddPatientSalt(patientID, _Salt);
				using (var _BankingPaymentService = new BankingPaymentServiceClient())
				{
					_Response = _BankingPaymentService.Func3(
						GenLib.HashUtil.Encrypt(
							String.Format("{0}|{1}|||||||||||||", staffID, isHospital),
							_Salt),
						GenLib.HashUtil.Encrypt(patientID.ToString()));
				}
				PatientProvider.Instance.RemovePatientSalt(patientID);

				return _Response;
			}
			catch (Exception _Ex)
			{
				AxLogger.Instance.LogInfo("Có lỗi xảy ra. Vui lòng liên hệ quản trị viên.",
					CurrentUser);
				PatientProvider.Instance.RemovePatientSalt(patientID);
				AxException axErr = AxException.CatchExceptionAndLogMessage(_Ex,
					ErrorNames.COMMON_SERVICE_PayForRegistration, CurrentUser);
				throw new FaultException<AxException>(axErr, new FaultReason(_Ex.Message));
			}
		}

		public TransactionResponse Deposit(bool isHospital, long patientID, decimal settlementAmount,
			long staffID)
		{
			try
			{
				TransactionResponse _Response = null;

				string _Salt = StringUtil.RandomString();
				PatientProvider.Instance.AddPatientSalt(patientID, _Salt);
				using (var _BankingPaymentService = new BankingPaymentServiceClient())
				{
					_Response = _BankingPaymentService.Func4(
						GenLib.HashUtil.Encrypt(
							String.Format("{0}|{1}||||{2}|||||||||",
								staffID, isHospital, settlementAmount),
							_Salt),
						GenLib.HashUtil.Encrypt(patientID.ToString()));
				}
				PatientProvider.Instance.RemovePatientSalt(patientID);

				return _Response;
			}
			catch (Exception _Ex)
			{
				AxLogger.Instance.LogInfo("Có lỗi xảy ra. Vui lòng liên hệ quản trị viên.",
					CurrentUser);
				PatientProvider.Instance.RemovePatientSalt(patientID);
				AxException axErr = AxException.CatchExceptionAndLogMessage(_Ex,
					ErrorNames.COMMON_SERVICE_PayForRegistration, CurrentUser);
				throw new FaultException<AxException>(axErr, new FaultReason(_Ex.Message));
			}
		}

		private List<OutPatientCashAdvanceDetails> GetOutPtCashAdvForBankingRequest(bool isHospital,
			ConcurrentDictionary<string, decimal> paymentList,
			ref Dictionary<string, decimal> _PaymentServices,
			List<OutPatientCashAdvanceDetails> outPatientCashAdvanceDetailsLst)
		{
			if ((null == paymentList || !paymentList.Any())
				|| (null == outPatientCashAdvanceDetailsLst
					|| !outPatientCashAdvanceDetailsLst.Any()))
			{
				return null;
			}

			List<OutPatientCashAdvanceDetails> _Result = new List<OutPatientCashAdvanceDetails>();
			foreach (OutPatientCashAdvanceDetails _OutPtCashAdvDtls in
				outPatientCashAdvanceDetailsLst)
			{
				if (!((isHospital
					        && _OutPtCashAdvDtls.CashAdvReceiptNum.Contains(
							    PatientProvider.HOS_RECEIPT_SIGN))
					|| (!isHospital
							&& _OutPtCashAdvDtls.CashAdvReceiptNum.Contains(
								PatientProvider.PHAR_RECEIPT_SIGN)))
					|| !(null == _OutPtCashAdvDtls.BankingTransactionId
						&& null == _OutPtCashAdvDtls.BankingRefundTransactionId))
				{
					continue;
				}
				string _Key = CreateServiceKey(_OutPtCashAdvDtls);
				if (String.IsNullOrEmpty(_Key))
				{
					continue;
				}
				if (paymentList.ContainsKey(_Key))
				{
					_Result.Add(_OutPtCashAdvDtls);
				}
				if (!_PaymentServices.ContainsKey(_Key))
				{
					paymentList.TryGetValue(_Key, out decimal _Value);
					_PaymentServices.Add(_Key, _Value);
				}
			}

			return _Result;
		}

		private ConcurrentDictionary<long, decimal> GetOutwardDrugForBankingRefund(bool isHospital,
			Dictionary<string, decimal> paymentServices,
			OutPatientCashAdvanceDetails refundOutPtCashAdvDtls,
			List<OutPatientCashAdvanceDetails> outPatientCashAdvanceDetailsLst,
			ref List<long> refundedTrxLst)
		{
			if ((null == paymentServices || !paymentServices.Any())
				|| (null == outPatientCashAdvanceDetailsLst
					|| !outPatientCashAdvanceDetailsLst.Any()))
			{
				return new ConcurrentDictionary<long, decimal>();
			}
			List<OutPatientCashAdvanceDetails> _PaymentLst = new List<OutPatientCashAdvanceDetails>();
			foreach (OutPatientCashAdvanceDetails _OutPtCashAdvDtls in outPatientCashAdvanceDetailsLst)
			{
				if ((!((isHospital && _OutPtCashAdvDtls.CashAdvReceiptNum.Contains("CR"))
						|| (!isHospital && _OutPtCashAdvDtls.CashAdvReceiptNum.Contains("NT"))))
					|| !(null != _OutPtCashAdvDtls.outiID
						&& null != _OutPtCashAdvDtls.BankingTransactionId
						&& null == _OutPtCashAdvDtls.BankingRefundTransactionId))
				{
					continue;
				}
				_PaymentLst.Add(_OutPtCashAdvDtls);
			}
			if (null == _PaymentLst
				|| !_PaymentLst.Any())
			{
				return new ConcurrentDictionary<long, decimal>();
			}
			_PaymentLst.Sort(delegate(OutPatientCashAdvanceDetails _Item1,
				OutPatientCashAdvanceDetails _Item2)
			{
				if (null == _Item1.BankingTransactionId
					&& null == _Item2.BankingTransactionId) return 0;
				else if (null == _Item1.BankingTransactionId) return 1;
				else if (null == _Item2.BankingTransactionId) return -1;
				else return _Item2.BankingTransactionId.Value.CompareTo(
					_Item1.BankingTransactionId.Value);
			});

			ConcurrentDictionary<long, decimal> _TmpDic = new ConcurrentDictionary<long, decimal>();
			string _CurKey = CreateServiceKey(refundOutPtCashAdvDtls);
			decimal _CurRefundAmount = refundOutPtCashAdvDtls.PaymentAmount < 0 ?
                refundOutPtCashAdvDtls.PaymentAmount * -1 : refundOutPtCashAdvDtls.PaymentAmount;
			foreach (OutPatientCashAdvanceDetails _CurOutPtCshAdvDtls in _PaymentLst)
			{
				if (!((isHospital && _CurOutPtCshAdvDtls.CashAdvReceiptNum.Contains("CR"))
					|| (!isHospital && _CurOutPtCshAdvDtls.CashAdvReceiptNum.Contains("NT"))))
				{
					continue;
				}
				string _Key = CreateServiceKey(_CurOutPtCshAdvDtls);
				if ((!_CurKey.Equals(_Key) && !_TmpDic.Any())
					|| _TmpDic.ContainsKey(_CurOutPtCshAdvDtls.BankingTransactionId.Value))
				{
					continue;
				}
				if (_CurKey.Equals(_Key)
					&& _CurRefundAmount - _CurOutPtCshAdvDtls.PaymentAmount == 0)
				{
					_TmpDic.AddOrUpdate(_CurOutPtCshAdvDtls.BankingTransactionId.Value,
						_CurRefundAmount);
					break;
				}
				if (_CurRefundAmount - _CurOutPtCshAdvDtls.PaymentAmount <= 0)
				{
					_TmpDic.AddOrUpdate(_CurOutPtCshAdvDtls.BankingTransactionId.Value,
						_CurRefundAmount);
					break;
				}
				_TmpDic.AddOrUpdate(_CurOutPtCshAdvDtls.BankingTransactionId.Value,
						_CurOutPtCshAdvDtls.PaymentAmount);
				_CurRefundAmount -= _CurOutPtCshAdvDtls.PaymentAmount;
			}
			if ((null != _TmpDic && _TmpDic.Any())
				&& (null != refundedTrxLst))
			{
				foreach (KeyValuePair<long, decimal> _KeyValuePair in _TmpDic)
				{
					if (refundedTrxLst.Contains(_KeyValuePair.Key))
					{
						continue;
					}
					refundedTrxLst.Add(_KeyValuePair.Key);
				}
			}

            if (null == _TmpDic || !_TmpDic.Any())
            {
                foreach (OutPatientCashAdvanceDetails _CurOutPtCshAdvDtls in _PaymentLst)
                {
                    if (!((isHospital && _CurOutPtCshAdvDtls.CashAdvReceiptNum.Contains("CR"))
                            || (!isHospital && _CurOutPtCshAdvDtls.CashAdvReceiptNum.Contains("NT")))
                        || !(null != _CurOutPtCshAdvDtls.BankingTransactionId
                            && null == _CurOutPtCshAdvDtls.BankingRefundTransactionId
                            && 0 != _CurOutPtCshAdvDtls.PaymentAmount
                            && _CurOutPtCshAdvDtls.PaymentAmount >= _CurRefundAmount))
                    {
                        continue;
                    }
                    _TmpDic.AddOrUpdate(_CurOutPtCshAdvDtls.BankingTransactionId.Value,
                        _CurRefundAmount);
                    refundedTrxLst.Add(_CurOutPtCshAdvDtls.BankingTransactionId.Value);
                    break;
                }
            }
			return _TmpDic;
		}

		private ConcurrentDictionary<long, decimal> GetOutPtCashAdvForBankingRefund(bool isHospital,
			Dictionary<string, decimal> paymentServices,
			OutPatientCashAdvanceDetails curOutPtCashAdvDtls,
			List<OutPatientCashAdvanceDetails> outPatientCashAdvanceDetailsLst,
			ref List<long> refundedTrxLst)
		{
			if ((null == paymentServices || !paymentServices.Any())
				|| (null == outPatientCashAdvanceDetailsLst
					|| !outPatientCashAdvanceDetailsLst.Any()))
			{
				return new ConcurrentDictionary<long, decimal>();
			}

			string _CurKey = CreateServiceKey(curOutPtCashAdvDtls);
			ConcurrentDictionary<long, decimal> _TmpDic = new ConcurrentDictionary<long, decimal>();
			foreach (OutPatientCashAdvanceDetails _OutPtCashAdvDtls in outPatientCashAdvanceDetailsLst)
			{
				if (!((isHospital && _OutPtCashAdvDtls.CashAdvReceiptNum.Contains("CR"))
					|| (!isHospital && _OutPtCashAdvDtls.CashAdvReceiptNum.Contains("NT"))))
				{
					continue;
				}
				string _Key = CreateServiceKey(_OutPtCashAdvDtls);
				bool _IsOutwardDrug = null != _OutPtCashAdvDtls.outiID;
				long? _BankingTransactionId = null != _OutPtCashAdvDtls.BankingTransactionId
					? _OutPtCashAdvDtls.BankingTransactionId
						: _OutPtCashAdvDtls.BankingRefundTransactionId;
				if ((null == _OutPtCashAdvDtls.BankingTransactionId
						&& null == _OutPtCashAdvDtls.BankingRefundTransactionId)
					|| curOutPtCashAdvDtls.OutPtCashAdvanceID == _OutPtCashAdvDtls.OutPtCashAdvanceID
					|| !_CurKey.Equals(_Key)
					|| !paymentServices.ContainsKey(_Key))
				{
					continue;
				}

				if ((!_TmpDic.ContainsKey(_BankingTransactionId.Value) && _TmpDic.Count == 1))
				{
					break;
				}

				if (_IsOutwardDrug && _Key.Equals(_CurKey))
				{
					_TmpDic.AddOrUpdate(_BankingTransactionId.Value,
						curOutPtCashAdvDtls.PaymentAmount);
					break;
				}
				else
				{
					paymentServices.TryGetValue(_Key, out decimal _Value);
					_TmpDic.AddOrUpdate(_BankingTransactionId.Value, _Value, (k, v) => v + _Value);
				}
			}
			if ((null != _TmpDic && _TmpDic.Any())
				&& (null != refundedTrxLst)
				&& !refundedTrxLst.Contains(_TmpDic.Keys.First()))
			{
				refundedTrxLst.Add(_TmpDic.Keys.First());
			}
			return _TmpDic;
		}

		private TransactionResponse RequestRefund(string salt, long ptRegistrationId,
			long staffId, long patientId, long transactionId, decimal totalPayment)
		{
			totalPayment = totalPayment < 0 ? totalPayment * -1 : totalPayment;
			totalPayment = Round(totalPayment);
			if (0 == totalPayment)
			{
				return new TransactionResponse() { IsSucceed = true };
			}

			TransactionResponse _Response = null;
			using (var _BankingPaymentService = new BankingPaymentServiceClient())
			{
				_Response = _BankingPaymentService.Func7(
					GenLib.HashUtil.Encrypt(
						String.Format("{0}|||||{1}|{2}||{3}||||||",
							staffId, totalPayment, ptRegistrationId, transactionId),
						salt),
				GenLib.HashUtil.Encrypt(patientId.ToString()));
			}
			AxLogger.Instance.LogInfo(_Response.ResponseDescription, CurrentUser);

			return _Response;
		}

		private TransactionResponse RequestPayment(string salt, bool isHospital,
			long ptRegistrationId, long staffId, long patientId,
			decimal totalPayment, string billID)
		{
			totalPayment = Round(totalPayment);

			// Thuc hien thanh toan
			TransactionResponse _Response = null;
			using (var _BankingPaymentService = new BankingPaymentServiceClient())
			{
				_Response = _BankingPaymentService.Func6(
					GenLib.HashUtil.Encrypt(
						String.Format("{0}|{1}||||{2}|{3}|{4}|||||||",
							staffId, isHospital, totalPayment, ptRegistrationId, billID),
						salt),
					GenLib.HashUtil.Encrypt(patientId.ToString()));
			}
			AxLogger.Instance.LogInfo(_Response.ResponseDescription, CurrentUser);

			return _Response;
		}

		private List<TransactionResponse> DoBankingRequest(ref StringBuilder msg,
			bool isHospital, ConcurrentDictionary<string, decimal> curPaymentList,
			List<OutPatientCashAdvanceDetails> outPatientCashAdvanceDetailsLst,
			long ptRegistrationID, long patientID, long staffID, long paymentMode,
			string salt)
		{
			if ((null == curPaymentList || !curPaymentList.Any())
				|| (null == outPatientCashAdvanceDetailsLst
					|| !outPatientCashAdvanceDetailsLst.Any()))
			{
				return null;
			}
			Dictionary<string, decimal> _PaymentServices = new Dictionary<string, decimal>();
			List<OutPatientCashAdvanceDetails> _CurOutPtCashAdvDtlsLst =
				GetOutPtCashAdvForBankingRequest(isHospital, curPaymentList,
				ref _PaymentServices, outPatientCashAdvanceDetailsLst);
			if (null == _CurOutPtCashAdvDtlsLst || !_CurOutPtCashAdvDtlsLst.Any())
			{
				return null;
			}

			List<TransactionResponse> _TransactionResponseLst = new List<TransactionResponse>();
			List<long> _RefundedTrxLst = new List<long>();
			List<long> _ResquestTrxLst = new List<long>();
			_RefundedTrxLst = new List<long>();
			long _LastTrxId = -1;
			foreach (OutPatientCashAdvanceDetails _OutPtCashAdvDtls in _CurOutPtCashAdvDtlsLst)
			{
				string _Key = null != _OutPtCashAdvDtls.PtRegDetailID
					? PatientProvider.PT_REG_PREFIX + _OutPtCashAdvDtls.PtRegDetailID
					: (null != _OutPtCashAdvDtls.PCLRequestID
						? PatientProvider.PCL_REG_PREFIX + _OutPtCashAdvDtls.PCLRequestID
							: PatientProvider.OUTI_REG_PREFIX + _OutPtCashAdvDtls.outiID);

				TransactionResponse _TransactionResponse = null;
				if (0 == _OutPtCashAdvDtls.PaymentAmount)
				{
					continue;
				}
				// Thuc hien thanh toan
				if (_OutPtCashAdvDtls.PaymentAmount > 0
					&& !_ResquestTrxLst.Contains(_OutPtCashAdvDtls.OutPtCashAdvanceID))
				{
					_TransactionResponse = this.RequestPayment(salt, isHospital, ptRegistrationID,
						staffID, patientID, _OutPtCashAdvDtls.PaymentAmount,
						_OutPtCashAdvDtls.CashAdvReceiptNum);
					PatientProvider.Instance.UpdateOutPatientCashAdvanceByBankingTrxId(
						_OutPtCashAdvDtls.OutPtCashAdvanceID, _TransactionResponse.TransactionId,
						paymentMode);
					_ResquestTrxLst.Add(_OutPtCashAdvDtls.OutPtCashAdvanceID);
					GetMsg(ref msg, false, _TransactionResponse,
						_OutPtCashAdvDtls.CashAdvReceiptNum, _OutPtCashAdvDtls.ServiceName);
				}
                // Thuc hien hoan tra
                if (_OutPtCashAdvDtls.PaymentAmount < 0)
				{
					_TransactionResponse = DoBankingRefund(ref _RefundedTrxLst, ref _LastTrxId,
						isHospital, _PaymentServices, _OutPtCashAdvDtls,
						outPatientCashAdvanceDetailsLst, ptRegistrationID, patientID,
						paymentMode, staffID, salt);
					GetMsg(ref msg, true, _TransactionResponse, _OutPtCashAdvDtls.CashAdvReceiptNum,
						_OutPtCashAdvDtls.ServiceName);
				}
				_TransactionResponseLst.Add(_TransactionResponse);
			}

			return _TransactionResponseLst;
		}

        private static readonly string DISPLAY_MONEY_FORMAT = "{0:0,0.##}";

        private void GetMsg(ref StringBuilder msg, bool isRefund,
			TransactionResponse transactionResponse, string cashAdvReceiptNum, string serviceName)
		{
            if (null == transactionResponse)
            {
                return;
            }
			if (null == msg)
			{
				msg = new StringBuilder();
			}
			string paymentType = isRefund ? "Hoàn trả" : "Thanh toán";
			string result = String.Empty;
			if (null != transactionResponse
				&& !String.IsNullOrEmpty(transactionResponse.ResponseCode))
			{
				result = transactionResponse.ResponseCode.Equals("00")
					? "Thành công" : "Lỗi: "
						+ transactionResponse.ResponseDescription;
			}
			if (null != msg
                && !string.IsNullOrEmpty(cashAdvReceiptNum)
                && !msg.ToString().Contains(cashAdvReceiptNum))
			{
				msg.AppendLine(string.Format("{0} - Phiếu thu {1}",
					paymentType, cashAdvReceiptNum));
			}
            if (String.IsNullOrEmpty(result))
            {
                result = "Không thực hiện giao dịch.";
            }
			msg.AppendLine(string.Format(">>> Dịch Vụ: {0} - Số Tiền: {1} - Trạng Thái: {2}",
                serviceName,
                String.Format(DISPLAY_MONEY_FORMAT, (null == transactionResponse ? 0 : transactionResponse.SettlementAmount)),
                result));
        }

		private TransactionResponse DoBankingRefund(ref List<long> refundedTrxLst, ref long lastTrxId,
			bool isHospital, Dictionary<string, decimal> paymentServices,
			OutPatientCashAdvanceDetails outPtCashAdvDtls,
			List<OutPatientCashAdvanceDetails> outPatientCashAdvanceDetailsLst,
			long ptRegistrationID, long patientID, long paymentMode, long staffID, string salt)
		{

			ConcurrentDictionary<long, decimal> _BankingRefundLst = null;
			bool _IsOutwardDrug = null != outPtCashAdvDtls.outiID;
			if (_IsOutwardDrug)
			{
				_BankingRefundLst = GetOutwardDrugForBankingRefund(isHospital, paymentServices,
					outPtCashAdvDtls, outPatientCashAdvanceDetailsLst, ref refundedTrxLst);
			}
			else
			{
				_BankingRefundLst = GetOutPtCashAdvForBankingRefund(isHospital, paymentServices,
					outPtCashAdvDtls, outPatientCashAdvanceDetailsLst, ref refundedTrxLst);
			}
            TransactionResponse _TransactionResponse = null;
            foreach (KeyValuePair<long, decimal> _KeyValuePair in _BankingRefundLst)
			{
                if (!isHospital && _IsOutwardDrug
                        && refundedTrxLst.GroupBy(id => id).Any(c => c.Count() > 1))
                {
                    lastTrxId = _KeyValuePair.Key;
                    continue;
                }
				if (refundedTrxLst.Contains(_KeyValuePair.Key))
				{
					_TransactionResponse = RequestRefund(salt, ptRegistrationID, staffID,
						patientID, _KeyValuePair.Key, _KeyValuePair.Value);
					lastTrxId = _KeyValuePair.Key;
				}
			}

			if (-1 == lastTrxId)
			{
				return _TransactionResponse;
			}

			PatientProvider.Instance.AddBankingRefundDetails(outPtCashAdvDtls.OutPtCashAdvanceID,
				outPtCashAdvDtls.PtRegDetailID, outPtCashAdvDtls.PCLRequestID,
				outPtCashAdvDtls.outiID, lastTrxId);
			PatientProvider.Instance.UpdateOutPatientCashAdvanceByBankingTrxId(
				outPtCashAdvDtls.OutPtCashAdvanceID, null, paymentMode);
			return _TransactionResponse;
		}

		private string CreateServiceKey(OutPatientCashAdvanceDetails outPtCashAdvDtls)
		{
			if (null == outPtCashAdvDtls)
			{
				return string.Empty;
			}
			return (null != outPtCashAdvDtls.PtRegDetailID ? PatientProvider.PT_REG_PREFIX + outPtCashAdvDtls.PtRegDetailID
					: (null != outPtCashAdvDtls.PCLRequestID ? PatientProvider.PCL_REG_PREFIX + outPtCashAdvDtls.PCLRequestID
					: (null != outPtCashAdvDtls.outiID ? PatientProvider.OUTI_REG_PREFIX + outPtCashAdvDtls.outiID : string.Empty)));
		}

		private string CreateServiceKey(PatientTransactionDetail patientTransactionDetail)
		{
			if (null == patientTransactionDetail)
			{
				return string.Empty;
			}
			return (null != patientTransactionDetail.PtRegDetailID ? PatientProvider.PT_REG_PREFIX + patientTransactionDetail.PtRegDetailID
					: (null != patientTransactionDetail.PCLRequestID ? PatientProvider.PCL_REG_PREFIX + patientTransactionDetail.PCLRequestID
					: (null != patientTransactionDetail.outiID ? PatientProvider.OUTI_REG_PREFIX + patientTransactionDetail.outiID : string.Empty)));
		}

		private void GetHospitalPaymentList(ref ConcurrentDictionary<string, decimal> regstDic,
			List<PatientRegistrationDetail> colPaidRegDetails,
			List<PatientPCLRequest> colPaidPclRequests,
			List<OutwardDrugInvoice> colPaidDrugInvoice, PatientRegistration regInfo)
		{
			GetRegistrationList(ref regstDic, colPaidRegDetails);
			GetRegistrationPCLList(ref regstDic, colPaidPclRequests);
			GetOutwardDrugInvoiceList(ref regstDic, true, colPaidDrugInvoice);
			GetAdditionalRegistrationList(ref regstDic, regInfo);
		}

		private void GetPharmacyPaymentList(ref ConcurrentDictionary<string, decimal> regstDic,
			List<OutwardDrugInvoice> colPaidDrugInvoice, PatientRegistration regInfo)
		{
			GetOutwardDrugInvoiceList(ref regstDic, false, colPaidDrugInvoice);
			GetAdditionalRegistrationList(ref regstDic, regInfo);
		}

        private static readonly string CONS_QUERY = @" AND (prd.PtRegDetailID IN ({0}) OR ptd.outiID IN ({1}) OR ptd.PCLRequestID IN ({2})) ";

        private string GetConditionsByServiceId(ConcurrentDictionary<string, decimal> regstDic)
        {
            if (null == regstDic || regstDic.IsEmpty)
            {
                return string.Empty;
            }

            string[] _Ids = new string[regstDic.Count];
            int _Idx = 0;
            foreach (KeyValuePair<string, decimal> _KeyValuePair in regstDic)
            {
                string _Id = _KeyValuePair.Key;
                _Id = _Id.Replace(PatientProvider.PT_REG_PREFIX, string.Empty);
                _Id = _Id.Replace(PatientProvider.PCL_REG_PREFIX, string.Empty);
                _Id = _Id.Replace(PatientProvider.OUTI_REG_PREFIX, string.Empty);
                _Id = _Id.Replace(PatientProvider.HOS_RECEIPT_SIGN, string.Empty);
                _Id = _Id.Replace(PatientProvider.PHAR_RECEIPT_SIGN, string.Empty);
                _Ids[_Idx++]=_Id;
            }
            string _IdsAsStr = string.Join(",", _Ids);
            
            return String.Format(CONS_QUERY, _IdsAsStr, _IdsAsStr, _IdsAsStr);
        }

		private void GetRegistrationList(ref ConcurrentDictionary<string, decimal> regstDic,
			List<PatientRegistrationDetail> colPaidRegDetails)
		{
			if (null == colPaidRegDetails || !colPaidRegDetails.Any())
			{
				return;
			}

			if (null == regstDic)
			{
				regstDic = new ConcurrentDictionary<string, decimal>();
			}

			foreach (PatientRegistrationDetail _PtRegDetail in colPaidRegDetails)
			{
				regstDic.AddOrUpdate(PatientProvider.PT_REG_PREFIX + _PtRegDetail.PtRegDetailID, _PtRegDetail.TotalPatientPayment);
			}
		}

		private void GetRegistrationPCLList(ref ConcurrentDictionary<string, decimal> regstDic,
			List<PatientPCLRequest> colPaidPclRequests)
		{
			if (null == colPaidPclRequests || !colPaidPclRequests.Any())
			{
				return;
			}

			if (null == regstDic)
			{
				regstDic = new ConcurrentDictionary<string, decimal>();
			}

			foreach (PatientPCLRequest _PtPclRequest in colPaidPclRequests)
			{
				foreach (PatientPCLRequestDetail _PtPclRequestDetail in _PtPclRequest.PatientPCLRequestIndicators)
				{
					regstDic.AddOrUpdate(PatientProvider.PCL_REG_PREFIX + _PtPclRequestDetail.PatientPCLReqID,
						_PtPclRequestDetail.TotalPatientPayment);
				}
			}
		}

		private void GetOutwardDrugInvoiceList(ref ConcurrentDictionary<string, decimal> regstDic, bool forHospital,
			List<OutwardDrugInvoice> colPaidDrugInvoice)
		{
			if (null == colPaidDrugInvoice || !colPaidDrugInvoice.Any())
			{
				return;
			}

			if (null == regstDic)
			{
				regstDic = new ConcurrentDictionary<string, decimal>();
			}

			foreach (OutwardDrugInvoice _OutwardDrugInvoice in colPaidDrugInvoice)
			{
				decimal _TotalPayment = GetOutwardDrugTotalPatientPayment(
					_OutwardDrugInvoice.OutwardDrugs);
				if (!forHospital &&
					(null != _OutwardDrugInvoice.StoreID
						&& _OutwardDrugInvoice.StoreID != Globals.AxServerSettings.CommonItems.StoreIDForHIPrescription))
				{
					regstDic.AddOrUpdate(PatientProvider.OUTI_REG_PREFIX + _OutwardDrugInvoice.outiID, _TotalPayment);
					continue;
				}
				if (forHospital &&
                    (null != _OutwardDrugInvoice.StoreID
						&& _OutwardDrugInvoice.StoreID == Globals.AxServerSettings.CommonItems.StoreIDForHIPrescription))
				{
					regstDic.AddOrUpdate(PatientProvider.OUTI_REG_PREFIX + _OutwardDrugInvoice.outiID, _TotalPayment);
				}
			}
		}

		private void GetAdditionalRegistrationList(
			ref ConcurrentDictionary<string, decimal> regstDic, PatientRegistration regInfo)
		{
			if (null == regInfo
				|| null == regInfo.PatientTransaction
				|| (null == regInfo.PatientTransaction.PatientTransactionDetails
					|| !regInfo.PatientTransaction.PatientTransactionDetails.Any()))
			{
				return;
			}
			if (null == regstDic)
			{
				regstDic = new ConcurrentDictionary<string, decimal>();
			}

			foreach (PatientTransactionDetail _PtTrxDetail in
				regInfo.PatientTransaction.PatientTransactionDetails.ToList())
			{
				if (_PtTrxDetail.IsPaided)
				{
					continue;
				}
				string _Key = this.CreateServiceKey(_PtTrxDetail);
				if (String.IsNullOrEmpty(_Key))
				{
					continue;
				}
				decimal _CurPtPayment = _PtTrxDetail.PatientPayment;
				if (0 == _CurPtPayment)
				{
					continue;
				}
                if (null != _PtTrxDetail.DiscountAmt
                        && _PtTrxDetail.DiscountAmt != 0 && _PtTrxDetail.DiscountAmt < 0)
                {
                    _CurPtPayment = _CurPtPayment - _PtTrxDetail.DiscountAmt.Value;
                }
				regstDic.TryGetValue(_Key, out decimal ptPayment);
                if (_CurPtPayment < 0)
                {
                    if ((_CurPtPayment * -1) == ptPayment)
                    {
                        _CurPtPayment = ptPayment;
                    }
                    else if (((_CurPtPayment * -1) != ptPayment))
                    {
                        _CurPtPayment = (_CurPtPayment + ptPayment);
                    }
                }
                else
                {
                    _CurPtPayment = (_CurPtPayment == ptPayment)
                        ? ptPayment : (_CurPtPayment + ptPayment);
                }
                
                if (_CurPtPayment < 0)
                {
                    _CurPtPayment = _CurPtPayment * -1;
                }
				regstDic.AddOrUpdate(_Key, _CurPtPayment);
			}
		}

		private decimal GetOutwardDrugTotalPatientPayment(ObservableCollection<OutwardDrug> outwardDrugs)
		{
			if (null == outwardDrugs || !outwardDrugs.Any())
			{
				return 0;
			}

			decimal _Total = 0;
			outwardDrugs.ToList<OutwardDrug>().ForEach(_OutwardDrug => _Total += _OutwardDrug.TotalPatientPayment);

			return _Total;
		}

		private bool HasPayment(ConcurrentDictionary<string, decimal> regstDic)
		{
			if (null == regstDic || !regstDic.Any())
			{
				return false;
			}
			return regstDic.Where(_Item => _Item.Value > 0)
					.Select(_Item => (KeyValuePair<string, decimal>?)_Item)
					.FirstOrDefault() != null ? true : false;
		}

		private decimal GetTotalPayment(ref List<String> tmpServiceLst,
			ConcurrentDictionary<string, decimal> regstDic)
		{
			if (null == regstDic || !regstDic.Any())
			{
				return 0;
			}

			decimal _Result = 0;
			foreach (KeyValuePair<string, decimal> _KeyValuePair in regstDic)
			{
				if (tmpServiceLst.Contains(_KeyValuePair.Key))
				{
					continue;
				}
				_Result += _KeyValuePair.Value;
				tmpServiceLst.Add(_KeyValuePair.Key);
			}

			return _Result;
		}

		private decimal Round(decimal value)
		{
			decimal _Factor = Convert.ToDecimal(Math.Pow(10, 0));
			int _Sign = Math.Sign(value);
			return Decimal.Truncate(value * _Factor + 0.5m * _Sign) / _Factor;
		}

		private TransactionResponse CanPay(string salt,
			ConcurrentDictionary<string, decimal> hospitalPaymentList,
			ConcurrentDictionary<string, decimal> pharmacyPaymentList,
			bool isHospital, long ptRegistrationId, long staffId, long patientId)
		{
			List<String> tmpServiceLst = new List<string>();
			decimal _TotalPayment = GetTotalPayment(ref tmpServiceLst, hospitalPaymentList);
			_TotalPayment += GetTotalPayment(ref tmpServiceLst, pharmacyPaymentList);
			_TotalPayment = Round(_TotalPayment);
			if (0 >= _TotalPayment)
			{
				return new TransactionResponse {
					IsSucceed = false,
					ResponseCode = "03",
					ResponseDescription = "Số tiền thanh toán không hợp lệ"
				};
			}

			TransactionResponse _Response = null;
			using (var _BankingPaymentService = new BankingPaymentServiceClient())
			{
				_Response = _BankingPaymentService.Func5(
					GenLib.HashUtil.Encrypt(
						String.Format("{0}|{1}||||{2}|{3}|{4}|||||||", staffId,
							isHospital, _TotalPayment, ptRegistrationId, String.Empty),
						salt),
					GenLib.HashUtil.Encrypt(patientId.ToString()));
			}

			return _Response;
		}

		private TransactionResponse CanPay(string salt, bool isHospital,
			decimal totalPayment, long ptRegistrationId, long staffId,
			long patientId)
		{
			if (0 >= totalPayment)
			{
				return new TransactionResponse { IsSucceed = true };
			}

			TransactionResponse _Response = null;
			using (var _BankingPaymentService = new BankingPaymentServiceClient())
			{
				_Response = _BankingPaymentService.Func5(
					GenLib.HashUtil.Encrypt(
						String.Format("{0}|{1}||||{2}|{3}|{4}|||||||", staffId,
							isHospital, totalPayment, ptRegistrationId, String.Empty),
						salt),
					GenLib.HashUtil.Encrypt(patientId.ToString()));
			}

			return _Response;
		}
		// VuTTM End

		private PatientPCLRequestDetail CreateRequestDetailsForPCLExamType(PCLExamType item)
        {
            var details = new PatientPCLRequestDetail();
            if (item != null)
            {
                details.PCLExamType = item;
                details.PCLExamTypeID = item.PCLExamTypeID;
                details.ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
                details.V_ExamRegStatus = (long)details.ExamRegStatus;
                if (item.PCLExamTypeLocations != null && item.PCLExamTypeLocations.Count > 0)
                {
                    details.DeptLocation = item.PCLExamTypeLocations[0].DeptLocation;
                }
            }
            return details;
        }

        public void GetInPatientBillingInvoiceDetails(long billingInvoiceID
            , out List<PatientRegistrationDetail> regDetails
            , out List<PatientPCLRequest> PCLRequestList
            , out List<OutwardDrugClinicDeptInvoice> allInPatientInvoices
            , bool IsUsePriceByNewCer
            , bool IsRecalInSecondTime
            , bool IsPassCheckNonBlockValidPCLExamDate)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all InPatient reg items.", CurrentUser);
                using (DbConnection connection = PatientProvider.Instance.CreateConnection())
                {
                    regDetails = PatientProvider.Instance.GetAllRegistrationDetailsByBillingInvoiceID(billingInvoiceID, connection, null, IsUsePriceByNewCer);
                    //Can lam sang
                    PCLRequestList = PatientProvider.Instance.GetPCLRequestListByBillingInvoiceID(billingInvoiceID, connection, null, IsUsePriceByNewCer, IsRecalInSecondTime, IsPassCheckNonBlockValidPCLExamDate);
                    allInPatientInvoices = PatientProvider.Instance.GetAllInPatientInvoicesByBillingInvoiceID(billingInvoiceID, connection, null);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading  InPatient reg items. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_CANNOT_ADD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼====: 004
        public void GetInPatientBillingInvoiceDetails(long billingInvoiceID
            , out List<PatientRegistrationDetail> regDetails
            , out List<PatientPCLRequest> PCLRequestList
            , out List<OutwardDrugClinicDeptInvoice> allInPatientInvoices
            , bool IsUsePriceByNewCer
            , bool ReCalBillingInv = false
            , long MedServiceItemPriceListID = 0
            , long PCLExamTypePriceListID = 0
            , bool IsRecalInSecondTime = false
            , bool IsPassCheckNonBlockValidPCLExamDate = false)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all InPatient reg items.", CurrentUser);
                using (DbConnection connection = PatientProvider.Instance.CreateConnection())
                {
                    regDetails = PatientProvider.Instance.GetAllRegistrationDetailsByBillingInvoiceID(billingInvoiceID, connection, null, IsUsePriceByNewCer, ReCalBillingInv, MedServiceItemPriceListID);
                    PCLRequestList = PatientProvider.Instance.GetPCLRequestListByBillingInvoiceID(billingInvoiceID, connection, null, IsUsePriceByNewCer, ReCalBillingInv, PCLExamTypePriceListID, true, IsPassCheckNonBlockValidPCLExamDate);
                    allInPatientInvoices = PatientProvider.Instance.GetAllInPatientInvoicesByBillingInvoiceID(billingInvoiceID, connection, null);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading  InPatient reg items. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_CANNOT_ADD, CurrentUser);
                //throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: 004

        public InPatientBillingInvoice GetInPatientBillingInvoice(long billingInvoiceID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all InPatient reg items.", CurrentUser);
                using (DbConnection connection = PatientProvider.Instance.CreateConnection())
                {
                    InPatientBillingInvoice invoice = PatientProvider.Instance.GetInPatientBillingInvoice(billingInvoiceID, connection, null);
                    if (null != invoice)
                    {
                        List<PatientRegistrationDetail> regDetails = PatientProvider.Instance.GetAllRegistrationDetailsByBillingInvoiceID(billingInvoiceID, connection, null, false);
                        if (regDetails != null)
                        {
                            invoice.RegistrationDetails = regDetails.ToObservableCollection();
                        }
                        List<PatientPCLRequest> PCLRequestList = PatientProvider.Instance.GetPCLRequestListByBillingInvoiceID(billingInvoiceID, connection, null, false);
                        if (PCLRequestList != null)
                        {
                            invoice.PclRequests = PCLRequestList.ToObservableCollection();
                        }
                        List<OutwardDrugClinicDeptInvoice> allInPatientInvoices = PatientProvider.Instance.GetAllInPatientInvoicesByBillingInvoiceID(billingInvoiceID, connection, null);
                        if (allInPatientInvoices != null)
                        {
                            invoice.OutwardDrugClinicDeptInvoices = allInPatientInvoices.ToObservableCollection();
                        }
                    }
                    return invoice;


                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading  InPatient reg items. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_CANNOT_ADD, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool SaveDrugs(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice OutwardInvoice, out OutwardDrugInvoice SavedOutwardInvoice)
        {
            try
            {
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

                PatientRegistration registrationInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(registrationID, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);//chi su dung cho ngoai tru

                if (registrationInfo == null)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1695_G1_KgTimThayDKCuaToa));
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.InitNewTxd(registrationInfo, false);
                List<long> newInvoiceIDList;
                paymentProcessor.AddOutwardDrugInvoice(StaffID, CollectorDeptLocID, Apply15HIPercent, OutwardInvoice, out newInvoiceIDList);

                if (newInvoiceIDList != null && newInvoiceIDList.Count > 0)
                {
                    OutwardInvoice.outiID = newInvoiceIDList[0];
                    SavedOutwardInvoice = OutwardInvoice;
                    SavedOutwardInvoice.OutwardDrugs = RefDrugGenericDetailsProvider.Instance.GetOutwardDrugDetailsByOutwardInvoice(OutwardInvoice.outiID).ToObservableCollection();
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
                }
                else
                {
                    throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                }
            }
        }

        //HPT 24/08/2015: Thêm parameter IsHICard_FiveYearsCont nhận giá trị truyền vào từ hàm gọi ở client để thực hiện tính quyền lợi bảo hiểm
        //Bệnh nhân có đăng ký bảo hiểm y tế đã hợp lệ nếu có giá trị IsHICard_FiveYearsCont = true sẽ được hưởng quyền lợi bảo hiểm 100%
        public double CalculateHiBenefit(HealthInsurance healthInsurance, PaperReferal paperReferal, out bool isConsideredAsCrossRegion, bool IsEmergency = false, long V_RegistrationType = 0, bool IsEmergInPtReExamination = false, bool IsHICard_FiveYearsCont = false, bool IsChildUnder6YearsOld = false, bool IsAllowCrossRegion = false)
        {
            HiPatientRegAndPaymentProcessor processor = new HiPatientRegAndPaymentProcessor();
            PatientRegistration regInfo = new PatientRegistration();
            regInfo.HealthInsurance = healthInsurance;
            regInfo.PaperReferal = paperReferal;
            regInfo.IsEmergency = IsEmergency;
            regInfo.IsHICard_FiveYearsCont = IsHICard_FiveYearsCont;
            regInfo.IsChildUnder6YearsOld = IsChildUnder6YearsOld;
            regInfo.IsAllowCrossRegion = IsAllowCrossRegion;
            return processor.CalcPatientHiBenefit(regInfo, regInfo.HealthInsurance, out isConsideredAsCrossRegion, V_RegistrationType, IsEmergInPtReExamination, IsAllowCrossRegion);
        }
        /*==== #002 ====*/
        public double CalculateHiBenefit_V2(HealthInsurance healthInsurance, PaperReferal paperReferal, out bool isConsideredAsCrossRegion, bool IsEmergency = false, long V_RegistrationType = 0, bool IsEmergInPtReExamination = false, bool IsHICard_FiveYearsCont = false, bool IsChildUnder6YearsOld = false, bool IsAllowCrossRegion = false, bool IsHICard_FiveYearsCont_NoPaid = false)
        {
            HiPatientRegAndPaymentProcessor processor = new HiPatientRegAndPaymentProcessor();
            PatientRegistration regInfo = new PatientRegistration();
            regInfo.HealthInsurance = healthInsurance;
            regInfo.PaperReferal = paperReferal;
            regInfo.IsEmergency = IsEmergency;
            regInfo.IsHICard_FiveYearsCont = IsHICard_FiveYearsCont;
            regInfo.IsChildUnder6YearsOld = IsChildUnder6YearsOld;
            regInfo.IsAllowCrossRegion = IsAllowCrossRegion;
            regInfo.IsHICard_FiveYearsCont_NoPaid = IsHICard_FiveYearsCont_NoPaid;
            return processor.CalcPatientHiBenefit(regInfo, regInfo.HealthInsurance, out isConsideredAsCrossRegion, V_RegistrationType, IsEmergInPtReExamination, IsAllowCrossRegion);
        }
        /*==== #002 ====*/
        //Dung de tinh tien cho truong hop Save And Pay
        public void CalcInvoiceForSaveAndPayButton(OutwardDrugInvoice OutwardInvoice, out OutwardDrugInvoice CalculatedInvoice, out PatientRegistration RegistrationInfo)
        {
            throw new Exception(eHCMSResources.Z1785_G1_NotImplemented);
        }

        public List<PatientRegistrationDetail> CheckServiceExist(List<PatientRegistrationDetail> regDetails, List<PatientRegistrationDetail> regDetailList)
        {
            //loc danh sach regDetailList ngoai tru nhung phan tu nam trong regDetails
            if (regDetailList == null || regDetailList.Count < 1)
            {
                return null;
            }
            if (regDetails == null || regDetails.Count < 1)
            {
                return regDetailList;
            }
            return regDetailList.Where(
                u => regDetailList.Select(i => i.PtRegDetailID).Except(regDetails.
                    Select(l => l.PtRegDetailID)).Contains(u.PtRegDetailID)).ToList();
        }

        public List<PatientRegistrationDetail> CheckServiceHasPaidExist(List<PatientRegistrationDetail> aDetailList, List<PatientRegistrationDetail> aModifyList, PatientRegistration aPatientRegistration, bool isDoing = false)
        {
            if (aDetailList == null || aModifyList == null)
            {
                return null;
            }
            if (isDoing)
            {
                if (Globals.AxServerSettings.CommonItems.ChangeHIAfterSaveAndPayRule)
                {
                    //▼===== 20200105 TTM: Bổ sung loại bỏ tình trạng đã hoàn tất dịch vụ. Nếu không bổ sung nó sẽ nhận biết dịch vụ này là dịch vụ huỷ => Báo lỗi.
                    return aModifyList.Where(u => aModifyList.Select(i => i.PtRegDetailID).Intersect(aDetailList.Where(s => s.PaidTime != null 
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.DANG_KY_KHAM 
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI 
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.HOAN_TAT).Select(l => l.PtRegDetailID)).Contains(u.PtRegDetailID)).ToList();
                }
                else
                {
                    return aModifyList.Where(u => aModifyList.Select(i => i.PtRegDetailID).Intersect(aDetailList.Where(s => s.PaidTime != null 
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.DANG_KY_KHAM && s.ExamRegStatus != AllLookupValues.ExamRegStatus.BAT_DAU_THUC_HIEN
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.HOAN_TAT
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI).Select(l => l.PtRegDetailID)).Contains(u.PtRegDetailID)).ToList();
                }

            }
            else
            {
                return aModifyList.Where(u => aModifyList.Select(i => i.PtRegDetailID).Intersect(aDetailList.Where(s => s.PaidTime != null)
                    .Select(l => l.PtRegDetailID)).Contains(u.PtRegDetailID)).ToList();
            }

        }

        public List<PatientPCLRequestDetail> CheckPCLExist(List<PatientPCLRequest> regPCLInfo, List<PatientPCLRequestDetail> regPCLDetails, bool isDoing = false)
        {
            if (regPCLDetails == null || regPCLDetails.Count < 1)
            {
                return null;
            }
            if (regPCLInfo == null || regPCLInfo.Count < 1)
            {
                return regPCLDetails;
            }
            List<PatientPCLRequestDetail> temp = new List<PatientPCLRequestDetail>();
            foreach (var item in regPCLInfo)
            {
                temp.AddRange(regPCLDetails.Where(u => regPCLDetails.Select(i => i.PCLReqItemID).Except(item.PatientPCLRequestIndicators.Select(k => k.PCLReqItemID)).Contains(u.PCLReqItemID)).ToList());
            }
            return temp;
        }

        public List<PatientPCLRequestDetail> CheckPCLPaidExist(List<PatientPCLRequest> aDetailListCollection, List<PatientPCLRequestDetail> aModifyList, PatientRegistration aPatientRegistration, bool isDoing = false)
        {
            if (aDetailListCollection == null || aModifyList == null)
            {
                return null;
            }
            List<PatientPCLRequestDetail> aDetailList = aDetailListCollection.SelectMany(x => x.PatientPCLRequestIndicators).ToList();
            aModifyList = aModifyList.Where(x => x.RecordState == RecordState.MODIFIED).ToList();
            if (isDoing)
            {
                //▼===== 20200114 TTM: Bổ sung loại bỏ tình trạng đã hoàn tất dịch vụ. Nếu không bổ sung nó sẽ nhận biết dịch vụ này là dịch vụ huỷ => Báo lỗi.
                //                     Và vì lý do đã hoàn tất thì không bao giờ huỷ được nên mở ra không sao cả.
                if (Globals.AxServerSettings.CommonItems.ChangeHIAfterSaveAndPayRule)
                {
                    return aModifyList.Where(u => aModifyList.Select(i => i.PCLReqItemID).Intersect(aDetailList.Where(s => s.PaidTime != null 
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.DANG_KY_KHAM 
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.HOAN_TAT
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.XOA_TRA_TIEN_LAI).Select(l => l.PCLReqItemID)).Contains(u.PCLReqItemID)).ToList();
                }
                else
                {
                    return aModifyList.Where(u => aModifyList.Select(i => i.PCLReqItemID).Intersect(aDetailList.Where(s => s.PaidTime != null 
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.DANG_KY_KHAM 
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                    && s.ExamRegStatus != AllLookupValues.ExamRegStatus.XOA_TRA_TIEN_LAI).Select(l => l.PCLReqItemID)).Contains(u.PCLReqItemID)).ToList();
                }
                //▲===== 
            }
            else
            {
                return aModifyList.Where(u => aModifyList.Select(i => i.PCLReqItemID).Intersect(aDetailList.Where(s => s.PaidTime != null)
                    .Select(l => l.PCLReqItemID)).Contains(u.PCLReqItemID)).ToList();
            }
            //List<PatientPCLRequestDetail> temp = new List<PatientPCLRequestDetail>();
            //foreach (var item in aDetailListCollection)
            //{
            //    if (isDoing)
            //    {
            //        temp.AddRange(aModifyList.Where(u => aModifyList.Select(i => i.PCLReqItemID).Intersect(item.PatientPCLRequestIndicators.Where(s => s.PaidTime != null && s.ExamRegStatus != AllLookupValues.ExamRegStatus.DANG_KY_KHAM && s.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            //            .Select(k => k.PCLReqItemID)).Contains(u.PCLReqItemID)).ToList());
            //    }
            //    else
            //    {
            //        temp.AddRange(aModifyList.Where(u => aModifyList.Select(i => i.PCLReqItemID).Intersect(item.PatientPCLRequestIndicators.Where(s => s.PaidTime != null)
            //            .Select(k => k.PCLReqItemID)).Contains(u.PCLReqItemID)).ToList());
            //    }
            //}
            //return temp;
        }

        public List<PatientPCLRequest> CheckPCLPaidExist(List<PatientPCLRequest> regPCLInfo, List<PatientPCLRequest> regPCLDetails, PatientRegistration aPatientRegistration, bool isDoing = false)
        {
            List<PatientPCLRequest> temps = new List<PatientPCLRequest>();
            foreach (var item in regPCLDetails)
            {
                var pclDetails = CheckPCLPaidExist(regPCLInfo, item.PatientPCLRequestIndicators.ToList(), aPatientRegistration, isDoing);
                if (pclDetails != null && pclDetails.Count > 0)
                {
                    var temp = item;
                    temp.PatientPCLRequestIndicators = new List<PatientPCLRequestDetail>().ToObservableCollection();
                    temp.PatientPCLRequestIndicators.AddRange(pclDetails);
                    temps.Add(temp);
                }
            }
            return temps;
        }

        private PatientRegistration GetRegistrationAndOtherDetails(long nPtRegistrationID, int nFindPatient, bool bGetRegDetails, bool bGetPCLReqDetails)
        {
            PatientRegistration regInfo = PatientProvider.Instance.GetRegistration(nPtRegistrationID, nFindPatient);
            List<PatientPCLRequest> pclRequestList = null;
            List<PatientRegistrationDetail> regDetails = null;
            using (DbConnection connection = PatientProvider.Instance.CreateConnection())
            {
                if (bGetRegDetails)
                {
                    regDetails = PatientProvider.Instance.GetAllRegistrationDetails(nPtRegistrationID, nFindPatient, connection, null);
                    if (regDetails != null)
                    {
                        regInfo.PatientRegistrationDetails = new List<PatientRegistrationDetail>().ToObservableCollection();
                        foreach (var item in regDetails)
                        {
                            regInfo.PatientRegistrationDetails.Add(item);
                        }
                    }
                }
                if (bGetPCLReqDetails)
                {
                    pclRequestList = PatientProvider.Instance.GetPCLRequestListByRegistrationID(nPtRegistrationID, connection, null);
                    if (pclRequestList != null)
                    {
                        regInfo.PCLRequests = pclRequestList.ToObservableCollection();
                    }
                }
            }
            return regInfo;
        }
        private DateTime ApplyValidMedicalInstructionDate(DateTime aMedicalInstructionDate, PatientRegistration aPatientRegistration)
        {
            aMedicalInstructionDate = aMedicalInstructionDate.Date;
            if (aPatientRegistration == null) return aMedicalInstructionDate;
            if (aPatientRegistration.AdmissionInfo != null
                && aPatientRegistration.AdmissionInfo.AdmissionDate.HasValue && aPatientRegistration.AdmissionInfo.AdmissionDate != null
                && aMedicalInstructionDate < aPatientRegistration.AdmissionInfo.AdmissionDate)
                aMedicalInstructionDate = aMedicalInstructionDate.Date.Add(TimeSpan.Parse(aPatientRegistration.AdmissionInfo.AdmissionDate.Value.AddSeconds(1).ToString("HH:mm:ss")));
            else if (aPatientRegistration.AdmissionDate.HasValue && aPatientRegistration.AdmissionDate != null
                && aMedicalInstructionDate < aPatientRegistration.AdmissionDate)
                aMedicalInstructionDate = aMedicalInstructionDate.Date.Add(TimeSpan.Parse(aPatientRegistration.AdmissionDate.Value.AddSeconds(1).ToString("HH:mm:ss")));
            else if ((aPatientRegistration.AdmissionInfo == null || !aPatientRegistration.AdmissionInfo.AdmissionDate.HasValue || aPatientRegistration.AdmissionInfo.AdmissionDate == null)
                && aPatientRegistration.ExamDate != null
                && aMedicalInstructionDate < aPatientRegistration.ExamDate)
                aMedicalInstructionDate = aMedicalInstructionDate.Date.Add(TimeSpan.Parse(aPatientRegistration.ExamDate.AddSeconds(1).ToString("HH:mm:ss")));
            return aMedicalInstructionDate;
        }
        public void AddPCLRequestsForInPt(long StaffID, long ReqFromDeptLocID, long ReqFromDeptID, PatientRegistration regInfo, PatientPCLRequest pclRequest, PatientPCLRequest deletedPclRequest, bool IsNotCheckInvalid,
                        out List<PatientPCLRequest> SavedPclRequestList, DateTime modifiedDate = default(DateTime))
        {
            try
            {
                SavedPclRequestList = null;

                AxLogger.Instance.LogInfo("Start registering patient.", CurrentUser);
                List<long> newPclRequestList = null;
                bool bLoadRegInforequired = true;
                if (regInfo.PtRegistrationID > 0)
                {
                    regInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(regInfo.PtRegistrationID, regInfo.FindPatient, true);
                    bLoadRegInforequired = false; // Registration Info already loaded here NO NEED to load it again in paymentProcessor (RegAndPaymentProcessorBase) below
                }
                if (regInfo == null)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z0083_G1_KhongTimThayDK));
                }
                if (regInfo.ExamDate == DateTime.MinValue)
                {
                    regInfo.ExamDate = DateTime.Now;
                }
                if (modifiedDate == default(DateTime))
                {
                    modifiedDate = regInfo.ExamDate;
                }
                if (pclRequest != null && pclRequest.PatientPCLRequestIndicators != null && pclRequest.PatientPCLRequestIndicators.Count() > 0)
                {
                    if (pclRequest.CreatedDate == DateTime.MinValue)
                    {
                        pclRequest.CreatedDate = DateTime.Now;
                    }
                    if (pclRequest.PatientPCLRequestIndicators != null)
                    {
                        foreach (var requestDetail in pclRequest.PatientPCLRequestIndicators)
                        {
                            if (requestDetail.CreatedDate == DateTime.MinValue)
                            {
                                requestDetail.CreatedDate = DateTime.Now;
                            }
                        }
                    }
                }
                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
                paymentProcessor.InitNewTxd(regInfo, bLoadRegInforequired); // Bước này sẽ gán dữ liệu từ regInfo sang cho CurrentRegistration
                RetryOnDatabaseDeadlock.RetryUntil(() =>
                {
                    paymentProcessor.AddPCLRequestsForInPt(StaffID, ReqFromDeptLocID, ReqFromDeptID, pclRequest, deletedPclRequest, IsNotCheckInvalid, modifiedDate, out newPclRequestList);
                }, 5);

                if (newPclRequestList != null)
                {
                    SavedPclRequestList = PatientProvider.Instance.GetPCLRequestListByIDList(newPclRequestList, (long)AllLookupValues.RegistrationType.NOI_TRU);
                }
                AxLogger.Instance.LogInfo("End of registering patient.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddServicesAndPCLRequestsInPt. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddServicesAndPCLRequests, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▼===== 20200729 TTM: Comment lại để viết hàm khác không load lại dữ liệu khi lưu và trả tiề.
        //public PatientRegistration AddServicesAndPCLRequests(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration regInfo, List<PatientRegistrationDetail> regDetailList, List<PatientPCLRequest> pclRequestList, List<PatientRegistrationDetail> deletedRegDetailList, List<PatientPCLRequest> deletedPclRequestList, out long NewRegistrationID
        //    , out List<PatientRegistrationDetail> SavedRegistrationDetailList, out List<PatientPCLRequest> SavedPclRequestList, out V_RegistrationError error, DateTime modifiedDate = default(DateTime), bool IsNotCheckInvalid = false, bool IsCheckPaid = false, bool IsProcess = false)
        //{
        //    try
        //    {
        //        SavedRegistrationDetailList = null;
        //        SavedPclRequestList = null;


        //        AxLogger.Instance.LogInfo("Start registering patient.", CurrentUser);
        //        List<long> newRegDetailsList = null;
        //        List<long> newPclRequestList = null;
        //        bool bLoadRegInforequired = true;
        //        error = V_RegistrationError.mNone;
        //        long? AppointmentIDOutTreatment = 0;
        //        if (regInfo.PtRegistrationID > 0)
        //        {
        //            var bkPromoDiscountProgramObj = regInfo.PromoDiscountProgramObj;

        //            //▼===== 20200628 TTM:  Clone lại AppointmentID vì getRegistrationTxd lấy lại đăng ký nên không đọc AppointmentID lên lại.
        //            //                      Sau đó gán lại để cập nhật tình trạng cuộc hẹn.
        //            if (regInfo.OutPtTreatmentProgramID > 0)
        //            {
        //                AppointmentIDOutTreatment = regInfo.AppointmentID;
        //            }

        //            regInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(regInfo.PtRegistrationID, regInfo.FindPatient, true, IsProcess);

        //            regInfo.AppointmentID = AppointmentIDOutTreatment;

        //            if (bkPromoDiscountProgramObj != null && regInfo.PromoDiscountProgramObj == null)
        //            {
        //                bkPromoDiscountProgramObj.RecordState = RecordState.ADDED;
        //                regInfo.PromoDiscountProgramObj = bkPromoDiscountProgramObj;
        //            }
        //            else if (bkPromoDiscountProgramObj != null && !regInfo.PromoDiscountProgramObj.CompareValues(bkPromoDiscountProgramObj))
        //            {
        //                bkPromoDiscountProgramObj.RecordState = RecordState.MODIFIED;
        //                regInfo.PromoDiscountProgramObj = bkPromoDiscountProgramObj;
        //            }
        //            else if (regInfo.PromoDiscountProgramObj != null)
        //            {
        //                regInfo.PromoDiscountProgramObj.RecordState = RecordState.UNCHANGED;
        //            }
        //            //regInfo = GetRegistrationAndOtherDetails(regInfo.PtRegistrationID, regInfo.FindPatient, true, true);
        //            bLoadRegInforequired = false; // Registration Info already loaded here NO NEED to load it again in paymentProcessor (RegAndPaymentProcessorBase) below

        //            //regInfo = PatientProvider.Instance.GetRegistration(regInfo.PtRegistrationID, regInfo.FindPatient);
        //            //regInfo = GetRegistrationInfo(regInfo.PtRegistrationID, regInfo.FindPatient);
        //        }

        //        if (regInfo == null)
        //        {
        //            throw new Exception(string.Format("{0}.", eHCMSResources.Z0083_G1_KhongTimThayDK));
        //        }
        //        if (regInfo.ExamDate == DateTime.MinValue)
        //        {
        //            regInfo.ExamDate = DateTime.Now;
        //        }
        //        if (modifiedDate == default(DateTime))
        //        {
        //            modifiedDate = regInfo.ExamDate;
        //        }
        //        //b2d Phải Kiểm Tra thêm ở đây một lần nữa
        //        //xem dịch vụ cls bị xóa đã có tính tiền chưa thì tra ve error
        //        //b2d kiểm tra những danh sách xóa
        //        if (deletedRegDetailList != null && deletedRegDetailList.Count > 0)
        //        {
        //            var deletedRegDetailPaidList = CheckServiceHasPaidExist(regInfo.PatientRegistrationDetails.ToList(), deletedRegDetailList, regInfo);
        //            if (deletedRegDetailPaidList != null && deletedRegDetailPaidList.Count > 0)
        //            {
        //                error = V_RegistrationError.mRefresh;
        //                NewRegistrationID = 0;
        //                return null;
        //            }
        //        }

        //        if (deletedPclRequestList != null && deletedPclRequestList.Count > 0 && regInfo.PCLRequests != null && regInfo.PCLRequests.Count > 0)
        //        {
        //            var deletedPclRequestPaidList = CheckPCLPaidExist(regInfo.PCLRequests.ToList(), deletedPclRequestList, regInfo);
        //            if (deletedPclRequestPaidList != null && deletedPclRequestPaidList.Count > 0)
        //            {
        //                error = V_RegistrationError.mRefresh;
        //                NewRegistrationID = 0;
        //                return null;
        //            }
        //        }

        //        ////------kiem tra nhung danh sach them--------------------

        //        if (regDetailList != null)
        //        {
        //            //20200222 TBL Mod TMV1: Set PaidTime = null cho những dịch vụ được lấy từ gói
        //            if (Globals.AxServerSettings.OutRegisElements.IsPerformingTMVFunctionsA && regInfo.PatientRegistrationDetails.ToList() != null && regInfo.PatientRegistrationDetails.ToList().Count > 0)
        //            {
        //                foreach (PatientRegistrationDetail item in regInfo.PatientRegistrationDetails.ToList())
        //                {
        //                    if (item.PaidTimeTmp != null)
        //                    {
        //                        item.PaidTime = null;
        //                    }
        //                }
        //            }
        //            //var addRegDetailPaidList = CheckServiceHasPaidExist(regInfo.PatientRegistrationDetails.ToList(), regDetailList, regInfo);
        //            //if (addRegDetailPaidList != null && addRegDetailPaidList.Count > 0)
        //            //{
        //            //    error = V_RegistrationError.mRefresh;
        //            //    NewRegistrationID = 0;
        //            //    return null;
        //            //}
        //            foreach (var patientRegistrationDetail in regDetailList)
        //            {
        //                if (patientRegistrationDetail.CreatedDate == DateTime.MinValue)
        //                {
        //                    //patientRegistrationDetail.CreatedDate = regInfo.ExamDate;
        //                    patientRegistrationDetail.CreatedDate = DateTime.Now;
        //                }
        //            }
        //        }

        //        if (pclRequestList != null)
        //        {
        //            if (regInfo.PCLRequests == null)
        //            {
        //                regInfo.PCLRequests = new System.Collections.ObjectModel.ObservableCollection<PatientPCLRequest>();
        //            }
        //            //var addPclRequestPaidList = CheckPCLPaidExist(regInfo.PCLRequests.ToList(), pclRequestList, regInfo);
        //            //if (addPclRequestPaidList != null && addPclRequestPaidList.Count > 0)
        //            //{
        //            //    error = V_RegistrationError.mRefresh;
        //            //    NewRegistrationID = 0;
        //            //    return null;
        //            //}
        //            foreach (var pclRequest in pclRequestList)
        //            {
        //                if (pclRequest.CreatedDate == DateTime.MinValue)
        //                {
        //                    pclRequest.CreatedDate = DateTime.Now;
        //                    //pclRequest.CreatedDate = regInfo.ExamDate;
        //                }
        //                if (pclRequest.PatientPCLRequestIndicators != null)
        //                {
        //                    foreach (var requestDetail in pclRequest.PatientPCLRequestIndicators)
        //                    {
        //                        if (requestDetail.CreatedDate == DateTime.MinValue)
        //                        {
        //                            requestDetail.CreatedDate = DateTime.Now;
        //                            //requestDetail.CreatedDate = regInfo.ExamDate;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
        //        paymentProcessor.InitNewTxd(regInfo, bLoadRegInforequired);
        //        long id = regInfo.PtRegistrationID > 0 ? regInfo.PtRegistrationID : -1;


        //        RetryOnDatabaseDeadlock.RetryUntil(() =>
        //        {
        //            paymentProcessor.AddServicesAndPCLRequests(StaffID, CollectorDeptLocID, Apply15HIPercent, regInfo, regDetailList, pclRequestList, deletedRegDetailList, deletedPclRequestList, modifiedDate, IsNotCheckInvalid, IsCheckPaid, out id, out newRegDetailsList, out newPclRequestList, IsProcess);
        //        }, 5);

        //        NewRegistrationID = id;



        //        //▼===== 20200213 TTM:  Loại bỏ khúc code này, vì xuống DB lấy dữ liệu để gán vào bảng PatientQueue nhưng khúc code gán giá trị cho bảng PatientQueue đã bị xoá ra rồi,
        //        //                      Dẫn đến khúc code get set dữ liệu này làm không mục đích => Loại bỏ.
        //        //-------- Code chỗ này vô dụng, gọi về DB làm gì để lấy danh sách gán mà không gán?
        //        if (newRegDetailsList != null)
        //        {
        //            //Lay danh sach dich vu vua add.
        //            SavedRegistrationDetailList = PatientProvider.Instance.GetAllRegistrationDetailsByIDList(newRegDetailsList);
        //        }
        //        if (newPclRequestList != null)
        //        {
        //            SavedPclRequestList = PatientProvider.Instance.GetPCLRequestListByIDList(newPclRequestList);
        //        }
        //        AxLogger.Instance.LogInfo("End of registering patient.", CurrentUser);

        //        ////Add vao QUEUE

        //        //PatientQueue queue;
        //        //if (SavedRegistrationDetailList != null)
        //        //{
        //        //    foreach (var item in SavedRegistrationDetailList)
        //        //    {
        //        //        queue = new PatientQueue();
        //        //        queue.RegistrationID = NewRegistrationID;//regInfo.PtRegistrationID;
        //        //        queue.RegistrationDetailsID = item.PtRegDetailID;
        //        //        queue.V_QueueType = (long)AllLookupValues.QueueType.THANH_TOAN_TIEN_KHAM;
        //        //        queue.V_PatientQueueItemsStatus = (long)AllLookupValues.PatientQueueItemsStatus.WAITING;
        //        //        queue.DeptLocID = item.DeptLocID;
        //        //        queue.FullName = regInfo.Patient.FullName;
        //        //        queue.PatientID = regInfo.Patient.PatientID;
        //        //        //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
        //        //        //PatientProvider.Instance.PatientQueue_Insert(queue);
        //        //    }
        //        //}

        //        //if (SavedPclRequestList != null)
        //        //{
        //        //    foreach (var item in SavedPclRequestList)
        //        //    {
        //        //        queue = new PatientQueue();
        //        //        queue.RegistrationID = NewRegistrationID;// regInfo.PtRegistrationID;
        //        //        queue.PatientPCLReqID = item.PatientPCLReqID;
        //        //        queue.V_QueueType = (long)AllLookupValues.QueueType.THANH_TOAN_TIEN_KHAM;
        //        //        queue.V_PatientQueueItemsStatus = (long)AllLookupValues.PatientQueueItemsStatus.WAITING;
        //        //        queue.DeptLocID = item.PCLDeptLocID;
        //        //        queue.FullName = regInfo.Patient.FullName;
        //        //        queue.PatientID = regInfo.Patient.PatientID;
        //        //        //KMx: Anh Tuấn không cho insert vào bảng PatientQueue.
        //        //        //PatientProvider.Instance.PatientQueue_Insert(queue);
        //        //    }
        //        //}
        //        //-----------------------------
        //        //▲===== 20200213 TTM


        //        //Update lai Paperreferal ID
        //        if (regInfo.PaperReferal != null
        //            && regInfo.PaperReferal.RefID > 0
        //            && (regInfo.PaperReferal.PtRegistrationID == null
        //                || regInfo.PaperReferal.PtRegistrationID < 1))
        //        {
        //            regInfo.PaperReferal.PtRegistrationID = NewRegistrationID;
        //            PatientProvider.Instance.UpdatePaperReferalRegID(regInfo.PaperReferal);
        //        }

        //        // TxD 01/01/2014 : Return PatientRegistration HERE to SAVE the Client a TRIP to CALL and GetRegistrationInfo

        //        PatientRegistration newRegInfoReloaded = RegAndPaymentProcessorBase.GetRegistrationTxd(NewRegistrationID, regInfo.FindPatient, true, IsProcess);

        //        return newRegInfoReloaded;

        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of AddServicesAndPCLRequests. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddServicesAndPCLRequests, CurrentUser);

        //        throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
        //    }
        //}
        //▲===== 20200729 TTM:
        public bool ConfirmRegistrationHIBenefit(long PtRegistrationID, long? StaffID, bool IsUpdateHisID, long? aHIID, double PtInsuranceBenefit)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ConfirmRegistrationHIBenefit.", CurrentUser);
                long? HisID = null;
                if (IsUpdateHisID && aHIID > 0)
                {
                    HisID = PatientProvider.Instance.ActiveHisID(new HealthInsurance { HIID = aHIID.Value });
                }
                bool mReturnValue = PaymentProvider.Instance.ConfirmRegistrationHIBenefit(PtRegistrationID, StaffID, IsUpdateHisID, HisID, PtInsuranceBenefit);
                AxLogger.Instance.LogInfo("End of ConfirmRegistrationHIBenefit. Status: Success.", CurrentUser);
                return mReturnValue;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ConfirmRegistrationHIBenefit. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddServicesAndPCLRequests, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public void AddPCLRequest(long StaffID, long patientID, long registrationID, PatientPCLRequest pclRequest, out PatientPCLRequest SavedPclRequest)
        {
            try
            {
                SavedPclRequest = null;
                if (patientID <= 0 && registrationID <= 0)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1792_G1_ThSoTruyenVaoKgDung));
                }
                AxLogger.Instance.LogInfo("Start adding pcl Request.", CurrentUser);
                long newPclRequest;
                if (registrationID > 0)
                {
                    int PatientFind = 0;
                    if (pclRequest.V_RegistrationType == (int)AllLookupValues.RegistrationType.NOI_TRU)
                    {
                        PatientFind = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
                    }

                    PatientRegistration regInfo = PatientProvider.Instance.GetRegistration(registrationID, PatientFind);
                    if (regInfo == null)
                    {
                        throw new Exception(string.Format("{0}.", eHCMSResources.Z0083_G1_KhongTimThayDK));
                    }
                    RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
                    paymentProcessor.AddPCLRequest(StaffID, regInfo, pclRequest, out newPclRequest);
                }
                else //Thêm yêu cầu CLS không không cho bệnh nhân.
                {
                    NormalPatientRegAndPaymentProcessor paymentProcessor = new NormalPatientRegAndPaymentProcessor();
                    paymentProcessor.AddPCLRequestForNonRegisteredPatient(patientID, pclRequest, pclRequest.V_RegistrationType, out newPclRequest);

                }

                if (newPclRequest > 0)
                {
                    List<long> ids = new List<long>() { newPclRequest };
                    List<PatientPCLRequest> requests = PatientProvider.Instance.GetPCLRequestListByIDList(ids);
                    if (requests != null && requests.Count > 0)
                    {
                        SavedPclRequest = requests[0];
                    }
                }
                AxLogger.Instance.LogInfo("End of adding pcl Request.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of adding pcl Request. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddPCLRequest, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void RemovePaidRegItems(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient,
                        List<PatientRegistrationDetail> colPaidRegDetails,
                        List<PatientPCLRequest> colPaidPclRequests,
                        List<OutwardDrugInvoice> colPaidDrugInvoice,
                        List<OutwardDrugClinicDeptInvoice> colPaidMedItemList,
                        List<OutwardDrugClinicDeptInvoice> colPaidChemicalItem
                        , out V_RegistrationError error)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start removing registered items.", CurrentUser);
                //Tim lai dang ky, neu chua co dang ky thi bao loi.
                //Neu dang ky nay chua co transaction cung khong duoc. Vi day la truong hop xoa nhung item da tinh tien roi -> da ton tai Transaction
                bool bLoadRegInforequired = true;
                error = V_RegistrationError.mNone;
                PatientRegistration regInfo = null;
                try
                {
                    //registrationInfo = PatientProvider.Instance.GetRegistration(registrationID, FindPatient);
                    regInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(registrationID, FindPatient, true);
                    //regInfo = GetRegistrationAndOtherDetails(registrationID, FindPatient, true, true);
                    bLoadRegInforequired = false; // Registration Info already loaded here NO NEED to load it again in paymentProcessor (RegAndPaymentProcessorBase) below
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                if (regInfo == null)//|| registrationInfo.PatientTransaction == null
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1700_G1_KgTimThayGDichCuaDK));
                }

                //b2d Phải Kiểm Tra thêm ở đây một lần nữa
                //xem dịch vụ cls bị xóa đã có tính tiền chưa thì tra ve error
                //b2d kiểm tra những danh sách xóa
                if (colPaidRegDetails != null && colPaidRegDetails.Count > 0)
                {
                    var deletedRegDetailPaidList = CheckServiceHasPaidExist(regInfo.PatientRegistrationDetails.ToList(), colPaidRegDetails, regInfo, true);
                    if (deletedRegDetailPaidList != null && deletedRegDetailPaidList.Count > 0)
                    {
                        error = V_RegistrationError.mRefresh;
                        return;
                    }
                }

                if (colPaidPclRequests != null && colPaidPclRequests.Count > 0
                    && regInfo.PCLRequests != null && regInfo.PCLRequests.Count > 0)
                {
                    var deletedPclRequestPaidList = CheckPCLPaidExist(regInfo.PCLRequests.ToList(), colPaidPclRequests, regInfo, true);
                    if (deletedPclRequestPaidList != null && deletedPclRequestPaidList.Count > 0)
                    {
                        error = V_RegistrationError.mRefresh;
                        return;
                    }
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
                paymentProcessor.InitNewTxd(regInfo, bLoadRegInforequired);

                paymentProcessor.RemovePaidRegItems(StaffID, CollectorDeptLocID, Apply15HIPercent, regInfo, colPaidRegDetails, colPaidPclRequests,
                                                    colPaidDrugInvoice, colPaidMedItemList, colPaidChemicalItem);
                AxLogger.Instance.LogInfo("End of removing registered items.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of removing registered items. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_RemovePaidRegItems, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Outward">Thực ra Ny gửi lên ở đây là Phiếu xuất chứ không phải phiểu trả</param>
        /// <param name="Details"></param>
        /// <param name="outiID"></param>
        /// <returns></returns>
        public bool AddOutwardDrugReturn(long StaffID, int? Apply15HIPercent, OutwardDrugInvoice Outward, List<OutwardDrug> Details, out long outiID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start returning outward drug invoice.", CurrentUser);

                //Tim lai dang ky, neu chua co dang ky thi bao loi.
                //Neu dang ky nay chua co transaction cung khong duoc. Vi day la truong hop xoa nhung item da tinh tien roi -> da ton tai Transaction
                PatientRegistration registrationInfo = null;
                if (!Outward.PtRegistrationID.HasValue)
                {
                    throw new Exception(eHCMSResources.Z1793_G1_KgCoDK);
                }
                try
                {
                    //registrationInfo = PatientProvider.Instance.GetRegistration(Outward.PtRegistrationID.Value
                    //    , (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);
                    registrationInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(Outward.PtRegistrationID.Value, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU, true);
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                if (registrationInfo == null || registrationInfo.PatientTransaction == null)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1700_G1_KgTimThayGDichCuaDK));
                }
                var paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
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
        public bool CancelOutwardDrugInvoice(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice invoice)
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
                    registrationInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(invoice.PtRegistrationID.Value, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                if (registrationInfo == null || registrationInfo.PatientTransaction == null)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1700_G1_KgTimThayGDichCuaDK));
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.InitNewTxd(registrationInfo, false);
                paymentProcessor.CancelOutwardDrugInvoice(StaffID, CollectorDeptLocID, Apply15HIPercent, invoice, null);
                AxLogger.Instance.LogInfo("End of returning outward drug invoice.", CurrentUser);
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of returning outward drug invoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CancelOutwardDrugInvoice);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #region Ny them member
        public bool AddTransactionForDrug(PatientTransactionPayment payment, long outiID, long V_TranRefType, out long PaymentID)//PatientPayment payment,
        {
            try
            {
                AxLogger.Instance.LogInfo("Start AddTransactionForDrug.", CurrentUser);

                bool OK = PaymentProvider.Instance.AddTransactionForDrug(payment, outiID, V_TranRefType, out PaymentID);

                AxLogger.Instance.LogInfo("End of AddTransactionForDrug. Status: Success.", CurrentUser);
                return OK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddTransactionForDrug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddTransactionForDrug, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        public void PayForInPatientRegistration(long StaffID, long registrationID, PatientTransactionPayment paymentDetails,//PatientPayment paymentDetails,
                                                List<InPatientBillingInvoice> billingInvoices
                                                , out PatientTransaction Transaction, out PatientTransactionPayment PaymentInfo, out long PtCashAdvanceID)//out PatientPayment PaymentInfo
        {
            try
            {
                if (registrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }
                AxLogger.Instance.LogInfo("Start processing payment for In-patient registration.", CurrentUser);
                PatientRegistration registrationInfo = null;
                try
                {
                    registrationInfo = PatientProvider.Instance.GetRegistration(registrationID
                        , (int)AllLookupValues.V_FindPatientType.NOI_TRU);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                Transaction = null;
                PaymentInfo = null;
                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                var paidTime = DateTime.Now;
                if (billingInvoices != null)
                {
                    foreach (var inv in billingInvoices)
                    {
                        if (inv.PaidTime == null)
                        {
                            inv.PaidTime = paidTime;
                        }
                    }
                }

                paymentProcessor.PayForInPatientRegistration(StaffID, registrationInfo, paymentDetails, billingInvoices, paidTime, out Transaction, out PaymentInfo, out PtCashAdvanceID);

                AxLogger.Instance.LogInfo("End of processing payment for In-patient registration. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing payment for In-patient registration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PayForInPatientRegistration, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void FinalizeInPatientBillingInvoices(long StaffID, long registrationID,
            PatientTransactionPayment paymentDetails,//PatientPayment paymentDetails,
            List<InPatientBillingInvoice> billingInvoices, decimal totalPaymentAmount, out string msg, out PatientTransaction Transaction)
        {
            long patientId = 0;
            try
            {
                if (registrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }
                AxLogger.Instance.LogInfo("Start processing payment for In-patient registration.", CurrentUser);
                PatientRegistration registrationInfo = null;
                try
                {
                    registrationInfo = PatientProvider.Instance.GetRegistration(registrationID
                        , (int)AllLookupValues.V_FindPatientType.NOI_TRU);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                if (registrationInfo.DischargeDate == null)
                {
                    throw new Exception("Bệnh nhân chưa xuất viện không thể quyết toán");
                }
                Transaction = null;

                msg = null;
                long? _BankingTrxId = 0;
                patientId = registrationInfo.Patient.PatientID;
                StringBuilder _StrBuilder = new StringBuilder("");
                _StrBuilder.AppendLine("[Xác Nhận Kết Quả Thanh Toán Qua Thẻ Khám Bệnh]");
                long _FirstPaymentMethod = PaymentProvider.Instance.GetFirstPaymentMode(registrationID);
                bool _IsBankingPayment = 0 != _FirstPaymentMethod && 4803 == _FirstPaymentMethod
                    && totalPaymentAmount > 0;
                TransactionResponse _TransactionResponse = null;
                string _Salt = null;
                if (_IsBankingPayment)
                {
                    _Salt = StringUtil.RandomString();
                    PatientProvider.Instance.AddPatientSalt(patientId, _Salt);
                    _TransactionResponse = this.CanPay(_Salt, true, totalPaymentAmount,
                        registrationID, StaffID, patientId);
                }
                if (_IsBankingPayment
                    && (null == _TransactionResponse || !_TransactionResponse.IsSucceed))
                {
                    GetMsg(ref _StrBuilder, false, _TransactionResponse, null, "Thu Tiền Quyết Toán");
                    msg = _StrBuilder.ToString();
                    PatientProvider.Instance.RemovePatientSalt(patientId);
                    throw new Exception(_StrBuilder.ToString());
                }
                if (_IsBankingPayment)
                {
                    _TransactionResponse = this.RequestPayment(_Salt, true, registrationID,
                    StaffID, patientId, totalPaymentAmount, null);
                }
                if (_IsBankingPayment
                    && (null == _TransactionResponse || !_TransactionResponse.IsSucceed))
                {
                    GetMsg(ref _StrBuilder, false, _TransactionResponse, null, "Thu Tiền Quyết Toán");
                    msg = _StrBuilder.ToString();
                    PatientProvider.Instance.RemovePatientSalt(patientId);
                    throw new Exception(_StrBuilder.ToString());
                }
                if (_IsBankingPayment
                    && (null != _TransactionResponse && _TransactionResponse.IsSucceed))
                {
                    _BankingTrxId = _TransactionResponse.TransactionId;
                    GetMsg(ref _StrBuilder, false, _TransactionResponse, null, "Thu Tiền Quyết Toán");
                    msg = _StrBuilder.ToString();
                }
                if (_IsBankingPayment)
                {
                    PatientProvider.Instance.RemovePatientSalt(patientId);
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                var paidTime = DateTime.Now;
                if (billingInvoices != null)
                {
                    foreach (var inv in billingInvoices)
                    {
                        if (inv.PaidTime == null)
                        {
                            inv.PaidTime = paidTime;
                        }
                    }
                }
                paymentProcessor.FinalizeBillingInvoice_For_InPt(StaffID, registrationInfo, paymentDetails, billingInvoices, paidTime,
                    _BankingTrxId, out Transaction);

                AxLogger.Instance.LogInfo("End of processing payment for In-patient registration. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                PatientProvider.Instance.RemovePatientSalt(patientId);
                AxLogger.Instance.LogInfo("End of processing payment for In-patient registration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PayForInPatientRegistration, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public string DeleteTransactionFinalization(string FinalizedReceiptNum, long StaffID, long V_RegistrationType, long? PtRegistrationID, bool IsWithOutBill) //<==== #008
        {
            try
            {
                return PatientProvider.Instance.DeleteTransactionFinalization(FinalizedReceiptNum, StaffID, V_RegistrationType, PtRegistrationID, IsWithOutBill); //<==== #008
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteTransactionFinalization. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PayForInPatientRegistration, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public void DeleteTranacsionPayment_InPt(string ReceiptNumber, long StaffID)
        {
            try
            {
                PatientProvider.Instance.DeleteTranacsionPayment_InPt(ReceiptNumber, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteTranacsionPayment_InPt. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PayForInPatientRegistration, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void InPatientPayForBill(PatientRegistration registration, IList<InPatientBillingInvoice> billingInvoices, decimal payAmount, long staffID)
        {
            try
            {
                if (registration == null || registration.PtRegistrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }
                AxLogger.Instance.LogInfo("Start InPatienPayForBill.", CurrentUser);

                PatientProvider.Instance.InPatientPayForBill(registration, billingInvoices, payAmount, staffID);

                AxLogger.Instance.LogInfo("End InPatienPayForBill. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("InPatienPayForBill. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_InPatientPayForBill, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool ReturnInPatientDrug(long registrationID, List<RefGenMedProductSummaryInfo> returnedItems, long? DeptID, long? StaffID = null)
        {
            try
            {
                if (registrationID <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }

                AxLogger.Instance.LogInfo("Start returning payment for In-patient drug.", CurrentUser);

                PatientRegistration registrationInfo = null;
                try
                {
                    registrationInfo = PatientProvider.Instance.GetRegistration(registrationID
                        , (int)AllLookupValues.V_FindPatientType.NOI_TRU);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.ReturnInPatientDrug(registrationInfo, returnedItems, DeptID, StaffID);
                AxLogger.Instance.LogInfo("End of returning payment for In-patient drug. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of returning payment for In-patient drug. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_ReturnInPatientDrug, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            return true;
        }
		
        public int CheckForPreloadingBills(long registrationID, out string errorMsg)
        {
            try
            {
                int result = 0;
                result = PatientProvider.Instance.CheckForPreloadingBills(registrationID, out errorMsg);
                return result;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading CheckForPreloadingBills. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_LoadInPatientRegItemsIntoBill);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
		
        public InPatientBillingInvoice LoadInPatientRegItemsIntoBill(long registrationID, long? DeptID, long StoreID, long StaffID
            , bool IsAdmin, DateTime? FromDate, DateTime? ToDate, int FindPatientType, int LoadBillType, DateTime? DischargeDate)
        {
            try
            {
                PatientRegistration registrationInfo;
                try
                {
                    registrationInfo = PatientProvider.Instance.GetRegistration(registrationID, FindPatientType);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                AxLogger.Instance.LogInfo("Start loading LoadInPatientRegItemsIntoBill...", CurrentUser);
                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                var inv = paymentProcessor.LoadInPatientRegItemsIntoBill(registrationInfo, DeptID, StoreID, StaffID, IsAdmin
                    , FromDate, ToDate, LoadBillType, DischargeDate);
                AxLogger.Instance.LogInfo("End of loading LoadInPatientRegItemsIntoBill. Status: Success.", CurrentUser);
                return inv;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading LoadInPatientRegItemsIntoBill. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_LoadInPatientRegItemsIntoBill);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        /// <summary>
        /// Chi lay thong tin dang ky noi tru va payment thoi(khong lay cac chi tiet dang ky nhu CLS, KCB)
        /// </summary>
        /// <param name="registrationID"></param>
        /// <param name="TotalLiabilities"></param>
        /// <param name="SumOfAdvance"></param>
        /// <param name="TotalPatientPayment_PaidInvoice"></param>
        /// <returns></returns>
        /// 
        public bool GetInPatientRegistrationAndPaymentInfo(long registrationID, bool GetSumOfCashAdvBalanceOnly, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient, out decimal TotalCashAdvBalanceAmount
                                                            , out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Payment Info.", CurrentUser);

                return PatientProvider.Instance.GetInPatientRegistrationNonFinalizedLiabilities(registrationID, GetSumOfCashAdvBalanceOnly, out TotalLiabilities, out SumOfAdvance, out TotalPatientPayment_PaidInvoice, out TotalRefundPatient, out TotalCashAdvBalanceAmount
                                                                                                , out TotalCharityOrgPayment, out TotalPatientPayment_NotFinalized, out TotalPatientPaid_NotFinalized, out TotalCharityOrgPayment_NotFinalized);

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Payment Info. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void RecalcInPatientBillingInvoice(long? StaffID, InPatientBillingInvoice billingInv, bool IsUsePriceByNewCer, bool IsAutoCheckCountHI
            , bool ReplaceMaxHIPay = false, bool ReCalBillingInv = false
            , bool IsNotCheckInvalid = false
            , bool IsPassCheckNonBlockValidPCLExamDate = false)
        {
            try
            {
                if (billingInv == null || (billingInv.PtRegistrationID <= 0 && billingInv.OutPtRegistrationID == 0))
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }

                AxLogger.Instance.LogInfo("Start RecalcInPatientBillingInvoice.", CurrentUser);
                List<PatientRegistrationDetail> regDetails;
                List<PatientPCLRequest> PCLRequestList;
                List<OutwardDrugClinicDeptInvoice> allInPatientInvoices;

                GetInPatientBillingInvoiceDetails(billingInv.InPatientBillingInvID, out regDetails, out PCLRequestList, out allInPatientInvoices, IsUsePriceByNewCer, ReCalBillingInv, 0, 0, true, IsPassCheckNonBlockValidPCLExamDate);

                billingInv.RegistrationDetails = regDetails != null ? regDetails.ToObservableCollection() : null;
                billingInv.PclRequests = PCLRequestList != null ? PCLRequestList.ToObservableCollection() : null;
                billingInv.OutwardDrugClinicDeptInvoices = allInPatientInvoices != null ? allInPatientInvoices.ToObservableCollection() : null;

                PatientRegistration registrationInfo;
                try
                {
                    if (billingInv.OutPtRegistrationID > 0)
                    {
                        registrationInfo = GetRegistrationNgoaiTru(billingInv.OutPtRegistrationID);
                    }
                    else
                    {
                        registrationInfo = GetRegistrationNoiTru(billingInv.PtRegistrationID);
                    }
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }

                string ListIDOutTranDetails = "";
                var paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.RecalcInPatientBillingInvoice(out ListIDOutTranDetails, StaffID, registrationInfo, billingInv, ReplaceMaxHIPay, IsAutoCheckCountHI, IsNotCheckInvalid);

                AxLogger.Instance.LogInfo("End of RecalcInPatientBillingInvoice.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RecalcInPatientBillingInvoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RecalcInPatientBillingInvoice, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼====: 004
        public void RecalcInPatientBillingInvoiceWithPriceList(long? StaffID, InPatientBillingInvoice billingInv, bool IsUsePriceByNewCer, bool IsAutoCheckCountHI, bool ReplaceMaxHIPay = false
            , long MedServiceItemPriceListID = 0, long PCLExamTypePriceListID = 0, bool ReCalBillingInv = false)
        {
            try
            {
                if (billingInv == null || (billingInv.PtRegistrationID <= 0 && billingInv.OutPtRegistrationID == 0))
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }

                AxLogger.Instance.LogInfo("Start RecalcInPatientBillingInvoiceWithPriceList.", CurrentUser);
                List<PatientRegistrationDetail> regDetails;
                List<PatientPCLRequest> PCLRequestList;
                List<OutwardDrugClinicDeptInvoice> allInPatientInvoices;

                GetInPatientBillingInvoiceDetails(billingInv.InPatientBillingInvID, out regDetails, out PCLRequestList, out allInPatientInvoices, IsUsePriceByNewCer, ReCalBillingInv, MedServiceItemPriceListID, PCLExamTypePriceListID);

                billingInv.RegistrationDetails = regDetails?.ToObservableCollection();
                billingInv.PclRequests = PCLRequestList?.ToObservableCollection();
                billingInv.OutwardDrugClinicDeptInvoices = allInPatientInvoices?.ToObservableCollection();

                PatientRegistration registrationInfo;
                try
                {
                    if (billingInv.OutPtRegistrationID > 0)
                    {
                        registrationInfo = GetRegistrationNgoaiTru(billingInv.OutPtRegistrationID);
                    }
                    else
                    {
                        registrationInfo = GetRegistrationNoiTru(billingInv.PtRegistrationID);
                    }
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }
                var paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.RecalcInPatientBillingInvoice(out string ListIDOutTranDetails, StaffID, registrationInfo, billingInv, ReplaceMaxHIPay,IsAutoCheckCountHI, false );

                AxLogger.Instance.LogInfo("End of RecalcInPatientBillingInvoiceWithPriceList.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RecalcInPatientBillingInvoiceWithPriceList. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_RecalcInPatientBillingInvoice, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: 004

        public List<InPatientBillingInvoice> GetAllInPatientBillingInvoices(long PtRegistrationID, long? DeptID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllInPatientBillingInvoices.", CurrentUser);
                return PatientProvider.Instance.GetAllInPatientBillingInvoices(PtRegistrationID, DeptID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllInPatientBillingInvoices. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PromoDiscountProgram GetPromoDiscountProgramByPromoDiscProgID(long PromoDiscProgID, long V_RegistrationType)
        {
            try
            {
                return PatientProvider.Instance.GetPromoDiscountProgramByPromoDiscProgID(PromoDiscProgID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetPromoDiscountProgramByPromoDiscProgID. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //==== #001
        public List<InPatientBillingInvoice> GetAllInPatientBillingInvoicesByDeptArray(long PtRegistrationID, long? DeptID, List<RefDepartment> DeptArray, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllInPatientBillingInvoicesByDeptArray.", CurrentUser);
                return PatientProvider.Instance.GetAllInPatientBillingInvoicesByDeptArray(PtRegistrationID, DeptID, DeptArray, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllInPatientBillingInvoicesByDeptArray. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //==== #001

        public List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_FromListDeptID(long PtRegistrationID, List<long> ListDeptIDs)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllInPatientBillingInvoices_FromListDeptID.", CurrentUser);
                return PatientProvider.Instance.GetAllInPatientBillingInvoices_FromListDeptID(PtRegistrationID, ListDeptIDs);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllInPatientBillingInvoices_FromListDeptID. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<InPatientBillingInvoice> GetAllInPatientBillingInvoices_ForCreateForm02(long RptForm02_InPtID, long PtRegistrationID, List<long> ListDeptIDs)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllInPatientBillingInvoices_ForCreateForm02.", CurrentUser);
                return PatientProvider.Instance.GetAllInPatientBillingInvoices_ForCreateForm02(RptForm02_InPtID, PtRegistrationID, ListDeptIDs);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllInPatientBillingInvoices_ForCreateForm02. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetAllInPatientBillingInvoices_ForCreateForm02, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PatientRegistration GetRegistrationNoiTru(long registrationID)
        {
            return PatientProvider.Instance.GetRegistration(registrationID, (int)AllLookupValues.V_FindPatientType.NOI_TRU);
        }

        public PatientRegistration GetRegistrationNgoaiTru(long registrationID)
        {
            return PatientProvider.Instance.GetRegistration(registrationID, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);
        }

        public bool PatientCashAdvance_Insert(PatientCashAdvance payment, long patientId,
			out long PtCashAdvanceID, out string msg)
        {
            try
            {
                msg = String.Empty;
                StringBuilder _StrBuilder = new StringBuilder();
                long _FirstPaymentMethod = PaymentProvider.Instance.GetFirstPaymentMode(payment.PtRegistrationID);
                if (0 != _FirstPaymentMethod
                    && payment.V_PaymentMode.LookupID != _FirstPaymentMethod)
                {
                    _StrBuilder.AppendLine("[Cảnh Báo]");
                    _StrBuilder.AppendLine(null);
                    _StrBuilder.AppendLine("Vui lòng chọn phương thức thanh toán [Thẻ Khám Bệnh]!");
                    msg = _StrBuilder.ToString();
                    PtCashAdvanceID = 0;
                    return false;
                }
                _StrBuilder.AppendLine("[Xác Nhận Kết Quả Thanh Toán Qua Thẻ Khám Bệnh]");

				bool _IsBankingPayment = payment.V_PaymentMode.LookupID == 4803;
				TransactionResponse _TransactionResponse = null;
				string _Salt = null;
				if (_IsBankingPayment)
				{
					_Salt = StringUtil.RandomString();
					PatientProvider.Instance.AddPatientSalt(patientId, _Salt);
					_TransactionResponse = this.CanPay(_Salt, true, payment.PaymentAmount,
						payment.PtRegistrationID, payment.StaffID, patientId);
				}
				
				if (_IsBankingPayment 
					&& (null == _TransactionResponse || !_TransactionResponse.IsSucceed))
				{
                    GetMsg(ref _StrBuilder, false, _TransactionResponse,
                        payment.CashAdvReceiptNum, "Thu Tiền Tạm Ứng");
                    msg = _StrBuilder.ToString();
                    PtCashAdvanceID = 0;
                    return false;
				}
				if (_IsBankingPayment)
				{
                    payment.CashAdvReceiptNum = PaymentProvider.Instance.GetCashAdvReceiptNum();
					_TransactionResponse = this.RequestPayment(_Salt, true, payment.PtRegistrationID,
					payment.StaffID, patientId, payment.PaymentAmount, payment.CashAdvReceiptNum);
				}

				if (_IsBankingPayment
					&& (null == _TransactionResponse || !_TransactionResponse.IsSucceed))
				{
                    GetMsg(ref _StrBuilder, false, _TransactionResponse,
                        payment.CashAdvReceiptNum, "Thu Tiền Tạm Ứng");
                    msg = _StrBuilder.ToString();
                    PtCashAdvanceID = 0;
                    return false;
                }
				if (_IsBankingPayment
					&& (null != _TransactionResponse && _TransactionResponse.IsSucceed))
				{
					payment.BankingTrxId = _TransactionResponse.TransactionId;
                    GetMsg(ref _StrBuilder, false, _TransactionResponse,
                        payment.CashAdvReceiptNum, "Thu Tiền Tạm Ứng");
                    msg = _StrBuilder.ToString();
                }
				if (_IsBankingPayment)
				{
					PatientProvider.Instance.RemovePatientSalt(patientId);
				}
				AxLogger.Instance.LogInfo("Start loading PatientCashAdvance_Insert.", CurrentUser);
                return PaymentProvider.Instance.PatientCashAdvance_Insert(payment, out PtCashAdvanceID);
            }
            catch (Exception ex)
            {
                PatientProvider.Instance.RemovePatientSalt(patientId);

                AxLogger.Instance.LogInfo("End of loading PatientCashAdvance_Insert. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PatientCashAdvance_Delete(PatientCashAdvance payment, long patientId,
			long staffID, out string msg)
        {
            try
            {
                msg = null;
                if (null == payment) return false;

                AxLogger.Instance.LogInfo("Start loading PatientCashAdvance_Delete.", CurrentUser);
                StringBuilder _StrBuilder = new StringBuilder();
                long _FirstPaymentMethod = PaymentProvider.Instance.GetFirstPaymentMode(payment.PtRegistrationID);
                if (0 != _FirstPaymentMethod
                    && payment.V_PaymentMode.LookupID != _FirstPaymentMethod)
                {
                    _StrBuilder.AppendLine("[Cảnh Báo]");
                    _StrBuilder.AppendLine(null);
                    _StrBuilder.AppendLine("Vui lòng chọn phương thức thanh toán [Thẻ Khám Bệnh]!");
                    msg = _StrBuilder.ToString();
                    return false;
                }
                if (null != payment.BankingTrxId && null != payment.BankingRefundTrxId)
                {
                    _StrBuilder.AppendLine("[Cảnh Báo]");
                    _StrBuilder.AppendLine(null);
                    _StrBuilder.AppendLine("Tạm ứng đã được hoàn trả. Vui lòng liên hệ quản trị viên!");
                    msg = _StrBuilder.ToString();
                    return false;
                }

                _StrBuilder.AppendLine("[Xác Nhận Kết Quả Hoàn Trả Qua Thẻ Khám Bệnh]");

                bool _Result = PaymentProvider.Instance.PatientCashAdvance_Delete(payment, staffID);
				if (4803 != payment.V_PaymentMode.LookupID)
				{
					return _Result;
				}
				if (null == payment.BankingTrxId)
				{
                    _StrBuilder.AppendLine("[Cảnh Báo]");
                    _StrBuilder.AppendLine(null);
                    _StrBuilder.AppendLine("Không tìm thấy giao dịch thanh toán tương ứng để hoàn trả.");
                    msg = _StrBuilder.ToString();
                    return false;
				}
				string _Salt = StringUtil.RandomString();
				PatientProvider.Instance.AddPatientSalt(patientId, _Salt);
				TransactionResponse _TransactionResponse = this.RequestRefund(_Salt,
					payment.PtRegistrationID, payment.StaffID, patientId,
					payment.BankingTrxId.Value, payment.PaymentAmount);
				if (null == _TransactionResponse
					|| !_TransactionResponse.IsSucceed)
				{
					PatientProvider.Instance.RemovePatientSalt(patientId);
                    GetMsg(ref _StrBuilder, true, _TransactionResponse,
                        payment.CashAdvReceiptNum, "Thu Tiền Tạm Ứng");
                    msg = _StrBuilder.ToString();
                    return false;
				}
                GetMsg(ref _StrBuilder, true, _TransactionResponse,
                        payment.CashAdvReceiptNum, "Thu Tiền Tạm Ứng");
                msg = _StrBuilder.ToString();
                PatientProvider.Instance.RemovePatientSalt(patientId);
				return PaymentProvider.Instance.PatientCashAdvance_Refund(payment.PtCashAdvanceID,
					_TransactionResponse.TransactionId.Value);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading PatientCashAdvance_Delete. Status: Failed.", CurrentUser);
				PatientProvider.Instance.RemovePatientSalt(patientId);

				var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_DeletePatientCashAdvance, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<PatientCashAdvance> GetCashAdvanceBill(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetCashAdvanceBill.", CurrentUser);
                return PaymentProvider.Instance.GetCashAdvanceBill(PtRegistrationID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetCashAdvanceBill. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetCashAdvanceBill, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool ThanhToanTienChoBenhNhan(PatientTransactionPayment payment, PatientTransactionDetail TrDetail,
            long PtRegistrationID, long patientId, long V_RegistrationType, out long PtTranPaymtID, out string msg)
        {
            try
            {
                // VuTTM - Begin
                msg = String.Empty;
                PtTranPaymtID = 0;
                StringBuilder _StrBuilder = new StringBuilder();
                
                long _FirstPaymentMethod = PaymentProvider.Instance.GetFirstPaymentMode(PtRegistrationID);
                if (0 != _FirstPaymentMethod
                    && null != payment.PaymentMode
                    && payment.PaymentMode.LookupID != _FirstPaymentMethod)
                {
                    _StrBuilder.AppendLine("[Cảnh Báo]");
                    _StrBuilder.AppendLine(null);
                    _StrBuilder.AppendLine("Vui lòng chọn phương thức thanh toán [Thẻ Khám Bệnh]!");
                    msg = _StrBuilder.ToString();
                    return false;
                }
                _StrBuilder.AppendLine("[Xác Nhận Kết Quả Hoàn Viện Phí Qua Thẻ Khám Bệnh]");

                bool _IsBankingPayment = null != payment.PaymentMode
                    && payment.PaymentMode.LookupID == 4803
                    && payment.PayAmount > 0;
                decimal _TotalRefundAmount = 0;
                List<PatientCashAdvance> _RefundLst = null;
                if (_IsBankingPayment)
                {
                    _RefundLst = PaymentProvider.Instance.GetRefundPatientCashAdvance(PtRegistrationID);
                    if (null == _RefundLst || !_RefundLst.Any())
                    {
                        _StrBuilder.AppendLine("[Cảnh Báo]");
                        _StrBuilder.AppendLine(null);
                        _StrBuilder.AppendLine("Không tìm thấy danh sách giao dịch thanh toán qua [Thẻ Khám Bệnh] cho việc hoàn trả.");
                        msg = _StrBuilder.ToString();
                        return false;
                    }
                    string _Salt = StringUtil.RandomString();
                    PatientProvider.Instance.AddPatientSalt(patientId, _Salt);
                    foreach (PatientCashAdvance _Item in _RefundLst)
                    {
                        decimal _RefundAmount = _Item.BalanceAmount;
                        if (payment.PayAmount < (_RefundAmount + _TotalRefundAmount))
                        {
                            break;
                        }

                        TransactionResponse _TransactionResponse = this.RequestRefund(_Salt,
                            _Item.PtRegistrationID, payment.StaffID, patientId,
                            _Item.BankingTrxId.Value, _RefundAmount);

                        if (null == _TransactionResponse
                            || !_TransactionResponse.IsSucceed)
                        {
                            PatientProvider.Instance.RemovePatientSalt(patientId);
                            GetMsg(ref _StrBuilder, true, _TransactionResponse,
                                _Item.CashAdvReceiptNum, "Hoàn Viện Phí");
                            msg = _StrBuilder.ToString();
                            return false;
                        }
                        PaymentProvider.Instance.PatientCashAdvance_Refund(_Item.PtCashAdvanceID,
                            _TransactionResponse.TransactionId.Value);
                        GetMsg(ref _StrBuilder, true, _TransactionResponse,
                            _Item.CashAdvReceiptNum, "Hoàn Viện Phí");
                        _TotalRefundAmount += _RefundAmount;
                    }
                    _StrBuilder.AppendLine(string.Format("Tổng số tiền hoàn trả viện phí: {0} VNĐ",
                        string.Format(DISPLAY_MONEY_FORMAT, _TotalRefundAmount)));
                    msg = _StrBuilder.ToString();
                    PatientProvider.Instance.RemovePatientSalt(patientId);
                }
                // VuTTM - End

                AxLogger.Instance.LogInfo("Start loading ThanhToanTienChoBenhNhan.", CurrentUser);
                return PaymentProvider.Instance.ThanhToanTienChoBenhNhan(payment, TrDetail, PtRegistrationID, V_RegistrationType, out PtTranPaymtID);
            }
            catch (Exception ex)
            {
                PatientProvider.Instance.RemovePatientSalt(patientId);
                AxLogger.Instance.LogInfo("End of loading ThanhToanTienChoBenhNhan. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool RptPatientCashAdvReminder_Insert(RptPatientCashAdvReminder payment, out long RptPtCashAdvRemID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading RptPatientCashAdvReminder_Insert.", CurrentUser);
                return PaymentProvider.Instance.RptPatientCashAdvReminder_Insert(payment, out RptPtCashAdvRemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RptPatientCashAdvReminder_Insert. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool RptPatientCashAdvReminder_Update(RptPatientCashAdvReminder payment)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading RptPatientCashAdvReminder_Update.", CurrentUser);
                return PaymentProvider.Instance.RptPatientCashAdvReminder_Update(payment);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RptPatientCashAdvReminder_Update. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool RptPatientCashAdvReminder_Delete(long RptPtCashAdvRemID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading RptPatientCashAdvReminder_Delete.", CurrentUser);
                return PaymentProvider.Instance.RptPatientCashAdvReminder_Delete(RptPtCashAdvRemID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RptPatientCashAdvReminder_Delete. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #region Quỹ hỗ trợ bệnh nhân sử dụng kỹ thuật cao
        public List<CharityOrganization> GetAllCharityOrganization()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetAllCharityOrganization", CurrentUser);
                return PaymentProvider.Instance.GetAllCharityOrganization();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetAllCharityOrganization. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        /*▼====: #003*/
        public List<CharitySupportFund> GetCharitySupportFundForInPt(long PtRegistrationID, long? BillingInvID)
        {
            return GetCharitySupportFundForInPt_V2(PtRegistrationID, BillingInvID, null);
        }
        public List<CharitySupportFund> GetCharitySupportFundForInPt_V2(long PtRegistrationID, long? BillingInvID, bool? IsHighTechServiceBill)
        /*▲====: #003*/
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading GetCharitySupportFundForInPt", CurrentUser);
                return PaymentProvider.Instance.GetCharitySupportFundForInPt(PtRegistrationID, BillingInvID, IsHighTechServiceBill);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetCharitySupportFundForInPt. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        /*▼====: #003*/
        public List<CharitySupportFund> SaveCharitySupportFundForInPt(long PtRegistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds)
        {
            return SaveCharitySupportFundForInPt_V2(PtRegistrationID, StaffID, BillingInvID, SupportFunds, false);
        }
        public List<CharitySupportFund> SaveCharitySupportFundForInPt_V2(long PtRegistrationID, long StaffID, long? BillingInvID, List<CharitySupportFund> SupportFunds, bool IsHighTechServiceBill)
        /*▲====: #003*/
        {
            try
            {
                AxLogger.Instance.LogInfo("Start saving CharitySupportFundForInPt", CurrentUser);
                return PaymentProvider.Instance.SaveCharitySupportFundForInPt(PtRegistrationID, StaffID, BillingInvID, SupportFunds, IsHighTechServiceBill);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving CharitySupportFundForInPt. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool AddCharityOrganization(string CharityOrgName)
        /*▲====: #003*/
        {
            try
            {
                AxLogger.Instance.LogInfo("Start add CharityOrganization", CurrentUser);
                return PaymentProvider.Instance.AddCharityOrganization(CharityOrgName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of add CharityOrganization. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool EditCharityOrganization(long CharityOrgID, string CharityOrgName)
        /*▲====: #003*/
        {
            try
            {
                AxLogger.Instance.LogInfo("Start edit CharityOrganization", CurrentUser);
                return PaymentProvider.Instance.EditCharityOrganization(CharityOrgID, CharityOrgName);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of edit CharityOrganization. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteCharityOrganization(long CharityOrgID, long StaffID)
        /*▲====: #003*/
        {
            try
            {
                AxLogger.Instance.LogInfo("Start delete CharityOrganization", CurrentUser);
                return PaymentProvider.Instance.DeleteCharityOrganization(CharityOrgID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of delete CharityOrganization. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion

        public bool Recal15PercentHIBenefit(long PtRegistrationID, long StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Recal15PercentHIBenefit.", CurrentUser);
                return PaymentProvider.Instance.Recal15PercentHIBenefit(PtRegistrationID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Recal15PercentHIBenefit. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool RecalRegistrationHIBenefit(long PtRegistrationID, long StaffID, out string OutputBalanceServicesXML
            , IList<InPatientBillingInvoice> OutPtBillingCollection
            , double? PtInsuranceBenefit)
        {
            try
            {
                if (OutPtBillingCollection != null && OutPtBillingCollection.Count > 0 && PtInsuranceBenefit.HasValue)
                {
                    bool IsNeedRecal;
                    bool IsValid15 = PatientProvider.Instance.CheckValid15PercentHIBenefitCase(PtRegistrationID, (long)AllLookupValues.RegistrationType.NGOAI_TRU, out IsNeedRecal);
                    if (IsNeedRecal)
                    {
                        foreach (var Item in OutPtBillingCollection)
                        {
                            using (DbConnection CurrentConnection = PatientProvider.Instance.CreateConnection())
                            {
                                Item.OutwardDrugClinicDeptInvoices = PatientProvider.Instance.GetAllInPatientInvoicesByBillingInvoiceID(Item.InPatientBillingInvID, CurrentConnection, null)?.ToObservableCollection();
                            }
                            RecalcInPatientBillingInvoice(StaffID, Item, true, false, false, true);
                        }
                    }
                }
                AxLogger.Instance.LogInfo("Start loading RecalRegistrationHIBenefit.", CurrentUser);
                return PaymentProvider.Instance.RecalRegistrationHIBenefit(PtRegistrationID, StaffID, out OutputBalanceServicesXML);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RecalRegistrationHIBenefit. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool RecalRegistrationHIBenefit_New(long PtRegistrationID, long StaffID, out string OutputBalanceServicesXML)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading RecalRegistrationHIBenefit_New.", CurrentUser);
                return PaymentProvider.Instance.RecalRegistrationHIBenefit_New(PtRegistrationID, StaffID, out OutputBalanceServicesXML);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading RecalRegistrationHIBenefit_New. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool AddOutPtTransactionFinalization(OutPtTransactionFinalization TransactionFinalizationObj, bool IsUpdateToken, byte ViewCase, out long TransactionFinalizationSummaryInfoID, out long OutTranFinalizationID)
        {
            try
            {
                return PaymentProvider.Instance.AddOutPtTransactionFinalization(TransactionFinalizationObj, IsUpdateToken, ViewCase, out TransactionFinalizationSummaryInfoID, out OutTranFinalizationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading AddOutPtTransactionFinalization. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public OutPtTransactionFinalization RptOutPtTransactionFinalization(long aPtRegistrationID, long V_RegistrationType, long TranFinalizationID)
        {
            try
            {
                return PaymentProvider.Instance.RptOutPtTransactionFinalization(aPtRegistrationID, V_RegistrationType, TranFinalizationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading AddOutPtTransactionFinalization. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool CancelConfirmHIBenefit(long PtRegistrationID, long StaffID, out string msg)
        {
            try
            {
                StringBuilder _StrBuilder = new StringBuilder();
                long _LastBankingTrxId = PaymentProvider.Instance.GetLastBankingTrxIdByPtReg(PtRegistrationID);
                if (0 != _LastBankingTrxId)
                {
                    _StrBuilder.AppendLine("[Cảnh Báo]");
                    _StrBuilder.AppendLine("");
                    _StrBuilder.AppendLine("Đã phát sinh thanh toán qua [THẺ KHÁM BỆNH] thành công.");
                    _StrBuilder.AppendLine("Bạn phải [HOÀN TIỀN] trước khi thực hiện các bước tiếp theo!");
                }
                msg = _StrBuilder.ToString();
                return PaymentProvider.Instance.CancelConfirmHIBenefit(PtRegistrationID, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading CancelConfirmHIBenefit. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public PatientRegistration SaveThenPayForServicesAndPCLReqs(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration regInfo,
            List<PatientRegistrationDetail> regDetailList, List<PatientPCLRequest> pclRequestList, List<PatientRegistrationDetail> deletedRegDetailList,
            List<PatientPCLRequest> deletedPclRequestList, PatientTransactionPayment paymentDetails, PromoDiscountProgram PromoDiscountProgramObj,
            out long NewRegistrationID, out List<PatientRegistrationDetail> SavedRegistrationDetailList, out List<PatientPCLRequest> SavedPclRequestList,
            out PatientTransaction Transaction, out PatientTransactionPayment paymentInfo, out List<PaymentAndReceipt> paymentInfoList,
            out V_RegistrationError SaveRegisError,
            out V_RegistrationError PayError,
			out string responseMsg,
			DateTime modifiedDate = default(DateTime), 
            bool checkBeforePay = false,
            long? ConfirmHIStaffID = null,
            string OutputBalanceServicesXML = null,
            bool IsReported = false,
            bool IsUpdateHisID = false,
            long? HIID = null,
            double? PtInsuranceBenefit = null, bool IsNotCheckInvalid = false, bool IsProcess = false
            //▼====: #005
            , string TranPaymtNote = null, long? V_PaymentMode = null
            , long? V_ReceiveMethod = null)
            //▲====: #005
        {
            PatientRegistration savedPtRegInfo = null;
            NewRegistrationID = 0;
            SavedRegistrationDetailList = null;
            SavedPclRequestList = null;
            SaveRegisError = V_RegistrationError.mNone;
            PayError = V_RegistrationError.mNone;
			responseMsg = string.Empty;

			Transaction = null;
            paymentInfo = null;
            paymentInfoList = null;

            try
            {                
                savedPtRegInfo = AddServicesAndPCLRequests(StaffID, CollectorDeptLocID, Apply15HIPercent, regInfo,
                                                        regDetailList, pclRequestList, deletedRegDetailList, deletedPclRequestList, out NewRegistrationID,
                                                        out SavedRegistrationDetailList, out SavedPclRequestList, out SaveRegisError, modifiedDate
                                                        //▼====: #005
                                                        , IsNotCheckInvalid, false, IsProcess, TranPaymtNote, V_PaymentMode, false, V_ReceiveMethod);
                                                        //▲====: #005
                if (SaveRegisError == V_RegistrationError.mNone)
                {
                    // TxD 26/04/2019: The following Lists were added to cater for cases where User presses Save first then presses SaveAndPay                    
                    List<PatientRegistrationDetail> payingRegDetailsList = null;
                    List<PatientPCLRequest> payingPCLReqsList = null;

                    payingRegDetailsList = savedPtRegInfo.PatientRegistrationDetails.Where(item => item.PaidTime == null).ToList();

                    payingPCLReqsList = savedPtRegInfo.PCLRequests.Where(item => item.PaidTime == null).ToList();

                    PayForRegistration_V3(StaffID, CollectorDeptLocID, Apply15HIPercent, NewRegistrationID, savedPtRegInfo.FindPatient, paymentDetails,
                                            payingRegDetailsList, payingPCLReqsList, null, null,
                                            PromoDiscountProgramObj,
                                            out Transaction, out paymentInfo, out paymentInfoList,
                                            out PayError, out responseMsg, checkBeforePay, ConfirmHIStaffID, OutputBalanceServicesXML, IsReported, IsUpdateHisID,
                                            //▼====: #005
                                            HIID, PtInsuranceBenefit, savedPtRegInfo, IsNotCheckInvalid, false, IsProcess
                                            , TranPaymtNote, V_PaymentMode);
                                            //▲====: #005

                    savedPtRegInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(savedPtRegInfo.PtRegistrationID, (int)AllLookupValues.PatientFindBy.NGOAITRU, true, IsProcess);
                    //SavedRegistrationDetailList = payingRegDetailsList;
                    //SavedPclRequestList = payingPCLReqsList;
                    //▼===== TTM 20200729:  Lấy danh sách cuối cùng để load thay vì chỉ lấy những thằng đã trả tiền. Vì có trường hợp dịch vụ thứ 2 đưa vào lớn tiền
                    //                      hơn dịch vụ 1 nên dịch vụ 1 bị biến thành dịch vụ thứ 2 => Tiền thay đổi nếu không load hết thì sẽ bị sai.
                    if (savedPtRegInfo != null)
                    {
                        if (savedPtRegInfo.PatientRegistrationDetails != null && savedPtRegInfo.PatientRegistrationDetails.Count > 0)
                        {
                            SavedRegistrationDetailList = savedPtRegInfo.PatientRegistrationDetails.ToList();
                        }
                        if (savedPtRegInfo.PCLRequests != null && savedPtRegInfo.PCLRequests.Count > 0)
                        {
                            SavedPclRequestList = savedPtRegInfo.PCLRequests.ToList();
                        }
                    }
                    //▲=====

                }
                return savedPtRegInfo;
            }
            catch(Exception ex)
            {
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "SaveThenPayForServicesAndPCLReqs", CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message)); 
            }
            
        }

        #region Tạm ứng ngoại trú
        public bool PatientAccountTransaction_Insert(PatientAccountTransaction payment, out long PtAccountTranID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Patient Account Transaction Insert.", CurrentUser);
                return PaymentProvider.Instance.PatientAccountTransaction_Insert(payment, out PtAccountTranID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Patient Account Transaction Insert. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientAccountTransaction> PatientAccountTransaction_GetAll(long PatientID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Patient Account Transaction Get All.", CurrentUser);
                return PaymentProvider.Instance.PatientAccountTransaction_GetAll(PatientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Patient Account Transaction Get All. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientAccount> PatientAccount_GetAll(long PatientID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Patient Account Get All.", CurrentUser);
                return PaymentProvider.Instance.PatientAccount_GetAll(PatientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Patient Account Get All. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PatientAccount_Insert(long PatientID, string AccountNumber)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Patient Account Insert.", CurrentUser);
                return PaymentProvider.Instance.PatientAccount_Insert(PatientID, AccountNumber);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Patient Account Transaction .Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DoSettlementForListOutPatient(List<PatientRegistration> ListPtRegistration, long PatientID, long StaffID)
        {
            try
            {
                if (ListPtRegistration == null || ListPtRegistration.Count <= 0)
                {
                    throw new Exception(eHCMSResources.Z1688_G1_TTinKgHopLe);
                }
                AxLogger.Instance.LogInfo("Start processing payment.", CurrentUser);
 


                if (ListPtRegistration != null && ListPtRegistration.Count > 0)
                {
                    //Tạo ra thông tin quyết toán cha sau đó out ra để đưa vào khi trả tiền xong quyết toán => biết được quyết toán đó của ai.
                    long OutPtGeneralFinalizationID;
                    PaymentProvider.Instance.OutPatientGeneralFinalizations_Insert(PatientID, StaffID, out OutPtGeneralFinalizationID);

                    //Vì chưa có cách trả tiền gộp nên anh Tuấn cho phép loop từng đăng ký để thực hiện.
                    foreach (var item in ListPtRegistration)
                    {
                        PatientRegistration regInfo = new PatientRegistration();
                        PatientTransaction Transaction = new PatientTransaction();
                        List<PatientRegistrationDetail> colPaidRegDetails = new List<PatientRegistrationDetail>();
                        List<PatientPCLRequest> colPaidPclRequests = new List<PatientPCLRequest>();
                        List<OutwardDrugInvoice> colPaidDrugInvoice = new List<OutwardDrugInvoice>();
                        List<InPatientBillingInvoice> colBillingInvoices = new List<InPatientBillingInvoice>();
                        PatientTransactionPayment paymentDetails = new PatientTransactionPayment();
                        long? HIID = 0;
                        double? PtInsuranceBenefit = 0;
                        //Danh sách có để out ra khi nào ra thì mới biết để làm gì
                        PatientTransactionPayment paymentInfo = new PatientTransactionPayment();
                        List<PaymentAndReceipt> paymentInfoList = new List<PaymentAndReceipt>();

                        //Danh sách không cần nhưng phải có
                        string OutputBalanceServicesXML = "";



                        regInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(item.PtRegistrationID, (int)AllLookupValues.PatientFindBy.NGOAITRU, true);
                        //Set giá trị cho Current Payment
                        paymentDetails = new PatientTransactionPayment
                        {
                            StaffID = 1210,
                            PaymentMode = new Lookup() { LookupID = (long)AllLookupValues.PaymentMode.TIEN_MAT },
                            PaymentType = new Lookup() { LookupID = (long)AllLookupValues.PaymentMode.TIEN_MAT },
                            Currency = new Lookup() { LookupID = (long)AllLookupValues.Currency.VND },
                            PtPmtAccID = 1,
                            V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.DANG_KY
                        };
                        //PayAmount của PaymentDetails phải là tổng tiền bệnh nhân cần thanh toán.
                        paymentDetails.PayAmount = regInfo.TotalPatientPaidForSettlement;

                        //Lấy chi tiết liên quan đến bảo hiểm của đăng ký
                        if(regInfo.HisID > 0)
                        {
                            HIID = (long?)regInfo.HealthInsurance.HIID;
                            PtInsuranceBenefit = regInfo.PtInsuranceBenefit;
                        }


                        //Lấy danh sách chi tiết đăng ký chưa trả tiền để đi trả tiền ở quyết toán
                        if (regInfo.AllSaveRegistrationDetails != null && regInfo.AllSaveRegistrationDetails.Count > 0)
                        {
                            foreach(var RegDetail in regInfo.AllSaveRegistrationDetails)
                            {
                                if (RegDetail.PaidTime == null 
                                    && RegDetail.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                                {
                                    colPaidRegDetails.Add(RegDetail);
                                }
                            }
                        }

                        //Lấy danh sách chi tiết CLS chưa trả tiền để đi trả tiền ở quyết toán
                        if (regInfo.AllSavePCLRequestDetails != null && regInfo.AllSavePCLRequestDetails.Count > 0)
                        {
                            foreach (var PCLRequestDetail in regInfo.PCLRequests)
                            {
                                if (PCLRequestDetail.PaidTime == null
                                  && PCLRequestDetail.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                                {
                                    colPaidPclRequests.Add(PCLRequestDetail);
                                }
                            }
                        }

                        //Lấy danh sách phiếu xuất thuốc chưa trả tiền
                        if (regInfo.DrugInvoices != null && regInfo.DrugInvoices.Count > 0)
                        {
                            foreach (var Invoices in regInfo.DrugInvoices)
                            {
                                if (Invoices.PaidTime == null
                                  && Invoices.V_OutDrugInvStatus != (long)AllLookupValues.V_OutDrugInvStatus.CANCELED 
                                  && Invoices.V_OutDrugInvStatus != (long)AllLookupValues.V_OutDrugInvStatus.RETURN)
                                {
                                    colPaidDrugInvoice.Add(Invoices);
                                }
                            }
                        }

                        //Lấy danh sách bill ngoại trú của bệnh nhân chưa được thanh toán.
                        if (regInfo.InPatientBillingInvoices != null && regInfo.InPatientBillingInvoices.Count > 0)
                        {
                            foreach (var Billing in regInfo.InPatientBillingInvoices)
                            {
                                if (Billing.PaidTime == null)
                                {
                                    colBillingInvoices.Add(Billing);
                                }
                            }
                        }

                        //▼===== Trả tiền cho tất cả các dịch vụ, CLS, thuốc ... chưa được trả tiền ở các đăng ký cần đi quyết toán.
                        //StaffID => Globals.LoggedUserAccount, CollectorDeptLocID => Globals.DeptLocation
                        //Apply15HIPercent => Null trước khi thực hiện trả tiền sẽ được gán lại.
                        //ConfirmHIStaffID => Không có confirm nên set về null
                        //IsReported => Always False chỉ khi chạy hàm btnCancelService ở trên Client thì nó mới được set lên false hiện tại chưa biết khi nào hàm này đc chạy
                        //IsUpdateHisID => Trên giao diện không thấy set = true khi nào cả
                        //IsNotCheckInvalid => Để khỏi kiểm tra giữa 2 lần bảo hiểm của CLS => Hiện tại set false để qua hết.
                        //IsRefundBilling => Có phải là huỷ trả tiền bill không => Do đang quyết toán nên chắc chắn phải trả => set là false.
                        //IsProcess: Biến này để phân biệt đang thực hiện trả tiền cho liệu trình Thẩm mỹ viện
                        //IsSettlement: Đây là hàm quyết toán ngoại trú nên có biến này truyền xuống store để thực hiện trả tiền cho quyết toán (đưa debit vào trong bảng PatientAccountTransaction)
                        RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
                        paymentProcessor.InitNewTxd(regInfo, false);
                        paymentProcessor.PayForRegistration(1210, 1, null, regInfo, paymentDetails
                            , colPaidRegDetails, colPaidPclRequests, colPaidDrugInvoice, colBillingInvoices
                            , out Transaction, out paymentInfo, out paymentInfoList, null, OutputBalanceServicesXML, false, false
                            , HIID, PtInsuranceBenefit, false, false, false, true);

                        //▼===== Quyết toán từng đăng ký.
                        //Chuẩn bị dữ liệu cho OutPtTransactionFinalization (TransactionFinalizationObj)
                        OutPtTransactionFinalization TransactionFinalizationObj = new OutPtTransactionFinalization
                        {
                            TranFinalizationID = 0,
                            PtRegistrationID = regInfo.PtRegistrationID,
                            StaffID = StaffID,
                            V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU,
                            V_PaymentMode = (long)AllLookupValues.PaymentMode.TIEN_MAT,
                            DateInvoice = DateTime.Now
                        };
                        //IsUpdateToken
                        //ViewCase 0: Xuất hóa đơn, 1: Phát hành hóa đơn điện tử, 2: Xuất hóa đơn chuẩn bị dữ liệu cho phát hành hóa đơn điện tử
                        long TransactionFinalizationSummaryInfoID = 0;
                        long OutTranFinalizationID = 0;
                        PaymentProvider.Instance.AddOutPtTransactionFinalization(TransactionFinalizationObj, false, 0, out TransactionFinalizationSummaryInfoID, out OutTranFinalizationID, OutPtGeneralFinalizationID);
                    }
                }
                else
                {
                    return false;
                }
                AxLogger.Instance.LogInfo("End of processing payment.", CurrentUser);
                return true;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of processing payment. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PayForRegistration, CurrentUser);

                //throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion


        //▼===== 20200729 TTM: Không load lại đăng ký khi làm việc dưới service.
        public PatientRegistration AddServicesAndPCLRequests(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent
            , PatientRegistration regInfo, List<PatientRegistrationDetail> regDetailList, List<PatientPCLRequest> pclRequestList
            , List<PatientRegistrationDetail> deletedRegDetailList, List<PatientPCLRequest> deletedPclRequestList
            , out long NewRegistrationID
            , out List<PatientRegistrationDetail> SavedRegistrationDetailList
            , out List<PatientPCLRequest> SavedPclRequestList, out V_RegistrationError error
            , DateTime modifiedDate = default(DateTime)
            , bool IsNotCheckInvalid = false
            //▼====: #005
            , bool IsCheckPaid = false, bool IsProcess = false
            , string TranPaymtNote = null, long? V_PaymentMode = null
            , bool IsFromRequestDoctor = false
            , long? V_ReceiveMethod = null)
            //▲====: #005
        {
            try
            {
                SavedRegistrationDetailList = null;
                SavedPclRequestList = null;
                AxLogger.Instance.LogInfo("Start registering patient.", CurrentUser);
                List<long> newRegDetailsList = null;
                List<long> newPclRequestList = null;
                error = V_RegistrationError.mNone;

                if (regInfo == null)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z0083_G1_KhongTimThayDK));
                }
                if (regInfo.ExamDate == DateTime.MinValue)
                {
                    regInfo.ExamDate = DateTime.Now;
                }
                if (modifiedDate == default(DateTime))
                {
                    modifiedDate = regInfo.ExamDate;
                }

                if (regDetailList != null)
                {
                    if (Globals.AxServerSettings.OutRegisElements.IsPerformingTMVFunctionsA && regInfo.PatientRegistrationDetails.ToList() != null && regInfo.PatientRegistrationDetails.ToList().Count > 0)
                    {
                        foreach (PatientRegistrationDetail item in regInfo.PatientRegistrationDetails.ToList())
                        {
                            if (item.PaidTimeTmp != null)
                            {
                                item.PaidTime = null;
                            }
                        }
                    }

                    foreach (var patientRegistrationDetail in regDetailList)
                    {
                        if (patientRegistrationDetail.CreatedDate == DateTime.MinValue)
                        {
                            patientRegistrationDetail.CreatedDate = DateTime.Now;
                        }
                    }
                }
                
                if (pclRequestList != null)
                {
                    if (regInfo.PCLRequests == null)
                    {
                        regInfo.PCLRequests = new System.Collections.ObjectModel.ObservableCollection<PatientPCLRequest>();
                    }
                    foreach (var pclRequest in pclRequestList)
                    {
                        if (pclRequest.CreatedDate == DateTime.MinValue)
                        {
                            pclRequest.CreatedDate = DateTime.Now;
                        }
                        if (pclRequest.PatientPCLRequestIndicators != null)
                        {
                            foreach (var requestDetail in pclRequest.PatientPCLRequestIndicators)
                            {
                                if (requestDetail.CreatedDate == DateTime.MinValue)
                                {
                                    requestDetail.CreatedDate = DateTime.Now;
                                }
                            }
                        }
                    }
                }
                //▼===== 20200729 TTM:  Tạo ra list đã trả tiền và UNCHANGED vì những thằng chưa trả tiền còn phải tách phiếu và tách phiếu thì phải 
                //                      gán lại vào CurRegistration để tách thành các XML nhỏ đi lưu.
                System.Collections.ObjectModel.ObservableCollection<PatientPCLRequest> ListUnchanged = new System.Collections.ObjectModel.ObservableCollection<PatientPCLRequest>();
                if (regInfo.PCLRequests != null && regInfo.PCLRequests.Count > 0)
                {
                    foreach (var item in regInfo.PCLRequests)
                    {
                        if (item.RecordState == RecordState.UNCHANGED || item.RecordState == RecordState.DELETED || item.RecordState == RecordState.MODIFIED)
                        {
                            ListUnchanged.Add(item);
                        }
                    }
                }
                regInfo.PCLRequests = ListUnchanged;

                //▲=====

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
                paymentProcessor.InitNewTxd(regInfo, false,0, false, true);

                long id = regInfo.PtRegistrationID > 0 ? regInfo.PtRegistrationID : -1;

                //▼====: #005
                RetryOnDatabaseDeadlock.RetryUntil(() =>
                {
                    paymentProcessor.AddServicesAndPCLRequests(StaffID, CollectorDeptLocID, Apply15HIPercent, regInfo, regDetailList, pclRequestList
                        , deletedRegDetailList, deletedPclRequestList, modifiedDate, IsNotCheckInvalid, IsCheckPaid
                        , out id, out newRegDetailsList, out newPclRequestList, IsProcess, true, TranPaymtNote, V_PaymentMode, IsFromRequestDoctor, V_ReceiveMethod);
                }, 5);
                //▲====: #005

                NewRegistrationID = id;


                AxLogger.Instance.LogInfo("End of registering patient.", CurrentUser);


                //Update lai Paperreferal ID
                if (regInfo.PaperReferal != null
                    && regInfo.PaperReferal.RefID > 0
                    && (regInfo.PaperReferal.PtRegistrationID == null
                        || regInfo.PaperReferal.PtRegistrationID < 1))
                {
                    regInfo.PaperReferal.PtRegistrationID = NewRegistrationID;
                    PatientProvider.Instance.UpdatePaperReferalRegID(regInfo.PaperReferal);
                }

                // TxD 01/01/2014 : Return PatientRegistration HERE to SAVE the Client a TRIP to CALL and GetRegistrationInfo

                PatientRegistration newRegInfoReloaded = RegAndPaymentProcessorBase.GetRegistrationTxd(NewRegistrationID, regInfo.FindPatient, true, IsProcess);
                //▼===== 20200729 TTM:  Thay vì load lại thông tin để không làm gì (Vì các biến này đều được set lại lần nữa từ sau khi chạy PayForRegistration_V3
                //                      Nên chỗ này có thể set null luôn cũng đc.
                //if (newRegInfoReloaded != null)
                //{
                //    if (newRegInfoReloaded.PatientRegistrationDetails != null && newRegInfoReloaded.PatientRegistrationDetails.Count > 0)
                //    {
                //        SavedRegistrationDetailList = newRegInfoReloaded.PatientRegistrationDetails.ToList();
                //    }
                //    if (newRegInfoReloaded.PCLRequests != null && newRegInfoReloaded.PCLRequests.Count > 0)
                //    {
                //        SavedPclRequestList = newRegInfoReloaded.PCLRequests.ToList();
                //    }
                //}
                if (newRegDetailsList != null)
                {
                    SavedRegistrationDetailList = PatientProvider.Instance.GetAllRegistrationDetailsByIDList(newRegDetailsList);
                }
                if (newPclRequestList != null)
                {
                    SavedPclRequestList = PatientProvider.Instance.GetPCLRequestListByIDList(newPclRequestList);
                }
                //▲=====
                return newRegInfoReloaded;

            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddServicesAndPCLRequests. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddServicesAndPCLRequests, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #region
        public void SaveQuotation(InPatientBillingInvoice aBillingInvoice, out long OutQuotationID, string QuotationTitle, long? PatientID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start saving Quotation.", CurrentUser);
                PatientProvider.Instance.SaveQuotation(aBillingInvoice, out OutQuotationID, QuotationTitle, PatientID);
                AxLogger.Instance.LogInfo("End of saving Quotation.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving Quotation. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public InPatientBillingInvoice GetQuotationAllDetail(long InPatientBillingInvID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start saving GetQuotationAllDetail.", CurrentUser);
                return PatientProvider.Instance.GetQuotationAllDetail(InPatientBillingInvID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving GetQuotationAllDetail. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<InPatientBillingInvoice> GetQuotationCollection(short ViewCase)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start saving GetQuotationCollection.", CurrentUser);
                return PatientProvider.Instance.GetQuotationCollection(ViewCase);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving GetQuotationCollection. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public void RemoveQuotation(long InPatientBillingInvID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start removing RemoveQuotation.", CurrentUser);
                PatientProvider.Instance.RemoveQuotation(InPatientBillingInvID);
                AxLogger.Instance.LogInfo("End of removing RemoveQuotation.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of removing RemoveQuotation. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void CreatePatientQuotation(long InPatientBillingInvID, long PatientID, string QuotationTitle)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start create PatientQuotation.", CurrentUser);
                PatientProvider.Instance.CreatePatientQuotation(InPatientBillingInvID, PatientID, QuotationTitle);
                AxLogger.Instance.LogInfo("End of create PatientQuotation.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of create PatientQuotation. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public bool UpdateQuotation(InPatientBillingInvoice aBillingInvoice, string QuotationTitle)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Update Quotation.", CurrentUser);

                if (aBillingInvoice == null)
                {
                    return false;
                }

                List<PatientRegistrationDetail> ListModifiRegistrationDetails = new List<PatientRegistrationDetail>();
                List<PatientRegistrationDetail> ListAddRegistrationDetails = new List<PatientRegistrationDetail>();

                List<PatientPCLRequestDetail> ListModifiPCLDetails = new List<PatientPCLRequestDetail>();
                List<PatientPCLRequestDetail> ListAddPCLDetails = new List<PatientPCLRequestDetail>();

                OutwardDrugClinicDeptInvoice OutwardDrugInvoice = new OutwardDrugClinicDeptInvoice();

                if (aBillingInvoice.RegistrationDetails != null && aBillingInvoice.RegistrationDetails.Count > 0)
                {
                    foreach (var Regdetails in aBillingInvoice.RegistrationDetails)
                    {
                        if (Regdetails.RecordState == RecordState.ADDED)
                        {
                            ListAddRegistrationDetails.Add(Regdetails);
                        }
                        else if (Regdetails.RecordState == RecordState.MODIFIED || Regdetails.RecordState == RecordState.DELETED)
                        {
                            if (Regdetails.RecordState == RecordState.DELETED)
                            {
                                Regdetails.MarkedAsDeleted = true;
                                Regdetails.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.XOA_TRA_TIEN_LAI;
                            }
                            ListModifiRegistrationDetails.Add(Regdetails);
                        }
                    }
                    PatientProvider.Instance.AddUpdateRegistrationDetailsForQuotation(aBillingInvoice.InPatientBillingInvID, ListAddRegistrationDetails, ListModifiRegistrationDetails);
                }

                if (aBillingInvoice.PclRequests != null && aBillingInvoice.PclRequests.Count > 0)
                {
                    foreach (var PCLdetails in aBillingInvoice.PclRequests)
                    {
                        foreach (var item in PCLdetails.PatientPCLRequestIndicators)
                        {
                            if (item.RecordState == RecordState.DETACHED)
                            {
                                ListAddPCLDetails.Add(item);
                            }
                            else if (item.RecordState == RecordState.MODIFIED || item.RecordState == RecordState.DELETED)
                            {
                                if (item.RecordState == RecordState.DELETED)
                                {
                                    item.MarkedAsDeleted = true;
                                    item.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.XOA_TRA_TIEN_LAI;
                                }
                                ListModifiPCLDetails.Add(item);
                            }
                        }
                    }
                    PatientProvider.Instance.AddUpdatePCLRequestDetailsForQuotation(aBillingInvoice.InPatientBillingInvID, ListAddPCLDetails, ListModifiPCLDetails);
                }

                if (aBillingInvoice.OutwardDrugClinicDeptInvoices != null && aBillingInvoice.OutwardDrugClinicDeptInvoices.Count > 0)
                {
                    if (OutwardDrugInvoice.OutwardDrugClinicDepts == null)
                    {
                        OutwardDrugInvoice.OutwardDrugClinicDepts = new System.Collections.ObjectModel.ObservableCollection<OutwardDrugClinicDept>();
                    }
                    foreach (var Drug in aBillingInvoice.OutwardDrugClinicDeptInvoices)
                    {
                        foreach (var item in Drug.OutwardDrugClinicDepts)
                        {
                            if (item.RecordState == RecordState.DELETED)
                            {
                                item.RefundTime = DateTime.Now;
                                OutwardDrugInvoice.OutwardDrugClinicDepts.Add(item);
                            }
                            else if (item.RecordState == RecordState.MODIFIED || item.RecordState == RecordState.ADDED)
                            {
                                OutwardDrugInvoice.OutwardDrugClinicDepts.Add(item);
                            }
                        }
                    }
                    PatientProvider.Instance.AddUpdateOutwardDrugForQuotation(aBillingInvoice.InPatientBillingInvID, OutwardDrugInvoice);
                }

                bool IsOK = PatientProvider.Instance.CalAdditionalFeeAndTotalBillForQuotation(aBillingInvoice.InPatientBillingInvID, aBillingInvoice.StaffID, true, aBillingInvoice.HIBenefit);
                return IsOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Update Quotation. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #endregion
        //▼====: #006
        #region Quản lý tất cả bill viện phí nội trú
        public ObservableCollection<MedRegItemBase> GetInPatientAllBillingInvoiceSummary(long PtRegistrationID, long DeptID, DateTime FromDate, DateTime ToDate
            , bool IsPassCheckNonBlockValidPCLExamDate)
        {
            try
            {
                ObservableCollection<MedRegItemBase> result = new ObservableCollection<MedRegItemBase>();
                AxLogger.Instance.LogInfo("Lấy danh sách chi tiết bill theo khoa và thời gian y lệnh.", CurrentUser);
                using (DbConnection connection = PatientProvider.Instance.CreateConnection())
                {
                    List<PatientRegistrationDetail> regDetails = PatientProvider.Instance.GetAllRegistrationDetailsByPtRegistrationID(PtRegistrationID
                        , DeptID, FromDate, ToDate, connection, null);
                    if (regDetails != null)
                    {
                        foreach (var itemDetail in regDetails)
                        {

                            result.Add(itemDetail);
                        }
                    }
                    List<PatientPCLRequest> PCLRequestList = PatientProvider.Instance.GetPCLRequestListByPtRegistrationID(PtRegistrationID
                        , DeptID, FromDate, ToDate, IsPassCheckNonBlockValidPCLExamDate, connection, null);
                    if (PCLRequestList != null)
                    {
                        foreach (var itemPCL in PCLRequestList)
                        {
                            if (itemPCL.PatientPCLRequestIndicators != null && itemPCL.PatientPCLRequestIndicators.Count() > 0)
                            {
                                foreach (var itemPCLItem in itemPCL.PatientPCLRequestIndicators)
                                {
                                    result.Add(itemPCLItem);
                                }
                            }
                        }
                    }
                    List<OutwardDrugClinicDeptInvoice> allInPatientInvoices = PatientProvider.Instance.GetAllInPatientInvoicesByPtRegistrationID(PtRegistrationID
                        , DeptID, FromDate, ToDate, connection, null);
                    if (allInPatientInvoices != null)
                    {
                        foreach (var invoice in allInPatientInvoices)
                        {
                            if (invoice.OutwardDrugClinicDepts != null && invoice.OutwardDrugClinicDepts.Count > 0)
                            {
                                foreach (var detail in invoice.OutwardDrugClinicDepts)
                                {
                                    result.Add(detail);
                                }
                            }
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("Lấy danh sách chi tiết bill theo khoa và thời gian y lệnh. Lỗi", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_CANNOT_ADD, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void UpdateInPatientBillingInvoiceByPtRegistrationID(long? StaffID, long PtRegistrationID
            , List<PatientRegistrationDetail> deletedRegDetails
            , List<PatientPCLRequestDetail> deletedPclRequestDetails
            , List<OutwardDrugClinicDept> deleteOutwardDrugClinicDepts
            , List<PatientRegistrationDetail> modifiedRegDetails
            , List<PatientPCLRequestDetail> modifiedPclRequestDetails
            , List<OutwardDrugClinicDept> modifiedOutwardDrugClinicDepts
            , bool IsNotCheckInvalid
            , out Dictionary<long, List<long>> DrugIDList_Error)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start update all billing invoice.", CurrentUser);
                PatientRegistration registrationInfo = null;
                try
                {
                    registrationInfo = PatientProvider.Instance.GetRegistration(PtRegistrationID, (int)AllLookupValues.V_FindPatientType.NOI_TRU);
                }
                catch
                {
                    throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                }

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                paymentProcessor.UpdateInPatientBillingInvoiceByPtRegistrationID(StaffID, registrationInfo
                                                                    , deletedRegDetails
                                                                    , deletedPclRequestDetails
                                                                    , deleteOutwardDrugClinicDepts
                                                                    , modifiedRegDetails
                                                                    , modifiedPclRequestDetails
                                                                    , modifiedOutwardDrugClinicDepts
                                                                    , IsNotCheckInvalid
                                                                    , out DrugIDList_Error);

                AxLogger.Instance.LogInfo("End of billing invoice.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of billing invoice. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateInPatientBillingInvoice, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        #endregion
        //▲====: #006

        //▼====: #007
        public bool ConfirmPatientPostponementAdvance(InPatientAdmDisDetails AdmissionInfo, long StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading ConfirmPatientPostponementAdvance.", CurrentUser);
                return PaymentProvider.Instance.ConfirmPatientPostponementAdvance(AdmissionInfo, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading ConfirmPatientPostponementAdvance. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #007
        //▼====: #009
        public OutPatientCashAdvance GetLastOutPatientCashAdvance(long PtRegistrationID, bool isGetLast)
        {
            try
            {
                return PaymentProvider.Instance.GetAllOutPatientCashAdvance(PtRegistrationID, isGetLast).FirstOrDefault();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading AddOutPtTransactionFinalization. Status: Failed.", CurrentUser);
                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #009
    }
}
