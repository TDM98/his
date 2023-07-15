using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Drawing.Printing;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Common;
using aEMR.Common.Printing;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using eHCMSLanguage;

namespace aEMR.SystemConfiguration.ViewModels
{
    [Export(typeof(IPrinterListing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PrinterListingViewModel : Conductor<object>, IPrinterListing
    {
        [ImportingConstructor]
        public PrinterListingViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            PrinterServerNotAvailableContent = Globals.GetViewModel<IFileDownload>();
        }

        public string ChildWindowTitle
        {
            get
            {
                return eHCMSResources.K2048_G1_ChonMayIn;
            }
        }

        private IFileDownload _printerServerNotAvailableContent;
        public IFileDownload PrinterServerNotAvailableContent
        {
            get { return _printerServerNotAvailableContent; }
            set
            {
                _printerServerNotAvailableContent = value;
                NotifyOfPropertyChange(() => PrinterServerNotAvailableContent);
            }
        }

        private bool _showButtonPanel;
        public bool ShowButtonPanel
        {
            get { return _showButtonPanel; }
            set
            {
                _showButtonPanel = value;
                NotifyOfPropertyChange(() => ShowButtonPanel);
            }
        }

        private PrinterInfo _selectedItem;
        public PrinterInfo SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        private ObservableCollection<PrinterInfo> _allPrinters;

        public ObservableCollection<PrinterInfo> AllPrinters
        {
            get { return _allPrinters; }
            set
            {
                _allPrinters = value;
                NotifyOfPropertyChange(() => AllPrinters);
            }
        }

        public PrinterInfo DefaultPrinter
        {
            get;
            private set;
        }
        
        private bool _printerServerInstalled = true;
        public bool PrinterServerInstalled
        {
            get
            {
                //_printerServerInstalled = AxonActiveXPrinter.PrinterServerAvailable;
                return _printerServerInstalled;
            }
            set
            {
                _printerServerInstalled = value;
                NotifyOfPropertyChange(() => PrinterServerInstalled);
            }
        }

        public void GetAllPrinters()
        {
            DefaultPrinter = null;
            try
            {
                //AllPrinters = AxonActiveXPrinter.GetAvailablePrinters();
                AllPrinters = new ObservableCollection<PrinterInfo>();
                //foreach (var p in AllPrinters)
                //{
                //    if (p.IsDefaultPrinter)
                //    {
                //        DefaultPrinter = p;
                //        break;
                //    }
                //}
                bool setDefPrt = false;
                foreach (var installedPrinter in PrinterSettings.InstalledPrinters)
                {
                    var prtInfo = new PrinterInfo();
                    prtInfo.PrinterName = installedPrinter.ToString();                    
                    
                    if (setDefPrt == false)
                    {
                        setDefPrt = true;
                        DefaultPrinter = new PrinterInfo { IsDefaultPrinter = true, PrinterName = prtInfo.PrinterName };
                    }
                    AllPrinters.Add(prtInfo);
                }
                
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
                DefaultPrinter = null;
            }
        }

        public void SelectCmd()
        {
            if (SelectedItem != null)
            {
                Globals.EventAggregator.Publish(new ItemSelected<PrinterInfo>() { Source = this, Item = SelectedItem, Cancel = false }); 
                CloseCmd();
            }
            else
            {
                MessageBox.Show(eHCMSResources.K2048_G1_ChonMayIn);
            }
        }

        public void CloseCmd()
        {
            try
            {
                TryClose();
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
        }

        public void DoubleClick(object eventArgs)
        {
            var args = eventArgs as EventArgs<object>;
            if (args != null)
            {
                if (SelectedItem != args.Value)
                {
                    SelectedItem = args.Value as PrinterInfo;
                }
                SelectCmd(); 
            }
        }
    }
}
