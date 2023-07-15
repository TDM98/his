/*
 * 20170522 #001 CMN:   Added variable to check InPt 5 year HI without paid enough
 * 20180508 #002 TxD:   Commented out all BeginTransaction in this class because it could have caused dead lock in DB (suspection only at this stage)
 * 20180926 #003 TTM:   (THAY ĐỔI TẠM CẦN REVIEW LẠI) Thay đổi vị điều kiện set HIBenefit tránh trường hợp bệnh nhân tick vào thông tuyến và 1 bệnh nhân 5 năm năm đủ CPK
 *                      nhưng chỉ tính tới thông tuyến là return => thiếu tính tick 5 năm đủ CPK => không thay đổi quyền lợi từ 80, 95 => 100%
 * 20180926 #004 TTM:   Thêm điều kiện kiểm tra nếu thẻ do bệnh viện cấp thì phải kiểm tra cùng với 5 năm để tính lại quyền lợi từ 80, 95 => 100%.
 * 20200305 #005 TTM:   BM 0023956: Fix lỗi liên quan đến dịch vụ có đổi giá sẽ dẫn đến không lưu thông tin AmountCoPay chính xác.
 * 20200523 #006 TTM:   BM 0038189: Fix lỗi Stent số 2 quyền lợi < 100% tính toán sai.
 * 20200709 #007 TTM:   BM 0039351: Bổ sung tính toán cho dịch vụ phải tính kèm cả HIPaymentPercent
 * 20200713 #008 TTM:   BM 0039358: Fix lỗi không hiển thị đúng khi load các dịch vụ ngoại trú lần 2 vào bill nội trú (Sáp nhập).
 * 20210717 #009 TNHX:  Truyền phương thức thanh toán + mã code thanh toán online
 * 20230104 #010 BLQ: Thêm thời hạn được tính bảo hiểm theo cấu hình NumDayHIAgreeToPayAfterHIExpiresInPt
 * 20230109 #011 QTD:   Thêm biến đánh dấu lưu chỉ định từ màn hình chỉ định dịch vụ bác sĩ
 * 20230701 #012 BLQ: Thêm điều kiện kiểm tra phiếu khám thứ 2 khi tính lại bill thì không nhân với tỷ lệ
 */
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using DataEntities;
using eHCMS.Configurations;

using eHCMSLanguage;
using AxLogging;
using aEMR.DataAccessLayer.Providers;

namespace eHCMSBillPaymt
{
    public class HiPatientRegAndPaymentProcessor : RegAndPaymentProcessorBase
    {
        #region CALCULATE HI BENEFIT
        public decimal MinPay
        {
            get
            {
                return Globals.AxServerSettings.HealthInsurances.HiPolicyMinSalary * (decimal)Globals.AxServerSettings.HealthInsurances.HiPolicyPercentageOnPayable;
            }
        }


        public double CalcPatientHiBenefit(PatientRegistration regInfo, HealthInsurance hPatientHiProfile, out bool isConsideredAsCrossRegion, long V_RegistrationType = 0, bool IsEmergInPtReExamination = false, bool IsAllowCrossRegion = false)
        {
            var hiBenefit = 0.0;

            isConsideredAsCrossRegion = false;
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
                return hiBenefit;
            }

            if (hPatientHiProfile.InsuranceBenefit == null)
            {
                return hiBenefit;
            }

            if (hPatientHiProfile.isDoing && !hPatientHiProfile.EditLocked) //neu la chinh sua tu tren dua xuong--dinh
            {
                if (regInfo.IsCrossRegion != null)
                {
                    isConsideredAsCrossRegion = regInfo.IsCrossRegion.Value;
                }
                if (hPatientHiProfile.HIPatientBenefit != null)
                {
                    return hPatientHiProfile.HIPatientBenefit.Value;
                }
            }

            hiBenefit = hPatientHiProfile.InsuranceBenefit.RebatePercentage;

            if (hiBenefit <= 0.0)
            {
                hiBenefit = 0.0;
                return hiBenefit;
            }
            //▼====== #003: Vị trí mới
            if (regInfo.IsEmergency)
            {

                if (regInfo.IsHICard_FiveYearsCont && !regInfo.IsHICard_FiveYearsCont_NoPaid)
                {
                    return hiBenefit = 1.0;
                }
                return hiBenefit;
            }
            //▼====== #004
            if (regInfo.PaperReferal != null && regInfo.PaperReferal.Hospital != null || regInfo.IsAllowCrossRegion == true || hPatientHiProfile.RegistrationCode.Trim().ToLower() == Globals.AxServerSettings.Hospitals.HospitalCode.Trim().ToLower())
            //▲====== #004
            {

                if (regInfo.IsHICard_FiveYearsCont && !regInfo.IsHICard_FiveYearsCont_NoPaid)
                {
                    return hiBenefit = 1.0;
                }

                return hiBenefit;
            }
            else
            {
                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU && IsEmergInPtReExamination == true)
                {
                    return hiBenefit;
                }
            }

            if (hPatientHiProfile.RegistrationCode.Trim().ToLower() == Globals.AxServerSettings.Hospitals.HospitalCode.Trim().ToLower())
            {
                return hiBenefit;
            }
            //▲====== #003
            if (regInfo.IsChildUnder6YearsOld || regInfo.IsAllowCrossRegion)
            {
                return hiBenefit;
            }

            //▼====== #003: Vị trí cũ

            ////HPT 24/08/2015 Phải kiểm tra BN có cấp cứu trước để đảm bảo đăng ký nội trú BHYT của bệnh nhân là hợp lệ
            ////Bệnh nhân phải đăng ký nhập viện BHYT hợp lệ mới được xét đến điều kiện hưởng quyền lợi BHYT 5 năm liên tiếp
            //if (regInfo.IsEmergency)
            //{
            //    /*==== #001 ====*/
            //    //if (regInfo.IsHICard_FiveYearsCont)
            //    if (regInfo.IsHICard_FiveYearsCont && !regInfo.IsHICard_FiveYearsCont_NoPaid)
            //    /*==== #001 ====*/
            //    {
            //        return hiBenefit = 1.0;
            //    }
            //    return hiBenefit;
            //}
            ////HPT 24/08/2015 Nếu BN không cấp cứu thì đăng ký BHYT hợp lệ khi có thẻ BHYT hợp lệ và giấy chuyển viện hợp lệ

            //if (hPatientHiProfile.RegistrationCode.Trim().ToLower() == Globals.AxServerSettings.Hospitals.HospitalCode.Trim().ToLower())
            //{
            //    return hiBenefit;
            //}
            ////Kiểm tra đã có giấy chuyển viện chưa, nếu có, kiểm tra tiếp có check Xác nhận BN có BHYT 5 năm liên tiếp không. Nếu có cho phép hưởng quyền lợi 100%
            //if (regInfo.PaperReferal != null && regInfo.PaperReferal.Hospital != null || regInfo.IsAllowCrossRegion == true)
            //{
            //    /*
            //     * 22/06/2012 Txd: The accepted list is no longer required here
            //     * By just having the User accept and enter a paper referal then it is automatically accepted by the software. 
            //    if (Globals.AxServerSettings.HealthInsurances.CrossRegionCodeAcceptedList.Contains(regInfo.PaperReferal.Hospital.HICode))
            //    {
            //        return hiBenefit;
            //    }
            //    */
            //    /*==== #001 ====*/
            //    //if (regInfo.IsHICard_FiveYearsCont)
            //    if (regInfo.IsHICard_FiveYearsCont && !regInfo.IsHICard_FiveYearsCont_NoPaid)
            //    /*==== #001 ====*/
            //    {
            //        return hiBenefit = 1.0;
            //    }

            //    return hiBenefit;
            //}
            //else
            //{
            //    // TxD 25/01/2015: Added new Case for a previous Emergency InPt come back for ReExamination (Tai Kham) and 
            //    //                  shall get BHYT not requiring a Peper referal
            //    if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU && IsEmergInPtReExamination == true)
            //    {
            //        return hiBenefit;
            //    }
            //}

            //▲====== #003
            isConsideredAsCrossRegion = true;

            if (Globals.AxServerSettings.HealthInsurances.ApplyHINewRule20150101)
            {
                if (V_RegistrationType > 0 && V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU)
                {
                    hiBenefit = hPatientHiProfile.InsuranceBenefit.RebatePercentage * Globals.AxServerSettings.HealthInsurances.RebatePercentage2015Level1_InPt;
                }
                else
                {
                    if (Globals.AxServerSettings.HealthInsurances.AllowOutPtCrossRegion)
                    {
                        hiBenefit = Globals.AxServerSettings.HealthInsurances.RebatePercentageLevel1;
                    }
                    else
                    {
                        hiBenefit = 0.0D;
                    }
                }
            }
            else
            {
                hiBenefit = Globals.AxServerSettings.HealthInsurances.RebatePercentageLevel1;
            }

            return hiBenefit;
        }

        #endregion

        protected override void GetItemPrice(IInvoiceItem invoiceItem, PatientRegistration registration, double hiBenefit)
        {
            if (!invoiceItem.HiApplied)
            {
                base.GetItemPrice(invoiceItem, registration, hiBenefit);
                return;
            }

            if (invoiceItem.ID > 0)
            {
                if (invoiceItem.HIBenefit.HasValue && invoiceItem.HIBenefit > 0.0)
                {
                    hiBenefit = invoiceItem.HIBenefit.Value;
                }
            }
            else
            {
                //Doi voi item moi
                //Neu ngay tao dich vu khong nam trong thoi han cho phep cua the bao hiem thi khong tinh nua.
                DateTime date = invoiceItem.CreatedDate.Date;
                if (date > registration.HealthInsurance.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date || date < registration.HealthInsurance.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date)
                {
                    hiBenefit = 0.0;
                    if (registration.HealthInsurance_3 != null && date >= registration.HealthInsurance_3.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date && date <= registration.HealthInsurance_3.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date)
                    {
                        hiBenefit = registration.PtInsuranceBenefit_3.GetValueOrDefault(0);
                    }
                    else if (registration.HealthInsurance_2 != null && date >= registration.HealthInsurance_2.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date && date <= registration.HealthInsurance_2.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date)
                    {
                        hiBenefit = registration.PtInsuranceBenefit_2.GetValueOrDefault(0);
                    }
                }

                if (invoiceItem.ChargeableItem != null)
                {
                    invoiceItem.HIBenefit = hiBenefit;
                    invoiceItem.HIAllowedPrice = invoiceItem.ChargeableItem.HIAllowedPrice;

                    //Kiểm tra nếu có sử dụng bảo hiểm + dịch vụ có bảo hiểm thì lấy giá của dịch vụ bằng với giá bảo hiểm.
                    //Nếu không thì lấy giá bình thường.
                    if (hiBenefit > 0 && invoiceItem.ChargeableItem.HIPatientPrice > 0)
                    {
                        invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.HIPatientPrice;
                    }
                    else
                    {
                        invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.NormalPrice;
                    }
                }
            }

            if (hiBenefit <= 0.0 || !invoiceItem.HIAllowedPrice.HasValue)
            {
                invoiceItem.HIAllowedPrice = 0;
            }
            //Truong hop dac biet neu la thuoc noi tru thi de xuong duoi luon.

            if (!(invoiceItem is OutwardDrugClinicDept) && invoiceItem.HIAllowedPrice.Value == 0)
            {
                invoiceItem.HisID = null;
                invoiceItem.HIBenefit = 0;
            }
            else
            {
                if (!invoiceItem.HisID.HasValue)
                {
                    invoiceItem.HisID = registration.HisID;
                }
            }
            invoiceItem.PriceDifference = invoiceItem.InvoicePrice - invoiceItem.HIAllowedPrice.Value;
            invoiceItem.HIPayment = invoiceItem.HIAllowedPrice.Value * (decimal)hiBenefit;
            invoiceItem.PatientCoPayment = invoiceItem.HIAllowedPrice.Value - invoiceItem.HIPayment;
            invoiceItem.PatientPayment = invoiceItem.InvoicePrice - invoiceItem.HIPayment;
        }

        //▼===== #005: GetBillingInvItemPrice_New Bổ sung thêm biến IsUpdatePrice để phân biệt dịch vụ nào là dịch vụ đổi giá dịch vụ nào không đổi giá.
        //             Nếu là dịch vụ đổi giá thì không cập nhật giá lại theo ChargeableItem.
        protected override void GetBillingInvItemPrice_New(MedRegItemBase invoiceItem, PatientRegistration registration, double hiBenefit, bool IsUpdatePrice = false)
        {
            if (!invoiceItem.IsCountHI)
            {
                base.GetBillingInvItemPrice_New(invoiceItem, registration, hiBenefit, IsUpdatePrice);
                return;
            }

            if (invoiceItem.ID > 0)
            {
                if (invoiceItem.HIBenefit.HasValue && invoiceItem.HIBenefit > 0.0)
                {
                    hiBenefit = invoiceItem.HIBenefit.Value;
                    DateTime date = invoiceItem.MedicalInstructionDate.HasValue && invoiceItem.MedicalInstructionDate > DateTime.MinValue ? invoiceItem.MedicalInstructionDate.Value.Date : invoiceItem.CreatedDate.Date;
                    //▼====: #010
                    if ((registration.HealthInsurance_2 == null 
                        && date > registration.HealthInsurance.ValidDateTo.GetValueOrDefault(DateTime.MaxValue)
                            .AddDays(Globals.AxServerSettings.InRegisElements.NumDayHIAgreeToPayAfterHIExpiresInPt).Date)
                    //▲====: #010
                        && (date > registration.HealthInsurance.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date
                        || date < registration.HealthInsurance.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date))
                    {
                        if (registration.HealthInsurance_3 != null 
                            && date >= registration.HealthInsurance_3.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date
                            && date <= registration.HealthInsurance_3.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date)
                        {
                            hiBenefit = registration.PtInsuranceBenefit_3.GetValueOrDefault(0);
                        }
                        else if (registration.HealthInsurance_2 != null 
                            && date >= registration.HealthInsurance_2.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date
                            && date <= registration.HealthInsurance_2.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date)
                        {
                            hiBenefit = registration.PtInsuranceBenefit_2.GetValueOrDefault(0);
                        }
                    }
                    if (invoiceItem.ChargeableItem != null && !IsUpdatePrice)
                    {
                        invoiceItem.HIBenefit = hiBenefit;
                        if (invoiceItem.ChargeableItem.HIAllowedPriceNew > 0)
                        {
                            invoiceItem.HIAllowedPrice = invoiceItem.ChargeableItem.HIAllowedPriceNew;
                        }
                        else if (invoiceItem.ChargeableItem.HIAllowedPrice > 0)
                        {
                            invoiceItem.HIAllowedPrice = invoiceItem.ChargeableItem.HIAllowedPrice;
                        }

                        //Kiểm tra nếu có sử dụng bảo hiểm + dịch vụ có bảo hiểm thì lấy giá của dịch vụ bằng với giá bảo hiểm.
                        //Nếu không thì lấy giá bình thường.
                        if (invoiceItem is PatientRegistrationDetail)
                        {
                            //▼===== #012
                            if (invoiceItem.MedProductType == AllLookupValues.MedProductType.KCB && (decimal)(invoiceItem as PatientRegistrationDetail).PaymentPercent < 1)
                            {
                                if (hiBenefit > 0 && invoiceItem.ChargeableItem.HIPatientPriceNew > 0)
                                {
                                    invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.HIPatientPriceNew;
                                }
                                else if (hiBenefit > 0 && invoiceItem.ChargeableItem.HIPatientPrice > 0)
                                {
                                    invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.HIPatientPrice;
                                }
                                else if (invoiceItem.ChargeableItem.NormalPrice > 0)
                                {
                                    invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.NormalPrice;
                                }
                            }
                            else
                            {
                                //▼===== #008
                                if (hiBenefit > 0 && invoiceItem.ChargeableItem.HIPatientPriceNew > 0)
                                {
                                    invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.HIPatientPriceNew * (decimal)(invoiceItem as PatientRegistrationDetail).PaymentPercent;
                                }
                                else if (hiBenefit > 0 && invoiceItem.ChargeableItem.HIPatientPrice > 0)
                                {
                                    invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.HIPatientPrice * (decimal)(invoiceItem as PatientRegistrationDetail).PaymentPercent;
                                }
                                else if (invoiceItem.ChargeableItem.NormalPrice > 0)
                                {
                                    invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.NormalPrice * (decimal)(invoiceItem as PatientRegistrationDetail).PaymentPercent;
                                }
                                //▲===== #008
                            }
                            //▲===== #012
                        }
                        else
                        {
                            if (hiBenefit > 0 && invoiceItem.ChargeableItem.HIPatientPriceNew > 0)
                            {
                                invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.HIPatientPriceNew;
                            }
                            else if (hiBenefit > 0 && invoiceItem.ChargeableItem.HIPatientPrice > 0)
                            {
                                invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.HIPatientPrice;
                            }
                            else if (invoiceItem.ChargeableItem.NormalPrice > 0)
                            {
                                invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.NormalPrice;
                            }
                        }
                    }
                }
            }
            else
            {
                //Doi voi item moi
                //Neu ngay tao dich vu khong nam trong thoi han cho phep cua the bao hiem thi khong tinh nua.
                DateTime date = invoiceItem.MedicalInstructionDate.HasValue 
                    && invoiceItem.MedicalInstructionDate > DateTime.MinValue ? invoiceItem.MedicalInstructionDate.Value.Date : invoiceItem.CreatedDate.Date;
                //▼====: #010
                if ((registration.HealthInsurance_2 == null 
                    && date > registration.HealthInsurance.ValidDateTo.GetValueOrDefault(DateTime.MaxValue)
                        .AddDays(Globals.AxServerSettings.InRegisElements.NumDayHIAgreeToPayAfterHIExpiresInPt).Date )
                //▲====: #010
                    && (date > registration.HealthInsurance.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date
                    || date < registration.HealthInsurance.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date))
                {
                    hiBenefit = 0.0;
                    if (registration.HealthInsurance_3 != null
                        && date >= registration.HealthInsurance_3.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date 
                        && date <= registration.HealthInsurance_3.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date)
                    {
                        hiBenefit = registration.PtInsuranceBenefit_3.GetValueOrDefault(0);
                    }
                    else if (registration.HealthInsurance_2 != null
                        && date >= registration.HealthInsurance_2.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date 
                        && date <= registration.HealthInsurance_2.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date)
                    {
                        hiBenefit = registration.PtInsuranceBenefit_2.GetValueOrDefault(0);
                    }
                }

                if (invoiceItem.ChargeableItem != null && !IsUpdatePrice)
                {
                    invoiceItem.HIBenefit = hiBenefit;
                    invoiceItem.HIAllowedPrice = invoiceItem.ChargeableItem.HIAllowedPrice;

                    //Kiểm tra nếu có sử dụng bảo hiểm + dịch vụ có bảo hiểm thì lấy giá của dịch vụ bằng với giá bảo hiểm.
                    //Nếu không thì lấy giá bình thường.
                    if (hiBenefit > 0 && invoiceItem.ChargeableItem.HIPatientPrice > 0)
                    {
                        invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.HIPatientPrice;
                    }
                    else
                    {
                        invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.NormalPrice;
                    }
                }
            }

            if (hiBenefit <= 0.0 || !invoiceItem.HIAllowedPrice.HasValue)
            {
                invoiceItem.HIAllowedPrice = 0;
            }
            //Truong hop dac biet neu la thuoc noi tru thi de xuong duoi luon.

            if (!(invoiceItem is OutwardDrugClinicDept) && invoiceItem.HIAllowedPrice.Value == 0)
            {
                invoiceItem.HisID = null;
                invoiceItem.HIBenefit = 0;
            }
            else
            {
                if (!invoiceItem.HisID.HasValue)
                {
                    invoiceItem.HisID = registration.HisID;
                }
            }
            
            //▼===== 20191229 TTM:  Thay đổi cách tính toán trước khi lưu bill.
            //                      Nếu trần vật tư trong bảng Lim < HiAllowPrice thì lấy Trần trong bảng Lim tính toán.
            if (invoiceItem is OutwardDrugClinicDept && (invoiceItem as OutwardDrugClinicDept).LimQtyAndHIPrice != null)
            {
                OutwardDrugClinicDept item = invoiceItem as OutwardDrugClinicDept;
                if (item.CountValue == 1)
                {
                    if (item.LimQtyAndHIPrice.ItemNumber1MaxBenefit > 0)
                    {
                        if (registration.IsCrossRegion == false)
                        {
                            hiBenefit = (double)item.LimQtyAndHIPrice.ItemNumber1MaxBenefit;
                        }
                        else
                        {
                            hiBenefit = Globals.AxServerSettings.HealthInsurances.RebatePercentage2015Level1_InPt;
                        }
                    }
                    invoiceItem.PriceDifference = invoiceItem.InvoicePrice - item.LimQtyAndHIPrice.ItemNumber1MaxPayAmt;
                    invoiceItem.HIPayment = Math.Min(item.LimQtyAndHIPrice.ItemNumber1MaxPayAmt, (decimal)item.HIAllowedPrice * item.LimQtyAndHIPrice.ItemNumber1MaxPayPerc) * (decimal)hiBenefit;
                    invoiceItem.PatientCoPayment = item.LimQtyAndHIPrice.ItemNumber1MaxPayAmt - invoiceItem.HIPayment;
                }
                else if (item.CountValue == 2)
                {
                    //▼===== #006:  Bổ sung thêm code kiểm tra tình trạng đúng trái tuyến của bệnh nhân. Và tăng lên full quyền lợi theo trường hợp
                    //              Cho vật tư được đánh dấu phải tăng full quyền lợi trong bảng Lim.
                    if (item.LimQtyAndHIPrice.ItemNumber2MaxBenefit > 0)
                    {
                        if (registration.IsCrossRegion == false)
                        {
                            hiBenefit = (double)item.LimQtyAndHIPrice.ItemNumber2MaxBenefit;
                        }
                        else
                        {
                            hiBenefit = Globals.AxServerSettings.HealthInsurances.RebatePercentage2015Level1_InPt;
                        }
                    }
                    invoiceItem.PriceDifference = invoiceItem.InvoicePrice - item.LimQtyAndHIPrice.ItemNumber2MaxPayAmt;
                    invoiceItem.HIPayment = Math.Min(item.LimQtyAndHIPrice.ItemNumber2MaxPayAmt, (decimal)item.HIAllowedPrice * item.LimQtyAndHIPrice.ItemNumber2MaxPayPerc) * (decimal)hiBenefit;
                    invoiceItem.PatientCoPayment = item.LimQtyAndHIPrice.ItemNumber2MaxPayAmt - invoiceItem.HIPayment;
                    //▲===== #006
                }
                else if (item.CountValue == 3)
                {
                    if (item.LimQtyAndHIPrice.ItemNumber3MaxBenefit > 0)
                    {
                        if (registration.IsCrossRegion == false)
                        {
                            hiBenefit = (double)item.LimQtyAndHIPrice.ItemNumber3MaxBenefit;
                        }
                        else
                        {
                            hiBenefit = Globals.AxServerSettings.HealthInsurances.RebatePercentage2015Level1_InPt;
                        }
                    }
                    invoiceItem.PriceDifference = invoiceItem.InvoicePrice - item.LimQtyAndHIPrice.ItemNumber3MaxPayAmt;
                    invoiceItem.HIPayment = Math.Min(item.LimQtyAndHIPrice.ItemNumber3MaxPayAmt, (decimal)item.HIAllowedPrice * item.LimQtyAndHIPrice.ItemNumber3MaxPayPerc) * (decimal)hiBenefit;
                    invoiceItem.PatientCoPayment = item.LimQtyAndHIPrice.ItemNumber3MaxPayAmt - invoiceItem.HIPayment;
                }
            }
            else
            {
                invoiceItem.PriceDifference = invoiceItem.InvoicePrice - invoiceItem.HIAllowedPrice.Value;

                //20200509 TBL: BM 0038169: Tính theo Ekip cho BH trả, BN cùng chi trả
                if (invoiceItem is PatientRegistrationDetail)
                {
                    if ((invoiceItem as PatientRegistrationDetail).V_EkipIndex != null 
                        && (invoiceItem as PatientRegistrationDetail).V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.CungEkip)
                    {
                        invoiceItem.HIPayment = invoiceItem.HIAllowedPrice.Value * (decimal)hiBenefit * (decimal)Globals.AxServerSettings.HealthInsurances.PercentForEkip;
                        invoiceItem.PatientCoPayment = invoiceItem.HIAllowedPrice.Value * (decimal)Globals.AxServerSettings.HealthInsurances.PercentForEkip - invoiceItem.HIPayment;
                        invoiceItem.PriceDifference = invoiceItem.InvoicePrice - invoiceItem.HIPayment - invoiceItem.PatientCoPayment;
                    }
                    else if ((invoiceItem as PatientRegistrationDetail).V_EkipIndex != null 
                        && (invoiceItem as PatientRegistrationDetail).V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.KhacEkip)
                    {
                        invoiceItem.HIPayment = invoiceItem.HIAllowedPrice.Value * (decimal)hiBenefit * (decimal)Globals.AxServerSettings.HealthInsurances.PercentForOtherEkip;
                        invoiceItem.PatientCoPayment = invoiceItem.HIAllowedPrice.Value * (decimal)Globals.AxServerSettings.HealthInsurances.PercentForOtherEkip - invoiceItem.HIPayment;
                        invoiceItem.PriceDifference = invoiceItem.InvoicePrice - invoiceItem.HIPayment - invoiceItem.PatientCoPayment;
                    }
                    else
                    {
                        //▼===== #007
                        invoiceItem.HIPayment = invoiceItem.HIAllowedPrice.Value * (decimal)hiBenefit * (decimal)(invoiceItem as PatientRegistrationDetail).HIPaymentPercent;
                        if (hiBenefit > 0)
                        {
                            invoiceItem.PatientCoPayment = (invoiceItem.HIPayment / (decimal)hiBenefit) * (decimal)(1 - hiBenefit);
                        }
                        else
                        {
                            invoiceItem.PatientCoPayment = 0;
                        }
                        invoiceItem.PatientPayment = invoiceItem.InvoicePrice - invoiceItem.HIPayment;
                        if (invoiceItem.HIAllowedPrice.Value > 0)
                        {
                            invoiceItem.PriceDifference = invoiceItem.InvoicePrice - (invoiceItem.PatientCoPayment + invoiceItem.HIPayment);
                        }
                        else
                        {
                            invoiceItem.PriceDifference = 0;
                        }
                        //invoiceItem.HIPayment = invoiceItem.HIAllowedPrice.Value * (decimal)hiBenefit;
                        //invoiceItem.PatientCoPayment = invoiceItem.HIAllowedPrice.Value - invoiceItem.HIPayment;
                        //▲===== #007
                    }
                }
                else
                {
                    invoiceItem.HIPayment = invoiceItem.HIAllowedPrice.Value * (decimal)hiBenefit;
                    invoiceItem.PatientCoPayment = invoiceItem.HIAllowedPrice.Value - invoiceItem.HIPayment;
                }
            }
            //▲=====
            invoiceItem.PatientPayment = invoiceItem.InvoicePrice - invoiceItem.HIPayment;
        }

        protected override void AddRegistrationDetailsToCurrentRegistration(List<PatientRegistrationDetail> regDetailList)
        {
            base.AddRegistrationDetailsToCurrentRegistration(regDetailList);
        }

        protected override void CalcInvoiceItems(IEnumerable<IInvoiceItem> colInvoiceItems, PatientRegistration registration)
        {
            foreach (IInvoiceItem item in colInvoiceItems)
            {
                GetItemPrice(item, registration, registration.PtInsuranceBenefit.GetValueOrDefault(0.0));
                GetItemTotalPrice(item);
            }
        }

        protected override void CalcBillingInvItems_New(IEnumerable<MedRegItemBase> colInvoiceItems, PatientRegistration registration)
        {
            // Hpt 30/11/2015: không tính lại những dịch vụ thuộc loại không có giá hoặc giá không cố định
            foreach (MedRegItemBase item in colInvoiceItems)
            {
                //▼===== #005: Phân biệt được đâu là dịch vụ đổi giá đâu không phải để tính toán cho chính xác.
                //if (item.V_NewPriceType != (Int32)AllLookupValues.V_NewPriceType.Unknown_PriceType && item.V_NewPriceType != (Int32)AllLookupValues.V_NewPriceType.Updatable_PriceType)
                //{
                bool IsUpdatePrice = item.V_NewPriceType == (Int32)AllLookupValues.V_NewPriceType.Updatable_PriceType 
                                        || item.V_NewPriceType == (Int32)AllLookupValues.V_NewPriceType.Unknown_PriceType ? true : false;
                    GetBillingInvItemPrice_New(item, registration, registration.PtInsuranceBenefit.GetValueOrDefault(0.0), IsUpdatePrice);
                    GetBillingInvItemTotalPrice_New(item);
                //}
                //▲===== 
            }
        }


        protected override void AddRegistrationDetails(PatientRegistration regInfo, List<PatientRegistrationDetail> regDetailList, DbConnection conn, DbTransaction tran, out List<long> newRegistrationIDList)
        {
            if (regDetailList != null)
            {
                foreach (var invoiceItem in regDetailList)
                {
                    GetItemPrice(invoiceItem, regInfo, regInfo.PtInsuranceBenefit.GetValueOrDefault(0.0));
                    if (invoiceItem.HisID == null && regInfo.HisID != null)
                    {
                        invoiceItem.HisID = regInfo.HisID;
                    }

                    GetItemTotalPrice(invoiceItem);
                }
            }

            base.AddRegistrationDetails(regInfo, regDetailList, conn, tran, out newRegistrationIDList);
        }

        protected override void AddRegistrationDetails_New(PatientRegistration regInfo, List<PatientRegistrationDetail> regDetailList, DbConnection conn, DbTransaction tran, out List<long> newRegistrationIDList)
        {
            if (regDetailList != null)
            {
                foreach (var invoiceItem in regDetailList)
                {
                    GetBillingInvItemPrice_New(invoiceItem, regInfo, regInfo.PtInsuranceBenefit.GetValueOrDefault(0.0));
                    if (invoiceItem.HisID == null && regInfo.HisID != null)
                    {
                        invoiceItem.HisID = regInfo.HisID;
                    }

                    GetBillingInvItemTotalPrice_New(invoiceItem);
                }
            }

            base.AddRegistrationDetails_New(regInfo, regDetailList, conn, tran, out newRegistrationIDList);
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
                    GetItemPrice(invoiceItem, regInfo, regInfo.PtInsuranceBenefit.GetValueOrDefault(0.0));
                    GetItemTotalPrice(invoiceItem);
                }
            }
            base.AddOutwardDrugClinicDept(regInfo, inv, updatedInwardItems, conn, tran, out inwardDrugIDListError);
        }
        protected override void AddNewRegistration(PatientRegistration regInfo, DbConnection conn, DbTransaction tran, out long newRegistrationID)
        {
            int sequenceNo;
            if (regInfo.HealthInsurance != null)
            {
                bool bConsiderAsCrossRegion;
                var hiBenefit = CalcPatientHiBenefit(regInfo, regInfo.HealthInsurance, out bConsiderAsCrossRegion);
                regInfo.PtInsuranceBenefit = hiBenefit;
                if (!regInfo.IsCrossRegion.HasValue)
                {
                    regInfo.IsCrossRegion = bConsiderAsCrossRegion;
                }
            }

            PatientProvider.Instance.AddRegistration(regInfo, conn, tran, out newRegistrationID, out sequenceNo);
            regInfo.PtRegistrationID = newRegistrationID;

            if (regInfo.PaperReferal == null) return;
            regInfo.PaperReferal.IsActive = false;
            PatientProvider.Instance.UpdatePaperReferal(regInfo.PaperReferal);
        }

        protected void CorrectRegistrationDetails(PatientRegistration regInfo, DbConnection conn, DbTransaction tran)
        {
            PatientProvider.Instance.CorrectRegistrationDetails(regInfo.PtRegistrationID, conn, tran);
        }

        /// <summary>
        /// Can bang dang ky va transaction
        /// </summary>
        /// <param name="balanceDate"></param>
        /// <param name="returnedOutInvoiceID">Ma phieu tra can cap nhat lai gia tien.</param>
        /// <param name="regInfo"></param>
        /// <param name="conn"></param>
        /// <param name="tran"></param>
        /// <returns>Tra ve True neu da can bang transaction sau khi ra khoi ham.</returns>
        private bool BalanceRegistrationAndTransaction(long StaffID, out string ListIDOutTranDetails, PatientRegistration regInfo, DbConnection conn, DbTransaction tran, DateTime balanceDate, long? returnedOutInvoiceID = null)
        {
            ListIDOutTranDetails = "";
            if (balanceDate == DateTime.MinValue)
            {
                balanceDate = DateTime.Now;
            }
            if (!regInfo.IsCrossRegion.GetValueOrDefault(false))
            {
                var oldProgMinusHi = regInfo.ProgSumMinusMinHI.GetValueOrDefault(0);
                decimal curProgSumMinusMinHi;
                PatientProvider.Instance.UpdateProgSumMinusMinHIForRegistration(regInfo.PtRegistrationID, MinPay, 0, out curProgSumMinusMinHi, conn, tran);
                if (oldProgMinusHi < 0)
                {
                    if (curProgSumMinusMinHi < 0)
                    {
                        PatientProvider.Instance.CorrectHiRegistration(regInfo.PtRegistrationID, false, conn, tran, returnedOutInvoiceID);
                        //Can bang transaction
                        BalanceTransaction(StaffID, regInfo.PtRegistrationID, regInfo.FindPatient, out ListIDOutTranDetails, balanceDate, conn, tran);
                        return true;
                    }
                    RefDrugGenericDetailsProvider.Instance.BackupOutwardDrugInvoiceOfRegistration(regInfo.PtRegistrationID, conn, tran);
                    PatientProvider.Instance.CorrectHiRegistration(regInfo.PtRegistrationID, true, conn, tran, returnedOutInvoiceID);
                    //Can bang transaction
                    BalanceTransaction(StaffID, regInfo.PtRegistrationID, regInfo.FindPatient, out ListIDOutTranDetails, balanceDate, conn, tran);
                    return true;
                }
                if (curProgSumMinusMinHi < 0)
                {
                    RefDrugGenericDetailsProvider.Instance.BackupOutwardDrugInvoiceOfRegistration(regInfo.PtRegistrationID, conn, tran);
                    PatientProvider.Instance.CorrectHiRegistration(regInfo.PtRegistrationID, false, conn, tran, returnedOutInvoiceID);
                    //Can bang transaction
                    BalanceTransaction(StaffID, regInfo.PtRegistrationID, regInfo.FindPatient, out ListIDOutTranDetails, balanceDate, conn, tran);
                    return true;
                }
                return false;
            }
            return false;
        }

        public override bool AddPCLRequest(long StaffID, PatientRegistration regInfo, PatientPCLRequest pclRequest, out long newPclRequestID)
        {
            using (DbConnection conn = PatientProvider.Instance.CreateConnection())
            {
                conn.Open();

                // =====▼ #002                                                                                                                            
                // DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                DbTransaction tran = null;
                // =====▲ #002

                try
                {
                    if (regInfo.PtRegistrationID <= 0)
                    {
                        throw new Exception(string.Format("{0}.",eHCMSResources.Z1701_G1_ChuaChonDKCuaCDinhCLS));
                    }

                    long medicalRecID;
                    long? medicalFileID;
                    Debug.Assert(regInfo.PatientID != null, "regInfo.PatientID != null");
                    CreatePatientMedialRecordIfNotExists(regInfo.PatientID.Value, regInfo.ExamDate, out medicalRecID, out medicalFileID,conn, tran);
                    if (pclRequest.PatientPCLRequestIndicators != null)
                    {
                        CalcInvoiceItems(pclRequest.PatientPCLRequestIndicators, regInfo);
                    }
                    AddPCLRequest(medicalRecID,medicalFileID, regInfo.PtRegistrationID, pclRequest, (long)regInfo.V_RegistrationType, conn, tran, out newPclRequestID);
                    string ListIDOutTranDetails = "";
                    BalanceRegistrationAndTransaction(StaffID, out ListIDOutTranDetails, regInfo, conn, tran, pclRequest.CreatedDate);
                    // =====▼ #002                                                                                                                                                
                    // tran.Commit();
                    // =====▲ #002
                }
                catch (Exception exObj)
                {
                    AxLogger.Instance.LogError(exObj);
                    // =====▼ #002                                                                                                                            
                    // tran.Rollback();
                    // =====▲ #002
                    throw;
                }
            }
            return true;
        }

        //public override bool AddOutwardDrugInvoice(PatientRegistration regInfo, OutwardDrugInvoice outwardInvoice, out long newInvoiceID)
        //{
        //    newInvoiceID = 0;

        //    using (DbConnection conn = PatientProvider.Instance.CreateConnection())
        //    {
        //        conn.Open();

        //        DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

        //        try
        //        {
        //            if (outwardInvoice.OutDate == DateTime.MinValue)
        //            {
        //                outwardInvoice.OutDate = DateTime.Now;
        //            }
        //            if (outwardInvoice.OutwardDrugs != null)
        //            {
        //                foreach (var item in outwardInvoice.OutwardDrugs)
        //                {
        //                    item.CreatedDate = outwardInvoice.OutDate;
        //                }
        //                CalcInvoiceItems(outwardInvoice.OutwardDrugs, regInfo);
        //            }

        //            RefDrugGenericDetailsProvider.Instance.InsertOrUpdateDrugInvoices(outwardInvoice, out newInvoiceID, conn, tran);

        //            BalanceRegistrationAndTransaction(regInfo, conn, tran,outwardInvoice.OutDate);
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

        //public override void UpdatePCLRequest(long StaffID, PatientRegistration registrationInfo, PatientPCLRequest pclRequest, out List<PatientPCLRequest> listPclSave, DateTime modifiedDate = default(DateTime))
        //{
        //    listPclSave = new List<PatientPCLRequest>();

        //    if (modifiedDate == default(DateTime))
        //    {
        //        modifiedDate = DateTime.Now;
        //    }
        //    using (DbConnection conn = PatientProvider.Instance.CreateConnection())
        //    {
        //        conn.Open();

        //        DbTransaction tran = conn.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

        //        try
        //        {
        //            var newPclRequestList = new List<PatientPCLRequestDetail>();
        //            var modifiedPclRequestList = new List<PatientPCLRequestDetail>();
        //            var deletedPclRequestList = new List<PatientPCLRequestDetail>();

        //            foreach (var requestDetail in pclRequest.PatientPCLRequestIndicators)
        //            {
        //                if (requestDetail.RecordState == RecordState.DETACHED)
        //                {
        //                    newPclRequestList.Add(requestDetail);
        //                    if (requestDetail.CreatedDate == DateTime.MinValue)
        //                    {
        //                        requestDetail.CreatedDate = DateTime.Now;
        //                    }
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
        //            if (newPclRequestList.Count > 0)
        //            {
        //                CalcInvoiceItems(newPclRequestList, registrationInfo);
        //                PatientProvider.Instance.AddPCLRequestDetails(pclRequest.PatientPCLReqID, newPclRequestList, conn, tran);
        //            }
        //            if (deletedPclRequestList.Count > 0)
        //            {
        //                PatientProvider.Instance.DeletePCLRequestDetailList(deletedPclRequestList, conn, tran);
        //            }
        //            if (modifiedPclRequestList.Count > 0)
        //            {
        //                CalcInvoiceItems(modifiedPclRequestList, registrationInfo);
        //                PatientProvider.Instance.UpdatePCLRequestDetailList(modifiedPclRequestList, conn, tran);
        //            }

        //            string ListIDOutTranDetails = "";
        //            bool balanced = BalanceRegistrationAndTransaction(StaffID,out ListIDOutTranDetails, registrationInfo, conn, tran, modifiedDate);
        //            if (!balanced)
        //            {
        //                BalanceTransaction(StaffID,registrationInfo.PtRegistrationID, registrationInfo.FindPatient, out ListIDOutTranDetails, modifiedDate, conn, tran);
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



        /// <summary>
        /// Cân bằng transaction của 1 dịch vụ đăng ký.
        /// </summary>
        /// <returns>Trả về null nếu transaction của dịch vụ này đã cân bằng.
        /// Nếu dịch vụ chưa cân bằng => Trả về 1 object PatientTransactionDetail để thêm vào Transaction cho cân bằng</returns>
        protected override PatientTransactionDetail BalanceTransactionOfService(PatientTransaction transaction, PatientRegistrationDetail details, DateTime balanceDate)
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

            decimal amount = 0;
            decimal priceDifference = 0;
            decimal amountCoPay = 0;
            decimal healthInsuranceRebate = 0;
            decimal discountAmt = 0;

            foreach (var tranItem in transaction.PatientTransactionDetails)
            {
                if (tranItem.PtRegDetailID == details.PtRegDetailID)
                {
                    amount += tranItem.Amount;
                    priceDifference += tranItem.PriceDifference.GetValueOrDefault(0);
                    amountCoPay += tranItem.AmountCoPay.GetValueOrDefault(0);
                    healthInsuranceRebate += tranItem.HealthInsuranceRebate.GetValueOrDefault(0);
                    discountAmt += tranItem.DiscountAmt.GetValueOrDefault(0);
                }
            }

            if (details.MarkedAsDeleted ||
               details.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
               || details.RecordState == RecordState.DELETED)
            {
                amount = -amount;
                priceDifference = -priceDifference;
                amountCoPay = -amountCoPay;
                healthInsuranceRebate = -healthInsuranceRebate;
                discountAmt = -discountAmt;
            }
            else
            {
                amount = details.TotalInvoicePrice - amount;
                priceDifference = details.TotalPriceDifference - priceDifference;
                amountCoPay = details.TotalCoPayment - amountCoPay;
                healthInsuranceRebate = details.TotalHIPayment - healthInsuranceRebate;
                discountAmt = details.TotalDiscountAmount - discountAmt;
            }


            if (amount == 0 && priceDifference == 0 && amountCoPay == 0 && healthInsuranceRebate == 0 && discountAmt == 0)
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
            tranDetail.PriceDifference = priceDifference;
            tranDetail.AmountCoPay = amountCoPay;
            tranDetail.HealthInsuranceRebate = healthInsuranceRebate;

            tranDetail.Discount = null;
            tranDetail.Qty = 1;
            tranDetail.RefDocID = null;
            tranDetail.ExchangeRate = null;
            tranDetail.TransItemRemarks = String.Empty;
            tranDetail.PCLRequestID = null;
            tranDetail.DiscountAmt = discountAmt;
            return tranDetail;
        }

        protected override decimal CalProgSumMinusMinHI()
        {
            decimal servicePrice = 0;
            decimal pclPrice = 0;
            decimal drugPrice = 0;

            if (CurrentRegistration.PatientRegistrationDetails != null)
            {
                servicePrice = CurrentRegistration.PatientRegistrationDetails
                                .Where(item => item.MarkedAsDeleted == false
                                                 && item.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                                                 && item.HIBenefit.GetValueOrDefault(-1) > 0)
                                .Sum(item => item.HIAllowedPrice.GetValueOrDefault(0));
            }
            if (CurrentRegistration.PCLRequests != null)
            {
                pclPrice = CurrentRegistration.PCLRequests.Where(request => request.MarkedAsDeleted == false
                    && request.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    .SelectMany(request => request.PatientPCLRequestIndicators)
                    .Where(reqDetail => reqDetail.MarkedAsDeleted == false
                    && reqDetail.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    .Sum(reqDetail => (decimal)(reqDetail.HIAllowedPrice.GetValueOrDefault(0) * reqDetail.Qty));
            }

            if (CurrentRegistration.DrugInvoices != null)
            {
                drugPrice = CurrentRegistration.DrugInvoices.Where(inv => !inv.ReturnID.HasValue && inv.V_OutDrugInvStatus != (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                    .SelectMany(inv => inv.OutwardDrugs)
                    .Sum(drugDetail => (decimal)((double)drugDetail.HIAllowedPrice.GetValueOrDefault(0) * (drugDetail.OutQuantity - drugDetail.OutQuantityReturn - drugDetail.QtyReturned)));
            }
            return servicePrice + pclPrice + drugPrice - MinPay;
        }

        private void CorrectHiRegistration(bool hiMinPayExceeded)
        {
            //*****************
            // Ham nay chua cap nhat lai phieu tra.
            // Se lam sau.
            //*****************/
            decimal totalHiPayment;
            decimal amountCoPay;

            if (CurrentRegistration.PatientRegistrationDetails != null)
            {
                foreach (var regDetail in CurrentRegistration.PatientRegistrationDetails)
                {
                    if (regDetail.HIAllowedPrice.HasValue && regDetail.HIAllowedPrice.Value > 0
                        && regDetail.HIBenefit.HasValue && regDetail.HIBenefit.Value > 0
                        && regDetail.MarkedAsDeleted == false
                        && regDetail.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    {
                        if (hiMinPayExceeded)
                        {
                            CalcInvoiceItemTotalHiPayment(regDetail, out totalHiPayment, out amountCoPay);
                        }
                        else
                        {
                            totalHiPayment = regDetail.HIAllowedPrice.Value;
                            amountCoPay = 0;
                        }

                        if (regDetail.TotalHIPayment != totalHiPayment || regDetail.TotalCoPayment != amountCoPay)
                        {
                            regDetail.TotalHIPayment = totalHiPayment;
                            regDetail.TotalCoPayment = amountCoPay;

                            if (regDetail.RecordState == RecordState.UNCHANGED)
                            {
                                regDetail.RecordState = RecordState.MODIFIED;
                            }
                        }
                    }
                }
            }

            if (CurrentRegistration.PCLRequests != null)
            {
                foreach (var request in CurrentRegistration.PCLRequests)
                {
                    if (!request.MarkedAsDeleted &&
                        request.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                    {
                        if (request.PatientPCLRequestIndicators != null)
                        {
                            foreach (var reqDetail in request.PatientPCLRequestIndicators)
                            {
                                if (!reqDetail.MarkedAsDeleted
                                    && reqDetail.HIBenefit.HasValue && reqDetail.HIBenefit.Value > 0
                                    && reqDetail.HIAllowedPrice.HasValue && reqDetail.HIAllowedPrice.Value > 0)
                                {
                                    if (hiMinPayExceeded)
                                    {
                                        CalcInvoiceItemTotalHiPayment(reqDetail, out totalHiPayment, out amountCoPay);
                                    }
                                    else
                                    {
                                        totalHiPayment = (decimal)(reqDetail.HIAllowedPrice.Value * reqDetail.Qty);
                                        amountCoPay = 0;
                                    }

                                    if (reqDetail.TotalHIPayment != totalHiPayment ||
                                        reqDetail.TotalCoPayment != amountCoPay)
                                    {
                                        reqDetail.TotalHIPayment = totalHiPayment;
                                        reqDetail.TotalCoPayment = amountCoPay;

                                        if (reqDetail.RecordState == RecordState.UNCHANGED)
                                        {
                                            reqDetail.RecordState = RecordState.MODIFIED;
                                        }
                                        //Update trang thai cho thang cha
                                        if (request.RecordState == RecordState.UNCHANGED)
                                        {
                                            request.RecordState = RecordState.MODIFIED;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (CurrentRegistration.DrugInvoices != null)
            {
                foreach (var inv in CurrentRegistration.DrugInvoices)
                {
                    //list tra thuoc
                    decimal totalcopayreturn = 0;
                    decimal totalhipayreturn = 0;
                    var ListReturn = inv.ReturnedInvoices;
                    //list can bang cua thuoc
                    var ListCB = CurrentRegistration.PatientTransaction != null ? CurrentRegistration.PatientTransaction.PatientTransactionDetails.Where(x => x.Amount == 0 && x.outiID == inv.outiID) : null;

                    //lam the nao de biet trong tran co add 1 dong can bang
                    if (!inv.ReturnID.HasValue)
                    {
                        if (inv.OutwardDrugs != null)
                        {
                            foreach (var drugDetail in inv.OutwardDrugs)
                            {
                                if (hiMinPayExceeded)//tren 15%
                                {
                                    CalcInvoiceItemTotalHiPayment(drugDetail, out totalHiPayment, out amountCoPay);

                                    if (drugDetail.TotalCoPayment > 0)
                                    //if (drugDetail.TotalHIPayment != totalHiPayment ||
                                    //   drugDetail.TotalCoPayment != amountCoPay)
                                    {
                                        drugDetail.TotalCoPayment = amountCoPay;
                                        drugDetail.TotalHIPayment = totalHiPayment;

                                        if (drugDetail.RecordState == RecordState.UNCHANGED)
                                        {
                                            drugDetail.RecordState = RecordState.MODIFIED;
                                        }
                                        //Update trang thai cho thang cha
                                        if (inv.RecordState == RecordState.UNCHANGED)
                                        {
                                            inv.RecordState = RecordState.MODIFIED;
                                        }
                                    }
                                }
                                else
                                {

                                    totalcopayreturn = 0;
                                    totalhipayreturn = 0;

                                    if (ListReturn != null && ListReturn.Count > 0)
                                    {
                                        foreach (var invreturn in ListReturn)
                                        {
                                            if (invreturn.OutwardDrugs != null)
                                            {
                                                foreach (var drugitem in invreturn.OutwardDrugs)
                                                {
                                                    if (drugitem.DrugID == drugDetail.DrugID && drugitem.InID == drugDetail.InID)
                                                    {
                                                        totalcopayreturn += drugitem.TotalCoPayment;
                                                        totalhipayreturn += drugitem.TotalHIPayment;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    Debug.Assert(drugDetail.HIAllowedPrice != null, "drugDetail.HIAllowedPrice != null");
                                    totalHiPayment = (drugDetail.HIAllowedPrice.Value * drugDetail.Qty);
                                    amountCoPay = 0;

                                    decimal HIchenhlech = 0;
                                    HIchenhlech = drugDetail.TotalHIPayment + totalhipayreturn;
                                    decimal CoPayChenhLech = 0;
                                    CoPayChenhLech = drugDetail.TotalCoPayment + totalcopayreturn;
                                    if (drugDetail.TotalHIPayment != totalHiPayment ||
                                        drugDetail.TotalCoPayment != amountCoPay)
                                    {
                                        if (HIchenhlech != 0)
                                        {
                                            drugDetail.TotalCoPayment = amountCoPay - totalcopayreturn;//amountCoPay;
                                            drugDetail.TotalHIPayment = drugDetail.TotalHIPayment + CoPayChenhLech;//totalHiPayment;

                                            if (drugDetail.RecordState == RecordState.UNCHANGED)
                                            {
                                                drugDetail.RecordState = RecordState.MODIFIED;
                                            }
                                            //Update trang thai cho thang cha
                                            if (inv.RecordState == RecordState.UNCHANGED)
                                            {
                                                inv.RecordState = RecordState.MODIFIED;
                                            }
                                        }
                                    }
                                }


                            }
                        }
                    }
                    //12346565656565656
                    //neu truoc do co can bang thuoc thi phai can bang lai
                    if (ListCB != null && ListCB.Count() > 0)
                    {
                        if (inv.RecordState == RecordState.UNCHANGED)
                        {
                            inv.RecordState = RecordState.MODIFIED;
                        }
                    }

                }
            }
        }

        private void CalcInvoiceItemTotalHiPayment(IInvoiceItem item, out decimal totalHiPay, out decimal totalCoPay)
        {
            Debug.Assert(item.HIAllowedPrice != null, "item.HIAllowedPrice != null");
            Debug.Assert(item.HIBenefit != null, "item.HIBenefit != null");
            decimal d = item.HIAllowedPrice.Value * (decimal)item.HIBenefit.Value;
            totalHiPay = Math.Floor((d * item.Qty));
            totalCoPay = Math.Ceiling(((item.HIAllowedPrice.Value - d) * item.Qty));
        }

        protected override void GetItemPrice(IInvoiceItem invoiceItem, PatientRegistration registration)
        {
            invoiceItem.GetItemPrice(registration, registration.PtInsuranceBenefit.GetValueOrDefault(0.0), null, true, Globals.AxServerSettings.HealthInsurances.FullHIBenefitForConfirm, Globals.AxServerSettings.HealthInsurances.HiPolicyMinSalary);
        }

        private decimal _progSumMinusMinHI;
        protected override void OnInit(long V_RegistrationType = 0)
        {
            _progSumMinusMinHI = CurrentRegistration.ProgSumMinusMinHI.GetValueOrDefault(0);
            
            //Khi init neu la dang ky moi thi tinh quyen loi bao hiem
            if (CurrentRegistration.PtRegistrationID > 0 || CurrentRegistration.HealthInsurance == null)
            {
                return;
            }
            bool bConsiderAsCrossRegion;
            //Lay the bao hiem active. Neu khong duoc bao loi luon.
            long? hisID = PatientProvider.Instance.GetActiveHisID(CurrentRegistration.HealthInsurance.HIID);
            if (hisID == null)
            {
                throw new Exception(eHCMSResources.Z1794_G1_CannotLoadHIItem);
            }
            CurrentRegistration.HisID = hisID;
            bool bIsEmergInPtReExamination = CurrentRegistration.EmergInPtReExamination.HasValue ? CurrentRegistration.EmergInPtReExamination.Value : false;
            var hiBenefit = CalcPatientHiBenefit(CurrentRegistration, CurrentRegistration.HealthInsurance, out bConsiderAsCrossRegion, V_RegistrationType, bIsEmergInPtReExamination);
            CurrentRegistration.PtInsuranceBenefit = hiBenefit;
            if (!CurrentRegistration.IsCrossRegion.HasValue)
            {
                CurrentRegistration.IsCrossRegion = bConsiderAsCrossRegion;
            }
        }

        //private void CorrectRegistrationDetails()
        //{
        //    //Có áp dụng luật Bảo hiểm đối với dịch vụ KCB hay không (Đối với tất cả các dịch vụ KCB, bảo hiểm chỉ tính 1 dịch vụ thôi, còn lại là không có BH)
        //    if (!Globals.AxServerSettings.HealthInsurances.SpecialRuleForHIConsultationApplied)
        //    {
        //        return;
        //    }
        //    if (CurrentRegistration.PatientRegistrationDetails == null)
        //    {
        //        return;
        //    }
        //    //Tinh tong so dich vu KCB duoc bao hiem thanh toan, neu tong so nay > 1 thi phai tinh lai
        //    //(Chi co 1 dich vu KCB duoc tinh bao hiem)
        //    //Lay tat ca cac dich vu KCB co bao hiem
        //    IList<PatientRegistrationDetail> hiRegDetails = CurrentRegistration.PatientRegistrationDetails.Where(registrationDetail =>
        //                //registrationDetail.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH
        //                //&& 
        //                registrationDetail.RecordState != RecordState.DELETED
        //                && !registrationDetail.MarkedAsDeleted
        //                && registrationDetail.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
        //                && registrationDetail.HisID.HasValue && registrationDetail.HisID.Value > 0
        //                && registrationDetail.HIAllowedPrice.HasValue
        //                && registrationDetail.HIAllowedPrice.Value > 0).ToList();
        //    //Tinh tong so nhung dich vu bao hiem chap nhan tinh
        //    Func<PatientRegistrationDetail, bool> hiAcceptedRegDetails = registrationDetail => registrationDetail.TotalHIPayment > 0;
        //    //BH chi tinh cho 1 dich vu KCB thoi
        //    var total = hiRegDetails.Where(hiAcceptedRegDetails).Count();
        //    if (hiRegDetails != null && hiRegDetails.Count > 0)
        //    {
        //        /*
        //        if (total == 0)
        //        {                                        
        //            var registrationDetail = hiRegDetails.FirstOrDefault();
        //            var totalHiPayment = registrationDetail.TotalHIPayment;
        //            var totalCoPayment = registrationDetail.TotalCoPayment;
        //            var hisID = registrationDetail.HisID;
        //            var benefit = registrationDetail.HIBenefit;
        //            if (registrationDetail.ID > 0 && registrationDetail.HisID.HasValue
        //                && registrationDetail.HisID.Value > 0
        //                && registrationDetail.HIAllowedPrice.HasValue
        //                && registrationDetail.HIAllowedPrice.Value > 0)//Co su dung the bh
        //            {
        //                registrationDetail.HIBenefit = CurrentRegistration.PtInsuranceBenefit;
        //            }
        //            GetItemPrice(registrationDetail, CurrentRegistration);
        //            GetItemTotalPrice(registrationDetail);
        //            if (registrationDetail.TotalCoPayment != totalCoPayment
        //                || registrationDetail.TotalHIPayment != totalHiPayment
        //                || hisID != registrationDetail.HisID
        //                || benefit != registrationDetail.HIBenefit)
        //            {
        //                if (registrationDetail.RecordState == RecordState.UNCHANGED)
        //                {
        //                    registrationDetail.RecordState = RecordState.MODIFIED;
        //                }
        //            }
        //        }
        //        else*/ 
        //        if (total > 1)
        //        {
        //            if (Globals.AxServerSettings.HealthInsurances.HIPercentOnDifDept != null)
        //            {
        //                List<long> mDeptMedItems = new List<long>();
        //                foreach (var registrationDetail in hiRegDetails)
        //                {
        //                    if ((registrationDetail.DeptLocation != null && mDeptMedItems.Contains(registrationDetail.DeptLocation.DeptID))
        //                    || (registrationDetail.RefMedicalServiceItem.allDeptLocation != null && registrationDetail.RefMedicalServiceItem.allDeptLocation.Any(x => mDeptMedItems.Contains(x.DeptID)))
        //                    || (registrationDetail.DeptLocation == null && registrationDetail.RefMedicalServiceItem.allDeptLocation == null))
        //                    {
        //                        var totalHiPayment = registrationDetail.TotalHIPayment;
        //                        var totalCoPayment = registrationDetail.TotalCoPayment;
        //                        registrationDetail.HIBenefit = 0;
        //                        registrationDetail.TotalCoPayment = 0;
        //                        registrationDetail.TotalHIPayment = 0;
        //                        registrationDetail.TotalPatientPayment = registrationDetail.TotalInvoicePrice;
        //                        registrationDetail.TotalPriceDifference = registrationDetail.TotalPatientPayment;
        //                        if (registrationDetail.TotalCoPayment != totalCoPayment || registrationDetail.TotalHIPayment != totalHiPayment)
        //                        {
        //                            if (registrationDetail.RecordState == RecordState.UNCHANGED)
        //                            {
        //                                registrationDetail.RecordState = RecordState.MODIFIED;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (mDeptMedItems.Count > 0)
        //                        {
        //                            var totalHiPayment = registrationDetail.TotalHIPayment;
        //                            var totalCoPayment = registrationDetail.TotalCoPayment;
        //                            if (Globals.AxServerSettings.HealthInsurances.HIPercentOnDifDept.Length > mDeptMedItems.Count - 1)
        //                            {
        //                                registrationDetail.HIBenefit = Math.Round(Globals.AxServerSettings.HealthInsurances.HIPercentOnDifDept[mDeptMedItems.Count - 1], 4);
        //                                GetItemPrice(registrationDetail, CurrentRegistration, registrationDetail.HIBenefit.Value);
        //                                GetItemTotalPrice(registrationDetail);
        //                            }
        //                            else
        //                            {
        //                                registrationDetail.HIBenefit = 0;
        //                                registrationDetail.TotalCoPayment = 0;
        //                                registrationDetail.TotalHIPayment = 0;
        //                                registrationDetail.TotalPatientPayment = registrationDetail.TotalInvoicePrice;
        //                                registrationDetail.TotalPriceDifference = registrationDetail.TotalPatientPayment;
        //                            }
        //                            if (registrationDetail.TotalCoPayment != totalCoPayment || registrationDetail.TotalHIPayment != totalHiPayment)
        //                            {
        //                                if (registrationDetail.RecordState == RecordState.UNCHANGED)
        //                                {
        //                                    registrationDetail.RecordState = RecordState.MODIFIED;
        //                                }
        //                            }
        //                        }
        //                        if (registrationDetail.RefMedicalServiceItem.allDeptLocation != null)
        //                            mDeptMedItems.AddRange(registrationDetail.RefMedicalServiceItem.allDeptLocation.Where(x => !mDeptMedItems.Contains(x.DeptID)).Select(x => x.DeptID).Distinct());
        //                        else if (registrationDetail.DeptLocation != null)
        //                            mDeptMedItems.Add(registrationDetail.DeptLocation.DeptID);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                var firstItem = hiRegDetails.First(hiAcceptedRegDetails);
        //                //Thang dau tien khong tinh.
        //                foreach (var registrationDetail in hiRegDetails.Where(item => item != firstItem))
        //                {
        //                    var totalHiPayment = registrationDetail.TotalHIPayment;
        //                    var totalCoPayment = registrationDetail.TotalCoPayment;
        //                    registrationDetail.HIBenefit = 0;
        //                    registrationDetail.TotalCoPayment = 0;
        //                    registrationDetail.TotalHIPayment = 0;
        //                    registrationDetail.TotalPatientPayment = registrationDetail.TotalInvoicePrice;
        //                    registrationDetail.TotalPriceDifference = registrationDetail.TotalPatientPayment;
        //                    if (registrationDetail.TotalCoPayment != totalCoPayment
        //                        || registrationDetail.TotalHIPayment != totalHiPayment)
        //                    {
        //                        if (registrationDetail.RecordState == RecordState.UNCHANGED)
        //                        {
        //                            registrationDetail.RecordState = RecordState.MODIFIED;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    //Total =1 la OK
        //}

        protected override void OnRegistrationSaving()
        {
            //CorrectRegistrationDetails();
        }

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

            // TxD 24/12/2013 : Commented the following to allow for Mod OutwardDrugInvoice to be paid 
            //                  in order to set the previous Invoices correctly
            //                  IF paymentDetails.PayAmount = 0 IS AN ISSUE ELSE WHERE then it should be reviewed and Change should be made accordingly
            //if (paymentDetails.PayAmount == 0 )
            //{
            //    if (paymentDetails.HiDelegation == false)
            //    {
            //        errorMessage = "Thông tin nhập vào không đúng.";
            //        return false;
            //    }
            //}

            if (!paymentDetails.PaymentDate.HasValue || paymentDetails.PaymentDate.Value == DateTime.MinValue)
            {
                paymentDetails.PaymentDate = DateTime.Now;
            }
            return true;
        }
        protected override void OnTransactionBalancing(int Apply15HIPercent)
        {
            FunctionBase.CorrectRegistrationDetails(CurrentRegistration);
            //Kiem tra bien ProgSumHi truoc va sau khi modify dang ky, co thay doi ( do co vu 15% BH) thi sua lai.
            CorrectHiRegistration(Apply15HIPercent);
        }

        private void CorrectHiRegistration(int Apply15HIPercent) //0:la khong tinh,1:la co tinh
        {
            if (!CurrentRegistration.IsCrossRegion.GetValueOrDefault(false) && Apply15HIPercent == 1)
            {
                var oldProgMinusHi = _progSumMinusMinHI;

                var curProgSumMinusMinHi = CurrentRegistration.ProgSumMinusMinHI.GetValueOrDefault(0);
                if (oldProgMinusHi < 0)
                {
                    if (curProgSumMinusMinHi < 0)
                    {
                        CorrectHiRegistration(false);
                        return;
                    }
                    CorrectHiRegistration(true);
                    return;
                }
                if (curProgSumMinusMinHi < 0)
                {
                    CorrectHiRegistration(false);
                    return;
                }
            }
        }

        public override bool AddServicesAndPCLRequests(long StaffID,long CollectorDeptLocID, int? Apply15HIPercent, PatientRegistration regInfo, List<PatientRegistrationDetail> regDetailList
                            , List<PatientPCLRequest> pclRequestList, List<PatientRegistrationDetail> deletedRegDetailList, List<PatientPCLRequest> deletedPclRequestList
                            , DateTime modifiedDate, bool IsNotCheckInvalid, bool IsCheckPaid, out long newRegistrationID
            //▼====: #009
            , out List<long> newRegistrationIDList, out List<long> newPclRequestList, bool IsProcess = false
            , bool IsNotLoadRegis = false, string TranPaymtNote = null, long? V_PaymentMode = null, bool IsFromRequestDoctor = false, long? V_ReceiveMethod = null)
            //▲====: #009
        {
            try
            {
                List<long> newPaymentIDList;
                List<long> newOutwardDrugIDList;
                List<long> billingInvoiceIDs;
                string PaymentIDListNy;
                if (!IsNotLoadRegis)
                {
                    AddRegistrationDetailsToCurrentRegistration(deletedRegDetailList);
                    AddRegistrationDetailsToCurrentRegistration(regDetailList);
                    AddPclRequestsToCurrentRegistration(GetNewPclList(pclRequestList, deletedPclRequestList));
                    AddPclRequestsToCurrentRegistration(deletedPclRequestList);
                    FunctionBase.CorrectRegistrationDetails_V2(CurrentRegistration);
                }
                else
                {
                    AddPclRequestsToCurrentRegistration(GetNewPclList(pclRequestList, deletedPclRequestList));
                    //AddPclRequestsToCurrentRegistration(deletedPclRequestList);
                }
                SaveRegistrationForOutPatient(StaffID, CollectorDeptLocID, Apply15HIPercent, modifiedDate, null, out newRegistrationID, out newRegistrationIDList, out newPclRequestList, out newPaymentIDList, out newOutwardDrugIDList, out billingInvoiceIDs, out PaymentIDListNy, 0, null, null, false, false, null, null, false, IsNotCheckInvalid, IsCheckPaid, false, IsProcess,
                    //▼====: #011
                    false, null, null, IsFromRequestDoctor, V_ReceiveMethod
                    //▲====: #011
                );
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}