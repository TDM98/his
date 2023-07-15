using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DataEntities;
using eHCMS.DAL;
using eHCMS.Services.Core;
using System.Data.Common;
using System;
using System.Transactions;
using System.Linq;
using EventManagementService;

namespace CommonServices
{
    public class RegAndPaymentProcessor
    {
        private PatientRegistration _RegistrationInfo;
        public PatientRegistration RegistrationInfo
        {
            get
            {
                return _RegistrationInfo;
            }
            protected set
            {
                _RegistrationInfo = value;
            }
        }
        private long _RegistrationID;
        public RegAndPaymentProcessor(long RegistrationID)
        {
            this._RegistrationID = RegistrationID;

            Init();
        }
        public RegAndPaymentProcessor(PatientRegistration registrationInfo)
        {
            this._RegistrationID = registrationInfo.PtRegistrationID;
            if (registrationInfo.PtRegistrationID <= 0)
            {
                this._RegistrationInfo = registrationInfo;
            }
            else
            {
                Init(); 
            }
        }
        public void Init()
        {
            if (_RegistrationID <= 0)
            {
                _RegistrationInfo = new PatientRegistration();
                return;
            }

            _RegistrationInfo = GetRegistration(_RegistrationID);
        }

        private PatientRegistration GetRegistration(long regID)
        {
            using (DbConnection connection = PatientProvider.Instance.CreateConnection())
            {
                return GetRegistration(regID, connection, null);
            }
        }

        private PatientRegistration GetRegistration(long regID, DbConnection connection, DbTransaction tran)
        {
            PatientRegistration registrationInfo = PatientProvider.Instance.GetRegistration(regID, connection, tran);
            if (registrationInfo == null)
            {
                //Bao loi khong co dang ky nay.
                throw new Exception("Cannot find this registration");
            }

            if (registrationInfo.PatientID.HasValue && registrationInfo.PatientID.Value > 0)
            {
                registrationInfo.Patient = PatientProvider.Instance.GetPatientByID_Simple(registrationInfo.PatientID.Value, connection, tran); 
            }

            List<PatientRegistrationDetail> regDetails = PatientProvider.Instance.GetAllRegistrationDetails(regID, connection, tran);

            if (regDetails != null)
            {
                registrationInfo.PatientRegistrationDetails = regDetails.ToObservableCollection<PatientRegistrationDetail>();
            }

            //Can lam sang
            List<PatientPCLRequest> PCLRequestList = PatientProvider.Instance.GetPCLRequestListByRegistrationID(regID, connection, tran);
            if(PCLRequestList != null)
            {
                registrationInfo.PCLRequests = PCLRequestList.ToObservableCollection();
            }

            List<OutwardDrugInvoice> invoiceList = PatientProvider.Instance.GetDrugInvoiceListByRegistrationID(regID, connection, tran);

            if (invoiceList != null)
            {
                registrationInfo.DrugInvoices = invoiceList.ToObservableCollection<OutwardDrugInvoice>();

            }
            if (registrationInfo.PatientTransaction != null)
            {
                List<PatientPayment> allPayments = CommonProvider.Payments.GetAllPayments(registrationInfo.PatientTransaction.TransactionID, connection, tran);
                if (allPayments != null)
                {
                    registrationInfo.PatientTransaction.PatientPayments = allPayments.ToObservableCollection<PatientPayment>();
                }
            }

            if(registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU
                || registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
            {
                //Lay danh sach thuoc, y dung cu, hoa chat.
                List<OutwardDrugClinicDeptInvoice> allInPatientInvoices = PatientProvider.Instance.GetAllInPatientInvoices(registrationInfo.PtRegistrationID, connection, tran);
                if (allInPatientInvoices != null)
                {
                    registrationInfo.InPatientInvoices = allInPatientInvoices.ToObservableCollection<OutwardDrugClinicDeptInvoice>();
                }
            }
            return registrationInfo;
        }
        /// <summary>
        /// Kiểm tra 1 item đã có trong collection hay chưa, chưa có thì add vô, có rồi thì thay thế.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="collection"></param>
        private void AddItemOrReplaceIfExists<T>(T item, ObservableCollection<T> collection)
        {
            int idx = collection.IndexOf(item);
            if (idx >= 0)//Co ton tai item
            {
                collection[idx] = item;
            }
            else
            {
                collection.Add(item);
            }
        }

        public void AddPCLRequestList(IList<PatientPCLRequest> requestList)
        {
            if (_RegistrationInfo.PCLRequests == null || _RegistrationInfo.PCLRequests.Count == 0)
            {
                _RegistrationInfo.PCLRequests = requestList.ToObservableCollection<PatientPCLRequest>();
                return;
            }
            PatientPCLRequest item;
            for (int i = 0; i < requestList.Count; i++)
            {
                item = requestList[i];
       
                AddItemOrReplaceIfExists<PatientPCLRequest>(item, _RegistrationInfo.PCLRequests);
            }
        }
        /// <summary>
        /// Them vao danh sach thuoc, y cu, hoa chat cua benh nhan noi tru.
        /// </summary>
        /// <param name="invoice"></param>
        public void AddInPatientInvoiceList(IList<OutwardDrugClinicDeptInvoice> invoiceList)
        {
            if (_RegistrationInfo.InPatientInvoices == null || _RegistrationInfo.InPatientInvoices.Count == 0)
            {
                _RegistrationInfo.InPatientInvoices = invoiceList.ToObservableCollection<OutwardDrugClinicDeptInvoice>();
                return;
            }
            OutwardDrugClinicDeptInvoice item;
            for (int i = 0; i < invoiceList.Count; i++)
            {
                item = invoiceList[i];
                AddItemOrReplaceIfExists<OutwardDrugClinicDeptInvoice>(item, _RegistrationInfo.InPatientInvoices);
            }

        }

        public void AddInvoice(OutwardDrugInvoice invoice)
        {
            if (_RegistrationInfo.DrugInvoices == null || _RegistrationInfo.DrugInvoices.Count == 0)
            {
                _RegistrationInfo.DrugInvoices = new List<OutwardDrugInvoice>().ToObservableCollection();
                _RegistrationInfo.DrugInvoices.Add(invoice);
                return;
            }
            AddItemOrReplaceIfExists<OutwardDrugInvoice>(invoice, _RegistrationInfo.DrugInvoices);
        }
        public void AddServiceRegDetails(IList<PatientRegistrationDetail> medServiceRegistrationDetailsItem)
        {
            //if (_RegistrationInfo.PatientRegistrationDetails == null || _RegistrationInfo.PatientRegistrationDetails.Count == 0)
            //{
            //    _RegistrationInfo.PatientRegistrationDetails = medServiceRegistrationDetailsItem.ToObservableCollection<PatientRegistrationDetail>();
            //    return;
            //}
            //PatientRegistrationDetail item;
            //for (int i = 0; i<medServiceRegistrationDetailsItem.Count;i++)
            //{
            //    item = medServiceRegistrationDetailsItem[i];
            //    AddItemOrReplaceIfExists<PatientRegistrationDetail>(item, _RegistrationInfo.PatientRegistrationDetails);
            //}
            AddServiceRegDetails(_RegistrationInfo, medServiceRegistrationDetailsItem);
        }

        public void AddServiceRegDetails(PatientRegistration regInfo, IList<PatientRegistrationDetail> medServiceRegistrationDetailsItem)
        {
            if (regInfo.PatientRegistrationDetails == null || regInfo.PatientRegistrationDetails.Count == 0)
            {
                regInfo.PatientRegistrationDetails = medServiceRegistrationDetailsItem.ToObservableCollection<PatientRegistrationDetail>();
                return;
            }
            PatientRegistrationDetail item;
            for (int i = 0; i < medServiceRegistrationDetailsItem.Count; i++)
            {
                item = medServiceRegistrationDetailsItem[i];
                AddItemOrReplaceIfExists<PatientRegistrationDetail>(item, regInfo.PatientRegistrationDetails);
            }
        }

        /// <summary>
        /// Kiểm tra sự thay đổi trên transaction ứng với đăng ký này.
        /// </summary>
        /// <param name="allItems"></param>
        /// <returns></returns>
        private PatientTransaction GetTransactionModifications()
        {
            return GetTransactionModifications(_RegistrationInfo);
        }
        
        //-----------------------------------------------------------------------------------------
        private PatientTransaction GetTransactionModifications(PatientRegistration regInfo)
        {
            if (regInfo.PatientTransaction == null || regInfo.PatientTransaction.TransactionID <= 0)
            {
                return null;
            }

            //Lay het chi tiet transaction cua dang ky nay.
            List<PatientTransactionDetail> allTranDetails = PatientProvider.Instance.GetAlltransactionDetails(regInfo.PatientTransaction.TransactionID);

            regInfo.PatientTransaction.PatientTransactionDetails = allTranDetails.ToObservableCollection();

            #region Tinh lai thay doi tren TRANSACTION tuong ung voi cac dich vu KHAM CHUA BENH
            //Lay danh sach tat ca cac item trong bang TransactionDetails cua cac dich vu dang ky.
            IEnumerable<PatientTransactionDetail> allTransDetailsOfServices = null;
            if (regInfo.PatientTransaction.PatientTransactionDetails != null)
            {
                allTransDetailsOfServices = regInfo.PatientTransaction.PatientTransactionDetails.Where(detailsItem =>
                {
                    return detailsItem.PtRegDetailID.HasValue; //La dich vu kham chua benh, PCL
                });
            }
            else
            {
                allTransDetailsOfServices = new List<PatientTransactionDetail>();
            }


            foreach (PatientRegistrationDetail item in regInfo.PatientRegistrationDetails)
            {
                //Kiem tra transaction details ung voi item nay co hay chua.
                //Neu chua co thi them 1 transaction details
                //Neu co thi kiem tra gia tien co thay doi thi cap nhat trang thai cua transaction details tuong ung.
                //Neu registration item nay bi delete thi danh dau transaction item tuong ung sang trang thai delete luon.
                PatientTransactionDetail temp = allTransDetailsOfServices.Where(detailsItem =>
                {
                    return detailsItem.PtRegDetailID == item.PtRegDetailID;
                }).FirstOrDefault();
                if (temp == null) //Khong co.
                {
                    //tao 1 transaction details va add vao.
                    temp = CreateTranDetailsForRegDetails(item);
                    if (temp != null)
                    {
                        regInfo.PatientTransaction.PatientTransactionDetails.Add(temp);   
                    }

                    continue;
                }

                // Co chi tiet dang ky nay trong transation details tuong ung.
                if (item.RecordState == RecordState.DELETED)
                {
                    //temp.RecordState = RecordState.DELETED;
                }
                else
                {
                    if (temp.HealthInsuranceRebate != item.TotalHIPayment || temp.AmountCoPay != item.TotalCoPayment)
                    {
                        temp.HealthInsuranceRebate = item.TotalHIPayment;
                        temp.AmountCoPay = item.TotalCoPayment;
                        //temp.RecordState = RecordState.MODIFIED;
                    }
                }

                continue;
            }
            #endregion


            #region Tinh lai thay doi tren TRANSACTION tuong ung voi THUOC
            //Lay danh sach tat ca cac item trong bang TransactionDetails cua cac dich vu ve thuoc
            IEnumerable<PatientTransactionDetail> allTransDetailsOfDrugs = regInfo.PatientTransaction.PatientTransactionDetails.Where(detailsItem =>
            {
                return detailsItem.outiID.HasValue; //La dich vu thuoc
            });

            //Đối với thuốc thì phải gộp những item thuốc cùng 1 invoice lại, rồi lấy id của invoice này 
            //thêm vào transaction details

            IList<OutwardDrugInvoice> allDrugInvoices = regInfo.DrugInvoices;

            foreach (OutwardDrugInvoice item in allDrugInvoices)
            {
                PatientTransactionDetail temp = allTransDetailsOfDrugs.Where(detailsItem =>
                {
                    return detailsItem.outiID == item.outiID;
                }).FirstOrDefault();
                if (temp == null) //Khong co.
                {
                    //tao 1 transaction details va add vao.
                    temp = CreateTranDetailsForDrugInvoice(item);
                    regInfo.PatientTransaction.PatientTransactionDetails.Add(temp);

                    continue;
                }
                else // Co chi tiet dang ky nay trong transation details tuong ung.
                {
                    decimal totalHIPayment = 0, totalCoPayment = 0;
                    //Tinh tong tien bao hiem phai tra cho item nay
                    foreach (OutwardDrug drug in item.OutwardDrugs)
                    {
                        totalHIPayment += drug.TotalHIPayment;
                        totalCoPayment += drug.TotalCoPayment;
                    }
                    if (temp.HealthInsuranceRebate != totalHIPayment || temp.AmountCoPay != totalCoPayment)
                    {
                        temp.HealthInsuranceRebate = totalHIPayment;
                        temp.AmountCoPay = totalCoPayment;
                        //temp.RecordState = RecordState.MODIFIED;
                    }
                    continue;
                }
            }
            #endregion

            #region Tinh lai thay doi tren TRANSACTION tuong ung voi CAN LAM SANG
            //Lay danh sach tat ca cac item trong bang TransactionDetails cua cac dich vu ve PCL
            IEnumerable<PatientTransactionDetail> allTransDetailsOfPCLItems = regInfo.PatientTransaction.PatientTransactionDetails.Where(detailsItem =>
            {
                return detailsItem.PCLRequestID.HasValue; //La dich vu PCL
            });

            //Đối với PCL thì phải gộp những item thuốc cùng 1 invoice lại, rồi lấy id của invoice này 
            //thêm vào transaction details

            IList<PatientPCLRequest> allPCLRequests = regInfo.PCLRequests;

            foreach (PatientPCLRequest item in allPCLRequests)
            {
                PatientTransactionDetail temp = allTransDetailsOfPCLItems.Where(detailsItem =>
                {
                    return detailsItem.PCLRequestID == item.PatientPCLReqID;
                }).FirstOrDefault();
                if (temp == null) //Khong co.
                {
                    //tao 1 transaction details va add vao.
                    temp = CreateTranDetailsForPCLRequest(item);
                    regInfo.PatientTransaction.PatientTransactionDetails.Add(temp);

                    continue;
                }
                else // Co chi tiet dang ky nay trong transation details tuong ung.
                {
                    decimal totalHIPayment = 0, totalCoPayment = 0;
                    //Tinh tong tien bao hiem phai tra cho item nay
                    foreach (PatientPCLRequestDetail PCLDetails in item.PatientPCLRequestIndicators)
                    {
                        totalHIPayment += PCLDetails.TotalHIPayment;
                        totalCoPayment += PCLDetails.TotalCoPayment;
                    }
                    if (temp.HealthInsuranceRebate != totalHIPayment || temp.AmountCoPay != totalCoPayment)
                    {
                        temp.HealthInsuranceRebate = totalHIPayment;
                        temp.AmountCoPay = totalCoPayment;
                        //temp.RecordState = RecordState.MODIFIED;
                    }
                    continue;
                }
            }
            #endregion

            return regInfo.PatientTransaction;
        }
        private PatientTransactionDetail CreateTranDetailsForRegDetails(PatientRegistrationDetail regDetails)
        {
            if(regDetails.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            {
                if(regDetails.PaidTime == null)
                {
                    return null;
                }
            }
            else
            {
                if (regDetails.RefundTime == null)
                {
                    return null;
                }
            }
            PatientTransactionDetail tranDetails = new PatientTransactionDetail();
            tranDetails.StaffID = regDetails.StaffID;
            tranDetails.PtRegDetailID = regDetails.PtRegDetailID;

            tranDetails.Amount = Math.Abs(regDetails.TotalInvoicePrice);
            tranDetails.AmountCoPay = Math.Abs(regDetails.TotalCoPayment);
            tranDetails.PriceDifference = Math.Abs(regDetails.TotalPriceDifference);

            tranDetails.HealthInsuranceRebate = Math.Abs((decimal)regDetails.TotalHIPayment);
            if(regDetails.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            {
               // tranDetails.CreditOrDebit = -1;
            }
            else
            {
               // tranDetails.CreditOrDebit = 1;
            }

            tranDetails.Qty = (byte)regDetails.Qty;

            return tranDetails;
        }
        private PatientTransactionDetail CreateTranDetailsForDrugInvoice(OutwardDrugInvoice item)
        {
            PatientTransactionDetail tranDetails = new PatientTransactionDetail();
            tranDetails.StaffID = item.StaffID;
            tranDetails.outiID = item.outiID;

            decimal amount = 0, amountCoPay = 0, priceDiff = 0, totalHIPayment = 0;
            foreach (OutwardDrug drugItem in item.OutwardDrugs)
            {
                amount += drugItem.TotalInvoicePrice;
                amountCoPay += drugItem.TotalCoPayment;
                priceDiff += drugItem.TotalPriceDifference;
                totalHIPayment += drugItem.TotalHIPayment;
            }

            tranDetails.Amount = amount;
            tranDetails.AmountCoPay = amountCoPay;
            tranDetails.PriceDifference = priceDiff;

            tranDetails.HealthInsuranceRebate = totalHIPayment;

            tranDetails.Qty = 1;

            return tranDetails;
        }

        private PatientTransactionDetail CreateTranDetailsForPCLRequest(PatientPCLRequest item)
        {
            if (item.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            {
                if (item.PaidTime == null)
                {
                    return null;
                }
            }
            else
            {
                if (item.RefundTime == null)
                {
                    return null;
                }
            }

            PatientTransactionDetail tranDetails = new PatientTransactionDetail();
            tranDetails.StaffID = item.StaffID;
            tranDetails.PCLRequestID = item.PatientPCLReqID;

            decimal amount = 0, amountCoPay = 0, priceDiff = 0, totalHIPayment = 0;
            foreach (PatientPCLRequestDetail requestDetails in item.PatientPCLRequestIndicators)
            {
                amount += requestDetails.TotalInvoicePrice;
                amountCoPay += requestDetails.TotalCoPayment;
                priceDiff += requestDetails.TotalPriceDifference;
                totalHIPayment += requestDetails.TotalHIPayment;    

                //if (!requestDetails.MarkedAsDeleted)
                //{
                //    if(requestDetails.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                //    {
                //        amount += requestDetails.TotalInvoicePrice;
                //        amountCoPay += requestDetails.TotalCoPayment;
                //        priceDiff += requestDetails.TotalPriceDifference;
                //        totalHIPayment += requestDetails.TotalHIPayment;    
                //    }
                //}
            }

            tranDetails.Amount = amount;
            tranDetails.AmountCoPay = amountCoPay;
            tranDetails.PriceDifference = priceDiff;

            tranDetails.HealthInsuranceRebate = totalHIPayment;

            if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            {
                //tranDetails.CreditOrDebit = -1;
            }
            else
            {
                //tranDetails.CreditOrDebit = 1;
            }

            tranDetails.Qty = 1;

            return tranDetails;
        }

       
        /// <summary>
        /// Tao moi mot dang ky va luu vao database.
        /// </summary>
        /// <param name="registrationInfo"></param>
        /// <returns></returns>
        private PatientRegistration CreateNewRegistration(PatientRegistration registrationInfo, DbConnection conn, DbTransaction tran, out List<long> PCLRequestIDList, out List<PatientRegistrationDetail> NewRegDetailsID)
        {
            PCLRequestIDList = null;
            NewRegDetailsID = null;

            long PatientRegistrationID;
            int SequenceNo;
            //La dang ky moi thi dang ky benh nhan nay.
            //Lay quyen loi bao hiem
            if (registrationInfo.PatientClassification.PatientClassID == (long)PatientType.INSUARED_PATIENT)
            {
                bool isCrossRegion = false;
                double hiBenefit = 0.0;
                hiBenefit = CalcRegProcessor.Instance.CalcPatientHiBenefit(registrationInfo, out isCrossRegion);
                registrationInfo.PtInsuranceBenefit = hiBenefit;
                registrationInfo.IsCrossRegion = isCrossRegion;
            }

            StartCalculating(registrationInfo);

            PatientProvider.Instance.RegisterPatient(registrationInfo, conn, tran, out PatientRegistrationID, out SequenceNo);
            registrationInfo.PtRegistrationID = PatientRegistrationID;
            registrationInfo.SequenceNo = SequenceNo;

            if(registrationInfo.PCLRequests != null)
            {
                //Kiem tra benh nhan nay da co benh an chua. Neu chua co thi tao moi benh an cho no,
                long MedicalRecordID;
                CreatePatientMedialRecordIfNotExists(registrationInfo, out MedicalRecordID, conn, tran);

                SaveChangesOnPCLRequests(MedicalRecordID, registrationInfo, conn, tran, out PCLRequestIDList);
            }

            //Luu danh sach thuoc, y dung cu, hoa chat cho noi tru.
            if (registrationInfo.InPatientInvoices != null && registrationInfo.InPatientInvoices.Count > 0)
            {
                List<OutwardDrugClinicDeptInvoice> outInvoiceList;
                SaveChangesOnInPatientInvoices(registrationInfo, conn, tran, out outInvoiceList);
            }
            
            //Lay lai danh sach chi tiet dang ky
            List<PatientRegistrationDetail> regDetails = PatientProvider.Instance.GetAllRegistrationDetails(registrationInfo.PtRegistrationID, conn, tran);

            registrationInfo.PatientRegistrationDetails = regDetails.ToObservableCollection();
            NewRegDetailsID = regDetails;

            return registrationInfo;
        }

        /// <summary>
        /// Tạo một đăng ký trống và lưu vào database.
        /// </summary>
        /// <param name="registrationInfo"></param>
        /// <returns></returns>
        private PatientRegistration CreateNewRegistration(PatientRegistration registrationInfo)
        {
            long PatientRegistrationID;
            int SequenceNo;
            //La dang ky moi thi dang ky benh nhan nay.
            //Lay quyen loi bao hiem
            if (registrationInfo.PatientClassification.PatientClassID == (long)PatientType.INSUARED_PATIENT)
            {
                bool isCrossRegion = false;
                double hiBenefit = 0.0;
                hiBenefit = CalcRegProcessor.Instance.CalcPatientHiBenefit(registrationInfo, out isCrossRegion);
                registrationInfo.PtInsuranceBenefit = hiBenefit;
                registrationInfo.IsCrossRegion = isCrossRegion;
            }

            PatientProvider.Instance.RegisterPatient(registrationInfo, out PatientRegistrationID, out SequenceNo);

            PatientRegistration savedRegistration = PatientProvider.Instance.GetRegistration(PatientRegistrationID);
            return savedRegistration;
        }
        
        //Tao benh an neu chua co.
        private void CreatePatientMedialRecordIfNotExists(PatientRegistration registrationInfo, out long PatientMedicalRecordID,DbConnection conn,DbTransaction tran)
        {
            PatientMedicalRecordID = -1;
            long MedicalRecordID = -1;

            if (!PatientProvider.Instance.CheckIfMedicalRecordExists(registrationInfo.PatientID.Value, out MedicalRecordID,conn,tran))
            {
                PatientMedicalRecord medicalRecord = new PatientMedicalRecord();
                medicalRecord.PatientID = registrationInfo.PatientID.Value;
                medicalRecord.NationalMedicalCode = "MedicalCode";
                //medicalRecord.PatientRecBarCode = registrationInfo.Patient.PatientBarcode;
                medicalRecord.CreatedDate = DateTime.Now;

                bool bCreateMROK = PatientProvider.Instance.AddNewPatientMedicalRecord(medicalRecord, out MedicalRecordID,conn,tran);
                if (bCreateMROK)
                {
                    PatientMedicalRecordID = MedicalRecordID;
                }
                else
                {
                    throw new Exception(string.Format("{0}.",eHCMSResources.Z1780_G1_CannotCreatePatient));
                }
            }
            PatientMedicalRecordID = MedicalRecordID;
        }

        private PatientServiceRecord CreateNewServiceRecord(PatientRegistration registrationInfo, long MedicalRecordID, DbConnection conn, DbTransaction tran)
        {
            //Tao 1 PatientServiceRecord.
            long ServiceRecordID;
            PatientServiceRecord serviceRecord = new PatientServiceRecord();
            serviceRecord.PtRegistrationID = registrationInfo.PtRegistrationID;
            serviceRecord.StaffID = null; 
            serviceRecord.ExamDate = DateTime.Now;
            serviceRecord.V_Behaving = (long)AllLookupValues.Behaving.CHI_DINH_XET_NGHIEM_CLS;
            serviceRecord.V_ProcessingType = (long)AllLookupValues.ProcessingType.PARA_CLINICAL_EXAMINATION;

            bool bOK = PatientProvider.Instance.AddNewPatientServiceRecord(MedicalRecordID, serviceRecord, out ServiceRecordID, conn, tran);
            if (bOK)
            {
                serviceRecord.ServiceRecID = ServiceRecordID;
                serviceRecord.PatientRegistration = registrationInfo;
            }
            else
            {
                throw new Exception(string.Format("{0}.",eHCMSResources.Z1796_G1_KgTaoDcPatientServiceRecord));
            }
            return serviceRecord;
        }
       

        private void SaveChangesOnPCLRequests(long medicalRecordId, PatientRegistration regInfo, DbConnection conn, DbTransaction tran, out List<long> newRequestIdList)
        {
            long pclRequestID;
            newRequestIdList = new List<long>();
            //Add vao bang CAN LAM SANG.
            foreach (PatientPCLRequest pclRequest in regInfo.PCLRequests)
            {
                if (pclRequest.PatientPCLReqID <= 0) //Thêm mới.
                {
                    //Kiem tra service record co chua. neu chua co thi tao service record truoc.
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
                        PatientServiceRecord serviceRecord = CreateNewServiceRecord(regInfo, medicalRecordId, conn, tran);
                        pclRequest.PatientServiceRecord = serviceRecord;
                    }

                    bool bOK = PatientProvider.Instance.AddNewPCLRequest(pclRequest, out pclRequestID, conn, tran);
                    if (bOK)
                    {

                        newRequestIdList.Add(pclRequestID);
                        long PCLRequestDetails;
                        pclRequest.PatientPCLReqID = pclRequestID;

                        foreach (PatientPCLRequestDetail requestDetails in pclRequest.PatientPCLRequestIndicators)
                        {
                            requestDetails.PatientPCLRequest = pclRequest;
                            if (requestDetails.RecordState != RecordState.DELETED)
                            {
                                PatientProvider.Instance.AddNewPCLRequestDetails(pclRequestID, requestDetails, out PCLRequestDetails, conn, tran);
                            }
                        }
                    }
                }
                //Có rồi => update
                else
                {
                    int numOfDeletedItems = 0;
                    int numOfCancelledItems = 0;
                    foreach (PatientPCLRequestDetail requestDetails in pclRequest.PatientPCLRequestIndicators)
                    {
                        if(requestDetails.MarkedAsDeleted)
                        {
                            numOfDeletedItems++;
                        }
                        if (requestDetails.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                        {
                            numOfCancelledItems++;
                        }
                        requestDetails.PatientPCLRequest = pclRequest;
                        //Neu chua co thi insert no vao, co roi thi update.
                        if(requestDetails.PCLReqItemID > 0)
                        {
                            PatientProvider.Instance.UpdatePCLRequestDetails(requestDetails,conn,tran);
                        }
                        else
                        {
                            long requestDetailsID;
                            if(requestDetails.RecordState != RecordState.DELETED)
                            {
                                PatientProvider.Instance.AddNewPCLRequestDetails(pclRequest.PatientPCLReqID, requestDetails, out requestDetailsID, conn, tran);
                            }
                            
                        }
                    }
                    //Nếu tất cả con nó bị markdelete => markdelete luôn.
                    if (numOfDeletedItems == pclRequest.PatientPCLRequestIndicators.Count)
                    {
                        pclRequest.MarkedAsDeleted = true;
                    }
                    else
                    {
                        if(numOfCancelledItems == pclRequest.PatientPCLRequestIndicators.Count)
                        {
                            pclRequest.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                        }
                    }

                    PatientProvider.Instance.UpdatePCLRequest(pclRequest, conn, tran);
                }
            }
        }
       
        /// <summary>
        /// Thực hiện tính toán cho đăng ký hiện tại, trả về kết quả là đăng ký đã được tính xong.
        /// </summary>
        public PatientRegistration StartCalculating()
        {
            return StartCalculating(_RegistrationInfo);
        }

        private PatientRegistration StartCalculating(PatientRegistration regInfo)
        {
            PatientRegistration CalculatedRegistration;
            CalcRegProcessor.Instance.CalcRegistration(regInfo);
            return regInfo;
        }

        public void CalcOutwardDrugInvoice(long registrationID, OutwardDrugInvoice drugInvoice, out PatientRegistration curRegistration)
        {
            AddInvoice(drugInvoice);
            StartCalculating();
            curRegistration = _RegistrationInfo;
        }

        public void CalcRegistrationDetails(PatientRegistration registrationInfo, out PatientRegistration curRegistration)
        {
            AddServiceRegDetails(registrationInfo.PatientRegistrationDetails);
            AddPCLRequestList(registrationInfo.PCLRequests);
            if (registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU
                || registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
            {
                AddInPatientInvoiceList(registrationInfo.InPatientInvoices); 
            }
            StartCalculating();
            curRegistration = RegistrationInfo;
        }



        public void SaveRegistrationDetails(PatientRegistration registrationInfo, out PatientRegistration SavedRegistration)
        {
            List<OutwardDrugInvoice> NewInvoiceList;
            SavedRegistration = registrationInfo;
            List<long> PCLRequestIDList;
            AddServiceRegDetails(registrationInfo.PatientRegistrationDetails);
            AddPCLRequestList(registrationInfo.PCLRequests);
            bool bOK = SaveRegistration(registrationInfo, null, out PCLRequestIDList, out NewInvoiceList);
            if (bOK)
            {
                SavedRegistration = GetRegistration(registrationInfo.PtRegistrationID);
                StartCalculating(SavedRegistration);
            }
        }
        
        //Lay list invoice tuong ung voi druglist
        private List<OutwardDrugInvoice> GetInvoiceList(IList<OutwardDrug> drugList)
        {
            List<OutwardDrugInvoice> allDrugInvoices = new List<OutwardDrugInvoice>();
            OutwardDrugInvoice tempInvoice = null;
            if (drugList != null)
            {
                foreach (OutwardDrug item in drugList)
                {
                    if (item.OutwardDrugInvoice != null)
                    {
                        if (item.OutwardDrugInvoice != tempInvoice)
                        {
                            tempInvoice = item.OutwardDrugInvoice;
                            tempInvoice.OutwardDrugs = new List<OutwardDrug>().ToObservableCollection();
                            tempInvoice.OutwardDrugs.Add(item);
                            allDrugInvoices.Add(tempInvoice);
                        }
                        else
                        {
                            tempInvoice.OutwardDrugs.Add(item);
                        }
                    }
                }
            }
            return allDrugInvoices;
        }
        //Tao transaction cho dang ky.
        private PatientTransaction CreateTransationForRegistration(PatientRegistration registrationInfo)
        {
            PatientTransaction tran = new PatientTransaction();
            tran.PtRegistrationID = registrationInfo.PtRegistrationID;

            tran.PatientTransactionDetails = new List<PatientTransactionDetail>().ToObservableCollection();
            PatientTransactionDetail tranDetails;
            if (registrationInfo.PatientRegistrationDetails != null)
            {
                foreach (PatientRegistrationDetail regDetails in registrationInfo.PatientRegistrationDetails)
                {
                    tranDetails = CreateTranDetailsForRegDetails(regDetails);
                    if(tranDetails != null)
                    {
                        tran.PatientTransactionDetails.Add(tranDetails);   
                    }
                } 
            }

            //CreateTranDetailsForPCLRequest
            if (registrationInfo.PCLRequests != null)
            {
                foreach (PatientPCLRequest PCLRequest in registrationInfo.PCLRequests)
                {
                    tranDetails = CreateTranDetailsForPCLRequest(PCLRequest);
                    tran.PatientTransactionDetails.Add(tranDetails);
                } 
            }

            if (registrationInfo.DrugInvoices != null)
            {
                foreach (OutwardDrugInvoice invoice in registrationInfo.DrugInvoices)
                {
                    tranDetails = CreateTranDetailsForDrugInvoice(invoice);
                    tran.PatientTransactionDetails.Add(tranDetails);
                } 
            }

            return tran;
        }

        private PatientTransaction CreateTransationForRegistration_V2(PatientRegistration registrationInfo)
        {
            PatientTransaction tran = new PatientTransaction();
            tran.PtRegistrationID = registrationInfo.PtRegistrationID;

            tran.PatientTransactionDetails = new List<PatientTransactionDetail>().ToObservableCollection();

            return tran;
        }

        /// <summary>
        /// Kiem tra thay doi tren transaction tuong ung voi danh sach cac item duoc tra tien.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="paidRegDetailsList"></param>
        /// <param name="paidPclRequestDetailsList"></param>
        /// <param name="paidDrugInvoice"></param>
        /// <param name="paidMedItemList"></param>
        /// <param name="paidChemicalItemList"></param>
        /// <returns></returns>
        private PatientTransaction GetTransactionModifications(PatientTransaction transaction, List<PatientRegistrationDetail> paidRegDetailsList,
                                        List<PatientPCLRequest> paidPclRequestList,
                                        List<OutwardDrugInvoice> paidDrugInvoice,
                                        List<OutwardDrugClinicDeptInvoice> paidMedItemList,
                                        List<OutwardDrugClinicDeptInvoice> paidChemicalItemList)
        {
            if (transaction == null || transaction.TransactionID <= 0)
            {
                return null;
            }

            //Lay het chi tiet transaction cua dang ky nay.
            List<PatientTransactionDetail> allTranDetails = PatientProvider.Instance.GetAlltransactionDetails(transaction.TransactionID);

            transaction.PatientTransactionDetails = allTranDetails.ToObservableCollection();

            #region Tinh lai thay doi tren TRANSACTION tuong ung voi cac dich vu KHAM CHUA BENH
            //Lay danh sach tat ca cac item trong bang TransactionDetails cua cac dich vu dang ky.
            IEnumerable<PatientTransactionDetail> allTransDetailsOfServices = null;
            if (transaction.PatientTransactionDetails != null)
            {
                allTransDetailsOfServices = transaction.PatientTransactionDetails.Where(detailsItem =>
                {
                    return detailsItem.PtRegDetailID.HasValue; //La dich vu kham chua benh, PCL
                });
            }
            else
            {
                allTransDetailsOfServices = new List<PatientTransactionDetail>();
            }


            if (paidRegDetailsList != null)
            {
                foreach (PatientRegistrationDetail item in paidRegDetailsList)
                {
                    //Kiem tra transaction details ung voi item nay co hay chua.
                    //Neu chua co thi them 1 transaction details
                    //Neu co thi kiem tra gia tien co thay doi thi cap nhat trang thai cua transaction details tuong ung.
                    //Neu registration item nay bi delete thi danh dau transaction item tuong ung sang trang thai delete luon.
                    PatientTransactionDetail temp = null;
                    Int16 iCreditOrDebit = 1;
                    if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    {
                        iCreditOrDebit = -1;
                    }
                    temp = allTransDetailsOfServices.Where(detailsItem =>
                    {
                        return detailsItem.PtRegDetailID == item.PtRegDetailID; //&& detailsItem.CreditOrDebit == iCreditOrDebit;
                    }).FirstOrDefault();
                    if (temp == null) //Khong co.
                    {
                        //tao 1 transaction details va add vao.
                        temp = CreateTranDetailsForRegDetails(item);
                        if (temp != null)
                        {
                            //temp.CreditOrDebit = iCreditOrDebit;
                            transaction.PatientTransactionDetails.Add(temp);
                        }

                        continue;
                    }

                    if (item.RecordState == RecordState.DELETED)
                    {
                        //temp.RecordState = RecordState.DELETED;
                    }
                    else
                    {
                        if (temp.HealthInsuranceRebate != item.TotalHIPayment || temp.AmountCoPay != item.TotalCoPayment)
                        {
                            temp.HealthInsuranceRebate = item.TotalHIPayment;
                            temp.AmountCoPay = item.TotalCoPayment;
                            //temp.RecordState = RecordState.MODIFIED;
                        }
                    }

                    continue;
                } 
            }
            #endregion


            #region Tinh lai thay doi tren TRANSACTION tuong ung voi CAN LAM SANG
            //Lay danh sach tat ca cac item trong bang TransactionDetails cua cac dich vu ve PCL
            IEnumerable<PatientTransactionDetail> allTransDetailsOfPCLItems = transaction.PatientTransactionDetails.Where(detailsItem =>
            {
                return detailsItem.PCLRequestID.HasValue; //La dich vu PCL
            });

            if (paidPclRequestList != null)
            {
                foreach (PatientPCLRequest item in paidPclRequestList)
                {
                    PatientTransactionDetail temp = null;
                    Int16 iCreditOrDebit = 1;
                    if (item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    {
                        iCreditOrDebit = -1;
                    }

                    temp = allTransDetailsOfPCLItems.Where(detailsItem =>
                    {
                        return detailsItem.PCLRequestID == item.PatientPCLReqID; //&& detailsItem.CreditOrDebit == iCreditOrDebit;
                    }).FirstOrDefault();
                    if (temp == null) //Khong co.
                    {
                        //tao 1 transaction details va add vao.
                        temp = CreateTranDetailsForPCLRequest(item);
                        transaction.PatientTransactionDetails.Add(temp);

                        continue;
                    }
                    else // Co chi tiet dang ky nay trong transation details tuong ung.
                    {
                        decimal totalHIPayment = 0, totalCoPayment = 0;
                        //Tinh tong tien bao hiem phai tra cho item nay
                        foreach (PatientPCLRequestDetail PCLDetails in item.PatientPCLRequestIndicators)
                        {
                            totalHIPayment += PCLDetails.TotalHIPayment;
                            totalCoPayment += PCLDetails.TotalCoPayment;
                        }
                        if (temp.HealthInsuranceRebate != totalHIPayment || temp.AmountCoPay != totalCoPayment)
                        {
                            temp.HealthInsuranceRebate = totalHIPayment;
                            temp.AmountCoPay = totalCoPayment;
                            //temp.RecordState = RecordState.MODIFIED;
                        }
                        continue;
                    }
                }
            }

            
            #endregion




            //#region Tinh lai thay doi tren TRANSACTION tuong ung voi THUOC
            ////Lay danh sach tat ca cac item trong bang TransactionDetails cua cac dich vu ve thuoc
            //IEnumerable<PatientTransactionDetail> allTransDetailsOfDrugs = transaction.PatientTransactionDetails.Where(detailsItem =>
            //{
            //    return detailsItem.outiID.HasValue; //La dich vu thuoc
            //});

            ////Đối với thuốc thì phải gộp những item thuốc cùng 1 invoice lại, rồi lấy id của invoice này 
            ////thêm vào transaction details

            //IList<OutwardDrugInvoice> allDrugInvoices = regInfo.DrugInvoices;

            //foreach (OutwardDrugInvoice item in allDrugInvoices)
            //{
            //    PatientTransactionDetail temp = allTransDetailsOfDrugs.Where(detailsItem =>
            //    {
            //        return detailsItem.outiID == item.outiID;
            //    }).FirstOrDefault();
            //    if (temp == null) //Khong co.
            //    {
            //        //tao 1 transaction details va add vao.
            //        temp = CreateTranDetailsForDrugInvoice(item);
            //        transaction.PatientTransactionDetails.Add(temp);

            //        continue;
            //    }
            //    else // Co chi tiet dang ky nay trong transation details tuong ung.
            //    {
            //        decimal totalHIPayment = 0, totalCoPayment = 0;
            //        //Tinh tong tien bao hiem phai tra cho item nay
            //        foreach (OutwardDrug drug in item.OutwardDrugs)
            //        {
            //            totalHIPayment += drug.TotalHIPayment;
            //            totalCoPayment += drug.TotalCoPayment;
            //        }
            //        if (temp.HealthInsuranceRebate != totalHIPayment || temp.AmountCoPay != totalCoPayment)
            //        {
            //            temp.HealthInsuranceRebate = totalHIPayment;
            //            temp.AmountCoPay = totalCoPayment;
            //            temp.RecordState = RecordState.MODIFIED;
            //        }
            //        continue;
            //    }
            //}
            //#endregion

            
            return transaction;
        }

        /// <summary>
        /// Lay danh sach chi tiet dang ky can tra tien.
        /// </summary>
        /// <param name="regInfo"></param>
        /// <param name="colPaidRegDetails">ID cua chi tiet dang ky</param>
        /// <returns>Tra ve danh sach dang ky</returns>
        private List<PatientRegistrationDetail> GetRegDetailsToPay(PatientRegistration regInfo,List<long> colPaidRegDetails)
        {
            DateTime now = DateTime.Now;
            if (colPaidRegDetails == null || colPaidRegDetails.Count == 0 || regInfo.PatientRegistrationDetails == null || _RegistrationInfo.PatientRegistrationDetails.Count == 0)
            {
                return null;
            }

            List<PatientRegistrationDetail> paidRegDetailsList = new List<PatientRegistrationDetail>();
            //Kiem tra trong danh sach chi tiet dang ky co thang nao bang voi thang can tinh tien thi lay ra.
            foreach (var details in regInfo.PatientRegistrationDetails)
            {
                if(colPaidRegDetails.Contains(details.PtRegDetailID))
                {
                    if (details.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    {
                        if (details.RefundTime == null)
                        {
                            details.RefundTime = now;
                            paidRegDetailsList.Add(details);
                            continue;
                        }
                    }
                    else
                    {
                        if (details.PaidTime == null)
                        {
                            details.PaidTime = now;
                            paidRegDetailsList.Add(details);
                            continue;
                        }
                    }
                }
            }
            return paidRegDetailsList;
        }

        private List<PatientPCLRequest> GetPclRequestToPay(PatientRegistration regInfo, List<long> colPaidPclRequests)
        {
            DateTime now = DateTime.Now;
            if (colPaidPclRequests == null || colPaidPclRequests.Count == 0 || regInfo.PCLRequests == null || _RegistrationInfo.PCLRequests.Count == 0)
            {
                return null;
            }

            List<PatientPCLRequest> paidPclRequestList = new List<PatientPCLRequest>();
            //Kiem tra trong danh sach chi tiet dang ky co thang nao bang voi thang can tinh tien thi lay ra.
            foreach (var details in regInfo.PCLRequests)
            {
                if (colPaidPclRequests.Contains(details.PatientPCLReqID))
                {
                    if (details.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    {
                        if (details.RefundTime == null)
                        {
                            details.RefundTime = now;
                            paidPclRequestList.Add(details);

                            if (details.PatientPCLRequestIndicators != null)
                            {
                                foreach (var item in details.PatientPCLRequestIndicators)
                                {
                                    if (item.RefundTime == null)
                                    {
                                        item.RefundTime = now;   
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
                            details.PaidTime = now;
                            paidPclRequestList.Add(details);

                            if (details.PatientPCLRequestIndicators != null)
                            {
                                foreach (var item in details.PatientPCLRequestIndicators)
                                {
                                    if(item.PaidTime == null)
                                    {
                                        item.RefundTime = now;   
                                    }
                                }
                            }

                            continue;
                        }
                    }
                }
            }
            return paidPclRequestList;
        }
        /// <summary>
        /// Xử lý tính tiền cho 1 đăng ký. (Chỉ tính tiền thôi)
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool PayForRegistration(long registrationID, PatientPayment paymentDetails,
                                        List<long> colPaidRegDetails,
                                        List<long> colPaidPclRequests,
                                        List<long> colPaidDrugInvoices,
                                        List<long> colPaidMedItems,
                                        List<long> colPaidChemicalItemList, out PatientTransaction Transaction)
        {
            bool bOK = false;
            //Lay lai dang ky nay trong database.
            //PatientRegistration CurrentRegistration = GetRegistration(registrationID);

            if (paymentDetails == null || paymentDetails.PayAmount == 0)
            {
                throw new Exception(eHCMSResources.Z1707_G1_ChuaCoTTinTinhTien);
            }

            if (paymentDetails.PayAmount < 0 && paymentDetails.V_PaymentType != (long)AllLookupValues.PaymentType.HOAN_TIEN)
            {
                throw new Exception(string.Format("{0}.",eHCMSResources.Z1834_G1_TTinNhapVaoKgDung));
            }

            _RegistrationInfo = GetRegistration(registrationID);
            
            if (_RegistrationInfo == null)
            {
                throw new Exception(eHCMSResources.Z0083_G1_KhongTimThayDK);
            }

            StartCalculating(_RegistrationInfo);
          
            DateTime now = DateTime.Now;

            //Nếu chưa có transaction => Tạo transaction.
            //Cập nhật những item nào muốn tính tiền.
            //Cân bằng Transaction.
            //Xử lý tính tiền.

            using (DbConnection conn = PatientProvider.Instance.CreateConnection())
            {
                 conn.Open();
                 DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.Serializable);
                try
                {
                    PatientTransaction currentTransaction;
                    //Neu da co transaction roi thi kiem tra thay doi tren transaction nay.
                    if (_RegistrationInfo.PatientTransaction == null)
                    {
                        currentTransaction = CreateTransationForRegistration_V2(_RegistrationInfo);
                        long tranID;
                        PatientProvider.Instance.OpenTransaction(currentTransaction, out tranID, conn, tran);
                        currentTransaction.TransactionID = tranID;
                        _RegistrationInfo.PatientTransaction = currentTransaction;
                    }
                    else
                    {
                        //Lay day du thong tin Transaction.
                        currentTransaction = _RegistrationInfo.PatientTransaction;
                        currentTransaction.PatientTransactionDetails = PatientProvider.Instance.GetAlltransactionDetails(currentTransaction.TransactionID).ToObservableCollection();
                    }

                    UpdatePaidTime(_RegistrationInfo, now, colPaidRegDetails, colPaidPclRequests, colPaidDrugInvoices, colPaidMedItems, colPaidChemicalItemList, conn, tran);
                    
                    BalanceTransaction(_RegistrationInfo,conn,tran);

                    ProcessPayment(paymentDetails,currentTransaction.TransactionID,conn,tran);
                    
                    Transaction = _RegistrationInfo.PatientTransaction;
                    try
                    {
                        List<PatientPayment> allPayments = CommonProvider.Payments.GetAllPayments(Transaction.TransactionID,conn,tran);
                        if (allPayments != null)
                        {
                            Transaction.PatientPayments = allPayments.ToObservableCollection();
                        }
                    }
                    catch (System.Exception ex)
                    {

                    }
                    tran.Commit();
                }
                catch (Exception outerEx)
                {
                    tran.Rollback();
                    throw;
                } 
                return bOK;
            }
        }

        /// <summary>
        /// Lưu thay đổi trên đăng ký và cho trả tiền luôn.
        /// Thay đổi trên đăng ký bao gồm thay đổi trong danh sách chi tiết đăng ký, thuốc hoặc y dụng cụ
        /// </summary>
        /// <param name="registrationInfo"></param>
        /// <param name="paymentDetails"></param>
        /// <param name="SavedRegistration"></param>
        public void SaveRegistrationAndPay(PatientRegistration registrationInfo, PatientPayment paymentDetails, out PatientRegistration SavedRegistration, out List<OutwardDrugInvoice> NewInvoiceList)
        {
            List<long> PCLRequestIDList;
            NewInvoiceList = new List<OutwardDrugInvoice>();
            SavedRegistration = null;
            using (DbConnection conn = PatientProvider.Instance.CreateConnection())
            {
                conn.Open();

                DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.Serializable);
                List<PatientRegistrationDetail> NewRegDetailsID;//Danh sach cac RegistrationDetailsId moi duoc them vao.
                List<PatientRegistrationDetail> DeletedRegDetailsID; //Danh sach cac Registration Details ID bi deleted.
                try
                {
                    //registrationInfo = SaveRegistration(registrationInfo, out PCLRequestIDList, out NewInvoiceList, conn, tran, out NewRegDetailsID, out DeletedRegDetailsID);
                    PatientTransaction CurrentTransaction = registrationInfo.PatientTransaction;
                    if (CurrentTransaction == null)
                    {
                        CurrentTransaction = CreateTransationForRegistration(registrationInfo);
                        long tranID;
                        PatientProvider.Instance.OpenTransaction(CurrentTransaction, out tranID, conn, tran);
                        CurrentTransaction.TransactionID = tranID;
                    }

                    //Neu co tinh tien thi update lai bang tinh tien.
                    if (paymentDetails != null && paymentDetails.PayAmount > 0)
                    {
                        long paymentID;
                        PatientProvider.Instance.ProcessPayment(paymentDetails, CurrentTransaction.TransactionID, out paymentID, conn, tran);
                        paymentDetails.PtPmtID = paymentID;
                    }

                    tran.Commit();

                    try
                    {
                        SavedRegistration = GetRegistration(registrationInfo.PtRegistrationID);
                    }
                    catch (System.Exception innerEx)
                    {

                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
            }
            try
            {
                StartCalculating(SavedRegistration);
            }
            catch (System.Exception ex)
            {

            }
        }

        /// <summary>
        /// Hàm này sử dụng để lưu đăng ký thôi.
        /// </summary>
        /// <param name="registrationInfo"></param>
        /// <param name="paymentInfo"></param>
        /// <returns></returns>
        private bool SaveRegistration(PatientRegistration registrationInfo, PatientPayment paymentInfo, out List<long> pclRequestIdList, out List<OutwardDrugInvoice> NewInvoiceList)
        {
            //Lưu đăng ký
            //Nếu có TRANSACTION rồi => Cân bằng Transaction.
            using (DbConnection conn = PatientProvider.Instance.CreateConnection())
            {
                conn.Open();

                DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.Serializable);

                List<PatientRegistrationDetail> NewRegDetailsID;//Danh sach cac RegistrationDetailsId moi duoc them vao.
                List<PatientRegistrationDetail> DeletedRegDetailsID; //Danh sach cac Registration Details ID bi deleted.
                try
                {
                    //SaveRegistration(registrationInfo, out pclRequestIdList, out NewInvoiceList, conn, tran, out NewRegDetailsID, out DeletedRegDetailsID);
                    SaveChangesOnRegistration(registrationInfo, conn, tran, out NewRegDetailsID, out pclRequestIdList, out NewInvoiceList, out DeletedRegDetailsID);
                    
                    if(registrationInfo.PatientTransaction != null)
                    {
                        //Balance Transaction
                        registrationInfo = GetRegistration(registrationInfo.PtRegistrationID, conn, tran);
                        registrationInfo.PatientTransaction.PatientTransactionDetails = PatientProvider.Instance.GetAlltransactionDetails(registrationInfo.PatientTransaction.TransactionID).ToObservableCollection();
                        BalanceTransaction(registrationInfo,conn,tran);
                    }
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }

                NotifyRegistrationInfoChanged(pclRequestIdList, NewRegDetailsID, DeletedRegDetailsID, NewInvoiceList);

                return true;
            }
        }

        private void NotifyRegistrationInfoChanged(List<long> PCLRequestIDList, List<PatientRegistrationDetail> NewRegDetailsID, List<PatientRegistrationDetail> DeletedRegDetailsID, List<OutwardDrugInvoice> NewInvoiceList)
        {
            try
            {
                EventManagementService.EventManagementService eventService = new EventManagementService.EventManagementService();
                if ((NewRegDetailsID != null && NewRegDetailsID.Count > 0) || (DeletedRegDetailsID != null && DeletedRegDetailsID.Count > 0))
                {
                    AxEvent evt = new AxEvent();
                    evt.EventType = AxEventType.GET_CONSULTATION_LIST;
                    eventService.Notify(evt);
                }
                if (PCLRequestIDList != null && PCLRequestIDList.Count > 0)
                {
                    AxEvent evt = new AxEvent();
                    evt.EventType = AxEventType.GET_PCL_LIST;
                    eventService.Notify(evt);
                }
                if (NewInvoiceList != null && NewInvoiceList.Count > 0)
                {
                    AxEvent evt = new AxEvent();
                    evt.EventType = AxEventType.GET_PRESCRIPTON_LIST;
                    eventService.Notify(evt);
                }
            }
            catch (Exception ex)
            {
                //Khong notify duoc thi thoi. Lam gi duoc nhau.
            }
        }

        //private PatientRegistration SaveRegistration(PatientRegistration registrationInfo, PatientPayment paymentInfo, out List<long> PCLRequestIDList, out List<OutwardDrugInvoice> NewInvoiceList, DbConnection conn, DbTransaction tran, out List<PatientRegistrationDetail> NewRegDetailsID, out List<PatientRegistrationDetail> DeletedRegDetailsID)
        //{
        //    NewInvoiceList = new List<OutwardDrugInvoice>();
        //    PCLRequestIDList = new List<long>();//Danh sach cac request id moi duoc them vao.
        //    NewRegDetailsID = new List<PatientRegistrationDetail>();//Danh sach cac RegistrationDetailsId moi duoc them vao.
        //    DeletedRegDetailsID = new List<PatientRegistrationDetail>(); //Danh sach cac Registration Details ID bi deleted.


        //    if (registrationInfo.PtRegistrationID == 0) //dang ky moi.
        //    {
        //        registrationInfo = CreateNewRegistration(registrationInfo, conn, tran, out PCLRequestIDList, out NewRegDetailsID);
        //    }
        //    else //Cap nhat lai dang ky cu
        //    {
        //        StartCalculating(registrationInfo);

        //        IList<PatientRegistrationDetail> oldRegDetailsList = registrationInfo.PatientRegistrationDetails;

        //        //Duyet qua danh sach registration xem co thang nao thay doi thi luu lai.
        //        PatientProvider.Instance.SaveChangesOnRegistration(registrationInfo, conn, tran);
        //        OutwardDrugInvoice tempInvoice = null;

        //        //Duyet qua danh sach CLS co thang nao them moi thi add vao,
        //        if (registrationInfo.PCLRequests != null && registrationInfo.PCLRequests.Count > 0)
        //        {
        //            long MedicalRecordID;
        //            CreatePatientMedialRecordIfNotExists(registrationInfo, out MedicalRecordID, conn, tran);

        //            SaveChangesOnPCLRequests(MedicalRecordID, registrationInfo, conn, tran, out PCLRequestIDList);
        //        }

        //        long outiID;
        //        bool bOK;
        //        bool isNewInvoice = false;
        //        foreach (OutwardDrugInvoice invoice in registrationInfo.DrugInvoices)
        //        {
        //            isNewInvoice = invoice.outiID <= 0;
        //            RefDrugGenericDetailsProvider.Instance.InsertOrUpdateDrugInvoices(invoice, out outiID, conn, tran);
        //            if (isNewInvoice && outiID > 0)
        //            {
        //                invoice.outiID = outiID;
        //                NewInvoiceList.Add(invoice);
        //            }
        //        }
        //        //TODO: Duyet qua danh sach y cu nua. Hien gio chua lam.

        //        //Luu danh sach thuoc, y dung cu, hoa chat cho noi tru.
        //        if (registrationInfo.InPatientInvoices != null && registrationInfo.InPatientInvoices.Count > 0)
        //        {
        //            List<OutwardDrugClinicDeptInvoice> outInvoiceList;
        //            SaveChangesOnInPatientInvoices(registrationInfo, conn, tran, out outInvoiceList);
        //        }

        //        //Giu lai danh sach bi xoa.
        //        List<PatientRegistrationDetail> deletedRegDetails = registrationInfo.PatientRegistrationDetails.Where(i => i.RecordState == RecordState.DELETED).ToList();
        //        DeletedRegDetailsID = deletedRegDetails;

        //        registrationInfo = GetRegistration(registrationInfo.PtRegistrationID, conn, tran);

        //        if (registrationInfo.PatientRegistrationDetails != null)
        //        {
        //            foreach (PatientRegistrationDetail outItem in registrationInfo.PatientRegistrationDetails)
        //            {
        //                if (!oldRegDetailsList.Contains(outItem))
        //                {
        //                    NewRegDetailsID.Add(outItem);
        //                }
        //            }
        //        }

        //        AddServiceRegDetails(registrationInfo, deletedRegDetails);

        //        StartCalculating(registrationInfo);
        //        //Neu da co transaction thi kiem tra xem transaction co thay doi khong
        //        //Neu chua co thi tao moi transaction.
        //        PatientTransaction CurrentTransaction;
        //        if (registrationInfo.PatientTransaction != null)
        //        {
        //            CurrentTransaction = GetTransactionModifications(registrationInfo);
        //            PatientProvider.Instance.SaveChangesOnTransaction(CurrentTransaction, conn, tran);
        //        }
                
        //    }

        //    if (DeletedRegDetailsID != null && DeletedRegDetailsID.Count > 0)
        //    {
        //        foreach (PatientRegistrationDetail item in DeletedRegDetailsID)
        //        {
        //            PatientProvider.Instance.PatientQueue_MarkDelete(registrationInfo.PtRegistrationID, item.PtRegDetailID, registrationInfo.PatientID.GetValueOrDefault(),
        //                (long)(int)AllLookupValues.QueueType.KHAM_BENH, item.DeptLocation.DeptLocationID, conn, tran);

        //        }
        //    }

        //    //Doi voi nhung invoice moi them vao duoc, xoa cac item tuong ung trong queue
        //    if (NewInvoiceList != null && NewInvoiceList.Count > 0)
        //    {
        //        foreach (OutwardDrugInvoice item in NewInvoiceList)
        //        {
        //            if (item.IssueID.HasValue)
        //            {
        //                PatientProvider.Instance.PatientQueue_MarkDelete(registrationInfo.PtRegistrationID, item.IssueID.Value, registrationInfo.PatientID.GetValueOrDefault(),
        //                                (long)(int)AllLookupValues.QueueType.MUA_THUOC, -1, conn, tran); 
        //            }

        //        }
        //    }
            

        //    if (NewRegDetailsID != null && NewRegDetailsID.Count > 0)
        //    {
        //        foreach (PatientRegistrationDetail item in NewRegDetailsID)
        //        {
        //            PatientQueue queue = new PatientQueue();
        //            queue.V_QueueType = (long)(int)AllLookupValues.QueueType.KHAM_BENH;
        //            queue.PatientID = registrationInfo.PatientID;
        //            queue.RegistrationID = registrationInfo.PtRegistrationID;
        //            queue.RegistrationDetailsID = item.PtRegDetailID;
        //            if (item.DeptLocation != null)
        //            {
        //                queue.DeptLocID = item.DeptLocation.DeptLocationID;
        //            }
        //            else
        //            {
        //                queue.DeptLocID = item.DeptLocID;
        //            }
        //            if (registrationInfo.Patient != null)
        //            {
        //                queue.FullName = registrationInfo.Patient.FullName;
        //            }

        //            PatientProvider.Instance.PatientQueue_Insert(queue, conn, tran);
        //        }
        //    }

        //    if (PCLRequestIDList != null && PCLRequestIDList.Count > 0)
        //    {
        //        foreach (long ID in PCLRequestIDList)
        //        {
        //            PatientQueue queue = new PatientQueue();
        //            queue.V_QueueType = (long)(int)AllLookupValues.QueueType.PCL;
        //            queue.PatientID = registrationInfo.PatientID;
        //            queue.RegistrationID = registrationInfo.PtRegistrationID;
        //            queue.PatientPCLReqID = ID;
        //            if (registrationInfo.Patient != null)
        //            {
        //                queue.FullName = registrationInfo.Patient.FullName;
        //            }

        //            PatientProvider.Instance.PatientQueue_Insert(queue, conn, tran);
        //        }
        //    }
        //    return registrationInfo;
        //}

        /// <summary>
        /// Lưu 1 chỉ định CẬN LÂM SÀNG của bác sĩ.
        /// Trước khi lưu, tính lại giá tiền của các item details trong chỉ định này.
        /// </summary>
        /// <param name="entity">Chỉ định CẬN LÂM SÀNG cần lưu.</param>
        /// <param name="SavedPCLRequest">Thông tin chỉ định CẬN LÂM SÀNG đã được lưu trữ.</param>
        public void SavePCLRequest(PatientPCLRequest request,out List<long> PCLRequestIDList)
        {
            List<PatientPCLRequest> tempList = new List<PatientPCLRequest>();
            tempList.Add(request);
            AddPCLRequestList(tempList);
            
            List<OutwardDrugInvoice> NewInvoiceList;
            SaveRegistration(_RegistrationInfo, null, out PCLRequestIDList, out NewInvoiceList);
        }

        public void SaveOutwardDrugInvoice( OutwardDrugInvoice OutwardInvoice, out List<OutwardDrugInvoice> NewInvoiceList )
        {
            AddInvoice(OutwardInvoice);

            List<long> PCLRequestIDList;
            SaveRegistration(_RegistrationInfo, null, out PCLRequestIDList, out NewInvoiceList);
        }

        /// <summary>
        /// Luu lai thong tin thuoc, y cu, hoa chat.
        /// </summary>
        /// <param name="regInfo"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <param name="NewDeptInvoiceList"></param>
        private void SaveChangesOnInPatientInvoices(PatientRegistration regInfo, DbConnection conn, DbTransaction tran, out List<OutwardDrugClinicDeptInvoice> NewDeptInvoiceList)
        {
            long pclRequestID;
            NewDeptInvoiceList = new List<OutwardDrugClinicDeptInvoice>();

            foreach (OutwardDrugClinicDeptInvoice invoice in regInfo.InPatientInvoices)
            {
                if (invoice.PtRegistrationID.GetValueOrDefault(-1) > 0 && invoice.PtRegistrationID.GetValueOrDefault(-1) != regInfo.PtRegistrationID)
                {
                    continue;
                }
                invoice.PtRegistrationID = regInfo.PtRegistrationID;
                PatientProvider.Instance.SaveChangesOnOutwardDrugClinicInvoice(invoice, conn, tran);
            }
        }
        
        //private PatientRegistration SaveRegistration(PatientRegistration registrationInfo, out List<long> pclRequestIdList, out List<OutwardDrugInvoice> NewInvoiceList, DbConnection conn, DbTransaction tran, out List<PatientRegistrationDetail> NewRegDetailsID, out List<PatientRegistrationDetail> DeletedRegDetailsID)
        //{
        //    NewInvoiceList = new List<OutwardDrugInvoice>();
        //    pclRequestIdList = new List<long>();//Danh sach cac request id moi duoc them vao.
        //    NewRegDetailsID = new List<PatientRegistrationDetail>();//Danh sach cac RegistrationDetailsId moi duoc them vao.
        //    DeletedRegDetailsID = new List<PatientRegistrationDetail>(); //Danh sach cac Registration Details ID bi deleted.


        //    if (registrationInfo.PtRegistrationID == 0) //dang ky moi.
        //    {
        //        registrationInfo = CreateNewRegistration(registrationInfo, conn, tran, out pclRequestIdList, out NewRegDetailsID);
        //    }
        //    else //Cap nhat lai dang ky cu
        //    {
        //        if(registrationInfo.PatientTransaction != null)
        //        {
        //            registrationInfo.PatientTransaction.PatientTransactionDetails = PatientProvider.Instance.GetAlltransactionDetails(registrationInfo.PatientTransaction.TransactionID).ToObservableCollection();
        //        }

        //        StartCalculating(registrationInfo);

        //        IList<PatientRegistrationDetail> oldRegDetailsList = registrationInfo.PatientRegistrationDetails;

        //        //Duyet qua danh sach registration xem co thang nao thay doi thi luu lai.
        //        PatientProvider.Instance.SaveChangesOnRegistration(registrationInfo, conn, tran);
        //        OutwardDrugInvoice tempInvoice = null;

        //        SavePCLRequests(registrationInfo, conn, tran, out pclRequestIdList);
        //        ////Duyet qua danh sach CLS co thang nao them moi thi add vao,
        //        //if (registrationInfo.PCLRequests != null && registrationInfo.PCLRequests.Count > 0)
        //        //{
        //        //    long MedicalRecordID;
        //        //    CreatePatientMedialRecordIfNotExists(registrationInfo, out MedicalRecordID, conn, tran);

        //        //    SaveChangesOnPCLRequests(MedicalRecordID, registrationInfo, conn, tran, out pclRequestIdList);
        //        //}

        //        long outiID;
        //        bool bOK;
        //        bool isNewInvoice = false;
        //        //foreach (OutwardDrugInvoice invoice in registrationInfo.DrugInvoices)
        //        //{
        //        //    isNewInvoice = invoice.outiID <= 0;
        //        //    RefDrugGenericDetailsProvider.Instance.InsertOrUpdateDrugInvoices(invoice, out outiID, conn, tran);
        //        //    if (isNewInvoice && outiID > 0)
        //        //    {
        //        //        invoice.outiID = outiID;
        //        //        NewInvoiceList.Add(invoice);
        //        //    }
        //        //}
        //        //TODO: Duyet qua danh sach y cu nua. Hien gio chua lam.

        //        ////Luu danh sach thuoc, y dung cu, hoa chat cho noi tru.
        //        //if (registrationInfo.InPatientInvoices != null && registrationInfo.InPatientInvoices.Count > 0)
        //        //{
        //        //    List<OutwardDrugClinicDeptInvoice> outInvoiceList;
        //        //    SaveChangesOnInPatientInvoices(registrationInfo, conn, tran, out outInvoiceList);
        //        //}

        //        //Giu lai danh sach bi xoa.
        //        //List<PatientRegistrationDetail> deletedRegDetails = registrationInfo.PatientRegistrationDetails.Where(i => i.RecordState == RecordState.DELETED).ToList();
        //        //DeletedRegDetailsID = deletedRegDetails;

        //        //registrationInfo = GetRegistration(registrationInfo.PtRegistrationID, conn, tran);

        //        //if (registrationInfo.PatientRegistrationDetails != null)
        //        //{
        //        //    foreach (PatientRegistrationDetail outItem in registrationInfo.PatientRegistrationDetails)
        //        //    {
        //        //        if (!oldRegDetailsList.Contains(outItem))
        //        //        {
        //        //            NewRegDetailsID.Add(outItem);
        //        //        }
        //        //    }
        //        //}

        //        //AddServiceRegDetails(registrationInfo, deletedRegDetails);

        //        //StartCalculating(registrationInfo);
        //        ////Neu da co transaction thi kiem tra xem transaction co thay doi khong
        //        ////Neu chua co thi tao moi transaction.
        //        //PatientTransaction CurrentTransaction;
        //        //if (registrationInfo.PatientTransaction != null)
        //        //{
        //        //    CurrentTransaction = GetTransactionModifications(registrationInfo);
        //        //    PatientProvider.Instance.SaveChangesOnTransaction(CurrentTransaction, conn, tran);
        //        //}

        //        //if (registrationInfo.PatientTransaction != null)
        //        //{
        //        //    PatientProvider.Instance.SaveChangesOnTransaction(registrationInfo.PatientTransaction, conn, tran); 
        //        //}
        //    }

        //    //Test thu xem:

        //    //Lay lai dang ky cu.=> tinh toan thay doi tren transaction, Can bang Transaction neu co the
        //    PatientRegistration regInfo = GetRegistration(_RegistrationInfo.PtRegistrationID,conn,tran);
        //    StartCalculating(regInfo);
        //    if(regInfo.PatientTransaction != null)
        //    {
        //        regInfo.PatientTransaction.PatientTransactionDetails = PatientProvider.Instance.GetAlltransactionDetails(regInfo.PatientTransaction.TransactionID).ToObservableCollection();

        //        //Se dua ra ngoai dau ngoac.
        //        var balancedTranDetails = new List<PatientTransactionDetail>();
        //        if (regInfo.PatientRegistrationDetails != null)
        //        {
        //            foreach (var item in regInfo.PatientRegistrationDetails)
        //            {
        //                if (item.PaidTime != null)
        //                {
        //                    PatientTransactionDetail tranDetail = BalanceTransactionOfService(regInfo.PatientTransaction, item);
        //                    if(tranDetail != null)
        //                    {
        //                        balancedTranDetails.Add(tranDetail);
        //                    }
        //                }
        //            }
        //        }

        //        if (regInfo.PCLRequests != null)
        //        {
        //            foreach (var item in regInfo.PCLRequests)
        //            {
        //                if (item.PaidTime != null)
        //                {
        //                    PatientTransactionDetail tranDetail = BalanceTransactionOfPclRequest(regInfo.PatientTransaction, item);
        //                    if (tranDetail != null)
        //                    {
        //                        balancedTranDetails.Add(tranDetail);
        //                    }
        //                }
        //            }
        //        }
        //        if(balancedTranDetails.Count > 0)
        //        {
        //            PatientProvider.Instance.AddTransactionDetailList(regInfo.PatientTransaction.TransactionID,
        //                                                              balancedTranDetails,conn,tran);
        //        }
        //    }

        //    if (DeletedRegDetailsID != null && DeletedRegDetailsID.Count > 0)
        //    {
        //        foreach (PatientRegistrationDetail item in DeletedRegDetailsID)
        //        {
        //            PatientProvider.Instance.PatientQueue_MarkDelete(registrationInfo.PtRegistrationID, item.PtRegDetailID, registrationInfo.PatientID.GetValueOrDefault(),
        //                (long)(int)AllLookupValues.QueueType.KHAM_BENH, item.DeptLocation.DeptLocationID, conn, tran);

        //        }
        //    }

        //    //Doi voi nhung invoice moi them vao duoc, xoa cac item tuong ung trong queue
        //    if (NewInvoiceList != null && NewInvoiceList.Count > 0)
        //    {
        //        foreach (OutwardDrugInvoice item in NewInvoiceList)
        //        {
        //            if (item.IssueID.HasValue)
        //            {
        //                PatientProvider.Instance.PatientQueue_MarkDelete(registrationInfo.PtRegistrationID, item.IssueID.Value, registrationInfo.PatientID.GetValueOrDefault(),
        //                                (long)(int)AllLookupValues.QueueType.MUA_THUOC, -1, conn, tran);
        //            }

        //        }
        //    }


        //    if (NewRegDetailsID != null && NewRegDetailsID.Count > 0)
        //    {
        //        foreach (PatientRegistrationDetail item in NewRegDetailsID)
        //        {
        //            PatientQueue queue = new PatientQueue();
        //            queue.V_QueueType = (long)(int)AllLookupValues.QueueType.KHAM_BENH;
        //            queue.PatientID = registrationInfo.PatientID;
        //            queue.RegistrationID = registrationInfo.PtRegistrationID;
        //            queue.RegistrationDetailsID = item.PtRegDetailID;
        //            if (item.DeptLocation != null)
        //            {
        //                queue.DeptLocID = item.DeptLocation.DeptLocationID;
        //            }
        //            else
        //            {
        //                queue.DeptLocID = item.DeptLocID;
        //            }
        //            if (registrationInfo.Patient != null)
        //            {
        //                queue.FullName = registrationInfo.Patient.FullName;
        //            }

        //            PatientProvider.Instance.PatientQueue_Insert(queue, conn, tran);
        //        }
        //    }

        //    if (pclRequestIdList != null && pclRequestIdList.Count > 0)
        //    {
        //        foreach (long ID in pclRequestIdList)
        //        {
        //            PatientQueue queue = new PatientQueue();
        //            queue.V_QueueType = (long)(int)AllLookupValues.QueueType.PCL;
        //            queue.PatientID = registrationInfo.PatientID;
        //            queue.RegistrationID = registrationInfo.PtRegistrationID;
        //            queue.PatientPCLReqID = ID;
        //            if (registrationInfo.Patient != null)
        //            {
        //                queue.FullName = registrationInfo.Patient.FullName;
        //            }

        //            PatientProvider.Instance.PatientQueue_Insert(queue, conn, tran);
        //        }
        //    }
        //    return registrationInfo;
        //}

        
        /// <summary>
        /// Lưu thay đổi trên danh sách CLS
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        private void SavePCLRequests(PatientRegistration registrationInfo, DbConnection conn, DbTransaction tran, out List<long> PCLRequestIDList)
        {
            PCLRequestIDList = new List<long>();

            if (registrationInfo.PCLRequests != null && registrationInfo.PCLRequests.Count > 0)
            {
                long medicalRecordId;
                CreatePatientMedialRecordIfNotExists(registrationInfo, out medicalRecordId, conn, tran);

                SaveChangesOnPCLRequests(medicalRecordId, registrationInfo, conn, tran, out PCLRequestIDList);
            }
        }

        /// <summary>
        /// Cân bằng transaction của 1 dịch vụ đăng ký.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="?"></param>
        /// <returns>Trả về null nếu transaction của dịch vụ này đã cân bằng.
        /// Nếu dịch vụ chưa cân bằng => Trả về 1 object PatientTransactionDetail để thêm vào Transaction cho cân bằng</returns>
        private PatientTransactionDetail BalanceTransactionOfService(PatientTransaction transaction, PatientRegistrationDetail details)
        {
            if(transaction == null || details == null)
            {
                return null;
            }

            //Nếu chưa trả tiền thì khỏi cân bằng
            if(details.PaidTime == null)
            {
                return null;
            }

            decimal amount = 0;
            decimal priceDifference = 0;
            decimal amountCoPay = 0;
            decimal healthInsuranceRebate = 0;

            foreach (var tranItem in transaction.PatientTransactionDetails)
            {
                if(tranItem.PtRegDetailID == details.PtRegDetailID)
                {
                    amount += tranItem.Amount;
                    priceDifference += tranItem.PriceDifference.GetValueOrDefault(0);
                    amountCoPay += tranItem.AmountCoPay.GetValueOrDefault(0);
                    healthInsuranceRebate += tranItem.HealthInsuranceRebate.GetValueOrDefault(0);
                }
            }

            amount = details.TotalInvoicePrice - amount;
            priceDifference = details.TotalPriceDifference - priceDifference;
            amountCoPay = details.TotalCoPayment - amountCoPay;
            healthInsuranceRebate = details.TotalHIPayment - healthInsuranceRebate;

            if(amount == 0 && priceDifference == 0 && amountCoPay == 0 && healthInsuranceRebate == 0)
            {
                return null;
            }

            var tranDetail = new PatientTransactionDetail();

            tranDetail.OutwBloodInvoiceID = null;
            tranDetail.OutDMedRscrID = null;
            tranDetail.StaffID = null;
            tranDetail.PtRegDetailID = details.PtRegDetailID;
            tranDetail.outiID = null;
            tranDetail.TransactionID = transaction.TransactionID;
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

            return tranDetail;
        }

        private PatientTransactionDetail BalanceTransactionOfPclRequest(PatientTransaction transaction, PatientPCLRequest request)
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

            amount = request.TotalInvoicePrice - amount;
            priceDifference = request.TotalPriceDifference - priceDifference;
            amountCoPay = request.TotalCoPayment - amountCoPay;
            healthInsuranceRebate = request.TotalHIPayment - healthInsuranceRebate;

            if (amount == 0 && priceDifference == 0 && amountCoPay == 0 && healthInsuranceRebate == 0)
            {
                return null;
            }

            var tranDetail = new PatientTransactionDetail();

            tranDetail.OutwBloodInvoiceID = null;
            tranDetail.OutDMedRscrID = null;
            tranDetail.StaffID = null;
            tranDetail.PtRegDetailID = null;
            tranDetail.outiID = null;
            tranDetail.TransactionID = transaction.TransactionID;
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
            tranDetail.PCLRequestID = request.PatientPCLReqID;

            return tranDetail;
        }



        /// <summary>
        /// Lưu những thay đổi trên đăng ký này
        /// </summary>
        /// <param name="registrationInfo"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <param name="newRegDetailsId"></param>
        /// <param name="pclRequestIdList"></param>
        /// <param name="newInvoiceList"></param>
        /// <param name="deletedRegDetailsId"></param>
        /// <returns></returns>
        private void SaveChangesOnRegistration(PatientRegistration registrationInfo, DbConnection conn, DbTransaction tran, out List<PatientRegistrationDetail> newRegDetailsId, out List<long> pclRequestIdList, out List<OutwardDrugInvoice> newInvoiceList, out List<PatientRegistrationDetail> deletedRegDetailsId)
        {
            newInvoiceList = new List<OutwardDrugInvoice>();
            pclRequestIdList = new List<long>();//Danh sach cac request id moi duoc them vao.
            newRegDetailsId = new List<PatientRegistrationDetail>();//Danh sach cac RegistrationDetailsId moi duoc them vao.
            deletedRegDetailsId = new List<PatientRegistrationDetail>(); //Danh sach cac Registration Details ID bi deleted.

            if (registrationInfo.PtRegistrationID == 0) //dang ky moi.
            {
                registrationInfo = CreateNewRegistration(registrationInfo, conn, tran, out pclRequestIdList, out newRegDetailsId);
            }
            else //Cap nhat lai dang ky cu
            {
                StartCalculating(registrationInfo);

                //Duyet qua danh sach registration xem co thang nao thay doi thi luu lai.
                PatientProvider.Instance.SaveChangesOnRegistration(registrationInfo, conn, tran);
                OutwardDrugInvoice tempInvoice = null;

                SavePCLRequests(registrationInfo, conn, tran, out pclRequestIdList);

                long outiID;
                bool bOK;
                bool isNewInvoice = false;
                //foreach (OutwardDrugInvoice invoice in registrationInfo.DrugInvoices)
                //{
                //    isNewInvoice = invoice.outiID <= 0;
                //    RefDrugGenericDetailsProvider.Instance.InsertOrUpdateDrugInvoices(invoice, out outiID, conn, tran);
                //    if (isNewInvoice && outiID > 0)
                //    {
                //        invoice.outiID = outiID;
                //        NewInvoiceList.Add(invoice);
                //    }
                //}
                //TODO: Duyet qua danh sach y cu nua. Hien gio chua lam.

                ////Luu danh sach thuoc, y dung cu, hoa chat cho noi tru.
                //if (registrationInfo.InPatientInvoices != null && registrationInfo.InPatientInvoices.Count > 0)
                //{
                //    List<OutwardDrugClinicDeptInvoice> outInvoiceList;
                //    SaveChangesOnInPatientInvoices(registrationInfo, conn, tran, out outInvoiceList);
                //}

                //Giu lai danh sach bi xoa.
                //List<PatientRegistrationDetail> deletedRegDetails = registrationInfo.PatientRegistrationDetails.Where(i => i.RecordState == RecordState.DELETED).ToList();
                //DeletedRegDetailsID = deletedRegDetails;

                //registrationInfo = GetRegistration(registrationInfo.PtRegistrationID, conn, tran);

                //if (registrationInfo.PatientRegistrationDetails != null)
                //{
                //    foreach (PatientRegistrationDetail outItem in registrationInfo.PatientRegistrationDetails)
                //    {
                //        if (!oldRegDetailsList.Contains(outItem))
                //        {
                //            NewRegDetailsID.Add(outItem);
                //        }
                //    }
                //}
            }

            if (deletedRegDetailsId != null && deletedRegDetailsId.Count > 0)
            {
                foreach (PatientRegistrationDetail item in deletedRegDetailsId)
                {
                    PatientProvider.Instance.PatientQueue_MarkDelete(registrationInfo.PtRegistrationID, item.PtRegDetailID, registrationInfo.PatientID.GetValueOrDefault(),
                        (long)(int)AllLookupValues.QueueType.KHAM_BENH, item.DeptLocation.DeptLocationID, conn, tran);

                }
            }

            //Doi voi nhung invoice moi them vao duoc, xoa cac item tuong ung trong queue
            if (newInvoiceList != null && newInvoiceList.Count > 0)
            {
                foreach (OutwardDrugInvoice item in newInvoiceList)
                {
                    if (item.IssueID.HasValue)
                    {
                        PatientProvider.Instance.PatientQueue_MarkDelete(registrationInfo.PtRegistrationID, item.IssueID.Value, registrationInfo.PatientID.GetValueOrDefault(),
                                        (long)(int)AllLookupValues.QueueType.MUA_THUOC, -1, conn, tran);
                    }

                }
            }


            if (newRegDetailsId != null && newRegDetailsId.Count > 0)
            {
                foreach (PatientRegistrationDetail item in newRegDetailsId)
                {
                    PatientQueue queue = new PatientQueue();
                    queue.V_QueueType = (long)(int)AllLookupValues.QueueType.KHAM_BENH;
                    queue.PatientID = registrationInfo.PatientID;
                    queue.RegistrationID = registrationInfo.PtRegistrationID;
                    queue.RegistrationDetailsID = item.PtRegDetailID;
                    if (item.DeptLocation != null)
                    {
                        queue.DeptLocID = item.DeptLocation.DeptLocationID;
                    }
                    else
                    {
                        queue.DeptLocID = item.DeptLocID;
                    }
                    if (registrationInfo.Patient != null)
                    {
                        queue.FullName = registrationInfo.Patient.FullName;
                    }

                    PatientProvider.Instance.PatientQueue_Insert(queue, conn, tran);
                }
            }

            if (pclRequestIdList != null && pclRequestIdList.Count > 0)
            {
                foreach (long ID in pclRequestIdList)
                {
                    PatientQueue queue = new PatientQueue();
                    queue.V_QueueType = (long)(int)AllLookupValues.QueueType.PCL;
                    queue.PatientID = registrationInfo.PatientID;
                    queue.RegistrationID = registrationInfo.PtRegistrationID;
                    queue.PatientPCLReqID = ID;
                    if (registrationInfo.Patient != null)
                    {
                        queue.FullName = registrationInfo.Patient.FullName;
                    }

                    PatientProvider.Instance.PatientQueue_Insert(queue, conn, tran);
                }
            }
        }

        /// <summary>
        /// Cân bằng Transaction cho đăng ký. Phải đưa thông tin đầy đủ của đăng ký.
        /// </summary>
        /// <param name="regInfo"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        private void BalanceTransaction(PatientRegistration regInfo,DbConnection conn,DbTransaction tran)
        {
            StartCalculating(regInfo);
            if (regInfo.PatientTransaction != null)
            {
                //regInfo.PatientTransaction.PatientTransactionDetails = PatientProvider.Instance.GetAlltransactionDetails(regInfo.PatientTransaction.TransactionID).ToObservableCollection();

                var balancedTranDetails = new List<PatientTransactionDetail>();
                if (regInfo.PatientRegistrationDetails != null)
                {
                    foreach (var item in regInfo.PatientRegistrationDetails)
                    {
                        if (item.PaidTime != null)
                        {
                            PatientTransactionDetail tranDetail = BalanceTransactionOfService(regInfo.PatientTransaction, item);
                            if (tranDetail != null)
                            {
                                balancedTranDetails.Add(tranDetail);
                            }
                        }
                    }
                }

                if (regInfo.PCLRequests != null)
                {
                    foreach (var item in regInfo.PCLRequests)
                    {
                        if (item.PaidTime != null)
                        {
                            PatientTransactionDetail tranDetail = BalanceTransactionOfPclRequest(regInfo.PatientTransaction, item);
                            if (tranDetail != null)
                            {
                                balancedTranDetails.Add(tranDetail);
                            }
                        }
                    }
                }
                if (balancedTranDetails.Count > 0)
                {
                    PatientProvider.Instance.AddTransactionDetailList(regInfo.PatientTransaction.TransactionID,
                                                                      balancedTranDetails, conn, tran);
                }
            }

        }

        /// <summary>
        /// Cập nhật PaidTime cho những item muốn tính tiền
        /// </summary>
        /// <param name="paidRegDetailsList"></param>
        /// <param name="paidPclRequestList"></param>
        /// <param name="paidDrugInvoice"></param>
        /// <param name="paidMedItemList"></param>
        /// <param name="paidChemicalItemList"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        private void UpdatePaidTime(PatientRegistration regInfo, DateTime paidTime, List<long> colPaidRegDetails,
                                        List<long> colPaidPclRequests,
                                        List<long> colPaidDrugInvoice,
                                        List<long> colPaidMedItemList,
                                        List<long> colPaidChemicalItem, DbConnection conn, DbTransaction tran)
        {
            DateTime now = DateTime.Now;

            List<PatientRegistrationDetail> paidRegDetailsList = GetRegDetailsToPay(regInfo, colPaidRegDetails);
            List<PatientPCLRequest> paidPclRequestList = GetPclRequestToPay(_RegistrationInfo, colPaidPclRequests);
            List<OutwardDrugInvoice> paidDrugInvoice = new List<OutwardDrugInvoice>();
            List<OutwardDrugClinicDeptInvoice> paidMedItemList = null;
            List<OutwardDrugClinicDeptInvoice> paidChemicalItemList = null;

            if (paidRegDetailsList != null && paidRegDetailsList.Count > 0)
            {
                foreach (var details in paidRegDetailsList)
                {
                    if (details.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && details.RecordState == RecordState.MODIFIED)
                    {
                        details.RefundTime = now;
                    }
                }
            }
            PatientProvider.Instance.UpdatePaidTime(paidRegDetailsList, paidPclRequestList, paidDrugInvoice, paidMedItemList, paidChemicalItemList, conn, tran);
        }

        private void ProcessPayment(PatientPayment paymentDetails,long transactionId,DbConnection conn,DbTransaction tran)
        {
            long paymentID;
            paymentDetails.PaymentDate = DateTime.Now;
            PatientProvider.Instance.ProcessPayment(paymentDetails, transactionId, out paymentID, conn, tran);
            paymentDetails.PtPmtID = paymentID;
        }
    }
}