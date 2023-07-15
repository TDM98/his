using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
/*
 * 20220526 #001 TNHX: 646 Tách STT theo yêu cầu    V_InstructionOrdinalType
    + Nhóm hướng thần gây nghiện + hướng thần (84602 + 84604) : theo ngày đánh riêng hoạt chất. ngày thì đánh dấu lại
    + Nhóm Corticoid (84603): Đánh dấu STT theo các hoạt chất cùng nhóm. qua ngày đánh dấu lại
    + Nhóm kháng sinh (84601): liên tục theo ngày y lệnh - đánh riêng hoạt chất.  
 */
namespace aEMR.Infrastructure
{
    //---------------------- LTN -----------------------
    //15/8/2015 --- khai bao gia muc gia tri don hang de tang giftcard, va itempk mac dinh cho giftcard duoc tang do
    public class SaleAmountToGiftCardPk
    {
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public long itempk { get; set; }
        public string itemkey { get; set; }
    }
    //---------------------- LTN -----------------------

    // TxD 21/12/2015 Added new Rule for new type of discount to be applied to full price items that have total exceeding a configurable amount
    public class SaleAmountToApplySpecialDiscount
    {
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public decimal DiscountPercent{ get; set; }        
    }

    
    public static partial class Globals
    {
        //public static StaffDto LoggedUser { get; set; }
        //public static Site SiteLogged { get; set; }
        //public static WarehouseDto WarehouseDto { get; set; }

        // TxD 26/05/2018: Concatinated 2 Globals classes so the following Keys are commented out
        //                  to use the one with Health Care 
        //public static string AxonKey = "EPos123*&^Axon";
        //public static string AxonPass = "~Tuan$Thie3nD1ao";

        //public static List<StaffGroupDto> userAuth;

        public static void checkAuthen() 
        {

        }

        //public static string[] AssemblyList = {
        //                                          "Caliburn.Micro.dll",
        //                                          "aEMR.DataContracts.dll",
        //                                          "aEMR.ServiceClient.dll",
        //                                          "aEMR.Infrastructure.dll",
        //                                          "aEMR.ViewContracts.dll",
        //                                          "EPos.ViewContracts.dll",                                                  
        //                                          "EPos.BusinessLayers.dll",
        //                                          "EPos.Inventory.dll",
        //                                          "EPos.Reports.dll",                                                  
        //                                          "EPos.SaleOrder.dll",
        //                                          "EPos.Settings.dll",
        //                                          "EPos.CommonViews.dll",
        //                                          "EPos.UserManage.dll",
        //                                      };

        public static string[] AssemblyList = {
                                                  "Caliburn.Micro.dll",
                                                  "aEMR.DataContracts.dll",
                                                  "aEMR.ServiceClient.dll",
                                                  "aEMR.Infrastructure.dll",
                                                  "aEMR.ViewContracts.dll",
                                                  "aEMR.CommonViews.dll",
                                                  "aEMR.Registration.dll",
                                                  "aEMR.Consultation.dll",
                                                  "aEMR.DrugDept.dll",
                                                  "aEMR.Appointment.dll",
                                                  "aEMR.PCLDepartment.dll",
                                                  "aEMR.Pharmacy.dll",
                                                  "aEMR.StoreDept.dll",
                                                  "aEMR.TransactionManager.dll",
                                                  "aEMR.Configuration.dll",
                                                  "aEMR.ResourceMaintenance.dll",
                                                  "aEMR.UserAccountManagement.dll",
                                                  "aEMR.ClinicManagement.dll"
                                              };

        //public static ObservableCollection<CategoryDto> Categories { get; set; }

        //public static ObservableCollection<SupplierDto> Suppliers { get; set; }

        //public static List<ItemtypeInstanceDto> allItemtypeInstance { get; set; }

        //public static ObservableCollection<FieldValueDto> allColor { get; set; }

        //public static ObservableCollection<FieldValueDto> allStyle { get; set; }

        //public static ObservableCollection<FieldValueDto> allSize { get; set; }

        //public static ObservableCollection<SiteDto> allSite { get; set; }

        //public static ObservableCollection<StaffDto> allStaff { get; set; }
        //---------------------- LTN -----------------------

        //----15.8.2015 tao 1 list cac gia tri don hang, neu nhu mua hang ma thuoc muc gia tri nao se duoc tang gift card co gia tri tuong ung voi muc do theo quy dinh.
        // ban dau moi de 2 muc, sau nay muon them, chi can them trong csdl, khng can phai vo day sua.
        public static List<SaleAmountToGiftCardPk> SaleAmountToItemPkList = null;

        // TxD 21/12/2015 Added new Rule for new type of discount to be applied to full price items that have total exceeding a configurable amount
        public static List<SaleAmountToApplySpecialDiscount> SaleAmountToApplySpecialDiscountList = null;

        //-------------- ngay gioi han cua giftcard & creditnot la 60 ngay
        private static int _expirydateforGiftcard = 90;

        public static int ExpirydateforGiftcard
        {
            get { return _expirydateforGiftcard; }
            set
            {
                _expirydateforGiftcard = value;
            }
        }

        //---------- category = 34 thi biet la dang ba gift card cho khach hang
        private static int _categorypkForPayGiftcard = 34;
        public static int CategorypkForPayGiftcard
        {
            get { return _categorypkForPayGiftcard; }
            set
            {
                _categorypkForPayGiftcard = value;
            }
        }
        // LTN --- end----------------
        private static string _TitleForm;
        public static string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
            }
        }

        private static string _HIRegistrationForm;
        public static string HIRegistrationForm
        {
            get { return _HIRegistrationForm; }
            set
            {
                _HIRegistrationForm = value;
            }
        }
        public static decimal DecimalRound(decimal aValue, int aRoundNumber)
        {
            if (aRoundNumber == 0 && aValue % 1 == 0.5m)
            {
                return decimal.Ceiling(aValue);
            }
            else
            {
                return decimal.Round(aValue, aRoundNumber);
            }
        }
        public static void OpenFileWithWindowsExpore(string aFilePath)
        {
            ProcessStartInfo pi = new ProcessStartInfo(aFilePath);
            pi.Arguments = Path.GetFileName(aFilePath);
            pi.UseShellExecute = true;
            pi.WorkingDirectory = Path.GetDirectoryName(aFilePath);
            pi.FileName = aFilePath;
            pi.Verb = "OPEN";
            Process.Start(pi);
        }
        public static void SaveBase64StringToFile(string aBase64Data, string aFilePath)
        {
            try
            {
                using (FileStream mStream = new FileStream(aFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    byte[] pdfBytes = Convert.FromBase64String(aBase64Data);
                    mStream.Write(pdfBytes, 0, pdfBytes.Length);
                    mStream.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static System.Windows.Size GetDefaultDialogViewSize()
        {
            return new System.Windows.Size(System.Windows.SystemParameters.PrimaryScreenWidth - 100, System.Windows.SystemParameters.PrimaryScreenHeight - 50);
        }

        public static System.Windows.Size GetHalfHeightAndThreeFourthWidthDefaultDialogViewSize()
        {
            return new System.Windows.Size((System.Windows.SystemParameters.PrimaryScreenWidth - 100) * 3/4, (System.Windows.SystemParameters.PrimaryScreenHeight - 50)/2);
        }

        public static System.Windows.Size GetTwoFourthWidthDefaultDialogViewSize()
        {
            return new System.Windows.Size((System.Windows.SystemParameters.PrimaryScreenWidth - 100) * 2 / 4, (System.Windows.SystemParameters.PrimaryScreenHeight - 50));
        }

        //20220830 TNHX: 2168 Thêm cấu hình thời gian tìm kiếm cho KSK
        public static System.Windows.Size GetPCLSearchPatientKSKDialogViewSize()
        {
            return new System.Windows.Size(1400, 600);
        }

        //CMN: Thêm biến Enable Task 1192 tắt popup hẹn bệnh khi ra toa và cho phép tạo hẹn bệnh tái khám bằng popup
        public const bool IsUse1192 = true;
        //List chức vụ tại bệnh viện
        public static IList<PositionInHospital> AllPositionInHospital { get; set; }
        //▼====: #001
        public static void AutoSetAntibioticIndex(IList<ReqOutwardDrugClinicDeptPatient> AntibioticTreatmentUsageHistories
            , ReqOutwardDrugClinicDeptPatient CurrentItem
            , DateTime? DefaultInstructionDate)
        {
            //Tự động đánh số thứ tự cho các thuốc bắt buộc nhập STT. Dựa vào V_InstructionOrdinalType
            // Yêu cầu đánh theo hoạt chất
            if (AntibioticTreatmentUsageHistories != null && CurrentItem.RefGenericDrugDetail != null
                && CurrentItem.RefGenericDrugDetail.V_InstructionOrdinalType != (long)AllLookupValues.V_InstructionOrdinalType.Thuong)
            {
                var CurrentMedicalInstructionDate = CurrentItem.MedicalInstructionDate.GetValueOrDefault(DefaultInstructionDate.GetValueOrDefault(Globals.GetCurServerDateTime())).Date;
                // do không tính theo hoạt chất mà theo nhóm nên tách riêng phần này
                if (CurrentItem.RefGenericDrugDetail.V_InstructionOrdinalType == (long)AllLookupValues.V_InstructionOrdinalType.KhangViemCorticoid)
                {
                    var LastAntibioticTreatmentKVCorticoid = AntibioticTreatmentUsageHistories.OrderByDescending(x => x.MedicalInstructionDate)
                                    .OrderByDescending(x => x.OutClinicDeptReqID)
                                    .FirstOrDefault(x => x.RefGenericDrugDetail.V_InstructionOrdinalType == CurrentItem.RefGenericDrugDetail.V_InstructionOrdinalType
                                        && x.MedicalInstructionDate.HasValue);
                    if (LastAntibioticTreatmentKVCorticoid != null)
                    {
                        if (LastAntibioticTreatmentKVCorticoid.MedicalInstructionDate.Value.Date == CurrentMedicalInstructionDate)
                        {
                            CurrentItem.AntibioticOrdinal = LastAntibioticTreatmentKVCorticoid.AntibioticOrdinal;
                        }
                        else if ((CurrentMedicalInstructionDate - LastAntibioticTreatmentKVCorticoid.MedicalInstructionDate.Value.Date).TotalDays == 1
                            || (CurrentMedicalInstructionDate - LastAntibioticTreatmentKVCorticoid.MedicalInstructionDate.Value.Date).TotalDays <= CurrentItem.RefGenericDrugDetail.MinDayOrdinalContinueIsAllowable)
                        {
                            CurrentItem.AntibioticOrdinal = LastAntibioticTreatmentKVCorticoid.AntibioticOrdinal + 1;
                        }
                        else
                        {
                            CurrentItem.AntibioticOrdinal = 1;
                        }
                    }
                    else
                    {
                        CurrentItem.AntibioticOrdinal = 1;
                    }
                }
                else if (!AntibioticTreatmentUsageHistories.Any(x => x.RefGenericDrugDetail.GenericID == CurrentItem.RefGenericDrugDetail.GenericID && x.MedicalInstructionDate.HasValue))
                {
                    CurrentItem.AntibioticOrdinal = 1;
                }
                else
                {
                    var LastAntibioticTreatment = AntibioticTreatmentUsageHistories.OrderByDescending(x => x.MedicalInstructionDate)
                        .OrderByDescending(x => x.OutClinicDeptReqID)
                        .FirstOrDefault(x => x.RefGenericDrugDetail.GenericID == CurrentItem.RefGenericDrugDetail.GenericID && x.MedicalInstructionDate.HasValue);
                    // Kiểm tra STT của thuốc theo y lệnh gần nhất để xài chung STT
                    if (LastAntibioticTreatment.MedicalInstructionDate.Value.Date == CurrentMedicalInstructionDate)
                    {
                        CurrentItem.AntibioticOrdinal = LastAntibioticTreatment.AntibioticOrdinal;
                    }
                    else
                    {
                        // nhóm gây nghiện/ hướng thần theo y cũ nên để default
                        switch (CurrentItem.RefGenericDrugDetail.V_InstructionOrdinalType)
                        {
                            case ((long)AllLookupValues.V_InstructionOrdinalType.KhangSinh):
                                if ((CurrentMedicalInstructionDate - LastAntibioticTreatment.MedicalInstructionDate.Value.Date).TotalDays > 0)
                                {
                                    CurrentItem.AntibioticOrdinal = LastAntibioticTreatment.AntibioticOrdinal + (int)(CurrentMedicalInstructionDate - LastAntibioticTreatment.MedicalInstructionDate.Value.Date).TotalDays;
                                }
                                else
                                {
                                    CurrentItem.AntibioticOrdinal = 1;
                                }
                                break;
                            default:
                                if ((CurrentMedicalInstructionDate - LastAntibioticTreatment.MedicalInstructionDate.Value.Date).TotalDays == 1
                                    || (CurrentMedicalInstructionDate - LastAntibioticTreatment.MedicalInstructionDate.Value.Date).TotalDays <= CurrentItem.RefGenericDrugDetail.MinDayOrdinalContinueIsAllowable)
                                {
                                    CurrentItem.AntibioticOrdinal = LastAntibioticTreatment.AntibioticOrdinal + 1;
                                }
                                else
                                {
                                    CurrentItem.AntibioticOrdinal = 1;
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                CurrentItem.AntibioticOrdinal = null;
            }
        }
        //▲====: #001

        public static T GetFirstChildByType<T>(DependencyObject prop) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(prop); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild((prop), i) as DependencyObject;
                if (child == null)
                    continue;

                T castedProp = child as T;
                if (castedProp != null)
                    return castedProp;

                castedProp = GetFirstChildByType<T>(child);

                if (castedProp != null)
                    return castedProp;
            }
            return null;
        }

        public static System.Windows.Size GetDefaultDialogViewSizeInput(double width, double height)
        {
            return new System.Windows.Size(width, height);
        }

        //▼====: 20220530 BLQ: Kiểm tra thời gian thao tác của bác sĩ
        private static DateTime? _StartDatetimeExam = null;
        public static DateTime? StartDatetimeExam
        {
            get { return _StartDatetimeExam; }
            set
            {
                _StartDatetimeExam = value;
            }
        }

        public static bool CheckDoctorContactPatientTime()
        {
            string ListLocation = ServerConfigSection.CommonItems.LocationNotCheckDoctorContactPatientTime;
            string curLocation = "|" + Globals.DeptLocation.LID.ToString() + "|";
            if (!String.IsNullOrEmpty(ListLocation) && ListLocation.Contains(curLocation))
            {
                return true;
            }
            DateTime CurrentTime = GetCurServerDateTime(); 
            if(StartDatetimeExam == null)
            {
                StartDatetimeExam = GetCurServerDateTime();
            }
            if(ServerConfigSection.CommonItems.DoctorContactPatientTime != 0 
                && (CurrentTime - StartDatetimeExam).Value.TotalSeconds < ServerConfigSection.CommonItems.DoctorContactPatientTime)
            {
                ShowMessage("Bác sĩ vui lòng tư vấn thêm thông tin cho người bệnh", eHCMSResources.G0442_G1_TBao);
                return false;
            }
            else
            {
                return true;
            }
        }
        //▲====: 20220530
    }
}