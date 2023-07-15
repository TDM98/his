using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Globalization;
using DevExpress.Xpf.Core;

namespace aEMRClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static CastleBootstrapper _bootstrapper;
        CultureInfo mDefaultCulture = null;
        public App()
        {
            ApplicationThemeHelper.UseLegacyDefaultTheme = true;
            _bootstrapper = new CastleBootstrapper();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            //DevExpress.Xpf.Core.ApplicationThemeHelper.UpdateApplicationThemeName();

            mDefaultCulture = new CultureInfo(ConfigurationManager.AppSettings.Get("DefaultCulture"));
            mDefaultCulture.NumberFormat.NumberDecimalSeparator = ".";
            mDefaultCulture.NumberFormat.NumberGroupSeparator = ",";
            mDefaultCulture.NumberFormat.CurrencyDecimalSeparator = ".";
            mDefaultCulture.NumberFormat.CurrencyGroupSeparator = ",";

            System.Threading.Thread.CurrentThread.CurrentUICulture = mDefaultCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = mDefaultCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

            base.OnStartup(e);
        
            eHCMSLanguage.eHCMSResources.Culture = mDefaultCulture;
        }
    }
}