using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;
using eHCMSLanguage;

namespace aEMR.OutstandingTasks.ViewModels
{
    public abstract class OutStandingTaskViewModelBase : Conductor<object>
    {
        public OutStandingTaskViewModelBase(AllLookupValues.QueueType queueType)
        {
            _searchCriteria = new PatientQueueSearchCriteria();
            _searchCriteria.V_QueueType = (long)queueType;

            Globals.EventAggregator.Subscribe(this);
            SearchStaffCatgs();            
        }
        
        private string _outstandingTitle;

        public string OutstandingTitle
        {
            get
            {
                return _outstandingTitle;
            }
            set
            {
                _outstandingTitle = value;
                NotifyOfPropertyChange(() => OutstandingTitle);
            }
        }

        

        private PatientQueueSearchCriteria _searchCriteria;
        public PatientQueueSearchCriteria SearchCriteria
        {
            get
            {
                return _searchCriteria;
            }
        }
            
        private int _itemsCount;

        public int ItemsCount
        {
            get
            {
                return _itemsCount;
            }
            set
            {
                _itemsCount = value;
                NotifyOfPropertyChange(()=>ItemsCount);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }

        private OutstandingItem _selectedItem;
        public virtual OutstandingItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        private ObservableCollection<OutstandingItem> _outstandingItems;
        public ObservableCollection<OutstandingItem> OutstandingItems
        {
            get
            {
                return _outstandingItems;
            }
            set
            {
                if (_outstandingItems != value)
                {
                    _outstandingItems = value;
                    NotifyOfPropertyChange(()=>OutstandingItems);
                    ItemsCount = _outstandingItems.Count;
                }
            }
        }

        private ObservableCollection<Staff> _staffs;
        public ObservableCollection<Staff> Staffs
        {
            get { return _staffs; }
            set
            {
                _staffs = value;
                NotifyOfPropertyChange(() => Staffs);
            }
        }

        public virtual void LoadOutstandingItems()
        {
            this.ShowBusyIndicator();
            //IsLoading = true;

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginPatientQueue_GetListPaging(SearchCriteria,0,10000,false,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                List<PatientQueue> allItem = null;
                                bool bOK = false;
                                int total;
                                try
                                {
                                    allItem = contract.EndPatientQueue_GetListPaging(out total,asyncResult);
                                    bOK = true;
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                    bOK = false;
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    bOK = false;
                                }
                                if(bOK)
                                {
                                    ObservableCollection<OutstandingItem> tempList = new ObservableCollection<OutstandingItem>();

                                    foreach (PatientQueue queueItem in allItem)
                                    if (queueItem != null)
                                    {
                                        tempList.Add(queueItem);
                                        //ItemsCount++;
                                    }
                                    OutstandingItems = tempList;
                                    ItemsCount = OutstandingItems.Count;
                                }
                                else
                                {
                                    OutstandingItems = null;
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void LoadStaffs()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent
                //{
                //    IsBusy = true,
                //    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0603_G1_DangLayDSNVien)
                //});
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetStaffsHaveRegistrations((byte)StaffRegistrationType.NORMAL,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Staff> allItems = new ObservableCollection<Staff>();
                                try
                                {
                                    allItems = contract.EndGetStaffsHaveRegistrations(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                Staffs = new ObservableCollection<Staff>(allItems);
                                Staff firstItem = new Staff();
                                firstItem.StaffID = -1;
                                firstItem.FullName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.A0343_G1_Msg_InfoChonNhVien);
                                Staffs.Insert(0, firstItem);
                                NotifyOfPropertyChange(()=>Staffs);
                                SearchCriteria.StaffID = -1;
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {                    
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void SearchStaffCatgs()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchStaffCat(new Staff { RefStaffCategory = new RefStaffCategory { V_StaffCatType = (long)V_StaffCatType.NhanVienQuayDangKy } }, 0, 1000, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            Staffs = contract.EndSearchStaffCat(asyncResult).ToObservableCollection();
                            Staff firstItem = new Staff();
                            firstItem.StaffID = -1;
                            firstItem.FullName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.A0343_G1_Msg_InfoChonNhVien);
                            Staffs.Insert(0, firstItem);
                            NotifyOfPropertyChange(() => Staffs);
                            SearchCriteria.StaffID = -1;
                        }
                        catch (Exception ex)
                        {
                            ClientLoggerHelper.ClientLogger(ex.ToString());
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

        public virtual void RefreshCmd()
        {
            LoadOutstandingItems();
            SelectedItem = null;
            OldSelectedIndex = -1;
            OldPtRegistrationID = 0;
        }
        private int _oldSelectedIndex = -1;
        public int OldSelectedIndex
        {
            get
            {
                return _oldSelectedIndex;
            }
            protected set
            {
                _oldSelectedIndex = value;
            }
        }

        private long _OldPtRegistrationID;
        public long OldPtRegistrationID
        {
            get
            {
                return _OldPtRegistrationID;
            }
            protected set
            {
                _OldPtRegistrationID = value;
            }
        }
        public virtual void OnSelectOutstandingItem(object sender, SelectionChangedEventArgs eventArgs)
        {
        }
    }
}
