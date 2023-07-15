using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Linq;
/*
 * 20190510 #001 TTM: BM 0006681: Chuyển sang sử dụng bảng InsuranceBenefitCategories để quản lý thẻ và quyền lợi của bệnh nhân. 
 */
namespace eHCMS.Services.Core
{
    public static partial class AxHelper
    {

        public const string EmailRegularExpression = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        ///// <summary>
        ///// Extract ma so the bao hiem y te
        ///// </summary>
        ///// <param name="strMaThe">Ma so ghi tren the bao hiem (gom 15 chu so)</param>
        ///// /// <param name="strDoiTuong">Ma Doi Tuong</param>
        ///// <param name="strQuyenLoi">Ma Quyen loi</param>
        ///// <param name="strTinh">Ma Tinh</param>
        ///// <param name="strQuanHuyen">Ma Quan, huyen</param>
        ///// <param name="strDonVi">Ma Don vi</param>
        ///// <param name="strSTT">So thu tu nguoi tham gia</param>
        ///// <returns>Tra ve True neu chuoi strMaThe nhap vao hop le. Neu khong, tra ve False</returns>
        //public static bool ExtractHICardNumber(string strMaThe, out string strDoiTuong,out string strQuyenLoi, out string strTinh,out string strQuanHuyen, out string strDonVi, out string strSTT)
        //{
        //    strDoiTuong = "";
        //    strQuyenLoi = "";
        //    strTinh = "";
        //    strQuanHuyen = "";
        //    strDonVi = "";
        //    strSTT = "";

        //    bool bOK = false;

        //    if(string.IsNullOrWhiteSpace(strMaThe))
        //    {
        //        return bOK;
        //    }

        //    string pattern = @"^([a-zA-Z]{2})(\d{1})(\d{2})(\d{2})(\d{3})(\d{5})$";
            
        //    Regex regEx = new Regex(pattern);

        //    Match match = regEx.Match(strMaThe);
        //    if (match.Success)
        //    {
        //        strDoiTuong = match.Groups[1].Value;
        //        strQuyenLoi = match.Groups[2].Value;
        //        strTinh = match.Groups[3].Value;
        //        strQuanHuyen = match.Groups[4].Value;
        //        strDonVi = match.Groups[5].Value;
        //        strSTT = match.Groups[6].Value;

        //        bOK = true;
        //    }

        //    return bOK;
        //}

        public static void ConvertAge(DateTime dob, out int yearOld, out int monthOld)
        {
            yearOld = 0;
            monthOld = 0;

            DateTime today = DateTime.Today;
            ////Lớn hơn 2 tuổi => năm tuổi.
            ////if (dob <= today.AddYears(-2))
            //if (today.Year - dob.Year >= 2)
            //{
            //    //yearOld = dob.DayOfYear <= today.DayOfYear ? (today.Year - dob.Year) : (today.Year - dob.Year) - 1;
            //    yearOld = today.Year - dob.Year;
            //}
            ////if (dob < today.AddMonths(-2)) 
            //else
            //{
            //    //Nhỏ hơn = 2 tuổi => tháng tuổi.
            //    //monthOld = dob.DayOfYear <= today.DayOfYear ? (today.Year - dob.Year) * 12 + (today.Month - dob.Month) :
            //    //    ((today.Year - dob.Year) * 12 + (today.Month - dob.Month)) - 1;
            //    monthOld = (today.Month + today.Year * 12) - (dob.Month + dob.Year * 12);
            //    if (monthOld <= 0)
            //    {
            //        monthOld = 1;
            //    }
            //}

            //------- DPT 08/11/2017 < 72 tháng tuổi
            int monthnew = 0;
            monthnew = (today.Month + today.Year * 12) - (dob.Month + dob.Year * 12);

            if (monthnew <= 72)
            {
                if (monthnew <= 0)
                {
                    monthOld = 1;
                }
                else { monthOld = monthnew; }
            }
            else
            {
                yearOld = today.Year - dob.Year;
            }
            //---------------------------------------




        }
        ////------- DPT 08/11/2017 < 72 tháng tuổi
        //public static void ConvertMonth(DateTime dob, out int yearOld, out int monthOld)
        //{ 
        //    yearOld = 0;
        //    monthOld = 0;

        //    DateTime today = DateTime.Today;                  
        //    int monthnew = 0;
        //    monthnew = (today.Month + today.Year * 12) - (dob.Month + dob.Year * 12);

        //    if (monthnew <= 72)
        //    {
        //        if (monthnew <= 0)
        //        {
        //            monthOld = 1;
        //        }
        //        else { monthOld = monthnew; }
        //    }
        //    else
        //    {
        //        yearOld = today.Year - dob.Year;
        //    }
        //    //---------------------------------------




        //}


        public static double CalDateDiffUsedInBedAllocation(DateTime dateFrom , DateTime dateTo)
        {
            double dateDiff = 0.0;
            if(dateTo <= dateFrom)
            {
                return  dateDiff;
            }
            dateDiff = dateTo.Subtract(dateFrom).Days + 1;
            return  dateDiff;
        }

        public static bool ValidateEmailAddress(string strEmail)
        {
            if (string.IsNullOrWhiteSpace(strEmail))
            {
                return false;
            }
            var regEx = new Regex(EmailRegularExpression);
            return regEx.IsMatch(strEmail);
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        /// <summary>
        /// Lon hon 1;bang nhau 0; nho hon 2
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        [Description(" Lon hon 1;bang nhau 0; nho hon 2")]
        public static int CompareDate(DateTime t1, DateTime t2)
        {
            //0:t1=t2;//1: t1>t2;2 :t1 < t2 
            int year1 = t1.Year;
            int year2 = t2.Year;
            int month1 = t1.Month;
            int month2 = t2.Month;
            int day1 = t1.Day;
            int day2 = t2.Day;
            if (year1 > year2)
            {
                //t1>t2
                return 1;
            }
            else
            {
                if (year1 == year2)
                {
                    if (month1 > month2)
                    {
                        return 1;
                    }
                    else
                    {
                        if (month1 == month2)
                        {
                            if (day1 > day2)
                            {
                                return 1;
                            }
                            else if (day1 == day2)
                            {
                                return 0;
                            }
                            else
                            {
                                return 2;
                            }
                        }
                    }
                }
            }
            return 2;
        }

    }







    

    ///////////////
    //Test
    ///////////////
    public class HICardValidatorFactory
    {
        static private HICardValidatorFactory _instance = null;
        
        static public HICardValidatorFactory Instance
        {
            get
            {
                lock (typeof(HICardValidatorFactory))
                {
                    if (_instance == null)
                    {
                        _instance = new HICardValidatorFactory();
                    }
                }
                return _instance;
            }
        }

        public HICardValidatorBase CreateValidator(long HICardType)
        {
            return new HICardValidator2010();
        }
    }
    public abstract class HICardValidatorBase
    {
        /// <summary>
        /// Extract ma so the bao hiem y te
        /// </summary>
        /// <param name="strMaThe">Ma so ghi tren the bao hiem (gom 15 chu so)</param>
        /// /// <param name="strDoiTuong">Ma Doi Tuong</param>
        /// <param name="strQuyenLoi">Ma Quyen loi</param>
        /// <param name="strTinh">Ma Tinh</param>
        /// <param name="strQuanHuyen">Ma Quan, huyen</param>
        /// <param name="strDonVi">Ma Don vi</param>
        /// <param name="strSTT">So thu tu nguoi tham gia</param>
        /// <param name="IBeID">ID quyền lợi</param>
        /// <returns>Tra ve True neu chuoi strMaThe nhap vao hop le. Neu khong, tra ve False</returns>
        public abstract bool ExtractHICardNumber(long V_HICardType, string strMaThe, out string strDoiTuong, out string strQuyenLoi, out string strTinh, out string strQuanHuyen, out string strDonVi, out string strSTT);
        public abstract bool ExtractHICardNumber_2015(long V_HICardType, string strMaThe, out string strDoiTuong, out string strQuyenLoi, out string strTinh, out string strQuanHuyen, out string strDonVi, out string strSTT, out string IBeID
            , string ValidHIPattern = null
            , List<string[]> InsuranceBenefitCategories = null);
    }
    public class HICardValidator2010 : HICardValidatorBase
    {
        Dictionary<string, int> PtType_Map_BenefitGroup_2015 = new Dictionary<string, int>()
        {
            {"CC", 1},{"TE", 1},
            
            {"CK", 2},{"CB", 2},{"KC", 2},            
            {"HN", 2},{"DT", 2},{"DK", 2},                        
            {"XD", 2},{"BT", 2},{"TS", 2},

            {"HT", 3},{"TC", 3},{"CN", 3},
            	
            {"DN", 4},{"HX", 4},{"CH", 4},
            {"NN", 4},{"TK", 4},{"HC", 4},
            {"XK", 4},{"TB", 4},{"NO", 4},
            {"CT", 4},{"XB", 4},{"TN", 4},
            {"CS", 4},{"XN", 4},{"MS", 4},
            {"HD", 4},{"TQ", 4},{"TA", 4},
            {"TY", 4},{"HG", 4},{"LS", 4},
            {"HS", 4},{"SV", 4},{"GB", 4},
            {"GD", 4},{"PV", 4},

            {"TL", 4},{"XV", 4},

            {"QN", 5},{"CA", 5},{"CY", 5}
            
        };


        /// <summary>
        /// Extract ma so the bao hiem y te
        /// </summary>
        /// <param name="strMaThe">Ma so ghi tren the bao hiem (gom 15 chu so)</param>
        /// /// <param name="strDoiTuong">Ma Doi Tuong</param>
        /// <param name="strQuyenLoi">Ma Quyen loi</param>
        /// <param name="strTinh">Ma Tinh</param>
        /// <param name="strQuanHuyen">Ma Quan, huyen</param>
        /// <param name="strDonVi">Ma Don vi</param>
        /// <param name="strSTT">So thu tu nguoi tham gia</param>
        /// <returns>Tra ve True neu chuoi strMaThe nhap vao hop le. Neu khong, tra ve False</returns>
        public override bool ExtractHICardNumber(long V_HICardType, string strMaThe, out string strDoiTuong,out string strQuyenLoi, out string strTinh,out string strQuanHuyen, out string strDonVi, out string strSTT)
        {
            strDoiTuong = "";
            strQuyenLoi = "";
            strTinh = "";
            strQuanHuyen = "";
            strDonVi = "";
            strSTT = "";

            bool bOK = false;

            if(string.IsNullOrWhiteSpace(strMaThe))
            {
                return bOK;
            }
            strMaThe = strMaThe.ToUpper();

            //string pattern = @"^(((?'doituong'CC|TE)(?'quyenloi'1))|((?'doituong'CK)(?'quyenloi'2))|((?'doituong'CA)(?'quyenloi'3))|((?'doituong'BT|HN)(?'quyenloi'4))|((?'doituong'HT)(?'quyenloi'5))|((?'doituong'CN)(?'quyenloi'6))|((?'doituong'DN|HX|CH|NN|TK|HC|XK|TB|MS|XB|XN|TN|CB|KC|HD|TC|TQ|TA|TY|HG|LS|HS|GD|TL|XV|NO)(?'quyenloi'7)))(?'tinhthanh'\d{2})(?'quanhuyen'\d{2})(?'donvi'\d{3})(?'sothutu'\d{5})$";
            //string regQuanHuyenTPHCM = "^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";

            string pattern = "";

            //Mẫu 2008, 2011
            if (V_HICardType == 5901 || V_HICardType == 5904)
            {
                pattern = @"^(((?'doituong'CC|TE|CK|CA|BT|HN|HT|CN|DN|HX|CH|NN|TK|HC|XK|TB|MS|XB|XN|TN|CB|KC|HD|TC|TQ|TA|TY|HG|LS|HS|GD|TL|XV|NO|QN)(?'quyenloi'[1-79])))(?'tinhthanh'\d{2})(?'quanhuyen'\d{2})(?'donvi'\d{3})(?'sothutu'\d{5})$";
            }

            //Mẫu 2013 TPHCM
            if (V_HICardType == 5905)
            {
                pattern = @"^(((?'doituong'CC|TE|CK|CA|BT|HN|HT|CN|DN|HX|CH|NN|TK|HC|XK|TB|MS|XB|XN|TN|CB|KC|HD|TC|TQ|TA|TY|HG|LS|HS|GD|TL|XV|NO|QN)(?'quyenloi'[1-79])))(?'tinhthanh'\d{2})(?'quanhuyen'(([A-V]{1}|[X-Z]{1})[A-Z]{1})|\d{2})(?'donvi'\d{3})(?'sothutu'\d{5})$";
            }

            // TxD 28/12/2014: Moved to method ExtractHICardNumber_2015
            ////Mẫu 2015 Ap dung toan quoc bat dau 01/01/2015
            //if (V_HICardType == 5907) // With missing doi tuong: TL & XV added at the end
            //{
            //    pattern = @"^(((?'doituong'CC|TE|CK|CB|KC|HN|DT|DK|XD|BT|TS|HT|TC|CN|DN|HX|CH|NN|TK|HC|XK|TB|NO|CT|XB|TN|CS|XN|MS|HD|TQ|TA|TY|HG|LS|HS|SV|GB|GD|QN|CA|CY|TL|XV)(?'quyenloi'[1-5])))(?'tinhthanh'\d{2})(?'quanhuyen'(([A-V]{1}|[X-Z]{1})[A-Z]{1})|\d{2})(?'donvi'\d{3})(?'sothutu'\d{5})$";
            //}

            Regex regEx = new Regex(pattern);

            Match match = regEx.Match(strMaThe);
            if (match.Success)
            {
                strDoiTuong = match.Groups["doituong"].Value;
                strQuyenLoi = match.Groups["quyenloi"].Value;
                strTinh = match.Groups["tinhthanh"].Value;
                strQuanHuyen = match.Groups["quanhuyen"].Value;
                strDonVi = match.Groups["donvi"].Value;
                strSTT = match.Groups["sothutu"].Value;

                //KMx: strQuanHuyen chỉ được là alphabet khi strTinh = 79 (TPHCM). Cho nên nếu không phải là TPHCM mà strQuanHuyen có chứa alphabet là lỗi.
                if (V_HICardType == 5905 && strTinh != "79" && !IsDigitsOnly(strQuanHuyen))
                {
                    bOK = false;
                }
                else
                {
                    bOK = true;
                }

            }

            return bOK;
        }


        /// <summary>
        /// Extract ma so the bao hiem y te
        /// </summary>
        /// <param name="strMaThe">Ma so ghi tren the bao hiem (gom 15 chu so)</param>
        /// /// <param name="strDoiTuong">Ma Doi Tuong</param>
        /// <param name="strQuyenLoi">Ma Quyen loi</param>
        /// <param name="strTinh">Ma Tinh</param>
        /// <param name="strQuanHuyen">Ma Quan, huyen</param>
        /// <param name="strDonVi">Ma Don vi</param>
        /// <param name="strSTT">So thu tu nguoi tham gia</param>
        /// <returns>Tra ve True neu chuoi strMaThe nhap vao hop le. Neu khong, tra ve False</returns>
        public override bool ExtractHICardNumber_2015(long V_HICardType, string strMaThe, out string strDoiTuong, out string strQuyenLoi, out string strTinh, out string strQuanHuyen, out string strDonVi, out string strSTT, out string IBeID
            , string ValidHIPattern = null
            , List<string[]> InsuranceBenefitCategories = null)
        {
            if (!string.IsNullOrEmpty(ValidHIPattern) && InsuranceBenefitCategories != null)
            {
                return ExtractHICardNumber_2015_V2(V_HICardType, strMaThe, out strDoiTuong, out strQuyenLoi, out strTinh, out strQuanHuyen, out strDonVi, out strSTT, out IBeID, ValidHIPattern, InsuranceBenefitCategories);
            }

            strDoiTuong = "";
            strQuyenLoi = "";
            strTinh = "";
            strQuanHuyen = "";
            strDonVi = "";
            strSTT = "";
            IBeID = "";
            bool bOK = false;

            if (string.IsNullOrWhiteSpace(strMaThe))
            {
                return bOK;
            }
            strMaThe = strMaThe.ToUpper();

            //string pattern = @"^(((?'doituong'CC|TE)(?'quyenloi'1))|((?'doituong'CK)(?'quyenloi'2))|((?'doituong'CA)(?'quyenloi'3))|((?'doituong'BT|HN)(?'quyenloi'4))|((?'doituong'HT)(?'quyenloi'5))|((?'doituong'CN)(?'quyenloi'6))|((?'doituong'DN|HX|CH|NN|TK|HC|XK|TB|MS|XB|XN|TN|CB|KC|HD|TC|TQ|TA|TY|HG|LS|HS|GD|TL|XV|NO)(?'quyenloi'7)))(?'tinhthanh'\d{2})(?'quanhuyen'\d{2})(?'donvi'\d{3})(?'sothutu'\d{5})$";
            //string regQuanHuyenTPHCM = "^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";

            string pattern = "";

            //Mẫu 2008, 2011
            if (V_HICardType == 5901 || V_HICardType == 5904)
            {
                pattern = @"^(((?'doituong'CC|TE|CK|CA|BT|HN|HT|CN|DN|HX|CH|NN|TK|HC|XK|TB|MS|XB|XN|TN|CB|KC|HD|TC|TQ|TA|TY|HG|LS|HS|GD|TL|XV|NO|QN)(?'quyenloi'[1-79])))(?'tinhthanh'\d{2})(?'quanhuyen'\d{2})(?'donvi'\d{3})(?'sothutu'\d{5})$";
            }

            //Mẫu 2013 TPHCM
            if (V_HICardType == 5905)
            {
                pattern = @"^(((?'doituong'CC|TE|CK|CA|BT|HN|HT|CN|DN|HX|CH|NN|TK|HC|XK|TB|MS|XB|XN|TN|CB|KC|HD|TC|TQ|TA|TY|HG|LS|HS|GD|TL|XV|NO|QN)(?'quyenloi'[1-79])))(?'tinhthanh'\d{2})(?'quanhuyen'(([A-V]{1}|[X-Z]{1})[A-Z]{1})|\d{2})(?'donvi'\d{3})(?'sothutu'\d{5})$";
            }

            //Mẫu 2015 Ap dung toan quoc bat dau 01/01/2015
            if (V_HICardType == 5907) // With missing doi tuong: TL & XV added at the end
            {
                pattern = @"^(((?'doituong'CC|TE|CK|CB|KC|HN|DT|DK|XD|BT|TS|HT|TC|CN|DN|HX|CH|NN|TK|HC|XK|TB|NO|CT|XB|TN|CS|XN|MS|HD|TQ|TA|TY|HG|LS|HS|SV|GB|GD|QN|CA|CY|TL|XV|PV)(?'quyenloi'[1-5])))(?'tinhthanh'\d{2})(?'quanhuyen'(([A-V]{1}|[X-Z]{1})[A-Z]{1})|\d{2})(?'donvi'\d{3})(?'sothutu'\d{5})$";
            }

            Regex regEx = new Regex(pattern);

            Match match = regEx.Match(strMaThe);
            if (!match.Success)
            {
                return false;
            }
            strDoiTuong = match.Groups["doituong"].Value;
            strQuyenLoi = match.Groups["quyenloi"].Value;
            strTinh = match.Groups["tinhthanh"].Value;
            strQuanHuyen = match.Groups["quanhuyen"].Value;
            strDonVi = match.Groups["donvi"].Value;
            strSTT = match.Groups["sothutu"].Value;

            //KMx: strQuanHuyen chỉ được là alphabet khi strTinh = 79 (TPHCM). Cho nên nếu không phải là TPHCM mà strQuanHuyen có chứa alphabet là lỗi.
            if (V_HICardType == 5905 && strTinh != "79" && !IsDigitsOnly(strQuanHuyen))
            {
                bOK = false;
            }
            else
            {
                bOK = true;
            }

            // TxD 24/12/2014: There are 2 exceptions of HT1 and HT2 which can not be in dictionary because there already exists HT3
            //                 HT1 and HT2 would be in both the old and the new 2015 The BHYT thus has to be processed separately
            // Hpt 10/10/2015: Bo sung ma the CT2
            if ((strDoiTuong == "HT" && (strQuyenLoi == "1" || strQuyenLoi == "2"))
                 || (strDoiTuong == "HC" && strQuyenLoi == "2") || (strDoiTuong == "XN" && strQuyenLoi == "2") || (strDoiTuong == "CT" && strQuyenLoi == "2"))
            {
                bOK = true;
            }
            else
            {
                //M?u 2015 Ap dung toan quoc bat dau 01/01/2015
                if (V_HICardType == 5907)
                {
                    if (int.Parse(strQuyenLoi) != PtType_Map_BenefitGroup_2015[strDoiTuong])
                    {
                        bOK = false;
                    }
                }
                else if (V_HICardType < 5907) // May the BHYT prior to Mau the 2015
                {
                    if (int.Parse(strQuyenLoi) > 2) // Old Benefit Groups from 3 -> 7 & 9 convert to new Benefit Group based of PtType (doi tuong)
                    {
                        strQuyenLoi = PtType_Map_BenefitGroup_2015[strDoiTuong].ToString();
                    }
                }
            }
            
            return bOK;

        }
        private bool ExtractHICardNumber_2015_V2(long V_HICardType, string strMaThe, out string strDoiTuong, out string strQuyenLoi, out string strTinh, out string strQuanHuyen, out string strDonVi, out string strSTT, out string IBeID
            , string ValidHIPattern
            , List<string[]> InsuranceBenefitCategories)
        {
            strDoiTuong = "";
            strQuyenLoi = "";
            strTinh = "";
            strQuanHuyen = "";
            strDonVi = "";
            strSTT = "";
            IBeID = "";
            bool bOK = false;

            if (string.IsNullOrWhiteSpace(strMaThe))
            {
                return bOK;
            }
            strMaThe = strMaThe.ToUpper();

            //string pattern = @"^(((?'doituong'CC|TE)(?'quyenloi'1))|((?'doituong'CK)(?'quyenloi'2))|((?'doituong'CA)(?'quyenloi'3))|((?'doituong'BT|HN)(?'quyenloi'4))|((?'doituong'HT)(?'quyenloi'5))|((?'doituong'CN)(?'quyenloi'6))|((?'doituong'DN|HX|CH|NN|TK|HC|XK|TB|MS|XB|XN|TN|CB|KC|HD|TC|TQ|TA|TY|HG|LS|HS|GD|TL|XV|NO)(?'quyenloi'7)))(?'tinhthanh'\d{2})(?'quanhuyen'\d{2})(?'donvi'\d{3})(?'sothutu'\d{5})$";
            //string regQuanHuyenTPHCM = "^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$";

            string pattern = "";

            //Mẫu 2008, 2011
            if (V_HICardType == 5901 || V_HICardType == 5904)
            {
                pattern = @"^(((?'doituong'CC|TE|CK|CA|BT|HN|HT|CN|DN|HX|CH|NN|TK|HC|XK|TB|MS|XB|XN|TN|CB|KC|HD|TC|TQ|TA|TY|HG|LS|HS|GD|TL|XV|NO|QN)(?'quyenloi'[1-79])))(?'tinhthanh'\d{2})(?'quanhuyen'\d{2})(?'donvi'\d{3})(?'sothutu'\d{5})$";
            }

            //Mẫu 2013 TPHCM
            if (V_HICardType == 5905)
            {
                pattern = @"^(((?'doituong'CC|TE|CK|CA|BT|HN|HT|CN|DN|HX|CH|NN|TK|HC|XK|TB|MS|XB|XN|TN|CB|KC|HD|TC|TQ|TA|TY|HG|LS|HS|GD|TL|XV|NO|QN)(?'quyenloi'[1-79])))(?'tinhthanh'\d{2})(?'quanhuyen'(([A-V]{1}|[X-Z]{1})[A-Z]{1})|\d{2})(?'donvi'\d{3})(?'sothutu'\d{5})$";
            }

            //Mẫu 2015 Ap dung toan quoc bat dau 01/01/2015
            if (V_HICardType == 5907) // With missing doi tuong: TL & XV added at the end
            {
                pattern = ValidHIPattern;
            }

            Regex regEx = new Regex(pattern);

            Match match = regEx.Match(strMaThe);
            if (!match.Success)
            {
                return false;
            }
            strDoiTuong = match.Groups["doituong"].Value;
            strQuyenLoi = match.Groups["quyenloi"].Value;
            strTinh = match.Groups["tinhthanh"].Value;
            strQuanHuyen = match.Groups["quanhuyen"].Value;
            strDonVi = match.Groups["donvi"].Value;
            strSTT = match.Groups["sothutu"].Value;

            //KMx: strQuanHuyen chỉ được là alphabet khi strTinh = 79 (TPHCM). Cho nên nếu không phải là TPHCM mà strQuanHuyen có chứa alphabet là lỗi.
            if (V_HICardType == 5905 && strTinh != "79" && !IsDigitsOnly(strQuanHuyen))
            {
                bOK = false;
            }
            else
            {
                bOK = true;
            }

            //M?u 2015 Ap dung toan quoc bat dau 01/01/2015
            if (V_HICardType == 5907)
            {
                string mDoiTuong = strDoiTuong;
                string mQuyenLoi = strQuyenLoi;

                //▼====== #001:     x.Length == 2 => 3 vì Dictionary thay đổi từ 2 string => 3 string.
                //                  Lý do thay đổi điều kiện vì: Khi không tìm thấy bất cứ thẻ nào thoả điều kiện sẽ set true tương đương với việc set false ban đầu nhưng tìm thấy 1 thẻ phù hợp điều kiện thì set true.
                //          
                //if (!InsuranceBenefitCategories.Any(x => x.Length == 2 && x[0] == mDoiTuong && x[1] == mQuyenLoi))
                //{
                //    bOK = false;
                //}
                bOK = false;
                foreach (var x in InsuranceBenefitCategories)
                {
                    if (x.Length == 3 && x[0] == mDoiTuong && x[1] == mQuyenLoi)
                    {
                        IBeID = x[2];
                        bOK = true;
                        break;
                    }
                }
                //▲====== #001
            }
            else if (V_HICardType < 5907) // May the BHYT prior to Mau the 2015
            {
                if (int.Parse(strQuyenLoi) > 2) // Old Benefit Groups from 3 -> 7 & 9 convert to new Benefit Group based of PtType (doi tuong)
                {
                    strQuyenLoi = PtType_Map_BenefitGroup_2015[strDoiTuong].ToString();
                }
            }

            return bOK;
        }

        public bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }
    }

    public static class MathExtensions
    {
        public static decimal? Abs(decimal? d)
        {
            if(d == null)
            {
                return null;
            }
            return Math.Abs(d.Value);
        }

        public static IDictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }

            return dictionary;
        }

        public static IDictionary<TKey,List<TValue>>  AddItem<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                if(!dictionary[key].Contains(value))
                {
                    dictionary[key].Add(value);    
                }
            }
            else
            {
                var lst = new List<TValue> {value};
                dictionary.Add(key, lst);
            }

            return dictionary;

            
        }
    }
}
