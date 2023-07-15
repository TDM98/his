using System;
using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.ReportModel.ReportModels;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;
using DevExpress.Xpf.Printing;
using DevExpress.ReportServer.Printing;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IPaymentReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PaymentReportViewModel : Conductor<object>, IPaymentReport
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PaymentReportViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            ReportModel = new PatientPaymentReportModel().PreviewModel;

            var userName = "";
            if (Globals.LoggedUserAccount != null
                && Globals.LoggedUserAccount.Staff != null)
            {
                userName = Globals.LoggedUserAccount.Staff.FullName;    
            }
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer theParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            theParams["param_User"].Value = userName;
            theParams["param_PaymentID"].Value = (int)_paymentID;
            theParams["FindPatient"].Value = FindPatient;
            //Chi su dung trong truong hop dang ky noi tru.
            theParams["param_CashAdvanceID"].Value = (int)CashAdvanceID;
            // ReportModel.AutoShowParametersPanel = false;

            LoadData(theParams);
        }
        private void LoadData(DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer theParams)
        {
            ReportModel.CreateDocument(theParams);
        }
     
        private decimal _registrationID;
        public decimal RegistrationID
        {
            set
            {
                _registrationID = value;
            }
        }
        private decimal _paymentID;
        public decimal PaymentID
        {
            set
            {
                _paymentID = value;
            }
        }

        public long CashAdvanceID { get; set; }

        private int _FindPatient;
        public int FindPatient
        {
            get { return _FindPatient; }
            set
            {
                _FindPatient = value;
            }
        }

        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(()=>ReportModel);
            }
        }

        public void OkCmd()
        {
            TryClose();
        }
    }
}
