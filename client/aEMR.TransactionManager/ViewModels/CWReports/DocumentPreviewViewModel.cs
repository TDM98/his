using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using DevExpress.Xpf.Printing;
using aEMR.ReportModel.ReportModels;
using System.Linq;
using Castle.Windsor;
using DevExpress.ReportServer.Printing;

namespace aEMR.TransactionManager.ViewModels
{
    [Export(typeof(IDocumentPreview)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DocumentPreviewViewModel : Conductor<object>, IDocumentPreview
    {
        [ImportingConstructor]
        public DocumentPreviewViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
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

        private string _StaffFullName = "";
        public string StaffFullName
        {
            get { return _StaffFullName; }
            set
            {
                _StaffFullName = value;
                NotifyOfPropertyChange(() => StaffFullName);
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
            var DieuDuongTruongKhoa = Globals.allStaffPositions.Where(x => x.PositionRefID == (int)AllLookupValues.StaffPositions_Enum.DIEU_DUONG_TRUONG_KHOA && x.IsActive).FirstOrDefault();
            DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer rParams = new DevExpress.DocumentServices.ServiceModel.DefaultValueParameterContainer();
            switch (_eItem)
            {
                case ReportName.TEMP38a:
                    ReportModel = null;
                    ReportModel = new TransactionsTemplate38().PreviewModel;
                    rParams["TransactionID"].Value = (int)ID;
                    rParams["PtRegistrationID"].Value = 0;

                    rParams["StaffFullName"].Value = StaffFullName;

                    rParams["DieuDuongTruongKhoa"].Value = DieuDuongTruongKhoa != null ? DieuDuongTruongKhoa.FullNameString : "";
                    rParams["parHospitalName"].Value = Globals.ServerConfigSection.CommonItems.ReportHospitalName;
                    rParams["parDepartmentOfHealth"].Value = Globals.ServerConfigSection.CommonItems.ReportDepartmentOfHealth;
                    break;
            }
         
            //if (ReportModel != null)
            //{
            //    ReportModel.RequestDefaultParameterValues -= new WeakEventHandler<EventArgs>(_reportModel_RequestDefaultParameterValues).Handler;
            //}

            //ReportModel = new TransactionsTemplate38().PreviewModel;

            //ReportModel.RequestDefaultParameterValues += new WeakEventHandler<EventArgs>(_reportModel_RequestDefaultParameterValues).Handler;
            // ReportModel.AutoShowParametersPanel = false;
            ReportModel.CreateDocument(rParams);
        }
     
        //public void _reportModel_RequestDefaultParameterValues(object sender, System.EventArgs e)
        //{
        //    switch (_eItem)
        //    {
        //        case ReportName.TEMP38a:
        //            ReportModel = null;
        //            ReportModel = new TransactionsTemplate38().PreviewModel;
        //            rParams["TransactionID"].Value = (int)ID;
        //            rParams["PtRegistrationID"].Value = 0;
        //            break;
        //        case ReportName.TEMP38aNoiTru:
        //             ReportModel = null;
        //             ReportModel = new TransactionsTemplate38NoiTru().PreviewModel;
        //            rParams["TransactionID"].Value = (int)ID;
        //            rParams["PtRegistrationID"].Value = 0;
        //            break;
        //    }
           
        //}
    }
}
