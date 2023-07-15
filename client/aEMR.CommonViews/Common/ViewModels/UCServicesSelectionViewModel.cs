using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows.Data;

namespace aEMR.Common.ViewModels
{
    [Export(typeof(IUCServicesSelection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UCServicesSelectionViewModel : ViewModelBase, IUCServicesSelection
    {
        #region Properties
        private ObservableCollection<RefMedicalServiceItem> _ServiceItemCollection;
        public ObservableCollection<RefMedicalServiceItem> ServiceItemCollection
        {
            get
            {
                return _ServiceItemCollection;
            }
            set
            {
                if (_ServiceItemCollection == value)
                {
                    return;
                }
                _ServiceItemCollection = value;
                NotifyOfPropertyChange(() => ServiceItemCollection);
                ServiceItemView = CollectionViewSource.GetDefaultView(ServiceItemCollection);
                ServiceItemView.Filter = (x) => { return CurrentServiceTypeID == 0 || (x as RefMedicalServiceItem).MedicalServiceTypeID == CurrentServiceTypeID; };
            }
        }
        private ICollectionView _ServiceItemView;
        public ICollectionView ServiceItemView
        {
            get
            {
                return _ServiceItemView;
            }
            set
            {
                if (_ServiceItemView == value)
                {
                    return;
                }
                _ServiceItemView = value;
                NotifyOfPropertyChange(() => ServiceItemView);
            }
        }
        private ObservableCollection<RefMedicalServiceType> _ServiceTypeCollection;
        public ObservableCollection<RefMedicalServiceType> ServiceTypeCollection
        {
            get
            {
                return _ServiceTypeCollection;
            }
            set
            {
                if (_ServiceTypeCollection == value)
                {
                    return;
                }
                _ServiceTypeCollection = value;
                NotifyOfPropertyChange(() => ServiceTypeCollection);
            }
        }
        private long _CurrentServiceTypeID;
        public long CurrentServiceTypeID
        {
            get
            {
                return _CurrentServiceTypeID;
            }
            set
            {
                if (_CurrentServiceTypeID == value)
                {
                    return;
                }
                _CurrentServiceTypeID = value;
                NotifyOfPropertyChange(() => CurrentServiceTypeID);
                ServiceItemView.Refresh();
            }
        }
        public IList<RefMedicalServiceItem> SelectedServiceItemCollection { get; set; }
        public ServiceItemCollectionLoadCompleted ServiceItemCollectionLoadCompletedCallback { get; set; }
        private string _ConsultationRoomStaffAllocationServiceListTitle;
        public string ConsultationRoomStaffAllocationServiceListTitle
        {
            get
            {
                return _ConsultationRoomStaffAllocationServiceListTitle;
            }
            set
            {
                if (_ConsultationRoomStaffAllocationServiceListTitle == value)
                {
                    return;
                }
                _ConsultationRoomStaffAllocationServiceListTitle = value;
                NotifyOfPropertyChange(() => ConsultationRoomStaffAllocationServiceListTitle);
            }
        }
        #endregion
        #region #Events
        [ImportingConstructor]
        public UCServicesSelectionViewModel()
        {
            GetServiceTypeCollection();
            GetAllServiceCollection();
        }
        public void SaveButton()
        {
            if (ServiceItemCollection.Any(x => x.IsCheckedInDataList.GetValueOrDefault(false)))
            {
                SelectedServiceItemCollection = ServiceItemCollection.Where(x => x.IsCheckedInDataList.GetValueOrDefault(false)).ToList();
            }
            if (SelectedServiceItemCollection == null || SelectedServiceItemCollection.Count == 0 || string.IsNullOrEmpty(ConsultationRoomStaffAllocationServiceListTitle))
            {
                return;
            }
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var CurrentFactory = new AppointmentServiceClient())
                    {
                        var Currentontract = CurrentFactory.ServiceInstance;
                        Currentontract.BeginSaveConsultationRoomStaffAllocationServiceList(0, ConsultationRoomStaffAllocationServiceListTitle, SelectedServiceItemCollection.Select(x => x.MedServiceID).ToArray(), Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                Currentontract.EndSaveConsultationRoomStaffAllocationServiceList(asyncResult);
                                TryClose();
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        #endregion
        #region Methods
        public void GetServiceTypeCollection()
        {
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var CurrentFactory = new PatientRegistrationServiceClient())
                    {
                        var Currentontract = CurrentFactory.ServiceInstance;
                        var CurrentGettingTypes = new List<long>
                        {
                            (long) AllLookupValues.V_RefMedicalServiceInOutOthers.NGOAITRU,
                            (long) AllLookupValues.V_RefMedicalServiceInOutOthers.NOITRU_NGOAITRU,
                        };
                        Currentontract.BeginGetMedicalServiceTypesByInOutType(CurrentGettingTypes, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                ObservableCollection<RefMedicalServiceType> ItemCollection = new ObservableCollection<RefMedicalServiceType>();
                                var GettedItemCollection = Currentontract.EndGetMedicalServiceTypesByInOutType(asyncResult);
                                if (GettedItemCollection != null || GettedItemCollection.Count > 0)
                                {
                                    ItemCollection = GettedItemCollection.ToObservableCollection();
                                    if (ItemCollection.Any(x => x.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH))
                                    {
                                        ServiceTypeCollection = ItemCollection.Where(x => x.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH).ToObservableCollection();
                                        CurrentServiceTypeID = ServiceTypeCollection.First().MedicalServiceTypeID;
                                        return;
                                    }
                                }
                                ItemCollection.Insert(0, new RefMedicalServiceType { MedicalServiceTypeID = 0, MedicalServiceTypeName = eHCMSResources.K2122_G1_ChonTatCa });
                                ServiceTypeCollection = ItemCollection;
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        private void GetAllServiceCollection()
        {
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllMedicalServiceItemsByType(null, null, null, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                ObservableCollection<RefMedicalServiceItem> ItemCollection = new ObservableCollection<RefMedicalServiceItem>();
                                var GettedCollection = contract.EndGetAllMedicalServiceItemsByType(asyncResult);
                                if (GettedCollection != null && GettedCollection.Count > 0)
                                {
                                    ItemCollection = GettedCollection.ToObservableCollection();
                                }
                                ServiceItemCollection = ItemCollection;
                                if (ServiceItemCollectionLoadCompletedCallback != null)
                                {
                                    ServiceItemCollectionLoadCompletedCallback();
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        public void ApplySelectedServiceCollection(long[] MedServiceIDCollection)
        {
            foreach(var aItem in ServiceItemCollection)
            {
                if (!MedServiceIDCollection.Contains(aItem.MedServiceID))
                {
                    continue;
                }
                aItem.IsCheckedInDataList = true;
            }
            if (ServiceItemCollection.Any(x => x.IsCheckedInDataList.GetValueOrDefault(false)))
            {
                SelectedServiceItemCollection = ServiceItemCollection.Where(x => x.IsCheckedInDataList.GetValueOrDefault(false)).ToList();
            }
        }
        #endregion
    }
}