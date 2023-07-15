using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using DataEntities;
using Service.Core.Common;
using eHCMS.DAL;
using System.Data.Common;
using System;

namespace CommonServices
{
    public class NormalPatientRegAndPaymentProcessor:RegAndPaymentProcessorBase
    {
        protected override void CalcInvoiceItems(IEnumerable<IInvoiceItem> colInvoiceItems, PatientRegistration registration)
        {
            foreach (var item in colInvoiceItems)
            {
                GetItemPrice(item, registration, 0.0);
                GetItemTotalPrice(item);
            }
        }
        protected override void AddRegistrationDetails(PatientRegistration regInfo, List<PatientRegistrationDetail> regDetailList, DbConnection conn, DbTransaction tran, out List<long> newRegistrationIDList)
        {
            if (regDetailList != null)
            {
                foreach (var invoiceItem in regDetailList)
                {
                    GetItemPrice(invoiceItem,null,0.0);
                    GetItemTotalPrice(invoiceItem);
                }
            }

            base.AddRegistrationDetails(regInfo, regDetailList, conn, tran, out newRegistrationIDList);
        }
        protected override void AddOutwardDrugClinicDept(PatientRegistration regInfo, OutwardDrugClinicDeptInvoice inv, List<InwardDrugClinicDept> updatedInwardItems, DbConnection conn, DbTransaction tran, out List<long> inwardDrugIDListError)
        {
            if (inv.PtRegistrationID.GetValueOrDefault(-1) <= 0)
            {
                inv.PtRegistrationID = regInfo.PtRegistrationID;
            }
            if (inv.OutwardDrugClinicDepts != null)
            {
                foreach (var invoiceItem in inv.OutwardDrugClinicDepts)
                {
                    GetItemPrice(invoiceItem, null, 0.0);
                    GetItemTotalPrice(invoiceItem);
                }
            }
            base.AddOutwardDrugClinicDept(regInfo, inv,updatedInwardItems, conn, tran, out inwardDrugIDListError);
        }
        protected override void AddNewRegistration(PatientRegistration regInfo, DbConnection conn, DbTransaction tran, out long newRegistrationID)
        {            
            int sequenceNo;
            PatientProvider.Instance.AddRegistration(regInfo, conn, tran, out newRegistrationID, out sequenceNo);
            regInfo.PtRegistrationID = newRegistrationID;
        }

        //public override bool AddServicesAndPCLRequests(PatientRegistration regInfo, List<PatientRegistrationDetail> regDetailList, List<PatientPCLRequest> pclRequestList, List<PatientRegistrationDetail> deletedRegDetailList, List<PatientPCLRequest> deletedPclRequestList, DateTime modifiedDate, out long NewRegistrationID, out List<long> NewRegDetailsList, out List<long> NewPclRequestList)
        //{
        //    NewRegistrationID = 0;
        //    using (DbConnection conn = PatientProvider.Instance.CreateConnection())
        //    {
        //        conn.Open();

        //        DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

        //        try
        //        {
        //            if (regInfo.PtRegistrationID <= 0)
        //            {
        //                AddNewRegistration(regInfo, conn,tran, out NewRegistrationID);
        //            }
        //            else
        //            {
        //                NewRegistrationID = regInfo.PtRegistrationID;
        //            }
        //            NewRegDetailsList = new List<long>();
        //            NewPclRequestList = new List<long>();
        //            if(regDetailList != null && regDetailList.Count > 0)
        //            {
        //                AddRegistrationDetails(regInfo, regDetailList, conn, tran, out NewRegDetailsList);
        //            }
        //            if (pclRequestList != null && pclRequestList.Count > 0)
        //            {
        //                long medicalRecID = 0;
        //                CreatePatientMedialRecordIfNotExists(regInfo.PatientID.Value,regInfo.ExamDate, out medicalRecID, conn, tran);
        //                long temp;
        //                foreach (var request in pclRequestList)
        //                {
        //                    if(request.PatientPCLRequestIndicators != null)
        //                    {
        //                        CalcInvoiceItems((IEnumerable<IInvoiceItem>)request.PatientPCLRequestIndicators, null);
        //                    }
        //                    AddPCLRequest(medicalRecID,regInfo.PtRegistrationID,request,conn,tran,out temp);
        //                    NewPclRequestList.Add(temp);
        //                }
        //            }
        //            if (deletedRegDetailList != null && deletedRegDetailList.Count > 0)
        //            {
        //                PatientProvider.Instance.UpdateRegistrationDetailsStatus(regInfo.PtRegistrationID, deletedRegDetailList, conn, tran);
        //            }
        //            if (deletedPclRequestList != null && deletedPclRequestList.Count > 0)
        //            {
        //                //Trong moi request chi lay nhung item da thay doi thoi.
        //                List<PatientPCLRequest> modifiedPcl = GetModifiedPclItems(deletedPclRequestList);
        //                if(modifiedPcl.Count > 0)
        //                {
        //                    PatientProvider.Instance.UpdatePclRequestStatus(regInfo.PtRegistrationID, modifiedPcl, conn, tran);
        //                }
        //            }
        //            tran.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            tran.Rollback();
        //            throw;
        //        }
        //    }
        //    return true;
        //}

        public bool AddPCLRequestForNonRegisteredPatient(long patientID, PatientPCLRequest pclRequest,long V_RegistrationType, out long newPclRequestID)
        {
            using (var conn = PatientProvider.Instance.CreateConnection())
            {
                conn.Open();

                var tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                try
                {
                    long medicalRecID;
                    long? medicalFileID;
                    CreatePatientMedialRecordIfNotExists(patientID,DateTime.Now, out medicalRecID,out medicalFileID, conn, tran);
                    if (pclRequest.PatientPCLRequestIndicators != null)
                    {
                        CalcInvoiceItems(pclRequest.PatientPCLRequestIndicators, null);
                    }
                    AddPCLRequest(medicalRecID,medicalFileID, null, pclRequest,V_RegistrationType, conn, tran, out newPclRequestID);

                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
            return true;
        }


        public override bool AddPCLRequest(long StaffID,PatientRegistration regInfo, PatientPCLRequest pclRequest, out long newPclRequestID)
        {
            using (var conn = PatientProvider.Instance.CreateConnection())
            {
                conn.Open();

                var tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                try
                {
                    if (regInfo.PtRegistrationID <= 0)
                    {
                        throw new Exception(string.Format("{0}.",eHCMSResources.Z1701_G1_ChuaChonDKCuaCDinhCLS));
                    }

                    long medicalRecID;
                    long? medicalFileID;
                    if(pclRequest.CreatedDate == DateTime.MinValue)
                    {
                        pclRequest.CreatedDate = DateTime.Now;
                    }
                    Debug.Assert(regInfo.PatientID != null, "regInfo.PatientID != null");
                    CreatePatientMedialRecordIfNotExists(regInfo.PatientID.Value, pclRequest.CreatedDate, out medicalRecID,out medicalFileID, conn, tran);
                    if (pclRequest.PatientPCLRequestIndicators != null)
                    {
                        CalcInvoiceItems(pclRequest.PatientPCLRequestIndicators, null);
                    }
                    AddPCLRequest(medicalRecID,medicalFileID, regInfo.PtRegistrationID, pclRequest,(long)regInfo.V_RegistrationType, conn, tran, out newPclRequestID);

                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
            return true;
        }

        //public override void UpdatePCLRequest(long StaffID,PatientRegistration registrationInfo, PatientPCLRequest pclRequest,out List<PatientPCLRequest> listPclSave, DateTime modifiedDate = default(DateTime))
        //{
        //    listPclSave=new List<PatientPCLRequest>();

        //    using (var conn = PatientProvider.Instance.CreateConnection())
        //    {
        //        conn.Open();

        //        var tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

        //        try
        //        {
        //            long PatientPCLReqID_Root = 0;

        //            if(pclRequest.PatientPCLRequestIndicators!=null &&pclRequest.PatientPCLRequestIndicators.Count>0)
        //            {
        //                PatientPCLReqID_Root=pclRequest.PatientPCLRequestIndicators[0].PatientPCLReqID;
        //            }

        //            var newPclRequestList = new List<PatientPCLRequestDetail>();
        //            var modifiedPclRequestList = new List<PatientPCLRequestDetail>();
        //            var deletedPclRequestList = new List<PatientPCLRequestDetail>();

        //            foreach(var requestDetail in pclRequest.PatientPCLRequestIndicators)
        //            {
        //                if(requestDetail.RecordState == RecordState.DETACHED)
        //                {
        //                    newPclRequestList.Add(requestDetail);
        //                    requestDetail.CreatedDate = DateTime.Now;
        //                }
        //                else if (requestDetail.RecordState == RecordState.DELETED)
        //                {
        //                    if (requestDetail.PaidTime != null)
        //                    {
        //                        requestDetail.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
        //                        requestDetail.MarkedAsDeleted = true;
        //                        modifiedPclRequestList.Add(requestDetail);
        //                    }
        //                    else
        //                    {
        //                        deletedPclRequestList.Add(requestDetail);
        //                    }
        //                }
        //                else if (requestDetail.RecordState == RecordState.MODIFIED)
        //                {
        //                    modifiedPclRequestList.Add(requestDetail);
        //                }
        //            }

        //            //Có 3 ds rồi. Lọc lại update, neu khác phòng thì xin số + tách phiếu
        //            //cùng phòng thì mở lại ruột phiếu đó add vô thêm.
        //            if (deletedPclRequestList.Count > 0)
        //            {
        //                PatientProvider.Instance.DeletePCLRequestDetailList(deletedPclRequestList, conn, tran);
        //            }

        //            //Đọc lại coi phiếu này còn detail không, và DeptLocID là nhiêu

        //            var dsAddThemVaoPhieuCungPhong = new List<PatientPCLRequestDetail>();
        //            var dsAddMoiTaoRaPhieuMoi = new List<PatientPCLRequestDetail>();
        //            var dsUpdateLaiPhieuCungPhong = new List<PatientPCLRequestDetail>();

        //            PatientPCLRequestDetail PatientPCLRequestDetail_Root = PatientProvider.Instance.PatientPCLRequestDetails_GetDeptLocID(PatientPCLReqID_Root);

        //            if (PatientPCLRequestDetail_Root != null && PatientPCLRequestDetail_Root.PatientPCLReqID > 0)
        //            {
        //                foreach (var itemnew in newPclRequestList)
        //                {
        //                    if(itemnew.DeptLocation!=null && itemnew.DeptLocation.DeptLocationID>0)
        //                    {
        //                        if (itemnew.DeptLocation.DeptLocationID == PatientPCLRequestDetail_Root.DeptLocation.DeptLocationID)
        //                        {
        //                            dsAddThemVaoPhieuCungPhong.Add(itemnew);
        //                        }
        //                        else
        //                        {
        //                            itemnew.EntityState = EntityState.DETACHED;
        //                            itemnew.RecordState = RecordState.DETACHED;

        //                            itemnew.PatientPCLRequest.EntityState = EntityState.DETACHED;
        //                            itemnew.PatientPCLRequest.RecordState= RecordState.DETACHED;

        //                            dsAddMoiTaoRaPhieuMoi.Add(itemnew);
        //                        }
        //                    }
        //                }

        //                foreach (var itemmodified in modifiedPclRequestList)
        //                {
        //                    if (itemmodified.DeptLocation != null && itemmodified.DeptLocation.DeptLocationID > 0)
        //                    {
        //                        if (itemmodified.DeptLocation.DeptLocationID == PatientPCLRequestDetail_Root.DeptLocation.DeptLocationID)
        //                        {
        //                            dsUpdateLaiPhieuCungPhong.Add(itemmodified);
        //                        }
        //                        else
        //                        {
        //                            itemmodified.EntityState = EntityState.DETACHED;
        //                            itemmodified.RecordState = RecordState.DETACHED;

        //                            itemmodified.PatientPCLRequest.EntityState = EntityState.DETACHED;
        //                            itemmodified.PatientPCLRequest.RecordState = RecordState.DETACHED;

        //                            dsAddMoiTaoRaPhieuMoi.Add(itemmodified);
        //                        }
        //                    }
        //                }
        //            }


        //            //if (newPclRequestList.Count > 0)
        //            //{
        //            //    CalcInvoiceItems(newPclRequestList, null);
        //            //    PatientProvider.Instance.AddPCLRequestDetails(pclRequest.PatientPCLReqID, newPclRequestList, conn, tran);
        //            //}
        //            if (dsAddMoiTaoRaPhieuMoi.Count>0)
        //            {
        //                PatientPCLRequest PhieuMoi=new PatientPCLRequest();
        //                PhieuMoi.ServiceRecID = pclRequest.ServiceRecID;
        //                PhieuMoi.ReqFromDeptLocID = pclRequest.ReqFromDeptLocID;
        //                PhieuMoi.PCLRequestNumID=Guid.NewGuid().ToString();
        //                PhieuMoi.Diagnosis = pclRequest.Diagnosis;
        //                PhieuMoi.DoctorComments = pclRequest.DoctorComments;
        //                PhieuMoi.IsExternalExam = pclRequest.IsExternalExam;
        //                PhieuMoi.IsImported = pclRequest.IsImported;
        //                PhieuMoi.IsCaseOfEmergency = pclRequest.IsCaseOfEmergency;
        //                PhieuMoi.StaffID = pclRequest.StaffID;
        //                PhieuMoi.MarkedAsDeleted = pclRequest.MarkedAsDeleted;
        //                PhieuMoi.V_PCLRequestType = pclRequest.V_PCLRequestType;
        //                PhieuMoi.V_PCLRequestStatus = pclRequest.V_PCLRequestStatus;
        //                PhieuMoi.PaidTime = null;
        //                PhieuMoi.RefundTime = null;
        //                PhieuMoi.CreatedDate = DateTime.Now;
        //                PhieuMoi.AgencyID = pclRequest.AgencyID;
        //                PhieuMoi.InPatientBillingInvID = pclRequest.InPatientBillingInvID;
        //                PhieuMoi.PatientPCLRequestIndicators =new ObservableCollection<PatientPCLRequestDetail>(dsAddMoiTaoRaPhieuMoi);
        //                PhieuMoi.EntityState = EntityState.DETACHED;
        //                PhieuMoi.RecordState = RecordState.DETACHED;

        //                CommonService service=new CommonService();
        //                long NewRegistrationID=0;
        //                List<PatientRegistrationDetail> listPatientRegistrationDetail=new List<PatientRegistrationDetail>();
                        
        //                List<PatientPCLRequest> ListSave = new List<PatientPCLRequest>();
        //                ListSave.Add(PhieuMoi);


        //                service.AddServicesAndPCLRequests(StaffID, registrationInfo, null, ListSave, null, null, out NewRegistrationID, out listPatientRegistrationDetail, out listPclSave, default(DateTime));
        //            }

        //            if (PatientPCLRequestDetail_Root != null && PatientPCLRequestDetail_Root.PatientPCLReqID > 0)
        //            {
        //                if (dsAddThemVaoPhieuCungPhong.Count > 0)
        //                {
        //                    foreach (var itemsave in dsAddThemVaoPhieuCungPhong)
        //                    {
        //                        itemsave.PatientPCLReqID = PatientPCLRequestDetail_Root.PatientPCLReqID;
        //                        itemsave.CreatedDate = DateTime.Now;
        //                        itemsave.V_ExamRegStatus = (long) AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
        //                        itemsave.ServiceSeqNum = PatientPCLRequestDetail_Root.ServiceSeqNum;
        //                        itemsave.ServiceSeqNumType = PatientPCLRequestDetail_Root.ServiceSeqNumType;
        //                        itemsave.DeptLocation = PatientPCLRequestDetail_Root.DeptLocation;
        //                    }

        //                    CalcInvoiceItems(dsAddThemVaoPhieuCungPhong, null);
        //                    PatientProvider.Instance.AddPCLRequestDetails(dsAddThemVaoPhieuCungPhong, conn, tran);
        //                }
        //            }


        //            //if (modifiedPclRequestList.Count > 0)
        //            //{
        //            //    CalcInvoiceItems(modifiedPclRequestList, null);
        //            //    PatientProvider.Instance.UpdatePCLRequestDetailList(modifiedPclRequestList, conn, tran);
        //            //}
        //            if (dsUpdateLaiPhieuCungPhong.Count > 0)
        //            {
        //                CalcInvoiceItems(dsUpdateLaiPhieuCungPhong, null);
        //                PatientProvider.Instance.UpdatePCLRequestDetailList(dsUpdateLaiPhieuCungPhong, conn, tran);
        //            }
                  
        //            tran.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            tran.Rollback();
        //            throw;
        //        }
        //    }
        //}

        public override bool ValidatePaymentInfo(PatientTransactionPayment paymentDetails, out string errorMessage)//PatientPayment paymentDetails,
        {
            errorMessage = string.Empty;
            if (paymentDetails == null)
            {
                errorMessage = "Chưa có thông tin tính tiền";
                return false;
            }

            if (paymentDetails.PayAmount < 0)
            {
                if (paymentDetails.PaymentType.LookupID != (long)AllLookupValues.PaymentType.HOAN_TIEN)
                {
                    errorMessage = "Thông tin nhập vào không đúng.";
                    return false;
                }
                paymentDetails.PayAmount = -paymentDetails.PayAmount;
                paymentDetails.CreditOrDebit = -1;
            }

            if (paymentDetails.PayAmount == 0 )//&& !paymentDetails.hasDetail)
            {
                errorMessage = "Thông tin nhập vào không đúng.";
                return false;
            }

            if (!paymentDetails.PaymentDate.HasValue || paymentDetails.PaymentDate.Value == DateTime.MinValue)
            {
                paymentDetails.PaymentDate = DateTime.Now;
            }

            return true;
        }
    }
}