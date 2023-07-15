using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Castle.Core.Logging;
using Castle.Windsor;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Extensions;
using aEMR.Infrastructure.Utils;
using aEMR.Infrastructure.ViewUtils;

using System.Reflection;
using System.Deployment.Application;
using aEMR.ViewContracts;

namespace aEMRClient.ViewModels
{
    [Export(typeof(aEMR.ViewContracts.IAboutViewModel))]
    public class AboutViewModel : Conductor<object>, aEMR.ViewContracts.IAboutViewModel
    {
        private readonly INavigationService _navigationService;
        
        [ImportingConstructor]
        public AboutViewModel(IWindsorContainer container, INavigationService navigationService)           
        {
            _navigationService = navigationService;
            Initial();
        }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                NotifyOfPropertyChange(() => Title);                
            }
        }

        private string _Company;
        public string Company
        {
            get { return _Company; }
            set
            {
                _Company = value;
                NotifyOfPropertyChange(() => Company);
            }
        }

        private string _Product;
        public string Product
        {
            get { return _Product; }
            set
            {
                _Product = value;
                NotifyOfPropertyChange(() => Product);
            }
        }

        private string _Copyright;
        public string Copyright
        {
            get { return _Copyright; }
            set
            {
                _Copyright = value;
                NotifyOfPropertyChange(() => Copyright);
            }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }
        private string _PublishVersion;
        public string PublishVersion
        {
            get { return _PublishVersion; }
            set
            {
                _PublishVersion = value;
                NotifyOfPropertyChange(() => PublishVersion);
            }
        }
     
        public void Initial()
        {
            Assembly app = Assembly.GetExecutingAssembly();

            AssemblyTitleAttribute aTitle = (AssemblyTitleAttribute)app.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0];
            AssemblyProductAttribute aProduct = (AssemblyProductAttribute)app.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0];
            AssemblyCopyrightAttribute aCopyright = (AssemblyCopyrightAttribute)app.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0];
            AssemblyCompanyAttribute aCompany = (AssemblyCompanyAttribute)app.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)[0];
            AssemblyDescriptionAttribute aDescription = (AssemblyDescriptionAttribute)app.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0];

            Title = aTitle.Title;
            Product = aProduct.Product;
            Copyright = aCopyright.Copyright;
            Company = aCompany.Company;
            Description = aDescription.Description;

            Version version = app.GetName().Version;                 

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment cd = ApplicationDeployment.CurrentDeployment;
                PublishVersion = cd.CurrentVersion.ToString();
                // show publish version in title or About box...
            }
        }

       

        public void AppExitCmd()
        {
            Application.Current.MainWindow.Close();
        }

    }
}
