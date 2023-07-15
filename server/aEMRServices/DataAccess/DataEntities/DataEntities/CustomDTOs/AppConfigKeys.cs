using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace AppConfigKeys
{
    public enum ConfigItemKey
    {
        SessionTimeOut=0,
        SMTPServer=1,
        ExpTimeoutForOPTrans=2,
        ExpTimeoutForIPTrans=3,
        DocumentRootPath=4,
        ParaClinicalExamFolder=5,
        PeriodOfBackupTrans=6,
        BackupTransFolder=7,
        HosLogoImagePath=8,
        TypeOfIssuePrescription=9,
        BillPrintedCases=10,
        NumberOfSeqPerDay=11,
        HospitalCode=12,
        HIPolicyMinSalary=13,
        HIPolicyPercentageOnPayable=14,
        PCLStorePool=15,
        PCLResourcePool=16,
        RptPCLLabResultWithAllHis=17,
        PCLThumbTemp=18,
        FileConsultation=19,
        HospitalName=20,
        NeedICD10=21,
        HIRebatePercentageLevel1=22,
        PCLImageStoragePath=23,
        PharmacyOutDrugFIFORule=24,
        PharmacyOutDrugExpiryDateRule=25,
        PharmacyUseSellingPriceList=26,
        PrefixNationalMedicalCode=27,
        OutstandingServerIP=28,
        OutstandingServerPort=29,

        DrugDeptOutDrugFIFORule=30,
        DrugDeptOutDrugExpiryDateRule=31,
        DrugDeptUseSellingPriceList=32,
        PharmacyDefaultVATInward=33,
        PharmacyMaxDaysHIRebate_NgoaiTru=34,
        PaperReferalMaxDays=35,
        //-----

        NetWorkMapDriver=36,
        NWMDUser=37,
        NWMDPass=38,
        LocalFolderName=39,

        RegistrationVIP = 40,// Cho phép khám bệnh VIP(khám không cần đăng ký)
        HISearchRegEx = 41,

        SpecialRuleForHIConsultationApplied=42,
        HIConsultationServiceHIAllowedPrice=43,
        SequenceNumberTypeAPrefix=44,
        SequenceNumberTypeBPrefix=45,
        SequenceNumberType_5=46,
        SequenceNumberType_10=47,
        SequenceNumberType_25=48,

        ReceiptPrintingMode=49,
        NumberTypePrescriptions_Rule=50,// 1 : chỉ 1 loại số không BH; 2: 2 loại số BH và không BH
        PharmacyMaxDaysHIRebate_NoiTru=51,
        FindRegistrationInDays_NgoaiTru=52,
        PharmacyCountMoneyIndependent = 53,
        MinPatientCashAdvance=54,
        ReceiptForEachLocationPrintingMode=55,
        RefundOrCancelCashReceipt=56,
        Apply15HIPercent = 57,
        AllowedPharmacyChangeHIPrescript=58,
        PrefixCodeMedical=59,
        PrefixCodeMachine=60,
        PrefixCodeChemical=61,
        PrefixCodeDrug=62,
        AllowDuplicateMedicalServiceItems=63,
        AllowDuplicatePCLExamType=64,
        DifferenceDayPrecriptHI = 65,
        DifferenceDayRegistrationHI = 66,
        StaffCatTypeBAC_SI=67,
        MaxDaySellPrescriptInsurance=68,
        KhoaPhongKham=69,
        EffectedDiagHours=70,
        EditDiagDays=71,
        EffectedPCLHours=72,
        RoomFunction = 73,
        LaboRmTp=74,
        ExcelStorePool=75,
        IsConfirmHI= 76,//103
        ShowAddRegisButton=77, //105
    }

    //public class ConfigItemKeycs
    //{
    //    public static string GetConfigItemKeyName(ConfigItemKey cik)
    //    {
    //        switch (cik)
    //        {
    //            case ConfigItemKey.NetWorkMapDriver:
    //                return ""; break;
    //            case ConfigItemKey.NWMDUser:
    //                return ""; break;
    //            case ConfigItemKey.NWMDPass:
    //                return ""; break;
    //            case ConfigItemKey.LocalFolderName:
    //                return ""; break;




    //        }
    //    }
    //}
    
}
