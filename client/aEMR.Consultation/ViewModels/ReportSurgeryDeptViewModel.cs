using System;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.Windows.Media.Imaging;
using System.IO;
using aEMR.Infrastructure;
using eHCMSLanguage;
using System.Threading;
using aEMR.ServiceClient;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IReportSurgeryDept)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReportSurgeryDeptViewModel : Conductor<object>, IReportSurgeryDept
    {
        private WriteableBitmap _ObjBitmapImage1;
        public WriteableBitmap ObjBitmapImage1
        {
            get { return _ObjBitmapImage1; }
            set
            {
                if (_ObjBitmapImage1 != value)
                {
                    _ObjBitmapImage1 = value;
                    NotifyOfPropertyChange(() => ObjBitmapImage1);
                }
            }
        }

        private WriteableBitmap _ObjBitmapImage2;
        public WriteableBitmap ObjBitmapImage2
        {
            get { return _ObjBitmapImage2; }
            set
            {
                if (_ObjBitmapImage2 != value)
                {
                    _ObjBitmapImage2 = value;
                    NotifyOfPropertyChange(() => ObjBitmapImage2);
                }
            }
        }

        private WriteableBitmap _ObjBitmapImage3;
        public WriteableBitmap ObjBitmapImage3
        {
            get { return _ObjBitmapImage3; }
            set
            {
                if (_ObjBitmapImage3 != value)
                {
                    _ObjBitmapImage3 = value;
                    NotifyOfPropertyChange(() => ObjBitmapImage3);
                }
            }
        }

        private WriteableBitmap _ObjBitmapImage4;
        public WriteableBitmap ObjBitmapImage4
        {
            get { return _ObjBitmapImage4; }
            set
            {
                if (_ObjBitmapImage4 != value)
                {
                    _ObjBitmapImage4 = value;
                    NotifyOfPropertyChange(() => ObjBitmapImage4);
                }
            }
        }
       
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ReportSurgeryDeptViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Image_LayoutUpdated1();
            Image_LayoutUpdated2();
            Image_LayoutUpdated3();
            Image_LayoutUpdated4();
        }
        private void GetVideoAndImage(string aPath, int number)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetVideoAndImage(aPath, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetVideoAndImage(asyncResult);
                            if (items != null && items.Length > 0)
                            {
                                Stream ObjGetVideoAndImage = new MemoryStream(items);
                                if (number == 1)
                                { 
                                    ObjBitmapImage1 = Globals.GetWriteableBitmapFromStream(ObjGetVideoAndImage);
                                }
                                else if (number == 2)
                                {
                                    ObjBitmapImage2 = Globals.GetWriteableBitmapFromStream(ObjGetVideoAndImage);
                                }
                                else if (number == 3)
                                {
                                    ObjBitmapImage3 = Globals.GetWriteableBitmapFromStream(ObjGetVideoAndImage);
                                }
                                else if (number == 4)
                                {
                                    ObjBitmapImage4 = Globals.GetWriteableBitmapFromStream(ObjGetVideoAndImage);
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                if (number == 1)
                                {
                                    ObjBitmapImage1 = null;
                                }
                                else if (number == 2)
                                {
                                    ObjBitmapImage2 = null;
                                }
                                else if (number == 3)
                                {
                                    ObjBitmapImage3 = null;
                                }
                                else if (number == 4)
                                {
                                    ObjBitmapImage4 = null;
                                }
                                else
                                {
                                    return;
                                }
                            }
                            this.HideBusyIndicator();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
     
        public void Image_LayoutUpdated1()
        {
            GetVideoAndImage(@"D:\AxDocuments\Pool\StorePool\Images\TTPT\Rpt1", 1);
            //GetVideoAndImage(@"D:\AxDocuments\Pool\StorePool\Images\170407\TuongTrinhPT.bmp", 1);
        }
        public void Image_LayoutUpdated2()
        {
            GetVideoAndImage(@"D:\AxDocuments\Pool\StorePool\Images\TTPT\Rpt2", 2);
        }
        public void Image_LayoutUpdated3()
        {
            GetVideoAndImage(@"D:\AxDocuments\Pool\StorePool\Images\TTPT\Rpt3", 3);
        }
        public void Image_LayoutUpdated4()
        {
            GetVideoAndImage(@"D:\AxDocuments\Pool\StorePool\Images\TTPT\Rpt4", 4);
        }

    }
}
