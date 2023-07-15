using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Windsor;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;

namespace aEMR.Appointment.ViewModels
{
    [Export(typeof(IHospitalClientEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HospitalClientEditViewModel : ViewModelBase, IHospitalClientEdit
    {
        #region Properties
        private HospitalClient _CurrentHospitalClient;
        private ObservableCollection<Lookup> _HosClientTypeCollection;
        public HospitalClient CurrentHospitalClient
        {
            get
            {
                return _CurrentHospitalClient;
            }
            set
            {
                _CurrentHospitalClient = value;
                NotifyOfPropertyChange(() => CurrentHospitalClient);
            }
        }
        public ObservableCollection<Lookup> HosClientTypeCollection
        {
            get
            {
                return _HosClientTypeCollection;
            }
            set
            {
                _HosClientTypeCollection = value;
                NotifyOfPropertyChange(() => HosClientTypeCollection);
            }
        }
        public bool IsCompleted { get; set; }
        #endregion
        #region Events
        [ImportingConstructor]
        public HospitalClientEditViewModel(IWindsorContainer aContainer, INavigationService aNavigation, IEventAggregator aEventAgr, ISalePosCaching aCaching)
        {
            HosClientTypeCollection = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_HosClientType).ToObservableCollection();
            CurrentHospitalClient = new HospitalClient { V_HosClientType = (long)AllLookupValues.V_HosClientType.Company };
        }
        public void btnSave()
        {
            if (CurrentHospitalClient == null || string.IsNullOrEmpty(CurrentHospitalClient.ClientName))
            {
                GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2784_G1_TenKhachHangKhongHopLe);
                return;
            }
            if (CurrentHospitalClient == null || string.IsNullOrEmpty(CurrentHospitalClient.CompanyName))
            {
                GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2784_G1_TenCongTyKhongHopLe);
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var mFactory = new AppointmentServiceClient())
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginEditHospitalClient(CurrentHospitalClient, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool mExCode;
                                var mUpdatedClient = mContract.EndEditHospitalClient(out mExCode, asyncResult);
                                if (mExCode)
                                {
                                    CurrentHospitalClient = mUpdatedClient;
                                    IsCompleted = true;
                                    TryClose();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.G0442_G1_TBao);
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