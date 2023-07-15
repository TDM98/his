using System;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using aEMR.Infrastructure;
using System.Windows;
using eHCMSLanguage;
using System.Linq;
using Castle.Windsor;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(ICatastropheHistory)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CatastropheHistoryViewModel : Conductor<object>, ICatastropheHistory
    {
        #region Properties
        private ObservableCollection<Lookup> _CatastropheTypeCollection;
        public ObservableCollection<Lookup> CatastropheTypeCollection
        {
            get
            {
                return _CatastropheTypeCollection;
            }
            set
            {
                if (_CatastropheTypeCollection == value)
                    return;
                _CatastropheTypeCollection = value;
                NotifyOfPropertyChange(() => CatastropheTypeCollection);
            }
        }
        private Lookup _SelectedCatastropheType;
        public Lookup SelectedCatastropheType
        {
            get
            {
                return _SelectedCatastropheType;
            }
            set
            {
                if (_SelectedCatastropheType == value)
                    return;
                _SelectedCatastropheType = value;
                NotifyOfPropertyChange(() => SelectedCatastropheType);
                NotifyOfPropertyChange(() => IsSurgery);
            }
        }
        private ObservableCollection<RefMedicalServiceItem> _MedicalServiceItemCollection;
        public ObservableCollection<RefMedicalServiceItem> MedicalServiceItemCollection
        {
            get
            {
                return _MedicalServiceItemCollection;
            }
            set
            {
                if (_MedicalServiceItemCollection == value)
                    return;
                _MedicalServiceItemCollection = value;
                NotifyOfPropertyChange(() => MedicalServiceItemCollection);
                NotifyOfPropertyChange(() => IsSurgery);
            }
        }
        private RefMedicalServiceItem _SelectedMedicalServiceItem;
        public RefMedicalServiceItem SelectedMedicalServiceItem
        {
            get
            {
                return _SelectedMedicalServiceItem;
            }
            set
            {
                if (_SelectedMedicalServiceItem == value)
                    return;
                _SelectedMedicalServiceItem = value;
                NotifyOfPropertyChange(() => SelectedMedicalServiceItem);
                NotifyOfPropertyChange(() => IsSurgery);
            }
        }
        public bool IsSurgery
        {
            get
            {
                return MedicalServiceItemCollection != null && MedicalServiceItemCollection.Count > 0 && SelectedCatastropheType != null && SelectedMedicalServiceItem != null;
            }
        }
        private long _PtRegistrationID;
        public long PtRegistrationID
        {
            get
            {
                return _PtRegistrationID;
            }
            set
            {
                if (_PtRegistrationID == value)
                    return;
                _PtRegistrationID = value;
                NotifyOfPropertyChange(() => PtRegistrationID);
            }
        }
        #endregion
        #region Events
        [ImportingConstructor]
        public CatastropheHistoryViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            LoadCatastropheTypes();
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            LoadSugeryInfo();
        }
        public void LoadCatastropheTypes()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.V_CatastropheType, Globals.DispatchCallback((asyncResult) =>
                        {
                            CatastropheTypeCollection = new ObservableCollection<Lookup>(contract.EndGetAllLookupValuesByType(asyncResult));
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void LoadSugeryInfo()
        {
            if (PtRegistrationID <= 0)
                return;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllSugeriesByPtRegistrationID(PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            MedicalServiceItemCollection = new ObservableCollection<RefMedicalServiceItem>(contract.EndGetAllSugeriesByPtRegistrationID(asyncResult));
                            if (MedicalServiceItemCollection != null && MedicalServiceItemCollection.Any(x => x.V_CatastropheType > 0))
                            {
                                SelectedMedicalServiceItem = MedicalServiceItemCollection.FirstOrDefault(x => x.V_CatastropheType > 0);
                                SelectedCatastropheType = CatastropheTypeCollection.Where(x => x.LookupID == SelectedMedicalServiceItem.V_CatastropheType).FirstOrDefault();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void btnSave()
        {
            if (SelectedMedicalServiceItem.PtRegDetailID <= 0)
                return;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSaveCatastropheByPtRegDetailID(SelectedMedicalServiceItem.PtRegDetailID, SelectedCatastropheType.LookupID, Globals.DispatchCallback((asyncResult) =>
                        {
                            if (contract.EndSaveCatastropheByPtRegDetailID(asyncResult))
                                MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            else
                                MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        #endregion
    }
}
