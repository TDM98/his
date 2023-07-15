using aEMR.Common.BaseModel;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IHospitalClient)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HospitalClientViewModel : ViewModelBase, IHospitalClient
    {
        #region Properties
        private ObservableCollection<HospitalClient> _HospitalClientCollection;
        public ObservableCollection<HospitalClient> HospitalClientCollection
        {
            get
            {
                return _HospitalClientCollection;
            }
            set
            {
                if (_HospitalClientCollection == value)
                {
                    return;
                }
                _HospitalClientCollection = value;
                NotifyOfPropertyChange(() => HospitalClientCollection);
            }
        }
        #endregion
        #region Events
        public HospitalClientViewModel()
        {
            GetHospitalClientsData();
        }
        public void EditHospitalClientItemCmd(HospitalClient aItem, object eventArgs)
        {
            if (aItem == null)
            {
                return;
            }
            IHospitalClientEdit mView = Globals.GetViewModel<IHospitalClientEdit>();
            mView.CurrentHospitalClient = aItem;
            GlobalsNAV.ShowDialog_V3(mView);
            if (mView.IsCompleted)
            {
                GetHospitalClientsData();
            }
        }
        #endregion
        #region Methods
        private void GetHospitalClientsData()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new AppointmentServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginGetHospitalClients(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var Collection = mContract.EndGetHospitalClients(asyncResult);
                                if (Collection == null || Collection.Count == 0)
                                {
                                    HospitalClientCollection = new ObservableCollection<HospitalClient>();
                                }
                                else
                                {
                                    HospitalClientCollection = new ObservableCollection<HospitalClient>(Collection);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
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