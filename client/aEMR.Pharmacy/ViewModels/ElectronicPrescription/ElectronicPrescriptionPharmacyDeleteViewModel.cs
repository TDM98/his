using System.ComponentModel.Composition;
using Caliburn.Micro;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using System;
using System.Windows;
using eHCMSLanguage;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
/*
* 20181114 #001 QTD: Create ViewModel
*/
namespace aEMR.Pharmacy.ViewModels.ElectronicPrescription
{
    [Export(typeof(IElectronicPrescriptionPharmacyDelete)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ElectronicPrescriptionPharmacyDeleteViewModel : Conductor<object>, IElectronicPrescriptionPharmacyDelete
    {
        private int _MaxWidth;
        public int MaxWidth
        {
            get { return _MaxWidth; }
            set
            {
                _MaxWidth = value;
                NotifyOfPropertyChange(() => MaxWidth);
            }
        }

        public string title
        {
            get
            {
                return "Huỷ xác nhận đẩy cổng đơn thuốc";
            }
        }

        private long _PtRegistrationID;
        public long PtRegistrationID
        {
            get { return _PtRegistrationID; }
            set
            {
                if(_PtRegistrationID != value)
                {
                    _PtRegistrationID = value;
                    NotifyOfPropertyChange(() => PtRegistrationID);
                }
            }
        }

        private long _DQGReportID;
        public long DQGReportID
        {
            get { return _DQGReportID; }
            set
            {
                if (_DQGReportID != value)
                {
                    _DQGReportID = value;
                    NotifyOfPropertyChange(() => DQGReportID);
                }
            }
        }

        private long _DTDTReportID;
        public long DTDTReportID
        {
            get { return _DTDTReportID; }
            set
            {
                if (_DTDTReportID != value)
                {
                    _DTDTReportID = value;
                    NotifyOfPropertyChange(() => DTDTReportID);
                }
            }
        }

        private string _CancelReason;
        public string CancelReason
        {
            get { return _CancelReason; }
            set
            {
                if (_CancelReason != value)
                {
                    _CancelReason = value;
                    NotifyOfPropertyChange(() => CancelReason);
                }
            }
        }

        private long _V_RegistrationType;
        public long V_RegistrationType
        {
            get { return _V_RegistrationType; }
            set
            {
                if (_V_RegistrationType != value)
                {
                    _V_RegistrationType = value;
                    NotifyOfPropertyChange(() => V_RegistrationType);
                }
            }
        }

        private long _IssueID;
        public long IssueID
        {
            get { return _IssueID; }
            set
            {
                if (_IssueID != value)
                {
                    _IssueID = value;
                    NotifyOfPropertyChange(() => IssueID);
                }
            }
        }

        private ObservableCollection<RefShelves> _AllRefShelves = new ObservableCollection<RefShelves>();
        public ObservableCollection<RefShelves> AllRefShelves
        {
            get
            {
                return _AllRefShelves;
            }
            set
            {
                if (_AllRefShelves != value)
                {
                    _AllRefShelves = value;
                    NotifyOfPropertyChange(() => AllRefShelves);
                }
            }
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ElectronicPrescriptionPharmacyDeleteViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
        }

        public void btnCancel()
        {
            initData();
            TryClose();
            Globals.EventAggregator.Unsubscribe(this);
        }
        private void initData()
        {
            CancelReason = "";
        }

        public void btnDeletePrescription()
        {
            if (PtRegistrationID == 0 || DQGReportID == 0 || IssueID == 0)
            {
                return;
            }

            if (CancelReason == null || CancelReason == string.Empty)
            {
                MessageBox.Show(eHCMSResources.Z2680_G1_ChuaNhapLyDoHuy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new TransactionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteDQGReportOutInpt(PtRegistrationID, IssueID, DQGReportID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault()
                    , CancelReason, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndDeleteDQGReportOutInpt(asyncResult))
                            {
                                Globals.EventAggregator.Publish(new ElectronicPrescriptionPharmacyDeleteEvent() { DeleteElectronicPrescriptionPharmacy = true });
                                MessageBox.Show(eHCMSResources.Z2414_G1_HuyXNThanhCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                btnCancel();
                            }
                            else
                            {
                                MessageBox.Show("Hủy xác nhận không thành công", eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
    }
}
