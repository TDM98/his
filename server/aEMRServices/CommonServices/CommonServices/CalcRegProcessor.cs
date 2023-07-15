using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DataEntities;
using eHCMS.DAL;
using eHCMS.Services.Core;
using System.Data.Common;
using System;
using AxLogging;
using ErrorLibrary;

namespace CommonServices
{
    public class CalcRegProcessor
    {
        static private CalcRegProcessor _instance = null;

        static public CalcRegProcessor Instance
        {
            get
            {
                lock (typeof(CalcRegProcessor))
                {
                    if (_instance == null)
                    {
                        _instance = new CalcRegProcessor();
                    }
                }
                return _instance;
            }
        }
        public void CalcRegistration(PatientRegistration currentRegistrationInfo)
        {
            IEnumerable<IInvoiceItem> services = null;
            IEnumerable<IInvoiceItem> medItems = null;
            IEnumerable<IInvoiceItem> drugItems = null; //Tam thoi gan null

            List<IInvoiceItem> allItems = new List<IInvoiceItem>();

            //Add danh sach service.
            if (currentRegistrationInfo.PatientRegistrationDetails != null)
            {
                services = (IEnumerable<IInvoiceItem>)currentRegistrationInfo.PatientRegistrationDetails.Where(item => { return item.RecordState != RecordState.DELETED; });
            }

            if (services != null)
            {
                allItems.AddRange(services);
            }

            if(currentRegistrationInfo.PCLRequests != null)
            {
                foreach (PatientPCLRequest pcldetails in currentRegistrationInfo.PCLRequests)
                {
                    if (pcldetails.PatientPCLRequestIndicators != null)
                    {
                        allItems.AddRange((IEnumerable<IInvoiceItem>)pcldetails.PatientPCLRequestIndicators.Where(item => { return item.RecordState != RecordState.DELETED; }));
                    }
                }
            }
            
            if (currentRegistrationInfo.DrugInvoices != null)
            {
                foreach (OutwardDrugInvoice invoice in currentRegistrationInfo.DrugInvoices)
                {
                    if (invoice.OutwardDrugs != null)
                    {
                        allItems.AddRange((IEnumerable<IInvoiceItem>)invoice.OutwardDrugs); 
                    }
                }
            }

            //Tinh thuoc, y cu, hoa chat cho benh nhan noi tru.
            if (currentRegistrationInfo.InPatientInvoices != null)
            {
                foreach (OutwardDrugClinicDeptInvoice invoice in currentRegistrationInfo.InPatientInvoices)
                {
                    if (invoice.OutwardDrugClinicDepts != null)
                    {
                        allItems.AddRange((IEnumerable<IInvoiceItem>)invoice.OutwardDrugClinicDepts);
                    }
                }
            }

            if (medItems !=null)
            {
                allItems.AddRange(medItems);
            }
            

            CalcInvoiceItems(allItems, currentRegistrationInfo, currentRegistrationInfo.HealthInsurance);
            currentRegistrationInfo.PayableSum = CalcPayableSum(allItems);
            //Neu benh nhan con no thi tinh luon
            if (currentRegistrationInfo != null && currentRegistrationInfo.PatientTransaction != null && currentRegistrationInfo.PatientTransaction.PatientPayments != null)
            {
                currentRegistrationInfo.PayableSum.TotalPatientPaid = currentRegistrationInfo.PatientTransaction.PatientPayments.Sum(payment => { return payment.PayAmount; });
            }
            else
            {
                currentRegistrationInfo.PayableSum.TotalPatientPaid = 0;
            }
            currentRegistrationInfo.PayableSum.TotalPatientRemainingOwed = currentRegistrationInfo.PayableSum.TotalPatientPayment - currentRegistrationInfo.PayableSum.TotalPatientPaid;
        }

        /// <summary>
        /// Tính lại giá tiền cho tất cả các item trong collection colInvoiceItems
        /// </summary>
        /// <param name="colInvoiceItems"></param>
        /// <param name="hi"></param>
        private void CalcInvoiceItems(IList<IInvoiceItem> colInvoiceItems, PatientRegistration CurrentRegistrationInfo, HealthInsurance hi = null)
        {
            //Chỉ tính tiền những dịch vụ chưa bị xóa hoặc trả tiền lại

            // NOTES: Hiện tại chưa tính đến một số trường hợp:
            // - CHỈ TÍNH TIỀN MỘT LẦN ĐỐI VỚI DỊCH VỤ KHÁM BỆNH
            // - CẬP NHẬT LẠI TRẠNG THÁI CỦA TỪNG ITEM TRONG COLLECTION colInvoiceItems

            bool bConsiderAsCrossRegion = false;

            double HiBenefit = CalcPatientHiBenefit(CurrentRegistrationInfo, hi, out bConsiderAsCrossRegion);
            CurrentRegistrationInfo.PtInsuranceBenefit = HiBenefit;
            decimal hiAllowedPriceTemp = 0;
            //decimal totalHIAccept = 0;
            ////Tính tiền cho collection này
            //bool bHI = false;
            //decimal minPay = ConfigValues.HIPolicyMinSalary * (decimal)ConfigValues.HIPolicyPercentageOnPayable;

            //Tính tiền cho mỗi item trong list colInvoiceItems.
            // NOTES:
            // -Chỉ có trường hợp đúng tuyến mới hưởng đầy đủ quyền lợi.
            // -Trái tuyến chỉ được hưởng 70% (giá trị này lưu trong .config file) và nếu tổng số tiền bảo hiểm quy định
            // dù có < 15% tháng lương tối thiểu thì bệnh nhân vẫn phải tính bình thường.

            // -Trong trường hợp đúng tuyến, nếu tổng số tiền bảo hiểm quy định <15% tháng lương tối thiểu thì BH trả hết
            decimal totalHIAllowedPrice = 0;
            //Tinh gia tien cho cac dich vu.
            foreach (IInvoiceItem item in colInvoiceItems)
            {
                if (item.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                    && item.RecordState != RecordState.DELETED)
                {
                    GetItemPrice(item, CurrentRegistrationInfo, HiBenefit);

                    totalHIAllowedPrice += item.HIAllowedPrice.Value * item.Qty;

                    GetItemTotalPrice(item, CurrentRegistrationInfo, HiBenefit); 
                }
            }

            //Trường hợp không có bảo hiểm, hoặc có bảo hiểm nhưng trái tuyến
            if (HiBenefit <= 0 || bConsiderAsCrossRegion)
            {
                return;
            }

            //Chỉ có trường hợp cùng tuyến, hoặc xem như cùng tuyến (cấp cứu, có giấy chuyển viện) mới được hưởng trường hợp này
            //Xem lại tổng số tiền bảo hiểm quy định
            decimal minPay = ConfigValues.HIPolicyMinSalary * (decimal)ConfigValues.HIPolicyPercentageOnPayable;
            if (totalHIAllowedPrice < minPay)
            {
                foreach (IInvoiceItem item in colInvoiceItems)
                {
                    if (item.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                    && item.RecordState != RecordState.DELETED)
                    {
                        //Check if this item is a HI service.
                        if (item.HIAllowedPrice.HasValue && item.HIAllowedPrice.Value > 0)
                        {
                            item.HIPayment = item.HIAllowedPrice.Value;
                            item.PatientCoPayment = 0;
                            item.PatientPayment = item.PriceDifference;
                        }
                        GetItemTotalPrice(item, CurrentRegistrationInfo, HiBenefit); 
                    }
                }
            }
        }


        #region Tính tiền cho các dịch vụ (các dịch vụ khám chữa bệnh, thuốc, y cụ)
        /// <summary>
        /// Tinh lai bao hiem cho dang ky nay.
        /// </summary>
        /// <param name="CurrentRegistrationInfo"></param>
        /// <param name="hPatientHiProfile"></param>
        /// <param name="IsConsideredAsCrossRegion"></param>
        /// <returns></returns>
        public double CalcPatientHiBenefit(PatientRegistration CurrentRegistrationInfo, out bool IsConsideredAsCrossRegion)
        {
            double HiBenefit = 0.0;

            IsConsideredAsCrossRegion = false;
            if (CurrentRegistrationInfo.PatientClassification.PatientClassID == (long)PatientType.INSUARED_PATIENT)
            {
                HiBenefit = CalcPatientHiBenefit(CurrentRegistrationInfo, CurrentRegistrationInfo.HealthInsurance, out IsConsideredAsCrossRegion);
            }
            return HiBenefit;
        }
        private string[] _CrossRegionCodeAcceptedList = new string[] { "89020", "89021", "89022", "89023", "89024", "89025", "89026", "89027", "89028", "89029", "89030"
                                                                        ,"89082","89045","89091","89090"};
        /// <summary>
        /// This function is to determine the HI benefit percentage for a given Policy
        /// Please NOTE that the Rule of TotalPayable < 15% of basic monthly wage should be treated as an exception. 
        /// </summary>
        /// <param name="hPatientHiProfile"></param>
        /// <returns></returns>
        private double CalcPatientHiBenefit(PatientRegistration CurrentRegistrationInfo, HealthInsurance hPatientHiProfile, out bool IsConsideredAsCrossRegion)
        {
            double HiBenefit = 0.0;

            IsConsideredAsCrossRegion = false;
            // Do all the calculations required to get the benefit percentage 
            // for Patient HI
            // NOTE: Remember to take into consideration the following rules that affect Patient benefit:
            //       Use designed flowchart
            //     1. Origin Registration (Kham Chua Benh Ban Dau KCBBD)  
            //     2. Cross Region (Trai Tuyen)
            //     3. Transfer Reference (Chuyen Vien) Use CurrentRegistrationInfo to find out if this patient has a valid Transfer Reference
            //     

            if (hPatientHiProfile == null)
            {
                return HiBenefit;
            }

            if (hPatientHiProfile.ActiveHealthInsuranceHIPatientBenefit != null)
            {
                return hPatientHiProfile.ActiveHealthInsuranceHIPatientBenefit.Benefit;
            }

            if (hPatientHiProfile.InsuranceBenefit == null)
            {
                return HiBenefit;
            }

            HiBenefit = hPatientHiProfile.InsuranceBenefit.RebatePercentage;

            if (HiBenefit <= 0.0)
            {
                HiBenefit = 0.0;
                return HiBenefit;
            }
            if (CurrentRegistrationInfo.EmergencyRecord != null)
            {
                return HiBenefit;
            }
            //Nếu có giấy chuyển viện hoặc có bảo hiểm thì xem như đúng tuyến.
            if (CurrentRegistrationInfo.PaperReferal != null && (_CrossRegionCodeAcceptedList.Contains(hPatientHiProfile.RegistrationCode.Trim().ToLower())))
            {
                //IsConsideredAsCrossRegion = false;
            }
            else
            {
                //Kiểm tra nơi khám chữa bệnh ban đầu.
                if (hPatientHiProfile.RegistrationCode.Trim().ToLower() != ConfigValues.HospitalCode.Trim().ToLower())
                {
                    IsConsideredAsCrossRegion = true;
                    HiBenefit = ConfigValues.RebatePercentageLevel_1;
                }
            }

            return HiBenefit;
        }

        /// <summary>
        /// Given Patient Registration details and Benefit determine the price of a chargeable item
        /// </summary>
        /// <param name="CurrentRegistrationInfo"></param>
        /// <param name="HiBenefit"></param>
        /// <returns></returns>
        private bool GetItemPrice(IInvoiceItem invoiceItem, PatientRegistration CurrentRegistrationInfo, double HiBenefit)
        {
            bool valid = true;
            decimal calcItemPrice = 0;

            if (invoiceItem.ID > 0)
            {
                if (invoiceItem.HIBenefit.HasValue && invoiceItem.HIBenefit > 0.0)
                {
                    HiBenefit = invoiceItem.HIBenefit.Value;
                } 
            }
            else
            {
                if (invoiceItem.ChargeableItem != null)
                {
                    invoiceItem.HIBenefit = HiBenefit;
                    invoiceItem.HIAllowedPrice = invoiceItem.ChargeableItem.HIAllowedPrice;

                    //Kiểm tra nếu có sử dụng bảo hiểm + dịch vụ có bảo hiểm thì lấy giá của dịch vụ bằng với giá bảo hiểm.
                    //Nếu không thì lấy giá bình thường.
                    if (HiBenefit > 0 && invoiceItem.ChargeableItem.HIPatientPrice > 0)
                    {
                        invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.HIPatientPrice;
                    }
                    else
                    {
                        invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.NormalPrice;
                    }
                } 
            } 
            if (HiBenefit <= 0.0 || !invoiceItem.HIAllowedPrice.HasValue)
            {
                invoiceItem.HIAllowedPrice = 0;
            }
            invoiceItem.PriceDifference = invoiceItem.InvoicePrice - invoiceItem.HIAllowedPrice.Value;
            invoiceItem.HIPayment = invoiceItem.HIAllowedPrice.Value * (decimal)HiBenefit;
            invoiceItem.PatientCoPayment = invoiceItem.HIAllowedPrice.Value - invoiceItem.HIPayment;
            invoiceItem.PatientPayment = invoiceItem.InvoicePrice - invoiceItem.HIPayment;

            if(invoiceItem.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
            {
                invoiceItem.PriceDifference = -invoiceItem.PriceDifference;
                invoiceItem.HIPayment = -invoiceItem.HIPayment;
                invoiceItem.PatientCoPayment = -invoiceItem.PatientCoPayment;
                invoiceItem.PatientPayment = -invoiceItem.PatientPayment;
                invoiceItem.InvoicePrice = -invoiceItem.InvoicePrice;
            }
            return valid;
        }

        /// <summary>
        /// Tinh tong so tien cho moi invoiceitem
        /// </summary>
        /// <param name="invoiceItem"></param>
        /// <param name="CurrentRegistrationInfo"></param>
        /// <param name="HiBenefit"></param>
        /// <returns></returns>
        private bool GetItemTotalPrice(IInvoiceItem invoiceItem, PatientRegistration CurrentRegistrationInfo, double HiBenefit)
        {
            bool valid = true;

            //Tính tổng tiền cho mỗi InvoiceItem.
            // Chưa tính trường hợp: nhiều dịch vụ KHÁM BỆNH chỉ tính 1 lần
            // Ở đây tạm thời sử dụng cái gì tính cái đó.

            invoiceItem.TotalCoPayment = invoiceItem.PatientCoPayment * invoiceItem.Qty;
            invoiceItem.TotalHIPayment = invoiceItem.HIPayment * invoiceItem.Qty;
            invoiceItem.TotalInvoicePrice = invoiceItem.InvoicePrice * invoiceItem.Qty;
            invoiceItem.TotalPatientPayment = invoiceItem.PatientPayment * invoiceItem.Qty;
            invoiceItem.TotalPriceDifference = invoiceItem.PriceDifference * invoiceItem.Qty;

            return valid;
        }

        private PayableSum CalcPayableSum(IList<IInvoiceItem> colInvoiceItems)
        {
            PayableSum sum = new PayableSum();
            foreach (IInvoiceItem item in colInvoiceItems)
            {
                sum.TotalCoPayment += item.TotalCoPayment;
                sum.TotalHIPayment += item.TotalHIPayment;
                sum.TotalPrice += item.TotalInvoicePrice;
                sum.TotalPatientPayment += item.TotalPatientPayment;
                sum.TotalPriceDifference += item.TotalPriceDifference;
            }
            return sum;
        }


        public void CalcDrugs(PatientRegistration reg, IList<OutwardDrug> colDrugs, out IList<OutwardDrug> colCalculatedDrugs)
        {
            colCalculatedDrugs = null;
        }
        #endregion

    }
}