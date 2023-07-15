using System;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using eHCMS.Services.Core.Base;
using System.ComponentModel;
using eHCMS.Services.Core;

namespace DataEntities
{
    public class ConfigValues
    {
        public static int SessionTimeOut;
        public static string SMTPServer;
        public static int ExpTimeoutForOPTrans;
        public static int ExpTimeoutForIPTrans;
        public static string DocumentRootPath;
        public static string ParaClinicalExamFolder;
        public static int PeriodOfBackupTrans;
        public static string BackupTransFolder;
        public static string HosLogoImagePath;
        public static int TypeOfIssuePrescription;
        public static int BillPrintedCases;
        public static int NumberOfSeqPerDay;
        public static string PrefixNationalMedicalCode;

     
        /// Thời gian hết hạn cho một đăng ký được mở. Tính theo ngày
     
        public static int PatientRegistrationTimeout;
     
        /// Tháng lương tối thiểu
     
        public static decimal HIPolicyMinSalary = 830000;
     
        /// Phần trăm tháng lương tối thiểu
     
        public static float HIPolicyPercentageOnPayable = 0.15F;

     
        /// Ma benh vien (de tinh BHYT vuot tuyen)
     
        public static string HospitalCode = "89339";

        public static double RebatePercentageLevel_1 = 0.3;
    }
}
