/*
 * 20170517 #001 CMN:   Thêm giá trị để cập nhật tên bệnh nhân không giới hạn với tài khoản quản trị
 * 20170522 #002 CMN:   Added variable to check InPt 5 year HI without paid enough
 * 20180508 #003 TxD:   Commented out all BeginTransaction in this class because it could have caused dead lock in DB (suspection only at this stage)
 * 20180522 #004 TTM:   Comment điều kiện không cho chỉnh sửa ngày đến < ngày hiện tại.
 * 20180814 #005 TTM:   Tạo mới method để thực hiện thêm mới và cập nhật bệnh nhân, thẻ BHYT, giấy CV 1 lần.
 * 20181119 #006 TTM:   BM 0005257: Tạo hàm lấy dữ liệu bệnh nhân đang nằm tại khoa.
 * 20181212 #007 TTM:   Tạo hàm cập nhật BasicDiagTreatment.
 * 20190506 #008 TTM:   BM 0006818: Lấy giá cho 1 ca load từ cuộc hẹn.
 * 20190510 #010 TTM:   BM 0006681: Sử dụng bảng InsuranceBenefitCategories để quản lý thẻ và quyền lợi của bệnh nhân
 * 20190623 #011 TTM:   BM 0011797: Lấy dữ liệu đăng ký của bệnh nhân theo bác sĩ và ngày. Đọc dữ liệu toa thuốc và CLS 
 * 20190831 #012 TTM:   BM 0013214: Thêm chức năng Kiểm tra Ngày thuốc
 * 20190908 #013 TTM:   BM 0013139: [Khám sức khoẻ] Import Bệnh nhân vào chương trình 
 * 20191002 #014 TTM:   BM 0017405: [Đề nghị nhập viện] Mặc định khoa nhập viện từ đề nghị nhập viện 
 * 20191119 #015 TTM:   BM 0019591: Thêm 1 danh sách BN chờ nhập viện bên OutstandingTask
 * 20200320 #016 TTM:   BM 0027031: [Đề nghị nhập viện] Lỗi tạo dịch vụ ngoại trú cho đăng ký nội trú do lấy đăng ký ngoại trú tạo đăng ký nội trú mà đăng ký ngoại trú có
 *                                  thông tin AppoimentID => Đọc dữ liệu chi tiết dịch vụ, CLS ngoại trú xuống store nội trú đi lưu => Sai.
 * 20200615 #017 TBL:   BM 0038208: Sáp nhập ca khám ngoại trú vào nội trú.
 * 20200709 #018 TTM:   BM 0039356: Tính đúng tính đủ khi đưa dịch vụ vào nội trú.
 * 20200807 #019 TNHX: Them Cau hinh xac nhan BN cap cuu ngoai tru
 * 20200812 #020 TMT:   BM 0041456: Fix lỗi sáp nhập đăng ký có chứa CLS giãn cách bị lỗi.
 * 20201211 #021 TNHX:   BM: Thêm func cập nhật đã đẩy PAC
 * 20210315 #022 BAOLQ:   Task 237 Lấy dữ liệu bệnh nhân xuất viện
 * 20210319 #022 TNHX: 219 Thêm điều kiện bỏ qua các phiếu khám không thu tiền để chuyển chi phí vào nội trú
 * 20211203 #023 TNHX: Lấy dsách PTTT từ phòng mổ
 * 20220510 #024 TNHX: 1340: Chuyển chi phí vào nội trú khi sát nhập(thuốc cản quang)
 * 20220530 #025 BLQ: Kiểm tra thời gian thao tác của bác sĩ
 * 20220615 #026 DatTB: Thay đổi thông báo sáp nhập cho đăng ký nội trú
 * 20220625 #027 BLQ: 
 *  + Lấy ICD chính cuối cùng của đợt điều trị ngoại trú
 *  + Lấy danh sách ICD cấu hình cho điều trị ngoại trú
 * 20220812 #028 DatTB: Chỉnh sửa màn hình hồ sơ ĐTNT
 * + Thêm hàm check toa thuốc để kiểm tra chỉnh sửa loại hình ĐTNT
 * 20220823 #029 QTD:  Cập nhật lại tình trạng cấp cứu khi xác nhận BHYT
 * 20221005 #030 BLQ: Thêm chức năng thẻ khách hàng
 * 20221114 #031 DatTB: Thêm biến để nhận biết gửi HSBA so với điều dưỡng xác nhận xuất viện ra viện.
 * 20230210 #032 BLQ: Thêm hàm check trước khi sát nhập
 * 20230624 #033 DatTB: Thay đổi điều kiện gàng buộc ICD
  */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Services;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using AxLogging;
using DataEntities;
using eHCMS.Caching;
using eHCMS.Configurations;
using eHCMS.Services.Core;
using ErrorLibrary;
using ErrorLibrary.Resources;
using EventManagementService;
using Service.Core.Common;
using eHCMSLanguage;
using eHCMSBillPaymt;
using aEMR.DataAccessLayer.Providers;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace PatientRegistrationService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [KnownType(typeof(AxException))]
    public class PatientRegistrationService : eHCMS.WCFServiceCustomHeader, IPatientRegistrationService
    {
        private string _ModuleName = "Patient Registration";

        public PatientRegistrationService()
        {
            int currentID = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        #region IPatientRegistrationService Members

        public List<Patient> GetPatientAll()
        {
            try
            {
                return PatientProvider.Instance.GetPatientAll();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return null;
            }
        }

        [SingleResult]
        public Patient GetPatientByID(long patientID, bool ToRegisOutPt)
        {
            Patient patient = new Patient();
            patient = PatientProvider.Instance.GetPatientByID(patientID, ToRegisOutPt);
            if (patient != null && patient.CurrentHealthInsurance != null)
            {
                patient.CurrentHealthInsurance = CalculateRealHIBenefit(patient.CurrentHealthInsurance);
            }
            return patient;
        }

        [SingleResult]
        public Patient GetPatientByID_InPt(long patientID, bool bGetOutPtRegInfo, bool ToRegisOutPt)
        {
            Patient patient = new Patient();
            patient = PatientProvider.Instance.GetPatientByID_InPt(patientID, ToRegisOutPt);
            if (patient != null && patient.CurrentHealthInsurance != null)
            {
                patient.CurrentHealthInsurance = CalculateRealHIBenefit(patient.CurrentHealthInsurance);
            }
            return patient;
        }


        [SingleResult]
        public PatientRegistration GetPatientByAppointment(PatientAppointment patientApp)
        {
            var pr = new PatientRegistration();
            if (patientApp.V_AppointmentType != (long)AllLookupValues.AppointmentType.HEN_DTNT_MOT_DK)
            {
                pr.ExamDate = DateTime.Now;
                Patient patient = new Patient();
                patient = PatientProvider.Instance.GetPatientByID(patientApp.Patient.PatientID);
                if (patient != null && patient.CurrentHealthInsurance != null)
                {
                    patient.CurrentHealthInsurance = CalculateRealHIBenefit(patient.CurrentHealthInsurance);
                }
                pr.PatientClassification = new PatientClassification() { PatientClassID = 1 };
                pr.Patient = patient;
                PatientAppointment appointment;
                appointment = AppointmentProvider.Instance.GetAppointmentByID(patientApp.AppointmentID, patientApp.HosClientContractID, patientApp.Patient == null ? patientApp.PatientID : patientApp.Patient.PatientID);
                var createdDate = DateTime.Now;
                if (appointment != null)
                {
                    pr.OutPtTreatmentProgramID = appointment.OutPtTreatmentProgramID;
                    long CurrentHosClientContractID = 0;
                    if (appointment.V_AppointmentType == (long)AllLookupValues.AppointmentType.HEN_KHAM_SUC_KHOE)
                    {
                        pr.PatientClassification = new PatientClassification() { PatientClassID = (long)ePatientClassification.CompanyHealthRecord };
                        pr.PatientClassID = (long)ePatientClassification.CompanyHealthRecord;
                        CurrentHosClientContractID = appointment.ClientContract == null ? 0 : appointment.ClientContract.HosClientContractID;
                    }
                    pr.Appointment = appointment;
                    pr.AppointmentID = appointment.AppointmentID;
                    pr.AppointmentDate = appointment.ApptDate;
                    foreach (var item in appointment.PatientApptServiceDetailList)
                    {
                        var regDetails = new PatientRegistrationDetail();
                        regDetails.HosClientContractID = CurrentHosClientContractID;
                        regDetails.RefMedicalServiceItem = item.MedService;
                        if (regDetails.RefMedicalServiceItem.RefMedicalServiceType == null
                            || regDetails.RefMedicalServiceItem.RefMedicalServiceType.MedicalServiceTypeID < 1)
                        {
                            regDetails.RefMedicalServiceItem.RefMedicalServiceType = new RefMedicalServiceType() { MedicalServiceTypeID = item.MedService.MedicalServiceTypeID.GetValueOrDefault(-1) };
                        }
                        regDetails.DeptLocation = item.DeptLocation;
                        regDetails.CreatedDate = createdDate;
                        regDetails.Qty = 1;
                        regDetails.RecordState = RecordState.ADDED;
                        regDetails.CanDelete = true;
                        regDetails.FromAppointment = true;
                        regDetails.AppointmentDate = appointment.ApptDate;
                        regDetails.ServiceSeqNum = item.ServiceSeqNum;
                        regDetails.StaffID = appointment.DoctorStaffID;
                        regDetails.AppointmentID = pr.AppointmentID;
                        regDetails.ApptSvcDetailID = item.ApptSvcDetailID;
                        regDetails.ConsultationRoomStaffAllocID = item.ConsultationRoomStaffAllocID;
                        regDetails.ClientContractSvcPtID = item.ClientContractSvcPtID;
                        regDetails.PackServDetailID = item.PackServDetailID;
                        pr.PatientRegistrationDetails.Add(regDetails);
                    }
                    if (appointment.PatientApptPCLRequestsList != null)
                    {
                        pr.PCLRequests = new ObservableCollection<PatientPCLRequest>();
                        foreach (var request in appointment.PatientApptPCLRequestsList)
                        {
                            var newReq = new PatientPCLRequest();
                            newReq.Diagnosis = request.Diagnosis;
                            newReq.ICD10List = request.ICD10List;
                            newReq.DoctorComments = request.DoctorComments;

                            newReq.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
                            newReq.CreatedDate = createdDate;

                            //20190605 TTM: Chuyển RecordState từ ADDED => DETACHED vì khi lưu và trả tiền chỉ nhận giá trị DETACHED Hoặc UNCHANGED thôi không có xài ADDED cho CLS.
                            //newReq.RecordState = RecordState.ADDED;
                            newReq.RecordState = RecordState.DETACHED;

                            newReq.DoctorStaffID = appointment.DoctorStaffID;
                            //newReq.PCLDeptLocID = request.DeptLocation != null ? request.DeptLocation.DeptLocationID : request.DeptLocationID;
                            //newReq.ReqFromDeptLocID = request.ReqFromDeptLocID;
                            //KMx: Phải lấy V_PCLMainCategory, nếu không khi load cuộc hẹn lên, xóa 1 PCL sẽ bị lỗi WCF.
                            newReq.V_PCLMainCategory = request.V_PCLMainCategory;
                            newReq.CanDelete = true;
                            newReq.HosClientContractID = CurrentHosClientContractID;
                            pr.PCLRequests.Add(newReq);

                            if (request.ObjPatientApptPCLRequestDetailsList != null)
                            {
                                newReq.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                                foreach (var reqDetails in request.ObjPatientApptPCLRequestDetailsList)
                                {
                                    PCLExamType examType = new PCLExamType();
                                    examType = PatientProvider.Instance.GetPclExamTypeByID(reqDetails.PCLExamTypeID, appointment.HosClientContractID);
                                    if (examType != null)
                                    {
                                        var newReqDetail = CreateRequestDetailsForPCLExamType(examType);
                                        newReqDetail.RecordState = RecordState.ADDED;
                                        newReqDetail.CreatedDate = createdDate;
                                        newReqDetail.CanDelete = true;
                                        newReqDetail.FromAppointment = true;
                                        newReqDetail.AppointmentDate = appointment.ApptDate;
                                        newReqDetail.ServiceSeqNum = reqDetails.ServiceSeqNum;
                                        newReqDetail.StaffID = appointment.DoctorStaffID;
                                        newReqDetail.AppointmentID = pr.AppointmentID;
                                        newReqDetail.HosClientContractID = CurrentHosClientContractID;
                                        newReqDetail.ClientContractSvcPtID = reqDetails.ClientContractSvcPtID;
                                        newReqDetail.IsCountHI = reqDetails.IsCountHI;
                                        newReq.PatientPCLRequestIndicators.Add(newReqDetail);
                                    }
                                }
                            }
                        }
                    }
                }
                //▼====== #008: Trước khi trả về thông tin hẹn của bệnh nhân cần tính lại giá để load sẽ có giá => Nếu không sẽ không có giá khi lưu và trả tiền.
                GetPriceForAppointment(pr);
                //▲====== #008
                return pr;
            }
            else
            {
                return GetRegistrationOutTreatmentOneRegitration(patientApp);
            }
        }

        public PatientRegistration GetRegistrationOutTreatmentOneRegitration(PatientAppointment patientApp)
        {
            PatientRegistration pr = RegAndPaymentProcessorBase.GetRegistrationTxd(patientApp.PtRegistrationID, (int)AllLookupValues.PatientFindBy.NGOAITRU);
            if (pr != null)
            {
                pr.PatientClassification = new PatientClassification() { PatientClassID = 1 };
                if (pr.Patient != null && pr.Patient.CurrentHealthInsurance != null)
                {
                    pr.PatientClassification = new PatientClassification() { PatientClassID = 2 };
                }
                PatientAppointment appointment;
                appointment = AppointmentProvider.Instance.GetAppointmentByID(patientApp.AppointmentID, patientApp.HosClientContractID, patientApp.Patient == null ? patientApp.PatientID : patientApp.Patient.PatientID);
                var createdDate = DateTime.Now;
                if (appointment != null)
                {
                    pr.OutPtTreatmentProgramID = appointment.OutPtTreatmentProgramID;
                    pr.Appointment = appointment;
                    pr.AppointmentID = appointment.AppointmentID;
                    pr.AppointmentDate = appointment.ApptDate;
                    foreach (var item in appointment.PatientApptServiceDetailList)
                    {
                        var regDetails = new PatientRegistrationDetail();
                        regDetails.HosClientContractID = 0;
                        regDetails.RefMedicalServiceItem = item.MedService;
                        if (regDetails.RefMedicalServiceItem.RefMedicalServiceType == null
                            || regDetails.RefMedicalServiceItem.RefMedicalServiceType.MedicalServiceTypeID < 1)
                        {
                            regDetails.RefMedicalServiceItem.RefMedicalServiceType = new RefMedicalServiceType() { MedicalServiceTypeID = item.MedService.MedicalServiceTypeID.GetValueOrDefault(-1) };
                        }
                        regDetails.DeptLocation = item.DeptLocation;
                        regDetails.CreatedDate = createdDate;
                        regDetails.Qty = 1;
                        regDetails.RecordState = RecordState.ADDED;
                        regDetails.CanDelete = true;
                        regDetails.FromAppointment = true;
                        regDetails.AppointmentDate = appointment.ApptDate;
                        regDetails.ServiceSeqNum = item.ServiceSeqNum;
                        regDetails.StaffID = appointment.DoctorStaffID;
                        regDetails.AppointmentID = pr.AppointmentID;
                        regDetails.ApptSvcDetailID = item.ApptSvcDetailID;
                        regDetails.ConsultationRoomStaffAllocID = item.ConsultationRoomStaffAllocID;
                        regDetails.ClientContractSvcPtID = item.ClientContractSvcPtID;
                        regDetails.PackServDetailID = item.PackServDetailID;
                        pr.PatientRegistrationDetails.Add(regDetails);
                    }
                    if (appointment.PatientApptPCLRequestsList != null)
                    {
                        pr.PCLRequests = new ObservableCollection<PatientPCLRequest>();
                        foreach (var request in appointment.PatientApptPCLRequestsList)
                        {
                            var newReq = new PatientPCLRequest();
                            newReq.Diagnosis = request.Diagnosis;
                            newReq.ICD10List = request.ICD10List;
                            newReq.DoctorComments = request.DoctorComments;

                            newReq.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
                            newReq.CreatedDate = createdDate;
                            newReq.RecordState = RecordState.DETACHED;

                            newReq.DoctorStaffID = appointment.DoctorStaffID;
                            newReq.V_PCLMainCategory = request.V_PCLMainCategory;
                            newReq.CanDelete = true;
                            newReq.HosClientContractID = 0;
                            pr.PCLRequests.Add(newReq);

                            if (request.ObjPatientApptPCLRequestDetailsList != null)
                            {
                                newReq.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                                foreach (var reqDetails in request.ObjPatientApptPCLRequestDetailsList)
                                {
                                    PCLExamType examType = new PCLExamType();
                                    examType = PatientProvider.Instance.GetPclExamTypeByID(reqDetails.PCLExamTypeID, appointment.HosClientContractID);
                                    if (examType != null)
                                    {
                                        var newReqDetail = CreateRequestDetailsForPCLExamType(examType);
                                        newReqDetail.RecordState = RecordState.ADDED;
                                        newReqDetail.CreatedDate = createdDate;
                                        newReqDetail.CanDelete = true;
                                        newReqDetail.FromAppointment = true;
                                        newReqDetail.AppointmentDate = appointment.ApptDate;
                                        newReqDetail.ServiceSeqNum = reqDetails.ServiceSeqNum;
                                        newReqDetail.StaffID = appointment.DoctorStaffID;
                                        newReqDetail.AppointmentID = pr.AppointmentID;
                                        newReqDetail.HosClientContractID = 0;
                                        newReqDetail.ClientContractSvcPtID = reqDetails.ClientContractSvcPtID;
                                        newReq.PatientPCLRequestIndicators.Add(newReqDetail);
                                    }
                                }
                            }
                        }
                    }
                }
                GetPriceForAppointment(pr);
            }
            return pr;
        }
        //▼====== #008
        public void GetPriceForAppointment(PatientRegistration curRegistration)
        {
            if (curRegistration == null)
            {
                return;
            }
            foreach (var tmpRegistration in curRegistration.GetSaveInvoiceItem())
            {
                tmpRegistration.GetItemPrice(curRegistration, DateTime.Now);
                tmpRegistration.GetItemTotalPrice();
            }
            curRegistration.CorrectRegistrationDetails(Globals.AxServerSettings.HealthInsurances.SpecialRuleForHIConsultationApplied, null, Globals.AxServerSettings.HealthInsurances.HIPercentOnDifDept, Globals.AxServerSettings.HealthInsurances.HiPolicyMinSalary, Globals.AxServerSettings.PharmacyElements.OnlyRoundResultForOutward, Globals.AxServerSettings.HealthInsurances.FullHIBenefitForConfirm, true, Globals.AxServerSettings.CommonItems.AddingServicesPercent, Globals.AxServerSettings.HealthInsurances.FullHIOfServicesForConfirm
                , Globals.AxServerSettings.CommonItems.AddingHIServicesPercent);
        }
        //▲====== #008
        [SingleResult]
        public Patient GetPatientByIDFullInfo(long patientID, bool ToRegisOutPt)
        {

            try
            {
                AxLogger.Instance.LogInfo("Start getting patient info.", CurrentUser);
                Patient patient = new Patient();
                patient = PatientProvider.Instance.GetPatientByID_Full(patientID, ToRegisOutPt);
                if (patient != null)
                {
                    if (patient.HealthInsurances != null)
                    {
                        foreach (var hiItem in patient.HealthInsurances)
                        {
                            bool ActiveHIFlag = false;
                            hiItem.CanDelete = hiItem.CanEdit = !hiItem.Used;
                            //HPT_10062016: thao tác gán dưới đây chỉ đúng khi mỗi bệnh nhân trong cùng một thời điểm đảm bảo chỉ có duy nhất một thẻ BHYT được Active
                            /* 
                             * Stored đọc danh sách thẻ BHYT từ Database thực hiện kiểm tra hết hạn. 
                             * Những thẻ đã hết hạn mặc dù được lưu trong bảng có IsActive = true tuy nhiên khi kết hợp với các kiểm tra trong Select sẽ bị biến thành false (hoặc null) nếu vi phạm các ràng buộc về thời gian, ví dụ hết hạn.
                             * Do đó phải lấy lên trạng thái IsHistoryActive, tức là trạng thái IsActive nguyên thủy được lưu dưới bảng của thẻ BHYT khi chưa thực hiện kiểm tra
                             * Nếu cấu hình cho phép và bệnh nhân là trẻ em dưới sáu tuổi, ngày kiểm tra là <= 30/9, thì kích hoạt lại thẻ đã hết hạn được sử dụng lần gần nhất trong quá khứ.
                             * Để đảm bảo mỗi bệnh nhân trong một thời điểm chỉ có duy nhất một thẻ BHYT được kích hoạt, đề phòng dữ liệu select có sai sót, thêm một biến ActiveHIFlag để đánh dấu xem đã có thẻ nào được kích hoạt hay chưa. 
                             * Nếu chưa thì mới thực hiện kích hoạt lại thẻ
                             * Đây là bước đầu tiên quyết định việc bệnh nhân dưới 6 tuổi có được sử dụng thẻ BHYT hết hạn hay không. 
                             * Nếu danh sách thẻ BHYT được lấy lên chưa có thẻ nào được kích hoạt, luật phần mềm lại không cho phép sửa đổi thông tin thẻ BHYT đã hết hạn nên xem như không thể xác nhận quyền lợi bào hiểm hoặc tiến hành đăng ký
                             */
                            if (!ActiveHIFlag && Globals.AxServerSettings.InRegisElements.AllowChildUnder6YearsOldUseHIOverDate && Globals.CanRegHIChildUnder6YearsOld(patient.Age.GetValueOrDefault()) && hiItem.IsHistoryActive == true)
                            {
                                hiItem.IsActive = true;
                                ActiveHIFlag = true;
                            }
                        }
                    }

                    if (patient.PaperReferals != null)
                    {
                        foreach (var item in patient.PaperReferals)
                        {
                            item.CanDelete = item.CanEdit = !item.Used;
                        }
                    }

                    // TxD 01/02/2018: Because Table HealthInsuranceHIPatientBenefit is NOT USED so the following is commented out
                    //if (patient.CurrentHealthInsurance != null)
                    //{
                    //    patient.CurrentHealthInsurance = CalculateRealHIBenefit(patient.CurrentHealthInsurance);

                    //    if (patient.HealthInsurances != null)
                    //    {
                    //        HealthInsurance activeItem = patient.HealthInsurances.Where(hi => hi.IsActive).FirstOrDefault();
                    //        if (activeItem != null)
                    //        {
                    //            if (activeItem.HIID == patient.CurrentHealthInsurance.HIID)
                    //            {
                    //                activeItem.ActiveHealthInsuranceHIPatientBenefit = patient.CurrentHealthInsurance.ActiveHealthInsuranceHIPatientBenefit;
                    //                activeItem.CalcBenefit();
                    //            }
                    //            else
                    //            {
                    //                activeItem.ActiveHealthInsuranceHIPatientBenefit = PatientProvider.Instance.GetActiveHIBenefit(activeItem.HIID);
                    //                activeItem.CalcBenefit();
                    //            }
                    //        }
                    //    }
                    //}

                }
                AxLogger.Instance.LogInfo("End of getting patient info. Status: Success.", CurrentUser);
                return patient;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of getting patient info. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdatePatientBloodType(long PatientID, int? BloodTypeID)
        {
            try
            {
                AxLogger.Instance.LogInfo("End of Updating blood type.", CurrentUser);
                return PatientProvider.Instance.UpdatePatientBloodTypeID(PatientID, BloodTypeID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Updating blood type. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<BloodType> GetAllBloodTypes()
        {
            try
            {
                return PatientProvider.Instance.GetAllBloodTypes();
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Updating blood type. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeletePatient(Patient patient, long StaffID)
        {
            return DeletePatientByID(patient.PatientID, StaffID);
        }

        private void Test(Patient p)
        {
            throw new NotImplementedException();

        }

        public bool ExtractHealthInsuranceCard(string CardID, out string classification, out int interest,
                                                out string provinceCode, out string districtCode, out string unitCode, out int order, out string errorMessage)
        {
            classification = "";
            interest = 0;
            provinceCode = "";
            districtCode = "";
            unitCode = "";
            order = 0;

            errorMessage = "";

            if (CardID.Length != 15)
            {
                errorMessage = "Required length: 15";
                return false;
            }

            classification = CardID.Substring(0, 2);
            if (!int.TryParse(CardID.Substring(2, 1), out interest))
            {
                errorMessage = "Invalid Interest Code";
                return false;
            }
            provinceCode = CardID.Substring(3, 2);
            districtCode = CardID.Substring(5, 2);
            unitCode = CardID.Substring(7, 3);

            if (!int.TryParse(CardID.Substring(10), out order))
            {
                errorMessage = "Invalid Order";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Cập nhật thông tin bệnh nhân.
        /// </summary>
        /// <param name="patient">Thông tin bệnh nhân từ client gửi tới</param>
        /// <param name="UpdatedPatient">Thông tin bệnh nhân đã được cập nhật. (Reload)</param>
        /// <returns></returns>
        /*==== #001 ====*/
        public bool UpdatePatient(Patient patient, out Patient UpdatedPatient)
        {
            return UpdatePatientAdmin(patient, false, out UpdatedPatient);
        }
        public bool UpdatePatientAdmin(Patient patient, bool IsAdmin, out Patient UpdatedPatient)
        //public bool UpdatePatient(Patient patient, out Patient UpdatedPatient)
        /*==== #001 ====*/
        {
            bool bUpdateOK = false;
            UpdatedPatient = patient;
            try
            {
                AxLogger.Instance.LogInfo("Start updating patient info.", CurrentUser);
                /*==== #001 ====*/
                //bUpdateOK = PatientProvider.Instance.UpdatePatient(patient);
                bUpdateOK = PatientProvider.Instance.UpdatePatient(patient, IsAdmin);
                /*==== #001 ====*/
                AxLogger.Instance.LogInfo("Start updating patient info. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of updating patient info. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

            try
            {
                AxLogger.Instance.LogInfo("Start getting patient info", CurrentUser);
                UpdatedPatient = PatientProvider.Instance.GetPatientByID_Full(patient.PatientID);
                AxLogger.Instance.LogInfo("End of getting patient info. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of getting patient info. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            return bUpdateOK;
        }

        public bool DeletePatientByID(long patientID, long StaffID)
        {
            bool result = false;
            try
            {
                AxLogger.Instance.LogInfo("Start delete patient", CurrentUser);
                result = PatientProvider.Instance.DeletePatientByID(patientID, StaffID);
                AxLogger.Instance.LogInfo("End delete patient. Status: Success.", CurrentUser);
                return result;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End delete patient. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "DeletePatientByID Failed");

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<Patient> GetAllPatients()
        {
            throw new NotImplementedException("not implemented");
        }

        public Patient GetPatientByCode(string patientCode)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IPatientRegistrationService Members

        /// <summary>
        /// Get the patients at a specific page.
        /// </summary>
        /// <param name="pageIndex">The index of current page</param>
        /// <param name="pageSize">The number of records to load</param>
        /// <param name="bCountTotal">This variable indicates whether we want to count all the records satisfy the condition</param>
        /// <param name="totalCount">Total records (available if bCountTotal equals to true)</param>
        /// <returns></returns>
        public IList<Patient> GetPatients(int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            return PatientProvider.Instance.GetPatients(pageIndex, pageSize, bCountTotal, out totalCount);
        }

        public IList<Patient> SearchPatients(PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            return PatientProvider.Instance.SearchPatients(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
        }

        public bool AddNewPatient(Patient newPatient, out Patient AddedPatient)
        {
            bool bAddOK = false;
            long PatientID;
            string PatientCode;
            string PatientBarCode;
            AddedPatient = newPatient;
            try
            {
                AxLogger.Instance.LogInfo("Start adding new patient...", CurrentUser);
                //bool bHICardIsValid = true;
                //if(newPatient.HealthInsurances != null)
                //{
                //    foreach (HealthInsurance hi in newPatient.HealthInsurances)
                //    {
                //        string strDoiTuong, strQuyenLoi, strTinh, strQuanHuyen, strDonVi, strSTT;
                //        if(!string.IsNullOrWhiteSpace(hi.HICardNo))
                //        {
                //            if (AxHelper.ExtractHICardNumber(hi.HICardNo, out strDoiTuong, out strQuyenLoi,
                //                                    out strTinh, out strQuanHuyen, out strDonVi, out strSTT))
                //            {
                //                hi.IBID = int.Parse(strQuyenLoi);
                //                hi.HIPCode = strDoiTuong;
                //            }
                //            else
                //            {
                //                bHICardIsValid = false;
                //            }
                //        }
                //        else
                //        {
                //            bHICardIsValid = false;
                //        }
                //        if (bHICardIsValid == false)
                //        {
                //            break;
                //        }
                //    }
                //}

                //if(bHICardIsValid)
                {
                    newPatient.PatientCode = new ServiceSequenceNumberProvider().GetPatientCode();
                    bAddOK = PatientProvider.Instance.AddPatient(newPatient, out PatientID, out PatientCode, out PatientBarCode);
                    AxLogger.Instance.LogInfo("End of adding new patient. Status: Success", CurrentUser);
                }
                //else
                //{
                //    //Thong bao the bao hiem khong hop le.
                //    throw new Exception(eHCMSResources.Z1840_G1_HICardInValid);
                //}
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of adding new patient. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            //Tra ve thong tin benh nhan vua moi dc them vao. (Chi lay thong tin phan general info.
            try
            {
                AxLogger.Instance.LogInfo("Start getting patient info", CurrentUser);
                AddedPatient = PatientProvider.Instance.GetPatientByID(PatientID);
                AxLogger.Instance.LogInfo("End of getting patient info. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of getting patient info. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            return bAddOK;
        }

        public IList<PatientClassification> GetAllClassifications()
        {
            //return PatientProvider.Instance.GetAllClassifications();
            string mainCacheKey = "AllClassifications";
            List<PatientClassification> classifications;
            //Kiểm tra nếu có trong cache thì lấy từ trong cache.
            if (ServerAppConfig.CachingEnabled)
            {
                classifications = (List<PatientClassification>)AxCache.Current[mainCacheKey];
                if (classifications != null)
                {
                    return classifications;
                }
            }
            classifications = PatientProvider.Instance.GetAllClassifications();
            //Lưu vào cache để lần sau sử dụng.
            if (ServerAppConfig.CachingEnabled)
            {
                if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                {
                    AxCache.Current[mainCacheKey] = classifications;
                }
                else
                {
                    AxCache.Current.Insert(mainCacheKey, classifications, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                }
            }
            GetAllMedicalServiceTypes();
            return classifications;
        }

        public IList<PatientClassHistory> GetAllClassHistories(long patientID)
        {
            //For testing only
            //System.Threading.Thread.Sleep(2000);
            return PatientProvider.Instance.GetAllClassificationHistories(patientID);
        }

        public IList<RegistrationType> GetAllRegistrationTypes()
        {
            //return PatientProvider.Instance.GetAllRegistrationTypes();
            string mainCacheKey = "AllRegistrationTypes";
            List<RegistrationType> allRegistrationTypes;
            //Kiểm tra nếu có trong cache thì lấy từ trong cache.
            if (ServerAppConfig.CachingEnabled)
            {
                allRegistrationTypes = (List<RegistrationType>)AxCache.Current[mainCacheKey];
                if (allRegistrationTypes != null)
                {
                    return allRegistrationTypes;
                }
            }
            allRegistrationTypes = PatientProvider.Instance.GetAllRegistrationTypes();
            //Lưu vào cache để lần sau sử dụng.
            if (ServerAppConfig.CachingEnabled)
            {
                if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                {
                    AxCache.Current[mainCacheKey] = allRegistrationTypes;
                }
                else
                {
                    AxCache.Current.Insert(mainCacheKey, allRegistrationTypes, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                }
            }
            return allRegistrationTypes;
        }

        //public void RegisterPatient(PatientRegistration registrationInfo, out long PatientRegistrationID, out int SequenceNo)
        //{
        //    //PatientProvider.Instance.RegisterPatient(registrationInfo, classification, out PatientRegistrationID, out SequenceNo);
        //    //registrationInfo.PtRegistrationID = PatientRegistrationID;
        //    //registrationInfo.SequenceNo = SequenceNo;
        //    //AxEventManager.Instance.DuplexEventManager.EnqueueEvent(new AxEvent() { EventType=AxEventType.GET_PATIENTS});
        //    //AxEventManager.Instance.DuplexEventManager.EnqueueEvent(new AxEvent() { EventType = AxEventType.GET_LOCATIONS });

        //    //Hiện tại chỉ cho đăng ký cấp cứu.

        //    try
        //    {
        //        AxLogger.Instance.LogInfo("Start registering patient.", CurrentUser);
        //        PatientRegistrationID = -1;
        //        SequenceNo = -1;
        //        //long EmergencyRecordID;
        //        //bool bAddEmergencyRecordOK = false;
        //        //if(registrationInfo.EmergencyRecord != null)
        //        //{
        //        //    bAddEmergencyRecordOK = PatientProvider.Instance.AddEmergencyRecord(registrationInfo.EmergencyRecord, out EmergencyRecordID);
        //        //    if(bAddEmergencyRecordOK)
        //        //    {
        //        //        registrationInfo.EmergRecID = EmergencyRecordID;
        //        //        registrationInfo.EmergencyRecord.EmergRecID = EmergencyRecordID;
        //        //    }
        //        //}

        //        //Tinh quyen loi bao hiem:
        //        if(registrationInfo.PatientClassification.PatientClassID == (long)PatientType.INSUARED_PATIENT)
        //        {
        //            CommonService comService = new CommonService();
        //            bool isCrossRegion = false;
        //            double hiBenefit = 0.0;
        //            hiBenefit = comService.CalcPatientHiBenefit(registrationInfo, out isCrossRegion);
        //            registrationInfo.PtInsuranceBenefit = hiBenefit;
        //            registrationInfo.IsCrossRegion = isCrossRegion;
        //        }

        //        PatientProvider.Instance.RegisterPatient(registrationInfo, out PatientRegistrationID, out SequenceNo);
        //        registrationInfo.PtRegistrationID = PatientRegistrationID;
        //        registrationInfo.SequenceNo = SequenceNo;
        //        AxLogger.Instance.LogInfo("End of registering patient. Status: Success.", CurrentUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of registering patient. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_RegisterPatient);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}
        #endregion

        public IList<Location> GetAllLocations(long? departmentID)
        {
            //return PatientProvider.Instance.GetAllLocations(departmentID);
            try
            {

                AxLogger.Instance.LogInfo("Start retrieving all locations.", CurrentUser);
                string mainCacheKey = "AllLocations";
                if (departmentID.HasValue)
                {
                    mainCacheKey += departmentID.Value.ToString();
                }
                List<Location> allLocations;
                //Kiểm tra nếu có trong cache thì lấy từ trong cache.
                if (ServerAppConfig.CachingEnabled)
                {
                    allLocations = (List<Location>)AxCache.Current[mainCacheKey];
                    if (allLocations != null)
                    {
                        return allLocations;
                    }
                }
                allLocations = PatientProvider.Instance.GetAllLocations(departmentID);
                //Lưu vào cache để lần sau sử dụng.
                if (ServerAppConfig.CachingEnabled)
                {
                    if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                    {
                        AxCache.Current[mainCacheKey] = allLocations;
                    }
                    else
                    {
                        AxCache.Current.Insert(mainCacheKey, allLocations, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                    }
                }
                AxLogger.Instance.LogInfo("End of retrieving all locations. Status: Success.", CurrentUser);
                return allLocations;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving all locations. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetAllLocations);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<DeptLocation> GetAllDeptLocForServicesByInOutType(List<long> RefMedicalServiceInOutOthersTypes)
        {
            try
            {
                List<DeptLocation> allLocations;
                allLocations = PatientProvider.Instance.GetAllDeptLocForServicesByInOutType(RefMedicalServiceInOutOthersTypes);
                AxLogger.Instance.LogInfo("End of retrieving locations. Status: Success.", CurrentUser);
                return allLocations;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving locations. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetLocationsByServiceID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public IList<DeptLocation> GetLocationsByServiceID(long medServiceID)
        {
            return PatientProvider.Instance.GetLocationsByServiceID(medServiceID);
        }


        //public IList<DeptLocation> GetLocationsByServiceID(long medServiceID)
        //{
        //    try
        //    {

        //        AxLogger.Instance.LogInfo("Start retrieving locations.", CurrentUser);
        //        string mainCacheKey = "Locations_ByService_";
        //        mainCacheKey += medServiceID.ToString();
        //        List<DeptLocation> allLocations;
        //        //Kiểm tra nếu có trong cache thì lấy từ trong cache.
        //        if (ServerAppConfig.CachingEnabled)
        //        {
        //            allLocations = (List<DeptLocation>)AxCache.Current[mainCacheKey];
        //            if (allLocations != null)
        //            {
        //                return allLocations;
        //            }
        //        }

        //        allLocations = PatientProvider.Instance.GetLocationsByServiceID(medServiceID);

        //        //Lưu vào cache để lần sau sử dụng.
        //        if (ServerAppConfig.CachingEnabled)
        //        {
        //            if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
        //            {
        //                AxCache.Current[mainCacheKey] = allLocations;
        //            }
        //            else
        //            {
        //                AxCache.Current.Insert(mainCacheKey, allLocations, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
        //            }
        //        }
        //        AxLogger.Instance.LogInfo("End of retrieving locations. Status: Success.", CurrentUser);
        //        return allLocations;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving locations. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetLocationsByServiceID);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}


        public IList<RefMedicalServiceType> GetAllMedicalServiceTypes()
        {
            //return PatientProvider.Instance.GetAllMedicalServiceTypes();
            string mainCacheKey = "AllMedicalServiceTypes";
            List<RefMedicalServiceType> allMedicalServiceTypes;
            //Kiểm tra nếu có trong cache thì lấy từ trong cache.
            if (ServerAppConfig.CachingEnabled && AxCache.Current[mainCacheKey] != null)
            {
                allMedicalServiceTypes = (List<RefMedicalServiceType>)AxCache.Current[mainCacheKey];
                if (allMedicalServiceTypes != null)
                {
                    return allMedicalServiceTypes;
                }
            }
            else
            {
                allMedicalServiceTypes = PatientProvider.Instance.GetAllMedicalServiceTypes();
                //Lưu vào cache để lần sau sử dụng.
                if (ServerAppConfig.CachingEnabled)
                {
                    if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                    {
                        AxCache.Current[mainCacheKey] = allMedicalServiceTypes;
                    }
                    else
                    {
                        AxCache.Current.Insert(mainCacheKey, allMedicalServiceTypes, new TimeSpan(0, 0, int.MaxValue), true);
                    }
                }
            }


            return allMedicalServiceTypes;
        }


        public IList<RefMedicalServiceType> GetMedicalServiceTypesByInOutType(List<long> RefMedicalServiceInOutOthersTypes)
        {
            if (RefMedicalServiceInOutOthersTypes == null || RefMedicalServiceInOutOthersTypes.Count == 0)
            {
                return null;
            }
            string str = "";
            foreach (var item in RefMedicalServiceInOutOthersTypes)
            {
                str += "_" + item.ToString();
            }
            string mainCacheKey = "MedicalServiceTypesByDepartment" + str;

            List<RefMedicalServiceType> allMedicalServiceTypes;
            //Kiểm tra nếu có trong cache thì lấy từ trong cache.
            if (ServerAppConfig.CachingEnabled)
            {
                allMedicalServiceTypes = (List<RefMedicalServiceType>)AxCache.Current[mainCacheKey];
                if (allMedicalServiceTypes != null)
                {
                    return allMedicalServiceTypes;
                }
            }
            allMedicalServiceTypes = PatientProvider.Instance.GetMedicalServiceTypesByInOutType(RefMedicalServiceInOutOthersTypes);
            //Lưu vào cache để lần sau sử dụng.
            if (ServerAppConfig.CachingEnabled)
            {
                if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                {
                    AxCache.Current[mainCacheKey] = allMedicalServiceTypes;
                }
                else
                {
                    AxCache.Current.Insert(mainCacheKey, allMedicalServiceTypes, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                }
            }
            return allMedicalServiceTypes;
        }
        public HealthInsurance GetActiveHICard(long patientID)
        {
            return PatientProvider.Instance.GetActiveHICard(patientID);
        }

        //public IList<RefMedicalServiceItem> GetAllMedicalServiceItemsByType(long? serviceTypeID)
        //{
        //    try
        //    {
        //        AxLogger.Instance.LogInfo("Start loading all Medical Service Items by Type...", CurrentUser);
        //        string mainCacheKey = "MedicalServiceItemsAndDeptLoc";

        //        List<RefMedicalServiceItem> allMedicalServiceItems;
        //        if (ServerAppConfig.CachingEnabled && (List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey]!=null)
        //        {
        //            allMedicalServiceItems = (List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey];
        //            if (serviceTypeID.GetValueOrDefault(-1) > 0)
        //            {
        //                allMedicalServiceItems = allMedicalServiceItems.Where(o => o.MedicalServiceTypeID == serviceTypeID).ToList();
        //            }
        //        }
        //        else
        //        {
        //            allMedicalServiceItems = PatientProvider.Instance.GetAllMedicalServiceItemsByType(0);
        //            foreach (var item in allMedicalServiceItems)
        //            {
        //                item.allDeptLocation = GetLocationsByServiceID(item.MedServiceID).ToObservableCollection();
        //            }
        //            if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
        //            {
        //                AxCache.Current[mainCacheKey] = allMedicalServiceItems;
        //            }
        //            else
        //            {
        //                AxCache.Current.Insert(mainCacheKey, allMedicalServiceItems, new TimeSpan(0, 0, int.MaxValue), true);
        //            }
        //            if (serviceTypeID.GetValueOrDefault(-1) > 0)
        //            {
        //                allMedicalServiceItems = allMedicalServiceItems.Where(o => o.MedicalServiceTypeID == serviceTypeID).ToList();
        //            }
        //        }
        //        AxLogger.Instance.LogInfo("End of loading all Medical Service Items by Type. Status: Success.", CurrentUser);
        //        return allMedicalServiceItems;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of loading all Medical Service Items by Type. Status: Failed.", CurrentUser);
        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetAllMedicalServiceItemsByType);
        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public IList<RefMedicalServiceItem> GetAllMedicalServiceItemsByType(long? serviceTypeID, long? MedServiceItemPriceListID, long? V_RefMedicalServiceTypes)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading all Medical Service Items by Type...", CurrentUser);
                string mainCacheKey = "MedicalServiceItemsAndDeptLoc";

                List<RefMedicalServiceItem> allMedicalServiceItems;
                if (MedServiceItemPriceListID > 0)
                {
                    allMedicalServiceItems = PatientProvider.Instance.GetAllMedicalServiceItemsByType(0, MedServiceItemPriceListID, V_RefMedicalServiceTypes);
                    foreach (var item in allMedicalServiceItems)
                    {
                        item.allDeptLocation = GetLocationsByServiceID(item.MedServiceID).ToObservableCollection();
                    }
                    if (serviceTypeID.GetValueOrDefault(-1) > 0)
                    {
                        allMedicalServiceItems = allMedicalServiceItems.Where(o => o.MedicalServiceTypeID == serviceTypeID).ToList();
                    }
                }
                else if (ServerAppConfig.CachingEnabled && (List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey] != null)
                {
                    allMedicalServiceItems = (List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey];
                    if (serviceTypeID.GetValueOrDefault(-1) > 0)
                    {
                        allMedicalServiceItems = allMedicalServiceItems.Where(o => o.MedicalServiceTypeID == serviceTypeID).ToList();
                    }
                    else
                    {
                        if (V_RefMedicalServiceTypes == 0)
                        {
                            allMedicalServiceItems = allMedicalServiceItems.Where(o => o.ObjMedicalServiceTypeID.V_RefMedicalServiceTypes != (long)AllLookupValues.V_RefMedicalServiceTypes.GIUONGBENH
                                                                     && o.ObjMedicalServiceTypeID.V_RefMedicalServiceTypes != (long)AllLookupValues.V_RefMedicalServiceTypes.THUTHUAT
                                                                     && o.ObjMedicalServiceTypeID.V_RefMedicalServiceTypes != (long)AllLookupValues.V_RefMedicalServiceTypes.MAU
                                                                     ).ToList();
                        }
                        else if (V_RefMedicalServiceTypes != null)
                        {
                            allMedicalServiceItems = allMedicalServiceItems.Where(o => o.ObjMedicalServiceTypeID.V_RefMedicalServiceTypes == V_RefMedicalServiceTypes).ToList();
                        }
                    }
                }
                else
                {
                    allMedicalServiceItems = PatientProvider.Instance.GetAllMedicalServiceItemsByType(0, null, V_RefMedicalServiceTypes);
                    foreach (var item in allMedicalServiceItems)
                    {
                        item.allDeptLocation = GetLocationsByServiceID(item.MedServiceID).ToObservableCollection();
                    }
                    if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                    {
                        AxCache.Current[mainCacheKey] = allMedicalServiceItems;
                    }
                    else
                    {
                        AxCache.Current.Insert(mainCacheKey, allMedicalServiceItems, new TimeSpan(0, 0, int.MaxValue), true);
                    }
                    if (serviceTypeID.GetValueOrDefault(-1) > 0)
                    {
                        allMedicalServiceItems = allMedicalServiceItems.Where(o => o.MedicalServiceTypeID == serviceTypeID).ToList();
                    }
                }
                AxLogger.Instance.LogInfo("End of loading all Medical Service Items by Type. Status: Success.", CurrentUser);
                return allMedicalServiceItems;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading all Medical Service Items by Type. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetAllMedicalServiceItemsByType);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public IList<RefMedicalServiceItem> GetMedicalServiceItems(long? departmentID, long? serviceTypeID)
        {
            if (departmentID.GetValueOrDefault(-1) <= 0 ||
                serviceTypeID.GetValueOrDefault(-1) <= 0)
            {
                return null;
            }

            string mainCacheKey = "MedServiceItems_Dept_Service_Type";
            if (departmentID != null)
            {
                mainCacheKey += departmentID.ToString();
            }
            if (serviceTypeID != null)
            {
                mainCacheKey += serviceTypeID.ToString();
            }
            List<RefMedicalServiceItem> allMedicalServiceItems;
            //Kiểm tra nếu có trong cache thì lấy từ trong cache.
            if (ServerAppConfig.CachingEnabled)
            {
                allMedicalServiceItems = (List<RefMedicalServiceItem>)AxCache.Current[mainCacheKey];
                if (allMedicalServiceItems != null)
                {
                    return allMedicalServiceItems;
                }
            }
            allMedicalServiceItems = PatientProvider.Instance.GetMedicalServiceItems(departmentID, serviceTypeID);
            //Lưu vào cache để lần sau sử dụng.
            if (ServerAppConfig.CachingEnabled)
            {
                if (ServerAppConfig.SlidingExpirationTime <= 0 || ServerAppConfig.SlidingExpirationTime == int.MaxValue)
                {
                    AxCache.Current[mainCacheKey] = allMedicalServiceItems;
                }
                else
                {
                    AxCache.Current.Insert(mainCacheKey, allMedicalServiceItems, new TimeSpan(0, 0, ServerAppConfig.SlidingExpirationTime), true);
                }
            }
            return allMedicalServiceItems;
        }
        #region IPatientRegistrationService Members

        //public HealthInsurance AddHICard(HealthInsurance hi, out long HIID)
        //{
        //    //PatientProvider.Instance.AddHICard(hi, out HIID);
        //    try
        //    {
        //        AxLogger.Instance.LogInfo("Start adding HI Card.", CurrentUser);
        //        bool bHICardIsValid = true;

        //        string strDoiTuong, strQuyenLoi, strTinh, strQuanHuyen, strDonVi, strSTT;
        //        if (AxHelper.ExtractHICardNumber(hi.HICardNo, out strDoiTuong, out strQuyenLoi,
        //                                        out strTinh, out strQuanHuyen, out strDonVi, out strSTT))
        //        {
        //            hi.IBID = int.Parse(strQuyenLoi);
        //            hi.HIPCode = strDoiTuong;
        //        }
        //        else
        //        {
        //            bHICardIsValid = false;
        //        }
        //        if (bHICardIsValid)
        //        {
        //            PatientProvider.Instance.AddHICard(hi, out HIID);
        //            hi.HIID = HIID;
        //            AxLogger.Instance.LogInfo("End of adding HI Card. Status: Success.", CurrentUser);
        //            return hi;
        //        }
        //        else
        //        {
        //            //Thong bao the bao hiem khong hop le.
        //            throw new Exception(eHCMSResources.Z1840_G1_HICardInValid);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of adding HI Card. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_AddHICard);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public void ActivateHICard(long hiCardID)
        {
            PatientProvider.Instance.ActivateHICard(hiCardID);
        }

        public HealthInsurance UpdateHICard(HealthInsurance hi)
        {
            //PatientProvider.Instance.UpdateHICard(hi);
            try
            {
                AxLogger.Instance.LogInfo("Start updating HI Card.", CurrentUser);
                bool bOK;
                bOK = PatientProvider.Instance.UpdateHICard(hi);
                if (!bOK)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1838_G1_CannotUpdateHICard));
                }
                else
                {
                    AxLogger.Instance.LogInfo("End of updating HI Card. Status: Success.", CurrentUser);
                    //Set lai gia tri IsValid (khoi mat cong doc database)
                    hi.ResetIsValidProperty();
                    return hi;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of updating HI Card. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_UpdateHICard);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        #endregion

        public AxResponse CalculatePaymentSummary(IList<PatientRegistrationDetail> services, InsuranceBenefit benefit, out PayableSum PayableSum, out IList<PatientRegistrationDetail> CalculatedServices)
        {
            AxResponse response = new AxResponse();
            PayableSum = null;
            CalculatedServices = services;
            try
            {
                PayableSum = new PayableSum();
                if (benefit == null)
                {
                    //Tính không bảo hiểm.
                    if (CalculatedServices != null && CalculatedServices.Count > 0)
                    {
                        decimal totalPrice = 0;
                        foreach (PatientRegistrationDetail d in CalculatedServices)
                        {
                            d.ServicePrice.Price = d.RefMedicalServiceItem.NormalPrice;
                            totalPrice += (decimal)d.ServiceQty.Value * d.ServicePrice.Price;
                            d.PaymentInfo.TotalHIPayment = 0;
                            d.PaymentInfo.TotalPatientPayment = (decimal)d.ServiceQty.Value * d.ServicePrice.Price;
                        }
                        PayableSum.TotalPrice = totalPrice;
                    }
                    PayableSum.TotalPatientPayment = PayableSum.TotalPrice;

                    return response;
                }

                if (services == null || services.Count == 0)
                {
                    return response;
                }
                //Tính cho trường hợp có bảo hiểm.
                //Tính số tiền 15% tháng lương tối thiểu.
                bool bHI = false;
                decimal minPay = Globals.AxServerSettings.HealthInsurances.HiPolicyMinSalary * (decimal)Globals.AxServerSettings.HealthInsurances.HiPolicyPercentageOnPayable;

                RefMedicalServiceItem medItem;
                //Tính tổng số tiền bảo hiểm chịu trả, nếu <15% tháng lương tối thiểu thì BH trả hết
                decimal totalHIAccept = 0;
                foreach (PatientRegistrationDetail details in CalculatedServices)
                {
                    medItem = details.RefMedicalServiceItem;
                    medItem.CalculatePayment(benefit);

                    details.ServicePrice.Price = medItem.NormalPrice;

                    details.ServicePrice.PriceDifference = medItem.PriceDifference;
                    details.ServicePrice.CoPayment = medItem.CoPayment;
                    details.ServicePrice.PatientPayment = medItem.PatientPayment;
                    details.ServicePrice.HIPayment = medItem.HIPayment;

                    details.PaymentInfo.TotalHIPayment = details.ServicePrice.HIPayment * (decimal)details.ServiceQty.Value;
                    details.PaymentInfo.TotalPatientPayment = details.ServicePrice.PatientPayment * (decimal)details.ServiceQty.Value;
                    details.PaymentInfo.TotalCoPayment = details.ServicePrice.CoPayment * (decimal)details.ServiceQty.Value;
                    details.PaymentInfo.TotalPriceDifference = details.ServicePrice.PriceDifference * (decimal)details.ServiceQty.Value;

                    //Kiểm tra có phải là dịch vụ bảo hiểm:
                    if (medItem.HIAllowedPrice.HasValue && medItem.HIAllowedPrice.Value > 0)
                    {
                        if (medItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH)
                        {
                            if (!bHI)
                            {
                                totalHIAccept += medItem.HIAllowedPrice.Value;
                                bHI = true;
                            }
                        }
                        else
                        {
                            totalHIAccept += medItem.HIAllowedPrice.Value * (decimal)details.ServiceQty.Value;
                        }

                    }
                }
                if (totalHIAccept < minPay)
                {
                    bHI = false;
                    foreach (PatientRegistrationDetail details in CalculatedServices)
                    {
                        medItem = details.RefMedicalServiceItem;
                        //Kiểm tra có phải là dịch vụ bảo hiểm:
                        if (medItem.HIAllowedPrice.HasValue && medItem.HIAllowedPrice.Value > 0)
                        {
                            if (medItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH)
                            {
                                if (!bHI)
                                {
                                    details.ServicePrice.PriceDifference = medItem.PriceDifference;
                                    details.ServicePrice.CoPayment = 0;
                                    details.ServicePrice.PatientPayment = medItem.PriceDifference;
                                    details.ServicePrice.HIPayment = medItem.HIAllowedPrice.Value;

                                    //Bao hiem chi tra cho 1 lan kham benh
                                    details.PaymentInfo.TotalHIPayment = details.ServicePrice.HIPayment;
                                    details.PaymentInfo.TotalPatientPayment = details.ServicePrice.Price * (decimal)details.ServiceQty.Value - details.PaymentInfo.TotalHIPayment;
                                    details.PaymentInfo.TotalCoPayment = details.ServicePrice.CoPayment;
                                    details.PaymentInfo.TotalPriceDifference = details.ServicePrice.PriceDifference;

                                    bHI = true;
                                }
                                else
                                {
                                    details.ServicePrice.PriceDifference = 0;
                                    details.ServicePrice.CoPayment = 0;
                                    details.ServicePrice.PatientPayment = details.ServicePrice.Price;
                                    details.ServicePrice.HIPayment = 0;

                                    details.PaymentInfo.TotalHIPayment = details.ServicePrice.HIPayment * (decimal)details.ServiceQty.Value;
                                    details.PaymentInfo.TotalPatientPayment = details.ServicePrice.PatientPayment * (decimal)details.ServiceQty.Value;
                                    details.PaymentInfo.TotalCoPayment = 0;
                                    details.PaymentInfo.TotalPriceDifference = 0;
                                }
                            }
                            else
                            {
                                //NOTHING TO DO HERE
                            }
                        }
                        else//Dich vu khong bao hiem
                        {
                            //NOTHING TO DO HERE
                        }
                    }
                }
                else
                {
                    bHI = false;
                    foreach (PatientRegistrationDetail details in CalculatedServices)
                    {
                        medItem = details.RefMedicalServiceItem;
                        //Kiểm tra có phải là dịch vụ bảo hiểm:
                        if (medItem.HIAllowedPrice.HasValue && medItem.HIAllowedPrice.Value > 0)
                        {
                            if (medItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH)
                            {
                                if (!bHI) //Chưa tính bảo hiểm cho dịch vụ khám chữa bệnh.
                                {

                                    //Bao hiem chi tra cho 1 lan kham benh
                                    details.PaymentInfo.TotalHIPayment = details.ServicePrice.HIPayment;
                                    details.PaymentInfo.TotalPatientPayment = details.ServicePrice.Price * (decimal)details.ServiceQty.Value - details.PaymentInfo.TotalHIPayment;
                                    details.PaymentInfo.TotalCoPayment = details.ServicePrice.CoPayment;
                                    details.PaymentInfo.TotalPriceDifference = details.ServicePrice.PriceDifference;

                                    bHI = true;
                                }
                                else //Đã tính bảo hiểm một lần rồi (Khám chữa bệnh chỉ tính 1 dịch vụ thôi), xem như không có bảo hiểm.
                                {
                                    details.ServicePrice.PriceDifference = 0;
                                    details.ServicePrice.CoPayment = 0;
                                    details.ServicePrice.PatientPayment = details.ServicePrice.Price;
                                    details.ServicePrice.HIPayment = 0;

                                    details.PaymentInfo.TotalHIPayment = details.ServicePrice.HIPayment * (decimal)details.ServiceQty.Value;
                                    details.PaymentInfo.TotalPatientPayment = details.ServicePrice.PatientPayment * (decimal)details.ServiceQty.Value;
                                    details.PaymentInfo.TotalCoPayment = 0;
                                    details.PaymentInfo.TotalPriceDifference = 0;
                                }
                            }
                            else
                            {
                                //NOTHING TO DO HERE
                            }
                        }
                        else//Dich vu khong bao hiem
                        {
                            //NOTHING TO DO HERE
                        }
                    }

                }
                foreach (PatientRegistrationDetail details in CalculatedServices)
                {
                    medItem = details.RefMedicalServiceItem;
                    if (medItem.HIAllowedPrice.HasValue && medItem.HIAllowedPrice.Value > 0)
                    {
                        PayableSum.TotalHIServicePrice += details.ServicePrice.Price * (decimal)details.ServiceQty.Value;
                    }
                    else
                    {
                        PayableSum.TotalNonHIServicePrice += details.ServicePrice.Price * (decimal)details.ServiceQty.Value;
                    }
                    PayableSum.TotalPrice += details.ServicePrice.Price * (decimal)details.ServiceQty.Value;

                    PayableSum.TotalHIPayment += details.PaymentInfo.TotalHIPayment;
                    PayableSum.TotalPatientPayment += details.PaymentInfo.TotalPatientPayment;
                    PayableSum.TotalCoPayment += details.PaymentInfo.TotalCoPayment;
                    PayableSum.TotalPriceDifference += details.PaymentInfo.TotalPriceDifference;
                }
                return response;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                AxException innerEx = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                innerEx.MethodName = curMethod.Name;
                innerEx.ClassName = curMethod.DeclaringType.FullName;

                innerEx.ErrorCode = "PR.0_0000002";
                innerEx.ModuleName = _ModuleName;
                response.Exception = innerEx;
            }
            return response;
        }

        /// <summary>
        /// Tinh tien cho dang ky nay..
        /// </summary>
        /// <param name="tran"></param>
        /// <returns>Thong tin chi tiet cho dang ky da duoc xu ly.</returns>
        public PatientRegistration CalculatePaymentForRegistration(PatientRegistration reg)
        {
            try
            {
                PayableSum payableSum = null;
                IList<PatientRegistrationDetail> registrationDetails = null;
                InsuranceBenefit benefit = null;
                if (reg.HealthInsurance != null)
                {
                    benefit = reg.HealthInsurance.InsuranceBenefit;
                }

                CalculatePaymentSummary(reg.PatientRegistrationDetails, benefit, out payableSum, out registrationDetails);

                PatientRegistration calculatedReg = reg;
                calculatedReg.PayableSum = payableSum;

                calculatedReg.PatientRegistrationDetails = registrationDetails.ToObservableCollection<PatientRegistrationDetail>(); ;

                return calculatedReg;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "PR.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
        }

        public AxResponse GetLatestClassificationAndActiveHICard(long patientID, out PatientClassification classification, out HealthInsurance activeHI)
        {
            AxResponse response = new AxResponse();
            PatientProvider.Instance.GetLatestClassificationAndActiveHICard(patientID, out classification, out activeHI);
            return response;
        }
        public AxResponse GetAllUnqueuedPatients(long locationID, PatientSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount, out List<PatientQueue> allUnqueueItems)
        {
            AxResponse response = new AxResponse();

            try
            {
                allUnqueueItems = PatientProvider.Instance.GetAllUnqueuedPatients(locationID, criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                totalCount = -1;
                allUnqueueItems = null;

                AxException innerEx = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                innerEx.MethodName = curMethod.Name;
                innerEx.ClassName = curMethod.DeclaringType.FullName;

                innerEx.ErrorCode = "PR.0_0000002";
                innerEx.ModuleName = _ModuleName;
                response.Exception = innerEx;
            }
            return response;
        }

        public AxResponse Enqueue(PatientQueue queueItem, out bool Result)
        {
            AxResponse response = new AxResponse();

            try
            {
                Result = PatientProvider.Instance.Enqueue(queueItem);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                Result = false;

                AxException innerEx = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                innerEx.MethodName = curMethod.Name;
                innerEx.ClassName = curMethod.DeclaringType.FullName;

                innerEx.ErrorCode = "PR.0_0000002";
                innerEx.ModuleName = _ModuleName;
                response.Exception = innerEx;
            }
            if (Result)
            {
                try
                {
                    AxEventManager.Instance.DuplexEventManager.EnqueueEvent(new AxEvent()
                    {
                        EventType = AxEventType.GET_CONSULTATION_LIST,
                        LocationID = queueItem.DeptLocID,
                        UserState = queueItem
                    });
                }
                catch (System.Exception ex)
                {
                    AxLogger.Instance.LogError(ex);
                }
            }
            return response;
        }
        public AxResponse GetAllQueuedPatients(long locationID, long queueType, out List<PatientQueue> QueuedItems)
        {
            AxResponse response = new AxResponse();

            try
            {
                QueuedItems = PatientProvider.Instance.GetAllQueuedPatients(locationID, queueType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                QueuedItems = null;

                AxException innerEx = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                innerEx.MethodName = curMethod.Name;
                innerEx.ClassName = curMethod.DeclaringType.FullName;

                innerEx.ErrorCode = "PR.0_0000002";
                innerEx.ModuleName = _ModuleName;
                response.Exception = innerEx;
            }
            return response;
        }

        public List<PatientRegistration> GetAllRegistrations(long patientID, bool IsInPtRegistration)
        {
            try
            {
                List<PatientRegistration> allRegistrations;
                allRegistrations = PatientProvider.Instance.GetAllRegistrations(patientID, IsInPtRegistration);
                return allRegistrations;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "PR.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
        }


        public List<PatientRegistrationDetail> GetAllRegistrationDetails_ForGoToKhamBenh(long registrationID)
        {
            try
            {
                return PatientProvider.Instance.GetAllRegistrationDetails_ForGoToKhamBenh(registrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "PR.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
        }


        public List<PatientRegistrationDetail> GetAllRegistrationDetails(long registrationID)
        {
            try
            {
                return PatientProvider.Instance.GetAllRegistrationDetails(registrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "PR.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
        }

        public List<PatientRegistrationDetail> GetAllRegistrationDetails_ByV_ExamRegStatus(Int64 PtRegistrationID, Int64 V_ExamRegStatus)
        {
            try
            {
                return PatientProvider.Instance.GetAllRegistrationDetails_ByV_ExamRegStatus(PtRegistrationID, V_ExamRegStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                AxException axErr = new AxException();
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                axErr.MethodName = curMethod.Name;
                axErr.ClassName = curMethod.DeclaringType.FullName;

                axErr.ErrorCode = "PR.0_0000002";
                axErr.ModuleName = _ModuleName;

                throw new FaultException<AxException>(axErr);
            }
        }

        public HealthInsurance CalculateRealHIBenefit(HealthInsurance hi)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start calculating HI Benefit.", CurrentUser);
                //if (hi.ActiveHealthInsuranceHIPatientBenefit == null)
                //{
                //    if (hi.RegistrationCode.Trim().ToUpper() == ConfigValues.HospitalCode)
                //    {
                //        hi.HIPatientBenefit = hi.InsuranceBenefit == null ? 0.0 : hi.InsuranceBenefit.RebatePercentage;
                //    }
                //    else
                //    {
                //        hi.HIPatientBenefit = ConfigValues.RebatePercentageLevel_1;
                //    }    
                //}

                AxLogger.Instance.LogInfo("End of calculating HI Benefit. Status: Success.", CurrentUser);
                return hi;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of calculating HI Benefit. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_CalculateRealHIBenefit);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdateHiItem(HealthInsurance hiItem, long StaffID, bool IsEditAfterRegistration, string Reason, out string Result)
        {
            bool bSaveOK = false;
            Result = "";
            try
            {
                AxLogger.Instance.LogInfo("Start saving health insurances...", CurrentUser);
                bool bHICardIsValid = true;

                // TxD 28/03/2014 : If this HI Card is going to be deleted so by passing all Validation checking below                
                if (hiItem.MarkAsDeleted == false)
                {
                    // dinh them kiem tra ngay o day
                    //▼====: #004
                    //Hàm này được gọi khi người dùng click vào nút sửa sau đó click vào nút lưu chỉnh sửa thông tin thẻ
                    //Ban đầu trước khi comment những dòng này, chương trình sẽ báo lỗi khi người dùng chỉnh sửa thẻ để ngày đến bé hơn ngày hiện tại
                    //Bây giờ chương trình chỉ kiểm tra đầu cuối khi xác nhận/đăng ký thẻ BHYT nên người dùng có thể nhập thẻ và chỉnh sửa thẻ tùy ý, => điều kiện này không còn cần đc sử dụng nữa
                    //bHiCardIsValid = false sẽ làm cho out Result ra thông báo lỗi.
                    //if (hiItem.ValidDateTo.Value.Date < DateTime.Now.Date)
                    //{
                    //    bHICardIsValid = false;
                    //}
                    //▲====: #004
                    int? retIBID;
                    string retHIPCode;
                    ValidateHiItem(hiItem, out retIBID, out retHIPCode);
                }

                if (bHICardIsValid)
                {
                    // 22/11/2013 Txd ToBeFixed
                    //long StaffID = CurrentUser == null ? 0 : CurrentUser.StaffID.GetValueOrDefault(0);
                    //KMx: Truyền StaffID từ Client qua, không được gán bậy như bên dưới (20/02/2014 16:34)
                    //long StaffID = 8888;
                    bSaveOK = PatientProvider.Instance.UpdateHiItem(out Result, hiItem, IsEditAfterRegistration, StaffID, Reason);
                    AxLogger.Instance.LogInfo("End of saving health insurances. Status: Success", CurrentUser);

                    //try
                    //{
                    //    if (!string.IsNullOrWhiteSpace(hiItem.RegistrationCode)
                    //        && !string.IsNullOrWhiteSpace(hiItem.RegistrationLocation))
                    //    {
                    //        PatientProvider.Instance.AddHospitalByHiCode(hiItem.RegistrationLocation, hiItem.RegistrationCode);
                    //    }
                    //}
                    //catch (Exception)
                    //{

                    //}
                }
                else
                {
                    //Thong bao the bao hiem khong hop le.
                    Result = "ErrValidate";
                    return false;
                    //throw new Exception(eHCMSResources.Z1840_G1_HICardInValid);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving health insurances. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CANNOT_UPDATE_HI_ITEM);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

            return bSaveOK;
        }

        public bool AddHospitalByHiCode(string hospitalName, string hiCode)
        {
            try
            {
                return PatientProvider.Instance.AddHospitalByHiCode(hospitalName, hiCode);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving AddHospitalByHiCode. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CANNOT_UPDATE_HI_ITEM);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddHospital(Hospital entity)
        {
            try
            {
                return PatientProvider.Instance.AddHospital(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving AddHospital. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ex.ToString());
                throw new FaultException<AxException>(axErr, new FaultReason(ex.ToString()));
            }
        }

        public bool UpdateHospital(Hospital entity)
        {
            try
            {
                return PatientProvider.Instance.UpdateHospital(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving AddHospital. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CANNOT_UPDATE_HI_ITEM);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        private bool ValidateHiItem(HealthInsurance hiItem, out int? retIBID, out string retHIPCode)
        {
            retIBID = 0;
            retHIPCode = "";
            bool bHICardIsValid = true;

            // TxD 22/12/2014 : Applying new HI rule to begin from 01/01/2015 
            if (!Globals.AxServerSettings.HealthInsurances.ApplyHINewRule20150101)
            {
                if (hiItem.HICardType.LookupID == 5907) // Not yet ready to accept this new HI Card format
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1839_G1_BHYTMau2015ChuaApDung));
                }
            }

            string strDoiTuong, strQuyenLoi, strTinh, strQuanHuyen, strDonVi, strSTT, IBeID = "";
            if (!string.IsNullOrWhiteSpace(hiItem.HICardNo))
            {
                HICardValidatorBase cardValidator;
                cardValidator = HICardValidatorFactory.Instance.CreateValidator(hiItem.HICardType != null ? hiItem.HICardType.LookupID : hiItem.V_HICardType);
                //if (cardValidator.ExtractHICardNumber(hiItem.HICardNo, out strDoiTuong, out strQuyenLoi,
                //                        out strTinh, out strQuanHuyen, out strDonVi, out strSTT))
                //KMx: Truyền thêm V_HICardType để biết mẫu nào mới kiểm tra được. (23/02/2014 11:47)
                // TxD 28/12/2014: New 2015 rule checking here and call the corresponding Extracting method
                bool bExtractBenefitOK = false;
                if (Globals.AxServerSettings.HealthInsurances.ApplyHINewRule20150101)
                {
                    bExtractBenefitOK = cardValidator.ExtractHICardNumber_2015(hiItem.HICardType.LookupID, hiItem.HICardNo, out strDoiTuong, out strQuyenLoi
                        , out strTinh, out strQuanHuyen, out strDonVi, out strSTT, out IBeID
                        , Globals.AxServerSettings.CommonItems.ValidHIPattern, Globals.AxServerSettings.CommonItems.InsuranceBenefitCategories);
                }
                else
                {
                    bExtractBenefitOK = cardValidator.ExtractHICardNumber(hiItem.HICardType.LookupID, hiItem.HICardNo, out strDoiTuong, out strQuyenLoi,
                                                                            out strTinh, out strQuanHuyen, out strDonVi, out strSTT);
                }

                if (bExtractBenefitOK)
                {
                    hiItem.IBID = int.Parse(strQuyenLoi);
                    //▼====== #010: Gán IBeID để đem đi lưu ở AddHIItem hoặc cập nhật ở UpdateHIItem. Vì chuyển qua sử dụng bảng InsuranceBenefitCategories thay vì InsuranceBenefit nên xài IBeID thay vì IBID
                    hiItem.IBeID = long.Parse(IBeID);
                    //▲====== #010
                    // TxD 22/12/2014 : Applying new HI rule to begin from 01/01/2015 
                    if (Globals.AxServerSettings.HealthInsurances.ApplyHINewRule20150101)
                    {
                        hiItem.IBID += 10;
                    }
                    retIBID = hiItem.IBID.GetValueOrDefault();
                    hiItem.HIPCode = strDoiTuong;
                    retHIPCode = hiItem.HIPCode;
                }
                else
                {
                    bHICardIsValid = false;
                }

            }
            else
            {
                bHICardIsValid = false;
            }
            return bHICardIsValid;
        }

        public bool AddHiItem(HealthInsurance hiItem, long StaffID, out long HIID)
        {
            bool bSaveOK = false;
            HIID = -1;
            int? retIBID = 0;
            string retHIPCode = "";
            try
            {
                //AxLogger.Instance.LogInfo("Start adding new Hi Item...", CurrentUser);
                bool bHICardIsValid = ValidateHiItem(hiItem, out retIBID, out retHIPCode);
                if (bHICardIsValid)
                {
                    bSaveOK = PatientProvider.Instance.AddHiItem(hiItem, out HIID, StaffID);
                    AxLogger.Instance.LogInfo("End of adding new Hi Item. Status: Success", CurrentUser);
                }
                else
                {
                    //Thong bao the bao hiem khong hop le.
                    throw new Exception(eHCMSResources.Z1840_G1_HICardInValid);
                }
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("DupCardNo:"))
                {
                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CANNOT_ADD_HI_ITEM);
                    throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }

            return bSaveOK;
        }

        public List<HealthInsurance> GetAllHealthInsurances(long patientID, bool IncludeDeletedItems = false)
        {
            try
            {
                List<HealthInsurance> allHIItems;
                AxLogger.Instance.LogInfo("Start getting health insurances", CurrentUser);
                allHIItems = PatientProvider.Instance.GetAllHealthInsurances(patientID, IncludeDeletedItems);
                if (allHIItems != null)
                {
                    foreach (var hiItem in allHIItems)
                    {
                        hiItem.CanDelete = hiItem.CanEdit = !hiItem.Used;
                    }
                }
                AxLogger.Instance.LogInfo("End of getting health insurances. Status: Success.", CurrentUser);
                return allHIItems;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of getting health insurances. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SaveReferalItems(long patientID, List<PaperReferal> allReferalItems, out List<PaperReferal> PatientReferalItems, bool Reload = false, bool IncludeDeletedItems = false)
        {

            bool bSaveOK = false;

            PatientReferalItems = allReferalItems;
            try
            {
                AxLogger.Instance.LogInfo("Start saving paper referals...", CurrentUser);

                //bSaveOK = PatientProvider.Instance.SaveReferalItems(patientID, allReferalItems);
                AxLogger.Instance.LogInfo("End of saving paper referals. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving paper referals. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

            if (Reload)
            {
                PaperReferal item;
                PatientReferalItems = GetAllPaperReferals(patientID, out item, IncludeDeletedItems);
            }
            return bSaveOK;
        }

        public List<PaperReferal> GetAllPaperReferals(long HIID, out PaperReferal LatestUsedItem, bool IncludeDeletedItems = false)
        {

            try
            {
                LatestUsedItem = null;
                List<PaperReferal> allItems;
                AxLogger.Instance.LogInfo("Start getting paper referals", CurrentUser);
                allItems = PatientProvider.Instance.GetAllPaperReferalsByHealthInsurance(HIID, IncludeDeletedItems);
                if (allItems != null)
                {
                    foreach (var item in allItems)
                    {
                        item.CanDelete = item.CanEdit = !item.Used;
                    }
                }
                LatestUsedItem = PatientProvider.Instance.GetLatestPaperReferal(HIID);
                AxLogger.Instance.LogInfo("End of getting paper referals. Status: Success.", CurrentUser);
                return allItems;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of getting paper referals. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddPaperReferal(PaperReferal addedReferal, out PaperReferal PaperReferal)
        {
            bool bAddOK = false;
            long PaperReferalID;
            PaperReferal = addedReferal;
            try
            {
                AxLogger.Instance.LogInfo("Start adding new referal...", CurrentUser);
                bAddOK = PatientProvider.Instance.AddPaperReferal(addedReferal, out PaperReferalID);
                if (bAddOK)
                {
                    PaperReferal.RefID = PaperReferalID;
                }
                AxLogger.Instance.LogInfo("End of adding new patient. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of adding new referal. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            return bAddOK;
        }

        public bool UpdatePaperReferal(PaperReferal Referal)
        {
            bool bUpdateOK = false;
            try
            {
                AxLogger.Instance.LogInfo("Start updating referal...", CurrentUser);
                bUpdateOK = PatientProvider.Instance.UpdatePaperReferal(Referal);
                AxLogger.Instance.LogInfo("End of updating referal. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of updating referal. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            return bUpdateOK;
        }

        public bool DeletePaperReferal(PaperReferal Referal)
        {
            bool bUpdateOK = false;
            try
            {
                AxLogger.Instance.LogInfo("Start Delete referal...", CurrentUser);
                bUpdateOK = PatientProvider.Instance.DeletePaperReferal(Referal);
                AxLogger.Instance.LogInfo("End of delete referal. Status: Success", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of deleting referal. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            return bUpdateOK;
        }

        /// <summary>
        /// Xác nhận hợp lệ một thẻ bảo hiểm.
        /// Tương úng với thao tác nhân viên xác nhận đã kiểm tra thẻ.
        /// </summary>
        /// <param name="patientID">Ma so benh nhan</param>
        /// <param name="ConfirmedHiItem"></param>
        /// <param name="hiItem">Thẻ cần xác nhận</param>
        /// <returns>Trả về kết quả thẻ có hợp lệ hay không.</returns>
        public bool ConfirmHI(long patientID, HealthInsurance hiItem, out HealthInsurance ConfirmedHiItem)
        {
            //if(hiItem != null && hiItem.IsActive)
            //{
            //    ConfirmedHiItem = hiItem;
            //    return true;
            //}
            //ConfirmedHiItem = null;
            //return false;

            ConfirmedHiItem = ConfirmHIByPatientID(patientID, hiItem);
            return ConfirmedHiItem != null;
        }

        /// <summary>
        /// Xác nhận hợp lệ một thẻ bảo hiểm.
        /// Tim kiem trong database co the nay duoc active khong. 
        ///  Phai la the dang active cua benh nhan nay
        /// </summary>
        /// <param name="patientID">Ma benh nhan.</param>
        /// <param name="hiItem"></param>
        /// <returns>Tra ve null neu the khong hop le (khong phai cua benh nhan nay hoac khong co trong database)</returns>
        private HealthInsurance ConfirmHIByPatientID(long patientID, HealthInsurance hiItem)
        {
            try
            {
                HealthInsurance confirmedItem = new HealthInsurance();
                confirmedItem = PatientProvider.Instance.GetActiveHICard(patientID);
                if (confirmedItem != null && confirmedItem.HIID == hiItem.HIID)
                {
                    return confirmedItem;
                }
            }
            catch (System.Exception ex)
            {
                AxLogger.Instance.LogError(ex);
                return null;
            }
            return null;
        }




        /// <summary>
        /// Xác nhận hợp lệ một giấy chuyển viện
        /// Tương úng với thao tác nhân viên xác nhận đã kiểm tra thẻ.
        /// </summary>
        /// <param name="hiItem">Thẻ cần xác nhận</param>
        /// <returns>Trả về kết quả thẻ có hợp lệ hay không.</returns>
        public bool ConfirmPaperReferal(PaperReferal referal, out PaperReferal ConfirmedPaperReferal)
        {
            if (referal != null && referal.IsActive && referal.IsValid)
            {
                ConfirmedPaperReferal = referal;
                return true;
            }
            ConfirmedPaperReferal = null;
            return false;
        }

        /// <summary>
        /// Lưu tất cả các thay đổi trên danh sách giấy chuyển viện
        /// Xác nhận hợp lệ (kiểm tra thẻ bằng mắt thường) thẻ đang active.
        /// </summary>
        /// <param name="patientID">Mã số bệnh nhân</param>
        /// <param name="allReferalItems">Danh sách các giáy chuyển viện đã thay đổi (thêm, xóa, sửa)</param>
        /// <param name="ConfirmedPaperReferal">Giấy chuyển viện đã được xác nhận.</param>
        /// <param name="ConfirmItemValid">Giấy chuyển viện cần confirm có valid hay không. Nếu nó valid thì mới nên sử dụng ConfirmedHIItem</param>
        /// <param name="PaperReferals">Danh sách giấy chuyển viện trà về</param>
        /// <param name="ReloadPatientHI">Có muốn reload lại danh sách bảo hiểm sau khi save changes hay không.</param>
        /// <param name="IncludeDeletedItems">Có lấy thêm những item bị đánh dấu xóa hay không.</param>
        /// <returns>Trả về true nếu update danh sách OK.</returns>
        public bool SavePaperReferalsAndConfirm(long patientID, List<PaperReferal> allReferalItems, out PaperReferal ConfirmedPaperReferal, out bool ConfirmItemValid, out List<PaperReferal> PaperReferals, bool ReloadPatientHI = false, bool IncludeDeletedItems = false)
        {

            bool bSaveOK = false;
            ConfirmedPaperReferal = null;
            ConfirmItemValid = false;
            try
            {
                bSaveOK = SaveReferalItems(patientID, allReferalItems, out PaperReferals, ReloadPatientHI, IncludeDeletedItems);
                if (PaperReferals != null)
                {
                    ConfirmedPaperReferal = PaperReferals.Where(item => item.IsActive).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of saving paper referal. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            ConfirmItemValid = ConfirmPaperReferal(ConfirmedPaperReferal, out ConfirmedPaperReferal);
            return bSaveOK;
        }

        /// <summary>
        /// Xu ly dang ky va tra tien luon.
        /// Danh sach chi tiet dang ky da duoc tinh toan roi. Khong can tinh lai tren server lan nua. Chi them vao thoi.
        /// </summary>
        /// <param name="registrationInfo"></param>
        /// <param name="payment"></param>
        /// <param name="PatientRegistrationID"></param>
        /// <param name="SequenceNo"></param>
        /// <param name="TransationID"></param>
        /// <param name="PaymentID"></param>
        /// <returns></returns>
        public bool RegisterAndPay(PatientRegistration registrationInfo, PatientTransactionPayment payment, out long PatientRegistrationID, out int SequenceNo, out long TransationID, out long PaymentID)
        {
            AxResponse response = new AxResponse();
            PatientRegistrationID = -1;
            TransationID = -1;
            PaymentID = -1;
            SequenceNo = -1;

            //Dang ky truoc de lay ma so dang ky. Neu khong duoc thi ve luon.
            try
            {
                AxLogger.Instance.LogInfo("Start registering patient info.", CurrentUser);
                PatientProvider.Instance.RegisterPatient(registrationInfo, out PatientRegistrationID, out SequenceNo);
                registrationInfo.PtRegistrationID = PatientRegistrationID;
                registrationInfo.SequenceNo = SequenceNo;
                AxLogger.Instance.LogInfo("End of registering patient info. Status: Success.", CurrentUser);
            }
            catch (System.Exception ex)
            {
                AxLogger.Instance.LogInfo("End of registering patient info. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

            //Neu dang ky duoc thi bat dau mo transaction va add thong tin payment luon.
            try
            {
                AxLogger.Instance.LogInfo("Start opening transaction and processing payment.", CurrentUser);
                PatientTransaction tran = CreateTransaction(registrationInfo);

                //Tinh tong so tien bao hiem tra, benh nhan tra.
                decimal totalHIPay = 0, totalPatientPay = 0;

                foreach (PatientTransactionDetail item in tran.PatientTransactionDetails)
                {
                    if (item.HealthInsuranceRebate.HasValue)
                    {
                        totalHIPay += item.HealthInsuranceRebate.Value;
                        totalPatientPay += item.Amount - item.HealthInsuranceRebate.Value;
                    }
                    else
                    {
                        totalPatientPay += item.Amount;
                    }
                }
                if (totalPatientPay >= payment.PayAmount)
                {
                    tran.TranPatientPaymentStatus = AllLookupValues.TranPatientPayment.BALANCED;
                }
                if (totalHIPay > 0)
                {
                    tran.TranHIPaymentStatus = AllLookupValues.TranHIPayment.OPENED;
                }
                else
                {
                    tran.TranHIPaymentStatus = AllLookupValues.TranHIPayment.BALANCED;
                }
                //tran.PatientRegistration = registrationInfo;
                //PatientProvider.Instance.OpenTransactionAndProcessPayment(tran, payment, out TransationID, out PaymentID);
                AxLogger.Instance.LogInfo("End of opening transaction and processing payment. Status: Success.", CurrentUser);
            }
            catch (System.Exception ex)
            {
                AxLogger.Instance.LogInfo("End of opening transaction and processing payment. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

            return true;
        }

        /// <summary>
        /// Ham nay dung de tao moi 1 transaction tuong ung voi 1 dang ky da co trong database roi nhung chua duoc tra tien
        /// </summary>
        /// <param name="calculatedRegistrationInfo"></param>
        /// <returns></returns>
        private PatientTransaction CreateTransaction(PatientRegistration calculatedRegistrationInfo)
        {
            PatientTransaction tran = new PatientTransaction();

            tran.TranHIPaymentStatus = AllLookupValues.TranHIPayment.OPENED;
            tran.TranPatientPaymentStatus = AllLookupValues.TranPatientPayment.OPENED;

            tran.PtRegistrationID = calculatedRegistrationInfo.PtRegistrationID;
            tran.PatientTransactionDetails = (new List<PatientTransactionDetail>()).ToObservableCollection();

            foreach (PatientRegistrationDetail d in calculatedRegistrationInfo.PatientRegistrationDetails)
            {
                tran.PatientTransactionDetails.Add(CreateTransactionDetails(d));
            }

            tran.PatientRegistration = calculatedRegistrationInfo;
            return tran;
        }
        /// <summary>
        /// Tao mot transaction details tuong ung voi registration details
        /// </summary>
        /// <param name="regDetails"></param>
        /// <returns></returns>
        private PatientTransactionDetail CreateTransactionDetails(PatientRegistrationDetail regDetails)
        {
            PatientTransactionDetail tranDetails = new PatientTransactionDetail();
            tranDetails.StaffID = regDetails.StaffID;
            tranDetails.PtRegDetailID = regDetails.PtRegDetailID;

            tranDetails.Amount = regDetails.TotalInvoicePrice;
            tranDetails.AmountCoPay = regDetails.TotalCoPayment;
            tranDetails.PriceDifference = regDetails.TotalPriceDifference;

            tranDetails.HealthInsuranceRebate = (decimal)regDetails.TotalHIPayment;

            tranDetails.Qty = (byte)regDetails.Qty;

            return tranDetails;
        }
        public List<PCLExamType> GetPCLExamTypesByMedServiceID(long medServiceID)
        {
            try
            {
                List<PCLExamType> allPCLExamTypes = new List<PCLExamType>();
                allPCLExamTypes = PatientProvider.Instance.GetPclExamTypesByMedServiceID(medServiceID);
                if (allPCLExamTypes != null && allPCLExamTypes.Count > 0)
                {
                    List<PCLExamTypeLocation> deptLocations = new List<PCLExamTypeLocation>();
                    deptLocations = ResourcesManagement.Instance.GetPclExamTypeLocationsByExamTypeList(allPCLExamTypes);
                    if (deptLocations != null)
                    {
                        foreach (var examType in allPCLExamTypes)
                        {
                            examType.PCLExamTypeLocations = new ObservableCollection<PCLExamTypeLocation>(deptLocations.Where(dl => dl.PCLExamTypeID == examType.PCLExamTypeID));
                        }
                    }
                }
                return allPCLExamTypes;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading default pclexamtype. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetPCLExamTypesByMedServiceID);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<DeptLocation> GetDeptLocationsByExamType(long examTypeId)
        {
            try
            {
                List<DeptLocation> deptLocations = new List<DeptLocation>();
                deptLocations = ResourcesManagement.Instance.GetDeptLocationByExamType(examTypeId);
                return deptLocations;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading locations. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetDeptLocationsByExamType);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public void CreateNewPCLRequest(long medServiceID, out PatientPCLRequest NewRequest, out PatientPCLRequest ExternalRequest)
        {
            try
            {
                NewRequest = null;
                ExternalRequest = null;
                //List<PCLExamType> allPCLExamTypes = PatientProvider.Instance.GetPclExamTypesByMedServiceID(medServiceID);
                var allPCLExamTypes = GetPCLExamTypesByMedServiceID(medServiceID);
                if (allPCLExamTypes != null)
                {
                    //Ung voi moi PCLItem -> Tao cho no 1 request details
                    var request = new PatientPCLRequest();
                    request.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
                    request.Diagnosis = "Chưa Xác Định";
                    request.PtRegDetailID = medServiceID;
                    request.PatientPCLRequestIndicators = new List<PatientPCLRequestDetail>().ToObservableCollection();

                    //Tao mot external request neu co.
                    var exRequest = new PatientPCLRequest();
                    exRequest.IsExternalExam = true;
                    exRequest.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
                    exRequest.Diagnosis = "Chưa Xác Định";
                    exRequest.PtRegDetailID = medServiceID;
                    exRequest.PatientPCLRequestIndicators = new List<PatientPCLRequestDetail>().ToObservableCollection();

                    foreach (var item in allPCLExamTypes)
                    {
                        if (item.IsExternalExam.GetValueOrDefault(false))
                        {
                            exRequest.PatientPCLRequestIndicators.Add(CreateRequestDetailsForPCLExamType(item));
                        }
                        else
                        {
                            request.PatientPCLRequestIndicators.Add(CreateRequestDetailsForPCLExamType(item));
                        }
                    }
                    NewRequest = request;
                    if (exRequest.PatientPCLRequestIndicators.Count > 0)
                    {
                        ExternalRequest = exRequest;
                    }
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of registering patient. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_CreateNewPCLRequest);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

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

        // TxD 16/08/2017: Commented out the following method because ...
        //public AllLookupValues.RegistrationPaymentStatus CloseRegistration(long PtRegistrationID, int FindPatient)
        //{
        //    AllLookupValues.RegistrationPaymentStatus PaymentStatus = AllLookupValues.RegistrationPaymentStatus.DEBIT;
        //    try
        //    {
        //        AxLogger.Instance.LogInfo("Start to close this Registration...");
        //        PatientRegistration registrationInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(PtRegistrationID, FindPatient);
        //        decimal sumTran = 0;
        //        decimal sumHI = 0;
        //        decimal sumPayment = 0;
        //        if (registrationInfo.PatientTransaction != null)
        //        {
        //            List<PatientTransactionDetail> allTransactionDetails = PatientProvider.Instance.GetAlltransactionDetails(
        //                    registrationInfo.PatientTransaction.TransactionID);
        //            foreach (var patientTransactionDetail in allTransactionDetails)
        //            {
        //                sumTran += patientTransactionDetail.Amount;
        //                sumHI += (Decimal)patientTransactionDetail.HealthInsuranceRebate;
        //            }
        //        }
        //        if (registrationInfo.PatientTransaction.PatientTransactionPayments != null)
        //        {
        //            foreach (var item in registrationInfo.PatientTransaction.PatientTransactionPayments)
        //            {
        //                sumPayment += item.PayAmount * item.CreditOrDebit;
        //            }
        //        }

        //        if (sumPayment + sumHI > sumTran)
        //        {
        //            PaymentStatus = AllLookupValues.RegistrationPaymentStatus.DEBIT;
        //        }
        //        else
        //        {
        //            if (sumPayment + sumHI < sumTran)
        //            {
        //                PaymentStatus = AllLookupValues.RegistrationPaymentStatus.CREDIT;
        //            }
        //        }
        //        CommonProvider.PatientRg.UpdatePatientRegistration(PtRegistrationID, (long)AllLookupValues.RegistrationStatus.COMPLETED);
        //        CommonProvider.PatientRg.UpdatePatientRegistrationPayStatus(PtRegistrationID, (long)PaymentStatus);
        //        //return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogError(ex);
        //    }
        //    return PaymentStatus;
        //}


        //-------Kiem tra phong kham---------------------
        public List<PatientRegistrationDetail> GetPatientRegistrationDetailsByDay(long DeptLocID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                List<PatientRegistrationDetail> retVal = new List<PatientRegistrationDetail>();
                retVal = PatientProvider.Instance.GetPatientRegistrationDetailsByDay(DeptLocID);
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetPatientRegistrationDetailsByDay);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientRegistrationDetail> GetPatientRegistrationDetailsByDay_ForXML(IList<DeptLocation> lstDeptLoc)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                List<PatientRegistrationDetail> retVal = new List<PatientRegistrationDetail>();
                retVal = PatientProvider.Instance.GetPatientRegistrationDetailsByDay_ForXML(lstDeptLoc);
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetPatientRegistrationDetailsByDay);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientRegistrationDetail> GetPtRegisDetailsByConsultTimeSegment(long DeptLocID
                                                                                            , long ConsultationTimeSegmentID
                                                                                            , long StartSequenceNumber
                                                                                            , long EndSequenceNumber)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                List<PatientRegistrationDetail> retVal = new List<PatientRegistrationDetail>();
                retVal = PatientProvider.Instance.GetPtRegisDetailsByConsultTimeSegment(DeptLocID, ConsultationTimeSegmentID, StartSequenceNumber, EndSequenceNumber);
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetPatientRegistrationDetailsByDay);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientRegistrationDetail> GetPtRegisDetailsByConsultTimeSegment_ForXML(IList<DeptLocation> lstDeptLocation, long ConsultationTimeSegmentID, long StartSequenceNumber, long EndSequenceNumber)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                List<PatientRegistrationDetail> retVal = new List<PatientRegistrationDetail>();
                retVal = PatientProvider.Instance.GetPtRegisDetailsByConsultTimeSegment_ForXML(lstDeptLocation, ConsultationTimeSegmentID, StartSequenceNumber, EndSequenceNumber);
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetPatientRegistrationDetailsByDay);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool checkDeptExist(ref IList<DeptLocInfo> lstDeptLocInfo, long deptLocID, V_ExamRegStatus status, bool MarkedAsDeleted)
        {
            foreach (var deptLocInfo in lstDeptLocInfo)
            {
                if (deptLocInfo.DeptLocationID == deptLocID)
                {
                    if (status == V_ExamRegStatus.mHoanTat
                        || status == V_ExamRegStatus.mNgungTraTienLai
                        || MarkedAsDeleted == true
                        )
                    {
                        deptLocInfo.KhamRoi++;
                    }
                    else
                    {
                        deptLocInfo.ChoKham++;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool? checkDeptExist(int RegStatus)
        {
            if (RegStatus == (int)RegDetailStatus.NotDiagYet)
            {
                return false;
            }
            else
                if (RegStatus == (int)RegDetailStatus.HasDiag
                    || RegStatus == (int)RegDetailStatus.DiagForAnotherDept)
            {
                return true;
            }
            return null;
        }

        public IList<DeptLocInfo> GetAllRegisDeptLoc(SeachPtRegistrationCriteria criteria, IList<DeptLocation> lstDeptLocation)
        {
            IList<DeptLocInfo> di = new List<DeptLocInfo>();
            try
            {
                int total = 0;
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                IList<PatientRegistration> retVal;
                retVal = PatientProvider.Instance.SearchRegistrations(criteria, 0, 1000, true, out total);
                //GetPatientRegistrationDetailsByDay(long DeptLocID)

                foreach (var Deptloc in lstDeptLocation)
                {
                    DeptLocInfo dl = new DeptLocInfo();
                    dl.DeptLocationID = Deptloc.DeptLocationID;
                    dl.Location = new Location();
                    dl.Location.LID = Deptloc.LID;
                    dl.Location.LocationName = Deptloc.Location.LocationName;
                    dl.RefDepartment = new RefDepartment();
                    dl.RefDepartment.DeptName = Deptloc.RefDepartment.DeptName;
                    dl.ChoKham = 0;

                    if (retVal != null)
                    {
                        for (int i = 0; i < retVal.Count; i++)
                        {
                            PatientRegistration patientRegistration = retVal[i];
                            PatientRegistration registrationInfo =
                                eHCMSBillPaymt.RegAndPaymentProcessorBase.GetRegistrationTxd(patientRegistration.PtRegistrationID
                                , (int)AllLookupValues.V_FindPatientType.NOI_TRU);
                            foreach (var regisDetail in registrationInfo.PatientRegistrationDetails)
                            {
                                if (Deptloc.DeptLocationID == regisDetail.DeptLocation.DeptLocationID)
                                {
                                    //checkDeptExist(ref di, Deptloc.DeptLocationID, (V_ExamRegStatus)regisDetail.V_ExamRegStatus, regisDetail.MarkedAsDeleted);
                                    bool? result = checkDeptExist(regisDetail.RegStatus);
                                    if (result == null)
                                    {
                                        continue;
                                    }
                                    if (result.Value)
                                    {
                                        dl.KhamRoi++;
                                    }
                                    else
                                    {
                                        dl.ChoKham++;
                                    }
                                }
                            }
                        }
                    }
                    di.Add(dl);
                }
                return di;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetAllRegisDeptLoc);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public IList<DeptLocInfo> GetAllRegisDeptLocS(IList<DeptLocation> lstDeptLocation)
        {
            IList<DeptLocInfo> di = new List<DeptLocInfo>();
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                //

                foreach (var Deptloc in lstDeptLocation)
                {
                    DeptLocInfo dl = new DeptLocInfo();
                    dl.DeptLocationID = Deptloc.DeptLocationID;
                    dl.Location = new Location();
                    dl.Location.LID = Deptloc.LID;
                    dl.Location.LocationName = Deptloc.Location.LocationName;
                    dl.RefDepartment = new RefDepartment();
                    dl.RefDepartment.DeptName = Deptloc.RefDepartment.DeptName;
                    dl.ChoKham = dl.KhamRoi = 0;
                    var lstPatientRegisDetails = GetPatientRegistrationDetailsByDay(Deptloc.DeptLocationID);
                    foreach (var regisDetail in lstPatientRegisDetails)
                    {
                        bool? result = checkDeptExist(regisDetail.RegStatus);
                        if (result == null)
                        {
                            continue;
                        }
                        if (result.Value)
                        {
                            dl.KhamRoi++;
                        }
                        else
                        {
                            dl.ChoKham++;
                        }
                    }
                    di.Add(dl);

                }
                return di;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<DeptLocInfo> GetAllRegisDeptLoc_ForXML(IList<DeptLocation> lstDeptLocation)
        {
            IList<DeptLocInfo> di = new List<DeptLocInfo>();
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                List<PatientRegistrationDetail> lstPatientRegisDetails = GetPatientRegistrationDetailsByDay_ForXML(lstDeptLocation);
                foreach (var Deptloc in lstDeptLocation)
                {
                    DeptLocInfo dl = new DeptLocInfo();
                    dl.DeptLocationID = Deptloc.DeptLocationID;
                    dl.Location = new Location();
                    dl.Location.LID = Deptloc.LID;
                    dl.Location.LocationName = Deptloc.Location.LocationName;
                    dl.RefDepartment = new RefDepartment();
                    dl.RefDepartment.DeptName = Deptloc.RefDepartment.DeptName;
                    dl.ChoKham = dl.KhamRoi = 0;
                    foreach (var regisDetail in lstPatientRegisDetails)
                    {
                        if (dl.DeptLocationID != regisDetail.DeptLocID)
                        {
                            continue;
                        }
                        bool? result = checkDeptExist(regisDetail.RegStatus);
                        if (result == null)
                        {
                            continue;
                        }
                        if (result.Value)
                        {
                            dl.KhamRoi++;
                        }
                        else
                        {
                            dl.ChoKham++;
                        }
                    }
                    di.Add(dl);

                }
                return di;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public int GetStartSequenceNumber(DateTime curDate, ConsultationRoomTarget crt)
        {
            switch (curDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return crt.MondayStartSequenceNumber;

                case DayOfWeek.Tuesday:
                    return crt.TuesdayStartSequenceNumber;

                case DayOfWeek.Wednesday:
                    return crt.WednesdayStartSequenceNumber;

                case DayOfWeek.Thursday:
                    return crt.ThursdayStartSequenceNumber;

                case DayOfWeek.Friday:
                    return crt.FridayStartSequenceNumber;

                case DayOfWeek.Saturday:
                    return crt.SaturdayStartSequenceNumber;

                case DayOfWeek.Sunday:
                    return crt.SundayStartSequenceNumber;
                default: return 0;
            }
        }
        public int GetEndSequenceNumber(DateTime curDate, ConsultationRoomTarget crt)
        {
            switch (curDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return crt.MondayEndSequenceNumber;

                case DayOfWeek.Tuesday:
                    return crt.TuesdayEndSequenceNumber;

                case DayOfWeek.Wednesday:
                    return crt.WednesdayEndSequenceNumber;

                case DayOfWeek.Thursday:
                    return crt.ThursdayEndSequenceNumber;

                case DayOfWeek.Friday:
                    return crt.FridayEndSequenceNumber;

                case DayOfWeek.Saturday:
                    return crt.SaturdayEndSequenceNumber;

                case DayOfWeek.Sunday:
                    return crt.SundayEndSequenceNumber;
                default: return 0;
            }
        }

        private void CheckStartNumberSequent(long DeptLocationID, long ConsultationTimeSegmentID
            , out int StartSequenceNumber, out int EndSequenceNumber)
        {
            StartSequenceNumber = 0;
            EndSequenceNumber = 0;
            DateTime day = DateTime.Now;
            string mainCacheKey = "AllConsultationRoomTarget";
            ClinicManagementProvider.instance.GetAllConsultationRoomTargetCache();
            if (ServerAppConfig.CachingEnabled)
            {
                var allConsultationRoomTarget = (List<ConsultationRoomTarget>)AxCache.Current[mainCacheKey];
                if (allConsultationRoomTarget != null)
                {
                    var item = allConsultationRoomTarget.Where(o => (o.ConsultationTimeSegmentID == ConsultationTimeSegmentID
                        && o.DeptLocationID == DeptLocationID)).FirstOrDefault();
                    StartSequenceNumber = GetStartSequenceNumber(day, item);
                    EndSequenceNumber = GetEndSequenceNumber(day, item);
                }
            }
        }

        public IList<DeptLocInfo> GetAllRegisDeptLocStaff(IList<DeptLocation> lstDeptLocation, int timeSegment)
        {
            IList<DeptLocInfo> di = new List<DeptLocInfo>();
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                //
                ConsultationTimeSegments curConsultationTimeSegments = new ConsultationTimeSegments();

                List<ConsultationTimeSegments> lstCTS = new List<ConsultationTimeSegments>();
                lstCTS = ClinicManagementProvider.instance.GetAllConsultationTimeSegmentsCache();
                //20191015 TBL: Không cần thiết nữa
                //ConsultationTimeSegments curTimeSegments1 = lstCTS.Where(o => o.V_TimeSegment == (long)V_TimeSegment.CaNgay).FirstOrDefault();
                if (timeSegment == 0)
                {
                    foreach (var consTimeSeg in lstCTS)
                    {
                        if (consTimeSeg.StartTime.Hour < DateTime.Now.Hour && DateTime.Now.Hour < consTimeSeg.EndTime.Hour)
                        {
                            curConsultationTimeSegments = consTimeSeg;
                        }
                    }
                }
                else
                {
                    foreach (var consTimeSeg in lstCTS)
                    {
                        if (consTimeSeg.ConsultationTimeSegmentID == timeSegment)
                        {
                            curConsultationTimeSegments = consTimeSeg;
                        }
                    }
                }
                //20191015 TBL: Chỉ gọi 1 lần xuống store
                List<ConsultationRoomStaffAllocations> allstaff = new List<ConsultationRoomStaffAllocations>();
                allstaff = ClinicManagementProvider.instance.GetConsultationRoomStaffAllocations_ForXML(lstDeptLocation, curConsultationTimeSegments.ConsultationTimeSegmentID);
                var tempPatientRegisDetails = GetPtRegisDetailsByConsultTimeSegment_ForXML(lstDeptLocation, curConsultationTimeSegments.V_TimeSegment, 0, 0);

                foreach (var Deptloc in lstDeptLocation)
                {
                    DeptLocInfo dl = new DeptLocInfo();
                    dl.DeptLocationID = Deptloc.DeptLocationID;
                    dl.Location = new Location();
                    dl.Location.LID = Deptloc.LID;
                    dl.Location.LocationName = Deptloc.Location.LocationName;
                    dl.RefDepartment = new RefDepartment();
                    dl.RefDepartment.DeptName = Deptloc.RefDepartment.DeptName;
                    dl.ChoKham = 0;
                    dl.ConsultTimeSeg = curConsultationTimeSegments;
                    dl.lstStaff = new ObservableCollection<Staff>();
                    //20191015 TBL: Không gọi store nhiều lần
                    //List<ConsultationRoomStaffAllocations> allstaff = new List<ConsultationRoomStaffAllocations>();
                    //allstaff = ClinicManagementProvider.instance.GetConsultationRoomStaffAllocations(Deptloc.DeptLocationID, curConsultationTimeSegments.ConsultationTimeSegmentID);
                    foreach (var Staff in allstaff)
                    {
                        if (Staff.AllocationDate.ToShortDateString() == allstaff[0].AllocationDate.ToShortDateString())
                        {
                            if (Staff.Staff.RefStaffCategory.V_StaffCatType == (long)V_StaffCatType.BacSi)
                            {
                                if (dl.staffName != "" && dl.staffName != null)
                                {
                                    dl.staffName += ", ";
                                }
                                dl.staffName += Staff.Staff.FullName;
                                dl.lstStaff.Add(Staff.Staff);
                            }
                        }
                    }
                    //kiem tra xem phòng và ca
                    //kiem tra ca bat dau tu so thu tu may
                    //int StartSequenceNumberCa1 = 0;
                    //int EndSequenceNumberCa1 = 0;
                    //lay len tat ca 
                    //20191015 TBL: Không gọi store nhiều lần
                    //var tempPatientRegisDetails = GetPtRegisDetailsByConsultTimeSegment(Deptloc.DeptLocationID, curConsultationTimeSegments.V_TimeSegment, 0, 0);
                    //20191015 TBL: Không cần thiết nữa
                    //CheckStartNumberSequent(Deptloc.DeptLocationID, curTimeSegments1.ConsultationTimeSegmentID, out StartSequenceNumberCa1, out EndSequenceNumberCa1);
                    //List<PatientRegistrationDetail> lstCa1 = new List<PatientRegistrationDetail>();
                    //List<PatientRegistrationDetail> lstCa2 = new List<PatientRegistrationDetail>();
                    //List<PatientRegistrationDetail> lstPatientRegisDetails = new List<PatientRegistrationDetail>();
                    //int count = 0;
                    //if (tempPatientRegisDetails != null && tempPatientRegisDetails.Count > 0)
                    //{
                    //    foreach (var item in tempPatientRegisDetails)
                    //    {
                    //        if (item.RecCreatedDate.Value.Hour < curTimeSegments1.EndTime.Hour
                    //            && count < EndSequenceNumberCa1)
                    //        {
                    //            lstCa1.Add(item);
                    //            count++;
                    //        }
                    //        else
                    //        {
                    //            lstCa2.Add(item);
                    //        }
                    //    }
                    //}

                    //chọn ra
                    //switch (curConsultationTimeSegments.V_TimeSegment)
                    //{
                    //    case (long)V_TimeSegment.Ca1:
                    //        lstPatientRegisDetails = lstCa1;
                    //        break;
                    //    case (long)V_TimeSegment.Ca2:
                    //        lstPatientRegisDetails = lstCa2;
                    //        break;
                    //}

                    foreach (var regisDetail in tempPatientRegisDetails)
                    {
                        //checkDeptExist(ref di, Deptloc.DeptLocationID, (V_ExamRegStatus)regisDetail.V_ExamRegStatus, regisDetail.MarkedAsDeleted);                        
                        bool? result = checkDeptExist(regisDetail.RegStatus);
                        if (result == null)
                        {
                            continue;
                        }
                        if (result.Value)
                        {
                            dl.KhamRoi++;
                        }
                        else
                        {
                            if (regisDetail.DeptLocID == Deptloc.DeptLocationID)
                            {
                                dl.ChoKham++;
                            }
                        }
                    }
                    di.Add(dl);
                }
                return di;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public IList<PatientRegistrationDetail> GetAllRegisDetail(IList<DeptLocInfo> lstDeptInfo)
        {
            IList<PatientRegistrationDetail> di = new List<PatientRegistrationDetail>();
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                //

                foreach (var Deptloc in lstDeptInfo)
                {
                    var lstPatientRegisDetails = GetPatientRegistrationDetailsByDay(Deptloc.DeptLocationID);
                    foreach (var regisDetail in lstPatientRegisDetails)
                    {
                        di.Add(regisDetail);
                    }
                }
                return di;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }


        //------------------------------

        public IList<PatientRegistration> GetSumRegistrations(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                IList<PatientRegistration> retVal;
                retVal = PatientProvider.Instance.SearchRegistrations(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
                //if (retVal!=null)
                //{
                //    for (int i = 0;i<retVal.Count; i++)
                //    {
                //        PatientRegistration patientRegistration = retVal[i];
                //        PatientRegistration registrationInfo =
                //            RegAndPaymentProcessorBase.GetRegistration(patientRegistration.PtRegistrationID);
                //        //Kiem tra giua 2 ben
                //        if (registrationInfo.PatientTransaction == null)
                //        {
                //            if ((registrationInfo.PatientRegistrationDetails!=null
                //                && registrationInfo.PatientRegistrationDetails.Count>0)
                //                ||(registrationInfo.PCLRequests != null
                //                && registrationInfo.PCLRequests.Count > 0)
                //                ||(registrationInfo.DrugInvoices!= null
                //                && registrationInfo.DrugInvoices.Count > 0))
                //            {
                //                retVal[i].CheckTransation = 1;
                //                patientRegistration.CheckTransation = 1;
                //                //fail

                //            }
                //            continue;    
                //        }
                //        bool flag = true;
                //        List<PatientTransactionDetail> allTranDetails =PatientProvider.Instance.GetAlltransactionDetailsSum(
                //                                                registrationInfo.PatientTransaction.TransactionID);
                //        if (registrationInfo.PatientRegistrationDetails!=null
                //                && registrationInfo.PatientRegistrationDetails.Count>0)
                //        {
                //            foreach (var pgd in registrationInfo.PatientRegistrationDetails)
                //            {
                //                if (pgd.PaidTime==null)
                //                {
                //                    continue;
                //                }
                //                flag = false;
                //                long ptID = pgd.PtRegDetailID;
                //                foreach (var patientTransactionDetail in allTranDetails)
                //                {
                //                    if(patientTransactionDetail.PtRegDetailID == ptID)
                //                    {
                //                        flag = true;
                //                        break;
                //                    }
                //                }   
                //                if(flag==false)
                //                {
                //                    retVal[i].CheckTransation = 2;
                //                    patientRegistration.CheckTransation = 2;
                //                    break;
                //                }
                //            }

                //        }
                //        if (flag == false)
                //            continue;
                //        if (registrationInfo.PCLRequests!= null
                //                && registrationInfo.PCLRequests.Count > 0)
                //        {
                //            foreach (var pgd in registrationInfo.PCLRequests)
                //            {
                //                if (pgd.PaidTime == null)
                //                {
                //                    continue;
                //                }
                //                flag = false;
                //                long ptID = pgd.PatientPCLReqID;
                //                foreach (var patientTransactionDetail in allTranDetails)
                //                {
                //                    if (patientTransactionDetail.PCLRequestID == ptID)
                //                    {
                //                        flag = true;
                //                        break;
                //                    }
                //                }
                //                if (flag == false)
                //                {
                //                    retVal[i].CheckTransation = 2;
                //                    patientRegistration.CheckTransation = 2;
                //                    break;
                //                }

                //            }

                //        }
                //        if(flag==false)
                //            continue;
                //        if (registrationInfo.DrugInvoices != null
                //                && registrationInfo.DrugInvoices.Count > 0)
                //        {
                //            foreach (var pgd in registrationInfo.DrugInvoices)
                //            {
                //                //if (pgd.PaidTime == null)
                //                //{
                //                //    continue;
                //                //}
                //                flag = false;
                //                long ptID = pgd.outiID;
                //                foreach (var patientTransactionDetail in allTranDetails)
                //                {
                //                    if (patientTransactionDetail.outiID== ptID)
                //                    {
                //                        flag = true;
                //                        break;
                //                    }
                //                }
                //                if (flag == false)
                //                {
                //                    retVal[i].CheckTransation = 2;
                //                    patientRegistration.CheckTransation = 2;
                //                    break;
                //                }
                //            }
                //        }
                //    }

                //}

                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetSumRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public PatientRegistration GetPatientRegistrationByPtRegistrationID(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                PatientRegistration retVal = new PatientRegistration();
                retVal = PatientProvider.Instance.GetPatientRegistrationByPtRegistrationID(PtRegistrationID);
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public long CheckRegistrationStatus(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start CheckRegistrationStatus...", CurrentUser);
                long l;
                l = PatientProvider.Instance.CheckRegistrationStatus(PtRegistrationID);
                AxLogger.Instance.LogInfo("End of CheckRegistrationStatus. Status: Success", CurrentUser);
                return l;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CheckRegistrationStatus. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PatientRegistration> SearchRegistrations(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                IList<PatientRegistration> retVal;
                retVal = PatientProvider.Instance.SearchRegistrations(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public IList<PatientRegistration> SearchRegistrationsInPt(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, bool bCalledFromSearchInPtPopup, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching in patient registration...", CurrentUser);
                IList<PatientRegistration> retVal;
                retVal = PatientProvider.Instance.SearchRegistrationsInPt(criteria, pageIndex, pageSize, bCountTotal, bCalledFromSearchInPtPopup, out totalCount);
                AxLogger.Instance.LogInfo("End of searching in patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching in patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrationsInPt);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public List<PatientRegistrationDetail> SearchRegistrationsForDiag(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                List<PatientRegistrationDetail> retVal = new List<PatientRegistrationDetail>();
                retVal = PatientProvider.Instance.SearchRegistrationsForDiag(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<PatientRegistrationDetail> SearchRegistrationsForProcess(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                List<PatientRegistrationDetail> retVal = new List<PatientRegistrationDetail>();
                retVal = PatientProvider.Instance.SearchRegistrationsForProcess(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public List<PatientRegistrationDetail> SearchRegistrationsForMedicalExaminationDiag(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration for medical examination...", CurrentUser);
                List<PatientRegistrationDetail> retVal = new List<PatientRegistrationDetail>();
                retVal = PatientProvider.Instance.SearchRegistrationsForMedicalExaminationDiag(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration for medical examination. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        public IList<PatientRegistration> SearchRegisPrescription(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                IList<PatientRegistration> retVal;
                retVal = PatientProvider.Instance.SearchRegisPrescription(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        //SearchRegistrationListForOST
        public IList<PatientRegistrationDetail> SearchRegistrationListForOST(long DeptID, long DeptLocID, long StaffID, long ExamRegStatus, long V_OutPtEntStatus)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration for OutStandingTask...", CurrentUser);
                IList<PatientRegistrationDetail> retVal;
                retVal = PatientProvider.Instance.SearchRegistrationListForOST(DeptID, DeptLocID, StaffID, ExamRegStatus, V_OutPtEntStatus);
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration for OutStandingTask. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public IList<PatientRegistrationDetail> SearchRegisDetailPrescription(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                IList<PatientRegistrationDetail> retVal;
                retVal = PatientProvider.Instance.SearchRegisDetailPrescription(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public IList<PatientRegistration> SearchRegistrationsDiagTrmt(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration DiagTrmt...", CurrentUser);
                IList<PatientRegistration> retVal;
                retVal = PatientProvider.Instance.SearchRegistrationsDiagTrmt(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
                AxLogger.Instance.LogInfo("End of searching patient registration DiagTrmt. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }

        public List<PatientQueue> PatientQueue_GetListPaging(PatientQueueSearchCriteria Criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return PatientProvider.Instance.PatientQueue_GetListPaging(Criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientQueue_GetListPaging. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "PatientQueue_GetListPaging cannot Load");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenMedDrugDetails> SearchMedDrugs(DrugSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return PatientProvider.Instance.SearchMedDrugs(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Medical Drug List. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenMedProductDetails> SearchMedProducts(MedProductSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return PatientProvider.Instance.SearchMedProducts(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Medical Drug List. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InwardDrugClinicDept> SearchInwardDrugClinicDept(MedProductSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return PatientProvider.Instance.SearchInwardDrugClinicDept(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Medical Drug List. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InwardDrugClinicDept> GetAllInwardDrugClinicDeptByIDList(List<long> inwardDrugIdList)
        {
            try
            {
                return PatientProvider.Instance.GetAllInwardDrugClinicDeptByIDList(inwardDrugIdList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Medical Drug List. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefGenMedProductDetails> GetGenMedProductsRemainingInStore(Dictionary<long, List<long>> drugIdList)
        {
            try
            {
                return PatientProvider.Instance.GetGenMedProductsRemainingInStore(drugIdList);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Medical Drug List. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<RefMedicalServiceItem> SearchMedServices(MedicalServiceSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return PatientProvider.Instance.SearchMedServices(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Medical Drug List. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<PCLItem> SearchPCLItems(PCLItemSearchCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                return PatientProvider.Instance.SearchPCLItems(criteria, pageIndex, pageSize, bCountTotal, out totalCount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Medical Drug List. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Medical Drug List");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<Staff> GetStaffsHaveRegistrations(byte Type)
        {
            try
            {
                return PatientProvider.Instance.GetStaffsHaveRegistrations(Type);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Staff List. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Staff list");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<Staff> GetStaffsHavePayments(long V_TradingPlaces)
        {
            try
            {
                return PatientProvider.Instance.GetStaffsHavePayments(V_TradingPlaces);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Staff List. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Staff list");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RegistrationSummaryInfo> SearchRegistrationSummaryList(SeachPtRegistrationCriteria criteria, int pageIndex, int pageSize, bool bCountTotal, out int totalCount, out RegistrationsTotalSummary totalSummaryInfo)
        {
            try
            {
                return PatientProvider.Instance.SearchRegistrationSummaryList(criteria, pageIndex, pageSize, bCountTotal, out totalCount, out totalSummaryInfo);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving Staff List. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Cannot load Staff list");

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool GetInPatientRegistrationNonFinalizedLiabilities(long registrationID, out decimal TotalLiabilities, out decimal SumOfAdvance, out decimal TotalPatientPayment_PaidInvoice, out decimal TotalRefundPatient
                                                                    , out decimal TotalCharityOrgPayment, out decimal TotalPatientPayment_NotFinalized, out decimal TotalPatientPaid_NotFinalized, out decimal TotalCharityOrgPayment_NotFinalized)
        {
            try
            {
                decimal tmp;
                AxLogger.Instance.LogInfo("Start loading In-Patient Non-Finalized Liabilities...", CurrentUser);
                bool retVal;
                retVal = PatientProvider.Instance.GetInPatientRegistrationNonFinalizedLiabilities(registrationID, false, out TotalLiabilities, out SumOfAdvance, out TotalPatientPayment_PaidInvoice, out TotalRefundPatient, out tmp
                                                                                                    , out TotalCharityOrgPayment, out TotalPatientPayment_NotFinalized, out TotalPatientPaid_NotFinalized, out TotalCharityOrgPayment_NotFinalized);
                //GetPatientRegistrationDetailsByDay(long DeptLocID)
                AxLogger.Instance.LogInfo("End of loading In-Patient Non-Finalized Liabilities. Status: Success.", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading In-Patient Non-Finalized Liabilities. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetInPatientRegistrationNonFinalizedLiabilities);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<RefGenMedProductSummaryInfo> GetRefGenMedProductSummaryByRegistration(long registrationID, AllLookupValues.MedProductType medProductType, long? DeptID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading OutwardDrugClinicDept By Registration...", CurrentUser);
                List<RefGenMedProductSummaryInfo> retVal = new List<RefGenMedProductSummaryInfo>();
                retVal = PatientProvider.Instance.GetRefGenMedProductSummaryByRegistration(registrationID, (long)medProductType, DeptID);
                AxLogger.Instance.LogInfo("End of loading OutwardDrugClinicDept By Registration. Status: Success.", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading InOutwardDrugClinicDept By Registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetAllOutwardDrugClinicDeptByRegistration);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public List<BedPatientRegDetail> GetAllBedPatientRegDetailsByBedPatientID(long BedPatientId, bool IncludeDeletedItems)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading BedPatientRegDetails by BedPatientID...", CurrentUser);
                List<BedPatientRegDetail> retVal = new List<BedPatientRegDetail>();
                retVal = PatientProvider.Instance.GetAllBedPatientRegDetailsByBedPatientID(BedPatientId, IncludeDeletedItems);
                AxLogger.Instance.LogInfo("End of loading BedPatientRegDetails by BedPatientID. Status: Success.", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading loading BedPatientRegDetails by BedPatientID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_GetPatientBedRegDetailsFailed);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        // TxD 16/08/2017: Commented out the following method because ...
        //public bool CloseRegistration(long registrationID, int FindPatient, out List<string> errorMessages)
        //{
        //    string strLog = "";
        //    bool retVal = false;
        //    errorMessages = new List<string>();
        //    try
        //    {
        //        strLog = string.Format("Start closing registration (RegistrationID = {0})", registrationID);
        //        AxLogger.Instance.LogInfo(strLog, CurrentUser);

        //        //TODO:
        //        /*
        //         * Kiểm tra đăng ký có hay chưa. 
        //         * Kiểm tra có transaction chưa.
        //         * Nếu có transaction -> xem lịch sử thanh toán
        //         * Tổng số tiền thanh toán có balance không.
        //         */
        //        using (DbConnection connection = PatientProvider.Instance.CreateConnection())
        //        {
        //            PatientRegistration regInfo = PatientProvider.Instance.GetRegistration(registrationID, FindPatient, connection, null);
        //            if (regInfo == null)
        //            {
        //                errorMessages.Add("Không tìm thấy đăng ký");
        //                return retVal;
        //            }
        //            AllLookupValues.RegistrationClosingStatus newStatus;
        //            if (regInfo.PatientTransaction != null)
        //            {
        //                var allPayments = CommonProvider.Payments.GetAllPayments_New(regInfo.PatientTransaction.TransactionID, connection, null);
        //                if (allPayments != null)
        //                {
        //                    regInfo.PatientTransaction.PatientTransactionPayments = allPayments.ToObservableCollection();
        //                }
        //                //var allPayments = CommonProvider.Payments.GetAllPayments(regInfo.PatientTransaction.TransactionID, connection, null);
        //                //if (allPayments != null)
        //                //{
        //                //    regInfo.PatientTransaction.PatientPayments = allPayments.ToObservableCollection();
        //                //}
        //                decimal total = PatientProvider.Instance.GetTotalPatientPayForTransaction(regInfo.PatientTransaction.TransactionID, connection, null);
        //                // decimal totalPatientPaid = regInfo.PatientTransaction.PatientPayments.Sum(payment => payment.PayAmount * payment.CreditOrDebit);
        //                decimal totalPatientPaid = regInfo.PatientTransaction.PatientTransactionPayments.Sum(payment => payment.PayAmount * payment.CreditOrDebit);
        //                if (total > totalPatientPaid) //BN tra thieu.
        //                {
        //                    //newStatus = AllLookupValues.RegistrationClosingStatus.NOTBALANCED_DEBIT;
        //                    errorMessages.Add("Bệnh nhân chưa trả hết tiền cho đăng ký này.");
        //                    return retVal;
        //                }
        //                else if (total == totalPatientPaid)
        //                {
        //                    newStatus = AllLookupValues.RegistrationClosingStatus.BALANCED;
        //                }
        //                else
        //                {
        //                    newStatus = AllLookupValues.RegistrationClosingStatus.NOTBALANCED_CREDIT;
        //                }
        //            }
        //            else
        //            {
        //                newStatus = AllLookupValues.RegistrationClosingStatus.BALANCED;
        //            }
        //            retVal = PatientProvider.Instance.CloseRegistration(registrationID, newStatus, connection, null);
        //        }

        //        strLog = string.Format("End of closing registration (RegistrationID = {0}). Status: Success", registrationID);
        //        AxLogger.Instance.LogInfo(strLog, CurrentUser);
        //        if (!retVal)
        //        {
        //            errorMessages = null;
        //        }
        //        return retVal;
        //    }
        //    catch (Exception ex)
        //    {
        //        strLog = string.Format("End of closing registration (RegistrationID = {0}). Status: Failed", registrationID);
        //        AxLogger.Instance.LogInfo(strLog, CurrentUser);
        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_LoadInPatientRegItemsIntoBill);
        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        // TxD 28/01/2018: Commented OUT the following because It has NEVER been USED 
        //public bool ConfirmHIBenefit(long staffID, long patientID, long hiid, float? benefit)
        //{
        //    var strLog = "";
        //    var retVal = false;
        //    try
        //    {
        //        strLog = string.Format("Start confirming hi benefit (HIID = {0})", hiid);
        //        AxLogger.Instance.LogInfo(strLog, CurrentUser);

        //        PatientProvider.Instance.ConfirmHIBenefit(staffID, patientID, hiid, benefit);

        //        strLog = string.Format("End of confirming hi benefit (HIID = {0}). Status: Success", hiid);
        //        AxLogger.Instance.LogInfo(strLog, CurrentUser);

        //        return retVal;
        //    }
        //    catch (Exception ex)
        //    {
        //        strLog = string.Format("End of confirming hi benefit (HIID = {0}). Status: Failed", hiid);
        //        AxLogger.Instance.LogInfo(strLog, CurrentUser);
        //        var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_ConfirmHiBenefit);
        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        public PatientRegistration GetRegistraionVIPByPatientID(long PatientID)
        {
            try
            {
                return PatientProvider.Instance.GetRegistraionVIPByPatientID(PatientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRegistraionVIPByPatientID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_GetRegistraionVIPByPatientID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //HPT 24/08/2015: Thêm parameter vào hàm ApplyHiToInPatientRegistration để tính quyền lợi khi xác nhận bảo hiểm (nội trú) có xét đến điều kiện người tham gia bảo hiểm 5 năm liên tiếp
        /*==== #002 ====*/
        //public bool ApplyHiToInPatientRegistration(long RegistrationID, long HIID, double HiBenefit, bool IsCrossRegion, long? PaperReferalID, int FindPatient, bool bIsEmergency, long ConfirmHiStaffID, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld)
        public bool ApplyHiToInPatientRegistration(long RegistrationID, long HIID, double HiBenefit, bool IsCrossRegion, long? PaperReferalID, int FindPatient,
                    bool bIsEmergency, long ConfirmHiStaffID, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, TypesOf_ConfirmHICardForInPt eConfirmType = TypesOf_ConfirmHICardForInPt.eConfirmNew)
        {
            return ApplyHiToInPatientRegistration_V2(RegistrationID, HIID, HiBenefit, IsCrossRegion, PaperReferalID, FindPatient, bIsEmergency, ConfirmHiStaffID, IsHICard_FiveYearsCont, IsChildUnder6YearsOld, false);
        }
        public bool ApplyHiToInPatientRegistration_V2(long RegistrationID, long HIID, double HiBenefit, bool IsCrossRegion, long? PaperReferalID, int FindPatient,
                    bool bIsEmergency, long ConfirmHiStaffID, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsHICard_FiveYearsCont_NoPaid, TypesOf_ConfirmHICardForInPt eConfirmType = TypesOf_ConfirmHICardForInPt.eConfirmNew)
        {
            return ApplyHiToInPatientRegistration_V3(RegistrationID, HIID, HiBenefit, IsCrossRegion, PaperReferalID, FindPatient, bIsEmergency, ConfirmHiStaffID, IsHICard_FiveYearsCont, IsChildUnder6YearsOld, false);
        }
        public bool ApplyHiToInPatientRegistration_V3(long RegistrationID, long HIID, double HiBenefit, bool IsCrossRegion, long? PaperReferalID, int FindPatient
            , bool bIsEmergency, long ConfirmHiStaffID, bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsHICard_FiveYearsCont_NoPaid
            , DateTime? FiveYearsAppliedDate = null, DateTime? FiveYearsARowDate = null, TypesOf_ConfirmHICardForInPt eConfirmType = TypesOf_ConfirmHICardForInPt.eConfirmNew
            , bool IsAllowCrossRegion = false)
        /*==== #002 ====*/
        {
            try
            {
                long? hisID;
                hisID = PatientProvider.Instance.GetActiveHisID(HIID);
                if (hisID == null)
                {
                    throw new Exception(eHCMSResources.Z1794_G1_CannotLoadHIItem);
                }
                bool bOK;
                bOK = PatientProvider.Instance.ApplyHiToInPatientRegistration(RegistrationID, HIID, hisID.Value, HiBenefit, IsCrossRegion, PaperReferalID, FindPatient,
                    bIsEmergency, ConfirmHiStaffID, IsHICard_FiveYearsCont, IsChildUnder6YearsOld, IsHICard_FiveYearsCont_NoPaid, FiveYearsAppliedDate, FiveYearsARowDate, 
                    eConfirmType, IsAllowCrossRegion);
                //TxD 24/12/2014: Update Paper Referal ID with PtRegistrationID
                if (PaperReferalID.HasValue && PaperReferalID > 0)
                {
                    PaperReferal paperRef = new PaperReferal();
                    paperRef.RefID = PaperReferalID.Value;
                    paperRef.PtRegistrationID = RegistrationID;
                    PatientProvider.Instance.UpdatePaperReferalRegID(paperRef);
                }

                return bOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ApplyHiToInPatientRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_ApplyHiToInPatientRegistration);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message.ToString()));
            }
        }

        public bool RemoveHiFromInPatientRegistration(long RegistrationID, bool bIsEmergency, long RemoveHiStaffID, long? OldPaperReferalID)
        {
            try
            {
                bool bOK;
                bOK = PatientProvider.Instance.RemoveHiFromInPatientRegistration(RegistrationID, bIsEmergency, RemoveHiStaffID, OldPaperReferalID);
                return bOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of RemoveHiFromInPatientRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_ApplyHiToInPatientRegistration);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<InPatientAdmDisDetails> InPatientAdmDisDetailsSearch(InPatientAdmDisDetailSearchCriteria p)
        {
            try
            {
                DbConnection conn;
                conn = PatientProvider.Instance.CreateConnection();
                using (conn)
                {
                    conn.Open();
                    // =====▼ #003                                                   
                    // DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                    DbTransaction tran = null;
                    // =====▲ #003

                    try
                    {
                        AxLogger.Instance.LogInfo("Start updating In-Patient admission.", CurrentUser);
                        IList<InPatientAdmDisDetails> lst = PatientProvider.Instance.InPatientAdmDisDetailsSearch(p, conn, tran);
                        AxLogger.Instance.LogInfo("End of updating In-Patient admission.", CurrentUser);
                        return lst;
                        //AxLogger.Instance.LogInfo("End of updating In-Patient admission.", CurrentUser);

                        //// =====▼ #003                                                   
                        //// tran.Commit();                                                 
                        //// =====▲ #003

                        //return lst;
                    }
                    catch (Exception exObj)
                    {
                        AxLogger.Instance.LogError(exObj);
                        // =====▼ #003                                                   
                        // tran.Rollback();
                        // =====▲ #003
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of update In-Patient admission. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateInPatientAdmDisDetails, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<InPatientAdmDisDetails> InPatientAdmDisDetailsSearchPaging(InPatientAdmDisDetailSearchCriteria p, int pageIndex, int pageSize, bool bCountTotal, out int totalCount)
        {
            try
            {
                DbConnection conn;
                conn = PatientProvider.Instance.CreateConnection();
                using (conn)
                {
                    conn.Open();
                    // =====▼ #003                                                   
                    // DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                    DbTransaction tran = null;
                    // =====▲ #003

                    try
                    {
                        AxLogger.Instance.LogInfo("Start updating In-Patient admission.", CurrentUser);
                        List<InPatientAdmDisDetails> lst = new List<InPatientAdmDisDetails>();
                        lst = PatientProvider.Instance.InPatientAdmDisDetailsSearchPaging(p, pageIndex, pageSize, bCountTotal, out totalCount, conn, tran);
                        AxLogger.Instance.LogInfo("End of updating In-Patient admission.", CurrentUser);
                        // =====▼ #003                                                                           
                        // tran.Commit();                                                 
                        // =====▲ #003

                        return lst;
                    }
                    catch (Exception ex)
                    {
                        AxLogger.Instance.LogError(ex);
                        // =====▼ #003                                                                           
                        // tran.Rollback();                                                                           
                        // =====▲ #003
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of update In-Patient admission. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateInPatientAdmDisDetails, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool UpdatePclRequestInfo(PatientPCLRequest p)
        {
            try
            {
                return PatientProvider.Instance.UpdatePclRequestInfo(p);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ApplyHiToInPatientRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_ApplyHiToInPatientRegistration);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #region StaffDeptResponsibility

        public List<InPatientTransferDeptReq> GetInPatientTransferDeptReq(InPatientTransferDeptReq p)
        {
            try
            {
                return PatientProvider.Instance.GetInPatientTransferDeptReq(p);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInPatientTransferDeptReq. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_GetRegistraionVIPByPatientID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool InsertInPatientTransferDeptReq(InPatientTransferDeptReq p)
        {
            try
            {
                return PatientProvider.Instance.InsertInPatientTransferDeptReq(p);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetRegistraionVIPByPatientID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_GetRegistraionVIPByPatientID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool UpdateInPatientTransferDeptReq(InPatientTransferDeptReq p)
        {
            try
            {
                return PatientProvider.Instance.UpdateInPatientTransferDeptReq(p);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateInPatientTransferDeptReq. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_GetRegistraionVIPByPatientID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteInPatientTransferDeptReq(long InPatientTransferDeptReqID)
        {
            try
            {
                return PatientProvider.Instance.DeleteInPatientTransferDeptReq(InPatientTransferDeptReqID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInPatientTransferDeptReq. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_GetRegistraionVIPByPatientID);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool UpdateDeleteInPatientDeptDetails(InPatientDeptDetail inDeptDetailToDelete, InPatientDeptDetail inDeptDetailToUpdate,
                out InPatientDeptDetail inDeptDetailUpdated)
        {
            try
            {
                return PatientProvider.Instance.UpdateDeleteInPatientDeptDetails(inDeptDetailToDelete, inDeptDetailToUpdate, out inDeptDetailUpdated);
            }
            catch (Exception ex)
            {
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_GetRegistraionVIPByPatientID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }


        public bool InPatientDeptDetailsTranfer(InPatientDeptDetail p, long InPatientTransferDeptReqID
            , bool IsAutoCreateOutDeptDiagnosis
            , out long InPatientDeptDetailID)
        {
            try
            {
                return PatientProvider.Instance.InPatientDeptDetailsTranfer(p, InPatientTransferDeptReqID, out InPatientDeptDetailID, IsAutoCreateOutDeptDiagnosis);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InPatientDeptDetailsTranfer. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_GetRegistraionVIPByPatientID);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool DeleteInPatientTransferDeptReqXML(IList<InPatientTransferDeptReq> lstInPtTransDeptReq)
        {
            try
            {
                return PatientProvider.Instance.DeleteInPatientTransferDeptReqXML(lstInPtTransDeptReq);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteInPatientTransferDeptReq. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_GetRegistraionVIPByPatientID);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool OutDepartment(InPatientDeptDetail InPtDeptDetail)
        {
            try
            {
                return PatientProvider.Instance.OutDepartment(InPtDeptDetail);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of OutDepartment. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.InPatientDeptDetail_OutDepartment);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        #endregion

        public PatientRegistrationDetail GetPtRegDetailNewByPatientID(long PatientID)
        {
            try
            {
                return PatientProvider.Instance.GetPtRegDetailNewByPatientID(PatientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientServiceRecordsGetForKhamBenh. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_STAFF_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientServiceRecord> PatientServiceRecordsFromPatientID(long PatientID)
        {
            try
            {
                long ptRegistrationID = 0;
                long ptRegisDetailID = 0;
                //PatientProvider.Instance.GetPtRegDetailIDNewByPatientID(PatientID, ref ptRegistrationID, ref ptRegisDetailID);
                PatientServiceRecord entity = new PatientServiceRecord
                {
                    PtRegDetailID = (long?)ptRegisDetailID,
                    PtRegistrationID = ptRegistrationID,
                    V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU

                };
                return CommonUtilsProvider.Instance.PatientServiceRecordsGetForKhamBenh(entity);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientServiceRecordsGetForKhamBenh. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_STAFF_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //HPT 10/01/2017
        //public List<HICardType> GetHICardTypeList()
        //{
        //    try
        //    {
        //        return PatientProvider.Instance.GetHICardTypeList();
        //    }
        //    catch (Exception ex)
        //    {
        //        AxLogger.Instance.LogInfo("End of retrieving PatientServiceRecordsGetForKhamBenh. Status: Failed.", CurrentUser);

        //        AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_STAFF_CANNOT_FOUND);

        //        throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
        //    }
        //}

        #region dinh them phan nay cho can lam sang ngoai vien

        public PatientPCLRequest_Ext AddPCLRequestExtWithDetails(PatientPCLRequest_Ext request, out long PatientPCLReqExtID)
        {
            PatientPCLRequest_Ext pa = null;
            try
            {
                DbConnection connection;
                connection = PatientProvider.Instance.CreateConnection();
                using (connection)
                {
                    AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                    bool b;
                    b = PatientProvider.Instance.AddPCLRequestExtWithDetails(request, out PatientPCLReqExtID);
                    if (PatientPCLReqExtID > 0)
                    {
                        pa = PatientProvider.Instance.PatientPCLRequestExtByID(PatientPCLReqExtID);
                    }
                    AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                    return pa;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool PCLRequestExtUpdate(PatientPCLRequest_Ext request)
        {
            try
            {
                DbConnection connection;
                connection = PatientProvider.Instance.CreateConnection();
                using (connection)
                {
                    AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                    bool b;
                    b = PatientProvider.Instance.PCLRequestExtUpdate(request);
                    AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                    return b;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeletePCLRequestDetailExtList(List<PatientPCLRequestDetail_Ext> requestDetailList)
        {
            try
            {
                DbConnection connection;
                connection = PatientProvider.Instance.CreateConnection();
                using (connection)
                {
                    AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                    bool b;
                    b = PatientProvider.Instance.DeletePCLRequestDetailExtList(requestDetailList);
                    AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                    return b;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientPCLRequest_Ext> GetPCLRequestListExtByRegistrationID(long RegistrationID)
        {
            try
            {
                DbConnection connection;
                connection = PatientProvider.Instance.CreateConnection();
                using (connection)
                {
                    AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                    List<PatientPCLRequest_Ext> lst = new List<PatientPCLRequest_Ext>();
                    lst = PatientProvider.Instance.GetPCLRequestListExtByRegistrationID(RegistrationID);
                    AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                    return lst;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PatientPCLRequest_Ext GetPCLRequestExtPK(long PatientPCLReqExtID)
        {
            try
            {
                DbConnection connection;
                connection = PatientProvider.Instance.CreateConnection();
                using (connection)
                {
                    AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                    PatientPCLRequest_Ext pa = new PatientPCLRequest_Ext();
                    pa = PatientProvider.Instance.GetPCLRequestExtPK(PatientPCLReqExtID);
                    AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                    return pa;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PatientPCLRequest_Ext PatientPCLRequestExtByID(long PatientPCLReqExtID)
        {
            try
            {
                DbConnection connection;
                connection = PatientProvider.Instance.CreateConnection();
                using (connection)
                {
                    AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                    PatientPCLRequest_Ext pa = new PatientPCLRequest_Ext();
                    pa = PatientProvider.Instance.PatientPCLRequestExtByID(PatientPCLReqExtID);
                    AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                    return pa;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<PatientPCLRequest_Ext> PatientPCLRequestExtPaging(PatientPCLRequestExtSearchCriteria SearchCriteria,
                        int PageIndex, int PageSize, string OrderBy, bool CountTotal, out int Total)
        {
            try
            {
                DbConnection connection;
                connection = PatientProvider.Instance.CreateConnection();
                using (connection)
                {
                    AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                    List<PatientPCLRequest_Ext> lst = new List<PatientPCLRequest_Ext>();
                    lst = PatientProvider.Instance.PatientPCLRequestExtPaging(SearchCriteria, PageIndex, PageSize, OrderBy, CountTotal, out Total);
                    AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                    return lst;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public List<PatientPCLRequestDetail_Ext> GetPCLRequestDetailListExtByRegistrationID(long RegistrationID)
        {
            try
            {
                DbConnection connection;
                connection = PatientProvider.Instance.CreateConnection();
                using (connection)
                {
                    AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                    List<PatientPCLRequestDetail_Ext> lst = new List<PatientPCLRequestDetail_Ext>();
                    lst = PatientProvider.Instance.GetPCLRequestDetailListExtByRegistrationID(RegistrationID);
                    AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                    return lst;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddNewPCLRequestDetailsExt(PatientPCLRequest_Ext request)
        {
            try
            {
                DbConnection connection;
                connection = PatientProvider.Instance.CreateConnection();
                using (connection)
                {
                    AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                    bool b;
                    b = PatientProvider.Instance.AddNewPCLRequestDetailsExt(request);
                    AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                    return b;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool DeletePCLRequestExt(long PatientPCLReqExtID)
        {
            try
            {
                DbConnection connection;
                connection = PatientProvider.Instance.CreateConnection();
                using (connection)
                {
                    AxLogger.Instance.LogInfo("Start searching patient registration...", CurrentUser);
                    bool b;
                    b = PatientProvider.Instance.DeletePCLRequestExt(PatientPCLReqExtID);
                    AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                    return b;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        #endregion

        #region Txd_New The following methods were moved from the Good Old Common Service

        public bool UpdatePatientRegistrationPayStatus(long PtRegistrationID, long V_RegistrationPaymentStatus)
        {
            try
            {
                return CommonProvider.PatientRg.UpdatePatientRegistrationPayStatus(PtRegistrationID, V_RegistrationPaymentStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdatePatientRegistrationPayStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdatePatientRegistrationPayStatus);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool UpdatePatientRegistration(long PtRegistrationID, long V_RegistrationStatus)
        {
            try
            {
                return CommonProvider.PatientRg.UpdatePatientRegistration(PtRegistrationID, V_RegistrationStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdatePatientRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdatePatientRegistration);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }


        public bool PatientRegistration_Close(long PtRegistrationID, int FindPatient, long V_RegistrationClosingStatus)
        {
            try
            {
                return CommonProvider.PatientRg.PatientRegistration_Close(PtRegistrationID, FindPatient, V_RegistrationClosingStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving PatientRegistration_Close. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PatientRegistration_Close);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public IList<PatientRegistration> SearchPtRegistration(SeachPtRegistrationCriteria criteria, int pageSize, int pageIndex, bool bcount, out int Totalcount)
        {
            try
            {
                return CommonProvider.PatientRg.SearchPtRegistration(criteria, pageSize, pageIndex, bcount, out Totalcount);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving SearchPtRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_SearchPtRegistration);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        // TxD 28/06/2017 Don't know why there are 2 CloseRegistration in both PatientRegistrationService and CommonService
        //                To be reviewed then remove and leave the appropriate one
        public AllLookupValues.RegistrationPaymentStatus CloseRegistration(long PtRegistrationID, int FindPatient)
        {
            AllLookupValues.RegistrationPaymentStatus PaymentStatus = AllLookupValues.RegistrationPaymentStatus.DEBIT;
            try
            {
                AxLogger.Instance.LogInfo("Start to close this Registration...");
                PatientRegistration registrationInfo = eHCMSBillPaymt.RegAndPaymentProcessorBase.GetRegistrationTxd(PtRegistrationID, FindPatient);
                decimal sumTran = 0;
                decimal sumHI = 0;
                decimal sumPayment = 0;
                if (registrationInfo.PatientTransaction != null)
                {
                    List<PatientTransactionDetail> allTransactionDetails = new List<PatientTransactionDetail>();
                    allTransactionDetails = PatientProvider.Instance.GetAlltransactionDetails(registrationInfo.PatientTransaction.TransactionID);
                    foreach (var patientTransactionDetail in allTransactionDetails)
                    {
                        sumTran += patientTransactionDetail.Amount;
                        sumHI += (Decimal)patientTransactionDetail.HealthInsuranceRebate;
                    }
                }
                if (registrationInfo.PatientTransaction.PatientTransactionPayments != null)
                {
                    foreach (var item in registrationInfo.PatientTransaction.PatientTransactionPayments)
                    {
                        sumPayment += item.PayAmount * item.CreditOrDebit;
                    }
                }

                if (sumPayment + sumHI > sumTran)
                {
                    PaymentStatus = AllLookupValues.RegistrationPaymentStatus.DEBIT;
                }
                else
                {
                    if (sumPayment + sumHI < sumTran)
                    {
                        PaymentStatus = AllLookupValues.RegistrationPaymentStatus.CREDIT;
                    }
                }
                CommonProvider.PatientRg.UpdatePatientRegistration(PtRegistrationID, (long)AllLookupValues.RegistrationStatus.COMPLETED);
                CommonProvider.PatientRg.UpdatePatientRegistrationPayStatus(PtRegistrationID, (long)PaymentStatus);
                //return false;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError(ex);
            }
            return PaymentStatus;
        }
        // CMN: Checked: CloseRegistration not working anywhere in eHCMSCal then remove one of this
        /*
        public bool CloseRegistration(long registrationID, int FindPatient, out List<string> errorMessages)
        {
            string strLog = "";
            bool retVal = false;
            errorMessages = new List<string>();
            try
            {
                strLog = string.Format("Start closing registration (RegistrationID = {0})", registrationID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);

                //TODO:
                
                //* Kiểm tra đăng ký có hay chưa. 
                //* Kiểm tra có transaction chưa.
                //* Nếu có transaction -> xem lịch sử thanh toán
                //* Tổng số tiền thanh toán có balance không.
                
                using (DbConnection connection = PatientProvider.Instance.CreateConnection())
                {
                    PatientRegistration regInfo = PatientProvider.Instance.GetRegistration(registrationID, FindPatient, connection, null);
                    if (regInfo == null)
                    {
                        errorMessages.Add("Không tìm thấy đăng ký");
                        return retVal;
                    }
                    AllLookupValues.RegistrationClosingStatus newStatus;
                    if (regInfo.PatientTransaction != null)
                    {
                        var allPayments = CommonProvider.Payments.GetAllPayments_New(regInfo.PatientTransaction.TransactionID, connection, null);
                        if (allPayments != null)
                        {
                            regInfo.PatientTransaction.PatientTransactionPayments = allPayments.ToObservableCollection();
                        }

                        //var allPayments = CommonProvider.Payments.GetAllPayments(regInfo.PatientTransaction.TransactionID, connection, null);
                        //if (allPayments != null)
                        //{
                        //    regInfo.PatientTransaction.PatientPayments = allPayments.ToObservableCollection();
                        //}
                        decimal total = PatientProvider.Instance.GetTotalPatientPayForTransaction(regInfo.PatientTransaction.TransactionID, connection, null);
                        //decimal totalPatientPaid = regInfo.PatientTransaction.PatientPayments.Sum(payment => payment.PayAmount * payment.CreditOrDebit);
                        decimal totalPatientPaid = regInfo.PatientTransaction.PatientTransactionPayments.Sum(payment => payment.PayAmount * payment.CreditOrDebit);
                        if (total > totalPatientPaid) //BN tra thieu.
                        {
                            //newStatus = AllLookupValues.RegistrationClosingStatus.NOTBALANCED_DEBIT;
                            errorMessages.Add("Bệnh nhân chưa trả hết tiền cho đăng ký này.");
                            return retVal;
                        }
                        else if (total == totalPatientPaid)
                        {
                            newStatus = AllLookupValues.RegistrationClosingStatus.BALANCED;
                        }
                        else
                        {
                            newStatus = AllLookupValues.RegistrationClosingStatus.NOTBALANCED_CREDIT;
                        }
                    }
                    else
                    {
                        newStatus = AllLookupValues.RegistrationClosingStatus.BALANCED;
                    }
                    retVal = PatientProvider.Instance.CloseRegistration(registrationID, newStatus, connection, null);
                }

                strLog = string.Format("End of closing registration (RegistrationID = {0}). Status: Success", registrationID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);
                if (!retVal)
                {
                    errorMessages = null;
                }
                return retVal;
            }
            catch (Exception ex)
            {
                strLog = string.Format("End of closing registration (RegistrationID = {0}). Status: Failed", registrationID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_LoadInPatientRegItemsIntoBill);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        */
        public bool CloseRegistrationAll(long PtRegistrationID, int FindPatient, out string Error)
        {
            bool Res = false;
            Error = "";
            try
            {
                DbConnection connection;
                connection = PatientProvider.Instance.CreateConnection();
                using (connection)
                {
                    PatientRegistration regInfo = GetRegistrationInfo(PtRegistrationID, FindPatient, false);

                    if (regInfo == null || regInfo.PtRegistrationID <= 0)
                    {
                        Error = "Không tìm thấy đăng ký!";
                        return false;
                    }

                    if (FindPatient == (int)AllLookupValues.V_FindPatientType.NOI_TRU)
                    {
                        if (regInfo.AdmissionInfo.DischargeDate != null)
                        {
                            Res = PatientRegistration_Close(PtRegistrationID, FindPatient, (long)AllLookupValues.RegistrationClosingStatus.BALANCED);
                        }
                        else
                        {
                            Error = "Bệnh Nhân này chưa hoàn tất thủ tục Xuất Viện! Không thể đóng Đăng Ký!";
                        }
                    }
                    if (FindPatient == (int)AllLookupValues.V_FindPatientType.NGOAI_TRU)
                    {
                        AllLookupValues.RegistrationClosingStatus newStatus = new AllLookupValues.RegistrationClosingStatus();

                        int flag = 0;

                        if (regInfo.PayableSum.TotalHIPayment + regInfo.PayableSum.TotalPatientPaid == regInfo.PayableSum.TotalPrice)
                        {
                            newStatus = AllLookupValues.RegistrationClosingStatus.BALANCED;
                            flag = 0;
                        }
                        if (regInfo.PayableSum.TotalHIPayment + regInfo.PayableSum.TotalPatientPaid < regInfo.PayableSum.TotalPrice)
                        {
                            newStatus = AllLookupValues.RegistrationClosingStatus.NOTBALANCED_DEBIT;
                            flag = 1;
                        }
                        if (regInfo.PayableSum.TotalHIPayment + regInfo.PayableSum.TotalPatientPaid > regInfo.PayableSum.TotalPrice)
                        {
                            newStatus = AllLookupValues.RegistrationClosingStatus.NOTBALANCED_CREDIT;
                            flag = 2;
                        }

                        if (flag == 0)
                        {
                            Res = PatientRegistration_Close(PtRegistrationID, FindPatient, (long)newStatus);
                        }
                        else
                        {
                            if (flag == 1)
                            {
                                Error = "Bệnh Nhân chưa thanh toán hết nợ! Không thể đóng Đăng Ký!";
                            }
                            if (flag == 2)
                            {
                                Error = "Bệnh Viện còn nợ Bệnh Nhân tiền! Cần hoàn tiền lại cho Bệnh Nhân!";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string strLog = string.Format("End of closing registration (PtRegistrationID = {0}). Status: Failed", PtRegistrationID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PatientRegistration_Close);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

            return Res;
        }

        public bool CancelRegistration(PatientRegistration registrationInfo, out PatientRegistration cancelledRegistration)
        {
            try
            {
                cancelledRegistration = null;
                AxLogger.Instance.LogInfo("Start removing registration.", CurrentUser);
                bool bOK = false;
                eHCMSBillPaymt.RegAndPaymentProcessorBase paymentProcessor = eHCMSBillPaymt.RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                //HPT 25/08/2015: Trước đây chỉ có thể hủy đăng ký ngoại trú nên không phân biệt
                //Bổ sung điều kiện loại đăng ký, tùy theo là đăng ký ngoại trú hay nội trú mà dùng hàm GetRegistrationInfo hay GetRegistrationInfo_InPt
                //Thêm câu lệnh if ở đây để khỏi định nghĩa thêm một hàm CancelRegistration_InPt trong service. 
                if (registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    bOK = paymentProcessor.CancelRegistration(registrationInfo);
                    if (bOK)
                    {
                        cancelledRegistration = GetRegistrationInfo(registrationInfo.PtRegistrationID, registrationInfo.FindPatient);
                    }
                }
                if (registrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    bOK = paymentProcessor.CancelRegistration_InPt(registrationInfo);
                    if (bOK)
                    {
                        //HPT 26/08/2015: đối tượng loadregtask xác định sẽ load lên những phần nào của đăng ký để tránh load dư thừa dữ liệu
                        LoadRegistrationSwitch loadregtask = new LoadRegistrationSwitch();
                        loadregtask.IsGetPatient = true;
                        loadregtask.IsGetRegistration = true;
                        cancelledRegistration = GetRegistrationInfo_InPt(registrationInfo.PtRegistrationID, registrationInfo.FindPatient, loadregtask, false);
                    }
                }
                AxLogger.Instance.LogInfo("End of removing registered items.", CurrentUser);
                return bOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of removing registration. Status: Failed.", CurrentUser);
                throw new Exception(ex.Message);

                //AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CancelRegistration, CurrentUser);
                //throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrationID"></param>
        /// <param name="loadFromAppointment">Neu bien nay set bang true thi se vao bang cuoc hen, lay danh sach cac dich vu da duoc
        /// hen roi add vao dang ky luon. Chi dung khi load dang ky trong, load san danh sach hen de nguoi dung luu dang ky cho tien.</param>
        /// <returns></returns>
        public PatientRegistration GetRegistrationInfo(long registrationID, int FindPatient, bool loadFromAppointment = false, bool IsProcess = false)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Registration Info.", CurrentUser);

                var regInfo = eHCMSBillPaymt.RegAndPaymentProcessorBase.GetRegistrationTxd(registrationID, FindPatient, false, IsProcess);
                if (loadFromAppointment)
                {
                    if (regInfo != null
                        && regInfo.AppointmentID.HasValue && regInfo.AppointmentID.Value > 0
                        && regInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU
                        && regInfo.V_RegistrationStatus == (long)AllLookupValues.RegistrationStatus.PENDING
                        && (regInfo.PatientRegistrationDetails == null || regInfo.PatientRegistrationDetails.Count == 0)
                        && (regInfo.PCLRequests == null || regInfo.PCLRequests.Count == 0))
                    {
                        PatientAppointment appointment = new PatientAppointment();
                        appointment = AppointmentProvider.Instance.GetAppointmentByID(regInfo.AppointmentID.Value);
                        var createdDate = DateTime.Now.Date;
                        if (appointment != null)
                        {
                            regInfo.PatientRegistrationDetails = new List<PatientRegistrationDetail>().ToObservableCollection();
                            foreach (var item in appointment.PatientApptServiceDetailList)
                            {
                                var regDetails = new PatientRegistrationDetail { RefMedicalServiceItem = item.MedService };
                                regDetails.RefMedicalServiceItem.RefMedicalServiceType = new RefMedicalServiceType() { MedicalServiceTypeID = item.MedService.MedicalServiceTypeID.GetValueOrDefault(-1) };
                                regDetails.DeptLocation = item.DeptLocation;
                                regDetails.CreatedDate = createdDate;
                                regDetails.Qty = 1;
                                regDetails.RecordState = RecordState.ADDED;
                                regDetails.CanDelete = true;
                                regDetails.FromAppointment = true;
                                regDetails.StaffID = appointment.DoctorStaffID;
                                regDetails.AppointmentDate = appointment.ApptDate;
                                regInfo.PatientRegistrationDetails.Add(regDetails);
                            }

                            if (appointment.PatientApptPCLRequestsList != null)
                            {
                                regInfo.PCLRequests = new List<PatientPCLRequest>().ToObservableCollection();
                                foreach (var request in appointment.PatientApptPCLRequestsList)
                                {
                                    var newReq = new PatientPCLRequest();
                                    newReq.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
                                    newReq.CreatedDate = createdDate;
                                    newReq.RecordState = RecordState.ADDED;
                                    newReq.ReqFromDeptLocID = 0;
                                    newReq.DoctorStaffID = appointment.DoctorStaffID;
                                    newReq.CanDelete = true;
                                    newReq.V_PCLMainCategory = request.V_PCLMainCategory;
                                    newReq.Diagnosis = request.Diagnosis;
                                    newReq.ICD10List = request.ICD10List;
                                    newReq.DoctorComments = request.DoctorComments;
                                    regInfo.PCLRequests.Add(newReq);

                                    if (request.ObjPatientApptPCLRequestDetailsList != null)
                                    {
                                        newReq.PatientPCLRequestIndicators = new List<PatientPCLRequestDetail>().ToObservableCollection();
                                        foreach (var reqDetails in request.ObjPatientApptPCLRequestDetailsList)
                                        {
                                            PCLExamType examType = new PCLExamType();
                                            examType = PatientProvider.Instance.GetPclExamTypeByID(reqDetails.PCLExamTypeID);
                                            if (examType != null)
                                            {
                                                var newReqDetail = CreateRequestDetailsForPCLExamType(examType);
                                                newReqDetail.RecordState = RecordState.ADDED;
                                                newReqDetail.CreatedDate = createdDate;
                                                newReqDetail.CanDelete = true;
                                                newReqDetail.FromAppointment = true;
                                                newReqDetail.StaffID = appointment.DoctorStaffID;
                                                newReqDetail.AppointmentDate = appointment.ApptDate;
                                                newReqDetail.DeptLocation = reqDetails.ObjDeptLocID;
                                                newReq.PatientPCLRequestIndicators.Add(newReqDetail);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return regInfo;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Registration Info. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public PatientRegistration GetRegistrationInfo_InPt(long registrationID, int FindPatient, LoadRegistrationSwitch loadRegisSwitch, bool loadFromAppointment = false)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Registration Info.", CurrentUser);

                var regInfo = eHCMSBillPaymt.RegAndPaymentProcessorBase.GetRegistration_InPt(registrationID, FindPatient, loadRegisSwitch);

                // TxD 06/12/2014: InPatient DO NOT Load from Appointment so a block of code that was a copy of OutPatient has been removed from here
                //
                return regInfo;
            }

            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Registration Info. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistrationInfo, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public Patient GetPatientInfoByPtRegistration(long? PtRegistrationID, long? PatientID, int FindPatient)
        {
            try
            {
                return CommonProvider.PatientRg.GetPatientInfoByPtRegistration(PtRegistrationID, PatientID, FindPatient);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving GetPatientInfoByPtRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_PATIENTINFO_CANNOT_GET);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        /// <summary>
        /// Hàm này hiện tại thấy chỉ xài cho bên nhận bệnh bảo hiểm thôi
        /// </summary>
        /// <param name="StaffID"></param>
        /// <param name="Apply15HIPercent"></param>
        /// <param name="registrationInfo"></param>
        /// <param name="SavedRegistration"></param>
        public void SaveEmptyRegistration(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration registrationInfo, long V_RegistrationType, out PatientRegistration SavedRegistration)
        {
            SaveEmptyRegistration_V2(StaffID, CollectorDeptLocID, Apply15HIPercent, registrationInfo, V_RegistrationType, out SavedRegistration, null);
        }

        public void SaveEmptyRegistration_V2(long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration registrationInfo, long V_RegistrationType, out PatientRegistration SavedRegistration, IList<DiagnosisIcd10Items> Icd10Items, bool IsHospitalizationProposal = false)
        {
            long PatientRegistrationID;
            SavedRegistration = null;

            try
            {
                if (registrationInfo == null)
                {
                    throw new Exception(eHCMSResources.K0300_G1_ChonDK);
                }

                if (registrationInfo.ExamDate == DateTime.MinValue)
                {
                    registrationInfo.ExamDate = DateTime.Now;
                }

                //AxLogger.Instance.LogInfo("Start creating new registration.", CurrentUser);
                //RegAndPaymentProcessorBase paymentProcesssor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);
                eHCMSBillPaymt.RegAndPaymentProcessorBase paymentProcesssor = eHCMSBillPaymt.RegAndPaymentProcessorFactory.GetPaymentProcessor(PaymentProcessorType.HealthInsurance);
                paymentProcesssor.InitNewTxd(registrationInfo, true, V_RegistrationType);

                List<long> newRegDetailsList;
                List<long> newPclRequestList;

                var registrationDetails = new List<PatientRegistrationDetail>();
                var pclRequests = new List<PatientPCLRequest>();

                if (registrationInfo.AppointmentID.HasValue && registrationInfo.AppointmentID.Value > 0
                    //▼===== #016
                    && !IsHospitalizationProposal)
                //▲===== #016
                {
                    PatientAppointment appointment = new PatientAppointment();
                    appointment = AppointmentProvider.Instance.GetAppointmentByID(registrationInfo.AppointmentID.Value);
                    //▼===== 20200721 TTM: Lấy thời gian tạo dịch vụ, CLS thì lấy luôn cả ngày và giờ. 
                    //var createdDate = DateTime.Now.Date;
                    var createdDate = DateTime.Now;
                    //▲=====
                    if (appointment != null)
                    {
                        foreach (var item in appointment.PatientApptServiceDetailList)
                        {
                            var regDetails = new PatientRegistrationDetail();
                            regDetails.RefMedicalServiceItem = item.MedService;
                            regDetails.RefMedicalServiceItem.RefMedicalServiceType = new RefMedicalServiceType() { MedicalServiceTypeID = item.MedService.MedicalServiceTypeID.GetValueOrDefault(-1) };
                            regDetails.DeptLocation = item.DeptLocation;
                            regDetails.CreatedDate = createdDate;
                            regDetails.Qty = 1;
                            regDetails.RecordState = RecordState.ADDED;
                            regDetails.CanDelete = true;
                            regDetails.FromAppointment = true;
                            regDetails.StaffID = appointment.DoctorStaffID;
                            regDetails.AppointmentDate = appointment.ApptDate;
                            regDetails.ServiceSeqNum = item.ServiceSeqNum;
                            registrationDetails.Add(regDetails);
                        }

                        if (appointment.PatientApptPCLRequestsList != null)
                        {
                            foreach (var request in appointment.PatientApptPCLRequestsList)
                            {
                                var newReq = new PatientPCLRequest();
                                newReq.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;
                                newReq.ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
                                newReq.CreatedDate = createdDate;
                                newReq.RecordState = RecordState.ADDED;
                                newReq.DoctorStaffID = appointment.DoctorStaffID;
                                newReq.CanDelete = true;
                                newReq.V_PCLMainCategory = request.V_PCLMainCategory;
                                newReq.Diagnosis = request.Diagnosis;
                                newReq.ICD10List = request.ICD10List;
                                newReq.DoctorComments = request.DoctorComments;


                                newReq.ObjV_PCLMainCategory = new Lookup { LookupID = 28200, ObjectValue = "Imaging" };
                                pclRequests.Add(newReq);

                                if (request.ObjPatientApptPCLRequestDetailsList != null)
                                {
                                    newReq.PatientPCLRequestIndicators = new List<PatientPCLRequestDetail>().ToObservableCollection();
                                    foreach (var reqDetails in request.ObjPatientApptPCLRequestDetailsList)
                                    {
                                        PCLExamType examType = new PCLExamType();
                                        examType = PatientProvider.Instance.GetPclExamTypeByID(reqDetails.PCLExamTypeID);
                                        if (examType != null)
                                        {
                                            var newReqDetail = CreateRequestDetailsForPCLExamType(examType);
                                            newReqDetail.RecordState = RecordState.ADDED;
                                            newReqDetail.CreatedDate = createdDate;
                                            newReqDetail.CanDelete = true;
                                            newReqDetail.FromAppointment = true;
                                            newReqDetail.StaffID = appointment.DoctorStaffID;
                                            newReqDetail.AppointmentDate = appointment.ApptDate;
                                            newReqDetail.ServiceSeqNum = reqDetails.ServiceSeqNum;
                                            newReqDetail.PatientPCLRequest = newReq;
                                            newReq.PatientPCLRequestIndicators.Add(newReqDetail);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

                paymentProcesssor.AddServicesAndPCLRequests(StaffID, CollectorDeptLocID, Apply15HIPercent, registrationInfo, registrationDetails, pclRequests, null, null, registrationInfo.ExamDate, false, false, out PatientRegistrationID, out newRegDetailsList, out newPclRequestList, Icd10Items);
                SavedRegistration = registrationInfo;

                SavedRegistration.PtRegistrationID = PatientRegistrationID;
                //KMx: Anh Tuấn không cho insert vào bảng PatientQueue
                //try
                //{
                //    var queue = new PatientQueue();
                //    queue.RegistrationID = PatientRegistrationID;//registrationInfo.PtRegistrationID;
                //    queue.V_QueueType = (long)AllLookupValues.QueueType.CHO_DANG_KY;
                //    queue.V_PatientQueueItemsStatus = (long)AllLookupValues.PatientQueueItemsStatus.WAITING;
                //    queue.FullName = registrationInfo.Patient.FullName;
                //    queue.PatientID = registrationInfo.Patient.PatientID;
                //    //PatientProvider.Instance.PatientQueue_Insert(queue);
                //}
                //catch
                //{
                //    throw new Exception("Đã tạo đăng ký thành công nhưng chưa thể thêm vào queue.");
                //}
                //Update lai Paperreferal ID
                if (registrationInfo.PaperReferal != null
                    && registrationInfo.PaperReferal.RefID > 0
                    && (registrationInfo.PaperReferal.PtRegistrationID == null
                        || registrationInfo.PaperReferal.PtRegistrationID < 1))
                {
                    registrationInfo.PaperReferal.PtRegistrationID = PatientRegistrationID;
                    PatientProvider.Instance.UpdatePaperReferalRegID(registrationInfo.PaperReferal);
                }
                //AxLogger.Instance.LogInfo("End of creating new registration.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveEmptyRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_SaveEmptyRegistration, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }

        public void UpdateDateStarted_ForPatientRegistrationDetails(long PtRegDetailID,out DateTime? DateStart)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start UpdateDateStarted_ForPatientRegistrationDetails.", CurrentUser);
                PatientProvider.Instance.UpdateDateStarted_ForPatientRegistrationDetails(PtRegDetailID,out DateStart);
                AxLogger.Instance.LogInfo("End of UpdateDateStarted_ForPatientRegistrationDetails.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdateDateStarted_ForPatientRegistrationDetails. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void AddInPatientAdmission(PatientRegistration registrationInfo, InPatientDeptDetail deptDetail, long StaffID, long Staff_DeptLocationID, out long RegistrationID)
        {
            try
            {
                if (registrationInfo == null || registrationInfo.PtRegistrationID <= 0)
                {
                    throw new Exception(eHCMSResources.K0300_G1_ChonDK);
                }
                if (registrationInfo.AdmissionInfo == null)
                {
                    throw new Exception(eHCMSResources.Z1686_G1_ChuaCoTTinNV);
                }
                if (registrationInfo.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.PENDING_INPT)
                {
                    throw new Exception(eHCMSResources.Z1687_G1_NVChoDKOTrangThaiCho);
                }
                registrationInfo.V_RegistrationStatus = (long)AllLookupValues.RegistrationStatus.OPENED;

                AxLogger.Instance.LogInfo("Start creating adding In-Patient admission.", CurrentUser);
                eHCMSBillPaymt.RegAndPaymentProcessorBase paymentProcesssor = eHCMSBillPaymt.RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);

                long newRegistrationId;
                long newAdmissionId;
                paymentProcesssor.AddInPatientAdmission(registrationInfo, deptDetail, StaffID, Staff_DeptLocationID, out newRegistrationId, out newAdmissionId);

                RegistrationID = registrationInfo.PtRegistrationID;

                AxLogger.Instance.LogInfo("End of adding In-Patient admission.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of adding In-Patient admission. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddInPatientAdmission, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }

        public void UpdateInPatientAdmDisDetails(InPatientAdmDisDetails entity, DeptLocation newDeptLoc)
        {

            try
            {
                AxLogger.Instance.LogInfo("Start updating In-Patient admission.", CurrentUser);
                bool result;
                result = PatientProvider.Instance.UpdateInPatientAdmDisDetails(entity, newDeptLoc);
                AxLogger.Instance.LogInfo("End of updating In-Patient admission.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of update In-Patient admission. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_UpdateInPatientAdmDisDetails, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public InPatientAdmDisDetails GetInPatientAdmissionByRegistrationID(long regID)
        {
            if (regID <= 0)
            {
                return null;
            }
            DbConnection connection;
            connection = PatientProvider.Instance.CreateConnection();
            using (connection)
            {
                return PatientProvider.Instance.GetInPatientAdmissionByRegistrationID(regID, connection, null);
            }
        }

        public PatientRegistration GetRegistration(long registrationID, int FindPatient)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Registration Info.", CurrentUser);
                return PatientProvider.Instance.GetRegistration(registrationID, FindPatient);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetRegistration. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistration, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PatientRegistration GetRegistrationSimple(long registrationID, int PatientFindBy)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Registration Simple.", CurrentUser);
                return PatientProvider.Instance.GetRegistrationSimple(registrationID, PatientFindBy);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading Registration Simple. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_RegistrationSimple, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public PatientRegistration ChangeRegistrationType(PatientRegistration regInfo, AllLookupValues.RegistrationType newType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start changing registration Type.", CurrentUser);

                bool retVal;
                retVal = PatientProvider.Instance.ChangeRegistrationType(regInfo, newType);
                if (retVal == false)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.T0432_G1_Error));
                }
                regInfo.V_RegistrationType = newType;
                AxLogger.Instance.LogInfo("End of changing registration Type. Status: Success.", CurrentUser);
                return regInfo;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of changing registration Type. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_ChangeRegistrationType, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool Hospitalize(PatientRegistration regInfo, BedPatientAllocs selectedBed, DateTime? admissionDate, int ExpectedStayingDays, out AllLookupValues.RegistrationType NewRegType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start changing hospitalization.", CurrentUser);
                if (regInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU
                    //|| regInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU
                    )
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1696_G1_BNDaNV));
                }
                if (selectedBed == null)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1697_G1_ChuaDatGiuong));
                }
                if (!admissionDate.HasValue)
                {
                    admissionDate = DateTime.Now;
                }
                //Dat giuong cho benh nhan truoc. Neu OK moi chuyen sang dang ky noi tru.
                bool bookOK;
                bookOK = BedAllocations.Instance.AddNewBedPatientAllocs(selectedBed.BedAllocationID, regInfo.PtRegistrationID
                                              , admissionDate
                                              , ExpectedStayingDays
                                              , null
                                              , true);
                if (bookOK)
                {
                    AllLookupValues.RegistrationType newType = AllLookupValues.RegistrationType.Unknown;
                    if (regInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        //newType = AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU;
                    }
                    else if (regInfo.V_RegistrationType == AllLookupValues.RegistrationType.Unknown)
                    {
                        newType = AllLookupValues.RegistrationType.NOI_TRU;
                    }
                    bool retVal;
                    retVal = PatientProvider.Instance.ChangeRegistrationType(regInfo, newType);
                    if (!retVal)
                    {
                        BedAllocations.Instance.DeleteBedAllocation(selectedBed.BedAllocationID);
                        throw new Exception(string.Format("{0}.", eHCMSResources.Z1698_G1_KgTheNVChoBN));
                    }
                    NewRegType = newType;

                    AxLogger.Instance.LogInfo("End of changing hospitalization. Status: Success.", CurrentUser);
                    return true;
                }
                else
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1699_G1_KgTheDatGiuongChoBN));
                }
            }

            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of changing hospitalization. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_Hospitalize, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public void CreateNewRegistrationVIP(PatientRegistration regInfo, out long newRegistrationID)
        {
            try
            {
                DbConnection conn;
                conn = PatientProvider.Instance.CreateConnection();
                using (conn)
                {
                    conn.Open();

                    int sequenceNo;
                    regInfo.ExamDate = DateTime.Now;
                    //regInfo.V_RegistrationType = AllLookupValues.RegistrationType.DANGKY_VIP;
                    PatientProvider.Instance.AddRegistration(regInfo, conn, null, out newRegistrationID, out sequenceNo);
                    regInfo.PtRegistrationID = newRegistrationID;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CreateNewRegistrationVIP. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_VIP, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool AddInPatientDischarge(InPatientAdmDisDetails admissionInfo, long StaffID, long? ConfirmedDTItemID, out List<string> errorMessages)
        {
            string strLog = "";
            bool retVal = false;
            errorMessages = new List<string>();
            try
            {
                strLog = string.Format("Start adding Patient discharge (AdmissionID = {0})", admissionInfo.InPatientAdmDisDetailID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);

                //TODO:
                /*
                 * Kiểm tra đăng ký có hay chưa. 
                 * Kiểm tra có transaction chưa.
                 * Nếu có transaction -> xem lịch sử thanh toán
                 * Tổng số tiền thanh toán có balance không.
                 */
                DbConnection connection;
                connection = PatientProvider.Instance.CreateConnection();
                using (connection)
                {
                    PatientRegistration regInfo = new PatientRegistration();
                    regInfo = PatientProvider.Instance.GetRegistration(admissionInfo.PatientRegistration != null && admissionInfo.PatientRegistration.PtRegistrationID > 0 ? admissionInfo.PatientRegistration.PtRegistrationID : admissionInfo.PtRegistrationID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, connection, null);
                    if (regInfo == null)
                    {
                        errorMessages.Add("Không tìm thấy đăng ký");
                        return retVal;
                    }
                    if (regInfo.PatientTransaction != null)
                    {
                        List<PatientTransactionPayment> allPayments = new List<PatientTransactionPayment>();
                        allPayments = CommonProvider.Payments.GetAllPayments_InPt(regInfo.PatientTransaction.TransactionID, connection, null);
                        if (allPayments != null)
                        {
                            regInfo.PatientTransaction.PatientTransactionPayments = allPayments.ToObservableCollection();
                        }

                        decimal total;
                        total = PatientProvider.Instance.GetTotalPatientPayForTransaction_InPt(regInfo.PatientTransaction.TransactionID, connection, null);
                        decimal totalPatientPaid = regInfo.PatientTransaction.PatientTransactionPayments.Sum(payment => payment.PayAmount * payment.CreditOrDebit);
                        if (total > totalPatientPaid) //BN tra thieu.
                        {
                            //newStatus = AllLookupValues.RegistrationClosingStatus.NOTBALANCED_DEBIT;
                            errorMessages.Add("Bệnh nhân chưa trả hết tiền cho đăng ký này.");
                            return retVal;
                        }
                    }

                    //Them thong tin chuyen vien.
                    //Sau do dong dang ky luon.
                    //Dong khong duoc thi thoi. Khong can thiet.
                    bool bOk = false;
                    if (admissionInfo.DeceasedInfo != null)
                    {
                        if (admissionInfo.DeceasedInfo.DSNumber <= 0)
                        {
                            admissionInfo.DeceasedInfo.PtRegistrationID = admissionInfo.PtRegistrationID;
                            long deceasedInfoId;
                            bOk = PatientProvider.Instance.AddDeceaseInfo(admissionInfo.DeceasedInfo, connection, null, out deceasedInfoId);
                            if (bOk)
                            {
                                admissionInfo.DeceasedInfo.DSNumber = deceasedInfoId;
                                bOk = PatientProvider.Instance.UpdateInPatientDischargeInfo(admissionInfo, StaffID, ConfirmedDTItemID, connection, null);
                            }
                            if (!bOk)
                            {
                                errorMessages.Add("Không thể thêm dữ liệu xuất viện.");
                            }
                        }
                        else
                        {
                            if (admissionInfo.DeceasedInfo.EntityState == EntityState.MODIFIED)
                            {
                                PatientProvider.Instance.UpdateDeceaseInfo(admissionInfo.DeceasedInfo, connection, null);
                            }
                            else if (admissionInfo.DeceasedInfo.EntityState == EntityState.DELETED_MODIFIED)
                            {
                                PatientProvider.Instance.DeleteDeceaseInfo(admissionInfo.DeceasedInfo.DSNumber, connection, null);
                                admissionInfo.DeceasedInfo = null;
                                admissionInfo.DSNumber = null;
                            }
                            bOk = PatientProvider.Instance.UpdateInPatientDischargeInfo(admissionInfo, StaffID, ConfirmedDTItemID, connection, null);
                        }
                    }
                    else
                    {
                        bOk = PatientProvider.Instance.UpdateInPatientDischargeInfo(admissionInfo, StaffID, ConfirmedDTItemID, connection, null);
                    }
                    retVal = bOk;

                }

                strLog = string.Format("End of adding Patient discharge (AdmissionID = {0})", admissionInfo.InPatientAdmDisDetailID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);
                if (!retVal)
                {
                    errorMessages = null;
                }
                return retVal;
            }
            catch (Exception ex)
            {
                strLog = string.Format("End of adding Patient discharge (AdmissionID = {0})", admissionInfo.InPatientAdmDisDetailID);
                AxLogger.Instance.LogInfo(strLog, CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddInPatientDischarge);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool RevertDischarge(InPatientAdmDisDetails admissionInfo, long staffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Revert Discharge.", CurrentUser);
                return PatientProvider.Instance.RevertDischarge(admissionInfo, staffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End Revert Discharge. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_RevertDischarge, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool CheckBeforeDischarge(long registrationID, long DischargeDeptID, bool ConfirmNotTreatedAsInPt
            , DateTime? DischargeDate
            //▼==== #031
            , bool IsSendingCheckMedicalFiles
            //▲==== #031
            , out string errorMessages, out string confirmMessages)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Check Before Discharge.", CurrentUser);
                return PatientProvider.Instance.CheckBeforeDischarge(registrationID, DischargeDeptID, ConfirmNotTreatedAsInPt, DischargeDate
                //▼==== #031
                , IsSendingCheckMedicalFiles
                //▲==== #031
                , out errorMessages, out confirmMessages);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End Check Before Discharge. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CheckBeforeDischarge, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }

        public bool ChangeInPatientDept(InPatientDeptDetail entity, out long inPatientDeptDetailID)
        {
            inPatientDeptDetailID = 0;
            try
            {
                AxLogger.Instance.LogInfo("Start ChangeInPatientDept.", CurrentUser);
                if (entity.FromDate == DateTime.MinValue)
                {
                    entity.FromDate = DateTime.Now;
                }
                bool bOK;
                bOK = PatientProvider.Instance.ChangeInPatientDept(entity, out inPatientDeptDetailID);
                AxLogger.Instance.LogInfo("End of ChangeInPatientDept. Status: Success", CurrentUser);
                return bOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ChangeInPatientDept. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_ChangeInPatientDept, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool Registrations_UpdateStatus(PatientRegistration regInfo, long V_RegistrationStatus)
        {
            try
            {
                return PatientProvider.Instance.Registrations_UpdateStatus(regInfo, V_RegistrationStatus);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Registrations_UpdateStatus. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_Registrations_UpdateStatus);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▼====== #006
        public IList<PatientRegistrationDetail> SearchInPatientRegistrationListForOST(long DeptID, bool IsSearchForListPatientCashAdvance)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient registration for OutStandingTask...", CurrentUser);
                IList<PatientRegistrationDetail> retVal;
                retVal = PatientProvider.Instance.SearchInPatientRegistrationListForOST(DeptID, IsSearchForListPatientCashAdvance);
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration for OutStandingTask. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====== #006
        //▼===== #015
        public IList<PatientRegistrationDetail> SearchInPatientRequestAdmissionListForOST(DateTime? FromDate, DateTime? ToDate)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start searching patient admission for OutStandingTask...", CurrentUser);
                IList<PatientRegistrationDetail> retVal;
                retVal = PatientProvider.Instance.SearchInPatientRequestAdmissionListForOST(FromDate, ToDate);
                AxLogger.Instance.LogInfo("End of searching patient admission. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient admission for OutStandingTask. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲===== #015
        #endregion
        //▼===== #005
        #region Thông tin hành chính, BHYT, CV
        public bool AddPatientAndHIDetails(Patient newPatient, HealthInsurance newHICard, PaperReferal newPaperReferal, out long PatientID, out string PatientCode, out string PatientBarCode, out long HIID, out PaperReferal PaperReferal)
        {
            bool bAddOK = false;
            //AddedPatient = newPatient;
            HIID = -1;
            long paperReferalID;
            PaperReferal = newPaperReferal;
            try
            {
                AxLogger.Instance.LogInfo("Start adding new patient...", CurrentUser);
                {
                    newPatient.PatientCode = new ServiceSequenceNumberProvider().GetPatientCode();
                    bAddOK = PatientProvider.Instance.AddPatientAndHIDetails(newPatient, newHICard, newPaperReferal, out PatientID, out PatientCode, out PatientBarCode, out HIID, out paperReferalID);
                    AxLogger.Instance.LogInfo("End of adding new patient. Status: Success", CurrentUser);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of adding new patient. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            //Tra ve thong tin benh nhan vua moi dc them vao. (Chi lay thong tin phan general info.
            try
            {
                AxLogger.Instance.LogInfo("Start getting patient info", CurrentUser);
                //AddedPatient = PatientProvider.Instance.GetPatientByID(PatientID);
                AxLogger.Instance.LogInfo("End of getting patient info. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of getting patient info. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            return bAddOK;
        }

        public bool AddConfirmPatientAndHIDetails(Patient newPatientAndHiDetails, bool IsEmergency, long V_RegistrationType, bool _isEmergInPtReExamination,
            bool _isHICard_FiveYearsCont, bool _isChildUnder6YearsOld, bool _isAllowCrossRegion, bool IsHICard_FiveYearsCont_NoPaid,
            out long PatientID, out string PatientCode, out string PatientBarCode, out HealthInsurance newHICard, out PaperReferal newPaperReferal)
        {
            bool bAddOK = false;

            long HIID = -1;
            long paperReferalID;
            newHICard = new HealthInsurance();
            newPaperReferal = new PaperReferal();
            try
            {
                AxLogger.Instance.LogInfo("Start adding new patient...", CurrentUser);
                {
                    newPatientAndHiDetails.PatientCode = new ServiceSequenceNumberProvider().GetPatientCode();
                    bAddOK = PatientProvider.Instance.AddPatientAndHIDetails(newPatientAndHiDetails, newPatientAndHiDetails.CurrentHealthInsurance, newPatientAndHiDetails.ActivePaperReferal,
                            out PatientID, out PatientCode, out PatientBarCode, out HIID, out paperReferalID);
                    AxLogger.Instance.LogInfo("End of adding new patient. Status: Success", CurrentUser);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of adding new patient. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

            return bAddOK;
        }

        public bool UpdatePatientAndHIDetails(Patient patient, bool IsAdmin, long StaffID, bool IsEditAfterRegistration, out Patient UpdatedPatient, out string Result)
        {
            bool bUpdateOK = false;
            UpdatedPatient = patient;
            try
            {
                AxLogger.Instance.LogInfo("Start updating patient info.", CurrentUser);
                bUpdateOK = PatientProvider.Instance.UpdatePatientAndHIDetails(patient, IsAdmin, StaffID, IsEditAfterRegistration, out Result);
                AxLogger.Instance.LogInfo("Start updating patient info. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of updating patient info. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

            try
            {
                AxLogger.Instance.LogInfo("Start getting patient info", CurrentUser);
                UpdatedPatient = PatientProvider.Instance.GetPatientByID_Full(patient.PatientID);
                AxLogger.Instance.LogInfo("End of getting patient info. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of getting patient info. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
            return bUpdateOK;
        }

        public double CalculateHiBenefit(HealthInsurance healthInsurance, PaperReferal paperReferal, out bool isConsideredAsCrossRegion, bool IsEmergency = false, long V_RegistrationType = 0, bool IsEmergInPtReExamination = false, bool IsHICard_FiveYearsCont = false, bool IsChildUnder6YearsOld = false, bool IsAllowCrossRegion = false, bool IsHICard_FiveYearsCont_NoPaid = false)
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

        public bool UpdateConfirmPatientAndHIDetails(Patient updatedPatientAndHiDetails, bool IsAdmin, long StaffID, bool IsEditAfterRegistration,
            bool IsEmergency, long V_RegistrationType, bool _isEmergInPtReExamination, bool _isHICard_FiveYearsCont, bool _isChildUnder6YearsOld, bool _isAllowCrossRegion,
            bool IsHICard_FiveYearsCont_NoPaid, out long PatientID, out string PatientCode, out long HIID, out string HIPCode, out int? IBID, out long PaperReferralID,
                                                 out double RebatePercentage, out double CalculatedHiBenefit, out string Result)
        {
            PatientID = 0;
            PatientCode = "";
            HIID = 0;
            HIPCode = "";
            IBID = 0;
            PaperReferralID = 0;
            RebatePercentage = 0;
            CalculatedHiBenefit = 0;
            Result = "";
            bool bUpdateOK = false;

            try
            {
                AxLogger.Instance.LogInfo("Start updating patient info.", CurrentUser);

                if (updatedPatientAndHiDetails.CurrentHealthInsurance != null)
                {
                    bool res = ValidateHiItem(updatedPatientAndHiDetails.CurrentHealthInsurance, out IBID, out HIPCode);
                }
                bUpdateOK = PatientProvider.Instance.UpdateNewPatientAndHIDetails(updatedPatientAndHiDetails, IsAdmin, StaffID, IsEditAfterRegistration,
                                        out PatientID, out HIID, out PaperReferralID, out RebatePercentage, out Result);
                bool isConsideredAsCrossRegion = false;
                if (bUpdateOK)
                {
                    CalculatedHiBenefit = CalculateHiBenefit(updatedPatientAndHiDetails.CurrentHealthInsurance, updatedPatientAndHiDetails.ActivePaperReferal,
                        out isConsideredAsCrossRegion, false, V_RegistrationType, _isEmergInPtReExamination, _isHICard_FiveYearsCont,
                        _isChildUnder6YearsOld, _isAllowCrossRegion, IsHICard_FiveYearsCont_NoPaid);
                    RebatePercentage = CalculatedHiBenefit;
                }

                AxLogger.Instance.LogInfo("Start updating patient info. Status: Success.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of updating patient info. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }


            return bUpdateOK;
        }

        public bool AddConfirmNewPatientAndHIDetails(Patient newPatientAndHiDetails, bool IsEmergency, long V_RegistrationType, bool _isEmergInPtReExamination,
            bool _isHICard_FiveYearsCont, bool _isChildUnder6YearsOld, bool _isAllowCrossRegion, bool IsHICard_FiveYearsCont_NoPaid,
            out long PatientID, out string PatientCode, out long HIID, out string HIPCode, out int? IBID, out long PaperReferralID,
                                                 out double RebatePercentage, out double CalculatedHiBenefit)
        {
            PatientID = 0;
            PatientCode = "";
            HIID = 0;
            HIPCode = "";
            IBID = 0;
            PaperReferralID = 0;
            RebatePercentage = 0;
            CalculatedHiBenefit = 0;
            bool bAddOK = false;
            Patient addedNewPatientHiDetails = new Patient();
            try
            {
                AxLogger.Instance.LogInfo("Start adding new patient and HI Details", CurrentUser);
                if (newPatientAndHiDetails.CurrentHealthInsurance != null)
                {
                    bool res = ValidateHiItem(newPatientAndHiDetails.CurrentHealthInsurance, out IBID, out HIPCode);
                }

                newPatientAndHiDetails.PatientCode = new ServiceSequenceNumberProvider().GetPatientCode();
                bAddOK = PatientProvider.Instance.AddNewPatientAndHIDetails(newPatientAndHiDetails, out PatientID, out HIID, out PaperReferralID);
                if (bAddOK)
                {
                    PatientCode = newPatientAndHiDetails.PatientCode;

                    bool isConsideredAsCrossRegion = false;

                    CalculatedHiBenefit = CalculateHiBenefit(newPatientAndHiDetails.CurrentHealthInsurance, newPatientAndHiDetails.ActivePaperReferal,
                            out isConsideredAsCrossRegion, false, V_RegistrationType, _isEmergInPtReExamination, _isHICard_FiveYearsCont,
                            _isChildUnder6YearsOld, _isAllowCrossRegion, IsHICard_FiveYearsCont_NoPaid);

                    RebatePercentage = CalculatedHiBenefit;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of adding new patient and HI Details. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

            return bAddOK;
        }

        #endregion
        //▲===== #005
        //▼====== #007
        public bool UpdateBasicDiagTreatment(PatientRegistration regInfo, DiseasesReference aAdmissionICD10, Staff gSelectedDoctorStaff)
        {
            try
            {
                bool bOK = false;
                bOK = PatientProvider.Instance.UpdateBasicDiagTreatment(regInfo, aAdmissionICD10, gSelectedDoctorStaff);
                return bOK;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving UpdateBasicDiagTreatment. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_STAFF_CANNOT_FOUND);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====== #007

        public PromoDiscountProgram EditPromoDiscountProgram(PromoDiscountProgram aPromoDiscountProgram, long PtRegistrationID)
        {
            try
            {
                return PatientProvider.Instance.EditPromoDiscountProgram(aPromoDiscountProgram, PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of retrieving EditPromoDiscountProgram. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.CO_STAFF_CANNOT_FOUND);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public bool SetEkipForService(PatientRegistration CurrentRegistration)
        {
            try
            {
                bool Result = false;
                Result = PatientProvider.Instance.SetEkipForService(CurrentRegistration);
                return Result;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SetEkipForService. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, "Error");
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        //▼===== #011
        public List<PatientRegistration> SearchRegistrationListForCheckDiagAndPrescription(CheckDiagAndPrescriptionSearchCriteria SearchCriteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Searching patient registration for Check Diagnosis and Prescription", CurrentUser);
                List<PatientRegistration> retVal;
                retVal = PatientProvider.Instance.SearchRegistrationListForCheckDiagAndPrescription(SearchCriteria);
                AxLogger.Instance.LogInfo("End of Searching patient registration for Check Diagnosis and Prescription. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Searching patient registration for Check Diagnosis and Prescription. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }
        public List<PatientRegistration> SearchRegistrationForCirculars56(DateTime FromDate, DateTime ToDate, long PatientFindBy,string PatientCode)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start SearchRegistrationForCirculars56", CurrentUser);
                List<PatientRegistration> retVal;
                retVal = PatientProvider.Instance.SearchRegistrationForCirculars56(FromDate, ToDate, PatientFindBy, PatientCode);
                AxLogger.Instance.LogInfo("End of SearchRegistrationForCirculars56. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SearchRegistrationForCirculars56. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public SummaryMedicalRecords GetSummaryMedicalRecords_ByPtRegID(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetSummaryMedicalRecords_ByPtRegID", CurrentUser);
                SummaryMedicalRecords retVal;
                retVal = PatientProvider.Instance.GetSummaryMedicalRecords_ByPtRegID(PtRegistrationID, V_RegistrationType);
                AxLogger.Instance.LogInfo("End of GetSummaryMedicalRecords_ByPtRegID. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetSummaryMedicalRecords_ByPtRegID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public PatientTreatmentCertificates GetPatientTreatmentCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType, out int LastCode)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetPatientTreatmentCertificates_ByPtRegID", CurrentUser);
                PatientTreatmentCertificates retVal;
                retVal = PatientProvider.Instance.GetPatientTreatmentCertificates_ByPtRegID(PtRegistrationID, V_RegistrationType, out LastCode);
                AxLogger.Instance.LogInfo("End of GetPatientTreatmentCertificates_ByPtRegID. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPatientTreatmentCertificates_ByPtRegID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public InjuryCertificates GetInjuryCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType,out int LastCode)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetInjuryCertificates_ByPtRegID", CurrentUser);
                InjuryCertificates retVal;
                retVal = PatientProvider.Instance.GetInjuryCertificates_ByPtRegID(PtRegistrationID, V_RegistrationType,out LastCode);
                AxLogger.Instance.LogInfo("End of GetInjuryCertificates_ByPtRegID. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInjuryCertificates_ByPtRegID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<BirthCertificates> GetBirthCertificates_ByPtRegID(long PtRegistrationID, long V_RegistrationType, out int LastCode)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetBirthCertificates_ByPtRegID", CurrentUser);
                List<BirthCertificates> retVal;
                retVal = PatientProvider.Instance.GetBirthCertificates_ByPtRegID(PtRegistrationID, V_RegistrationType, out LastCode);
                AxLogger.Instance.LogInfo("End of GetBirthCertificates_ByPtRegID. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetBirthCertificates_ByPtRegID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public VacationInsuranceCertificates GetVacationInsuranceCertificates_ByPtRegID(long PtRegistrationID, bool IsPrenatal, out int LastCode)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Get VacationInsuranceCertificates_ByPtRegID", CurrentUser);
                VacationInsuranceCertificates retVal;
                retVal = PatientProvider.Instance.GetVacationInsuranceCertificates_ByPtRegID(PtRegistrationID, IsPrenatal, out LastCode);
                AxLogger.Instance.LogInfo("End of Get VacationInsuranceCertificates_ByPtRegID. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Get VacationInsuranceCertificates_ByPtRegID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public SummaryMedicalRecords SaveSummaryMedicalRecords(SummaryMedicalRecords CurrentSummaryMedicalRecords, long UserID, string Date)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start SaveSummaryMedicalRecords", CurrentUser);
                SummaryMedicalRecords retVal;
                retVal = PatientProvider.Instance.SaveSummaryMedicalRecords(CurrentSummaryMedicalRecords, UserID, Date);
                AxLogger.Instance.LogInfo("End of SaveSummaryMedicalRecords. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveSummaryMedicalRecords. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public PatientTreatmentCertificates SavePatientTreatmentCertificates(PatientTreatmentCertificates CurrentPatientTreatmentCertificates, long UserID, string Date)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start SavePatientTreatmentCertificates", CurrentUser);
                PatientTreatmentCertificates retVal;
                retVal = PatientProvider.Instance.SavePatientTreatmentCertificates(CurrentPatientTreatmentCertificates,UserID, Date);
                AxLogger.Instance.LogInfo("End of SavePatientTreatmentCertificates. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SavePatientTreatmentCertificates. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public InjuryCertificates SaveInjuryCertificates(InjuryCertificates CurrentInjuryCertificates, long UserID, string Date)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start SaveInjuryCertificates", CurrentUser);
                InjuryCertificates retVal;
                retVal = PatientProvider.Instance.SaveInjuryCertificates(CurrentInjuryCertificates, UserID, Date);
                AxLogger.Instance.LogInfo("End of SaveInjuryCertificates. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveInjuryCertificates. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool SaveBirthCertificates(BirthCertificates CurrentBirthCertificates, long UserID, string Date)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start SaveBirthCertificates", CurrentUser);
                bool retVal;
                retVal = PatientProvider.Instance.SaveBirthCertificates(CurrentBirthCertificates, UserID, Date);
                AxLogger.Instance.LogInfo("End of SaveBirthCertificates. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveBirthCertificates. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public VacationInsuranceCertificates SaveVacationInsuranceCertificates(VacationInsuranceCertificates CurrentVacationInsuranceCertificates, bool IsPrenatal, long UserID, string Date)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Save VacationInsuranceCertificates", CurrentUser);
                VacationInsuranceCertificates retVal;
                retVal = PatientProvider.Instance.SaveVacationInsuranceCertificates(CurrentVacationInsuranceCertificates, IsPrenatal, UserID, Date);
                AxLogger.Instance.LogInfo("End of Save VacationInsuranceCertificates. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Save VacationInsuranceCertificates. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public string GetSummaryPCLResultByPtRegistrationID(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetSummaryPCLResultByPtRegistrationID", CurrentUser);
                string retVal;
                retVal = PatientProvider.Instance.GetSummaryPCLResultByPtRegistrationID(PtRegistrationID, V_RegistrationType);
                AxLogger.Instance.LogInfo("End of GetSummaryPCLResultByPtRegistrationID. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetSummaryPCLResultByPtRegistrationID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public string GetPCLResultForInjuryCertificatesByPtRegistrationID(long PtRegistrationID, out string PCLResultFromAdmissionExamination)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetPCLResultForInjuryCertificatesByPtRegistrationID", CurrentUser);
                string retVal;
                retVal = PatientProvider.Instance.GetPCLResultForInjuryCertificatesByPtRegistrationID(PtRegistrationID, out PCLResultFromAdmissionExamination);
                AxLogger.Instance.LogInfo("End of GetPCLResultForInjuryCertificatesByPtRegistrationID. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetPCLResultForInjuryCertificatesByPtRegistrationID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool GetPatientPrescriptionAndPCLReq(long PtRegistrationID, long DoctorID, out List<PrescriptionDetail> curPrescriptionDetail, out List<PatientPCLRequestDetail> curPatientPCLRequestDetail, out bool bPreOK, out bool bPCLOK)
        {
            try
            {
                bPreOK = false;
                bPCLOK = false;
                curPrescriptionDetail = new List<PrescriptionDetail>();
                var ListPrescriptionDetail = new List<PrescriptionDetail>();
                curPatientPCLRequestDetail = new List<PatientPCLRequestDetail>();
                var LisPatientPCLRequestDetail = new List<PatientPCLRequestDetail>();

                AxLogger.Instance.LogInfo("Start Get Prescription Info.", CurrentUser);
                bPreOK = ePrescriptionsProvider.Instance.GetPrescriptionByPtRegistrationID(PtRegistrationID, DoctorID, out ListPrescriptionDetail);
                if (bPreOK)
                {
                    curPrescriptionDetail = ListPrescriptionDetail;
                }
                AxLogger.Instance.LogInfo("End of Get Prescription Info. Status: Success", CurrentUser);

                AxLogger.Instance.LogInfo("Start Get PCL Request Details Info.", CurrentUser);
                bPCLOK = ePrescriptionsProvider.Instance.GetPatientPCLReqByPtRegistrationID(PtRegistrationID, out LisPatientPCLRequestDetail);
                if (bPCLOK)
                {
                    curPatientPCLRequestDetail = LisPatientPCLRequestDetail;
                }
                AxLogger.Instance.LogInfo("End of Get PCL Request Details. Status: Success", CurrentUser);
                if (bPreOK && bPCLOK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Get Prescription And PCL Request By PtRegistrationID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }

        }
        //▲===== #011
        //▼===== #022
        public List<PatientRegistration> SearchRegistrationListForCheckMedicalFiles(CheckMedicalFilesSearchCriteria SearchCriteria)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Searching patient registration for Check Medical Files", CurrentUser);
                List<PatientRegistration> retVal;
                retVal = PatientProvider.Instance.SearchRegistrationListForCheckMedicalFiles(SearchCriteria);
                AxLogger.Instance.LogInfo("End of Searching patient registration for Check Medical Files. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Searching patient registration for Check Medical Files. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool GetPatientPCLReq(long PtRegistrationID, out List<PatientPCLRequestDetail> curPatientPCLRequestDetail, out bool bPCLOK)
        {
            try
            {
                bPCLOK = false;

                curPatientPCLRequestDetail = new List<PatientPCLRequestDetail>();
                var LisPatientPCLRequestDetail = new List<PatientPCLRequestDetail>();

                AxLogger.Instance.LogInfo("Start Get Prescription Info.", CurrentUser);

                AxLogger.Instance.LogInfo("End of Get Prescription Info. Status: Success", CurrentUser);

                AxLogger.Instance.LogInfo("Start Get PCL Request Details Info.", CurrentUser);
                bPCLOK = ePrescriptionsProvider.Instance.GetPatientPCLReqByInPtRegistrationID(PtRegistrationID, out LisPatientPCLRequestDetail);
                if (bPCLOK)
                {
                    curPatientPCLRequestDetail = LisPatientPCLRequestDetail;
                }
                AxLogger.Instance.LogInfo("End of Get PCL Request Details. Status: Success", CurrentUser);
                if (bPCLOK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Get Prescription And PCL Request By PtRegistrationID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool GetPatientRegistrationDetails(long PtRegistrationID, out List<PatientRegistrationDetail> curPatientRegistrationDetail, out bool bPCLOK)
        {
            try
            {
                bPCLOK = false;
                curPatientRegistrationDetail = new List<PatientRegistrationDetail>();
                var ListPatientRegistrationDetail = new List<PatientRegistrationDetail>();
                AxLogger.Instance.LogInfo("Start Get PatientRegistrationDetail for Check medical file of KHTH.", CurrentUser);
                bPCLOK = ePrescriptionsProvider.Instance.GetListPatientRegistrationDetailForCheckMedicalFile(PtRegistrationID, out ListPatientRegistrationDetail);
                if (bPCLOK)
                {
                    curPatientRegistrationDetail = ListPatientRegistrationDetail;
                }
                AxLogger.Instance.LogInfo("End of Get PatientRegistrationDetail. Status: Success", CurrentUser);
                if (bPCLOK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PatientRegistrationDetail for Check medical file of KHTH. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public CheckMedicalFiles GetMedicalFilesByPtRegistrationID(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetMedicalFilesByPtRegistrationID", CurrentUser);
                return PatientProvider.Instance.GetMedicalFilesByPtRegistrationID(PtRegistrationID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedicalFilesByPtRegistrationID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool SaveMedicalFiles(CheckMedicalFiles CheckMedicalFile, bool Is_KHTH, long V_RegistrationType, long StaffID, out long CheckMedicalFileIDNew)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start SaveMedicalFiles", CurrentUser);
                return PatientProvider.Instance.SaveMedicalFiles(CheckMedicalFile, Is_KHTH, V_RegistrationType, StaffID, out CheckMedicalFileIDNew);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveMedicalFiles. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public List<CheckMedicalFiles> GetListCheckMedicalFilesByPtID(long PtRegistrationID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetListCheckMedicalFilesByPtID", CurrentUser);
                return PatientProvider.Instance.GetListCheckMedicalFilesByPtID(PtRegistrationID, V_RegistrationType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetListCheckMedicalFilesByPtID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲===== #022
        public bool SaveNutritionalRating(long PtRegistrationID, NutritionalRating curNutritionalRating, long StaffID, DateTime Date, out long NutritionalRatingIDNew)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start SaveNutritionalRating", CurrentUser);
                return PatientProvider.Instance.SaveNutritionalRating(PtRegistrationID, curNutritionalRating, StaffID, Date, out NutritionalRatingIDNew);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveNutritionalRating. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public List<NutritionalRating> GetNutritionalRatingByPtRegistrationID(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetNutritionalRatingByPtRegistrationID", CurrentUser);
                return PatientProvider.Instance.GetNutritionalRatingByPtRegistrationID(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetNutritionalRatingByPtRegistrationID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool DeleteNutritionalRating(long NutritionalRatingID, long StaffID, DateTime Date)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start DeleteNutritionalRating", CurrentUser);
                return PatientProvider.Instance.DeleteNutritionalRating(NutritionalRatingID, StaffID, Date);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of DeleteNutritionalRating. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public bool CheckBeforeChangeDept(long registrationID, long DeptID
         , out string errorMessages, out string confirmMessages)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Check Before Change Dept.", CurrentUser);
                return PatientProvider.Instance.CheckBeforeChangeDept(registrationID, DeptID, out errorMessages, out confirmMessages);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End Check Before Change Dept. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CheckBeforeDischarge, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }

        //▼===== #012
        public Prescription CheckOldConsultationPatient(long PatientID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Check Old Consultation Patient.", CurrentUser);
                return PatientProvider.Instance.CheckOldConsultationPatient(PatientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End Check Old Consultation Patient. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CheckBeforeDischarge, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }
        //▲===== #012

        //▲===== #013
        public List<Patient> AddNewListPatient(List<APIPatient> ListPatient, string ContractNo)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start adding new list patient...", CurrentUser);
                {
                    List<Patient> PatientList = new List<Patient>();
                    List<APIPatient> tmpListPatient = ListPatient;
                    List<APIPatient> tmpListPatientAfterCreateCode = new List<APIPatient>();
                    foreach (APIPatient tmpPatient in tmpListPatient)
                    {
                        tmpPatient.PatientCode = new ServiceSequenceNumberProvider().GetPatientCode();
                        tmpListPatientAfterCreateCode.Add(tmpPatient);
                    }
                    ListPatient = tmpListPatientAfterCreateCode;
                    PatientList = PatientProvider.Instance.AddListPatient(ListPatient, ContractNo);
                    AxLogger.Instance.LogInfo("End of adding new list patient. Status: Success", CurrentUser);
                    return PatientList;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of adding new list patient. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== #013
        //▼===== #014
        public RefDepartment LoadDeptAdmissionRequest(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Load Dept Admission Request...", CurrentUser);
                {
                    RefDepartment department = new RefDepartment();
                    department = PatientProvider.Instance.LoadDeptAdmissionRequest(PtRegistrationID);
                    AxLogger.Instance.LogInfo("End of Load Dept Admission Request. Status: Success", CurrentUser);
                    return department;
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Check Dept Admission Request. Status: Failed.", CurrentUser);

                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");

                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== #014
        public IList<InfectionCase> GetInfectionCaseByPtRegID(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetInfectionCaseByPtRegID...", CurrentUser);
                {
                    return PatientProvider.Instance.GetInfectionCaseInfo(PtRegistrationID, null);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInfectionCaseByPtRegID. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<InfectionCase> GetInfectionCaseByPtID(long PatientID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetInfectionCaseByPtID...", CurrentUser);
                {
                    return PatientProvider.Instance.GetInfectionCaseInfo(null, PatientID);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInfectionCaseByPtID. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public InfectionCase GetInfectionCaseDetail(InfectionCase aInfectionCase)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetInfectionCaseDetail...", CurrentUser);
                {
                    return PatientProvider.Instance.GetInfectionCaseDetail(aInfectionCase);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInfectionCaseDetail. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public InfectionCase GetInfectionCaseAllDetails(InfectionCase aInfectionCase)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetInfectionCaseAllDetails...", CurrentUser);
                {
                    return PatientProvider.Instance.GetInfectionCaseDetail(aInfectionCase, true);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInfectionCaseAllDetails. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<InfectionVirus> GetAllInfectionVirus()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetAllInfectionVirus...", CurrentUser);
                {
                    return PatientProvider.Instance.GetAllInfectionVirus();
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllInfectionVirus. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public long EditInfectionCase(InfectionCase aInfectionCase)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start EditInfectionCase...", CurrentUser);
                {
                    return PatientProvider.Instance.EditInfectionCase(aInfectionCase);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of EditInfectionCase. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public void EditAntibioticTreatment(AntibioticTreatment aAntibioticTreatment)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start EditAntibioticTreatment...", CurrentUser);
                {
                    PatientProvider.Instance.EditAntibioticTreatment(aAntibioticTreatment);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of EditAntibioticTreatment. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<AntibioticTreatmentMedProductDetail> GetAntibioticTreatmentMedProductDetails(long AntibioticTreatmentID, long V_AntibioticTreatmentType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetAntibioticTreatmentMedProductDetails...", CurrentUser);
                {
                    return PatientProvider.Instance.GetAntibioticTreatmentMedProductDetails(AntibioticTreatmentID, V_AntibioticTreatmentType);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAntibioticTreatmentMedProductDetails. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<OutwardDrugClinicDept> GetDrugsInUseOfAntibioticTreatment(AntibioticTreatment aAntibioticTreatment, long DeptID, long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetDrugsInUseOfAntibioticTreatment...", CurrentUser);
                {
                    return PatientProvider.Instance.GetDrugsInUseOfAntibioticTreatment(aAntibioticTreatment, DeptID, PtRegistrationID);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetDrugsInUseOfAntibioticTreatment. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<InfectionCase> GetInfectionCaseAllContentInfo(DateTime StartDate, DateTime EndDate, long V_InfectionCaseStatus, string DrugName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetInfectionCaseAllContentInfo...", CurrentUser);
                {
                    return PatientProvider.Instance.GetInfectionCaseAllContentInfo(StartDate, EndDate, V_InfectionCaseStatus, DrugName);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInfectionCaseAllContentInfo. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<AntibioticTreatmentMedProductDetailSummaryContent> GetAllContentInfoOfInfectionCaseCollection(DateTime StartDate, DateTime EndDate, long V_InfectionCaseStatus, string DrugName)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetInfectionCaseAllContentInfo...", CurrentUser);
                {
                    return PatientProvider.Instance.GetAllContentInfoOfInfectionCaseCollection(StartDate, EndDate, V_InfectionCaseStatus, DrugName);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetInfectionCaseAllContentInfo. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public IList<AntibioticTreatment> GetAntibioticTreatmentsByPtRegID(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetAntibioticTreatmentsByPtRegID...", CurrentUser);
                {
                    return PatientProvider.Instance.GetAntibioticTreatmentsByPtRegID(PtRegistrationID);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAntibioticTreatmentsByPtRegID. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void InsertAntibioticTreatmentIntoInstruction(long AntibioticTreatmentID, long InfectionCaseID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start InsertAntibioticTreatmentIntoInstruction...", CurrentUser);
                {
                    PatientProvider.Instance.InsertAntibioticTreatmentIntoInstruction(AntibioticTreatmentID, InfectionCaseID);
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of InsertAntibioticTreatmentIntoInstruction. Status: Failed.", CurrentUser);
                MethodBase curMethod = MethodBase.GetCurrentMethod();
                AxException axErr = new AxException(ex, curMethod, "PR.0_0000002");
                AxLogger.Instance.LogDebug(axErr, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public IList<PatientRegistration> SearchRegistrationAndGetPrescription(SeachPtRegistrationCriteria regCriteria, int regPageIndex, int regPageSize, bool bregCountTotal,
                    PrescriptionSearchCriteria presCriteria, int presPageIndex, int presPageSize, bool bpresCountTotal, out int presTotalCount, out IList<Prescription> lstPrescriptions,
                    out int regTotalCount)
        {
            presTotalCount = 0;
            regTotalCount = 0;
            lstPrescriptions = null;
            try
            {
                AxLogger.Instance.LogInfo("SearchRegistrationAndGetPrescription BEGIN ...", CurrentUser);

                IList<PatientRegistration> retVal = null; ;
                retVal = PatientProvider.Instance.SearchRegistrations(regCriteria, regPageIndex, regPageSize, bregCountTotal, out regTotalCount);

                if (retVal != null && retVal.Count == 1)
                {
                    if (presCriteria.PtRegistrationID == 0)
                    {
                        presCriteria.PtRegistrationID = retVal[0].PtRegistrationID;
                    }
                    lstPrescriptions = aEMR.DataAccessLayer.Providers.ePrescriptionsProvider.Instance.SearchPrescription(presCriteria, presPageIndex, presPageSize, bpresCountTotal, out presTotalCount);
                }

                AxLogger.Instance.LogInfo("SearchRegistrationAndGetPrescription END: Success", CurrentUser);

                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError("SearchRegistrationAndGetPrescription Exception Error: ", ex.Message);
            }

            return null;

        }

        public IList<PatientRegistration> GetAllRegistrationForSettlement(long PatientID)
        {

            try
            {
                AxLogger.Instance.LogInfo("GetAllRegistrationForSettlement BEGIN ...", CurrentUser);

                IList<PatientRegistration> retVal = null; ;
                retVal = PatientProvider.Instance.GetAllRegistrationForSettlement(PatientID);
                AxLogger.Instance.LogInfo("GetAllRegistrationForSettlement END: Success", CurrentUser);

                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogError("GetAllRegistrationForSettlement Exception Error: ", ex.Message);
            }

            return null;

        }
        //▼===== #017
        public void MergerPatientRegistration(PatientRegistration PtRegistration, long StaffID)
        {
            try
            {
                //Danh sách các biến sử dụng cho Merge
                List<PatientRegistrationDetail> CurRegistrationDetails = new List<PatientRegistrationDetail>();
                List<OutwardDrugInvoice> CurOutwardDrugs = new List<OutwardDrugInvoice>();
                PatientRegistration registrationInfoForCancel = new PatientRegistration();
                PatientPCLRequest PCLReq = new PatientPCLRequest();
                long DepLocID = 0;
                decimal PayAmount = 0;
                PatientTransaction Transaction = new PatientTransaction();
                PatientTransactionPayment paymentInfo = new PatientTransactionPayment();
                List<PaymentAndReceipt> paymentInfoList = new List<PaymentAndReceipt>();
                V_RegistrationError error = V_RegistrationError.mNone;

                DateTime DateTimeRefund = DateTime.Now;

                AxLogger.Instance.LogInfo("Start MergerPatientRegistration.", CurrentUser);
                
                //▼==== #026
                if (PtRegistration != null && PtRegistration.OutPtRegistrationID == 0)
                {
                    AxLogger.Instance.LogInfo("MergerPatientRegistration => GetRegistrationTxd => Registration is cancel.");
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z3248_G1_DkyKCanSatNhap));
                }
                //▲==== #026

                //Lấy tất cả chi tiết của đăng ký ngoại trú
                PatientRegistration registrationInfo = RegAndPaymentProcessorBase.GetRegistrationTxd(PtRegistration.OutPtRegistrationID, (int)AllLookupValues.V_FindPatientType.NGOAI_TRU);
                if (registrationInfo == null || (registrationInfo != null && registrationInfo.V_RegistrationStatus == (long)AllLookupValues.RegistrationStatus.REFUND))
                {
                    AxLogger.Instance.LogInfo("MergerPatientRegistration => GetRegistrationTxd => Registration is cancel.");
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z3036_G1_DaDuocSapNhap));
                }

                if (registrationInfo != null && registrationInfo.ConfirmHIStaffID > 0)
                {
                    AxLogger.Instance.LogInfo("MergerPatientRegistration => GetRegistrationTxd => Registration is Confirm.");
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z3041_G1_DangKyDaDuocXacNhanKhongTheSapNhap));
                }
                if (registrationInfo != null)
                {
                    CurOutwardDrugs = registrationInfo.DrugInvoices.Where(item => (item.V_OutDrugInvStatus != (long)AllLookupValues.V_OutDrugInvStatus.RETURN
                        && item.V_OutDrugInvStatus != (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)).ToList();
                    registrationInfoForCancel = DeepCopier.EntityDeepCopy(registrationInfo);
                }

                //▼===== 20200622 TTM: Chưa biết được có chặn còn thuốc hay không nên comment ra
                if (CurOutwardDrugs != null && CurOutwardDrugs.Count > 0)
                {
                    throw new Exception(string.Format("{0}", eHCMSResources.Z3039_G1_KiemTraPhieuXuatSapNhap));
                }
                //▲===== 

                //Cập nhật lại ngày nhập viện
                try
                {
                    PatientProvider.Instance.UpdateAdmissionDate(PtRegistration.PtRegistrationID);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("MergerPatientRegistration => UpdateAdmissionDate => ", ex.ToString());
                    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddInPatientAdmission, CurrentUser);
                    throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
                }

                //Lấy kết quả CLS

                //List<PatientPCLImagingResult> patientPCLImagingResults = PatientProvider.Instance.GetPatientPCLImagingResult_ByPtRegistrationID(PtRegistration.OutPtRegistrationID);
                //List<PatientPCLLaboratoryResultDetail> patientPCLLaboratoryResultDetails = PatientProvider.Instance.GetPatientPCLLaboratoryResultDetail_ByPtRegistrationID(PtRegistration.OutPtRegistrationID);

                //Thay đổi ID của các chi tiết để chuẩn bị lưu cho nội trú
                try
                {
                    PCLReq.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
                    foreach (PatientPCLRequest req in registrationInfo.PCLRequests)
                    {

                        foreach (PatientPCLRequestDetail detail in req.PatientPCLRequestIndicators)
                        {
                            if (detail.PaidTime != null && detail.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && detail.ExamRegStatus != AllLookupValues.ExamRegStatus.XOA_TRA_TIEN_LAI)
                            {
                                detail.PCLReqItemID = 0;
                                detail.ServiceSeqNum = 0;
                                detail.IsCountPatient = true;
                                detail.PatientPCLRequest.DoctorStaffID = req.DoctorStaffID;
                                //▼===== Set giá trị cho V_PCLRequestStatus nội trú theo ngoại trú (ngoại đã thực hiện thì đem vào nội cũng đã thực hiện).

                                detail.PatientPCLRequest.V_PCLRequestStatus = req.V_PCLRequestStatus;

                                detail.PatientPCLRequest.V_PCLRequestType = AllLookupValues.V_PCLRequestType.NOI_TRU;

                                //▼=====    Set ngày y lệnh thành ngày tạo phiếu vì ngoại trú rất nhiều trường hợp ngày y lệnh không có dẫn tới đưa vào SplitVote sẽ
                                //          sai
                                detail.MedicalInstructionDate = req.CreatedDate;

                                PCLReq.PatientPCLRequestIndicators.Add(detail);
                            }
                        }
                    }

                    //Gọi hàm insert các chi tiết vào bảng của nội trú (CLS cần phải gọi cục cấp số để tạo ra mã phiếu mới)
                    AddPCLRequestsForInPt(PtRegistration, registrationInfo.StaffID.GetValueOrDefault(), PtRegistration.AdmissionInfo.DeptLocationID, PtRegistration.AdmissionInfo.DeptID, PCLReq, null, false, DateTime.Now);
                    //▼===== #018
                    //foreach (PatientRegistrationDetail detail in registrationInfo.PatientRegistrationDetails)
                    //{
                    //    detail.ReqFromDeptID = PtRegistration.AdmissionInfo.DeptID;
                    //    detail.IsCountPatient = true;
                    //    detail.IsCountHI = detail.TotalHIPayment > 0;
                    //}
                    CalcPatientRegistrationDetail(registrationInfo, PtRegistration);
                    //▲===== #018
                    PatientProvider.Instance.AddRegistrationDetailList_InPt(registrationInfo);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("MergerPatientRegistration => AddRegistrationDetailList_InPt => ", ex.ToString());

                    //▼=====    Do Service đang Reference tới file eHCMS Language cũ (ehcms) nên không thấy những gì mới của aEMR nên set bằng chữ khi nào sửa được sẽ 
                    //          set lại cho chính xác (Do anh Tuấn nói không được đụng vào Reference.
                    throw new Exception(string.Format("{0}", eHCMSResources.Z3034_G1_DuLieuSapNhapLoi));
                    //throw new Exception(string.Format("{0}.", "Dữ liệu ngoại trú sáp nhập nội trú có vấn đề."));
                }

                //Tạo 1 phiếu thu tạm ứng cho các dịch vụ được đưa vào nội trú
                try
                {
                    //▼====: #022
                    if (Globals.AxServerSettings.CommonItems.AllowFirstHIExaminationWithoutPay)
                    {
                        CurRegistrationDetails = registrationInfoForCancel.PatientRegistrationDetails.Where(item => (item.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && item.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.XOA_TRA_TIEN_LAI)).ToList();
                    }
                    else
                    {
                        CurRegistrationDetails = registrationInfoForCancel.PatientRegistrationDetails.Where(item => item.PaidTime != null && (item.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && item.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.XOA_TRA_TIEN_LAI)).ToList();
                    }
                    //▼===== 20200708 TTM:  Thay đổi cách tính toán từ tính theo công thức dựa trên Details
                    //                      => sử dụng các biên lai thu tiền của bệnh nhân. Để tính ra só tiền tạm ứng cho nội trú.
                    //PayAmount = CalcPatientPayment(registrationInfoForCancel, CurRegistrationDetails, PCLReq, null);
                    if (registrationInfoForCancel.PatientTransaction != null)
                    {
                        foreach (var PatientCashAdvDetail in registrationInfoForCancel.PatientTransaction.PatientCashAdvances)
                        {
                            PayAmount += PatientCashAdvDetail.PaymentAmount;
                        }
                        //▲===== 
                        var payment = new PatientCashAdvance
                        {
                            PaymentAmount = PayAmount,
                            //▼=====    Do Service đang Reference tới file eHCMS Language cũ (ehcms) nên không thấy những gì mới của aEMR nên set bằng chữ khi nào sửa được sẽ 
                            //          set lại cho chính xác (Do anh Tuấn nói không được đụng vào Reference.
                            GeneralNote = eHCMSResources.Z3033_G1_ChuyenTienSapNhap,
                            //GeneralNote = "Chuyển tiền thu ngoại trú vào nội trú.",

                            PtRegistrationID = PtRegistration.PtRegistrationID,
                            V_RegistrationType = PtRegistration.V_RegistrationType,
                            PaymentDate = DateTimeRefund,
                            StaffID = StaffID,
                            RptPtCashAdvRemID = 0,
                            V_PaymentMode = new Lookup(),
                            V_PaymentReason = new Lookup(),
                        };
                        payment.V_PaymentMode.LookupID = (long)AllLookupValues.PaymentMode.TIEN_MAT;
                        payment.V_PaymentReason.LookupID = (long)AllLookupValues.V_PaymentReason.TAM_UNG_NOI_TRU;
                        long PtCashAdvanceID = 0;
                        if (PayAmount > 0)
                        {
                            PaymentProvider.Instance.PatientCashAdvance_Insert(payment, out PtCashAdvanceID);
                        }
                        //▲====: #022
                    }
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("MergerPatientRegistration => PatientCashAdvance_Insert => ", ex.ToString());
                    //▼=====    Do Service đang Reference tới file eHCMS Language cũ (ehcms) nên không thấy những gì mới của aEMR nên set bằng chữ khi nào sửa được sẽ 
                    //          set lại cho chính xác (Do anh Tuấn nói không được đụng vào Reference.
                    throw new Exception(string.Format("{0}", eHCMSResources.Z3032_G1_KhongTheTamUngSapNhap));
                    //throw new Exception(string.Format("{0}.", "Không tạo được dữ liệu tạm ứng cho ca ngoại trú sáp nhập."));
                }

                //Huỷ các dịch vụ, CLS, ... ngoại trú
                //Set RecordState của các dịch vụ về Deleted để khi xuống lưu tự động sẽ đưa vào danh sách huỷ.
                try
                {
                    foreach (var item in CurRegistrationDetails)
                    {
                        item.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                        item.RecordState = RecordState.MODIFIED;
                        item.RefundTime = DateTimeRefund;
                    }
                    foreach (var item in registrationInfoForCancel.PCLRequests)
                    {
                        foreach (var detail in item.PatientPCLRequestIndicators)
                        {
                            detail.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                            detail.CanDelete = false;
                            detail.RecordState = RecordState.MODIFIED;
                            detail.RefundTime = DateTimeRefund;
                        }
                        item.RecordState = RecordState.MODIFIED;
                        item.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.CANCEL;
                    }
                    //20200630 TTM: Mặc định DepLocID = 0 vì trong hàm Remove => AddUpdate đã không còn sử dụng nên mặc định bằng 0 để hợp thức hoá Method 
                    //              RemovePaidRegItems và PayForRegistration_Cancel
                    RemovePaidRegItems(registrationInfoForCancel, StaffID, (long)DepLocID, null, PtRegistration.OutPtRegistrationID, (int)AllLookupValues.PatientFindBy.NGOAITRU
                        , CurRegistrationDetails, registrationInfoForCancel.PCLRequests.ToList(), null, null, null, true);
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("MergerPatientRegistration => RemovePaidRegItems => ", ex.ToString());
                    //▼=====    Do Service đang Reference tới file eHCMS Language cũ (ehcms) nên không thấy những gì mới của aEMR nên set bằng chữ khi nào sửa được sẽ 
                    //          set lại cho chính xác (Do anh Tuấn nói không được đụng vào Reference.
                    throw new Exception(string.Format("{0}", eHCMSResources.Z3031_G1_KhongTheHuyDichVuCLSSapNhap));
                    //throw new Exception(string.Format("{0}.", "Không thể huỷ dịch vụ, CLS của đăng ký ngoại trú xáp nhập."));
                }

                try
                {
                    PatientTransactionPayment CurrentPayment = new PatientTransactionPayment
                    {
                        StaffID = StaffID,
                        PaymentMode = new Lookup() { LookupID = (long)AllLookupValues.PaymentMode.TIEN_MAT },
                        PaymentType = new Lookup() { LookupID = (long)AllLookupValues.PaymentType.HOAN_TIEN },
                        Currency = new Lookup() { LookupID = (long)AllLookupValues.Currency.VND },
                        PtPmtAccID = 1, //PtPmtAccID = 3 => Bệnh viện thanh toán.
                        V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.DANG_KY,
                    };

                    //▼===== 20200622 TTM: Do RemovePaidRegItems tạo ra để đem làm dòng cân bằng huỷ, mà lúc này không load lại nên các dòng này không có ID nếu đem chạy tiếp store trả
                    //tiền thi sẽ bị nhận nhầm là các dòng chưa có đưa vào bảng PatientTransactionDetails => tạo ra 2 lần trong PatientTransactionDetail => trả tiền 2 lần.
                    if (registrationInfoForCancel != null && registrationInfoForCancel.PatientTransaction != null)
                    {
                        foreach (var Transdetail in registrationInfoForCancel.PatientTransaction.PatientTransactionDetails)
                        {
                            if (Transdetail.TransItemID == 0)
                            {
                                //▼===== 20200622 TTM: Set giá trị mặc định để lúc tạo ra phiếu thu huỷ không đem các PatientTransactionDetails đó lưu lần 2
                                Transdetail.TransItemID = 99999999;
                                //▲=====
                            }
                        }
                    }

                    //▼===== 20200622 TTM:  Vì lý do không load lại dữ liệu (Code bình thường khi huỷ sẽ đi theo hướng 
                    //                      => Chọn dịch vụ huỷ => lưu => load lại => trả tiền => đem dịch vụ load lại đã huỷ đi hoàn tiền.
                    //                      Do không theo quy trình chuẩn nên phải set lại sau khi huỷ cho tương tự như load lại
                    foreach (var item in CurRegistrationDetails)
                    {
                        item.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                        item.RecordState = RecordState.UNCHANGED;
                    }
                    foreach (var item in registrationInfoForCancel.PCLRequests)
                    {
                        foreach (var detail in item.PatientPCLRequestIndicators)
                        {
                            detail.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                            detail.CanDelete = false;
                            detail.RecordState = RecordState.UNCHANGED;
                        }
                        item.RecordState = RecordState.UNCHANGED;
                        item.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.CANCEL;
                    }
                    //▼====: #024
                    foreach (var item in registrationInfoForCancel.InPatientBillingInvoices)
                    {
                        if (item.PaidTime != null)
                        {
                            item.RecordState = RecordState.MERGER;
                        }
                    }
                    //▲====: #024
                    //▲===== 
                    CurrentPayment.PayAmount = PayAmount;
                    //▼===== #020: Đối với sáp nhập mặc định khi insert CLS sẽ không kiểm tra giãn cách vì insert xong thì ngoại trú cũng xóa ra mà thôi.
                    CurrentPayment.PayAmount = PayAmount;
                    PayForRegistration_Cancel(registrationInfoForCancel, StaffID, (long)DepLocID, null, PtRegistration.PtRegistrationID, (int)AllLookupValues.PatientFindBy.NGOAITRU
                    , CurrentPayment
                    , CurRegistrationDetails
                    , registrationInfoForCancel.PCLRequests.ToList()
                    , null
                    //▼====: #024
                    , registrationInfoForCancel.InPatientBillingInvoices.ToList()
                    //▲====: #024
                    , null
                    , out Transaction
                    , out paymentInfo
                    , out paymentInfoList
                    , out error
                    , false, null, null, false, false, null, null, null, true);
                    //▲===== #020
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("MergerPatientRegistration => PayForRegistration_Cancel => ", ex.ToString());
                    //▼=====    Do Service đang Reference tới file eHCMS Language cũ (ehcms) nên không thấy những gì mới của aEMR nên set bằng chữ khi nào sửa được sẽ 
                    //          set lại cho chính xác (Do anh Tuấn nói không được đụng vào Reference.
                    throw new Exception(string.Format("{0}", eHCMSResources.Z3030_G1_KhongTheTraTienDichVuHuySapNhap));
                    //throw new Exception(string.Format("{0}.", "Không thể hoàn tiền dịch vụ đã huỷ của ca ngoại trú xáp nhập."));
                }

                //▼====: #024 Chuyển phiếu xuất thuốc cản quang từ ngoại trú vào nội trú
                try
                {
                    PatientProvider.Instance.AddBillingInvoice_InPt(registrationInfo, StaffID);
                    AxLogger.Instance.LogInfo("MergerPatientRegistration => AddBillingInvoice_Insert. Status: Success");
                }
                catch (Exception ex)
                {
                    AxLogger.Instance.LogInfo("MergerPatientRegistration => AddBillingInvoice_Insert. Status: Failed => ", ex.ToString());
                }
                //▲====: #024
                //Huỷ đăng ký sau khi hoàn tiền dịch vụ
                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfoForCancel);
                registrationInfoForCancel.RegCancelStaffID = StaffID;
                registrationInfo.RegistrationStatus = AllLookupValues.RegistrationStatus.REFUND;
                paymentProcessor.CancelRegistration(registrationInfoForCancel);

                AxLogger.Instance.LogInfo("End MergerPatientRegistration.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of MergerPatientRegistration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddInPatientAdmission, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        public virtual bool AddPCLRequestsForInPt(PatientRegistration PtRegistration, long StaffID, long ReqFromDeptLocID, long ReqFromDeptID
            , PatientPCLRequest pclRequest, PatientPCLRequest deletedPclRequest, bool IsNotCheckInvalid, DateTime modifiedDate)
        {
            List<PatientPCLRequest> newPclList = null;
            //▼===== TTM và PVT: Preview code 20200624 11:22AM: Khúc code này sai và không cần thiết.
            //          1. lstPCLReqExists => Không bao giờ có giá trị do trước khi vào hàm đã set toàn bộ PCLReqItemID về 0.
            //          2. Do lstPCLReqExists không có giá trị, PclRequest cũng không thể nào có inPatientBillingInvID và V_PCLRequestStatus luôn = Open
            //              => If không bao giờ vào. If trong else cũng không có giá trị để so sánh vì lstPCLReqExists count = 0. Nên luôn luôn foreach
            //                 để add vào biến lstDetail_CreateNew => Mà add như vậy không nhất thiết phải để khúc code này lại chuyển thành code dòng 6348.
            //          3. GetSequenceNoForPclRequestDetailsList => Chưa tách phiếu vào đây luôn lỗi.
            //List<PatientPCLRequestDetail> lstDetail_ReUseServiceSeqNum = new List<PatientPCLRequestDetail>();
            //List<PatientPCLRequestDetail> lstDetail_CreateNew = new List<PatientPCLRequestDetail>();
            //if (pclRequest != null)
            //{

            //List<PatientPCLRequestDetail> lstNotYetSeqNum = (from c in pclRequest.PatientPCLRequestIndicators
            //                                                 where c.PCLReqItemID <= 0 && c.ServiceSeqNum <= 0 && c.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.HOAN_TAT
            //                                                 select c).ToList();
            //List<PatientPCLRequestDetail> lstPCLReqExists = (from c in pclRequest.PatientPCLRequestIndicators
            //                                                 where c.PCLReqItemID > 0
            //                                                 select c).ToList();
            // HPT 09/09/2016: Nếu pclRequest đã thực hiện hoặc đã thanh toán thì dịch vụ mới thêm vào phải đưa vào phiếu mới
            // Nếu phiếu chưa thực hiện và chưa thanh toán thì kiểm tra xem dịch vụ mới thêm vào có chung phòng thực hiện hay không? Nếu có thì add vô thêm, nếu không thì tạo phiếu mới
            //if (pclRequest.InPatientBillingInvID > 0 || pclRequest.V_PCLRequestStatus != AllLookupValues.V_PCLRequestStatus.OPEN
            //    || lstPCLReqExists.Where(x => x.DeptLocation != null && x.DeptLocation.DeptLocationID > 0).GroupBy(x => x.DeptLocation.DeptLocationID).Count() != 1)

            //{
            //    lstDetail_CreateNew = new List<PatientPCLRequestDetail>(lstNotYetSeqNum);
            //}
            //else
            //{
            //    long OldDeptLocID = lstPCLReqExists.FirstOrDefault().DeptLocation.DeptLocationID;
            //    foreach (var detail in lstNotYetSeqNum)
            //    {
            //        if (detail.DeptLocation.DeptLocationID == OldDeptLocID
            //            && detail.PCLExamType.HITTypeID == lstPCLReqExists.FirstOrDefault().PCLExamType.HITTypeID
            //            && detail.PCLExamType.IsExternalExam == lstPCLReqExists.FirstOrDefault().PCLExamType.IsExternalExam
            //            && (detail.PCLExamType.IsExternalExam == false || ((detail.PCLExamType.IsExternalExam.GetValueOrDefault() == true
            //                                                                && lstPCLReqExists.FirstOrDefault().PCLExamType.HosIDofExternalExam == detail.PCLExamType.HosIDofExternalExam))))
            //        {
            //            detail.PatientPCLReqID = pclRequest.PatientPCLReqID;

            //            DeptLocation DL = new DeptLocation();
            //            DL.DeptLocationID = OldDeptLocID;
            //            detail.DeptLocation = DL;
            //            detail.CreatedDate = DateTime.Now;
            //            detail.ServiceSeqNum = pclRequest.ServiceSeqNum;
            //            detail.ServiceSeqNumType = pclRequest.ServiceSeqNumType;
            //            detail.DoctorStaff = pclRequest.DoctorStaff;
            //            detail.MedicalInstructionDate = pclRequest.MedicalInstructionDate;

            //            lstDetail_ReUseServiceSeqNum.Add(detail);
            //        }
            //        else
            //        {
            //            lstDetail_CreateNew.Add(detail);
            //        }
            //    }
            //}
            //lstDetail_CreateNew = new List<PatientPCLRequestDetail>(lstNotYetSeqNum);
            //PtRegistration.lstDetail_ReUseServiceSeqNum = lstDetail_ReUseServiceSeqNum.ToObservableCollection();
            //GetSequenceNoForPclRequestDetailsList(lstDetail_CreateNew);
            //newPclList = SplitVote(lstDetail_CreateNew);
            //}
            //▲===== 

            if (pclRequest.PatientPCLRequestIndicators.Count > 0)
            {
                newPclList = SplitVote(pclRequest.PatientPCLRequestIndicators.ToList());
                //▼===== 20200626 TTM:  Chuyển vào trong foreach vì nếu như trong trường hợp newPclList không có giá trị thì sẽ không foreach (newPclList = null
                //                      khi foreach thì sẽ ra lỗi).
                foreach (var item in newPclList)
                {
                    if (item.PatientPCLRequestIndicators != null && item.PatientPCLRequestIndicators.Count > 0)
                    {
                        item.MedicalInstructionDate = item.PatientPCLRequestIndicators.FirstOrDefault().MedicalInstructionDate;
                    }
                    if (item.DoctorStaffID.GetValueOrDefault() == 0)
                    {
                        item.DoctorStaffID = pclRequest.DoctorStaffID;
                    }
                }
                //▲=====
            }

            List<long> newPclRequestIdList = null;
            long medicalRecordID;
            long? medicalFileID;
            CreatePatientMedialRecordIfNotExists(PtRegistration.Patient.PatientID, PtRegistration.ExamDate, out medicalRecordID, out medicalFileID);
            return PatientProvider.Instance.SavePCLRequestsForInPt(PtRegistration.PtRegistrationID, StaffID, medicalRecordID, ReqFromDeptLocID, ReqFromDeptID, newPclList, deletedPclRequest,
                                                       null, (int)AllLookupValues.RegistrationType.NOI_TRU, IsNotCheckInvalid, out newPclRequestIdList, true);
        }
        public void CreatePatientMedialRecordIfNotExists(long patientID, DateTime medicalRecordDate, out long patientMedicalRecordID, out long? medicalFileID)
        {
            patientMedicalRecordID = -1;
            long medicalRecordID;

            if (!PatientProvider.Instance.CheckIfMedicalRecordExists(patientID, out medicalRecordID, out medicalFileID))
            {
                var medicalRecord = new PatientMedicalRecord
                {
                    PatientID = patientID,
                    NationalMedicalCode = "MedicalCode",
                    CreatedDate = medicalRecordDate == DateTime.MinValue ? DateTime.Now : medicalRecordDate
                };

                var bCreateMROK = PatientProvider.Instance.AddNewPatientMedicalRecord(medicalRecord, out medicalRecordID);
                if (!bCreateMROK)
                {
                    throw new Exception(string.Format("{0}.", eHCMSResources.Z1780_G1_CannotCreatePatient));
                }
            }
            patientMedicalRecordID = medicalRecordID;
        }
        public static List<PatientPCLRequest> SplitVote(List<PatientPCLRequestDetail> lstDetail_CreateNew)
        {
            var requests = new List<PatientPCLRequest>(); //new Dictionary<long ,PatientPCLRequest>();
            Dictionary<long, PCLExamType> MAPPCLExamTypeDeptLoc = null;

            MAPPCLExamTypeDeptLoc = DataProviderBase.MAPPCLExamTypeDeptLoc;
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
                    if (item.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Imaging || !string.IsNullOrEmpty(reqDetails.PCLExamType.TemplateFileName))
                    {
                        continue;
                    }
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
                    if (reqDetails.NumberOfTest.HasValue && reqDetails.NumberOfTest > 1
                        && (reqDetails.PCLExamType == null || reqDetails.PCLExamType.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.GeneralSugery))
                    {
                        byte NumberOfTest = reqDetails.NumberOfTest.Value;
                        for (int i = 0; i < NumberOfTest; i++)
                        {
                            reqDetails.NumberOfTest = 1;
                            requests.Add(GetNewContentSplitRequest(reqDetails));
                        }
                    }
                    else
                    {
                        requests.Add(GetNewContentSplitRequest(reqDetails));
                    }
                }
            }
            return requests;
        }
        private static PatientPCLRequest GetNewContentSplitRequest(PatientPCLRequestDetail reqDetails)
        {
            var newRequest = new PatientPCLRequest();
            newRequest.PatientPCLReqID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.PatientPCLReqID : 0;
            newRequest.StaffID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.StaffID : 0;
            newRequest.DoctorStaffID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DoctorStaffID : 0;
            newRequest.Diagnosis = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.Diagnosis : "";
            newRequest.DoctorComments = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DoctorComments : "";
            newRequest.DeptID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.DeptID : 0;
            newRequest.PCLDeptLocID = reqDetails.DeptLocation != null ? reqDetails.DeptLocation.DeptLocationID : 0;
            newRequest.ReqFromDeptID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ReqFromDeptID : 0;
            newRequest.ReqFromDeptLocID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ReqFromDeptLocID : 0;
            newRequest.RequestedDoctorName = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.RequestedDoctorName : "";
            newRequest.ServiceRecID = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ServiceRecID : 0;
            newRequest.V_PCLRequestStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatus : 0;
            newRequest.V_PCLRequestType = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestType : 0;
            newRequest.V_PCLRequestStatusName = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.V_PCLRequestStatusName : "";
            newRequest.ExamRegStatus = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ExamRegStatus : 0;
            newRequest.RecordState = RecordState.DETACHED;
            newRequest.EntityState = EntityState.NEW;
            newRequest.CreatedDate = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.CreatedDate : DateTime.Now;
            newRequest.ExamDate = reqDetails.PatientPCLRequest != null ? reqDetails.PatientPCLRequest.ExamDate : DateTime.Now;
            newRequest.PatientPCLRequestIndicators = new List<PatientPCLRequestDetail>().ToObservableCollection();
            newRequest.PatientPCLRequestIndicators.Add(reqDetails);
            newRequest.V_PCLMainCategory = reqDetails.PCLExamType.V_PCLMainCategory;
            newRequest.IsExternalExam = reqDetails.PCLExamType.IsExternalExam;
            newRequest.AgencyID = reqDetails.PCLExamType.HosIDofExternalExam;
            newRequest.PCLRequestNumID = new ServiceSequenceNumberProvider().GetCode_PCLRequest_InPt(newRequest.V_PCLMainCategory);
            return newRequest;
        }
        public static bool SplitVoteCondition(PatientPCLRequestDetail reqDetails, PatientPCLRequest item)
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
                    //if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.V_PCLMainCategory == reqDetails.PCLExamType.V_PCLMainCategory
                    //    && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HITTypeID == reqDetails.PCLExamType.HITTypeID
                    //    && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == reqDetails.PCLExamType.IsExternalExam)
                    //20200601 TBL: BM 0038204: Thêm điều kiện nếu khác BS chỉ định hoặc khác ngày y lệnh thì tách phiếu
                    if (item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.V_PCLMainCategory == reqDetails.PCLExamType.V_PCLMainCategory
                        && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.HITTypeID == reqDetails.PCLExamType.HITTypeID
                        && item.PatientPCLRequestIndicators.FirstOrDefault().PCLExamType.IsExternalExam == reqDetails.PCLExamType.IsExternalExam
                        && ((item.PatientPCLRequestIndicators.FirstOrDefault().DoctorStaff != null && reqDetails.DoctorStaff != null
                        && item.PatientPCLRequestIndicators.FirstOrDefault().DoctorStaff.StaffID == reqDetails.DoctorStaff.StaffID)
                        || item.PatientPCLRequestIndicators.FirstOrDefault().DoctorStaff == null || reqDetails.DoctorStaff == null)
                        && item.PatientPCLRequestIndicators.FirstOrDefault().MedicalInstructionDate == reqDetails.MedicalInstructionDate)
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
        private void GetSequenceNoForPclRequestDetailsList(IEnumerable<PatientPCLRequestDetail> pclRequestDetails, bool bUseExistingLocation = false)
        {
            try
            {
                var sb = new StringBuilder();
                //var allPclRequestItems = new List<PatientPCLRequestDetail>().ToObservableCollection();
                ObservableCollection<PatientPCLRequestDetail> allPclRequestItems = new ObservableCollection<PatientPCLRequestDetail>();
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

                                // TxD 11/11/2014: Added the following to be used when reassigning sequence number 
                                //                  and Location details are required by Cash Receipt
                                if (bUseExistingLocation && requestDetail.DeptLocation != null && requestDetail.DeptLocation.Location != null)
                                {
                                    var exLocation = new Location();
                                    exLocation.LID = requestDetail.DeptLocation.Location.LID;
                                    exLocation.LocationName = requestDetail.DeptLocation.Location.LocationName;
                                    exLocation.LocationDescription = requestDetail.DeptLocation.Location.LocationDescription;
                                    deptLoc.Location = exLocation;
                                }

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
                        throw new Exception(string.Format("{0}.", eHCMSResources.Z1836_G1_StrInIncorrectFormat));
                    }
                }
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("ERROR: " + ex.ToString());
            }
        }
        //▲===== #017

        //▼===== 20200622 TTM: Code phụ trợ cho hàm sáp nhập
        //public decimal CalcPatientPayment(PatientRegistration Registration
        //   , List<PatientRegistrationDetail> RegistrationDetails
        //   , PatientPCLRequest PclRequests
        //   , List<OutwardDrugInvoice> DrugInvoices)
        //{
        //    decimal payment = 0;
        //    if (Registration == null)
        //    {
        //        return payment;
        //    }
        //    if (RegistrationDetails != null)
        //    {
        //        foreach (var item in RegistrationDetails)
        //        {
        //            if (item.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
        //            {
        //                payment += CalPaymentForMedRegItem(item);
        //            }
        //        }
        //    }
        //    if (PclRequests != null && PclRequests.PatientPCLRequestIndicators != null)
        //    {
        //        foreach (var item in PclRequests.PatientPCLRequestIndicators)
        //        {
        //            if (item.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
        //            {
        //                payment += CalPaymentForMedRegItem(item);
        //            }
        //        }
        //    }
        //    if (DrugInvoices != null)
        //    {
        //        bool onlyRoundResultForOutward = Globals.AxServerSettings.PharmacyElements.OnlyRoundResultForOutward;
        //        if (DrugInvoices.Where(x => x.OutwardDrugs != null && x.PaidTime == null).SelectMany(x => x.OutwardDrugs).Any(x => x.PaidTime == null) && Registration.PtInsuranceBenefit.GetValueOrDefault(0) == 0)
        //        {
        //            payment = 0;
        //        }
        //        foreach (var invoice in DrugInvoices)
        //        {
        //            if (invoice.ReturnID.GetValueOrDefault(0) <= 0)//Phieu xuat.
        //            {
        //                if (invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE
        //                    || invoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.DRUGCOLLECTED)
        //                {
        //                    if (invoice.OutwardDrugs != null && invoice.PaidTime == null)
        //                    {

        //                        if (!onlyRoundResultForOutward)
        //                        {
        //                            foreach (var item in invoice.OutwardDrugs)
        //                            {
        //                                payment += item.TotalPatientPayment;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            decimal TotalHIPayment = 0;
        //                            decimal TotalInvoicePrice = 0;
        //                            foreach (var item in invoice.OutwardDrugs)
        //                            {
        //                                TotalHIPayment += item.TotalHIPayment;
        //                                TotalInvoicePrice += item.TotalInvoicePrice;
        //                            }
        //                            payment += Math.Round(TotalInvoicePrice - TotalHIPayment, MidpointRounding.AwayFromZero);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (invoice.OutwardDrugs != null
        //                    && invoice.PaidTime == null)
        //                {
        //                    if (!onlyRoundResultForOutward)
        //                    {
        //                        foreach (var item in invoice.OutwardDrugs)
        //                        {
        //                            payment -= item.TotalPatientPayment;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        decimal TotalHIPayment = 0;
        //                        decimal TotalInvoicePrice = 0;
        //                        foreach (var item in invoice.OutwardDrugs)
        //                        {
        //                            TotalHIPayment -= item.TotalHIPayment;
        //                            TotalInvoicePrice -= item.TotalInvoicePrice;
        //                        }
        //                        payment = Math.Round(TotalInvoicePrice - TotalHIPayment, MidpointRounding.AwayFromZero);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (Registration.InPatientBillingInvoices != null && Registration.InPatientBillingInvoices.Count > 0
        //        && Registration.InPatientBillingInvoices.Any(x => x.V_InPatientBillingInvStatus != AllLookupValues.V_InPatientBillingInvStatus.NGUNG_TRA_TIEN_LAI))
        //    {
        //        foreach (var aItem in Registration.InPatientBillingInvoices.Where(x => x.V_InPatientBillingInvStatus != AllLookupValues.V_InPatientBillingInvStatus.NGUNG_TRA_TIEN_LAI).ToList())
        //        {
        //            payment += CalPaymentForMedRegItem(aItem);
        //        }
        //    }
        //    return payment;
        //}
        //public decimal CalPaymentForMedRegItem(MedRegItemBase item)
        //{
        //    if (item.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
        //    {
        //        if (item.PaidTime != null)
        //        {
        //            //▼===== 20200630 TTM: Loại bỏ DiscountAmt vì DiscountAmt không cần phải tính vào khi chuyển tiền từ ngoại trú sang nội trú.
        //            //return item.TotalPatientPayment + item.DiscountAmt;
        //            return item.TotalPatientPayment;
        //            //▲===== 20200630
        //        }
        //    }
        //    return 0;
        //}
        //public decimal CalPaymentForMedRegItem(InPatientBillingInvoice item)
        //{
        //    if (item.V_InPatientBillingInvStatus != AllLookupValues.V_InPatientBillingInvStatus.NGUNG_TRA_TIEN_LAI)
        //    {
        //        if (item.PaidTime != null)
        //        {
        //            return item.TotalPatientPayment;
        //        }
        //        else if (item.RefundTime == null && item.RecordState == RecordState.DELETED)
        //        {
        //            return -item.TotalPatientPayment;
        //        }
        //    }
        //    return 0;
        //}

        public void RemovePaidRegItems(PatientRegistration CurReg
            , long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient,
                List<PatientRegistrationDetail> colPaidRegDetails,
                List<PatientPCLRequest> colPaidPclRequests,
                List<OutwardDrugInvoice> colPaidDrugInvoice,
                List<OutwardDrugClinicDeptInvoice> colPaidMedItemList,
                List<OutwardDrugClinicDeptInvoice> colPaidChemicalItem,
                bool IsFromMergeMethod = false)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start removing registered items.", CurrentUser);
                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(CurReg);
                paymentProcessor.InitNewTxd(CurReg, false);

                paymentProcessor.RemovePaidRegItems(StaffID, CollectorDeptLocID, Apply15HIPercent, CurReg, colPaidRegDetails, colPaidPclRequests,
                                                    colPaidDrugInvoice, colPaidMedItemList, colPaidChemicalItem, default(DateTime), IsFromMergeMethod);
                AxLogger.Instance.LogInfo("End of removing registered items.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of MergerPatientRegistration => RemovePaidRegItems. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PayForRegistration, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }

        public void PayForRegistration_Cancel(PatientRegistration regInfo, long StaffID, long CollectorDeptLocID, int? Apply15HIPercent, long registrationID, int FindPatient
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
            bool checkBeforePay = false,
            long? ConfirmHIStaffID = null,
            string OutputBalanceServicesXML = null,
            bool IsReported = false,
            bool IsUpdateHisID = false,
            long? HIID = null,
            double? PtInsuranceBenefit = null,
            PatientRegistration existingPtRegisInfo = null,
            bool IsNotCheckInvalid = false,
            bool IsRefundBilling = false, bool IsProcess = false)
        {

            try
            {
                Transaction = new PatientTransaction();
                paymentInfo = new PatientTransactionPayment();
                paymentInfoList = new List<PaymentAndReceipt>();
                error = V_RegistrationError.mNone;

                RegAndPaymentProcessorBase paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(regInfo);
                paymentProcessor.InitNewTxd(regInfo, false, 0, IsProcess);
                paymentProcessor.PayForRegistration(StaffID, CollectorDeptLocID, Apply15HIPercent, regInfo, paymentDetails, colPaidRegDetails, colPaidPclRequests, colPaidDrugInvoice, colBillingInvoices, out Transaction, out paymentInfo, out paymentInfoList, ConfirmHIStaffID, OutputBalanceServicesXML, IsReported, IsUpdateHisID
                    , HIID, PtInsuranceBenefit, IsNotCheckInvalid, IsRefundBilling, IsProcess);

                AxLogger.Instance.LogInfo("End of PayForRegistration_Cancel.", CurrentUser);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of PayForRegistration_Cancel. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_PayForRegistration, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲=====
        //▼===== #018
        public void CalcPatientRegistrationDetail(PatientRegistration RegInfo, PatientRegistration PtRegistration)
        {
            if (RegInfo.PatientRegistrationDetails != null && RegInfo.PatientRegistrationDetails.Count > 0)
            {
                //Set lại giá trị HIBenefit cho các dịch vụ trước khi tính đúng tính đủ cho các dịch vụ ngoại trú
                foreach (PatientRegistrationDetail detail in RegInfo.PatientRegistrationDetails)
                {
                    detail.HIBenefit = PtRegistration.PtInsuranceBenefit;
                }
                //Tính toán lại thông tin tiền bạc. Cờ tính full 100% dịch vụ khám sẽ bị chuyển sang false.
                RegInfo.CorrectRegistrationDetails_V2(Globals.AxServerSettings.HealthInsurances.SpecialRuleForHIConsultationApplied, null,
                    Globals.AxServerSettings.HealthInsurances.HIPercentOnDifDept, Globals.AxServerSettings.HealthInsurances.HiPolicyMinSalary,
                    Globals.AxServerSettings.PharmacyElements.OnlyRoundResultForOutward, Globals.AxServerSettings.CommonItems.AddingServicesPercent,
                    Globals.AxServerSettings.CommonItems.AddingHIServicesPercent, false,
                    Globals.AxServerSettings.HealthInsurances.PercentForEkip, Globals.AxServerSettings.HealthInsurances.PercentForOtherEkip);

                //Sau khi đã tính toán xong sẽ set giá trị IsCountHI và IsCountPatient
                foreach (PatientRegistrationDetail detail in RegInfo.PatientRegistrationDetails)
                {
                    detail.ReqFromDeptID = PtRegistration.AdmissionInfo.DeptID;
                    detail.IsCountPatient = true;
                    detail.IsCountHI = detail.TotalHIPayment > 0;
                }
            }
        }
        //▲===== #018

        public IList<PatientRegistration> SearchRegistration_ByServiceID(long MedServiceID, DateTime? FromDate, DateTime? ToDate, out List<DiagnosisTreatment> DiagnosisList)
        {
            try
            {
                DiagnosisList = new List<DiagnosisTreatment>();
                AxLogger.Instance.LogInfo("Start searching patient registration by ServiceID ...", CurrentUser);
                IList<PatientRegistration> retVal;
                retVal = PatientProvider.Instance.SearchRegistration_ByServiceID(MedServiceID, FromDate, ToDate, out DiagnosisList);
                AxLogger.Instance.LogInfo("End of searching patient registration by ServiceID. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of searching patient registration. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<Resources> GetResourcesForMedicalServices()
        {
            try
            {
                IList<Resources> retVal = PatientProvider.Instance.GetResourcesForMedicalServices();
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetResourcesForMedicalServices. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        public IList<RefMedicalServiceItem> GetMedicalServiceItemByHIRepResourceCode(string HIRepResourceCode)
        {
            try
            {
                IList<RefMedicalServiceItem> retVal = PatientProvider.Instance.GetMedicalServiceItemByHIRepResourceCode(HIRepResourceCode);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetMedicalServiceItemByHIRepResourceCode. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        public bool SaveSmallProcedureAutomatic(ObservableCollection<SmallProcedure> SmallProcedureList)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start SaveSmallProcedureAutomatic ...", CurrentUser);
                bool retVal = PatientProvider.Instance.SaveSmallProcedureAutomatic(SmallProcedureList);
                AxLogger.Instance.LogInfo("End of SaveSmallProcedureAutomatic. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of SaveSmallProcedureAutomaticn. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }

        //▼===== #019
        public bool ConfirmEmergencyOutPtByPtRegistrionID(long PtRegistrationID, long StaffID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start ConfirmEmergencyOutPtByPtRegistrionID ...", CurrentUser);
                bool retVal = PatientProvider.Instance.ConfirmEmergencyOutPtByPtRegistrionID(PtRegistrationID, StaffID, V_RegistrationType);
                AxLogger.Instance.LogInfo("End of ConfirmEmergencyOutPtByPtRegistrionID. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of ConfirmEmergencyOutPtByPtRegistrionID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲===== #019

        public bool ConfirmSpecialRegistrationByPtRegistrationID(PatientRegistration PatientRegistration, long StaffID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start Confirm Special Registration By PtRegistrationID ...", CurrentUser);
                bool retVal = PatientProvider.Instance.ConfirmSpecialRegistrationByPtRegistrationID(PatientRegistration, StaffID);
                AxLogger.Instance.LogInfo("End of Confirm Special Registration By PtRegistrationID. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of Confirm Special Registration By PtRegistrationID. Status: Failed.", CurrentUser);
                throw new Exception(ex.Message);
            }
        }

        //▼====: #021
        public bool UpdatePclRequestTransferToPAC(ObservableCollection<PatientPCLRequest> PatientPCLRequestList, long V_PCLRequestType)
        {
            try
            {
                return PatientProvider.Instance.UpdatePclRequestTransferToPAC(PatientPCLRequestList, V_PCLRequestType);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of UpdatePclRequestTransferToPAC. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_ApplyHiToInPatientRegistration);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #021
        //▼====: #023
        public List<PatientRegistrationDetail> GetListMedServiceInSurgeryDept(long PtRegistrationID)
        {
            AxLogger.Instance.LogInfo("Start GetListMedServiceInSurgeryDept ...", CurrentUser);
            try
            {
                return PatientProvider.Instance.GetListMedServiceInSurgeryDept(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetListMedServiceInSurgeryDept. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_ApplyHiToInPatientRegistration);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #023
        public bool CreateRequestDrugInwardClinicDept_ByPCLRequest(PatientPCLRequest PatientPCLRequest, out long ReqDrugInClinicDeptID)
        {
            try
            {
                return PatientProvider.Instance.CreateRequestDrugInwardClinicDept_ByPCLRequest(PatientPCLRequest,out ReqDrugInClinicDeptID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of CreateRequestDrugInwardClinicDept_ByPCLRequest. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_ApplyHiToInPatientRegistration);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▼====: #025
        public bool AddUpdateDoctorContactPatientTime(DoctorContactPatientTime doctorContactPatientTime)
        {
            try
            {
                return PatientProvider.Instance.AddUpdateDoctorContactPatientTime(doctorContactPatientTime);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of AddDoctorContactPatientTime. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_ApplyHiToInPatientRegistration);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▲====: #025
        //▼====: #027
        //▼==== #033
        public bool GetLastIDC10MainTreatmentProgram_ByPtRegID(long PtRegistrationID, long PtRegDetailID, out long LastOutpatientTreatmentTypeID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetLastIDC10MainTreatmentProgram_ByPtRegID", CurrentUser);
                bool retVal = PatientProvider.Instance.GetLastIDC10MainTreatmentProgram_ByPtRegID(PtRegistrationID, PtRegDetailID, out LastOutpatientTreatmentTypeID);
                AxLogger.Instance.LogInfo("End of GetLastIDC10MainTreatmentProgram_ByPtRegID. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetLastIDC10MainTreatmentProgram_ByPtRegID. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #033
        public List<OutpatientTreatmentType> GetAllOutpatientTreatmentType()
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetAllOutpatientTreatmentType", CurrentUser);
                List<OutpatientTreatmentType> retVal = PatientProvider.Instance.GetAllOutpatientTreatmentType();
                AxLogger.Instance.LogInfo("End of GetAllOutpatientTreatmentType. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of GetAllOutpatientTreatmentType. Status: Failed.", CurrentUser);
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.PATIENT_REGIS_SearchRegistrations);
                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲====: #027
        public bool RefundInPatientCost(PatientRegistration PtRegistration, long StaffID)
        {
            try
            {
                return PatientProvider.Instance.RefundInPatientCost(PtRegistration, StaffID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("MergerPatientRegistration => UpdateAdmissionDate => ", ex.ToString());
                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_AddInPatientAdmission, CurrentUser);
                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }
        }
        //▼==== #028
        public List<PrescriptionIssueHistory> GetPrescriptionIssueHistory_ByPtRegDetailID(long PtRegDetailID, long V_RegistrationType)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start loading Registration Info.", CurrentUser);
                List<PrescriptionIssueHistory> retVal = PatientProvider.Instance.GetPrescriptionIssueHistory_ByPtRegDetailID(PtRegDetailID, V_RegistrationType);
                AxLogger.Instance.LogInfo("End of GetPrescriptionIssueHistory_ByPtRegDetailID. Status: Success", CurrentUser);
                return retVal;
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End of loading GetPrescriptionIssueHistory_ByPtRegDetailID. Status: Failed.", CurrentUser);

                AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_GetRegistration, CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(eHCMSResources.Z1684_G1_WCFError));
            }
        }
        //▲==== #028
        //▼====: #029
        public bool UpdateIsConfirmedEmergencyPatient(long PtRegistrationID, bool IsConfirmedEmergencyPatient)
        {
            try
            {
                return PatientProvider.Instance.UpdateIsConfirmedEmergencyPatient(PtRegistrationID, IsConfirmedEmergencyPatient);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End UpdateIsConfirmedEmergencyPatient. Status: Failed.", CurrentUser);
                throw new Exception(ex.Message);
            }
        }
        //▲====: #029 
        //▼====: #030
        public PatientCardDetail GetCardDetailByPatientID(long PatientID)
        {
            try
            {
                return PatientProvider.Instance.GetCardDetailByPatientID(PatientID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End GetCardDetailByPatientID. Status: Failed.", CurrentUser);
                throw new Exception(ex.Message);
            }
        }

        public bool SaveCardDetail(PatientCardDetail Obj, bool IsExtendCard)
        {
            try
            {
                return PatientProvider.Instance.SaveCardDetail(Obj, IsExtendCard);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End SaveCardDetail. Status: Failed.", CurrentUser);
                throw new Exception(ex.Message);
            }
        }
        //▲====: #030
        //▼==== #032
        public bool CheckBeforeMergerPatientRegistration(long registrationID, out string errorMessages, out string confirmMessages)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start CheckBeforeMergerPatientRegistration.", CurrentUser);
                return PatientProvider.Instance.CheckBeforeMergerPatientRegistration(registrationID, out errorMessages, out confirmMessages);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End CheckBeforeMergerPatientRegistration. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, "Lõi sáp nhập", CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }
        //▲==== #032
        public DeathCheckRecord GetDeathCheckRecordByPtRegID(long PtRegistrationID)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start GetDeathCheckRecordByPtRegID.", CurrentUser);
                return PatientProvider.Instance.GetDeathCheckRecordByPtRegID(PtRegistrationID);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End GetDeathCheckRecordByPtRegID. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, "Lõi sáp nhập", CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }
        public bool SaveDeathCheckRecord(DeathCheckRecord CurDeathCheckRecord)
        {
            try
            {
                AxLogger.Instance.LogInfo("Start SaveDeathCheckRecord.", CurrentUser);
                return PatientProvider.Instance.SaveDeathCheckRecord(CurDeathCheckRecord);
            }
            catch (Exception ex)
            {
                AxLogger.Instance.LogInfo("End SaveDeathCheckRecord. Status: Failed.", CurrentUser);

                var axErr = AxException.CatchExceptionAndLogMessage(ex, "Lõi sáp nhập", CurrentUser);

                throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            }

        }
    }
}
