using eHCMS.Configurations;
using System;
using System.Linq;

namespace DataEntities
{
    public interface IInvoiceItem : IDBRecordState, ITotalPrice
    {
        bool IsCountHI { get; set; }

        bool HiApplied { get; set; }

        long ID { get; set; }
        decimal InvoicePrice
        {
            get;
            set;
        }
        /// <summary>
        /// Gia tri de show len form
        /// </summary>
        decimal? MaskedHIAllowedPrice
        {
            get;
        }
        decimal? HIAllowedPrice
        {
            get;
            set;
        }
        decimal PriceDifference
        {
            get;
            set;
        }
        decimal HIPayment
        {
            get;
            set;
        }
        decimal PatientCoPayment
        {
            get;
            set;
        }
        decimal PatientPayment
        {
            get;
            set;
        }
        //int Qty
        //{
        //    get;
        //    set;
        //}
        decimal Qty
        {
            get;
            set;
        }
        IChargeableItemPrice ChargeableItem
        {
            get;
        }
        double? HIBenefit
        {
            get;
            set;
        }
        long? HisID
        {
            get;
            set;
        }

        DateTime? PaidTime { get; set; }

        DateTime? RefundTime { get; set; }

        DateTime CreatedDate { get; set; }

        AllLookupValues.ExamRegStatus ExamRegStatus { get; set; }

        decimal DiscountAmt { get; set; }
        bool IsDiscounted { get; }
        decimal OtherAmt { get; set; }
        bool IsCountPatientCOVID { get; set; }
    }
    public static class IInvoiceItemBase
    {
        private static void GetItemPriceNotAppliedBH(IInvoiceItem aInvoiceItem, bool IsGetItemPrice = true)
        {
            aInvoiceItem.HisID = null;
            aInvoiceItem.HIBenefit = null;

            if (aInvoiceItem.ChargeableItem != null && aInvoiceItem.ChargeableItem.NormalPrice > 0 && IsGetItemPrice)
            {
                aInvoiceItem.InvoicePrice = aInvoiceItem.ChargeableItem.NormalPrice;
                aInvoiceItem.InvoicePrice = aInvoiceItem.InvoicePrice * (aInvoiceItem is PatientRegistrationDetail && (aInvoiceItem as PatientRegistrationDetail).PaymentPercent != 1.0 ? (decimal)(aInvoiceItem as PatientRegistrationDetail).PaymentPercent : 1);
            }

            aInvoiceItem.PriceDifference = 0;
            aInvoiceItem.HIPayment = 0;
            aInvoiceItem.PatientCoPayment = 0;
            aInvoiceItem.PatientPayment = aInvoiceItem.InvoicePrice;
        }
        public static void GetItemPrice(this IInvoiceItem aInvoiceItem, PatientRegistration aRegistration, double aHIBenefit, DateTime? aPaidTime, bool aDetectHiApplied = false, bool aFullHIBenefitForConfirm = false, long aHIPolicyMinSalary = 1390000)
        {
            if (aRegistration == null)
            {
                return;
            }
            //Chỉ cho phép bán thuốc kho đăng ký khám sức khỏe
            //if (aRegistration.PatientClassID == (long)ePatientClassification.CompanyHealthRecord && !(aInvoiceItem is OutwardDrug))
            //{
            //    GetItemPriceNotAppliedBH(aInvoiceItem, false);
            //    return;
            //}
            // BN khong bao hiem
            if ((aRegistration.HisID == null || aRegistration.HisID <= 0) || (aDetectHiApplied && !aInvoiceItem.IsCountHI))
            {
                GetItemPriceNotAppliedBH(aInvoiceItem);
                return;
            }
            //KMx: Dịch vụ đã lưu nhưng chưa trả tiền thì cũng phải kiểm tra lại thẻ BH đã hết hạn chưa (11/11/2014 17:10).
            if (aDetectHiApplied && aInvoiceItem.ID > 0)
            {
                if (aInvoiceItem.HIBenefit.HasValue && aInvoiceItem.HIBenefit > 0.0)
                {
                    aHIBenefit = aInvoiceItem.HIBenefit.Value;
                }
            }
            else if (!aDetectHiApplied || aInvoiceItem.ID <= 0)
            {
                //Doi voi item moi
                //Neu ngay tao dich vu khong nam trong thoi han cho phep cua the bao hiem thi khong tinh nua.
                //DateTime date = invoiceItem.CreatedDate.Date;
                //DateTime date = Globals.ServerDate.Value.Date;
                DateTime date = aInvoiceItem.CreatedDate.Date;
                if (aPaidTime.HasValue && aPaidTime != null)
                    date = aPaidTime.Value.Date;
                if (date > aRegistration.HealthInsurance.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date || date < aRegistration.HealthInsurance.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date)
                {
                    aHIBenefit = 0.0;
                    if (aRegistration.HealthInsurance_3 != null && date >= aRegistration.HealthInsurance_3.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date && date <= aRegistration.HealthInsurance_3.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date)
                    {
                        aHIBenefit = aRegistration.PtInsuranceBenefit_3.GetValueOrDefault(0);
                    }
                    else if (aRegistration.HealthInsurance_2 != null && date >= aRegistration.HealthInsurance_2.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date && date <= aRegistration.HealthInsurance_2.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date)
                    {
                        aHIBenefit = aRegistration.PtInsuranceBenefit_2.GetValueOrDefault(0);
                    }
                }

                if (aInvoiceItem.ChargeableItem != null && !(aInvoiceItem is OutwardDrug))
                {
                    aInvoiceItem.HIBenefit = aHIBenefit;
                    aInvoiceItem.HIAllowedPrice = aInvoiceItem.ChargeableItem.HIAllowedPriceNew.GetValueOrDefault(0) > 0 ? aInvoiceItem.ChargeableItem.HIAllowedPriceNew : aInvoiceItem.ChargeableItem.HIAllowedPrice;

                    //Kiểm tra nếu có sử dụng bảo hiểm + dịch vụ có bảo hiểm thì lấy giá của dịch vụ bằng với giá bảo hiểm.
                    //Nếu không thì lấy giá bình thường.
                    if (aHIBenefit > 0 && (aInvoiceItem.ChargeableItem.HIPatientPriceNew > 0 || aInvoiceItem.ChargeableItem.HIPatientPrice > 0))
                    {
                        aInvoiceItem.InvoicePrice = aInvoiceItem.ChargeableItem.HIPatientPriceNew > 0 ? aInvoiceItem.ChargeableItem.HIPatientPriceNew : aInvoiceItem.ChargeableItem.HIPatientPrice;
                    }
                    else
                    {
                        aInvoiceItem.InvoicePrice = aInvoiceItem.ChargeableItem.NormalPriceNew > 0 ? aInvoiceItem.ChargeableItem.NormalPriceNew : aInvoiceItem.ChargeableItem.NormalPrice;
                    }
                    aInvoiceItem.InvoicePrice = aInvoiceItem.InvoicePrice * (aInvoiceItem is PatientRegistrationDetail && (aInvoiceItem as PatientRegistrationDetail).PaymentPercent != 1.0 ? (decimal)(aInvoiceItem as PatientRegistrationDetail).PaymentPercent : 1);
                }
            }
            if (aHIBenefit <= 0.0 || !aInvoiceItem.HIAllowedPrice.HasValue)
            {
                aInvoiceItem.HIAllowedPrice = 0;
            }
            //Truong hop dac biet neu la thuoc noi tru thi de xuong duoi luon.
            if (!(aInvoiceItem is OutwardDrugClinicDept || aInvoiceItem is OutwardDrug) && aInvoiceItem.HIAllowedPrice.Value == 0)
            {
                aInvoiceItem.HisID = null;
                aInvoiceItem.HIBenefit = 0;
            }
            else
            {
                if (!aInvoiceItem.HisID.HasValue)
                {
                    aInvoiceItem.HisID = aRegistration.HisID;
                }
            }
            if (aInvoiceItem.HIAllowedPrice.Value > 0)
            {
                aInvoiceItem.PriceDifference = aInvoiceItem.InvoicePrice - aInvoiceItem.HIAllowedPrice.Value;
            }
            else
            {
                aInvoiceItem.PriceDifference = 0;
            }
            if (aInvoiceItem.PaidTime == null
                && aInvoiceItem is PatientRegistrationDetail
                && (aInvoiceItem as PatientRegistrationDetail).V_EkipIndex != null
                && (aInvoiceItem as PatientRegistrationDetail).HIPaymentPercent == 1.0d)
            {
                if ((aInvoiceItem as PatientRegistrationDetail).V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.CungEkip)
                {
                    (aInvoiceItem as PatientRegistrationDetail).HIPaymentPercent = Math.Round(Globals.AxServerSettings.HealthInsurances.PercentForEkip, 2);
                }
                else if ((aInvoiceItem as PatientRegistrationDetail).V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.KhacEkip)
                {
                    (aInvoiceItem as PatientRegistrationDetail).HIPaymentPercent = Math.Round(Globals.AxServerSettings.HealthInsurances.PercentForOtherEkip, 2);
                }
            }
            if (aInvoiceItem is PatientRegistrationDetail && (aInvoiceItem as PatientRegistrationDetail).HIPaymentPercent != 1)
            {
                aInvoiceItem.HIPayment = aInvoiceItem.HIAllowedPrice.Value * (decimal)aHIBenefit * (decimal)(aInvoiceItem as PatientRegistrationDetail).HIPaymentPercent;
            }
            else
            {
                aInvoiceItem.HIPayment = aInvoiceItem.HIAllowedPrice.Value * (decimal)aHIBenefit;
            }

            aInvoiceItem.PatientCoPayment = aInvoiceItem.HIAllowedPrice.Value - aInvoiceItem.HIPayment;
            aInvoiceItem.PatientPayment = aInvoiceItem.InvoicePrice - aInvoiceItem.HIPayment;
            //▼===== TTM: Nếu là dịch vụ có tỷ lệ thanh toán thì tính lại tiền bệnh nhân cùng chi trả, tiền bệnh nhân trả và chênh lệch.
            if (aInvoiceItem is PatientRegistrationDetail && (aInvoiceItem as PatientRegistrationDetail).HIPaymentPercent != 1)
            {
                if ((aInvoiceItem as PatientRegistrationDetail).HIBenefit > 0)
                {
                    aInvoiceItem.PatientCoPayment = (aInvoiceItem.HIPayment / (decimal)(aInvoiceItem as PatientRegistrationDetail).HIBenefit) * (decimal)(1 - aInvoiceItem.HIBenefit);
                }
                else
                {
                    aInvoiceItem.PatientCoPayment = 0;
                }
                aInvoiceItem.PatientPayment = aInvoiceItem.InvoicePrice - aInvoiceItem.HIPayment;
                if (aInvoiceItem.HIAllowedPrice.Value > 0)
                {
                    aInvoiceItem.PriceDifference = aInvoiceItem.InvoicePrice - (aInvoiceItem.PatientCoPayment + aInvoiceItem.HIPayment);
                }
                else
                {
                    aInvoiceItem.PriceDifference = 0;
                }
            }
            //▲=====
        }
        public static void GetItemPrice(this IInvoiceItem aInvoiceItem, PatientRegistration aRegistration, DateTime? aPaidTime, bool aDetectHiApplied = false, bool aFullHIBenefitForConfirm = false, long aHIPolicyMinSalary = 0)
        {
            aInvoiceItem.GetItemPrice(aRegistration, aRegistration.PtInsuranceBenefit.GetValueOrDefault(0.0), aPaidTime, aDetectHiApplied, aFullHIBenefitForConfirm, aHIPolicyMinSalary);
        }
        public static void GetItemTotalPrice(this IInvoiceItem aInvoiceItem, bool aOnlyRoundResultForOutward = false)
        {
            //Tính tổng tiền cho mỗi InvoiceItem.
            // Chưa tính trường hợp: nhiều dịch vụ KHÁM BỆNH chỉ tính 1 lần
            if (!aOnlyRoundResultForOutward)
            {
                aInvoiceItem.TotalCoPayment = (decimal)Math.Ceiling((double)(aInvoiceItem.PatientCoPayment * aInvoiceItem.Qty));
                aInvoiceItem.TotalHIPayment = (decimal)Math.Floor((double)(aInvoiceItem.HIPayment * aInvoiceItem.Qty));
                aInvoiceItem.TotalInvoicePrice = aInvoiceItem.InvoicePrice * (decimal)aInvoiceItem.Qty;
                aInvoiceItem.TotalPatientPayment = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.DiscountAmt - aInvoiceItem.OtherAmt;
                aInvoiceItem.TotalPriceDifference = aInvoiceItem.PriceDifference * (decimal)aInvoiceItem.Qty;
            }
            else
            {
                aInvoiceItem.TotalCoPayment = aInvoiceItem.PatientCoPayment * (decimal)aInvoiceItem.Qty;
                aInvoiceItem.TotalHIPayment = aInvoiceItem.HIPayment * (decimal)aInvoiceItem.Qty;
                aInvoiceItem.TotalInvoicePrice = aInvoiceItem.InvoicePrice * (decimal)aInvoiceItem.Qty;
                aInvoiceItem.TotalPatientPayment = aInvoiceItem.TotalInvoicePrice - aInvoiceItem.TotalHIPayment - aInvoiceItem.DiscountAmt - aInvoiceItem.OtherAmt;
                aInvoiceItem.TotalPriceDifference = aInvoiceItem.PriceDifference * (decimal)aInvoiceItem.Qty;
            }
        }
        public static void ChangeHIBenefit(this IInvoiceItem aInvoiceItem, double aHIBenefit = 0, PatientRegistration aRegistration = null, DateTime? aPaidTime = null, bool aOnlyRoundResultForOutward = false, bool aDetectHiApplied = false)
        {
            var totalHiPayment = aInvoiceItem.TotalHIPayment;
            var totalCoPayment = aInvoiceItem.TotalCoPayment;
            var benefit = aInvoiceItem.HIBenefit;

            if (aHIBenefit == 0)
            {
                aInvoiceItem.HIBenefit = 0;
                aInvoiceItem.TotalCoPayment = 0;
                aInvoiceItem.TotalHIPayment = 0;

                aInvoiceItem.TotalPatientPayment = aInvoiceItem.TotalInvoicePrice;
                aInvoiceItem.TotalPriceDifference = aInvoiceItem.TotalPatientPayment;
            }
            else
            {
                if (aRegistration == null) throw new Exception("Thiếu thông tin đăng ký");

                aInvoiceItem.HIBenefit = aHIBenefit;

                aInvoiceItem.GetItemPrice(aRegistration, aHIBenefit, aPaidTime, aDetectHiApplied);
                aInvoiceItem.GetItemTotalPrice(aOnlyRoundResultForOutward);
            }

            if (aInvoiceItem.TotalCoPayment != totalCoPayment
                || aInvoiceItem.TotalHIPayment != totalHiPayment
                || benefit != aInvoiceItem.HIBenefit)
            {
                if (aInvoiceItem.RecordState == RecordState.UNCHANGED)
                {
                    aInvoiceItem.RecordState = RecordState.MODIFIED;
                }
            }
        }
    }
}