using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AxLogging;
using DataEntities;
using Service.Core.Common;
using eHCMS.DAL;
using System.Data.Common;
using System;
using eHCMS.Services.Core;
using eHCMS.Configurations;
using System.IO;
using System.Xml.Serialization;
using eHCMS.Caching;

namespace CommonServices
{
    public abstract class RegAndPaymentProcessorBase
    {
        protected PatientRegistration CurrentRegistration { get; set; }

        protected virtual void OnInit()
        {

        }

        public void Init(long registrationID, int FindPatient, bool loadFullInfo = true)
        {
            var registrationInfo = PatientProvider.Instance.GetRegistration(registrationID, FindPatient);
            if (registrationInfo == null)
            {
                throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
            }
            //cho nay nen xem ky lai......
            // CurrentRegistration = registrationInfo.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU ? GetRegistration(registrationID, true) : registrationInfo;
            CurrentRegistration = GetRegistration(registrationID, FindPatient, true);
            // CurrentRegistration = registrationInfo.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU ? GetRegistration(registrationID, true) : registrationInfo;
            // CurrentRegistration = GetRegistration(registrationID, true);
            OnInit();
        }
        public void Init(PatientRegistration regInfo, bool loadFullInfo = true)
        {
            if (regInfo == null)
                throw new Exception(eHCMSResources.T0432_G1_Error);
            if (regInfo.PtRegistrationID > 0)
            {
                if (loadFullInfo)
                {
                    Init(regInfo.PtRegistrationID, regInfo.FindPatient);
                }
                else
                {
                    CurrentRegistration = regInfo;
                }
            }
            else
            {
                CurrentRegistration = regInfo;
                CurrentRegistration.PatientRegistrationDetails = null;
                CurrentRegistration.PCLRequests = null;
                CurrentRegistration.DrugInvoices = null;
                CurrentRegistration.InPatientBillingInvoices = null;

                OnInit();
            }
        }

        protected string GetInPatientBillingInvNumber()
        {
            //Sau nay goi ham Anh Tuan.
            return DateTime.Now.ToString("YYMMddHHmmss");
        }
        //public virtual void AddInPatientBillingInvoice(PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, bool CalcPaymentToEndOfDay, out Dictionary<long, List<long>> DrugIDList_Error, out long NewBillingInvoiceID)
        //{
        //    NewBillingInvoiceID = -1;
        //    DrugIDList_Error = new Dictionary<long, List<long>>();
        //    using (DbConnection conn = PatientProvider.Instance.CreateConnection())
        //    {
        //        conn.Open();

        //        DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

        //        try
        //        {
        //            long NewRegistrationID;
        //            if (registrationInfo.PtRegistrationID <= 0)
        //            {
        //                registrationInfo.V_RegistrationType = AllLookupValues.RegistrationType.NOI_TRU;
        //                AddNewRegistration(registrationInfo, conn, tran, out NewRegistrationID);
        //                registrationInfo.PtRegistrationID = NewRegistrationID;
        //            }

        //            var regDetailIds_Old = new List<long>();
        //            var pclRequestIds_Old = new List<long>();
        //            var outDrugInvIds_Old = new List<long>();

        //            var regDetailList_New = new List<PatientRegistrationDetail>();
        //            var pclRequestList_New = new List<PatientPCLRequest>();
        //            var clinicDeptInvoiceList_New = new List<OutwardDrugClinicDeptInvoice>();

        //            //List nay co chua thong tin tien giuong tinh rieng (truong hop nguoi dung load bill tien giuong co san).
        //            var newRegDetails_BedAlloc = new List<PatientRegistrationDetail>();
        //            if (billingInv.RegistrationDetails != null)
        //            {
        //                foreach (var detail in billingInv.RegistrationDetails)
        //                {
        //                    detail.PatientRegistration = registrationInfo;
        //                    detail.InPatientBillingInvID = billingInv.InPatientBillingInvID;

        //                    if (detail.EntityState == EntityState.NEW || detail.EntityState == EntityState.DETACHED)
        //                    {
        //                        if(detail.BedPatientRegDetail != null)
        //                        {
        //                            newRegDetails_BedAlloc.Add(detail);    
        //                        }
        //                        else
        //                        {
        //                            regDetailList_New.Add(detail);    
        //                        }
        //                    }
        //                    else
        //                    {
        //                        regDetailIds_Old.Add(detail.PtRegDetailID);
        //                    }
        //                }
        //            }
        //            if (billingInv.PclRequests != null)
        //            {
        //                foreach (var patientPCLRequest in billingInv.PclRequests)
        //                {
        //                    if(patientPCLRequest.PatientPCLReqID > 0)
        //                    {
        //                        pclRequestIds_Old.Add(patientPCLRequest.PatientPCLReqID);
        //                    }
        //                    else
        //                    {
        //                        pclRequestList_New.Add(patientPCLRequest);
        //                    }
        //                }
        //            }
        //            if (billingInv.OutwardDrugClinicDeptInvoices != null)
        //            {
        //                foreach (var inv in billingInv.OutwardDrugClinicDeptInvoices)
        //                {
        //                    if (inv.outiID > 0)
        //                    {
        //                        outDrugInvIds_Old.Add(inv.outiID);
        //                    }
        //                    else
        //                    {
        //                        clinicDeptInvoiceList_New.Add(inv);
        //                    }
        //                }
        //            }

        //            bool hasData = regDetailIds_Old.Count > 0
        //                            || pclRequestIds_Old.Count > 0
        //                            || outDrugInvIds_Old.Count > 0
        //                            || regDetailList_New.Count > 0
        //                            || pclRequestList_New.Count > 0
        //                            || clinicDeptInvoiceList_New.Count > 0
        //                            || newRegDetails_BedAlloc.Count > 0;

        //            if(hasData)
        //            {
        //                billingInv.BillingInvNum = GetInPatientBillingInvNumber();
        //                if (registrationInfo.StaffID.HasValue)
        //                {
        //                    billingInv.StaffID = registrationInfo.StaffID.Value;
        //                }
        //                else if (registrationInfo.Staff != null)
        //                {
        //                    billingInv.StaffID = registrationInfo.Staff.StaffID;
        //                }
        //                else
        //                {
        //                    billingInv.StaffID = -1;
        //                }
        //                if (billingInv.InvDate == DateTime.MinValue)
        //                {
        //                    billingInv.InvDate = DateTime.Now;
        //                }
        //                billingInv.PtRegistrationID = registrationInfo.PtRegistrationID;
        //                billingInv.V_InPatientBillingInvStatus = AllLookupValues.V_InPatientBillingInvStatus.NEW;


        //                PatientProvider.Instance.AddInPatientBillingInvoice(billingInv, conn, tran, out NewBillingInvoiceID);
        //                billingInv.InPatientBillingInvID = NewBillingInvoiceID;


        //                foreach (var item in regDetailList_New)
        //                {
        //                    item.InPatientBillingInvID = NewBillingInvoiceID;
        //                }
        //                foreach (var item in newRegDetails_BedAlloc)
        //                {
        //                    item.InPatientBillingInvID = NewBillingInvoiceID;
        //                }

        //                foreach (var item in pclRequestList_New)
        //                {
        //                    item.InPatientBillingInvID = NewBillingInvoiceID;
        //                }
        //                foreach (var item in clinicDeptInvoiceList_New)
        //                {
        //                    item.InPatientBillingInvID = NewBillingInvoiceID;
        //                }
        //            }

        //            List<long> NewRegDetailsList;
        //            if (regDetailList_New.Count > 0)
        //            {
        //                AddRegistrationDetails(registrationInfo, regDetailList_New, conn, tran, out NewRegDetailsList);
        //            }
        //            if (pclRequestList_New.Count > 0)
        //            {
        //                long medicalRecID = 0;
        //                CreatePatientMedialRecordIfNotExists(registrationInfo.PatientID.Value, registrationInfo.ExamDate, out medicalRecID, conn, tran);
        //                long temp;
        //                foreach (var request in pclRequestList_New)
        //                {
        //                    if (request.PatientPCLRequestIndicators != null)
        //                    {
        //                        CalcInvoiceItems((IEnumerable<IInvoiceItem>)request.PatientPCLRequestIndicators, registrationInfo);
        //                    }
        //                    AddPCLRequest(medicalRecID, registrationInfo.PtRegistrationID, request, conn, tran, out temp);
        //                }
        //            }
        //            if (clinicDeptInvoiceList_New.Count > 0)
        //            {
        //                List<long> tempIDList = null;
        //                foreach (var inv in clinicDeptInvoiceList_New)
        //                {
        //                    List<long> ids = null;
        //                    if (inv.OutwardDrugClinicDepts != null)
        //                    {
        //                        ids = new List<long>();
        //                        foreach (var item in inv.OutwardDrugClinicDepts)
        //                        {
        //                            ids.Add(item.GenMedProductItem.GenMedProductID);
        //                        }

        //                        var modifiedInwardList = new List<InwardDrugClinicDept>();
        //                        var newOutwardList = new List<OutwardDrugClinicDept>();

        //                        List<InwardDrugClinicDept> inwardDrugList = PatientProvider.Instance.GetAllInwardDrugClinicDeptByProductList(inv.StoreID.Value, (long)inv.MedProductType, ids, conn, tran);
        //                        if (inwardDrugList != null && inwardDrugList.Count > 0)
        //                        {
        //                            List<InwardDrugClinicDept> tempInwardList;
        //                            List<OutwardDrugClinicDept> tempOutwardList;
        //                            bool bOK;
        //                            foreach (var item in inv.OutwardDrugClinicDepts)
        //                            {
        //                                bOK = CreateOutwardDrugClinicDeptFromInward(inwardDrugList, item, out tempInwardList, out tempOutwardList);
        //                                if (bOK)
        //                                {
        //                                    if (tempInwardList != null)
        //                                    {
        //                                        modifiedInwardList.AddRange(tempInwardList);
        //                                    }
        //                                    if (tempOutwardList != null)
        //                                    {
        //                                        newOutwardList.AddRange(tempOutwardList);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    DrugIDList_Error.AddItem(item.GenMedProductItem.GenMedProductID, inv.StoreID.Value);
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            foreach (var id in ids)
        //                            {
        //                                DrugIDList_Error.AddItem(id, inv.StoreID.Value);
        //                            }
        //                        }
        //                        inv.OutwardDrugClinicDepts = new ObservableCollection<OutwardDrugClinicDept>(newOutwardList);
        //                        if (DrugIDList_Error.Count == 0)
        //                        {
        //                            AddOutwardDrugClinicDept(registrationInfo, inv, modifiedInwardList, conn, tran, out tempIDList);
        //                        }
        //                    }

        //                }
        //            }

        //            PatientProvider.Instance.UpdateRegItemsToBill(NewBillingInvoiceID, regDetailIds_Old, pclRequestIds_Old, outDrugInvIds_Old, conn, tran);

        //            //Danh sach newRegDetails co chua thong tin tien giuong.
        //            if (newRegDetails_BedAlloc.Count > 0)
        //            {
        //                foreach (var detail in newRegDetails_BedAlloc)
        //                {
        //                    long regItemID;
        //                    bool bOK = PatientProvider.Instance.AddNewRegistrationDetails(detail, out regItemID, conn, tran);
        //                    if (bOK && detail.BedPatientRegDetail != null)
        //                    {
        //                        if(CalcPaymentToEndOfDay)
        //                        {
        //                            if(detail.BedPatientRegDetail.BillToDate.Date < DateTime.Now.Date)
        //                            {
        //                                detail.BedPatientRegDetail.BillToDate = DateTime.Now.Date.AddSeconds(86399);
        //                            }
        //                        }
        //                        long bedAllocRegDetailId;
        //                        detail.BedPatientRegDetail.PtRegDetailID = regItemID;
        //                        PatientProvider.Instance.AddBedPatientRegDetail(detail.BedPatientRegDetail, out bedAllocRegDetailId, conn, tran);
        //                    }
        //                }
        //            }

        //            if (DrugIDList_Error == null || DrugIDList_Error.Count == 0)
        //            {
        //                tran.Commit();
        //            }
        //            else
        //            {
        //                tran.Rollback();
        //                NewBillingInvoiceID = -1;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            tran.Rollback();
        //            NewBillingInvoiceID = -1;
        //            throw;
        //        }
        //    }
        //}

        public virtual void UpdateInPatientBillingInvoice(long? StaffID, PatientRegistration registrationInfo, InPatientBillingInvoice billingInv
            , List<PatientRegistrationDetail> newRegDetails
            , List<PatientRegistrationDetail> deletedRegDetails
            , List<PatientPCLRequest> newPclRequests
            , List<PatientPCLRequestDetail> newPclRequestDetails
            , List<PatientPCLRequestDetail> deletedPclRequestDetails
            , List<OutwardDrugClinicDeptInvoice> newOutwardDrugClinicDeptInvoices
            , List<OutwardDrugClinicDeptInvoice> modifiedOutwardDrugClinicDeptInvoices
            , List<PatientRegistrationDetail> modifiedRegDetails
            , List<PatientPCLRequestDetail> modifiedPclRequestDetails

            , out Dictionary<long, List<long>> drugIDListError)
        {
            drugIDListError = new Dictionary<long, List<long>>();

            if (billingInv.InPatientBillingInvID <= 0)
            {
                throw new Exception(string.Format("{0}.",eHCMSResources.Z1702_G1_BillChuaCoID));
            }
            if (billingInv.PaidTime != null)
            {
                throw new Exception(string.Format("{0}.",eHCMSResources.Z1703_G1_BillDaTToanKgTheCNhat));
            }

            using (DbConnection conn = PatientProvider.Instance.CreateConnection())
            {
                conn.Open();

                DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                try
                {
                    var deletedDrugClinicDepts = new List<OutwardDrugClinicDept>();
                    var modifiedDrugClinicDepts = new List<OutwardDrugClinicDept>();
                    if (modifiedOutwardDrugClinicDeptInvoices != null && modifiedOutwardDrugClinicDeptInvoices.Count > 0)
                    {
                        foreach (var inv in modifiedOutwardDrugClinicDeptInvoices)
                        {
                            if (inv.OutwardDrugClinicDepts != null)
                            {
                                foreach (var outwardDrug in inv.OutwardDrugClinicDepts)
                                {
                                    if (outwardDrug.RecordState == RecordState.DELETED)
                                    {
                                        deletedDrugClinicDepts.Add(outwardDrug);
                                    }
                                    else if (outwardDrug.RecordState == RecordState.MODIFIED)
                                    {
                                        //Tinh tien lai.
                                        GetItemPrice(outwardDrug, registrationInfo, 0.0);
                                        GetItemTotalPrice(outwardDrug);

                                        modifiedDrugClinicDepts.Add(outwardDrug);
                                    }
                                }
                            }
                        }
                    }

                    if (deletedRegDetails != null && deletedRegDetails.Count > 0)
                    {
                        PatientProvider.Instance.DeleteRegistrationDetailList(deletedRegDetails, conn, tran);
                    }
                    if (deletedPclRequestDetails != null && deletedPclRequestDetails.Count > 0)
                    {
                        PatientProvider.Instance.DeletePCLRequestDetailList(deletedPclRequestDetails, conn, tran);
                    }

                    if (deletedDrugClinicDepts.Count > 0)
                    {
                        PatientProvider.Instance.DeleteOutwardDrugClinicDeptList(deletedDrugClinicDepts, conn, tran);
                    }
                    if (modifiedDrugClinicDepts.Count > 0)
                    {
                        PatientProvider.Instance.UpdateOutwardDrugClinicDeptList(modifiedDrugClinicDepts, StaffID, conn, tran);
                    }
                    if (newRegDetails != null)
                    {
                        foreach (var item in newRegDetails)
                        {
                            item.InPatientBillingInvID = billingInv.InPatientBillingInvID;
                        }
                    }
                    //if (newPclRequests != null)
                    //{
                    //    foreach (var item in newPclRequests)
                    //    {
                    //        item.InPatientBillingInvID = billingInv.InPatientBillingInvID;
                    //    }
                    //}
                    if (newOutwardDrugClinicDeptInvoices != null)
                    {
                        foreach (var item in newOutwardDrugClinicDeptInvoices)
                        {
                            item.InPatientBillingInvID = billingInv.InPatientBillingInvID;
                        }
                    }

                    if (newRegDetails != null && newRegDetails.Count > 0)
                    {
                        List<long> newRegDetailsList;
                        AddRegistrationDetails(registrationInfo, newRegDetails, conn, tran, out newRegDetailsList);
                    }
                    if (modifiedRegDetails != null && modifiedRegDetails.Count > 0)
                    {
                        CalcInvoiceItems(modifiedRegDetails, registrationInfo);
                        PatientProvider.Instance.UpdateRegistrationDetails(modifiedRegDetails, (int)AllLookupValues.V_FindPatientType.NOI_TRU, conn, tran);
                    }
                    if (newPclRequests != null && newPclRequests.Count > 0)
                    {
                        long medicalRecID;
                        long? medicalFileID;
                        Debug.Assert(registrationInfo.PatientID != null, "registrationInfo.PatientID != null");
                        CreatePatientMedialRecordIfNotExists(registrationInfo.PatientID.Value, registrationInfo.ExamDate, out medicalRecID, out medicalFileID, conn, tran);
                        //tach phieu CLS Noi tru o day ne!
                        //List<PatientPCLRequest> newPclRequests
                        foreach (PatientPCLRequest row in newPclRequests)
                        {
                            if (row.PatientPCLRequestIndicators != null)
                            {
                                foreach (PatientPCLRequestDetail itemrow in row.PatientPCLRequestIndicators)
                                {
                                    itemrow.PatientPCLRequest = row;
                                }
                            }
                        }
                        List<PatientPCLRequestDetail> listrequestDetail = newPclRequests.SelectMany(x => x.PatientPCLRequestIndicators).ToList();
                        var LstRequestNew = SplitVote(listrequestDetail);

                        foreach (var request in LstRequestNew)
                        {
                            request.InPatientBillingInvID = billingInv.InPatientBillingInvID;
                            request.PCLRequestNumID = new ServiceSequenceNumberProvider().GetCode_PCLRequest(request.V_PCLMainCategory);
                            if (request.PatientPCLRequestIndicators != null)
                            {
                                CalcInvoiceItems(request.PatientPCLRequestIndicators, registrationInfo);
                            }
                            long temp;
                            AddPCLRequest(medicalRecID, medicalFileID, registrationInfo.PtRegistrationID, request, (long)registrationInfo.V_RegistrationType, conn, tran, out temp);
                        }
                    }

                    if (newPclRequestDetails != null && newPclRequestDetails.Count > 0)
                    {
                        PatientProvider.Instance.AddPCLRequestDetails(newPclRequestDetails, conn, tran);
                    }

                    if (modifiedPclRequestDetails != null && modifiedPclRequestDetails.Count > 0)
                    {
                        CalcInvoiceItems(modifiedPclRequestDetails, registrationInfo);
                        PatientProvider.Instance.UpdatePCLRequestDetailList(modifiedPclRequestDetails, conn, tran);
                    }

                    if (newOutwardDrugClinicDeptInvoices != null && newOutwardDrugClinicDeptInvoices.Count > 0)
                    {
                        foreach (var inv in newOutwardDrugClinicDeptInvoices)
                        {
                            if (inv.OutwardDrugClinicDepts == null) continue;
                            var ids = inv.OutwardDrugClinicDepts.Select(item => item.GenMedProductItem.GenMedProductID).ToList();

                            var modifiedInwardList = new List<InwardDrugClinicDept>();
                            var newOutwardList = new List<OutwardDrugClinicDept>();

                            var inwardDrugList = PatientProvider.Instance.GetAllInwardDrugClinicDeptByProductList(inv.StoreID.Value, (long)inv.MedProductType, ids, conn, tran);
                            if (inwardDrugList != null && inwardDrugList.Count > 0)
                            {
                                foreach (var item in inv.OutwardDrugClinicDepts)
                                {
                                    List<InwardDrugClinicDept> tempInwardList;
                                    List<OutwardDrugClinicDept> tempOutwardList;
                                    var bOK = CreateOutwardDrugClinicDeptFromInward(inwardDrugList, item, out tempInwardList, out tempOutwardList);
                                    if (bOK)
                                    {
                                        if (tempInwardList != null)
                                        {
                                            modifiedInwardList.AddRange(tempInwardList);
                                        }
                                        if (tempOutwardList != null)
                                        {
                                            newOutwardList.AddRange(tempOutwardList);
                                        }
                                    }
                                    else
                                    {
                                        drugIDListError.AddItem(item.GenMedProductItem.GenMedProductID, inv.StoreID.Value);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var id in ids)
                                {
                                    drugIDListError.AddItem(id, inv.StoreID.Value);
                                }
                            }
                            inv.OutwardDrugClinicDepts = new ObservableCollection<OutwardDrugClinicDept>(newOutwardList);
                            if (drugIDListError.Count == 0)
                            {
                                List<long> tempIDList;
                                AddOutwardDrugClinicDept(registrationInfo, inv, modifiedInwardList, conn, tran, out tempIDList);
                            }
                        }
                    }

                    if (modifiedOutwardDrugClinicDeptInvoices != null && modifiedOutwardDrugClinicDeptInvoices.Count > 0)
                    {
                        //Chi xu ly nhung thang them moi. Delete da xu ly o tren roi.
                        foreach (var inv in modifiedOutwardDrugClinicDeptInvoices)
                        {
                            if (inv.OutwardDrugClinicDepts != null)
                            {
                                var ids = (from item in inv.OutwardDrugClinicDepts where item.RecordState == RecordState.DETACHED select item.GenMedProductItem.GenMedProductID).ToList();

                                var modifiedInwardList = new List<InwardDrugClinicDept>();
                                var newOutwardList = new List<OutwardDrugClinicDept>();

                                var inwardDrugList = PatientProvider.Instance.GetAllInwardDrugClinicDeptByProductList(inv.StoreID.Value, (long)inv.MedProductType, ids, conn, tran);
                                if (inwardDrugList != null && inwardDrugList.Count > 0)
                                {
                                    foreach (var item in inv.OutwardDrugClinicDepts)
                                    {
                                        List<InwardDrugClinicDept> tempInwardList;
                                        List<OutwardDrugClinicDept> tempOutwardList;
                                        var bOK = CreateOutwardDrugClinicDeptFromInward(inwardDrugList, item, out tempInwardList, out tempOutwardList);
                                        if (bOK)
                                        {
                                            if (tempInwardList != null)
                                            {
                                                modifiedInwardList.AddRange(tempInwardList);
                                            }
                                            if (tempOutwardList != null)
                                            {
                                                newOutwardList.AddRange(tempOutwardList);
                                            }
                                        }
                                        else
                                        {
                                            drugIDListError.AddItem(item.GenMedProductItem.GenMedProductID, inv.StoreID.Value);
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var id in ids)
                                    {
                                        drugIDListError.AddItem(id, inv.StoreID.Value);
                                    }
                                }
                                inv.OutwardDrugClinicDepts = new ObservableCollection<OutwardDrugClinicDept>(newOutwardList);
                                if (drugIDListError.Count == 0)
                                {
                                    List<long> tempIDList;
                                    AddOutwardDrugClinicDept(registrationInfo, inv, modifiedInwardList, conn, tran, out tempIDList);
                                }
                            }

                        }
                    }

                    if (drugIDListError == null || drugIDListError.Count == 0)
                    {
                        tran.Commit();
                    }
                    else
                    {
                        tran.Rollback();
                    }
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public virtual void CreateBillingInvoiceFromExistingItems(PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, out long newBillingInvoiceID)
        {
            using (DbConnection conn = PatientProvider.Instance.CreateConnection())
            {
                conn.Open();

                DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                try
                {
                    billingInv.BillingInvNum = GetInPatientBillingInvNumber();
                    if (registrationInfo.PtRegistrationID <= 0)
                    {
                        throw new Exception("Chua chon thong tin dang ky");
                    }
                    billingInv.PtRegistrationID = registrationInfo.PtRegistrationID;

                    PatientProvider.Instance.AddInPatientBillingInvoice(billingInv, conn, tran, out newBillingInvoiceID);
                    billingInv.InPatientBillingInvID = newBillingInvoiceID;
                    var regDetailIds = new List<long>();
                    List<long> pclRequestIds = null;
                    List<long> outDrugInvIds = null;

                    var newRegDetails = new List<PatientRegistrationDetail>();
                    if (billingInv.RegistrationDetails != null)
                    {
                        foreach (var detail in billingInv.RegistrationDetails)
                        {
                            detail.PatientRegistration = registrationInfo;
                            detail.InPatientBillingInvID = billingInv.InPatientBillingInvID;

                            if (detail.EntityState == EntityState.NEW || detail.EntityState == EntityState.DETACHED)
                            {
                                newRegDetails.Add(detail);
                            }
                            else
                            {
                                regDetailIds.Add(detail.PtRegDetailID);
                            }
                        }
                    }
                    if (billingInv.PclRequests != null)
                    {
                        pclRequestIds = billingInv.PclRequests.Select(item => item.PatientPCLReqID).ToList();
                    }
                    if (billingInv.OutwardDrugClinicDeptInvoices != null)
                    {
                        outDrugInvIds = billingInv.OutwardDrugClinicDeptInvoices.Select(item => item.outiID).ToList();
                    }
                    PatientProvider.Instance.UpdateRegItemsToBill(newBillingInvoiceID, regDetailIds, pclRequestIds, outDrugInvIds, conn, tran);

                    //Danh sach newRegDetails co chua thong tin tien giuong.
                    if (newRegDetails.Count > 0)
                    {
                        foreach (var detail in newRegDetails)
                        {
                            long regItemID;
                            bool bOK = PatientProvider.Instance.AddNewRegistrationDetails(detail, out regItemID, conn, tran);
                            if (bOK && detail.BedPatientRegDetail != null)
                            {
                                long bedAllocRegDetailId;
                                detail.BedPatientRegDetail.PtRegDetailID = regItemID;
                                PatientProvider.Instance.AddBedPatientRegDetail(detail.BedPatientRegDetail, out bedAllocRegDetailId, conn, tran);
                            }
                        }
                    }

                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="registrationInfo"></param>
        ///// <param name="DrugIDList_Error">Danh sach cac ID phieu nhap khong du so luong xuat</param>
        //public virtual void SaveInPatientRegistration(PatientRegistration registrationInfo, long billingType, out Dictionary<long, List<long>> DrugIDList_Error, out InPatientBillingInvoice AddedBillingInvoice)
        //{
        //    //DO NOTHING HERE
        //    DrugIDList_Error = null;
        //    AddedBillingInvoice = null;
        //}
        /// <summary>
        /// Thêm danh sách các dịch vụ vào một đăng ký đã có.
        /// </summary>

        protected virtual void AddRegistrationDetails(PatientRegistration regInfo, List<PatientRegistrationDetail> regDetailList, DbConnection conn, DbTransaction tran, out List<long> newRegistrationIDList)
        {
            PatientProvider.Instance.AddRegistrationDetails(regInfo.PtRegistrationID, regInfo.FindPatient, regDetailList, conn, tran, out newRegistrationIDList);
        }
        protected virtual void AddOutwardDrugClinicDept(PatientRegistration regInfo, OutwardDrugClinicDeptInvoice inv, List<InwardDrugClinicDept> updatedInwardItems, DbConnection conn, DbTransaction tran, out List<long> inwardDrugIDListError)
        {
            PatientProvider.Instance.AddOutwardDrugClinicInvoice(inv, updatedInwardItems, conn, tran, out inwardDrugIDListError);
        }

        /// <summary>
        /// Tạo mới đăng ký (không bao gồm chi tiết)
        /// </summary>
        /// <param name="regInfo"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <param name="newRegistrationID"></param>
        protected abstract void AddNewRegistration(PatientRegistration regInfo,
                                                           DbConnection conn, DbTransaction tran, out long newRegistrationID);



        public virtual bool AddInPatientAdmission(PatientRegistration regInfo,
                                                   out long newRegistrationID, out long admissionId)
        {
            newRegistrationID = 0;
            admissionId = 0;
            using (DbConnection conn = PatientProvider.Instance.CreateConnection())
            {
                conn.Open();

                DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                try
                {
                    //AddNewRegistration(regInfo, conn, tran, out newRegistrationID);
                    //if (newRegistrationID <= 0) //Add khong duoc.
                    //{
                    //    tran.Rollback();
                    //    return false;
                    //}
                    regInfo.AdmissionInfo.PtRegistrationID = regInfo.PtRegistrationID;

                    if (regInfo.AdmissionInfo.InPatientAdmDisDetailID <= 0)
                    {
                        PatientProvider.Instance.InsertInPatientAdmDisDetails(regInfo.AdmissionInfo, conn, tran, out admissionId);
                        regInfo.AdmissionInfo.InPatientAdmDisDetailID = admissionId;
                        PatientProvider.Instance.UpdatePatientRegistrationStatus(regInfo.PtRegistrationID, regInfo.FindPatient, AllLookupValues.RegistrationStatus.OPENED, conn, tran);
                        //PatientProvider.Instance.UpdatePatientRegistrationStatus(regInfo.PtRegistrationID, AllLookupValues.RegistrationStatus.OPENED, conn, tran);
                    }

                    if (regInfo.AdmissionInfo.InPatientDeptDetails != null)
                    {
                        var firstDeptDetail = regInfo.AdmissionInfo.InPatientDeptDetails
                                            .Where(item => item.InPatientDeptDetailID <= 0).FirstOrDefault();

                        Debug.Assert(firstDeptDetail != null, "firstDeptDetail != null");
                        firstDeptDetail.InPatientAdmDisDetailID = regInfo.AdmissionInfo.InPatientAdmDisDetailID;
                        long inPatientDeptDetailId;
                        PatientProvider.Instance.InsertInPatientDeptDetails(firstDeptDetail, conn, tran, out inPatientDeptDetailId);
                    }

                    tran.Commit();
                    //Dat giuong khong duoc cung khong sao.
                    if (regInfo.BedAllocations != null)
                    {
                        foreach (var item in regInfo.BedAllocations)
                        {
                            if (item.BedPatientID <= 0)
                            {
                                try
                                {
                                    item.PtRegistrationID = regInfo.PtRegistrationID;
                                    BedAllocations.Instance.AddNewBedPatientAllocs(item);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        /// <summary>
        /// Tạo một Chỉ định CLS cho một đăng ký đã có.
        /// </summary>
        /// <param name="regInfo"></param>
        /// <param name="pclRequest"></param>
        /// <param name="newPclRequestID"></param>
        /// <returns></returns>
        public abstract bool AddPCLRequest(long StaffID, PatientRegistration regInfo, PatientPCLRequest pclRequest,
                                           out long newPclRequestID);


        protected void CreatePatientMedialRecordIfNotExists(long patientID, DateTime medicalRecordDate, out long patientMedicalRecordID,
            out long? medicalFileID, DbConnection conn, DbTransaction tran)
        {
            patientMedicalRecordID = -1;
            long medicalRecordID;

            if (!PatientProvider.Instance.CheckIfMedicalRecordExists(patientID, out medicalRecordID, out medicalFileID, conn, tran))
            {
                var medicalRecord = new PatientMedicalRecord
                                        {
                                            PatientID = patientID,
                                            NationalMedicalCode = "MedicalCode",
                                            CreatedDate =
                                                medicalRecordDate == DateTime.MinValue
                                                    ? DateTime.Now
                                                    : medicalRecordDate
                                        };

                var bCreateMROK = PatientProvider.Instance.AddNewPatientMedicalRecord(medicalRecord, out medicalRecordID, conn, tran);
                if (!bCreateMROK)
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1780_G1_CannotCreatePatient));
                }
            }
            patientMedicalRecordID = medicalRecordID;
        }

        /// <summary>
        /// Lưu yêu cầu CLS (cùng với các chi tiết yêu cầu) vào database.
        /// </summary>
        /// <param name="medicalRecordId"></param>
        /// <param name="registrationID"></param>
        /// <param name="pclRequest"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <param name="newRequestId"></param>
        protected virtual void AddPCLRequest(long medicalRecordId, long? patientMedicalFileID, long? registrationID, PatientPCLRequest pclRequest
            , long V_RegistrationType, DbConnection conn, DbTransaction tran, out long newRequestId)
        {
            newRequestId = 0;
            if (pclRequest.PatientPCLReqID <= 0) //Thêm mới.
            {
                long serviceRecordID;
                if (pclRequest.PatientServiceRecord != null && pclRequest.PatientServiceRecord.ServiceRecID > 0)
                {
                    serviceRecordID = pclRequest.PatientServiceRecord.ServiceRecID;
                }
                else
                {
                    serviceRecordID = pclRequest.ServiceRecID.GetValueOrDefault(-1);
                }
                if (serviceRecordID <= 0) // Chua co => Tao moi
                {
                    PatientServiceRecord serviceRecord = CreateNewServiceRecord(registrationID, medicalRecordId, patientMedicalFileID, pclRequest.CreatedDate, V_RegistrationType, conn, tran);
                    pclRequest.PatientServiceRecord = serviceRecord;
                }
                //DateTime now = DateTime.Now;
                if (pclRequest.PatientPCLRequestIndicators != null)
                {
                    foreach (var details in pclRequest.PatientPCLRequestIndicators)
                    {
                        //details.CreatedDate = now;
                        if (details.CreatedDate == DateTime.MinValue)
                        {
                            details.CreatedDate = pclRequest.CreatedDate;
                        }
                    }
                }
                PatientProvider.Instance.AddPCLRequestWithDetails(pclRequest, conn, tran, out newRequestId);
            }
        }

        private PatientServiceRecord CreateNewServiceRecord(long? registrationID, long medicalRecordID, long? PatientMedicalFileID,
            DateTime serviceRecDate, long _V_RegistrationType, DbConnection conn, DbTransaction tran)
        {
            //Tao 1 PatientServiceRecord.
            long serviceRecordID;
            var serviceRecord = new PatientServiceRecord
                                    {
                                        PtRegistrationID = registrationID,
                                        StaffID = null,
                                        ExamDate = serviceRecDate == DateTime.MinValue ? DateTime.Now : serviceRecDate,
                                        V_Behaving = (long)AllLookupValues.Behaving.CHI_DINH_XET_NGHIEM_CLS,
                                        V_ProcessingType =
                                            (long)AllLookupValues.ProcessingType.PARA_CLINICAL_EXAMINATION,
                                        V_RegistrationType = (AllLookupValues.RegistrationType)_V_RegistrationType

                                    };

            var bOK = PatientProvider.Instance.AddNewPatientServiceRecord(medicalRecordID, PatientMedicalFileID, serviceRecord, out serviceRecordID, conn, tran);
            if (bOK)
            {
                serviceRecord.ServiceRecID = serviceRecordID;
            }
            else
            {
                throw new Exception(string.Format("{0}.",eHCMSResources.Z1796_G1_KgTaoDcPatientServiceRecord));
            }
            return serviceRecord;
        }
        public virtual bool CancelRegistration(PatientRegistration registrationInfo)
        {
            using (var conn = PatientProvider.Instance.CreateConnection())
            {
                conn.Open();

                var tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                try
                {
                    if (registrationInfo.RegistrationStatus != AllLookupValues.RegistrationStatus.OPENED
                    && registrationInfo.RegistrationStatus != AllLookupValues.RegistrationStatus.PENDING)
                    {
                        throw new Exception(string.Format("{0}.",eHCMSResources.Z1705_G1_KgTheHuyDK));
                    }

                    if (registrationInfo.DrugInvoices != null && registrationInfo.DrugInvoices.Count > 0)
                    {
                        throw new Exception(string.Format("{0}.",eHCMSResources.Z0198_G1_DaLayThuocKgTheHuyDK));
                    }

                    registrationInfo.RegistrationStatus = AllLookupValues.RegistrationStatus.REFUND;
                    var bOK = PatientProvider.Instance.UpdateRegistrationStatus(registrationInfo, conn, tran);
                    if (!bOK)
                    {
                        tran.Rollback();
                        return false;
                    }
                    var cancelledRegItems = new List<PatientRegistrationDetail>();
                    var deletedRegItems = new List<PatientRegistrationDetail>();
                    if (registrationInfo.PatientRegistrationDetails != null)
                    {
                        foreach (var regDetail in registrationInfo.PatientRegistrationDetails)
                        {
                            if (regDetail.ExamRegStatus == AllLookupValues.ExamRegStatus.HOAN_TAT)
                            {
                                throw new Exception(string.Format("{0}.",eHCMSResources.Z0199_G1_DaKBKgTheHuyDK));
                            }
                            if (regDetail.PaidTime != null)
                            {
                                regDetail.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                                cancelledRegItems.Add(regDetail);
                            }
                            else
                            {
                                regDetail.RecordState = RecordState.DELETED;
                                regDetail.RegDetailCancelStaffID = registrationInfo.RegCancelStaffID;
                                deletedRegItems.Add(regDetail);
                            }
                        }
                    }
                    var cancelledPclRequests = new List<PatientPCLRequest>();
                    var deletedPclRequests = new List<PatientPCLRequest>();
                    if (registrationInfo.PCLRequests != null)
                    {
                        foreach (var request in registrationInfo.PCLRequests)
                        {
                            if (request.PatientPCLRequestIndicators != null)
                            {
                                foreach (var requestDetails in request.PatientPCLRequestIndicators)
                                {
                                    if (requestDetails.ExamRegStatus == AllLookupValues.ExamRegStatus.HOAN_TAT)
                                    {
                                        throw new Exception(string.Format("{0}.",eHCMSResources.Z0200_G1_DaLamCLSKgTheHuyDK));
                                    }
                                    if (requestDetails.PaidTime.HasValue)
                                    {
                                        requestDetails.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                                        requestDetails.RecordState = RecordState.MODIFIED;
                                    }
                                    else
                                    {
                                        requestDetails.MarkedAsDeleted = true;
                                        requestDetails.RecordState = RecordState.DELETED;
                                    }
                                }

                                if (request.PaidTime != null)
                                {
                                    request.RecordState = RecordState.MODIFIED;
                                    request.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.CANCEL;
                                    cancelledPclRequests.Add(request);
                                }
                                else
                                {
                                    request.MarkedAsDeleted = true;
                                    request.RecordState = RecordState.DELETED;
                                    deletedPclRequests.Add(request);
                                }
                            }
                        }
                    }



                    if (deletedRegItems.Count > 0)
                    {
                        PatientProvider.Instance.UpdateRegistrationDetailsStatus(registrationInfo.PtRegistrationID, deletedRegItems, conn, tran);
                    }
                    if (deletedPclRequests.Count > 0)
                    {
                        //Trong moi request chi lay nhung item da thay doi thoi.
                        List<PatientPCLRequest> modifiedPcl = GetModifiedPclItems(deletedPclRequests);
                        if (modifiedPcl.Count > 0)
                        {
                            PatientProvider.Instance.UpdatePclRequestStatus(registrationInfo.PtRegistrationID, modifiedPcl, conn, tran);
                        }
                    }

                    //RemovePaidRegItems(registrationInfo,cancelledRegItems,cancelledPclRequests,null,null,null,conn,tran);
                    tran.Commit();
                    return true;
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }

        #region PHẦN TÍNH GIÁ TIỀN
        protected virtual void GetItemTotalPrice(IInvoiceItem invoiceItem)
        {
            //Tính tổng tiền cho mỗi InvoiceItem.
            // Chưa tính trường hợp: nhiều dịch vụ KHÁM BỆNH chỉ tính 1 lần

            invoiceItem.TotalCoPayment = Math.Ceiling(invoiceItem.PatientCoPayment * (decimal)invoiceItem.Qty);
            invoiceItem.TotalHIPayment = Math.Floor(invoiceItem.HIPayment * (decimal)invoiceItem.Qty);
            invoiceItem.TotalInvoicePrice = invoiceItem.InvoicePrice * (decimal)invoiceItem.Qty;
            invoiceItem.TotalPatientPayment = invoiceItem.TotalInvoicePrice - invoiceItem.TotalHIPayment; //invoiceItem.PatientPayment * (decimal)invoiceItem.Qty;
            invoiceItem.TotalPriceDifference = invoiceItem.PriceDifference * (decimal)invoiceItem.Qty;
        }

        /// <summary>
        /// Tính giá tiền của 1 invoice item (Chi tiết đăng ký, Chi tiết CLS, Chi tiết thuốc...) trong ngữ cảnh của đăng ký registration
        /// </summary>
        /// <param name="invoiceItem"></param>
        /// <param name="registration"></param>
        /// <param name="hiBenefit"></param>
        protected virtual void GetItemPrice(IInvoiceItem invoiceItem, PatientRegistration registration, double hiBenefit)
        {
            invoiceItem.HisID = null;
            invoiceItem.HIBenefit = null;

            if (invoiceItem.ChargeableItem != null)
            {
                invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.NormalPrice;
            }

            invoiceItem.PriceDifference = 0;
            invoiceItem.HIPayment = 0;
            invoiceItem.PatientCoPayment = 0;
            invoiceItem.PatientPayment = invoiceItem.InvoicePrice;
        }

        /// <summary>
        /// Tính giá tiền cho 1 invoice item (bao  gồm tổng số tiền) 
        /// </summary>
        /// <param name="invoiceItem"></param>
        /// <param name="registration"></param>
        protected virtual void CalcInvoiceItem(IInvoiceItem invoiceItem, PatientRegistration registration)
        {
            GetItemPrice(invoiceItem, registration);
            GetItemTotalPrice(invoiceItem);
        }

        /// <summary>
        /// Tính giá tiền cho 1 list các invoice item
        /// </summary>
        protected virtual void CalcInvoiceItems(IEnumerable<IInvoiceItem> colInvoiceItems, PatientRegistration registration)
        {

        }

        protected static PayableSum CalcPayableSum(IList<IInvoiceItem> colInvoiceItems)
        {
            var sum = new PayableSum();
            foreach (var item in colInvoiceItems)
            {
                if (item.RecordState == RecordState.DELETED ||
                    item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI) continue;
                sum.TotalCoPayment += item.TotalCoPayment;
                sum.TotalHIPayment += item.TotalHIPayment;
                sum.TotalPrice += item.TotalInvoicePrice;
                sum.TotalPatientPayment += item.TotalPatientPayment;
                sum.TotalPriceDifference += item.TotalPriceDifference;
            }
            return sum;
        }
        #endregion

        #region PHẦN TRẢ TIỀN
        //neu chi dung cho noi tru thi sua o day la dc roi ne
        public virtual void PayForInPatientRegistration(long StaffID, PatientRegistration registrationInfo,
                                       PatientTransactionPayment paymentDetails,//PatientPayment paymentDetails,
                                       List<InPatientBillingInvoice> colBillingInvoices,
                                       DateTime paidTime,
                                       out PatientTransaction transaction, out PatientTransactionPayment paymentInfo, out long PtCashAdvanceID)//out PatientPayment paymentInfo
        {
            PtCashAdvanceID = -1;
            paymentInfo = null;
            if (paymentDetails == null)
            {
                throw new Exception(eHCMSResources.Z1707_G1_ChuaCoTTinTinhTien);
            }

            if (paymentDetails.PayAmount < 0)
            {
                if (paymentDetails.PaymentType.LookupID != (long)AllLookupValues.PaymentType.HOAN_TIEN)
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1834_G1_TTinNhapVaoKgDung));
                }
                paymentDetails.PayAmount = -paymentDetails.PayAmount;
                paymentDetails.CreditOrDebit = -1;
            }

            if (paidTime == DateTime.MinValue)
            {
                paidTime = DateTime.Now;
            }

            using (var conn = PatientProvider.Instance.CreateConnection())
            {
                conn.Open();
                var tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                try
                {
                    if (registrationInfo == null)
                    {
                        throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
                    }
                    PatientTransaction currentTransaction;
                    //Neu da co transaction roi thi kiem tra thay doi tren transaction nay.
                    if (registrationInfo.PatientTransaction == null)
                    {
                        currentTransaction = CreateTransationForRegistration(registrationInfo);
                        long tranID;
                        PatientProvider.Instance.OpenTransaction_InPt(currentTransaction, out tranID, conn, tran);
                        currentTransaction.TransactionID = tranID;
                        registrationInfo.PatientTransaction = currentTransaction;
                    }
                    else
                    {
                        currentTransaction = registrationInfo.PatientTransaction;
                        currentTransaction.PatientTransactionDetails = PatientProvider.Instance.GetAlltransactionDetails_InPt(currentTransaction.TransactionID).ToObservableCollection();
                    }
                    PatientProvider.Instance.UpdatePaidTimeForBillingInvoice(colBillingInvoices, paidTime, conn, tran);

                    string ListIDOutTranDetails = "";
                    BalanceTransaction(StaffID, out ListIDOutTranDetails, registrationInfo, null, null, null, colBillingInvoices, paidTime, conn, tran);

                    if (paymentDetails.PayAmount != 0 || paymentDetails.PayAdvance != 0)
                    {
                        CreatePaymentAndPayAdvance(registrationInfo.PtRegistrationID, (long)registrationInfo.V_RegistrationType, ListIDOutTranDetails, paymentDetails, currentTransaction.TransactionID, conn, tran, out PtCashAdvanceID);
                    }

                    paymentInfo = paymentDetails;

                    transaction = registrationInfo.PatientTransaction;
                    try
                    {
                        var allPayments = CommonProvider.Payments.GetAllPayments_InPt(transaction.TransactionID, conn, tran);
                        if (allPayments != null)
                        {
                            transaction.PatientTransactionPayments = allPayments.ToObservableCollection();
                        }
                    }
                    catch (Exception)
                    {

                    }
                    tran.Commit();
                }
                catch (Exception outerEx)
                {
                    AxLogger.Instance.LogError(outerEx);
                    tran.Rollback();
                    throw;
                }
            }
        }

        private void UpdateInvoiceItemState(IInvoiceItem invoiceItem, RecordState fromState, RecordState toState)
        {
            if (invoiceItem.RecordState == fromState)
            {
                invoiceItem.RecordState = toState;
            }
        }

        /// <summary>
        /// Cập nhật PaidTime cho những item muốn tính tiền
        /// </summary>
        protected virtual void UpdatePaidOrRefundTime(PatientRegistration regInfo, DateTime paidTime,
                                        List<PatientRegistrationDetail> colPaidRegDetails,
                                        List<PatientPCLRequest> colPaidPclRequests,
                                        List<OutwardDrugInvoice> colPaidDrugInvoice,
                                        List<InPatientBillingInvoice> colBillingInvoices,
                                        DbConnection conn, DbTransaction tran)
        {
            //Cap nhat Ngay Tra Tien hoac Ngay Hoan Tien cho danh sach Dich Vu.
            if (colPaidRegDetails != null)
            {
                foreach (var details in colPaidRegDetails)
                {
                    if (details.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    {
                        if (details.RefundTime == null)
                        {
                            details.RefundTime = paidTime;
                            UpdateInvoiceItemState(details, RecordState.UNCHANGED, RecordState.MODIFIED);
                            continue;
                        }
                    }
                    else
                    {
                        if (details.PaidTime == null)
                        {
                            details.PaidTime = paidTime;
                            UpdateInvoiceItemState(details, RecordState.UNCHANGED, RecordState.MODIFIED);
                            continue;
                        }
                    }
                }
            }
            //Cap nhat Ngay Tra Tien hoac Ngay Hoan Tien cho danh sach CLS
            if (colPaidPclRequests != null)
            {
                foreach (var details in colPaidPclRequests)
                {
                    //if (details.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    if (details.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CANCEL)
                    {
                        if (details.RefundTime == null)
                        {
                            details.RefundTime = paidTime;
                            UpdateInvoiceItemState(details, RecordState.UNCHANGED, RecordState.MODIFIED);
                            if (details.PatientPCLRequestIndicators != null)
                            {
                                foreach (var item in details.PatientPCLRequestIndicators)
                                {
                                    if (item.RefundTime == null)
                                    {
                                        item.RefundTime = paidTime;
                                        UpdateInvoiceItemState(item, RecordState.UNCHANGED, RecordState.MODIFIED);
                                    }
                                }
                            }

                            continue;
                        }
                    }
                    else
                    {
                        if (details.PaidTime == null)
                        {
                            details.PaidTime = paidTime;
                            UpdateInvoiceItemState(details, RecordState.UNCHANGED, RecordState.MODIFIED);
                            if (details.PatientPCLRequestIndicators != null)
                            {
                                foreach (var item in details.PatientPCLRequestIndicators)
                                {
                                    if (item.PaidTime == null)
                                    {
                                        item.PaidTime = paidTime;
                                        UpdateInvoiceItemState(item, RecordState.UNCHANGED, RecordState.MODIFIED);
                                    }
                                }
                            }

                            continue;
                        }
                        if (details.PatientPCLRequestIndicators != null)
                        {
                            foreach (var item in details.PatientPCLRequestIndicators)
                            {
                                if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && item.RefundTime == null)
                                {
                                    item.RefundTime = paidTime;
                                    UpdateInvoiceItemState(item, RecordState.UNCHANGED, RecordState.MODIFIED);
                                }
                            }
                        }
                    }
                }
            }
            if (colPaidDrugInvoice != null)
            {
                foreach (var invoice in colPaidDrugInvoice)
                {
                    if (invoice.ReturnID == null) //Phieu xuat binh thuong
                    {
                        if (invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)
                        {
                            if (invoice.PaidTime == null)
                            {
                                invoice.PaidTime = paidTime;
                                UpdateInvoiceItemState(invoice, RecordState.UNCHANGED, RecordState.MODIFIED);
                            }
                        }
                        else if (invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                        {
                            if (invoice.RefundTime != null)
                            {
                                invoice.RefundTime = paidTime;
                                UpdateInvoiceItemState(invoice, RecordState.UNCHANGED, RecordState.MODIFIED);
                            }
                        }
                    }
                    else
                    {
                        if (invoice.PaidTime == null)
                        {
                            invoice.PaidTime = paidTime;
                            UpdateInvoiceItemState(invoice, RecordState.UNCHANGED, RecordState.MODIFIED);
                        }
                    }
                }
            }

            if (colBillingInvoices != null)
            {
                foreach (var billingInvoice in colBillingInvoices)
                {
                    if (billingInvoice.V_InPatientBillingInvStatus == AllLookupValues.V_InPatientBillingInvStatus.NGUNG_TRA_TIEN_LAI)
                    {
                        if (billingInvoice.RefundTime == null)
                        {
                            billingInvoice.RefundTime = paidTime;
                            //UpdateInvoiceItemState(billingInvoice, RecordState.UNCHANGED, RecordState.MODIFIED);
                            continue;
                        }
                    }
                    else
                    {
                        if (billingInvoice.PaidTime == null)
                        {
                            billingInvoice.PaidTime = paidTime;
                            //UpdateInvoiceItemState(billingInvoice, RecordState.UNCHANGED, RecordState.MODIFIED);
                            continue;
                        }
                    }
                }
            }
            //PatientProvider.Instance.UpdatePaidTime(colPaidRegDetails, colPaidPclRequests, colPaidDrugInvoice, colBillingInvoices, conn, tran);
        }
        protected virtual void ProcessPayment(string ListIDOutTranDetails, PatientTransactionPayment paymentDetails, long transactionId, DbConnection conn, DbTransaction tran)//PatientPayment paymentDetails
        {
            long paymentID;
            PatientProvider.Instance.ProcessPayment_New(ListIDOutTranDetails, paymentDetails, transactionId, out paymentID, conn, tran);
            paymentDetails.PtTranPaymtID = paymentID;
        }
        protected virtual void CreatePaymentAndPayAdvance(long PtRegistrationID, long V_RegistrationType, string ListIDOutTranDetails, PatientTransactionPayment paymentDetails, long transactionId, DbConnection conn, DbTransaction tran, out  long PtCashAdvanceID)//PatientPayment paymentDetails
        {
            long paymentID;
            PatientProvider.Instance.CreatePaymentAndPayAdvance(PtRegistrationID, V_RegistrationType, ListIDOutTranDetails, paymentDetails, transactionId, out paymentID, out PtCashAdvanceID, conn, tran);
            paymentDetails.PtTranPaymtID = paymentID;
        }

        #endregion

        public static PatientRegistration GetRegistration(long regID, int FindPatient, bool getFullInfo = false)
        {
            if (regID <= 0)
            {
                return null;
            }
            using (DbConnection connection = PatientProvider.Instance.CreateConnection())
            {
                if (FindPatient == (int)AllLookupValues.PatientFindBy.NGOAITRU)
                {
                    return GetRegistration(regID, FindPatient, connection, null, getFullInfo);
                }
                else
                {
                    return GetRegistration_InPt(regID, FindPatient, connection, null, getFullInfo);
                }
                // return GetRegistration(regID, connection, null, getFullInfo);
            }
        }

        public static PatientRegistration GetRegistration_InPt(long regID, int FindPatient, bool getFullInfo = false)
        {
            if (regID <= 0)
            {
                return null;
            }
            using (DbConnection connection = PatientProvider.Instance.CreateConnection())
            {
                return GetRegistration_InPt(regID, FindPatient, connection, null, getFullInfo);
                // return GetRegistration(regID, connection, null, getFullInfo);
            }
        }

        protected static PatientRegistration GetRegistration(long regID, int FindPatient, DbConnection connection, DbTransaction tran, bool getFullInfo = false)
        {
            PatientRegistration registrationInfo = PatientProvider.Instance.GetRegistration(regID, FindPatient, connection, tran);
            if (registrationInfo == null)
            {
                //Bao loi khong co dang ky nay.
                throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
            }

            if (registrationInfo.PatientID.HasValue && registrationInfo.PatientID.Value > 0)
            {
                registrationInfo.Patient = PatientProvider.Instance.GetPatientByID_Simple(registrationInfo.PatientID.Value, connection, tran);
            }

            //Neu la dang ky noi tru thi chi lay danh sach billing invoice thoi.
            if (//registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU || 
                registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
            {
                List<InPatientBillingInvoice> billingInvoices = PatientProvider.Instance.GetAllInPatientBillingInvoices(registrationInfo.PtRegistrationID, null);
                if (billingInvoices != null)
                {
                    registrationInfo.InPatientBillingInvoices = billingInvoices.ToObservableCollection();
                }
                //Lay thong tin nhap vien:
                registrationInfo.AdmissionInfo = PatientProvider.Instance.GetInPatientAdmissionByRegistrationID(registrationInfo.PtRegistrationID, connection, tran);
                if (registrationInfo.AdmissionInfo != null)
                {
                    registrationInfo.AdmissionInfo.PatientRegistration = registrationInfo;
                }
                //Lay thong tin giuong:
                List<BedPatientAllocs> allBedPatientAlloc = BedAllocations.Instance.GetAllBedAllocationsByRegistrationID(registrationInfo.PtRegistrationID, null, connection, tran);
                if (allBedPatientAlloc != null)
                {
                    registrationInfo.BedAllocations = allBedPatientAlloc.ToObservableCollection();
                }
            }

            List<PatientRegistrationDetail> regDetails = PatientProvider.Instance.GetAllRegistrationDetails(regID, FindPatient, connection, tran);

            if (regDetails != null)
            {
                //registrationInfo.PatientRegistrationDetails = regDetails.ToObservableCollection<PatientRegistrationDetail>();
                registrationInfo.PatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                foreach (var item in regDetails)
                {
                    registrationInfo.PatientRegistrationDetails.Add(item);
                }
            }

            //Can lam sang
            var pclRequestList = PatientProvider.Instance.GetPCLRequestListByRegistrationID(regID, connection, tran);
            if (pclRequestList != null)
            {
                registrationInfo.PCLRequests = pclRequestList.ToObservableCollection();
            }

            //neu co tinh thuoc doc lap = 1 va la benh nhan ngoai tru thi ko thuc hien nhung ham ben duoi nay
            //else thi van de binh thuong

            List<OutwardDrugInvoice> invoiceList = null;
            //ny: vi noi tru cung ban thuoc theo toa cho benh nhan khi xuat vien nen mo cai nay ra

            // if(registrationInfo.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            {
                invoiceList = PatientProvider.Instance.GetDrugInvoiceListByRegistrationID(regID, connection, tran);
            }

            if (invoiceList != null)
            {
                registrationInfo.DrugInvoices = invoiceList.ToObservableCollection();

            }

            if (registrationInfo.PatientTransaction != null)
            {
                if (//registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU || 
                    registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    List<PatientTransactionPayment> allPayments = CommonProvider.Payments.GetAllPayments_InPt(registrationInfo.PatientTransaction.TransactionID, connection, tran);
                    if (allPayments != null)
                    {
                        registrationInfo.PatientTransaction.PatientTransactionPayments = allPayments.ToObservableCollection();
                    }
                    //if (getFullInfo)
                    {
                        registrationInfo.PatientTransaction.PatientTransactionDetails = PatientProvider.Instance.GetAlltransactionDetails_InPt(registrationInfo.PatientTransaction.TransactionID).ToObservableCollection();
                    }
                }
                else
                {
                    List<PatientTransactionPayment> allPayments = CommonProvider.Payments.GetAllPayments_New(registrationInfo.PatientTransaction.TransactionID, connection, tran);
                    if (allPayments != null)
                    {
                        registrationInfo.PatientTransaction.PatientTransactionPayments = allPayments.ToObservableCollection();
                    }
                    //if (getFullInfo)
                    {
                        registrationInfo.PatientTransaction.PatientTransactionDetails = PatientProvider.Instance.GetAlltransactionDetails(registrationInfo.PatientTransaction.TransactionID).ToObservableCollection();
                    }
                }

            }

            //tam ung neu co
            var allAdvances = CommonProvider.Payments.PatientCashAdvance_GetAll(registrationInfo.PtRegistrationID, (long)registrationInfo.V_RegistrationType);
            if (allAdvances != null)
            {
                registrationInfo.PatientCashAdvances = allAdvances.ToObservableCollection();
            }

            if (//registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU
                //|| 
                registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
            {
                //Lay danh sach thuoc, y dung cu, hoa chat.
                List<OutwardDrugClinicDeptInvoice> allInPatientInvoices = PatientProvider.Instance.GetAllInPatientInvoices(registrationInfo.PtRegistrationID, connection, tran);
                if (allInPatientInvoices != null)
                {
                    registrationInfo.InPatientInvoices = allInPatientInvoices.ToObservableCollection();
                }
            }

            //Tinh tong so tien:
            var allItems = new List<IInvoiceItem>();

            //Add danh sach service.
            if (registrationInfo.PatientRegistrationDetails != null)
            {
                allItems.AddRange(registrationInfo.PatientRegistrationDetails.Where(item => item.RecordState != RecordState.DELETED));
            }

            if (registrationInfo.PCLRequests != null)
            {
                foreach (var pcldetails in registrationInfo.PCLRequests)
                {
                    if (pcldetails.PatientPCLRequestIndicators != null)
                    {
                        allItems.AddRange(pcldetails.PatientPCLRequestIndicators.Where(item => item.RecordState != RecordState.DELETED));
                    }
                }
            }

            if (registrationInfo.DrugInvoices != null)
            {
                foreach (var invoice in registrationInfo.DrugInvoices)
                {
                    if (invoice.OutwardDrugs != null)
                    {
                        //allItems.AddRange(invoice.OutwardDrugs.Where(item => { return item.RecordState != RecordState.DELETED; }));
                        allItems.AddRange(invoice.OutwardDrugs);
                    }
                    if (invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                    {
                        //Phieu tra.
                        foreach (var item in invoice.OutwardDrugs)
                        {
                            item.TotalInvoicePrice = -Math.Abs(item.TotalInvoicePrice);
                            item.TotalPriceDifference = -Math.Abs(item.TotalPriceDifference);
                            item.TotalHIPayment = -Math.Abs(item.TotalHIPayment);
                            item.TotalCoPayment = -Math.Abs(item.TotalCoPayment);
                            item.TotalPatientPayment = -Math.Abs(item.TotalPatientPayment);

                            item.InvoicePrice = -Math.Abs(item.InvoicePrice);
                            if (item.HIAllowedPrice.HasValue)
                            {
                                item.HIAllowedPrice = -Math.Abs(item.HIAllowedPrice.Value);
                            }
                        }
                    }
                    if (invoice.ReturnedInvoices == null || invoice.ReturnedInvoices.Count <= 0) continue;
                    foreach (var returnedInv in invoice.ReturnedInvoices)
                    {
                        if (returnedInv.OutwardDrugs == null || returnedInv.OutwardDrugs.Count <= 0) continue;
                        foreach (var item in returnedInv.OutwardDrugs)
                        {
                            item.TotalInvoicePrice = -Math.Abs(item.TotalInvoicePrice);
                            item.TotalPriceDifference = -Math.Abs(item.TotalPriceDifference);
                            item.TotalHIPayment = -Math.Abs(item.TotalHIPayment);
                            item.TotalCoPayment = -Math.Abs(item.TotalCoPayment);
                            item.TotalPatientPayment = -Math.Abs(item.TotalPatientPayment);

                            item.InvoicePrice = -Math.Abs(item.InvoicePrice);
                            if (item.HIAllowedPrice.HasValue)
                            {
                                item.HIAllowedPrice = -Math.Abs(item.HIAllowedPrice.Value);
                            }
                        }
                        allItems.AddRange(returnedInv.OutwardDrugs);
                    }
                }
            }

            //Tinh thuoc, y cu, hoa chat cho benh nhan noi tru.
            if (registrationInfo.InPatientInvoices != null)
            {
                foreach (var invoice in registrationInfo.InPatientInvoices)
                {
                    if (invoice.OutwardDrugClinicDepts != null)
                    {
                        allItems.AddRange(invoice.OutwardDrugClinicDepts.Where(item => item.RecordState != RecordState.DELETED));
                    }
                }
            }

            ////lay nhung dong duoc bo vao de can bang Transaction o day ne!
            //if (registrationInfo.PatientTransaction != null && registrationInfo.PatientTransaction.PatientTransactionDetails != null)
            //{
            //    var LstTranDetails = registrationInfo.PatientTransaction.PatientTransactionDetails.Where(x => x.Amount == 0 && x.TranRefID > 0 && (x.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NGOAITRU || x.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NOITRU_KHOCUAKHOA || x.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NOITRU_KHODUOC));
            //    if (LstTranDetails != null)
            //    {
            //        foreach (var item in LstTranDetails)
            //        {
            //            OutwardDrug AddItem = new OutwardDrug();//dung cho cho cac tran can bang luon
            //            AddItem.TotalCoPayment = item.AmountCoPay.GetValueOrDefault(0);
            //            AddItem.TotalHIPayment = item.HealthInsuranceRebate.GetValueOrDefault(0);
            //            AddItem.TotalInvoicePrice = item.Amount;
            //            AddItem.TotalPatientPayment = item.PatientPayment;
            //            AddItem.TotalPriceDifference = item.PriceDifference.GetValueOrDefault(0);
            //            allItems.Add(AddItem);
            //        }

            //    }

            //}
            registrationInfo.PayableSum = CalcPayableSum(allItems);
            if (registrationInfo.PatientTransaction != null && registrationInfo.PatientTransaction.PatientTransactionPayments != null)//&& registrationInfo.PatientTransaction.PatientPayments != null
            {
                // registrationInfo.PayableSum.TotalPatientPaid = registrationInfo.PatientTransaction.PatientPayments.Sum(payment => payment.PayAmount * payment.CreditOrDebit);
                registrationInfo.PayableSum.TotalPatientPaid = registrationInfo.PatientTransaction.PatientTransactionPayments.Where(x => !x.IsDeleted.GetValueOrDefault()).Sum(payment => payment.PayAmount * payment.CreditOrDebit);
                registrationInfo.PayableSum.TotalPaymentForTransaction = PatientProvider.Instance.GetTotalPatientPayForTransaction(registrationInfo.PatientTransaction.TransactionID, connection, tran);
            }
            else
            {
                registrationInfo.PayableSum.TotalPatientPaid = 0;
                registrationInfo.PayableSum.TotalPaymentForTransaction = 0;
            }
            registrationInfo.PayableSum.TotalPatientRemainingOwed = registrationInfo.PayableSum.TotalPatientPayment - registrationInfo.PayableSum.TotalPatientPaid;

            return registrationInfo;
        }

        protected static PatientRegistration GetRegistration_InPt(long regID, int FindPatient, DbConnection connection, DbTransaction tran, bool getFullInfo = false)
        {
            PatientRegistration registrationInfo = PatientProvider.Instance.GetRegistration(regID, FindPatient, connection, tran);
            if (registrationInfo == null)
            {
                //Bao loi khong co dang ky nay.
                throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
            }

            if (registrationInfo.PatientID.HasValue && registrationInfo.PatientID.Value > 0)
            {
                registrationInfo.Patient = PatientProvider.Instance.GetPatientByID_Simple(registrationInfo.PatientID.Value, connection, tran);
            }

            //Neu la dang ky noi tru thi chi lay danh sach billing invoice thoi.
            if (//registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU || 
                registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
            {
                List<InPatientBillingInvoice> billingInvoices = PatientProvider.Instance.GetAllInPatientBillingInvoices(registrationInfo.PtRegistrationID, null);
                if (billingInvoices != null)
                {
                    registrationInfo.InPatientBillingInvoices = billingInvoices.ToObservableCollection();
                }
                //Lay thong tin nhap vien:
                registrationInfo.AdmissionInfo = PatientProvider.Instance.GetInPatientAdmissionByRegistrationID(registrationInfo.PtRegistrationID, connection, tran);
                if (registrationInfo.AdmissionInfo != null)
                {
                    registrationInfo.AdmissionInfo.PatientRegistration = registrationInfo;
                }
                //Lay thong tin giuong:
                List<BedPatientAllocs> allBedPatientAlloc = BedAllocations.Instance.GetAllBedAllocationsByRegistrationID(registrationInfo.PtRegistrationID, null, connection, tran);
                if (allBedPatientAlloc != null)
                {
                    registrationInfo.BedAllocations = allBedPatientAlloc.ToObservableCollection();
                }
            }

            List<PatientRegistrationDetail> regDetails = PatientProvider.Instance.GetAllRegistrationDetails(regID, FindPatient, connection, tran);

            if (regDetails != null)
            {
                //registrationInfo.PatientRegistrationDetails = regDetails.ToObservableCollection<PatientRegistrationDetail>();
                registrationInfo.PatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                foreach (var item in regDetails)
                {
                    //if (!GetFullInfo)
                    //{
                    //    if (item.TotalHIPayment == 0 && item.TotalCoPayment == 0)
                    //    {
                    //        item.HIAllowedPrice = 0;
                    //        item.PriceDifference = 0;
                    //        item.TotalPriceDifference = 0;
                    //    } 
                    //}
                    registrationInfo.PatientRegistrationDetails.Add(item);
                }
            }

            //Can lam sang
            var pclRequestList = PatientProvider.Instance.GetPCLRequestListByRegistrationID(regID, connection, tran);
            if (pclRequestList != null)
            {
                registrationInfo.PCLRequests = pclRequestList.ToObservableCollection();
            }

            //neu co tinh thuoc doc lap = 1 va la benh nhan ngoai tru thi ko thuc hien nhung ham ben duoi nay
            //else thi van de binh thuong

            List<OutwardDrugInvoice> invoiceList = null;
            //ny: vi noi tru cung ban thuoc theo toa cho benh nhan khi xuat vien nen mo cai nay ra

            // if(registrationInfo.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
            {
                invoiceList = PatientProvider.Instance.GetDrugInvoiceListByRegistrationID(regID, connection, tran);
            }

            if (invoiceList != null)
            {
                registrationInfo.DrugInvoices = invoiceList.ToObservableCollection();

            }
            if (registrationInfo.PatientTransaction != null)
            {
                List<PatientTransactionPayment> allPayments = null;
                if (//registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU
                    //|| 
                    registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    allPayments = CommonProvider.Payments.GetAllPayments_InPt(registrationInfo.PatientTransaction.TransactionID, connection, tran);
                }
                else
                {
                    allPayments = CommonProvider.Payments.GetAllPayments_New(registrationInfo.PatientTransaction.TransactionID, connection, tran);
                }
                if (allPayments != null)
                {
                    registrationInfo.PatientTransaction.PatientTransactionPayments = allPayments.ToObservableCollection();
                }
                //if (getFullInfo)
                {
                    registrationInfo.PatientTransaction.PatientTransactionDetails = PatientProvider.Instance.GetAlltransactionDetails_InPt(registrationInfo.PatientTransaction.TransactionID).ToObservableCollection();
                }
            }

            //tam ung neu co
            var allAdvances = CommonProvider.Payments.PatientCashAdvance_GetAll(registrationInfo.PtRegistrationID, (long)registrationInfo.V_RegistrationType);
            if (allAdvances != null)
            {
                registrationInfo.PatientCashAdvances = allAdvances.ToObservableCollection();
            }

            if (//registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU
                //|| 
                registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
            {
                //Lay danh sach thuoc, y dung cu, hoa chat.
                List<OutwardDrugClinicDeptInvoice> allInPatientInvoices = PatientProvider.Instance.GetAllInPatientInvoices(registrationInfo.PtRegistrationID, connection, tran);
                if (allInPatientInvoices != null)
                {
                    registrationInfo.InPatientInvoices = allInPatientInvoices.ToObservableCollection();
                }
            }

            //Tinh tong so tien:
            var allItems = new List<IInvoiceItem>();

            //Add danh sach service.
            if (registrationInfo.PatientRegistrationDetails != null)
            {
                allItems.AddRange(registrationInfo.PatientRegistrationDetails.Where(item => item.RecordState != RecordState.DELETED));
            }

            if (registrationInfo.PCLRequests != null)
            {
                foreach (var pcldetails in registrationInfo.PCLRequests)
                {
                    if (pcldetails.PatientPCLRequestIndicators != null)
                    {
                        allItems.AddRange(pcldetails.PatientPCLRequestIndicators.Where(item => item.RecordState != RecordState.DELETED));
                    }
                }
            }

            if (registrationInfo.DrugInvoices != null)
            {
                foreach (var invoice in registrationInfo.DrugInvoices)
                {
                    if (invoice.OutwardDrugs != null)
                    {
                        //allItems.AddRange(invoice.OutwardDrugs.Where(item => { return item.RecordState != RecordState.DELETED; }));
                        allItems.AddRange(invoice.OutwardDrugs);
                    }
                    if (invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                    {
                        //Phieu tra.
                        foreach (var item in invoice.OutwardDrugs)
                        {
                            item.TotalInvoicePrice = -Math.Abs(item.TotalInvoicePrice);
                            item.TotalPriceDifference = -Math.Abs(item.TotalPriceDifference);
                            item.TotalHIPayment = -Math.Abs(item.TotalHIPayment);
                            item.TotalCoPayment = -Math.Abs(item.TotalCoPayment);
                            item.TotalPatientPayment = -Math.Abs(item.TotalPatientPayment);

                            item.InvoicePrice = -Math.Abs(item.InvoicePrice);
                            if (item.HIAllowedPrice.HasValue)
                            {
                                item.HIAllowedPrice = -Math.Abs(item.HIAllowedPrice.Value);
                            }
                        }
                    }
                    if (invoice.ReturnedInvoices == null || invoice.ReturnedInvoices.Count <= 0) continue;
                    foreach (var returnedInv in invoice.ReturnedInvoices)
                    {
                        if (returnedInv.OutwardDrugs == null || returnedInv.OutwardDrugs.Count <= 0) continue;
                        foreach (var item in returnedInv.OutwardDrugs)
                        {
                            item.TotalInvoicePrice = -Math.Abs(item.TotalInvoicePrice);
                            item.TotalPriceDifference = -Math.Abs(item.TotalPriceDifference);
                            item.TotalHIPayment = -Math.Abs(item.TotalHIPayment);
                            item.TotalCoPayment = -Math.Abs(item.TotalCoPayment);
                            item.TotalPatientPayment = -Math.Abs(item.TotalPatientPayment);

                            item.InvoicePrice = -Math.Abs(item.InvoicePrice);
                            if (item.HIAllowedPrice.HasValue)
                            {
                                item.HIAllowedPrice = -Math.Abs(item.HIAllowedPrice.Value);
                            }
                        }
                        allItems.AddRange(returnedInv.OutwardDrugs);
                    }
                }
            }

            //Tinh thuoc, y cu, hoa chat cho benh nhan noi tru.
            if (registrationInfo.InPatientInvoices != null)
            {
                foreach (var invoice in registrationInfo.InPatientInvoices)
                {
                    if (invoice.OutwardDrugClinicDepts != null)
                    {
                        allItems.AddRange(invoice.OutwardDrugClinicDepts.Where(item => item.RecordState != RecordState.DELETED));
                    }
                }
            }

            //lay nhung dong duoc bo vao de can bang Transaction o day ne!
            if (registrationInfo.PatientTransaction != null && registrationInfo.PatientTransaction.PatientTransactionDetails != null)
            {
                var LstTranDetails = registrationInfo.PatientTransaction.PatientTransactionDetails.Where(x => x.Amount == 0 && x.TranRefID > 0 && (x.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NGOAITRU || x.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NOITRU_KHOCUAKHOA || x.V_TranRefType == AllLookupValues.V_TranRefType.DRUG_NOITRU_KHODUOC));
                if (LstTranDetails != null)
                {
                    foreach (var item in LstTranDetails)
                    {
                        OutwardDrug AddItem = new OutwardDrug();//dung cho cho cac tran can bang luon
                        AddItem.TotalCoPayment = item.AmountCoPay.GetValueOrDefault(0);
                        AddItem.TotalHIPayment = item.HealthInsuranceRebate.GetValueOrDefault(0);
                        AddItem.TotalInvoicePrice = item.Amount;
                        AddItem.TotalPatientPayment = item.PatientPayment;
                        AddItem.TotalPriceDifference = item.PriceDifference.GetValueOrDefault(0);
                        allItems.Add(AddItem);
                    }

                }

            }
            registrationInfo.PayableSum = CalcPayableSum(allItems);
            if (registrationInfo.PatientTransaction != null && registrationInfo.PatientTransaction.PatientTransactionPayments != null)//&& registrationInfo.PatientTransaction.PatientPayments != null
            {
                // registrationInfo.PayableSum.TotalPatientPaid = registrationInfo.PatientTransaction.PatientPayments.Sum(payment => payment.PayAmount * payment.CreditOrDebit);
                registrationInfo.PayableSum.TotalPatientPaid = registrationInfo.PatientTransaction.PatientTransactionPayments.Sum(payment => payment.PayAmount * payment.CreditOrDebit);
                if (//registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU
                    //|| 
                    registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    registrationInfo.PayableSum.TotalPaymentForTransaction = PatientProvider.Instance.GetTotalPatientPayForTransaction_InPt(registrationInfo.PatientTransaction.TransactionID, connection, tran);
                }
                else
                {
                    registrationInfo.PayableSum.TotalPaymentForTransaction = PatientProvider.Instance.GetTotalPatientPayForTransaction(registrationInfo.PatientTransaction.TransactionID, connection, tran);
                }
            }
            else
            {
                registrationInfo.PayableSum.TotalPatientPaid = 0;
                registrationInfo.PayableSum.TotalPaymentForTransaction = 0;
            }
            registrationInfo.PayableSum.TotalPatientRemainingOwed = registrationInfo.PayableSum.TotalPatientPayment - registrationInfo.PayableSum.TotalPatientPaid;

            return registrationInfo;
        }

        #region UTILITIES
        /// <summary>
        /// Lấy danh sách những PCLRequest + PCLRequestDetails thực sự bị thay đổi.
        /// </summary>
        /// <param name="pclRequestList"></param>
        /// <returns></returns>
        protected List<PatientPCLRequest> GetModifiedPclItems(List<PatientPCLRequest> pclRequestList)
        {
            var modifiedPcl = new List<PatientPCLRequest>();
            foreach (var item in pclRequestList)
            {
                if (item.RecordState == RecordState.DELETED)
                {
                    foreach (var detail in item.PatientPCLRequestIndicators)
                    {
                        detail.RecordState = RecordState.DELETED;
                    }
                    modifiedPcl.Add(item);
                }
                else if (item.RecordState == RecordState.MODIFIED)
                {
                    item.PatientPCLRequestIndicators = item.PatientPCLRequestIndicators.Where(o => o.RecordState == RecordState.DELETED || o.RecordState == RecordState.MODIFIED).ToObservableCollection();
                    modifiedPcl.Add(item);
                }
            }
            return modifiedPcl;
        }

        #endregion
        #region PHẦN CÂN BẰNG TRANSACTION

        /// <summary>
        /// Cân bằng Transaction cho đăng ký. Phải đưa thông tin đầy đủ của đăng ký.
        /// Dùng hàm này khi tính tiền.
        /// </summary>
        /// <param name="regInfo"></param>
        /// <param name="balanceDate"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <param name="colPaidRegDetails"></param>
        /// <param name="colPaidPclRequests"></param>
        /// <param name="colPaidDrugInvoice"></param>
        /// <param name="colBillingInvoices"></param>
        protected void BalanceTransaction(long StaffID, out string ListIDOutTranDetails, PatientRegistration regInfo, List<PatientRegistrationDetail> colPaidRegDetails, List<PatientPCLRequest> colPaidPclRequests, List<OutwardDrugInvoice> colPaidDrugInvoice, List<InPatientBillingInvoice> colBillingInvoices, DateTime balanceDate, DbConnection conn, DbTransaction tran)
        {
            ListIDOutTranDetails = "";
            if (balanceDate == DateTime.MinValue)
            {
                balanceDate = DateTime.Now;
            }
            if (regInfo.PatientTransaction != null)
            {
                var balancedTranDetails = new List<PatientTransactionDetail>();
                if (colPaidRegDetails != null)
                {
                    balancedTranDetails.AddRange(colPaidRegDetails.Where(item => item.PaidTime != null).Select(item => BalanceTransactionOfService(regInfo.PatientTransaction, item, balanceDate)).Where(tranDetail => tranDetail != null));
                }

                if (colPaidPclRequests != null)
                {
                    balancedTranDetails.AddRange(colPaidPclRequests.Where(item => item.PaidTime != null).Select(item => BalanceTransactionOfPclRequest(regInfo.PatientTransaction, item, balanceDate)).Where(tranDetail => tranDetail != null));
                }

                if (colPaidDrugInvoice != null)
                {
                    balancedTranDetails.AddRange(colPaidDrugInvoice.Where(item => item.PaidTime != null).Select(item => BalanceTransactionOfDrugInvoice(regInfo.PatientTransaction, item, balanceDate)).Where(tranDetail => tranDetail != null));
                }

                if (colBillingInvoices != null)
                {
                    balancedTranDetails.AddRange(colBillingInvoices.Where(item => item.PaidTime != null).Select(item => BalanceTransactionOfBillingInvoice(regInfo.PatientTransaction, item)).Where(tranDetail => tranDetail != null));
                }

                if (balancedTranDetails.Count > 0)
                {
                    if (regInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                    {
                        PatientProvider.Instance.AddTransactionDetailList_InPt(StaffID, regInfo.PatientTransaction.TransactionID, balancedTranDetails, out ListIDOutTranDetails, conn, tran);
                    }
                    else
                    {
                        PatientProvider.Instance.AddTransactionDetailList(StaffID, regInfo.PatientTransaction.TransactionID, balancedTranDetails, out ListIDOutTranDetails, conn, tran);
                    }
                }
            }

        }

        /// <summary>
        /// Cân bằng TRANSACTION cho một đăng ký. 
        /// </summary>
        /// <param name="regID"></param>
        /// <param name="balanceDate"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        protected void BalanceTransaction(long StaffID, long regID, int FindPatient, out string ListOutIDTranDetails, DateTime balanceDate, DbConnection conn, DbTransaction tran)
        //=======  protected void BalanceTransaction(long regID, out string ListOutIDTranDetails, DateTime balanceDate, DbConnection conn, DbTransaction tran)
        {
            ListOutIDTranDetails = "";
            var regInfo = GetRegistration(regID, FindPatient, conn, tran);
            if (regInfo.PatientTransaction == null)
            {
                return;
            }
            regInfo.PatientTransaction.PatientTransactionDetails =
                PatientProvider.Instance.GetAlltransactionDetails(regInfo.PatientTransaction.TransactionID).
                    ToObservableCollection();

            var balancedTranDetails = new List<PatientTransactionDetail>();
            if (regInfo.PatientRegistrationDetails != null)
            {
                balancedTranDetails.AddRange(regInfo.PatientRegistrationDetails.Where(item => item.PaidTime != null).Select(item => BalanceTransactionOfService(regInfo.PatientTransaction, item, balanceDate)).Where(tranDetail => tranDetail != null));
            }

            if (regInfo.PCLRequests != null)
            {
                balancedTranDetails.AddRange(regInfo.PCLRequests.Where(item => item.PaidTime != null).Select(item => BalanceTransactionOfPclRequest(regInfo.PatientTransaction, item, balanceDate)).Where(tranDetail => tranDetail != null));
            }
            var allOutInvoices = new List<OutwardDrugInvoice>();
            if (regInfo.DrugInvoices != null)
            {
                foreach (var item in regInfo.DrugInvoices)
                {
                    allOutInvoices.Add(item);
                    if (item.ReturnedInvoices != null)
                    {
                        allOutInvoices.AddRange(item.ReturnedInvoices);
                    }
                }
            }
            balancedTranDetails.AddRange(allOutInvoices.Where(item => item.PaidTime != null).Select(item => BalanceTransactionOfDrugInvoice(regInfo.PatientTransaction, item, balanceDate)).Where(tranDetail => tranDetail != null));

            if (balancedTranDetails.Count > 0)
            {
                PatientProvider.Instance.AddTransactionDetailList(StaffID, regInfo.PatientTransaction.TransactionID,
                                                                  balancedTranDetails, out ListOutIDTranDetails, conn, tran);
            }
        }

        /// <summary>
        /// Cân bằng transaction của 1 dịch vụ đăng ký.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="?"></param>
        /// <param name="details"></param>
        /// <param name="balanceDate"></param>
        /// <returns>Trả về null nếu transaction của dịch vụ này đã cân bằng.
        /// Nếu dịch vụ chưa cân bằng => Trả về 1 object PatientTransactionDetail để thêm vào Transaction cho cân bằng</returns>
        protected virtual PatientTransactionDetail BalanceTransactionOfService(PatientTransaction transaction, PatientRegistrationDetail details, DateTime balanceDate)
        {
            if (transaction == null || details == null)
            {
                return null;
            }
            //Nếu chưa trả tiền thì khỏi cân bằng
            if (details.PaidTime == null)
            {
                return null;
            }

            decimal amount = transaction.PatientTransactionDetails.Where(tranItem => tranItem.PtRegDetailID == details.PtRegDetailID).Sum(tranItem => tranItem.Amount);

            //amount = details.TotalInvoicePrice - amount;
            if (details.MarkedAsDeleted ||
                details.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                || details.RecordState == RecordState.DELETED)
            {
                amount = -amount;
            }
            else
            {
                amount = details.TotalInvoicePrice - amount;
            }
            if (amount == 0)
            {
                return null;
            }

            var tranDetail = new PatientTransactionDetail
                                 {
                                     OutwBloodInvoiceID = null,
                                     OutDMedRscrID = null,
                                     StaffID = null,
                                     PtRegDetailID = details.PtRegDetailID,
                                     outiID = null,
                                     TransactionID = transaction.TransactionID
                                 };

            tranDetail.OutwBloodInvoiceID = null;
            tranDetail.TransactionDate = balanceDate;

            tranDetail.Amount = amount;
            tranDetail.PriceDifference = null;
            tranDetail.AmountCoPay = null;
            tranDetail.HealthInsuranceRebate = null;

            tranDetail.Discount = null;
            tranDetail.Qty = 1;
            tranDetail.RefDocID = null;
            tranDetail.ExchangeRate = null;
            tranDetail.TransItemRemarks = String.Empty;
            tranDetail.PCLRequestID = null;

            return tranDetail;
        }

        protected virtual PatientTransactionDetail BalanceTransactionOfPclRequest(PatientTransaction transaction, PatientPCLRequest request, DateTime balanceDate)
        {
            if (transaction == null || request == null)
            {
                return null;
            }

            //Nếu chưa trả tiền thì khỏi cân bằng
            if (request.PaidTime == null)
            {
                return null;
            }

            decimal amount = 0;
            decimal priceDifference = 0;
            decimal amountCoPay = 0;
            decimal healthInsuranceRebate = 0;

            request.CalTotal();

            foreach (var tranItem in transaction.PatientTransactionDetails)
            {
                if (tranItem.PCLRequestID == request.PatientPCLReqID)
                {
                    amount += tranItem.Amount;
                    priceDifference += tranItem.PriceDifference.GetValueOrDefault(0);
                    amountCoPay += tranItem.AmountCoPay.GetValueOrDefault(0);
                    healthInsuranceRebate += tranItem.HealthInsuranceRebate.GetValueOrDefault(0);
                }
            }

            if (request.MarkedAsDeleted ||
                //request.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                request.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CANCEL
               || request.RecordState == RecordState.DELETED)
            {
                amount = -amount;
                priceDifference = -priceDifference;
                amountCoPay = -amountCoPay;
                healthInsuranceRebate = -healthInsuranceRebate;
            }
            else
            {
                amount = request.TotalInvoicePrice - amount;
                priceDifference = request.TotalPriceDifference - priceDifference;
                amountCoPay = request.TotalCoPayment - amountCoPay;
                healthInsuranceRebate = request.TotalHIPayment - healthInsuranceRebate;
            }

            if (amount == 0 && priceDifference == 0 && amountCoPay == 0 && healthInsuranceRebate == 0)
            // && !request.hasNonPriceDetail)
            {
                return null;
            }

            var tranDetail = new PatientTransactionDetail
                                 {
                                     OutwBloodInvoiceID = null,
                                     OutDMedRscrID = null,
                                     StaffID = null,
                                     PtRegDetailID = null,
                                     outiID = null,
                                     TransactionID = transaction.TransactionID
                                 };

            tranDetail.OutwBloodInvoiceID = null;
            tranDetail.TransactionDate = balanceDate;

            tranDetail.Amount = amount;
            tranDetail.PriceDifference = priceDifference;
            tranDetail.AmountCoPay = amountCoPay;
            tranDetail.HealthInsuranceRebate = healthInsuranceRebate;

            tranDetail.Discount = null;
            tranDetail.Qty = 1;
            tranDetail.RefDocID = null;
            tranDetail.ExchangeRate = null;
            tranDetail.TransItemRemarks = String.Empty;
            tranDetail.PCLRequestID = request.PatientPCLReqID;

            return tranDetail;
        }

        protected virtual PatientTransactionDetail BalanceTransactionOfDrugInvoice(PatientTransaction transaction, OutwardDrugInvoice invoice, DateTime balanceDate)
        {
            if (transaction == null || invoice == null)
            {
                return null;
            }
            if (invoice.PaidTime == null)//Nếu chưa trả tiền thì khỏi cân bằng
            {
                return null;
            }

            if (invoice.ReturnID.GetValueOrDefault(0) > 0)//Phieu tra
            {
                if (invoice.OutwardDrugs != null && invoice.OutwardDrugs.Count > 0)
                {
                    foreach (var item in invoice.OutwardDrugs)
                    {
                        item.TotalInvoicePrice = -Math.Abs(item.TotalInvoicePrice);
                        item.TotalPriceDifference = -Math.Abs(item.TotalPriceDifference);
                        item.TotalHIPayment = -Math.Abs(item.TotalHIPayment);
                        item.TotalCoPayment = -Math.Abs(item.TotalCoPayment);
                        item.TotalPatientPayment = -Math.Abs(item.TotalPatientPayment);
                    }
                }
            }

            decimal amount = 0;
            decimal priceDifference = 0;
            decimal amountCoPay = 0;
            decimal healthInsuranceRebate = 0;

            //gia tri phieu xuat cu, da cap nhat BH (tren hay duoi 15%)
            invoice.CalTotal();

            foreach (var tranItem in transaction.PatientTransactionDetails)
            {
                //neu trong tran detail da co phieu nay roi thi lay tat ca gia tri cua no ra
                if (tranItem.outiID == invoice.outiID)
                {
                    amount += tranItem.Amount;
                    priceDifference += tranItem.PriceDifference.GetValueOrDefault(0);
                    amountCoPay += tranItem.AmountCoPay.GetValueOrDefault(0);
                    healthInsuranceRebate += tranItem.HealthInsuranceRebate.GetValueOrDefault(0);
                }

            }

            if (invoice.ReturnID == null
                && invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                amount = -amount;
                priceDifference = -priceDifference;
                amountCoPay = -amountCoPay;
                healthInsuranceRebate = -healthInsuranceRebate;
            }
            else
            {
                amount = invoice.TotalInvoicePrice - amount;
                priceDifference = invoice.TotalPriceDifference - priceDifference;
                amountCoPay = invoice.TotalCoPayment - amountCoPay;
                healthInsuranceRebate = invoice.TotalHIPayment - healthInsuranceRebate;
            }

            if (amount == 0 && priceDifference == 0 && amountCoPay == 0 && healthInsuranceRebate == 0)
            {
                return null;
            }

            var tranDetail = new PatientTransactionDetail
                                 {
                                     OutwBloodInvoiceID = null,
                                     OutDMedRscrID = null,
                                     StaffID = null,
                                     PtRegDetailID = null,
                                     outiID = invoice.outiID,
                                     TransactionID = transaction.TransactionID,
                                     TranRefID = invoice.outiID,
                                     V_TranRefType = AllLookupValues.V_TranRefType.DRUG_NGOAITRU
                                 };

            tranDetail.OutwBloodInvoiceID = null;
            tranDetail.TransactionDate = balanceDate;

            tranDetail.Amount = amount;
            tranDetail.PriceDifference = priceDifference;
            tranDetail.AmountCoPay = amountCoPay;
            tranDetail.HealthInsuranceRebate = healthInsuranceRebate;

            tranDetail.Discount = null;
            tranDetail.Qty = 1;
            tranDetail.RefDocID = null;
            tranDetail.ExchangeRate = null;
            tranDetail.TransItemRemarks = String.Empty;
            tranDetail.PCLRequestID = null;

            return tranDetail;
        }

        protected virtual PatientTransactionDetail BalanceTransactionOfBillingInvoice(PatientTransaction transaction, InPatientBillingInvoice invoice)
        {
            if (transaction == null || invoice == null)
            {
                return null;
            }

            if (invoice.PaidTime == null)
            {
                return null;
            }

            decimal amount = 0;
            decimal priceDifference = 0;
            decimal amountCoPay = 0;
            decimal healthInsuranceRebate = 0;

            foreach (var tranItem in transaction.PatientTransactionDetails)
            {
                if (tranItem.TranRefID.HasValue
                    && tranItem.V_TranRefType == AllLookupValues.V_TranRefType.BILL_NOI_TRU
                    && tranItem.TranRefID.Value == invoice.InPatientBillingInvID)
                {
                    amount += tranItem.Amount;
                    priceDifference += tranItem.PriceDifference.GetValueOrDefault(0);
                    amountCoPay += tranItem.AmountCoPay.GetValueOrDefault(0);
                    healthInsuranceRebate += tranItem.HealthInsuranceRebate.GetValueOrDefault(0);
                }
            }

            amount = invoice.TotalInvoicePrice - amount;
            amountCoPay = invoice.TotalCoPayment - amountCoPay;
            healthInsuranceRebate = invoice.TotalHIPayment - healthInsuranceRebate;
            if (healthInsuranceRebate != 0)
            {
                priceDifference = amount - amountCoPay - healthInsuranceRebate;
            }
            if (amount == 0 && priceDifference == 0 && amountCoPay == 0 && healthInsuranceRebate == 0)
            {
                return null;
            }

            var tranDetail = new PatientTransactionDetail
                                 {
                                     OutwBloodInvoiceID = null,
                                     OutDMedRscrID = null,
                                     StaffID = null,
                                     PtRegDetailID = null,
                                     outiID = null,
                                     TransactionID = transaction.TransactionID
                                 };

            tranDetail.OutwBloodInvoiceID = null;
            tranDetail.TransactionDate = DateTime.Now;

            tranDetail.Amount = amount;
            tranDetail.PriceDifference = priceDifference;
            tranDetail.AmountCoPay = amountCoPay;
            tranDetail.HealthInsuranceRebate = healthInsuranceRebate;

            tranDetail.Discount = null;
            tranDetail.Qty = 1;
            tranDetail.RefDocID = null;
            tranDetail.ExchangeRate = null;
            tranDetail.TransItemRemarks = String.Empty;
            tranDetail.PCLRequestID = null;

            tranDetail.TranRefID = invoice.InPatientBillingInvID;
            tranDetail.V_TranRefType = AllLookupValues.V_TranRefType.BILL_NOI_TRU;

            return tranDetail;
        }
        #endregion

        //public abstract void UpdatePCLRequest(long StaffID, PatientRegistration registrationInfo, PatientPCLRequest request, out List<PatientPCLRequest> listPclSave, DateTime modifiedDate = default(DateTime));

        /// <summary>
        /// Tạo danh sách các y dụng cụ, thuốc.. xuất từ danh sách nhập. Nếu trả về false là không đủ thuốc.
        /// </summary>
        /// <param name="inwardList"></param>
        /// <param name="newItem">Item được thêm vào</param>
        /// <param name="modifiedInwardList">Danh sách các item nhập đã cập nhật lại số lượng</param>
        /// <param name="newOutwardList">Danh sách các item xuất theo lô.</param>
        protected bool CreateOutwardDrugClinicDeptFromInward(List<InwardDrugClinicDept> inwardList, OutwardDrugClinicDept newItem, out List<InwardDrugClinicDept> modifiedInwardList, out List<OutwardDrugClinicDept> newOutwardList)
        {
            modifiedInwardList = new List<InwardDrugClinicDept>();
            newOutwardList = new List<OutwardDrugClinicDept>();

            var completed = false;
            var found = false;//Su dung bien nay de cho biet co tim thay InwardDrugClinicDept chua.
            //Luu y: Danh sach InwardDrugClinicDept da duoc sap xep thu tu
            var tempQty = (int)newItem.Qty;

            foreach (var inw in inwardList)
            {
                if (inw.GenMedProductID == newItem.GenMedProductItem.GenMedProductID)
                {
                    found = true;
                    if (tempQty <= inw.Remaining)
                    {
                        var outwardItem = new OutwardDrugClinicDept
                                              {
                                                  GenMedProductItem = inw.RefGenMedProductDetails,
                                                  Qty = tempQty,
                                                  InID = inw.InID,
                                                  CreatedDate = newItem.CreatedDate
                                              };

                        newOutwardList.Add(outwardItem);
                        inw.Remaining -= outwardItem.Qty;
                        modifiedInwardList.Add(inw);

                        completed = true;
                        break;
                    }
                    else
                    {
                        var outwardItem = new OutwardDrugClinicDept
                                              {
                                                  GenMedProductItem = inw.RefGenMedProductDetails,
                                                  Qty = Convert.ToInt32(inw.Remaining),
                                                  InID = inw.InID,
                                                  CreatedDate = newItem.CreatedDate
                                              };

                        newOutwardList.Add(outwardItem);
                        inw.Remaining -= outwardItem.Qty;
                        tempQty -= (int)outwardItem.Qty;
                        modifiedInwardList.Add(inw);
                    }
                }
                else
                {
                    if (found)
                    {
                        break;
                    }
                }
            }
            return completed;
        }

        public bool ReturnInPatientDrug(PatientRegistration registrationInfo, List<RefGenMedProductSummaryInfo> returnedItems, long? DeptID, long? StaffID = null)
        {
            using (DbConnection conn = PatientProvider.Instance.CreateConnection())
            {
                conn.Open();

                DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                try
                {
                    var ids = returnedItems.Select(returnedItem => returnedItem.MedProductDetails.GenMedProductID).ToList();
                    //Danh sach chua nhung invoice (moi invoice chi chua nhung outwardDrugClinicDept la thuoc duoc tra lai thoi.)
                    var invList = PatientProvider.Instance.GetInPatientInvoicesHasMedProducts(registrationInfo.PtRegistrationID, ids, DeptID);
                    //Danh sach nhung PHIEU TRA
                    var returnInvList = new List<OutwardDrugClinicDeptInvoice>();

                    //TODO: Lay danh sach invoice chua cac thuoc duoc tra lai.
                    foreach (var returnedItem in returnedItems)
                    {
                        List<OutwardDrugClinicDeptInvoice> tempReturnedInvList;
                        if (!GetReturnedInvoices(invList, returnedItem, out tempReturnedInvList))
                        {
                            return false;
                        }

                        foreach (var tempReturnedInv in tempReturnedInvList)
                        {
                            var exists = false;
                            //Kiem tra xem invoice voi id the nay da co trong list chua.
                            //Ny xem lai cho nay ne
                            foreach (var inv in returnInvList)
                            {
                                if (tempReturnedInv == null || (inv.outiID != tempReturnedInv.outiID) || (inv.ReturnID != tempReturnedInv.ReturnID)) continue;
                                inv.OutwardDrugClinicDepts.AddRange(tempReturnedInv.OutwardDrugClinicDepts);
                                exists = true;
                                break;
                            }
                            if (!exists)
                            {
                                returnInvList.Add(tempReturnedInv);
                            }
                        }
                    }

                    //Insert cac item trong returnInvList //PHIEU TRA
                    foreach (var returnInv in returnInvList)
                    {
                        List<long> inwardDrugIDListError;
                        PatientProvider.Instance.AddOutwardDrugClinicInvoice(returnInv, null, conn, tran, out inwardDrugIDListError);
                    }

                    //Cap nhat invList (thay doi so luong item)
                    var modifiedDrugClinicDepts = new List<OutwardDrugClinicDept>();
                    foreach (var inv in invList)
                    {
                        foreach (var drug in inv.OutwardDrugClinicDepts)
                        {
                            drug.Qty = drug.OutQuantity - drug.QtyReturn;//Cho dung voi ben Ny

                            GetItemPrice(drug, registrationInfo, 0.0);
                            GetItemTotalPrice(drug);
                            modifiedDrugClinicDepts.Add(drug);
                        }
                    }
                    if (modifiedDrugClinicDepts.Count > 0)
                    {
                        PatientProvider.Instance.UpdateOutwardDrugClinicDeptList(modifiedDrugClinicDepts, StaffID, conn, tran);
                        //goi ham can bang o day luon ne!
                        //chi can lay ma phieu xuat cu ra thoi

                        XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                           new XElement("Root",
                           from item in invList
                           select new XElement("IDList",
                           new XElement("ID", item.InPatientBillingInvID))));

                        string BillingInvoices = xmlDocument.ToString();
                        PatientProvider.Instance.GetBalanceInPatientBillingInvoice(StaffID.GetValueOrDefault(0), registrationInfo.PtRegistrationID, (long)registrationInfo.V_RegistrationType, BillingInvoices, conn, tran);

                    }
                    tran.Commit();
                    return true;
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        protected bool GetReturnedInvoices(List<OutwardDrugClinicDeptInvoice> invList, RefGenMedProductSummaryInfo returnedItem, out List<OutwardDrugClinicDeptInvoice> returedInvList)
        {
            returedInvList = new List<OutwardDrugClinicDeptInvoice>();

            foreach (var inv in invList)
            {
                foreach (var drug in inv.OutwardDrugClinicDepts)
                {
                    if (drug.GenMedProductID == returnedItem.MedProductDetails.GenMedProductID && drug.OutQuantity - drug.QtyReturn > 0)
                    {
                        var returnedInv = CreateInPatientReturnInvFromDrugInv(inv);
                        returnedInv.OutwardDrugClinicDepts = new ObservableCollection<OutwardDrugClinicDept>();
                        returedInvList.Add(returnedInv);

                        if (drug.OutQuantity - drug.QtyReturn >= returnedItem.QtyReturn)//Tra ok, Khong can qua bill khac
                        {
                            var returnedDrug = CreateInPatientReturnDrugFromDrug(drug, returnedItem.QtyReturn);
                            returnedInv.OutwardDrugClinicDepts.Add(returnedDrug);

                            drug.QtyReturn += returnedItem.QtyReturn;
                            returnedItem.QtyReturn = 0;

                            return true;
                        }
                        else//Phai tiep tuc tra qua bill khac
                        {
                            var returnedDrug = CreateInPatientReturnDrugFromDrug(drug, drug.OutQuantity - drug.QtyReturn);
                            returnedInv.OutwardDrugClinicDepts.Add(returnedDrug);

                            returnedItem.QtyReturn -= drug.OutQuantity - drug.QtyReturn;
                            drug.QtyReturn = drug.OutQuantity;
                        }
                    }
                }
            }

            if (returnedItem.QtyReturn > 0)//So luong tra nhieu hon so luong xuat.
            {
                return false;
            }
            return true;
        }
        private OutwardDrugClinicDeptInvoice CreateInPatientReturnInvFromDrugInv(OutwardDrugClinicDeptInvoice inv)
        {
            var returnedInv = new OutwardDrugClinicDeptInvoice
                                  {
                                      StoreID = inv.StoreID,
                                      StaffID = inv.StaffID,
                                      PtRegistrationID = inv.PtRegistrationID,
                                      MedProductType = inv.MedProductType,
                                      ReturnID = inv.outiID
                                  };

            return returnedInv;
        }

        private OutwardDrugClinicDept CreateInPatientReturnDrugFromDrug(OutwardDrugClinicDept drug, decimal qtyReturn)
        {
            var returnedDrug = new OutwardDrugClinicDept
                                   {
                                       GenMedProductItem = drug.GenMedProductItem,
                                       GenMedProductID = drug.GenMedProductID,
                                       InID = drug.InID,
                                       Qty = qtyReturn,
                                       HIAllowedPrice = drug.HIAllowedPrice,
                                       HIBenefit = drug.HIBenefit,
                                       InvoicePrice = drug.InvoicePrice,
                                       PriceDifference = drug.PriceDifference,
                                       PatientCoPayment = drug.PatientCoPayment
                                   };



            returnedDrug.HIPayment = returnedDrug.HIAllowedPrice.GetValueOrDefault(0) * (decimal)returnedDrug.HIBenefit.GetValueOrDefault(0);

            GetItemTotalPrice(returnedDrug);

            return returnedDrug;
        }

        public List<PatientRegistrationDetail> CreatePatientRegDetailsFromBedPatientAllocation(PatientRegistration regInfo, BedPatientAllocs allocs, DateTime? lastestBillDate)
        {
            if (allocs == null)
            {
                return null;
            }
            if (!allocs.CheckInDate.HasValue)
            {
                throw new Exception(eHCMSResources.Z1835_G1_InvalidInfo);
            }
            if (lastestBillDate.HasValue)
            {
                if (lastestBillDate.Value < allocs.CheckInDate)
                {
                    throw new Exception(eHCMSResources.Z1835_G1_InvalidInfo);
                }
                if (allocs.CheckOutDate.HasValue && lastestBillDate.Value > allocs.CheckOutDate)
                {
                    return null;
                }
            }

            var retVal = new List<PatientRegistrationDetail>();

            DateTime billToDate = allocs.CheckOutDate.HasValue ? allocs.CheckOutDate.Value : DateTime.Now;
            DateTime billFromDate = lastestBillDate.HasValue ? lastestBillDate.Value : allocs.CheckInDate.Value;

            int numDays = 0;
            if (billToDate < billFromDate)
            {
                //luc nao cung load len 1 dong de ngta tinh tien giuong
                numDays = 0;
                //return null;
            }
            else
            {
                //Tinh den cuoi ngay hom truoc.
                billToDate = billToDate.AddDays(-1).Date.AddSeconds(86399);
                if (billToDate < billFromDate)
                {
                    billToDate = billToDate.AddDays(1);//Truong hop moi nhan giuong trong ngay. Phai tinh den cuoi ngay.
                }

                numDays = billToDate.Subtract(billFromDate).Days;
                if (numDays < 0)
                {
                    numDays = 0;
                }
            }
            var details = new PatientRegistrationDetail
                              {
                                  CreatedDate = DateTime.Now,
                                  MedProductType = AllLookupValues.MedProductType.TIEN_GIUONG,
                                  ExamRegStatus = AllLookupValues.ExamRegStatus.HOAN_TAT,
                                  RefMedicalServiceItem = allocs.VBedAllocation.VRefMedicalServiceItem,
                                  Qty = numDays
                              };
            retVal.Add(details);

            var bedRegDetail = new BedPatientRegDetail
                                   {
                                       BedPatientID = allocs.BedPatientID,
                                       RecCreatedDate = details.CreatedDate,
                                       BillFromDate = billFromDate,
                                       BillToDate = billToDate
                                   };

            details.BedPatientRegDetail = bedRegDetail;

            CalcInvoiceItems(retVal, regInfo);
            return retVal;
        }

        public InPatientBillingInvoice LoadInPatientRegItemsIntoBill(PatientRegistration regInfo, long? DeptID)
        {
            var inv = new InPatientBillingInvoice
                          {
                              InvDate = DateTime.Now,
                              V_BillingInvType = AllLookupValues.V_BillingInvType.TINH_TIEN_NOI_TRU,
                              V_InPatientBillingInvStatus = AllLookupValues.V_InPatientBillingInvStatus.NEW
                          };

            using (var conn = PatientProvider.Instance.CreateConnection())
            {
                var tempRegDetailList = PatientProvider.Instance.GetAllInPatientRegistrationDetails_NoBill(regInfo.PtRegistrationID, DeptID, conn, null);
                if (tempRegDetailList != null)
                {
                    inv.RegistrationDetails = tempRegDetailList.ToObservableCollection();
                }
                var tempRequestList = PatientProvider.Instance.spGetInPatientPCLRequest_NoBill(regInfo.PtRegistrationID, DeptID, conn, null);
                if (tempRequestList != null)
                {
                    inv.PclRequests = tempRequestList.ToObservableCollection();
                }
                var tempInvoiceList = PatientProvider.Instance.GetAllInPatientInvoices_NoBill(regInfo.PtRegistrationID, DeptID, conn, null);
                if (tempInvoiceList != null)
                {
                    inv.OutwardDrugClinicDeptInvoices = tempInvoiceList.ToObservableCollection();
                }

                var allBedPatientAlloc = BedAllocations.Instance.GetAllBedAllocationsByRegistrationID(regInfo.PtRegistrationID, DeptID, conn, null);
                if (allBedPatientAlloc != null)
                {
                    foreach (var item in allBedPatientAlloc)
                    {
                        //if (item.CheckOutDate == null && item.IsActive)
                        if (item.IsDeleted) continue;
                        var tempDate = PatientProvider.Instance.BedPatientAlloc_GetLatestBillToDate(item.BedPatientID, conn, null);
                        var regDetailList = CreatePatientRegDetailsFromBedPatientAllocation(regInfo, item, tempDate);
                        if (regDetailList != null)
                        {
                            if (inv.RegistrationDetails == null)
                            {
                                inv.RegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                            }
                            inv.RegistrationDetails.AddRange(regDetailList);
                        }
                    }
                }

            }

            return inv;
        }



        private XDocument ConvertModifiedTransactionToXml(PatientTransaction tran, int bNgoaiTru)
        {
            if (tran == null)
            {
                return null;
            }
            var xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                                                  new XElement("PatientTransaction",
                                                               new XElement("TransactionID", tran.TransactionID),
                                                               new XElement("PtRegistrationID", tran.PtRegistrationID),
                                                               new XElement("TransactionTypeID", tran.TransactionTypeID),
                                                               new XElement("BDID", tran.BDID),
                                                               new XElement("TransactionBeginDate", tran.TransactionBeginDate.ToString("yyyy-MM-dd HH:mm:ss.fff")),
                                                               new XElement("TransactionEndDate", tran.TransactionEndDate.HasValue ? tran.TransactionEndDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null),
                                                               new XElement("TransactionRemarks", tran.TransactionRemarks),
                                                               new XElement("V_TranHIPayment", tran.V_TranHIPayment),
                                                               new XElement("V_TranPatientPayment", tran.V_TranPatientPayment),
                                                               new XElement("IsClosed", tran.IsClosed),
                                                               new XElement("IsAdjusted", tran.IsAdjusted),
                                                               new XElement("RecordState", tran.RecordState)
                                                      ));

            var tranElem = xmlDocument.Element("PatientTransaction");
            if (tran.PatientTransactionDetails != null)
            {
                IList<PatientTransactionDetail> newItems = tran.PatientTransactionDetails.Where(item => item.TransItemID <= 0).ToList();
                if (newItems.Count > 0)
                {
                    Debug.Assert(tranElem != null, "tranElem != null");
                    tranElem.Add(new XElement("PatientTransactionDetails",
                    from item in newItems
                    select new XElement("PatientTransactionDetail",
                          new XElement("TransItemID", item.TransItemID),
                          new XElement("OutwBloodInvoiceID", item.OutwBloodInvoiceID),
                          new XElement("OutDMedRscrID", item.OutDMedRscrID),
                          new XElement("StaffID", item.StaffID),
                          new XElement("PtRegDetailID", item.PtRegDetailID),
                          new XElement("outiID", item.outiID),
                          new XElement("TransactionDate", item.TransactionDate.HasValue ? item.TransactionDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null),
                          new XElement("Amount", item.Amount),
                          new XElement("PriceDifference", item.PriceDifference),
                          new XElement("AmountCoPay", item.AmountCoPay),
                          new XElement("HealthInsuranceRebate", item.HealthInsuranceRebate),
                          new XElement("Discount", item.Discount),
                          new XElement("Qty", item.Qty),
                          new XElement("RefDocID", item.RefDocID),
                          new XElement("ExchangeRate", item.ExchangeRate),
                          new XElement("TransItemRemarks", item.TransItemRemarks),
                          new XElement("PCLRequestID", item.PCLRequestID),
                          new XElement("TranRefID", item.TranRefID),
                          new XElement("V_TranRefType", (long)item.V_TranRefType)
                        )));
                }
            }
            if (tran.PatientTransactionPayments != null)
            {
                // IList<PatientPayment> newPayments = tran.PatientPayments.Where(item => item.PtPmtID <= 0).ToList();
                IList<PatientTransactionPayment> newPayments = tran.PatientTransactionPayments.Where(item => item.PtTranPaymtID <= 0).ToList();
                if (newPayments.Count > 0)
                {
                    Debug.Assert(tranElem != null, "tranElem != null");
                    tranElem.Add(new XElement("PatientPayments",
                    from item in newPayments
                    select new XElement("PatientPayment",
                        new XElement("PtTranPaymtID", item.PtTranPaymtID),
                       new XElement("InvoiceID", item.InvoiceID),
                          new XElement("IntRcptTypeID", item.IntRcptTypeID),
                          new XElement("PaymentDate", item.PaymentDate.HasValue ? item.PaymentDate.Value.ToString("yyyy-MM-dd HH:mm:ss.fff") : null),
                          new XElement("PayAmount", item.PayAmount),
                          new XElement("StaffID", item.StaffID),
                        //khong goi lay so o day nua
                        //new XElement("ReceiptNumber", new ServiceSequenceNumberProvider().GetReceiptNumber_NgoaiTru(bNgoaiTru)),
                          new XElement("ManualReceiptNumber", item.ManualReceiptNumber),
                          new XElement("TranPaymtNote", item.TranPaymtNote),
                          new XElement("V_Currency", item.V_Currency ?? (item.Currency != null ? item.Currency.LookupID : 0)),
                          new XElement("V_PaymentType", item.V_PaymentType ?? (item.PaymentType != null ? item.PaymentType.LookupID : 0)),
                          new XElement("V_PaymentMode", item.V_PaymentMode ?? (item.PaymentMode != null ? item.PaymentMode.LookupID : 0)),
                          new XElement("CreditOrDebit", item.CreditOrDebit),
                          new XElement("PtPmtAccID", item.PtPmtAccID),
                          new XElement("V_TradingPlaces", item.V_TradingPlaces)
                        )));
                }
            }
            return xmlDocument;
        }


        /// <summary>
        /// Lưu lại tất cả những thay đổi trên đăng ký hiện tại
        /// </summary>
        public bool SaveRegistrationForOutPatient(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, DateTime modifiedDate
            , out long newRegistrationID
            , out List<long> newRegistrationDetailIdList
            , out List<long> newPclRequestIdList
           , out List<long> newPaymentIDList
            , out List<long> newOutwardDrugIDList
            , out List<long> billingInvoiceIDs
            , out string PaymentIDListNy)
        {
            // Txd 05/07/2012
            newRegistrationDetailIdList = null;
            newPclRequestIdList = null;

            newPaymentIDList = null;
            newRegistrationID = -1;
            newOutwardDrugIDList = null;
            billingInvoiceIDs = null;
            PaymentIDListNy = null;
            //Tính lại đăng ký
            if (Apply15HIPercent == null)
            {
                Apply15HIPercent = Globals.AxServerSettings.HealthInsurances.Apply15HIPercent;
            }

            if (CurrentRegistration.PtRegistrationID <= 0)//Đăng ký mới
            {
                //TODO:
                //Add dang ky
                AddNewRegistration(out newRegistrationID, out newRegistrationDetailIdList, out newPclRequestIdList);
                return true;
            }
            BalanceTransaction(modifiedDate);
            newRegistrationID = CurrentRegistration.PtRegistrationID;
            //Đăng ký cũ
            return UpdateRegistration(StaffID,CollectorDeptLocID, out newRegistrationDetailIdList, out newPclRequestIdList, out newPaymentIDList, out newOutwardDrugIDList, out billingInvoiceIDs, out  PaymentIDListNy);
        }


        // Txd 06/07/12: Added for In Patient Only to fix previous Issue with making PCL request for In Patient from Consultation module. 
        public bool SaveRegistrationForInPatient(int? Apply15HIPercent, DateTime modifiedDate, out long newRegistrationID, out List<long> newRegistrationDetailIdList
                                    , out List<long> newPclRequestIdList
            , out List<long> newPaymentIDList
            , out List<long> newOutwardDrugIDList
            , out List<long> billingInvoiceIDs)
        {
            // Txd 05/07/2012
            newRegistrationDetailIdList = null;
            newPclRequestIdList = null;

            newPaymentIDList = null;
            newOutwardDrugIDList = null;
            billingInvoiceIDs = null;
            //Tính lại đăng ký
            if (Apply15HIPercent == null)
            {
                Apply15HIPercent = Globals.AxServerSettings.HealthInsurances.Apply15HIPercent;
            }

            if (CurrentRegistration.PtRegistrationID <= 0)//Đăng ký mới
            {
                //TODO:
                //Add dang ky
                AddNewRegistration(out newRegistrationID, out newRegistrationDetailIdList, out newPclRequestIdList);
                return true;
            }
            BalanceTransaction(modifiedDate);
            newRegistrationID = CurrentRegistration.PtRegistrationID;
            return UpdateInPatientRegistration(out billingInvoiceIDs, out newPaymentIDList);

        }

        public void GetModifiedItems(long StaffID,
            PatientRegistration registrationInfo
                                    , out List<PatientRegistrationDetail> newRegistrationDetailList
                                    , out List<PatientRegistrationDetail> modifiedRegistrationDetailList
                                    , out List<PatientRegistrationDetail> deletedRegistrationDetailList
                                    , out List<PatientPCLRequest> newPclRequestList
                                    , out List<PatientPCLRequest> modifiedPclRequestList
                                    , out List<PatientPCLRequest> deletedPclRequestList
                                    , out List<OutwardDrugInvoice> newInvoiceList
                                    , out List<OutwardDrugInvoice> modifiedInvoiceList
                                    , out List<OutwardDrugInvoice> deletedInvoiceList)
        {
            newRegistrationDetailList = new List<PatientRegistrationDetail>();
            modifiedRegistrationDetailList = new List<PatientRegistrationDetail>();
            deletedRegistrationDetailList = new List<PatientRegistrationDetail>();

            newPclRequestList = new List<PatientPCLRequest>();
            modifiedPclRequestList = new List<PatientPCLRequest>();
            deletedPclRequestList = new List<PatientPCLRequest>();

            newInvoiceList = new List<OutwardDrugInvoice>();
            modifiedInvoiceList = new List<OutwardDrugInvoice>();
            deletedInvoiceList = new List<OutwardDrugInvoice>();

            if (registrationInfo.PatientRegistrationDetails != null)
            {
                foreach (var regDetail in registrationInfo.PatientRegistrationDetails)
                {
                    switch (regDetail.RecordState)
                    {
                        case RecordState.ADDED:
                        case RecordState.DETACHED:
                            newRegistrationDetailList.Add(regDetail);
                            break;

                        case RecordState.MODIFIED:
                            modifiedRegistrationDetailList.Add(regDetail);
                            break;

                        case RecordState.DELETED:
                            deletedRegistrationDetailList.Add(regDetail);
                            break;
                    }
                    regDetail.StaffID = StaffID;
                }
            }

            if (registrationInfo.PCLRequests != null)
            {
                foreach (var request in registrationInfo.PCLRequests)
                {
                    //request.StaffID = StaffID;
                    switch (request.RecordState)
                    {
                        case RecordState.ADDED:
                        case RecordState.DETACHED:
                            newPclRequestList.Add(request);
                            break;

                        case RecordState.MODIFIED:
                            modifiedPclRequestList.Add(request);
                            break;

                        case RecordState.DELETED:
                            deletedPclRequestList.Add(request);
                            break;
                    }
                }
            }

            if (registrationInfo.DrugInvoices != null)
            {
                foreach (var inv in registrationInfo.DrugInvoices)
                {
                    inv.StaffID = StaffID;
                    AddInvoiceToAppropriateList(inv, newInvoiceList, modifiedInvoiceList, deletedInvoiceList);
                    if (inv.ReturnedInvoices != null)
                    {
                        foreach (var returnedInv in inv.ReturnedInvoices)
                        {
                            returnedInv.StaffID = StaffID;
                            AddInvoiceToAppropriateList(returnedInv, newInvoiceList, modifiedInvoiceList, deletedInvoiceList);
                        }
                    }
                    //switch (inv.RecordState)
                    //{
                    //    case RecordState.ADDED:
                    //    case RecordState.DETACHED:
                    //        newInvoiceList.Add(inv);
                    //        break;

                    //    case RecordState.MODIFIED:
                    //        modifiedInvoiceList.Add(inv);
                    //        break;

                    //    case RecordState.DELETED:
                    //        deletedInvoiceList.Add(inv);
                    //        break;
                    //}
                }
            }
        }

        private void AddInvoiceToAppropriateList(OutwardDrugInvoice inv, List<OutwardDrugInvoice> newInvoiceList,
                                                 List<OutwardDrugInvoice> modifiedInvoiceList,
                                                 List<OutwardDrugInvoice> deletedInvoiceList)
        {
            switch (inv.RecordState)
            {
                case RecordState.ADDED:
                case RecordState.DETACHED:
                    newInvoiceList.Add(inv);
                    break;

                case RecordState.MODIFIED:
                    modifiedInvoiceList.Add(inv);
                    break;

                case RecordState.DELETED:
                    deletedInvoiceList.Add(inv);
                    break;
            }
        }

        public virtual void AddServicesAndPCLRequests(List<PatientRegistrationDetail> regDetailList
                                                    , List<PatientPCLRequest> pclRequestList
                                                    , List<PatientRegistrationDetail> deletedRegDetailList
                                                    , List<PatientPCLRequest> deletedPclRequestList
                                                    , DateTime modifiedDate, out long newRegistrationID)
        {
            newRegistrationID = -1;
        }

        /// <summary>
        /// Override ham nay de sua chua nhung thu can thiet truoc khi luu dang ky
        /// </summary>
        protected virtual void OnRegistrationSaving()
        {

        }
        protected virtual bool AddNewRegistration(out long newRegistrationID, out List<long> newRegistrationDetailIdList
                                    , out List<long> newPclRequestIdList)
        {
            CurrentRegistration.PtRegistrationCode = AxCodeGenerator.CreateRegistrationCode();
            OnRegistrationSaving();

            List<PatientRegistrationDetail> newRegistrationDetailList;
            List<PatientRegistrationDetail> modifiedRegistrationDetailList;
            List<PatientRegistrationDetail> deletedRegistrationDetailList;
            List<PatientPCLRequest> newPclRequestList;
            List<PatientPCLRequest> modifiedPclRequestList;
            List<PatientPCLRequest> deletedPclRequestList;
            List<OutwardDrugInvoice> newInvoiceList;
            List<OutwardDrugInvoice> modifiedInvoiceList;
            List<OutwardDrugInvoice> deletedInvoiceList;

            GetModifiedItems(CurrentRegistration.StaffID.Value, CurrentRegistration
                                    , out newRegistrationDetailList
                                    , out modifiedRegistrationDetailList
                                    , out deletedRegistrationDetailList
                                    , out newPclRequestList
                                    , out modifiedPclRequestList
                                    , out deletedPclRequestList
                                    , out newInvoiceList
                                    , out modifiedInvoiceList
                                    , out deletedInvoiceList);

            //Cap so thu tu cho cac item trong dang ky
            CreateSequenceNoForCurrentRegistration(newRegistrationDetailList, newPclRequestList, modifiedPclRequestList, newInvoiceList);

            bool bOK = false;

            /*Bang Check*/
            newRegistrationID = 0;
            newRegistrationDetailIdList = null;
            newPclRequestIdList = null;
            /*Bang Check*/

            try
            {
                bOK = PatientProvider.Instance.AddNewRegistration(CurrentRegistration, out newRegistrationID, out newRegistrationDetailIdList, out newPclRequestIdList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("Check: PatientProvider.Instance.AddNewRegistration ERROR.Detail:" + ex.ToString());
            }


            if (CurrentRegistration.AppointmentID.HasValue && CurrentRegistration.AppointmentID.Value > 0)
            {
                //Cap nhat lai cuoc hen.
                try
                {
                    PatientProvider.Instance.UpdateAppointmentStatus(CurrentRegistration.AppointmentID.Value, AllLookupValues.ApptStatus.ACTIONED);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("Check: PatientProvider.Instance.UpdateAppointmentStatus ERROR.Detail:" + ex.ToString());
                }
            }

            return bOK;
        }
        protected virtual bool UpdateRegistration(long StaffID, long CollectorDeptLocID, out List<long> newRegistrationDetailIdList
                                    , out List<long> newPclRequestIdList, out List<long> newPaymentIDList
                                    , out List<long> newDrugInvoiceIDList
                                    , out List<long> billingInvoiceIDs
                                    , out string PaymentIDListNy)
        {
            newRegistrationDetailIdList = null;
            newPclRequestIdList = null;
            newDrugInvoiceIDList = null;
            billingInvoiceIDs = null;
            PaymentIDListNy = null;
            /*
            // Txd commented out and moved into the calling method SaveRegistration
            if(CurrentRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
            {
                return UpdateInPatientRegistration(out billingInvoiceIDs, out newPaymentIDList);
            }
            */
            //Kiem tra co PatientMedicalRecord chua. Neu chua co thi tao moi luon.
            long medicalRecordID;
            long? medicalFileID;
            CreatePatientMedialRecordIfNotExists(CurrentRegistration.Patient.PatientID, CurrentRegistration.ExamDate, out medicalRecordID, out medicalFileID);
            OnRegistrationSaving();
            List<PatientRegistrationDetail> newRegistrationDetailList;
            List<PatientRegistrationDetail> modifiedRegistrationDetailList;
            List<PatientRegistrationDetail> deletedRegistrationDetailList;
            List<PatientPCLRequest> newPclRequestList;
            List<PatientPCLRequest> modifiedPclRequestList;
            List<PatientPCLRequest> deletedPclRequestList;
            List<OutwardDrugInvoice> newInvoiceList;
            List<OutwardDrugInvoice> modifiedInvoiceList;
            List<OutwardDrugInvoice> deletedInvoiceList;

            GetModifiedItems(StaffID, CurrentRegistration
                                    , out newRegistrationDetailList
                                    , out modifiedRegistrationDetailList
                                    , out deletedRegistrationDetailList
                                    , out newPclRequestList
                                    , out modifiedPclRequestList
                                    , out deletedPclRequestList
                                    , out newInvoiceList
                                    , out modifiedInvoiceList
                                    , out deletedInvoiceList);

            //Cap so thu tu cho cac item trong dang ky
            CreateSequenceNoForCurrentRegistration(newRegistrationDetailList, newPclRequestList, modifiedPclRequestList, newInvoiceList);

            List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum = new List<PatientPCLRequestDetail>();
            if (CurrentRegistration.lstDetail_ReUseServiceSeqNum != null)
            {
                lstDetail_ReUseServiceSeqNum = CurrentRegistration.lstDetail_ReUseServiceSeqNum.ToList();
            }

            long newPatientTransactionID;

            var xDocModifiedTran = ConvertModifiedTransactionToXml(CurrentRegistration.PatientTransaction, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);

            bool bOK = PatientProvider.Instance.AddUpdateServiceForRegistration(CurrentRegistration.PtRegistrationID, StaffID, CollectorDeptLocID
                , CurrentRegistration.RecordState == RecordState.MODIFIED, CurrentRegistration.ProgSumMinusMinHI, medicalRecordID
                                    , newRegistrationDetailList
                                    , modifiedRegistrationDetailList
                                    , deletedRegistrationDetailList
                                    , newPclRequestList
                                    , modifiedPclRequestList
                                    , deletedPclRequestList

                                    , lstDetail_ReUseServiceSeqNum

                                    , newInvoiceList
                                    , modifiedInvoiceList
                                    , deletedInvoiceList
                                    , xDocModifiedTran != null ? xDocModifiedTran.ToString() : null
                                    , (long)CurrentRegistration.V_RegistrationType
                                    , out newRegistrationDetailIdList
                                    , out newPclRequestIdList
                                    , out newPatientTransactionID
                                    , out newPaymentIDList
                                    , out newDrugInvoiceIDList
                                    , out PaymentIDListNy);


            return bOK;
        }

        private void GetSequenceNoForRegistrationDetail(PatientRegistrationDetail registrationDetail)
        {
            //Lay so thu tu
            try
            {
                List<PatientRegistrationDetail> newServices = null;
                if (CurrentRegistration.PatientRegistrationDetails != null)
                {
                    newServices = CurrentRegistration.PatientRegistrationDetails.Where(
                            item => (item.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.DANG_KY_KHAM) && (item.MarkedAsDeleted == false)).ToList();
                }
                var deptLocID = (uint)(registrationDetail.DeptLocation != null
                    && registrationDetail.DeptLocation.DeptLocationID > 0 ? registrationDetail.DeptLocation.DeptLocationID
                                        : registrationDetail.DeptLocID.GetValueOrDefault(0));

                PatientRegistrationDetail tempRegDetail = null;
                if (newServices != null)
                {
                    foreach (var service in newServices)
                    {
                        var tempDeptLocID = (uint)(service.DeptLocation != null ? service.DeptLocation.DeptLocationID
                                                 : service.DeptLocID.GetValueOrDefault(0));
                        if (tempDeptLocID == deptLocID && tempDeptLocID > 0)
                        {
                            tempRegDetail = service;
                            break;
                        }
                    }
                }

                if (tempRegDetail != null)
                {
                    registrationDetail.ServiceSeqNum = tempRegDetail.ServiceSeqNum;
                    registrationDetail.ServiceSeqNumType = tempRegDetail.ServiceSeqNumType;
                    return;
                }

                //var deptLocID = (uint)(registrationDetail.DeptLocation != null ? registrationDetail.DeptLocation.DeptLocationID
                //    :registrationDetail.DeptLocID.GetValueOrDefault(0));
                uint sequenceNo = 0;
                byte sequenceNoType = 0;

                var provider = new ServiceSequenceNumberProvider();
                if ((registrationDetail.FromAppointment && registrationDetail.AppointmentDate == null)
                    || (registrationDetail.FromAppointment
                        && registrationDetail.AppointmentDate != null
                        && registrationDetail.AppointmentDate.Value.Date == DateTime.Now.Date))
                {
                    int bReassignedSeqNum;
                    sequenceNo = (uint)registrationDetail.ServiceSeqNum;
                    provider.ConfirmApptConsultRoomAndSequenceNumber(1, out bReassignedSeqNum, out sequenceNoType, ref deptLocID, ref sequenceNo);
                }
                else
                {
                    //Kiem tra dich vu nay co duoc cap so hay khong
                    if (CheckCanGetSequenceNumber((long)registrationDetail.RefMedicalServiceItem.MedicalServiceTypeID))
                    {
                        provider.GetConsultRoomAndSequenceNumberAuto(out sequenceNoType, ref deptLocID, out sequenceNo);
                    }
                }

                registrationDetail.DeptLocID = deptLocID;
                registrationDetail.DeptLocation = null;
                registrationDetail.ServiceSeqNum = (int)sequenceNo;
                registrationDetail.ServiceSeqNumType = sequenceNoType;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("ERROR: " + ex.ToString());
            }
        }

        public bool CheckCanGetSequenceNumber(long MedicalServiceTypeID)
        {
            if (AxCache.Current != null && AxCache.Current["AllMedicalServiceTypes"] != null)
            {
                    var AllMedicalServiceTypes = (List<RefMedicalServiceType>)AxCache.Current["AllMedicalServiceTypes"];
                    return AllMedicalServiceTypes.Where(item => item.MedicalServiceTypeID == MedicalServiceTypeID
                        && item.V_RefMedicalServiceInOutOthers == (long)AllLookupValues.V_RefMedicalServiceInOutOthers.NGOAITRU
                        && item.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH).Count() > 0;
                                                   
            }
            return true;
        }

        private void GetSequenceNoForPclRequestDetailsList(IEnumerable<PatientPCLRequestDetail> pclRequestDetails)
        {
            try
            {
                var sb = new StringBuilder();
                var allPclRequestItems = new ObservableCollection<PatientPCLRequestDetail>();
                foreach (var reqDetail in pclRequestDetails)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(";");
                    }
                    var deptLocId = reqDetail.DeptLocation != null ? reqDetail.DeptLocation.DeptLocationID : 0;
                    var examTypeId = reqDetail.PCLExamType != null ? reqDetail.PCLExamType.PCLExamTypeID : 0;
                    sb.AppendFormat("({0},{1})", examTypeId, deptLocId);
                    allPclRequestItems.Add(reqDetail);
                }
                if (sb.Length > 0)
                {
                    var provider = new ServiceSequenceNumberProvider();

                    var strSequenceNumList = string.Empty;
                    provider.GetPCLSequenceNumber(sb.ToString(), out strSequenceNumList);

                    var s = @"^(\(\d{1,},\d{1,},\d{1,},\d{1,}\)(;\(\d{1,},\d{1,},\d{1,},\d{1,}\))*)$";
                    var regEx = new Regex(s);
                    var match = regEx.Match(strSequenceNumList);
                    if (match.Success)
                    {
                        //Lam tam. Hien gio khong lay dc group con.

                        var sTemp = strSequenceNumList.Replace("(", "").Replace(")", "").Replace(";", ",");
                        var arr = sTemp.Split(new char[] { ',' });
                        for (var i = 0; i < arr.Length; i += 4)
                        {
                            var pclExamTypeId = int.Parse(arr[i]);
                            var deptLocID = int.Parse(arr[i + 1]);
                            var seqNumber = int.Parse(arr[i + 2]);
                            var seqNumberType = byte.Parse(arr[i + 3]);

                            //Duyet qua ds cls hoi nay xin so, cap nhat lai tri phong.
                            for (var index = 0; index < allPclRequestItems.Count; index++)
                            {
                                var requestDetail = allPclRequestItems[index];
                                if (requestDetail.PCLExamType.PCLExamTypeID != pclExamTypeId
                                    || requestDetail.SequenceNoReassigned)
                                    continue;
                                var deptLoc = new DeptLocation { DeptLocationID = deptLocID };
                                requestDetail.DeptLocation = deptLoc;
                                requestDetail.ServiceSeqNum = (int)seqNumber;
                                requestDetail.ServiceSeqNumType = seqNumberType;
                                requestDetail.SequenceNoReassigned = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("{0}.",eHCMSResources.Z1836_G1_StrInIncorrectFormat));
                    }
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("ERROR: " + ex.ToString());
            }
        }

        struct DeptLocSeqNum
        {            
            public long nDeptLocID;
            public uint nSequenceNum;
            public byte nSequenceNumType;
        };

        protected void CreateSeqNumForRegDetailServices(List<PatientRegistrationDetail> newRegDetailList)
        {
            if (newRegDetailList == null || newRegDetailList.Count() == 0)
                return; // Nothing to do here
            // 1. Order the list of new Reg Detail Items
            newRegDetailList = newRegDetailList.OrderByDescending(x => x.FromAppointment).ToList();
                        
            List<DeptLocSeqNum> existDeptLocSeqNumList = null;
            if (CurrentRegistration.PatientRegistrationDetails != null)
            {
                existDeptLocSeqNumList = (from regItem in CurrentRegistration.PatientRegistrationDetails
                                         where ((regItem.ServiceSeqNum > 0) && (regItem.V_ExamRegStatus == (long)AllLookupValues.ExamRegStatus.DANG_KY_KHAM) && (regItem.MarkedAsDeleted == false))
                                          select new DeptLocSeqNum { nDeptLocID = regItem.DeptLocation.DeptLocationID, nSequenceNum = (uint)regItem.ServiceSeqNum, nSequenceNumType = regItem.ServiceSeqNumType}).ToList();
            }

            var provider = new ServiceSequenceNumberProvider();
            foreach (var regDetailItem in newRegDetailList)
            {
                long itemDeptLocID = regDetailItem.DeptLocation != null && regDetailItem.DeptLocation.DeptLocationID > 0 ? regDetailItem.DeptLocation.DeptLocationID : regDetailItem.DeptLocID.GetValueOrDefault(0);
                long MedServiceID = regDetailItem.RefMedicalServiceItem != null ? regDetailItem.RefMedicalServiceItem.MedServiceID : regDetailItem.MedServiceID.GetValueOrDefault(0);

                bool bAssignedSeqNum = false;                
                uint sequenceNo = 0;
                byte sequenceNoType = 0;
                uint deptLocID = 0;

                if ((regDetailItem.FromAppointment && regDetailItem.AppointmentDate == null && regDetailItem.ServiceSeqNum > 0)
                    || (regDetailItem.FromAppointment && regDetailItem.AppointmentDate != null && regDetailItem.AppointmentDate.Value.Date == DateTime.Now.Date && regDetailItem.ServiceSeqNum > 0))
                {
                    int bReassignedSeqNum = 0;
                    sequenceNo = (uint)regDetailItem.ServiceSeqNum;
                    deptLocID = (uint)regDetailItem.DeptLocation.DeptLocationID;
                    provider.ConfirmApptConsultRoomAndSequenceNumber(1, out bReassignedSeqNum, out sequenceNoType, ref deptLocID, ref sequenceNo);
                    bAssignedSeqNum = true;
                }
                else
                {
                    if (itemDeptLocID > 0)  // RegDetail Item that has been assigned with DeptLocID (Room) from the GUI
                    {
                        foreach (var deptLocSeqNum in existDeptLocSeqNumList)
                        {
                            if (itemDeptLocID == deptLocSeqNum.nDeptLocID)
                            {
                                bAssignedSeqNum = true;
                                sequenceNo = deptLocSeqNum.nSequenceNum;
                                sequenceNoType = deptLocSeqNum.nSequenceNumType;
                                deptLocID = (uint)deptLocSeqNum.nDeptLocID;
                                break;
                            }
                        }

                        if (!bAssignedSeqNum)
                        {
                            if (CheckCanGetSequenceNumber((long)regDetailItem.RefMedicalServiceItem.MedicalServiceTypeID))
                            {
                                deptLocID = (uint)regDetailItem.DeptLocation.DeptLocationID;
                                provider.GetConsultRoomAndSequenceNumberAuto(out sequenceNoType, ref deptLocID, out sequenceNo);
                                bAssignedSeqNum = true;
                            }
                        }
                    }
                    else
                    {
                        string mainCacheKey = "MedicalServiceItemsAndDeptLoc";
                        List<DeptLocation> LstDept = null;
                        if (ServerAppConfig.CachingEnabled && (List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey] != null)
                        {
                            List<RefMedicalServiceItem> listRefMedSvrItem = ((List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey]);
                            var medSvrItem = (from theItem in listRefMedSvrItem
                                              where theItem.MedServiceID == MedServiceID && theItem.allDeptLocation != null && theItem.allDeptLocation.Count > 0
                                              select theItem).SingleOrDefault();
                            if (medSvrItem != null && medSvrItem.allDeptLocation != null && medSvrItem.allDeptLocation.Count > 0)
                            {
                                LstDept = medSvrItem.allDeptLocation.ToList();
                            }
                        }
                        else
                        {
                            LstDept = PatientProvider.Instance.GetLocationsByServiceID(MedServiceID);
                        }

                        if (LstDept != null && LstDept.Count() > 0)
                        {                            
                            foreach (var deptLocSeqNum in existDeptLocSeqNumList)
                            {
                                if (LstDept.Where(x => x.DeptLocationID == deptLocSeqNum.nDeptLocID).Count() > 0)
                                {
                                    bAssignedSeqNum = true;
                                    sequenceNo = deptLocSeqNum.nSequenceNum;
                                    sequenceNoType = deptLocSeqNum.nSequenceNumType;
                                    deptLocID = (uint)deptLocSeqNum.nDeptLocID;
                                    
                                    break;
                                }
                            }

                            if (!bAssignedSeqNum)
                            {
                                //Kiem tra dich vu nay co duoc cap so hay khong
                                if (CheckCanGetSequenceNumber((long)regDetailItem.RefMedicalServiceItem.MedicalServiceTypeID))
                                {
                                    provider.GetConsultRoomAndSequenceNumberAuto(out sequenceNoType, ref deptLocID, out sequenceNo);
                                    bAssignedSeqNum = true;
                                }
                            }
                        }                        
                    }

                }

                if (bAssignedSeqNum)
                {
                    regDetailItem.DeptLocation = new DeptLocation();
                    regDetailItem.DeptLocation.DeptLocationID = deptLocID;
                    regDetailItem.DeptLocID = deptLocID;
                    regDetailItem.ServiceSeqNum = (int)sequenceNo;
                    regDetailItem.ServiceSeqNumType = sequenceNoType;

                    existDeptLocSeqNumList.Add(new DeptLocSeqNum
                                                    {
                                                        nDeptLocID = deptLocID,
                                                        nSequenceNum = sequenceNo,
                                                        nSequenceNumType = sequenceNoType
                                                    });

                }

            }

        }

        protected void CreateSequenceNoForCurrentRegistration(List<PatientRegistrationDetail> newRegDetailList,
            List<PatientPCLRequest> newPclRequestList, List<PatientPCLRequest> modifiedPclRequestList, List<OutwardDrugInvoice> newInvoiceList)
        {

            try
            {

                CreateSeqNumForRegDetailServices(newRegDetailList);

                ////co thoi gian xem lai cho nay,co j do hoi lung cung o day ne!
                //string mainCacheKey = "MedicalServiceItemsAndDeptLoc";
                //if (newRegDetailList != null)
                //{
                //    newRegDetailList = newRegDetailList.OrderByDescending(x => x.FromAppointment).ToList();

                //    //var DeptLocIDlst = newRegDetailList.Select(x => new
                //    //{
                //    //    DeptlocID = x.DeptLocation != null
                //    //        && x.DeptLocation.DeptLocationID > 0 ? x.DeptLocation.DeptLocationID : x.DeptLocID.GetValueOrDefault(0)
                //    //}).Distinct().ToList();

                //    var DeptLocIDlst = (from regDetItem in newRegDetailList
                //                        where regDetItem.DeptLocation != null && regDetItem.DeptLocation.DeptLocationID > 0
                //                        select new { DeptlocID = regDetItem.DeptLocation.DeptLocationID }).Distinct().ToList();

                //    int bcount = 0;
                //    uint sequenceNo = 0;
                //    byte sequenceNoType = 0;
                //    long DeptLocIDReturn = 0;

                //    //gan phong cho benh nhan neu co phong khac cung co kham nhung dich vu nay
                //    foreach (var invoiceItem in newRegDetailList)
                //    {
                //        long DeptLocID = invoiceItem.DeptLocation != null && invoiceItem.DeptLocation.DeptLocationID > 0
                //            ? invoiceItem.DeptLocation.DeptLocationID : invoiceItem.DeptLocID.GetValueOrDefault(0);
                //        long MedServiceID = invoiceItem.RefMedicalServiceItem != null ? invoiceItem.RefMedicalServiceItem.MedServiceID : invoiceItem.MedServiceID.GetValueOrDefault(0);
                //        if (DeptLocID <= 0 && DeptLocIDlst != null && DeptLocIDlst.Count() > 0)
                //        {
                //            foreach (var i in DeptLocIDlst)
                //            {
                //                List<DeptLocation> LstDept = null;
                //                if (ServerAppConfig.CachingEnabled && (List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey] != null)
                //                {
                //                    //LstDept = ((List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey])
                //                    //    .Where(o => o.MedServiceID == MedServiceID).ToList()[0].allDeptLocation.ToList();

                //                    List<RefMedicalServiceItem> listRefMedSvrItem = ((List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey]);
                //                    var medSvrItem = (from theItem in listRefMedSvrItem
                //                                     where theItem.MedServiceID == MedServiceID && theItem.allDeptLocation != null && theItem.allDeptLocation.Count > 0
                //                                     select theItem).SingleOrDefault();
                //                    if (medSvrItem != null && medSvrItem.allDeptLocation != null && medSvrItem.allDeptLocation.Count > 0)
                //                    {
                //                        LstDept = medSvrItem.allDeptLocation.ToList();
                //                    }
                //                }
                //                else
                //                {
                //                    LstDept = PatientProvider.Instance.GetLocationsByServiceID(MedServiceID);
                //                }

                //                if (LstDept != null && LstDept.Count() > 0)
                //                {
                //                    if (LstDept.Where(x => x.DeptLocationID == i.DeptlocID && i.DeptlocID > 0).Count() > 0)
                //                    {
                //                        invoiceItem.DeptLocation = new DeptLocation();
                //                        invoiceItem.DeptLocation.DeptLocationID = i.DeptlocID;
                //                        invoiceItem.DeptLocID = i.DeptlocID;
                //                    }
                //                }
                //                //neu khong gan cho bat cu phong nao thi nen cho trang thai nao do de biet,dv nay khong co phong,khong can vao xin so nua
                //                else
                //                {
                //                    invoiceItem.DeptLocation = new DeptLocation();
                //                    invoiceItem.DeptLocation.DeptLocationID = -10;//gan vay de ben duoi khong thay,de biet dv nay khong co phong nao het
                //                    invoiceItem.DeptLocID = -10;
                //                }
                //            }
                //        }
                //    }

                //    //gan xong thi lay lai danh sach phong
                //    var DeptLocIDlstFinal = newRegDetailList.Select(x => new
                //    {
                //        DeptlocID = x.DeptLocation != null
                //            && x.DeptLocation.DeptLocationID > 0 ? x.DeptLocation.DeptLocationID : x.DeptLocID.GetValueOrDefault(0)
                //    }).Distinct();

                //    foreach (var i in DeptLocIDlstFinal)
                //    {
                //        bcount = 0;
                //        sequenceNo = 0;
                //        sequenceNoType = 0;
                //        DeptLocIDReturn = 0;
                //        //gan phong cho benh nhan neu cac phong khac cung co kham nhung dich vu nay

                //        foreach (var invoiceItem in newRegDetailList)
                //        {
                //            long DeptLocID = invoiceItem.DeptLocation != null
                //                && invoiceItem.DeptLocation.DeptLocationID > 0 ? invoiceItem.DeptLocation.DeptLocationID : invoiceItem.DeptLocID.GetValueOrDefault(0);
                //            long MedServiceID = invoiceItem.RefMedicalServiceItem != null ? invoiceItem.RefMedicalServiceItem.MedServiceID : invoiceItem.MedServiceID.GetValueOrDefault(0);
                //            if (DeptLocID == i.DeptlocID)
                //            {
                //                bcount++;
                //                if (DeptLocID <= 0)
                //                {
                //                    //neu dv do co phong
                //                    List<DeptLocation> LstDept = PatientProvider.Instance.GetLocationsByServiceID(MedServiceID);
                //                    if (LstDept != null && LstDept.Count() > 0)
                //                    {
                //                        //neu co chua phong moi tra ve thi lay phong do va STT do luon
                //                        if (LstDept.Where(x => x.DeptLocationID == DeptLocIDReturn && DeptLocIDReturn > 0).Count() > 0)
                //                        {
                //                            invoiceItem.ServiceSeqNum = (int)sequenceNo;
                //                            invoiceItem.ServiceSeqNumType = sequenceNoType;
                //                            invoiceItem.DeptLocID = DeptLocIDReturn;
                //                            continue;
                //                        }
                //                    }
                //                    //neu dv khong co phong thi khong cap so j het
                //                    else
                //                    {
                //                        continue;
                //                    }
                //                }

                //                if (bcount == 1)
                //                {
                //                    GetSequenceNoForRegistrationDetail(invoiceItem);
                //                    sequenceNo = (uint)invoiceItem.ServiceSeqNum;
                //                    sequenceNoType = invoiceItem.ServiceSeqNumType;
                //                    DeptLocIDReturn = invoiceItem.DeptLocID.GetValueOrDefault(0);
                //                }
                //                else
                //                {
                //                    invoiceItem.ServiceSeqNum = (int)sequenceNo;
                //                    invoiceItem.ServiceSeqNumType = sequenceNoType;
                //                    invoiceItem.DeptLocID = DeptLocIDReturn;
                //                }
                //            }
                //        }
                //    }
                //}

                var sb = new StringBuilder();
                //var allPclRequestItems = new ObservableCollection<PatientPCLRequestDetail>();
                //Chuoi dua vao ham anh Tuan co dang (examTypeID1,deptLocID1),(examTypeID2,deptLocID2)
                //Chuoi output tu ham anh Tuan co dang (examTypeID1,deptLocID1,seqNumber1,seqNumberType1),(examTypeID2,deptLocID2,seqNumber2,seqNumberType2)
                //Khong xin so cho request moi
                //Doi voi request moi => da xin so truoc roi.
                //if(newPclRequestList != null)
                //{
                //    foreach (var newReqDetail in newPclRequestList.SelectMany(item => item.PatientPCLRequestIndicators))
                //    {
                //        if(sb.Length > 0)
                //        {
                //            sb.Append(";");
                //        }
                //        var deptLocId = newReqDetail.DeptLocation != null ?  newReqDetail.DeptLocation.DeptLocationID : 0;
                //        var examTypeId = newReqDetail.PCLExamType != null ? newReqDetail.PCLExamType.PCLExamTypeID : 0;
                //        sb.AppendFormat("({0},{1})", examTypeId, deptLocId);
                //        allPclRequestItems.Add(newReqDetail);
                //    }
                //}
                if (modifiedPclRequestList != null)
                {
                    //foreach (var reqDetail in modifiedPclRequestList.SelectMany(item => item.PatientPCLRequestIndicators.Where(obj => obj.LocationChanged)))
                    //{
                    //    if (sb.Length > 0)
                    //    {
                    //        sb.Append(";");
                    //    }
                    //    var deptLocId = reqDetail.DeptLocation != null ? reqDetail.DeptLocation.DeptLocationID : 0;
                    //    var examTypeId = reqDetail.PCLExamType != null ? reqDetail.PCLExamType.PCLExamTypeID : 0;
                    //    sb.AppendFormat("({0},{1})", examTypeId, deptLocId);
                    //    allPclRequestItems.Add(reqDetail);
                    //}
                    GetSequenceNoForPclRequestDetailsList(modifiedPclRequestList.SelectMany(item => item.PatientPCLRequestIndicators.Where(obj => obj.LocationChanged)));
                }
                //if(sb.Length > 0)
                //{
                //    var provider = new ServiceSequenceNumberProvider();
                //    long sequenceNo;
                //    byte sequenceNoType;

                //    var strSequenceNumList = string.Empty;
                //    provider.GetPCLSequenceNumber(sb.ToString(),out strSequenceNumList);

                //    var s = @"^(\(\d{1,},\d{1,},\d{1,},\d{1,}\)(;\(\d{1,},\d{1,},\d{1,},\d{1,}\))*)$";
                //    var regEx = new Regex(s);
                //    var match = regEx.Match(strSequenceNumList);
                //    if (match.Success)
                //    {
                //        //Lam tam. Hien gio khong lay dc group con.

                //        var sTemp = strSequenceNumList.Replace("(","").Replace(")","").Replace(";",",");
                //        var arr = sTemp.Split(new char[] { ',' });
                //        for(var i=0;i<arr.Length;i+=4)
                //        {
                //            var pclExamTypeId = int.Parse(arr[i]);
                //            var deptLocID = int.Parse(arr[i+1]);
                //            var seqNumber = int.Parse(arr[i+2]);
                //            var seqNumberType = byte.Parse(arr[i+3]);

                //            //Duyet qua ds cls hoi nay xin so, cap nhat lai tri phong.
                //            for (int index = 0; index < allPclRequestItems.Count; index++)
                //            {
                //                var requestDetail = allPclRequestItems[index];
                //                if (requestDetail.PCLExamType.PCLExamTypeID != pclExamTypeId
                //                    || requestDetail.SequenceNoReassigned)
                //                    continue;
                //                var deptLoc = new DeptLocation {DeptLocationID = deptLocID};
                //                requestDetail.DeptLocation = deptLoc;
                //                requestDetail.ServiceSeqNum = (int) seqNumber;
                //                requestDetail.ServiceSeqNumType = seqNumberType;
                //                requestDetail.SequenceNoReassigned = true;
                //                break;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        throw new Exception(string.Format("{0}.",eHCMSResources.Z1836_G1_StrInIncorrectFormat));
                //    }
                //}

                if (newInvoiceList != null)
                {
                    foreach (var invoice in newInvoiceList)
                    {
                        if (invoice.ReturnID.GetValueOrDefault(0) > 0 || invoice.IsUpdate.GetValueOrDefault(false))
                        {
                            continue;
                        }
                        var numProvider = new ServiceSequenceNumberProvider();
                        PharmacyInvType type = PharmacyInvType.KHONG_BAO_HIEM;
                        if (invoice.IsHICount.GetValueOrDefault(false))
                        {
                            type = PharmacyInvType.CO_BAO_HIEM;
                        }
                        byte seqNumType;
                        uint seqNum;
                        numProvider.GetPharmacyOutDrugInvSeqNum(out seqNumType, type, out seqNum);
                        invoice.ColectDrugSeqNum = (int)seqNum;
                        invoice.ColectDrugSeqNumType = seqNumType;
                    }
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("ERROR: " + ex.ToString());
            }

        }

        protected void BalanceTransaction(DateTime balanceDate = default(DateTime))
        {
            if (CurrentRegistration.PatientTransaction == null)
            {
                return;
            }
            List<PatientRegistrationDetail> colRegDetails = null;
            List<PatientPCLRequest> colPclRequests = null;
            List<OutwardDrugInvoice> colDrugInvoice = null;
            List<InPatientBillingInvoice> colBillingInvoices = null;
            //Lay tat ca nhung item nao cua dang ky bi thay doi thi can bang lai.
            if (CurrentRegistration.PatientRegistrationDetails != null)
            {
                colRegDetails = CurrentRegistration.PatientRegistrationDetails.Where(item => item.PtRegDetailID > 0 && item.RecordState != RecordState.UNCHANGED).ToList();
            }
            if (CurrentRegistration.PCLRequests != null)
            {
                colPclRequests = CurrentRegistration.PCLRequests.Where(item => item.PatientPCLReqID > 0 && item.RecordState != RecordState.UNCHANGED).ToList();
            }
            if (CurrentRegistration.DrugInvoices != null)
            {
                //colDrugInvoice = CurrentRegistration.DrugInvoices.Where(item => item.outiID > 0 && item.RecordState != RecordState.UNCHANGED).ToList();
                colDrugInvoice = new List<OutwardDrugInvoice>();
                foreach (var invoice in CurrentRegistration.DrugInvoices)
                {
                    if (CheckIfInvoiceCanBeBalanced(invoice))
                    {
                        colDrugInvoice.Add(invoice);
                    }
                    if (invoice.ReturnedInvoices != null)
                    {
                        foreach (var returnedInvoice in invoice.ReturnedInvoices)
                        {
                            if (CheckIfInvoiceCanBeBalanced(returnedInvoice))
                            {
                                colDrugInvoice.Add(returnedInvoice);
                            }
                        }
                    }

                }
            }
            if (CurrentRegistration.InPatientBillingInvoices != null)
            {
                colBillingInvoices = CurrentRegistration.InPatientBillingInvoices.Where(item => item.InPatientBillingInvID > 0 && item.RecordState != RecordState.UNCHANGED).ToList();
            }
            BalanceTransaction(colRegDetails, colPclRequests, colDrugInvoice, colBillingInvoices, balanceDate);
        }

        private bool CheckIfInvoiceCanBeBalanced(OutwardDrugInvoice invoice)
        {
            return (invoice.RecordState == RecordState.DETACHED || (invoice.outiID > 0 && invoice.RecordState != RecordState.UNCHANGED));
        }

        /// <summary>
        /// Tính lại toàn bộ đăng ký (bao gồm thông tin tiền bạc trong transaction nếu có)
        /// </summary>
        protected virtual decimal CalProgSumMinusMinHI()
        {
            return 0;
        }

        public virtual bool AddServicesAndPCLRequests_Bang_Old(long StaffID,long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration regInfo, List<PatientRegistrationDetail> regDetailList,
                                             List<PatientPCLRequest> pclRequestList, List<PatientRegistrationDetail> deletedRegDetailList
           , List<PatientPCLRequest> deletedPclRequestList
           , DateTime modifiedDate
           , out long newRegistrationID, out List<long> newRegistrationIDList, out List<long> newPclRequestList)
        {
            List<PatientPCLRequest> newPclList = null;

            if (pclRequestList != null)
            {
                // Dieu Kien dau tien: Phai co Map <PCLExamTypeID, DeptLoc> duoc tao ra global roi

                // 1. Duyet qua tat ca PCLRequest de lay ra tat ca PCLExamTypes chua co Seq va chua thuc hien

                // 2. Khi lam Step 1 o tren luu lai Map<PCLReqID, DeptLoc> cho nhung PCLReq co ID > 0 de qua buoc 3 so sanh tim PCLReq co DeptLocID nhet them vo

                // 3. Duyet qua nhung PCLExamTypes moi chua co Seq:

                // a. Nhung PCLExamType nao co chi dinh DeptLoc neu tim duoc PCLReq nao co trung DeptLoc thi add vo
                // b. Con Nhung PCLExamType nao khong co chi dinh Deptloc thi dung Map (Dieu Kien o tren) de tim DeptLoc va neu add duoc thi add vo PCLReq co roi.

                // 4. Gom nhung PCLExamType khong the add vao PCLReq da co roi o tren step 3 goi ham GetSequenceNoForPclRequestDetailsList de lay Seq.

                // 5. Cuoi cung dung khuc code o duoi de phan bo vao nhung PCLReq moi se duoc tao ra.


                //Code
                //1
                List<PatientPCLRequestDetail> lstPCLDetailAddOn_temp = new List<PatientPCLRequestDetail>();
                foreach (var itemdetail in pclRequestList.SelectMany(item => item.PatientPCLRequestIndicators))
                {
                    lstPCLDetailAddOn_temp.Add(itemdetail);
                }

                //cho nay co j do sai ne!
                //cac PCLExamType chua co SeqNum va chua thuc hien
                var lstNotYetSeqNum = (from c in lstPCLDetailAddOn_temp
                                       where c.PCLReqItemID <= 0 && c.ServiceSeqNum <= 0 && c.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.HOAN_TAT
                                       select c).ToList();
                //1

                //2
                var lstPCLReqExists = (from c in pclRequestList
                                       where c.PatientPCLReqID > 0
                                       select c).ToList();
                //2

                //3
                List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum = new List<PatientPCLRequestDetail>();
                List<PatientPCLRequestDetail> lstDetail_CreateNew = new List<PatientPCLRequestDetail>();
                Split2List(lstPCLReqExists, lstNotYetSeqNum, out lstDetail_ReUseServiceSeqNum, out lstDetail_CreateNew);

                CurrentRegistration.lstDetail_ReUseServiceSeqNum = new ObservableCollection<PatientPCLRequestDetail>(lstDetail_ReUseServiceSeqNum);
                CalcInvoiceItems(CurrentRegistration.lstDetail_ReUseServiceSeqNum);
                //3

                //4
                GetSequenceNoForPclRequestDetailsList(lstDetail_CreateNew);

                var requests = new Dictionary<long, PatientPCLRequest>();
                foreach (var reqDetails in lstDetail_CreateNew)
                {
                    if (reqDetails.DeptLocation == null)
                    {
                        reqDetails.DeptLocation = new DeptLocation();
                    }
                    if (requests.ContainsKey(reqDetails.DeptLocation.DeptLocationID))
                    {
                        var item = requests[reqDetails.DeptLocation.DeptLocationID];
                        item.PatientPCLRequestIndicators.Add(reqDetails);
                    }
                    else
                    {
                        var newRequest = new PatientPCLRequest();
                        newRequest.StaffID = reqDetails.PatientPCLRequest.StaffID;
                        newRequest.DoctorStaffID = reqDetails.PatientPCLRequest.DoctorStaffID;
                        newRequest.Diagnosis = reqDetails.PatientPCLRequest.Diagnosis;

                        newRequest.DoctorComments = reqDetails.PatientPCLRequest.DoctorComments;
                        newRequest.DeptID = reqDetails.PatientPCLRequest.DeptID;
                        newRequest.PCLDeptLocID = reqDetails.PatientPCLRequest.PCLDeptLocID;
                        newRequest.ReqFromDeptLocID = reqDetails.PatientPCLRequest.ReqFromDeptLocID;
                        newRequest.RequestedDoctorName = reqDetails.PatientPCLRequest.RequestedDoctorName;
                        newRequest.ServiceRecID = reqDetails.PatientPCLRequest.ServiceRecID;

                        newRequest.V_PCLRequestStatus = reqDetails.PatientPCLRequest.V_PCLRequestStatus;
                        newRequest.V_PCLRequestType = reqDetails.PatientPCLRequest.V_PCLRequestType;
                        newRequest.V_PCLRequestStatusName = reqDetails.PatientPCLRequest.V_PCLRequestStatusName;
                        newRequest.ExamRegStatus = reqDetails.PatientPCLRequest.ExamRegStatus;
                        newRequest.RecordState = RecordState.DETACHED;//reqDetails.PatientPCLRequest.RecordState;
                        newRequest.EntityState = EntityState.NEW;//reqDetails.PatientPCLRequest.EntityState;
                        newRequest.CreatedDate = reqDetails.PatientPCLRequest.CreatedDate;
                        newRequest.ExamDate = reqDetails.PatientPCLRequest.ExamDate;

                        newRequest.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                        newRequest.PatientPCLRequestIndicators.Add(reqDetails);
                        newRequest.V_PCLMainCategory = reqDetails.PCLExamType.V_PCLMainCategory;

                        requests.Add(reqDetails.DeptLocation.DeptLocationID, newRequest);
                    }
                }
                newPclList = requests.Values.ToList();
                //4
            }

            AddRegistrationDetailsToCurrentRegistration(regDetailList);
            AddPclRequestsToCurrentRegistration(newPclList, StaffID);
            AddRegistrationDetailsToCurrentRegistration(deletedRegDetailList);
            AddPclRequestsToCurrentRegistration(deletedPclRequestList);
            List<long> newPaymentIDList;
            List<long> newOutwardDrugIDList;
            List<long> billingInvoiceIDs;
            string PaymentIDListNy;
            SaveRegistrationForOutPatient(StaffID,CollectorDeptLocID, Apply15HIPercent, modifiedDate, out newRegistrationID, out newRegistrationIDList, out newPclRequestList, out newPaymentIDList, out newOutwardDrugIDList, out billingInvoiceIDs, out PaymentIDListNy);

            return true;
        }


        public virtual bool AddServicesAndPCLRequests(long StaffID, long CollectorDeptLocID,int? Apply15HIPercent, PatientRegistration regInfo, List<PatientRegistrationDetail> regDetailList,
                                              List<PatientPCLRequest> pclRequestList, List<PatientRegistrationDetail> deletedRegDetailList
            , List<PatientPCLRequest> deletedPclRequestList
            , DateTime modifiedDate
            , out long newRegistrationID, out List<long> newRegistrationIDList, out List<long> newPclRequestList)
        {

            List<PatientPCLRequest> newPclList = null;

            if (pclRequestList != null)
            {
                // Dieu Kien dau tien: Phai co Map <PCLExamTypeID, DeptLoc> duoc tao ra global roi

                // 1. Duyet qua tat ca PCLRequest de lay ra tat ca PCLExamTypes chua co Seq va chua thuc hien

                // 2. Khi lam Step 1 o tren luu lai Map<PCLReqID, DeptLoc> cho nhung PCLReq co ID > 0 de qua buoc 3 so sanh tim PCLReq co DeptLocID nhet them vo

                // 3. Duyet qua nhung PCLExamTypes moi chua co Seq:

                // a. Nhung PCLExamType nao co chi dinh DeptLoc neu tim duoc PCLReq nao co trung DeptLoc thi add vo
                // b. Con Nhung PCLExamType nao khong co chi dinh Deptloc thi dung Map (Dieu Kien o tren) de tim DeptLoc va neu add duoc thi add vo PCLReq co roi.

                // 4. Gom nhung PCLExamType khong the add vao PCLReq da co roi o tren step 3 goi ham GetSequenceNoForPclRequestDetailsList de lay Seq.

                // 5. Cuoi cung dung khuc code o duoi de phan bo vao nhung PCLReq moi se duoc tao ra.


                //Code
                //1
                List<PatientPCLRequestDetail> lstPCLDetailAddOn_temp = new List<PatientPCLRequestDetail>();
                foreach (var itemdetail in pclRequestList.SelectMany(item => item.PatientPCLRequestIndicators))
                {
                    lstPCLDetailAddOn_temp.Add(itemdetail);
                }

                //cho nay co j do sai ne!
                //cac PCLExamType chua co SeqNum va chua thuc hien
                var lstNotYetSeqNum = (from c in lstPCLDetailAddOn_temp
                                       where c.PCLReqItemID <= 0 && c.ServiceSeqNum <= 0 && c.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.HOAN_TAT
                                       select c).ToList();
                //1

                //2
                var lstPCLReqExists = (from c in pclRequestList
                                       where c.PatientPCLReqID > 0
                                       select c).ToList();
                //2

                //3
                List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum = new List<PatientPCLRequestDetail>();
                List<PatientPCLRequestDetail> lstDetail_CreateNew = new List<PatientPCLRequestDetail>();
                Split2List(lstPCLReqExists, lstNotYetSeqNum, out lstDetail_ReUseServiceSeqNum, out lstDetail_CreateNew);

                CurrentRegistration.lstDetail_ReUseServiceSeqNum = new ObservableCollection<PatientPCLRequestDetail>(lstDetail_ReUseServiceSeqNum);
                CalcInvoiceItems(CurrentRegistration.lstDetail_ReUseServiceSeqNum);
                //3

                //4
                GetSequenceNoForPclRequestDetailsList(lstDetail_CreateNew);

                newPclList = SplitVote_Ngtr(lstDetail_CreateNew);
                //4
            }

            AddRegistrationDetailsToCurrentRegistration(regDetailList);
            AddPclRequestsToCurrentRegistration(newPclList, StaffID);
            AddRegistrationDetailsToCurrentRegistration(deletedRegDetailList);
            AddPclRequestsToCurrentRegistration(deletedPclRequestList);
            List<long> newPaymentIDList;
            List<long> newOutwardDrugIDList;
            List<long> billingInvoiceIDs;
            string PaymentIDListNy;
            SaveRegistrationForOutPatient(StaffID,CollectorDeptLocID, Apply15HIPercent, modifiedDate, out newRegistrationID, out newRegistrationIDList, out newPclRequestList, out newPaymentIDList, out newOutwardDrugIDList, out billingInvoiceIDs, out PaymentIDListNy);

            return true;
        }


        public virtual List<PatientPCLRequest> GetNewPclList(List<PatientPCLRequest> pclRequestList, List<PatientPCLRequest> deletedPclRequestList)
        {
            if (pclRequestList == null)
            {
                return null;
            }

            List<PatientPCLRequest> newPclList = null;

            // Dieu Kien dau tien: Phai co Map <PCLExamTypeID, DeptLoc> duoc tao ra global roi

            // 1. Duyet qua tat ca PCLRequest de lay ra tat ca PCLExamTypes chua co Seq va chua thuc hien

            // 2. Khi lam Step 1 o tren luu lai Map<PCLReqID, DeptLoc> cho nhung PCLReq co ID > 0 de qua buoc 3 so sanh tim PCLReq co DeptLocID nhet them vo

            // 3. Duyet qua nhung PCLExamTypes moi chua co Seq:

            // a. Nhung PCLExamType nao co chi dinh DeptLoc neu tim duoc PCLReq nao co trung DeptLoc thi add vo
            // b. Con Nhung PCLExamType nao khong co chi dinh Deptloc thi dung Map (Dieu Kien o tren) de tim DeptLoc va neu add duoc thi add vo PCLReq co roi.

            // 4. Gom nhung PCLExamType khong the add vao PCLReq da co roi o tren step 3 goi ham GetSequenceNoForPclRequestDetailsList de lay Seq.

            // 5. Cuoi cung dung khuc code o duoi de phan bo vao nhung PCLReq moi se duoc tao ra.


            //Code
            //1
            List<PatientPCLRequestDetail> lstPCLDetailAddOn_temp = new List<PatientPCLRequestDetail>();
            foreach (var itemdetail in pclRequestList.SelectMany(item => item.PatientPCLRequestIndicators))
            {
                lstPCLDetailAddOn_temp.Add(itemdetail);
            }

            //cho nay co j do sai ne!
            //cac PCLExamType chua co SeqNum va chua thuc hien
            var lstNotYetSeqNum = (from c in lstPCLDetailAddOn_temp
                                   where c.PCLReqItemID <= 0 && c.ServiceSeqNum <= 0 && c.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.HOAN_TAT
                                   select c).ToList();
            //1

            //2
            var lstPCLReqExists = (from c in pclRequestList
                                   where c.PatientPCLReqID > 0
                                   select c).ToList();
            //2

            //3
            List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum = new List<PatientPCLRequestDetail>();
            List<PatientPCLRequestDetail> lstDetail_CreateNew = new List<PatientPCLRequestDetail>();
            Split2List(lstPCLReqExists, lstNotYetSeqNum, out lstDetail_ReUseServiceSeqNum, out lstDetail_CreateNew);

            CurrentRegistration.lstDetail_ReUseServiceSeqNum = new ObservableCollection<PatientPCLRequestDetail>(lstDetail_ReUseServiceSeqNum);
            CalcInvoiceItems(CurrentRegistration.lstDetail_ReUseServiceSeqNum);
            //3

            //4
            GetSequenceNoForPclRequestDetailsList(lstDetail_CreateNew);

            newPclList = SplitVote_Ngtr(lstDetail_CreateNew);
            //4


            return newPclList;

        }


        private List<PatientPCLRequest> SplitPclItemIntoRequests(List<PatientPCLRequest> listExistingPCLRequests, List<PatientPCLRequestDetail> lstDetail_CreateNew)
        {
            List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum = new List<PatientPCLRequestDetail>();

            // 1. First Assign DeptLocation for new PCL Items

            Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc = DataProviderBase.MAPPCLExamTypeDeptLoc;
            foreach (var pclItemDetail in lstDetail_CreateNew)
            {
                if (pclItemDetail.DeptLocation != null && pclItemDetail.DeptLocation.DeptLocationID > 0)
                {
                    continue;
                }

                List<DeptLocation> lstDeptLoc = MAPPCLExamTypeDeptLoc[pclItemDetail.PCLExamType.PCLExamTypeID].ObjDeptLocationList.ToList();
                bool bFoundExistingItemWithSameDeptLoc = false;
                foreach (var dept in lstDeptLoc)
                {
                    PatientPCLRequest PCLReqFind = null;

                    foreach (var p in listExistingPCLRequests)
                    {
                        if (dept.DeptLocationID == p.DeptLocID)
                        {
                            PatientPCLRequestDetail item = p.PatientPCLRequestIndicators != null ? p.PatientPCLRequestIndicators.FirstOrDefault() : null;

                            if (item != null && pclItemDetail.PCLExamType.HITTypeID == item.PCLExamType.HITTypeID && pclItemDetail.PCLExamType.V_PCLMainCategory == pclItemDetail.PCLExamType.V_PCLMainCategory)
                            {
                                PCLReqFind = p;
                                bFoundExistingItemWithSameDeptLoc = true;
                                break;
                            }
                        }
                    }
                    if (bFoundExistingItemWithSameDeptLoc)
                    {
                        pclItemDetail.PatientPCLReqID = PCLReqFind.PatientPCLReqID;

                        DeptLocation DL = new DeptLocation();
                        DL.DeptLocationID = PCLReqFind.DeptLocID;
                        pclItemDetail.DeptLocation = DL;
                        pclItemDetail.ServiceSeqNum = PCLReqFind.ServiceSeqNum;
                        pclItemDetail.ServiceSeqNumType = PCLReqFind.ServiceSeqNumType;

                        lstDetail_ReUseServiceSeqNum.Add(pclItemDetail);
                        break;
                    }

                }
            }

            var requests = new List<PatientPCLRequest>(); //new Dictionary<long ,PatientPCLRequest>();
            foreach (var reqDetails in lstDetail_CreateNew)
            {
                if (reqDetails.DeptLocation == null)
                {
                    reqDetails.DeptLocation = new DeptLocation();
                }
                bool exists = false;
                foreach (PatientPCLRequest item in requests)
                {
                    exists = false;
                    if (item.PCLDeptLocID == reqDetails.DeptLocation.DeptLocationID)
                    {
                        if (item.PatientPCLRequestIndicators != null)
                        {
                            foreach (var row in item.PatientPCLRequestIndicators)
                            {
                                //neu = nhau thi tach phieu
                                if (row.PCLExamType.PCLExamTypeID == reqDetails.PCLExamType.PCLExamTypeID)
                                {
                                    exists = true;
                                    break;
                                }
                            }
                            if (exists)
                            {
                                exists = false;
                            }
                            else
                            {
                                //o day chua kiem tra de tach theo phong
                                if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.V_PCLMainCategory == reqDetails.PCLExamType.V_PCLMainCategory
                                    && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HITTypeID == reqDetails.PCLExamType.HITTypeID
                                    && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == reqDetails.PCLExamType.IsExternalExam)
                                {
                                    if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == true)
                                    {
                                        if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HosIDofExternalExam == reqDetails.PCLExamType.HosIDofExternalExam)
                                        {
                                            item.PatientPCLRequestIndicators.Add(reqDetails);
                                            exists = true;
                                        }
                                    }
                                    else
                                    {
                                        item.PatientPCLRequestIndicators.Add(reqDetails);
                                        exists = true;
                                    }
                                }
                            }
                        }
                    }
                    if (exists)
                    {
                        break;
                    }
                }
                if (!exists)
                {
                    var newRequest = new PatientPCLRequest();
                    newRequest.StaffID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.StaffID : 0;
                    newRequest.DoctorStaffID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DoctorStaffID : 0;
                    newRequest.Diagnosis = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.Diagnosis : "";
                    newRequest.ICD10List = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ICD10List : "";

                    newRequest.DoctorComments = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DoctorComments : "";
                    newRequest.DeptID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DeptID : 0;
                    newRequest.PCLDeptLocID = reqDetails.PatientPCLRequest != null ? reqDetails.DeptLocation.DeptLocationID : 0;// reqDetails.PatientPCLRequest.PCLDeptLocID;
                    newRequest.ReqFromDeptLocID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ReqFromDeptLocID : 0;
                    newRequest.RequestedDoctorName = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.RequestedDoctorName : "";
                    newRequest.ServiceRecID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ServiceRecID : 0;

                    //cho nay sai trai ne ExamRegStatus=0??--xem lai cho nay
                    //newRequest.V_PCLRequestStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatus : 0;
                    newRequest.V_PCLRequestStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatus : AllLookupValues.V_PCLRequestStatus.OPEN;

                    //newRequest.V_PCLRequestType = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestType : 0;
                    newRequest.V_PCLRequestType = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestType : AllLookupValues.V_PCLRequestType.NGOAI_TRU;

                    newRequest.V_PCLRequestStatusName = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatusName : "";
                    //newRequest.ExamRegStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ExamRegStatus : 0;
                    newRequest.ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;

                    newRequest.RecordState = RecordState.DETACHED;//reqDetails.PatientPCLRequest.RecordState;
                    newRequest.EntityState = EntityState.NEW;//reqDetails.PatientPCLRequest.EntityState;
                    newRequest.CreatedDate = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.CreatedDate : DateTime.Now;
                    newRequest.ExamDate = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ExamDate : DateTime.Now;

                    newRequest.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                    newRequest.PatientPCLRequestIndicators.Add(reqDetails);

                    newRequest.V_PCLMainCategory = reqDetails.PCLExamType.V_PCLMainCategory;
                    newRequest.IsExternalExam = reqDetails.PCLExamType.IsExternalExam;
                    newRequest.AgencyID = reqDetails.PCLExamType.HosIDofExternalExam;
                    requests.Add(newRequest);
                    //requests.Add(reqDetails.DeptLocation.DeptLocationID, newRequest);
                }

            }
            return requests;
        }


        //ny create
        private List<PatientPCLRequest> SplitVote_Ngtr(List<PatientPCLRequestDetail> lstDetail_CreateNew)
        {
            var requests = new List<PatientPCLRequest>(); //new Dictionary<long ,PatientPCLRequest>();
            foreach (var reqDetails in lstDetail_CreateNew)
            {
                if (reqDetails.DeptLocation == null)
                {
                    reqDetails.DeptLocation = new DeptLocation();
                }
                bool exists = false;
                foreach (PatientPCLRequest item in requests)
                {
                    exists = false;
                    if (item.PCLDeptLocID == reqDetails.DeptLocation.DeptLocationID)
                    {
                        if (item.PatientPCLRequestIndicators != null)
                        {
                            foreach (var row in item.PatientPCLRequestIndicators)
                            {
                                //neu = nhau thi tach phieu
                                if (row.PCLExamType.PCLExamTypeID == reqDetails.PCLExamType.PCLExamTypeID)
                                {
                                    exists = true;
                                    break;
                                }
                            }
                            if (exists)
                            {
                                exists = false;
                            }
                            else
                            {
                                //o day chua kiem tra de tach theo phong
                                if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.V_PCLMainCategory == reqDetails.PCLExamType.V_PCLMainCategory
                                    && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HITTypeID == reqDetails.PCLExamType.HITTypeID
                                    && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == reqDetails.PCLExamType.IsExternalExam)
                                {
                                    if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == true)
                                    {
                                        if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HosIDofExternalExam == reqDetails.PCLExamType.HosIDofExternalExam)
                                        {
                                            item.PatientPCLRequestIndicators.Add(reqDetails);
                                            exists = true;
                                        }
                                    }
                                    else
                                    {
                                        item.PatientPCLRequestIndicators.Add(reqDetails);
                                        exists = true;
                                    }
                                }
                            }
                        }
                    }
                    if (exists)
                    {
                        break;
                    }
                }
                if (!exists)
                {
                    var newRequest = new PatientPCLRequest();
                    newRequest.StaffID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.StaffID : 0;
                    newRequest.DoctorStaffID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DoctorStaffID : 0;
                    newRequest.Diagnosis = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.Diagnosis : "";
                    newRequest.ICD10List = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ICD10List : "";

                    newRequest.DoctorComments = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DoctorComments : "";
                    newRequest.DeptID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DeptID : 0;
                    newRequest.PCLDeptLocID = reqDetails.PatientPCLRequest != null ? reqDetails.DeptLocation.DeptLocationID : 0;// reqDetails.PatientPCLRequest.PCLDeptLocID;
                    newRequest.ReqFromDeptLocID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ReqFromDeptLocID : 0;
                    newRequest.RequestedDoctorName = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.RequestedDoctorName : "";
                    newRequest.ServiceRecID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ServiceRecID : 0;

                    //cho nay sai trai ne ExamRegStatus=0??--xem lai cho nay
                    //newRequest.V_PCLRequestStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatus : 0;
                    newRequest.V_PCLRequestStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatus : AllLookupValues.V_PCLRequestStatus.OPEN;

                    //newRequest.V_PCLRequestType = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestType : 0;
                    newRequest.V_PCLRequestType = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestType : AllLookupValues.V_PCLRequestType.NGOAI_TRU;

                    newRequest.V_PCLRequestStatusName = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatusName : "";
                    //newRequest.ExamRegStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ExamRegStatus : 0;
                    newRequest.ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;

                    newRequest.RecordState = RecordState.DETACHED;//reqDetails.PatientPCLRequest.RecordState;
                    newRequest.EntityState = EntityState.NEW;//reqDetails.PatientPCLRequest.EntityState;
                    newRequest.CreatedDate = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.CreatedDate : DateTime.Now;
                    newRequest.ExamDate = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ExamDate : DateTime.Now;

                    newRequest.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                    newRequest.PatientPCLRequestIndicators.Add(reqDetails);

                    newRequest.V_PCLMainCategory = reqDetails.PCLExamType.V_PCLMainCategory;
                    newRequest.IsExternalExam = reqDetails.PCLExamType.IsExternalExam;
                    newRequest.AgencyID = reqDetails.PCLExamType.HosIDofExternalExam;
                    requests.Add(newRequest);
                    //requests.Add(reqDetails.DeptLocation.DeptLocationID, newRequest);
                }

            }
            return requests;
        }

        private List<PatientPCLRequest> SplitVote(List<PatientPCLRequestDetail> lstDetail_CreateNew)
        {
            var requests = new List<PatientPCLRequest>(); //new Dictionary<long ,PatientPCLRequest>();
            Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc = DataProviderBase.MAPPCLExamTypeDeptLoc;

            foreach (var reqDetails in lstDetail_CreateNew)
            {
                reqDetails.ObjDeptLocIDList = MAPPCLExamTypeDeptLoc[reqDetails.PCLExamType.PCLExamTypeID].ObjDeptLocationList;

                if (reqDetails.DeptLocation == null)
                {
                    reqDetails.DeptLocation = new DeptLocation();
                }

                bool exists = false;
                foreach (PatientPCLRequest item in requests)
                {
                    exists = false;
                    if (item.PCLDeptLocID == reqDetails.DeptLocation.DeptLocationID && item.PCLDeptLocID.GetValueOrDefault(0) > 0)
                    {
                        //neu duoc chon phong ngay tu dau
                        exists = SplitVoteCondition(reqDetails, item);
                    }
                    else
                    {
                        PatientPCLRequestDetail OldItem = item.PatientPCLRequestIndicators.FirstOrDefault();
                        if (OldItem.ObjDeptLocIDList != null && OldItem.ObjDeptLocIDList.Count > 0 && reqDetails.ObjDeptLocIDList != null && reqDetails.ObjDeptLocIDList.Count > 0)
                        {
                            foreach (var item1 in OldItem.ObjDeptLocIDList)
                            {
                                foreach (var detail1 in reqDetails.ObjDeptLocIDList)
                                {
                                    if (detail1.DeptLocationID == item1.DeptLocationID)
                                    {

                                        exists = true;
                                        break;
                                    }
                                }
                                if (exists)
                                {
                                    //da ton tai,thi dua vao nhung dk khac de nhu V_Maincatelogy and HITypeID de tach phieu
                                    exists = SplitVoteCondition(reqDetails, item);
                                    break;
                                }
                            }
                        }
                    }

                    if (exists)
                    {
                        break;
                    }
                }
                if (!exists)
                {
                    var newRequest = new PatientPCLRequest();
                    newRequest.StaffID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.StaffID : 0;
                    newRequest.DoctorStaffID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DoctorStaffID : 0;
                    newRequest.Diagnosis = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.Diagnosis : "";

                    newRequest.DoctorComments = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DoctorComments : "";
                    newRequest.DeptID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DeptID : 0;
                    newRequest.PCLDeptLocID = reqDetails.DeptLocation != null ? reqDetails.DeptLocation.DeptLocationID : 0;// reqDetails.PatientPCLRequest.PCLDeptLocID;
                    newRequest.ReqFromDeptLocID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ReqFromDeptLocID : 0;
                    newRequest.RequestedDoctorName = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.RequestedDoctorName : "";
                    newRequest.ServiceRecID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ServiceRecID : 0;

                    newRequest.V_PCLRequestStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatus : 0;
                    newRequest.V_PCLRequestType = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestType : 0;
                    newRequest.V_PCLRequestStatusName = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatusName : "";
                    newRequest.ExamRegStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ExamRegStatus : 0;
                    newRequest.RecordState = RecordState.DETACHED;//reqDetails.PatientPCLRequest.RecordState;
                    newRequest.EntityState = EntityState.NEW;//reqDetails.PatientPCLRequest.EntityState;
                    newRequest.CreatedDate = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.CreatedDate : DateTime.Now;
                    newRequest.ExamDate = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ExamDate : DateTime.Now;

                    newRequest.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                    newRequest.PatientPCLRequestIndicators.Add(reqDetails);
                    newRequest.V_PCLMainCategory = reqDetails.PCLExamType.V_PCLMainCategory;
                    newRequest.IsExternalExam = reqDetails.PCLExamType.IsExternalExam;
                    newRequest.AgencyID = reqDetails.PCLExamType.HosIDofExternalExam;
                    requests.Add(newRequest);
                    //requests.Add(reqDetails.DeptLocation.DeptLocationID, newRequest);
                }

            }
            return requests;
        }

        private bool SplitVoteCondition(PatientPCLRequestDetail reqDetails, PatientPCLRequest item)
        {
            bool exists = false;
            if (item.PatientPCLRequestIndicators != null)
            {
                foreach (var row in item.PatientPCLRequestIndicators)
                {
                    //neu = nhau thi tach phieu
                    if (row.PCLExamType.PCLExamTypeID == reqDetails.PCLExamType.PCLExamTypeID)
                    {
                        exists = true;
                        break;
                    }
                }
                if (exists)
                {
                    exists = false;
                }
                else
                {
                    //o day chua kiem tra de tach theo phong
                    if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.V_PCLMainCategory == reqDetails.PCLExamType.V_PCLMainCategory
                        && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HITTypeID == reqDetails.PCLExamType.HITTypeID
                        && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == reqDetails.PCLExamType.IsExternalExam)
                    {
                        if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == true)
                        {
                            if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HosIDofExternalExam == reqDetails.PCLExamType.HosIDofExternalExam)
                            {
                                item.PatientPCLRequestIndicators.Add(reqDetails);
                                exists = true;
                            }
                        }
                        else
                        {
                            item.PatientPCLRequestIndicators.Add(reqDetails);
                            exists = true;
                        }
                    }
                }
            }
            return exists;
        }

        #region Cac ham cho Update dang ky CLS
        private void Split2List_Bang_Old(List<PatientPCLRequest> lstPCLReqExists
            , List<PatientPCLRequestDetail> lstNotYetSeqNum
            , out List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum
            , out List<PatientPCLRequestDetail> lstDetail_CreateNew)
        {
            lstDetail_ReUseServiceSeqNum = new List<PatientPCLRequestDetail>();
            lstDetail_CreateNew = new List<PatientPCLRequestDetail>();

            foreach (var detail in lstNotYetSeqNum)
            {
                if (detail.DeptLocation != null && detail.DeptLocation.DeptLocationID > 0)
                {
                    bool bfind = false;

                    //Xet co phieu nao co phong truoc do chua add vo. 
                    foreach (var p in lstPCLReqExists)
                    {
                        if (detail.DeptLocation.DeptLocationID == p.DeptLocID)
                        {
                            detail.PatientPCLReqID = p.PatientPCLReqID;

                            DeptLocation DL = new DeptLocation();
                            DL.DeptLocationID = p.DeptLocID;
                            detail.DeptLocation = DL;
                            detail.CreatedDate = DateTime.Now;
                            detail.ServiceSeqNum = p.ServiceSeqNum;
                            detail.ServiceSeqNumType = p.ServiceSeqNumType;

                            lstDetail_ReUseServiceSeqNum.Add(detail);
                            bfind = true;
                        }
                    }
                    if (bfind == false)/*Co chi dinh phong, ma phong nay tim khong thay trong cac phieu truoc thi se duoc tao phieu moi*/
                    {
                        lstDetail_CreateNew.Add(detail);
                    }
                }
                else//Doc ds phong ra, coi co phieu nao co phong truoc do chua 
                {
                    Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc = DataProviderBase.MAPPCLExamTypeDeptLoc;

                    List<DeptLocation> lstDeptLoc = MAPPCLExamTypeDeptLoc[detail.PCLExamType.PCLExamTypeID].ObjDeptLocationList.ToList();

                    bool bfind = false;

                    foreach (var dept in lstDeptLoc)
                    {
                        PatientPCLRequest PCLReqFind = null;

                        foreach (var p in lstPCLReqExists)
                        {
                            if (dept.DeptLocationID == p.DeptLocID)
                            {
                                PCLReqFind = p;
                                bfind = true;
                                break;
                            }
                        }
                        if (bfind)
                        {
                            detail.PatientPCLReqID = PCLReqFind.PatientPCLReqID;

                            DeptLocation DL = new DeptLocation();
                            DL.DeptLocationID = PCLReqFind.DeptLocID;
                            detail.DeptLocation = DL;
                            detail.ServiceSeqNum = PCLReqFind.ServiceSeqNum;
                            detail.ServiceSeqNumType = PCLReqFind.ServiceSeqNumType;

                            lstDetail_ReUseServiceSeqNum.Add(detail);
                            break;
                        }

                    }
                    if (bfind == false)
                    {
                        lstDetail_CreateNew.Add(detail);
                    }
                }
            }
        }

        private void Split2List(List<PatientPCLRequest> lstPCLReqExists
           , List<PatientPCLRequestDetail> lstNotYetSeqNum
           , out List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum
           , out List<PatientPCLRequestDetail> lstDetail_CreateNew)
        {
            lstDetail_ReUseServiceSeqNum = new List<PatientPCLRequestDetail>();
            lstDetail_CreateNew = new List<PatientPCLRequestDetail>();

            foreach (var detail in lstNotYetSeqNum)
            {
                if (detail.DeptLocation != null && detail.DeptLocation.DeptLocationID > 0)
                {
                    bool bfind = false;

                    //Xet co phieu nao co phong truoc do chua add vo. 
                    foreach (var p in lstPCLReqExists)
                    {
                        if (detail.DeptLocation.DeptLocationID == p.DeptLocID)
                        {
                            PatientPCLRequestDetail item = p.PatientPCLRequestIndicators != null ? p.PatientPCLRequestIndicators.FirstOrDefault() : null;

                            if (item != null && detail.PCLExamType.HITTypeID == item.PCLExamType.HITTypeID && detail.PCLExamType.V_PCLMainCategory == item.PCLExamType.V_PCLMainCategory)
                            {
                                detail.PatientPCLReqID = p.PatientPCLReqID;

                                DeptLocation DL = new DeptLocation();
                                DL.DeptLocationID = p.DeptLocID;
                                detail.DeptLocation = DL;
                                detail.CreatedDate = DateTime.Now;
                                detail.ServiceSeqNum = p.ServiceSeqNum;
                                detail.ServiceSeqNumType = p.ServiceSeqNumType;

                                lstDetail_ReUseServiceSeqNum.Add(detail);
                                bfind = true;
                                break;//bbd: dinh them o day neu khong no se tiep tuc them vao danh sach nua
                            }
                        }
                    }
                    if (bfind == false)/*Co chi dinh phong, ma phong nay tim khong thay trong cac phieu truoc thi se duoc tao phieu moi*/
                    {
                        lstDetail_CreateNew.Add(detail);
                    }
                }
                else//Doc ds phong ra, coi co phieu nao co phong truoc do chua 
                {
                    Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc = DataProviderBase.MAPPCLExamTypeDeptLoc;

                    List<DeptLocation> lstDeptLoc = MAPPCLExamTypeDeptLoc[detail.PCLExamType.PCLExamTypeID].ObjDeptLocationList.ToList();

                    bool bfind = false;

                    foreach (var dept in lstDeptLoc)
                    {
                        PatientPCLRequest PCLReqFind = null;

                        foreach (var p in lstPCLReqExists)
                        {
                            if (dept.DeptLocationID == p.DeptLocID)
                            {
                                PatientPCLRequestDetail item = p.PatientPCLRequestIndicators != null ? p.PatientPCLRequestIndicators.FirstOrDefault() : null;

                                if (item != null && detail.PCLExamType.HITTypeID == item.PCLExamType.HITTypeID && detail.PCLExamType.V_PCLMainCategory == detail.PCLExamType.V_PCLMainCategory)
                                {
                                    PCLReqFind = p;
                                    bfind = true;
                                    break;
                                }
                            }
                        }
                        if (bfind)
                        {
                            detail.PatientPCLReqID = PCLReqFind.PatientPCLReqID;

                            DeptLocation DL = new DeptLocation();
                            DL.DeptLocationID = PCLReqFind.DeptLocID;
                            detail.DeptLocation = DL;
                            detail.ServiceSeqNum = PCLReqFind.ServiceSeqNum;
                            detail.ServiceSeqNumType = PCLReqFind.ServiceSeqNumType;

                            lstDetail_ReUseServiceSeqNum.Add(detail);
                            break;
                        }

                    }
                    if (bfind == false)
                    {
                        lstDetail_CreateNew.Add(detail);
                    }
                }
            }
        }
        #endregion


        protected virtual void GetItemPrice(IInvoiceItem invoiceItem, PatientRegistration registration)
        {
            if (invoiceItem.ID <= 0)
            {
                if (invoiceItem.ChargeableItem != null)
                {
                    invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.NormalPrice;
                }
            }

            invoiceItem.PriceDifference = 0;
            invoiceItem.HIPayment = 0;
            invoiceItem.PatientCoPayment = 0;
            invoiceItem.PatientPayment = invoiceItem.InvoicePrice;
        }

        protected virtual void AddRegistrationDetailsToCurrentRegistration(List<PatientRegistrationDetail> regDetailList)
        {
            if (regDetailList == null || regDetailList.Count == 0)
            {
                return;
            }
            if (CurrentRegistration.PatientRegistrationDetails == null)
            {
                CurrentRegistration.PatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
            }
            foreach (var invoiceItem in regDetailList)
            {
                if (invoiceItem.RecordState == RecordState.DETACHED
                    || invoiceItem.RecordState == RecordState.ADDED)
                {
                    GetItemPrice(invoiceItem, CurrentRegistration);
                    GetItemTotalPrice(invoiceItem);

                    CurrentRegistration.PatientRegistrationDetails.Add(invoiceItem);
                }
                else
                {
                    bool bExists = false;
                    //Kiem tra neu chua co trong dang ky thi add, neu co roi thi gan lai.
                    for (int i = 0; i < CurrentRegistration.PatientRegistrationDetails.Count; i++)
                    {
                        if (invoiceItem.PtRegDetailID > 0 && CurrentRegistration.PatientRegistrationDetails[i].PtRegDetailID == invoiceItem.PtRegDetailID)
                        {
                            CurrentRegistration.PatientRegistrationDetails[i] = invoiceItem;
                            bExists = true;
                            break;
                        }
                    }
                    if (!bExists)
                    {
                        CurrentRegistration.PatientRegistrationDetails.Add(invoiceItem);
                    }
                }
            }
        }
        protected void AddPclRequestsToCurrentRegistration(List<PatientPCLRequest> pclRequestList, long staffid = 0)
        {
            if (pclRequestList == null || pclRequestList.Count == 0)
            {
                return;
            }
            if (CurrentRegistration.PCLRequests == null)
            {
                CurrentRegistration.PCLRequests = new ObservableCollection<PatientPCLRequest>();
            }
            foreach (var patientPCLRequest in pclRequestList)
            {
                if (patientPCLRequest.RecordState == RecordState.DETACHED
                    || patientPCLRequest.RecordState == RecordState.ADDED
                    )//|| patientPCLRequest.RecordState == RecordState.MODIFIED
                {
                    CalcInvoiceItems(patientPCLRequest.PatientPCLRequestIndicators);
                }

                if (patientPCLRequest.PatientPCLReqID <= 0)
                {
                    patientPCLRequest.PatientServiceRecord = CreateNewServiceRecord(patientPCLRequest.ExamDate);
                    patientPCLRequest.PatientServiceRecord.StaffID = patientPCLRequest.StaffID;
                    patientPCLRequest.PCLRequestNumID = new ServiceSequenceNumberProvider().GetCode_PCLRequest(patientPCLRequest.V_PCLMainCategory);
                    CurrentRegistration.PCLRequests.Add(patientPCLRequest);
                }
                else
                {
                    //Kiem tra neu chua co trong dang ky thi add, neu co roi thi gan lai.
                    for (int i = 0; i < CurrentRegistration.PCLRequests.Count; i++)
                    {
                        if (CurrentRegistration.PCLRequests[i].PatientPCLReqID == patientPCLRequest.PatientPCLReqID)
                        {
                            //Lay nhung PCLRequestDetail bi thay doi, kiem tra phong co thay doi khong
                            //Neu co thay doi phong thi sau nay se xin cap so lai.
                            if (patientPCLRequest.PatientPCLRequestIndicators != null)
                            {
                                foreach (var reqDetail in patientPCLRequest.PatientPCLRequestIndicators.Where(item => item.RecordState == RecordState.MODIFIED))
                                {
                                    var persistedReqDetail = CurrentRegistration.PCLRequests[i].PatientPCLRequestIndicators.Where(item => item.PCLReqItemID == reqDetail.PCLReqItemID).FirstOrDefault();
                                    if (persistedReqDetail == null) continue;
                                    if (persistedReqDetail.DeptLocation != null
                                        && reqDetail.DeptLocation != null
                                        && persistedReqDetail.DeptLocation.DeptLocationID != reqDetail.DeptLocation.DeptLocationID)
                                    {
                                        reqDetail.LocationChanged = true;
                                    }
                                    if (staffid > 0 && (reqDetail.StaffID == null || reqDetail.StaffID.Value < 1))
                                    {
                                        reqDetail.StaffID = staffid;
                                    }
                                }
                            }
                            CurrentRegistration.PCLRequests[i] = patientPCLRequest;
                            break;
                        }
                    }
                }

            }
        }

        protected void AddOutwardDrugInvoicesToCurrentRegistration(List<OutwardDrugInvoice> drugInvoices)
        {
            if (drugInvoices == null || drugInvoices.Count == 0)
            {
                return;
            }
            if (CurrentRegistration.DrugInvoices == null)
            {
                CurrentRegistration.DrugInvoices = new ObservableCollection<OutwardDrugInvoice>();
            }
            foreach (var inv in drugInvoices)
            {
                if (inv.RecordState == RecordState.DETACHED
                    || inv.RecordState == RecordState.ADDED
                    || inv.RecordState == RecordState.MODIFIED)
                {
                    if (inv.OutwardDrugs != null)
                    {
                        foreach (var item in inv.OutwardDrugs)
                        {
                            item.CreatedDate = inv.OutDate;
                        }
                        CalcInvoiceItems(inv.OutwardDrugs);
                    }
                }
                if (inv.outiID <= 0)
                {
                    CurrentRegistration.DrugInvoices.Add(inv);
                }
                else
                {
                    bool bExists = false;
                    if (inv.ReturnID == null)//Phieu xuat
                    {
                        //Kiem tra neu chua co trong dang ky thi add, neu co roi thi gan lai.
                        for (int i = 0; i < CurrentRegistration.DrugInvoices.Count; i++)
                        {
                            if (CurrentRegistration.DrugInvoices[i].outiID == inv.outiID)
                            {
                                CurrentRegistration.DrugInvoices[i] = inv;
                                bExists = true;
                                break;
                            }
                        }
                        if (!bExists)
                        {
                            CurrentRegistration.DrugInvoices.Add(inv);
                        }
                    }
                    else //Phieu tra
                    {
                        //Kiem tra neu chua co trong dang ky thi add, neu co roi thi gan lai.
                        foreach (var invoice in CurrentRegistration.DrugInvoices)// (int i = 0; i < CurrentRegistration.DrugInvoices.Count; i++)
                        {
                            if (invoice.ReturnedInvoices != null)
                            {
                                //Kiem tra neu chua co trong dang ky thi add, neu co roi thi gan lai.
                                for (var i = 0; i < invoice.ReturnedInvoices.Count; i++)
                                {
                                    if (invoice.ReturnedInvoices[i].outiID == inv.outiID)
                                    {
                                        invoice.ReturnedInvoices[i] = inv;
                                        bExists = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!bExists)
                        {
                            CurrentRegistration.DrugInvoices.Add(inv);
                        }
                    }
                }
            }
        }

        protected void AddInPatientBillingInvoicesToCurrentRegistration(List<InPatientBillingInvoice> billingInvoices)
        {
            if (billingInvoices == null || billingInvoices.Count == 0)
            {
                return;
            }
            if (CurrentRegistration.InPatientBillingInvoices == null)
            {
                CurrentRegistration.InPatientBillingInvoices = new ObservableCollection<InPatientBillingInvoice>();
            }
            foreach (var inv in billingInvoices)
            {
                if (inv.RegistrationDetails != null)
                {
                    CalcInvoiceItems(inv.RegistrationDetails.Where(item => item.ID <= 0));
                }

                if (inv.PclRequests != null)
                {
                    foreach (PatientPCLRequest row in inv.PclRequests)
                    {
                        if (row.PatientPCLRequestIndicators != null)
                        {
                            foreach (PatientPCLRequestDetail itemrow in row.PatientPCLRequestIndicators)
                            {
                                itemrow.PatientPCLRequest = row;
                            }
                        }
                    }
                    List<PatientPCLRequestDetail> newLstRequestDetails = inv.PclRequests.SelectMany(request => request.PatientPCLRequestIndicators).ToList();
                    var newLstRequests = SplitVote(newLstRequestDetails);

                    inv.PclRequests = newLstRequests.ToObservableCollection();
                    CalcInvoiceItems(inv.PclRequests.SelectMany(request => request.PatientPCLRequestIndicators)
                        .Where(item => item.ID <= 0));
                }
                if (inv.OutwardDrugClinicDeptInvoices != null)
                {
                    CalcInvoiceItems(inv.OutwardDrugClinicDeptInvoices.SelectMany(invoice => invoice.OutwardDrugClinicDepts)
                        .Where(item => item.OutID <= 0));
                }

                if (inv.InPatientBillingInvID <= 0)
                {
                    CurrentRegistration.InPatientBillingInvoices.Add(inv);
                }
                else
                {
                    bool bExists = false;
                    //Kiem tra neu chua co trong dang ky thi add, neu co roi thi gan lai.
                    for (int i = 0; i < CurrentRegistration.InPatientBillingInvoices.Count; i++)
                    {
                        if (CurrentRegistration.InPatientBillingInvoices[i].InPatientBillingInvID == inv.InPatientBillingInvID)
                        {
                            CurrentRegistration.InPatientBillingInvoices[i] = inv;
                            bExists = true;
                            break;
                        }
                    }
                    if (!bExists)
                    {
                        CurrentRegistration.InPatientBillingInvoices.Add(inv);
                    }
                }
            }
        }

        private PatientServiceRecord CreateNewServiceRecord(DateTime? serviceRecDate)
        {
            //Tao 1 PatientServiceRecord.
            var serviceRecord = new PatientServiceRecord { StaffID = null };
            if (!serviceRecDate.HasValue || serviceRecDate == DateTime.MinValue)
            {
                serviceRecord.ExamDate = DateTime.Now;
            }
            else
            {
                serviceRecord.ExamDate = serviceRecDate.Value;
            }
            serviceRecord.V_Behaving = (long)AllLookupValues.Behaving.CHI_DINH_XET_NGHIEM_CLS;
            serviceRecord.V_ProcessingType = (long)AllLookupValues.ProcessingType.PARA_CLINICAL_EXAMINATION;

            return serviceRecord;
        }
        protected void CreatePatientMedialRecordIfNotExists(long patientID, DateTime medicalRecordDate, out long patientMedicalRecordID
            , out long? medicalFileID)
        {
            patientMedicalRecordID = -1;
            long medicalRecordID;

            if (!PatientProvider.Instance.CheckIfMedicalRecordExists(patientID, out medicalRecordID, out medicalFileID))
            {
                var medicalRecord = new PatientMedicalRecord
                                        {
                                            PatientID = patientID,
                                            NationalMedicalCode = "MedicalCode",
                                            CreatedDate =
                                                medicalRecordDate == DateTime.MinValue
                                                    ? DateTime.Now
                                                    : medicalRecordDate
                                        };

                var bCreateMROK = PatientProvider.Instance.AddNewPatientMedicalRecord(medicalRecord, out medicalRecordID);
                if (!bCreateMROK)
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1780_G1_CannotCreatePatient));
                }
            }
            patientMedicalRecordID = medicalRecordID;
        }
        protected void CalcInvoiceItems(IEnumerable<IInvoiceItem> colInvoiceItems)
        {
            foreach (var item in colInvoiceItems)
            {
                GetItemPrice(item, CurrentRegistration);
                GetItemTotalPrice(item);
            }
        }

        public virtual void RemovePaidRegItems(long StaffID,long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration registration,
                                                List<PatientRegistrationDetail> colPaidRegDetails,
                                                List<PatientPCLRequest> colPaidPclRequests,
                                                List<OutwardDrugInvoice> colPaidDrugInvoice,
                                                List<OutwardDrugClinicDeptInvoice> colPaidMedItemList,
                                                List<OutwardDrugClinicDeptInvoice> colPaidChemicalItem, DateTime removeDate = default(DateTime))
        {
            if (removeDate == default(DateTime))
            {
                removeDate = DateTime.Now;
            }
            AddRegistrationDetailsToCurrentRegistration(colPaidRegDetails);
            AddPclRequestsToCurrentRegistration(colPaidPclRequests);
            long newRegistrationID;
            List<long> newRegistrationDetailIdList;
            List<long> newPclRequestIdList;
            List<long> newPaymentIDList;
            List<long> newOutwardDrugIDList;
            List<long> billingInvoiceIDs;
            string PaymentIDListNy;
            SaveRegistrationForOutPatient(StaffID,CollectorDeptLocID, Apply15HIPercent, removeDate, out newRegistrationID, out newRegistrationDetailIdList
                                    , out newPclRequestIdList, out newPaymentIDList, out newOutwardDrugIDList, out billingInvoiceIDs, out PaymentIDListNy);
        }

        public virtual void PayForRegistrationNewOnlyDrugReturn(long StaffID,long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration registrationInfo,
                                              PatientTransactionPayment paymentDetails,//PatientPayment paymentDetails,
                                              List<PatientRegistrationDetail> colPaidRegDetails,
                                              List<PatientPCLRequest> colPaidPclRequests,
                                              List<OutwardDrugInvoice> colPaidDrugInvoice,
                                              List<InPatientBillingInvoice> colBillingInvoices,
                                              out PatientTransaction transaction, out PatientTransactionPayment paymentInfo)//out PatientPayment paymentInfo
        {

            paymentInfo = null;
            transaction = CurrentRegistration.PatientTransaction;

            DateTime paidTime = DateTime.Now; ;

            //khong can vi khi tra thuoc chua co hoan tien gi het.chi luu vay thoi
            ////cap nhat ngay tra tien hoac hoan tien cho cac dich vu --dua vao trang thai cua no
            //UpdatePaidOrRefundTime(paidTime, colPaidRegDetails, colPaidPclRequests, colPaidDrugInvoice, colBillingInvoices);

            ////kiem tra chi tiet dang ki neu co roi thi gan lai,neu chua co thi add vao
            //AddRegistrationDetailsToCurrentRegistration(colPaidRegDetails);
            ////kiem tra cls neu co roi thi gan lai,neu chua co thi add vao
            //AddPclRequestsToCurrentRegistration(colPaidPclRequests);

            //kiem tra phieu xuat thuoc,neu co roi thi gan lai,neu chua co thi add vao
            //AddOutwardDrugInvoicesToCurrentRegistration(colPaidDrugInvoice);

            long newRegistrationID;
            List<long> newRegistrationIDList;
            List<long> newPclRequestList;
            List<long> newPaymentIDList;
            List<long> newOutwardDrugIDList;
            List<long> billingInvoiceIDs;
            string PaymentIDListNy;
            SaveRegistrationForOutPatient(StaffID,CollectorDeptLocID, Apply15HIPercent, paidTime, out newRegistrationID, out newRegistrationIDList, out newPclRequestList, out newPaymentIDList, out newOutwardDrugIDList, out billingInvoiceIDs, out PaymentIDListNy);
        }


        public virtual void PayForRegistration(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration registrationInfo,
                                                PatientTransactionPayment paymentDetails,//PatientPayment paymentDetails,
                                                List<PatientRegistrationDetail> colPaidRegDetails,
                                                List<PatientPCLRequest> colPaidPclRequests,
                                                List<OutwardDrugInvoice> colPaidDrugInvoice,
                                                List<InPatientBillingInvoice> colBillingInvoices,
                                                out PatientTransaction transaction, out PatientTransactionPayment paymentInfo, out List<PaymentAndReceipt> paymentInfoList)//out PatientPayment paymentInfo
        {
            string paymentErrorMessage;
            var isValid = ValidatePaymentInfo(paymentDetails, out paymentErrorMessage);
            paymentInfo = null;
            paymentInfoList = null;
            transaction = CurrentRegistration.PatientTransaction;
            if (!isValid)
            {
                throw new Exception(paymentErrorMessage);
            }

            DateTime paidTime;
            if (!paymentDetails.PaymentDate.HasValue || paymentDetails.PaymentDate.Value == DateTime.MinValue)
            {
                paidTime = DateTime.Now;
            }
            else
            {
                paidTime = paymentDetails.PaymentDate.Value;
            }

            //cap nhat ngay tra tien hoac hoan tien cho cac dich vu --dua vao trang thai cua no
            UpdatePaidOrRefundTime(paidTime, colPaidRegDetails, colPaidPclRequests, colPaidDrugInvoice, colBillingInvoices);

            //kiem tra chi tiet dang ki neu co roi thi gan lai,neu chua co thi add vao
            AddRegistrationDetailsToCurrentRegistration(colPaidRegDetails);
            //kiem tra cls neu co roi thi gan lai,neu chua co thi add vao
            AddPclRequestsToCurrentRegistration(colPaidPclRequests);
            //kiem tra phieu xuat thuoc,neu co roi thi gan lai,neu chua co thi add vao
            AddOutwardDrugInvoicesToCurrentRegistration(colPaidDrugInvoice);

            //Neu chua co Transaction cua dang ki nay thi tao moi,neu co roi thi thoi
            //Sau do add payment vao transaction nay
            ProcessPayment(paymentDetails);
            paymentInfo = paymentDetails;

            long newRegistrationID;
            List<long> newRegistrationIDList;
            List<long> newPclRequestList;
            List<long> newPaymentIDList;
            List<long> newOutwardDrugIDList;
            List<long> billingInvoiceIDs;
            string PaymentIDListNy;
            SaveRegistrationForOutPatient(StaffID, CollectorDeptLocID,Apply15HIPercent, paidTime, out newRegistrationID, out newRegistrationIDList, out newPclRequestList, out newPaymentIDList, out newOutwardDrugIDList, out billingInvoiceIDs, out PaymentIDListNy);

            List<PaymentAndReceipt> ListPaymentIDAndService = null;
            if (!string.IsNullOrEmpty(PaymentIDListNy))
            {
                XDocument xdoc1 = XDocument.Parse(PaymentIDListNy);
                ListPaymentIDAndService = (from _student in xdoc1.Element("Root").Elements("IDList")
                                           orderby Convert.ToInt64(_student.Element("PaymentID").Value)
                                           select new PaymentAndReceipt
                                           {
                                               PaymentID = Convert.ToInt64(_student.Element("PaymentID").Value),
                                               ServiceID = Convert.ToInt64(_student.Element("ServiceID").Value),
                                               ServiceItemType = Convert.ToInt64(_student.Element("ServiceTypeID").Value),
                                               V_TradingPlaces = Convert.ToInt64(_student.Element("V_TradingPlaces").Value),
                                               IsBalance = Convert.ToInt16(_student.Element("IsBalance").Value),
                                               Amount = Convert.ToDecimal(_student.Element("Amount").Value)
                                           }).ToList();

            }

            paymentInfoList = ListPaymentIDAndService;
            //Thuc ra chi co 1 payment thoi.
            if (newPaymentIDList != null && newPaymentIDList.Count > 0)
            {
                paymentInfo.PtTranPaymtID = newPaymentIDList[0];
                transaction = CurrentRegistration.PatientTransaction;
            }

            //Lay nhung thang dang ky kham benh, cls vua tinh tien, add vao bang ReportOutPatientCashReceipt
            if (CurrentRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU
                //|| CurrentRegistration.V_RegistrationType == AllLookupValues.RegistrationType.DANGKY_VIP
                )
            {

                var cashReceiptItems = new List<ReportOutPatientCashReceipt>();
                if (Globals.AxServerSettings.CommonItems.RefundOrCancelCashReceipt == 1)
                {
                    AddReportOutPatientCashReceiptCancel(paymentInfo, ListPaymentIDAndService, cashReceiptItems);
                }
                else
                {
                    AddReportOutPatientCashReceiptRefund(paymentInfo, ListPaymentIDAndService, cashReceiptItems);
                }

                try
                {
                    if (cashReceiptItems != null && cashReceiptItems.Count > 0)
                    {
                        PatientProvider.Instance.AddReportOutPatientCashReceiptList(cashReceiptItems);
                    }
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                }
            }
        }
        private void AddReportOutPatientCashReceiptRefund(PatientTransactionPayment paymentInfo, List<PaymentAndReceipt> ListPaymentIDAndService, List<ReportOutPatientCashReceipt> cashReceiptItems)
        {
            if (CurrentRegistration.PatientRegistrationDetails != null)
            {
                foreach (var source in CurrentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.MODIFIED))
                {
                    if (ListPaymentIDAndService != null && ListPaymentIDAndService.Count > 0)
                    {
                        foreach (var item in ListPaymentIDAndService)
                        {
                            if (source.PtRegDetailID == item.ServiceID && item.ServiceItemType == (long)AllLookupValues.ServiceItemType.CHI_TIET_KCB && item.IsBalance == 0)
                            {
                                var cashReportItem = new ReportOutPatientCashReceipt();
                                cashReportItem.PaymentID = item.PaymentID;
                                cashReportItem.ItemID = source.PtRegDetailID;
                                cashReportItem.ServiceItemType = (long)AllLookupValues.ServiceItemType.CHI_TIET_KCB;
                                cashReportItem.ServiceName = source.ChargeableItemName;
                                if (!string.IsNullOrEmpty(source.SpecialNote))
                                {
                                    cashReportItem.ServiceName += " - " + source.SpecialNote;
                                }
                                cashReportItem.PatientID = CurrentRegistration.Patient.PatientID;
                                cashReportItem.PtRegistrationID = CurrentRegistration.PtRegistrationID;
                                cashReportItem.DeptLocID = (source.DeptLocation != null && source.DeptLocation.Location != null) ? source.DeptLocation.Location.LID : 0;
                                cashReportItem.ServiceSeqNum = source.ServiceSeqNum;
                                cashReportItem.ServiceSeqNumType = source.ServiceSeqNumType;
                                if (source.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                                {
                                    cashReportItem.Amount = -source.TotalInvoicePrice;
                                    cashReportItem.PatientAmount = -(source.TotalInvoicePrice - source.TotalHIPayment);

                                }
                                else
                                {
                                    cashReportItem.Amount = source.TotalInvoicePrice;
                                    cashReportItem.PatientAmount = (source.TotalInvoicePrice - source.TotalHIPayment);
                                }

                                cashReceiptItems.Add(cashReportItem);
                                break;
                            }
                        }
                    }
                }
            }
            if (CurrentRegistration.PCLRequests != null)
            {
                foreach (var source in CurrentRegistration.PCLRequests.SelectMany(item => item.PatientPCLRequestIndicators).Where(obj => obj.RecordState == RecordState.MODIFIED))
                {
                    if (ListPaymentIDAndService != null && ListPaymentIDAndService.Count > 0)
                    {
                        foreach (var item in ListPaymentIDAndService)
                        {
                            if (source.PatientPCLReqID == item.ServiceID && item.ServiceItemType == (long)AllLookupValues.ServiceItemType.CHI_TIET_CLS && item.IsBalance == 0)
                            {
                                var cashReportItem = new ReportOutPatientCashReceipt();
                                cashReportItem.PaymentID = item.PaymentID;
                                cashReportItem.ItemID = source.PCLReqItemID;
                                cashReportItem.ServiceItemType = (long)AllLookupValues.ServiceItemType.CHI_TIET_CLS;
                                cashReportItem.ServiceName = source.ChargeableItemName;
                                cashReportItem.PatientID = CurrentRegistration.Patient.PatientID;
                                cashReportItem.PtRegistrationID = CurrentRegistration.PtRegistrationID;
                                cashReportItem.DeptLocID = (source.DeptLocation != null && source.DeptLocation.Location != null) ? source.DeptLocation.Location.LID : 0;
                                cashReportItem.ServiceSeqNum = source.ServiceSeqNum;
                                cashReportItem.ServiceSeqNumType = source.ServiceSeqNumType;
                                if (source.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                                {
                                    cashReportItem.Amount = -source.TotalInvoicePrice;
                                    cashReportItem.PatientAmount = -(source.TotalInvoicePrice - source.TotalHIPayment);

                                }
                                else
                                {
                                    cashReportItem.Amount = source.TotalInvoicePrice;
                                    cashReportItem.PatientAmount = (source.TotalInvoicePrice - source.TotalHIPayment);
                                }

                                cashReceiptItems.Add(cashReportItem);
                                break;
                            }
                        }
                    }
                }
            }

            //nhung dong duoc can bang se dc luu o day
            if (ListPaymentIDAndService != null && ListPaymentIDAndService.Count > 0)
            {
                var items = ListPaymentIDAndService.Where(x => x.IsBalance == 1);
                if (items != null && items.Count() > 0)
                {
                    foreach (var ite in items)
                    {
                        var cashReportItem = new ReportOutPatientCashReceipt();
                        cashReportItem.PaymentID = ite.PaymentID;
                        cashReportItem.ItemID = ite.ServiceID;
                        cashReportItem.ServiceItemType = ite.ServiceItemType;
                        cashReportItem.ServiceName = "Can Bang";
                        cashReportItem.PatientID = CurrentRegistration.Patient.PatientID;
                        cashReportItem.PtRegistrationID = CurrentRegistration.PtRegistrationID;
                        cashReportItem.Amount = ite.Amount;
                        cashReportItem.PatientAmount = ite.Amount;
                        cashReceiptItems.Add(cashReportItem);
                    }
                }
            }
        }

        private void AddReportOutPatientCashReceiptCancel(PatientTransactionPayment paymentInfo, List<PaymentAndReceipt> ListPaymentIDAndService, List<ReportOutPatientCashReceipt> cashReceiptItems)
        {
            if (CurrentRegistration.PatientRegistrationDetails != null)
            {
                foreach (var source in CurrentRegistration.PatientRegistrationDetails.Where(item => item.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI))
                {
                    if (ListPaymentIDAndService != null && ListPaymentIDAndService.Count > 0)
                    {
                        foreach (var item in ListPaymentIDAndService)
                        {
                            if (source.PtRegDetailID == item.ServiceID && item.ServiceItemType == (long)AllLookupValues.ServiceItemType.CHI_TIET_KCB && item.IsBalance == 0)
                            {
                                var cashReportItem = new ReportOutPatientCashReceipt();
                                cashReportItem.PaymentID = item.PaymentID;
                                cashReportItem.ItemID = source.PtRegDetailID;
                                cashReportItem.ServiceItemType = (long)AllLookupValues.ServiceItemType.CHI_TIET_KCB;
                                cashReportItem.ServiceName = source.ChargeableItemName;
                                if (!string.IsNullOrEmpty(source.SpecialNote))
                                {
                                    cashReportItem.ServiceName += " - " + source.SpecialNote;
                                }
                                
                                cashReportItem.PatientID = CurrentRegistration.Patient.PatientID;
                                cashReportItem.PtRegistrationID = CurrentRegistration.PtRegistrationID;
                                cashReportItem.DeptLocID = (source.DeptLocation != null && source.DeptLocation.Location != null) ? source.DeptLocation.Location.LID : 0;
                                cashReportItem.ServiceSeqNum = source.ServiceSeqNum;
                                cashReportItem.ServiceSeqNumType = source.ServiceSeqNumType;
                                if (source.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                                {
                                    cashReportItem.Amount = -source.TotalInvoicePrice;
                                    cashReportItem.PatientAmount = -(source.TotalInvoicePrice - source.TotalHIPayment);

                                }
                                else
                                {
                                    cashReportItem.Amount = source.TotalInvoicePrice;
                                    cashReportItem.PatientAmount = (source.TotalInvoicePrice - source.TotalHIPayment);
                                }

                                cashReceiptItems.Add(cashReportItem);
                                break;
                            }
                        }
                    }
                }
            }
            if (CurrentRegistration.PCLRequests != null)
            {
                foreach (var source in CurrentRegistration.PCLRequests.SelectMany(item => item.PatientPCLRequestIndicators).Where(obj => obj.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI))
                {
                    if (ListPaymentIDAndService != null && ListPaymentIDAndService.Count > 0)
                    {
                        foreach (var item in ListPaymentIDAndService)
                        {
                            if (source.PatientPCLReqID == item.ServiceID && item.ServiceItemType == (long)AllLookupValues.ServiceItemType.CHI_TIET_CLS && item.IsBalance == 0)
                            {
                                var cashReportItem = new ReportOutPatientCashReceipt();
                                cashReportItem.PaymentID = item.PaymentID;
                                cashReportItem.ItemID = source.PCLReqItemID;
                                cashReportItem.ServiceItemType = (long)AllLookupValues.ServiceItemType.CHI_TIET_CLS;
                                cashReportItem.ServiceName = source.ChargeableItemName;
                                cashReportItem.PatientID = CurrentRegistration.Patient.PatientID;
                                cashReportItem.PtRegistrationID = CurrentRegistration.PtRegistrationID;
                                cashReportItem.DeptLocID = (source.DeptLocation != null && source.DeptLocation.Location != null) ? source.DeptLocation.Location.LID : 0;
                                cashReportItem.ServiceSeqNum = source.ServiceSeqNum;
                                cashReportItem.ServiceSeqNumType = source.ServiceSeqNumType;
                                if (source.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                                {
                                    cashReportItem.Amount = -source.TotalInvoicePrice;
                                    cashReportItem.PatientAmount = -(source.TotalInvoicePrice - source.TotalHIPayment);

                                }
                                else
                                {
                                    cashReportItem.Amount = source.TotalInvoicePrice;
                                    cashReportItem.PatientAmount = (source.TotalInvoicePrice - source.TotalHIPayment);
                                }

                                cashReceiptItems.Add(cashReportItem);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Kiem tra thong tin tinh tien co dung hay khong.
        /// </summary>
        /// <param name="paymentDetails"></param>
        /// <param name="errorMessage">Cau thong bao neu kiem tra thong tin khong dung</param>
        /// <returns></returns>
        public abstract bool ValidatePaymentInfo(PatientTransactionPayment paymentDetails, out string errorMessage);//PatientPayment paymentDetails, 

        protected PatientTransaction CreateTransationForRegistration(PatientRegistration registrationInfo)
        {
            var tran = new PatientTransaction
                           {
                               PtRegistrationID = registrationInfo.PtRegistrationID,
                               TransactionBeginDate = registrationInfo.ExamDate,
                               V_RegistrationType = registrationInfo.V_RegistrationType,
                               PatientTransactionDetails = new List<PatientTransactionDetail>().ToObservableCollection()
                           };


            return tran;
        }
        /// <summary>
        /// Bat dau can ban transaction. Danh cho thang nao muon override.
        /// </summary>
        protected virtual void OnTransactionBalancing(int Apply15HIPercent)
        {

        }
        protected virtual void BalanceTransaction(List<PatientRegistrationDetail> colRegDetails
                                        , List<PatientPCLRequest> colPclRequests
                                        , List<OutwardDrugInvoice> colDrugInvoice
                                        , List<InPatientBillingInvoice> colBillingInvoices, DateTime balanceDate)
        {
            if (balanceDate == DateTime.MinValue)
            {
                balanceDate = DateTime.Now;
            }
            // OnTransactionBalancing();//khong can goi ham nay vi tren kia da tinh lai roi,goi nua se thanh goi 2 lan va so lieu ra sai
            if (CurrentRegistration.PatientTransaction == null)
            {
                CurrentRegistration.PatientTransaction = CreateTransationForRegistration(CurrentRegistration);
            }
            if (CurrentRegistration.PatientTransaction.PatientTransactionDetails == null)
            {
                CurrentRegistration.PatientTransaction.PatientTransactionDetails = new ObservableCollection<PatientTransactionDetail>();
            }

            if (colRegDetails != null)
            {
                CurrentRegistration.PatientTransaction.PatientTransactionDetails.AddRange(colRegDetails.Where(item => item.PtRegDetailID > 0 && item.PaidTime != null).Select(item => BalanceTransactionOfService(CurrentRegistration.PatientTransaction, item, balanceDate)).Where(tranDetail => tranDetail != null));
            }

            if (colPclRequests != null)
            {
                CurrentRegistration.PatientTransaction.PatientTransactionDetails.AddRange(colPclRequests.Where(item => item.PatientPCLReqID > 0 && item.PaidTime != null).Select(item => BalanceTransactionOfPclRequest(CurrentRegistration.PatientTransaction, item, balanceDate)).Where(tranDetail => tranDetail != null));
            }

            if (colDrugInvoice != null)
            {
                CurrentRegistration.PatientTransaction.PatientTransactionDetails.AddRange(colDrugInvoice.Where(item => (item.outiID > 0 && item.PaidTime != null)).Select(item => BalanceTransactionOfDrugInvoice(CurrentRegistration.PatientTransaction, item, balanceDate)).Where(tranDetail => tranDetail != null));
            }

            if (colBillingInvoices != null)
            {
                CurrentRegistration.PatientTransaction.PatientTransactionDetails.AddRange(colBillingInvoices.Where(item => item.InPatientBillingInvID > 0 && item.PaidTime != null).Select(item => BalanceTransactionOfBillingInvoice(CurrentRegistration.PatientTransaction, item)).Where(tranDetail => tranDetail != null));
            }
        }
        private void AddPaymentToTransaction(PatientTransaction tran, PatientTransactionPayment payment)//, PatientPayment payment
        {
            if (tran == null)
            {
                return;
            }
            //if (tran.PatientPayments == null)
            //{
            //    tran.PatientPayments = new ObservableCollection<PatientPayment>();
            //}
            // tran.PatientPayments.Add(payment);
            if (tran.PatientTransactionPayments == null)
            {
                tran.PatientTransactionPayments = new ObservableCollection<PatientTransactionPayment>();
            }
            tran.PatientTransactionPayments.Add(payment);
        }

        protected void ProcessPayment(PatientTransactionPayment paymentDetails)//PatientPayment paymentDetails
        {
            if (CurrentRegistration.PatientTransaction == null)
            {
                CurrentRegistration.PatientTransaction = CreateTransationForRegistration(CurrentRegistration);
            }
            AddPaymentToTransaction(CurrentRegistration.PatientTransaction, paymentDetails);
        }

        /// <summary>
        /// Cập nhật PaidTime cho những item muốn tính tiền
        /// </summary>
        protected void UpdatePaidOrRefundTime(DateTime paidTime,
                                        List<PatientRegistrationDetail> colPaidRegDetails,
                                        List<PatientPCLRequest> colPaidPclRequests,
                                        List<OutwardDrugInvoice> colPaidDrugInvoice,
                                        List<InPatientBillingInvoice> colBillingInvoices)
        {
            //Cap nhat Ngay Tra Tien hoac Ngay Hoan Tien cho danh sach Dich Vu.
            if (colPaidRegDetails != null)
            {
                foreach (var details in colPaidRegDetails)
                {
                    if (details.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    {
                        if (details.RefundTime == null)
                        {
                            details.RefundTime = paidTime;
                            UpdateInvoiceItemState(details, RecordState.UNCHANGED, RecordState.MODIFIED);
                            continue;
                        }
                    }
                    else
                    {
                        if (details.PaidTime == null)
                        {
                            details.PaidTime = paidTime;
                            UpdateInvoiceItemState(details, RecordState.UNCHANGED, RecordState.MODIFIED);
                            continue;
                        }
                    }
                }
            }
            //Cap nhat Ngay Tra Tien hoac Ngay Hoan Tien cho danh sach CLS
            if (colPaidPclRequests != null)
            {
                foreach (var details in colPaidPclRequests)
                {
                    //if (details.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    if (details.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CANCEL)
                    {
                        if (details.RefundTime == null)
                        {
                            details.RefundTime = paidTime;
                            UpdateInvoiceItemState(details, RecordState.UNCHANGED, RecordState.MODIFIED);
                            if (details.PatientPCLRequestIndicators != null)
                            {
                                foreach (var item in details.PatientPCLRequestIndicators)
                                {
                                    if (item.RefundTime == null)
                                    {
                                        item.RefundTime = paidTime;
                                        UpdateInvoiceItemState(item, RecordState.UNCHANGED, RecordState.MODIFIED);
                                    }
                                }
                            }

                            continue;
                        }
                    }
                    else
                    {
                        if (details.PaidTime == null)
                        {
                            details.PaidTime = paidTime;
                            UpdateInvoiceItemState(details, RecordState.UNCHANGED, RecordState.MODIFIED);
                            if (details.PatientPCLRequestIndicators != null)
                            {
                                foreach (var item in details.PatientPCLRequestIndicators)
                                {
                                    if (item.PaidTime == null)
                                    {
                                        item.PaidTime = paidTime;
                                        UpdateInvoiceItemState(item, RecordState.UNCHANGED, RecordState.MODIFIED);
                                    }
                                }
                            }

                            continue;
                        }
                        if (details.PatientPCLRequestIndicators != null)
                        {
                            UpdateInvoiceItemState(details, RecordState.UNCHANGED, RecordState.MODIFIED);
                            foreach (var item in details.PatientPCLRequestIndicators)
                            {
                                if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && item.RefundTime == null)
                                {
                                    item.RefundTime = paidTime;
                                    UpdateInvoiceItemState(item, RecordState.UNCHANGED, RecordState.MODIFIED);
                                }
                            }
                        }
                    }
                }
            }
            if (colPaidDrugInvoice != null)
            {
                foreach (var invoice in colPaidDrugInvoice)
                {
                    if (invoice.ReturnID == null) //Phieu xuat binh thuong
                    {
                        if (invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)
                        {
                            if (invoice.PaidTime == null)
                            {
                                invoice.PaidTime = paidTime;
                                UpdateInvoiceItemState(invoice, RecordState.UNCHANGED, RecordState.MODIFIED);
                            }
                        }
                        else if (invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                        {
                            if (invoice.RefundTime == null)
                            {
                                invoice.RefundTime = paidTime;
                                UpdateInvoiceItemState(invoice, RecordState.UNCHANGED, RecordState.MODIFIED);
                            }
                        }
                    }
                    else
                    {
                        if (invoice.PaidTime == null)
                        {
                            invoice.PaidTime = paidTime;
                            UpdateInvoiceItemState(invoice, RecordState.UNCHANGED, RecordState.MODIFIED);
                        }
                    }
                }
            }

            if (colBillingInvoices != null)
            {
                foreach (var billingInvoice in colBillingInvoices)
                {
                    if (billingInvoice.V_InPatientBillingInvStatus == AllLookupValues.V_InPatientBillingInvStatus.NGUNG_TRA_TIEN_LAI)
                    {
                        if (billingInvoice.RefundTime == null)
                        {
                            billingInvoice.RefundTime = paidTime;
                            //UpdateInvoiceItemState(billingInvoice, RecordState.UNCHANGED, RecordState.MODIFIED);
                            continue;
                        }
                    }
                    else
                    {
                        if (billingInvoice.PaidTime == null)
                        {
                            billingInvoice.PaidTime = paidTime;
                            //UpdateInvoiceItemState(billingInvoice, RecordState.UNCHANGED, RecordState.MODIFIED);
                            continue;
                        }
                    }
                }
            }
        }



        public virtual bool AddOutwardDrugInvoice(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, OutwardDrugInvoice outwardInvoice, out List<long> newOutwardDrugIDList)
        {
            DateTime modifiedDate = DateTime.Now;

            if (outwardInvoice.OutDate == DateTime.MinValue || outwardInvoice.OutDate.Date.Subtract(modifiedDate.Date).Days == 0)
            {
                outwardInvoice.OutDate = modifiedDate;
            }
            else
            {
                modifiedDate = outwardInvoice.OutDate;
            }

            //Ben Ny gui len trang thai khong dung.(dang le phai la DETACHED)
            if (outwardInvoice.outiID <= 0)
            {
                outwardInvoice.RecordState = RecordState.DETACHED;
            }

            AddOutwardDrugInvoicesToCurrentRegistration(new List<OutwardDrugInvoice> { outwardInvoice });

            List<long> newRegistrationIDList;
            List<long> newPclRequestList;
            long newRegistrationID;
            List<long> newPaymentIDList;
            List<long> billingInvoiceIDs;
            string PaymentIDListNy;
            var bOK = SaveRegistrationForOutPatient(StaffID,CollectorDeptLocID, Apply15HIPercent, modifiedDate, out newRegistrationID, out newRegistrationIDList, out newPclRequestList, out newPaymentIDList, out newOutwardDrugIDList, out billingInvoiceIDs, out PaymentIDListNy);

            return bOK;
        }

        public virtual bool AddOutwardDrugReturn(long StaffID, int? Apply15HIPercent, OutwardDrugInvoice outward, List<OutwardDrug> details, out long outiID)
        {
            long returnedInvId;
            var bOK = RefDrugGenericDetailsProvider.Instance.AddOutWardDrugInvoiceReturnVisitor(outward, out returnedInvId);
            if (bOK)
            {
                var items = PatientProvider.Instance.GetDrugInvoiceListByRegistrationID(CurrentRegistration.PtRegistrationID);
                CurrentRegistration.DrugInvoices = items != null ? items.ToObservableCollection() : null;
            }
            outiID = returnedInvId;
            if (CurrentRegistration.DrugInvoices != null)
            {
                foreach (var invoice in CurrentRegistration.DrugInvoices)
                {
                    if (invoice.ReturnedInvoices != null)
                    {
                        var returnedInv = invoice.ReturnedInvoices.FirstOrDefault(item => item.outiID == returnedInvId);
                        if (returnedInv != null)
                        {
                            returnedInv.RecordState = RecordState.MODIFIED;//Danh dau cho biet se duoc can bang.
                            invoice.RecordState = RecordState.MODIFIED;
                        }
                    }
                }
            }

            BalanceTransaction(DateTime.Now);

            return bOK;

            //DateTime modifiedDate = DateTime.Now;
            ///*
            // * Ở đây Outward thực ra là phiếu xuất (Ny gui len nhu vay). không phải phiểu trả.
            // * 
            // * Như vậy phải tự tạo 1 phiếu trả tương ứng.
            // */
            //outiID = 0;
            //if (details == null || details.Count == 0)
            //{
            //    return false;
            //}
            //var ChiTietTras = details.Where(x => x.OutQuantityReturn > 0);
            //foreach (var outwardDrug in ChiTietTras)
            //{
            //    outwardDrug.TotalHIPayment = outwardDrug.TotalHIPayment * outwardDrug.OutQuantityReturn / (int)outwardDrug.Qty;
            //    outwardDrug.TotalInvoicePrice = outwardDrug.TotalInvoicePrice * outwardDrug.OutQuantityReturn / (int)outwardDrug.Qty;
            //    outwardDrug.TotalCoPayment = outwardDrug.TotalCoPayment * outwardDrug.OutQuantityReturn / (int)outwardDrug.Qty;

            //    //Tai vi day la chi tiet phieu xuat chu khong phai phieu tra.
            //    outwardDrug.Qty = outwardDrug.OutQuantityReturn;
            //}

            //var returnInv = new OutwardDrugInvoice
            //                    {
            //                        SelectedStorage = outward.SelectedStorage,
            //                        SelectedStaff = outward.SelectedStaff,
            //                        TypID = outward.TypID,
            //                        ReturnID = outward.outiID,
            //                        V_OutDrugInvStatus = outward.V_OutDrugInvStatus,
            //                        OutDate = DateTime.Now,
            //                        OutwardDrugs = ChiTietTras.ToObservableCollection(),
            //                        DrugInvoice = outward
            //                    };

            //if (outward.ReturnedInvoices == null)
            //{
            //    outward.ReturnedInvoices = new ObservableCollection<OutwardDrugInvoice>();
            //}
            //outward.ReturnedInvoices.Add(returnInv);

            //// AddOutwardDrugInvoicesToCurrentRegistration(new List<OutwardDrugInvoice> { outward, returnInv });
            ////Chi can luu phieu tra thoi,ko can cap nhat phieu xuat nua dau
            //AddOutwardDrugInvoicesToCurrentRegistration(new List<OutwardDrugInvoice> { returnInv });

            //List<long> newRegistrationIDList;
            //List<long> newPclRequestList;
            //long newRegistrationID;
            //List<long> newPaymentIDList;
            //List<long> newOutwardDrugIDList;
            //List<long> billingInvoiceIDs;
            //string PaymentIDListNy;
            //////Tinh chenh lech giua ngay mua thuoc va ngay tra thuoc:
            //var bOK = SaveRegistrationForOutPatient(StaffID, Apply15HIPercent, modifiedDate, out newRegistrationID, out newRegistrationIDList, out newPclRequestList, out newPaymentIDList, out newOutwardDrugIDList, out billingInvoiceIDs, out PaymentIDListNy);
            //if (newOutwardDrugIDList != null && newOutwardDrugIDList.Count > 0)
            //{
            //    outiID = newOutwardDrugIDList[0];
            //}
            //return bOK;
        }

        public virtual bool CancelOutwardDrugInvoice(long StaffID, long CollectorDeptLocID,int? Apply15HIPercent, OutwardDrugInvoice invoice, DateTime cancelledDate = default(DateTime))
        {
            if (cancelledDate == default(DateTime))
            {
                cancelledDate = DateTime.Now;
            }
            invoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.CANCELED;

            if (invoice.OutwardDrugs != null)
            {
                foreach (var drug in invoice.OutwardDrugs)
                {
                    drug.OutQuantityReturn = drug.OutQuantity;
                    drug.TotalPriceDifference = 0;
                    drug.TotalInvoicePrice = 0;
                    drug.TotalHIPayment = 0;
                    drug.TotalCoPayment = 0;
                    drug.TotalPatientPayment = 0;
                }
            }

            invoice.RecordState = RecordState.MODIFIED;
            AddOutwardDrugInvoicesToCurrentRegistration(new List<OutwardDrugInvoice> { invoice });

            List<long> newRegistrationIDList;
            List<long> newPclRequestList;
            long newRegistrationID;
            List<long> newPaymentIDList;
            List<long> newOutwardDrugIDList;
            List<long> billingInvoiceIDs;
            string PaymentIDListNy;
            var bOK = SaveRegistrationForOutPatient(StaffID,CollectorDeptLocID, Apply15HIPercent, cancelledDate, out newRegistrationID, out newRegistrationIDList, out newPclRequestList, out newPaymentIDList, out newOutwardDrugIDList, out billingInvoiceIDs, out PaymentIDListNy);

            return bOK;
        }

        public virtual void AddInPatientBillingInvoice(int? Apply15HIPercent, InPatientBillingInvoice billingInv, bool calcPaymentToEndOfDay, out Dictionary<long, List<long>> drugIDListError, out long newBillingInvoiceID)
        {
            newBillingInvoiceID = -1;
            drugIDListError = new Dictionary<long, List<long>>();

            if (billingInv.InvDate == DateTime.MinValue)
            {
                billingInv.InvDate = DateTime.Now;
            }

            if (billingInv.OutwardDrugClinicDeptInvoices != null)
            {
            }

            //List nay co chua thong tin tien giuong tinh rieng (truong hop nguoi dung load bill tien giuong co san).
            var newRegDetailsBedAlloc = new List<PatientRegistrationDetail>();
            if (billingInv.RegistrationDetails != null)
            {
                newRegDetailsBedAlloc = billingInv.RegistrationDetails.Where(item => item.ID <= 0 && item.BedPatientRegDetail != null).ToList();
            }
            //Danh sach newRegDetails co chua thong tin tien giuong.
            if (newRegDetailsBedAlloc.Count > 0)
            {
                foreach (var detail in newRegDetailsBedAlloc)
                {
                    if (calcPaymentToEndOfDay)
                    {
                        if (detail.BedPatientRegDetail.BillToDate.Date < DateTime.Now.Date)
                        {
                            detail.BedPatientRegDetail.BillToDate = DateTime.Now.Date.AddSeconds(86399);
                        }
                    }
                }
            }

            billingInv.BillingInvNum = GetInPatientBillingInvNumber();
            if (CurrentRegistration.StaffID.HasValue)
            {
                billingInv.StaffID = CurrentRegistration.StaffID.Value;
            }
            else if (CurrentRegistration.Staff != null)
            {
                billingInv.StaffID = CurrentRegistration.Staff.StaffID;
            }
            else
            {
                billingInv.StaffID = -1;
            }

            billingInv.PtRegistrationID = CurrentRegistration.PtRegistrationID;
            billingInv.V_InPatientBillingInvStatus = AllLookupValues.V_InPatientBillingInvStatus.NEW;

            AddInPatientBillingInvoicesToCurrentRegistration(new List<InPatientBillingInvoice> { billingInv });

            long newRegistrationID;
            List<long> newRegistrationDetailIdList;
            List<long> newPclRequestIdList;
            List<long> newPaymentIDList;
            List<long> newOutwardDrugIDList;
            List<long> billingInvoiceIDs;

            SaveRegistrationForInPatient(Apply15HIPercent, billingInv.InvDate, out newRegistrationID, out newRegistrationDetailIdList
                                    , out newPclRequestIdList, out newPaymentIDList, out newOutwardDrugIDList, out billingInvoiceIDs);
            if (billingInvoiceIDs != null && billingInvoiceIDs.Count > 0)
            {
                newBillingInvoiceID = billingInvoiceIDs[0];
            }
        }

        public void GetModifiedItemsOfInPatientRegistration(PatientRegistration registrationInfo
                                    , out List<InPatientBillingInvoice> newInvoiceList
                                    , out List<InPatientBillingInvoice> modifiedInvoiceList
                                    , out List<InPatientBillingInvoice> deletedInvoiceList)
        {
            if (CurrentRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
                throw new Exception("Hay chon dang ky noi tru");

            newInvoiceList = new List<InPatientBillingInvoice>();
            modifiedInvoiceList = new List<InPatientBillingInvoice>();
            deletedInvoiceList = new List<InPatientBillingInvoice>();

            if (registrationInfo.InPatientBillingInvoices != null)
            {
                foreach (var inv in registrationInfo.InPatientBillingInvoices)
                {
                    switch (inv.RecordState)
                    {
                        case RecordState.ADDED:
                        case RecordState.DETACHED:
                            newInvoiceList.Add(inv);
                            break;

                        case RecordState.MODIFIED:
                            modifiedInvoiceList.Add(inv);
                            break;

                        case RecordState.DELETED:
                            deletedInvoiceList.Add(inv);
                            break;
                    }
                }
            }
        }
        protected virtual bool UpdateInPatientRegistration(out List<long> newInPatientBillingInvoice, out List<long> newPaymentIDList)
        {
            //Kiem tra co PatientMedicalRecord chua. Neu chua co thi tao moi luon.
            long medicalRecordID;
            long? medicalFileID;
            CreatePatientMedialRecordIfNotExists(CurrentRegistration.Patient.PatientID, CurrentRegistration.ExamDate, out medicalRecordID, out medicalFileID);

            OnRegistrationSaving();

            List<InPatientBillingInvoice> newInvoiceList;
            List<InPatientBillingInvoice> modifiedInvoiceList;
            List<InPatientBillingInvoice> deletedInvoiceList;

            GetModifiedItemsOfInPatientRegistration(CurrentRegistration
                                    , out newInvoiceList
                                    , out modifiedInvoiceList
                                    , out deletedInvoiceList);

            long newPatientTransactionID;

            var xDocModifiedTran = ConvertModifiedTransactionToXml(CurrentRegistration.PatientTransaction, (int)AllLookupValues.V_FindPatientType.NOI_TRU);

            var bOK = PatientProvider.Instance.AddUpdateBillingInvoices(CurrentRegistration.PtRegistrationID
                                    , medicalRecordID
                                    , newInvoiceList
                                    , modifiedInvoiceList
                                    , deletedInvoiceList
                                    , xDocModifiedTran != null ? xDocModifiedTran.ToString() : null
                                    , (long)CurrentRegistration.V_RegistrationType
                                    , out newInPatientBillingInvoice
                                    , out newPatientTransactionID
                                    , out newPaymentIDList);
            return bOK;
        }

        public virtual void RecalcInPatientBillingInvoice(out string ListIDOutTranDetails, long? StaffID, PatientRegistration registrationInfo, InPatientBillingInvoice billingInv)
        {
            ListIDOutTranDetails = "";
            if (billingInv.InPatientBillingInvID <= 0)
            {
                throw new Exception(string.Format("{0}.",eHCMSResources.Z1702_G1_BillChuaCoID));
            }

            using (var conn = PatientProvider.Instance.CreateConnection())
            {
                conn.Open();

                var tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                try
                {
                    if (billingInv.OutwardDrugClinicDeptInvoices != null && billingInv.OutwardDrugClinicDeptInvoices.Count > 0)
                    {
                        foreach (var inv in billingInv.OutwardDrugClinicDeptInvoices)
                        {
                            if (inv.OutwardDrugClinicDepts != null)
                            {
                                foreach (var outwardDrug in inv.OutwardDrugClinicDepts)
                                {
                                    if (registrationInfo.PtInsuranceBenefit.GetValueOrDefault(0) > 0)
                                    {
                                        if (outwardDrug.GenMedProductItem != null && outwardDrug.GenMedProductItem.InsuranceCover == true)
                                        {
                                            outwardDrug.HiApplied = true;
                                            outwardDrug.HIBenefit = registrationInfo.PtInsuranceBenefit;
                                            if (outwardDrug.InwardDrugClinicDept != null)
                                            {
                                                outwardDrug.HIAllowedPrice = outwardDrug.InwardDrugClinicDept.HIAllowedPrice;
                                                outwardDrug.OutPrice = outwardDrug.InwardDrugClinicDept.HIPatientPrice;
                                            }
                                        }
                                    }
                                    outwardDrug.RecordState = RecordState.MODIFIED;
                                    //Tinh tien lai.
                                    GetItemPrice(outwardDrug, registrationInfo, 0.0);
                                    GetItemTotalPrice(outwardDrug);
                                }
                            }
                        }
                    }

                    if (billingInv.OutwardDrugClinicDeptInvoices != null && billingInv.OutwardDrugClinicDeptInvoices.Count > 0)
                    {
                        PatientProvider.Instance.UpdateOutwardDrugClinicDeptList(billingInv.OutwardDrugClinicDeptInvoices.SelectMany(item => item.OutwardDrugClinicDepts).ToList(), StaffID, conn, tran);
                    }

                    if (billingInv.RegistrationDetails != null && billingInv.RegistrationDetails.Count > 0)
                    {
                        foreach (PatientRegistrationDetail p in billingInv.RegistrationDetails)
                        {
                            p.HiApplied = true;
                            p.HIBenefit = registrationInfo.PtInsuranceBenefit.GetValueOrDefault(0);
                            p.HisID = registrationInfo.HisID;
                            p.HIAllowedPrice = p.ChargeableItem.HIAllowedPriceNew;
                            p.InvoicePrice = p.ChargeableItem.HIPatientPriceNew;
                        }
                        CalcInvoiceItems(billingInv.RegistrationDetails, registrationInfo);
                        PatientProvider.Instance.UpdateRegistrationDetails(billingInv.RegistrationDetails, (int)AllLookupValues.V_FindPatientType.NOI_TRU, conn, tran);
                    }
                    if (billingInv.PclRequests != null && billingInv.PclRequests.Count > 0)
                    {
                        var requestDetailList = billingInv.PclRequests.SelectMany(item => item.PatientPCLRequestIndicators).ToList();
                        foreach (PatientPCLRequestDetail p in requestDetailList)
                        {
                            p.HiApplied = true;
                            p.HIBenefit = registrationInfo.PtInsuranceBenefit.GetValueOrDefault(0);
                            p.HisID = registrationInfo.HisID;
                            p.HIAllowedPrice = p.ChargeableItem.HIAllowedPriceNew;
                            p.InvoicePrice = p.ChargeableItem.HIPatientPriceNew;
                        }
                        CalcInvoiceItems(requestDetailList, registrationInfo);
                        PatientProvider.Instance.UpdatePCLRequestDetailList(requestDetailList, conn, tran);
                    }

                    if (billingInv.PaidTime != null)
                    {
                        if (registrationInfo.PatientTransaction != null)
                        {
                            decimal TotalInvoicePrice = 0;
                            decimal TotalPriceDifference = 0;
                            decimal TotalCoPayment = 0;
                            decimal TotalHIPayment = 0;

                            if (billingInv.RegistrationDetails != null)
                            {
                                foreach (var item in billingInv.RegistrationDetails)
                                {
                                    TotalInvoicePrice += item.TotalInvoicePrice;
                                    TotalPriceDifference += item.TotalPriceDifference;
                                    TotalCoPayment += item.TotalCoPayment;
                                    TotalHIPayment += item.TotalHIPayment;
                                }
                            }

                            if (billingInv.PclRequests != null)
                            {
                                foreach (var item in billingInv.PclRequests.SelectMany(request => request.PatientPCLRequestIndicators))
                                {
                                    TotalInvoicePrice += item.TotalInvoicePrice;
                                    TotalPriceDifference += item.TotalPriceDifference;
                                    TotalCoPayment += item.TotalCoPayment;
                                    TotalHIPayment += item.TotalHIPayment;
                                }
                            }

                            if (billingInv.OutwardDrugClinicDeptInvoices != null)
                            {
                                foreach (var item in billingInv.OutwardDrugClinicDeptInvoices.SelectMany(inv => inv.OutwardDrugClinicDepts))
                                {
                                    TotalInvoicePrice += item.TotalInvoicePrice;
                                    TotalPriceDifference += item.TotalPriceDifference;
                                    TotalCoPayment += item.TotalCoPayment;
                                    TotalHIPayment += item.TotalHIPayment;
                                }
                            }

                            billingInv.TotalInvoicePrice = TotalInvoicePrice;
                            billingInv.TotalPriceDifference = TotalPriceDifference;
                            billingInv.TotalCoPayment = TotalCoPayment;
                            billingInv.TotalHIPayment = TotalHIPayment;

                            var balancedTranDetails = new List<PatientTransactionDetail>();
                            registrationInfo.PatientTransaction.PatientTransactionDetails = PatientProvider.Instance.GetAlltransactionDetails(registrationInfo.PatientTransaction.TransactionID).ToObservableCollection();
                            var tranDetail = BalanceTransactionOfBillingInvoice(registrationInfo.PatientTransaction, billingInv);

                            if (tranDetail != null)
                            {
                                balancedTranDetails.Add(tranDetail);
                            }

                            if (balancedTranDetails.Count > 0)
                            {
                                XDocument xmlDocument = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
                                  new XElement("Root",
                                  from item in balancedTranDetails
                                  select new XElement("IDList",
                                  new XElement("ID", item.TranRefID))));

                                string BillingInvoices = xmlDocument.ToString();
                                PatientProvider.Instance.GetBalanceInPatientBillingInvoice(StaffID.GetValueOrDefault(0), registrationInfo.PtRegistrationID, (long)registrationInfo.V_RegistrationType, BillingInvoices, conn, tran);

                                // PatientProvider.Instance.AddTransactionDetailList(StaffID.GetValueOrDefault(), registrationInfo.PatientTransaction.TransactionID, balancedTranDetails, out ListIDOutTranDetails, conn, tran);
                            }
                        }
                    }
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
    }

    public class RegAndPaymentProcessorFactory
    {
        public static RegAndPaymentProcessorBase GetPaymentProcessor(PaymentProcessorType type)
        {
            switch (type)
            {
                case PaymentProcessorType.HealthInsurance:
                    return new HiPatientRegAndPaymentProcessor();
                default: return new NormalPatientRegAndPaymentProcessor();
            }
        }

        public static RegAndPaymentProcessorBase GetPaymentProcessor(PatientRegistration registration)
        {
            if (registration.HealthInsurance != null)
            {
                return GetPaymentProcessor(PaymentProcessorType.HealthInsurance);
            }
            return GetPaymentProcessor(PaymentProcessorType.Normal);
        }
    }
}

//private bool checkExistDept(List<PatientPCLRequest> PCLRequests) 
//{
//    foreach (PatientPCLRequest item in PCLRequests)
//    {
//        if (item.PCLDeptLocID == reqDetails.DeptLocation.DeptLocationID)
//            return true;
//    }
//    return false;
//}
/*
private List<PatientPCLRequest> SplitPCLRequest(List<PatientPCLRequestDetail> lstDetail_CreateNew)
{
    var requests = new List<PatientPCLRequest>(); //new Dictionary<long ,PatientPCLRequest>();
    foreach (var reqDetails in lstDetail_CreateNew)
    {
        if (reqDetails.DeptLocation == null)
        {
            reqDetails.DeptLocation = new DeptLocation();
        }
        bool exists = false;
        foreach (PatientPCLRequest item in requests)
        {
            exists = false;
            if (item.PCLDeptLocID == reqDetails.DeptLocation.DeptLocationID)
            {
                if (item.PatientPCLRequestIndicators != null)
                {
                    foreach (var row in item.PatientPCLRequestIndicators)
                    {
                        //neu = nhau thi tach phieu
                        if (row.PCLExamType.PCLExamTypeID == reqDetails.PCLExamType.PCLExamTypeID)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (exists)
                    {
                        exists = false;
                    }
                    else
                    {
                        //o day chua kiem tra de tach theo phong
                        if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.V_PCLMainCategory == reqDetails.PCLExamType.V_PCLMainCategory
                            && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HITTypeID == reqDetails.PCLExamType.HITTypeID
                            && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == reqDetails.PCLExamType.IsExternalExam)
                        {
                            if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == true)
                            {
                                if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HosIDofExternalExam == reqDetails.PCLExamType.HosIDofExternalExam)
                                {
                                    item.PatientPCLRequestIndicators.Add(reqDetails);
                                    exists = true;
                                }
                            }
                            else
                            {
                                item.PatientPCLRequestIndicators.Add(reqDetails);
                                exists = true;
                            }
                        }
                    }
                }
            }
            if (exists)
            {
                break;
            }
        }
               
    }
    return requests;
}
*/