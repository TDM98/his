using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using DataEntities;
using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;

namespace aEMR.CommonViews.ViewModels
{
    [Export(typeof(IUCServiceListsSelection)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class UCServiceListsSelectionViewModel : ViewModelBase, IUCServiceListsSelection
    {
        #region Properties
        private ObservableCollection<ConsultationRoomStaffAllocationServiceList> _ServiceListCollection;
        public ObservableCollection<ConsultationRoomStaffAllocationServiceList> ServiceListCollection
        {
            get
            {
                return _ServiceListCollection;
            }
            set
            {
                if (_ServiceListCollection == value)
                {
                    return;
                }
                _ServiceListCollection = value;
                NotifyOfPropertyChange(() => ServiceListCollection);
            }
        }
        private ConsultationRoomStaffAllocationServiceList _CurrentServiceList;
        public ConsultationRoomStaffAllocationServiceList CurrentServiceList
        {
            get
            {
                return _CurrentServiceList;
            }
            set
            {
                if (_CurrentServiceList == value)
                {
                    return;
                }
                _CurrentServiceList = value;
                NotifyOfPropertyChange(() => CurrentServiceList);
            }
        }
        public ConsultationRoomStaffAllocationServiceList ConfirmedServiceList { get; set; }
        public bool IsConfirmed { get; set; } = false;
        public ServiceItemCollectionLoadCompleted ServiceItemCollectionLoadCompletedCallback { get; set; }
        #endregion
        #region Events
        public UCServiceListsSelectionViewModel()
        {
            LoadServiceListCollection();
        }
        public void CreateNewCmd()
        {
            IUCServicesSelection DialogView = Globals.GetViewModel<IUCServicesSelection>();
            GlobalsNAV.ShowDialog_V3(DialogView);
            LoadServiceListCollection();
            //SelectedServiceItemCollection = DialogView.SelectedServiceItemCollection;
        }
        public void SaveButton()
        {
            IsConfirmed = true;
            ConfirmedServiceList = CurrentServiceList;
            TryClose();
        }
        #endregion
        #region Methods
        private void LoadServiceListCollection()
        {
            this.DlgShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var CurrentFactory = new AppointmentServiceClient())
                    {
                        var Currentontract = CurrentFactory.ServiceInstance;
                        Currentontract.BeginGetConsultationRoomStaffAllocationServiceLists(Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var GettedItemCollection = Currentontract.EndGetConsultationRoomStaffAllocationServiceLists(asyncResult);
                                if (GettedItemCollection != null || GettedItemCollection.Count > 0)
                                {
                                    ServiceListCollection = GettedItemCollection.ToObservableCollection();
                                }
                                else
                                {
                                    ServiceListCollection = new ObservableCollection<ConsultationRoomStaffAllocationServiceList>();
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                                if (ServiceItemCollectionLoadCompletedCallback != null)
                                {
                                    ServiceItemCollectionLoadCompletedCallback();
                                }
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
        public void ApplySelectedServiceCollection(long ConsultationRoomStaffAllocationServiceListID)
        {
            if (ServiceListCollection == null)
            {
                return;
            }
            CurrentServiceList = ServiceListCollection.FirstOrDefault(x => x.ConsultationRoomStaffAllocationServiceListID == ConsultationRoomStaffAllocationServiceListID);
        }
        #endregion
    }
}