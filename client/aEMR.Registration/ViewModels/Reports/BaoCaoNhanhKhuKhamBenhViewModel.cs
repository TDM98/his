using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System;
using System.Windows;
using aEMR.Infrastructure;
using DataEntities;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IBaoCaoNhanhKhuKhamBenh)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BaoCaoNhanhKhuKhamBenhViewModel : Conductor<object>, IBaoCaoNhanhKhuKhamBenh
    {
        public BaoCaoNhanhKhuKhamBenhViewModel()
        {
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
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.FromDate = FromDate;
                    proAlloc.ToDate = ToDate;
                    proAlloc.eItem = eItem;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }


        private DateTime? _FromDate = Globals.GetCurServerDateTime();
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }

        private DateTime? _ToDate = Globals.GetCurServerDateTime();
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
