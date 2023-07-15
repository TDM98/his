using System.ComponentModel.Composition;
using Caliburn.Micro;
using DataEntities;
using aEMR.ReportModel.ReportModels;
using DevExpress.Xpf.Printing;
using aEMR.ViewContracts;
using Castle.Windsor;
using DevExpress.ReportServer.Printing;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IConsultationDocumentPreview))]
    public class ConsultationDocumentPreviewViewModel : Conductor<object>, IConsultationDocumentPreview
    {
        [ImportingConstructor]
        public ConsultationDocumentPreviewViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
        }
        private RemoteDocumentSource _reportModel;
        public RemoteDocumentSource ReportModel
        {
            get { return _reportModel; }
            set
            {
                _reportModel = value;
                NotifyOfPropertyChange(() => ReportModel);
            }
        }
        private long _ID = 0;
        public long ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
                NotifyOfPropertyChange(() => ID);
            }
        }
        private long _PatientID = 0;
        public long PatientID
        {
            get { return _PatientID; }
            set
            {
                _PatientID = value;
                NotifyOfPropertyChange(() => PatientID);
            }
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

        protected override void OnActivate()
        {
            base.OnActivate();
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            //if (ReportModel != null)
            //{
            //    ReportModel.RequestDefaultParameterValues -= new WeakEventHandler<EventArgs>(_reportModel_RequestDefaultParameterValues).Handler;
            //}
            switch (_eItem)
            {
                case ReportName.CONSULTATION_LABRESULTS:
                    ReportModel = null;
                    ReportModel = new ConsultationPCLLabResultReportModel().PreviewModel;
                    rParams["InvID"].Value = ID;
                    break;
                case ReportName.CONSULTATION_PMR:
                    ReportModel = null;
                    ReportModel = new ConsultationPMRReportModel().PreviewModel;
                    rParams["parPatientID"].Value = PatientID;
                    break;
                case ReportName.CONSULTATION_REQUESTPCLFROM:
                    ReportModel = null;
                    ReportModel = new ConsultationPCLFormReportModel().PreviewModel;
                    rParams["OutiID"].Value = ID;
                    rParams["PatientID"].Value = PatientID;
                    break;
                case ReportName.CONSULTATION_REQUESTPCL:
                    ReportModel = null;
                    ReportModel = new ConsultationPCLFormRequestReportModel().PreviewModel;
                    break;
                case ReportName.CONSULTATION_TOATHUOC:
                    ReportModel = null;
                    ReportModel = new ConsultationEPrescriptionReportModel().PreviewModel;
                    rParams["parPatientID"].Value = PatientID;
                    rParams["parPrescriptID"].Value = ID;
                    break;
            }
            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(rParams);
        }
    }
}