using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using DevExpress.Xpf.Printing;
using System;
using System.Windows;
using aEMR.Infrastructure;
using DataEntities;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IBaoCaoNhanhKhuKhamBenh)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BaoCaoNhanhKhuKhamBenhViewModel : Conductor<object>, IBaoCaoNhanhKhuKhamBenh
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public BaoCaoNhanhKhuKhamBenhViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            authorization();
        }

        private ReportName _eItem;
        public ReportName eItem
        {
            get
            {
                return _eItem;
            }
            set
            {
                _eItem = value;
                NotifyOfPropertyChange(() => eItem);
            }
        }

        #region checking account

        private bool _mBaoCaoNhanhKhuKhamBenh_XemIn = true;

        public bool mBaoCaoNhanhKhuKhamBenh_XemIn
        {
            get
            {
                return _mBaoCaoNhanhKhuKhamBenh_XemIn;
            }
            set
            {
                if (_mBaoCaoNhanhKhuKhamBenh_XemIn == value)
                    return;
                _mBaoCaoNhanhKhuKhamBenh_XemIn = value;
            }
        }

        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mBaoCaoNhanhKhuKhamBenh_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mYVu_Management
                                               , (int)eYVu_Management.mBaoCaoNhanhKhuKhamBenh,
                                               (int)oYVu_ManagementEx.mViewAndPrint, (int)ePermission.mView);
        }
        
        public void OKButton()
        {
            if (FromDate == null)
            {
                MessageBox.Show(eHCMSResources.K0454_G1_NhapTuNg, eHCMSResources.K1576_G1_CBao, MessageBoxButton.OK);
                return;
            }
            if (ToDate == null)
            {
                MessageBox.Show(eHCMSResources.K0427_G1_NhapDenNg, eHCMSResources.K1576_G1_CBao, MessageBoxButton.OK);
                return;
            }
            else
            {
                //var proAlloc = Globals.GetViewModel<ICommonPreviewView>();
                //proAlloc.FromDate = FromDate;
                //proAlloc.ToDate = ToDate;
                //proAlloc.eItem = eItem;
                //var instance = proAlloc as Conductor<object>;
                //Globals.ShowDialog(instance, (o) => { });

                Action<ICommonPreviewView> onInitDlg = (proAlloc) =>
                {
                    proAlloc.FromDate = FromDate;
                    proAlloc.ToDate = ToDate;
                    proAlloc.eItem = eItem;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }


        private DateTime? _FromDate = DateTime.Now;
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }

        private DateTime? _ToDate = DateTime.Now;
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }
    }
}
