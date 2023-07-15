using eHCMS.Services.Core;
using eHCMSLanguage;

namespace eHCMS.ReportLib
{
    public class Globals
    {
        public static string ReadMoneyToString(decimal aMoney, string aMoneyPrefix = "")
        {
            NumberToLetterConverter mConverter = new NumberToLetterConverter();
            System.Globalization.CultureInfo mCultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            decimal aTempMoney = 0;
            if (aMoney < 0)
            {
                aTempMoney = 0 - aMoney;
                aMoneyPrefix = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am.ToLower());
            }
            else
            {
                aTempMoney = aMoney;
                aMoneyPrefix = "";
            }
            return aMoneyPrefix + mConverter.Convert(aTempMoney.ToString(), mCultureInfo.NumberFormat.CurrencyDecimalSeparator.ToCharArray()[0], eHCMSResources.Z0871_G1_Le.ToLower(), eHCMSResources.G1616_G1_VND.ToUpper());
        }
    }
}