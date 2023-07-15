using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using System.Linq;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts.Configuration;
using System.Windows;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPriceAdapt)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PriceAdaptViewModel : ViewModelBase, IPriceAdapt
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PriceAdaptViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching) 
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        public void SetValue(string serviceName, decimal oldPrice)
        {
            ServiceName = serviceName;
            OldPrice = oldPrice;
        }

        private string _ServiceName;
        public string ServiceName
        {
            get { return _ServiceName; }
            set
            {
                _ServiceName = value;
                NotifyOfPropertyChange(() => ServiceName);
            }
        }

        private string _Comments="";
        public string Comments
        {
            get { return _Comments; }
            set
            {
                _Comments = value;
                NotifyOfPropertyChange(() => Comments);
            }
        }

        private decimal _OldPrice;
        public decimal OldPrice
        {
            get { return _OldPrice; }
            set
            {
                _OldPrice = value;
                NotifyOfPropertyChange(() => OldPrice);
            }
        }

        private decimal _NewPrice;
        public decimal NewPrice
        {
            get { return _NewPrice; }
            set
            {
                _NewPrice = value;
                NotifyOfPropertyChange(() => NewPrice);
            }
        }

        private bool _IsOk=false;
        public bool IsOk
        {
            get { return _IsOk; }
            set
            {
                _IsOk = value;
                NotifyOfPropertyChange(() => IsOk);
            }
        }

        
        public void OkCmd()
        {
            try 
	        {	        
		        if(NewPrice<1000)
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0558_G1_Msg_InfoGiaTienKhHopLe));
                    return;
                }
            
                string temp = Comments.Replace("\n\r","").Replace("\n","").Replace("\r","").Replace(" ","");
                if (string.IsNullOrEmpty(temp))
                {
                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0601_G1_Msg_InfoNhapChuThich));
                    return;
                }
                IsOk = true;
                TryClose();
	        }
	        catch (Exception)
	        {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0838_G1_DLieuKgHopLe));
	        }
            
        }
        public void CancelCmd() 
        {
            IsOk = false;
            TryClose();
        }
    }
}
