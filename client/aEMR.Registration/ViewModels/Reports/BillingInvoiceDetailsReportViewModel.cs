using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.ReportModel.ReportModels;
using DevExpress.ReportServer.Printing;
using aEMR.Infrastructure;
/*
 * 20181118 #001 TNHX: [BM0005265] Update report
 */
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IBillingInvoiceDetailsReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BillingInvoiceDetailsReportViewModel : Conductor<object>, IBillingInvoiceDetailsReport
    {
        public BillingInvoiceDetailsReportViewModel()
        {
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            ReportModel = null;
            ReportModel = new InPatientBillingInvoiceDetailsReportModel().PreviewModel;
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            //_reportModel.Parameters["param_InPatientBillingInvID"].Value = (int)_inPatientBillingInvID;
            rParams["InPatientBillingInvID"].Value = (int)InPatientBillingInvID;
            rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
            rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
            ReportModel.CreateDocument(rParams);
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

        public long InPatientBillingInvID { get; set; }
    }
}
