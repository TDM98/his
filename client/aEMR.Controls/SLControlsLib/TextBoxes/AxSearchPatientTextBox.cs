using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using aEMR.Common;
using aEMR.Infrastructure;
using aEMR.DataContracts;
/*
 * 20180831 #002 TTM:   Clear dữ liệu Mã khám bệnh, nhập viện sau khi tìm kiếm 1 bệnh nhân
 * 20181127 #003 TTM:   Thay đổi const string PCL_CODE từ kiểu cũ "PCL-" => kiểu mới "PH|PL|QH|QL". 
 * 20181227 #004 TNHX:  BM 0005471: Add Focus for txtName
 * 20190522 #005 TTM:   BM 0006771: Bổ sung thêm điều kiện để trường hợp định dạng thẻ bị sai (Hexa Decimal Parse bị sai) thì trả về HICardNo để tìm kiếm thẻ. Nếu không có thì đưa trường lỗi lên là rỗng để người dùng nhập vào.
 * 20230530 #006 DatTB:
 * + Thêm service tìm kiếm bệnh nhân bằng QRCode CCCD
 * + Định dạng dữ liệu đầu vào là QR CCCD
 */
namespace aEMR.Controls
{
    public class AxSearchPatientTextBox : TextBox
    {
        //▼====== #004
        public AxSearchPatientTextBox()
            : base()
        {
            this.TextChanged += new TextChangedEventHandler(AxSearchPatientTextBox_TextChanged);
            this.Loaded += new RoutedEventHandler(AxSearchPatientTextBox_Loaded);
            base.GotFocus += OnGotFocus;
        }
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            base.SelectAll();
        }
        //▲====== #004

        void AxSearchPatientTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            ExtractUserInput(this.Text);
        }

        void AxSearchPatientTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ExtractUserInput(this.Text);
        }

        //public const string HICARD_REG_EXP = @"^(DN|HX|CH|NN|TK|HC|XK|CA|HT|TB|MS|XB|XN|TN|CC|CK|CB|KC|HD|BT|HN|TC|TQ|TA|TY|TE|HG|LS|CN|HS|GD|TL|XV|NO)([1-7])(\d{2})(\d{2})(\d{3})(\d{5})$";
        //public const string HICARD_REG_EXP = @"^(((?'doituong'CC|TE)(?'quyenloi'1))|((?'doituong'CK)(?'quyenloi'2))|((?'doituong'CA)(?'quyenloi'3))|((?'doituong'BT|HN)(?'quyenloi'4))|((?'doituong'HT)(?'quyenloi'5))|((?'doituong'CN)(?'quyenloi'6))|((?'doituong'DN|HX|CH|NN|TK|HC|XK|TB|MS|XB|XN|TN|CB|KC|HD|TC|TQ|TA|TY|HG|LS|HS|GD|TL|XV|NO)(?'quyenloi'7)))(?'tinhthanh'\d{2})(?'quanhuyen'\d{2})(?'donvi'\d{3})(?'sothutu'\d{5})$";
        public const string HICARD_REG_EXP = @"^(((?'doituong'CC|TE|CK|CA|BT|HN|HT|CN|DN|HX|CH|NN|TK|HC|XK|TB|MS|XB|XN|TN|CB|KC|HD|TC|TQ|TA|TY|HG|LS|HS|GD|TL|XV|NO|SV|DK|CT|QN|CS|CY|XD|DT|TS|PV|GB|HK|ND|)(?'quyenloi'[1-7])))(?'tinhthanh'\d{2})(?'quanhuyen'\d{2})(?'donvi'\d{3})(?'sothutu'\d{5})$";
        public const string PMF_CODE = @"^\d{6,8}$";

        //▼====== #003
        //public const string PCL_CODE = @"^PCL-";
        //20181210 TTM: Bổ sung: nếu bắt đầu bằng PH|PL|QH|QL và đi kèm là số thì đó là PCL_Code còn bắt đầu bằng PH|PL|QH|QL mà không kèm số. Ví dụ: Phạm, Phan, Phương, Phong .... thì đó là tên người.
        //public const string PCL_CODE = @"^PH|PL|QH|QL";
        public const string PCL_CODE = @"^(PH|PL|QH|QL)[0-9]";
        //▲====== #003

        //==== #001
        public const string HIQR_CODE = @".*[$]$";
        //==== #001
        //==== #006
        public const string IDNUMBER_REG = @"^(001|002|004|006|008|010|011|012|014|015|017|019|020|022|024|025|026|027|030|031|033|034|035|036|037|038|040|042|044|045|046|048|049|051|052|054|056|058|060|062|064|066|067|068|070|072|074|075|077|079|080|082|083|084|086|087|089|091|092|093|094|095|096)[0-9]{9}$";
        public const string IDNUMBER_OLD_REG = @"^(01|02|03|04|05|06|07|08|09|10|11|12|13|14|15|16|17|18|19|20|21|22|23|24|25|26|27|28|29|30|31|32|33|34|35|36|37|38)[0-9]{7}$";
        public const string IDCARD_QRCODE = @"^(001|002|004|006|008|010|011|012|014|015|017|019|020|022|024|025|026|027|030|031|033|034|035|036|037|038|040|042|044|045|046|048|049|051|052|054|056|058|060|062|064|066|067|068|070|072|074|075|077|079|080|082|083|084|086|087|089|091|092|093|094|095|096)[0-9]{9}|{*}$";
        //==== #006
        //public const string PATIENT_CODE_REG_EXP = "^[0-9]{8}$";
        public const string PT_CODE = @"^KB|NV";
        public const string ISSUE_CODE = @"^TT";
        public const string QMS_CODE = @"^(qms)";
        public string PtRegistrationCode
        {
            get
            {
                return (string)GetValue(PtRegistrationCodeProperty);
            }
            set
            {
                SetValue(PtRegistrationCodeProperty, value);
            }
        }
        public static readonly DependencyProperty PtRegistrationCodeProperty = DependencyProperty.Register("PtRegistrationCode", typeof(string), typeof(AxSearchPatientTextBox), new PropertyMetadata(null, null));
        public string PrescriptionIssueCode
        {
            get
            {
                return (string)GetValue(PrescriptionIssueCodeProperty);
            }
            set
            {
                SetValue(PrescriptionIssueCodeProperty, value);
            }
        }
        public static readonly DependencyProperty PrescriptionIssueCodeProperty = DependencyProperty.Register("PrescriptionIssueCode", typeof(string), typeof(AxSearchPatientTextBox), new PropertyMetadata(null, null));
        public string HICardNumber
        {
            get
            {
                return (string)GetValue(HICardNumberProperty);
            }
            set
            {
                SetValue(HICardNumberProperty, value);
            }
        }

        public static readonly DependencyProperty HICardNumberProperty = DependencyProperty.Register("HICardNumber",
            typeof(string),
            typeof(AxSearchPatientTextBox),
            new PropertyMetadata(null, OnHICardNumberChanged));
        private static void OnHICardNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //AxSearchPatientTextBox box = (AxSearchPatientTextBox)d;
            //box.Text = e.NewValue.ToString();
        }

        public string FullName
        {
            get
            {
                return (string)GetValue(FullNameProperty);
            }
            set
            {
                SetValue(FullNameProperty, value);
            }
        }

        public static readonly DependencyProperty FullNameProperty = DependencyProperty.Register("FullName",
            typeof(string),
            typeof(AxSearchPatientTextBox),
            new PropertyMetadata(null, null));

        public string PatientCode
        {
            get
            {
                return (string)GetValue(PatientCodeProperty);
            }
            set
            {
                SetValue(PatientCodeProperty, value);
            }
        }

        public static readonly DependencyProperty PatientCodeProperty = DependencyProperty.Register("PatientCode",
            typeof(string),
            typeof(AxSearchPatientTextBox),
            new PropertyMetadata(null, OnPatientCodeChanged));
        private static void OnPatientCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //AxSearchPatientTextBox box = (AxSearchPatientTextBox)d;
            //box.Text = e.NewValue.ToString();
        }


        public string PCLRequestNumID
        {
            get
            {
                return (string)GetValue(PCLRequestNumIDProperty);
            }
            set
            {
                SetValue(PCLRequestNumIDProperty, value);
            }
        }

        public static readonly DependencyProperty PCLRequestNumIDProperty = DependencyProperty.Register("PCLRequestNumID",
            typeof(string),
            typeof(AxSearchPatientTextBox),
            new PropertyMetadata(null, null));

        public string PMFCode 
        {
            get { return (string)GetValue(PMFCodeProperty); }
            set { SetValue(PMFCodeProperty, value); }
        }

        public static readonly DependencyProperty PMFCodeProperty = DependencyProperty.Register("PMFCode"
            , typeof(string),typeof(AxSearchPatientTextBox),new PropertyMetadata(null,null));
        //==== #001
        public string QRCode
        {
            get
            {
                return (string)GetValue(QRCodeProperty);
            }
            set
            {
                SetValue(QRCodeProperty, value);
            }
        }
        public static readonly DependencyProperty QRCodeProperty = DependencyProperty.Register("QRCode", typeof(string), typeof(AxSearchPatientTextBox), new PropertyMetadata(null, null));
        //==== #001

        //==== #006
        public string IDCardQRCode
        {
            get
            {
                return (string)GetValue(IDCardQRCodeProperty);
            }
            set
            {
                SetValue(IDCardQRCodeProperty, value);
            }
        }
        public static readonly DependencyProperty IDCardQRCodeProperty = DependencyProperty.Register("IDCardQRCode", typeof(string), typeof(AxSearchPatientTextBox), new PropertyMetadata(null, null));

        public string IDNumber
        {
            get
            {
                return (string)GetValue(IDNumberProperty);
            }
            set
            {
                SetValue(IDNumberProperty, value);
            }
        }
        public static readonly DependencyProperty IDNumberProperty = DependencyProperty.Register("IDNumber", typeof(string), typeof(AxSearchPatientTextBox), new PropertyMetadata(null, null));

        public string IDNumberOld
        {
            get
            {
                return (string)GetValue(IDNumberOldProperty);
            }
            set
            {
                SetValue(IDNumberOldProperty, value);
            }
        }
        public static readonly DependencyProperty IDNumberOldProperty = DependencyProperty.Register("IDNumberOld", typeof(string), typeof(AxSearchPatientTextBox), new PropertyMetadata(null, null));
        //==== #006

        public static readonly DependencyProperty QMSSerialProperty = DependencyProperty.Register("QMSSerial",
        typeof(string),
        typeof(AxSearchPatientTextBox),
        new PropertyMetadata(null, null));
        public string QMSSerial
        {
            get
            {
                return (string)GetValue(QMSSerialProperty);
            }
            set
            {
                SetValue(QMSSerialProperty, value);
            }
        }
        public void ExtractUserInput(string sInput)
        {
            Regex regEx = new Regex(HICARD_REG_EXP, RegexOptions.IgnoreCase);
            Regex regPMF = new Regex(PMF_CODE, RegexOptions.IgnoreCase);
            Regex regPCL = new Regex(PCL_CODE, RegexOptions.IgnoreCase);
            Regex regPtCode = new Regex(PT_CODE, RegexOptions.IgnoreCase);
            Regex regIssueCode = new Regex(ISSUE_CODE, RegexOptions.IgnoreCase);
            Regex regQMSCode = new Regex(QMS_CODE, RegexOptions.IgnoreCase);
            //▼==== #006
            Regex regIDNumber = new Regex(IDNUMBER_REG, RegexOptions.IgnoreCase);
            Regex regIDNumberOld = new Regex(IDNUMBER_OLD_REG, RegexOptions.IgnoreCase);
            //▲==== #006

            FullName = string.Empty;
            HICardNumber = string.Empty;
            PatientCode = string.Empty;
            PCLRequestNumID = string.Empty;
            //▼====== #002
            PtRegistrationCode = string.Empty;
            //▲====== #002
            QMSSerial = string.Empty;

            Match match = regEx.Match(sInput);
            if (match.Success)//Search theo the bao hiem
            {
                HICardNumber = sInput.Trim();
            }
            else if (regPCL.Match(sInput).Success)
            {
                PCLRequestNumID = sInput.Trim();
            }
            //KMx: Nếu là Viện Tim thì mới tìm theo số hồ sơ, còn phòng mạch tư thì tìm theo Patient Code (28/05/2014 17:00).
            //kiem tra phai la so ho so
            else if (regPtCode.Match(sInput).Success)
            {
                PtRegistrationCode = sInput.Trim();
            }
            else if (regIssueCode.Match(sInput).Success)
            {
                PrescriptionIssueCode = sInput.Trim();
            }
            else if (regPMF.Match(sInput).Success && Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 0)
            {
                FullName = "hs:" + sInput.Trim();
                //PMFCode = sInput.Trim();
            }
            else if (regQMSCode.Match(sInput).Success && Globals.ServerConfigSection.CommonItems.UseQMSSystem)
            {
                int countStr = sInput.Length;
                if (countStr > 6)
                {
                    QMSSerial = sInput.Trim().Substring(3, countStr - 3).ToString();
                }
            }
            //▼==== #006
            else if (regIDNumberOld.Match(sInput).Success)
            {
                IDNumberOld = sInput.Trim();
            }
            else if (regIDNumber.Match(sInput).Success)
            {
                IDNumber = sInput.Trim();
            }
            //▲==== #006
            else
            {
                //Co bat ky 1 con so nao thi xem nhu la patientcode.
                bool isMatch = Regex.IsMatch(sInput, @"\d");

                if (isMatch)
                {
                    PatientCode = sInput.Trim();
                }
                else
                {
                    FullName = sInput.Trim();
                }
            }
            //==== #001
            if (Globals.ServerConfigSection != null && Globals.ServerConfigSection.CommonItems != null && Globals.ServerConfigSection.CommonItems.UseQRCode)
            {
                try
                {
                    Regex regQR = new Regex(HIQR_CODE, RegexOptions.IgnoreCase);
                    if (regQR.Match(sInput).Success)
                    {
                        string[] mInputArray = sInput.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (mInputArray.Length >= 15)
                        {
                            //▼====== #005
                            try
                            {
                                mInputArray[1] = Globals.HexToString(mInputArray[1]);
                            }
                            catch
                            {
                                mInputArray[1] = "";
                            }
                            try
                            {
                                mInputArray[4] = Globals.HexToString(mInputArray[4]);
                            }
                            catch
                            {
                                mInputArray[4] = "";
                            }
                            //▲====== #005
                            mInputArray[3] = mInputArray[3].ToString() == "1" ? "M" : "F";
                            HICardNumber = mInputArray[0];
                            FullName = mInputArray[1].ToString();
                            PatientCode = null;
                            QRCode = string.Join("|", mInputArray);
                        }
                    }
                    else
                        QRCode = null;
                }
                catch
                {
                    QRCode = null;
                }
            }
            else
                QRCode = null;
            //==== #001
            //==== #006
            if (Globals.ServerConfigSection != null && Globals.ServerConfigSection.CommonItems != null && Globals.ServerConfigSection.CommonItems.UseIDCardQRCode)
            {
                try
                {
                    Regex regIDQR = new Regex(IDCARD_QRCODE, RegexOptions.IgnoreCase);
                    if (regIDQR.Match(sInput).Success)
                    {
                        string[] mInputArray = sInput.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (mInputArray.Length >= 6 || mInputArray.Length <= 7)
                        {
                            if (mInputArray.Length == 6)
                            {
                                IDNumber = mInputArray[0];
                            }
                            else if (mInputArray.Length == 7)
                            {
                                IDNumber = mInputArray[0];
                                IDNumberOld = mInputArray[1];
                            }
                            PatientCode = null;
                            IDCardQRCode = sInput;
                        }
                    }
                    else
                        IDCardQRCode = null;
                }
                catch
                {
                    IDCardQRCode = null;
                }
            }
            else
                IDCardQRCode = null;
            //==== #006
            //Fire event
            if (ExtractUserInputCompleted != null)
            {
                ExtractUserInputCompleted(this, new EventArgs<object>(sInput));
            }
        }
        public event EventHandler<EventArgs<object>> ExtractUserInputCompleted;
       
    }
}
